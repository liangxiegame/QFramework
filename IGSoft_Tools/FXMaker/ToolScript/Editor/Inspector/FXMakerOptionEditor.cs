// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

// --------------------------------------------------------------------------------------
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;

[CustomEditor(typeof(FXMakerOption))]

public class FXMakerOptionEditor : FXMakerEditor
{
	// Attribute ------------------------------------------------------------------------
	protected	FXMakerOption	m_Sel;

	// Property -------------------------------------------------------------------------
	// Event Function -------------------------------------------------------------------
	void OnEnable()
    {
		m_Sel			= target as FXMakerOption;
		m_UndoManager	= new FXMakerUndoManager(m_Sel, "FXMakerOption");
    }

    void OnDisable()
    {
    }

	public override void OnInspectorGUI()
	{
//		base.OnInspectorGUI();

		AddScriptNameField(m_Sel);
		m_UndoManager.CheckUndo();

		// Help Option
		m_Sel.m_bHintRedBox				= EditorGUILayout.Toggle(GetHelpContent("m_bHintRedBox")				, m_Sel.m_bHintRedBox);
		m_Sel.m_bShowCursorTooltip		= EditorGUILayout.Toggle(GetHelpContent("m_bShowCursorTooltip")			, m_Sel.m_bShowCursorTooltip);
		m_Sel.m_bShowBottomTooltip		= EditorGUILayout.Toggle(GetHelpContent("m_bShowBottomTooltip")			, m_Sel.m_bShowBottomTooltip);
		m_Sel.m_LanguageType			= (FXMakerOption.LANGUAGE)EditorGUILayout.EnumPopup(GetHelpContent("m_LanguageType"), m_Sel.m_LanguageType, GUILayout.MaxWidth(Screen.width));

 		// GUILayout Option
		m_Sel.m_nMinTopToolbarCount		= EditorGUILayout.IntField(GetHelpContent("m_nMinTopToolbarCount")		, m_Sel.m_nMinTopToolbarCount);
		m_Sel.m_nMinBottomToolbarCount	= EditorGUILayout.IntField(GetHelpContent("m_nMinBottomToolbarCount")	, m_Sel.m_nMinBottomToolbarCount);
		m_Sel.m_fFixedWindowWidth		= EditorGUILayout.FloatField(GetHelpContent("m_fFixedWindowWidth")		, m_Sel.m_fFixedWindowWidth);
		m_Sel.m_fTopMenuHeight			= EditorGUILayout.FloatField(GetHelpContent("m_fTopMenuHeight")			, m_Sel.m_fTopMenuHeight);

	 	// AutoSave Option
		m_Sel.m_bActiveRecycleBin		= EditorGUILayout.Toggle(GetHelpContent("m_bActiveRecycleBin")			, m_Sel.m_bActiveRecycleBin);
		m_Sel.m_bAutoSaveAppPause		= EditorGUILayout.Toggle(GetHelpContent("m_bAutoSaveAppPause")			, m_Sel.m_bAutoSaveAppPause);
		m_Sel.m_bAutoSaveAppQuit		= EditorGUILayout.Toggle(GetHelpContent("m_bAutoSaveAppQuit")			, m_Sel.m_bAutoSaveAppQuit);

		// 3DLayout Option
		m_Sel.m_fScaleTransSpeed		= EditorGUILayout.FloatField(GetHelpContent("m_fScaleTransSpeed")		, m_Sel.m_fScaleTransSpeed);
		m_Sel.m_SimulateArcCurve		= EditorGUILayout.CurveField(GetHelpContent("m_SimulateArcCurve")		, m_Sel.m_SimulateArcCurve);
		m_Sel.m_GridUnit				= EditorGUILayout.FloatField(GetHelpContent("m_GridUnit")				, m_Sel.m_GridUnit);
		m_Sel.m_GridSize				= EditorGUILayout.FloatField(GetHelpContent("m_GridSize")				, m_Sel.m_GridSize);
// 		m_Sel.m_bGizmoGridMoveUnit		= EditorGUILayout.Toggle(GetHelpContent("m_bGizmoGridMoveUnit")			, m_Sel.m_bGizmoGridMoveUnit);
		m_Sel.m_fGizmoGridMoveUnit		= EditorGUILayout.FloatField(GetHelpContent("m_fGizmoGridMoveUnit")		, m_Sel.m_fGizmoGridMoveUnit);
		m_Sel.m_ArrowGridMoveType		= (FXMakerOption.ARROWMOVE_TYPE)EditorGUILayout.EnumPopup(GetHelpContent("m_ArrowGridMoveType"), m_Sel.m_ArrowGridMoveType);
		m_Sel.m_fArrowGridMoveUnit		= EditorGUILayout.FloatField(GetHelpContent("m_fArrowGridMoveUnit")			, m_Sel.m_fArrowGridMoveUnit);

		m_Sel.m_nCameraAangleXValues[0]	= EditorGUILayout.IntField(GetHelpContent("m_nCameraAangleXValues0")	, m_Sel.m_nCameraAangleXValues[0]);
		m_Sel.m_nCameraAangleXValues[1]	= EditorGUILayout.IntField(GetHelpContent("m_nCameraAangleXValues1")	, m_Sel.m_nCameraAangleXValues[1]);
		m_Sel.m_nCameraAangleXValues[2]	= EditorGUILayout.IntField(GetHelpContent("m_nCameraAangleXValues2")	, m_Sel.m_nCameraAangleXValues[2]);
		m_Sel.m_nCameraAangleXValues[3]	= EditorGUILayout.IntField(GetHelpContent("m_nCameraAangleXValues3")	, m_Sel.m_nCameraAangleXValues[3]);
		m_Sel.m_nCameraAangleYValues[0]	= EditorGUILayout.IntField(GetHelpContent("m_nCameraAangleYValues0")	, m_Sel.m_nCameraAangleYValues[0]);
		m_Sel.m_nCameraAangleYValues[1]	= EditorGUILayout.IntField(GetHelpContent("m_nCameraAangleYValues1")	, m_Sel.m_nCameraAangleYValues[1]);
		m_Sel.m_nCameraAangleYValues[2]	= EditorGUILayout.IntField(GetHelpContent("m_nCameraAangleYValues2")	, m_Sel.m_nCameraAangleYValues[2]);
		m_Sel.m_nCameraAangleYValues[3]	= EditorGUILayout.IntField(GetHelpContent("m_nCameraAangleYValues3")	, m_Sel.m_nCameraAangleYValues[3]);

		// EffectHierarchy
		m_Sel.m_bDragDrop_WorldSpace	= EditorGUILayout.Toggle(GetHelpContent("m_bDragDrop_WorldSpace")		, m_Sel.m_bDragDrop_WorldSpace);
		m_Sel.m_ColorRootBoundsBox		= EditorGUILayout.ColorField(GetHelpContent("m_ColorRootBoundsBox")		, m_Sel.m_ColorRootBoundsBox);
		m_Sel.m_ColorChildBoundsBox		= EditorGUILayout.ColorField(GetHelpContent("m_ColorChildBoundsBox")	, m_Sel.m_ColorChildBoundsBox);
		m_Sel.m_ColorRootWireframe		= EditorGUILayout.ColorField(GetHelpContent("m_ColorRootWireframe")		, m_Sel.m_ColorRootWireframe);
		m_Sel.m_ColorChildWireframe		= EditorGUILayout.ColorField(GetHelpContent("m_ColorChildWireframe")	, m_Sel.m_ColorChildWireframe);

		// Popup
		m_Sel.m_bUpdateNewMaterial		= EditorGUILayout.Toggle(GetHelpContent("m_bUpdateNewMaterial")			, m_Sel.m_bUpdateNewMaterial);
		m_Sel.m_PrefabDlg_RightClick	= (FXMakerOption.DLG_RIGHTCLICK)EditorGUILayout.EnumPopup(GetHelpContent("m_PrefabDlg_RightClick"), m_Sel.m_PrefabDlg_RightClick, GUILayout.MaxWidth(Screen.width));

		{
			EditorPrefs.SetInt	("FXMakerOption.m_LanguageType"		, (int)m_Sel.m_LanguageType);
			EditorPrefs.SetFloat("FXMakerOption.m_fFixedWindowWidth"	, m_Sel.m_fFixedWindowWidth);
			EditorPrefs.SetFloat("FXMakerOption.m_fTopMenuHeight"		, m_Sel.m_fTopMenuHeight);
		}

		EditorGUI.BeginChangeCheck();
		m_Sel.m_AlphaWeightCurve		= EditorGUILayout.CurveField(GetHelpContent("m_AlphaWeightCurve")		, m_Sel.m_AlphaWeightCurve);
		if (EditorGUI.EndChangeCheck())
			FXMakerOption.NormalizeCurveTime(m_Sel.m_AlphaWeightCurve);

		m_UndoManager.CheckDirty();

		if (GUI.tooltip != "")
			m_LastTooltip	= GUI.tooltip;
		HelpBox(m_LastTooltip);
	}

	// Control Function -----------------------------------------------------------------
	protected GUIContent GetHelpContent(string tooltip)
	{
		string caption	= tooltip;
		string text		= FXMakerTooltip.GetHsEditor_FXMakerOption(tooltip);
		return GetHelpContent(caption, text);
	}

	protected override void HelpBox(string caption)
	{
		string	keyword	= caption;
		int		index	= caption.IndexOf('\n');
		if (0 <= index)
			keyword	= caption.Substring(0, index);
 		base.HelpBox(FXMakerTooltip.GetHsEditor_FXMakerOption(keyword));
	}
}
