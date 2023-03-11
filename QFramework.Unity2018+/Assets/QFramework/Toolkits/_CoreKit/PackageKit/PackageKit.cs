/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
namespace QFramework
{
    internal class PackageKit : Architecture<PackageKit>
    {
        protected override void Init()
        {

            // 包类型
            RegisterModel<IPackageTypeConfigModel>(new PackageTypeConfigModel());
            
            // 已安装类型
            RegisterModel<ILocalPackageVersionModel>(new LocalPackageVersionModel());
            RegisterModel<IPackageManagerModel>(new PackageManagerModel());
            RegisterModel<IPackageManagerServer>(new PackageManagerServer());
        }
    }
}
#endif