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
    public class OnPointerEnterEventTrigger : MonoBehaviour, IPointerEnterHandler
    {
        public readonly EasyEvent<PointerEventData> OnPointerEnterEvent = new EasyEvent<PointerEventData>();

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnPointerEnterEvent.Trigger(eventData);
        }
    }

    public static class OnPointerEnterEventTriggerExtension
    {
        public static IUnRegister OnPointerEnterEvent<T>(this T self, Action<PointerEventData> onPointerEnter)
            where T : Component
        {
            return self.GetOrAddComponent<OnPointerEnterEventTrigger>().OnPointerEnterEvent.Register(onPointerEnter);
        }
        
        public static IUnRegister OnPointerEnterEvent(this GameObject self, Action<PointerEventData> onPointerEnter)
        {
            return self.GetOrAddComponent<OnPointerEnterEventTrigger>().OnPointerEnterEvent.Register(onPointerEnter);
        }
    }
}