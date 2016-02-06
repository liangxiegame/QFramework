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

[CustomEditor(typeof(NcDuplicator))]

public class NcDuplicatorEditor : FXMakerEditor
{
	// Attribute ------------------------------------------------------------------------
	protected	NcDuplicator		m_Sel;

	// Property -------------------------------------------------------------------------
	// Event Function -------------------------------------------------------------------
    void OnEnable()
    {
 		m_Sel = target as NcDuplicator;
 		m_UndoManager	= new FXMakerUndoManager(m_Sel, "NcDuplicator");
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

			m_Sel.m_fDuplicateTime		= EditorGUILayout.FloatField	(GetHelpContent("m_fDuplicateTime")		, m_Sel.m_fDuplicateTime);
			m_Sel.m_nDuplicateCount		= EditorGUILayout.IntField		(GetHelpContent("m_nDuplicateCount")	, m_Sel.m_nDuplicateCount);
			m_Sel.m_fDuplicateLifeTime	= EditorGUILayout.FloatField	(GetHelpContent("m_fDuplicateLifeTime")	, m_Sel.m_fDuplicateLifeTime);
			m_Sel.m_RandomRange			= EditorGUILayout.Vector3Field	("m_RandomRange"						, m_Sel.m_RandomRange, null);
			m_Sel.m_AddStartPos			= EditorGUILayout.Vector3Field	("m_AddStartPos"						, m_Sel.m_AddStartPos, null);
			m_Sel.m_AccumStartRot		= EditorGUILayout.Vector3Field	("m_AccumStartRot"						, m_Sel.m_AccumStartRot, null);

			SetMinValue(ref m_Sel.m_fDuplicateTime, 0.01f);
			SetMinValue(ref m_Sel.m_nDuplicateCount, 0);
			SetMinValue(ref m_Sel.m_fDuplicateLifeTime, 0);

			// err check
			if (GetFXMakerMain())
				if (m_Sel.gameObject == GetFXMakerMain().GetOriginalEffectObject())
				{
					m_Sel.enabled = false;
// 					NgLayout.GUIColorBackup(Color.red);
// 					GUILayout.TextArea(GetHsScriptMessage("SCRIPT_ERROR_ROOT", ""), GUILayout.MaxHeight(80));
// 					NgLayout.GUIColorRestore();
				}
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
		string text		= FXMakerTooltip.GetHsEditor_NcDuplicator(tooltip);
		return GetHelpContent(caption, text);
	}

	protected override void HelpBox(string caption)
	{
		string	str	= caption;
		if (caption == "" || caption == "Script")
			str = FXMakerTooltip.GetHsEditor_NcDuplicator("");
		base.HelpBox(str);
	}
}
