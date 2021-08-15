using QFramework;
using UnityEngine;

namespace QFramework
{ 
    /// <summary>
    /// 服务器的 dll 版本号
    /// </summary>
    public static class ServerDllVersion
    {
        public static DLLVersion Version;

        /// <summary>
        /// 异步的
        /// </summary>
        public static void Get()
        {
            // 服务器里获取版本

            // 假装 WebPlayerTemplates 是服务器

            // 服务器的版本文件
            var remoteJsonPath = Application.dataPath + "/WebPlayerTemplates/HotfixServer/hotfix.json";
            
            Version = SerializeHelper.LoadJson<DLLVersion>(remoteJsonPath);

            Debug.Log("ServerJsonPath:" + Version.Version);
        }
    }
}