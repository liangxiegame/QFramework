/****************************************************************************
 * Copyright (c) 2018.7 liangxie
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

namespace QFramework
{
    using System.IO;
    using UniRx;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// 下载最新版本的 Framework
    /// </summary>
    public class DownloadLatestFramework : NodeAction
    {
        private const string URL_GITHUB_API_LATEST_RELEASE =
            "https://api.github.com/repos/liangxiegame/QFramework/releases/latest";

        protected override void OnBegin()
        {
            ObservableWWW.Get(URL_GITHUB_API_LATEST_RELEASE).Subscribe(response =>
            {
                var latestedVersionInfo = FrameworkVersionInfo.ParseLatest(response);
                latestedVersionInfo.Assets.ForEach(asset =>
                {
                    Log.I(asset.Name);
                    if (asset.Name.StartsWith("QFramework"))
                    {
                        var version = asset.Name.Replace("QFramework_v", string.Empty).Replace(".unitypackage", string.Empty);
                        var versionChars = version.Split('.');
                        var versionCode = 0;

                        versionChars.ForEach(versionChar =>
                        {
                            versionCode *= 100;
                            versionCode += versionChar.ToInt();
                        });

                        Log.I(versionCode);

                        // 版本比较
                        ObservableWWW.GetAndGetBytes(asset.BrowserDownloadUrl).Subscribe(bytes =>
                        {
                            File.WriteAllBytes(Application.dataPath + "/" + asset.Name, bytes);
                            AssetDatabase.ImportPackage(Application.dataPath + "/" + asset.Name, true);
                        });
                    }
                });
            }, e => { Log.E(e); });
        }
    }
}