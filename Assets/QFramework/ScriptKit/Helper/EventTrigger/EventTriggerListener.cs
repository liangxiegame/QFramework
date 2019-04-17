using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class EventTriggerListener : EventTrigger
{
    public delegate void VoidDelegate (GameObject go,BaseEventData kEvtData);
    public VoidDelegate onClick;
    public VoidDelegate onDown;
    public VoidDelegate onEnter;
    public VoidDelegate onExit;
    public VoidDelegate onUp;
    public VoidDelegate onSelect;
    public VoidDelegate onUpdateSelect;
    public VoidDelegate onDrag;
    public VoidDelegate onDrop;
    public VoidDelegate onBeginDrag;
    public VoidDelegate onEndDrag;


    static public EventTriggerListener Get(GameObject go)
    {
        EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
        if (listener == null) listener = go.AddComponent<EventTriggerListener>();
        return listener;
    }
    public override void OnPointerClick(PointerEventData kEvtData)
    {
        if (Input.touchCount > 1)
            return;
        if (onClick != null) onClick(gameObject,kEvtData);
    }
    public override void OnPointerDown(PointerEventData kEvtData)
    {
        if (onDown != null) onDown(gameObject,kEvtData);
    }
    public override void OnPointerEnter(PointerEventData kEvtData)
    {
        if (onEnter != null) onEnter(gameObject,kEvtData);
    }
    public override void OnPointerExit(PointerEventData kEvtData)
    {
        if (onExit != null) onExit(gameObject,kEvtData);
    }
    public override void OnPointerUp(PointerEventData kEvtData)
    {
        if (onUp != null) onUp(gameObject,kEvtData);
    }
    public override void OnSelect(BaseEventData kEvtData)
    {
        if (onSelect != null) onSelect(gameObject,kEvtData);
    }
    public override void OnUpdateSelected(BaseEventData kEvtData)
    {
        if (onUpdateSelect != null) onUpdateSelect(gameObject,kEvtData);
    }

    public override void OnBeginDrag(PointerEventData kEvtData)
    {
        if (null != onBeginDrag) onBeginDrag(gameObject, kEvtData);
    }
    public override void OnDrag(PointerEventData kEvtData)
    {
        if (null != onDrag) onDrag(gameObject, kEvtData);
    }
    public override void OnDrop(PointerEventData kEvtData)
    {
        if(null != onDrop) onDrop(gameObject,kEvtData);
    }

    public override void OnEndDrag(PointerEventData kEvtData)
    {
        if (null != onEndDrag) onEndDrag(gameObject, kEvtData);
    }

    public override void OnMove(AxisEventData eventData)
    {

    }
    public override void OnCancel(BaseEventData eventData)
    {

    }

    public override void OnScroll(PointerEventData eventData)
    {

    }

 


}