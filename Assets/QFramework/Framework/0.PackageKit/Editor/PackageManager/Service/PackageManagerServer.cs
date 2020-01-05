using System;
using System.Collections.Generic;
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

        public void GetAllRemotePackageInfo(Action<List<PackageData>> onResponse)
        {
            if (User.Logined)
            {
                var form = new WWWForm();

                form.AddField("username", User.Username.Value);
                form.AddField("password", User.Password.Value);

                EditorHttp.Post("https://api.liangxiegame.com/qf/v4/package/list", form,
                    (response) => OnResponse(response, onResponse));
            }
            else
            {
                EditorHttp.Post("https://api.liangxiegame.com/qf/v4/package/list", new WWWForm(),
                    (response) => OnResponse(response, onResponse));
            }
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
                    
                    onResponse(listPackageResponseResult.repositories, listPackageResponseResult.categories);
                }
            }
            else
            {
                onResponse(null, null);
            }
        }

        void OnResponse(EditorHttpResponse response, Action<List<PackageData>> onResponse)
        {
            if (response.Type == ResponseType.SUCCEED)
            {
                var responseJson =
                    JsonUtility.FromJson<QFrameworkServerResultFormat<List<ResultPackage>>>(response.Text);

                if (responseJson !=null && responseJson.code == 1)
                {
                    var packageInfosJson = responseJson.data;

                    var packageDatas = new List<PackageData>();
                    foreach (var packageInfo in packageInfosJson)
                    {
                        var name = packageInfo.name;

                        var package = packageDatas.Find(packageData => packageData.Name == name);

                        if (package == null)
                        {
                            package = new PackageData()
                            {
                                Name = name,
                            };

                            packageDatas.Add(package);
                        }

                        var id = packageInfo.id;
                        var version = packageInfo.version;
                        var url = packageInfo.downloadUrl;
                        var installPath = packageInfo.installPath;
                        var releaseNote = packageInfo.releaseNote;
                        var createAt = packageInfo.createAt;
                        var creator = packageInfo.username;
                        var releaseItem = new ReleaseItem(version, releaseNote, creator, DateTime.Parse(createAt), id);
                        var accessRightName = packageInfo.accessRight;
                        var typeName = packageInfo.type;

                        var packageType = PackageType.FrameworkModule;

                        switch (typeName)
                        {
                            case "fm":
                                packageType = PackageType.FrameworkModule;
                                break;
                            case "s":
                                packageType = PackageType.Shader;
                                break;
                            case "agt":
                                packageType = PackageType.AppOrGameDemoOrTemplate;
                                break;
                            case "p":
                                packageType = PackageType.Plugin;
                                break;
                            case "master":
                                packageType = PackageType.Master;
                                break;
                        }

                        var accessRight = PackageAccessRight.Public;

                        switch (accessRightName)
                        {
                            case "public":
                                accessRight = PackageAccessRight.Public;
                                break;
                            case "private":
                                accessRight = PackageAccessRight.Private;
                                break;
                        }

                        package.PackageVersions.Add(new PackageVersion()
                        {
                            Id = id,
                            Version = version,
                            DownloadUrl = url,
                            InstallPath = installPath,
                            Type = packageType,
                            AccessRight = accessRight,
                            Readme = releaseItem,
                        });

                        package.readme.AddReleaseNote(releaseItem);
                    }

                    packageDatas.ForEach(packageData =>
                    {
                        packageData.PackageVersions.Sort((a, b) =>
                            b.VersionNumber - a.VersionNumber);
                        packageData.readme.items.Sort((a, b) =>
                            b.VersionNumber - a.VersionNumber);
                    });

                    onResponse(packageDatas);

                    new PackageInfosRequestCache()
                    {
                        PackageDatas = packageDatas
                    }.Save();
                }
            }
            else
            {
                onResponse(null);
            }
        }
    }
}