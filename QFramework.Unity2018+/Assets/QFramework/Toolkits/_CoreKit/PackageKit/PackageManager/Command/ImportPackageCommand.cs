/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using UnityEditor;

namespace QFramework
{
    internal class ImportPackageCommand : AbstractCommand
    {
        private readonly PackageRepository mPackageRepository;

        public ImportPackageCommand(PackageRepository packageRepository)
        {
            mPackageRepository = packageRepository;
        }

        protected override void OnExecute()
        {
            EditorWindow.GetWindow<PackageKitWindow>().Close();

            this.SendCommand(new InstallPackageCommand(mPackageRepository));
        }
    }
}
#endif