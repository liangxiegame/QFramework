// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class NcAddForce : NcEffectBehaviour
{
	// Attribute ------------------------------------------------------------------------
	public		Vector3		m_AddForce				= new Vector3(0, 300, 0);
	public		Vector3		m_RandomRange			= new Vector3(100, 100, 100);
	public		ForceMode	m_ForceMode				= ForceMode.Force;

	// Property -------------------------------------------------------------------------
#if UNITY_EDITOR
	public override string CheckProperty()
	{
		if (GetComponent<Rigidbody>() == null)
			return "SCRIPT_NEED_RIGIDBODY";
		return "";	// no error
	}
#endif

	// Loop Function --------------------------------------------------------------------
	void Start()
	{
		if (enabled == false)
			return;
		AddForce();
	}

	// Control Function -----------------------------------------------------------------
	void AddForce()
	{
		// AddForce
		if (GetComponent<Rigidbody>() != null)
		{
			// Random pos
			Vector3 addForce = new Vector3(Random.Range(-m_RandomRange.x, m_RandomRange.x)+m_AddForce.x, Random.Range(-m_RandomRange.y, m_RandomRange.y)+m_AddForce.y, Random.Range(-m_RandomRange.z, m_RandomRange.z)+m_AddForce.z);
			GetComponent<Rigidbody>().AddForce(addForce, m_ForceMode);
		}
	}

	// Event Function -------------------------------------------------------------------
}

