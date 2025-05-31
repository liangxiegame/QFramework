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
    public class OnTriggerStayEventTrigger : MonoBehaviour
    {
        public readonly EasyEvent<Collider> OnTriggerStayEvent = new EasyEvent<Collider>();
        private void OnTriggerStay(Collider collider)
        {
            OnTriggerStayEvent.Trigger(collider);         
        }
    }

    public static class OnTriggerStayEventTriggerExtension
    {
        public static IUnRegister OnTriggerStayEvent<T>(this T self, Action<Collider> onTriggerStay)
            where T : Component
        {
            return self.GetOrAddComponent<OnTriggerStayEventTrigger>().OnTriggerStayEvent
                .Register(onTriggerStay);
        }
        
        public static IUnRegister OnTriggerStayEvent(this GameObject self, Action<Collider> onTriggerStay)
        {
            return self.GetOrAddComponent<OnTriggerStayEventTrigger>().OnTriggerStayEvent
                .Register(onTriggerStay);
        }
    }
}