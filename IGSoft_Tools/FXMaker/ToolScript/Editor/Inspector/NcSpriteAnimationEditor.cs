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

[CustomEditor(typeof(NcSpriteAnimation))]

public class NcSpriteAnimationEditor : FXMakerEditor
{
	// Attribute ------------------------------------------------------------------------
	protected	NcSpriteAnimation		m_Sel;
	protected	FxmPopupManager			m_FxmPopupManager;

	// Property -------------------------------------------------------------------------
	// Event Function -------------------------------------------------------------------
    void OnEnable()
    {
 		m_Sel = target as NcSpriteAnimation;
 		m_UndoManager	= new FXMakerUndoManager(m_Sel, "NcSpriteAnimation");
   }

    void OnDisable()
    {
		if (m_FxmPopupManager != null && m_FxmPopupManager.IsShowByInspector())
			m_FxmPopupManager.CloseNcPrefabPopup();
    }

	public override void OnInspectorGUI()
	{
		Rect rect;
		AddScriptNameField(m_Sel);
		m_UndoManager.CheckUndo();
		m_FxmPopupManager = GetFxmPopupManager();

//		test code
// 		if (GUILayout.Button("Pause"))
// 			FxmInfoIndexing.FindInstanceIndexing(m_Sel.transform, false).GetComponent<NcSpriteAnimation>().PauseAnimation();
// 		if (GUILayout.Button("Resume"))
// 			FxmInfoIndexing.FindInstanceIndexing(m_Sel.transform, false).GetComponent<NcSpriteAnimation>().ResumeAnimation();

		// --------------------------------------------------------------
		bool bClickButton = false;
		EditorGUI.BeginChangeCheck();
		{
//			DrawDefaultInspector();
			m_Sel.m_fUserTag = EditorGUILayout.FloatField(GetCommonContent("m_fUserTag"), m_Sel.m_fUserTag);

			m_Sel.m_TextureType		= (NcSpriteAnimation.TEXTURE_TYPE)EditorGUILayout.EnumPopup(GetHelpContent("m_TextureType"), m_Sel.m_TextureType);

			EditorGUILayout.Space();

			if (m_Sel.m_TextureType == NcSpriteAnimation.TEXTURE_TYPE.TileTexture)
			{
				if (GUILayout.Button(GetHelpContent("ConvertTo : TrimTexture")))
				{
					m_Sel.m_NcSpriteFrameInfos = NgAtlas.TileToTrimTexture(m_Sel.GetComponent<Renderer>().sharedMaterial, m_Sel.m_nTilingX, m_Sel.m_nTilingY, 0, m_Sel.m_nFrameCount, 4096);
					if (m_Sel.m_NcSpriteFrameInfos != null)
						m_Sel.m_TextureType = NcSpriteAnimation.TEXTURE_TYPE.TrimTexture;
				}
				if (GUILayout.Button(GetHelpContent("ExportTo : SplitTexture")))
				{
					string path = FXMakerCapture.GetExportSlitDir();
					path = NgAtlas.ExportSplitTexture(path, m_Sel.GetComponent<Renderer>().sharedMaterial.mainTexture, m_Sel.m_nTilingX, m_Sel.m_nTilingY, 0, m_Sel.m_nFrameCount);
					if (path != "")
					{
						Debug.Log(path);
						EditorUtility.OpenWithDefaultApp(path);
					}
				}
			} else
			if (m_Sel.m_TextureType == NcSpriteAnimation.TEXTURE_TYPE.TrimTexture)
			{
				if (GUILayout.Button(GetHelpContent("ExportTo : SplitTexture")))
				{
					string path = FXMakerCapture.GetExportSlitDir();
					path = NgAtlas.ExportSplitTexture(path, m_Sel.GetComponent<Renderer>().sharedMaterial.mainTexture, m_Sel.m_NcSpriteFrameInfos);
					if (path != "")
					{
						Debug.Log(path);
						EditorUtility.OpenWithDefaultApp(path);
					}
				}
			} else
			if (m_Sel.m_TextureType == NcSpriteAnimation.TEXTURE_TYPE.SpriteFactory)
			{
			}
			EditorGUILayout.Space();

			if (m_Sel.m_TextureType == NcSpriteAnimation.TEXTURE_TYPE.TileTexture)
			{
				m_Sel.m_nTilingX	= EditorGUILayout.IntField	(GetHelpContent("m_nTilingX")		, m_Sel.m_nTilingX);
				m_Sel.m_nTilingY	= EditorGUILayout.IntField	(GetHelpContent("m_nTilingY")		, m_Sel.m_nTilingY);
			} else
			if (m_Sel.m_TextureType == NcSpriteAnimation.TEXTURE_TYPE.TrimTexture)
			{
				m_Sel.m_MeshType	= (NcSpriteFactory.MESH_TYPE)EditorGUILayout.EnumPopup(GetHelpContent("m_MeshType"), m_Sel.m_MeshType);
				m_Sel.m_AlignType	= (NcSpriteFactory.ALIGN_TYPE)EditorGUILayout.EnumPopup(GetHelpContent("m_AlignType"), m_Sel.m_AlignType);
				if (m_Sel.m_AlignType == NcSpriteFactory.ALIGN_TYPE.LEFTCENTER)
					m_Sel.m_fShowRate	= EditorGUILayout.FloatField	(GetHelpContent("m_fShowRate")		, m_Sel.m_fShowRate);
				m_Sel.m_bTrimCenterAlign	= EditorGUILayout.Toggle("m_bTrimCenterAlign", m_Sel.m_bTrimCenterAlign);
				SetMaxValue(ref m_Sel.m_fShowRate, 1.0f);
				SetMinValue(ref m_Sel.m_fShowRate, 0);
			} else
			if (m_Sel.m_TextureType == NcSpriteAnimation.TEXTURE_TYPE.SpriteFactory)
			{
				m_Sel.m_NcSpriteFactoryPrefab	= (GameObject)EditorGUILayout.ObjectField(GetHelpContent("m_NcSpriteFactoryPrefab"), m_Sel.m_NcSpriteFactoryPrefab, typeof(GameObject), false, null);
				// --------------------------------------------------------------
				EditorGUILayout.Space();
				rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(m_fButtonHeight));
				{
					if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(rect, 3, 0, 1), GetHelpContent("Select SpriteFactory"), (m_FxmPopupManager != null)))
						m_FxmPopupManager.ShowSelectPrefabPopup(m_Sel, true, 0);
					if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(rect, 3, 1, 1), GetHelpContent("Clear SpriteFactory"), (m_Sel.m_NcSpriteFactoryPrefab != null)))
					{
						bClickButton = true;
						m_Sel.m_NcSpriteFactoryPrefab = null;
					}
					if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(rect, 3, 2, 1), GetHelpContent("Open Prefab"), (m_FxmPopupManager != null) && (m_Sel.m_NcSpriteFactoryPrefab != null)))
					{
						bClickButton = true;
						GetFXMakerMain().OpenPrefab(m_Sel.m_NcSpriteFactoryPrefab);
						return;
					}
					GUILayout.Label("");
				}
				EditorGUILayout.EndHorizontal();

				// --------------------------------------------------------------
				if (m_Sel.m_NcSpriteFactoryPrefab != null && m_Sel.m_NcSpriteFactoryPrefab.GetComponent<Renderer>() != null && m_Sel.GetComponent<Renderer>())
					if (m_Sel.m_NcSpriteFactoryPrefab.GetComponent<Renderer>().sharedMaterial != m_Sel.GetComponent<Renderer>().sharedMaterial)
						m_Sel.UpdateFactoryMaterial();

				// --------------------------------------------------------------
				rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(m_fButtonHeight));
				{
					if (FXMakerLayout.GUIButton(rect, GetHelpContent("Update from FactoryPrefab"), (m_Sel.m_NcSpriteFactoryPrefab != null)))
					{
						m_Sel.SetSpriteFactoryIndex(m_Sel.m_nSpriteFactoryIndex, false);
					}
					GUILayout.Label("");
				}
				EditorGUILayout.EndHorizontal();

				NcSpriteFactory ncSpriteFactory	= (m_Sel.m_NcSpriteFactoryPrefab == null ? null : m_Sel.m_NcSpriteFactoryPrefab.GetComponent<NcSpriteFactory>());
				if (ncSpriteFactory != null)
				{
					int nSelIndex	= EditorGUILayout.IntSlider(GetHelpContent("m_nSpriteFactoryIndex")	, m_Sel.m_nSpriteFactoryIndex, 0, ncSpriteFactory.GetSpriteNodeCount()-1);
					if (m_Sel.m_nSpriteFactoryIndex != nSelIndex)
						m_Sel.SetSpriteFactoryIndex(nSelIndex, false);
				}

				m_Sel.m_MeshType	= (NcSpriteFactory.MESH_TYPE)EditorGUILayout.EnumPopup(GetHelpContent("m_MeshType"), m_Sel.m_MeshType);
				m_Sel.m_AlignType	= (NcSpriteFactory.ALIGN_TYPE)EditorGUILayout.EnumPopup(GetHelpContent("m_AlignType"), m_Sel.m_AlignType);
				if (m_Sel.m_AlignType == NcSpriteFactory.ALIGN_TYPE.LEFTCENTER)
					m_Sel.m_fShowRate	= EditorGUILayout.FloatField	(GetHelpContent("m_fShowRate")		, m_Sel.m_fShowRate);
				m_Sel.m_bTrimCenterAlign	= EditorGUILayout.Toggle("m_bTrimCenterAlign", m_Sel.m_bTrimCenterAlign);
				SetMaxValue(ref m_Sel.m_fShowRate, 1.0f);
				SetMinValue(ref m_Sel.m_fShowRate, 0);
			}

			m_Sel.m_PlayMode				= (NcSpriteAnimation.PLAYMODE)EditorGUILayout.EnumPopup (GetHelpContent("m_PlayMode")		, m_Sel.m_PlayMode, GUILayout.MaxWidth(Screen.width));
			if (m_Sel.m_PlayMode != NcSpriteAnimation.PLAYMODE.SELECT)
				m_Sel.m_fDelayTime			= EditorGUILayout.FloatField(GetHelpContent("m_fDelayTime")		, m_Sel.m_fDelayTime);

			m_Sel.m_nStartFrame	= EditorGUILayout.IntField	(GetHelpContent("m_nStartFrame")	, m_Sel.m_nStartFrame);
			m_Sel.m_nFrameCount	= EditorGUILayout.IntField	(GetHelpContent("m_nFrameCount")	, m_Sel.m_nFrameCount);

			if (m_Sel.m_PlayMode == NcSpriteAnimation.PLAYMODE.SELECT)
				m_Sel.m_nSelectFrame		= EditorGUILayout.IntField	(GetHelpContent("m_nSelectFrame")	, m_Sel.m_nSelectFrame);

			if (m_Sel.m_PlayMode != NcSpriteAnimation.PLAYMODE.RANDOM && m_Sel.m_PlayMode != NcSpriteAnimation.PLAYMODE.SELECT)
			{
				bool bOldLoop = m_Sel.m_bLoop;
				m_Sel.m_bLoop				= EditorGUILayout.Toggle	(GetHelpContent("m_bLoop")			, m_Sel.m_bLoop);
				if (!bOldLoop && m_Sel.m_bLoop)
				{
					m_Sel.m_nLoopStartFrame	= 0;
					m_Sel.m_nLoopFrameCount	= m_Sel.m_nFrameCount;
					m_Sel.m_nLoopingCount	= 0;
				}
				if (m_Sel.m_bLoop && m_Sel.m_PlayMode == NcSpriteAnimation.PLAYMODE.DEFAULT)
				{
					m_Sel.m_nLoopStartFrame	= EditorGUILayout.IntField("  nLoopStartFrame", m_Sel.m_nLoopStartFrame);
					m_Sel.m_nLoopFrameCount	= EditorGUILayout.IntField("  nLoopFrameCount", m_Sel.m_nLoopFrameCount);
					m_Sel.m_nLoopingCount	= EditorGUILayout.IntField("  nLoopingCount", m_Sel.m_nLoopingCount);
					if (m_Sel.m_fDelayTime == 0)
						m_Sel.m_bLoopRandom	= EditorGUILayout.Toggle	(GetHelpContent("m_bLoopRandom")	, m_Sel.m_bLoopRandom);
				}

				if (m_Sel.m_bLoop == false || 0 < m_Sel.m_nLoopingCount)
					m_Sel.m_bAutoDestruct	= EditorGUILayout.Toggle	(GetHelpContent("m_bAutoDestruct")	, m_Sel.m_bAutoDestruct);
				m_Sel.m_fFps				= EditorGUILayout.FloatField(GetHelpContent("m_fFps")			, m_Sel.m_fFps);
			}

			// check
			SetMinValue(ref m_Sel.m_nTilingX, 1);
			SetMinValue(ref m_Sel.m_nTilingY, 1);
			SetMinValue(ref m_Sel.m_fFps, 0.1f);
			SetMinValue(ref m_Sel.m_fDelayTime, 0);
			SetMaxValue(ref m_Sel.m_nStartFrame, m_Sel.GetMaxFrameCount()-1);
			SetMinValue(ref m_Sel.m_nStartFrame, 0);
			SetMaxValue(ref m_Sel.m_nFrameCount, m_Sel.GetValidFrameCount());
			SetMinValue(ref m_Sel.m_nFrameCount, 1);
			SetMaxValue(ref m_Sel.m_nSelectFrame, (0 < m_Sel.m_nFrameCount ? m_Sel.m_nFrameCount-1 : m_Sel.m_nTilingX*m_Sel.m_nTilingY-1));
			SetMinValue(ref m_Sel.m_nSelectFrame, 0);

			SetMaxValue(ref m_Sel.m_nLoopStartFrame, m_Sel.m_nFrameCount-1);
			SetMinValue(ref m_Sel.m_nLoopStartFrame, 0);
			SetMinValue(ref m_Sel.m_nLoopFrameCount, 0);
			SetMaxValue(ref m_Sel.m_nLoopFrameCount, m_Sel.m_nFrameCount-m_Sel.m_nLoopStartFrame);
			SetMinValue(ref m_Sel.m_nLoopingCount, 0);

			if (m_Sel.m_PlayMode != NcSpriteAnimation.PLAYMODE.RANDOM && m_Sel.m_PlayMode != NcSpriteAnimation.PLAYMODE.SELECT)
				EditorGUILayout.TextField(GetHelpContent("DurationTime"), m_Sel.GetDurationTime().ToString());


			// Texture --------------------------------------------------------------
			rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(GetDrawTextureHeight(m_Sel.GetComponent<Renderer>())));
			{
				GUI.Box(rect, "");
				GUILayout.Label("");

				Rect subRect = rect;

				// draw texture
				if (m_Sel.GetComponent<Renderer>() != null && m_Sel.GetComponent<Renderer>().sharedMaterial != null && m_Sel.GetComponent<Renderer>().sharedMaterial.mainTexture != null)
				{
					int nClickFrameIndex;
					if (m_Sel.m_TextureType == NcSpriteAnimation.TEXTURE_TYPE.TileTexture)
					{
						if (DrawTileTexture(subRect, (m_Sel.m_PlayMode == NcSpriteAnimation.PLAYMODE.SELECT), m_Sel.GetComponent<Renderer>().sharedMaterial, m_Sel.m_nTilingX, m_Sel.m_nTilingY, m_Sel.m_nStartFrame, m_Sel.m_nFrameCount, m_Sel.m_nSelectFrame, out nClickFrameIndex))
						{
							bClickButton	= true;
							if (bClickButton && m_Sel.m_PlayMode == NcSpriteAnimation.PLAYMODE.SELECT)
								m_Sel.m_nSelectFrame = nClickFrameIndex;
						}
					}

					if (m_Sel.m_TextureType != NcSpriteAnimation.TEXTURE_TYPE.TileTexture)
					{
						if (DrawTrimTexture(subRect, (m_Sel.m_PlayMode == NcSpriteAnimation.PLAYMODE.SELECT), m_Sel.GetComponent<Renderer>().sharedMaterial, m_Sel.m_NcSpriteFrameInfos, m_Sel.m_nStartFrame, m_Sel.m_nFrameCount, m_Sel.m_nSelectFrame, out nClickFrameIndex))
						{
							bClickButton	= true;
							if (bClickButton && m_Sel.m_PlayMode == NcSpriteAnimation.PLAYMODE.SELECT)
								m_Sel.m_nSelectFrame = nClickFrameIndex;
						}
					}
				}
			}
			EditorGUILayout.EndHorizontal();
			m_UndoManager.CheckDirty();

			EditorGUILayout.Space();
			// Remove AlphaChannel
			if (GUILayout.Button(GetHelpContent("Remove AlphaChannel")))
				NgAtlas.ConvertAlphaTexture(m_Sel.GetComponent<Renderer>().sharedMaterial, false, m_Sel.m_curveAlphaWeight, 1, 1, 1);
			// AlphaWeight
			if ((m_Sel.m_curveAlphaWeight == null || m_Sel.m_curveAlphaWeight.length <= 0) && FXMakerOption.inst != null)
				m_Sel.m_curveAlphaWeight = FXMakerOption.inst.m_AlphaWeightCurve;
			if (m_Sel.m_curveAlphaWeight != null)
			{
				bool bHighLight = m_Sel.m_bNeedRebuildAlphaChannel;
				if (bHighLight)
	 				FXMakerLayout.GUIColorBackup(FXMakerLayout.m_ColorHelpBox);
				if (GUILayout.Button(GetHelpContent("Adjust the alpha channel with AlphaWeight")))
				{
					m_Sel.m_bNeedRebuildAlphaChannel = false;
					NgAtlas.ConvertAlphaTexture(m_Sel.GetComponent<Renderer>().sharedMaterial, true, m_Sel.m_curveAlphaWeight, 1, 1, 1);
//					NgAtlas.ConvertAlphaTexture(m_Sel.renderer.sharedMaterial, m_Sel.m_curveAlphaWeight, m_Sel.m_fRedAlphaWeight, m_Sel.m_fGreenAlphaWeight, m_Sel.m_fBlueAlphaWeight);
				}
				if (bHighLight)
					FXMakerLayout.GUIColorRestore();

				EditorGUI.BeginChangeCheck();
				m_Sel.m_curveAlphaWeight	= EditorGUILayout.CurveField(GetHelpContent("m_curveAlphaWeight"), m_Sel.m_curveAlphaWeight);
				if (EditorGUI.EndChangeCheck())
					m_Sel.m_bNeedRebuildAlphaChannel = true;
// 				m_Sel.m_fRedAlphaWeight		= EditorGUILayout.Slider("", m_Sel.m_fRedAlphaWeight	, 0, 1.0f);
// 				m_Sel.m_fGreenAlphaWeight	= EditorGUILayout.Slider("", m_Sel.m_fGreenAlphaWeight	, 0, 1.0f);
//	 			m_Sel.m_fBlueAlphaWeight	= EditorGUILayout.Slider("", m_Sel.m_fBlueAlphaWeight	, 0, 1.0f);
			}

			EditorGUILayout.Space();

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
	protected GUIContent GetHelpContent(string tooltip)
	{
		string caption	= tooltip;
		string text		= FXMakerTooltip.GetHsEditor_NcSpriteAnimation(tooltip);
		return GetHelpContent(caption, text);
	}

	protected override void HelpBox(string caption)
	{
		string	str	= caption;
		if (caption == "" || caption == "Script")
			str = FXMakerTooltip.GetHsEditor_NcSpriteAnimation("");
		base.HelpBox(str);
	}
}
