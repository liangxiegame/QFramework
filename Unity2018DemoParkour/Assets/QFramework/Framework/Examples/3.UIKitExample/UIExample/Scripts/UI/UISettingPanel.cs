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

namespace QFramework.Example
{
	public class UISettingPanelData : UIPanelData
	{
		// TODO: Query Mgr's Data
	}

	public partial class UISettingPanel : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			//please add init code here

			EventBtn.onClick.AddListener(() => { SendEvent(UIEventID.MenuPanel.ChangeMenuColor); });

			BackBtn.onClick.AddListener(() => { CloseSelf(); });
			
			this.SendMsg(new AudioMusicMsg(
				"GameBg",
				loop: false,
				allowMusicOff: false,
				onMusicBeganCallback: () => { Debug.Log("Music Start"); },
				onMusicEndedCallback: () => { Debug.Log("MusicEnd"); })
			);

			this.SendMsg(new AudioMusicMsg("HomeBg"));
		}

		protected override void ProcessMsg(int eventId, QMsg msg)
		{
			throw new System.NotImplementedException();
		}

		protected override void OnClose()
		{
		}
	}
}