using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QFramework.PackageKit
{

    [Serializable]
    public class QFrameworkServerResultFormat<T>
    {
        public int code;

        public string msg;

        public T data;
    }

    public class PackageManagerServer : IPackageManagerServer
    {
        public void DeletePackage(string packageId, Action onResponse)
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
                        Debug.Log("删除成功");

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
                
                if (responseJson.code == 1)
                {
                    var listPackageResponseResult = responseJson.data;
                    
                    
                    foreach (var packageRepository in listPackageResponseResult.repositories)
                    {
                        packageRepository.type = PackageTypeHelper.TryGetFullName(packageRepository.type);
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
    }
}