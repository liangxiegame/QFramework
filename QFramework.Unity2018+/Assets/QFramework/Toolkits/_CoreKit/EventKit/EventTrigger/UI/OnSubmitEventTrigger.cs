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
    public class OnSubmitEventTrigger: MonoBehaviour, ISubmitHandler
    {
        public readonly EasyEvent<BaseEventData> OnSubmitEvent = new EasyEvent<BaseEventData>();

        public void OnSubmit(BaseEventData eventData)
        {
            OnSubmitEvent.Trigger(eventData);
        }
    }

    public static class OnSubmitEventTriggerExtension
    {
        public static IUnRegister OnSubmitEvent<T>(this T self, Action<BaseEventData> onSubmit)
            where T : Component
        {
            return self.GetOrAddComponent<OnSubmitEventTrigger>().OnSubmitEvent.Register(onSubmit);
        }
        
        public static IUnRegister OnSubmitEvent(this GameObject self, Action<BaseEventData> onSubmit)
        {
            return self.GetOrAddComponent<OnSubmitEventTrigger>().OnSubmitEvent.Register(onSubmit);
        }
    }
}