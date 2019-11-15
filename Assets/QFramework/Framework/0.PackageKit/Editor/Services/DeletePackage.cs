using Newtonsoft.Json.Linq;
using QFramework;
using UnityEngine;

namespace QF.Editor
{
    public class DeletePackage : NodeAction
    {
        public DeletePackage(string id)
        {
            mId = id;
        }

        private string mId;

        protected override void OnBegin()
        {
            var form = new WWWForm();

            form.AddField("username", User.Username.Value);
            form.AddField("password", User.Password.Value);
            form.AddField("id", mId);

            EditorHttp.Post("https://api.liangxiegame.com/qf/v4/package/delete", form, (type, response) =>
            {
                if (type == ResponseType.SUCCEED)
                {
                    var result = JObject.Parse(response);

                    if (result.Value<int>("code") == 1)
                    {
                        Debug.Log("删除成功");
                    }

                    Finish();
                }
            });
        }
    }
}