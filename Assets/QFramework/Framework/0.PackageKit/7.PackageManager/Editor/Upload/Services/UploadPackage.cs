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

using System.Collections.Generic;
using System.IO;
using QF.Editor;
using QF.Extensions;
using UniRx;
using UnityEditor;
using UnityEngine;

namespace QF.PackageKit.Upload
{
    public static class UploadPackage
    {
        private static string UPLOAD_URL
        {
            get { return "https://api.liangxiegame.com/qf/v4/package/add"; }
        }

        public static void DoUpload(PackageVersion packageVersion, System.Action succeed)
        {
            EditorUtility.DisplayProgressBar("插件上传", "打包中...", 0.1f);

            var fileName = packageVersion.Name + "_" + packageVersion.Version + ".unitypackage";
            var fullpath = PackageManagerView.ExportPaths(fileName, packageVersion.InstallPath);
            var file = File.ReadAllBytes(fullpath);

            var form = new WWWForm();
            form.AddField("username", User.Username.Value);
            form.AddField("password", User.Password.Value);
            form.AddField("name", packageVersion.Name);
            form.AddField("version", packageVersion.Version);
            form.AddBinaryData("file", file);
            form.AddField("version", packageVersion.Version);
            form.AddField("releaseNote", packageVersion.Readme.content);
            form.AddField("installPath", packageVersion.InstallPath);
            form.AddField("accessRight", packageVersion.AccessRight.ToString().ToLower());
            form.AddField("docUrl", packageVersion.DocUrl);

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
            else if (packageVersion.Type == PackageType.Master)
            {
                form.AddField("type", "master");
            }

            Debug.Log(fullpath);

            EditorUtility.DisplayProgressBar("插件上传", "上传中...", 0.2f);


            ObservableWWW.Post(UPLOAD_URL, form,
                    new Dictionary<string, string> {{"Authorization", "Token " + User.Token.Value}})
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