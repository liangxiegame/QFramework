using System.Collections.Generic;
using System.Linq;

namespace QFramework
{
    public class ResTable : Table<IRes>
    {
        public TableIndex<string, IRes> NameIndex = new TableIndex<string, IRes>(res => res.AssetName.ToLower());

        public IRes GetResBySearchKeys(ResSearchKeys resSearchKeys)
        {
            var assetName = resSearchKeys.AssetName;

            var reses = NameIndex
                .Get(assetName);
            
            if (resSearchKeys.AssetType != null)
            {
                reses = reses.Where(res => res.AssetType == resSearchKeys.AssetType);
            }

            if (resSearchKeys.OwnerBundle != null)
            {
                reses = reses.Where(res => res.OwnerBundleName == resSearchKeys.OwnerBundle);
            }

            return reses.FirstOrDefault();
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