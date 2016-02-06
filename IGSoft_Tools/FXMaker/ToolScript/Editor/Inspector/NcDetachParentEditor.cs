// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

// --------------------------------------------------------------------------------------
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;

[CustomEditor(typeof(NcDetachParent))]

public class NcDetachParentEditor : FXMakerEditor
{
	// Attribute ------------------------------------------------------------------------
	protected	NcDetachParent		m_Sel;

	// Property -------------------------------------------------------------------------
	// Event Function -------------------------------------------------------------------
    void OnEnable()
    {
 		m_Sel = target as NcDetachParent;
 		m_UndoManager	= new FXMakerUndoManager(m_Sel, "NcDetachParent");
   }

    void OnDisable()
    {
    }

	public override void OnInspectorGUI()
	{
		AddScriptNameField(m_Sel);
		m_UndoManager.CheckUndo();
		// --------------------------------------------------------------
		bool bClickButton = false;
		EditorGUI.BeginChangeCheck();
		{
//			DrawDefaultInspector();
			m_Sel.m_fUserTag = EditorGUILayout.FloatField(GetCommonContent("m_fUserTag"), m_Sel.m_fUserTag);

			m_Sel.m_bFollowParentTransform			= EditorGUILayout.Toggle	(GetHelpContent("m_bFollowParentTransform")		, m_Sel.m_bFollowParentTransform);
			m_Sel.m_bParentHideToStartDestroy		= EditorGUILayout.Toggle	(GetHelpContent("m_bParentHideToStartDestroy")	, m_Sel.m_bParentHideToStartDestroy);
			m_Sel.m_bDisableEmit					= EditorGUILayout.Toggle	(GetHelpContent("m_bDisableEmit")				, m_Sel.m_bDisableEmit);
			m_Sel.m_fSmoothDestroyTime				= EditorGUILayout.FloatField(GetHelpContent("m_fSmoothDestroyTime")			, m_Sel.m_fSmoothDestroyTime);
			if (0 < m_Sel.m_fSmoothDestroyTime)
			{
				m_Sel.m_bSmoothHide					= EditorGUILayout.Toggle	(GetHelpContent("m_bSmoothHide")				, m_Sel.m_bSmoothHide);
				m_Sel.m_bMeshFilterOnlySmoothHide	= EditorGUILayout.Toggle	(GetHelpContent("m_bMeshFilterOnlySmoothHide")	, m_Sel.m_bMeshFilterOnlySmoothHide);
			}

// 			// check
// 			if (m_Sel.m_bFollowParentTransform && (m_Sel.GetComponent<NcCurveAnimation>() != null || m_Sel.GetComponent<NcRotation>() != null || m_Sel.GetComponent<Animation>() != null))
// 				WarringBox(FXMakertip.GetHsScriptMessage("SCRIPT_WARRING_NCDETACHPARENT"));
		}
		m_UndoManager.CheckDirty();
		// --------------------------------------------------------------
		if ((EditorGUI.EndChangeCheck() || bClickButton) && GetFXMakerMain())
			OnEditComponent();
		// ---------------------------------------------------------------------
		if (GUI.tooltip != "")
			m_LastTooltip	= GUI.tooltip;
		HelpBox(m_LastTooltip);
	}

	// ----------------------------------------------------------------------------------
	// ----------------------------------------------------------------------------------
	protected GUIContent GetHelpContent(string tooltip)
	{
		string caption	= tooltip;
		string text		= FXMakerTooltip.GetHsEditor_NcDetachParent(tooltip);
		return GetHelpContent(caption, text);
	}

	protected override void HelpBox(string caption)
	{
		string	str	= caption;
		if (caption == "" || caption == "Script")
			str = FXMakerTooltip.GetHsEditor_NcDetachParent("");
		base.HelpBox(str);
	}
}
