using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
/// <summary>
/// 拖拽辅助，拖拽时的Clone与是否需要拖拽角度
/// TODO：是否需要Clone的开关，设置克隆属性
/// </summary>
public class DragHelper : MonoBehaviour,IAngleDrag, ICloneDrag
{
    public enum AngleTyep
    {
        Horizontal = 1001,
        Vertical = 1002
    }
    public bool needClone = false;
    public bool needAngle = false;
    public AngleTyep angleType = AngleTyep.Horizontal;
    public float angleValue = 15f;


    private GameObject DragParent;
    private GameObject mClone;
    private CanvasGroup cloneCanvasGroup;
    private DragEventListener mDragEventListener;

    public void Clear() {
        DragParent = null;
        if (mClone != null)
        {
            Destroy(mClone);
        }
        cloneCanvasGroup = null;
        mDragEventListener = null;
    }


    public void SetListener(DragEventListener listener) {
        mDragEventListener = listener;
    }
 
    public void BeginDrag() {
        if (DragParent == null)
            DragParent = GameObject.Find("dragParent");
        if (needClone) {
            Clone();
        }
        this.transform.SetParent(DragParent.transform);
    }

    public void EndDrag() {
        DestoryClone();
        this.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public void DestoryClone() {
        if (mClone != null)
        {
            Destroy(mClone);
        }
        mClone = null;
    }

    public bool CheckAngleToDrag(PointerEventData eventData)
    {
        return ConditionToDrag(eventData);
    }

    private bool ConditionToDrag(PointerEventData eventData)
    {
        if (!needAngle)
            return true;

        float angle = Vector3.Angle(eventData.delta, Vector3.up);
        bool tempCondition = false;

        if ((angle >= 90 - angleValue && angle <= 90 + angleValue) || (angle >= 270 - angleValue && angle <= 270 + angleValue))
            tempCondition = false;
        else
            tempCondition = true;

        if (angleType == AngleTyep.Horizontal)
            tempCondition = !tempCondition;

        return tempCondition;
    }

    public void Clone()
    {
        if (mClone == null)
        {
            mClone = Instantiate(this.gameObject);
            mClone.transform.SetParent(this.transform.parent);
            mClone.transform.localPosition = this.transform.localPosition;
            mClone.transform.localScale = this.transform.localScale;
            cloneCanvasGroup = mClone.GetComponent<CanvasGroup>();
            if (cloneCanvasGroup == null)
                cloneCanvasGroup = mClone.AddComponent<CanvasGroup>();
            int index1 = this.transform.GetSiblingIndex();
            int index2 = mClone.transform.GetSiblingIndex();
            mClone.transform.SetSiblingIndex(index1);
            this.transform.SetSiblingIndex(index2);
        }
        mClone.SetActive(true);
        cloneCanvasGroup.alpha = 0.5f;
    }
}
