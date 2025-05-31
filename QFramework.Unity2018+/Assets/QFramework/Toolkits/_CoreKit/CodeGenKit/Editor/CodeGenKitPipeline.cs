/****************************************************************************
 * Copyright (c) 2015 ~ 2025 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace QFramework
{
    public class CodeGenKitPipeline : ScriptableObject
    {
        private static CodeGenKitPipeline mInstance;

        public static CodeGenKitPipeline Default
        {
            get
            {
                if (mInstance) return mInstance;

                var filePath = Dir.Value + FileName;

                if (File.Exists(filePath))
                {
                    return mInstance = AssetDatabase.LoadAssetAtPath<CodeGenKitPipeline>(filePath);
                }

                return mInstance = CreateInstance<CodeGenKitPipeline>();
            }
        }

        public void Save()
        {
            var filePath = Dir.Value + FileName;

            if (!File.Exists(filePath))
            {
                AssetDatabase.CreateAsset(this, Dir.Value + FileName);
            }

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static readonly Lazy<string> Dir =
            new Lazy<string>(() => "Assets/QFrameworkData/CodeGenKit/".CreateDirIfNotExists());

        private const string FileName = "Pipeline.asset";

        [SerializeField] public CodeGenTask CurrentTask;

        public void Generate(CodeGenTask task)
        {
            CurrentTask = task;
            CurrentTask.Status = CodeGenTaskStatus.Search;
            BindSearchHelper.Search(task);
            CurrentTask.Status = CodeGenTaskStatus.Gen;
            ViewControllerCodeTemplate.Generate(CurrentTask);
            ViewControllerDesignerCodeTemplate.Generate(CurrentTask);

            Save();

            CurrentTask.Status = CodeGenTaskStatus.Compile;
        }

        private void OnCompile()
        {
            if (CurrentTask == null) return;
            if (CurrentTask.Status == CodeGenTaskStatus.Compile)
            {
                var generateClassName = CurrentTask.ClassName;
                var generateNamespace = CurrentTask.Namespace;

                var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(assembly =>
                    !assembly.FullName.StartsWith("Unity"));

                var typeName = generateNamespace + "." + generateClassName;

                var type = assemblies.Where(a => a.GetType(typeName) != null)
                    .Select(a => a.GetType(typeName)).FirstOrDefault();

                if (type == null)
                {
                    Debug.Log("编译失败");
                    return;
                }

                Debug.Log(type);

                var gameObject = CurrentTask.GameObject;

                var scriptComponent = gameObject.GetComponent(type);

                if (!scriptComponent)
                {
                    scriptComponent = gameObject.AddComponent(type);
                }

                var serializedObject = new SerializedObject(scriptComponent);

                foreach (var bindInfo in CurrentTask.BindInfos)
                {
                    var serializedProperty = serializedObject.FindProperty(bindInfo.MemberName);
                    serializedProperty.objectReferenceValue = gameObject.transform.Find(bindInfo.PathToRoot).gameObject;
                }

                var referenceBinds = gameObject.GetComponent<OtherBinds>();
                if (referenceBinds)
                {
                    foreach (var bind in referenceBinds.Binds)
                    {
                        var serializedProperty = serializedObject.FindProperty(bind.MemberName);
                        serializedProperty.objectReferenceValue = bind.Object;
                    }
                }

                var codeGenerateInfo = gameObject.GetComponent<ViewController>();

                if (codeGenerateInfo)
                {
                    serializedObject.FindProperty("ScriptsFolder").stringValue = codeGenerateInfo.ScriptsFolder;
                    serializedObject.FindProperty("PrefabFolder").stringValue = codeGenerateInfo.PrefabFolder;
                    serializedObject.FindProperty("GeneratePrefab").boolValue = codeGenerateInfo.GeneratePrefab;
                    serializedObject.FindProperty("ScriptName").stringValue = codeGenerateInfo.ScriptName;
                    serializedObject.FindProperty("Namespace").stringValue = codeGenerateInfo.Namespace;
                    serializedObject.FindProperty("ArchitectureFullTypeName").stringValue =
                        codeGenerateInfo.ArchitectureFullTypeName;

                    var generatePrefab = codeGenerateInfo.GeneratePrefab;
                    var prefabFolder = codeGenerateInfo.PrefabFolder;

                    if (codeGenerateInfo.GetType() != type)
                    {
                        DestroyImmediate(codeGenerateInfo, true);
                    }

                    serializedObject.ApplyModifiedProperties();
                    serializedObject.UpdateIfRequiredOrScript();

                    if (generatePrefab)
                    {
                        prefabFolder.CreateDirIfNotExists();

                        var generatePrefabPath = prefabFolder + "/" + gameObject.name + ".prefab";

                        if (File.Exists(generatePrefabPath))
                        {
                            // PrefabUtility.SavePrefabAsset(gameObject);
                        }
                        else
                        {
                            PrefabUtils.SaveAndConnect(generatePrefabPath, gameObject);
                        }
                    }
                }
                else
                {
                    serializedObject.FindProperty("ScriptsFolder").stringValue = "Assets/Scripts";
                    serializedObject.ApplyModifiedProperties();
                    serializedObject.UpdateIfRequiredOrScript();
                }

                EditorUtility.SetDirty(gameObject);

                CurrentTask.Status = CodeGenTaskStatus.Complete;
                CurrentTask = null;
            }
        }

        [DidReloadScripts]
        static void Compile()
        {
            Default.OnCompile();
        }
    }
}
#endif