using System;
using System.Linq;

using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace QFramework
{
    public class CreateViewControllerCode : EditorWindow
    {
        [MenuItem("GameObject/@(Alt+V)QF-UI Kit-Add View Controller &v", false, 0)]
        static void AddView()
        {
            var gameObject = Selection.objects.First() as GameObject;

            if (!gameObject)
            {
                Debug.LogWarning("需要选择 GameObject");
                return;
            }

            var view = gameObject.GetComponent<ViewController>();

            if (!view)
            {
                gameObject.AddComponent<ViewController>();
            }
        }

        [MenuItem("GameObject/@(Alt+C)QF-UI Kit-Create Code &c", false, 0)]
        static void CreateCode()
        {
            var gameObject = Selection.objects.First() as GameObject;
            DoCreateCodeFromScene(gameObject);
        }


        public static void DoCreateCodeFromScene(GameObject gameObject)
        {
            if (!gameObject)
            {
                Debug.LogWarning("需要选择 GameObject");
                return;
            }

            if (gameObject.GetComponent<Bind>() && !gameObject.GetComponent<ViewController>())
            {
                var parentController = gameObject.GetComponentInParent<ViewController>();

                if (parentController)
                {
                    gameObject = parentController.gameObject;
                }
            }

            Debug.Log("Create Code");

            var generateInfo = gameObject.GetComponent<ViewController>();

            var scriptsFolder = Application.dataPath + "/Scripts";

            if (generateInfo)
            {
                scriptsFolder = generateInfo.ScriptsFolder;
            }

            scriptsFolder.CreateDirIfNotExists();


            var panelCodeInfo = new PanelCodeInfo {GameObjectName = generateInfo.name};


            // 搜索所有绑定
            BindCollector.SearchBinds(gameObject.transform, "", panelCodeInfo);

            var uikitSettingData = UIKitSettingData.Load();

            ViewControllerTemplate.Write(generateInfo.ScriptName, scriptsFolder, uikitSettingData);
            ViewControllerDesignerTemplate.Write(generateInfo.ScriptName, scriptsFolder, panelCodeInfo,
                uikitSettingData);

            EditorPrefs.SetString("GENERATE_CLASS_NAME", generateInfo.ScriptName);
            EditorPrefs.SetString("GAME_OBJECT_NAME", gameObject.name);
            AssetDatabase.Refresh();
        }


        [DidReloadScripts]
        static void AddComponent2GameObject()
        {
            var generateClassName = EditorPrefs.GetString("GENERATE_CLASS_NAME");
            var gameObjectName = EditorPrefs.GetString("GAME_OBJECT_NAME");

            if (string.IsNullOrEmpty(generateClassName))
            {
                EditorPrefs.DeleteKey("GENERATE_CLASS_NAME");
                EditorPrefs.DeleteKey("GAME_OBJECT_NAME");
            }
            else
            {

                var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                var defaultAssembly = assemblies.First(assembly => assembly.GetName().Name == "Assembly-CSharp");

                var typeName = UIKitSettingData.Load().Namespace + "." + generateClassName;

                var type = defaultAssembly.GetType(typeName);

                if (type == null)
                {
                    Debug.Log("编译失败");
                    return;
                }

                Debug.Log(type);

                var gameObject = GameObject.Find(gameObjectName);

                if (!gameObject)
                {
                    Debug.Log("上次的 View Controller 生成失败,找不到 GameObject:{0}".FillFormat(gameObjectName));

                    Clear();
                    return;
                }
                

                var scriptComponent = gameObject.GetComponent(type);

                if (!scriptComponent)
                {
                    scriptComponent = gameObject.AddComponent(type);
                }

                var serialiedScript = new SerializedObject(scriptComponent);

                var panelCodeInfo = new PanelCodeInfo();

                panelCodeInfo.GameObjectName = gameObjectName;

                // 搜索所有绑定
                BindCollector.SearchBinds(gameObject.transform, "", panelCodeInfo);

                foreach (var bindInfo in panelCodeInfo.BindInfos)
                {
                    var name = bindInfo.Name;

                    var componentName = bindInfo.BindScript.ComponentName.Split('.').Last();
                    
                    serialiedScript.FindProperty(name).objectReferenceValue =
                        gameObject.transform.Find(bindInfo.PathToElement)
                            .GetComponent(componentName);
                }


                var codeGenerateInfo = gameObject.GetComponent<ViewController>();

                if (codeGenerateInfo)
                {
                    serialiedScript.FindProperty("ScriptsFolder").stringValue = codeGenerateInfo.ScriptsFolder;
                    serialiedScript.FindProperty("PrefabFolder").stringValue = codeGenerateInfo.PrefabFolder;
                    serialiedScript.FindProperty("GeneratePrefab").boolValue = codeGenerateInfo.GeneratePrefab;
                    serialiedScript.FindProperty("ScriptName").stringValue = codeGenerateInfo.ScriptName;

                    var generatePrefab = codeGenerateInfo.GeneratePrefab;
                    var prefabFolder = codeGenerateInfo.PrefabFolder;

                    var fullPrefabFolder = prefabFolder.Replace("Assets", Application.dataPath);

                    if (codeGenerateInfo.GetType() != type)
                    {
                        DestroyImmediate(codeGenerateInfo, false);
                    }

                    serialiedScript.ApplyModifiedPropertiesWithoutUndo();

                    if (generatePrefab)
                    {
                        fullPrefabFolder.CreateDirIfNotExists();

                        var genereateFolder = prefabFolder + "/" + gameObject.name + ".prefab";
                        
                        PrefabUtils.SaveAndConnect(genereateFolder,gameObject);
                    }
                }
                else
                {
                    serialiedScript.FindProperty("ScriptsFolder").stringValue = "Assets/Scripts";
                    serialiedScript.ApplyModifiedPropertiesWithoutUndo();
                }

                Clear();

                EditorUtils.MarkCurrentSceneDirty();
            }
        }

        static void Clear()
        {
            EditorPrefs.DeleteKey("GENERATE_CLASS_NAME");
            EditorPrefs.DeleteKey("GAME_OBJECT_NAME");
        }
    }
}