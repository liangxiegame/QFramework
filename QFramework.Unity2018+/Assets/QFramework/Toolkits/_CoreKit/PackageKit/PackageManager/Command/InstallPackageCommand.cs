/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    internal class InstallPackageCommand : AbstractCommand
    {
        private readonly PackageRepository mRequestPackageData;
        private readonly Action mOnAfterDownloadBeforeImport;

        private void OnProgressChanged(float progress)
        {
            EditorUtility.DisplayProgressBar("插件更新",
                $"插件下载中 {progress:P2}", progress);
        }

        public InstallPackageCommand(PackageRepository requestPackageData, Action onAfterDownloadBeforeImport = null)
        {
            mRequestPackageData = requestPackageData;
            mOnAfterDownloadBeforeImport = onAfterDownloadBeforeImport;
        }

        protected override void OnExecute()
        {
            var tempFile = "Assets/" + mRequestPackageData.name + ".unitypackage";

            Debug.Log(mRequestPackageData.latestDownloadUrl + ">>>>>>:");

            EditorUtility.DisplayProgressBar("插件更新", "插件下载中 ...", 0.1f);
            
            EditorHttp.Download(mRequestPackageData.latestDownloadUrl, response =>
            {
                if (response.Type == ResponseType.SUCCEED)
                {
                    File.WriteAllBytes(tempFile, response.Bytes);

                    EditorUtility.ClearProgressBar();
                    
                    mOnAfterDownloadBeforeImport?.Invoke();

                    AssetDatabase.ImportPackage(tempFile, false);

                    File.Delete(tempFile);

                    AssetDatabase.Refresh();

                    Debug.Log("PackageManager:插件下载成功");


                    LocalPackageVersionModel.Default
                        .Reload();
                }
                else
                {
                    EditorUtility.ClearProgressBar();

                    EditorUtility.DisplayDialog(mRequestPackageData.name,
                        "插件安装失败,请联系 liangxiegame@163.com 或者加入 QQ 群:623597263" + response.Error + ";", "OK");
                }
            }, OnProgressChanged);
        }
    }
}
#endif