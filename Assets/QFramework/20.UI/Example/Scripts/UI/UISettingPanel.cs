using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.PlatformRunner
{
	public class UISettingPanelData : UIPageData
	{
		// TODO: Query Mgr's Data
	}

	public partial class UISettingPanel : QUIBehaviour
	{
		protected override void InitUI(IUIData uiData = null)
		{
			mData = uiData as UISettingPanelData;
            ShowLog("show");
			//please add init code here
		}

        protected override void ProcessMsg(int eventId, QMsg msg)
        {
            ShowLog(eventId.ToString());
            throw new System.NotImplementedException();
        }


		protected override void RegisterUIEvent()
		{
            musicBar.onValueChanged.AddListener((float value) => { musicText.text = value.ToString(); } );
            backBtn.onClick.AddListener(() => { CloseSelf(); });
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
            UnRegisterAllEvent();
			base.OnClose();
		}

		void ShowLog(string content)
		{
			Debug.Log("[ UISettingPanel:]" + content);
		}

		UISettingPanelData mData = null;
	}
}