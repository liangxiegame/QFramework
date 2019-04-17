using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI.CoroutineTween;
using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class CustomDropdown : Dropdown
{
	protected CustomDropdown()
	{

	}
	private static T GetOrAddComponent<T>(GameObject go) where T : Component
	{
		T comp = go.GetComponent<T>();
		if (!comp)
			comp = go.AddComponent<T>();
		return comp;
	}

	public override void OnPointerClick(PointerEventData eventData)
	{
		Show();
        if (onClick != null) 
			onClick(gameObject,eventData);
	}

	public delegate void VoidDelegate (GameObject go,BaseEventData kEvtData);
    public VoidDelegate onClick = null;

	// public override void OnSubmit(BaseEventData eventData)
	// {
	// 	Show();
	// }

	// public override void OnCancel(BaseEventData eventData)
	// {
	// 	Hide();
	// }
}



