/****************************************************************************
 * Copyright (c) 2020.10 liangxiegame Under Mit License
 * 
 * https://qframework.cn
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
            foreach (var installedVersionIncludeFileOrFolder in mInstalledVersion.IncludeFileOrFolders)
            {
                var path = Application.dataPath.Replace("Assets", installedVersionIncludeFileOrFolder);
                
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
                else if (File.Exists(path))
                {
                    File.Delete(path);
                }
                else if (Directory.Exists(path + "/"))
                {
                    Directory.Delete(path + "/");
                }
            }

            RenderEndCommandExecutor.PushCommand(() =>
            {
                AssetDatabase.Refresh();

                EditorWindow.GetWindow<PackageKitWindow>().Close();

                this.SendCommand(new InstallPackageCommand(mPackageRepository));
            });
        }
    }
}
#endif