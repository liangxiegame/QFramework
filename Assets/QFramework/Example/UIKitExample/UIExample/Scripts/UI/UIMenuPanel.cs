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

using UniRx;

namespace QFramework.Example
{
	public class UIMenuPanelData : IUIData
	{
		// TODO: Query
	}

	public partial class UIMenuPanel : UIPanel
	{
		protected override void InitUI(IUIData uiData = null)
		{
			ImageBg.color = "#FFFFFFFF".HtmlStringToColor();

			Observable.NextFrame().Subscribe(_ => { UIMgr.GetPanel<UIMenuPanel>().LogInfo(); });
		}

		protected override void ProcessMsg(int eventId, QMsg msg)
		{
			Log.I("Process");
			switch (eventId)
			{
				case (int) UIEventID.MenuPanel.ChangeMenuColor:
					Log.I("{0}:Process EventId {1}", Transform.name, eventId);
					ImageBg.color = "#00FFFFFF".HtmlStringToColor();
					break;
			}
		}

		protected override void RegisterUIEvent()
		{
			RegisterEvent(UIEventID.MenuPanel.ChangeMenuColor);

			BtnPlay.onClick.AddListener(() =>
			{
				this.DoTransition<UISectionPanel>(new FadeInOut(), UILevel.Common,
					prefabName: "Resources/UISectionPanel");
			});

			BtnSetting.onClick.AddListener(() =>
			{
				UIMgr.OpenPanel<UISettingPanel>(UILevel.PopUI,
					prefabName: "Resources/UISettingPanel");
			});
		}
		
		protected override void OnClose()
		{
			
		}
	}
}