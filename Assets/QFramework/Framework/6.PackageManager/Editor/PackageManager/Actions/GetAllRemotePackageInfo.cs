/****************************************************************************
 * Copyright (c) 2018.7 ~ 11 liangxie
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

using System.IO;
using UnityEngine;

namespace QFramework
{
    using System;
    using UniRx;
    using System.Collections.Generic;
    using Newtonsoft.Json.Linq;
    
    public class GetAllRemotePackageInfo : NodeAction
    {
        private Action<List<PackageData>> mOnGet;
        
        public GetAllRemotePackageInfo(Action <List<PackageData>> onGet)
        {
            mOnGet = onGet;
        }

        protected override void OnBegin()
		{
			ObservableWWW.Get ("http://liangxiegame.com/framework_package/get_all_package_info").Subscribe (response =>
			{
				var responseJson = JObject.Parse (response);
				JArray packageInfosJson = responseJson ["all_package_info"] as JArray;

				var packageDatas = new List<PackageData> ();
				foreach (var demoInfo in packageInfosJson)
				{
					var name = demoInfo ["name"].Value<string> ();
                    
					var package = packageDatas.Find (packageData => packageData.Name == name);

					if (package == null)
					{
						package = new PackageData () {
							Name = name,
						};
                        
						packageDatas.Add (package);
					}
                                       
					var version = demoInfo ["version"].Value<string> ();
					var url = demoInfo ["download_url"].Value<string> ();
					var installPath = demoInfo ["install_path"].Value<string> ();
					var release_note = demoInfo["release_note"].Value<string>();

					var releaseItem = new ReleaseItem(version, release_note, "liangxie", "now");
					
					package.PackageVersions.Add (new PackageVersion () {
						Version = version,
						DownloadUrl = url,
						InstallPath = installPath,
						Readme =releaseItem
					});
					
					package.readme.AddReleaseNote(releaseItem);
				}

				packageDatas.ForEach (packageData =>
				{
					packageData.PackageVersions.Sort((a, b) =>
							GetVersionNumber(b.Version) - GetVersionNumber(a.Version));
					packageData.readme.items.Sort((a, b) =>
						GetVersionNumber(b.version) - GetVersionNumber(a.version));
				});
                
				mOnGet.InvokeGracefully (packageDatas);

				new PackageInfosRequestCache () {
					PackageDatas = packageDatas
				}.Save ();
                
				Finished = true;
			}, e =>
			{
				mOnGet.InvokeGracefully (null);
				Log.E (e);
			});
		}

        public static int GetVersionNumber(string version)
        {
            return int.Parse(version.Replace("v", string.Empty).Replace(".", ""));
        }
	}

    public class PackageInfosRequestCache
    {
        public List<PackageData> PackageDatas = new List<PackageData>();

        private static string mFilePath
        {
            get
            {
                return (Application.dataPath + "/.qframework/PackageManager/").CreateDirIfNotExists() +
                       "PackageInfosRequestCache.json";
            }
        }
        
        public static PackageInfosRequestCache Get()
        {
            if (File.Exists(mFilePath))
            {
                return SerializeHelper.LoadJson<PackageInfosRequestCache>(mFilePath);
            }
            
            return new PackageInfosRequestCache();
        }

        public void Save()
        {
            this.SaveJson(mFilePath);
        }
    }
}