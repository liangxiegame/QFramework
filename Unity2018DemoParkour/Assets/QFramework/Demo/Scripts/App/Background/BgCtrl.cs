/****************************************************************************
 * Copyright (c) 2018.7 liangxie
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
	using UnityEngine;
	using System.Collections;
	using QFramework;

	/// <summary>
	/// 负责场景的切换。背景资源的卸载等。
	/// </summary>
	public enum ArrowState
	{
		Shooting,
		None,
	}

	public class BgCtrl : QMonoBehaviour
	{
		public override IManager Manager
		{
			get { return GameManager.Instance; }
		}

		private Transform  SwitchTrans; // 这个是转换的大树
		private BossSun    bossSun;
		private BossSun    bossMoon;
		private ArrowState arrowState = ArrowState.None;

		public Transform CurBgTrans;

//	SpawnPool bossPool;

		void Awake()
		{
			// 注册到游戏管理中
			GameManager.Instance.bgCtrl = this;

//		FadeBlackTrans = transform.Find ("FadeBlack");
//			SwitchTrans = transform.Find("SwitchTex").Hide();
//
//			bossSun = transform.Find("BossSun").GetComponent<BossSun>();
//			bossMoon = transform.Find("BossMoon").GetComponent<BossSun>();
//
//			bossSun.Hide();
//			bossMoon.Hide();
		}

		void Start()
		{
			transform.localPosition = new Vector3(0, 0, 2);

		}

		public void Launch()
		{
//		bossPool = PoolManager.Pools["Boss"];
		}


		Coroutine coArrow = null;

		public void SunAppear()
		{
			bossSun.Show();
			bossSun.Appear();
			arrowState = ArrowState.Shooting;

			Invoke("RespawnArrow", 1.0f);

			GameManager.Instance.stageCtrl.BossShakeBegan();
		}

		/// <summary>
		/// 关闭射箭
		/// </summary>
		void RespawnArrow()
		{
			StopSpawnArrow();

			if (arrowState == ArrowState.None) return;
			coArrow = StartCoroutine(SpawnArrow());
		}

		void StopSpawnArrow()
		{
			if (coArrow != null)
			{
				StopCoroutine(coArrow);
				coArrow = null;
			}

//		if (bossPool == null)
//		{
//			Launch();
//		}
//
//		bossPool.DespawnAll();
		}

		public void MoonAppear()
		{
			bossMoon.Show();
			bossMoon.Appear();
			arrowState = ArrowState.Shooting;

			Invoke("RespawnArrow", 1.0f);

			GameManager.Instance.stageCtrl.BossShakeBegan();
		}

		public void SunDied()
		{
			bossSun.Died();

			StopSpawnArrow();

//		GameManager.Instance.uiCtrl.gameWnd.gameProgress.RemainSun--;
			GameManager.Instance.stageCtrl.BossShakeEnd();
		}

		public void MoonDied()
		{
			bossMoon.Died();

			if (coArrow != null)
			{
				StopCoroutine(coArrow);
				coArrow = null;
			}

//		GameManager.Instance.uiCtrl.gameWnd.gameProgress.RemainMoon--;
			GameManager.Instance.stageCtrl.BossShakeEnd();
		}

		public void Idle()
		{
			if (coArrow != null)
			{
				StopCoroutine(coArrow);
				coArrow = null;
			}

			arrowState = ArrowState.None;
			bossMoon.Hide();
			bossSun.Hide();
			GameManager.Instance.stageCtrl.BossShakeEnd();
		}

		IEnumerator SpawnArrow()
		{
			yield return new WaitForSeconds(Random.Range(0.5f, 1.0f));
//		var ArrowTrans = bossPool.Spawn("Arrow");
//		ArrowTrans.parent = transform;
//		ArrowTrans.Identity();
//		ArrowTrans.localPosition = Vector3.left * 10;
//		ArrowTrans.GetComponent<ICmd>().Execute();
//		coArrow = StartCoroutine(SpawnArrow());
		}

		/// <summary>
		/// 开始转换,主要是背景部分的
		/// </summary>
		public IEnumerator SwitchBegan(int theme, bool isEnterGame)
		{
			Debug.Log("switch");
			SwitchTrans.PositionX(isEnterGame ? 0.0f : 15.0f);

//		QTween.MoveTo(SwitchTrans, 0.5f, Vector3.zero); // 开始转换

			yield return new WaitForSeconds(0.5f);
		}

		public void UpdateView()
		{

		}

		/// <summary>
		/// 转换背景的遮罩过程。
		/// </summary>
		/// <param name="theme">Theme.</param>
		public IEnumerator Switch(int theme)
		{

			if (CurBgTrans != null)
			{
				var bgInternface = CurBgTrans.GetComponent<IBg>();
				bgInternface.Despawn();
				CurBgTrans = null;
			}

//		var bgTrans = PoolManager.Pools["Stage" + theme].Spawn("Bg" + theme);
//		bgTrans.parent = transform;
//
//		var newBgInterface = bgTrans.GetComponent<IBg>();
//		newBgInterface.InitBg();
//
//		CurBgTrans = bgTrans;


			var cameraCtrl = AppLoader.Container.Resolve<CameraCtrl>();
			
			if (theme == 3)
			{
				cameraCtrl.Sun();
//			QSoundMgr.Instance.StopSound(SOUND.FIRE);
//			QSoundMgr.Instance.StopSound(SOUND.FOREST);
//			QSoundMgr.Instance.PlayClipSync(SOUND.FIRE);
			}
			else
			{
				if (Random.Range(0, 10) < 3)
				{
					cameraCtrl.Rain();
				}
				else
				{
					cameraCtrl.Idle();
//				QSoundMgr.Instance.StopSound(SOUND.FIRE);
//				QSoundMgr.Instance.StopSound(SOUND.FOREST);
//				QSoundMgr.Instance.PlayClipSync(SOUND.FOREST);
				}
			}

			yield return 0;
		}

		/// <summary>
		/// 结束转换
		/// </summary>
		public void SwitchEnd()
		{
//		QTween.FadeOut (FadeBlackTrans, 1.0f);
//		QTween.MoveTo(SwitchTrans, 1.0f, Vector3.left * 15.0f);

			GameManager.Instance.StartCoroutine(UnloadRes());
		}

		IEnumerator UnloadRes()
		{
			yield return new WaitForSeconds(1.0f);
			GoManager.Instance.UnloadUnusedStagePool();
			yield return new WaitForSeconds(1.0f);
//		QResMgr.Instance.Unload ();
		}
	}
}