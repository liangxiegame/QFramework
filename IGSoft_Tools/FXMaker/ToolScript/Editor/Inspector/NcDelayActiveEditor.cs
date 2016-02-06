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
/*
[CustomEditor(typeof(NcDelayActive))]

public class NcDelayActiveEditor : FXMakerEditor
{
	// Attribute ------------------------------------------------------------------------
	protected	NcDelayActive	m_Sel;

	// Property -------------------------------------------------------------------------
	// Event Function -------------------------------------------------------------------
    void OnEnable()
    {
 		m_Sel = target as NcDelayActive;
 		m_UndoManager	= new FXMakerUndoManager(m_Sel, "NcDelayActive");
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
			m_Sel.m_fDelayTime			= EditorGUILayout.FloatField(GetHelpContent("m_fDelayTime")				, m_Sel.m_fDelayTime);
			m_Sel.m_bActiveRecursively	= EditorGUILayout.Toggle	(GetHelpContent("m_bActiveRecursively")		, true);	// fix true
// 			m_Sel.m_fAliveTime			= EditorGUILayout.FloatField(GetHelpContent("m_fAliveTime")				, m_Sel.m_fAliveTime);
// 			EditorGUILayout.FloatField(GetHelpContent("m_fStartedTime")				, m_Sel.m_fStartedTime);
			EditorGUILayout.FloatField(GetHelpContent("m_fParentDelayTime")			, m_Sel.m_fParentDelayTime);

			// check
			SetMinValue(ref m_Sel.m_fDelayTime, 0);

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
		string text		= FXMakerTooltip.GetHsEditor_NcDelayActive(tooltip);
		return GetHelpContent(caption, text);
	}

	protected override void HelpBox(string caption)
	{
		string	str	= caption;
		if (caption == "" || caption == "Script")
			str = FXMakerTooltip.GetHsEditor_NcDelayActive("");
		base.HelpBox(str);
	}
}
*/