using Newtonsoft.Json.Linq;
using QF.Action;
using QFramework;
using UniRx;
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
            Debug.Log(mId);
            
            var form = new WWWForm();

            form.AddField("username", User.Username.Value);
            form.AddField("password", User.Password.Value);
            form.AddField("id", mId);

            ObservableWWW.Post("https://api.liangxiegame.com/qf/v4/package/delete", form)
                .Subscribe(response =>
                {
                    var result = JObject.Parse(response);

                    if (result.Value<int>("code") == 1)
                    {
                         Debug.Log("删除成功");
                    }
                    
                    Finish();
                });
        }
    }
}