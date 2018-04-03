using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using UniRx;

namespace QFramework
{
    public enum UIEvent
    {
        Msg=QMgrID.UI,
    }

	public class UIMsgData : UIPageData
	{
		// TODO: Query Mgr's Data
	}

	public partial class UIMsg : QUIBehaviour
	{
		protected override void InitUI(IUIData uiData = null)
		{
			mData = uiData as UIMsgData;
			//please add init code here
		}

		protected override void ProcessMsg (int eventId,QMsg msg)
		{
            switch (eventId)
            {
                case (int)UIEvent.Msg:
                    Text.text +=(msg as SocketMsg).Msg;
                    break;
            }
		}

        protected override void RegisterUIEvent()
        {
            RegisterEvent(UIEvent.Msg);

            Button.OnClickAsObservable().Where(x => InputField.text != null).Subscribe(

                _ => PCConnectMobileManager.Instance.SendMsg(new SocketMsg() { EventID = (int)PCConnectMobileEvent.SocketEvent, ToEventID = (int)UIEvent.Msg, Msg = InputField.text}
                
                ));
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
			Debug.Log("[ UIMsg:]" + content);
		}

		UIMsgData mData = null;
	}
}