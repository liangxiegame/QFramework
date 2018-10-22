/****************************************************************************
 * Copyright (c) 2018.9 liangxie
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
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
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

namespace QFramework
{
    public class LoginAction
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
                    return "http://127.0.0.1:8000/users/api_login/";

                }
                else
                {
                    return "http://liangxiegame.com/users/api_login/";
                }
            }
        }

        private const bool mIsTest = false;

        public static IEnumerator DoLogin(string username, string password,Action succeed)
        {
            var loginPage = UnityWebRequest.Get(LOGIN_URL);

            Debug.Log(username + ":" + password);
            
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


            var SetCookie = loginPage.GetResponseHeader("set-cookie");
            var rxCookie = new Regex("csrftoken=(?<csrf_token>.{64});");
            var cookieMatches = rxCookie.Matches(SetCookie);
            var csrfCookie = cookieMatches[0].Groups["csrf_token"].Value;

            // get the middleware value
            string loginPageHtml = loginPage.downloadHandler.text;
            Regex rxMiddleware = new Regex("name='csrfmiddlewaretoken' value='(?<csrf_token>.{64})'");
            MatchCollection middlewareMatches = rxMiddleware.Matches(loginPageHtml);
            string csrfMiddlewareToken = middlewareMatches[0].Groups ["csrf_token"].Value;
            
            /*
             * Make a login request.
             */
            if (mIsTest)
            {
                yield return new WaitForSeconds(0.3f);
            }

            var form = new WWWForm();

            form.AddField("username", username);
            form.AddField("password", password);
            form.AddField("csrfmiddlewaretoken", csrfMiddlewareToken);
            
            ObservableWWW.Post(REGISTER_URL, form, new Dictionary<string, string>()
            {
                {"cookie", "csrftoken=" + csrfCookie},
                {"X-CSRFToken", csrfCookie}
            }).Subscribe(result =>
            {
                Log.I(result);
                succeed.InvokeGracefully();
            }, Log.E);
        }
    }
}