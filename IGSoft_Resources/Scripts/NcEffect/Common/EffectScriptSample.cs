// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;

public class EffectScriptSample : NcEffectBehaviour
{
	// Attribute ------------------------------------------------------------------------

	//	[HideInInspector]

	// Property -------------------------------------------------------------------------
#if UNITY_EDITOR
	public override string CheckProperty()
	{
		return "";	// no error
	}
#endif

	// Control --------------------------------------------------------------------------

	// UpdateLoop -----------------------------------------------------------------------
	void Awake()
	{
	}

	void Start()
	{
	}

	void Update()
	{
	}

	void FixedUpdate()
	{
	}

	public void LateUpdate()
	{
	}

	// Event -------------------------------------------------------------------------
	void OnDrawGizmos()
	{
	}

	// Function ----------------------------------------------------------------------
}
