using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using UniRx;

namespace QFramework.Example
{
	public class UITestUniRxData : UIPageData
	{
		// TODO: Query Mgr's Data
	}

	public partial class UITestUniRx : QUIBehaviour
	{
		protected override void InitUI(IUIData uiData = null)
		{
			mData = uiData as UITestUniRxData;
			//please add init code here
		}

		protected override void ProcessMsg (int eventId,QMsg msg)
		{
			throw new System.NotImplementedException ();
		}

		protected override void RegisterUIEvent()
		{
            Button.onClick
                .AsObservable()
                .Subscribe(x=> this.LogInfo("按下按钮"));
        
            InputField.OnValueChangedAsObservable()
                .Where(x => x != null)
                .SubscribeToText(Text);

            Toggle.OnValueChangedAsObservable()
                .SubscribeToInteractable(Button);
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
			Debug.Log("[ UITestUniRx:]" + content);
		}

		UITestUniRxData mData = null;
	}
}