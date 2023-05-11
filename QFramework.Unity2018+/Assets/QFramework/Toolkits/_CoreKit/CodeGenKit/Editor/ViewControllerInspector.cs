/****************************************************************************
 * Copyright (c) 2017 xiaojun
 * Copyright (c) 2015 ~ 2022 liangxiegame UNDER MIT LICENSE
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace QFramework
{
    [CustomEditor(typeof(ViewController), true)]
    public class ViewControllerInspector : Editor
    {
        [MenuItem("GameObject/QFramework/CodeGenKit/@(Alt+V)Add View Controller &v", false, 0)]
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

        [MenuItem("GameObject/QFramework/CodeGenKit/@(Alt+B)Add Bind &b", false, 1)]
        public static void AddBind()
        {
            foreach (var o in Selection.objects.OfType<GameObject>())
            {
                if (o)
                {
                    var uiMark = o.GetComponent<Bind>();

                    if (!uiMark)
                    {
                        o.AddComponent<Bind>();
                    }

                    EditorUtility.SetDirty(o);
                    EditorSceneManager.MarkSceneDirty(o.scene);
                }
            }
        }

        [MenuItem("GameObject/QFramework/CodeGenKit/@(Alt+C)Create Code &c", false, 2)]
        static void CreateCode()
        {
            var gameObject = Selection.objects.First() as GameObject;
            CodeGenKit.Generate(gameObject.GetComponent<IBindGroup>());
        }

        private ViewControllerInspectorLocale mLocaleText = new ViewControllerInspectorLocale();


        public ViewController ViewController => target as ViewController;


        private void OnEnable()
        {
            if (string.IsNullOrEmpty(ViewController.ScriptsFolder))
            {
                var setting = CodeGenKitSetting.Load();
                ViewController.ScriptsFolder = setting.ScriptDir;
            }

            if (string.IsNullOrEmpty(ViewController.PrefabFolder))
            {
                var setting = CodeGenKitSetting.Load();
                ViewController.PrefabFolder = setting.PrefabDir;
            }

            if (string.IsNullOrEmpty(ViewController.ScriptName))
            {
                ViewController.ScriptName = ViewController.name;
            }

            if (string.IsNullOrEmpty(ViewController.Namespace))
            {
                var setting = CodeGenKitSetting.Load();
                ViewController.Namespace = setting.Namespace;
            }
        }


        private readonly ViewControllerInspectorStyle mStyle = new ViewControllerInspectorStyle();

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.BeginVertical("box");

            GUILayout.Label(mLocaleText.CodegenPart, mStyle.BigTitleStyle.Value);

            LocaleKitEditor.DrawSwitchToggle(GUI.skin.label.normal.textColor);

            GUILayout.BeginHorizontal();
            GUILayout.Label(mLocaleText.Namespace, GUILayout.Width(150));
            ViewController.Namespace = EditorGUILayout.TextArea(ViewController.Namespace);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(mLocaleText.ScriptName, GUILayout.Width(150));
            ViewController.ScriptName = EditorGUILayout.TextArea(ViewController.ScriptName);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(mLocaleText.ScriptsFolder, GUILayout.Width(150));
            ViewController.ScriptsFolder =
                EditorGUILayout.TextArea(ViewController.ScriptsFolder, GUILayout.Height(30));

            GUILayout.EndHorizontal();


            EditorGUILayout.Space();
            EditorGUILayout.LabelField(mLocaleText.DragDescription);
            var sfxPathRect = EditorGUILayout.GetControlRect();
            sfxPathRect.height = 50;
            GUI.Box(sfxPathRect, string.Empty);
            EditorGUILayout.LabelField(string.Empty, GUILayout.Height(35));
            if (
                (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform) 
                && sfxPathRect.Contains(Event.current.mousePosition)
            )
            {
                //改变鼠标的外表  
                DragAndDrop.visualMode = DragAndDropVisualMode.Generic;

                if (Event.current.type == EventType.DragPerform)
                {
                    if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
                    {
                        if (DragAndDrop.paths[0] != "")
                        {
                            var newPath = DragAndDrop.paths[0];
                            ViewController.ScriptsFolder = newPath;
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                        }
                    }
                }

                Event.current.Use();

            }


            GUILayout.BeginHorizontal();
            ViewController.GeneratePrefab =
                GUILayout.Toggle(ViewController.GeneratePrefab, mLocaleText.GeneratePrefab);
            GUILayout.EndHorizontal();

            if (ViewController.GeneratePrefab)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(mLocaleText.PrefabGenerateFolder, GUILayout.Width(150));
                ViewController.PrefabFolder =
                    GUILayout.TextArea(ViewController.PrefabFolder, GUILayout.Height(30));
                GUILayout.EndHorizontal();

                EditorGUILayout.Space();
                EditorGUILayout.LabelField(mLocaleText.DragDescription);

                var dragRect = EditorGUILayout.GetControlRect();
                dragRect.height = 100;
                GUI.Box(dragRect, string.Empty);
                EditorGUILayout.LabelField(string.Empty, GUILayout.Height(85));
                if (
                    Event.current.type == EventType.DragUpdated
                    && dragRect.Contains(Event.current.mousePosition)
                )
                {
                    //改变鼠标的外表  
                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                    if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
                    {
                        if (DragAndDrop.paths[0] != "")
                        {
                            var newPath = DragAndDrop.paths[0];
                            ViewController.PrefabFolder = newPath;
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                        }
                    }
                }
            }


            if (!ViewController.GetComponent<OtherBinds>())
            {
                if (GUILayout.Button(mLocaleText.AddOtherBinds, GUILayout.Height(30)))
                {
                    ViewController.gameObject.AddComponent<OtherBinds>();
                    EditorUtility.SetDirty(ViewController.gameObject);
                    EditorSceneManager.MarkSceneDirty(ViewController.gameObject.scene);
                }
            }

            var fileFullPath = ViewController.ScriptsFolder + "/" + ViewController.ScriptName + ".cs";
            if (File.Exists(ViewController.ScriptsFolder + "/" + ViewController.ScriptName + ".cs"))
            {
                var scriptObject = AssetDatabase.LoadAssetAtPath<MonoScript>(fileFullPath);
                if (GUILayout.Button(mLocaleText.OpenScript, GUILayout.Height(30)))
                {
                    AssetDatabase.OpenAsset(scriptObject);
                }

                if (GUILayout.Button(mLocaleText.SelectScript, GUILayout.Height(30)))
                {
                    Selection.activeObject = scriptObject;
                }
            }


            if (GUILayout.Button(mLocaleText.Generate, GUILayout.Height(30)))
            {
                CodeGenKit.Generate(ViewController);
                GUIUtility.ExitGUI();
            }

            GUILayout.EndVertical();
        }
    }
}
#endif