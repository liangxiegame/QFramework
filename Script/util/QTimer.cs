using UnityEngine;
using System.Collections;

public class QTimer {

	public delegate void QTimerCallfunc ();


	/// <summary>
	/// 延时调用函数
	/// </summary>
	/// <param name="seconds">Seconds.</param>
	/// <param name="callfunc">Callfunc.</param>
	public static void ExecuteAfterSeconds(float seconds,QTimerCallfunc callfunc)
	{
		App.Instance ().StartCoroutine (Execute (seconds, callfunc));
	}
		
	static IEnumerator Execute(float seconds,QTimerCallfunc callfunc)
	{

		yield return new WaitForSeconds (seconds);
		callfunc ();
	}
}
