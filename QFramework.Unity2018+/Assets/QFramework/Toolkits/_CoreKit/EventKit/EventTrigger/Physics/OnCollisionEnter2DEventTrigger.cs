/****************************************************************************
 * Copyright (c) 2016 - 2023 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using UnityEngine;

namespace QFramework
{
    public class OnCollisionEnter2DEventTrigger : MonoBehaviour
    {
        public readonly EasyEvent<Collision2D> OnCollisionEnter2DEvent = new EasyEvent<Collision2D>();
        private void OnCollisionEnter2D(Collision2D col)
        {
            OnCollisionEnter2DEvent.Trigger(col);         
        }
    }

    public static class OnCollisionEnter2DEventTriggerExtension
    {
        public static IUnRegister OnCollisionEnter2DEvent<T>(this T self, Action<Collision2D> onCollisionEnter2D)
            where T : Component
        {
            return self.GetOrAddComponent<OnCollisionEnter2DEventTrigger>().OnCollisionEnter2DEvent
                .Register(onCollisionEnter2D);
        }
        
        public static IUnRegister OnCollisionEnter2DEvent(this GameObject self, Action<Collision2D> onCollisionEnter2D)
        {
            return self.GetOrAddComponent<OnCollisionEnter2DEventTrigger>().OnCollisionEnter2DEvent
                .Register(onCollisionEnter2D);
        }
    }
}