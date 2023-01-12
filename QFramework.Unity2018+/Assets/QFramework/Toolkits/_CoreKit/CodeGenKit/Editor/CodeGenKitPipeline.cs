/****************************************************************************
 * Copyright (c) 2015 ~ 2022 liangxiegame UNDER MIT LICENSE
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

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


            // var writer = File.CreateText(scriptFile);

            var tabChar = task.IsUseNamespace ? "\t" : "";

            var writer = new StringBuilder();
            writer.AppendLine("using UnityEngine;");
            writer.AppendLine("using QFramework;");
            writer.AppendLine();

            if (tabChar != "")
            {
                if (CodeGenKit.Setting.IsDefaultNamespace)
                {
                    writer.AppendLine("// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间");
                    writer.AppendLine("// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改");
                }

                writer.AppendLine(
                    $"namespace {((string.IsNullOrWhiteSpace(task.Namespace)) ? CodeGenKit.Setting.Namespace : task.Namespace)}");
                writer.AppendLine("{");
            }

            
            writer.AppendFormat(tabChar + "public partial class {0} : ViewController", task.ClassName).AppendLine();
            writer.AppendLine(tabChar + "{");
            writer.AppendLine(tabChar + "\tvoid Start()");
            writer.AppendLine(tabChar + "\t{");
            writer.AppendLine(tabChar + "\t\t// Code Here");
            writer.AppendLine(tabChar + "\t}");
            writer.AppendLine(tabChar + "}");

            if (tabChar != "")
            {
                writer.AppendLine("}");
            }

            task.MainCode = writer.ToString();
            writer.Clear();

            writer.AppendLine($"// Generate Id:{Guid.NewGuid().ToString()}");
            writer.AppendLine("using UnityEngine;");
            writer.AppendLine();

            if (tabChar != "")
            {
                if (CodeGenKit.Setting.IsDefaultNamespace)
                {
                    writer.AppendLine("// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间");
                    writer.AppendLine("// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改");
                }
                writer.AppendLine($"namespace {(string.IsNullOrWhiteSpace(task.Namespace) ? CodeGenKit.Setting.Namespace : task.Namespace)}");
                writer.AppendLine("{");
            }

            writer.AppendFormat(tabChar + "public partial class {0}", task.ClassName).AppendLine();
            writer.AppendLine(tabChar + "{");

            foreach (var bindData in task.BindInfos)
            {
                writer.AppendLine();
                if (bindData.BindScript.Comment.IsNotNullAndEmpty())
                {
                    writer.AppendLine(tabChar + "\t/// <summary>");
                    foreach (var comment in bindData.BindScript.Comment.Split('\n'))
                    {
                        writer.AppendFormat(tabChar + "\t/// {0}", comment).AppendLine();
                    }

                    writer.AppendLine(tabChar + "\t/// </summary>");
                }

                writer.AppendFormat(tabChar + "\tpublic {0} {1};", bindData.TypeName, bindData.MemberName).AppendLine();
            }

            writer.AppendLine();
            writer.AppendLine(tabChar + "}");

            if (tabChar != "")
            {
                writer.AppendLine("}");
            }

            task.DesignerCode = writer.ToString();
            writer.Clear();


            var scriptFile = string.Format(task.ScriptsFolder + "/{0}.cs", (task.ClassName));

            if (!File.Exists(scriptFile))
            {
                scriptFile.GetFolderPath().CreateDirIfNotExists();
                File.WriteAllText(scriptFile, CurrentTask.MainCode);
            }


            scriptFile = string.Format(task.ScriptsFolder + "/{0}.Designer.cs", task.ClassName);
            File.WriteAllText(scriptFile, CurrentTask.DesignerCode);

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
                
                var typeName =  generateNamespace + "." + generateClassName;

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

                    var componentName = bindInfo.TypeName.Split('.').Last();
                    var serializedProperty = serializedObject.FindProperty(bindInfo.MemberName);
                    var component = gameObject.transform.Find(bindInfo.PathToRoot).GetComponent(componentName);

                    if (!component)
                    {
                        component = gameObject.transform.Find(bindInfo.PathToRoot).GetComponent(bindInfo.TypeName);
                    }

                    serializedProperty.objectReferenceValue = component;

                    // Debug.Log(componentName + "@@@@" + serializedProperty + "@@@@" + component);
                }


                var codeGenerateInfo = gameObject.GetComponent<ViewController>();

                if (codeGenerateInfo)
                {
                    serializedObject.FindProperty("ScriptsFolder").stringValue = codeGenerateInfo.ScriptsFolder;
                    serializedObject.FindProperty("PrefabFolder").stringValue = codeGenerateInfo.PrefabFolder;
                    serializedObject.FindProperty("GeneratePrefab").boolValue = codeGenerateInfo.GeneratePrefab;
                    serializedObject.FindProperty("ScriptName").stringValue = codeGenerateInfo.ScriptName;
                    serializedObject.FindProperty("Namespace").stringValue = codeGenerateInfo.Namespace;
                    serializedObject.FindProperty("IsUseNamespace").boolValue = codeGenerateInfo.IsUseNamespace;

                    var generatePrefab = codeGenerateInfo.GeneratePrefab;
                    var prefabFolder = codeGenerateInfo.PrefabFolder;


                    if (codeGenerateInfo.GetType() != type)
                    {
                        DestroyImmediate(codeGenerateInfo, false);
                    }

                    serializedObject.ApplyModifiedPropertiesWithoutUndo();

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
                    serializedObject.ApplyModifiedPropertiesWithoutUndo();
                }

                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());


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