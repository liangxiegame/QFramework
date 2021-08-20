/****************************************************************************
 * Copyright (c) 2016.3 ~ 2021.8 liangxie
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
    public enum StageEvent
    {
        Began = GameEvent.End,
        ResetStage,
        BeginScroll,
        SwitchEnd,
        ResetData,
        End
    }

    /// <summary>
    /// 负责管理关卡
    /// </summary>
    public class StageManager : QMonoBehaviour,ISingleton,IController
	{
		public StageMonoBehviour LoadStage(string stageName)
		{
			var theme = this.GetModel<IGameModel>().CurTheme.Value;

			return stageName == "Stage"
				? mResLoader.LoadSync<GameObject>(stageName).Instantiate().GetComponent<StageMonoBehviour>()
				: mResLoader.LoadSync<GameObject>("Stage" + theme, stageName).Instantiate()
					.GetComponent<StageMonoBehviour>();
		}

		public static StageManager Instance => MonoSingletonProperty<StageManager>.Instance;

		public override IManager Manager => GameManager.Instance;

        public Transform StagePattern; // 
	//	public TweenPosition shakeEffect;	// 震动效果

		public StageMonoBehviour StageA;
		public StageMonoBehviour StageB;

		public float ThemeLength;

		public GameObject AutoBlock; // 不死的块,对应prop_auto道具

		public float meterCounter = 0.0f;

		bool scrollable = false;

        ResLoader mResLoader = ResLoader.Allocate();

        private IPlayerModel mPlayerModel;
        private IGameModel mGameModel;

        private void Awake()
        {
            this.LocalPosition(new Vector3(0.0f, 0.0f, 8.88f));

            AutoBlock = new GameObject("AutoBlock")
                .transform
                .Parent(transform)
                .gameObject
                .AddComponent<BoxCollider2D>()
                .ApplySelfTo(boxCollider2D =>
                {
                    boxCollider2D.size = new Vector2(23.09f, 1);
                    boxCollider2D.offset = new Vector2(0, -1.1f);
                }).gameObject
                .Hide();


            RegisterEvent(StageEvent.ResetStage);
            RegisterEvent(StageEvent.BeginScroll);
            RegisterEvent(StageEvent.SwitchEnd);
            RegisterEvent(StageEvent.ResetData);

            mPlayerModel = this.GetModel<IPlayerModel>();
            mGameModel = this.GetModel<IGameModel>();

//		shakeEffect = GetComponent<TweenPosition> ();
        }

        protected override void ProcessMsg(int eventId, QMsg msg)
        {
            if (eventId == (int)StageEvent.ResetStage)
            {

            }
            else if (eventId == (int)StageEvent.BeginScroll)
            {
                scrollable = true;
            }
            else if (eventId == (int)StageEvent.SwitchEnd)
            {
                SwitchEnded();
            }
            else if (eventId == (int)StageEvent.ResetData)
            {
                ResetData();
            }
        }

        /// <summary>
        /// 进入场景后第一次进行加载
        /// </summary>
        /// <returns>The stage.</returns>
        /// <param name="gamePlayCtrl">Game play ctrl.</param>
        internal void ResetStage(Game gamePlayCtrl)
        {
            //      InfoManager.Instance.gameInfo.curTheme 
            ResetData();

            Switch(mGameModel.CurTheme.Value, gamePlayCtrl);
        }

        /// <summary>
        /// 重置Layer 重新玩时候调用
        /// </summary>
        private void ResetData()
		{
			DestroyStageAB();

            // 初始化数据
            meterCounter = 0.0f;
            scrollable = false;

            this.SendMsg(new MsgWithInt((ushort)GameEvent.SetMeter, 0));

            mGameModel.CurTheme.Value = 1;

            ThemeLength = StageModel.Instance.ThemeLength;
            //      GameManager.Instance.uiCtrl.UpdateMeter ();

            GenStageAB();
        }

		/// <summary>
		/// 回收掉StageAB
		/// </summary>
		public void DestroyStageAB()
		{
			if (StageA)
			{
                StageA.DestroyGameObj();
				StageA = null;
			}

			if (StageB)
			{
                StageB.DestroyGameObj();
				StageB = null;
			}

			scrollable = false;
		}

		/// <summary>
		/// 生成一个地形 以后要干掉
		/// </summary>
		private void GenStage()
		{
            var stageTrans = LoadStage(StageModel.Instance.GetStageName());
                                       
		    var stageInterface = stageTrans.GetComponent<StageMonoBehviour> ();

            stageTrans.Parent(this)
                      .Identity()
                      .PositionX(StageA.GetPosition().x + ThemeLength);

		    GameManager.Instance.StartCoroutine(stageInterface.Parse (StageModel.Instance.GetStageData ()));

		    StageB = stageInterface;
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

			StageA.transform.Translate(Vector3.left * mPlayerModel.CurSpeed.Value * Time.deltaTime, Space.World);
            StageB.transform.Translate(Vector3.left * mPlayerModel.CurSpeed.Value * Time.deltaTime, Space.World);

			if (StageA.transform.localPosition.x <= -ThemeLength)
			{
                StageA.DestroyGameObj();
                StageA = null;
				StageA = StageB;
				StageB = null;
				GenStage();
			}
		}

		/// <summary>
		/// 开始转换到下一个场景
		/// </summary>

        public IEnumerator SwitchBegan(int nextStage, bool isEnterGame, Game gamePlayCtrl)
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



            GameManager.Instance.SendMsg(new MsgWithInt((ushort)GameEvent.EnergyAdd, 1000));
            //			GameManager.Instance.uiCtrl.UpdateEnergy();

            // 恢复加载
            Time.timeScale = currentTimeScale;

            Switch(nextStage, gamePlayCtrl);

        }

		/// <summary>
		/// 开始转换
		/// </summary>
        public void Switch(int nextStage, Game gamePlayCtrl)
		{
			//yield return GameManager.Instance.StartCoroutine(
				//GameManager.Instance.bgCtrl.Switch(nextStage, gamePlayCtrl)); // 开始更换背景

			// 开始替换跑道 还要确定是否
			if (StageA)
			{
				GameManager.Instance.StartCoroutine(StageA.FadeOut());
			}

			if (StageB)
			{
				GameManager.Instance.StartCoroutine(StageB.FadeOut());
			}

            this.SendMsg(new MsgWithInt((ushort) InfoEvent.SetTheme, nextStage));
			ThemeLength = StageModel.Instance.ThemeLength;

			GenStageAB();

			SwitchEnded();
		}

        /// <summary>
        /// 结束转换
        /// </summary>
        private void SwitchEnded()
        {
            // 移走背景
            //GameManager.Instance.bgCtrl.SwitchEnd();

            StageModel.Instance.Switching = false;

            if (PropModel.Instance.prop_auto_on)
            {
            }
            else
            {
                AutoBlock.Hide();
            }
        }

		/// <summary>
		/// 同时生成两个
		/// </summary>
		void GenStageAB()
		{
            //		StageModel.Instance.ParseStagePattern (GoManager.Instance.StageSpawn ("StagePattern"));

            StageA = LoadStage("loop");
            StageB = LoadStage("loop");

			Debug.Log(StageA + ":" + StageB);

            StageA.Parent(transform)
                  .LocalIdentity();

            StageB.Parent(transform)
                  .LocalIdentity()
                  .LocalPositionX(ThemeLength);
		}

		/******以下是道具效果********/

		/// <summary>
		/// Big道具
		/// </summary>
		public void BigReset()
		{
			if (StageA)
			{
				StageA.ResetAirBlock();
			}

			if (StageB)
			{
				StageB.ResetAirBlock();
			}
		}

		/// <summary>
		/// 落地之后要有震动效果
		/// </summary>
		public void Shake()
		{
			StartCoroutine(Shaking());
//		QSoundMgr.Instance.PlayClipAsync (SOUND.SHAKE);

			if (StageA)
			{
				StageA.Shake();
			}

			if (StageB)
			{
				StageB.Shake();
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

        public void OnSingletonInit()
        {

        }

        public IArchitecture GetArchitecture()
        {
	        return PlatformRunner.Interface;
        }
	}
}