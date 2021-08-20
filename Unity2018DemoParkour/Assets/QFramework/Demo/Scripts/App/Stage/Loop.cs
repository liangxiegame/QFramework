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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace QFramework.PlatformRunner
{
	public class Loop : QMonoBehaviour, IStage
	{
		public override IManager Manager
		{
			get { return GameManager.Instance; }
		}


		List<Transform> mBlockList = new List<Transform>();
		List<Transform> mAirList   = new List<Transform>();
		List<Transform> mCoinList  = new List<Transform>();
		List<Transform> mForeList  = new List<Transform>();
		List<Transform> mPropList  = new List<Transform>();
		List<Transform> mEnemyList = new List<Transform>();
		List<Transform> mFruitList = new List<Transform>();

		// 用于回收的对象
		public List<Transform> BlockList
		{
			get { return mBlockList; }
		}

		public List<Transform> AirList
		{
			get { return mAirList; }
		}

		public List<Transform> CoinList
		{
			get { return mCoinList; }
		}

		public List<Transform> ForeList
		{
			get { return mForeList; }
		}

		public List<Transform> PropList
		{
			get { return mPropList; }
		}

		public List<Transform> EnemyList
		{
			get { return mEnemyList; }
		}

		public List<Transform> FruitList
		{
			get { return mFruitList; }
		}


		public IEnumerator FadeIn()
		{
			for (int i = 0; i < transform.childCount; i++)
			{
//			QTween.FadeIn(transform.GetChild(i), 0.5f, false);
			}

			yield return new WaitForSeconds(0.5f);
		}

		public IEnumerator FadeOut()
		{
			for (int i = 0; i < transform.childCount; i++)
			{
//			QTween.FadeOut(transform.GetChild(i), 0.5f, false);
			}

			yield return new WaitForSeconds(0.5f);
			yield return GameManager.Instance.StartCoroutine(Despawn());
		}

		[SerializeField] int theme = 0;

		public int Theme
		{
			get { return theme; }
			set { theme = value; }
		}

		/// <summary>
		/// 解析
		/// </summary>
		/// <param name="data">Data.</param>
		public IEnumerator Parse(StageData data)
		{
			yield return 0;
		}

		/// <summary>
		/// 回收掉关卡
		/// </summary>
		public IEnumerator Despawn()
		{
//		if (PoolManager.Pools.ContainsKey("Stage" + Theme))
//		{
//
//			var StagePool = GoManager.Instance.GetThemeSpawnPool(Theme);
//			StagePool.Despawn(transform);
//
//		}

			this.DestroyGameObjGracefully();
			yield return 0;
		}


		/// <summary>
		/// 吸铁石效果
		/// </summary>
		public void MagnetiteExecute()
		{
			for (int i = 0; i < mFruitList.Count; i++)
			{
				mFruitList[i].GetComponent<MagnetiteEffect>().enabled = true;
			}

			for (int i = 0; i < mCoinList.Count; i++)
			{
				mCoinList[i].GetComponent<Coins>().MagnetiteOn();
			}
		}

		/// <summary>
		/// Bigs the reset.
		/// </summary>
		public void BigReset()
		{

		}

		/// <summary>
		/// 震动把enemy 都震掉
		/// </summary>
		public void Shake()
		{
			foreach (var enemy in mEnemyList)
			{
				enemy.GetComponent<Enemy>().Shake();
			}
		}


		/// <summary>
		/// 重
		/// </summary>
		public void ResetAirBlock()
		{
			foreach (var air in mAirList)
			{
				air.GetComponent<BlockAir>().ResetBlock();
			}
		}
	}
}