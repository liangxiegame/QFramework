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

[CustomEditor(typeof(NcTilingTexture))]

public class NcTilingTextureEditor : FXMakerEditor
{
	// Attribute ------------------------------------------------------------------------
	protected	NcTilingTexture		m_Sel;

	// Property -------------------------------------------------------------------------
	// Event Function -------------------------------------------------------------------
    void OnEnable()
    {
 		m_Sel = target as NcTilingTexture;
		m_UndoManager	= new FXMakerUndoManager(m_Sel, "NcTilingTexture");
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

			m_Sel.m_fTilingX		= EditorGUILayout.FloatField(GetHelpContent("m_fTilingX")		, m_Sel.m_fTilingX);
			m_Sel.m_fTilingY		= EditorGUILayout.FloatField(GetHelpContent("m_fTilingY")		, m_Sel.m_fTilingY);
			m_Sel.m_fOffsetX		= EditorGUILayout.FloatField(GetHelpContent("m_fOffsetX")		, m_Sel.m_fOffsetX);
			m_Sel.m_fOffsetY		= EditorGUILayout.FloatField(GetHelpContent("m_fOffsetY")		, m_Sel.m_fOffsetY);
			m_Sel.m_bFixedTileSize	= EditorGUILayout.Toggle	(GetHelpContent("m_bFixedTileSize")	, m_Sel.m_bFixedTileSize);
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
		string text		= FXMakerTooltip.GetHsEditor_NcTilingTexture(tooltip);
		return GetHelpContent(caption, text);
	}

	protected override void HelpBox(string caption)
	{
		string	str	= caption;
		if (caption == "" || caption == "Script")
			str = FXMakerTooltip.GetHsEditor_NcTilingTexture("");
		base.HelpBox(str);
	}
}
