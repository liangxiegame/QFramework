#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    internal class ListMyPackageCommand : AbstractCommand
    {
        private const string Url = "https://api.liangxiegame.com/qf/my_package/list";


        private readonly Action<List<PackageRepository>> mOnResponse;

        internal ListMyPackageCommand(Action<List<PackageRepository>> onResponse)
        {
            mOnResponse = onResponse;
        }

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
                mOnResponse(new List<PackageRepository>());
            }
        }

        void OnResponseV5(EditorHttpResponse response, Action<List<PackageRepository>> onResponse)
        {
            if (response.Type == ResponseType.SUCCEED)
            {
                var responseJson =
                    JsonUtility.FromJson<ResultFormat<ListPackageResponseResult>>(response.Text);
                
                if (responseJson == null)
                {
                    onResponse(null);
                    return;
                }

                if (responseJson.code == 1)
                {
                    onResponse(responseJson.data.repositories);
                }
            }
            else
            {
                onResponse(null);
            }
        }
        


        [Serializable]
        public class ListPackageResponseResult
        {
            public List<PackageRepository> repositories;
        }
    }
}
#endif