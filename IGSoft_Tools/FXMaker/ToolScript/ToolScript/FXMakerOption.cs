// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;

public class FXMakerOption : MonoBehaviour
{
	// Attribute ------------------------------------------------------------------------
	public		static FXMakerOption	inst;

	// Help Option
	public		bool				m_bHintRedBox					= true;
	public		bool				m_bShowCursorTooltip			= true;
	public		bool				m_bShowBottomTooltip			= false;
	public		enum LANGUAGE		{ENGLISH=0, KOREAN};
	public		LANGUAGE			m_LanguageType					= LANGUAGE.ENGLISH;

	// GUILayout Option
	public		int					m_nMinTopToolbarCount			= 8;
	public		int					m_nMinBottomToolbarCount		= 10;
	public		float				m_fFixedWindowWidth				= 115;
	public		float				m_fTopMenuHeight				= 92;

	// AutoSave Option
	public		bool				m_bActiveRecycleBin				= true;
	public		bool				m_bAutoSaveAppPause				= false;
	public		bool				m_bAutoSaveAppQuit				= false;

	// 3DLayout Option
	public		float				m_fScaleTransSpeed				= 1.0f;
	public		AnimationCurve		m_SimulateArcCurve;
	public		float				m_GridUnit						= 1.0f;
	public		float				m_GridSize						= 24.0f;
// 	public		bool				m_bGizmoGridMoveUnit			= false;
	public		float				m_fGizmoGridMoveUnit			= 1.0f;
	public		enum ARROWMOVE_TYPE	{ADD, MULTIPLE};
	public		ARROWMOVE_TYPE		m_ArrowGridMoveType				= ARROWMOVE_TYPE.MULTIPLE;
	public		float				m_fArrowGridMoveUnit			= 1.0f;
	public		int[]				m_nCameraAangleXValues			= { 0, 20, 40,  90 };
	public		int[]				m_nCameraAangleYValues			= { 0, 45, 90, 180 };

	// EffectHierarchy
	public		bool				m_bDragDrop_WorldSpace			= true;
	public		Color				m_ColorRootBoundsBox			= new Color(0.1f, 0.9f, 0.1f, 0.5f);
	public		Color				m_ColorChildBoundsBox			= new Color(0.9f, 0.1f, 0.1f, 0.3f);
	public		Color				m_ColorRootWireframe			= new Color(0.3f, 0.5f, 0.7f, 0.3f);
	public		Color				m_ColorChildWireframe			= new Color(0.7f, 0.5f, 0.7f, 0.3f);

	// Dialog
	public		bool				m_bUpdateNewMaterial			= false;
	public		enum DLG_RIGHTCLICK	{PINGOBJECT, APPEND};
	public		DLG_RIGHTCLICK		m_PrefabDlg_RightClick			= DLG_RIGHTCLICK.APPEND;

	// Reset
	public		bool				m_bResetTimeScaleAppQuit		= true;

	// Sprite
	public		AnimationCurve		m_AlphaWeightCurve;

	//	[HideInInspector]

	// Property -------------------------------------------------------------------------

	// Control --------------------------------------------------------------------------
	// UpdateLoop -----------------------------------------------------------------------
	FXMakerOption()
	{
		inst = this;
	}

	void Awake()
	{
		FXMakerTooltip.CheckAllFunction();
		if (m_nCameraAangleXValues.Length != 4)
			m_nCameraAangleXValues	= new int[] { 0, 20, 40,  90 };
		if (m_nCameraAangleYValues.Length != 4)
			m_nCameraAangleYValues	= new int[] { 0, 45, 90, 180 };
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
	public static void NormalizeCurveTime(AnimationCurve curve)
	{
		int n = 0;
		while (n < curve.keys.Length)
		{
			Keyframe	key = curve[n];
			float	fMax = Mathf.Max(0, key.time);
			float	fVal = Mathf.Min(1, Mathf.Max(fMax, key.time));
			if (fVal != key.time)
			{
				Keyframe	newKey = new Keyframe(fVal, key.value, key.inTangent, key.outTangent);
				curve.RemoveKey(n);
				n = 0;
				curve.AddKey(newKey);
				continue;
			}
			n++;
		}
	}
}
#endif
