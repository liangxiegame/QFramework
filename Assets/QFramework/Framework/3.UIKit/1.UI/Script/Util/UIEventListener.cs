/****************************************************************************
 * Copyright (c) 2017 liangxie
****************************************************************************/

namespace QFramework 
{
	using UnityEngine;
	using UnityEngine.EventSystems;
	#if SLUA_SUPPORT
	using SLua;
	[CustomLuaClass]
	#endif
	public class UIEventListener : EventTrigger
	{
		// TODO: refactor proper name
		public System.Action onClick;
	
		public System.Action<GameObject> onSelect;
		public System.Action<GameObject> onUpdateSelect;

		public System.Action<BaseEventData> onPointerDown;
		public System.Action<BaseEventData> onPointerEnter;
		public System.Action<BaseEventData> onPointerExit;
		public System.Action<BaseEventData> onPointerUp;

		public System.Action<BaseEventData> onBeginDrag;
		public System.Action<BaseEventData> onEndDrag;
		public System.Action<BaseEventData> onDrag;

		public System.Action<bool> onValueChanged;

		public static UIEventListener CheckAndAddListener(GameObject go)
		{
			UIEventListener listener = go.GetComponent<UIEventListener>();
			if (listener == null) listener = go.AddComponent<UIEventListener>();

			return listener;
		}
		public static  UIEventListener Get(GameObject go)
		{
			return CheckAndAddListener (go);
		}

		public override void OnPointerClick(PointerEventData eventData)
		{
			if (onClick != null) onClick();
		}
		public override void OnPointerDown(PointerEventData eventData)
		{
			if (onPointerDown != null) onPointerDown(eventData);
		}
		public override void OnPointerEnter(PointerEventData eventData)
		{
			if (onPointerEnter != null) onPointerEnter(eventData);
		}
		public override void OnPointerExit(PointerEventData eventData)
		{
			if (onPointerExit != null) onPointerExit(eventData);
		}
		public override void OnPointerUp(PointerEventData eventData)
		{
			if (onPointerUp != null) onPointerUp(eventData);
		}
		public override void OnSelect(BaseEventData eventData)
		{
			if (onSelect != null) onSelect(gameObject);
		}
		public override void OnUpdateSelected(BaseEventData eventData)
		{
			if (onUpdateSelect != null) onUpdateSelect(gameObject);
		}
		public override void OnBeginDrag(PointerEventData eventData)
		{
			if (onBeginDrag != null) onBeginDrag(eventData);
		}
		public override void OnEndDrag(PointerEventData eventData)
		{
			if (onEndDrag != null) onEndDrag(eventData);
		}
		public override void OnDrag(PointerEventData eventData) 
		{
			if (onDrag != null) onDrag(eventData);
		}

	    void OnDestroy()
	    {
	        onClick = null;
	        onSelect = null;
	        onUpdateSelect = null;

	        onPointerDown = null;
	        onPointerEnter = null;
	        onPointerExit = null;
	        onPointerUp = null;

	        onBeginDrag = null;
	        onEndDrag = null;
	        onDrag = null;

	        onValueChanged = null;
	    }
	}
}