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

namespace QFramework.Editor
{
    public static class UploadPackage
    {
        private static string UPLOAD_URL
        {
            get
            {
                if (User.Test)
                {
                    return "http://127.0.0.1:8000/api/packages/";

                }
                else
                {
                    return "http://liangxiegame.com/api/packages/";
                }
            }
        }

        public static void DoUpload(PackageVersion packageVersion, Action succeed)
        {
            EditorUtility.DisplayProgressBar("插件上传", "打包中...", 0.1f);

            var fileName = packageVersion.Name + "_" + packageVersion.Version + ".unitypackage";
            var fullpath = FrameworkPMView.ExportPaths(fileName, packageVersion.InstallPath);
            var file = File.ReadAllBytes(fullpath);

            var form = new WWWForm();
            form.AddField("name", packageVersion.Name);
            form.AddField("version", packageVersion.Version);
            form.AddField("file_name", fileName);
            form.AddBinaryData("file", file);
            form.AddField("version", packageVersion.Version);
            form.AddField("release_note", packageVersion.Readme.content);
            form.AddField("install_path", packageVersion.InstallPath);
            form.AddField("access_right", packageVersion.AccessRight.ToString().ToLower());
            form.AddField("doc_url",packageVersion.DocUrl);
            
            if (packageVersion.Type == PackageType.FrameworkModule)
            {
                form.AddField("type", "fm");
            }
            else if (packageVersion.Type == PackageType.Shader)
            {
                form.AddField("type", "s");
            }
            else if (packageVersion.Type == PackageType.AppOrGameDemoOrTemplate)
            {
                form.AddField("type", "agt");
            }
            else if (packageVersion.Type == PackageType.Plugin)
            {
                form.AddField("type", "p");
            }

            Debug.Log(fullpath);

            EditorUtility.DisplayProgressBar("插件上传", "上传中...", 0.2f);


            ObservableWWW.Post(UPLOAD_URL, form, new Dictionary<string, string> { { "Authorization", "Token " + User.Token.Value } })
                         .Subscribe(responseContent =>
                         {
                             EditorUtility.ClearProgressBar();
                             Debug.Log(responseContent);
                             succeed.InvokeGracefully();
                             File.Delete(fullpath);
                         }, e =>
                         {
                             EditorUtility.ClearProgressBar();
                             EditorUtility.DisplayDialog("插件上传", "上传失败!{0}".FillFormat(e.Message), "确定");
                             File.Delete(fullpath);

                         });
        }
    }
}