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
    public class OnInitializePotentialDragEventTrigger: MonoBehaviour, IInitializePotentialDragHandler
    {
        public readonly EasyEvent<PointerEventData> OnInitializePotentialDragEvent = new EasyEvent<PointerEventData>();
        

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            OnInitializePotentialDragEvent.Trigger(eventData);
        }
    }

    public static class OnInitializePotentialDragEventTriggerExtension
    {
        public static IUnRegister OnInitializePotentialDragEvent<T>(this T self, Action<PointerEventData> onInitializePotentialDrag)
            where T : Component
        {
            return self.GetOrAddComponent<OnInitializePotentialDragEventTrigger>().OnInitializePotentialDragEvent.Register(onInitializePotentialDrag);
        }
        
        public static IUnRegister OnInitializePotentialDragEvent(this GameObject self, Action<PointerEventData> onInitializePotentialDrag)
        {
            return self.GetOrAddComponent<OnInitializePotentialDragEventTrigger>().OnInitializePotentialDragEvent.Register(onInitializePotentialDrag);
        }
    }
}