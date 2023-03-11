namespace QFramework
{
    public class ResourcesResCreator : IResCreator
    {
        public bool Match(ResSearchKeys resSearchKeys)
        {
            return resSearchKeys.AssetName.StartsWith("resources/") ||
                   resSearchKeys.AssetName.StartsWith("resources://");
        }

        public IRes Create(ResSearchKeys resSearchKeys)
        {
            var resourcesRes = ResourcesRes.Allocate(resSearchKeys.AssetName,
                resSearchKeys.AssetName.StartsWith("resources://")
                    ? InternalResNamePrefixType.Url
                    : InternalResNamePrefixType.Folder);
            resourcesRes.AssetType = resSearchKeys.AssetType;
            return resourcesRes;
        }
    }
}