/****************************************************************************
 * Copyright (c) 2016 - 2023 liangxiegame UNDER MIT License
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

        public string GetDisplayName()
        {
            if (LocaleKitEditor.IsCN.Value && DisplayNameCN.IsNotNullAndEmpty())
            {
                return DisplayNameCN;
            }

            if (!LocaleKitEditor.IsCN.Value && DisplayNameEN.IsNotNullAndEmpty())
            {
                return DisplayNameEN;
            }

            return DisplayName ?? DisplayNameCN ?? DisplayNameEN;
        }

        public string DisplayName { get; set; }

        public string DisplayNameCN { get; set; }
        public string DisplayNameEN { get; set; }

        public string GroupName { get; set; }

        public int RenderOrder { get; set; }

        public PackageKitViewRenderInfo(IPackageKitView @interface)
        {
            Interface = @interface;

            var type = @interface.GetType();

            if (type.HasAttribute<DisplayNameAttribute>())
            {
                DisplayName = type.GetAttribute<DisplayNameAttribute>().DisplayName;
            }

            if (type.HasAttribute<DisplayNameCNAttribute>())
            {
                DisplayNameCN = type.GetAttribute<DisplayNameCNAttribute>().DisplayName;
            }

            if (type.HasAttribute<DisplayNameENAttribute>())
            {
                DisplayNameEN = type.GetAttribute<DisplayNameENAttribute>().DisplayName;
            }


            var renderOrder = type
                .GetCustomAttributes(typeof(PackageKitRenderOrderAttribute), false)
                .FirstOrDefault() as PackageKitRenderOrderAttribute;

            RenderOrder = renderOrder != null ? renderOrder.Order : int.MaxValue;

            var group = type
                .GetCustomAttributes(typeof(PackageKitGroupAttribute), false)
                .FirstOrDefault() as PackageKitGroupAttribute;

            GroupName = group != null
                ? group.GroupName
                : "未分组";
        }
    }
}
#endif