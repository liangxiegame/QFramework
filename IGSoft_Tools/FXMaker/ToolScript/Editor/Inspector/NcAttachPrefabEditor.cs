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

[CustomEditor(typeof(NcAttachPrefab))]

public class NcAttachPrefabEditor : FXMakerEditor
{
	// Attribute ------------------------------------------------------------------------
	protected	NcAttachPrefab		m_Sel;
	protected	FxmPopupManager	m_FxmPopupManager;

	// Property -------------------------------------------------------------------------
	// Event Function -------------------------------------------------------------------
    void OnEnable()
    {
 		m_Sel = target as NcAttachPrefab;
 		m_UndoManager	= new FXMakerUndoManager(m_Sel, "NcAttachPrefab");
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

			m_Sel.m_AttachType				= (NcAttachPrefab.AttachType)EditorGUILayout.EnumPopup(GetHelpContent("m_AttachType")	, m_Sel.m_AttachType, GUILayout.MaxWidth(Screen.width));

			if (m_Sel.m_AttachType == NcAttachPrefab.AttachType.Active)
			{
				m_Sel.m_fDelayTime			= EditorGUILayout.FloatField	(GetHelpContent("m_fDelayTime")				, m_Sel.m_fDelayTime);
				m_Sel.m_fRepeatTime			= EditorGUILayout.FloatField	(GetHelpContent("m_fRepeatTime")			, m_Sel.m_fRepeatTime);
			}

			m_Sel.m_nRepeatCount			= EditorGUILayout.IntField		(GetHelpContent("m_nRepeatCount")			, m_Sel.m_nRepeatCount);
			m_Sel.m_AttachPrefab			= (GameObject)EditorGUILayout.ObjectField(GetHelpContent("m_AttachPrefab")	, m_Sel.m_AttachPrefab, typeof(GameObject), false, null);
			m_Sel.m_fPrefabSpeed			= EditorGUILayout.FloatField	(GetHelpContent("m_fPrefabSpeed")			, m_Sel.m_fPrefabSpeed);
			m_Sel.m_fPrefabLifeTime			= EditorGUILayout.FloatField	(GetHelpContent("m_fPrefabLifeTime")		, m_Sel.m_fPrefabLifeTime);

			if (m_Sel.m_AttachType == NcAttachPrefab.AttachType.Destroy)
			{
				SetMinValue(ref m_Sel.m_nRepeatCount, 1);

				FXMakerLayout.GUIEnableBackup(false);
				EditorGUILayout.Toggle		(GetHelpContent("m_bWorldSpace")			, true);
				FXMakerLayout.GUIEnableRestore();
			}

			if (m_Sel.m_AttachType == NcAttachPrefab.AttachType.Active)
				m_Sel.m_bWorldSpace			= EditorGUILayout.Toggle		(GetHelpContent("m_bWorldSpace")			, m_Sel.m_bWorldSpace);

			m_Sel.m_RandomRange				= EditorGUILayout.Vector3Field	("m_RandomRange"							, m_Sel.m_RandomRange, null);
			m_Sel.m_AddStartPos				= EditorGUILayout.Vector3Field	("m_AddStartPos"							, m_Sel.m_AddStartPos, null);
			m_Sel.m_AccumStartRot			= EditorGUILayout.Vector3Field	("m_AccumStartRot"							, m_Sel.m_AccumStartRot, null);

			// check
			SetMinValue(ref m_Sel.m_fDelayTime, 0);
			SetMinValue(ref m_Sel.m_fRepeatTime, 0);
			SetMinValue(ref m_Sel.m_nRepeatCount, 0);
			SetMinValue(ref m_Sel.m_fPrefabSpeed, 0.01f);
			SetMinValue(ref m_Sel.m_fPrefabLifeTime, 0);

			// --------------------------------------------------------------
			EditorGUILayout.Space();
			rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(m_fButtonHeight));
			{
				if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(rect, 3, 0, 1), GetHelpContent("Select Prefab"), (m_FxmPopupManager != null)))
					m_FxmPopupManager.ShowSelectPrefabPopup(m_Sel, true, 0);
				if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(rect, 3, 1, 1), GetHelpContent("Clear Prefab"), (m_Sel.m_AttachPrefab != null)))
				{
					bClickButton = true;
					m_Sel.m_AttachPrefab = null;
				}
				if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(rect, 3, 2, 1), GetHelpContent("Open Prefab"), (m_FxmPopupManager != null) && (m_Sel.m_AttachPrefab != null)))
				{
					bClickButton = true;
					GetFXMakerMain().OpenPrefab(m_Sel.m_AttachPrefab);
					return;
				}
				GUILayout.Label("");
			}
			EditorGUILayout.EndHorizontal();

			// m_nSpriteFactoryIndex
			EditorGUILayout.Space();
			if (m_Sel.m_AttachPrefab)
			{
				NcSpriteFactory ncSpriteFactory = m_Sel.m_AttachPrefab.GetComponent<NcSpriteFactory>();
				if (ncSpriteFactory)
				{
					m_Sel.m_nSpriteFactoryIndex = EditorGUILayout.IntSlider(GetHelpContent("m_nSpriteFactoryIndex"), m_Sel.m_nSpriteFactoryIndex, -1, ncSpriteFactory.GetSpriteNodeCount()-1);

					string[]	spriteNames = new string[ncSpriteFactory.GetSpriteNodeCount()+1];
					spriteNames[0] = "Default";
					for (int n = 0; n < ncSpriteFactory.GetSpriteNodeCount(); n++)
						spriteNames[n+1] = ncSpriteFactory.GetSpriteNode(n).m_SpriteName;
					m_Sel.m_nSpriteFactoryIndex = EditorGUILayout.Popup("SpriteName", m_Sel.m_nSpriteFactoryIndex+1, spriteNames)-1;
				} else m_Sel.m_nSpriteFactoryIndex = -1;
			} else m_Sel.m_nSpriteFactoryIndex = -1;
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
		string text		= FXMakerTooltip.GetHsEditor_NcAttachPrefab(tooltip);
		return GetHelpContent(caption, text);
	}

	protected override void HelpBox(string caption)
	{
		string	str	= caption;
		if (caption == "" || caption == "Script")
			str = FXMakerTooltip.GetHsEditor_NcAttachPrefab("");
		base.HelpBox(str);
	}
}
