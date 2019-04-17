using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PointClickEvent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler,IPointerClickHandler,IBeginDragHandler,IDragHandler,IEndDragHandler
{
    public Scrollbar targBar;

    public delegate void PointerEvent();
    public PointerEvent onPress = null;
    public PointerEvent onUp = null;
    public PointerEvent onClick = null;
    public PointerEvent onBeginDragEvent = null;
    public PointerEvent onDragEvent = null;
    public PointerEvent onEndDragEvent = null;


    public void OnPointerDown(PointerEventData eventData)
    {
        if(onPress != null)
            onPress();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (onUp != null)
            onUp();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        targBar.OnBeginDrag(eventData);
        if (onBeginDragEvent != null)
            onBeginDragEvent();
    }

    public void OnDrag(PointerEventData eventData)
    {
        targBar.OnDrag(eventData);
        if (onDragEvent != null)
            onDragEvent();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (onEndDragEvent != null)
            onEndDragEvent();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (onClick != null)
            onClick();
    }
}
