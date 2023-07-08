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
            var model = this.GetModel<IPackageManagerModel>();
            var server = this.GetModel<IPackageManagerServer>();
            var installedPackageVersionsModel = this.GetModel<ILocalPackageVersionModel>();
            installedPackageVersionsModel.Reload();

            PackageManagerState.PackageRepositories.Value = model.Repositories.OrderBy(p => p.name).ToList();
            this.SendCommand<UpdateCategoriesFromModelCommand>();

            server.GetAllRemotePackageInfoV5((list, categories) =>
            {
                if (list != null && categories != null)
                {
                    model.Repositories = list.OrderBy(p => p.name).ToList();
                    PackageManagerState.PackageRepositories.Value = model.Repositories;
                    this.SendCommand<UpdateCategoriesFromModelCommand>();
                }
                else
                {
                    EditorUtility.DisplayDialog("服务器请求失败", "请检查网络或排查问题", "确定");
                }
            });
        }
    }
}
#endif