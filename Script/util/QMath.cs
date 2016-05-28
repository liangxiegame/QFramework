using UnityEngine;
using System.Collections;

/// <summary>
/// Q math.常用的数学库
/// </summary>
public class QMath {

	/// <summary>
	/// 输入百分比的概率
	/// </summary>
	public static bool Percent(int percent)
	{
		return Random.Range (0, 100) <= percent ? true : false;
	}


	/// <summary>
	/// Randoms 输入几个变量返回其中一个
	/// </summary>
	public static T RandomWithParams<T>(params T[] inputParams)
	{
		return inputParams[Random.Range(0,inputParams.Length)];
	}
}
