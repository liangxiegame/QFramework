// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

// Attribute ------------------------------------------------------------------------
// Property -------------------------------------------------------------------------
// Loop Function --------------------------------------------------------------------
// Control Function -----------------------------------------------------------------
// Event Function -------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class NcRotation : NcEffectBehaviour
{
	// Attribute ------------------------------------------------------------------------
	public	bool		m_bWorldSpace		= false;
	public	Vector3		m_vRotationValue	= new Vector3(0, 360, 0);

	// Property -------------------------------------------------------------------------
#if UNITY_EDITOR
	public override string CheckProperty()
	{
		if (GetComponent<NcBillboard>() != null)
			return "SCRIPT_CLASH_ROTATEBILL";
		return "";	// no error
	}
#endif

	// --------------------------------------------------------------------------
	void Update()
	{
		 transform.Rotate(GetEngineDeltaTime() * m_vRotationValue.x, GetEngineDeltaTime() * m_vRotationValue.y, GetEngineDeltaTime() * m_vRotationValue.z, (m_bWorldSpace ? Space.World : Space.Self));
	}

	// Event Function -------------------------------------------------------------------
	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		m_vRotationValue		*= fSpeedRate;
	}
}
