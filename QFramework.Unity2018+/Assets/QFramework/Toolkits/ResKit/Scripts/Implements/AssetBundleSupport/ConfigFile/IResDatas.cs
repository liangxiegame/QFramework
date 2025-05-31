using System.Collections;

namespace QFramework
{
    public interface IResDatas
    {
        string[] GetAllDependenciesByUrl(string url);

        void LoadFromFile(string outRes);
        void Reset();
        IEnumerator LoadFromFileAsync(string outRes);
        AssetData GetAssetData(ResSearchKeys resSearchKeys);
        int AddAssetBundleName(string abName, string[] depends, out AssetDataGroup @group);
        string GetABHash(string assetName);
    }
}