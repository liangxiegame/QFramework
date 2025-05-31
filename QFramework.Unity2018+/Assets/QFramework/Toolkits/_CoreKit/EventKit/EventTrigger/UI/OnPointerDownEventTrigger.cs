/****************************************************************************
 * Copyright (c) 2016 - 2023 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace QFramework
{
    public class OnPointerDownEventTrigger : MonoBehaviour,IPointerDownHandler
    {
        public readonly EasyEvent<PointerEventData> OnPointerDownEvent = new EasyEvent<PointerEventData>();

        public void OnPointerDown(PointerEventData eventData)
        {
            OnPointerDownEvent.Trigger(eventData);
        }
    }

    public static class OnPointerDownEventTriggerExtension
    {
        public static IUnRegister OnPointerDownEvent<T>(this T self, Action<PointerEventData> onPointerDownEvent)
            where T : Component
        {
            return self.GetOrAddComponent<OnPointerDownEventTrigger>().OnPointerDownEvent
                .Register(onPointerDownEvent);
        }
        
        public static IUnRegister OnPointerDownEvent(this GameObject self, Action<PointerEventData> onPointerDownEvent)
        {
            return self.GetOrAddComponent<OnPointerDownEventTrigger>().OnPointerDownEvent
                .Register(onPointerDownEvent);
        }
    }
}