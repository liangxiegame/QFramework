using System.Collections.Generic;
using QF.Action;
using QF.Extensions;

namespace QF.Editor
{
    public class DeletePackage : NodeAction
    {
        private static string DELETE_URL_TEMPLATE
        {
            get
            {
                if (User.Test)
                {
                    return "http://127.0.0.1:8000/api/packages/{0}/";

                }
                else
                {
                    return "http://liangxiegame.com/api/packages/{0}/";
                }
            }
        }

        public DeletePackage(string id)
        {
            mId = id;
        }

        private string mId;

        protected override void OnBegin()
        {
//            API.HttpDelete(DELETE_URL_TEMPLATE.FillFormat(mId),
//                new Dictionary<string, string> {{"Authorization", "Token " + User.Token.Value}},
//                Finish);
        }
    }
}