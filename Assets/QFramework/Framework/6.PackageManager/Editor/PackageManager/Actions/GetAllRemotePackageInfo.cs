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
            ObservableWWW.Get("http://liangxiegame.com/demo/get_all_demo_info").Subscribe(response =>
            {
                var responseJson = JObject.Parse(response);
                JArray demoInfos = responseJson["all_demo_info"] as JArray;

                var packageDatas = new List<PackageData>();
                foreach (var demoInfo in demoInfos)
                {
                    var name = demoInfo["name"].Value<string>();
                    var version = demoInfo["version"].Value<string>();
                    var url = demoInfo["download_url"].Value<string>();
                    packageDatas.Add(new PackageData()
                    {
                        name = name,
                        version = version,
                        url = url,
                        type = "type"
                    });
                }

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
                return (Application.dataPath + "/QFrameworkData/PackageManager/").CreateDirIfNotExists() +
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