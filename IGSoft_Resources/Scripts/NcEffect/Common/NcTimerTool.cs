using UnityEngine;
using System.Collections;

// frame timer
// 일정 시간 후 알림
// 일정 시간 후 반복 알림
// 일정 시간 후 반복횟수만큼 알림

public class NcTimerTool
{
	// Attribute ------------------------------------------------------------------------
	protected	bool		m_bEnable;
	private		float		m_fLastEngineTime	= 0;
	private		float		m_fCurrentTime;
	private		float		m_fLastTime			= 0;
	private		float		m_fTimeScale		= 1.0f;

	private		int			m_nSmoothCount		= 10;
	private		int			m_nSmoothIndex		= 0;
	private		float		m_fSmoothRate		= 1.3f;
	private		float[]		m_fSmoothTimes;
	private		float		m_fLastSmoothDeltaTime;

	// Property -------------------------------------------------------------------------
	public static float GetEngineTime()
	{
		if (Time.time == 0)
			return 0.000001f;
		return Time.time;
	}

	public static float GetEngineDeltaTime()
	{
		return Time.deltaTime;
	}

	private void InitSmoothTime()
	{
		if (m_fSmoothTimes == null)
		{
			m_fSmoothTimes = new float[m_nSmoothCount];
			for (int n = 0; n < m_nSmoothCount; n++)
				m_fSmoothTimes[n] = Time.deltaTime;
			m_fLastSmoothDeltaTime = Time.deltaTime;
		}
	}

	private float UpdateSmoothTime(float fDeltaTime)
	{
		m_fSmoothTimes[m_nSmoothIndex++] = Mathf.Min(fDeltaTime, m_fLastSmoothDeltaTime * m_fSmoothRate);
		if (m_nSmoothCount <= m_nSmoothIndex)
			m_nSmoothIndex = 0;

		m_fLastSmoothDeltaTime = 0;
		for (int n = 0; n < m_nSmoothCount; n++)
			m_fLastSmoothDeltaTime += m_fSmoothTimes[n];
		m_fLastSmoothDeltaTime /= m_nSmoothCount;
		return m_fLastSmoothDeltaTime;
	}

	public bool IsUpdateTimer()
	{
		return (m_fLastEngineTime != GetEngineTime());
	}

	private float UpdateTimer()
	{
		if (m_bEnable)
		{
			if (m_fLastEngineTime != GetEngineTime())
			{
				m_fLastTime		= m_fCurrentTime;
				m_fCurrentTime += (GetEngineTime() - m_fLastEngineTime) * GetTimeScale();
				m_fLastEngineTime = GetEngineTime();
				if (m_fSmoothTimes != null)
					UpdateSmoothTime(m_fCurrentTime - m_fLastTime);
			}
		} else {
			m_fLastEngineTime = GetEngineTime();
		}
		return m_fCurrentTime;
	}

	public float GetTime()
	{
		return UpdateTimer();
	}

	public float GetDeltaTime()
	{
		if (m_bEnable)
		{
			if (Time.timeScale == 0)
				return 0;
			UpdateTimer();
			return m_fCurrentTime - m_fLastTime;
		}
		return 0;
	}

	public float GetSmoothDeltaTime()
	{
		if (m_bEnable)
		{
			if (Time.timeScale == 0)
				return 0;
			if (m_fSmoothTimes == null)
				InitSmoothTime();
			UpdateTimer();
			return m_fLastSmoothDeltaTime;
		}
		return 0;
	}

	public bool IsEnable()
	{
		return m_bEnable;
	}

	public void Start()
	{
		m_bEnable			= true;
		m_fCurrentTime		= 0;
		m_fLastEngineTime	= GetEngineTime() - 0.000001f;
		UpdateTimer();
	}

	public void Reset(float fElapsedTime)
	{
		m_fCurrentTime		= fElapsedTime;
		m_fLastEngineTime	= GetEngineTime() - 0.000001f;
		UpdateTimer();
	}

	public void Pause()
	{
		UpdateTimer();
		m_bEnable = false;
		UpdateTimer();
	}

	public void Resume()
	{
		UpdateTimer();
		m_bEnable = true;
		UpdateTimer();
	}

	public void SetTimeScale(float fTimeScale)
	{
		m_fTimeScale = fTimeScale;
	}

	protected virtual float GetTimeScale()
	{
		return m_fTimeScale;
	}

	// Control Function -----------------------------------------------------------------
}
