/****************************************************************************
 * Copyright (c) 2018.3 vin129
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

using UnityEngine;

namespace QFramework.UIExample
{
	public class UISettingPanelData : UIPanelData
	{
		// TODO: Query Mgr's Data
	}

	public partial class UISettingPanel : UIPanel
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
            eventBtn.onClick.AddListener(()=>{ SendEvent<int>((int)UIEventID.MenuPanel.ChangeMenuColor); });
            backBtn.onClick.AddListener(() =>{ CloseSelf(); });
		}

		protected override void OnHide()
		{
            base.OnHide();
		}

		protected override void OnShow()
		{
            base.OnShow();
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