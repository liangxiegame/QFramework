// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Collections;
using System.IO;

public class FXMakerBackground : MonoBehaviour
{
	public			static FXMakerBackground	inst;

	// const
	public			enum RESCLONE_TYPE	{LIGHTS=0, CAMERAS, COUNT};
	public			enum RESREFER_TYPE	{BACKGROUND1=0, BACKGROUND2, BACKGROUND3, BACKGROUND4, COUNT};
	public			const int			m_CloneTypeCount			= (int)RESCLONE_TYPE.COUNT;
	public			const int			m_ReferenceTypeCount		= (int)RESREFER_TYPE.COUNT;
	public static	string[]			m_ResourceSubDir			= {"Lights", "Cameras", "Background1", "Background2", "Background3", "Background4"};

	public			GameObject			m_DefaultBackgroundPrefab;		// 생성용 디폴트 prefab
	public			GameObject			m_CurrentBackgroundRoot;
// 	public			GameObject			m_TestCubeObject;
// 	public			GameObject			m_TestSphereObject;

	protected		bool				m_bGroupFoler				= false;
	protected		GUIContent[]		m_GroupFolerContents;
	protected		GUIContent[]		m_BackgroundContents;
	protected		GameObject[]		m_BackgroundPrefabs;
	protected		int					m_nGroupIndex;
	protected		int					m_nBackgroundIndex;
	protected		int					m_nGroupCount;
	protected		int					m_nBackgroundCount;
	protected		FxmInfoBackground	m_CurrentBackgroundInfo		= null;

	protected		GameObject[][]		m_ClonePrefabs				= new GameObject[m_CloneTypeCount][];
	protected		GUIContent[][]		m_CloneContents				= new GUIContent[m_CloneTypeCount][];
	protected		int[]				m_nClonePrefabSelIndex			= new int[m_CloneTypeCount];
	protected		Vector2[]			m_CloneWindowScrollPos		= new Vector2[m_CloneTypeCount];
	protected		bool				m_bProcessDelete			= false;

	protected		bool				m_bShowRightMenuPopup	= false;
	protected		int					m_nIndexRightMenuPopup	= -1;
	protected		string				m_EditingName			= "";

	// -------------------------------------------------------------------------------------------
	FXMakerBackground()
	{
		inst = this;
	}

	// -------------------------------------------------------------------------------------------
	public static Rect GetRect_Resources()
	{
		Rect	hintRect	= FXMakerLayout.GetZeroRect();
		for (int n = 0; n < m_ResourceSubDir.Length; n++)
		{
			Rect	rect	= FXMakerLayout.GetResListRect(n);
			rect.height		= 160;
			if (n == 0)
				hintRect = rect;
			else hintRect = FXMakerLayout.GetSumRect(hintRect, rect);
		}
		return hintRect;
	}

	// -------------------------------------------------------------------------------------------
	void Awake()
	{
		NgUtil.LogDevelop("Awake - FxmBackToolMain");
	}

	void Start()
	{
	}

	void OnEnable()
	{
		NgUtil.LogDevelop("OnEnable - FxmBackToolMain");
		if (FXMakerLayout.m_bMinimizeTopMenu)
			FXMakerLayout.m_bMinimizeTopMenu = false;
		LoadProject();
	}

	void OnDisable()
	{
// 		if (m_TestCubeObject != null)
// 			m_TestCubeObject.active = false;
// 		if (m_TestSphereObject != null)
// 			m_TestSphereObject.active = false;
	}

	// Update is called once per frame
	void Update()
	{
		// right menu
		CheckRightMenu();
	}

	Rect GetBackgroundPrefabRect(Rect base0Rect, int nPrefabIndex)
	{
		Rect	rect	= base0Rect;
		float	fWidth	= rect.x + rect.width;
		rect.x += Screen.width - fWidth * 2;
		rect.x += fWidth * (nPrefabIndex%2);
		rect.y += (160+4) * (nPrefabIndex/2);
		rect.height = 160;
		return rect;
	}

	// -------------------------------------------------------------------------------------------
	void OnGUI()
	{
		FXMakerMain.inst.OnGUIStart();
		// Menu Toolbar UI ------------------------------------------------------
		FXMakerMain.inst.AutoFocusWindow(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.TOP_CENTER), FXMakerLayout.GetMenuToolbarRect(), winMenuToolbar, "MainToolbar - Backgroud");

		// Menu Edit UI ---------------------------------------------------------
		FXMakerMain.inst.AutoFocusWindow(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.TOP_RIGHT), FXMakerLayout.GetMenuTopRightRect(), winMenuEdit, (m_CurrentBackgroundInfo != null ? m_BackgroundContents[m_nBackgroundIndex].text : "Not Select"));

		// List Window ---------------------------------------------
		if (m_CurrentBackgroundInfo != null)
		{
			for (int n = 0; n < m_CloneTypeCount+m_ReferenceTypeCount; n++)
			{
				GameObject	settingObj	= m_CurrentBackgroundInfo.GetChildObject(n);
				Rect		rect;
				
				if (n < m_CloneTypeCount)
				{
					rect = FXMakerLayout.GetResListRect(n);
					if (settingObj != null)
						rect.height = 160;
				} else {
					rect = GetBackgroundPrefabRect(FXMakerLayout.GetResListRect(0), n-m_CloneTypeCount);
				}
				FXMakerMain.inst.AutoFocusWindow(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.RESOURCE_START)+n, rect, winResourceList, m_ResourceSubDir[n]);
			}
		}

		FXMakerMain.inst.OnGUIEnd();
	}

	// -------------------------------------------------------------------------------------------
	public void ShowBackground(bool bShow, bool bSavePrefs)
	{
		if (bSavePrefs)
			EditorPrefs.SetBool("FXMakerGizmo.m_bBackground", bShow);
		m_CurrentBackgroundInfo.ShowBackground(bShow);
	}

	public void UpdateBackground()
	{
		bool bShow = EditorPrefs.GetBool("FXMakerGizmo.m_bBackground", true);
		m_CurrentBackgroundInfo.ShowBackground(bShow);
	}

	public void LoadProject()
	{
		NgUtil.LogDevelop("LoadProject - FxmBackMain");
		// group 폴더들 이름 불러오기
		if (m_bGroupFoler)
		{
			string[] groupFolerStrings = NgAsset.GetFolderList(FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.BACKGROUNDPREFABS), null, null, FXMakerOption.inst.m_nMinTopToolbarCount, out m_nGroupCount);
			m_GroupFolerContents = NgConvert.StringsToContents(groupFolerStrings);
			for (int n=0; n < m_GroupFolerContents.Length; n++)
			{
				if (m_GroupFolerContents[n].text == null)
					 m_GroupFolerContents[n].tooltip	= FXMakerTooltip.GetHsToolBackground("EMPTYGROUP_HOVER", FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.BACKGROUNDPREFABS));
				else m_GroupFolerContents[n].tooltip	= FXMakerTooltip.GetHsToolBackground("GROUP_HOVER", m_GroupFolerContents[n].text);
			}
		} else {
			m_GroupFolerContents	= new GUIContent[1];
			m_GroupFolerContents[0]	= new GUIContent("", "");
			m_nGroupCount			= 1;
		}

		// 미리 정의된 리소스 로드
		LoadResourceFolder();
		// 이전 선택정보 처리
		int	nGroupIndex			= 0;
		int	nBackgroundIndex	= 0;
		if (FXMakerLayout.m_bDevelopPrefs == false)
		{
			nGroupIndex			= UnityEditor.EditorPrefs.GetInt("FXMakerBackground.m_nGroupIndex"			, 0);
			nBackgroundIndex	= UnityEditor.EditorPrefs.GetInt("FXMakerBackground.m_nBackgroundIndex"	, 0);
		}
		SelectToolbar(nGroupIndex, nBackgroundIndex);

		// default Background
		if (m_CurrentBackgroundInfo == null)
			SelectToolbar(0, 0);
	}

	public void SaveProject()
	{
		if (this.enabled == false)
			return;
		if (m_CurrentBackgroundInfo != null)
			SaveBackgroundPrefab();
	}

	// group와 prefab path로 group 및 Background prefab index를 지정한다.
	void SelectToolbar(int nGroupIndex, int nBackgroundIndex)
	{
		if (m_nGroupCount <= nGroupIndex)
			nGroupIndex = m_nGroupCount-1;
		m_nGroupIndex	= nGroupIndex;

		if (0 <= m_nGroupIndex && m_nGroupIndex < m_nGroupCount)
		{
			LoadBackgroundFolder("");
			SetActiveBackground(nBackgroundIndex);
		} else {
			m_nGroupCount	= 0;
			m_nGroupIndex	= -1;
		}
	}
	void SelectToolbar(int nGroupIndex, string backgroundPrefabPath)
	{
		if (m_nGroupCount <= nGroupIndex)
			nGroupIndex = m_nGroupCount-1;
		m_nGroupIndex	= nGroupIndex;

		if (0 <= m_nGroupIndex && m_nGroupIndex < m_nGroupCount)
		{
			int nBackgroundIndex	= LoadBackgroundFolder(backgroundPrefabPath);
			SetActiveBackground(nBackgroundIndex);
		} else {
			m_nGroupCount	= 0;
			m_nGroupIndex	= -1;
		}
	}

	void SetActiveBackground(int nBackgroundIndex)
	{
		if (m_CurrentBackgroundInfo != null)
			DestroyImmediate(m_CurrentBackgroundInfo);

		if (0 < m_nBackgroundCount && (nBackgroundIndex < 0 || m_nBackgroundCount <= nBackgroundIndex))
			nBackgroundIndex = 0;
		m_nBackgroundIndex	= nBackgroundIndex;
		if (0 <= m_nBackgroundIndex && m_nBackgroundIndex < m_nBackgroundCount)
		{
			m_EditingName = m_BackgroundContents[m_nBackgroundIndex].text;
			SetCurrentBackgroundInfo(m_BackgroundPrefabs[m_nBackgroundIndex]);
		} else {
			m_EditingName = "";
			m_nBackgroundIndex	= -1;
			SetCurrentBackgroundInfo(null);
		}
		if (enabled)
			ShowBackground(true, true);
		else UpdateBackground();

		// 마지막 선택정보 저장
		UnityEditor.EditorPrefs.SetInt("FXMakerBackground.m_nGroupIndex", m_nGroupIndex);
		UnityEditor.EditorPrefs.SetInt("FXMakerBackground.m_nBackgroundIndex", m_nBackgroundIndex);
	}


	// BackgroundFolder내의 prefab list와 이름구성하기, selectPrefabPath를 지정할 경우 BackgroundIndex 리턴
	int LoadBackgroundFolder(string selectPrefabPath)
	{
// 		Debug.Log(selectPrefabPath);
		NgUtil.ClearObjects(m_BackgroundPrefabs);
		m_nBackgroundCount = 0;

		// load Group
		if (0 < m_nGroupCount)
		{
			// load Background Folder
			string	loaddir		= NgFile.CombinePath(FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.BACKGROUNDPREFABS), m_GroupFolerContents[m_nGroupIndex].text);
// 			Debug.Log(loaddir);
			m_BackgroundPrefabs	= NgAsset.GetPrefabList(loaddir, false, true, 0, out m_nBackgroundCount);
			m_BackgroundContents	= new GUIContent[Mathf.Max(m_nBackgroundCount, FXMakerOption.inst.m_nMinBottomToolbarCount)];
			for (int n=0; n < m_BackgroundPrefabs.Length; n++)
			{
				GUIContent econ = new GUIContent();
				econ.image		= FXMakerMain.inst.GetPrefabThumbTexture(m_BackgroundPrefabs[n]);
				econ.text		= m_BackgroundPrefabs[n].name;
				econ.tooltip	= FXMakerTooltip.GetHsToolBackground("BACKGROUND_HOVER", m_BackgroundPrefabs[n].name);
				m_BackgroundContents[n] = econ;
			}
			for (int n=m_BackgroundPrefabs.Length; n < FXMakerOption.inst.m_nMinBottomToolbarCount; n++)
			{
				GUIContent econ = new GUIContent();
				econ.image		= null;
				econ.text		= "";
				econ.tooltip	= FXMakerTooltip.GetHsToolBackground("EMPTYBACKGROUND_HOVER", "");
				m_BackgroundContents[n] = econ;
			}

			// select prefab
			if (selectPrefabPath != "")
				return NgAsset.FindPathIndex(loaddir, selectPrefabPath, "prefab");
		} else {
		}
		return -1;
	}

	// background에 사용될 리소스 목록 구성하기
	void LoadResourceFolder()
	{
		for (int n = 0; n < m_CloneTypeCount; n++)
		{
			string		strDir		= NgFile.CombinePath(FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.BACKGROUNDRESOURCES), m_ResourceSubDir[n]);
			int			nFileCount	= 0;

			m_ClonePrefabs[n]		= NgAsset.GetPrefabList(strDir, false, true, FXMakerLayout.m_nMaxResourceListCount, out nFileCount);
			m_CloneContents[n]		= new GUIContent[m_ClonePrefabs[n].Length];
		}
		BuildResourceContents();
	}

	void BuildResourceContents()
	{
		CancelInvoke("BuildResourceContents");
		if (enabled == false)
			return;
		int nNotLoadPreviewCount = 0;

		for (int n = 0; n < m_CloneTypeCount; n++)
		{
			for (int i=0; i < m_ClonePrefabs[n].Length; i++)
			{
				if (m_CloneContents[n][i] == null)
				{
					m_CloneContents[n][i]			= new GUIContent();
					m_CloneContents[n][i].text		= m_ClonePrefabs[n][i].name;
					m_CloneContents[n][i].tooltip	= FXMakerTooltip.GetHsToolBackground("RESOURCE_HOVER", m_ClonePrefabs[n][i].name);
				}
				if (m_CloneContents[n][i].image == null)
					m_CloneContents[n][i].image = FXMakerMain.inst.GetPrefabThumbTexture(m_ClonePrefabs[n][i]);
				if (m_CloneContents[n][i].image == null)
					nNotLoadPreviewCount++;
			}
		}
		if (0 < nNotLoadPreviewCount)
			Invoke("BuildResourceContents", FXMakerLayout.m_fReloadPreviewTime);
	}

	void SetCurrentBackgroundInfo(GameObject setPrefab)
	{
		// clear current
		NgObject.RemoveAllChildObject(m_CurrentBackgroundRoot, true);
		if (m_CurrentBackgroundInfo != null)
			DestroyImmediate(m_CurrentBackgroundInfo.gameObject);

		if (setPrefab != null)
		{
			GameObject obj = NgAsset.LoadPrefab(setPrefab, m_CurrentBackgroundRoot);
			obj.name = obj.name.Replace("(Clone)", "(Original)");
			m_CurrentBackgroundInfo = obj.GetComponent<FxmInfoBackground>();
			m_CurrentBackgroundInfo.SetActive();
			if (FXMakerMain.inst.IsLoadingProject() == false)
				FXMakerAsset.SetPingObject(setPrefab, m_CurrentBackgroundInfo);

			FXMakerMain.inst.GetFXMakerMouse().UpdateCamera();
		}
	}

    // ==========================================================================================================
	void winMenuToolbar(int id)
	{
		if (m_nGroupCount <= 0)
			return;

		// Group List
		int nGroupIndex	= 0;
		if (m_bGroupFoler)
		{
//			nGroupIndex		= GUI.Toolbar(FXMakerLayout.GetChildVerticalRect(FXMakerLayout.GetMenuToolbarRect(), 0, 10, 0, 3), m_nGroupIndex, m_GroupFolerContents);
			nGroupIndex		= FXMakerLayout.TooltipToolbar(FXMakerLayout.GetMenuToolbarRect(), FXMakerLayout.GetChildVerticalRect(FXMakerLayout.GetMenuToolbarRect(), 0, 10, 0, 3), m_nGroupIndex, m_GroupFolerContents);
			if (GUI.changed)
				SelectToolbar(nGroupIndex, "");

			// Draw line
			Rect lineRect = FXMakerLayout.GetChildVerticalRect(FXMakerLayout.GetMenuToolbarRect(), 0, 10, 3, 1);
			NgGUIDraw.DrawHorizontalLine(new Vector2(lineRect.x, lineRect.y), (int)lineRect.width, new Color(0.1f, 0.1f, 0.1f, 0.7f), 2, false);
		}

		// Background List
		Rect	gridRect;
		if (m_bGroupFoler)
			 gridRect	= FXMakerLayout.GetChildVerticalRect(FXMakerLayout.GetMenuToolbarRect(), 0, 10, 4, 6);
		else gridRect	= FXMakerLayout.GetChildVerticalRect(FXMakerLayout.GetMenuToolbarRect(), 0, 10, 0,10);
// 		int nSetIndex	= GUI.SelectionGrid(gridRect, Mathf.Min(m_nBackgroundIndex, m_nBackgroundCount-1), m_BackgroundContents, m_BackgroundContents.Length);
		int nSetIndex	= FXMakerLayout.TooltipSelectionGrid(FXMakerLayout.GetMenuToolbarRect(), gridRect, Mathf.Min(m_nBackgroundIndex, m_nBackgroundCount-1), m_BackgroundContents, m_BackgroundContents.Length);

		// select
		if (GUI.changed)
		{
			if (nSetIndex < m_nBackgroundCount)
			{
				NgUtil.LogDevelop("changed MenuToolbar = " + nGroupIndex + " " + nSetIndex);
				// Change Background Prefab
				if (0 <= nSetIndex)
				{
					// right button
					if (Input.GetMouseButtonUp(1))
						ShowRightMenu(nSetIndex, true);
					// active
					SelectToolbar(nGroupIndex, nSetIndex);
				} else {
				}
			} else {
				// right button
				if (Input.GetMouseButtonUp(1))
					ShowRightMenu(-1, false);
			}
		}
		FXMakerMain.inst.SaveTooltip();
	}

	void winMenuEdit(int id)
	{
		if (m_nGroupCount <= 0)
			return;

		// Change Group name
		if (0 <= m_nGroupIndex && m_nGroupIndex < m_nGroupCount)
		{
// 			string newName = GUI.TextField(gNcLayout.GetChildVerticalRect(gNcLayout.GetMenuTopRightRect(), 0, 2, 0), m_GroupFolerStrings[m_nGroupIndex], 50);
// 			newName = newName.Trim();
// 			if (newName != "" && m_GroupFolerStrings[m_nGroupIndex] != newName)
// 			{
// 				FileUtil.ReplaceDirectory(NgFile.CombinePath(m_RootBackgroundDir, m_GroupFolerStrings[m_nGroupIndex]), NgFile.CombinePath(m_RootBackgroundDir, newName));
// 				m_GroupFolerStrings[m_nGroupIndex]	= newName;
// 				FXMakerMain.inst.AssetDatabaseRefresh();
// 			}
		}

		// Add button
		if (m_nBackgroundCount < FXMakerOption.inst.m_nMinBottomToolbarCount)
		{
			if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(FXMakerLayout.GetChildVerticalRect(FXMakerLayout.GetMenuTopRightRect(), 0, 3, 0, 1), 2, 0, 1), FXMakerTooltip.GetHcToolBackground("New", NgFile.CombinePath(FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.BACKGROUNDPREFABS), m_GroupFolerContents[m_nGroupIndex].text))))
			{
				RenameCurrentPrefab(m_EditingName);
				NewPrefab();
				return;
			}
		}

		// Selected state
		if (m_CurrentBackgroundInfo != null)
		{
			// Delete button
			if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(FXMakerLayout.GetChildVerticalRect(FXMakerLayout.GetMenuTopRightRect(), 0, 3, 0, 1), 2, 1, 1), FXMakerTooltip.GetHcToolBackground("Del", m_BackgroundContents[m_nBackgroundIndex].text)))
			{
				RenameCurrentPrefab(m_EditingName);
				m_bProcessDelete = true;
			}

			if (m_bProcessDelete)
			{
				m_bProcessDelete = FxmPopupManager.inst.ShowModalOkCancelMessage("'" + m_BackgroundContents[m_nBackgroundIndex].text + "'\n" + FXMakerTooltip.GetHsToolMessage("DIALOG_DELETEPREFAB", ""));
				if (m_bProcessDelete == false)
				{
					if (FxmPopupManager.inst.GetModalMessageValue() == FXMakerLayout.MODALRETURN_TYPE.MODALRETURN_OK)
					{
						if (m_CurrentBackgroundInfo != null)
							DestroyImmediate(m_CurrentBackgroundInfo.gameObject);
						FXMakerAsset.DeleteEffectPrefab(m_BackgroundPrefabs[m_nBackgroundIndex]);
						SelectToolbar(m_nGroupIndex, m_nBackgroundIndex-1);
						return;
					}
				}
			}

			// Clone button
			if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(FXMakerLayout.GetChildVerticalRect(FXMakerLayout.GetMenuTopRightRect(), 0, 3, 1, 1), 2, 0, 1), FXMakerTooltip.GetHcToolBackground("Clone", m_BackgroundContents[m_nBackgroundIndex].text)))
			{
				RenameCurrentPrefab(m_EditingName);
				ClonePrefab();
				return;
			}

			// Thumb Selected
			if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(FXMakerLayout.GetChildVerticalRect(FXMakerLayout.GetMenuTopRightRect(), 0, 3, 1, 1), 2, 1, 1), FXMakerTooltip.GetHcToolBackground("Thumb", m_BackgroundContents[m_nBackgroundIndex].text)))
			{
				RenameCurrentPrefab(m_EditingName);
				ThumbPrefab();
				return;
			}

			// ChangeName
			if (m_CurrentBackgroundInfo != null && 0 <= m_nBackgroundIndex && m_nBackgroundIndex < m_nBackgroundCount)
			{
				GUI.SetNextControlName("TextField");
				m_EditingName = GUI.TextField(FXMakerLayout.GetChildVerticalRect(FXMakerLayout.GetMenuTopRightRect(), 0, 3, 2, 1), m_EditingName, 50);

				if (FXMakerMain.GetPrevWindowFocus() == (int)FXMakerLayout.WINDOWID.TOP_RIGHT && FXMakerMain.GetWindowFocus() != (int)FXMakerLayout.WINDOWID.TOP_RIGHT)
					RenameCurrentPrefab(m_EditingName);
			}
		}
		FXMakerMain.inst.SaveTooltip();
	}

	// ----------------------------------------------------------------------------------------------------------
	void ShowRightMenu(int nObjectIndex, bool bSelected)
	{
		Rect	toolbarRect	= FXMakerLayout.GetMenuToolbarRect();

		if (FxmPopupManager.inst.IsShowPopupExcludeMenu() || toolbarRect.Contains(FXMakerLayout.GetGUIMousePosition()) == false)
			return;

		m_bShowRightMenuPopup	= true;
		m_nIndexRightMenuPopup	= nObjectIndex;
		bool bNotReadOnly = true;
		FxmPopupManager.inst.ShowMenuPopup("Prefab", new string[]{"PingPrefab", "-", "New", "Delete", "Clone", "Thumb"},
				new bool  []{bSelected, false, bNotReadOnly, bSelected&&bNotReadOnly, bSelected&&bNotReadOnly, bSelected&&bNotReadOnly, bSelected&&bNotReadOnly});
	}

	void CheckRightMenu()
	{
		if (m_bShowRightMenuPopup)
		{
			if (FxmPopupManager.inst.IsShowCurrentMenuPopup() == false)
			{
				switch (FxmPopupManager.inst.GetSelectedIndex())
				{
					case 0:		PingPrefab();				break;
					case 1:		break;
					case 2:		NewPrefab();				break;
					case 3:		m_bProcessDelete = true;	break;
					case 4:		ClonePrefab();				break;
					case 5:		ThumbPrefab();				break;
				}
				m_bShowRightMenuPopup	= false;
			}
		}
	}

	void PingPrefab()
	{
		FXMakerAsset.SetPingObject(m_BackgroundPrefabs[m_nBackgroundIndex]);
	}

	void NewPrefab()
	{
		if (m_bGroupFoler == false ||  m_GroupFolerContents[m_nGroupIndex].text != "")
		{
			SaveProject();
			if (m_CurrentBackgroundInfo != null)
				DestroyImmediate(m_CurrentBackgroundInfo.gameObject);
			string createPath = NgAsset.CreateDefaultUniquePrefab(m_DefaultBackgroundPrefab, NgFile.CombinePath(FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.BACKGROUNDPREFABS), NgFile.CombinePath(m_GroupFolerContents[m_nGroupIndex].text, "NewBackground.prefab")));
			SaveProject();
			SelectToolbar(m_nGroupIndex, createPath);
		}
	}

	void ClonePrefab()
	{
		SaveProject();
		string createPath = FXMakerAsset.CloneEffectPrefab(m_BackgroundPrefabs[m_nBackgroundIndex]);
		SelectToolbar(m_nGroupIndex, createPath);
	}

	void ThumbPrefab()
	{
		FXMakerCapture.StartSaveBackThumb(NgAsset.GetPrefabThumbFilename(m_BackgroundPrefabs[m_nBackgroundIndex]));
	}

	// ==========================================================================================================
	void winResourceList(int id)
	{
		GUIStyle	labelStyle	= GUI.skin.GetStyle("BackMain_NotSelected");
		int			nWinIndex	= id-FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.RESOURCE_START);
		int			nBackIndex	= nWinIndex-m_CloneTypeCount;
		bool		bCloneType	= nWinIndex < m_CloneTypeCount;
		GameObject	settingObj	= (m_CurrentBackgroundInfo != null) ? m_CurrentBackgroundInfo.GetChildObject(nWinIndex) : null;

		// 현재 선택된 prefab
		if (settingObj != null)
		{
			GUIContent econ = new GUIContent();
			econ.image		= FXMakerMain.inst.GetPrefabThumbTexture(m_CurrentBackgroundInfo.GetClildThumbFilename(nWinIndex));
			econ.text		= settingObj.name;
			econ.tooltip	= bCloneType ? FXMakerTooltip.GetHsToolBackground("RES_CLONE_HOVER", settingObj.name) : FXMakerTooltip.GetHsToolBackground("RES_REFERENCE_HOVER", settingObj.name);
			// Current Selected
			if (settingObj != null && GUI.Button(new Rect(FXMakerLayout.m_rectInnerMargin.x, 20, FXMakerLayout.GetFixedWindowWidth()-FXMakerLayout.m_rectInnerMargin.x*2, FXMakerLayout.m_fScrollButtonHeight), econ))
				m_CurrentBackgroundInfo.SetPingObject(nWinIndex);

			// Clear Selected
			if (GUI.Button(new Rect(FXMakerLayout.m_rectInnerMargin.x, 20+FXMakerLayout.m_fScrollButtonHeight+3, FXMakerLayout.GetFixedWindowWidth()-FXMakerLayout.m_rectInnerMargin.x*2, 25), FXMakerTooltip.GetHcToolBackground("Clear Selected", settingObj.name)))
			{
				settingObj = null;
				if (bCloneType)
					m_CurrentBackgroundInfo.SetCloneObject(nWinIndex, null);
				else m_CurrentBackgroundInfo.SetReferenceObject(nBackIndex, null);
//				SaveBackgroundPrefab();
			}
			// Thumb Selected
			if (bCloneType && m_CurrentBackgroundInfo.GetClildThumbFilename(nWinIndex) != "" && GUI.Button(new Rect(FXMakerLayout.m_rectInnerMargin.x, 48+FXMakerLayout.m_fScrollButtonHeight+3, FXMakerLayout.GetFixedWindowWidth()-FXMakerLayout.m_rectInnerMargin.x*2, 25), FXMakerTooltip.GetHcToolBackground("Create Thumb", settingObj.name)))
			{
				FXMakerCapture.StartSaveBackThumb(m_CurrentBackgroundInfo.GetClildThumbFilename(nWinIndex));
				return;
			}
		} else {
			if (bCloneType)
			{
				string	strDir = NgFile.CombinePath(FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.BACKGROUNDRESOURCES), m_ResourceSubDir[nWinIndex]);
				GUI.Box(new Rect(FXMakerLayout.m_rectInnerMargin.x, 20, FXMakerLayout.GetFixedWindowWidth()-FXMakerLayout.m_rectInnerMargin.x*2, FXMakerLayout.m_fScrollButtonHeight), FXMakerTooltip.GetHcToolBackground("[Not Selected]", strDir), labelStyle);

				// list ----------------------------
				int		nNodeCount				= m_ClonePrefabs[nWinIndex].Length;
				Rect	listRect				= FXMakerLayout.GetChildVerticalRect(FXMakerLayout.GetResListRect(nWinIndex), 160, 1, 0, 1);
				Rect	scrollRect				= FXMakerLayout.GetScrollViewRect((int)listRect.width, nNodeCount, 1);
				Rect	gridRect				= FXMakerLayout.GetScrollGridRect((int)listRect.width, nNodeCount, 1);
				m_CloneWindowScrollPos[nWinIndex]	= GUI.BeginScrollView(listRect, m_CloneWindowScrollPos[nWinIndex], scrollRect);
//				m_nResourceSelIndex[nWinIndex]		= GUI.SelectionGrid(gNcLayout.GetChildVerticalRect(gNcLayout.GetResListRect(nWinIndex), 80, 1, 0, 1), m_nResourceSelIndex[nWinIndex], m_strResourceList[nWinIndex], 1);
//				m_nClonePrefabSelIndex[nWinIndex]	= GUI.SelectionGrid(gridRect, m_nClonePrefabSelIndex[nWinIndex], m_CloneContents[nWinIndex], 1);
				m_nClonePrefabSelIndex[nWinIndex]	= FXMakerLayout.TooltipSelectionGrid(FXMakerLayout.GetOffsetRect(FXMakerLayout.GetResListRect(nWinIndex), 0, -m_CloneWindowScrollPos[nWinIndex].y), listRect, gridRect, m_nClonePrefabSelIndex[nWinIndex], m_CloneContents[nWinIndex], 1);

				if (GUI.changed)
				{
					NgUtil.LogDevelop("changed m_nResourceSelIndex - nWinIndex = " + nWinIndex + ", value = " + m_nClonePrefabSelIndex[nWinIndex]);

					GameObject selPrefab = m_ClonePrefabs[nWinIndex][m_nClonePrefabSelIndex[nWinIndex]];
					m_CurrentBackgroundInfo.SetCloneObject(nWinIndex, selPrefab);
//					SaveBackgroundPrefab();
				}
				GUI.EndScrollView();
			}
		}

		// select prefab
		if (bCloneType == false)
		{
			Rect subRect = new Rect(FXMakerLayout.m_rectInnerMargin.x, 48+FXMakerLayout.m_fScrollButtonHeight+3, FXMakerLayout.GetFixedWindowWidth()-FXMakerLayout.m_rectInnerMargin.x*2, 25);
			if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(subRect, 2, 0, 1), FXMakerTooltip.GetHcToolBackground("Select", "")))
			{
				FxmPopupManager.inst.ShowSelectPrefabPopup(m_CurrentBackgroundInfo, false, nBackIndex);
//				SaveBackgroundPrefab();
			}
			if (NgLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(subRect, 2, 1, 1), FXMakerTooltip.GetHcToolBackground("Open", ""), m_CurrentBackgroundInfo.GetReferenceObject(nBackIndex) != null))
			{
				FXMakerEffect.inst.LoadProject(AssetDatabase.GetAssetPath(m_CurrentBackgroundInfo.GetReferenceObject(nBackIndex)));
				FXMakerMain.inst.SetActiveTool(1);
//				SaveBackgroundPrefab();
			}
		}

		FXMakerMain.inst.SaveTooltip();
	}

	public void SaveBackgroundPrefab()
	{
		NgUtil.LogDevelop("SaveBackgroundPrefab - FxmBack prefab");
		m_BackgroundPrefabs[m_nBackgroundIndex] = FXMakerAsset.SaveEffectPrefab(m_CurrentBackgroundInfo.gameObject, m_BackgroundPrefabs[m_nBackgroundIndex]);
		ShowBackground(true, true);
	}

	void RenameCurrentPrefab(string newName)
	{
		if (newName != m_BackgroundContents[m_nBackgroundIndex].text)
		{
			newName = newName.Trim();
			if (newName != "")
			{
				SaveProject();
				m_BackgroundContents[m_nBackgroundIndex].text	= newName;
				m_BackgroundPrefabs[m_nBackgroundIndex].name	= newName;
				m_CurrentBackgroundInfo.gameObject.name			= newName;
				FXMakerAsset.RenameEffectPrefab(m_CurrentBackgroundInfo.gameObject, m_BackgroundPrefabs[m_nBackgroundIndex], newName);
			}
		}
	}
}

#endif
