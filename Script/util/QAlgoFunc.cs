using UnityEngine;
using System.Collections;

public class QAlgoFunc {
	/// <summary>
	/// 交换 A 和 B
	/// </summary>
	/// <param name="container">Container.</param>
	/// <param name="indexA">Index a.</param>
	/// <param name="indexB">Index b.</param>
	public static void exchange(object[] container,int indexA,int indexB)
	{
		object temp = container [indexA];
		container [indexA] = container [indexB];
		container [indexB] = temp;
	}		
}
