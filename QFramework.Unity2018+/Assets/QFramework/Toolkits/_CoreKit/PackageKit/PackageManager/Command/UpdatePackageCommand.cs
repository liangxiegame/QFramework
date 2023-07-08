/****************************************************************************
 * Copyright (c) 2015 - 2023 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    internal class UpdatePackageCommand : AbstractCommand
    {
        public UpdatePackageCommand(PackageRepository packageRepository, PackageVersion installedVersion)
        {
            mPackageRepository = packageRepository;
            mInstalledVersion = installedVersion;
        }

        private readonly PackageRepository mPackageRepository;
        private readonly PackageVersion mInstalledVersion;

        protected override void OnExecute()
        {
            RenderEndCommandExecutor.PushCommand(() =>
            {
                AssetDatabase.Refresh();

                EditorWindow.GetWindow<PackageKitWindow>().Close();

                this.SendCommand(new InstallPackageCommand(mPackageRepository, () =>
                {
                    PackageHelper.DeletePackageFiles(mInstalledVersion);
                }));
            });
        }
    }
}
#endif