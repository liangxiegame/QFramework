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
using UnityEngine;
using System.Collections.Generic;
using System.Text;

namespace QFramework
{
    public class AssetDataGroup
    {
        /// <summary>
        /// 代表依赖关系的类
        /// </summary>
        [Serializable]
        public class ABUnit
        {
            public string abName;
            public string[] abDepends;

            public ABUnit(string name, string[] depends)
            {
                abName = name;
                if (depends == null || depends.Length == 0)
                {

                }
                else
                {
                    abDepends = depends;
                }
            }

            public override string ToString()
            {
                var result = string.Format("ABName:" + abName);
                if (abDepends == null)
                {
                    return result;
                }

                foreach (var abDepend in abDepends)
                {
                    result += string.Format(" #:{0}", abDepend);
                }

                return result;
            }
        }

        [Serializable]
        public class SerializeData
        {
            private string mKey;
            private ABUnit[] mAbUnitArray;
            private AssetData[] mAssetDataArray;

            public string key
            {
                get { return mKey; }
                set { mKey = value; }
            }

            public ABUnit[] abUnitArray
            {
                get { return mAbUnitArray; }
                set { mAbUnitArray = value; }
            }

            public AssetData[] assetDataArray
            {
                get { return mAssetDataArray; }
                set { mAssetDataArray = value; }
            }
        }

        private string m_Key;

        private List<ABUnit> mABUnitArray;
        private Dictionary<string, AssetData> mAssetDataMap;
        private Dictionary<string, AssetData> mUUID4AssetData;
        public string key
        {
            get { return m_Key; }
        }

        public AssetDataGroup(string key)
        {
            m_Key = key;
        }

        public AssetDataGroup(SerializeData data)
        {
            m_Key = data.key;
            SetSerizlizeData(data);
        }

        public void Reset()
        {
            if (mABUnitArray != null)
            {
                mABUnitArray.Clear();
            }

            if (mAssetDataMap != null)
            {
                mAssetDataMap.Clear();
            }
        }

        public int AddAssetBundleName(string name, string[] depends)
        {
            if (string.IsNullOrEmpty(name))
            {
                return -1;
            }

            if (mABUnitArray == null)
            {
                mABUnitArray = new List<ABUnit>();
            }

            AssetData config = GetAssetData(name);

            if (config != null)
            {
                return config.AssetBundleIndex;
            }

            mABUnitArray.Add(new ABUnit(name, depends));

            int index = mABUnitArray.Count - 1;

            AddAssetData(new AssetData(name, ResType.AssetBundle, index,null));

            return index;
        }

        public bool GetAssetBundleName(string assetName, int index, out string result)
        {
            result = null;

            if (mABUnitArray == null)
            {
                return false;
            }

            if (index >= mABUnitArray.Count)
            {
                return false;
            }

            if (mAssetDataMap.ContainsKey(assetName))
            {
                result = mABUnitArray[index].abName;
                return true;
            }

            return false;
        }

        public ABUnit GetABUnit(string assetName)
        {
            AssetData data = GetAssetData(assetName);

            if (data == null)
            {
                return null;
            }

            if (mABUnitArray == null)
            {
                return null;
            }

            return mABUnitArray[data.AssetBundleIndex];
        }

        public bool GetAssetBundleDepends(string abName, out string[] result)
        {
            result = null;

            ABUnit unit = GetABUnit(abName);

            if (unit == null)
            {
                return false;
            }

            result = unit.abDepends;

            return true;
        }
        
        public AssetData GetAssetData(string assetName)
        {
            if (mAssetDataMap == null)
            {
                return null;
            }

            string key = assetName.ToLower() ;

            AssetData result = null;
            if (mAssetDataMap.TryGetValue(key, out result))
            {
                return result;
            }

            return null;
        }
        

        public AssetData GetAssetData(string assetName,string ownerBundle)
        {
            if (mUUID4AssetData == null)
            {
                return null;
            }

            string uuid = (ownerBundle + assetName).ToLower();

            AssetData result = null;
            if (mUUID4AssetData.TryGetValue(uuid, out result))
            {
                return result;
            }

            return null;
        }

        public bool AddAssetData(AssetData data)
        {
            if (mAssetDataMap == null)
            {
                mAssetDataMap = new Dictionary<string, AssetData>();
            }

            if (mUUID4AssetData == null)
            {
                mUUID4AssetData = new Dictionary<string, AssetData>();
            }
 
            string key = data.AssetName.ToLower();

            if (mAssetDataMap.ContainsKey(key))
            {
                AssetData old = GetAssetData(data.AssetName, null);

                try
                {
                    Log.E("Already Add AssetData :{0} \n OldAB:{1}      NewAB:{2}", data.AssetName,
                        mABUnitArray[old.AssetBundleIndex].abName, mABUnitArray[data.AssetBundleIndex].abName);
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                }
            }
            else
            {
                mAssetDataMap.Add(key, data);
            }

            if (mUUID4AssetData.ContainsKey(data.UUID))
            {
                AssetData old = GetAssetData(data.AssetName, data.OwnerBundleName);

                Log.E("Already Add AssetData :{0} \n OldAB:{1}      NewAB:{2}", data.UUID,
                    mABUnitArray[old.AssetBundleIndex].abName, mABUnitArray[data.AssetBundleIndex].abName);
            }
            else
            {
                mUUID4AssetData.Add(data.UUID,data);
            }
            return true;
        }

        public SerializeData GetSerializeData()
        {
            var sd = new SerializeData();
            sd.key = m_Key;
            sd.abUnitArray = mABUnitArray.ToArray();
            if (mAssetDataMap != null)
            {
                AssetData[] acArray = new AssetData[mAssetDataMap.Count];

                int index = 0;
                foreach (var item in mAssetDataMap)
                {
                    acArray[index++] = item.Value;
                }

                sd.assetDataArray = acArray;
            }

            return sd;
        }


        private void SetSerizlizeData(SerializeData data)
        {
            if (data == null)
            {
                return;
            }

            mABUnitArray = new List<ABUnit>(data.abUnitArray);

            if (data.assetDataArray != null)
            {
                mAssetDataMap = new Dictionary<string, AssetData>();

                foreach (var config in data.assetDataArray)
                {
                    AddAssetData(config);
                }
            }
        }
    }
}
