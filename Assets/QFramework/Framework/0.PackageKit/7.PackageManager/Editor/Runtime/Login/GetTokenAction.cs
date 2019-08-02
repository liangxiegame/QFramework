/****************************************************************************
 * Copyright (c) 2018.10 liangxie
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
using Newtonsoft.Json.Linq;
using QF.Extensions;

namespace QF
{
    public class GetTokenAction
    {
        private static string URL
        {
            get { return "https://api.liangxiegame.com/qf/v4/token"; }
        }

        public static void DoGetToken(string username, string password, Action<string> onTokenGetted)
        {
            var form = new WWWForm();
            form.AddField("username", username);
            form.AddField("password", password);

            ObservableWWW.Post(URL, form)
                .Subscribe(response =>
                {
                    Debug.Log(response);

                    var responseJson = JObject.Parse(response);

                    var code = responseJson["code"].Value<int>();

                    if (code == 1)
                    {
                        var token = responseJson["data"]["token"].Value<string>();
                        onTokenGetted(token);
                    }
                });
        }
    }
}