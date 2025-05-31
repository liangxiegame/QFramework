using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QFramework
{
    public class AssetDataTable : Table<AssetData>
    {
        public TableIndex<string, AssetData> NameIndex = new TableIndex<string, AssetData>(data => data.AssetName);

        public AssetData GetAssetDataByResSearchKeys(ResSearchKeys resSearchKeys)
        {
            var assetName = resSearchKeys.AssetName.ToLower();

            var assetDatas = NameIndex
                .Get(assetName);

            if (resSearchKeys.OwnerBundle != null)
            {
                assetDatas = assetDatas.Where(a => a.OwnerBundleName == resSearchKeys.OwnerBundle);
            }

            if (resSearchKeys.AssetType != null)
            {
                var assetTypeCode = resSearchKeys.AssetType.ToCode();

                if (assetTypeCode == 0)
                {
                }
                else
                {
                    var newAssetDatas = assetDatas.Where(a => a.AssetObjectTypeCode == assetTypeCode);

                    // 有可能是从旧的 AssetBundle 中加载出来的资源
                    if (newAssetDatas.Any())
                    {
                        assetDatas = newAssetDatas;
                    }
                }

            }

            return assetDatas.FirstOrDefault();
        }

        protected override void OnAdd(AssetData item)
        {
            NameIndex.Add(item);
        }

        protected override void OnRemove(AssetData item)
        {
            NameIndex.Remove(item);
        }

        protected override void OnClear()
        {
            NameIndex.Clear();
        }

        public override IEnumerator<AssetData> GetEnumerator()
        {
            return NameIndex.Dictionary.SelectMany(kv => kv.Value).GetEnumerator();
        }

        protected override void OnDispose()
        {
            NameIndex.Dispose();
        }
    }
}