using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

namespace QFramework
{
    public class RegisterAction
    {
        private static string LOGIN_URL
        {
            get {
                if (mIsTest)
                {
                    return "http://127.0.0.1:8000/xadmin/login/";
                }
                else
                {
                    return "http://liangxiegame.com/xadmin/login/";
                }
            }
        }

        private static string REGISTER_URL
        {
            get
            {
                if (mIsTest)
                {
                    return "http://127.0.0.1:8000/users/register/";

                }
                else
                {
                    return "http://liangxiegame.com/users/api_register/";
                }
            }
        }

        private const bool mIsTest = true;

        public static IEnumerator DoRegister(string username, string password,string email,Action succeed)
        {
            var loginPage = UnityWebRequest.Get(LOGIN_URL);

            Debug.Log(username + ":" + password + ":" + email);
            
            yield return loginPage.Send();

            #if UNITY_2017_1_OR_NEWER
            if (loginPage.isNetworkError)
            #else
            if (loginPage.isError)
            #endif
            {
                Debug.Log(loginPage.error);
                yield break;
            }

            // get the csrf cookie
            var SetCookie = loginPage.GetResponseHeader("set-cookie");
            var rxCookie = new Regex("csrftoken=(?<csrf_token>.{64});");
            var cookieMatches = rxCookie.Matches(SetCookie);
            var csrfCookie = cookieMatches[0].Groups["csrf_token"].Value;

            /*
             * Make a login request.
             */

            var form = new WWWForm();

            form.AddField("username", username);
            form.AddField("email", email);
            form.AddField("password1", password);
            form.AddField("password2", password);

            var doRegister =
                UnityWebRequest.Post(REGISTER_URL, form);
            doRegister.SetRequestHeader("cookie", "csrftoken=" + csrfCookie);
            doRegister.SetRequestHeader("X-CSRFToken", csrfCookie);


            yield return doRegister.Send();

            #if UNITY_2017_1_OR_NEWER
            if (doRegister.isNetworkError)
            #else
            if (doRegister.isError)
            #endif            
            {
                Log.E(doRegister.error);
            }
            else
            {
                Log.I(doRegister.downloadHandler.text);
                succeed.InvokeGracefully();
            }
        }
    }
}