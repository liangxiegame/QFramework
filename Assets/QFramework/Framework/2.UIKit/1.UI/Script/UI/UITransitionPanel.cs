/****************************************************************************
 * 2018.9 凉鞋的MacBook Pro (2)
 ****************************************************************************/

using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework
{
	public class UITransitionPanelData : UIPanelData
	{
		public UITransition Transition;
	}
	
	public partial class UITransitionPanel : UIPanel
	{
		public Image Image;

		protected override void InitUI(IUIData uiData = null)
		{
			mData = uiData as UITransitionPanelData ?? new UITransitionPanelData();
			//please add init code here

			Image = GetComponent<Image>();

			mData.Transition.OutCompleted = CloseSelf;

			mData.Transition.Do(this);
			
		}

		protected override void ProcessMsg (int eventId,QMsg msg)
		{
			throw new System.NotImplementedException ();
		}

		protected override void RegisterUIEvent()
		{
		}

		protected override void OnClose()
		{
		}

		void ShowLog(string content)
		{
			Debug.Log("[ UITransitionPanel:]" + content);
		}
	}
}