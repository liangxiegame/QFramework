/****************************************************************************
 * Copyright (c) 2016 - 2024 liangxiegame UNDER MIT License
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
    public class OnSelectUnityEvent : MonoBehaviour, ISelectHandler
    {
        public UnityEvent<BaseEventData> OnSelectEvent;

        public void OnSelect(BaseEventData eventData)
        {
            OnSelectEvent?.Invoke(eventData);
        }
    }
}