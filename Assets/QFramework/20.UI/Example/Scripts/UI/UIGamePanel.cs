using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.PlatformRunner
{
	public class UIGamePanelData : UIPageData
	{
        // TODO: Query Mgr's Data
        public int SectionNo;
        public UIGamePanelData(int SectionNo){
            this.SectionNo = SectionNo;
        }
	}

	public partial class UIGamePanel : QUIBehaviour
	{
		protected override void InitUI(IUIData uiData = null)
		{
			mData = uiData as UIGamePanelData;
            //please add init code here
            gameText.text = string.Format("Hello,You are in Section {0}",mData.SectionNo);
		}

		protected override void ProcessMsg(int eventId, QMsg msg)
		{
			throw new System.NotImplementedException ();
		}

		protected override void RegisterUIEvent()
		{
            backBtn.onClick.AddListener(() => { 
                UIMgr.OpenPanel<UISectionPanel>(UILevel.Common, prefabName: "Resources/UISectionPanel");
                CloseSelf(); });
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
			Debug.Log("[ UIGamePanel:]" + content);
		}

		UIGamePanelData mData = null;
	}
}