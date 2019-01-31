using UnityEngine;

namespace QFramework
{
    public interface IModeLogic
    {
        Object LoadAsset(string assetBundleName, string assetName);
    }
}