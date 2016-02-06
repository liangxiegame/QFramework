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

[CustomEditor(typeof(NcParticleSpiral))]

public class NcParticleSpiralEditor : FXMakerEditor
{
	// Attribute ------------------------------------------------------------------------
	protected	NcParticleSpiral	m_Sel;
	protected	FxmPopupManager		m_FxmPopupManager;

	// Property -------------------------------------------------------------------------
	// Event Function -------------------------------------------------------------------
    void OnEnable()
    {
 		m_Sel = target as NcParticleSpiral;
 		m_UndoManager	= new FXMakerUndoManager(m_Sel, "NcParticleSpiral");
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

		Rect	rect;

		m_FxmPopupManager = GetFxmPopupManager();

		// --------------------------------------------------------------
		bool bClickButton = false;
		EditorGUI.BeginChangeCheck();
		{
//			DrawDefaultInspector();
			m_Sel.m_fUserTag = EditorGUILayout.FloatField(GetCommonContent("m_fUserTag"), m_Sel.m_fUserTag);

			m_Sel.m_ParticlePrefab	= (GameObject)EditorGUILayout.ObjectField(GetHelpContent("m_ParticlePrefab"), m_Sel.m_ParticlePrefab, typeof(GameObject), false, null);
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

			m_Sel.m_fDelayTime				= EditorGUILayout.FloatField	(GetHelpContent("m_fDelayTime")				, m_Sel.m_fDelayTime);
			m_Sel.m_nNumberOfArms			= EditorGUILayout.IntField		(GetHelpContent("m_nNumberOfArms")			, m_Sel.m_nNumberOfArms);
			m_Sel.m_nParticlesPerArm		= EditorGUILayout.IntField		(GetHelpContent("m_nParticlesPerArm")		, m_Sel.m_nParticlesPerArm);
			m_Sel.m_fParticleSeparation		= EditorGUILayout.FloatField	(GetHelpContent("m_fParticleSeparation")	, m_Sel.m_fParticleSeparation);
			m_Sel.m_fTurnDistance			= EditorGUILayout.FloatField	(GetHelpContent("m_fTurnDistance")			, m_Sel.m_fTurnDistance);
			m_Sel.m_fVerticalTurnDistance	= EditorGUILayout.FloatField	(GetHelpContent("m_fVerticalTurnDistance")	, m_Sel.m_fVerticalTurnDistance);
			m_Sel.m_fOriginOffset			= EditorGUILayout.FloatField	(GetHelpContent("m_fOriginOffset")			, m_Sel.m_fOriginOffset);
			m_Sel.m_fTurnSpeed				= EditorGUILayout.FloatField	(GetHelpContent("m_fTurnSpeed")				, m_Sel.m_fTurnSpeed);
			m_Sel.m_fFadeValue				= EditorGUILayout.FloatField	(GetHelpContent("m_fFadeValue")				, m_Sel.m_fFadeValue);
			m_Sel.m_fSizeValue				= EditorGUILayout.FloatField	(GetHelpContent("m_fSizeValue")				, m_Sel.m_fSizeValue);
			m_Sel.m_nNumberOfSpawns			= EditorGUILayout.IntField		(GetHelpContent("m_nNumberOfSpawns")		, m_Sel.m_nNumberOfSpawns);
			m_Sel.m_fSpawnRate				= EditorGUILayout.FloatField	(GetHelpContent("m_fSpawnRate")				, m_Sel.m_fSpawnRate);

			Rect butRect = EditorGUILayout.BeginHorizontal(GUILayout.Height(m_fButtonHeight*2));
			{
				if (FXMakerLayout.GUIButton(butRect, GetHelpContent("Randomize"), true))
				{
					m_Sel.RandomizeEditor();
					bClickButton = true;
				}
				GUILayout.Label("");
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();

			SetMinValue(ref m_Sel.m_fDelayTime, 0);
			SetMinValue(ref m_Sel.m_nNumberOfArms, 1);
			SetMinValue(ref m_Sel.m_nParticlesPerArm, 1);
			SetMinValue(ref m_Sel.m_fFadeValue, -1);
			SetMaxValue(ref m_Sel.m_fFadeValue, 1);
			SetMinValue(ref m_Sel.m_nNumberOfSpawns, 1);
			SetMinValue(ref m_Sel.m_fSpawnRate, 0.1f);
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
		string text		= FXMakerTooltip.GetHsEditor_NcParticleSpiral(tooltip);
		return GetHelpContent(caption, text);
	}

	protected override void HelpBox(string caption)
	{
		string	str	= caption;
		if (caption == "" || caption == "Script")
			str = FXMakerTooltip.GetHsEditor_NcParticleSpiral("");
		base.HelpBox(str);
	}
}
