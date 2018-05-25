/****************************************************************************
 * Copyright (c) 2017 liangxie
****************************************************************************/

using UnityEngine;
using UnityEngine.EventSystems;
//using UnityEngine.UI;

namespace QFramework
{
	public class DragMe : MonoBehaviour,IPointerDownHandler, IDragHandler, IPointerUpHandler
	{
        //private Vector2 onBeginDragPosition;

		private RectTransform mDraggingPlane;

		public void OnPointerDown (PointerEventData data)
		{
            //onBeginDragPosition = data.position;
		}

		public void OnDrag(PointerEventData data)
		{
			SetDraggedPosition(data);
			data.IsScrolling();
		}

		private void SetDraggedPosition(PointerEventData data)
		{
			if (data.pointerEnter != null && data.pointerEnter.transform is RectTransform)
                mDraggingPlane = data.pointerEnter.transform as RectTransform;

			var rt = gameObject.GetComponent<RectTransform>();
			Vector3 globalMousePos;
			if (RectTransformUtility.ScreenPointToWorldPointInRectangle(mDraggingPlane, data.position, data.pressEventCamera, out globalMousePos))
			{
				rt.position = globalMousePos;
				rt.rotation = mDraggingPlane.rotation;
			}
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			//		Destroy(m_DraggingIcon);
		}

		public static T FindInParents<T>(GameObject go) where T : Component
		{
			if (go == null) return null;
			var comp = go.GetComponent<T>();

			if (comp != null)
				return comp;

			Transform t = go.transform.parent;
			while (t != null && comp == null)
			{
				comp = t.gameObject.GetComponent<T>();
				t = t.parent;
			}

			return comp;
		}

	    private void CreateDragIcon( PointerEventData data)
		{
			var canvas = FindInParents<Canvas>(gameObject);
			if (canvas == null)
				return;


			CanvasGroup group = gameObject.AddComponent<CanvasGroup>();
			group.blocksRaycasts = false;

			mDraggingPlane = transform as RectTransform;

			SetDraggedPosition(data);
		}
	}
}