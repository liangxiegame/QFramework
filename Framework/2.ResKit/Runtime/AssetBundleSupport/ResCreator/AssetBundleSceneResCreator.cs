namespace QFramework
{
    public class AssetBundleSceneResCreator : IResCreator
    {
        public bool Match(ResSearchKeys resSearchKeys)
        {
            var assetData = ResKit.ResDatas.GetAssetData(resSearchKeys);

            if (assetData != null)
            {
                return assetData.AssetType == ResType.ABScene;
            }

            return false;
        }

        public IRes Create(ResSearchKeys resSearchKeys)
        {
            return AssetBundleSceneRes.Allocate(resSearchKeys.AssetName);
        }
    }
}