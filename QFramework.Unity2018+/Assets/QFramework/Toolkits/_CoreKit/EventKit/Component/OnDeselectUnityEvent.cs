/****************************************************************************
 * Copyright (c) 2015 - 2024 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace QFramework
{
    public class OnDeselectUnityEvent: MonoBehaviour, IDeselectHandler
    {
        public UnityEvent<BaseEventData> OnDeselectEvent;

        public void OnDeselect(BaseEventData eventData)
        {
            OnDeselectEvent?.Invoke(eventData);
        }
    }
}