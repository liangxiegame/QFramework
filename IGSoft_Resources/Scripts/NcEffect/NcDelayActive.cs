// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

// [AddComponentMenu("FXMaker/NcDelayActive	%#F")]

public class NcDelayActive : NcEffectBehaviour
{
	public		string		NotAvailable	 = "This component is not available.";

	public float GetParentDelayTime(bool bCheckStarted)	{ return 0; }


	// Attribute ------------------------------------------------------------------------
	public		float		m_fDelayTime;
	public		bool		m_bActiveRecursively	= true;
	protected	float		m_fAliveTime;

	// read only
	public		float		m_fParentDelayTime;

	protected	bool		m_bAddedInvoke			= false;
	protected	float		m_fStartedTime			= 0;
/*
	// Property -------------------------------------------------------------------------
#if UNITY_EDITOR
	public override string CheckProperty()
	{
		if (1 < gameObject.GetComponents(GetType()).Length)
			return "SCRIPT_WARRING_DUPLICATE";

		return "";	// no error
	}
#endif

	public override int GetAnimationState()
	{
		if (enabled)
		{
			if (m_fStartedTime == 0)
				return 1;
			if (m_fAliveTime != 0 && gameObject.active)
				return 1;
		}
		return 0;
	}

	public bool IsAddedInvoke()
	{
// 		Debug.Log("IsAddedInvoke " + name);
		return m_bAddedInvoke;
	}

	public bool IsStarted()
	{
		return m_fStartedTime != 0;
	}

	// Loop Function --------------------------------------------------------------------
	void Awake()
	{
		m_fAliveTime	= 0;	// disable
		m_fStartedTime	= 0;

		if (enabled == false)
			return;

// 		Debug.Log("Awake" + m_fDelayTime + gameObject.name);
		if (m_fDelayTime != 0)
		{
			if (m_bActiveRecursively)
				SetActiveRecursively(gameObject, false);
			else gameObject.active = false;
		}
#if UNITY_EDITOR
		if (IsCreatingEditObject() == false)
	 		InitNcDelayActive();
#else
 		InitNcDelayActive();
#endif
	}

	public void InitNcDelayActive()
	{
		return;	// not use
// 		Debug.Log("InitNcDelayActive " + m_fStartedTime + gameObject.name);
		if (m_bAddedInvoke == false)
		{
			if (m_bActiveRecursively)
				SetActiveRecursively(gameObject, false);
			else gameObject.active = false;

			float	fInvokeTime = GetParentDelayTime(true) + m_fDelayTime;
//  			Debug.Log("InitNcDelayActive " + fInvokeTime + gameObject.name);
			Invoke("OnStartActive", fInvokeTime);
			m_bAddedInvoke = true;
		}
	}

	void Update()
	{
		if (0 < m_fStartedTime && m_fAliveTime != 0)
		{
// 			Debug.Log("Active.Update");
			if (m_fStartedTime + m_fAliveTime < GetEngineTime())
				OnEndActive();
		}
	}

	// Control Function -----------------------------------------------------------------
	public float GetParentDelayTime(bool bCheckStarted)
	{
		Transform	parentTrans = transform.parent;

		m_fParentDelayTime = 0;
		while (parentTrans != null)
		{
			NcDelayActive	ncInvoke = (NcDelayActive)parentTrans.gameObject.GetComponent(typeof(NcDelayActive));
			parentTrans = parentTrans.parent;

			if (ncInvoke != null)
			{
				if (bCheckStarted && ncInvoke.IsStarted())
					return m_fParentDelayTime;
				m_fParentDelayTime += ncInvoke.m_fDelayTime;
// 				Debug.Log(m_fParentDelayTime);
			}
		}
		return m_fParentDelayTime;
	}

	public void CancelDelayActive()
	{
// 		Debug.Log("CancelDelayActive " + name);
		m_fStartedTime = 0;
		CancelInvoke("OnStartActive");
		m_bAddedInvoke = false;
	}

	// Event Function -------------------------------------------------------------------
	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		m_fDelayTime	/= fSpeedRate;
		m_fAliveTime	/= fSpeedRate;
	}

	void OnStartActive()
	{
//   		Debug.Log("OnStartActive" + m_fStartedTime + gameObject.name);
		if (0 < m_fStartedTime)
			return;
		m_fStartedTime = GetEngineTime();
		gameObject.active = true;

		if (m_bActiveRecursively)
			for (int n = 0; n < transform.childCount; n++)
				SetChildActive(transform.GetChild(n));
	}

	void SetChildActive(Transform trans)
	{
		NcDelayActive ac = trans.GetComponent<NcDelayActive>();
		if (ac == null || ac.enabled == false)
			trans.gameObject.active = true;
		if (ac != null)
		{
			if (ac.IsAddedInvoke() == false)
				trans.gameObject.active = true;
			if (ac.m_bActiveRecursively)
				return;
		}

		for (int n = 0; n < trans.childCount; n++)
			SetChildActive(trans.GetChild(n));
	}

	void OnEndActive()
	{
		if (m_bActiveRecursively)
			SetActiveRecursively(gameObject, false);
		else gameObject.active = false;
	}
*/
}
