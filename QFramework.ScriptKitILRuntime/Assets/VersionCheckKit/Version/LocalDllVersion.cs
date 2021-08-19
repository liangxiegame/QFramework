using UnityEngine;
using UnityEngine.Experimental.AssetBundlePatching;

namespace QFramework
{ 
    /// <summary>
    /// 本地的 dll 版本
    /// </summary>
    public static class LocalDllVersion
    {
        public static DLLVersion Version;

        public static void Load()
        {
            // 获取本地的版本
            
            // 从哪里获取 本地版本号
            // 2. persistentDataPath
            
            // 从来没更新过就从 stream 加载
            
            // 更新过 从 persistentDataPath
            
            // 怎么判断是否进行更新过

            // 1. streamingAssetsPath
            var versionJsonPath = Application.streamingAssetsPath + "/" + AssetBundleSettings.GetPlatformForAssetBundles(Application.platform) +
                                  "/hotfix/hotfix.json";

            var version = SerializeHelper.LoadJson<DLLVersion>(versionJsonPath);
            Version = version;
            Debug.Log("Local Version Loded:" + version.Version);
        }
    }
}