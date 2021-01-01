using System;

namespace QFramework
{
    public class ResSearchKeys : IPoolable,IPoolType
    {   
        public string AssetName { get; set; }

        public string OwnerBundle { get;  set; }

        public Type AssetType { get; set; }

        public static ResSearchKeys Allocate(string assetName, string ownerBundleName = null, Type assetType = null)
        {
            var resSearchRule = SafeObjectPool<ResSearchKeys>.Instance.Allocate();
            resSearchRule.AssetName = assetName.ToLower();
            resSearchRule.OwnerBundle = ownerBundleName == null ? null : ownerBundleName.ToLower();
            resSearchRule.AssetType = assetType;
            return resSearchRule;
        }
        
        // public static ResSearchKeys Allocate(IRes res)
        // {
        //     var resSearchRule = SafeObjectPool<ResSearchKeys>.Instance.Allocate();
        //     res.FillInfo2ResSearchKeys(resSearchRule);
        //     return resSearchRule;
        // }
        
        public void Recycle2Cache()
        {
            SafeObjectPool<ResSearchKeys>.Instance.Recycle(this);
        }

        public bool Match(IRes res)
        {
            if (res.AssetName == AssetName)
            {
                var isMatch = true;

                if (AssetType != null)
                {
                    isMatch = res.AssetType == AssetType;
                }

                if (OwnerBundle != null)
                {
                    isMatch = isMatch && res.OwnerBundleName == OwnerBundle;
                }
                 
                return isMatch;
            }
            

            return false;
        }

        public override string ToString()
        {
            return string.Format("AssetName:{0} BundleName:{1} TypeName:{2}", AssetName, OwnerBundle,
                AssetType);
        }

        void IPoolable.OnRecycled()
        {
            AssetName = null;

            OwnerBundle = null;

            AssetType = null;
        }

        bool IPoolable.IsRecycled { get; set; }
    }
}