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
using System.Collections;

namespace QFramework.PlatformRunner
{
	/// <summary>
	/// 负责管理控制器。
	/// </summary>
	public class StageCtrl : ViewController,IController
	{
		public Transform StagePattern; // 
//	public TweenPosition shakeEffect;	// 震动效果

		public Transform StageA;
		public Transform StageB;

		public float ThemeLength;

		public GameObject AutoBlock; // 不死的块,对应prop_auto道具

		public float meterCounter = 0.0f;

		bool scrollable = false;
		
		
		ResLoader mResLoader = ResLoader.Allocate();

		private IPlayerModel mPlayerModel;  

		private void Awake()
		{
			this.LocalPosition(0.0f, 0.0f, 8.88f);

			StageModel.Instance.InitModel();

			AutoBlock = transform.Find("AutoBlock").gameObject;
			AutoBlock.SetActive(false);

			mPlayerModel = this.GetModel<IPlayerModel>();

//		shakeEffect = GetComponent<TweenPosition> ();
		}

		/// <summary>
		/// 初始化层 一般第一次进入游戏时候会调用
		/// </summary>
		void InitLayer()
		{
			// 初始化数据
			meterCounter = 0.0f;
			scrollable = false;

			GameManager.Instance.SendMsg(new MsgWithInt((ushort) GameEvent.SetMeter, 0));
			this.GetModel<IGameModel>().CurTheme.Value = 1;

			ThemeLength = StageModel.Instance.ThemeLength;
//		GameManager.Instance.uiCtrl.UpdateMeter ();

			GenStageAB();
		}

		/// <summary>
		/// 重置Layer 重新玩时候调用
		/// </summary>
		public IEnumerator ResetLayer()
		{
			yield return DespawnStageAB();

			InitLayer();
		}

		/// <summary>
		/// 回收掉StageAB
		/// </summary>
		public IEnumerator DespawnStageAB()
		{
			if (StageA != null)
			{
				yield return StageA.GetComponent<IStage>().Despawn();
				StageA = null;
			}

			if (StageB != null)
			{
				yield return StageB.GetComponent<IStage>().Despawn();
				StageB = null;
			}

			scrollable = false;
		}

		/// <summary>
		/// 开始滚动
		/// </summary>
		public void BeginScroll()
		{
			scrollable = true;
		}

		/// <summary>
		/// 生成一个地形 以后要干掉
		/// </summary>
		private void GenStage()
		{
			
			
//		var stageTrans = GoManager.Instance.StageSpawn (model.GetStageName());

//		GameManager.Instance.StartCoroutine(stageInterface.Parse (StageModel.Instance.GetStageData ()));

			var stageName = StageModel.Instance.GetStageName();
			
			Debug.Log(stageName);
			
			StageB = LoadStage("stage1", stageName, StageA.GetPosition().x + ThemeLength);
		}

		/// <summary>
		/// 更新位置
		/// </summary>
		public void Update()
		{
			if (scrollable && !StageModel.Instance.Switching)
			{

			}
			else
			{
				return;
			}

			// 计算米数 玩家的速度
			meterCounter += Time.deltaTime * mPlayerModel.CurSpeed.Value / (ThemeLength * 0.1f);

			if (meterCounter >= 1.0f)
			{
				meterCounter -= 1.0f;

				GameManager.Instance.SendMsg(new MsgWithInt((ushort) GameEvent.MeterAdd, 1));
				
				if (PropModel.Instance.prop_auto_on)
				{

				}
				else
				{
					GameManager.Instance.SendMsg(new MsgWithInt((ushort) GameEvent.EnergyAdd, -5));
//				GameManager.Instance.uiCtrl.UpdateEnergy ();
				}

//			GameManager.Instance.uiCtrl.UpdateMeter ();
			}

			StageA.Translate( mPlayerModel.CurSpeed.Value * Time.deltaTime * Vector3.left, Space.World);
			StageB.Translate(mPlayerModel.CurSpeed.Value  * Time.deltaTime * Vector3.left, Space.World);

			if (StageA.localPosition.x <= -ThemeLength)
			{
				GameManager.Instance.StartCoroutine(StageA.GetComponent<IStage>().Despawn());
				StageA = null;
				StageA = StageB;
				StageB = null;
				GenStage();
			}
		}

		/// <summary>
		/// 开始转换到下一个场景
		/// </summary>
		Coroutine switchCoroutine = null;

		public IEnumerator SwitchBegan(int nextStage, bool isEnterGame, GamePlayCtrl gamePlayCtrl)
		{

			if (PropModel.Instance.prop_auto_on)
			{
			}
			else
			{
				AutoBlock.SetActive(true); // 确保玩家不会死亡
			}

			// 遮罩移动到中央 耗时0.5秒
			GameManager.Instance.bgCtrl.SwitchBegan(nextStage, isEnterGame);
			yield return new WaitForSeconds(0.5f);

			StageModel.Instance.Switching = true;

			float currentTimeScale = Time.timeScale;
			Time.timeScale = 0;

			// 开始加载资源
			GoManager.Instance.LoadStagePool(nextStage, gamePlayCtrl, delegate
			{

				GameManager.Instance.SendMsg(new MsgWithInt((ushort) GameEvent.EnergyAdd, 1000));
//			GameManager.Instance.uiCtrl.UpdateEnergy();

				// 到这里资源已经加载好了
				if (switchCoroutine != null)
				{
					GameManager.Instance.StopCoroutine(switchCoroutine);
					switchCoroutine = null;
				}

				// 恢复加载
				Time.timeScale = currentTimeScale;

				switchCoroutine = GameManager.Instance.StartCoroutine(Switch(nextStage));
			});
		}

		/// <summary>
		/// 开始转换
		/// </summary>
		public IEnumerator Switch(int nextStage)
		{

			yield return GameManager.Instance.StartCoroutine(
				GameManager.Instance.bgCtrl.Switch(nextStage)); // 开始更换背景

			// 开始替换跑道 还要确定是否
			if (StageA)
			{
				StartCoroutine(StageA.GetComponent<IStage>().FadeOut());
			}

			if (StageB)
			{
				StartCoroutine(StageB.GetComponent<IStage>().FadeOut());
			}

			// Model.GameModel.Dispatch("setTheme", nextStage);

			ThemeLength = StageModel.Instance.ThemeLength;

			GenStageAB();

			yield return new WaitForSeconds(0.1f);
			SwitchEnded(nextStage);
		}

		/// <summary>
		/// 结束转换
		/// </summary>
		public void SwitchEnded(int nextStage)
		{
			// 移走背景
			GameManager.Instance.bgCtrl.SwitchEnd();

			StageModel.Instance.Switching = false;

			if (PropModel.Instance.prop_auto_on)
			{
			}
			else
			{
				AutoBlock.SetActive(false);
			}

			switchCoroutine = null;
		}

		/// <summary>
		/// 同时生成两个
		/// </summary>
		void GenStageAB()
		{
			StageA = mResLoader.LoadSync<GameObject>("stage1", "loop")
				.Instantiate()
				.transform
				.Parent(transform)
				.LocalIdentity();
			
			StageB = mResLoader.LoadSync<GameObject>("stage1", "loop")
				.Instantiate()
				.transform
				.Parent(transform)
				.LocalIdentity()
				.LocalPositionX(ThemeLength);
			
			Debug.Log(StageA + ":" + StageB);
		}

		Transform LoadStage(string themeName, string stageName,float initPosX = 0)
		{
			return mResLoader.LoadSync<GameObject>(themeName, stageName)
				.Instantiate()
				.transform
				.Parent(transform)
				.LocalIdentity()
				.LocalPositionX(initPosX);
		}

		/******以下是道具效果********/

		/// <summary>
		/// Big道具
		/// </summary>
		public void BigReset()
		{
			if (StageA != null)
			{
				StageA.GetComponent<IStage>().ResetAirBlock();
			}

			if (StageB != null)
			{
				StageB.GetComponent<IStage>().ResetAirBlock();
			}
		}

		/// <summary>
		/// 落地之后要有震动效果
		/// </summary>
		public void Shake()
		{
			GameManager.Instance.StartCoroutine(Shaking());
//		QSoundMgr.Instance.PlayClipAsync (SOUND.SHAKE);

			if (StageA != null)
			{
				StageA.GetComponent<IStage>().Shake();
			}

			if (StageB != null)
			{
				StageB.GetComponent<IStage>().Shake();
			}
		}

		/// <summary>
		/// 震动开始
		/// </summary>
		public IEnumerator Shaking()
		{
			if (bossShaking)
				yield return 0;
//		shakeEffect.enabled = false;
//		shakeEffect.from = Vector3.up * 0.05f;
//		shakeEffect.to = Vector3.down * 0.05f;
//		shakeEffect.style = UITweener.Style.PingPong;
//		shakeEffect.ignoreTimeScale = true;
//		shakeEffect.duration = 0.01f;
//		shakeEffect.enabled = true;
//		shakeEffect.ResetToBeginning ();
//		shakeEffect.PlayForward ();

//		yield return new WaitForSeconds (0.4f);
//		if (bossShaking) {
//		}
//		else {
//			shakeEffect.enabled = false;

//			transform.localPosition = Vector2.zero;
//		}
		}

		bool bossShaking = false;

		public void BossShakeBegan()
		{
//		QSoundMgr.Instance.PlayClipAsync (SOUND.BOSS_SHAKE,true);

			bossShaking = true;
//		shakeEffect.enabled = false;
//		shakeEffect.from = Vector3.up * 0.05f;
//		shakeEffect.to = Vector3.down * 0.05f;
//		shakeEffect.style = UITweener.Style.PingPong;
//		shakeEffect.ignoreTimeScale = true;
//		shakeEffect.duration = 0.01f;
//		shakeEffect.enabled = true;
//		shakeEffect.ResetToBeginning ();
//		shakeEffect.PlayForward ();
		}

		public void BossShakeEnd()
		{
//		QSoundMgr.Instance.StopClip (SOUND.BOSS_SHAKE);
			bossShaking = false;

//		shakeEffect.enabled = false;

			transform.localPosition = Vector2.zero;
		}


		/// <summary>
		/// 执行吸铁石操作 不用了先
		/// </summary>
		public void MagnetiteOn()
		{
//		if (StageA != null) {
//			StageA.GetComponent<IStage> ().MagnetiteExecute ();
//		}

//		if (StageB != null) {
//			StageB.GetComponent<IStage> ().MagnetiteExecute ();
//		}
		}

		private void OnDestroy()
		{
			mResLoader.Recycle2Cache();
			mResLoader = null;

			mPlayerModel = null;
		}

		public IArchitecture GetArchitecture()
		{
			return PlatformRunner.Interface;
		}
	}
}