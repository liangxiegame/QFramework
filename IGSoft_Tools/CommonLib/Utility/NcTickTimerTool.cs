using UnityEngine;
using System.Collections;

// System Timer

public class NcTickTimerTool
{
	// Attribute ------------------------------------------------------------------------
	protected	int		m_nStartTickCount;
	protected	int		m_nCheckTickCount;

	// Construct -------------------------------------------------------------------------
	public NcTickTimerTool()
	{
		StartTickCount();
	}

	public static NcTickTimerTool GetTickTimer()
	{
		return new NcTickTimerTool();
	}

	// Property -------------------------------------------------------------------------
	public void StartTickCount()
	{
		m_nStartTickCount	= System.Environment.TickCount;
		m_nCheckTickCount	= m_nStartTickCount;
	}

	public int GetStartedTickCount()
	{
		return System.Environment.TickCount - m_nStartTickCount;
	}

	public int GetElapsedTickCount()
	{
		int n = System.Environment.TickCount - m_nCheckTickCount;
		m_nCheckTickCount = System.Environment.TickCount;
		return n;
	}

	public void LogElapsedTickCount()
	{
		Debug.Log(GetElapsedTickCount());
	}
}

