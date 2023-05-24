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
    public class OnEndDragEventTrigger: MonoBehaviour, IEndDragHandler
    {
        public readonly EasyEvent<PointerEventData> OnEndDragEvent = new EasyEvent<PointerEventData>();
        
        public void OnEndDrag(PointerEventData eventData)
        {
            OnEndDragEvent.Trigger(eventData);

        }
    }

    public static class OnEndDragEventTriggerExtension
    {
        public static IUnRegister OnEndDragEvent<T>(this T self, Action<PointerEventData> onEndDrag)
            where T : Component
        {
            return self.GetOrAddComponent<OnEndDragEventTrigger>().OnEndDragEvent.Register(onEndDrag);
        }
        
        public static IUnRegister OnEndDragEvent(this GameObject self, Action<PointerEventData> onEndDrag)
        {
            return self.GetOrAddComponent<OnEndDragEventTrigger>().OnEndDragEvent.Register(onEndDrag);
        }
    }
}