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

[CustomEditor(typeof(NcTrailTexture))]

public class NcTrailTextureEditor : FXMakerEditor
{
	// Attribute ------------------------------------------------------------------------
	protected	NcTrailTexture		m_Sel;
	protected	FxmPopupManager		m_FxmPopupManager;

	// Property -------------------------------------------------------------------------
	// Event Function -------------------------------------------------------------------
    void OnEnable()
    {
 		m_Sel = target as NcTrailTexture;
 		m_UndoManager	= new FXMakerUndoManager(m_Sel, "NcTrailTexture");
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

		m_FxmPopupManager = GetFxmPopupManager();

		// --------------------------------------------------------------
		bool bClickButton = false;
		EditorGUI.BeginChangeCheck();
		{
//			DrawDefaultInspector();
			m_Sel.m_fUserTag = EditorGUILayout.FloatField(GetCommonContent("m_fUserTag"), m_Sel.m_fUserTag);

			m_Sel.m_fDelayTime				= EditorGUILayout.FloatField	(GetHelpContent("m_fDelayTime")				, m_Sel.m_fDelayTime);
			m_Sel.m_fEmitTime				= EditorGUILayout.FloatField	(GetHelpContent("m_fEmitTime")				, m_Sel.m_fEmitTime);
			m_Sel.m_bSmoothHide				= EditorGUILayout.Toggle		(GetHelpContent("m_bSmoothHide")			, m_Sel.m_bSmoothHide);
			m_Sel.m_fLifeTime				= EditorGUILayout.FloatField	(GetHelpContent("m_fLifeTime")				, m_Sel.m_fLifeTime);
			m_Sel.m_TipAxis					= (NcTrailTexture.AXIS_TYPE)EditorGUILayout.EnumPopup(GetHelpContent("m_TipAxis"), m_Sel.m_TipAxis, GUILayout.MaxWidth(Screen.width));
			m_Sel.m_fTipSize				= EditorGUILayout.FloatField	(GetHelpContent("m_fTipSize")				, m_Sel.m_fTipSize);
			m_Sel.m_bCenterAlign			= EditorGUILayout.Toggle		(GetHelpContent("m_bCenterAlign")			, m_Sel.m_bCenterAlign);
			m_Sel.m_UvFlipHorizontal		= EditorGUILayout.Toggle		(GetHelpContent("m_UvFlipHorizontal")		, m_Sel.m_UvFlipHorizontal);
			m_Sel.m_UvFlipVirtical			= EditorGUILayout.Toggle		(GetHelpContent("m_UvFlipVirtical")			, m_Sel.m_UvFlipVirtical);

			m_Sel.m_nFadeHeadCount			= EditorGUILayout.IntField		(GetHelpContent("m_nFadeHeadCount")			, m_Sel.m_nFadeHeadCount);
			m_Sel.m_nFadeTailCount			= EditorGUILayout.IntField		(GetHelpContent("m_nFadeTailCount")			, m_Sel.m_nFadeTailCount);

			int nColorCount	= EditorGUILayout.IntField(GetHelpContent("ColorCount")		, m_Sel.m_Colors.Length);
			if (nColorCount != m_Sel.m_Colors.Length)
				m_Sel.m_Colors = NgConvert.ResizeArray<Color>(m_Sel.m_Colors, nColorCount, Color.white);
			for (int n = 0; n < m_Sel.m_Colors.Length; n++)
				m_Sel.m_Colors[n]	= EditorGUILayout.ColorField(GetHelpContent("    Color " + n)	, m_Sel.m_Colors[n]);

			int nSizeCount	= EditorGUILayout.IntField(GetHelpContent("SizeRateCount")	, m_Sel.m_SizeRates.Length);
			if (nSizeCount != m_Sel.m_SizeRates.Length)
				m_Sel.m_SizeRates = NgConvert.ResizeArray<float>(m_Sel.m_SizeRates, nSizeCount, 1);
			for (int n = 0; n < m_Sel.m_SizeRates.Length; n++)
			{
				m_Sel.m_SizeRates[n]	= EditorGUILayout.FloatField(GetHelpContent("    SizeRate " + n)	, m_Sel.m_SizeRates[n]);
				SetMinValue(ref m_Sel.m_SizeRates[n], 0);
			}

			m_Sel.m_bInterpolation			= EditorGUILayout.Toggle		(GetHelpContent("m_bInterpolation")			, m_Sel.m_bInterpolation);
			if (m_Sel.m_bInterpolation)
			{
				m_Sel.m_nMaxSmoothCount		= EditorGUILayout.IntField		(GetHelpContent("    nMaxSmoothCount")		, m_Sel.m_nMaxSmoothCount);
				m_Sel.m_nSubdivisions		= EditorGUILayout.IntField		(GetHelpContent("    nSubdivisions")		, m_Sel.m_nSubdivisions);
			}

			m_Sel.m_fMinVertexDistance		= EditorGUILayout.FloatField	(GetHelpContent("m_fMinVertexDistance")		, m_Sel.m_fMinVertexDistance);
			m_Sel.m_fMaxVertexDistance		= EditorGUILayout.FloatField	(GetHelpContent("m_fMaxVertexDistance")		, m_Sel.m_fMaxVertexDistance);
			m_Sel.m_fMaxAngle				= EditorGUILayout.FloatField	(GetHelpContent("m_fMaxAngle")				, m_Sel.m_fMaxAngle);

			m_Sel.m_bAutoDestruct			= EditorGUILayout.Toggle		(GetHelpContent("m_bAutoDestruct")			, m_Sel.m_bAutoDestruct);

			SetMinValue(ref m_Sel.m_fDelayTime, 0);
			SetMinValue(ref m_Sel.m_fEmitTime, 0);
			SetMinValue(ref m_Sel.m_fLifeTime, 0.01f);
			SetMinValue(ref m_Sel.m_fTipSize, 0.01f);
			SetMinValue(ref m_Sel.m_fMinVertexDistance, 0.01f);
			SetMinValue(ref m_Sel.m_fMaxVertexDistance, 0.02f);
			SetMinValue(ref m_Sel.m_fMaxAngle, 0);

			SetMinValue(ref m_Sel.m_nMaxSmoothCount, 3);
			SetMinValue(ref m_Sel.m_nSubdivisions, 2);
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
		string text		= FXMakerTooltip.GetHsEditor_NcTrailTexture(tooltip);
		return GetHelpContent(caption, text);
	}

	protected override void HelpBox(string caption)
	{
		string	str	= caption;
		if (caption == "" || caption == "Script")
			str = FXMakerTooltip.GetHsEditor_NcTrailTexture("");
		base.HelpBox(str);
	}
}
