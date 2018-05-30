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
using UnityEngine;

namespace QFramework
{
    public class ResDatas : QSingleton<ResDatas>
    {
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

        private List<AssetDataGroup> m_ActiveAssetDataGroup = new List<AssetDataGroup>();
        private List<AssetDataGroup> m_AllAssetDataGroup = new List<AssetDataGroup>();

        public void SwitchLanguage(string key)
        {
            m_ActiveAssetDataGroup.Clear();

            var languageKey = string.Format("[{0}]", key);

            for (var i = m_AllAssetDataGroup.Count - 1; i >= 0; --i)
            {
                var group = m_AllAssetDataGroup[i];

                if (!group.key.Contains("i18res"))
                {
                    m_ActiveAssetDataGroup.Add(group);
                }
                else if (group.key.Contains(languageKey))
                {
                    m_ActiveAssetDataGroup.Add(group);
                }

            }
            Log.I("AssetDataTable Switch 2 Language:" + key);
        }

        public static ResDatas Create()
        {
            return Instance;
        }
        
        private ResDatas(){}

        public void Reset()
        {
            for (int i = m_AllAssetDataGroup.Count - 1; i >= 0; --i)
            {
                m_AllAssetDataGroup[i].Reset();
            }

            m_AllAssetDataGroup.Clear();
            m_ActiveAssetDataGroup.Clear();
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
                m_AllAssetDataGroup.Add(group);
            }

            return group.AddAssetBundleName(name, depends);
        }

        public string GetAssetBundleName(string assetName, int index,string onwerBundleName)
        {
            for (var i = m_ActiveAssetDataGroup.Count - 1; i >= 0; --i)
            {
                string result;
                if (!m_ActiveAssetDataGroup[i].GetAssetBundleName(assetName, index, out result))
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
			var abName = FrameworkSettingData.AssetBundleUrl2Name(url);
            //var a = new AssetBundleManifest();
            
            for (var i = m_ActiveAssetDataGroup.Count - 1; i >= 0; --i)
            {
                string[] depends;
                if (!m_ActiveAssetDataGroup[i].GetAssetBundleDepends(abName, out depends))
                {
                    continue;
                }

                return depends;
            }

            return null;
        }
        
        public AssetData GetAssetData(string assetName)
        {
            for (int i = m_ActiveAssetDataGroup.Count - 1; i >= 0; --i)
            {
                AssetData result = m_ActiveAssetDataGroup[i].GetAssetData(assetName);
                if (result == null)
                {
                    continue;
                }
                return result;
            }
            //Log.W(string.Format("Not Find Asset : {0}", assetName));
            return null;
        }

        public AssetData GetAssetData(string assetName,string ownerBundle)
        {
            for (int i = m_ActiveAssetDataGroup.Count - 1; i >= 0; --i)
            {
                AssetData result = m_ActiveAssetDataGroup[i].GetAssetData(assetName,ownerBundle);
                if (result == null)
                {
                    continue;
                }
                return result;
            }
            //Log.W(string.Format("Not Find Asset : {0}", assetName));
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
            SerializeData sd = new SerializeData();

            sd.AssetDataGroup = new AssetDataGroup.SerializeData[m_AllAssetDataGroup.Count];

            for (int i = 0; i < m_AllAssetDataGroup.Count; ++i)
            {
                sd.AssetDataGroup[i] = m_AllAssetDataGroup[i].GetSerializeData();
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
                m_AllAssetDataGroup.Add(BuildAssetDataGroup(data.AssetDataGroup[i]));
            }
        }

        private AssetDataGroup BuildAssetDataGroup(AssetDataGroup.SerializeData data)
        {
            return new AssetDataGroup(data);
        }

        private AssetDataGroup GetAssetDataGroup(string key)
        {
            for (int i = m_AllAssetDataGroup.Count - 1; i >= 0; --i)
            {
                if (m_AllAssetDataGroup[i].key.Equals(key))
                {
                    return m_AllAssetDataGroup[i];
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
