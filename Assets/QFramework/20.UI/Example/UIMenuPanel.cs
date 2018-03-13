using System;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.UI;


namespace QFramework.PlatformRunner
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
			mUIComponents.BtnPlay.onClick.AddListener(() => {
                Log.I("on btn play clicked"); 
                UIMgr.OpenPanel<UISectionPanel>(UILevel.Common, prefabName: "Resources/UISectionPanel");
                Hide();
            });

			mUIComponents.BtnSetting.onClick.AddListener(() => { 
                Log.I("on btn setting clicked");
                UIMgr.OpenPanel<UISettingPanel>(UILevel.PopUI,prefabName: "Resources/UISettingPanel");
            });
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