namespace QFramework
{
    public class AssetResCreator : IResCreator
    {
        public bool Match(ResSearchKeys resSearchKeys)
        {
            var assetData =  AssetBundleSettings.AssetBundleConfigFile.GetAssetData(resSearchKeys);

            if (assetData != null)
            {
                return assetData.AssetType == ResLoadType.ABAsset;
            }
            
            foreach (var subProjectAssetBundleConfigFile in AssetBundleSettings.SubProjectAssetBundleConfigFiles)
            {
                assetData = subProjectAssetBundleConfigFile.GetAssetData(resSearchKeys);
                
                if (assetData != null)
                {
                    return assetData.AssetType == ResLoadType.ABAsset;
                }
            }

            return false;
        }

        public IRes Create(ResSearchKeys resSearchKeys)
        {
            return AssetRes.Allocate(resSearchKeys.AssetName, resSearchKeys.OwnerBundle, resSearchKeys.AssetType);
        }
    }
}