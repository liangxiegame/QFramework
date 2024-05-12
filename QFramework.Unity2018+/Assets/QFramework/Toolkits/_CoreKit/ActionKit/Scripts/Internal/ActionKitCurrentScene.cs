/****************************************************************************
 * Copyright (c) 2016 - 2024 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using UnityEngine;

namespace QFramework
{
    public class ActionKitCurrentScene : MonoBehaviour
    {
        public static ActionKitCurrentScene mSceneComponent = null;

        public static ActionKitCurrentScene SceneComponent
        {
            get
            {
                if (!mSceneComponent)
                {
                    mSceneComponent = new GameObject("ActionKitCurrentScene").AddComponent<ActionKitCurrentScene>();
                }

                return mSceneComponent;
            }
        }

        private void Awake() => hideFlags = HideFlags.HideInHierarchy;

        private void OnDestroy() => mSceneComponent = null;
    }
}