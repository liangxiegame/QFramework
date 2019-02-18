/****************************************************************************
 * Copyright (c) 2018.7 ~ 12 liangxie
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

	    public GetAllRemotePackageInfo(Action<List<PackageData>> onGet)
	    {
		    mOnGet = onGet;
	    }

	    protected override void OnBegin()
	    {
		    Dictionary<string, string> headers = null;

		    if (User.Logined)
		    {
			    headers = new Dictionary<string, string>()
			    {
				    {"Authorization", "Token " + User.Token}
			    };
		    }

		    ObservableWWW.Get("http://liangxiegame.com/api/packages/", headers).Subscribe(response =>
		    {
			    var responseJson = JArray.Parse(response);
			    var packageInfosJson = responseJson;

			    var packageDatas = new List<PackageData>();
			    foreach (var packageInfo in packageInfosJson)
			    {
				    var name = packageInfo["name"].Value<string>();

				    var package = packageDatas.Find(packageData => packageData.Name == name);

				    if (package == null)
				    {
					    package = new PackageData()
					    {
						    Name = name,
					    };

					    packageDatas.Add(package);
				    }

				    var id = packageInfo["id"].Value<string>();
				    var version = packageInfo["version"].Value<string>();
				    var url = packageInfo["file"].Value<string>(); // download_url
				    var installPath = packageInfo["install_path"].Value<string>();
				    var releaseNote = packageInfo["release_note"].Value<string>();
				    var createAt = packageInfo["create_at"].Value<string>();
				    var creator = packageInfo["creator"].Value<string>();
				    var docUrl = packageInfo["doc_url"].Value<string>();
				    var releaseItem = new ReleaseItem(version, releaseNote, creator, DateTime.Parse(createAt));
				    var typeName = packageInfo["type"].Value<string>();
				    var accessRightName = packageInfo["access_right"].Value<string>();

				    var packageType = PackageType.FrameworkModule;

				    switch (typeName)
				    {
					    case "fm":
						    packageType = PackageType.FrameworkModule;
						    break;
					    case "s":
						    packageType = PackageType.Shader;
						    break;
					    case "agt":
						    packageType = PackageType.AppOrGameDemoOrTemplate;
						    break;
					    case "p":
						    packageType = PackageType.Plugin;
						    break;
				    }

				    var accessRight = PackageAccessRight.Public;

				    switch (accessRightName)
				    {
					    case "public":
						    accessRight = PackageAccessRight.Public;
						    break;
					    case "private":
						    accessRight = PackageAccessRight.Private;
						    break;

				    }

				    package.PackageVersions.Add(new PackageVersion()
				    {
					    Id = id,
					    Version = version,
					    DownloadUrl = url,
					    InstallPath = installPath,
					    Type = packageType,
					    AccessRight = accessRight,
					    Readme = releaseItem,
					    DocUrl = docUrl,

				    });

				    package.readme.AddReleaseNote(releaseItem);
			    }

			    packageDatas.ForEach(packageData =>
			    {
				    packageData.PackageVersions.Sort((a, b) =>
					    b.VersionNumber - a.VersionNumber);
				    packageData.readme.items.Sort((a, b) =>
					    b.VersionNumber - a.VersionNumber);
			    });

			    mOnGet.InvokeGracefully(packageDatas);

			    new PackageInfosRequestCache()
			    {
				    PackageDatas = packageDatas
			    }.Save();

			    Finished = true;
		    }, e =>
		    {
			    mOnGet.InvokeGracefully(null);
			    Log.E(e);
		    });
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