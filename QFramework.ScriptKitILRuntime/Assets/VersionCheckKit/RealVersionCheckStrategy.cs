using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

namespace QFramework
{
    public class RealVersionCheckStrategy : IVersionCheckStrategy
    {
        string StreamingAssetPath
        {
            get
            {
                if (Application.platform == RuntimePlatform.OSXEditor ||
                    Application.platform == RuntimePlatform.OSXPlayer)
                {
                    return "file://" + Application.streamingAssetsPath;
                }
                else
                {
                    return Application.streamingAssetsPath;
                }
            }
        }

        string PersistentDataPath
        {
            get
            {
                if (Application.platform == RuntimePlatform.OSXEditor ||
                    Application.platform == RuntimePlatform.OSXPlayer)
                {
                    return "file://" + Application.persistentDataPath;
                }
                else
                {
                    return Application.persistentDataPath;
                }
            }
        }

        public string ServerHost
        {
            get { return "http://hotfix.liangxiegame.com/"; }
        }

        public void UpdateRes(Action done)
        {

            Observable.FromCoroutine(() => DoDownload()).Subscribe(_ => { done(); });
        }

        IEnumerator DoDownload()
        {
            var savePath = PersistentDataPath.CombinePath("AssetBundles")
                .CombinePath(AssetBundleSettings.GetPlatformForAssetBundles(Application.platform));

            savePath.DeleteDirIfExists();
            savePath.CreateDirIfNotExists();

            var fileNames = new string[]
            {
                "Android",
                "Android.manifest",
                "Android.manifest.meta",
                "Android.meta",
                "README.en.md",
                "asset_bindle_config.bin",
                "asset_bindle_config.bin.meta",
                "audio",
                "audio.manifest",
                "audio.manifest.meta",
                "audio.meta",
                "hotfix.dll",
                "hotfix.dll.mdb",
                "hotfix.dll.mdb.meta",
                "hotfix.dll.meta",
                "hotfix.json",
                "hotfix.json.meta",
                "shapes",
                "shapes.manifest",
                "shapes.manifest.meta",
                "shapes.meta",
                "uitetrispanel_prefab",
                "uitetrispanel_prefab.manifest",
                "uitetrispanel_prefab.manifest.meta",
                "uitetrispanel_prefab.meta",
            };

            foreach (var fileName in fileNames)
            {
                var www = new UnityWebRequest(ServerHost + fileName);
                var persistFile = savePath.CombinePath(fileName);

                www.downloadHandler = new DownloadHandlerFile(persistFile);
                yield return www.SendWebRequest();
            }
        }

        public void LocalVersionGetter(Action<DLLVersion> onLocalVersionGetted)
        {
            if (ILRuntimeScriptSetting.LoadDLLFromStreamingAssetsPath)
            {
                var localJsonFile =
                    StreamingAssetPath.CombinePath(
                        "AssetBundles/{0}/hotfix.json".FillFormat(AssetBundleSettings.GetPlatformForAssetBundles(Application.platform)));

                ObservableWWW.Get(localJsonFile).Subscribe(content =>
                {
                    var localVersion = content.FromJson<DLLVersion>();

                    onLocalVersionGetted(localVersion);
                });
            }
            else
            {
                var localJsonFile =
                    PersistentDataPath.CombinePath(
                        "AssetBundles/{0}/hotfix.json".FillFormat(AssetBundleSettings.GetPlatformForAssetBundles(Application.platform)));

                var localVersion = localJsonFile.LoadJson<DLLVersion>();

                onLocalVersionGetted(localVersion);
            }
        }

        public void ServerVersionGetter(Action<DLLVersion> onServerVersionGetted)
        {
            ObservableWWW.Get(ServerHost + "hotfix.json").Subscribe(result =>
            {
                result.LogInfo();

                var serverVersion = result.FromJson<DLLVersion>();

                Debug.Log(serverVersion.Version);

                onServerVersionGetted(serverVersion);
            });
        }

        public void VersionCheck(Action<bool, DLLVersion, DLLVersion> onHasRes)
        {
            LocalVersionGetter(localVersion =>
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
            });
        }
    }
}