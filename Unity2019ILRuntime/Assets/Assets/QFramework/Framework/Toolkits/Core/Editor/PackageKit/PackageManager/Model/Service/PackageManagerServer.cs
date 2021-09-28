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

using System;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{

    [Serializable]
    public class QFrameworkServerResultFormat<T>
    {
        public int code;

        public string msg;

        public T data;
    }

    public class PackageManagerServer : AbstractModel, IPackageManagerServer,ICanGetModel
    {
        public void DeletePackage(string packageId, System.Action onResponse)
        {
            var form = new WWWForm();

            form.AddField("username", User.Username.Value);
            form.AddField("password", User.Password.Value);
            form.AddField("id", packageId);

            EditorHttp.Post("https://api.liangxiegame.com/qf/v4/package/delete", form, (response) =>
            {
                if (response.Type == ResponseType.SUCCEED)
                {
                    var result = JsonUtility.FromJson<QFrameworkServerResultFormat<object>>(response.Text);

                    if (result.code == 1)
                    {
                        Log.I("删除成功");

                        onResponse();
                    }
                }
            });
        }
        
        public void GetAllRemotePackageInfoV5(Action<List<PackageRepository>, List<string>> onResponse)
        {
            if (User.Logined)
            {
                var form = new WWWForm();

                form.AddField("username", User.Username.Value);
                form.AddField("password", User.Password.Value);

                EditorHttp.Post("https://api.liangxiegame.com/qf/v5/package/list", form,
                    (response) => OnResponseV5(response, onResponse));
            }
            else
            {
                EditorHttp.Post("https://api.liangxiegame.com/qf/v5/package/list", new WWWForm(),
                    (response) => OnResponseV5(response, onResponse));
            }
        }

        [Serializable]
        public class ResultPackage
        {
            public string id;
            public string name;
            public string version;
            public string downloadUrl;
            public string installPath;
            public string releaseNote;
            public string createAt;
            public string username;
            public string accessRight;
            public string type;
        }


        [Serializable]
        public class ListPackageResponseResult
        {
            public List<string> categories;
            public List<PackageRepository> repositories;
        }
        
        void OnResponseV5(EditorHttpResponse response, Action<List<PackageRepository>,List<string>> onResponse)
        {
            if (response.Type == ResponseType.SUCCEED)
            {
                var responseJson =
                    JsonUtility.FromJson<QFrameworkServerResultFormat<ListPackageResponseResult>>(response.Text);


                if (responseJson == null)
                {
                    onResponse(null, null);
                    return;
                }
                
                if (responseJson.code == 1)
                {
                    var listPackageResponseResult = responseJson.data;


                    var packageTypeConfigModel = this.GetModel<IPackageTypeConfigModel>();
                    foreach (var packageRepository in listPackageResponseResult.repositories)
                    {
                        packageRepository.type = packageTypeConfigModel.GetFullTypeName(packageRepository.type);
                    }
                    
                    new PackageInfosRequestCache()
                    {
                        PackageRepositories = listPackageResponseResult.repositories
                    }.Save();
                    
                    onResponse(listPackageResponseResult.repositories, listPackageResponseResult.categories);
                }
            }
            else
            {
                onResponse(null, null);
            }
        }

        protected override void OnInit()
        {
            
        }
    }
}