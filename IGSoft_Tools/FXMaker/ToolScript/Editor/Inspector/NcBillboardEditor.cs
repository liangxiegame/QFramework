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

[CustomEditor(typeof(NcBillboard))]

public class NcBillboardEditor : FXMakerEditor
{
	// Attribute ------------------------------------------------------------------------
	protected	NcBillboard		m_Sel;

	// Property -------------------------------------------------------------------------
	// Event Function -------------------------------------------------------------------
    void OnEnable()
    {
 		m_Sel = target as NcBillboard;
 		m_UndoManager	= new FXMakerUndoManager(m_Sel, "NcBillboard");
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

			m_Sel.m_bCameraLookAt		= EditorGUILayout.Toggle(GetHelpContent("m_bCameraLookAt")			, m_Sel.m_bCameraLookAt);
			m_Sel.m_bFixedObjectUp		= EditorGUILayout.Toggle(GetHelpContent("m_bFixedObjectUp")			, m_Sel.m_bFixedObjectUp);
			m_Sel.m_bFixedStand			= EditorGUILayout.Toggle(GetHelpContent("m_bFixedStand")			, m_Sel.m_bFixedStand);
			m_Sel.m_FrontAxis			= (NcBillboard.AXIS_TYPE)EditorGUILayout.EnumPopup(GetHelpContent("m_FrontAxis"), m_Sel.m_FrontAxis, GUILayout.MaxWidth(Screen.width));

			m_Sel.m_RatationMode		= (NcBillboard.ROTATION)EditorGUILayout.EnumPopup(GetHelpContent("m_RatationMode"), m_Sel.m_RatationMode, GUILayout.MaxWidth(Screen.width));
			if (m_Sel.m_RatationMode == NcBillboard.ROTATION.RND || m_Sel.m_RatationMode == NcBillboard.ROTATION.ROTATE)
				m_Sel.m_RatationAxis	= (NcBillboard.AXIS)EditorGUILayout.EnumPopup(GetHelpContent("m_RatationAxis"), m_Sel.m_RatationAxis, GUILayout.MaxWidth(Screen.width));
			if (m_Sel.m_RatationMode == NcBillboard.ROTATION.ROTATE)
				m_Sel.m_fRotationValue	= EditorGUILayout.FloatField(GetHelpContent("m_fRotationValue")		, m_Sel.m_fRotationValue);
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
		string text		= FXMakerTooltip.GetHsEditor_NcBillboard(tooltip);
		return GetHelpContent(caption, text);
	}

	protected override void HelpBox(string caption)
	{
		string	str	= caption;
		if (caption == "" || caption == "Script")
			str = FXMakerTooltip.GetHsEditor_NcBillboard("");
		base.HelpBox(str);
	}
}
