/****************************************************************************
 * Copyright (c) 2015 - 2023 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using UnityEngine;

namespace QFramework
{
    public class OnBecameInvisibleEventTrigger : MonoBehaviour
    {
        public readonly EasyEvent OnBecameInvisibleEvent = new EasyEvent();

        private void OnBecameInvisible()
        {
            OnBecameInvisibleEvent.Trigger();
        }
    }

    public static class OnBecameInvisibleEventTriggerExtension
    {
        public static IUnRegister OnBecameInvisibleEvent<T>(this T self, Action onBecameInvisible)
            where T : Component
        {
            return self.GetOrAddComponent<OnBecameInvisibleEventTrigger>().OnBecameInvisibleEvent
                .Register(onBecameInvisible);
        }

        public static IUnRegister OnBecameInvisibleEvent(this GameObject self, Action onBecameInvisible)
        {
            return self.GetOrAddComponent<OnBecameInvisibleEventTrigger>().OnBecameInvisibleEvent
                .Register(onBecameInvisible);
        }
    }
}