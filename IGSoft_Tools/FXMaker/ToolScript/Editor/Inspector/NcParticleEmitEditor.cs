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

[CustomEditor(typeof(NcParticleEmit))]

public class NcParticleEmitEditor : FXMakerEditor
{
	// Attribute ------------------------------------------------------------------------
	protected	NcParticleEmit	m_Sel;
	protected	FxmPopupManager		m_FxmPopupManager;

	// Property -------------------------------------------------------------------------
	// Event Function -------------------------------------------------------------------
    void OnEnable()
    {
 		m_Sel = target as NcParticleEmit;
 		m_UndoManager	= new FXMakerUndoManager(m_Sel, "NcParticleEmit");
   }

    void OnDisable()
    {
		if (m_FxmPopupManager != null && m_FxmPopupManager.IsShowByInspector())
			m_FxmPopupManager.CloseNcPrefabPopup();
    }

	public override void OnInspectorGUI()
	{
		AddScriptNameField(m_Sel);
		m_UndoManager.CheckUndo();

		Rect			rect;

		m_FxmPopupManager = GetFxmPopupManager();

		// --------------------------------------------------------------
		bool bClickButton = false;
		EditorGUI.BeginChangeCheck();
		{
//			DrawDefaultInspector();
			m_Sel.m_fUserTag = EditorGUILayout.FloatField(GetCommonContent("m_fUserTag"), m_Sel.m_fUserTag);

			m_Sel.m_AttachType				= (NcParticleEmit.AttachType)EditorGUILayout.EnumPopup(GetHelpContent("m_AttachType")	, m_Sel.m_AttachType, GUILayout.MaxWidth(Screen.width));

			if (m_Sel.m_AttachType == NcParticleEmit.AttachType.Active)
			{
				m_Sel.m_fDelayTime			= EditorGUILayout.FloatField	(GetHelpContent("m_fDelayTime")				, m_Sel.m_fDelayTime);
				m_Sel.m_fRepeatTime			= EditorGUILayout.FloatField	(GetHelpContent("m_fRepeatTime")			, m_Sel.m_fRepeatTime);
			}

			m_Sel.m_nRepeatCount			= EditorGUILayout.IntField		(GetHelpContent("m_nRepeatCount")			, m_Sel.m_nRepeatCount);
			m_Sel.m_ParticlePrefab			= (GameObject)EditorGUILayout.ObjectField(GetHelpContent("m_ParticlePrefab"), m_Sel.m_ParticlePrefab, typeof(GameObject), false, null);
			// --------------------------------------------------------------
			EditorGUILayout.Space();
			rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(m_fButtonHeight));
			{
				if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(rect, 4, 0, 2), GetHelpContent("SelectParticlePrefab"), (m_FxmPopupManager != null)))
					m_FxmPopupManager.ShowSelectPrefabPopup(m_Sel, true, 0);
				if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(rect, 4, 2, 1), GetHelpContent("ClearPrefab"), (m_Sel.m_ParticlePrefab != null)))
				{
					bClickButton = true;
					m_Sel.m_ParticlePrefab = null;
				}
				if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(rect, 4, 3, 1), GetHelpContent("OpenPrefab"), (m_FxmPopupManager != null) && (m_Sel.m_ParticlePrefab != null)))
				{
					bClickButton = true;
					GetFXMakerMain().OpenPrefab(m_Sel.m_ParticlePrefab);
					return;
				}
				GUILayout.Label("");
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			// --------------------------------------------------------------

			if (m_Sel.m_AttachType == NcParticleEmit.AttachType.Destroy)
				SetMinValue(ref m_Sel.m_nRepeatCount, 1);
			FXMakerLayout.GUIEnableBackup(false);
			EditorGUILayout.Toggle		(GetHelpContent("m_bWorldSpace")			, true);
			FXMakerLayout.GUIEnableRestore();

			m_Sel.m_EmitCount				= EditorGUILayout.IntField		("m_EmitCount"								, m_Sel.m_EmitCount);
			m_Sel.m_RandomRange				= EditorGUILayout.Vector3Field	("m_RandomRange"							, m_Sel.m_RandomRange, null);
			m_Sel.m_AddStartPos				= EditorGUILayout.Vector3Field	("m_AddStartPos"							, m_Sel.m_AddStartPos, null);

			// check
			SetMinValue(ref m_Sel.m_EmitCount, 1);
			SetMinValue(ref m_Sel.m_fDelayTime, 0);
			SetMinValue(ref m_Sel.m_fRepeatTime, 0);
			SetMinValue(ref m_Sel.m_nRepeatCount, 0);

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
		string text		= FXMakerTooltip.GetHsEditor_NcParticleEmit(tooltip);
		return GetHelpContent(caption, text);
	}

	protected override void HelpBox(string caption)
	{
		string	str	= caption;
		if (caption == "" || caption == "Script")
			str = FXMakerTooltip.GetHsEditor_NcParticleEmit("");
		base.HelpBox(str);
	}
}
