#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    internal class ListPackageCommand : AbstractCommand
    {
        private readonly Action<List<PackageRepository>, List<string>> mOnResponse;

        public ListPackageCommand(Action<List<PackageRepository>, List<string>> onResponse)
        {
            mOnResponse = onResponse;
        }

        const string Url = "https://api.liangxiegame.com/qf/v5/package/list";

        protected override void OnExecute()
        {
            if (User.Logined)
            {
                var form = new WWWForm();

                form.AddField("username", User.Username.Value);
                form.AddField("password", User.Password.Value);

                EditorHttp.Post(Url, form,
                    (response) => OnResponseV5(response, mOnResponse));
            }
            else
            {
                EditorHttp.Post(Url, new WWWForm(),
                    (response) => OnResponseV5(response, mOnResponse));
            }
        }

        void OnResponseV5(EditorHttpResponse response, Action<List<PackageRepository>, List<string>> onResponse)
        {
            if (response.Type == ResponseType.SUCCEED)
            {
                var responseJson =
                    JsonUtility.FromJson<ResultFormat<ListPackageResponseResult>>(response.Text);


                if (responseJson == null)
                {
                    onResponse(null, null);
                    return;
                }

                
                if (responseJson.code == 1)
                {
                    var listPackageResponseResult = responseJson.data;
                    
                    var packageTypeConfigModel = this.GetModel<PackageTypeConfigModel>();
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
    }
}
#endif