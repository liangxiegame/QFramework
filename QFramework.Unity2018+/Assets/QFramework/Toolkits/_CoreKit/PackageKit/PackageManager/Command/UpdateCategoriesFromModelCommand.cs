/****************************************************************************
 * Copyright (c) 2015 - 2023 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System.Linq;

namespace QFramework
{
    internal class UpdateCategoriesFromModelCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var model = this.GetModel<PackageManagerModel>();

            var packageTypeConfigModel = this.GetModel<PackageTypeConfigModel>();
            var categories = model.Repositories.Select(p => p.type).Distinct()
                .Select(t => packageTypeConfigModel.GetFullTypeName(t))
                .ToList();
            categories.Insert(0, "All");
            PackageKit.Categories.Value = categories;
        }
    }
}
#endif