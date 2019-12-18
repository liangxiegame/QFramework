using System;
using QFramework;
using QFramework.PackageKit;
using UnityEngine;

namespace QFramework.PackageKit
{
    public class PacakgeLoginService : IPackageLoginService
    {
        [Serializable]
        public class ResultFormatData
        {
            public string token;
        }

        public void DoGetToken(string username, string password, Action<string> onTokenGetted)
        {
            var form = new WWWForm();
            form.AddField("username", username);
            form.AddField("password", password);

            EditorHttp.Post("https://api.liangxiegame.com/qf/v4/token", form, response =>
            {
                if (response.Type == ResponseType.SUCCEED)
                {
                    Debug.Log(response.Text);

                    var responseJson =
                        JsonUtility.FromJson<QFrameworkServerResultFormat<ResultFormatData>>(response.Text);

                    var code = responseJson.code;

                    if (code == 1)
                    {
                        var token = responseJson.data.token;
                        onTokenGetted(token);
                    }
                }
                else if (response.Type == ResponseType.EXCEPTION)
                {
                    Debug.LogError(response.Error);
                }
            });
        }
    }
}