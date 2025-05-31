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
    public class OnCollisionStay2DEventTrigger : MonoBehaviour
    {
        public readonly EasyEvent<Collision2D> OnCollisionStay2DEvent = new EasyEvent<Collision2D>();
        private void OnCollisionStay2D(Collision2D col)
        {
            OnCollisionStay2DEvent.Trigger(col);         
        }
    }

    public static class OnCollisionStay2DEventTriggerExtension
    {
        public static IUnRegister OnCollisionStay2DEvent<T>(this T self, Action<Collision2D> onCollisionStay2D)
            where T : Component
        {
            return self.GetOrAddComponent<OnCollisionStay2DEventTrigger>().OnCollisionStay2DEvent
                .Register(onCollisionStay2D);
        }
        
        public static IUnRegister OnCollisionStay2DEvent(this GameObject self, Action<Collision2D> onCollisionStay2D)
        {
            return self.GetOrAddComponent<OnCollisionStay2DEventTrigger>().OnCollisionStay2DEvent
                .Register(onCollisionStay2D);
        }
    }
}