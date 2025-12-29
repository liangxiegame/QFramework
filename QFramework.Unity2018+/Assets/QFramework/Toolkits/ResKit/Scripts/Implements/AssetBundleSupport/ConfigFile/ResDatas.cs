/****************************************************************************
 * Copyright (c) 2016 ~ 2024 liangxiegame UNDER MIT LINCENSE
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace QFramework
{
    /// <summary>
    /// 默认的 ResData 支持
    /// </summary>
    public class ResDatas : IResDatas
    {
        public string AESKey = string.Empty;


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

        /// <summary>
        /// 如果是以前的命名错误版本，大家可以通过设置 ResDatas.FileName = "asset_bindle_config.bin" 来兼容以前的代码; 
        /// </summary>
        public static string FileName = "asset_bundle_config.bin";

        public IList<AssetDataGroup> AllAssetDataGroups => mAllAssetDataGroup;

        private readonly List<AssetDataGroup> mAllAssetDataGroup = new List<AssetDataGroup>();

        private AssetDataTable mAssetDataTable = null;

        public void Reset()
        {
            for (int i = mAllAssetDataGroup.Count - 1; i >= 0; --i)
            {
                mAllAssetDataGroup[i].Reset();
            }

            mAllAssetDataGroup.Clear();

            if (mAssetDataTable != null)
            {
                mAssetDataTable.Dispose();
            }

            mAssetDataTable = null;
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
                Debug.Log("#Create Config Group:" + key);
                mAllAssetDataGroup.Add(group);
            }

            return group.AddAssetBundleName(name, depends);
        }

        public string GetABHash(string assetName)
        {
            foreach (var assetDataGroup in mAllAssetDataGroup)
            {
                var abUnit = assetDataGroup.GetABUnit(assetName);

                if (abUnit != null)
                {
                    return abUnit.Hash;
                }
            }

            return null;
        }

        public string[] GetAllDependenciesByUrl(string url)
        {
            var abName = AssetBundleSettings.AssetBundleUrl2Name(url);

            for (var i = mAllAssetDataGroup.Count - 1; i >= 0; --i)
            {
                string[] depends;
                if (!mAllAssetDataGroup[i].GetAssetBundleDepends(abName, out depends))
                {
                    continue;
                }

                return depends;
            }

            return null;
        }


        public AssetData GetAssetData(ResSearchKeys resSearchKeys)
        {
            if (mAssetDataTable == null)
            {
                mAssetDataTable = new AssetDataTable();

                for (var i = mAllAssetDataGroup.Count - 1; i >= 0; --i)
                {
                    foreach (var assetData in mAllAssetDataGroup[i].AssetDatas)
                    {
                        mAssetDataTable.Add(assetData);
                    }
                }
            }

            return mAssetDataTable.GetAssetDataByResSearchKeys(resSearchKeys);
        }

        public void LoadFromFile(string path)
        {
            var binarySerializer = Architecture.BinarySerializer;
            var zipFileHelper = Architecture.ZipFileHelper;
            
            object data;

           //  if (File.ReadAllText(path).Contains(AES.AESHead))
           //  {
           //      if (AESKey == string.Empty)
           //      {
           //         AESKey=JsonUtility.FromJson<EncryptConfig>( Resources.Load<TextAsset>("EncryptConfig").text).AESKey;
           //      }
           //      data = binarySerializer
           // .DeserializeBinary((AES.AESFileByteDecrypt(path, AESKey)));
           //      //try
           //      //{
           //
           //      //}
           //      //catch (Exception e)
           //      //{
           //      //    Log.E("解密AB包失败,请检查秘钥!!当前使用的秘钥:" + AESKey);
           //      //}
           //
           //  }
           //  else
           //  {
                data = binarySerializer
           .DeserializeBinary(zipFileHelper.OpenReadStream(path));
            // }


            Debug.Log(path);

            if (data == null)
            {
                Debug.LogError("Failed Deserialize AssetDataTable:" + path);
                return;
            }

            var sd = data as SerializeData;

            if (sd == null)
            {
                Debug.LogError("Failed Load AssetDataTable:" + path);
                return;
            }


            SetSerializeData(sd);
        }


        public IEnumerator LoadFromFileAsync(string path)
        {
#pragma warning disable CS0618
            using (var www = new WWW(path))
#pragma warning restore CS0618
            {
                yield return www;

                if (www.error != null)
                {
                    Debug.LogError("Failed Deserialize AssetDataTable:" + path + " Error:" + www.error);
                    yield break;
                }

                var stream = new MemoryStream(www.bytes);

                var data = Architecture.BinarySerializer
                    .DeserializeBinary(stream);

                if (data == null)
                {
                    Debug.LogError("Failed Deserialize AssetDataTable:" + path);
                    yield break;
                }

                var sd = data as SerializeData;

                if (sd == null)
                {
                    Debug.LogError("Failed Load AssetDataTable:" + path);
                    yield break;
                }

                Debug.Log("Load AssetConfig From File:" + path);
                SetSerializeData(sd);
            }
        }

        public virtual void Save(string outPath)
        {
            var sd = new SerializeData
            {
                AssetDataGroup = new AssetDataGroup.SerializeData[mAllAssetDataGroup.Count]
            };

            for (var i = 0; i < mAllAssetDataGroup.Count; ++i)
            {
                sd.AssetDataGroup[i] = mAllAssetDataGroup[i].GetSerializeData();
            }

            if (Architecture.BinarySerializer
                .SerializeBinary(outPath, sd))
            {
                Debug.Log("Success Save AssetDataTable:" + outPath);
            }
            else
            {
                Debug.LogError("Failed Save AssetDataTable:" + outPath);
            }
        }

        private void SetSerializeData(SerializeData data)
        {

            if (data == null || data.AssetDataGroup == null)
            {
                return;
            }

            for (int i = data.AssetDataGroup.Length - 1; i >= 0; --i)
            {
                mAllAssetDataGroup.Add(BuildAssetDataGroup(data.AssetDataGroup[i]));
            }

            if (mAssetDataTable == null)
            {
                mAssetDataTable = new AssetDataTable();

                foreach (var serializeData in data.AssetDataGroup)
                {
                    foreach (var assetData in serializeData.assetDataArray)
                    {
                        mAssetDataTable.Add(assetData);
                    }
                }
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
            var pIndex = name.IndexOf('/');

            if (pIndex < 0)
            {
                return name;
            }

            var key = name.Substring(0, pIndex);

            return key;
        }
    }
}