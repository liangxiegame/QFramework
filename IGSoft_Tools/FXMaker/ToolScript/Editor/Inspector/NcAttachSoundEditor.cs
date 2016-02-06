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

[CustomEditor(typeof(NcAttachSound))]

public class NcAttachSoundEditor : FXMakerEditor
{
	// Attribute ------------------------------------------------------------------------
	protected	NcAttachSound		m_Sel;
	protected	FxmPopupManager		m_FxmPopupManager;

	// Property -------------------------------------------------------------------------
	// Event Function -------------------------------------------------------------------
    void OnEnable()
    {
 		m_Sel = target as NcAttachSound;
 		m_UndoManager	= new FXMakerUndoManager(m_Sel, "NcAttachSound");
   }

    void OnDisable()
    {
// 		if (m_FxmPopupManager != null && m_FxmPopupManager.IsShowByInspector())
// 			m_FxmPopupManager.CloseNcCurvePopup();
    }

	public override void OnInspectorGUI()
	{
		AddScriptNameField(m_Sel);
		m_UndoManager.CheckUndo();

		Rect	rect;

		m_FxmPopupManager = null;		// GetFxmPopupManager();

		// --------------------------------------------------------------
		bool bClickButton = false;
		EditorGUI.BeginChangeCheck();
		{
//			DrawDefaultInspector();
			m_Sel.m_fUserTag = EditorGUILayout.FloatField(GetCommonContent("m_fUserTag"), m_Sel.m_fUserTag);

			m_Sel.m_PlayType			= (NcAttachSound.PLAY_TYPE)EditorGUILayout.EnumPopup("m_PlayType"		, m_Sel.m_PlayType);
			if (m_Sel.m_PlayType != NcAttachSound.PLAY_TYPE.MultiPlay)
				m_Sel.m_bSharedAudioSource	= EditorGUILayout.Toggle	(GetHelpContent("m_bSharedAudioSource")	, m_Sel.m_bSharedAudioSource);
			m_Sel.m_bPlayOnActive		= EditorGUILayout.Toggle	(GetHelpContent("m_bPlayOnActive")			, m_Sel.m_bPlayOnActive);
			m_Sel.m_fDelayTime			= EditorGUILayout.FloatField(GetHelpContent("m_fDelayTime")				, m_Sel.m_fDelayTime);
			m_Sel.m_fRepeatTime			= EditorGUILayout.FloatField(GetHelpContent("m_fRepeatTime")			, m_Sel.m_fRepeatTime);
			m_Sel.m_nRepeatCount		= EditorGUILayout.IntField	(GetHelpContent("m_nRepeatCount")			, m_Sel.m_nRepeatCount);
			m_Sel.m_AudioClip			= (AudioClip)EditorGUILayout.ObjectField(GetHelpContent("m_AudioClip")	, m_Sel.m_AudioClip, typeof(AudioClip), false, null);
			m_Sel.m_nPriority			= EditorGUILayout.IntField	(GetHelpContent("m_nPriority")				, m_Sel.m_nPriority);
			m_Sel.m_bLoop				= EditorGUILayout.Toggle	(GetHelpContent("m_bLoop")					, m_Sel.m_bLoop);
			m_Sel.m_fVolume				= EditorGUILayout.Slider(GetHelpContent("m_fVolume")					, m_Sel.m_fVolume, 0, 1.0f, null);
			m_Sel.m_fPitch				= EditorGUILayout.Slider(GetHelpContent("m_fPitch")						, m_Sel.m_fPitch, -3, 3.0f, null);

			// check
			SetMinValue(ref m_Sel.m_fDelayTime, 0);
			SetMinValue(ref m_Sel.m_fRepeatTime, 0);
			SetMinValue(ref m_Sel.m_nRepeatCount, 0);

			// --------------------------------------------------------------
			EditorGUILayout.Space();
			rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(m_fButtonHeight));
			{
				if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(rect, 2, 0, 1), GetHelpContent("Select AudioClip"), (m_FxmPopupManager != null)))
					m_FxmPopupManager.ShowSelectAudioClipPopup(m_Sel);
				if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(rect, 2, 1, 1), GetHelpContent("Clear AudioClip"), (m_Sel.m_AudioClip != null)))
				{
					bClickButton		= true;
					m_Sel.m_AudioClip	= null;
				}
				GUILayout.Label("");
			}
			EditorGUILayout.EndHorizontal();
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
		string text		= FXMakerTooltip.GetHsEditor_NcAttachSound(tooltip);
		return GetHelpContent(caption, text);
	}

	protected override void HelpBox(string caption)
	{
		string	str	= caption;
		if (caption == "" || caption == "Script")
			str = FXMakerTooltip.GetHsEditor_NcAttachSound("");
		base.HelpBox(str);
	}
}
