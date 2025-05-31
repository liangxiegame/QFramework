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
    public class OnSelectEventTrigger: MonoBehaviour, ISelectHandler
    {
        public readonly EasyEvent<BaseEventData> OnSelectEvent = new EasyEvent<BaseEventData>();
        
        public void OnSelect(BaseEventData eventData)
        {
            OnSelectEvent.Trigger(eventData);
        }
    }

    public static class OnSelectEventTriggerTriggerExtension
    {
        public static IUnRegister OnSelectEvent<T>(this T self, Action<BaseEventData> onSelect)
            where T : Component
        {
            return self.GetOrAddComponent<OnSelectEventTrigger>().OnSelectEvent.Register(onSelect);
        }
        
        public static IUnRegister OnSelectEvent(this GameObject self, Action<BaseEventData> onSelect)
        {
            return self.GetOrAddComponent<OnSelectEventTrigger>().OnSelectEvent.Register(onSelect);
        }
    }
}