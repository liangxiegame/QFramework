/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System.Collections.Generic;

#if UNITY_EDITOR
namespace QFramework
{
    internal class PackageKit : Architecture<PackageKit>
    {
        protected override void Init()
        {
            // 包类型
            RegisterModel(new PackageTypeConfigModel());
            
            // 已安装类型
            RegisterModel(LocalPackageVersionModel.Default);
            RegisterModel(new PackageManagerModel());
        }
        
        public static BindableProperty<List<PackageRepository>> PackageRepositories =
            new BindableProperty<List<PackageRepository>>(new List<PackageRepository>());
        
        public static BindableProperty<int> CategoryIndex = new BindableProperty<int>(0);
        
        public static BindableProperty<List<string>> Categories = new BindableProperty<List<string>>();
        
        public static BindableProperty<int> AccessRightIndex = new BindableProperty<int>(0);
        
        public static BindableProperty<string> SearchKey = new BindableProperty<string>("");
    }
}
#endif