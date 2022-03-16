using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

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

            if (gameObject.GetComponent<AbstractBind>() && !gameObject.GetComponent<ViewController>())
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

            ViewControllerTemplate.Write(generateInfo.ScriptName, scriptsFolder, generateInfo.Namespace,
                uikitSettingData);
            ViewControllerDesignerTemplate.Write(generateInfo.ScriptName, scriptsFolder, generateInfo.Namespace,
                panelCodeInfo,
                uikitSettingData);

            EditorPrefs.SetString("GENERATE_CLASS_NAME", generateInfo.ScriptName);
            EditorPrefs.SetString("GENERATE_NAMESPACE", !string.IsNullOrWhiteSpace(generateInfo.Namespace) ? uikitSettingData.Namespace : generateInfo.Namespace);
            EditorPrefs.SetString("GAME_OBJECT_NAME", gameObject.name);
            AssetDatabase.Refresh();
        }


        [DidReloadScripts]
        static void AddComponent2GameObject()
        {
            var generateClassName = EditorPrefs.GetString("GENERATE_CLASS_NAME");
            var gameObjectName = EditorPrefs.GetString("GAME_OBJECT_NAME");
            var generateNamespace = EditorPrefs.GetString("GENERATE_NAMESPACE");

            if (string.IsNullOrEmpty(generateClassName))
            {
                EditorPrefs.DeleteKey("GENERATE_CLASS_NAME");
                EditorPrefs.DeleteKey("GAME_OBJECT_NAME");
            }
            else
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                var defaultAssembly = assemblies.First(assembly => assembly.GetName().Name == "Assembly-CSharp");

                var typeName = generateNamespace + "." + generateClassName;

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
                    serialiedScript.FindProperty("Namespace").stringValue = codeGenerateInfo.Namespace;

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

                        PrefabUtils.SaveAndConnect(genereateFolder, gameObject);
                    }
                }
                else
                {
                    serialiedScript.FindProperty("ScriptsFolder").stringValue = "Assets/Scripts";
                    serialiedScript.ApplyModifiedPropertiesWithoutUndo();
                }

                Clear();

                MarkCurrentSceneDirty();
            }
        }

        static void Clear()
        {
            EditorPrefs.DeleteKey("GENERATE_CLASS_NAME");
            EditorPrefs.DeleteKey("GAME_OBJECT_NAME");
        }
        
        public static void MarkCurrentSceneDirty()
        {
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
    }
    
       /// <summary>
    /// 对 System.IO 的一些扩展
    /// </summary>
    internal static class IOExtension
    {
        /// <summary>
        /// 检测路径是否存在，如果不存在则创建
        /// </summary>
        /// <param name="path"></param>
        public static string CreateDirIfNotExists4FilePath(this string path)
        {
            var direct = Path.GetDirectoryName(path);

            if (!Directory.Exists(direct))
            {
                Directory.CreateDirectory(direct);
            }

            return path;
        }


        /// <summary>
        /// 创建新的文件夹,如果存在则不创建
        /// <code>
        /// var testDir = "Assets/TestFolder";
        /// testDir.CreateDirIfNotExists();
        /// // 结果为，在 Assets 目录下创建 TestFolder
        /// </code>
        /// </summary>
        public static string CreateDirIfNotExists(this string dirFullPath)
        {
            if (!Directory.Exists(dirFullPath))
            {
                Directory.CreateDirectory(dirFullPath);
            }

            return dirFullPath;
        }

        /// <summary>
        /// 删除文件夹，如果存在
        /// <code>
        /// var testDir = "Assets/TestFolder";
        /// testDir.DeleteDirIfExists();
        /// // 结果为，在 Assets 目录下删除了 TestFolder
        /// </code>
        /// </summary>
        public static void DeleteDirIfExists(this string dirFullPath)
        {
            if (Directory.Exists(dirFullPath))
            {
                Directory.Delete(dirFullPath, true);
            }
        }

        /// <summary>
        /// 清空 Dir（保留目录),如果存在。
        /// <code>
        /// var testDir = "Assets/TestFolder";
        /// testDir.EmptyDirIfExists();
        /// // 结果为，清空了 TestFolder 里的内容
        /// </code>
        /// </summary>
        public static void EmptyDirIfExists(this string dirFullPath)
        {
            if (Directory.Exists(dirFullPath))
            {
                Directory.Delete(dirFullPath, true);
            }

            Directory.CreateDirectory(dirFullPath);
        }

        /// <summary>
        /// 删除文件 如果存在
        /// <code>
        /// // 示例
        /// var filePath = "Assets/Test.txt";
        /// File.Create("Assets/Test);
        /// filePath.DeleteFileIfExists();
        /// // 结果为，删除了 Test.txt
        /// </code>
        /// </summary>
        /// <param name="fileFullPath"></param>
        /// <returns> 是否进行了删除操作 </returns>
        public static bool DeleteFileIfExists(this string fileFullPath)
        {
            if (File.Exists(fileFullPath))
            {
                File.Delete(fileFullPath);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 合并路径
        /// <code>
        /// // 示例：
        /// Application.dataPath.CombinePath("Resources").LogInfo();  // /projectPath/Assets/Resources
        /// </code>
        /// </summary>
        /// <param name="selfPath"></param>
        /// <param name="toCombinePath"></param>
        /// <returns> 合并后的路径 </returns>
        public static string CombinePath(this string selfPath, string toCombinePath)
        {
            return Path.Combine(selfPath, toCombinePath);
        }

        #region 未经过测试

        /// <summary>
        /// 打开文件夹
        /// </summary>
        /// <param name="path"></param>
        public static void OpenFolder(string path)
        {
#if UNITY_STANDALONE_OSX
            System.Diagnostics.Process.Start("open", path);
#elif UNITY_STANDALONE_WIN
            System.Diagnostics.Process.Start("explorer.exe", path);
#endif
        }


        /// <summary>
        /// 获取文件夹名
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetDirectoryName(string fileName)
        {
            fileName = MakePathStandard(fileName);
            return fileName.Substring(0, fileName.LastIndexOf('/'));
        }

        /// <summary>
        /// 获取文件名
        /// </summary>
        /// <param name="path"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string GetFileName(string path, char separator = '/')
        {
            path = IOExtension.MakePathStandard(path);
            return path.Substring(path.LastIndexOf(separator) + 1);
        }

        /// <summary>
        /// 获取不带后缀的文件名
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string GetFileNameWithoutExtention(string fileName, char separator = '/')
        {
            return GetFilePathWithoutExtention(GetFileName(fileName, separator));
        }

        /// <summary>
        /// 获取不带后缀的文件路径
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetFilePathWithoutExtention(string fileName)
        {
            if (fileName.Contains("."))
                return fileName.Substring(0, fileName.LastIndexOf('.'));
            return fileName;
        }

        /// <summary>
        /// 使目录存在,Path可以是目录名必须是文件名
        /// </summary>
        /// <param name="path"></param>
        public static void MakeFileDirectoryExist(string path)
        {
            string root = Path.GetDirectoryName(path);
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }
        }

        /// <summary>
        /// 使目录存在
        /// </summary>
        /// <param name="path"></param>
        public static void MakeDirectoryExist(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// 获取父文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetPathParentFolder(this string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            return Path.GetDirectoryName(path);
        }


        /// <summary>
        /// 使路径标准化，去除空格并将所有'\'转换为'/'
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string MakePathStandard(string path)
        {
            return path.Trim().Replace("\\", "/");
        }

        public static List<string> GetDirSubFilePathList(this string dirABSPath, bool isRecursive = true,
            string suffix = "")
        {
            var pathList = new List<string>();
            var di = new DirectoryInfo(dirABSPath);

            if (!di.Exists)
            {
                return pathList;
            }

            var files = di.GetFiles();
            foreach (var fi in files)
            {
                if (!string.IsNullOrEmpty(suffix))
                {
                    if (!fi.FullName.EndsWith(suffix, System.StringComparison.CurrentCultureIgnoreCase))
                    {
                        continue;
                    }
                }

                pathList.Add(fi.FullName);
            }

            if (isRecursive)
            {
                var dirs = di.GetDirectories();
                foreach (var d in dirs)
                {
                    pathList.AddRange(GetDirSubFilePathList(d.FullName, isRecursive, suffix));
                }
            }

            return pathList;
        }

        public static List<string> GetDirSubDirNameList(this string dirABSPath)
        {
            var di = new DirectoryInfo(dirABSPath);

            var dirs = di.GetDirectories();

            return dirs.Select(d => d.Name).ToList();
        }

        public static string GetFileName(this string absOrAssetsPath)
        {
            var name = absOrAssetsPath.Replace("\\", "/");
            var lastIndex = name.LastIndexOf("/");

            return lastIndex >= 0 ? name.Substring(lastIndex + 1) : name;
        }

        public static string GetFileNameWithoutExtend(this string absOrAssetsPath)
        {
            var fileName = GetFileName(absOrAssetsPath);
            var lastIndex = fileName.LastIndexOf(".");

            return lastIndex >= 0 ? fileName.Substring(0, lastIndex) : fileName;
        }

        public static string GetFileExtendName(this string absOrAssetsPath)
        {
            var lastIndex = absOrAssetsPath.LastIndexOf(".");

            if (lastIndex >= 0)
            {
                return absOrAssetsPath.Substring(lastIndex);
            }

            return string.Empty;
        }

        public static string GetDirPath(this string absOrAssetsPath)
        {
            var name = absOrAssetsPath.Replace("\\", "/");
            var lastIndex = name.LastIndexOf("/");
            return name.Substring(0, lastIndex + 1);
        }

        public static string GetLastDirName(this string absOrAssetsPath)
        {
            var name = absOrAssetsPath.Replace("\\", "/");
            var dirs = name.Split('/');

            return absOrAssetsPath.EndsWith("/") ? dirs[dirs.Length - 2] : dirs[dirs.Length - 1];
        }

        #endregion
    }
}