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

using System;
using System.Collections;
using UnityEngine;

namespace QFramework.Example
{
	public class UIMenuPanelData : IUIData
	{
		// TODO: Query
	}

	public partial class UIMenuPanel : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			var color = default(Color);
			ColorUtility.TryParseHtmlString("#FFFFFFFF", out color);
			ImageBg.color = color;

			StartCoroutine(Delay(0.1f, () =>
			{
				Debug.Log(UIKit.GetPanel<UIMenuPanel>());
			}));
			

			// 注册事件
			RegisterEvent(UIEventID.MenuPanel.ChangeMenuColor);

			BtnPlay.onClick.AddListener(() =>
			{
				UIKit.OpenPanel<UISectionPanel>(UILevel.Common, prefabName: "resources://UISectionPanel");
				// this.DoTransition<UISectionPanel>(new FadeInOut(), UILevel.Common,
				// prefabName: "Resources/UISectionPanel");
			});

			BtnSetting.onClick.AddListener(() =>
			{
				UIKit.OpenPanel<UISettingPanel>(UILevel.PopUI,
					prefabName: "Resources/UISettingPanel");
			});
			
		}

		IEnumerator Delay(float time, Action callback)
		{
			yield return new WaitForSeconds(time);

			callback();
		}

		protected override void ProcessMsg(int eventId, QMsg msg)
		{
			switch (eventId)
			{
				// 处理事件
				case (int) UIEventID.MenuPanel.ChangeMenuColor:
					Debug.LogFormat("{0}:Process EventId {1}", Transform.name, eventId);
					var color = default(Color);
					ColorUtility.TryParseHtmlString("#00FFFFFF", out color);
					ImageBg.color = color;
					break;
			}
		}

		protected override void OnClose()
		{

		}
	}
}