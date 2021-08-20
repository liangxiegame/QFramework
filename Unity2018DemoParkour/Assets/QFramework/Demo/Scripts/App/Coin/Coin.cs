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

namespace QFramework.PlatformRunner
{
	public class Coin : MonoBehaviour, ICmd
	{

//		SpawnPool coinPool;
//		SpawnPool particlePool;

		void Start()
		{
//			coinPool = PoolManager.Pools["Coin"];
//			particlePool = PoolManager.Pools["Particle"];
		}

		private Transform      mParticleTrans;
		private ParticleSystem mSystemCom;

		private void DespawnParticle()
		{
			mSystemCom.Stop();
			mSystemCom.Clear();
			mParticleTrans.parent = null;
//			particlePool.Despawn(particleTrans);
		}

		[SerializeField] string    mCoinValue1 = "CoinValue1";
		private string    mCoinValue2 = "CoinValue2";
		string    coinValueName;
		private Transform mCoinValue;
		CoinValue coinValueScript;

		private void DespawnCoinValue()
		{
//			coinPool.Despawn(coinValue);
			mCoinValue = null;
			coinValueScript = null;
		}

		Transform      particleStarTrans;
		ParticleSystem systemStarCom;

		private void DespawnParticleStar()
		{
			systemStarCom.Stop();
			systemStarCom.Clear();

			particleStarTrans.parent = null;
//			particlePool.Despawn(particleStarTrans);

			particleStarTrans = null;
			systemStarCom = null;
		}

		/// <summary>
		/// 金币的逻辑执行
		/// </summary>
		public void Execute()
		{
//			#region 圆圆的闪光效果
//
//			particleTrans = particlePool.Spawn("coin");
//			systemCom = particleTrans.GetComponent<ParticleSystem>();
//			particleTrans.parent = transform.parent;
//			particleTrans.Identity();
//			particleTrans.localPosition = transform.localPosition;
//			systemCom.Play();
//
//			Invoke("DespawnParticle", systemCom.duration);
//
//			#endregion
//
//			#region 金币分数值效果
//
//			coinValueName = PropModel.Instance.prop_gold_x2_on ? COIN_VALUE2 : COIN_VALUE1;
//			coinValue = coinPool.Spawn(coinValueName);
//			coinValueScript = coinValue.GetComponent<CoinValue>();
//			coinValueScript.ResetCoinValue();
//			coinValue.parent = transform.parent;
//			coinValue.localPosition = transform.localPosition;
//			coinValue.rotation = Quaternion.identity; // 有得金币会旋转 
//			coinValue.parent = null;
//			coinValue.localScale = Vector3.one;
//			float duration = 0.8f;
//			coinValueScript.Execute();
//			Invoke("DespawnCoinValue", duration);
//
//			#endregion
//
//			#region 星星效果
//
//			particleStarTrans = particlePool.Spawn("LightStar");
//			systemStarCom = particleStarTrans.GetComponent<ParticleSystem>();
//			particleStarTrans.parent = transform.parent;
//			particleStarTrans.Identity();
//			particleStarTrans.localRotation = Quaternion.Euler(Vector3.left * 90.0f);
//			particleStarTrans.localPosition = transform.localPosition;
//
//			systemStarCom.Play();
//
//			Invoke("DespawnParticleStar", systemStarCom.duration);
//
//			// 逻辑在这里执行
//			if (PropModel.Instance.prop_gold_x2_on)
//			{
//				GameManager.Instance.SendMsg(new MsgWithInt((ushort) GameEvent.CoinAdd, 2));
//			}
//			else
//			{
//				GameManager.Instance.SendMsg(new MsgWithInt((ushort) GameEvent.CoinAdd, 1));
//			}
//
//			#endregion
//
////		GameManager.Instance.uiCtrl.UpdateCoin ();
////		QSoundMgr.Instance.PlayClipAsync (SOUND.COIN);

			this.Hide();
		}
	}
}