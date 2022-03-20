/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System.ComponentModel;
using System.Linq;

namespace QFramework
{
    internal class PackageKitViewRenderInfo
    {
        public IPackageKitView Interface { get; private set; }
            
        public string DisplayName { get;  set; }

        public string GroupName { get; set; }

        public int RenderOrder { get;  set; }

        public PackageKitViewRenderInfo(IPackageKitView @interface)
        {
            Interface = @interface;

            var displayName = @interface.GetType().GetCustomAttributes(typeof(DisplayNameAttribute), false)
                .FirstOrDefault() as DisplayNameAttribute;
            DisplayName = displayName != null ? displayName.DisplayName : @interface.GetType().Name;

            var renderOrder = @interface.GetType()
                .GetCustomAttributes(typeof(PackageKitRenderOrderAttribute), false)
                .FirstOrDefault() as PackageKitRenderOrderAttribute;

            RenderOrder = renderOrder != null ? renderOrder.Order : int.MaxValue;

            var group = @interface.GetType()
                .GetCustomAttributes(typeof(PackageKitGroupAttribute), false)
                .FirstOrDefault() as PackageKitGroupAttribute;

            GroupName = group != null
                ? group.GroupName
                : "未分组";
        }
    }
}
#endif