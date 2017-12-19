/****************************************************************************
 * Copyright (c) 2017 liqingyun@putao.com
 ****************************************************************************/

namespace QFramework
{
    using UnityEngine.UI;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    public static class ButtonExtension
    {
        /// <summary>
        /// Simple wrapper for onclick event
        /// </summary>
        /// <param name="selfBtn"></param>
        /// <param name="onClickEvent"></param>
        public static void RegOnClickEvent(this Button selfBtn, UnityAction onClickEvent,
            UnityAction onBeforeClickEvent = null,UnityAction onPointerUpEvent = null)
        {
            if (null != onBeforeClickEvent)
            {
                UIPointerDownEventListener.Get(selfBtn.gameObject).OnPointerDownEvent += delegate(PointerEventData arg0)
                {
                    onBeforeClickEvent.InvokeGracefully();
                    ExecuteEvents.Execute<Button>(selfBtn.gameObject, arg0,
                        delegate(Button handler, BaseEventData data)
                        {
                            handler.OnPointerDown(data as PointerEventData);
                        });
                };
            }
            
            
            if (null != onPointerUpEvent)
            {
                UIPointerUpEventListener.Get(selfBtn.gameObject).OnPointerUpEvent += delegate(PointerEventData arg0)
                {
                    onPointerUpEvent.InvokeGracefully();
                    ExecuteEvents.Execute<Button>(selfBtn.gameObject, arg0,
                        delegate(Button handler, BaseEventData data)
                        {
                            handler.OnPointerUp(data as PointerEventData);
                        });
                };
            }
 
            selfBtn.onClick.AddListener(onClickEvent);
        }
    }
}