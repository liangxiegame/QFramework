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
	/// <summary>
	/// 用户自己拼的关卡
	/// </summary>
	public class CustomStage : QMonoBehaviour, IStage
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

		public List<Vector2> mEnemyPos = new List<Vector2>();
		public List<Vector2> mFruitPos = new List<Vector2>();
		bool                 isParsed  = false;

		[SerializeField] int theme = 0;

		public int Theme
		{
			get { return theme; }
			set { theme = value; }
		}

		/// <summary>
		/// 解析关卡数据
		/// </summary>
		public IEnumerator Parse(StageData data)
		{
			if (isParsed)
			{

				for (int i = 0; i < mCoinList.Count; i++)
				{
					mCoinList[i].GetComponent<Coins>().ResetPos();

				}

				yield return new WaitForEndOfFrame();

				for (int i = 0; i < mAirList.Count; i++)
				{
					mAirList[i].GetComponent<BlockAir>().ResetBox();
				}

				yield return new WaitForEndOfFrame();

				for (int i = 0; i < mPropList.Count; i++)
				{
					mPropList[i].GetComponent<Prop>().ResetProp();
				}

				yield return new WaitForEndOfFrame();

				for (int i = 0; i < mFruitList.Count; i++)
				{
					mFruitList[i].parent = transform;
					mFruitList[i].GetComponent<Fruit>().ResetFruit();
					mFruitList[i].localPosition = mFruitPos[i];
				}

				yield return new WaitForEndOfFrame();

				for (int i = 0; i < mEnemyList.Count; i++)
				{
					mEnemyList[i].parent = transform;
					mEnemyList[i].GetComponent<Enemy>().ResetEnemy();
					mEnemyList[i].localPosition = mEnemyPos[i];
				}


			}
			else
			{
				for (int i = 0; i < transform.childCount; i++)
				{
					var childTrans = transform.GetChild(i);

					switch (childTrans.tag)
					{
						case "Coins":
							childTrans.GetComponent<Coins>().ResetPos();
							mCoinList.Add(childTrans);
							break;
						case "block_air":
							childTrans.GetComponent<BlockAir>().ResetBox();
							mAirList.Add(childTrans);
							break;
						case "prop_auto":
						case "prop_big":
						case "prop_fruit_x2":
						case "prop_gold_x2":
						case "prop_magnetite":
						case "prop_protect":
							childTrans.GetComponent<Prop>().ResetProp();
							mPropList.Add(childTrans);
							break;
						case "fruit_banana":
						case "fruit_coconut":
						case "fruit_mango":
						case "fruit_pineapple":
						case "fruit_pitaya":
						case "fruit_quince":
							childTrans.GetComponent<Fruit>().ResetFruit();
							mFruitList.Add(childTrans);
							mFruitPos.Add(childTrans.localPosition);
							break;
						case "enemy_turtle":
						case "enemy_crab":
							childTrans.GetComponent<Enemy>().ResetEnemy();
							mEnemyList.Add(childTrans);
							mEnemyPos.Add(childTrans.localPosition);
							break;
					}

					yield return new WaitForEndOfFrame();
				}

				isParsed = true;
			}

			yield return 0;
		}

		public IEnumerator Despawn()
		{
//		if (PoolManager.Pools.ContainsKey("Stage" + Theme))
//		{
//			var StagePool = GoManager.Instance.GetThemeSpawnPool(Theme);
//			StagePool.Despawn(transform);
//		}
			this.DestroyGameObjGracefully();

			yield return 0;
		}

		public IEnumerator FadeIn()
		{
			yield return 0;
		}

		public IEnumerator FadeOut()
		{
			StartCoroutine(Despawn());
			yield return 0;
		}

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
		/// 吸铁石效果
		/// </summary>
		public void MagnetiteExecute()
		{
			foreach (var fruit in mFruitList)
			{
				fruit.GetComponent<MagnetiteEffect>().enabled = true;
			}

			foreach (var coin in mCoinList)
			{
				coin.GetComponent<Coins>().MagnetiteOn();
			}
		}


		/// <summary>
		/// 重置道具效果
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