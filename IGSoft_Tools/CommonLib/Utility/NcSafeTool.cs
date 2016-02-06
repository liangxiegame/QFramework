// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;

public class NcSafeTool : MonoBehaviour
{
	public	static	bool	m_bShuttingDown;
	public	static	bool	m_bLoadLevel;
	private	static	NcSafeTool	s_Instance	= null;

	// Property -------------------------------------------------------------------------
	private static void Instance()
	{
		if (s_Instance == null)
		{
			GameObject gm;
			gm = UnityEngine.GameObject.Find("_GlobalManager");
			if (gm == null)
				gm = new GameObject("_GlobalManager");
			else s_Instance = (NcSafeTool)gm.GetComponent(typeof(NcSafeTool));
			if (s_Instance == null)
				s_Instance = (NcSafeTool)gm.AddComponent(typeof(NcSafeTool));
		}
	}

	public static bool IsSafe()
	{
		return (!m_bShuttingDown && !m_bLoadLevel);
	}

	public static	Object	SafeInstantiate(Object original)
	{
		if (m_bShuttingDown)
			return null;
		if (s_Instance == null)
			Instance();

		return Object.Instantiate(original);
	}

	public static Object SafeInstantiate(Object original, Vector3 position, Quaternion rotation)
	{
		if (m_bShuttingDown)
			return null;
		if (s_Instance == null)
			Instance();

		return Object.Instantiate(original, position, rotation);
	}

	public static void LoadLevel(int nLoadLevel)
	{
		if (m_bShuttingDown)
			return;
		if (s_Instance == null)
			Instance();
		m_bLoadLevel = true;
		Debug.Log("Safe LoadLevel start " + nLoadLevel);
		Application.LoadLevel(nLoadLevel);
		Debug.Log("Safe LoadLevel end");
		m_bLoadLevel = false;
	}

	public void OnApplicationQuit()
	{
		m_bShuttingDown = true;
	}
}
