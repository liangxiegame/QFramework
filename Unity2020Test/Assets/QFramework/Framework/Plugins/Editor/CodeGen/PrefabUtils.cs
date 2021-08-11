using UnityEditor;
using UnityEngine;

namespace QFramework
{
    public static class PrefabUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetPrefabPath">Assets/xxx/yyy.prefab</param>
        /// <param name="gameObject"></param>
        public static Object SaveAndConnect(string assetPrefabPath, GameObject gameObject)
        {
#if UNITY_2018_3_OR_NEWER
            var fullPrefabPath = assetPrefabPath.Replace("Assets", Application.dataPath);

            return PrefabUtility.SaveAsPrefabAssetAndConnect(gameObject,
                fullPrefabPath,
                InteractionMode.AutomatedAction);
#else
            return PrefabUtility.CreatePrefab(assetPrefabPath, gameObject, ReplacePrefabOptions.ConnectToPrefab);
#endif
        }
    }
}