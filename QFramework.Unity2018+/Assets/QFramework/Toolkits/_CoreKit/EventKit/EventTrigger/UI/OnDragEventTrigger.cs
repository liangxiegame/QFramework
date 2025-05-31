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
    public class OnDragEventTrigger: MonoBehaviour, IDragHandler
    {
        public readonly EasyEvent<PointerEventData> OnDragEvent = new EasyEvent<PointerEventData>();
        
        public void OnDrag(PointerEventData eventData)
        {
            OnDragEvent.Trigger(eventData);

        }
    }

    public static class OnDragEventTriggerExtension
    {
        public static IUnRegister OnDragEvent<T>(this T self, Action<PointerEventData> onDrag)
            where T : Component
        {
            return self.GetOrAddComponent<OnDragEventTrigger>().OnDragEvent.Register(onDrag);
        }
        
        public static IUnRegister OnDragEvent(this GameObject self, Action<PointerEventData> onDrag)
        {
            return self.GetOrAddComponent<OnDragEventTrigger>().OnDragEvent.Register(onDrag);
        }
    }
}