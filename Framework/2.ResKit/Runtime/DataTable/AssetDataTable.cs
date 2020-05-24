using System.Collections.Generic;
using System.Linq;

namespace QFramework
{
    public class AssetDataTable : Table<AssetData>
    {
        public TableIndex<string, AssetData> NameIndex = new TableIndex<string, AssetData>(data => data.AssetName);

        public AssetData GetAssetDataByResSearchKeys(ResSearchKeys resSearchKeys)
        {
            var assetName = resSearchKeys.AssetName.ToLower();
            var ownerBundleName = string.IsNullOrEmpty(resSearchKeys.OwnerBundle) ? null :resSearchKeys.OwnerBundle.ToLower();
            
            var assetDatas = NameIndex
                .Get(assetName)
                .Where(r => ownerBundleName == null ||
                            r.OwnerBundleName == ownerBundleName);

            // var retData = assetDatas.FirstOrDefault(r => r.Type == resSearchKeys.AssetType);

            // if (retData == null)
            // {
               var retData = assetDatas.FirstOrDefault();
            // }

            return retData;
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