/****************************************************************************
 * Copyright (c) 2018.7 ~ 9 liangxie
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
using System.IO;
using System.Text.RegularExpressions;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace QFramework
{
    public static class UploadPackage
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

        private static string UPLOAD_URL
        {
            get
            {
                if (mIsTest)
                {
                    return "http://127.0.0.1:8000/framework_package/upload_package/";

                }
                else
                {
                    return "http://liangxiegame.com/framework_package/upload_package/";
                }
            }
        }

        private const bool mIsTest = false;

        public static IEnumerator DoUpload(string username, string password, PackageVersion packageVersion,Action succeed)
        {
            var loginPage = UnityWebRequest.Get(LOGIN_URL);
            EditorUtility.DisplayProgressBar("插件上传","获取验证信息",0.1f);
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

            
            EditorUtility.DisplayProgressBar("插件上传","生成 token",0.1f);

            var SetCookie = loginPage.GetResponseHeader("set-cookie");
            var rxCookie = new Regex("csrftoken=(?<csrf_token>.{64});");
            var cookieMatches = rxCookie.Matches(SetCookie);
            var csrfCookie = cookieMatches[0].Groups["csrf_token"].Value;


            var form = new WWWForm();
            var fileName = packageVersion.Name + "_" + packageVersion.Version + ".unitypackage";
            var fullpath = FrameworkPMView.ExportPaths(fileName, packageVersion.InstallPath);
            var file = File.ReadAllBytes(fullpath);

            form.AddField("username", username);
            form.AddField("password", password);

            form.AddField("name", packageVersion.Name);
            form.AddField("file_name", fileName);
            form.AddBinaryData("file", file);
            form.AddField("version", packageVersion.Version);
            form.AddField("release_note", packageVersion.Readme.content);

            if (packageVersion.Type == PackageType.FrameworkModule)
            {
                form.AddField("type", "fm");
            }
            else if (packageVersion.Type == PackageType.Shader)
            {
                form.AddField("type","s");
            } 
            else if (packageVersion.Type == PackageType.AppOrGameDemoOrTemplate)
            {
                form.AddField("type", "agt");
            }

            
            var doLogin3 =
                UnityWebRequest.Post(UPLOAD_URL, form);
            doLogin3.SetRequestHeader("cookie", "csrftoken=" + csrfCookie);
            doLogin3.SetRequestHeader("X-CSRFToken", csrfCookie);

            File.Delete(fullpath);

            var timer = Observable.Interval(TimeSpan.FromSeconds(0.5f)).Subscribe(_ => { Log.E("upload"); });
            yield return doLogin3.Send();
            timer.Dispose();
            
            EditorUtility.ClearProgressBar();

            #if UNITY_2017_1_OR_NEWER
            if (doLogin3.isNetworkError)
            #else
            if (doLogin3.isError)
            #endif            
            {
                EditorUtility.DisplayDialog("插件上传", "上传失败!{0}".FillFormat(doLogin3.error), "确定");
            }
            else
            {
                Log.I(doLogin3.downloadHandler.text);
                succeed.InvokeGracefully();
            }
        }
        
    }
}