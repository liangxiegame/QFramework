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
using System.Collections;
using System.IO;
using System.Timers;

public class FxmFolderPopup_Texture : FxmFolderPopup
{
	// Attribute ------------------------------------------------------------------------
	// const
	protected		int					m_nMaterialBottomHeight		= 96;
	protected		int					m_nPaletteBottomHeight		= 80;

	public			string				m_DefaultGroupName			= "[Texture]";
	public			Texture2D			m_PalettePanelImage;
	public			Texture2D			m_PaletteBarColorImage;
	public			Texture2D			m_PaletteBarGrayImage;

	// popup
	protected		int					m_nSelRendererMatIndex;
	protected		Material			m_OriMaterial;
	protected		GUIContent			m_OriObjectContent;
	protected		Material			m_SelectedMaterial;
	protected		Material[]			m_SelectedTextureMaterials;
	protected		bool				m_bUniqueSave;

	protected		int					m_nSelMainTextureIndex;
	protected		int					m_nSelMaskTextureIndex;

	protected		Material[]			m_CurrentTextureMaterials;
	protected		int					m_nCurrentTextureMaterialIndex;
	protected		GUIContent[]		m_CurrentTextureMaterialContents;
	protected		Vector2				m_CurrentTextureMaterialListScrollPos;

	protected		Material[]			m_NewShaderMaterials;
	protected		int					m_nNewShaderMaterialIndex;
	protected		GUIContent[]		m_NewShaderMaterialContents;
	protected		Vector2				m_NewShaderMaterialListScrollPos;

	protected		float				m_fPaletteBarColorValue;
	protected		float				m_fPaletteBarGrayValue;
	protected		bool				m_bEnterPalette;

	protected		int					m_nTempLoadFrameIndex;
	protected		int					m_nTempSaveFrameIndex;
	protected		int					m_nPreviewMaterialWidth;

	// Property -------------------------------------------------------------------------
	public bool ShowPopupWindow(Transform selTrans, int nSelMatIndex)
	{
		m_SelectedTransform		= selTrans;
		m_nSelRendererMatIndex	= nSelMatIndex;
		return ShowPopupWindow(selTrans.GetComponent<Renderer>().sharedMaterials[nSelMatIndex], false);
	}

	public void ChangeMaterialColor(Transform selTrans, int nSelMatIndex, Color color)
	{
		m_SelectedTransform		= selTrans;
		m_nSelRendererMatIndex	= nSelMatIndex;
		ShowPopupWindow(selTrans.GetComponent<Renderer>().sharedMaterials[nSelMatIndex], false);
		SetActiveMaterialColor(color);
		ClosePopup(true);
	}

	public override bool ShowPopupWindow(Object selObj, bool bSaveDialog)
	{
		Material	currentObj = selObj as Material;

		// No Shader Selected
		if (currentObj != null && (currentObj.shader == null || currentObj.shader.name == ""))
			currentObj = null;

		// init
		m_OriMaterial					= currentObj;
		m_OriObjectContent				= (currentObj == null ? new GUIContent() : new GUIContent(currentObj.name, currentObj.mainTexture, FXMakerTooltip.Tooltip(GetTextureInfo(currentObj.mainTexture, false))));
		m_SelectedMaterial				= currentObj;
		m_CurrentTextureMaterials		= null;
		m_nCurrentTextureMaterialIndex	= -1;
		m_NewShaderMaterials			= null;
		m_nNewShaderMaterialIndex		= -1;
		m_nBottomHeight					= m_nOriginalBottomHeight + m_nMaterialBottomHeight + m_nPaletteBottomHeight;
		m_bUniqueSave					= false;
		m_PrefsName						= "Texture";
		m_BaseDefaultGroupName			= m_DefaultGroupName;
		bool baseRet = base.ShowPopupWindow(m_SelectedTransform, bSaveDialog);

		// arg select
		m_SelObjectContent	= m_OriObjectContent;
		if (currentObj != null)
			SetActiveObject(currentObj.mainTexture, false);

		return baseRet;
	}

	public override void ClosePopup(bool bSave)
	{
		if (enabled == false)
			return;

		// Save Material
		if (bSave && m_SelectedMaterial != null && NgMaterial.IsSameMaterial(m_SelectedMaterial, m_OriMaterial, true) == false)
			SaveMaterial(m_SelectedMaterial);

		m_SelectedMaterial		= null;
		base.ClosePopup(bSave);
	}

	Texture GetObjectNodeTexture(int nObjIndex)
	{
		if (m_ObjectNodes[nObjIndex].m_Object != null)
			return (Texture)m_ObjectNodes[nObjIndex].m_Object;
		m_ObjectNodes[nObjIndex].m_Object = AssetDatabase.LoadAssetAtPath(m_ObjectNodes[nObjIndex].m_AssetPath, typeof(Texture2D));
		return (Texture)m_ObjectNodes[nObjIndex].m_Object;
	}

	// -------------------------------------------------------------------------------------------
	void Awake()
	{
		m_nObjectColumn	= 10;
	}

	void Start()
	{
	}

	void Update()
	{
 		if (0 < m_nNotLoadCount)
 			AyncLoadObject();
	}

	public override void OnGUIPopup()
	{
		base.OnGUIPopup();
		// Popup Window ---------------------------------------------------------
//		FXMakerMain.inst.PopupFocusWindow(NgLayout.GetWindowId(NgLayout.WINDOWID.POPUP), GetPopupRect(), winPopup, "Material");
		FXMakerMain.inst.ModalMsgWindow(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.POPUP), GetPopupRect(), winPopup, "Material");

		if (NgMaterial.GetTexture(m_SelectedMaterial, true) != null)
		{
			DrawPreviewMaterial(1, NgMaterial.GetTexture(m_SelectedMaterial, false));
			DrawPreviewMaterial(0, NgMaterial.GetTexture(m_SelectedMaterial, true));
		} else {
			if (NgMaterial.GetTexture(m_SelectedMaterial, false) != null)
				DrawPreviewMaterial(0, NgMaterial.GetTexture(m_SelectedMaterial, false));
		}
	}

	protected void DrawPreviewMaterial(int nIndex, Texture texture)
	{
		m_nPreviewMaterialWidth = 72;

		bool		bShowRight	= (Screen.width/2 < m_PopupPosition.x);
		GUIStyle	styleButton	= GUI.skin.GetStyle("Preview_ImageOnly");
		Rect		baseRect	= GetPopupRect();
		Rect		previewRect;
		Texture		oldTexture	= m_PreviewTexture;
		int			nMainWidth	= m_nPreviewMaterialWidth;

		m_PreviewTexture	= texture;
		if (nIndex == 0)
			 baseRect.y		= baseRect.height - m_nPreviewMaterialWidth * 2;
		else baseRect.y		= baseRect.height - m_nPreviewMaterialWidth * 1;

// 		previewRect	= new Rect((bShowRight ? baseRect.x-m_nPreviewMaterialWidth+2 : baseRect.x+baseRect.width), baseRect.y, m_nPreviewMaterialWidth, m_nPreviewMaterialWidth);
// 		GUI.Window(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.POPUP)+10+nIndex, previewRect, winPreview0, new GUIContent(texture), styleButton);
// 		GUI.BringWindowToFront(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.POPUP)+10+nIndex);
		nMainWidth = 0;

		styleButton	= GUI.skin.GetStyle("Preview_TextOnly");
		previewRect	= new Rect((bShowRight ? baseRect.x-nMainWidth+2-(m_nPreviewMaterialWidth-m_nPreviewCaptionHeight)*1 : baseRect.x+baseRect.width-2+nMainWidth), baseRect.y, m_nPreviewMaterialWidth-m_nPreviewCaptionHeight, m_nPreviewMaterialWidth);
		if (nIndex == 0)
			 GUI.Window(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.POPUP)+20+nIndex, previewRect, winColor0, "Color0", styleButton);
		else GUI.Window(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.POPUP)+20+nIndex, previewRect, winColor1, "Color1", styleButton);
		GUI.BringWindowToFront(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.POPUP)+20+nIndex);

		previewRect	= new Rect((bShowRight ? baseRect.x-nMainWidth+2-(m_nPreviewMaterialWidth-m_nPreviewCaptionHeight)*2 : baseRect.x+baseRect.width-4+nMainWidth+m_nPreviewMaterialWidth-m_nPreviewCaptionHeight), baseRect.y, m_nPreviewMaterialWidth-m_nPreviewCaptionHeight, m_nPreviewMaterialWidth);
		if (nIndex == 0)
			 GUI.Window(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.POPUP)+30+nIndex, previewRect, winAlpha0, "Alpha0", styleButton);
		else GUI.Window(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.POPUP)+30+nIndex, previewRect, winAlpha1, "Alpha1", styleButton);
		GUI.BringWindowToFront(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.POPUP)+30+nIndex);

		m_PreviewTexture = oldTexture;
	}

	// ==========================================================================================================
	protected void winPreview0(int id)
	{
	}

	protected void winColor0(int id)
	{
		Texture tex = NgMaterial.GetTexture(m_SelectedMaterial, false);
		if (tex != null)
		{
			Rect rect = FXMakerLayout.GetOffsetRect(new Rect(0, m_nPreviewCaptionHeight, m_nPreviewMaterialWidth-m_nPreviewCaptionHeight, m_nPreviewMaterialWidth-m_nPreviewCaptionHeight), -4);
			EditorGUI.DrawPreviewTexture(rect, tex);
		}
	}
	protected void winColor1(int id)
	{
		Texture tex = NgMaterial.GetTexture(m_SelectedMaterial, true);
		if (tex != null)
		{
			Rect rect = FXMakerLayout.GetOffsetRect(new Rect(0, m_nPreviewCaptionHeight, m_nPreviewMaterialWidth-m_nPreviewCaptionHeight, m_nPreviewMaterialWidth-m_nPreviewCaptionHeight), -4);
			EditorGUI.DrawPreviewTexture(rect, tex);
		}
	}

	protected void winAlpha0(int id)
	{
		Texture tex = NgMaterial.GetTexture(m_SelectedMaterial, false);
		if (tex != null)
		{
			Rect rect = FXMakerLayout.GetOffsetRect(new Rect(0, m_nPreviewCaptionHeight, m_nPreviewMaterialWidth-m_nPreviewCaptionHeight, m_nPreviewMaterialWidth-m_nPreviewCaptionHeight), -4);
			EditorGUI.DrawTextureAlpha(rect, tex);
		}
	}
	protected void winAlpha1(int id)
	{
		Texture tex = NgMaterial.GetTexture(m_SelectedMaterial, true);
		if (tex != null)
		{
			Rect rect = FXMakerLayout.GetOffsetRect(new Rect(0, m_nPreviewCaptionHeight, m_nPreviewMaterialWidth-m_nPreviewCaptionHeight, m_nPreviewMaterialWidth-m_nPreviewCaptionHeight), -4);
			EditorGUI.DrawTextureAlpha(rect, tex);
		}
	}

	// ==========================================================================================================
	protected override void DrawOverObjectList(Rect gridRect, GUIStyle styleButton)
	{
// 		if (0 <= m_nSelMainTextureIndex)
// 			DrawOverBox(gridRect, styleButton, m_nSelMainTextureIndex, Color.magenta, 3);
// 		if (0 <= m_nSelMaskTextureIndex)
// 			DrawOverBox(gridRect, styleButton, m_nSelMaskTextureIndex, Color.yellow, 5);
	}

	void DrawOverBox(Rect gridRect, GUIStyle styleButton, int nObjectIndex, Color color, int nImageMargin)
	{
		int		nColumn			= nObjectIndex%m_nObjectColumn;
		int		nRow			= nObjectIndex/m_nObjectColumn;

		int		nMargin			= styleButton.margin.left;
		float	fButtonWidth	= ((gridRect.width+(nMargin*2)) / m_nObjectColumn);
		float	fButtonHeight	= (m_fButtonAspect * fButtonWidth)-1f;

		Rect buttonRect = new Rect(fButtonWidth*nColumn+nMargin, fButtonHeight*nRow+nMargin, fButtonWidth-nMargin*2, fButtonHeight-nMargin*2);
		buttonRect		= FXMakerLayout.GetOffsetRect(buttonRect, nImageMargin);
		NgGUIDraw.DrawBox(buttonRect, color, 2, false);
	}

	// ==========================================================================================================
	protected override void DrawBottomRect(Rect baseRect)
	{
		Rect matRect		= baseRect;
		Rect palRect		= baseRect;
		Rect conRect		= baseRect;

		matRect.height		= m_nMaterialBottomHeight;
		palRect.y			= matRect.yMax;
		palRect.height		= m_nPaletteBottomHeight;
		conRect.y			= palRect.yMax;
		conRect.height		= m_nOriginalBottomHeight;

		DrawBottomMaterialRect(matRect);
		DrawBottomPaletteRect(palRect);
		DrawBottomMenuRect(conRect);
	}

	void DrawBottomPaletteRect(Rect baseRect)
	{
		if (m_SelectedMaterial == null)
			return;

		GUIStyle	styleBox = GUI.skin.GetStyle("MaterialList_Box");
		Color		oldColor = GUI.color;
		Color		oldMatColor;
		bool		bReinstance	= false;

		if (NgMaterial.IsMaterialColor(m_SelectedMaterial) == false)
			return;

		// Color Palette Image
		baseRect			= FXMakerLayout.GetOffsetRect(baseRect, 0, -2, 0, -3);
		GUI.Box(baseRect, FXMakerTooltip.GetGUIContentNoTooltip(), styleBox);
		baseRect			= FXMakerLayout.GetOffsetRect(baseRect, 2, 2, -2, -2);
		Rect popupRect		= GetPopupRect();
		Rect leftRect		= FXMakerLayout.GetInnerHorizontalRect(baseRect, 2, 0, 1);
		Rect rightRect		= FXMakerLayout.GetInnerHorizontalRect(baseRect, 2, 1, 1);

		// Pickup Color
		GUI.color			= Color.white;
		Color	pickColor	= oldMatColor = NgMaterial.GetMaterialColor(m_SelectedMaterial);
		Color32	oldMatCol32	= oldMatColor;

		// Color Progress
		Rect barRect		= FXMakerLayout.GetOffsetRect(FXMakerLayout.GetInnerVerticalRect(leftRect, 5, 0, 1), -1, 1, 1, -1);
		GUI.DrawTexture(FXMakerLayout.GetOffsetRect(barRect, 5, 0, -5, 0), m_PaletteBarColorImage);
		float fPaletteBarValue = GUI.HorizontalSlider(barRect, m_fPaletteBarColorValue, 0, 1.0f);
		if (m_fPaletteBarColorValue != fPaletteBarValue)
		{
			m_fPaletteBarColorValue = fPaletteBarValue;
			pickColor	= m_PaletteBarColorImage.GetPixelBilinear(m_fPaletteBarColorValue, 0.5f);
		}

		// Color Palette
		Rect	palRect		= FXMakerLayout.GetOffsetRect(FXMakerLayout.GetInnerVerticalRect(leftRect, 5, 1, 4), 4, 0, -4, -1);
		Rect	lockRect	= new Rect(palRect.x + popupRect.x, palRect.y + popupRect.y, palRect.width, palRect.height+1);
		Vector2	mousePos	= FXMakerLayout.GetGUIMousePosition();

		GUI.DrawTexture(palRect, m_PalettePanelImage);
 		if (Input.GetMouseButtonDown(0) && lockRect.Contains(mousePos))
 			m_bEnterPalette	= true;
 		if (Input.GetMouseButtonUp(0) && m_bEnterPalette)
 		{
			m_bEnterPalette	= false;
			bReinstance		= true;
		}

		if (Input.GetMouseButton(0) && m_bEnterPalette)
		{
			int	xpos	= (int)(mousePos.x - lockRect.x);
			int	ypos	= (int)(mousePos.y - lockRect.y);
			if (xpos < 0) xpos = 0;
			if (ypos < 0) ypos = 0;
			if (lockRect.width <= xpos)	 xpos = (int)(lockRect.width);
			if (lockRect.height <= ypos) ypos = (int)(lockRect.height);

			pickColor	= m_PalettePanelImage.GetPixelBilinear(xpos/palRect.width, (palRect.height-ypos)/palRect.height);
		}

		// Gray Progress
		GUI.changed = false;
		barRect		= FXMakerLayout.GetOffsetRect(FXMakerLayout.GetInnerVerticalRect(rightRect, 5, 0, 1), -1, 1, 1, -1);
		GUI.DrawTexture(FXMakerLayout.GetOffsetRect(barRect, 5, 0, -5, 0), m_PaletteBarGrayImage);
		fPaletteBarValue = GUI.HorizontalSlider(barRect, m_fPaletteBarGrayValue, 0, 1.0f);
		if (GUI.changed)	// m_fPaletteBarGrayValue != fPaletteBarValue
		{
			m_fPaletteBarGrayValue = fPaletteBarValue;
			pickColor	= m_PaletteBarGrayImage.GetPixelBilinear(m_fPaletteBarGrayValue, 0.5f);
			if (Input.GetMouseButtonDown(1))
			{
				m_fPaletteBarGrayValue = 0.5f;
				pickColor	= new Color(0.5f, 0.5f, 0.5f);
			}
			GUI.changed = false;
		}

		GUI.color	= oldColor;

		// Color RGB Scroll
		rightRect				= FXMakerLayout.GetOffsetRect(rightRect, 0, 3, -5, -3);
		Color32	selColor32	= new Color(pickColor.r, pickColor.g, pickColor.b, oldMatColor.a);
		int		nTextWidth	= 15;
		int		nEditWidth	= 38;
		string[] rgbName	= {"R", "G", "B", "A"};
		Color[]	rgbColor	= {Color.red, Color.green, Color.blue, Color.white};
		byte[]	RGBA		= new byte[4];

		RGBA[0] = selColor32.r;
		RGBA[1] = selColor32.g;
		RGBA[2] = selColor32.b;
		RGBA[3] = selColor32.a;

		// RGB Progress
		for (int n = 0; n < 4; n++)
		{
			string	str;
			oldColor	= GUI.color;
			Rect line	= FXMakerLayout.GetInnerVerticalRect(rightRect, 5, n+1, 1);
			line.width	= nTextWidth;
			GUI.color	= rgbColor[n];
			GUI.Label(line, rgbName[n]);

			GUI.color	= oldColor;
			line		= FXMakerLayout.GetInnerVerticalRect(rightRect, 5, n+1, 1);
			RGBA[n]		= (byte)GUI.HorizontalSlider(FXMakerLayout.GetOffsetRect(line, nTextWidth, 0, -nEditWidth-8, 0), RGBA[n], 0, 255);
			if (GUI.changed)
			{
				if (Input.GetMouseButtonDown(1))
					RGBA[n]	= 127;
				GUI.changed = false;
			}
			line.x		= line.x + line.width - nEditWidth;
			line.width	= nEditWidth;
			str			= GUI.TextField(FXMakerLayout.GetOffsetRect(line, 0, -2, 0, 2), RGBA[n].ToString());
 			RGBA[n]		= (byte)(255 < NgConvert.ToUint(str, RGBA[n]) ? 255 : NgConvert.ToUint(str, RGBA[n]));
		}

		selColor32.r	= RGBA[0];
		selColor32.g	= RGBA[1];
		selColor32.b	= RGBA[2];
		selColor32.a	= RGBA[3];

		if (selColor32.r != oldMatCol32.r || selColor32.g != oldMatCol32.g || selColor32.b != oldMatCol32.b || selColor32.a != oldMatCol32.a)
		{
			SetActiveMaterialColor(selColor32);
		}

		// mouse up - reinstance
		Rect chkRect = new Rect(baseRect.x + popupRect.x, baseRect.y + popupRect.y, baseRect.width, baseRect.height);
		if (Input.GetMouseButtonUp(0) && chkRect.Contains(mousePos))
			bReinstance = true;

		if (bReinstance)
		{
// 			CreateNewShaderMaterials(m_SelectedMaterial);
			FXMakerMain.inst.CreateCurrentInstanceEffect(true);
		}
	}

	void DrawBottomMaterialRect(Rect baseRect)
	{
		int			nVMargin = 4;
		GUIStyle	styleBox		= GUI.skin.GetStyle("MaterialList_Box");
		GUIStyle	styleButton		= GUI.skin.GetStyle("ImageOnly_Button");

		// Draw line
		NgGUIDraw.DrawHorizontalLine(new Vector2(baseRect.x+nVMargin, baseRect.y+nVMargin), (int)baseRect.width, new Color(0.1f, 0.1f, 0.1f, 0.7f), 2, false);

		baseRect = FXMakerLayout.GetOffsetRect(baseRect, 0, nVMargin*2, 0, -nVMargin);
		int			nObjectCount;
		Rect		listRect;
		int			nColumn;
		Rect		scrollRect;
		Rect		gridRect;
		int			nSelectIndex;
		bool		bShowList = true;	// (0 <= m_nObjectIndex && m_nSelObjectProjectIndex == m_nProjectIndex && m_nGroupIndex == m_nSelObjectGroupIndex);

		// New Shader Material List ------------------------------------------------------
		listRect		= FXMakerLayout.GetInnerHorizontalRect(baseRect, 2, 0, 1);
		GUI.Box(listRect, GetHelpContent("New Shader Material"), styleBox);

		if (bShowList && m_NewShaderMaterials != null && 0 < m_NewShaderMaterials.Length)
		{
			listRect		= FXMakerLayout.GetOffsetRect(listRect, 5, 15, -5, -3);
			nColumn			= (int)((listRect.width-5) / listRect.height*2.3f);
			nObjectCount	= m_NewShaderMaterials.Length;
			scrollRect		= FXMakerLayout.GetAspectScrollViewRect((int)listRect.width, m_fButtonAspect, nObjectCount, nColumn, true);
			gridRect		= FXMakerLayout.GetAspectScrollGridRect((int)listRect.width, m_fButtonAspect, nObjectCount, nColumn, true);

			m_NewShaderMaterialListScrollPos	= GUI.BeginScrollView(listRect, m_NewShaderMaterialListScrollPos, scrollRect);
//			nSelectIndex	= GUI.SelectionGrid(gridRect, m_nNewShaderMaterialIndex, m_NewShaderMaterialContents, nColumn, styleButton);
			nSelectIndex	= FXMakerLayout.TooltipSelectionGrid(FXMakerLayout.GetOffsetRect(GetPopupRect(), 0, -m_NewShaderMaterialListScrollPos.y), listRect, gridRect, m_nNewShaderMaterialIndex, m_NewShaderMaterialContents, nColumn, styleButton);

			if (m_nNewShaderMaterialIndex != nSelectIndex)
			{
				if (Input.GetMouseButtonUp(1))
				{
					FXMakerAsset.SetPingObject(m_NewShaderMaterials[nSelectIndex]);
				} else {
					m_nNewShaderMaterialIndex = nSelectIndex;
					if (m_SelectedMaterial != null)
						NgMaterial.CopyMaterialArgument(m_SelectedMaterial, m_NewShaderMaterials[nSelectIndex]);
					SetActiveMaterial(m_NewShaderMaterials[nSelectIndex], null, true, false);
				}
			}
			GUI.EndScrollView();
		}

		// Found Material List ------------------------------------------------------
		listRect		= FXMakerLayout.GetInnerHorizontalRect(baseRect, 2, 1, 1);
		GUI.Box(listRect, GetHelpContent("Found Material"), styleBox);

		if (bShowList && m_CurrentTextureMaterials != null && 0 < m_CurrentTextureMaterials.Length)
		{
			listRect		= FXMakerLayout.GetOffsetRect(listRect, 5, 15, -5, -3);
			nColumn			= (int)((listRect.width-5) / listRect.height*2.3f);
			nObjectCount	= m_CurrentTextureMaterials.Length;
			scrollRect		= FXMakerLayout.GetAspectScrollViewRect((int)listRect.width, m_fButtonAspect, nObjectCount, nColumn, true);
			gridRect		= FXMakerLayout.GetAspectScrollGridRect((int)listRect.width, m_fButtonAspect, nObjectCount, nColumn, true);

			m_CurrentTextureMaterialListScrollPos	= GUI.BeginScrollView(listRect, m_CurrentTextureMaterialListScrollPos, scrollRect);
//			nSelectIndex	= GUI.SelectionGrid(gridRect, m_nCurrentTextureMaterialIndex, m_CurrentTextureMaterialContents, nColumn, styleButton);
			nSelectIndex	= FXMakerLayout.TooltipSelectionGrid(FXMakerLayout.GetOffsetRect(GetPopupRect(), 0, -m_CurrentTextureMaterialListScrollPos.y), listRect, gridRect, m_nCurrentTextureMaterialIndex, m_CurrentTextureMaterialContents, nColumn, styleButton);

			if (m_nCurrentTextureMaterialIndex != nSelectIndex)
			{
				if (Input.GetMouseButtonUp(1))
				{
					FXMakerAsset.SetPingObject(m_NewShaderMaterials[nSelectIndex]);
				} else {
					m_nCurrentTextureMaterialIndex = nSelectIndex;
					SetActiveMaterial(m_CurrentTextureMaterials[nSelectIndex], null, false, false);
				}
			}
			GUI.EndScrollView();
		}
	}

	void DrawBottomMenuRect(Rect baseRect)
	{
		GUI.Box(baseRect, "");

		Rect	imageRect	= baseRect;
		imageRect.width		= m_nOriginalBottomHeight-1;
		Rect	rightRect	= baseRect;

		rightRect.x			+= imageRect.width;
		rightRect.width		-= imageRect.width;
		rightRect			= FXMakerLayout.GetOffsetRect(rightRect, 5, 3, -5, -3);

		Rect	buttonRect	= FXMakerLayout.GetInnerVerticalRect(rightRect, 12, 0, 5);
		int		nButtonWidht= 70;
		buttonRect.width	= nButtonWidht/2;

		if (m_SelectedMaterial != null && m_SelObjectContent != null)
		{
			// image
			if (GUI.Button(imageRect, new GUIContent("", m_SelObjectContent.image, m_SelObjectContent.tooltip), GUI.skin.GetStyle("PopupBottom_ImageButton")))
			{
				if (Input.GetMouseButtonUp(0))
				{
					if (0 <= m_nNewShaderMaterialIndex)
						FXMakerAsset.SetPingObject(m_SelectedTransform.gameObject);
					else FXMakerAsset.SetPingObject(m_SelectedMaterial);
					FXMakerMain.inst.CreateCurrentInstanceEffect(true);
				}
				if (Input.GetMouseButtonUp(1))
					FXMakerAsset.SetPingObject(m_SelObjectContent.image);
			}

			// Current Color
			Color matColor	= NgMaterial.GetMaterialColor(m_SelectedMaterial);
			Rect colorRect	= FXMakerLayout.GetOffsetRect(FXMakerLayout.GetInnerVerticalRect(rightRect, 12, 5, 7), 0, 2, 0, 0);
			colorRect.width	= nButtonWidht/2;
			if (NgMaterial.IsMaterialColor(m_SelectedMaterial))
				EditorGUIUtility.DrawColorSwatch(colorRect, matColor);

			// text
			rightRect.x += colorRect.width;
			rightRect.width	-= colorRect.width;
			GUI.Label(FXMakerLayout.GetInnerVerticalRect(rightRect, 12, 5, 3), m_SelObjectContent.text);
			GUI.Label(FXMakerLayout.GetInnerVerticalRect(rightRect, 12, 7, 5), (m_SelectedMaterial.shader != null ? m_SelectedMaterial.shader.name : "[Not Selected]"));


			bool bChange = (NgMaterial.IsSameMaterial(m_SelectedMaterial, m_OriMaterial, true) == false);
			// Undo
			if (FXMakerLayout.GUIButton(buttonRect, GetHelpContent("Undo"), bChange))
			{
				UndoObject();
				return;
			}

			// UniqueSave
			buttonRect.x		+= buttonRect.width + 5;
			buttonRect.width	= nButtonWidht;
			if (FXMakerLayout.GUIButton(buttonRect, GetHelpContent("UniqueSave"), bChange))
			{
				m_bUniqueSave	= true;
				ClosePopup(true);
				return;
			}
		}

		// close
		buttonRect.x		+= buttonRect.width + 5;
		buttonRect.width	= baseRect.width - buttonRect.x;
		if (GUI.Button(buttonRect, GetHelpContent("Close")))
			ClosePopup(true);

		// ���õȰ�, ������, ��ġ, ȸ��, ����, �ݱ�, ����
	}

	protected override void UndoObject()
	{
		SetSelectedTransformMaterial(m_OriMaterial);
		m_SelObjectContent				= m_OriObjectContent;
		m_nObjectIndex					= -1;
		m_nSelObjectGroupIndex			= -1;
		m_nSelObjectProjectIndex		= -1;

		m_nCurrentTextureMaterialIndex	= -1;
		m_nNewShaderMaterialIndex		= -1;

		if (m_OriMaterial != null)
			SetActiveObject(m_OriMaterial.mainTexture, false);
		FXMakerMain.inst.CreateCurrentInstanceEffect(true);
	}

	// ----------------------------------------------------------------------------------------------------------
	protected override void SetActiveObject(int nObjectIndex)
	{
		if (nObjectIndex < 0 || m_nObjectCount <= nObjectIndex)
			return;

		Texture	selTexture	= GetObjectNodeTexture(nObjectIndex);
		bool	bMaskTex	= false;

		// right button, image Ping and not select
		if (Input.GetMouseButtonUp(1))
		{
			if (m_SelectedMaterial != null && NgMaterial.IsMaskTexture(m_SelectedMaterial))
			{
				m_nSelMaskTextureIndex = nObjectIndex;
				bMaskTex = true;
			} else {
				FXMakerAsset.SetPingObject(selTexture);
				return;
			}
		} else {
			m_nSelMainTextureIndex = nObjectIndex;
		}

// 		// ������ ��Ŭ��
// 		if (m_nObjectIndex == nObjectIndex)
// 		{
// 			FXMakerAsset.SetPingObject(m_SelectedMaterial);
// 			FXMakerMain.inst.CreateCurrentInstanceEffect(true);
// 			return;
// 		}

		SetActiveObject(selTexture, bMaskTex);

		// GUI Update
		m_nObjectIndex			= nObjectIndex;
		m_nSelObjectProjectIndex= m_nProjectIndex;
		m_nSelObjectGroupIndex	= (m_bOptionRecursively ? -1 : m_nGroupIndex);
		m_SelObjectContent		= new GUIContent(m_ObjectContents[nObjectIndex].text, m_ObjectContents[nObjectIndex].image, m_ObjectContents[nObjectIndex].tooltip);
	}

	protected void SetActiveObject(Texture selTexture, bool bMaskTex)
	{
		if (selTexture == null)
			return;

		// Create Mat
		if (m_SelectedMaterial == null || NgMaterial.GetTexture(m_SelectedMaterial, bMaskTex) != selTexture)
		{
			SetActiveMaterial(null, selTexture, false, bMaskTex);
		}

		// Find NewTexture Materials
 		FindCurrentTextureMaterials(m_SelectedMaterial);

		// Create NewShader Materials
		CreateNewShaderMaterials(m_SelectedMaterial);
		m_SelectedTextureMaterials	= m_CurrentTextureMaterials;
	}

	void SetActiveMaterial(Material srcMaterial, Texture newTexture, bool bNewShaderMaterial, bool bMaskTex)
	{
		Material	newMat = m_SelectedMaterial;

		// Create mat
		if (srcMaterial != null)
		{
			newMat = new Material(srcMaterial);
		} else {
			if (m_SelectedMaterial == null)
			{
				newMat = CreateDefaultMaterial();
			} else {
				// �ؽ��� �޶���
				if (m_SelectedMaterial == m_OriMaterial)
					newMat = new Material(m_SelectedMaterial);
			}
		}

		// Set Property
		if (newTexture != null)
		{
			NgMaterial.SetMaskTexture(newMat, bMaskTex, newTexture);
			if (newMat.mainTexture == null)
				 newMat.name = newTexture.name;
			else newMat.name = newMat.mainTexture.name;
		}

		// Current Change
		SetSelectedTransformMaterial(newMat);

		if (bNewShaderMaterial)
			m_nCurrentTextureMaterialIndex = -1;
		else if (newTexture != null) m_nNewShaderMaterialIndex = -1;

		// Reinstance
		FXMakerMain.inst.CreateCurrentInstanceEffect(true);
	}

	void SetActiveMaterialColor(Color newColor)
	{
		Material	newMat = m_SelectedMaterial;

		// Create mat
		if (m_SelectedMaterial == null)
		{
			newMat = CreateDefaultMaterial();
		} else {
			if (m_SelectedMaterial == m_OriMaterial)
				newMat = new Material(m_SelectedMaterial);
		}

		// Change Color
		NgMaterial.SetMaterialColor(newMat, newColor);
		if (m_NewShaderMaterials != null)
			foreach (Material mat in m_NewShaderMaterials)
				NgMaterial.SetMaterialColor(mat, newColor);

		// Current Change
		SetSelectedTransformMaterial(newMat);
	}

	Material CreateDefaultMaterial()
	{
		Material newMat;
		newMat = new Material(Shader.Find("Particles/Alpha Blended"));	// Particles/Additive
		return newMat;
	}

	void SetSelectedTransformMaterial(Material selMaterial)
	{
		Material[] mats = m_SelectedTransform.GetComponent<Renderer>().sharedMaterials;

		m_SelectedMaterial = selMaterial;
		mats[m_nSelRendererMatIndex] = selMaterial;
		m_SelectedTransform.GetComponent<Renderer>().sharedMaterials = mats;
// 		m_SelectedTransform.renderer.sharedMaterial = selMaterial;
		FXMakerHierarchy.inst.SetActiveComponent(m_SelectedTransform.gameObject, m_SelectedMaterial, false);
		FXMakerAsset.SetPingObject(m_SelectedMaterial);
	}

	protected override void LoadObjects()
	{
		if (m_nTempLoadFrameIndex == FXMakerMain.inst.GetUnityFrameCount())
		{
			return;
		}
		m_nTempLoadFrameIndex = FXMakerMain.inst.GetUnityFrameCount();

		m_ObjectNodes		= null;
		m_ObjectContents	= null;
		m_nObjectCount		= 0;
		m_nObjectIndex		= -1;

		if (0 < m_nGroupCount)
		{
			string	loaddir;
			if (m_bOptionRecursively)
				 loaddir	= NgFile.CombinePath(FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.PROJECTS), m_ProjectFolerContents[m_nProjectIndex].text + "/");
			else loaddir	= NgFile.CombinePath(FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.PROJECTS), m_ProjectFolerContents[m_nProjectIndex].text + "/" + m_GroupFolderContents[m_nGroupIndex].text  + "/");
			AddObjects(loaddir);
		}
	}

	void AddObjects(string loaddir)
	{
// 		StartCoroutine("NgAsset.GetTextureList()");

		m_LoadDirectory		= NgFile.PathSeparatorNormalize(loaddir);
//		m_TextureObjects	= NgAsset.GetTextureList(loaddir, true, FXMakerLayout.m_nMaxTextureListCount, out m_nObjectCount);
		m_ObjectNodes		= NgAsset.GetTextureList(loaddir, true, FXMakerLayout.m_nMaxTextureListCount, true, out m_nObjectCount);
		m_ObjectContents	= new GUIContent[m_nObjectCount];

		if (m_SelectedMaterial != null)
		{
			Texture maskTex = NgMaterial.GetMaskTexture(m_SelectedMaterial);
			m_nSelMainTextureIndex	= -1;
			m_nSelMaskTextureIndex	= -1;

			string selTexPath	= AssetDatabase.GetAssetPath(m_SelectedMaterial.mainTexture);
			string selMaskPath	= (maskTex != null ? AssetDatabase.GetAssetPath(maskTex) : null);

			for (int n=0; n < m_nObjectCount; n++)
			{
				// Current Select
				if (m_ObjectNodes[n].m_AssetPath == selTexPath)
					m_nSelMainTextureIndex = m_nObjectIndex = n;
				// Current Mask Select
				if (m_ObjectNodes[n].m_AssetPath == selMaskPath)
					m_nSelMaskTextureIndex = n;
			}
		}
		BuildContents();
	}

	void BuildContents()
	{
		if (enabled == false)
			return;

		m_nNotLoadCount = 0;
		for (int n=0; n < m_nObjectCount; n++)
		{
			if (m_ObjectContents[n] == null)
			{
				m_ObjectContents[n]			= new GUIContent();
				m_ObjectContents[n].text	= NgFile.GetFilename(m_ObjectNodes[n].m_AssetPath);
			}
			if (m_ObjectNodes[n].m_Object == null)
				m_nNotLoadCount++;
			else {
 				m_ObjectContents[n].image	= NgAsset.GetMiniThumbnail(m_ObjectNodes[n].m_Object);
				m_ObjectContents[n].tooltip	= FXMakerTooltip.Tooltip(GetTextureInfo(m_ObjectContents[n].image, true));
				m_ObjectContents[n].tooltip	+= FXMakerTooltip.AddPopupPreview(m_ObjectContents[n].image);
			}
		}
	}

	void AyncLoadObject()
	{
		if (enabled == false)
			return;
		NcTickTimerTool	tickTimer = new NcTickTimerTool();

		for (int n=0; n < m_nObjectCount; n++)
		{
			if (m_ObjectNodes[n].m_Object == null)
			{
				m_ObjectNodes[n].m_Object	= GetObjectNodeTexture(n);
 				m_ObjectContents[n].image	= NgAsset.GetMiniThumbnail(m_ObjectNodes[n].m_Object);
				m_ObjectContents[n].tooltip	= FXMakerTooltip.Tooltip(GetTextureInfo(m_ObjectContents[n].image, true));
				m_ObjectContents[n].tooltip	+= FXMakerTooltip.AddPopupPreview(m_ObjectContents[n].image);
				m_nNotLoadCount--;
// 				return;
				if (200 < tickTimer.GetStartedTickCount())
					return;
			}
		}
	}

	string GetTextureInfo(Texture tex, bool bShowSubDir)
	{
		Texture2D tex2d = (tex as Texture2D);
		
		if (tex2d != null)
		{
			string	desc = FXMakerTooltip.GetHsFolderPopup_Texture("HOVER_DESC");
			string	name = "";
			if (bShowSubDir)
			{
				name = AssetDatabase.GetAssetPath(tex);
				name = NgFile.PathSeparatorNormalize(name).Replace(m_LoadDirectory, "");
			} else name = tex.name;

			return string.Format("{0}\n{1}x{2} {3}\n{4} {5} M={6} A={7}\n{8}"
				, name, tex2d.width, tex2d.height, tex2d.format, tex2d.filterMode, (tex2d.wrapMode==0 ? "Repeat" : "Clamp"), tex2d.mipmapCount, tex2d.anisoLevel, desc);
		}
		return "";
	}

	// -------------------------------------------------------------------------------------------
	void FindCurrentTextureMaterials(Material currentMat)
	{
		m_nCurrentTextureMaterialIndex	= -1;
		m_CurrentTextureMaterials		= null;
		m_CurrentTextureMaterialContents= null;

		if (currentMat.mainTexture == null)
			return;

		string texPath = AssetDatabase.GetAssetPath(currentMat.mainTexture);
		if (texPath == "")
			return;

		int			nMaterialCount	= 0;
		Material[]	materialObjects	= NgAsset.GetMaterialList(NgFile.TrimFilenameExt(texPath), true, FXMakerLayout.m_nMaxMaterialListCount, out nMaterialCount);

		ArrayList	matList = new ArrayList();

		// find
		foreach (Material mat in materialObjects)
		{
			// ������ ǥ��
			if (mat.mainTexture == currentMat.mainTexture)
			{
				if (mat == currentMat)
				{
//					Debug.Log("if (mat == currentMat)----------------------");
					m_nCurrentTextureMaterialIndex = matList.Count;
				}
				matList.Add(mat);
			}

			m_CurrentTextureMaterials		 = NgConvert.ToArray<Material>(matList);
			m_CurrentTextureMaterialContents = new GUIContent[m_CurrentTextureMaterials.Length];

			// build content
			BuildCurrentTextureContents();
		}
	}

	void BuildCurrentTextureContents()
	{
		CancelInvoke("BuildCurrentTextureContents");
		if (enabled == false)
			return;

		if (m_CurrentTextureMaterials == null || m_CurrentTextureMaterials.Length <= 0)
			return;

		int nNotLoadPreviewCount = 0;
		int nCount = 0;
		foreach (Material mat in m_CurrentTextureMaterials)
		{
			if (m_CurrentTextureMaterialContents[nCount] == null)
			{
				m_CurrentTextureMaterialContents[nCount]			= new GUIContent();
				m_CurrentTextureMaterialContents[nCount].text		= mat.name;
				m_CurrentTextureMaterialContents[nCount].tooltip	= FXMakerTooltip.Tooltip(mat.name + "\n" + (mat.shader != null ? mat.shader.name : "Not Selected"));
			}
			if (m_CurrentTextureMaterialContents[nCount].image == null)
			{
				m_CurrentTextureMaterialContents[nCount].image		= NgAsset.GetAssetPreview(mat);
				if (m_CurrentTextureMaterialContents[nCount].image != null)
					m_CurrentTextureMaterialContents[nCount].tooltip += FXMakerTooltip.AddPopupPreview(m_CurrentTextureMaterialContents[nCount].image);// + FXMakertip.AddPopupPreview(NgMaterial.GetTexture(mat, false)) + FXMakertip.AddPopupPreview(NgMaterial.GetTexture(mat, true));
			}
			if (m_CurrentTextureMaterialContents[nCount].image == null)
				nNotLoadPreviewCount++;
			nCount++;
		}
		if (0 < nNotLoadPreviewCount)
			Invoke("BuildCurrentTextureContents", FXMakerLayout.m_fReloadPreviewTime);
	}

	void CreateNewShaderMaterials(Material currentMat)
	{
		if (m_NewShaderMaterials != null && FXMakerOption.inst.m_bUpdateNewMaterial == false)
			return;

		// Load m_DefaultShaderMaterialsDir
		int		nMaterialCount;
		m_NewShaderMaterials		= NgAsset.GetMaterialList(FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.DEFAULTSHADERMATERIALS), true, 0, out nMaterialCount);
		m_NewShaderMaterialContents	= null;

		// Change Texture
		if (m_NewShaderMaterials != null)
		{
			foreach (Material mat in m_NewShaderMaterials)
				NgMaterial.CopyMaterialArgument(currentMat, mat);

 			FXMakerAsset.AssetDatabaseSaveAssets();
			m_nTempSaveFrameIndex = FXMakerMain.inst.GetUnityFrameCount();
			m_NewShaderMaterialContents = new GUIContent[m_NewShaderMaterials.Length];
			// build content
			BuildNewShaderContents();
		}
	}

	void BuildNewShaderContents()
	{
		CancelInvoke("BuildNewShaderContents");
		if (enabled == false)
			return;

		if (m_NewShaderMaterials == null || m_NewShaderMaterials.Length <= 0)
			return;

		int nNotLoadPreviewCount = 0;
		int nCount = 0;
		foreach (Material mat in m_NewShaderMaterials)
		{
			if (m_NewShaderMaterialContents[nCount] == null)
			{
				m_NewShaderMaterialContents[nCount]			= new GUIContent();
				m_NewShaderMaterialContents[nCount].text	= mat.name;
				m_NewShaderMaterialContents[nCount].tooltip	= FXMakerTooltip.Tooltip(mat.name + "\n" + (mat.shader != null ? mat.shader.name : "Not Selected"));
			}
			if (m_NewShaderMaterialContents[nCount].image == null)
			{
				m_NewShaderMaterialContents[nCount].image	= NgAsset.GetAssetPreview(mat);
				if (m_NewShaderMaterialContents[nCount].image != null)
					m_NewShaderMaterialContents[nCount].tooltip	+= FXMakerTooltip.AddPopupPreview(m_NewShaderMaterialContents[nCount].image);
			}
			if (m_NewShaderMaterialContents[nCount].image == null)
				nNotLoadPreviewCount++;
			nCount++;
		}
		if (0 < nNotLoadPreviewCount)
			Invoke("BuildNewShaderContents", FXMakerLayout.m_fReloadPreviewTime);
	}

	Material CreateMaterials(ArrayList addMatList, Texture newTex, string shaderName)
	{
		Shader		newShader	= Shader.Find(shaderName);
		if (newShader != null)
		{
			Material	newMat	= new Material(newShader);
			newMat.name	= newTex.name;
			newMat.mainTexture = newTex;
			if (addMatList != null)
				addMatList.Add(newMat);
			return newMat;
		}
		return null;
	}

	void SaveMaterial(Material addMat)
	{
		string		path		= "";
		string		newPath		= "";
		string		uniquePath;
		Object		existsObj;
		int			nUniqueCount = 0;

		// Find Same Material
		if (m_bUniqueSave == false && m_SelectedTextureMaterials != null)
		{
			foreach (Material mat in m_SelectedTextureMaterials)
			{
				if (NgMaterial.IsSameMaterial(mat, addMat, false))
				{
					NgUtil.LogMessage(FXMakerTooltip.GetHsToolMessage("MATERIAL_EXISTSAVED", ""));
					SetSelectedTransformMaterial(mat);
					return;
				}
			}
		}

		// Create Path
		bool	bUseDefaultFolder = false;
		string	devMatDir	= "_MaterialsTool";
		string	userMatDir	= "_MaterialsUser";
		string	matDir;

		if (FXMakerLayout.m_bDevelopState)
			 matDir = devMatDir;
		else matDir = userMatDir;

		if ((addMat.mainTexture != null))
		{
			path = AssetDatabase.GetAssetPath(addMat.mainTexture);
			if (path == "")
				bUseDefaultFolder = true;
			else {
				newPath = NgFile.CombinePath(NgFile.TrimFilenameExt(path), matDir);
				// Default SubDirectory
				if (NgAsset.ExistsDirectory(newPath) == false)
					AssetDatabase.CreateFolder(NgFile.TrimFilenameExt(path), matDir);
			}
		} else {
			newPath = NgFile.TrimFilenameExt(AssetDatabase.GetAssetPath(m_OriMaterial));
			if (newPath == "")
			{
				bUseDefaultFolder = true;
			} else {
				string tmpPath = NgFile.TrimLastFolder(newPath);
				string tmpLast = NgFile.GetLastFolder(newPath);

				if (FXMakerLayout.m_bDevelopState)
				{
					if (tmpLast != devMatDir)
					{
						newPath	= NgFile.CombinePath(tmpPath, devMatDir);
						if (NgAsset.ExistsDirectory(newPath) == false)
							AssetDatabase.CreateFolder(tmpPath, matDir);
					}
				} else {
					if (tmpLast != userMatDir)
					{
						newPath	= NgFile.CombinePath(tmpPath, userMatDir);
						if (NgAsset.ExistsDirectory(newPath) == false)
							AssetDatabase.CreateFolder(tmpPath, matDir);
					}
				}
			}
		}

		if (bUseDefaultFolder)
		{
			path	= FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.BACKGROUNDRESOURCES);
			newPath	= NgFile.CombinePath(path, matDir);
			// Default SubDirectory
			if (NgAsset.ExistsDirectory(newPath) == false)
				AssetDatabase.CreateFolder(path, matDir);
		}

		// Unique Name
		string texname = addMat.name;
		if (addMat.mainTexture != null)
			texname = addMat.mainTexture.name;
		int nLoopCount = 0;
		while (true)
		{
			string matName = texname + (0<nUniqueCount ? "_"+nUniqueCount.ToString() : "") + ".mat";
			uniquePath	= NgFile.CombinePath(newPath, matName);
			existsObj = AssetDatabase.LoadAssetAtPath(uniquePath, typeof(Material));
			if (existsObj == null)
			{
				break;
			}
			nLoopCount++;
			nUniqueCount++;
			if (999 < nUniqueCount)
				nUniqueCount = 1;
			if (999 < nLoopCount)
			{
				Debug.LogError("Over Loop ----------------------");
				return;
			}
		}

		AssetDatabase.CreateAsset(addMat, uniquePath);
		NgUtil.LogMessage(FXMakerTooltip.GetHsToolMessage("MATERIAL_NEWSAVED", "") + "\n" + uniquePath);
		FXMakerAsset.AssetDatabaseRefresh();
		FXMakerAsset.AssetDatabaseSaveAssets();
	}

	// -------------------------------------------------------------------------------------------
	protected override GUIContent GetHelpContent(string text)
	{
		return FXMakerTooltip.GetHcFolderPopup_Texture(text);
	}
}
#endif
