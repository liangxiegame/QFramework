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
	/// <returns>The with parameters.</returns>
	public static object RandomWithParams(params object[] inputParams)
	{
		return inputParams [Random.Range (0, inputParams.Length)];
	}
}
