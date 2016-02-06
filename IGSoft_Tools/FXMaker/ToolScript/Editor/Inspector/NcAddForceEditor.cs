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

[CustomEditor(typeof(NcAddForce))]

public class NcAddForceEditor : FXMakerEditor
{
	// Attribute ------------------------------------------------------------------------
	protected	NcAddForce		m_Sel;

	// Property -------------------------------------------------------------------------
	// Event Function -------------------------------------------------------------------
    void OnEnable()
    {
 		m_Sel = target as NcAddForce;
 		m_UndoManager	= new FXMakerUndoManager(m_Sel, "NcAddForce");
   }

    void OnDisable()
    {
    }

	public override void OnInspectorGUI()
	{
		AddScriptNameField(m_Sel);
		m_UndoManager.CheckUndo();

		Rect			rect;

		// --------------------------------------------------------------
		bool bClickButton = false;
		EditorGUI.BeginChangeCheck();
		{
			m_Sel.m_fUserTag = EditorGUILayout.FloatField(GetCommonContent("m_fUserTag"), m_Sel.m_fUserTag);

			m_Sel.m_AddForce				= EditorGUILayout.Vector3Field	("m_AddForce"							, m_Sel.m_AddForce, null);
			m_Sel.m_RandomRange				= EditorGUILayout.Vector3Field	("m_RandomRange"						, m_Sel.m_RandomRange, null);
			m_Sel.m_ForceMode				= (ForceMode)EditorGUILayout.EnumPopup(GetHelpContent("m_ForceMode")	, m_Sel.m_ForceMode, GUILayout.MaxWidth(Screen.width));

			// check

			// --------------------------------------------------------------
			if (m_Sel.GetComponent<Rigidbody>() == false)
			{
				EditorGUILayout.Space();
				rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(m_fButtonHeight));
				{

					FXMakerLayout.GUIColorBackup(FXMakerLayout.m_ColorHelpBox);
					if (FXMakerLayout.GUIButton(rect, GetHelpContent("Add RigidBody Component"), true))
						m_Sel.gameObject.AddComponent<Rigidbody>();
					FXMakerLayout.GUIColorRestore();
					GUILayout.Label("");
				}
				EditorGUILayout.EndHorizontal();
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
		string text		= FXMakerTooltip.GetHsEditor_NcAddForce(tooltip);
		return GetHelpContent(caption, text);
	}

	protected override void HelpBox(string caption)
	{
		string	str	= caption;
		if (caption == "" || caption == "Script")
			str = FXMakerTooltip.GetHsEditor_NcAddForce("");
		base.HelpBox(str);
	}
}
