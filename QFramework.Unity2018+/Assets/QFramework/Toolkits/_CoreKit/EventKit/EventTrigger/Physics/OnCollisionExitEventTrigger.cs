/****************************************************************************
 * Copyright (c) 2016 - 2022 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using UnityEngine;

namespace QFramework
{
    public class OnCollisionExitEventTrigger : MonoBehaviour
    {
        public readonly EasyEvent<Collision> OnCollisionExitEvent = new EasyEvent<Collision>();
        private void OnCollisionExit(Collision col)
        {
            OnCollisionExitEvent.Trigger(col);         
        }
    }

    public static class OnCollisionExitEventTriggerExtension
    {
        public static IUnRegister OnCollisionExitEvent<T>(this T self, Action<Collision> onCollisionExit)
            where T : Component
        {
            return self.GetOrAddComponent<OnCollisionExitEventTrigger>().OnCollisionExitEvent
                .Register(onCollisionExit);
        }
        
        public static IUnRegister OnCollisionExitEvent(this GameObject self, Action<Collision> onCollisionExit)
        {
            return self.GetOrAddComponent<OnCollisionExitEventTrigger>().OnCollisionExitEvent
                .Register(onCollisionExit);
        }
    }
}