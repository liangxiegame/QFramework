using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// PlayerPrefs的定制化封装
/// </summary>
public class PlayerPrefService {

//	/// <summary>
//	/// 加载玩家存储数据
//	/// </summary>
//	public static GameInfo LoadGameInfo()
//	{
//		var retGameInfo = new GameInfo ();
//
//		retGameInfo.curTheme = 1;
//
//		retGameInfo.coin = PlayerPrefs.GetInt ("coin",5000);
//		retGameInfo.bestMeter = PlayerPrefs.GetInt ("bestMeter",0);
//		retGameInfo.soundState = PlayerPrefs.GetInt ("soundState",0);
//
//		return retGameInfo;
//	}



	/// <summary>
	/// 记载道具数据
	/// </summary>
	public static Dictionary<string,PropInfo> LoadPropInfos(List<string> propNames)
	{
		var retPropInfos = new Dictionary<string,PropInfo> ();

		int propNamesCount = propNames.Count;

		for (int i = 0; i < propNamesCount; i++) {
			retPropInfos [propNames [i]] = new PropInfo (propNames [i],PlayerPrefs.GetInt (propNames [i] + "Level",0));
			Debug.Log ("PlayerPrefService @@@@ LoadPropInfos " + propNames [i] + " Level Loaded");
		}
			
		return retPropInfos;
	}
		
//	/// <summary>
//	/// 保存玩家数据
//	/// </summary>
//	public static void SaveGameInfo(GameInfo gameInfo)
//	{
//		gameInfo.bestMeter = gameInfo.bestMeter > gameInfo.curMeter ? gameInfo.bestMeter : gameInfo.curMeter;
//
//		PlayerPrefs.SetInt ("coin", gameInfo.coin);
//		PlayerPrefs.SetInt ("bestMeter", gameInfo.bestMeter);
//		PlayerPrefs.SetInt ("soundState", gameInfo.soundState);
//	}

	/// <summary>
	/// 保存所有的道具数据
	/// </summary>
	public static void SavePropInfos(Dictionary<string,PropInfo> propInfos)
	{
		foreach (KeyValuePair<string,PropInfo> pair in propInfos) {
			PlayerPrefs.SetInt (pair.Key + "Level", pair.Value.level);
		}
		Debug.Log ("PlayerPrefService @@@@ SavePropInfos Saved");
	}
}

