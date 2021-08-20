/****************************************************************************
 * Copyright (c) 2018.3 liangxie
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
	/// <summary>
	/// 游戏结束窗口
	/// </summary>

	public class OverWnd : QMonoBehaviour
	{
		public override IManager Manager
		{
			get { return UIManager.Instance; }
		}

//		public UIButton m_pBtnHome;
//		public UIButton m_pBtnReplay;
//		public UIButton m_pBtnShop;

//		public NumberLabel meterLabel;
//		public NumberLabel bestMeterLabel;
//		public NumberLabel coinLabel;

		public void Start()
		{
//			m_pBtnHome.onClick.Add(new EventDelegate(delegate
//			{
//			QSoundMgr.Instance.PlayClipSync(SOUND.BTN);
				UIModel.Instance.fsm.HandleEvent("home");
//			}));

//			m_pBtnReplay.onClick.Add(new EventDelegate(delegate
//			{
//			QSoundMgr.Instance.PlayClipSync(SOUND.BTN);
				UIModel.Instance.fsm.HandleEvent("restart");
//			}));

//			m_pBtnShop.onClick.Add(new EventDelegate(delegate
//			{
//			QSoundMgr.Instance.PlayClipSync(SOUND.BTN);
//			SoundMgr.Instance.PlayMusic(MUSIC.HOME_BG);
				UIModel.Instance.fsm.HandleEvent("shop");
//			}));
		}

		public void UpdateView()
		{
//			meterLabel.SetNum(InfoManager.Instance.gameInfo.curMeter);
//			bestMeterLabel.SetNum(InfoManager.Instance.gameInfo.bestMeter);
//			coinLabel.SetNum(InfoManager.Instance.gameInfo.coin);
		}
	}
}