/****************************************************************************
 * Copyright (c) 2016 - 2022 liangxiegame UNDER MIT License
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
    public class OnPointerClickEventTrigger : MonoBehaviour, IPointerClickHandler
    {
        public readonly EasyEvent<PointerEventData> OnPointerClickEvent = new EasyEvent<PointerEventData>();

        public void OnPointerClick(PointerEventData eventData)
        {
            OnPointerClickEvent.Trigger(eventData);
        }
    }

    public static class OnPointerClickEventTriggerExtension
    {
        public static IUnRegister OnPointerClickEvent<T>(this T self, Action<PointerEventData> onClick)
            where T : Component
        {
            return self.GetOrAddComponent<OnPointerClickEventTrigger>().OnPointerClickEvent.Register(onClick);
        }
        
        public static IUnRegister OnPointerClickEvent(this GameObject self, Action<PointerEventData> onClick)
        {
            return self.GetOrAddComponent<OnPointerClickEventTrigger>().OnPointerClickEvent.Register(onClick);
        }
    }
}