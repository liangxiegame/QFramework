/****************************************************************************
 * Copyright (c) 2018.3 liangxie
 * 
 * http://liangxiegame.com
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
using System.Collections.Generic;
using QFramework;

namespace QF.Res
{
    public class ResDatas : Singleton<ResDatas>
    {
        public IList<AssetDataGroup> AllAssetDataGroups
        {
            get { return mAllAssetDataGroup; }
        }

        [Serializable]
        public class SerializeData
        {
            private AssetDataGroup.SerializeData[] mAssetDataGroup;

            public AssetDataGroup.SerializeData[] AssetDataGroup
            {
                get { return mAssetDataGroup; }
                set { mAssetDataGroup = value; }
            }
        }

        private readonly List<AssetDataGroup> mActiveAssetDataGroup = new List<AssetDataGroup>();
        private readonly List<AssetDataGroup> mAllAssetDataGroup = new List<AssetDataGroup>();

        public void SwitchLanguage(string key)
        {
            mActiveAssetDataGroup.Clear();

            var languageKey = string.Format("[{0}]", key);

            for (var i = mAllAssetDataGroup.Count - 1; i >= 0; --i)
            {
                var group = mAllAssetDataGroup[i];

                if (!group.key.Contains("i18res"))
                {
                    mActiveAssetDataGroup.Add(group);
                }
                else if (group.key.Contains(languageKey))
                {
                    mActiveAssetDataGroup.Add(group);
                }

            }
            Log.I("AssetDataTable Switch 2 Language:" + key);
        }

        public static ResDatas Create()
        {
            if (Instance != null)
            {
                Instance.Dispose();
            }
            
            return Instance;
        }
        
        private ResDatas(){}

        public void Reset()
        {
            for (int i = mAllAssetDataGroup.Count - 1; i >= 0; --i)
            {
                mAllAssetDataGroup[i].Reset();
            }

            mAllAssetDataGroup.Clear();
            mActiveAssetDataGroup.Clear();
        }

        public int AddAssetBundleName(string name, string[] depends, out AssetDataGroup group)
        {
            group = null;

            if (string.IsNullOrEmpty(name))
            {
                return -1;
            }

            var key = GetKeyFromABName(name);

            if (key == null)
            {
                return -1;
            }

            group = GetAssetDataGroup(key);

            if (group == null)
            {
                group = new AssetDataGroup(key);
                Log.I("#Create Config Group:" + key);
                mAllAssetDataGroup.Add(group);
            }

            return group.AddAssetBundleName(name, depends);
        }

        public string GetAssetBundleName(string assetName, int index,string onwerBundleName)
        {
            for (var i = mActiveAssetDataGroup.Count - 1; i >= 0; --i)
            {
                string result;
                if (!mActiveAssetDataGroup[i].GetAssetBundleName(assetName, index, out result))
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(onwerBundleName) && !result.Equals(onwerBundleName))
                {
                    continue;
                }

                return result;
            }
            Log.W(string.Format("Failed GetAssetBundleName : {0} - Index:{1}", assetName, index));
            return null;
        }

        public string[] GetAllDependenciesByUrl(string url)
        {
			var abName = ResKitUtil.AssetBundleUrl2Name(url);
            
            for (var i = mActiveAssetDataGroup.Count - 1; i >= 0; --i)
            {
                string[] depends;
                if (!mActiveAssetDataGroup[i].GetAssetBundleDepends(abName, out depends))
                {
                    continue;
                }

                return depends;
            }

            return null;
        }
        

        public AssetData GetAssetData(ResSearchRule resSearchRule)
        {
            for (var i = mActiveAssetDataGroup.Count - 1; i >= 0; --i)
            {
                var result = mActiveAssetDataGroup[i].GetAssetData(resSearchRule);
                if (result == null)
                {
                    continue;
                }
                return result;
            }

            return null;
        }

        public void LoadFromFile(string path)
        {
			var data = SerializeHelper.DeserializeBinary(FileMgr.Instance.OpenReadStream(path));

            if (data == null)
            {
                Log.E("Failed Deserialize AssetDataTable:" + path);
                return;
            }

            var sd = data as SerializeData;

            if (sd == null)
            {
                Log.E("Failed Load AssetDataTable:" + path);
                return;
            }

            Log.I("Load AssetConfig From File:" + path);
            SetSerizlizeData(sd);
        }

        public void Save(string outPath)
        {
            SerializeData sd = new SerializeData
            {
                AssetDataGroup = new AssetDataGroup.SerializeData[mAllAssetDataGroup.Count]
            };

            for (var i = 0; i < mAllAssetDataGroup.Count; ++i)
            {
                sd.AssetDataGroup[i] = mAllAssetDataGroup[i].GetSerializeData();
            }

            if (SerializeHelper.SerializeBinary(outPath, sd))
            {
                Log.I("Success Save AssetDataTable:" + outPath);
            }
            else
            {
                Log.E("Failed Save AssetDataTable:" + outPath);
            }
        }

        private void SetSerizlizeData(SerializeData data)
        {
            if (data == null || data.AssetDataGroup == null)
            {
                return;
            }

            for (int i = data.AssetDataGroup.Length - 1; i >= 0; --i)
            {
                mAllAssetDataGroup.Add(BuildAssetDataGroup(data.AssetDataGroup[i]));
            }
        }

        private AssetDataGroup BuildAssetDataGroup(AssetDataGroup.SerializeData data)
        {
            return new AssetDataGroup(data);
        }

        private AssetDataGroup GetAssetDataGroup(string key)
        {
            for (int i = mAllAssetDataGroup.Count - 1; i >= 0; --i)
            {
                if (mAllAssetDataGroup[i].key.Equals(key))
                {
                    return mAllAssetDataGroup[i];
                }
            }

            return null;
        }

        private static string GetKeyFromABName(string name)
        {
            int pIndex = name.IndexOf('/');

            if (pIndex < 0)
            {
                return name;
            }

            string key = name.Substring(0, pIndex);

            if (name.Contains("i18res"))
            {
                int i18Start = name.IndexOf("i18res") + 7;
                name = name.Substring(i18Start);
                pIndex = name.IndexOf('/');
                if (pIndex < 0)
                {
                    Log.W("Not Valid AB Path:" + name);
                    return null;
                }

                string language = string.Format("[{0}]", name.Substring(0, pIndex));
                key = string.Format("{0}-i18res-{1}", key, language);
            }

            return key;
        }

    }
}
