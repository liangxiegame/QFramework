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
	/// 游戏UI窗口
	/// </summary>
	public class GameWnd : QMonoBehaviour
	{
		public override IManager Manager
		{
			get { return UIManager.Instance; }
		}


//		private UIButton    m_pBtnPause;
//		public  NumberLabel meterLabel;

		public GameProgressView   gameProgress;
		public EnergyProgressView energyProgress;

		public UsingProps usingProps;

		// Use this for initialization
		void Awake()
		{
			// 注册到单例中
//		GameManager.Instance.uiCtrl.gameWnd = this;
//			m_pBtnPause = transform.Find("btn_pause").GetComponent<UIButton>();
//			meterLabel = transform.Find("meter").GetComponent<NumberLabel>();

			gameProgress = transform.Find("game_progress").GetComponent<GameProgressView>();
			energyProgress = transform.Find("energy_progress").GetComponent<EnergyProgressView>();
			usingProps = transform.Find("using_props").GetComponent<UsingProps>();
		}

		void Start()
		{
//			m_pBtnPause.onClick.Add(new EventDelegate(delegate
//			{
//			QSoundMgr.Instance.PlayClipSync(SOUND.BTN);
				UIModel.Instance.fsm.HandleEvent("pause");
//			}));
		}

		void onShow()
		{
			UpdateView();
		}

		public void UpdateView()
		{
			UpdateGameProgress();
			UpdateEnergyProgress();
			UpdateMeter();
		}

		public void UpdateGameProgress()
		{
			gameProgress.UpdateView();
		}

		public void UpdateEnergyProgress()
		{
			energyProgress.UpdateView();
		}

		public void UpdateMeter()
		{
//			meterLabel.SetNum(InfoManager.Instance.gameInfo.curMeter);
			UpdateGameProgress();
		}
	}
}