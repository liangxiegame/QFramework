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

using System.Collections;
using UnityEngine;

namespace QFramework.PlatformRunner
{
	public partial class UIGamePanel : UIPanel,IController
	{
		GameProgressView   mGameProgress;
		EnergyProgressView mEnergyProgress;
		UsingProps         mUsingProps;
		
		protected override void OnClose()
		{
			
		}

		protected override void OnInit(IUIData uiData = null)
		{

			InfoManager.Instance.Init();

			mGamePlayCtrl = GamePlayCtrl.Load();

			GoManager.Instance.LoadStagePool(this.GetModel<IGameModel>().CurTheme.Value, mGamePlayCtrl,
				() => { StartCoroutine(LoadStage()); });

//			mGameProgress = mUIComponents.GameProgress_Slider.GetComponent<GameProgressView>();
//			mEnergyProgress = mUIComponents.EnergyProgress_Slider.GetComponent<EnergyProgressView>();
//			mUsingProps = UsingProps.GetComponent<UsingProps>();

			TypeEventSystem.Global.Register<GameOverEvent>(e =>
			{
				CloseSelf();
				UIKit.OpenPanel<UIHomePanel>();
			}).UnRegisterWhenGameObjectDestroyed(gameObject);
			
			BtnPause.onClick.AddListener(() =>
			{
//			QSoundMgr.Instance.PlayClipSync(SOUND.BTN);
				Log.I("click");
				UIKit.OpenPanel<UIPausePanel>(UILevel.PopUI);
				UIModel.Instance.fsm.HandleEvent("pause");
			});
			
		}

		private GamePlayCtrl mGamePlayCtrl;


		IEnumerator LoadStage()
		{
//			yield return mGamePlayCtrl.StageCtrl.DespawnStageAB();
//			yield return mGamePlayCtrl.StageCtrl.DespawnStageAB();
////		InfoManager.Instance.gameInfo.curTheme 
//			yield return mGamePlayCtrl.StageCtrl.ResetLayer();
//
//			yield return mGamePlayCtrl.StageCtrl.Switch(InfoManager.Instance.gameInfo.curTheme, mGamePlayCtrl);
//
//			mGamePlayCtrl.PlayerCtrl.OnGameStart();
//
//			mGamePlayCtrl.StageCtrl.BeginScroll();
////		uiCtrl.uiFade.FadeOut();
//			mGamePlayCtrl.StageCtrl.SwitchEnded(InfoManager.Instance.gameInfo.curTheme);
yield break;
		}
		
		protected override void OnShow()
		{
		}

		protected override void OnHide()
		{
		}

		private void ShowLog(string content)
		{
			Debug.Log("[UIGameLayer:]" + content);
		}


//		public NumberLabel meterLabel;

		public GameProgressView   gameProgress;
		public EnergyProgressView energyProgress;

		public UsingProps usingProps;


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

		private void UpdateGameProgress()
		{
			gameProgress.UpdateView();
		}

		private void UpdateEnergyProgress()
		{
			energyProgress.UpdateView();
		}

		private void UpdateMeter()
		{
//			meterLabel.SetNum(InfoManager.Instance.gameInfo.curMeter);
			UpdateGameProgress();

		}

		public IArchitecture GetArchitecture()
		{
			return PlatformRunner.Interface;
		}
	}
}