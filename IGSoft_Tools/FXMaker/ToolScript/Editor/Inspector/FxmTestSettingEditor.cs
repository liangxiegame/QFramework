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

[CustomEditor(typeof(FxmTestSetting))]

public class FxmTestSettingEditor : FXMakerEditor
{
	// Attribute ------------------------------------------------------------------------
	protected	FxmTestSetting		m_Sel;


	// Property -------------------------------------------------------------------------
	// Event Function -------------------------------------------------------------------
    void OnEnable()
    {
 		m_Sel = target as FxmTestSetting;
 		m_UndoManager	= new FXMakerUndoManager(m_Sel, "FxmTestSetting");
   }

    void OnDisable()
    {
    }

	public override void OnInspectorGUI()
	{
		AddScriptNameField(m_Sel);
		m_UndoManager.CheckUndo();
		// --------------------------------------------------------------
		EditorGUI.BeginChangeCheck();
		{
//			DrawDefaultInspector();
			m_Sel.m_nPlayIndex			= EditorGUILayout.Popup("PlayType"						, m_Sel.m_nPlayIndex, m_Sel.GetPlayContents(), GUILayout.MaxWidth(Screen.width));
			m_Sel.m_nTransIndex			= EditorGUILayout.Popup("PlayType"						, m_Sel.m_nTransIndex, NgConvert.ContentsToStrings(FxmTestControls.GetHcEffectControls_Trans(m_Sel.m_nTransAxis)), GUILayout.MaxWidth(Screen.width));
			m_Sel.m_nTransAxis			= (FxmTestControls.AXIS)EditorGUILayout.EnumPopup("TransAxis"	, m_Sel.m_nTransAxis, GUILayout.MaxWidth(Screen.width));
			m_Sel.m_fTransRate			= EditorGUILayout.FloatField("MaxDistance"				, m_Sel.m_fTransRate);
			m_Sel.m_fStartPosition		= EditorGUILayout.FloatField("StartPosition"			, m_Sel.m_fStartPosition);
			m_Sel.m_fDistPerTime		= EditorGUILayout.FloatField("DistPerTime"				, m_Sel.m_fDistPerTime);
			m_Sel.m_nRotateIndex		= EditorGUILayout.Popup("RotateIndex"					, m_Sel.m_nRotateIndex, new string[]{"Rot", "Fix"}, GUILayout.MaxWidth(Screen.width));
			m_Sel.m_nMultiShotCount		= EditorGUILayout.IntField("MultiShotCount"				, m_Sel.m_nMultiShotCount);
		}

		m_UndoManager.CheckDirty();
	}

	// ----------------------------------------------------------------------------------
	// ----------------------------------------------------------------------------------
	protected GUIContent GetHelpContent(string caption)
	{
//  		GUIContent	con = FXMakertip.GetHcEffectControls(caption, "");
// 		string		str = FXMakertip.GetTooltip(FXMakertip.TOOLTIPSPLIT.Tooltip, con.text);
		return new GUIContent(caption, caption);
	}

}
