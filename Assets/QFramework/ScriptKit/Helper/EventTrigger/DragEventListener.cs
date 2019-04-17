using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
/// <summary>
/// 从EventTriggerListener单独把拖拽相关事件抽了出来
/// 主要处理拖拽事件（是否存在角度，是否向下传递拖拽事件）
/// TODO：把角度判断移出去，稍微改改命名吧...
/// </summary>
public class DragEventListener : EventTriggerListener
{
    public new VoidDelegate onBeginDrag;
    public new VoidDelegate onDrag;
    public new VoidDelegate onEndDrag;
    public new VoidDelegate onDrop;
    public delegate void NewVoidDelegate(GameObject go, BaseEventData kEvtData,bool canClick);
    public NewVoidDelegate onNotDrag;

    public GameObject svobj = null;


    private DragHelper mDragHelper;
    private bool isDrag = false;
    private bool canDrag = true;
    private bool onlySinglePointer = true;
    private bool emAble = true;
    public void ClearDelegate() {
        //this.enabled = false;
        emAble = false;
    }

    public void RevertDelegate() {
        //this.enabled = true;
        emAble = true;
    }

    static public new DragEventListener Get(GameObject go)
    {
        DragEventListener listener = go.GetComponent<DragEventListener>();
        if (listener == null) listener = go.AddComponent<DragEventListener>();
        return listener;
    }

    public void SetDragHelper(DragHelper helper) {
        mDragHelper = helper;
    }

    public override void OnBeginDrag(PointerEventData kEvtData)
    {
        if (!emAble)
            return;
        if (Input.touchCount > 1 && !isDrag)
            onlySinglePointer = false;
        else
        {
            isDrag = true;
            onlySinglePointer = true;
        }
        if (!onlySinglePointer)
            return;

        if (onBeginDrag != null)
        {
            if (mDragHelper == null)
                mDragHelper = GetComponent<DragHelper>();
            if (mDragHelper != null && mDragHelper.needAngle)
            {
                canDrag = false;
                if (mDragHelper.CheckAngleToDrag(kEvtData))
                {
                    canDrag = true;
                }
            }
            else
            {
                canDrag = true;
            }
            if (canDrag)
            {
                if (mDragHelper != null)
                    mDragHelper.BeginDrag();
                onBeginDrag(gameObject, kEvtData);
            }
            else {
                if (onNotDrag != null)
                    onNotDrag(gameObject, kEvtData,false);
                svobj = null;
                PassEvent(kEvtData, ExecuteEvents.beginDragHandler);
            }
        }
        else
        {
            svobj = null;
            PassEvent(kEvtData, ExecuteEvents.beginDragHandler);
        }
    }
    public override void OnDrag(PointerEventData kEvtData )
    {
        if (!emAble)
            return;
        if (!isDrag)
            return;
                
        if (null != onDrag && canDrag)
        {
            RectTransform kTs = gameObject.GetComponent<RectTransform>();
            kTs.pivot.Set(0, 0);
            PointerEventData kData = kEvtData as PointerEventData;
            Vector3 globalMousePos;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(kTs, kData.position, kData.pressEventCamera, out globalMousePos))
                kTs.position = globalMousePos;
            onDrag(gameObject, kEvtData);
        }
        else
        {
            PassEvent(kEvtData, ExecuteEvents.dragHandler);
        }
    }

    public override void OnEndDrag(PointerEventData kEvtData)
    {
        if (!emAble)
            return;
        if (!isDrag)
            return;
        if (null != onEndDrag && canDrag)
        {
            onEndDrag(gameObject, kEvtData);
            canDrag = false;
            if (mDragHelper != null)
                mDragHelper.EndDrag();
        }
        else
        {
            if (onNotDrag != null)
                onNotDrag(gameObject, kEvtData, true);
            PassEvent(kEvtData, ExecuteEvents.endDragHandler);
        }
        isDrag = false;
    }

    public override void OnDrop(PointerEventData kEvtData)
    {
        if (!emAble)
            return;
        if (null != onDrop) onDrop(gameObject, kEvtData);
    }

    private void PassEvent<T>(PointerEventData data, ExecuteEvents.EventFunction<T> function)
         where T : IEventSystemHandler
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);
        GameObject current = data.pointerCurrentRaycast.gameObject;
        if (results.Count > 0 && svobj == null)
        {
            for (int i = 0; i < results.Count; i++)
            {
                if (current != results[i].gameObject)
                {
                    if ((results[i].gameObject.name == "Scroll View") && svobj == null)
                    {
                        svobj = results[i].gameObject;
                        ExecuteEvents.Execute(results[i].gameObject, data, function);
                    }
                    else if (svobj != null)
                    {
                        ExecuteEvents.Execute(svobj, data, function);
                    }
                }
            }
        }
        else {
            if (svobj != null)
                ExecuteEvents.Execute(svobj, data, function);
        }
        
    }
}
