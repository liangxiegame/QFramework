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
    public class OnUpdateSelectedEventTrigger: MonoBehaviour, IUpdateSelectedHandler
    {
        public readonly EasyEvent<BaseEventData> OnUpdateSelectedEvent = new EasyEvent<BaseEventData>();
        

        public void OnUpdateSelected(BaseEventData eventData)
        {
            OnUpdateSelectedEvent.Trigger(eventData);
        }
    }

    public static class  OnUpdateSelectedEventTriggerExtension
    {
        public static IUnRegister OnUpdateSelectedEvent<T>(this T self, Action<BaseEventData> onUpdateSelected)
            where T : Component
        {
            return self.GetOrAddComponent<OnUpdateSelectedEventTrigger>().OnUpdateSelectedEvent.Register(onUpdateSelected);
        }
        
        public static IUnRegister OnUpdateSelectedEvent(this GameObject self, Action<BaseEventData> onUpdateSelected)
        {
            return self.GetOrAddComponent<OnUpdateSelectedEventTrigger>().OnUpdateSelectedEvent.Register(onUpdateSelected);
        }
    }
}