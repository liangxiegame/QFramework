using UnityEngine;
using System.Collections;

/// <summary>
/// GameObject操作,太长了
/// </summary>
public class QGo  {

	public static void Show(Transform trans)
	{
		trans.gameObject.SetActive (true);
	}

	public static void Show(GameObject gameObj)
	{
		gameObj.SetActive (true);
	}

	public static void Hide(Transform trans)
	{
		trans.gameObject.SetActive (false);
	}

	public static void Hide(GameObject gameObj)
	{
		gameObj.SetActive (false);
	}
}
