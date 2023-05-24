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
        public static IUnRegister OnPointerClickEvent<T>(this T self, Action<PointerEventData> onPointerClick)
            where T : Component
        {
            return self.GetOrAddComponent<OnPointerClickEventTrigger>().OnPointerClickEvent.Register(onPointerClick);
        }
        
        public static IUnRegister OnPointerClickEvent(this GameObject self, Action<PointerEventData> onPointerClick)
        {
            return self.GetOrAddComponent<OnPointerClickEventTrigger>().OnPointerClickEvent.Register(onPointerClick);
        }
    }
}