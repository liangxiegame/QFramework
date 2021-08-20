using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 关卡定义
/// </summary>
public interface IStage  {

	List<Transform> BlockList { get; }
	List<Transform> AirList { get; }
	List<Transform> CoinList { get;}
	List<Transform> ForeList{ get;}
	List<Transform> PropList { get;}
	List<Transform> EnemyList { get;}
	List<Transform> FruitList { get;}

	/// <summary>
	/// 关卡的主题
	/// </summary>
	int Theme{ get; set;}

	/// <summary>
	/// 关卡数据的生成,和解析。
	/// </summary>
	IEnumerator Parse (StageData data);
	IEnumerator Despawn();

	/// <summary>
	/// 解析数据
	/// </summary>
	IEnumerator FadeOut();
	IEnumerator FadeIn();

	/// <summary>
	/// 道具效果实现
	/// </summary>
	void Shake();
	void MagnetiteExecute();
	void ResetAirBlock();
}
