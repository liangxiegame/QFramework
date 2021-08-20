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

using UnityEngine;
using System.Collections;

namespace QFramework.PlatformRunner
{
/*
 *  1.是游戏的入口
 *  2.不同功能模块的通信的中转站（当然用发消息的模式可能更好一些)
 *  3.处理游戏的一些挂起,进入后台等待特殊事件。
 *  4.一些资源的预加载,释放资源
 *  5.游戏的主线程 控制网络消息和逻辑消息的调用(只负责调用不负责处理,当然这种写法也不是很好)
 *  6.大模块跳转,HOME->GAME,GAME->HOME
 */

	/// <summary>
	/// 管理所有的控制器,资源的加载 场景的切换都在左立做
	/// </summary>

	// 给的职权太大了,要重构
	public class GameManager : QMgrBehaviour, ISingleton,IController
	{
		public override int ManagerId
		{
			get { return QMgrID.Game; }
		}

		public static GameManager Instance
		{
			get { return MonoSingletonProperty<GameManager>.Instance; }
		}

		/// <summary>
		/// 各种模块控制器
		/// </summary>
		public StageCtrl stageCtrl;

		public BgCtrl     bgCtrl;
		public CameraCtrl cameraCtrl;

		/// <summary>
		/// 初始化
		/// </summary>
		public new IEnumerator Init()
		{
			// 初始化内存数据,可更改的数据
			InfoManager.Instance.Init();

			// 音频资源加载
			yield return SoundManager.Instance.Init();

			yield return null;
		}

		public IEnumerator ResetGameUI()
		{

			// TODO:以上改成这句话
//		UGUIManager.Instance.GetLayer(UIName.GameLayer).gameProgress.ResetView(); 
			bgCtrl.Idle();

			yield return null;
		}

		/// <summary>
		/// 从Home进入游戏
		/// TODO: 这块太不清晰了 要重构下
		/// </summary>
		public IEnumerator EnterGame()
		{
			yield return ResetGameUI();

			this.SendMsg(new QMsg((ushort) InfoEvent.Reset));

//			
//		// 重置乱七八糟的东西
//		uiCtrl.ResetProp ();
			PropModel.Instance.InitModel();
//
			StageModel.Instance.Switching = true;
//
//		// 遮罩移动到中央 耗时0.5秒
			yield return bgCtrl.SwitchBegan(this.GetModel<IGameModel>().CurTheme.Value, true);
			StageModel.Instance.Switching = true;



			stageCtrl.BeginScroll();
//
//		uiCtrl.uiFade.FadeOut();
//
			stageCtrl.SwitchEnded(this.GetModel<IGameModel>().CurTheme.Value);

			yield return 0;
		}

		/// <summary>
		/// 从Game进入Home
		/// </summary>
		public IEnumerator EnterHome()
		{
			Debug.LogWarning("@@@@@@@@@@@@@@@");
			SendMsg(new AudioMusicMsg("HOME"));
			yield return new WaitForEndOfFrame();
			Time.timeScale = 0.0f;
		}

		/// <summary>
		/// Games the restart.
		/// </summary>
		public void GameRestart()
		{
//		QSoundMgr.Instance.PlayMusic(QAB.SOUND.GAME);
			stageCtrl.DespawnStageAB(); // 回收掉当前的关卡
			StartCoroutine(EnterGame());
		}

		void OnApplicationQuit()
		{
			InfoManager.Instance.SaveAllInfos();
		}

		public void OnSingletonInit()
		{
		}

		public IArchitecture GetArchitecture()
		{
			return PlatformRunner.Interface;
		}
	}
}