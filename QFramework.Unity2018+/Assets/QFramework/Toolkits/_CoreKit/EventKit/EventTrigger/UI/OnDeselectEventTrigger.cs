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
    public class OnDeselectEventTrigger: MonoBehaviour, IDeselectHandler
    {
        public readonly EasyEvent<BaseEventData> OnDeselectEvent = new EasyEvent<BaseEventData>();
        

        public void OnDeselect(BaseEventData eventData)
        {
            OnDeselectEvent.Trigger(eventData);
        }
    }

    public static class OnDeselectEventTriggerExtension
    {
        public static IUnRegister OnDeselectEvent<T>(this T self, Action<BaseEventData> onDeselect)
            where T : Component
        {
            return self.GetOrAddComponent<OnDeselectEventTrigger>().OnDeselectEvent.Register(onDeselect);
        }
        
        public static IUnRegister OnDeselectEvent(this GameObject self, Action<BaseEventData> onDeselect)
        {
            return self.GetOrAddComponent<OnDeselectEventTrigger>().OnDeselectEvent.Register(onDeselect);
        }
    }
}