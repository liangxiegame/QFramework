using UnityEngine;
using System.Collections;
/// <summary>
/// Q print.QuickUnity 输出控制
/// </summary>
public class QPrint {

	public static void Warn(object message)
	{
		if (GAME_DATA.DEBUG) {
			Debug.LogWarning ("@@@@@@@@@ " + message);
		}
	}
}
