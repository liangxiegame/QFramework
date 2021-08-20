using UnityEngine;
using System.Collections;
using QFramework;
using System.Collections.Generic;
using QFramework.PlatformRunner;

/// <summary>
/// 解析关卡数据
/// </summary> 
/// 
public class RandomStage : QMonoBehaviour, IStage
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

	/// <summary>
	/// 生成结点
	/// </summary>
	Transform GenTrans(string poolName, string prefabName, Vector3 pos, List<Transform> list)
	{
//		var retTrans = PoolManager.Pools[poolName].Spawn(prefabName);
//		retTrans.transform.parent = transform;
//		retTrans.transform.localPosition = pos;
//		list.Add(retTrans);

//		return retTrans;
		return null;
	}

	/// <summary>
	/// 生成底部的块	
	/// </summary>
	void GenBlock(int name, Vector3 pos)
	{
		switch (name)
		{
			case STAGE.BA:
				GenTrans("Stage" + Theme, "air", pos, mAirList).GetComponent<BlockAir>().ResetBox();
				break;
			case STAGE.BL:
				GenTrans("Stage" + Theme, "left", pos, mBlockList);
				break;
			case STAGE.BM:
				GenTrans("Stage" + Theme, "middle", pos, mBlockList);

				break;
			case STAGE.BR:
				GenTrans("Stage" + Theme, "right", pos, mBlockList);
				break;
		}
	}

	/// <summary>
	/// 生成水果	
	/// </summary>
	/// <param name="name">Name.</param>
	/// <param name="pos">Position.</param>
	void GenFruit(int name, Vector3 pos)
	{
		switch (name)
		{
			case STAGE.FB:
				GenTrans("Fruit", "fruit_banana", pos, mFruitList).GetComponent<Fruit>().ResetFruit();
				break;
			case STAGE.FC:
				GenTrans("Fruit", "fruit_coconut", pos, mFruitList).GetComponent<Fruit>().ResetFruit();
				break;
			case STAGE.FM:
				GenTrans("Fruit", "fruit_mango", pos, mFruitList).GetComponent<Fruit>().ResetFruit();
				break;
			case STAGE.FPA:
				GenTrans("Fruit", "fruit_pineapple", pos, mFruitList).GetComponent<Fruit>().ResetFruit();
				break;
			case STAGE.FP:
				GenTrans("Fruit", "fruit_pitaya", pos, mFruitList).GetComponent<Fruit>().ResetFruit();
				break;
			default:
				break;
		}
	}

	/// <summary>
	/// 生成路边的背景
	/// </summary>
	/// <param name="name">Name.</param>
	/// <param name="pos">Position.</param>
	void GenComponent(int name, Vector3 pos)
	{
		Debug.LogWarning("compoent:" + (name - STAGE.C1 + 1));
//		GenTrans ("Stage" + Theme,"component" + (name - STAGE.C1 + 1), pos, mForeList).localScale = Random.Range(0,2) == 1 ? new Vector3(1,1,1):new Vector3(1.0f,1,1);
	}

	/// <summary>
	/// 生成道具
	/// </summary>
	/// <param name="name">Name.</param>
	/// <param name="pos">Position.</param>
	void GenProp(int name, Vector3 pos)
	{
		switch (name)
		{
			case STAGE.PA: // Prop Auto
				GenTrans("Prop", "prop_auto", pos, mPropList).GetComponent<Prop>().ResetProp();
				break;
			case STAGE.PB: // Prop Big
				GenTrans("Prop", "prop_big", pos, mPropList).GetComponent<Prop>().ResetProp();
				break;
			case STAGE.PF2: // Prop Fruit x2
				GenTrans("Prop", "prop_fruit_x2", pos, mPropList).GetComponent<Prop>().ResetProp();
				break;
			case STAGE.PG2:
				GenTrans("Prop", "prop_gold_x2", pos, mPropList).GetComponent<Prop>().ResetProp();
				break;
			case STAGE.PM:
				GenTrans("Prop", "prop_magnetite", pos, mPropList).GetComponent<Prop>().ResetProp();
				break;
			case STAGE.PP:
				GenTrans("Prop", "prop_protect", pos, mPropList).GetComponent<Prop>().ResetProp();
				break;
			case STAGE.PTE:
				GenTrans("Prop", "prop_time_extra", pos, mPropList).GetComponent<Prop>().ResetProp();
				break;
			default:
				break;
		}
	}

	/// <summary>
	/// 生成金币
	/// </summary>
	/// <param name="name">Name.</param>
	/// <param name="pos">Position.</param>
	/// <param name="layer">Layer.</param>
	void GenGold(int name, Vector3 pos, int layer = 1)
	{
		switch (name)
		{
			case STAGE.G1:
				GenTrans("Coin", "coin", pos, CoinList).GetComponent<Coins>().ResetPos();
				break;
			case STAGE.G3:
				Debug.LogWarning("G3");
				if (layer == 1)
				{
					GenTrans("Coin", "Coin3", pos + Vector3.up * 0.5f, mCoinList).GetComponent<Coins>().ResetPos();
				}
				else if (layer == 2)
				{
					GenTrans("Coin", "Coin3", pos + Vector3.up * 1.5f, mCoinList).GetComponent<Coins>().ResetPos();
				}

				break;
			case STAGE.G6:
				if (layer == 1)
				{
					GenTrans("Coin", "Coin6", pos + Vector3.up * 0.5f, mCoinList).GetComponent<Coins>().ResetPos();
				}
				else if (layer == 2)
				{
					GenTrans("Coin", "Coin6", pos + Vector3.up * 1.5f, mCoinList).GetComponent<Coins>().ResetPos();
				}

				break;
			case STAGE.GC:
				GenTrans("Coin", "CoinCircle", pos, mCoinList).GetComponent<Coins>().ResetPos();
				break;
			default:
				break;
		}

	}

	/// <summary>
	/// 生成敌人
	/// </summary>
	/// <param name="name">Name.</param>
	/// <param name="pos">Position.</param>
	void GenEnemy(int name, Vector3 pos)
	{
		switch (name)
		{
			case STAGE.ET:
				GenTrans("Enemy", "enemy_turtle", pos + Vector3.up * 1.0f, mEnemyList).GetComponent<Enemy>().ResetEnemy();
				break;
			case STAGE.EC:
				GenTrans("Enemy", "enemy_crab", pos + Vector3.up * 1.0f, mEnemyList).GetComponent<Enemy>().ResetEnemy();
				break;
			default:
				break;
		}
	}


	/// <summary>
	/// 解析第一层数据
	/// </summary>
	/// <param name="stageData">Stage data.</param>
	/// <param name="index">Index.</param>
	void ParseFirstLayer(StageData stageData, int index)
	{
		int name = stageData.FirstNums[index];

		if (name >= STAGE.BLOCK_BEGIN && name < STAGE.BLOCK_END)
		{
			GenBlock(name, stageData.Generator.FirstLayerPos()[index]);
		}
		else if (name >= STAGE.GOLD_BEGIN && name < STAGE.GOLD_END)
		{
			GenGold(name, stageData.Generator.FirstLayerPos()[index], 1);
		}
	}

	/// <summary>
	/// 解析第二层数据
	/// </summary>
	/// <param name="stageData">Stage data.</param>
	/// <param name="index">Index.</param>
	void ParseSecondLayer(StageData stageData, int index)
	{
		int name = stageData.SecondNums[index];

		if (name >= STAGE.GOLD_BEGIN && name < STAGE.GOLD_END)
		{
			GenGold(name, stageData.Generator.FirstLayerPos()[index], 2);
		}
		else if (name >= STAGE.ENEMY_BEGIN && name < STAGE.ENEMY_END)
		{
			GenEnemy(name, stageData.Generator.FirstLayerPos()[index]);
		}
	}

	/// <summary>
	/// 解析第三层数据
	/// </summary>
	/// <param name="stageData">Stage data.</param>
	/// <param name="index">Index.</param>
	void ParseThirdLayer(StageData stageData, int index)
	{
		int name = stageData.ThirdNums[index];

		if (name >= STAGE.BLOCK_BEGIN && name < STAGE.BLOCK_END)
		{
			GenBlock(name, stageData.Generator.ThirdLayerPos()[index]);
		}
		else if (name >= STAGE.GOLD_BEGIN && name < STAGE.GOLD_END)
		{
			GenGold(name, stageData.Generator.ThirdLayerPos()[index]);
		}
	}

	/// <summary>
	/// 解析前边的数据
	/// </summary>
	/// <param name="stageData">Stage data.</param>
	/// <param name="index">Index.</param>
	void ParseForeLayer(StageData stageData, int index)
	{
		int name = stageData.ForeNums[index];
		if (name >= STAGE.C1 && name <= STAGE.C20)
		{
			GenComponent(name, stageData.Generator.FirstLayerPos()[index]);
		}
	}

	/// <summary>
	/// 第四层内容生成
	/// </summary>
	void ParseFourthLayer(StageData stageData, int index)
	{
		int name = stageData.FourthNums[index];

		if (name >= STAGE.GOLD_BEGIN && name < STAGE.GOLD_END)
		{
			GenGold(name, stageData.Generator.ThirdLayerPos()[index] + Vector3.up * 1.5f);
		}
		else if (name >= STAGE.PROP_BEGIN && name < STAGE.PROP_END)
		{
			GenProp(name, stageData.Generator.ThirdLayerPos()[index] + Vector3.up * 1.5f);
		}
		else if (name >= STAGE.FRUIT_BEGIN && name < STAGE.FRUIT_END)
		{
			GenFruit(name, stageData.Generator.ThirdLayerPos()[index] + Vector3.up * 1.5f);
		}
	}

	/// <summary>
	/// 关卡主题
	/// </summary>
	[SerializeField] int theme = 0;

	public int Theme
	{
		get { return theme; }
		set { theme = value; }
	}

	/// <summary>
	/// 解析关卡数据
	/// </summary>
	public IEnumerator Parse(StageData stageData)
	{
		Theme = stageData.Theme;

		// 第一层的数据解析
		for (int i = 0; i < stageData.FirstNumCount; i++)
		{
			ParseFirstLayer(stageData, i);
			yield return new WaitForEndOfFrame();
		}

		// 第二层数据解析
		for (int i = 0; i < stageData.SecondNumCount; i++)
		{
			ParseSecondLayer(stageData, i);
			yield return new WaitForEndOfFrame();

		}

		// 前景层数据解析
		for (int i = 0; i < stageData.ForeCount; i++)
		{
			ParseForeLayer(stageData, i);
			yield return new WaitForEndOfFrame();
		}

		// 第三层数据解析
		for (int i = 0; i < stageData.ThirdNumCount; i++)
		{
			ParseThirdLayer(stageData, i);
			yield return new WaitForEndOfFrame();
		}

		// 第四层数据解析
		for (int i = 0; i < stageData.FourthNumCount; i++)
		{
			ParseFourthLayer(stageData, i);
			yield return new WaitForEndOfFrame();
		}
	}

	/// <summary>
	/// 回收
	/// </summary>
	public IEnumerator Despawn()
	{
//		if (PoolManager.Pools.ContainsKey("Stage" + Theme))
//		{
//
//			var StagePool = GoManager.Instance.GetThemeSpawnPool(Theme);
//
//			for (int i = 0; i < mBlockList.Count; i++)
//			{
//				mBlockList[i].parent = null;
//				StagePool.Despawn(mBlockList[i]);
//
//				yield return new WaitForEndOfFrame();
//			}
//
//			for (int i = 0; i < mAirList.Count; i++)
//			{
//				mAirList[i].parent = null;
//				StagePool.Despawn(mAirList[i]);
//
//				yield return new WaitForEndOfFrame();
//
//			}
//
//			for (int i = 0; i < mCoinList.Count; i++)
//			{
//				mCoinList[i].parent = null;
////				PoolManager.Pools["Coin"].Despawn(mCoinList[i]);
//
//				yield return new WaitForEndOfFrame();
//			}
//
//			for (int i = 0; i < mForeList.Count; i++)
//			{
//				mForeList[i].parent = null;
//				StagePool.Despawn(mForeList[i]);
//
//				yield return new WaitForEndOfFrame();
//			}
//
//			for (int i = 0; i < mEnemyList.Count; i++)
//			{
//				mEnemyList[i].parent = null;
////				PoolManager.Pools["Enemy"].Despawn(mEnemyList[i]);
//
//				yield return new WaitForEndOfFrame();
//
//			}
//
//			for (int i = 0; i < mPropList.Count; i++)
//			{
//				mPropList[i].parent = null;
////				PoolManager.Pools["Prop"].Despawn(mPropList[i]);
//
//				yield return new WaitForEndOfFrame();
//
//			}
//
//			for (int i = 0; i < mFruitList.Count; i++)
//			{
//				mFruitList[i].parent = null;
//				mFruitList[i].GetComponent<MagnetiteEffect>().enabled = false;
////				PoolManager.Pools["Fruit"].Despawn(mFruitList[i]);
//
//				yield return new WaitForEndOfFrame();
//			}
//
//			mBlockList.Clear();
//			mAirList.Clear();
//			mCoinList.Clear();
//			mForeList.Clear();
//			mEnemyList.Clear();
//			mPropList.Clear();
//			mFruitList.Clear();
//
//			StagePool.Despawn(transform);
//
//			yield return new WaitForEndOfFrame();
//		}
//		else
//		{
//			// 如果要Destroy的话,将Emiter放到Player的Transform下面,以免被回收掉
//			GameManager.Instance.playerCtrl.ResetLandEffect();
//			Destroy(transform);
//			yield return 0;
//		}

		this.DestroyGameObjGracefully();


		yield return null;
	}

	public void ResetAirBlock()
	{
		for (int i = 0; i < mAirList.Count; i++)
		{
			mAirList[i].GetComponent<BlockAir>().ResetBlock();
		}
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
	/// 震动把enemy 都震掉
	/// </summary>
	public void Shake()
	{
		for (int i = 0; i < mEnemyList.Count; i++)
		{
			mEnemyList[i].GetComponent<Enemy>().Shake();
		}
	}

	/// <summary>
	/// 这里的FadeOut暂时改为Despawn
	/// </summary>
	public IEnumerator FadeOut()
	{
		// 先不用FadeOut了 被大叔挡住了
//		var LoopScript = GetComponent<Loop> ();

//		if (LoopScript != null) {
//			LoopScript.FadeOut ();
//		} else {
//			for (int i = 0; i < BlockList.Count; i++) 	QTween.FadeOut (BlockList [i], 0.5f, false);
//			for (int i = 0; i < AirList.Count; i++) 	QTween.FadeOut (AirList [i], 0.5f, false);
//			for (int i = 0; i < CoinList.Count; i++)    CoinList [i].gameObject.SetActive (false);
//			for (int i = 0; i < ForeList.Count; i++)	ForeList [i].gameObject.SetActive (false);
//			for (int i = 0; i < PropList.Count; i++)	QTween.FadeOut (PropList [i], 0.5f, false);
//			for (int i = 0; i < EnemyList.Count; i++)	QTween.FadeOut (EnemyList [i], 0.5f, false);
//			for (int i = 0; i < FruitList.Count; i++)	QTween.FadeOut (FruitList [i], 0.5f, false);
//
//			yield return new WaitForSeconds (0.5f);
//		}
//
//		if (LoopScript != null) {
//		} else {
//			hide ();
//
//			for (int i = 0; i < BlockList.Count; i++) 	BlockList [i].gameObject.GetComponent<SpriteRenderer> ().color = Color.white;
//			for (int i = 0; i < AirList.Count; i++) 	AirList [i].GetComponent<SpriteRenderer> ().color = Color.white;
//			for (int i = 0; i < PropList.Count; i++)	PropList [i].GetComponent<SpriteRenderer> ().color = Color.white;
//			for (int i = 0; i < EnemyList.Count; i++)	EnemyList [i].GetComponent<SpriteRenderer> ().color = Color.white;
//			for (int i = 0; i < FruitList.Count; i++)	FruitList [i].GetComponent<SpriteRenderer> ().color = Color.white;
//		}
		yield return GameManager.Instance.StartCoroutine(Despawn());
	}

	public IEnumerator FadeIn()
	{
		yield return 0;
	}
}