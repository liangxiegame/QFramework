using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace QFramework.ResSystem
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
                this.abName = name;
                if (depends == null || depends.Length == 0)
                {

                }
                else
                {
                    this.abDepends = depends;
                }
            }

            public override string ToString()
            {
                string result = string.Format("ABName:" + abName);
                if (abDepends == null)
                {
                    return result;
                }

                for (int i = 0; i < abDepends.Length; ++i)
                {
                    result += string.Format(" #:{0}", abDepends[i]);
                }

                return result;
            }
        }

        [Serializable]
        public class SerializeData
        {
            private string m_Key;
            private ABUnit[] m_ABUnitArray;
            private AssetData[] m_AssetDataArray;

            public string key
            {
                get { return m_Key; }
                set { m_Key = value; }
            }

            public ABUnit[] abUnitArray
            {
                get { return m_ABUnitArray; }
                set { m_ABUnitArray = value; }
            }

            public AssetData[] assetDataArray
            {
                get { return m_AssetDataArray; }
                set { m_AssetDataArray = value; }
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

        public int GetAssetBundleIndex(string name)
        {
            if (mABUnitArray == null)
            {
                return -1;
            }

            for (int i = 0; i < mABUnitArray.Count; ++i)
            {
                if (mABUnitArray[i].abName.Equals(name))
                {
                    return i;
                }
            }
            Log.w("Failed Find AssetBundleIndex By Name:" + name);
            return -1;
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
                    Log.e("Already Add AssetData :{0} \n OldAB:{1}      NewAB:{2}", data.AssetName,
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

                Log.e("Already Add AssetData :{0} \n OldAB:{1}      NewAB:{2}", data.UUID,
                    mABUnitArray[old.AssetBundleIndex].abName, mABUnitArray[data.AssetBundleIndex].abName);
            }
            else
            {
                mUUID4AssetData.Add(data.UUID,data);
            }
            return true;
        }

        public void LoadFromFile(string path)
        {
            object data = SerializeHelper.DeserializeBinary(path);

            if (data == null)
            {
                Log.e("Failed Deserialize AssetDataTable:" + path);
                return;
            }

            SerializeData sd = data as SerializeData;

            if (sd == null)
            {
                Log.e("Failed Load AssetDataTable:" + path);
                return;
            }

            Reset();

            SetSerizlizeData(sd);
        }

        public SerializeData GetSerializeData()
        {
            SerializeData sd = new SerializeData();
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

        public void Save(string outPath)
        {
            SerializeData sd = new SerializeData();
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

            if (SerializeHelper.SerializeBinary(outPath, sd))
            {
                Log.i("Success Save AssetDataTable:" + outPath);
            }
            else
            {
                Log.e("Failed Save AssetDataTable:" + outPath);
            }
        }

        public void Dump()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("#DUMP AssetDataGroup :" + m_Key);

            if (mABUnitArray != null)
            {
                builder.AppendLine(" #DUMP AssetBundleNameArray BEGIN");
                for (int i = 0; i < mABUnitArray.Count; ++i)
                {
                    builder.AppendLine(mABUnitArray[i].ToString());
                }
                builder.AppendLine(" #DUMP AssetBundleNameArray END");
            }

            if (mAssetDataMap != null)
            {
                builder.AppendLine(" #DUMP AssetBundleNameArray BEGIN");
                foreach (var item in mAssetDataMap)
                {
                    builder.AppendLine(item.Key);
                }
                builder.AppendLine(" #DUMP AssetBundleNameArray END");
            }

            builder.AppendLine("#DUMP AssetDataGroup END");

//            Log.i(builder.ToString());
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

                for (int i = 0; i < data.assetDataArray.Length; ++i)
                {
                    AssetData config = data.assetDataArray[i];
                    AddAssetData(config);
                }
            }
        }
    }
}
