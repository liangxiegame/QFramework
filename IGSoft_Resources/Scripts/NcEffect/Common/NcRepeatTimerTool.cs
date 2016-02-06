using UnityEngine;
using System.Collections;

// frame timer
// 일정 시간 후 알림
// 일정 시간 후 반복 알림
// 일정 시간 후 반복횟수만큼 알림

public class NcRepeatTimerTool : NcTimerTool
{
	// Attribute ------------------------------------------------------------------------
	protected	float		m_fUpdateTime;
	protected	float		m_fIntervalTime;
	protected	int			m_nRepeatCount		= 0;

	protected	int			m_nCallCount		= 0;
	protected	object		m_ArgObject			= null;

	// Property -------------------------------------------------------------------------
	public bool UpdateTimer()
	{
		if (m_bEnable == false)
			return false;
		bool bNext = (m_fUpdateTime <= GetTime());
		if (bNext)
		{
			m_fUpdateTime += m_fIntervalTime;
			m_nCallCount++;
			if (m_fIntervalTime <= 0 || m_nRepeatCount != 0 && m_nRepeatCount <= m_nCallCount)
				m_bEnable = false;
		}
		return bNext;
	}

	public void ResetUpdateTime()
	{
		m_fUpdateTime = GetTime() + m_fIntervalTime;
	}

	public int GetCallCount()
	{
		return m_nCallCount;
	}

	public object GetArgObject()
	{
		return m_ArgObject;
	}

	public float GetElapsedRate()
	{
		if (m_fUpdateTime == 0)
			return 1;
		return (GetTime() / m_fUpdateTime);
	}

	// Control Function -----------------------------------------------------------------
	public void SetTimer(float fStartTime)
	{
		SetRelTimer(fStartTime - NcTimerTool.GetEngineTime());
	}

	public void SetTimer(float fStartTime, float fRepeatTime)
	{
		SetRelTimer(fStartTime - NcTimerTool.GetEngineTime(), fRepeatTime);
	}

	public void SetTimer(float fStartTime, float fRepeatTime, int nRepeatCount)
	{
		SetRelTimer(fStartTime - NcTimerTool.GetEngineTime(), fRepeatTime, nRepeatCount);
	}

	// --------------------------------------------------------------------------
	public void SetTimer(float fStartTime, object arg)
	{
		SetRelTimer(fStartTime - NcTimerTool.GetEngineTime(), arg);
	}

	public void SetTimer(float fStartTime, float fRepeatTime, object arg)
	{
		SetRelTimer(fStartTime - NcTimerTool.GetEngineTime(), fRepeatTime, arg);
	}

	public void SetTimer(float fStartTime, float fRepeatTime, int nRepeatCount, object arg)
	{
		SetRelTimer(fStartTime - NcTimerTool.GetEngineTime(), fRepeatTime, nRepeatCount, arg);
	}

	// --------------------------------------------------------------------------
	public void SetRelTimer(float fStartRelTime)
	{
		Start();
		m_nCallCount	= 0;
		m_fUpdateTime	= fStartRelTime;
		m_fIntervalTime	= 0;
		m_nRepeatCount	= 0;
	}

	public void SetRelTimer(float fStartRelTime, float fRepeatTime)
	{
		Start();
		m_nCallCount	= 0;
		m_fUpdateTime	= fStartRelTime;
		m_fIntervalTime	= fRepeatTime;
		m_nRepeatCount	= 0;
	}

	public void SetRelTimer(float fStartRelTime, float fRepeatTime, int nRepeatCount)
	{
		Start();
		m_nCallCount	= 0;
		m_fUpdateTime	= fStartRelTime;
		m_fIntervalTime	= fRepeatTime;
		m_nRepeatCount	= nRepeatCount;
	}

	// --------------------------------------------------------------------------
	public void SetRelTimer(float fStartRelTime, object arg)
	{
		Start();
		m_nCallCount	= 0;
		m_fUpdateTime	= fStartRelTime;
		m_fIntervalTime	= 0;
		m_nRepeatCount	= 0;
		m_ArgObject		= arg;
	}

	public void SetRelTimer(float fStartRelTime, float fRepeatTime, object arg)
	{
		Start();
		m_nCallCount	= 0;
		m_fUpdateTime	= fStartRelTime;
		m_fIntervalTime	= fRepeatTime;
		m_nRepeatCount	= 0;
		m_ArgObject		= arg;
	}

	public void SetRelTimer(float fStartRelTime, float fRepeatTime, int nRepeatCount, object arg)
	{
		Start();
		m_nCallCount	= 0;
		m_fUpdateTime	= fStartRelTime;
		m_fIntervalTime	= fRepeatTime;
		m_nRepeatCount	= nRepeatCount;
		m_ArgObject		= arg;
	}
}
