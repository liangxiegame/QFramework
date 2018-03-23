/****************************************************************************
 * Copyright (c) 2017 xiaojun@putao.com
 * Copyright (c) 2017 liangxie
****************************************************************************/

using System;

namespace QFramework 
{
	using UnityEngine;

	/// <summary>
	/// IUI behaviour.
	/// </summary>
	public interface IUIBehaviour
	{
		Transform Transform { get; }
		
		void Init(IUIData uiData = null);

		void Show();

		void Hide();
		
		void Close(bool destroy = true);

		void OnClosed(System.Action onPanelClosed);
	}
}