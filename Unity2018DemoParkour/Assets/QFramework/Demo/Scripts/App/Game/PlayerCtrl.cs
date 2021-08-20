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
	/// 角色控制器
	/// </summary>
	public class PlayerCtrl : QMonoBehaviour,IController
	{
		public Animator    Animator;
		public Rigidbody2D RigidBd;

		public SpriteRenderer SpriteRender;
//		public TweenAlpha     tweenAlpha;
//		public TweenScale     tweenScale;
		public Protect        Protect;
		public Transform      ShadowTrans;
		public ResLoader      mResLoader = ResLoader.Allocate();

		public override IManager Manager
		{
			get { return UIManager.Instance; }
		}

		private IPlayerModel mPlayerModel;
		
		

		public void OnGameStart()
		{
			transform.localPosition = new Vector3(3.0f, 5.0f, 0.0f);
			gameObject.SetActive(true);
			StartCoroutine(Init());

			mPlayerModel.FSM.Start("idle");
			mPlayerModel.FSM.HandleEvent("start");
		}

		protected override void ProcessMsg(int eventKey, QMsg msg)
		{
			switch (msg.EventID)
			{
				case (ushort) GameEvent.Pause:

					break;
				case (ushort) GameEvent.Resume:

					break;
				case (ushort) PlayerEvent.Land:
					mPlayerModel.FSM.HandleEvent("land");
					break;
			}
		}


		/// <summary>
		/// 各种道具的粒子特效
		/// </summary>
		public ParticleSystem LandEffectEmitter;

//		public ParticleSystem MagnetiteEffectEmitter;
//		public ParticleSystem AutoEffectEmmiter;

		/// <summary>
		/// 构造的时候将PlayerCtrl注册进GameManager中
		/// </summary>
		private void Awake()
		{
			// 注册进单例中
			AppLoader.Container.RegisterInstance(this);

			RegisterEvent(GameEvent.Pause);
			RegisterEvent(GameEvent.Resume);
			RegisterEvent(PlayerEvent.Land);

			mPlayerModel = this.GetModel<IPlayerModel>();
			
			gameObject.Hide();

			// 添加状态
			mPlayerModel.FSM.AddState("idle");
			mPlayerModel.FSM.AddState("run");
			mPlayerModel.FSM.AddState("jump");
			mPlayerModel.FSM.AddState("double_jump");
			mPlayerModel.FSM.AddState("die");

			// 添加跳转
			mPlayerModel.FSM.AddTranslation("idle", "start", "run", _ => GameStart());
			mPlayerModel.FSM.AddTranslation("run", "touch_down", "jump", _ => Jump());
			mPlayerModel.FSM.AddTranslation("jump", "touch_down", "double_jump", _ => DoubleJump());
			mPlayerModel.FSM.AddTranslation("jump", "land", "run", _ => Run());
			mPlayerModel.FSM.AddTranslation("double_jump", "land", "run", _ => Run());

			mPlayerModel.FSM.AddTranslation("run", "die", "die", _ => Death());
			mPlayerModel.FSM.AddTranslation("jump", "die", "die", _ => Death());
			mPlayerModel.FSM.AddTranslation("double_jump", "die", "die", _ => Death());

			mPlayerModel.FSM.AddTranslation("die", "start", "run", _ => GameStart());
			mPlayerModel.FSM.AddTranslation("jump", "start", "run", _ => GameStart());
			mPlayerModel.FSM.AddTranslation("double_jump", "start", "run", _ => GameStart());
			mPlayerModel.FSM.AddTranslation("run", "start", "run", _ => GameStart());

			mPlayerModel.FSM.Start("idle");
		}

		public IEnumerator Init()
		{
			Debug.Log("PlayerCtrl Init");
			gameObject.SetActive(true);
			transform.localPosition = new Vector3(-3.0f, 5.0f, 9.88f);
			// 获取组件引用
			Animator = GetComponent<Animator>();
			RigidBd = GetComponent<Rigidbody2D>();
			SpriteRender = GetComponent<SpriteRenderer>();
//			tweenAlpha = GetComponent<TweenAlpha>();
//			tweenScale = GetComponent<TweenScale>();

			ShadowTrans = transform.Find("shadow");
			ShadowTrans.gameObject.SetActive(false);

//			MagnetiteEffectEmitter = transform.Find("MagnetiteHeroEffect").GetComponent<ParticleSystem>();
//			AutoEffectEmmiter = transform.Find("AutoHeroFire").GetComponent<ParticleSystem>();
//			MagnetiteEffectEmitter.Pause();
//			AutoEffectEmmiter.Pause();

			var LandEffectEmitterPrefab = mResLoader.LoadSync<GameObject>("LANDEFFECT");
			var LandEffectEmitter = Instantiate(LandEffectEmitterPrefab);
			LandEffectEmitter.transform.SetParent(transform);
			this.LandEffectEmitter = LandEffectEmitter.GetComponent<ParticleSystem>();

			Protect = transform.Find("Shield").GetComponent<Protect>();
			Protect.Hide();

			yield return null;
		}

		/// <summary>
		/// 游戏开始
		/// </summary>
		void GameStart()
		{
			RigidBd.velocity = Vector2.zero;
			transform.localPosition = new Vector2(-3.0f, 1.0f);
			// 去掉这个后会没有状态机bug
//		animator.SetTrigger ("land");			
		}

		/// <summary>
		/// 碰撞器应该归谁管,给GameCtrl处理
		/// </summary>
		void OnTriggerEnter2D(Collider2D other)
		{
			Debug.Log("onTrigger" + other.gameObject.name);
			mPlayerModel.HandleTriggerEnterEvent(other.gameObject);

			if (other.gameObject.name == "DeathZone")
			{
				TypeEventSystem.Global.Send<GameOverEvent>();
			}
		}

		/// <summary>
		/// 跑动
		/// </summary>
		void Run()
		{
			Animator.SetTrigger("land");

			if (PropModel.Instance.prop_auto_on)
			{
			}
			else
			{
				ShadowTrans.gameObject.SetActive(true);
				PlayLandEffect();
			}
		}

		/// <summary>
		/// 第一次跳跃
		/// </summary>
		void Jump()
		{
			RigidBd.velocity = Vector2.zero;

			RigidBd.AddForce(new Vector2(0,  mPlayerModel.CurJumpPower.Value * 1.3f));
	//		QSoundMgr.Instance.PlayClipAsync (SOUND.JUMP);
			Animator.SetTrigger("jump");
			ShadowTrans.gameObject.SetActive(false);
		}

		/// <summary>
		/// 二段跳
		/// </summary>
		void DoubleJump()
		{
			RigidBd.velocity = Vector2.zero;

			RigidBd.AddForce(new Vector2(0,   mPlayerModel.CurJumpPower.Value * 1.0f));

	//		QSoundMgr.Instance.PlayClipAsync (SOUND.JUMP);
			Animator.SetTrigger("jump");
		}

		/// <summary>
		/// 死亡
		/// </summary>
		void Death()
		{
//		QSoundMgr.Instance.PlayClipAsync (SOUND.DEATH);
			gameObject.SetActive(false);
		}

		/// <summary>
		/// 这里直接处理碰撞
		/// </summary>
		void OnCollisionEnter2D(Collision2D other)
		{

			if (other.transform.CompareTag("block_air") || other.transform.CompareTag("block_bottom"))
			{
				mPlayerModel.FSM.HandleEvent("land");
				if (PropModel.Instance.prop_big_on && RigidBd.velocity.y < -9.0f)
				{
					GameManager.Instance.stageCtrl.Shake();
				}
			}
			else if (other.transform.CompareTag("enemy_crab") || other.transform.CompareTag("enemy_turtle"))
			{
				if (PropModel.Instance.prop_auto_on || PropModel.Instance.prop_protect_on)
				{
					other.transform.GetComponent<Enemy>()
						.Kick(this.transform.position - other.transform.transform.position);
					return;
				}

				if (PropModel.Instance.prop_big_on)
				{
					other.transform.GetComponent<Enemy>()
						.Kick(other.transform.position - this.transform.position);
				}
				else
				{
					other.transform.GetComponent<Enemy>().Hit();
					GameManager.Instance.SendMsg(new MsgWithInt((ushort) GameEvent.EnergyAdd, -300));

//					#region 水果分数值

//					var fruitValue = PoolManager.Pools["Number"].Spawn("FruitValue");
//					var fruitValueScript = fruitValue.GetComponent<FruitValue>();
//					fruitValueScript.ResetFruitValue();
//					fruitValueScript.Plus();
//					fruitValue.parent = transform.parent;
//					fruitValue.localPosition = transform.localPosition;
//					fruitValue.localRotation = transform.localRotation;
//					fruitValue.parent = null;
//					fruitValue.localScale = Vector3.one;
//					float duration = 0.8f;
//					fruitValueScript.number.SetNum(300);
//					fruitValueScript.Minus();
//					fruitValueScript.Execute();
//
//					this.Delay(duration, () => { PoolManager.Pools["Number"].Despawn(fruitValue); });
//
//					#endregion


//				GameManager.Instance.uiCtrl.UpdateEnergy ();
				}
			}

		}

		protected override void OnBeforeDestroy()
		{
			mResLoader.Recycle2Cache();
			mResLoader = null;

			mPlayerModel = null;
		}

		/*-----------------------------------------以下是道具效果-----------------------------------*/
		/// <summary>
		/// 开始闪烁
		/// </summary>
		public void BlinkBegin()
		{
//			tweenAlpha.enabled = true;
//			tweenAlpha.ResetToBeginning();
//			tweenAlpha.PlayForward();
		}

		/// <summary>
		/// 结束闪烁
		/// </summary>
		public void BlinkEnd()
		{
//			tweenAlpha.enabled = false;
			SpriteRender.color = Color.white;
		}

		/// <summary>
		/// 开始变大
		/// </summary>
		public void BigBegin()
		{
//			tweenScale.enabled = false;
//			tweenScale.enabled = true;

//			tweenScale.from = new Vector3(1.0f, 1.0f, 1.0f);
//			tweenScale.to = new Vector3(1.5f, 1.5f, 0.0f);
//			tweenScale.ResetToBeginning();

//			tweenScale.PlayForward();
		}

		/// <summary>
		/// 变大结束
		/// </summary>
		public void BigEnd()
		{
//			tweenScale.from = new Vector3(1.5f, 1.5f, 1.0f);
//			tweenScale.to = new Vector3(1.0f, 1.0f, 1.0f);
//			tweenScale.ResetToBeginning();
//			tweenScale.PlayForward();
		}

		/// <summary>
		/// 重置大小
		/// </summary>
		public void BigReset()
		{
//			tweenScale.enabled = false;
		}

		/*---------------------------粒子效果相关----------------------------*/
		/// <summary>
		/// 重置落地的粒子效果,海南的说 落地的效果先不要了。
		/// </summary>
		public void ResetLandEffect()
		{
//		landEffectEmitter.emit = false;
//		landEffectEmitter.transform.parent = transform.parent;
//
//		if (PropModel.Instance.prop_big_on) {
//			QTrans.SetPosXY (landEffectEmitter.transform, QTrans.PosX (trans), QTrans.PosY (trans) + -0.64f + -0.32f);
//		} else {
//			QTrans.SetPosXY (landEffectEmitter.transform, QTrans.PosX(trans), QTrans.PosY(trans) +  -0.64f);
//		}
		}

		/// <summary>
		/// 播放落地的例子效果,海南的说 不要了
		/// </summary>
		void PlayLandEffect()
		{

//		ResetLandEffect ();
//
//		if (StageModel.Instance.Switching)
//			return;
//		
//		var emitterTrans = landEffectEmitter.transform;
//
//		emitterTrans.parent = GameManager.Instance.stageCtrl.StageB;
//
//		landEffectEmitter.Emit (15);
//
//		QTimer.ExecuteAfterSeconds (1.01f, delegate {
//			ResetLandEffect();
//		});
		}
		/*--------------------------- End ----------------------------*/


		/// <summary>
		/// 关闭效果
		/// </summary>
		public void ResetPlayer()
		{
			BlinkEnd();
			BigReset();
			transform.LocalScale(1.0f, 1.0f);
//			MagnetiteEffectEmitter.Pause();
//			AutoEffectEmmiter.Pause();
			Protect.Hide(); // 隐藏掉保护膜
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			mPlayerModel = null;
		}

		public IArchitecture GetArchitecture()
		{
			return PlatformRunner.Interface;
		}
	}
}