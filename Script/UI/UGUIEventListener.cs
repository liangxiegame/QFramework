using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace QFramework.UI {


	/// <summary>
	/// 事件监听器
	/// </summary>
	public class UGUIEventListener : UnityEngine.EventSystems.EventTrigger
	{
		public VoidDelegate.WithGo onClick;
		public VoidDelegate.WithGo onDown;
		public VoidDelegate.WithGo onEnter;
		public VoidDelegate.WithGo onExit;
		public VoidDelegate.WithGo onUp;
		public VoidDelegate.WithGo onSelect;
		public VoidDelegate.WithGo onUpdateSelect;

		public VoidDelegate.WithEvent onBeginDrag;
		public VoidDelegate.WithEvent onEndDrag;
		public VoidDelegate.WithEvent onDrag;


		public static UGUIEventListener CheckAndAddListener(GameObject go)
		{
			UGUIEventListener listener = go.GetComponent<UGUIEventListener>();
			if (listener == null) listener = go.AddComponent<UGUIEventListener>();

			return listener;
		}
		static public UGUIEventListener Get(GameObject go)
		{
			return CheckAndAddListener (go);
		}

		public override void OnPointerClick(PointerEventData eventData)
		{
			if (onClick != null) onClick(gameObject);
		}
		public override void OnPointerDown(PointerEventData eventData)
		{
			if (onDown != null) onDown(gameObject);
		}
		public override void OnPointerEnter(PointerEventData eventData)
		{
			if (onEnter != null) onEnter(gameObject);
		}
		public override void OnPointerExit(PointerEventData eventData)
		{
			if (onExit != null) onExit(gameObject);
		}
		public override void OnPointerUp(PointerEventData eventData)
		{
			if (onUp != null) onUp(gameObject);
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

	}

}