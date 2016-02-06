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

[CustomEditor(typeof(NcSpriteFactory))]

public class NcSpriteFactoryEditor : FXMakerEditor
{
	// Attribute ------------------------------------------------------------------------
	protected	NcSpriteFactory		m_Sel;
	protected	FxmPopupManager		m_FxmPopupManager;

	// Property -------------------------------------------------------------------------
	// Event Function -------------------------------------------------------------------
    void OnEnable()
    {
 		m_Sel = target as NcSpriteFactory;
 		m_UndoManager	= new FXMakerUndoManager(m_Sel, "NcSpriteFactory");
   }

    void OnDisable()
    {
		if (m_FxmPopupManager != null && m_FxmPopupManager.IsShowByInspector())
			m_FxmPopupManager.CloseNcPrefabPopup();
    }

	public override void OnInspectorGUI()
	{
		AddScriptNameField(m_Sel);

		int				nClickIndex		= -1;
		int				nClickButton	= 0;
		Rect			rect;
		int				nLeftWidth		= 34;
		int				nAddHeight		= 30;
		int				nDelWidth		= 35;
		int				nLineHeight		= 18;
		int				nSpriteHeight	= nLeftWidth;
		List<NcSpriteFactory.NcSpriteNode>	spriteList = m_Sel.m_SpriteList;

		m_FxmPopupManager = GetFxmPopupManager();

		// --------------------------------------------------------------
		bool bClickButton = false;
		EditorGUI.BeginChangeCheck();
		{
			m_UndoManager.CheckUndo();
			// --------------------------------------------------------------
			m_Sel.m_fUserTag = EditorGUILayout.FloatField(GetCommonContent("m_fUserTag"), m_Sel.m_fUserTag);

			EditorGUILayout.Space();
			m_Sel.m_SpriteType			= (NcSpriteFactory.SPRITE_TYPE)EditorGUILayout.EnumPopup(GetHelpContent("m_SpriteType"), m_Sel.m_SpriteType);

			// --------------------------------------------------------------
			if (m_Sel.m_SpriteType == NcSpriteFactory.SPRITE_TYPE.NcSpriteAnimation && m_Sel.gameObject.GetComponent("NcSpriteAnimation") == null)
			{
				rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(m_fButtonHeight));
				{
					if (FXMakerLayout.GUIButton(rect, GetHelpContent("Add NcSpriteAnimation Component"), true))
						m_Sel.gameObject.AddComponent<NcSpriteAnimation>();
					GUILayout.Label("");
				}
				EditorGUILayout.EndHorizontal();
			}
			// --------------------------------------------------------------
			if (m_Sel.m_SpriteType == NcSpriteFactory.SPRITE_TYPE.NcSpriteTexture && m_Sel.gameObject.GetComponent("NcSpriteTexture") == null)
			{
				rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(m_fButtonHeight));
				{
					if (FXMakerLayout.GUIButton(rect, GetHelpContent("Add NcSpriteTexture Component"), true))
						m_Sel.gameObject.AddComponent<NcSpriteTexture>();
					GUILayout.Label("");
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.Space();

			// --------------------------------------------------------------
			int		nSelIndex			= EditorGUILayout.IntSlider(GetHelpContent("m_nCurrentIndex")	, m_Sel.m_nCurrentIndex, 0, (spriteList==null ? 0 : spriteList.Count-1));
			float	fUvScale			= EditorGUILayout.FloatField(GetHelpContent("m_fUvScale")		, m_Sel.m_fUvScale);
			if (m_Sel.m_nCurrentIndex != nSelIndex || fUvScale != m_Sel.m_fUvScale)
			{
				m_Sel.m_nCurrentIndex	= nSelIndex;
				m_Sel.m_fUvScale		= fUvScale;	
				m_Sel.SetSprite(nSelIndex, false);
			}

			// Rebuild Check
			EditorGUI.BeginChangeCheck();
			m_Sel.m_bTrimBlack			= EditorGUILayout.Toggle(GetHelpContent("m_bTrimBlack")			, m_Sel.m_bTrimBlack);
			m_Sel.m_bTrimAlpha			= EditorGUILayout.Toggle(GetHelpContent("m_bTrimAlpha")			, m_Sel.m_bTrimAlpha);
			m_Sel.m_nMaxAtlasTextureSize= EditorGUILayout.IntPopup("nMaxAtlasTextureSize", m_Sel.m_nMaxAtlasTextureSize, NgEnum.m_TextureSizeStrings, NgEnum.m_TextureSizeIntters);
// 			m_Sel.m_AtlasMaterial		= (Material)EditorGUILayout.ObjectField(GetHelpContent("m_AtlasMaterial")	, m_Sel.m_AtlasMaterial, typeof(Material), false);
			if (EditorGUI.EndChangeCheck())
				m_Sel.m_bNeedRebuild = true;

			if (spriteList.Count <= m_Sel.m_nBuildStartIndex)
				m_Sel.m_nBuildStartIndex = 0;
			int nBuildStartIndex		= EditorGUILayout.IntSlider("m_nBuildStartIndex", m_Sel.m_nBuildStartIndex, 0, (spriteList==null ? 0 : spriteList.Count-1));
			if (spriteList != null && m_Sel.m_nBuildStartIndex < spriteList.Count && nBuildStartIndex != m_Sel.m_nBuildStartIndex)
			{
				for (int n = Mathf.Min(m_Sel.m_nBuildStartIndex, nBuildStartIndex); n < Mathf.Max(m_Sel.m_nBuildStartIndex, nBuildStartIndex); n++)
					if (spriteList[n].IsUnused() == false && spriteList[n].IsEmptyTexture() == false)
						m_Sel.m_bNeedRebuild = true;
				m_Sel.m_nBuildStartIndex	= nBuildStartIndex;
			}

			// SpriteResizeRate
			m_Sel.m_fSpriteResizeRate = EditorGUILayout.Slider("fSpriteResizeRate", m_Sel.m_fSpriteResizeRate, 0.1f, 1.0f);

			// nImageHeight
			nSpriteHeight	= PlayerPrefs.GetInt("FXMakerEditor.nFactorySpriteHeight", nSpriteHeight);
			nSpriteHeight	= EditorGUILayout.IntSlider("nSpriteHeight", nSpriteHeight, 0, 400);
			PlayerPrefs.SetInt("FXMakerEditor.nFactorySpriteHeight", nSpriteHeight);


			// Add Button ------------------------------------------------------
			EditorGUILayout.Space();
			rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(nAddHeight*2));
			{
				Rect lineRect = FXMakerLayout.GetInnerVerticalRect(rect, 2, 0, 1);
				if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(lineRect, 3, 0, 1), GetHelpContent("Add Sprite")))
				{
					bClickButton	= true;
					m_Sel.AddSpriteNode();
				}

				bool bHighLight = m_Sel.m_bNeedRebuild;
				if (bHighLight)
		 			FXMakerLayout.GUIColorBackup(FXMakerLayout.m_ColorHelpBox);
				if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(lineRect, 3, 1, 1), GetHelpContent("Build Sprite")))
				{
#if UNITY_WEBPLAYER
		Debug.LogError("In WEB_PLAYER mode, you cannot run the FXMaker.");
		Debug.Break();
#else
					bClickButton	= true;
					CreateSpriteAtlas(m_Sel.GetComponent<Renderer>().sharedMaterial);
					m_Sel.m_bNeedRebuild = false;
#endif
				}
				if (bHighLight)
					FXMakerLayout.GUIColorRestore();
				if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(lineRect, 3, 2, 1), GetHelpContent("Clear All"), (0 < m_Sel.GetSpriteNodeCount())))
				{
					bClickButton	= true;
					if (m_FxmPopupManager != null)
						m_FxmPopupManager.CloseNcPrefabPopup();
					m_Sel.ClearAllSpriteNode();
				}
				lineRect = FXMakerLayout.GetInnerVerticalRect(rect, 2, 1, 1);
				if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(lineRect, 3, 0, 1), GetHelpContent("Sequence"), (0 < m_Sel.GetSpriteNodeCount())))
				{
					m_Sel.m_bSequenceMode	= true;
					bClickButton			= true;
					m_Sel.SetSprite(0, false);
				}
				if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(lineRect, 3, 1, 1), GetHelpContent("NewMaterial"), true))
				{
					Material	newMat		= new Material(m_Sel.GetComponent<Renderer>().sharedMaterial);
					string		matPath		= AssetDatabase.GetAssetPath(m_Sel.GetComponent<Renderer>().sharedMaterial);
					NgMaterial.SaveMaterial(newMat, NgFile.TrimFilenameExt(matPath), m_Sel.name); 
					m_Sel.GetComponent<Renderer>().sharedMaterial = newMat;
// 					m_Sel.renderer.sharedMaterial = (Material)AssetDatabase.LoadAssetAtPath(savePath, typeof(Material));
				}

 				GUILayout.Label("");
			}
			EditorGUILayout.EndHorizontal();

			// Select ShotType -------------------------------------------------
//			showType		= (NcSpriteFactory.SHOW_TYPE)EditorGUILayout.EnumPopup		(GetHelpContent("m_ShowType")	, showType);
			// --------------------------------------------------------------
			EditorGUILayout.Space();
			NcSpriteFactory.SHOW_TYPE showType = (NcSpriteFactory.SHOW_TYPE)EditorPrefs.GetInt("NcSpriteFactory.SHOW_TYPE", 0);
	
			rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(nLineHeight));
			{
				showType	= FXMakerLayout.GUIToggle(FXMakerLayout.GetInnerHorizontalRect(rect, 5, 0, 1), showType==NcSpriteFactory.SHOW_TYPE.NONE		, GetHelpContent("NONE")		, true) ? NcSpriteFactory.SHOW_TYPE.NONE	: showType;
				showType	= FXMakerLayout.GUIToggle(FXMakerLayout.GetInnerHorizontalRect(rect, 5, 1, 1), showType==NcSpriteFactory.SHOW_TYPE.ALL		, GetHelpContent("ALL")			, true) ? NcSpriteFactory.SHOW_TYPE.ALL		: showType;
				if (m_Sel.m_SpriteType == NcSpriteFactory.SPRITE_TYPE.NcSpriteAnimation || m_Sel.m_SpriteType == NcSpriteFactory.SPRITE_TYPE.Auto)
				{
					showType= FXMakerLayout.GUIToggle(FXMakerLayout.GetInnerHorizontalRect(rect, 5, 2, 1), showType==NcSpriteFactory.SHOW_TYPE.SPRITE	, GetHelpContent("SPRITE")		, true) ? NcSpriteFactory.SHOW_TYPE.SPRITE		: showType;
					showType= FXMakerLayout.GUIToggle(FXMakerLayout.GetInnerHorizontalRect(rect, 5, 3, 1), showType==NcSpriteFactory.SHOW_TYPE.ANIMATION, GetHelpContent("ANIMATION")	, true) ? NcSpriteFactory.SHOW_TYPE.ANIMATION	: showType;
				}
				showType	= FXMakerLayout.GUIToggle(FXMakerLayout.GetInnerHorizontalRect(rect, 5, 4, 1), showType==NcSpriteFactory.SHOW_TYPE.EFFECT	, GetHelpContent("EFFECT")		, true) ? NcSpriteFactory.SHOW_TYPE.EFFECT		: showType;
				GUILayout.Label("");
			}
			EditorGUILayout.EndHorizontal();

			EditorPrefs.SetInt("NcSpriteFactory.SHOW_TYPE", ((int)showType));

			// Show Option -------------------------------------------------
			EditorGUILayout.Space();
			rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(nLineHeight));
			{
				m_Sel.m_bShowEffect			= FXMakerLayout.GUIToggle(FXMakerLayout.GetInnerHorizontalRect(rect, 3, 0, 1), m_Sel.m_bShowEffect	, GetHelpContent("m_bShowEffect")	, true);
				if (m_Sel.m_SpriteType == NcSpriteFactory.SPRITE_TYPE.NcSpriteAnimation || m_Sel.m_SpriteType == NcSpriteFactory.SPRITE_TYPE.Auto)
				{
					m_Sel.m_bTestMode		= FXMakerLayout.GUIToggle(FXMakerLayout.GetInnerHorizontalRect(rect, 3, 1, 1), m_Sel.m_bTestMode		, GetHelpContent("m_bTestMode")		, true);
					m_Sel.m_bSequenceMode	= FXMakerLayout.GUIToggle(FXMakerLayout.GetInnerHorizontalRect(rect, 3, 2, 1), m_Sel.m_bSequenceMode	, GetHelpContent("m_bSequenceMode")	, true);
				}
				GUILayout.Label("");
			}
			EditorGUILayout.EndHorizontal();

			// Node List ------------------------------------------------------
			for (int n = 0; n < (spriteList != null ? spriteList.Count : 0); n++)
			{
				if (n < m_Sel.m_nBuildStartIndex)
					continue;

				EditorGUILayout.Space();

				EditorGUI.BeginChangeCheck();
				// Load Texture ---------------------------------------------------------
				Texture2D	selTexture = null;
				if (spriteList[n].m_TextureGUID != "")
					selTexture = (Texture2D)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(spriteList[n].m_TextureGUID), typeof(Texture2D));

				// Enabled --------------------------------------------------------------
				rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(nLineHeight));
				{
					Rect subRect;
					EditorGUI.BeginChangeCheck();
					// enable
					spriteList[n].m_bIncludedAtlas = GUILayout.Toggle(spriteList[n].m_bIncludedAtlas, "Idx", GUILayout.Width(nLeftWidth));
					// change index
					subRect = rect;
					subRect.x += nLeftWidth;
					subRect.width = nLineHeight*2;
					int newPos = EditorGUI.IntPopup(subRect, n, NgConvert.GetIntStrings(0, spriteList.Count), NgConvert.GetIntegers(0, spriteList.Count));
					if (newPos != n)
					{
						NcSpriteFactory.NcSpriteNode node = spriteList[n];
						m_Sel.m_SpriteList.Remove(node);
						m_Sel.m_SpriteList.Insert(newPos, node);
						return;
					}
					if (EditorGUI.EndChangeCheck())
						m_Sel.m_bNeedRebuild = true;

					// name
					subRect = rect;
					subRect.x += nLeftWidth+nLineHeight*2;
					subRect.width -= nLeftWidth+nLineHeight*2;
 					spriteList[n].m_TextureName = selTexture==null ? "" : selTexture.name;
 					GUI.Label(subRect, (selTexture==null ? "" : "(" + spriteList[n].m_nFrameCount + ") " + selTexture.name));
					GUI.Box(subRect, "");
					GUI.Box(rect, "");

					// delete
					if (GUI.Button(new Rect(subRect.x+subRect.width-nDelWidth, subRect.y, nDelWidth, subRect.height), GetHelpContent("Del")))
					{
						m_Sel.m_bNeedRebuild = true;
						bClickButton	= true;
						if (m_FxmPopupManager != null)
							m_FxmPopupManager.CloseNcPrefabPopup();
						m_Sel.DeleteSpriteNode(n);
						return;
					}
				}
				EditorGUILayout.EndHorizontal();

				// SpriteName MaxAlpha -----------------------------------------------------------
				rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(nLineHeight));
				{
					GUILayout.Label("", GUILayout.Width(nLineHeight));
					GUI.Label(FXMakerLayout.GetInnerHorizontalRect(rect, 7, 0, 2), "Name,fMaxAlpha");
					spriteList[n].m_SpriteName			= EditorGUI.TextField(FXMakerLayout.GetInnerHorizontalRect(rect, 7, 2, 4), spriteList[n].m_SpriteName);
					spriteList[n].m_fMaxTextureAlpha	= EditorGUI.FloatField(FXMakerLayout.GetInnerHorizontalRect(rect, 7, 6, 1), spriteList[n].m_fMaxTextureAlpha);
				}
				EditorGUILayout.EndHorizontal();

				// Texture --------------------------------------------------------------
				rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(nSpriteHeight / (selTexture == null ? 2 : 1)));
				{
					GUILayout.Label("", GUILayout.Width(nLeftWidth));

					Rect subRect = rect;
					subRect.width = nLeftWidth;
					FXMakerLayout.GetOffsetRect(rect, 0, 5, 0, -5);
					EditorGUI.BeginChangeCheck();
					selTexture	= (Texture2D)EditorGUI.ObjectField(subRect, GetHelpContent(""), selTexture, typeof(Texture2D), false);
					if (EditorGUI.EndChangeCheck())
					{
						if (selTexture != null)
							spriteList[n].m_TextureGUID	= AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(selTexture));
						m_Sel.m_bNeedRebuild = true;
					}

					// draw texture
					subRect = FXMakerLayout.GetOffsetRect(rect, nLeftWidth+4, 0, 0, -4);
					Rect drawRect = FXMakerLayout.GetOffsetRect(subRect, 0, 0, -nDelWidth, 0);

					if (selTexture != null)
					{
						// draw texture
						GUI.DrawTexture(drawRect, selTexture, ScaleMode.ScaleToFit, true, selTexture.width/selTexture.height);

						// draw tile
						float	fDrawRatio	= drawRect.width / drawRect.height;
						float	fImageRatio	= selTexture.width / selTexture.height;
						if (fDrawRatio < fImageRatio)
						{
							drawRect = FXMakerLayout.GetVOffsetRect(drawRect, drawRect.height * -(1 - (fDrawRatio / fImageRatio)) / 2);
						} else {
							drawRect = FXMakerLayout.GetHOffsetRect(drawRect, drawRect.width * -(1 - (fImageRatio / fDrawRatio)) / 2);
						}
						float	tileWidth	= (drawRect.width  / spriteList[n].m_nTilingX);
						float	tileHeight	= (drawRect.height / spriteList[n].m_nTilingY);
						int		nCount		= 0;

						for (int tn = spriteList[n].m_nStartFrame; tn < spriteList[n].m_nTilingX * spriteList[n].m_nTilingY; tn += spriteList[n].m_nSkipFrame+1, nCount++)
						{
							if (spriteList[n].m_nStartFrame + spriteList[n].m_nFrameCount <= nCount)
								break;

							int posx = tn % spriteList[n].m_nTilingX;
							int posy = tn / spriteList[n].m_nTilingX;
							Rect tileRect = new Rect(drawRect.x+posx*tileWidth, drawRect.y+posy*tileHeight, tileWidth, tileHeight);
							NgGUIDraw.DrawBox(FXMakerLayout.GetOffsetRect(tileRect, -1), Color.green, 1, false);
						}
					}

					// delete
					if (GUI.Button(new Rect(subRect.x+subRect.width-nDelWidth, subRect.y, nDelWidth, subRect.height), GetHelpContent("Rmv")))
						spriteList[n].SetEmpty();
					GUI.Box(rect, "");
				}
				EditorGUILayout.EndHorizontal();

				// Change selIndex
				Event e = Event.current;
				if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
				{
					nClickIndex = n;
					nClickButton = e.button;
				}

				// -----------------------------------------------------------------------------------------------------------------------------------------------
				if (spriteList[n].IsEmptyTexture() == false)
				{
					// m_nSkipFrame
					rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(nLineHeight));
					{
						GUILayout.Label("", GUILayout.Width(nLineHeight));
						GUI.Label(FXMakerLayout.GetInnerHorizontalRect(rect, 8, 0, 4), "m_nSkipFrame");

						EditorGUI.BeginChangeCheck();
						spriteList[n].m_nSkipFrame		= EditorGUI.IntField(FXMakerLayout.GetInnerHorizontalRect(rect, 8, 4, 1), spriteList[n].m_nSkipFrame);
						SetMinValue(ref spriteList[n].m_nSkipFrame	, 0);
						if (EditorGUI.EndChangeCheck())
							m_Sel.m_bNeedRebuild = true;
					}
					EditorGUILayout.EndHorizontal();

					// Frame tile
					rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(nLineHeight));
					{
						GUILayout.Label("", GUILayout.Width(nLineHeight));
						GUI.Label(FXMakerLayout.GetInnerHorizontalRect(rect, 8, 0, 4), "nTileXY,Start,Count");

						// m_nTilingX, m_nTilingY
						EditorGUI.BeginChangeCheck();
						spriteList[n].m_nTilingX		= EditorGUI.IntField(FXMakerLayout.GetInnerHorizontalRect(rect, 8, 4, 1), spriteList[n].m_nTilingX);
						spriteList[n].m_nTilingY		= EditorGUI.IntField(FXMakerLayout.GetInnerHorizontalRect(rect, 8, 5, 1), spriteList[n].m_nTilingY);
						SetMinValue(ref spriteList[n].m_nTilingX	, 1);
						SetMinValue(ref spriteList[n].m_nTilingY	, 1);
						if (EditorGUI.EndChangeCheck())
						{
							spriteList[n].m_nFrameCount	= (spriteList[n].m_nTilingX*spriteList[n].m_nTilingY-spriteList[n].m_nStartFrame) / spriteList[n].m_nSkipFrame;
							m_Sel.m_bNeedRebuild = true;
						}

						// m_nStartFrame, m_nFrameCount
						EditorGUI.BeginChangeCheck();
						spriteList[n].m_nStartFrame		= EditorGUI.IntField(FXMakerLayout.GetInnerHorizontalRect(rect, 8, 6, 1), spriteList[n].m_nStartFrame);
						spriteList[n].m_nFrameCount		= EditorGUI.IntField(FXMakerLayout.GetInnerHorizontalRect(rect, 8, 7, 1), spriteList[n].m_nFrameCount);
						SetMaxValue(ref spriteList[n].m_nStartFrame	, spriteList[n].m_nTilingX*spriteList[n].m_nTilingY-1);
						SetMinValue(ref spriteList[n].m_nStartFrame	, 0);
						SetMaxValue(ref spriteList[n].m_nFrameCount	, (spriteList[n].m_nTilingX*spriteList[n].m_nTilingY-spriteList[n].m_nStartFrame) / (spriteList[n].m_nSkipFrame+1));
						SetMinValue(ref spriteList[n].m_nFrameCount	, 1);
						if (EditorGUI.EndChangeCheck())
							m_Sel.m_bNeedRebuild = true;
					}
					EditorGUILayout.EndHorizontal();

					// SpriteNode ----------------------------------------------------------
					if (bClickButton == false)
					{
						if ((m_Sel.m_SpriteType == NcSpriteFactory.SPRITE_TYPE.NcSpriteAnimation || m_Sel.m_SpriteType == NcSpriteFactory.SPRITE_TYPE.Auto) && (showType == NcSpriteFactory.SHOW_TYPE.ALL || showType == NcSpriteFactory.SHOW_TYPE.SPRITE))
						{
							rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(nLineHeight));
							{
								GUILayout.Label("", GUILayout.Width(nLineHeight));
								GUI.Label(FXMakerLayout.GetInnerHorizontalRect(rect, 8, 0, 4), "Loop,Start,FCnt,LCnt");

								bool bOldLoop = spriteList[n].m_bLoop;
								spriteList[n].m_bLoop				= EditorGUI.Toggle	(FXMakerLayout.GetInnerHorizontalRect(rect, 8, 4, 1), spriteList[n].m_bLoop);
								if (!bOldLoop && spriteList[n].m_bLoop)
								{
									spriteList[n].m_nLoopStartFrame	= 0;
									spriteList[n].m_nLoopFrameCount	= spriteList[n].m_nFrameCount;
									spriteList[n].m_nLoopingCount	= 0;
								}
								if (spriteList[n].m_bLoop)
								{
									spriteList[n].m_nLoopStartFrame	= EditorGUI.IntField(FXMakerLayout.GetInnerHorizontalRect(rect, 8, 5, 1), spriteList[n].m_nLoopStartFrame);
									spriteList[n].m_nLoopFrameCount	= EditorGUI.IntField(FXMakerLayout.GetInnerHorizontalRect(rect, 8, 6, 1), spriteList[n].m_nLoopFrameCount);
									spriteList[n].m_nLoopingCount	= EditorGUI.IntField(FXMakerLayout.GetInnerHorizontalRect(rect, 8, 7, 1), spriteList[n].m_nLoopingCount);
								}
								// check
								SetMaxValue(ref spriteList[n].m_nLoopStartFrame, spriteList[n].m_nFrameCount-1);
								SetMinValue(ref spriteList[n].m_nLoopStartFrame, 0);
								SetMaxValue(ref spriteList[n].m_nLoopFrameCount, spriteList[n].m_nFrameCount-spriteList[n].m_nLoopStartFrame);
								SetMinValue(ref spriteList[n].m_nLoopingCount, 0);
							}
							EditorGUILayout.EndHorizontal();

							spriteList[n].m_fFps				= EditorGUILayout.Slider	(GetHelpContent("m_fFps")	, spriteList[n].m_nFrameCount/spriteList[n].m_fTime, 50, 1, null);
							spriteList[n].m_fTime				= EditorGUILayout.Slider	(GetHelpContent("m_fTime")	, spriteList[n].m_nFrameCount/spriteList[n].m_fFps, 0, 5, null);
						}

						if ((m_Sel.m_SpriteType == NcSpriteFactory.SPRITE_TYPE.NcSpriteAnimation || m_Sel.m_SpriteType == NcSpriteFactory.SPRITE_TYPE.Auto) && (showType == NcSpriteFactory.SHOW_TYPE.ALL || showType == NcSpriteFactory.SHOW_TYPE.ANIMATION))
						{
							spriteList[n].m_nNextSpriteIndex= EditorGUILayout.Popup			("m_nNextSpriteIndex"		, spriteList[n].m_nNextSpriteIndex+1, GetSpriteNodeNames()) - 1;
							if (0 <= spriteList[n].m_nNextSpriteIndex)
							{
								spriteList[n].m_nTestMode		= EditorGUILayout.Popup		("m_nTestMode"				, spriteList[n].m_nTestMode, NgConvert.ContentsToStrings(FxmTestControls.GetHcEffectControls_Trans(FxmTestControls.AXIS.Z)), GUILayout.MaxWidth(Screen.width));
								spriteList[n].m_fTestSpeed		= EditorGUILayout.FloatField("m_fTestSpeed"				, spriteList[n].m_fTestSpeed);

								SetMinValue(ref spriteList[n].m_fTestSpeed, 0.01f);
							}
						}

						if (showType == NcSpriteFactory.SHOW_TYPE.ALL || showType == NcSpriteFactory.SHOW_TYPE.EFFECT)
						{
							EditorGUILayout.Separator();
							// char effect -------------------------------------------------------------
							spriteList[n].m_EffectPrefab	= (GameObject)EditorGUILayout.ObjectField(GetHelpContent("m_EffectPrefab")	, spriteList[n].m_EffectPrefab, typeof(GameObject), false, null);

							rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(m_fButtonHeight*0.7f));
							{
								if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(rect, 3, 0, 1), GetHelpContent("SelEffect"), (m_FxmPopupManager != null)))
									m_FxmPopupManager.ShowSelectPrefabPopup(m_Sel, n, 0, true);
								if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(rect, 3, 1, 1), GetHelpContent("ClearEffect"), (spriteList[n].m_EffectPrefab != null)))
								{
									bClickButton = true;
									spriteList[n].m_EffectPrefab = null;
								}
								if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(rect, 3, 2, 1), GetHelpContent("OpenEffect"), (m_FxmPopupManager != null) && (spriteList[n].m_EffectPrefab != null)))
								{
									bClickButton = true;
									GetFXMakerMain().OpenPrefab(spriteList[n].m_EffectPrefab);
									return;
								}
								GUILayout.Label("");
							}
							EditorGUILayout.EndHorizontal();

							if (spriteList[n].m_EffectPrefab != null)
							{
								// m_nSpriteFactoryIndex
								EditorGUILayout.Space();
								if (spriteList[n].m_EffectPrefab)
								{
									NcSpriteFactory ncSpriteFactory = spriteList[n].m_EffectPrefab.GetComponent<NcSpriteFactory>();
									if (ncSpriteFactory)
									{
										spriteList[n].m_nSpriteFactoryIndex = EditorGUILayout.IntSlider(GetHelpContent("m_nSpriteFactoryIndex"), spriteList[n].m_nSpriteFactoryIndex, -1, ncSpriteFactory.GetSpriteNodeCount()-1);

										string[]	spriteNames = new string[ncSpriteFactory.GetSpriteNodeCount()+1];
										spriteNames[0] = "Default";
										for (int ni = 0; ni < ncSpriteFactory.GetSpriteNodeCount(); ni++)
											spriteNames[ni+1] = ncSpriteFactory.GetSpriteNode(ni).m_SpriteName;
										spriteList[n].m_nSpriteFactoryIndex = EditorGUILayout.Popup("SpriteName", spriteList[n].m_nSpriteFactoryIndex+1, spriteNames)-1;
									} else spriteList[n].m_nSpriteFactoryIndex = -1;
								} else spriteList[n].m_nSpriteFactoryIndex = -1;

								spriteList[n].m_bEffectInstantiate	= EditorGUILayout.Toggle	(GetHelpContent("m_bEffectInstantiate"), spriteList[n].m_bEffectInstantiate);
								
								if (m_Sel.m_SpriteType == NcSpriteFactory.SPRITE_TYPE.NcSpriteAnimation || m_Sel.m_SpriteType == NcSpriteFactory.SPRITE_TYPE.Auto)
								{
									spriteList[n].m_nEffectFrame	= EditorGUILayout.IntSlider		(GetHelpContent("m_nEffectFrame")		, spriteList[n].m_nEffectFrame, 0, spriteList[n].m_nFrameCount, null);

									rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(nLineHeight));
									{
										GUILayout.Label("", GUILayout.Width(nLineHeight));
										GUI.Label(FXMakerLayout.GetInnerHorizontalRect(rect, 4, 0, 2), "bOnlyFirst,bEffDetach");
										spriteList[n].m_bEffectOnlyFirst= EditorGUI.Toggle	(FXMakerLayout.GetInnerHorizontalRect(rect, 4, 2, 1), spriteList[n].m_bEffectOnlyFirst);
										spriteList[n].m_bEffectDetach	= EditorGUI.Toggle	(FXMakerLayout.GetInnerHorizontalRect(rect, 4, 3, 1), spriteList[n].m_bEffectDetach);
									}
									EditorGUILayout.EndHorizontal();
								}
								rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(nLineHeight));
								{
									GUILayout.Label("", GUILayout.Width(nLineHeight));
									GUI.Label(FXMakerLayout.GetInnerHorizontalRect(rect, 4, 0, 2), "fSpeed, fScale");
									spriteList[n].m_fEffectSpeed	= EditorGUI.FloatField	(FXMakerLayout.GetInnerHorizontalRect(rect, 4, 2, 1), spriteList[n].m_fEffectSpeed);
									spriteList[n].m_fEffectScale	= EditorGUI.FloatField	(FXMakerLayout.GetInnerHorizontalRect(rect, 4, 3, 1), spriteList[n].m_fEffectScale);
								}
								EditorGUILayout.EndHorizontal();

								spriteList[n].m_EffectPos			= EditorGUILayout.Vector3Field	("m_EffectPos"			, spriteList[n].m_EffectPos, null);
								spriteList[n].m_EffectRot			= EditorGUILayout.Vector3Field	("m_EffectRot"			, spriteList[n].m_EffectRot, null);

								SetMinValue(ref spriteList[n].m_fEffectScale, 0.001f);
							}

							EditorGUILayout.Space();

							// char sound -------------------------------------------------------------
							spriteList[n].m_AudioClip		= (AudioClip)EditorGUILayout.ObjectField(GetHelpContent("m_AudioClip")		, spriteList[n].m_AudioClip, typeof(AudioClip), false, null);

							rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(m_fButtonHeight*0.7f));
							{
	// 							if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(rect, 2, 0, 1), GetHelpContent("SelAudio"), (m_FxmPopupManager != null)))
	//								m_FxmPopupManager.ShowSelectAudioClipPopup(m_Sel);
								if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(rect, 2, 1, 1), GetHelpContent("ClearAudio"), (spriteList[n].m_AudioClip != null)))
								{
									bClickButton = true;
									spriteList[n].m_AudioClip = null;
								}
								GUILayout.Label("");
							}
							EditorGUILayout.EndHorizontal();

							if (spriteList[n].m_AudioClip != null)
							{
								if (m_Sel.m_SpriteType == NcSpriteFactory.SPRITE_TYPE.NcSpriteAnimation || m_Sel.m_SpriteType == NcSpriteFactory.SPRITE_TYPE.Auto)
								{
									spriteList[n].m_nSoundFrame		= EditorGUILayout.IntSlider	(GetHelpContent("m_nSoundFrame")		, spriteList[n].m_nSoundFrame, 0, spriteList[n].m_nFrameCount, null);
									spriteList[n].m_bSoundOnlyFirst	= EditorGUILayout.Toggle	(GetHelpContent("m_bSoundOnlyFirst")	, spriteList[n].m_bSoundOnlyFirst);
								}
								spriteList[n].m_bSoundLoop			= EditorGUILayout.Toggle	(GetHelpContent("m_bSoundLoop")			, spriteList[n].m_bSoundLoop);
								spriteList[n].m_fSoundVolume		= EditorGUILayout.Slider	(GetHelpContent("m_fSoundVolume")		, spriteList[n].m_fSoundVolume, 0, 1.0f, null);
								spriteList[n].m_fSoundPitch			= EditorGUILayout.Slider	(GetHelpContent("m_fSoundPitch")		, spriteList[n].m_fSoundPitch, -3, 3.0f, null);
							}
						}
					}
				}

				if (EditorGUI.EndChangeCheck())
					nClickIndex = n;

				selTexture = null;
			}

			// Select Node ----------------------------------------------------
			if (0 <= nClickIndex)
			{
				m_Sel.SetSprite(nClickIndex, false);
				if (m_Sel.m_bTestMode && 0 <= spriteList[nClickIndex].m_nTestMode && GetFXMakerMain())
					GetFXMakerMain().GetFXMakerControls().SetTransIndex(spriteList[nClickIndex].m_nTestMode, (4 <= spriteList[nClickIndex].m_nTestMode ? 1.8f : 1.0f), spriteList[nClickIndex].m_fTestSpeed);
				// Rotate
				if (nClickButton == 1)
					m_Sel.transform.Rotate(0, 180, 0);
				nClickIndex		= -1;
				bClickButton	= true;
			}

			m_UndoManager.CheckDirty();
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
	string[] GetSpriteNodeNames()
	{
		List<NcSpriteFactory.NcSpriteNode>	spriteList	= m_Sel.m_SpriteList;
		string[]						retNames	= new string[(spriteList != null ? spriteList.Count : 0) + 1];

		retNames[0] = string.Format("{0} {1}", "X", "None");
		for (int n = 0; n < (spriteList != null ? spriteList.Count : 0); n++)
		{
			if (m_Sel.IsUnused(n) || spriteList[n].IsEmptyTexture())
				continue;

			Texture2D	selTexture = (Texture2D)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(spriteList[n].m_TextureGUID), typeof(Texture2D));
			if (selTexture)
				retNames[n+1] = string.Format("{0} {1}", n, selTexture.name);
		}
		return retNames;
	}

	// ----------------------------------------------------------------------------------
	void CreateSpriteAtlas(Material tarMat)
	{
		if (m_Sel.GetComponent<Renderer>() == null)
		{
			Debug.LogWarning("m_Sel.renderer is nul!!!");
			return;
		}
		if (m_Sel.GetComponent<Renderer>().sharedMaterial == null)
		{
			Debug.LogWarning("m_Sel.renderer.sharedMaterial is nul!!!");
			return;
		}
		if (m_Sel.m_SpriteList == null || m_Sel.m_SpriteList.Count < 1)
			return;

		Texture2D	AtlasTexture;
		AtlasTexture = BuildSpriteAtlas();

		byte[]	bytes		= AtlasTexture.EncodeToPNG();
		string	pathTexture	= (tarMat.mainTexture != null ? AssetDatabase.GetAssetPath(tarMat.mainTexture) : NgFile.TrimFileExt(AssetDatabase.GetAssetPath(tarMat)) + ".png");

		// save texture
		File.WriteAllBytes(pathTexture, bytes);
		Debug.Log(pathTexture);
 		AssetDatabase.Refresh();
		Object.DestroyImmediate(AtlasTexture);
//		ReimportTexture(pathTexture, m_wrapMode, m_filterMode, m_anisoLevel, m_nSpriteTextureSizes[(int)m_fSpriteTextureIndex], m_SpriteTextureFormat[(int)m_fSpriteTextureFormatIdx]);

		// Material
		tarMat.mainTexture = (Texture)AssetDatabase.LoadAssetAtPath(pathTexture, typeof(Texture));
		AssetDatabase.SaveAssets();

		m_Sel.SetSprite(m_Sel.GetCurrentSpriteIndex(), false);
	}

	Texture2D BuildSpriteAtlas()
	{
		List<Texture2D>						frameTextures	= new List<Texture2D>();
		List<Rect>							frameRects		= new List<Rect>();
		bool								bCheckAlpha		= false;
		List<Texture2D>						loadTextures	= new List<Texture2D>();
		List<NcSpriteFactory.NcFrameInfo[]>	loadFrameInfos	= new List<NcSpriteFactory.NcFrameInfo[]>();
		NcSpriteFactory.NcFrameInfo[]		frameInfo;
//		bool			bCheckBlack		= false;

		for (int n = 0; n < m_Sel.m_SpriteList.Count; n++)
		{
			if (m_Sel.IsUnused(n) || m_Sel.m_SpriteList[n].IsEmptyTexture())
				continue;

			bool		bFindAlpha;
			bool		bFindBlack;
			Texture2D	selTexture	= (Texture2D)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(m_Sel.m_SpriteList[n].m_TextureGUID), typeof(Texture2D));
			int			nFindIndex	= loadTextures.IndexOf(selTexture);
			if (0 <= nFindIndex)
			{
				bFindAlpha	= false;
				bFindBlack	= false;
				frameInfo = loadFrameInfos[nFindIndex];
			} else {
				NgAtlas.SetSourceTexture(selTexture);
				frameInfo = NgAtlas.TrimTexture(selTexture, m_Sel.m_bTrimBlack, m_Sel.m_bTrimAlpha, m_Sel.m_SpriteList[n].m_nTilingX, m_Sel.m_SpriteList[n].m_nTilingY, m_Sel.m_SpriteList[n].m_nStartFrame, m_Sel.m_SpriteList[n].m_nSkipFrame, m_Sel.m_SpriteList[n].m_nFrameCount, ref frameTextures, ref frameRects, out bFindAlpha, out bFindBlack);
				loadTextures.Add(selTexture);
				loadFrameInfos.Add(m_Sel.m_SpriteList[n].m_FrameInfos);
			}

			m_Sel.m_SpriteList[n].m_FrameInfos = frameInfo;
			if (bFindAlpha)
				bCheckAlpha = true;
//			if (bFindBlack)
//				bCheckBlack = true;

// 			Debug.Log(m_Sel.m_SpriteList[n].m_FrameInfos[0].m_FrameUvOffset);
		}

		// Resize
		if (m_Sel.m_fSpriteResizeRate < 1)
		{
			for (int nTexIndex = 0; nTexIndex < frameTextures.Count; nTexIndex++)
			{
				frameTextures[nTexIndex] = ScaleTexture(frameTextures[nTexIndex], (int)(frameTextures[nTexIndex].width * m_Sel.m_fSpriteResizeRate), (int)(frameTextures[nTexIndex].height * m_Sel.m_fSpriteResizeRate));
// 				reTex[nTexIndex].Resize((int)(frameRects[nTexIndex].width * m_Sel.m_fSpriteResizeRate), (int)(frameRects[nTexIndex].height * m_Sel.m_fSpriteResizeRate), TextureFormat.ARGB32, false);
// 				reTex[nTexIndex].Apply();
			}
		}


		Color		clearColor		= new Color(0, 0, 0, (bCheckAlpha ? 0 : 1));
		Texture2D	AtlasTexture	= new Texture2D(32, 32, TextureFormat.ARGB32, false);
		Rect[]		uvRects			= AtlasTexture.PackTextures(frameTextures.ToArray(), 2, m_Sel.m_nMaxAtlasTextureSize);

		// clear
		for (int x = 0; x < AtlasTexture.width; x++)
			for (int y = 0; y < AtlasTexture.height; y++)
				AtlasTexture.SetPixel(x, y, clearColor);

		// copy
		for (int n = 0; n < m_Sel.m_SpriteList.Count; n++)
		{
			if (m_Sel.IsUnused(n) || m_Sel.m_SpriteList[n].IsEmptyTexture())
			{
				if (m_Sel.m_SpriteList[n].IsEmptyTexture())
					m_Sel.m_SpriteList[n].SetEmpty();
				continue;
			}

			for (int fc = 0; fc < m_Sel.m_SpriteList[n].m_FrameInfos.Length; fc++)
			{
				int		nFrameIndex = m_Sel.m_SpriteList[n].m_FrameInfos[fc].m_nFrameIndex;
				Rect	imageUvRect	= m_Sel.m_SpriteList[n].m_FrameInfos[fc].m_TextureUvOffset	= uvRects[m_Sel.m_SpriteList[n].m_FrameInfos[fc].m_nFrameIndex];
				// �������� ó��
				Color[]	colBuf = frameTextures[nFrameIndex].GetPixels();
				for (int an = 0; an < colBuf.Length; an++)
					if (m_Sel.m_SpriteList[n].m_fMaxTextureAlpha < colBuf[an].a)
						colBuf[an].a = m_Sel.m_SpriteList[n].m_fMaxTextureAlpha;

				AtlasTexture.SetPixels((int)(imageUvRect.x*AtlasTexture.width), (int)(imageUvRect.y*AtlasTexture.height), (int)(imageUvRect.width*AtlasTexture.width), (int)(imageUvRect.height*AtlasTexture.height), colBuf);
			}
		}
		for (int n = 0; n < frameTextures.Count; n++)
			Object.DestroyImmediate(frameTextures[n]);

		return AtlasTexture;
	}

	private Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
	{
		Texture2D	result	= new Texture2D(targetWidth,targetHeight,source.format,false);
		Color[]		rpixels	= result.GetPixels(0);
		float		incX=(1.0f / (float)targetWidth);
		float		incY=(1.0f / (float)targetHeight); 
		for (int px = 0; px < rpixels.Length; px++)
			rpixels[px] = source.GetPixelBilinear(incX*((float)px%targetWidth), incY*((float)Mathf.Floor(px/(float)targetWidth))); 
		result.SetPixels(rpixels,0); 
		result.Apply(); 
		return result; 
	}

	// ----------------------------------------------------------------------------------
	protected GUIContent GetHelpContent(string tooltip)
	{
		string caption	= tooltip;
		string text		= FXMakerTooltip.GetHsEditor_NcSpriteFactory(tooltip);
		return GetHelpContent(caption, text);
	}

	protected override void HelpBox(string caption)
	{
		string	str	= caption;
		if (caption == "" || caption == "Script")
			str = FXMakerTooltip.GetHsEditor_NcSpriteFactory("");
		base.HelpBox(str);
	}
}
