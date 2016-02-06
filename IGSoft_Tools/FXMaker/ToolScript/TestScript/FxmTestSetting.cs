// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;

public class FxmTestSetting : MonoBehaviour
{
	// -------------------------------------------------------------------------------------------
	public		int					m_nPlayIndex			= 0;
	public		int					m_nTransIndex			= 0;
	public		FxmTestControls.AXIS m_nTransAxis			= FxmTestControls.AXIS.Z;
	public		float				m_fTransRate			= 1.0f;
	public		float				m_fStartPosition		= 0;

	public		float				m_fDistPerTime			= 10.0f;
	public		int					m_nRotateIndex			= 0;
	public		int					m_nMultiShotCount		= 1;

	protected	float[]		m_fPlayToolbarTimes		= {/*AutoRepeat*/ 1.0f, 1.0f, /*OneShot*/ 1.0f, /*Repeat*/ 0.3f, 0.6f, 1.0f, 2.0f, 3.0f};

	// -------------------------------------------------------------------------------------------
	public string[]	GetPlayContents()
	{
		int nRepeatIndex = 3;
		GUIContent[] cons = FxmTestControls.GetHcEffectControls_Play(0, 0, m_fPlayToolbarTimes[1], m_fPlayToolbarTimes[nRepeatIndex], m_fPlayToolbarTimes[nRepeatIndex+1], m_fPlayToolbarTimes[nRepeatIndex+2], m_fPlayToolbarTimes[nRepeatIndex+3], m_fPlayToolbarTimes[nRepeatIndex+4]);
		return NgConvert.ContentsToStrings(cons);
	}
}


