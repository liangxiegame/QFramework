/****************************************************************************
 * Copyright (c) 2020.10 liangxie
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

using System.Linq;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    public class PackageManagerInitCommand : AbstractCommand
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