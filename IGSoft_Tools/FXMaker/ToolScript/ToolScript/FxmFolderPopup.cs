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

public class FxmFolderPopup : FxmPopup
{
	// Attribute ------------------------------------------------------------------------
	// const
	protected		const int			m_nDefaultPopupColumn		= 3;
	protected		int					m_nPopupColumn				= m_nDefaultPopupColumn;
	protected		int					m_nProjectColumn			= 3;
	protected		int					m_nGroupColumn				= 2;
	protected		int					m_nObjectColumn;			// setting sub class
	protected		int					m_nOriMaxObjectColumn		= 6;
	protected		int					m_nMaxObjectColumn			= m_nDefaultPopupColumn;
	protected		int					m_nMenuButtonHeight			= 19;
	protected		float				m_fButtonAspect				= 1.0f;
	protected		const int			m_nOriginalBottomHeight		= 75;
	protected		int					m_nBottomHeight				= m_nOriginalBottomHeight;
	protected		float				m_nPreviewWidth				= 250;
	protected		int					m_nPreviewCaptionHeight		= 13;

	// popup
	public			string				m_DefaultProjectName		= "[Resources]";
	protected		string				m_BaseDefaultGroupName		= "";
	protected		bool				m_bFixedOptionRecursively;
	protected		bool				m_bDisableOptionShowName	= false;
	protected		bool				m_bOptionRecursively;
	protected		bool				m_bOptionShowName;
	protected		GUIContent[]		m_ProjectFolerContents;
	protected		GUIContent[]		m_GroupFolderContents;
	protected		NgAsset.ObjectNode[]m_ObjectNodes;
	protected		GUIContent[]		m_ObjectContents;
	protected		int					m_nNotLoadCount;
	protected		string				m_LoadDirectory;
	protected		bool				m_bInvokeLoadObject;
	protected		string				m_PrefsName;
	protected		bool				m_bShowLeftPopup			= false;

	protected		bool				m_bDrawRedProject;
	protected		bool				m_bDrawRedGroup;
	protected		bool				m_bDrawRedBottom;
	protected		bool				m_bDrawRedBottomButtom;
	protected		bool				m_bSaveDialog;
	protected		string 				m_SaveFilename;
	protected		Texture				m_PreviewTexture;

	protected		int					m_nProjectIndex				= -1;
	protected		int					m_nGroupIndex				= -1;
	protected		int					m_nSelObjectProjectIndex;
	protected		int					m_nSelObjectGroupIndex;
	protected		GUIContent			m_SelObjectContent;
	protected		int					m_nObjectIndex;
	protected		int					m_nProjectCount;
	protected		int					m_nGroupCount;
	protected		int					m_nObjectCount;

	protected		Vector2				m_ObjectListScrollPos;

	// Property -------------------------------------------------------------------------
	public virtual bool ShowPopupWindow(Object selObj, bool bSaveDialog)
	{
		if (selObj == null)
		{
			Debug.LogError("Invaild Call ShowPopupWindow() -------------");
		}

		m_bSaveDialog		= bSaveDialog;
		enabled				= true;

		m_nSelObjectProjectIndex= -1;
		m_nSelObjectGroupIndex	= -1;
		m_nObjectIndex			= -1;
		m_SelObjectContent		= null;
		m_PreviewTexture		= null;

		LoadPrefs();
		LoadProjects();

		// Search DefaultDir
		if (m_nProjectIndex < 0)
		{
			for (m_nProjectIndex = 0; m_nProjectIndex < m_ProjectFolerContents.Length; m_nProjectIndex++)
				if (m_ProjectFolerContents[m_nProjectIndex].text == m_DefaultProjectName)
					break;
		}

		SetProjectIndex(m_nProjectIndex, true);

		base.ShowPopupWindow(null);
		return enabled;
	}

	protected virtual void LoadPrefs()
	{
		if (FXMakerLayout.m_bDevelopPrefs == false)
		{
			m_nProjectIndex			= UnityEditor.EditorPrefs.GetInt	(m_PrefsName + ".m_nProjectIndex"		, m_nProjectIndex);
			m_nGroupIndex			= UnityEditor.EditorPrefs.GetInt	(m_PrefsName + ".m_nGroupIndex"			, m_nGroupIndex);

			m_bShowLeftPopup		= UnityEditor.EditorPrefs.GetBool	(m_PrefsName + ".m_bShowLeftPopup"		, m_bShowLeftPopup);
			m_nPopupColumn			= UnityEditor.EditorPrefs.GetInt	(m_PrefsName + ".m_nPopupColumn"		, m_nPopupColumn);
 			m_nObjectColumn			= UnityEditor.EditorPrefs.GetInt	(m_PrefsName + ".m_nObjectColumn"		, m_nObjectColumn);
			m_bOptionShowName		= UnityEditor.EditorPrefs.GetBool	(m_PrefsName + ".m_bOptionShowName"		, m_bOptionShowName);

			if (m_bFixedOptionRecursively == false)
				m_bOptionRecursively= UnityEditor.EditorPrefs.GetBool	(m_PrefsName + ".m_bOptionRecursively"	, m_bOptionRecursively);
		}
		if (m_bDisableOptionShowName)
			m_bOptionShowName = false;

		UpdatePopupColumn(m_nPopupColumn);
	}

	protected virtual void SavePrefs()
	{
		// Update Prefs
		UnityEditor.EditorPrefs.SetInt	(m_PrefsName + ".m_nProjectIndex"			, m_nProjectIndex);
		UnityEditor.EditorPrefs.SetInt	(m_PrefsName + ".m_nGroupIndex"				, m_nGroupIndex);

		UnityEditor.EditorPrefs.SetBool	(m_PrefsName + ".m_bShowLeftPopup"			, m_bShowLeftPopup);
		UnityEditor.EditorPrefs.SetInt	(m_PrefsName + ".m_nPopupColumn"			, m_nPopupColumn);
		UnityEditor.EditorPrefs.SetInt	(m_PrefsName + ".m_nObjectColumn"			, m_nObjectColumn);
		if (m_bDisableOptionShowName == false)
			UnityEditor.EditorPrefs.SetBool(m_PrefsName + ".m_bOptionShowName"		, m_bOptionShowName);
		if (m_bFixedOptionRecursively == false)
			UnityEditor.EditorPrefs.SetBool(m_PrefsName + ".m_bOptionRecursively"	, m_bOptionRecursively);
	}

	protected void UpdatePopupColumn(int nPopupColumn)
	{
		m_nProjectColumn	= nPopupColumn;
		m_nGroupColumn		= nPopupColumn / 2 + 1;
		m_nMaxObjectColumn	= nPopupColumn * m_nOriMaxObjectColumn;
		float fObjectColumn	= (m_nObjectColumn / (float)m_nPopupColumn) * nPopupColumn;
		m_nObjectColumn		= (fObjectColumn < 1 ? 1 : (int)fObjectColumn);
		m_nPopupColumn		= nPopupColumn;
		SavePrefs();
	}

	public void SetPreviewTexture(Texture2D PreviewTexture)
	{
		m_PreviewTexture = PreviewTexture;
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
		// Preview Window ---------------------------------------------------------
		if (m_PreviewTexture != null)
		{
			DrawPreviewTexture();
		}
	}

	protected void DrawPreviewTexture()
	{
//		if (128 < texture.width)
//			 m_nPreviewWidth = 266;
//		else m_nPreviewWidth = 160;
		m_nPreviewWidth = 160;

		bool		bShowRight	= (Screen.width/2 < m_PopupPosition.x);
		GUIStyle	styleButton	= GUI.skin.GetStyle("Preview_ImageOnly");
		Rect		baseRect	= GetPopupRect();
		Rect		previewRect;

		previewRect	= new Rect((bShowRight ? baseRect.x-m_nPreviewWidth+2 : baseRect.x+baseRect.width), baseRect.y, m_nPreviewWidth, m_nPreviewWidth);
		GUI.Window(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.POPUP)+1, previewRect, winPreview, new GUIContent(m_PreviewTexture), styleButton);
		GUI.BringWindowToFront(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.POPUP)+1);

		styleButton	= GUI.skin.GetStyle("Preview_TextOnly");
		previewRect	= new Rect((bShowRight ? baseRect.x-m_nPreviewWidth+4-m_nPreviewWidth/2+m_nPreviewCaptionHeight : baseRect.x+baseRect.width+m_nPreviewWidth), baseRect.y, m_nPreviewWidth/2-m_nPreviewCaptionHeight, m_nPreviewWidth/2);
		GUI.Window(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.POPUP)+2, previewRect, winColor, "Color", styleButton);
		GUI.BringWindowToFront(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.POPUP)+2);

		previewRect	= new Rect((bShowRight ? baseRect.x-m_nPreviewWidth+4-m_nPreviewWidth/2+m_nPreviewCaptionHeight : baseRect.x+baseRect.width+m_nPreviewWidth), baseRect.y+m_nPreviewWidth/2, m_nPreviewWidth/2-m_nPreviewCaptionHeight, m_nPreviewWidth/2);
		GUI.Window(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.POPUP)+3, previewRect, winAlpha, "Alpha ", styleButton);
		GUI.BringWindowToFront(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.POPUP)+3);
	}

	// ==========================================================================================================
	protected void winPreview(int id)
	{
	}

	protected void winColor(int id)
	{
		if (m_PreviewTexture != null)
		{
			Rect rect = FXMakerLayout.GetOffsetRect(new Rect(0, m_nPreviewCaptionHeight, m_nPreviewWidth/2-m_nPreviewCaptionHeight, m_nPreviewWidth/2-m_nPreviewCaptionHeight), -4);
			EditorGUI.DrawPreviewTexture(rect, m_PreviewTexture);
		}
	}

	protected void winAlpha(int id)
	{
		if (m_PreviewTexture != null)
		{
			Rect rect = FXMakerLayout.GetOffsetRect(new Rect(0, m_nPreviewCaptionHeight, m_nPreviewWidth/2-m_nPreviewCaptionHeight, m_nPreviewWidth/2-m_nPreviewCaptionHeight), -4);
			EditorGUI.DrawTextureAlpha(rect, m_PreviewTexture);
		}
	}

	protected void winPopup(int id)
	{
		Rect		baseRect	= GetPopupRect();

// 		if (UnscreenClose())
// 			return;

		// --------------------------------------------------------------------------------
		int		nProjectRows	= m_nProjectCount/m_nProjectColumn + (0<(m_nProjectCount%m_nProjectColumn) ? 1 : 0);
		int		nGroupRows		= m_nGroupCount/m_nGroupColumn + (0<(m_nGroupCount%m_nGroupColumn) ? 1 : 0);
		int		nTopHeight		= (nProjectRows + (m_bOptionRecursively ? 0 : nGroupRows) + 2) * m_nMenuButtonHeight + m_nMenuButtonHeight/2;
		Rect	topRect			= FXMakerLayout.GetChildTopRect(baseRect, 0, nTopHeight);
		Rect	scrollRect		= FXMakerLayout.GetChildVerticalRect(baseRect, nTopHeight, 1, 0, 1);
		Rect	bottomRect		= FXMakerLayout.GetChildBottomRect(baseRect, m_nBottomHeight);

		// left Align ----------------------------------------------------------------
		if (m_bShowLeftPopup)
		{
			m_bShowLeftPopup = GUI.Toggle(new Rect(baseRect.width-50, 1, baseRect.width, 16), m_bShowLeftPopup, "Left");
			if (GUI.changed)
				EditorPrefs.SetBool	(m_PrefsName + ".m_bShowLeftPopup", m_bShowLeftPopup);
		} else {
			m_bShowLeftPopup = !GUI.Toggle(new Rect(3, 1, baseRect.width, 16), !m_bShowLeftPopup, "Right");
			if (GUI.changed)
				EditorPrefs.SetBool	(m_PrefsName + ".m_bShowLeftPopup", m_bShowLeftPopup);
		}
		GUI.changed = false;

		bottomRect.y	  -= 3;
		scrollRect.height -= (m_nBottomHeight + 3);
		DrawFolderList(topRect);
		DrawObjectList(scrollRect);
		DrawBottomRect(bottomRect);

		FXMakerMain.inst.SaveTooltip();
	}

	// ==========================================================================================================
	protected void DrawFolderList(Rect baseRect)
	{
		if (m_nProjectCount <= 0)
			return;

		Rect	rect;
		int		nPRowCount		= (m_nProjectCount/m_nProjectColumn + (0<(m_nProjectCount%m_nProjectColumn) ? 1 : 0));
		int		nGRowCount		= (m_nGroupCount/m_nGroupColumn + (0<(m_nGroupCount%m_nGroupColumn) ? 1 : 0));

		// Group Project
		rect = FXMakerLayout.GetInnerTopRect(baseRect, 0, nPRowCount*m_nMenuButtonHeight);
//		int nProjectIndex	= GUI.SelectionGrid(rect, m_nProjectIndex, m_ProjectFolerContents, m_nProjectColumn);
		int nProjectIndex	= FXMakerLayout.TooltipSelectionGrid(GetPopupRect(), rect, m_nProjectIndex, m_ProjectFolerContents, m_nProjectColumn);

		if (m_nProjectIndex != nProjectIndex)
			SetProjectIndex(nProjectIndex, false);
		if (m_bDrawRedProject)
			NgGUIDraw.DrawBox(FXMakerLayout.GetOffsetRect(rect, 3), FXMakerLayout.m_ColorHelpBox, 2, false);

		// Draw line
		rect = FXMakerLayout.GetInnerTopRect(baseRect, nPRowCount*m_nMenuButtonHeight, m_nMenuButtonHeight);
		NgGUIDraw.DrawHorizontalLine(new Vector2(rect.x, rect.y+m_nMenuButtonHeight/4+1), (int)rect.width, new Color(0.1f, 0.1f, 0.1f, 0.7f), 2, false);

		// ResizePopup, Recursively , ScrollColumn
		rect = FXMakerLayout.GetInnerTopRect(baseRect, nPRowCount*m_nMenuButtonHeight+m_nMenuButtonHeight/3, m_nMenuButtonHeight);
		rect = FXMakerLayout.GetOffsetRect(rect, 0, rect.height/5, 0, 0);

		bool	bOldOptionRecursively = m_bOptionRecursively;
		int		nLeftSize	= 195;
		Rect	resizeRect	= rect;
		Rect	toggleRect;
		resizeRect.width	= nLeftSize;
		string[]	resizeStrings = {"1", "2", "3", "4", "5"};
		GUIStyle	resizeStyle	= GUI.skin.GetStyle("Popup_ResizePopup");
		int			nPopupColumn	= (int)GUI.SelectionGrid(FXMakerLayout.GetInnerHorizontalRect(resizeRect, 40, 0, 20), m_nPopupColumn-2, resizeStrings, 5, resizeStyle)+2;
		if (GUI.changed && m_nPopupColumn != nPopupColumn)
			UpdatePopupColumn(nPopupColumn);
		if (4 < m_nPopupColumn)
		{
			toggleRect				= FXMakerLayout.GetInnerHorizontalRect(resizeRect, 40, 21, 19);
			m_bOptionRecursively	= FXMakerLayout.GUIToggle(toggleRect, m_bOptionRecursively, GetHelpContent("Recursively"), (m_bFixedOptionRecursively == false));
			if (GUI.changed && bOldOptionRecursively != m_bOptionRecursively)
			{
				SavePrefs();
				SetGroupIndex(0);
			}
			toggleRect.x += toggleRect.width;
			m_bOptionShowName = FXMakerLayout.GUIToggle(toggleRect, m_bOptionShowName, GetHelpContent("ShowName"), !m_bDisableOptionShowName);
			if (GUI.changed)
				SavePrefs();
		} else {
			toggleRect				= FXMakerLayout.GetInnerHorizontalRect(resizeRect, 40, 21, 19);
			toggleRect.y -= 5;
			m_bOptionRecursively	= FXMakerLayout.GUIToggle(toggleRect, m_bOptionRecursively, GetHelpContent("Recursively"), (m_bFixedOptionRecursively == false));
			if (GUI.changed && bOldOptionRecursively != m_bOptionRecursively)
			{
				SavePrefs();
				SetGroupIndex(0);
			}
			toggleRect.y += 12;
			m_bOptionShowName = FXMakerLayout.GUIToggle(toggleRect, m_bOptionShowName, GetHelpContent("ShowName"), !m_bDisableOptionShowName);
			if (GUI.changed)
				SavePrefs();
		}
		if (nLeftSize < (rect.width - nLeftSize))
			nLeftSize = (int)rect.width - nLeftSize;
		int nObjectColumn	= (int)GUI.HorizontalScrollbar(FXMakerLayout.GetOffsetRect(rect, nLeftSize, 1, 0, -2), m_nObjectColumn, 1, 1, m_nMaxObjectColumn+1);
		if (GUI.changed && m_nObjectColumn != nObjectColumn)
		{
			m_nObjectColumn = nObjectColumn;
			SavePrefs();
			if (m_nSelObjectGroupIndex == -1 && m_bOptionRecursively && 0 < m_nObjectCount)
			{
				Rect scrollRect	= FXMakerLayout.GetAspectScrollViewRect((int)baseRect.width, m_fButtonAspect, m_ObjectContents.Length, m_nObjectColumn, false);
				m_ObjectListScrollPos.y = scrollRect.height * (m_nObjectIndex-m_nObjectColumn) / (float)m_nObjectCount;
			}
		}

		// Draw line
		rect = FXMakerLayout.GetInnerTopRect(baseRect, nPRowCount*m_nMenuButtonHeight+m_nMenuButtonHeight+m_nMenuButtonHeight/3, m_nMenuButtonHeight);
		NgGUIDraw.DrawHorizontalLine(new Vector2(rect.x, rect.y+m_nMenuButtonHeight/4+1), (int)rect.width, new Color(0.1f, 0.1f, 0.1f, 0.7f), 2, false);

		// Group List
		if (m_nGroupCount <= 0)
			return;
		if (m_bOptionRecursively == false)
		{
			rect = FXMakerLayout.GetInnerTopRect(baseRect, nPRowCount*m_nMenuButtonHeight+m_nMenuButtonHeight+m_nMenuButtonHeight, nGRowCount*m_nMenuButtonHeight);
//			int nGroupIndex		= GUI.SelectionGrid(rect, m_nGroupIndex, m_GroupFolderContents, m_nGroupColumn);
			int	nGroupIndex		= FXMakerLayout.TooltipSelectionGrid(GetPopupRect(), rect, m_nGroupIndex, m_GroupFolderContents, m_nGroupColumn);

			if (m_nGroupIndex != nGroupIndex)
				SetGroupIndex(nGroupIndex);
			if (m_bDrawRedGroup)
				NgGUIDraw.DrawBox(FXMakerLayout.GetOffsetRect(rect, 3), FXMakerLayout.m_ColorHelpBox, 2, false);
		}
	}

	// ==========================================================================================================
	protected virtual void DrawObjectList(Rect baseRect)
	{
		if (m_bInvokeLoadObject)
		{
			baseRect = FXMakerLayout.GetOffsetRect(baseRect, baseRect.width/8, baseRect.height/3, -baseRect.width/8, -baseRect.height/3);
			GUI.Box(baseRect, "Loading...", GUI.skin.GetStyle("PopupLoading_Box"));
			return;
		}

		if (m_nObjectCount <= 0)
			return;

		int			nObjectCount	= m_ObjectContents.Length;
		int			nColumn			= m_nObjectColumn;

		if (m_nObjectColumn == 0)
		{
			Debug.Break();
			return;
		}

		// Object List ------------------------------------------------------
		Rect		listRect		= baseRect;
		Rect		scrollRect		= FXMakerLayout.GetAspectScrollViewRect((int)listRect.width, m_fButtonAspect, nObjectCount, nColumn, !m_bOptionShowName);
		Rect		gridRect		= FXMakerLayout.GetAspectScrollGridRect((int)listRect.width, m_fButtonAspect, nObjectCount, nColumn, !m_bOptionShowName);
		GUIStyle	styleButton		= GUI.skin.GetStyle("ScrollList_Button");
		m_ObjectListScrollPos		= GUI.BeginScrollView(listRect, m_ObjectListScrollPos, scrollRect);
		styleButton.imagePosition	= (m_bOptionShowName ? ImagePosition.ImageAbove : ImagePosition.ImageOnly);
//		int			nObjectIndex	= GUI.SelectionGrid(gridRect, ((m_nSelObjectProjectIndex == m_nProjectIndex && ((m_nGroupIndex == m_nSelObjectGroupIndex) || (m_nSelObjectGroupIndex == -1 && m_bOptionRecursively))) ? m_nObjectIndex : -1), m_ObjectContents, nColumn, styleButton);

		bool OldEnabled = GUI.enabled;
		if (m_bSaveDialog)
			GUI.enabled = false;

//		int	nObjectIndex	= GUI.SelectionGrid(gridRect, m_nObjectIndex, m_ObjectContents, nColumn, styleButton);
		int	nObjectIndex	= FXMakerLayout.TooltipSelectionGrid(FXMakerLayout.GetOffsetRect(GetPopupRect(), 0, -m_ObjectListScrollPos.y), listRect, gridRect, m_nObjectIndex, m_ObjectContents, nColumn, styleButton);

		if (GUI.changed && nObjectIndex != m_nObjectIndex)
			this.SetActiveObject(nObjectIndex);

		// Draw over
		DrawOverObjectList(gridRect, styleButton);

		if (m_bSaveDialog)
			GUI.enabled = OldEnabled;

		GUI.EndScrollView();
	}

	protected virtual void DrawOverObjectList(Rect gridRect, GUIStyle styleButton)
	{
	}

	// ==========================================================================================================
	protected virtual void DrawBottomRect(Rect baseRect)
	{
	}

	// ----------------------------------------------------------------------------------------------------------
	public override Rect GetPopupRect()
	{
		m_PopupPosition		= new Vector2(m_bShowLeftPopup ? 0 : Screen.width, 0);
		return GetPopupRectTop(((int)FXMakerLayout.GetFixedWindowWidth()*m_nPopupColumn), Screen.height);
	}

	protected virtual void LoadProjects()
	{
		m_ProjectFolerContents	= FXMakerEffect.inst.GetProjectContents();
		m_nProjectCount	= (m_ProjectFolerContents != null) ? m_ProjectFolerContents.Length : 0;
	}

	protected void SetProjectIndex(int nProjectIndex, bool bShowPopup)
	{
		if (nProjectIndex < 0 || m_nProjectCount <= nProjectIndex)
			nProjectIndex = 0;
		bool bChange = m_nProjectIndex != nProjectIndex;
		m_nProjectIndex	= nProjectIndex;

		this.LoadGroups();

		// Search DefaultDir
		if (m_nGroupIndex < 0)
		{
			for (m_nGroupIndex = 0; m_nGroupIndex < m_GroupFolderContents.Length; m_nGroupIndex++)
				if (m_GroupFolderContents[m_nGroupIndex].text == m_BaseDefaultGroupName)
					break;
		}
		SetGroupIndex((bChange && !bShowPopup) ? 0 : m_nGroupIndex);
	}

	protected virtual void LoadGroups()
	{
		// Load Group
		if (0 < m_nProjectCount)
		{
			string[] groupFolderStrings = NgAsset.GetFolderList(NgFile.CombinePath(FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.PROJECTS), m_ProjectFolerContents[m_nProjectIndex].text), null, null, 0, out m_nGroupCount);
			m_GroupFolderContents = NgConvert.StringsToContents(groupFolderStrings);
		}
	}

	protected void SetGroupIndex(int nGroupIndex)
	{
		if (nGroupIndex < 0 || m_nGroupCount <= nGroupIndex)
			nGroupIndex = 0;
		if (m_bOptionRecursively)
			nGroupIndex = -1;

		m_nGroupIndex	= nGroupIndex;

		// Load Prefab
		m_bInvokeLoadObject = true;
		Invoke("invokeLoadObjects", 0.05f);
		SavePrefs();
	}

	void invokeLoadObjects()
	{
		if (m_bInvokeLoadObject == false)
			return;
		m_bInvokeLoadObject = false;
		this.LoadObjects();
	}

	protected virtual void LoadObjects()
	{
	}

	protected virtual void SetActiveObject(int nObjectIndex)
	{
	}

	// Control Function -----------------------------------------------------------------
	protected bool IsReadOnlyFolder(string name)
	{
		return (0 < name.Length && name[0] == '[');
	}

	protected int IsReadOnlyFolder()
	{
		if (FXMakerLayout.m_bDevelopState == true)
			return 0;
		if (0 <= m_nProjectIndex && m_nProjectIndex < m_nProjectCount)
		{
			if (IsReadOnlyFolder(m_ProjectFolerContents[m_nProjectIndex].text))
				return 1;
		} else return -1;
		if (0 <= m_nGroupIndex && m_nGroupIndex < m_nGroupCount)
		{
			if (IsReadOnlyFolder(m_GroupFolderContents[m_nGroupIndex].text))
				return 1;
			return 0;
		}
		return 0;
	}

	// Event Function -------------------------------------------------------------------


	// -------------------------------------------------------------------------------------------
	protected virtual GUIContent GetHelpContent(string text)
	{
		return FXMakerTooltip.GetHcFolderPopup_Common(text);
	}
}
#endif
