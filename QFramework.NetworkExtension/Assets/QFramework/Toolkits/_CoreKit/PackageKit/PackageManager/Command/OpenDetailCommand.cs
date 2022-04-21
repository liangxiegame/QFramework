/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using UnityEngine;

namespace QFramework
{
    internal class OpenDetailCommand : AbstractCommand
    {
        private readonly PackageRepository mPackageRepository;

        public OpenDetailCommand(PackageRepository packageRepository)
        {
            mPackageRepository = packageRepository;
        }

        protected override void OnExecute()
        {
            Application.OpenURL("https://qframework.cn/package/detail/" + mPackageRepository.id);
        }
    }
}
#endif