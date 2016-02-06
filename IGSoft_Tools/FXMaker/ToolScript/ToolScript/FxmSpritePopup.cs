// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

// Attribute ------------------------------------------------------------------------
// Property -------------------------------------------------------------------------
// Loop Function --------------------------------------------------------------------
// Control Function -----------------------------------------------------------------
// Event Function -------------------------------------------------------------------
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class FxmSpritePopup : FxmPopup
{
	// Attribute ------------------------------------------------------------------------
	// const
	protected	const float				m_fMaxCaptureTime		= 5.0f;
	protected	const float				m_fMaxRandomCaptureCount= 32;

	protected	string					m_SelectedPrefabName;

	public		enum CAPTURE_TYPE		{ANIMATION, RANDOM};
	protected	CAPTURE_TYPE			m_CaptureType			= CAPTURE_TYPE.ANIMATION;
	protected	int[]					m_nCaptureSizeValues	= {32, 64, 128, 256, 512, 0};
	protected	int						m_nCaptureSizeIndex		= 2;
	protected	int[]					m_nFrameCountValues		= {30, 25, 20, 15, 10, 5, 3, 2, 1};
	protected	int						m_nFrameCountIndex		= 1;
	protected	int						m_nRandomCaptureCount	= 0;
	protected	float					m_fCaptureTime			= 1;
	[HideInInspector]
	public		bool					m_bCreatePrefab			= true;
	protected	bool					m_bLoop					= false;
	protected	enum SHADER_TYPE		{ADDITIVE, ADDITIVE_MOBILE, ADDITIVE_SOFT, ALPHA_BLENDED, ALPHA_BLENDED_MOBILE};
	protected	SHADER_TYPE				m_ShaderType			= SHADER_TYPE.ADDITIVE;
	protected	bool					m_bFadeIn				= false;
	protected	bool					m_bFadeOut				= false;
	protected	bool					m_bGUITexture			= false;

	// SpriteTexture format
	protected	int[]					m_nSpriteTextureSizes	= {32, 64, 128, 256, 512, 1024, 2048, 0};
	protected	float					m_fSpriteTextureIndex	= 4;

	protected	string[]				m_SpriteTextureFormatName= {"Compressed", "Auto16bit", "AutoTruecolor"};
	protected	TextureImporterFormat[]	m_SpriteTextureFormat	= {TextureImporterFormat.AutomaticCompressed, TextureImporterFormat.Automatic16bit, TextureImporterFormat.AutomaticTruecolor};
	protected	float					m_fSpriteTextureFormatIdx= 1;

	// default
	protected	TextureWrapMode			m_wrapMode				= TextureWrapMode.Clamp;
	protected	FilterMode				m_filterMode			= FilterMode.Bilinear;
	protected	int						m_anisoLevel			= 4;

	// result
	protected	NcSpriteAnimation.PLAYMODE	m_PlayMode			= 0;
	protected	int						m_nSkipFrameCount		= 0;
	protected	int						m_nTotalFrameCount;
	protected	int						m_nSaveFrameCount;
	protected	int						m_nResultFps;
	protected	int						m_nResultCaptureSize;
	protected	int						m_nResultTextureSize;

	// Property -------------------------------------------------------------------------
	public bool ShowPopupWindow()
	{
		m_PopupPosition			= FXMakerLayout.GetGUIMousePosition();
		m_SelectedPrefabName	= FXMakerMain.inst.GetOriginalEffectPrefab().name;
		enabled					= true;

		LoadPrefs();
		base.ShowPopupWindow(null);
		return enabled;
	}

	public override void ClosePopup()
	{
		SavePrefs();
		base.ClosePopup();
	}

	protected void LoadPrefs()
	{
		if (FXMakerLayout.m_bDevelopPrefs == false)
		{
			m_CaptureType				= (CAPTURE_TYPE)EditorPrefs.GetInt	("FxmSpritePopup.m_CaptureType", (int)m_CaptureType);
			m_nCaptureSizeIndex			= EditorPrefs.GetInt	("FxmSpritePopup.m_nCaptureSizeIndex"		, m_nCaptureSizeIndex);
			m_nFrameCountIndex			= EditorPrefs.GetInt	("FxmSpritePopup.m_nFrameCountIndex"		, m_nFrameCountIndex);
			m_nRandomCaptureCount		= EditorPrefs.GetInt	("FxmSpritePopup.m_nRandomCaptureCount"		, m_nRandomCaptureCount);
			m_nSkipFrameCount			= EditorPrefs.GetInt	("FxmSpritePopup.m_nSkipFrameCount"			, m_nSkipFrameCount);
			m_PlayMode					= (NcSpriteAnimation.PLAYMODE)EditorPrefs.GetInt	("FxmSpritePopup.m_PlayMode"	, (int)m_PlayMode);
			m_fCaptureTime				= EditorPrefs.GetFloat	("FxmSpritePopup.m_fCaptureTime"			, m_fCaptureTime);
			m_ShaderType				= (SHADER_TYPE)EditorPrefs.GetInt	("FxmSpritePopup.m_ShaderType"	, (int)m_ShaderType);
			m_bFadeIn					= EditorPrefs.GetBool	("FxmSpritePopup.m_bFadeIn"					, m_bFadeIn);
			m_bFadeOut					= EditorPrefs.GetBool	("FxmSpritePopup.m_bFadeOut"				, m_bFadeOut);
			m_bCreatePrefab				= EditorPrefs.GetBool	("FxmSpritePopup.m_bCreatePrefab"			, m_bCreatePrefab);
			m_bGUITexture				= EditorPrefs.GetBool	("FxmSpritePopup.m_bGUITexture"				, m_bGUITexture);
			m_fSpriteTextureIndex		= EditorPrefs.GetFloat	("FxmSpritePopup.m_fSpriteTextureIndex"		, m_fSpriteTextureIndex);
			m_fSpriteTextureFormatIdx	= EditorPrefs.GetFloat	("FxmSpritePopup.m_fSpriteTextureFormatIdx"	, m_fSpriteTextureFormatIdx);
		}
	}

	protected void SavePrefs()
	{
		// Update Prefs
		EditorPrefs.SetInt		("FxmSpritePopup.m_CaptureType"				, (int)m_CaptureType);
		EditorPrefs.SetInt		("FxmSpritePopup.m_nCaptureSizeIndex"		, m_nCaptureSizeIndex);
		EditorPrefs.SetInt		("FxmSpritePopup.m_nFrameCountIndex"		, m_nFrameCountIndex);
		EditorPrefs.SetInt		("FxmSpritePopup.m_nRandomCaptureCount"		, m_nRandomCaptureCount);
		EditorPrefs.SetInt		("FxmSpritePopup.m_nSkipFrameCount"			, m_nSkipFrameCount);
		EditorPrefs.SetInt		("FxmSpritePopup.m_PlayMode"				, (int)m_PlayMode);
		EditorPrefs.SetFloat	("FxmSpritePopup.m_fCaptureTime"			, m_fCaptureTime);
		EditorPrefs.SetInt		("FxmSpritePopup.m_ShaderType"				, (int)m_ShaderType);
		EditorPrefs.SetBool		("FxmSpritePopup.m_bFadeIn"					, m_bFadeIn);
		EditorPrefs.SetBool		("FxmSpritePopup.m_bFadeOut"				, m_bFadeOut);
		EditorPrefs.SetBool		("FxmSpritePopup.m_bCreatePrefab"			, m_bCreatePrefab);
		EditorPrefs.SetBool		("FxmSpritePopup.m_bGUITexture"				, m_bGUITexture);
		EditorPrefs.SetFloat	("FxmSpritePopup.m_fSpriteTextureIndex"		, m_fSpriteTextureIndex);
		EditorPrefs.SetFloat	("FxmSpritePopup.m_fSpriteTextureFormatIdx"	, m_fSpriteTextureFormatIdx);
	}

	// -------------------------------------------------------------------------------------------
	void Awake()
	{
	}

	void Start()
	{
	}

	void Update()
	{
	}

	public override void OnGUIPopup()
	{
// 		List<int> list = new List<int>();
// 		int nMaxFps = (int)(1 / Time.fixedDeltaTime);
// 		while (0 < nMaxFps)
// 		{
// 			list.Add(nMaxFps);
// 			nMaxFps = nMaxFps / 2;
// 		}
// 		list.Add(0);
// 		m_nFrameCountValues = list.ToArray();
// 		if (m_nFrameCountValues.Length <= m_nFrameCountIndex)
// 			m_nFrameCountIndex = m_nFrameCountValues.Length-1;

		// Popup Window ---------------------------------------------------------
		FXMakerMain.inst.PopupFocusWindow(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.SPRITEPOPUP), GetPopupRect(), winPopup, "Build Sprite");

		NgGUIDraw.DrawBox(FXMakerLayout.GetOffsetRect(GetSpriteCaptureRect(), 5), new Color(0.8f, 0, 0), 2, false);
	}

	// ==========================================================================================================
	Rect GetScrollbarRect(Rect baseRect)
	{
		return FXMakerLayout.GetOffsetRect(baseRect, 0, 1, 0, -2);
	}

	Rect GetToggleRect(Rect baseRect)
	{
		return FXMakerLayout.GetOffsetRect(baseRect, 0, 1, 0, -1);
	}

	float GetFrameTime()
	{
		return 1.0f / m_nFrameCountValues[m_nFrameCountIndex];
	}

	void winPopup(int id)
	{
		Rect		baseRect	= GetPopupRect();
		Rect		lineRect;
		int			nMargin		= 2;
		int			nTopHeight	= 1;
		int			nLineCount	= 25;
		int			nIncLine	= 0;
		string		str;

// 		if (UnfocusClose(baseRect, 5, 0, 0, 0))
// 			return;

		baseRect = FXMakerLayout.GetChildVerticalRect(baseRect, nTopHeight, 1, 0, 1);
		baseRect = FXMakerLayout.GetOffsetRect(baseRect, -nMargin);

		// Capture
		lineRect = FXMakerLayout.GetInnerVerticalRect(baseRect, nLineCount, nIncLine++, 1);
		NgGUIDraw.DrawHorizontalLine(new Vector2(lineRect.x, lineRect.y+lineRect.height-1), (int)lineRect.width, Color.grey, 2, false);
 		GUI.Label(FXMakerLayout.GetInnerHorizontalRect(lineRect, 10, 0, 3), "Capture");

		// m_CaptureType
		lineRect = FXMakerLayout.GetInnerVerticalRect(baseRect, nLineCount, nIncLine++, 1);
		FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(lineRect, 10, 0, 3), GetHelpContent("CaptureType"), false);
		m_CaptureType = FXMakerLayout.GUIToggle(FXMakerLayout.GetInnerHorizontalRect(lineRect, 20, 7, 6), m_CaptureType==CAPTURE_TYPE.ANIMATION, new GUIContent("Animation"), true) ? CAPTURE_TYPE.ANIMATION : m_CaptureType;
		m_CaptureType = FXMakerLayout.GUIToggle(FXMakerLayout.GetInnerHorizontalRect(lineRect, 20, 13, 6), m_CaptureType==CAPTURE_TYPE.RANDOM, new GUIContent("Random"), true) ? CAPTURE_TYPE.RANDOM : m_CaptureType;

		// size 32, 64, 128, 256, 512
		lineRect = FXMakerLayout.GetInnerVerticalRect(baseRect, nLineCount, nIncLine++, 1);
		FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(lineRect, 10, 0, 3), GetHelpContent("Size"), false);
		m_nCaptureSizeIndex	= (int)GUI.HorizontalScrollbar(GetScrollbarRect(FXMakerLayout.GetInnerHorizontalRect(lineRect, 10, 3, 5)), m_nCaptureSizeIndex, 1, 0, m_nCaptureSizeValues.Length-1);
		FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(lineRect, 10, 8, 2), m_nCaptureSizeValues[m_nCaptureSizeIndex].ToString(), false);

		if (m_CaptureType == CAPTURE_TYPE.ANIMATION)
		{
			// frame
			lineRect = FXMakerLayout.GetInnerVerticalRect(baseRect, nLineCount, nIncLine++, 1);
			FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(lineRect, 10, 0, 3), GetHelpContent("Fps"), false);
			m_nFrameCountIndex	= (int)GUI.HorizontalScrollbar(GetScrollbarRect(FXMakerLayout.GetInnerHorizontalRect(lineRect, 10, 3, 5)), m_nFrameCountIndex, 1, 0, m_nFrameCountValues.Length);
			FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(lineRect, 10, 8, 2), m_nFrameCountValues[m_nFrameCountIndex].ToString(), false);

			// Get Time
			lineRect = FXMakerLayout.GetInnerVerticalRect(baseRect, nLineCount, nIncLine++, 1);
 			if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(lineRect, 1, 0, 1), "Get Duration = " + FXMakerMain.inst.GetFXMakerControls().GetLastAutoRetTime().ToString("0.00"), true))
				m_fCaptureTime = System.Convert.ToSingle(FXMakerMain.inst.GetFXMakerControls().GetLastAutoRetTime().ToString("0.00"));

			// Rescale Time
			lineRect = FXMakerLayout.GetInnerVerticalRect(baseRect, nLineCount, nIncLine++, 1);
			FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(lineRect, 10, 0, 3), GetHelpContent("Time"), false);
			m_fCaptureTime	= GUI.HorizontalScrollbar(GetScrollbarRect(FXMakerLayout.GetInnerHorizontalRect(lineRect, 10, 3, 5)), m_fCaptureTime, GetFrameTime(), GetFrameTime(), m_fMaxCaptureTime);
			str = FXMakerLayout.GUITextField(FXMakerLayout.GetInnerHorizontalRect(lineRect, 10, 8, 2), m_fCaptureTime.ToString(), true).Trim();
			m_fCaptureTime	= NgConvert.ToFloat(str, m_fCaptureTime);

			// SkipFrameCount
			lineRect = FXMakerLayout.GetInnerVerticalRect(baseRect, nLineCount, nIncLine++, 1);
			FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(lineRect, 10, 0, 3), GetHelpContent("SkipFrame"), false);
			if (m_nTotalFrameCount <= m_nSkipFrameCount)
				m_nSkipFrameCount = m_nTotalFrameCount - 1;
			if (m_nTotalFrameCount <= 1)
				m_nSkipFrameCount = 0;
			else m_nSkipFrameCount	= (int)GUI.HorizontalScrollbar(GetScrollbarRect(FXMakerLayout.GetInnerHorizontalRect(lineRect, 10, 3, 5)), m_nSkipFrameCount, 1, 0, m_nTotalFrameCount);
			FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(lineRect, 10, 8, 2), m_nSkipFrameCount.ToString(), false);

			// total
			m_nTotalFrameCount	= (int)(m_fCaptureTime / GetFrameTime()) + 1;
			m_nSaveFrameCount	= (m_nTotalFrameCount - m_nSkipFrameCount);
			m_nResultFps		= m_nFrameCountValues[m_nFrameCountIndex];
		}
		if (m_CaptureType == CAPTURE_TYPE.RANDOM)
		{
			// m_nRandomCaptureCount
			lineRect = FXMakerLayout.GetInnerVerticalRect(baseRect, nLineCount, nIncLine++, 1);
			FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(lineRect, 10, 0, 3), GetHelpContent("CaptureCount"), false);
			m_nRandomCaptureCount	= (int)GUI.HorizontalScrollbar(GetScrollbarRect(FXMakerLayout.GetInnerHorizontalRect(lineRect, 10, 3, 5)), m_nRandomCaptureCount, 1, 1, m_fMaxRandomCaptureCount);
			FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(lineRect, 10, 8, 2), m_nRandomCaptureCount.ToString(), false);

			// SkipTime
			lineRect = FXMakerLayout.GetInnerVerticalRect(baseRect, nLineCount, nIncLine++, 1);
			FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(lineRect, 10, 0, 3), GetHelpContent("SkipTime"), false);
			m_fCaptureTime	= GUI.HorizontalScrollbar(GetScrollbarRect(FXMakerLayout.GetInnerHorizontalRect(lineRect, 10, 3, 5)), m_fCaptureTime, 0.01f, 0, m_fMaxCaptureTime);
			str = FXMakerLayout.GUITextField(FXMakerLayout.GetInnerHorizontalRect(lineRect, 10, 8, 2), m_fCaptureTime.ToString(), true).Trim();
			m_fCaptureTime	= NgConvert.ToFloat(str, m_fCaptureTime);

			nIncLine += 2;

			// total
			m_nSkipFrameCount	= 0;
			m_nTotalFrameCount	= m_nRandomCaptureCount;
			m_nSaveFrameCount	= m_nRandomCaptureCount;
			m_nResultFps		= m_nFrameCountValues[m_nFrameCountIndex];
		}

		// total frame
		lineRect = FXMakerLayout.GetInnerVerticalRect(baseRect, nLineCount, nIncLine++, 1);
		FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(lineRect, 10, 0, 3), GetHelpContent("TotalFrame"), false);
		FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(lineRect, 10, 3, 7), (m_nTotalFrameCount - m_nSkipFrameCount).ToString(), false);

		// m_nResultTextureSize
		m_nResultCaptureSize = m_nCaptureSizeValues[(int)m_nCaptureSizeIndex];
		m_nResultTextureSize = NgAtlas.GetTextureSize(m_nSaveFrameCount, m_nResultCaptureSize);
		lineRect = FXMakerLayout.GetInnerVerticalRect(baseRect, nLineCount, nIncLine++, 1);
		FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(lineRect, 10, 0, 3), GetHelpContent("TextureSize"), false);
		FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(lineRect, 10, 3, 7), m_nResultTextureSize.ToString() + " x " + (m_nSaveFrameCount <= (m_nResultTextureSize / m_nResultCaptureSize)*(m_nResultTextureSize / m_nResultCaptureSize)/2 ? m_nResultTextureSize/2 : m_nResultTextureSize).ToString(), false);

		// -------------------------------------------------------------------------------------------------------------------------
		// Output
		lineRect = FXMakerLayout.GetInnerVerticalRect(baseRect, nLineCount, nIncLine++, 1);
		NgGUIDraw.DrawHorizontalLine(new Vector2(lineRect.x, lineRect.y+lineRect.height-1), (int)lineRect.width, Color.grey, 2, false);
 		GUI.Label(FXMakerLayout.GetInnerHorizontalRect(lineRect, 10, 0, 3), "Output");

		// bCreatePrefab
		lineRect = FXMakerLayout.GetInnerVerticalRect(baseRect, nLineCount, nIncLine++, 1);
		FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(lineRect, 10, 0, 3), GetHelpContent("Create"), false);
		m_bCreatePrefab = FXMakerLayout.GUIToggle(FXMakerLayout.GetInnerHorizontalRect(lineRect, 20, 7, 6), m_bCreatePrefab, new GUIContent("Prefab"), true);
		m_bCreatePrefab = !FXMakerLayout.GUIToggle(FXMakerLayout.GetInnerHorizontalRect(lineRect, 20, 13, 6), !m_bCreatePrefab, new GUIContent("Texture"), true);

		// PlayMode
		lineRect = FXMakerLayout.GetInnerVerticalRect(baseRect, nLineCount, nIncLine++, 1);
		FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(lineRect, 10, 0, 3), GetHelpContent("PlayMode"), false);
		if (FXMakerLayout.GUIToggle(FXMakerLayout.GetInnerHorizontalRect(lineRect, 20,  7, 6), (m_bCreatePrefab && m_PlayMode==NcSpriteAnimation.PLAYMODE.DEFAULT) , new GUIContent("Default"), m_bCreatePrefab))
			m_PlayMode = NcSpriteAnimation.PLAYMODE.DEFAULT;
		if (FXMakerLayout.GUIToggle(FXMakerLayout.GetInnerHorizontalRect(lineRect, 20, 13, 6), (m_bCreatePrefab && m_PlayMode==NcSpriteAnimation.PLAYMODE.INVERSE) , new GUIContent("Inverse"), m_bCreatePrefab))
			m_PlayMode = NcSpriteAnimation.PLAYMODE.INVERSE;
		lineRect = FXMakerLayout.GetInnerVerticalRect(baseRect, nLineCount, nIncLine++, 1);
		if (FXMakerLayout.GUIToggle(FXMakerLayout.GetInnerHorizontalRect(lineRect, 20, 7, 6), (m_bCreatePrefab && m_PlayMode==NcSpriteAnimation.PLAYMODE.PINGPONG), new GUIContent("PingPong"), m_bCreatePrefab))
			m_PlayMode = NcSpriteAnimation.PLAYMODE.PINGPONG;
		if (FXMakerLayout.GUIToggle(FXMakerLayout.GetInnerHorizontalRect(lineRect, 20, 13, 6), (m_bCreatePrefab && m_PlayMode==NcSpriteAnimation.PLAYMODE.RANDOM), new GUIContent("Random"), m_bCreatePrefab))
			m_PlayMode = NcSpriteAnimation.PLAYMODE.RANDOM;

		// Loop
		lineRect = FXMakerLayout.GetInnerVerticalRect(baseRect, nLineCount, nIncLine++, 1);
		FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(lineRect, 10, 0, 3), GetHelpContent("Loop"), false);
		if (m_bCreatePrefab)
		{
			m_bLoop = !FXMakerLayout.GUIToggle(FXMakerLayout.GetInnerHorizontalRect(lineRect, 20, 7, 6), (m_bCreatePrefab && !m_bLoop), new GUIContent("Once"), m_bCreatePrefab);
			m_bLoop =  FXMakerLayout.GUIToggle(FXMakerLayout.GetInnerHorizontalRect(lineRect, 20,13, 6), (m_bCreatePrefab &&  m_bLoop), new GUIContent("Loop"), m_bCreatePrefab);
		} else {
			FXMakerLayout.GUIToggle(FXMakerLayout.GetInnerHorizontalRect(lineRect, 20, 7, 6), (m_bCreatePrefab &&  m_bLoop), new GUIContent("Once"), m_bCreatePrefab);
			FXMakerLayout.GUIToggle(FXMakerLayout.GetInnerHorizontalRect(lineRect, 20,13, 6), (m_bCreatePrefab && !m_bLoop), new GUIContent("Loop"), m_bCreatePrefab);
		}

		// Shader
		lineRect = FXMakerLayout.GetInnerVerticalRect(baseRect, nLineCount, nIncLine++, 1);
		FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(lineRect, 10, 0, 3), GetHelpContent("Shader"), false);
		if (m_bCreatePrefab)
		{
			m_ShaderType = FXMakerLayout.GUIToggle(FXMakerLayout.GetInnerHorizontalRect(lineRect, 20, 7, 7), (m_bCreatePrefab && m_ShaderType==SHADER_TYPE.ADDITIVE), new GUIContent("Additive"), m_bCreatePrefab) ? SHADER_TYPE.ADDITIVE : m_ShaderType;
			m_ShaderType = FXMakerLayout.GUIToggle(FXMakerLayout.GetInnerHorizontalRect(lineRect, 20,13, 7), (m_bCreatePrefab && m_ShaderType==SHADER_TYPE.ADDITIVE_MOBILE), new GUIContent("AdditiveMobile"), m_bCreatePrefab) ? SHADER_TYPE.ADDITIVE_MOBILE : m_ShaderType;
			lineRect	 = FXMakerLayout.GetInnerVerticalRect(baseRect, nLineCount, nIncLine++, 1);
			m_ShaderType = FXMakerLayout.GUIToggle(FXMakerLayout.GetInnerHorizontalRect(lineRect, 20, 7, 7), (m_bCreatePrefab && m_ShaderType==SHADER_TYPE.ADDITIVE_SOFT), new GUIContent("AdditiveSoft"), m_bCreatePrefab) ? SHADER_TYPE.ADDITIVE_SOFT : m_ShaderType;
			m_ShaderType = FXMakerLayout.GUIToggle(FXMakerLayout.GetInnerHorizontalRect(lineRect, 20,13, 7), (m_bCreatePrefab && m_ShaderType==SHADER_TYPE.ALPHA_BLENDED), new GUIContent("AlphaBlended"), m_bCreatePrefab) ? SHADER_TYPE.ALPHA_BLENDED : m_ShaderType;
			lineRect	 = FXMakerLayout.GetInnerVerticalRect(baseRect, nLineCount, nIncLine++, 1);
			m_ShaderType = FXMakerLayout.GUIToggle(FXMakerLayout.GetInnerHorizontalRect(lineRect, 20, 7,13), (m_bCreatePrefab && m_ShaderType==SHADER_TYPE.ALPHA_BLENDED_MOBILE), new GUIContent("AlphaBlendedMobile"), m_bCreatePrefab) ? SHADER_TYPE.ALPHA_BLENDED_MOBILE : m_ShaderType;
		} else {
			FXMakerLayout.GUIToggle(FXMakerLayout.GetInnerHorizontalRect(lineRect, 20, 7, 5), (m_bCreatePrefab && m_ShaderType==SHADER_TYPE.ADDITIVE), new GUIContent("Additive"), m_bCreatePrefab);
			FXMakerLayout.GUIToggle(FXMakerLayout.GetInnerHorizontalRect(lineRect, 20,12, 4), (m_bCreatePrefab && m_ShaderType==SHADER_TYPE.ADDITIVE_MOBILE), new GUIContent("Mobile"), m_bCreatePrefab);
			FXMakerLayout.GUIToggle(FXMakerLayout.GetInnerHorizontalRect(lineRect, 20,17, 3), (m_bCreatePrefab && m_ShaderType==SHADER_TYPE.ADDITIVE_SOFT), new GUIContent("Soft"), m_bCreatePrefab);
			lineRect	 = FXMakerLayout.GetInnerVerticalRect(baseRect, nLineCount, nIncLine++, 1);
			FXMakerLayout.GUIToggle(FXMakerLayout.GetInnerHorizontalRect(lineRect, 20,  7, 7), (m_bCreatePrefab && m_ShaderType==SHADER_TYPE.ALPHA_BLENDED), new GUIContent("AlphaBlended"), m_bCreatePrefab);
			FXMakerLayout.GUIToggle(FXMakerLayout.GetInnerHorizontalRect(lineRect, 20, 14, 7), (m_bCreatePrefab && m_ShaderType==SHADER_TYPE.ALPHA_BLENDED_MOBILE), new GUIContent("AlphaBlendedMobile"), m_bCreatePrefab);
		}

		// Fade
		lineRect	= FXMakerLayout.GetInnerVerticalRect(baseRect, nLineCount, nIncLine++, 1);
		bool bFade	= (m_bCreatePrefab && m_ShaderType==SHADER_TYPE.ADDITIVE || m_bCreatePrefab && m_ShaderType==SHADER_TYPE.ALPHA_BLENDED);
		FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(lineRect, 10, 0, 3), GetHelpContent("Fade"), false);
		if (bFade)
		{
			m_bFadeIn	= FXMakerLayout.GUIToggle(FXMakerLayout.GetInnerHorizontalRect(lineRect, 20, 7, 6), m_bFadeIn , new GUIContent("FadeIn") , bFade);
			m_bFadeOut	= FXMakerLayout.GUIToggle(FXMakerLayout.GetInnerHorizontalRect(lineRect, 20,13, 6), m_bFadeOut, new GUIContent("FadeOut"), bFade);
		} else {
			FXMakerLayout.GUIToggle(FXMakerLayout.GetInnerHorizontalRect(lineRect, 20, 7, 6), false, new GUIContent("FadeIn") , bFade);
			FXMakerLayout.GUIToggle(FXMakerLayout.GetInnerHorizontalRect(lineRect, 20,13, 6), false, new GUIContent("FadeOut"), bFade);
		}

		// Texture Format ---------------------------------------------------------------------------------------
		// TextureType
		lineRect = FXMakerLayout.GetInnerVerticalRect(baseRect, nLineCount, nIncLine++, 1);
		FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(lineRect, 10, 0, 3), GetHelpContent("TextureType"), false);
		m_bGUITexture = FXMakerLayout.GUIToggle(GetToggleRect(FXMakerLayout.GetInnerHorizontalRect(lineRect, 20, 7, 6)), m_bGUITexture, new GUIContent("GUI"), true);
		m_bGUITexture = !FXMakerLayout.GUIToggle(GetToggleRect(FXMakerLayout.GetInnerHorizontalRect(lineRect, 20, 13, 6)), !m_bGUITexture, new GUIContent("Texture"), true);

		// AnisoLevel
		lineRect = FXMakerLayout.GetInnerVerticalRect(baseRect, nLineCount, nIncLine++, 1);
		FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(lineRect, 10, 0, 3), GetHelpContent("AnisoLevel"), false);
		if (!m_bGUITexture)
		{
			m_anisoLevel	= (int)GUI.HorizontalScrollbar(GetScrollbarRect(FXMakerLayout.GetInnerHorizontalRect(lineRect, 10, 3, 5)), m_anisoLevel, 1, 0, 9+1);
			FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(lineRect, 10, 8, 2), m_anisoLevel.ToString(), false);
		}

		// m_nSpriteTextureSizes
		lineRect = FXMakerLayout.GetInnerVerticalRect(baseRect, nLineCount, nIncLine++, 1);
		FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(lineRect, 10, 0, 3), GetHelpContent("MaxSize"), false);
		m_fSpriteTextureIndex	= GUI.HorizontalScrollbar(GetScrollbarRect(FXMakerLayout.GetInnerHorizontalRect(lineRect, 10, 3, 5)), m_fSpriteTextureIndex, 1, 0, m_nSpriteTextureSizes.Length-1);
		FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(lineRect, 10, 8, 2), m_nSpriteTextureSizes[(int)m_fSpriteTextureIndex].ToString(), false);

		// m_nSpriteTextureFormat
		lineRect = FXMakerLayout.GetInnerVerticalRect(baseRect, nLineCount, nIncLine++, 1);
 		FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(lineRect, 10, 0, 3), GetHelpContent("Format"), false);
		lineRect = FXMakerLayout.GetInnerVerticalRect(baseRect, nLineCount, nIncLine++, 1);
		for (int n = 0; n < m_SpriteTextureFormatName.Length; n++)
		{
			Rect	toggleRect	= GetToggleRect(FXMakerLayout.GetInnerHorizontalRect(lineRect, 3, n, 1));
			if (n == 2)
			{
				toggleRect.x -= 15;
				toggleRect.width += 15;
			}
			bool	bCheck		= (GUI.Toggle(toggleRect, (m_fSpriteTextureFormatIdx == n), m_SpriteTextureFormatName[n]));
			if (bCheck && m_fSpriteTextureFormatIdx != n)
			{
				m_fSpriteTextureFormatIdx = n;
// 				EditorPrefs.SetInt("FxmSpritePopup.m_nShowGameObjectOptionIndex", m_nShowGameObjectOptionIndex);
			}
		}


		nIncLine++;
		lineRect = FXMakerLayout.GetInnerVerticalRect(baseRect, nLineCount, nIncLine++, 1);
		lineRect = FXMakerLayout.GetOffsetRect(lineRect, 0, -12, 0, 0);

		if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(lineRect, 2, 0, 1), "Build Sprite", (m_nResultTextureSize <= 2048)))
		{
 			FXMakerCapture.StartSpriteCapture(m_CaptureType, m_nTotalFrameCount, (m_CaptureType == CAPTURE_TYPE.ANIMATION ? GetFrameTime() : m_fCaptureTime), GetSpriteCaptureRect());
			ClosePopup();
		}
		if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(lineRect, 2, 1, 1), "Close", true))
		{
			ClosePopup();
		}

		FXMakerMain.inst.SaveTooltip();
	}

	// ----------------------------------------------------------------------------------------------------------
	public override Rect GetPopupRect()
	{
		return GetPopupRectLeftBottom(310, 460);
	}

	// Control Function -----------------------------------------------------------------
	float GetMultipleValue(float val, float unit)
	{
		int nUnit	= System.Convert.ToInt32(unit*100);
		int nVal	= System.Convert.ToInt32(val*100);
		int next	= nUnit;
		while (next <= nVal)
			next += nUnit;
		return (next - nUnit) / 100.0f;
	}

	public Rect GetSpriteCaptureRect()
	{
		// 하면 가운데
		int		width	= m_nResultCaptureSize;
		int		height	= m_nResultCaptureSize;
		int		x		= (Screen.width - width) / 2;
		int		y		= (Screen.height - height) / 2; // - (int)(Screen.height * 0.1f);
		return new Rect(x, y, width, height);
	}

	// Event Function -------------------------------------------------------------------

	// -------------------------------------------------------------------------------------------
	GUIContent GetHelpContent(string text)
	{
//		return FXMakertip.GetGUIContentNoTooltip(text);
 		return FXMakerTooltip.GetHcPopup_Sprite(text);
	}

	// ---------------------------------------------------------------------
	public string EndCapture(Texture2D[] SpriteTextures)
	{
#if UNITY_WEBPLAYER
		Debug.LogError("In WEB_PLAYER mode, you cannot run the FXMaker.");
		Debug.Break();
		return null;
#else
		int			nTexSize		= m_nResultTextureSize;
		int			nCapSize		= m_nResultCaptureSize;
		int			nMaxCount		= (nTexSize / nCapSize) * (nTexSize / nCapSize);
		int			nTexHeight		= (m_nSaveFrameCount <= nMaxCount / 2 ? nTexSize / 2 : nTexSize);
		Texture2D	spriteTexture	= new Texture2D(nTexSize, nTexHeight, TextureFormat.ARGB32, false);
		int			nSaveCount		= 0;

		for (int x = 0; x < spriteTexture.width; x++)
			for (int y = 0; y < spriteTexture.height; y++)
				spriteTexture.SetPixel(x, y, Color.black);

		for (int n = m_nSkipFrameCount; n < m_nTotalFrameCount; n++, nSaveCount++)
		{
			Color[]	srcColors = SpriteTextures[n].GetPixels(0);

			if (m_ShaderType == SHADER_TYPE.ALPHA_BLENDED || m_ShaderType == SHADER_TYPE.ALPHA_BLENDED_MOBILE)
				srcColors = NgAtlas.ConvertAlphaTexture(srcColors, true, FXMakerOption.inst.m_AlphaWeightCurve, 1, 1, 1);
			spriteTexture.SetPixels(((nSaveCount) % (nTexSize/nCapSize)) * nCapSize, nTexHeight - (((nSaveCount) / (nTexSize/nCapSize) + 1) * nCapSize), nCapSize, nCapSize, srcColors);
			Object.DestroyImmediate(SpriteTextures[n]);
		}

		byte[]	bytes		= spriteTexture.EncodeToPNG();
		string	texBasePath;
		if (FXMakerLayout.m_bDevelopState)
			 texBasePath = FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.SPRITE_TOOL);
		else texBasePath = FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.SPRITE_USER);
		string	pathTexture	= NgTexture.UniqueTexturePath(texBasePath, m_SelectedPrefabName);

		// save texture
		File.WriteAllBytes(pathTexture, bytes);
 		AssetDatabase.Refresh();
		NgTexture.ReimportTexture(pathTexture, m_bGUITexture, m_wrapMode, m_filterMode, m_anisoLevel, m_nSpriteTextureSizes[(int)m_fSpriteTextureIndex], m_SpriteTextureFormat[(int)m_fSpriteTextureFormatIdx]);
		Object.DestroyImmediate(spriteTexture);

		// Create Prefab
		if (m_bCreatePrefab)
		{
			string		pathMaterial	= CreateMaterial(pathTexture);
			Material	newMat			= (Material)AssetDatabase.LoadAssetAtPath(pathMaterial, typeof(Material));
			GameObject	newPrefab		= (GameObject)Instantiate(FXMakerMain.inst.m_FXMakerSpritePrefab);

			newPrefab.transform.rotation= Quaternion.identity;
 			newPrefab.GetComponent<Renderer>().material	= newMat;
 			NcSpriteAnimation spriteCom = newPrefab.GetComponent<NcSpriteAnimation>();
			spriteCom.m_bBuildSpriteObj	= true;
			spriteCom.m_nFrameCount		= m_nSaveFrameCount;
			spriteCom.m_fFps			= m_nResultFps;
			spriteCom.m_nTilingX		= m_nResultTextureSize / m_nResultCaptureSize;
			spriteCom.m_nTilingY		= (m_nSaveFrameCount <= spriteCom.m_nTilingX*spriteCom.m_nTilingX/2 ? spriteCom.m_nTilingX/2 : spriteCom.m_nTilingX);
			spriteCom.m_PlayMode		= m_PlayMode;
			spriteCom.m_bLoop			= m_bLoop;
			spriteCom.m_nLoopStartFrame	= 0;
			spriteCom.m_nLoopFrameCount	= spriteCom.m_nFrameCount;
			spriteCom.m_nLoopingCount	= 0;

			spriteCom.m_bAutoDestruct	= !m_bLoop;

 			NcCurveAnimation curveCom = newPrefab.GetComponent<NcCurveAnimation>();
			if (curveCom.GetCurveInfoCount()  != 3)
			{
				Debug.LogError("FXMakerMain.inst.m_FxmSpritePrefab : curveCom Count Error!!!");
			}

			curveCom.GetCurveInfo(0).m_bEnabled = false;		// both
			curveCom.GetCurveInfo(1).m_bEnabled = false;		// fadein
			curveCom.GetCurveInfo(2).m_bEnabled = false;		// fadeout
			curveCom.m_bAutoDestruct = false;
			curveCom.m_fDurationTime = spriteCom.GetDurationTime();

			if (m_bFadeIn && m_bFadeOut)
			{
				curveCom.GetCurveInfo(0).m_bEnabled = true;
			} else {
				if (m_bFadeIn)
					curveCom.GetCurveInfo(1).m_bEnabled = true;
				if (m_bFadeOut)
					curveCom.GetCurveInfo(2).m_bEnabled = true;
			}

			string		basePath		= AssetDatabase.GetAssetPath(FXMakerMain.inst.GetOriginalEffectPrefab());
			string		prefabPath		= UniquePrefabPath(NgFile.TrimFilenameExt(basePath));
 			GameObject	createPrefab	= PrefabUtility.CreatePrefab(prefabPath, newPrefab);
			Destroy(newPrefab);

			// Create Thumb
			CreateSpriteThumb(createPrefab, pathTexture);

			AssetDatabase.SaveAssets();

			return prefabPath;
		} else return pathTexture;
#endif
	}

	string UniquePrefabPath(string basePath)
	{
		// Unique Name
		string		texname			= m_SelectedPrefabName;
		int			nLoopCount		= 0;
		int			nUniqueCount	= 0;
		string		uniquePath;
		Object		existsObj;

		while (true)
		{
			string matName = texname + (0<nUniqueCount ? "_"+nUniqueCount.ToString() : "") + ".prefab";
			uniquePath	= NgFile.CombinePath(basePath, matName);
			existsObj = AssetDatabase.LoadAssetAtPath(uniquePath, typeof(GameObject));
			if (existsObj == null)
				break;
			nLoopCount++;
			nUniqueCount++;
			if (999 < nUniqueCount)
				nUniqueCount = 1;
			if (999 < nLoopCount)
			{
				Debug.LogError("Over Loop ----------------------");
				return "";
			}
		}
		return uniquePath;
	}

	string CreateMaterial(string pathTexture)
	{
		Material	newMat;
		
		// Shader
		switch (m_ShaderType)
		{
			case SHADER_TYPE.ADDITIVE:				newMat = new Material(Shader.Find("Particles/Additive"));				break;
			case SHADER_TYPE.ADDITIVE_MOBILE:		newMat = new Material(Shader.Find("Mobile/Particles/Additive"));		break;
			case SHADER_TYPE.ADDITIVE_SOFT:			newMat = new Material(Shader.Find("Particles/Additive (Soft)"));		break;
			case SHADER_TYPE.ALPHA_BLENDED:			newMat = new Material(Shader.Find("Particles/Alpha Blended"));			break;
			case SHADER_TYPE.ALPHA_BLENDED_MOBILE:	newMat = new Material(Shader.Find("Mobile/Particles/Alpha Blended"));	break;
			default : 
				{
					Debug.LogError("Missing m_ShaderType");
					return "";
				}
		}
		newMat.mainTexture	= (Texture)AssetDatabase.LoadAssetAtPath(pathTexture, typeof(Texture));
		return NgMaterial.SaveMaterial(newMat, NgFile.TrimFilenameExt(pathTexture), NgFile.GetFilename(pathTexture), FXMakerLayout.m_bDevelopState);
	}

	void CreateSpriteThumb(GameObject createPrefab, string pathTexture)
	{
		string filename	= NgFile.CombinePath(FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.TEMPPREVIEWTEXTURE), NgAsset.GetPrefabThumbFilename(createPrefab));

		if (AssetDatabase.CopyAsset(pathTexture, filename))
			NgAsset.CaptureResize(filename);
	}
}
#endif
