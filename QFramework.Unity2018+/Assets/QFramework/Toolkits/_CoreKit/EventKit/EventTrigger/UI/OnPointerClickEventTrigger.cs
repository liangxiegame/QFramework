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
        private readonly float doubleClickTimeThreshold = 0.3f; // 双击时间间隔阈值
        private float lastClickTime = 0f;


        public readonly EasyEvent<PointerEventData> OnPointerClickEvent = new EasyEvent<PointerEventData>();
        public readonly EasyEvent<PointerEventData> OnPointerDoubleClickEvent = new EasyEvent<PointerEventData>();

        public void OnPointerClick(PointerEventData eventData)
        {
            OnPointerClickEvent.Trigger(eventData);
            float currentTime = Time.time;

            if (currentTime - lastClickTime <= doubleClickTimeThreshold)
            {
                // 双击事件处理
                OnPointerDoubleClickEvent.Trigger(eventData);
            }
            lastClickTime = currentTime;
        }
    }

    public static class OnPointerClickEventTriggerExtension
    {
        public static IUnRegister OnPointerClickEvent<T>(this T self, Action<PointerEventData> onPointerClick)
            where T : Component
        {
            return self.GetOrAddComponent<OnPointerClickEventTrigger>().OnPointerClickEvent.Register(onPointerClick);
        }
        public static IUnRegister OnPointerDoubleClickEvent<T>(this T self, Action<PointerEventData> onPointerClick)
    where T : Component
        {
            return self.GetOrAddComponent<OnPointerClickEventTrigger>().OnPointerDoubleClickEvent.Register(onPointerClick);
        }
        public static IUnRegister OnPointerClickEvent(this GameObject self, Action<PointerEventData> onPointerClick)
        {
            return self.GetOrAddComponent<OnPointerClickEventTrigger>().OnPointerClickEvent.Register(onPointerClick);
        }
        public static IUnRegister OnPointerDoubleClickEvent(this GameObject self, Action<PointerEventData> onPointerClick)
        {
            return self.GetOrAddComponent<OnPointerClickEventTrigger>().OnPointerDoubleClickEvent.Register(onPointerClick);
        }
    }
}