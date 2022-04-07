/****************************************************************************
 * Copyright (c) 2015 ~ 2022 liangxiegame UNDER MIT LICENSE
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
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
            return PrefabUtility.SaveAsPrefabAssetAndConnect(gameObject,
                assetPrefabPath,
                InteractionMode.AutomatedAction);
        }
    }
}
#endif