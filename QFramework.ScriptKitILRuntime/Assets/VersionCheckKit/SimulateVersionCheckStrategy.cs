using System;
using System.IO;
using UnityEngine;

namespace QFramework
{
    /// <summary>
    /// 模拟策略
    /// </summary>
    public class SimulateVersionCheckStrategy : IVersionCheckStrategy
    {
        /// <summary>
        /// 服务器路径
        /// </summary>
        public string ServerHost
        {
            get { return Application.dataPath.CombinePath("../SimulateHotfixServer"); }
        }

        public void UpdateRes(Action done)
        {
            var savePath = Application.persistentDataPath.CombinePath("AssetBundles/Android");

            savePath.DeleteDirIfExists();
            savePath.CreateDirIfNotExists();

            var files = Directory.GetFiles(ServerHost);

            files.ForEach(f =>
            {
                var persistFile = f.Replace(ServerHost, savePath);

                File.Copy(f, persistFile);
            });

            done();
        }

        public void LocalVersionGetter(Action<DLLVersion> onLocalVersionGetted)
        {
            var localJsonFile = ILRuntimeScriptSetting.LoadDLLFromStreamingAssetsPath
                ? Application.streamingAssetsPath.CombinePath("AssetBundles/Android/hotfix.json")
                : Application.persistentDataPath.CombinePath("AssetBundles/Android/hotfix.json");

            var localVersion = localJsonFile.LoadJson<DLLVersion>();

            onLocalVersionGetted(localVersion);
        }

        public void ServerVersionGetter(Action<DLLVersion> onServerVersionGetted)
        {
            var versionJsonFile = ServerHost.CombinePath("hotfix.json");

            var serverVersion = versionJsonFile.LoadJson<DLLVersion>();

            Debug.Log(serverVersion.Version);

            onServerVersionGetted(serverVersion);
        }

        public void VersionCheck(Action<bool, DLLVersion, DLLVersion> onHasRes)
        {
            LocalVersionGetter((localVersion =>
            {
                ServerVersionGetter(serverVersion =>
                {
                    if (localVersion.Version < serverVersion.Version)
                    {
                        onHasRes(true, localVersion, serverVersion);
                    }
                    else
                    {
                        onHasRes(false, null, null);
                    }
                });
            }));
        }
    }
}