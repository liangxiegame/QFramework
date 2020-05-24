using System.Collections.Generic;
using System.Linq;

namespace QFramework
{
    public class ResTable : Table<IRes>
    {
        public TableIndex<string, IRes> NameIndex = new TableIndex<string, IRes>(res => res.AssetName);

        public IRes GetResBySearchKeys(ResSearchKeys resSearchKeys)
        {
            var assetName = resSearchKeys.AssetName.ToLower();
            var ownerBundleName = string.IsNullOrEmpty(resSearchKeys.OwnerBundle) ? null :resSearchKeys.OwnerBundle.ToLower();
            
            var reses = NameIndex
                .Get(assetName)
                .Where(r => ownerBundleName == null ||
                            r.OwnerBundleName == ownerBundleName);

            var retData = reses.FirstOrDefault(r => r.AssetType == resSearchKeys.AssetType);

            if (retData == null)
            {
                retData = reses.FirstOrDefault();
            }

            return retData;
        }
        
        protected override void OnAdd(IRes item)
        {
            NameIndex.Add(item);
        }

        protected override void OnRemove(IRes item)
        {
            NameIndex.Remove(item);
        }

        protected override void OnClear()
        {
            NameIndex.Clear();
        }

        public override IEnumerator<IRes> GetEnumerator()
        {
            return NameIndex.Dictionary.SelectMany(d => d.Value)
                .GetEnumerator();
        }

        protected override void OnDispose()
        {
        }
    }
}