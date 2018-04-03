using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using UniRx;

namespace QFramework.Example
{
	public class UIConnectData : UIPageData
	{
		// TODO: Query Mgr's Data
	}

	public partial class UIConnect : QUIBehaviour
	{
		protected override void InitUI(IUIData uiData = null)
		{
			mData = uiData as UIConnectData;
			//please add init code here
		}

		protected override void ProcessMsg (int eventId,QMsg msg)
		{
			throw new System.NotImplementedException ();
		}

		protected override void RegisterUIEvent()
		{
            Button.OnClickAsObservable().Where(x => InputField.text != null).Subscribe(
                     
                     x => { QEventSystem.SendEvent(101); PCClient.Instance.ConnectToMobile(InputField.text, (z, y) => { }); }
                    
                );
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
			Debug.Log("[ UIConnect:]" + content);
		}

		UIConnectData mData = null;
	}
}