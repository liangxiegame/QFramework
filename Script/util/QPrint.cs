using UnityEngine;
using System.Collections;
/// <summary>
/// Q print.QuickUnity 输出控制
/// </summary>
public class QPrint {

	/// <summary>
	/// 给用户使用的:警告
	/// </summary>
	public static void Warn(object message)
	{
		if (APP_CONFIG.DEBUG) {
			Debug.LogWarning ("@@@@Custom@@@@@ " + message);
		}
	}

	/// <summary>
	/// 框架内部:警告
	/// </summary>
	public static void FrameworkWarn(object message)
	{
		if (APP_CONFIG.DEBUG) {
			Debug.LogWarning ("@@@@Framework@@@@ " + message);
		}
	}

	/// <summary>
	/// 框架内部:报错
	/// </summary>
	public static void FrameworkError(object message)
	{
		if (APP_CONFIG.DEBUG) {
			Debug.LogError ("@@@@Framework@@@@ " + message);
		}
	}
		
	/// <summary>
	/// 框架内部:日志
	/// </summary>
	public static void FrameworkLog(object message)
	{
		if (APP_CONFIG.DEBUG) {
			Debug.Log ("@@@@Framework@@@@ " + message);
		}
	}
}
