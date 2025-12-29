/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System.Linq;
using UnityEditor;

namespace QFramework
{
    internal class PackageManagerInitCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var model = this.GetModel<PackageManagerModel>();
            var installedPackageVersionsModel = this.GetModel<LocalPackageVersionModel>();
            installedPackageVersionsModel.Reload();

            PackageKit.PackageRepositories.Value = model.Repositories.OrderBy(p => p.name).ToList();
            this.SendCommand<UpdateCategoriesFromModelCommand>();
            this.SendCommand(new ListPackageCommand((list, categories) =>
            {
                if (list != null && categories != null)
                {
                    model.Repositories = list.OrderBy(p => p.name).ToList();
                    PackageKit.PackageRepositories.Value = model.Repositories;
                    this.SendCommand<UpdateCategoriesFromModelCommand>();
                }
                else
                {
                    EditorUtility.DisplayDialog("服务器请求失败", "请检查网络或排查问题", "确定");
                }
            }));
        }
    }
}
#endif