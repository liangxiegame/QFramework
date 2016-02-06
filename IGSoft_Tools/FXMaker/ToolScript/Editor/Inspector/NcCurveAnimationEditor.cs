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

[CustomEditor(typeof(NcCurveAnimation))]

public class NcCurveAnimationEditor : FXMakerEditor
{
	// Attribute ------------------------------------------------------------------------
	protected	NcCurveAnimation	m_Sel;
	protected	FxmPopupManager	m_FxmPopupManager;

	// Property -------------------------------------------------------------------------
	// Event Function -------------------------------------------------------------------
    void OnEnable()
    {
 		m_Sel = target as NcCurveAnimation;
 		m_UndoManager	= new FXMakerUndoManager(m_Sel, "NcCurveAnimation");
   }

    void OnDisable()
    {
		if (m_FxmPopupManager != null && m_FxmPopupManager.IsShowByInspector())
			m_FxmPopupManager.CloseNcCurvePopup();
    }

	public override void OnInspectorGUI()
	{
		AddScriptNameField(m_Sel);

		Rect			rect;
		int				nLeftWidth		= 115;
		int				nAddHeight		= 22;
		int				nDelHeight		= 17;
		int				nLineHeight		= 19;
		int				nCurveHeight	= 50;
		List<NcCurveAnimation.NcInfoCurve>	curveInfoList = m_Sel.m_CurveInfoList;

		m_FxmPopupManager = GetFxmPopupManager();

//		test code
// 		if (GUILayout.Button("Pause"))
// 			FxmInfoIndexing.FindInstanceIndexing(m_Sel.transform, false).GetComponent<NcCurveAnimation>().PauseAnimation();
// 		if (GUILayout.Button("Resume"))
// 			FxmInfoIndexing.FindInstanceIndexing(m_Sel.transform, false).GetComponent<NcCurveAnimation>().ResumeAnimation();

		// --------------------------------------------------------------
		bool bClickButton = false;
		EditorGUI.BeginChangeCheck();
		{
			m_UndoManager.CheckUndo();
			// --------------------------------------------------------------
			m_Sel.m_fUserTag = EditorGUILayout.FloatField(GetCommonContent("m_fUserTag"), m_Sel.m_fUserTag);

			EditorGUILayout.Space();
			m_Sel.m_fDelayTime		= EditorGUILayout.FloatField(GetHelpContent("m_fDelayTime")			, m_Sel.m_fDelayTime);
			m_Sel.m_fDurationTime	= EditorGUILayout.FloatField(GetHelpContent("m_fDurationTime")		, m_Sel.m_fDurationTime);
 			m_Sel.m_bAutoDestruct	= EditorGUILayout.Toggle	(GetHelpContent("m_bAutoDestruct")		, m_Sel.m_bAutoDestruct);

			// check
			SetMinValue(ref m_Sel.m_fDelayTime, 0);
			SetMinValue(ref m_Sel.m_fDurationTime, 0.01f);

			// --------------------------------------------------------------
			EditorGUILayout.Space();
			rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(nAddHeight*3));
			{
				if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerVerticalRect(rect, 3, 0, 1), GetHelpContent("Clear All"), (0 <m_Sel.GetCurveInfoCount())))
				{
					bClickButton	= true;
					m_Sel.ClearAllCurveInfo();
				}
				if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(FXMakerLayout.GetInnerVerticalRect(rect, 3, 1, 1), 2, 0, 1), GetHelpContent("Load Curves"), (m_FxmPopupManager != null)))
					m_FxmPopupManager.ShowNcCurveAnimationPopup(m_Sel, false);
				if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(FXMakerLayout.GetInnerVerticalRect(rect, 3, 1, 1), 2, 1, 1), GetHelpContent("Save Curves"), (m_FxmPopupManager != null && 0 < m_Sel.GetCurveInfoCount())))
					m_FxmPopupManager.ShowNcCurveAnimationPopup(m_Sel, true);
				if (GUI.Button(FXMakerLayout.GetInnerVerticalRect(rect, 3, 2, 1), GetHelpContent("Add EmptyCurve")))
				{
					bClickButton	= true;
					m_Sel.AddCurveInfo();
				}
 				GUILayout.Label("");
			}
			EditorGUILayout.EndHorizontal();
			m_UndoManager.CheckDirty();

			// --------------------------------------------------------------
			for (int n = 0; n < (curveInfoList != null ? curveInfoList.Count : 0); n++)
			{
				EditorGUILayout.Space();

				// Enabled --------------------------------------------------------------
				rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(nDelHeight));
				{
					GUI.Box(rect, "");
					curveInfoList[n].m_bEnabled = GUILayout.Toggle(curveInfoList[n].m_bEnabled, "CurveInfo " + n.ToString(), GUILayout.Width(nLeftWidth));
//					GUILayout.Label("CurveInfo Index " + n.ToString(), GUILayout.Width(nLeftWidth));
				}
				EditorGUILayout.EndHorizontal();

				// CurveName -----------------------------------------------------------
				curveInfoList[n].m_CurveName = EditorGUILayout.TextField(GetHelpContent("m_CurveName"), curveInfoList[n].m_CurveName);

				// ApplyType --------------------------------------------------------------
				EditorGUI.BeginChangeCheck();
				{
					rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(nLineHeight));
					{
						GUI.Box(rect, "");
						GUILayout.Label("", GUILayout.Width(nLeftWidth));
						NcCurveAnimation.NcInfoCurve.APPLY_TYPE nApplyType	= (NcCurveAnimation.NcInfoCurve.APPLY_TYPE)EditorGUI.Popup(new Rect(rect.x, rect.y, nLeftWidth, rect.height), (int)curveInfoList[n].m_ApplyType, NcCurveAnimation.NcInfoCurve.m_TypeName);
						if (curveInfoList[n].m_ApplyType != nApplyType)
						{
							curveInfoList[n].m_ApplyType = nApplyType;
							curveInfoList[n].SetDefaultValueScale();
						}

						// Add Component
						bool bShowOption = true;
						if (curveInfoList[n].m_ApplyType == NcCurveAnimation.NcInfoCurve.APPLY_TYPE.TEXTUREUV)
						{
							if (m_Sel.gameObject.GetComponent<NcUvAnimation>() == null)
							{
								bShowOption = false;
								FXMakerLayout.GUIColorBackup(FXMakerLayout.m_ColorHelpBox);
								if (GUI.Button(new Rect(rect.x+nLeftWidth, rect.y, rect.width-nLeftWidth, rect.height), GetHelpContent("Add NcUvAnimation Script")))
									m_Sel.gameObject.AddComponent<NcUvAnimation>();
								FXMakerLayout.GUIColorRestore();
							}
						}
						if (bShowOption)
							for (int nValueIndex = 0; nValueIndex < curveInfoList[n].GetValueCount(); nValueIndex++)
								curveInfoList[n].m_bApplyOption[nValueIndex] = GUILayout.Toggle(curveInfoList[n].m_bApplyOption[nValueIndex], curveInfoList[n].GetValueName(nValueIndex));
						if (curveInfoList[n].m_ApplyType == NcCurveAnimation.NcInfoCurve.APPLY_TYPE.SCALE)
								GUILayout.Label("LocalSpace");
					}
					EditorGUILayout.EndHorizontal();
				}
				if (EditorGUI.EndChangeCheck())
					m_Sel.CheckInvalidOption(n);

				if (curveInfoList[n].m_ApplyType == NcCurveAnimation.NcInfoCurve.APPLY_TYPE.MATERIAL_COLOR)
				{
					// ValueScale --------------------------------------------------------------
					rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(nLineHeight * 2));
					{
						GUI.Box(rect, "");
						GUILayout.Label("", GUILayout.Width(nLeftWidth));
						bool bEnableColor	= (m_Sel.GetComponent<Renderer>() != null && m_Sel.GetComponent<Renderer>().sharedMaterial != null & NgMaterial.IsMaterialColor(m_Sel.GetComponent<Renderer>().sharedMaterial));
						Rect colorRect		= FXMakerLayout.GetInnerVerticalRect(rect, 2, 0, 1);
						colorRect.width		= nLeftWidth;
						if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(colorRect, 2, 0, 1), GetHelpContent("White"), bEnableColor))
							curveInfoList[n].m_ToColor = Color.white;
						if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(colorRect, 2, 1, 1), GetHelpContent("Current"), bEnableColor))
							curveInfoList[n].m_ToColor = NgMaterial.GetMaterialColor(m_Sel.GetComponent<Renderer>().sharedMaterial);
						colorRect.x += colorRect.width;
						GUI.Label(colorRect, GetHelpContent("ToColor"));
						colorRect.x += 60;
						colorRect.width = rect.width - colorRect.x;
						curveInfoList[n].m_ToColor	= EditorGUI.ColorField(colorRect, curveInfoList[n].m_ToColor);

						// m_bRecursively
						Rect recRect = FXMakerLayout.GetInnerVerticalRect(rect, 2, 1, 1);
						curveInfoList[n].m_bRecursively = GUI.Toggle(FXMakerLayout.GetRightRect(recRect, rect.width-nLeftWidth), curveInfoList[n].m_bRecursively, GetHelpContent("Recursively"));
					}
					EditorGUILayout.EndHorizontal();
				} else
				if (curveInfoList[n].m_ApplyType == NcCurveAnimation.NcInfoCurve.APPLY_TYPE.MESH_COLOR)
				{
					// ValueScale --------------------------------------------------------------
					rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(nLineHeight * 3));
					{
						GUI.Box(rect, "");
						GUILayout.Label("", GUILayout.Width(nLeftWidth));
//						bool bEnableColor	= (m_Sel.renderer != null && m_Sel.renderer.sharedMaterial != null & NgMaterial.IsMaterialColor(m_Sel.renderer.sharedMaterial));
						// From Color
						Rect colorRect		= FXMakerLayout.GetInnerVerticalRect(rect, 3, 0, 1);
						colorRect.width		= nLeftWidth;
						if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(colorRect, 2, 0, 1), GetHelpContent("White"), true))
							curveInfoList[n].m_FromColor = Color.white;
						if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(colorRect, 2, 1, 1), GetHelpContent("Black"), true))
							curveInfoList[n].m_FromColor = Color.black;
						colorRect.x += colorRect.width;
						GUI.Label(colorRect, GetHelpContent("FromColor"));
						colorRect.x += 60;
						colorRect.width = rect.width - colorRect.x;
						curveInfoList[n].m_FromColor	= EditorGUI.ColorField(colorRect, curveInfoList[n].m_FromColor);

						// To Color
						colorRect			= FXMakerLayout.GetInnerVerticalRect(rect, 3, 1, 1);
						colorRect.width		= nLeftWidth;
						if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(colorRect, 2, 0, 1), GetHelpContent("White"), true))
							curveInfoList[n].m_ToColor = Color.white;
						if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(colorRect, 2, 1, 1), GetHelpContent("Black"), true))
							curveInfoList[n].m_ToColor = Color.black;
						colorRect.x += colorRect.width;
						GUI.Label(colorRect, GetHelpContent("ToColor"));
						colorRect.x += 60;
						colorRect.width = rect.width - colorRect.x;
						curveInfoList[n].m_ToColor	= EditorGUI.ColorField(colorRect, curveInfoList[n].m_ToColor);

						// m_bRecursively
						Rect recRect = FXMakerLayout.GetInnerVerticalRect(rect, 3, 2, 1);
						curveInfoList[n].m_bRecursively = GUI.Toggle(FXMakerLayout.GetRightRect(recRect, rect.width-nLeftWidth), curveInfoList[n].m_bRecursively, GetHelpContent("Recursively"));
					}
					EditorGUILayout.EndHorizontal();
				} else {
					// ValueScale --------------------------------------------------------------
					rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(nLineHeight));
					{
						GUI.Box(rect, "");
						GUILayout.Label("", GUILayout.Width(nLeftWidth));
						if (curveInfoList[n].m_ApplyType == NcCurveAnimation.NcInfoCurve.APPLY_TYPE.SCALE)
							 curveInfoList[n].m_fValueScale = EditorGUI.FloatField(rect, GetHelpContent("Value Scale"), curveInfoList[n].m_fValueScale+1)-1;
						else curveInfoList[n].m_fValueScale = EditorGUI.FloatField(rect, GetHelpContent("Value Scale"), curveInfoList[n].m_fValueScale);
					}
					EditorGUILayout.EndHorizontal();
				}

				// Curve --------------------------------------------------------------
				rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(nCurveHeight));
				{
					GUI.Box(rect, "");
					GUILayout.Label("", GUILayout.Width(nLeftWidth));
					EditorGUI.BeginChangeCheck();
 					{
						curveInfoList[n].m_AniCurve	= EditorGUI.CurveField(FXMakerLayout.GetOffsetRect(rect, nLeftWidth+4, 0, 0, -4), curveInfoList[n].m_AniCurve, Color.green, curveInfoList[n].GetEditRange());
//						curveInfoList[n].m_AniCurve	= EditorGUILayout.CurveField(" ", curveInfoList[n].m_AniCurve, Color.green, curveInfoList[n].GetEditRange(), GUILayout.Height(nCurveHeight-4));
//						curveInfoList[n].m_AniCurve	= EditorGUILayout.CurveField(" ", curveInfoList[n].m_AniCurve, GUILayout.Height(nCurveHeight-4));
					}
					if (EditorGUI.EndChangeCheck())
						curveInfoList[n].NormalizeCurveTime();

					if (m_FxmPopupManager != null)
					{
						Rect buttonRect = rect;
						buttonRect.width = nLeftWidth;
						FXMakerLayout.GetOffsetRect(rect, 0, 5, 0, -5);
						if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(FXMakerLayout.GetInnerVerticalRect(buttonRect, 2, 0, 1), 2, 0, 1), GetHelpContent("Change")))
							m_FxmPopupManager.ShowNcInfoCurvePopup(m_Sel, n, false);
						if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(FXMakerLayout.GetInnerVerticalRect(buttonRect, 2, 0, 1), 2, 1, 1), GetHelpContent("Save")))
							m_FxmPopupManager.ShowNcInfoCurvePopup(m_Sel, n, true);
						if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(FXMakerLayout.GetInnerVerticalRect(buttonRect, 2, 1, 1), 2, 0, 2), GetHelpContent("Delete")))
						{
							bClickButton	= true;
							m_Sel.DeleteCurveInfo(n);
						}
					}
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();
			}
		}
		// --------------------------------------------------------------
		if ((EditorGUI.EndChangeCheck() || bClickButton) && GetFXMakerMain())
			OnEditComponent();
		// ---------------------------------------------------------------------
		if (GUI.tooltip != "")
			m_LastTooltip	= GUI.tooltip;
		HelpBox(m_LastTooltip);
	}

	// ----------------------------------------------------------------------------------
	Rect GetCurveRect(int line)
	{
		int		nLineWidth	= 100;
		int		nLineHeight	= 100;

		return new Rect(0, line * nLineHeight, nLineWidth, nLineHeight);
	}

	protected GUIContent GetHelpContent(string tooltip)
	{
		string caption	= tooltip;
		string text		= FXMakerTooltip.GetHsEditor_NcCurveAnimation(tooltip);
		return GetHelpContent(caption, text);
	}

	protected override void HelpBox(string caption)
	{
		string	str	= caption;
		if (caption == "" || caption == "Script")
			str = FXMakerTooltip.GetHsEditor_NcCurveAnimation("");
		base.HelpBox(str);
	}
}
