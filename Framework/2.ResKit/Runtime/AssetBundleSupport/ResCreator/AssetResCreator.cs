namespace QFramework
{
    public class AssetResCreator : IResCreator
    {
        public bool Match(ResSearchKeys resSearchKeys)
        {
            var assetData = ResKit.ResDatas.GetAssetData(resSearchKeys);

            if (assetData != null)
            {
                return assetData.AssetType == ResType.ABAsset;
            }

            return false;
        }

        public IRes Create(ResSearchKeys resSearchKeys)
        {
            return AssetRes.Allocate(resSearchKeys.AssetName, resSearchKeys.OwnerBundle, resSearchKeys.AssetType);
        }
    }
}