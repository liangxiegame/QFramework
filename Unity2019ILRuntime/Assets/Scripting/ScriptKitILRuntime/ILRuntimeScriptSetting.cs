using System.IO;
using UnityEngine;

namespace QFramework
{
    public enum HotfixCodeRunMode
    {
        ILRuntime,
        Reflection,
    }
    
    public class ILRuntimeScriptSetting : ScriptableObject
    {
        private static string ScriptObjectPath => $"Resources/{LoadPath}";
        private const string LoadPath = "Config/ILRuntimeConfig";
        private static ILRuntimeScriptSetting defaultVal;
        public static ILRuntimeScriptSetting Default
        {
            get
            {
                if (defaultVal != null) return defaultVal;
                defaultVal = Resources.Load<ILRuntimeScriptSetting>(LoadPath);
                if (defaultVal != null) return defaultVal;
                defaultVal = CreateInstance<ILRuntimeScriptSetting>();
                Save();
                return defaultVal;
            }
        }

        public static void Save()
        {
#if UNITY_EDITOR
            var filePath = $"{Application.dataPath}/{ScriptObjectPath}.asset";
            if (!File.Exists(filePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                UnityEditor.AssetDatabase.CreateAsset(defaultVal, $"Assets/{ScriptObjectPath}.asset");
            }
            UnityEditor.EditorUtility.SetDirty(Default);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

        /// <summary>
        /// 检测是否热更过
        /// </summary>
        public static bool LoadDLLFromStreamingAssetsPath
        {
            get { return PlayerPrefs.GetInt("LoadDLLFromStreammingAssetsPath", 1) == 1; }
            set { PlayerPrefs.SetInt("LoadDLLFromStreammingAssetsPath", value ? 1 : 0); }
        }


        public static string DllFileStreamingFullPath
        {
            get
            {
                return Application.streamingAssetsPath + "/AssetBundles/" +
                       AssetBundleSettings.GetPlatformForAssetBundles(Application.platform) + "hotfix.dll";
            }
        }

        public static string DllFilePersistentFullPath
        {
            get
            {
                return Application.persistentDataPath + "/AssetBundles/" +
                       AssetBundleSettings.GetPlatformForAssetBundles(Application.platform) + "hotfix.dll";
            }
        }

        public HotfixCodeRunMode HotfixRunMode = HotfixCodeRunMode.Reflection;
        public string GenAdaptorPath = "QFramework/Scripting/ScriptKitILRuntime/ILRuntime/Adapter";
        public string GenClrBindPath = "QFrameworkData/ScriptKitILRuntimeCLRBindingCodeGen";
        public string HotfixAsmdefName = "*@hotfix";
        public string DllOutPath => Application.streamingAssetsPath + "/AssetBundles/" +
                                    AssetBundleSettings.GetPlatformForAssetBundles(Application.platform);
        [HideInInspector]
        public bool UsePdb = false;

        public bool AutoCompile = false;
    }
}