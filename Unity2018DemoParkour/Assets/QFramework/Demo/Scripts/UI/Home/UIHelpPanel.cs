/****************************************************************************
 * Copyright (c) 2018.3 ~ 7 liangxie
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

namespace QFramework.PlatformRunner
{
	public partial class UIHelpPanel : UIPanel
	{
		public enum HelpState
		{
			ONE,
			TWO,
			THREE,
		}

		private HelpState mCurState = HelpState.ONE;

		protected override void OnShow()
		{
			base.OnShow();
			mCurState = HelpState.ONE; // 默认都是1
			UpdateView();
		}


		private void Previous()
		{
			switch (mCurState)
			{
				case HelpState.ONE:
					//不可能
					break;
				case HelpState.TWO:
					mCurState = HelpState.ONE;
					break;
				case HelpState.THREE:
					mCurState = HelpState.TWO;
					break;
			}
		}

		private void Next()
		{
			switch (mCurState)
			{
				case HelpState.ONE:
					mCurState = HelpState.TWO;
					break;
				case HelpState.TWO:
					mCurState = HelpState.THREE;
					break;
				case HelpState.THREE:
					mCurState = HelpState.ONE; // 不可能
					break;
			}
		}

		public void UpdateView()
		{
			switch (mCurState)
			{
				case HelpState.ONE:
					BtnLeft.gameObject.SetActive(false);
					Help1.gameObject.SetActive(true);
					Help2.gameObject.SetActive(false);
					Help3.gameObject.SetActive(false);
					BtnRight.gameObject.SetActive(true);
					break;
				case HelpState.TWO:
					BtnLeft.gameObject.SetActive(true);
					Help1.gameObject.SetActive(false);
					Help2.gameObject.SetActive(true);
					Help3.gameObject.SetActive(false);
					BtnRight.gameObject.SetActive(true);
					break;
				case HelpState.THREE:
					BtnLeft.gameObject.SetActive(true);
					Help1.gameObject.SetActive(false);
					Help2.gameObject.SetActive(false);
					Help3.gameObject.SetActive(true);
					BtnRight.gameObject.SetActive(false);
					break;
			}
		}


		protected override void OnClose()
		{
			
		}

		protected override void OnInit(IUIData uiData = null)
		{
			BtnLeft.onClick.AddListener(delegate()
			{
//			QSoundMgr.Instance.PlayClipSync(SOUND.BTN);
				Previous();
				UpdateView();
			});

			BtnRight.onClick.AddListener(delegate()
			{
//			QSoundMgr.Instance.PlayClipSync(SOUND.BTN);
				Next();
				UpdateView();
			});

			BtnBack.onClick.AddListener(delegate()
			{
//			QSoundMgr.Instance.PlayClipSync(SOUND.BTN);
				CloseSelf();
			});

			UpdateView();
			
		}


		protected override void OnHide()
		{
		}
	}
}