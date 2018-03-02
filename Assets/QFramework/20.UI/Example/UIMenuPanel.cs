using System;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.UI;


namespace QFramework.Example
{
	public class UIMenuPanelData : IUIData
	{
		// TODO: Query
	}

	public class UIMenuPanel : QUIBehaviour
	{
		protected override void InitUI(IUIData uiData = null)
		{
			mUIComponents = mIComponents as UIMenuPanelComponents;
			mData = uiData as UIMenuPanelData;
			//please add init code here
		}

		protected override void ProcessMsg (int eventId,QMsg msg)
		{
			throw new System.NotImplementedException ();
		}

		protected override void RegisterUIEvent()
		{
			mUIComponents.BtnPlay.onClick.AddListener(() => { Log.I("on btn play clicked"); });

			mUIComponents.BtnSetting.onClick.AddListener(() => { Log.I("on btn setting clicked"); });
		}

		protected override void OnShow()
		{
			base.OnShow();
		}

		protected override void OnHide()
		{
			base.OnHide();
		}

		protected override void OnClose()
		{
			base.OnClose();
		}

		void ShowLog(string content)
		{
			Debug.Log("[ UIMenuPanel:]" + content);
		}

		UIMenuPanelComponents mUIComponents = null;
		UIMenuPanelData mData = null;
	}
}