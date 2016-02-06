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

[CustomEditor(typeof(NcAutoDeactive))]

public class NcAutoDeactiveEditor : FXMakerEditor
{
	// Attribute ------------------------------------------------------------------------
	protected	NcAutoDeactive	m_Sel;

	// Property -------------------------------------------------------------------------
	// Event Function -------------------------------------------------------------------
    void OnEnable()
    {
 		m_Sel = target as NcAutoDeactive;
 		m_UndoManager	= new FXMakerUndoManager(m_Sel, "NcAutoDeactive");
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

			m_Sel.m_fLifeTime						= EditorGUILayout.FloatField(GetHelpContent("m_fLifeTime")					, m_Sel.m_fLifeTime);
// 			m_Sel.m_bEndNcCurveAnimation			= EditorGUILayout.Toggle	(GetHelpContent("m_bEndNcCurveAnimation")		, m_Sel.m_bEndNcCurveAnimation);
			m_Sel.m_fSmoothDestroyTime				= EditorGUILayout.FloatField(GetHelpContent("m_fSmoothDestroyTime")			, m_Sel.m_fSmoothDestroyTime);
			if (0 < m_Sel.m_fSmoothDestroyTime)
			{
				m_Sel.m_bDisableEmit				= EditorGUILayout.Toggle	(GetHelpContent("m_bDisableEmit")				, m_Sel.m_bDisableEmit);
				m_Sel.m_bSmoothHide					= EditorGUILayout.Toggle	(GetHelpContent("m_bSmoothHide")				, m_Sel.m_bSmoothHide);
				m_Sel.m_bMeshFilterOnlySmoothHide	= EditorGUILayout.Toggle	(GetHelpContent("m_bMeshFilterOnlySmoothHide")	, m_Sel.m_bMeshFilterOnlySmoothHide);
			}

			// Collision
			m_Sel.m_CollisionType			= (NcAutoDeactive.CollisionType)EditorGUILayout.EnumPopup(GetHelpContent("m_CollisionType"), m_Sel.m_CollisionType, GUILayout.MaxWidth(Screen.width));
			if (m_Sel.m_CollisionType != NcAutoDeactive.CollisionType.NONE)
			{
				if (m_Sel.m_CollisionType == NcAutoDeactive.CollisionType.COLLISION)
				{
					m_Sel.m_CollisionLayer		= LayerMaskField			(GetHelpContent("m_CollisionLayer")				, m_Sel.m_CollisionLayer);
					m_Sel.m_fCollisionRadius	= EditorGUILayout.FloatField(GetHelpContent("m_fCollisionRadius")			, m_Sel.m_fCollisionRadius);

					SetMinValue(ref m_Sel.m_fCollisionRadius, 0.01f);
				}
				if (m_Sel.m_CollisionType == NcAutoDeactive.CollisionType.WORLD_Y)
				{
					m_Sel.m_fDestructPosY		= EditorGUILayout.FloatField	(GetHelpContent("m_fDestructPosY")			, m_Sel.m_fDestructPosY);
				}
			}

			SetMinValue(ref m_Sel.m_fLifeTime, 0);
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
		string text		= FXMakerTooltip.GetHsEditor_NcAutoDeactive(tooltip);
		return GetHelpContent(caption, text);
	}

	protected override void HelpBox(string caption)
	{
		string	str	= caption;
		if (caption == "" || caption == "Script")
			str = FXMakerTooltip.GetHsEditor_NcAutoDeactive("");
		base.HelpBox(str);
	}
}
