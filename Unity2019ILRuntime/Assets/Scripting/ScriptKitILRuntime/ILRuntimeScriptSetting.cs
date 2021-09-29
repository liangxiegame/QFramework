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
        private const string scriptObjectPath = "Config/IlRuntimeConfig";
        private static ILRuntimeScriptSetting defaultVal;
        public static ILRuntimeScriptSetting Default
        {
            get
            {
                if (defaultVal != null) return defaultVal;
                defaultVal = Resources.Load<ILRuntimeScriptSetting>(scriptObjectPath);
                if (defaultVal != null) return defaultVal;
                defaultVal = CreateInstance<ILRuntimeScriptSetting>();
#if UNITY_EDITOR
                {
                    var savePath = $"{Application.dataPath}/Resources/Config";
                    Directory.CreateDirectory(savePath);
                    UnityEditor.AssetDatabase.CreateAsset(defaultVal, "Assets/Resources/" + scriptObjectPath + ".asset");
                    UnityEditor.AssetDatabase.SaveAssets();
                    UnityEditor.AssetDatabase.Refresh();
                }
#endif

                return defaultVal;
            }
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
        [HideInInspector]
        public bool UsePdb = false;
    }
}