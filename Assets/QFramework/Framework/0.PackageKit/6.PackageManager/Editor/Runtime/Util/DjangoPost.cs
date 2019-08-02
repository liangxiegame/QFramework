/****************************************************************************
 * Copyright (c) 2018.7 ~ 10 liangxie
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
using System.Text.RegularExpressions;
using QF.Extensions;
using UnityEngine;
using UnityEngine.Networking;

namespace QF
{
    public class DjangoPost 
    {
        private static string LOGIN_URL
        {
            get { return false ? "http://127.0.0.1:8000/xadmin/login/" : "http://liangxiegame.com/xadmin/login/"; }
        }
        
        public static IEnumerator Post(string url, WWWForm form, string username, string password, Action<string> succeed)
        {
            var loginPage = UnityWebRequest.Get(LOGIN_URL);
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

            form = form ?? new WWWForm();
            form.AddField("username", username);
            form.AddField("password", password);

            var request =
                UnityWebRequest.Post(url, form);
            request.SetRequestHeader("cookie", "csrftoken=" + csrfCookie);
            request.SetRequestHeader("X-CSRFToken", csrfCookie);


            yield return request.Send();

#if UNITY_2017_1_OR_NEWER
            if (request.isNetworkError)
#else
            if (request.isError)
#endif
            {
                Debug.LogError(request.error);

            }
            else
            {
                succeed.InvokeGracefully(request.downloadHandler.text);
            }
        }
    }
}