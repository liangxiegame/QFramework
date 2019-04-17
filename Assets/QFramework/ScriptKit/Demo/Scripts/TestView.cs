/****************************************************************************
 * 2019.4 XAVIER
 ****************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public class TestViewData : UIPanelData
	{
		// TODO: Query Mgr's Data
	}

	public partial class TestView : UILuaPanel
	{
		protected override void ProcessMsg (int eventId,QMsg msg)
		{
			throw new System.NotImplementedException ();
		}

		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as TestViewData ?? new TestViewData();
			//please add init code here
			BindLuaComponent();
		}

		protected override void OnOpen(IUIData uiData = null)
		{
		}

		protected override void OnShow()
		{
		}

		protected override void OnHide()
		{
		}

		protected override void OnClose()
		{
		}

		void ShowLog(string content)
		{
			Debug.Log("[ TestView:]" + content);
		}
	}
}