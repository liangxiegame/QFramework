/****************************************************************************
 * Copyright (c) 2018.3 ~ 2021.8 liangxie
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

namespace QFramework.PlatformRunner
{
	public partial class UIPausePanel : UIPanel
	{
		protected override void OnClose()
		{
			
		}

		protected override void OnInit(IUIData uiData = null)
		{
			BtnReplay.onClick.AddListener(() =>
			{
//			QSoundMgr.Instance.PlayClipSync(SOUND.BTN);
				UIModel.Instance.fsm.HandleEvent("restart");
			});

			BtnContinue.onClick.AddListener(CloseSelf);

			BtnHome.onClick.AddListener(() =>
			{
				UIKit.ClosePanel<UIGamePanel>();
				CloseSelf();
				UIKit.OpenPanel<UIHomePanel>();
			});

			BtnSound.onValueChanged.AddListener((on) =>
			{
//			QSoundMgr.Instance.PlayClipSync(SOUND.BTN);
				if (on)
				{
//				QSoundMgr.Instance.On();
				}
				else
				{
//				QSoundMgr.Instance.Off();
				}
			});
		}
		

		protected override void OnShow()
		{
			UpdateView();
			Debug.LogWarning("Pause On Show");
		}

		public void UpdateView()
		{
//		if (InfoManager.Instance.gameInfo.soundState == SOUND.ON)
//		{
//			m_pBtnSound.value = true;
//		}
//		else
//		{
//			m_pBtnSound.value = false;
//		}
		}

	}
}