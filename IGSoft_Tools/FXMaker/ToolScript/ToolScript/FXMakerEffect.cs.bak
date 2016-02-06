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
using System.Collections.Generic;

public class FXMakerEffect : MonoBehaviour
{
	public			static FXMakerEffect	inst;

	// const
	protected		const int			m_nGuiTopHeight			= 80;

	public			GameObject			m_DefaultEmptyPrefab;
	public			GameObject			m_DefaultPlanePrefab;
	public			GameObject			m_DefaultLegacyPrefab;
	public			GameObject			m_DefaultShurikenPrefab;
	public			GameObject			m_CurrentEffectRoot;
	protected		bool				m_bChangePrefab			= false;

	// 자동 설정
	protected		GUIContent[]		m_ProjectFolerContents;
	protected		GUIContent[]		m_GroupFolderContents;	// = new string[gNcLayout.m_nMaxTop2RowToolbarCount];
//	protected		string[]			m_EffectNameStrings;	// = new string[gNcLayout.m_nMaxEffectListCount];
	protected		GUIContent[]		m_EffectContents;		// = new GUIContent[gNcLayout.m_nMaxEffectListCount];
	protected		GameObject[]		m_EffectPrefabs;		// = new GameObject[gNcLayout.m_nMaxEffectListCount];
	protected		string				m_LoadDirectory;
	protected		string				m_DefaultProjectName	= "[EffectSample]";

	protected		int					m_nScrollColumn			= 3;
	protected		float				m_nMaxObjectColumn		= 6;
	protected		bool				m_bMinimize				= false;
	protected		bool				m_bMinimizeAll			= true;

	protected		int					m_nProjectIndex;
	protected		int[]				m_nGroupIndexs;
	protected		int					m_nEffectIndex;
	protected		int					m_nProjectCount;
	protected		int					m_nGroupCount;
	protected		int					m_nEffectCount;
	protected		int					m_nCameraAngleXIndex	= 1;
	protected		int					m_nCameraAngleYIndex	= 3;

	protected		Vector2				m_EffectListScrollPos;
	protected		bool				m_bProcessDelete		= false;
	protected		bool				m_bProcessDelSprite		= false;
	protected		bool				m_bShowRightMenuPopup	= false;
	protected		int					m_nIndexRightMenuPopup	= -1;
	protected		string				m_EditingName			= "";

	public			enum				NEW_TYPE {NEW_EMPTY, NEW_PLANE, NEW_LEGACY, NEW_SHURIKEN};
	protected		bool				m_bShowNewMenuPopup		= false;

	// -------------------------------------------------------------------------------------------
	FXMakerEffect()
	{
		inst = this;
	}

	// -------------------------------------------------------------------------------------------
	public static Rect GetWindowRect()
	{
		return FXMakerLayout.GetEffectListRect();
	}

	public static Rect GetRect_Group()
	{
		Rect groupRect  = FXMakerLayout.GetChildVerticalRect(FXMakerLayout.GetMenuToolbarRect(), 0, 10, 4, 6);
		return FXMakerLayout.GetOffsetRect(groupRect, FXMakerLayout.GetMenuToolbarRect().x, FXMakerLayout.GetMenuToolbarRect().y);
	}

	int GetGroupIndex()
	{
		if (0 <= m_nProjectIndex && m_nProjectIndex < m_nProjectCount)
		{
			if (m_nGroupIndexs == null || m_nGroupIndexs.Length != m_nProjectCount)
			{
				m_nGroupIndexs = new int[m_nProjectCount];
				for (int n = 0; n < m_nGroupIndexs.Length; n++)
					m_nGroupIndexs[n] = -1;
			}
			if (m_nGroupCount <= m_nGroupIndexs[m_nProjectIndex])
				m_nGroupIndexs[m_nProjectIndex] = m_nGroupCount-1;
			if (0 <= m_nGroupIndexs[m_nProjectIndex] && m_nGroupIndexs[m_nProjectIndex] < m_nGroupCount)
				return m_nGroupIndexs[m_nProjectIndex];
		}
		return -1;
	}

	int GetGroupIndex(int nProjectIndex)
	{
		GetGroupIndex();
		return m_nGroupIndexs[nProjectIndex];
	}

	void SetGroupIndex(int nIndex)
	{
		GetGroupIndex();
		m_nGroupIndexs[m_nProjectIndex] = nIndex;
	}

	// -------------------------------------------------------------------------------------------
	void Awake()
	{
		NgUtil.LogDevelop("Awake - FXMakerMain");
		if (FXMakerLayout.m_bDevelopPrefs == false)
		{
			m_nCameraAngleXIndex	= EditorPrefs.GetInt("FXMakerEffect.m_nCameraAngleXIndex", m_nCameraAngleXIndex);
			m_nCameraAngleYIndex	= EditorPrefs.GetInt("FXMakerEffect.m_nCameraAngleYIndex", m_nCameraAngleYIndex);
		}
//		FXMakerLayout.m_bMinimizeTopMenu	= EditorPrefs.GetBool("FXMakerEffect.m_bMinimize", FXMakerLayout.m_bMinimizeTopMenu);
	}

	void OnEnable()
	{
		NgUtil.LogDevelop("OnEnable - FXMakerMain");
		LoadProject();
	}

	void Start()
	{
		FXMakerMain.inst.GetFXMakerMouse().ChangeAngle(FXMakerOption.inst.m_nCameraAangleXValues[m_nCameraAngleXIndex], FXMakerOption.inst.m_nCameraAangleYValues[m_nCameraAngleYIndex]);
	}

	// Update is called once per frame
	void Update()
	{
		// ShotKey - Delete
		if (FXMakerMain.inst.GetFocusInputKey(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.EFFECT_LIST)) == KeyCode.Delete && (IsReadOnlyFolder() == 0))
		{
			if (0 <= m_nEffectIndex && m_nEffectIndex < m_nEffectCount)
				m_bProcessDelete = true;
			FXMakerMain.inst.SetFocusInputKey(0);
		}

		// new menu
		CheckNewMenu();
		// right menu
		CheckRightMenu();
	}

	// -------------------------------------------------------------------------------------------
	void OnGUI()
	{
		FXMakerMain.inst.OnGUIStart();
		Rect rect;

		// Menu Toolbar UI ------------------------------------------------------
		FXMakerMain.inst.AutoFocusWindow(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.TOP_CENTER), FXMakerLayout.GetMenuToolbarRect(), winMenuToolbar, "MainToolbar - ProjectFolder");

		// Menu Camera UI -------------------------------------------------------
		FXMakerMain.inst.AutoFocusWindow(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.TOP_RIGHT), FXMakerLayout.GetMenuTopRightRect(), winTopRight, "Camera");

		// Effect List Window ---------------------------------------------------
		rect = FXMakerLayout.GetEffectListRect();
		if (FXMakerLayout.m_bMinimizeAll || m_bMinimize)
			rect.height = FXMakerLayout.m_MinimizeHeight;
		FXMakerMain.inst.AutoFocusWindow(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.EFFECT_LIST), rect, winEffectList, "PrefabList");

		FXMakerMain.inst.OnGUIEnd();
	}

	// -------------------------------------------------------------------------------------------
	bool IsReadOnlyFolder(string name)
	{
		return (0 < name.Length && name[0] == '[');
	}

	public int IsReadOnlyFolder()
	{
		if (FXMakerLayout.m_bDevelopState == true)
			return 0;
		if (0 <= m_nProjectIndex && m_nProjectIndex < m_nProjectCount)
		{
			if (IsReadOnlyFolder(m_ProjectFolerContents[m_nProjectIndex].text))
				return 1;
		} else return -1;
		if (0 <= GetGroupIndex())
		{
			if (IsReadOnlyFolder(m_GroupFolderContents[GetGroupIndex()].text))
				return 1;
			return 0;
		}
		return 0;
	}

	public void LoadProject()
	{
		LoadProject("");
	}

	public void LoadProject(string defaultEffectPath)
	{
// 		Debug.Log(defaultEffectPath);
		NgUtil.LogDevelop("LoadProject - FXMakerMain");

		// clear current
		FXMakerMain.inst.ClearCurrentEffectObject(m_CurrentEffectRoot, true);

		// load Project
		string[] projectFolerStrings	= NgAsset.GetFolderList(FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.PROJECTS), null, null, FXMakerOption.inst.m_nMinTopToolbarCount, out m_nProjectCount);
		m_ProjectFolerContents = NgConvert.StringsToContents(projectFolerStrings);
		for (int n=0; n < m_ProjectFolerContents.Length; n++)
		{
			if (m_ProjectFolerContents[n].text == null)
				 m_ProjectFolerContents[n].tooltip	= FXMakerTooltip.GetHsToolEffect("EMPTYPROJECT_HOVER", FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.PROJECTS));
			else {
				if (IsReadOnlyFolder(m_ProjectFolerContents[n].text))
					 m_ProjectFolerContents[n].tooltip	= FXMakerTooltip.GetHsToolEffect("PROJECT_HOVER", NgFile.CombinePath(FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.PROJECTS), m_ProjectFolerContents[n].text) + "\n" + FXMakerTooltip.GetHsToolMessage("FOLDER_READONLY", ""));
				else m_ProjectFolerContents[n].tooltip	= FXMakerTooltip.GetHsToolEffect("PROJECT_HOVER", NgFile.CombinePath(FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.PROJECTS), m_ProjectFolerContents[n].text));
			}
		}

		int	nProjectIndex	= -1;
		int	nGroupIndex		= 0;
		int	nEffectIndex	= 0;
		// 이전 선택정보 처리
		if (FXMakerLayout.m_bDevelopPrefs == false)
		{
			nProjectIndex	= UnityEditor.EditorPrefs.GetInt("FXMakerEffect.m_nProjectIndex"	, nProjectIndex);
			nGroupIndex		= UnityEditor.EditorPrefs.GetInt("FXMakerEffect.m_nGroupIndex"		, nGroupIndex);
			nEffectIndex	= UnityEditor.EditorPrefs.GetInt("FXMakerEffect.m_nEffectIndex"		, nEffectIndex);
			m_nScrollColumn	= UnityEditor.EditorPrefs.GetInt("FXMakerEffect.m_nScrollColumn"	, m_nScrollColumn);
		}

		// defaultEffectPath
		if (defaultEffectPath != null && defaultEffectPath != "")
		{
			string projectPath	= defaultEffectPath.Replace(FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.PROJECTS), "");
			string projectName	= NgFile.GetSplit(projectPath, 1);
			string groupName	= NgFile.GetSplit(projectPath, 2);

			if (projectPath.Contains("Assets/Resources") && projectName == "Resources" && projectPath.Contains("Res.."))
				projectName	= "3 SkyWing";

			for (nProjectIndex = 0; nProjectIndex < m_ProjectFolerContents.Length; nProjectIndex++)
				if (m_ProjectFolerContents[nProjectIndex].text == projectName)
					break;
			 SelectToolbar(nProjectIndex, groupName, defaultEffectPath);
		} else {
			// Search DefaultDir
			if (nProjectIndex < 0)
			{
				for (nProjectIndex = 0; nProjectIndex < m_ProjectFolerContents.Length; nProjectIndex++)
					if (m_ProjectFolerContents[nProjectIndex].text == m_DefaultProjectName)
						break;
			}
			SelectToolbar(nProjectIndex, nGroupIndex, nEffectIndex);
		}
	}

	public bool SaveProject()
	{
		return SaveProject(false);
	}

	public bool SaveProject(bool bClickSaveButton)
	{
		if (bClickSaveButton == false && m_bChangePrefab == false)
			return false;
		if (FXMakerMain.inst.GetOriginalEffectObject() == null || FXMakerMain.inst.GetOriginalEffectPrefab() == null)
			return false;

		if (0 < IsReadOnlyFolder())
		{
			NgUtil.LogMessage(FXMakerTooltip.GetHsToolMessage("READONLY_NOTSAVE", ""));
			m_bChangePrefab = false;
		} else {
			if (0 <= m_nEffectIndex && m_nEffectIndex < m_nEffectCount)
			{
				NgUtil.LogDevelop("SaveProject - FXMaker prefab");

				FXMakerMain.inst.GetOriginalEffectObject().transform.parent = null;
				ChangeActiveState(FXMakerMain.inst.GetOriginalEffectObject());
				FXMakerMain.inst.GetOriginalEffectObject().transform.parent = m_CurrentEffectRoot.transform;

				m_EffectPrefabs[m_nEffectIndex] = FXMakerAsset.SaveEffectPrefab(FXMakerMain.inst.GetOriginalEffectObject(), FXMakerMain.inst.GetOriginalEffectPrefab());
				// clear current
				FXMakerMain.inst.ClearCurrentEffectObject(m_CurrentEffectRoot, true);
				SetCurrentEffectPrefab(m_EffectPrefabs[m_nEffectIndex]);
				// Baground Reload
				FXMakerBackground.inst.UpdateBackground();

				m_bChangePrefab = false;
				return true;
			}
		}
		return false;
	}

	void ChangeActiveState(GameObject oriEffectObject)
	{
		bool bObjEnable = (oriEffectObject.GetComponent<NcDontActive>() == null);
		if (bObjEnable)
		{
			NgObject.SetActive(oriEffectObject, bObjEnable);
		}
		foreach (Transform trans in oriEffectObject.transform)
			ChangeActiveState(trans.gameObject);
	}

	public void SetChangePrefab()
	{
		m_bChangePrefab = true;
	}

	// group와 prefab path로 group 및 Effect prefab index를 지정한다.
	void SelectToolbar(int nProjectIndex, int nGroupIndex, int nEffectIndex)
	{
		if (LoadGroup(nProjectIndex, nGroupIndex, ""))
 			SetActiveEffect(nEffectIndex);
	}

	void SelectToolbar(int nProjectIndex, int nGroupIndex, string EffectPrefabPath)
	{
		if (LoadGroup(nProjectIndex, nGroupIndex, ""))
		{
			int nEffectIndex	= LoadEffectFolder(EffectPrefabPath);
			SetActiveEffect(nEffectIndex);
		}
	}

	void SelectToolbar(int nProjectIndex, string groupName, string EffectPrefabPath)
	{
		if (LoadGroup(nProjectIndex, 0, groupName))
		{
			int nEffectIndex	= LoadEffectFolder(EffectPrefabPath);
			SetActiveEffect(nEffectIndex);
		}
	}

	bool LoadGroup(int nProjectIndex, int nGroupIndex, string groupName)
	{
		if (m_nProjectCount <= nProjectIndex)
			nProjectIndex = m_nProjectCount-1;
		m_nProjectIndex	= nProjectIndex;

		// load Group
		if (0 <= m_nProjectIndex && m_nProjectIndex < m_nProjectCount)
		{
			string[] groupFolderStrings = NgAsset.GetFolderList(NgFile.CombinePath(FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.PROJECTS), m_ProjectFolerContents[m_nProjectIndex].text), null, null, FXMakerOption.inst.m_nMinBottomToolbarCount, out m_nGroupCount);
			m_GroupFolderContents = NgConvert.StringsToContents(groupFolderStrings);
			if (m_GroupFolderContents != null)
				for (int n=0; n < m_GroupFolderContents.Length; n++)
				{
					if (groupName != "" && m_GroupFolderContents[n].text == groupName)
						nGroupIndex = n;
					if (m_GroupFolderContents[n].text == null)
						 m_GroupFolderContents[n].tooltip	= FXMakerTooltip.GetHsToolEffect("EMPTYGROUP_HOVER", NgFile.CombinePath(FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.PROJECTS), m_ProjectFolerContents[m_nProjectIndex].text));
					else {
						if (IsReadOnlyFolder(m_ProjectFolerContents[m_nProjectIndex].text) || IsReadOnlyFolder(m_GroupFolderContents[n].text))
							 m_GroupFolderContents[n].tooltip	= FXMakerTooltip.GetHsToolEffect("GROUP_HOVER", NgFile.CombinePath(FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.PROJECTS), m_ProjectFolerContents[m_nProjectIndex].text, m_GroupFolderContents[n].text) + "\n" + FXMakerTooltip.GetHsToolMessage("FOLDER_READONLY", ""));
						else m_GroupFolderContents[n].tooltip	= FXMakerTooltip.GetHsToolEffect("GROUP_HOVER", NgFile.CombinePath(FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.PROJECTS), m_ProjectFolerContents[m_nProjectIndex].text, m_GroupFolderContents[n].text));
					}
				}

			SetGroupIndex(nGroupIndex);
 			LoadEffectFolder("");
			return true;
		} else {
			m_nGroupCount	= 0;
			SetGroupIndex(-1);
			return false;
		}
	}

	void SetActiveEffect(int nEffectIndex)
	{
		// current save
		if (FXMakerMain.inst.IsCurrentEffectObject())
		{
			if (SaveProject())
				NgUtil.LogMessage(FXMakerTooltip.GetHsToolMessage("TOOL_AUTOSAVE", ""));
		}

		// clear current
		FXMakerMain.inst.ClearCurrentEffectObject(m_CurrentEffectRoot, true);

		m_EditingName	= "";
		m_nEffectIndex	= nEffectIndex;
		if (0 <= m_nEffectIndex && m_nEffectIndex < m_nEffectCount)
		{
			// Create to Hierarchy
			GameObject setPrefab = m_EffectPrefabs[m_nEffectIndex];
			if (setPrefab != null)
			{
				m_EditingName	= m_EffectPrefabs[m_nEffectIndex].name;
				SetCurrentEffectPrefab(setPrefab);
			}
		} else {
			m_nEffectIndex	= -1;
		}
		// 마지막 선택정보 저장
		UnityEditor.EditorPrefs.SetInt("FXMakerEffect.m_nProjectIndex"	, m_nProjectIndex);
		UnityEditor.EditorPrefs.SetInt("FXMakerEffect.m_nGroupIndex"	, GetGroupIndex());
		UnityEditor.EditorPrefs.SetInt("FXMakerEffect.m_nEffectIndex"	, m_nEffectIndex);
	}

	void SetCurrentEffectPrefab(GameObject setPrefab)
	{
		m_bChangePrefab = false;
		FXMakerMain.inst.SetCurrentEffectPrefab(setPrefab, m_CurrentEffectRoot, false);
	}

	// EffectFolder내의 prefab파일 구성하기, selectPrefabPath를 지정할 경우 EffectIndex 리턴
	int LoadEffectFolder(string selectPrefabPath)
	{
		NgUtil.ClearObjects(m_EffectPrefabs);
		m_nEffectCount = 0;

		// load Group
		if (0 < m_nGroupCount)
		{
			if (GetGroupIndex() < 0)
				SetGroupIndex(0);

			// load Effect Folder
			string	loaddir		= GetCurrentDirPath();
			m_LoadDirectory		= NgFile.PathSeparatorNormalize(loaddir);
			m_EffectPrefabs		= NgAsset.GetPrefabList(m_LoadDirectory, false, true, FXMakerLayout.m_nMaxPrefabListCount, out m_nEffectCount);
//			m_EffectNameStrings	= new string[m_nEffectCount];
			m_EffectContents	= new GUIContent[m_nEffectCount];
			BuildContents();

			// select prefab
			if (selectPrefabPath != "")
				return NgAsset.FindPathIndex(loaddir, selectPrefabPath, "prefab");
		} else
		{
		}
		return -1;
	}

	void BuildContents()
	{
		CancelInvoke("BuildContents");
		if (enabled == false)
			return;

		int nNotLoadPreviewCount = 0;
		for (int n=0; n < m_nEffectCount; n++)
		{
			if (m_EffectContents[n] == null)
			{
				string subDir = AssetDatabase.GetAssetPath(m_EffectPrefabs[n]);
				subDir = NgFile.PathSeparatorNormalize(subDir).Replace(m_LoadDirectory, "");

				m_EffectContents[n]			= new GUIContent();
				m_EffectContents[n].text	= m_EffectPrefabs[n].name;
				m_EffectContents[n].tooltip	= FXMakerTooltip.GetHsToolEffect("EFFECT_HOVER", subDir);
			}
			if (m_EffectContents[n].image == null)
				m_EffectContents[n].image	= FXMakerMain.inst.GetPrefabThumbTexture(m_EffectPrefabs[n]);
			if (m_EffectContents[n].image == null)
				nNotLoadPreviewCount++;
		}
//		Debug.Log("BuildContents - " + nNotLoadPreviewCount);
		if (0 < nNotLoadPreviewCount)
			Invoke("BuildContents", FXMakerLayout.m_fReloadPreviewTime);
	}

// 	void SaveCurrentEffectObject()
// 	{
// 		// clear current
// 		if (FXMakerMain.inst.IsCurrentEffectObject() && FXMakerMain.inst.GetOriginalEffectPrefab() != null)
// 		{
// 			FXMakerMain.inst.SavePrefab(FXMakerMain.inst.GetOriginalEffectObject(), FXMakerMain.inst.GetOriginalEffectPrefab());
// 		} else {
// 			FxmPopupManager.inst.ShowToolMessage("SaveCurrentEffectObject - Not Selected");
// 		}
// 	}

	public GUIContent[] GetProjectContents()
	{
		if (m_nProjectCount <= 0)
			return null;
		GUIContent[] cons = new GUIContent[m_nProjectCount];
		System.Array.Copy(m_ProjectFolerContents, cons, m_nProjectCount);
		return cons;
	}
	// ==========================================================================================================
	void winMenuToolbar(int id)
	{
		if (m_nProjectCount <= 0)
			return;

		bool		bChanged	= false;
		Rect		popupRect	= FXMakerLayout.GetMenuToolbarRect();

		// window desc -----------------------------------------------------------
		FXMakerTooltip.WindowDescription(popupRect, FXMakerLayout.WINDOWID.TOP_CENTER, null);


		// mini ----------------------------------------------------------------
		FXMakerLayout.m_bMinimizeTopMenu = GUI.Toggle(new Rect(3, 1, FXMakerLayout.m_fMinimizeClickWidth, FXMakerLayout.m_fMinimizeClickHeight), FXMakerLayout.m_bMinimizeTopMenu, "Mini");
// 		if (GUI.changed)
// 			EditorPrefs.SetBool("FXMakerEffect.m_bMinimize", FXMakerLayout.m_bMinimizeTopMenu);
		FXMakerLayout.m_bMinimizeAll	= GUI.Toggle(new Rect(popupRect.width-60, 1, FXMakerLayout.m_fMinimizeClickWidth, FXMakerLayout.m_fMinimizeClickHeight), FXMakerLayout.m_bMinimizeAll, "MiniAll");
		GUI.changed = false;
		if (FXMakerLayout.m_bMinimizeAll || FXMakerLayout.m_bMinimizeTopMenu)
		{
			FXMakerMain.inst.SaveTooltip();
			return;
		}

//		FXMakerMain.inst.ToggleGlobalLangSkin(true);
		// Group Project
// 		int nProjectIndex	= GUI.Toolbar(FXMakerLayout.GetChildVerticalRect(popupRect, 0, 10, 0, 3), m_nProjectIndex, m_ProjectFolerContents);
		int nProjectIndex	= FXMakerLayout.TooltipToolbar(popupRect, FXMakerLayout.GetChildVerticalRect(popupRect, 0, 10, 0, 3), m_nProjectIndex, m_ProjectFolerContents);
		if (GUI.changed)
			bChanged = true;

		// Draw line
		Rect lineRect = FXMakerLayout.GetChildVerticalRect(popupRect, 0, 10, 3, 1);
		NgGUIDraw.DrawHorizontalLine(new Vector2(lineRect.x, lineRect.y+2), (int)lineRect.width, new Color(0.1f, 0.1f, 0.1f, 0.7f), 2, false);

		// Group List
		Rect groupRect  = FXMakerLayout.GetChildVerticalRect(popupRect, 0, 10, 4, 6);
//		int nGroupIndex	= GUI.Toolbar(gNcLayout.GetChildVerticalRect(gNcLayout.GetMenuToolbarRect(), 0, 10, 4, 6), GetGroupIndex(), m_GroupFolderStrings);
//		int nGroupIndex	= GUI.SelectionGrid(groupRect, GetGroupIndex(), m_GroupFolderContents, m_GroupFolderContents.Length/2+m_GroupFolderContents.Length%2);
		int nGroupIndex	= FXMakerLayout.TooltipSelectionGrid(popupRect, groupRect, GetGroupIndex(), m_GroupFolderContents, m_GroupFolderContents.Length/2+m_GroupFolderContents.Length%2);

		if (GUI.changed)
			bChanged = true;
//		FXMakerMain.inst.ToggleGlobalLangSkin(false);

		if (bChanged)
		{
			NgUtil.LogDevelop("changed MenuToolbar = " + nProjectIndex + " " + nGroupIndex);
			SaveProject();

			if (m_nProjectCount <= nProjectIndex)
			{
				nProjectIndex = m_nProjectCount-1;
				m_nProjectIndex	= nProjectIndex;
			}
			SelectToolbar(nProjectIndex, (m_nProjectIndex != nProjectIndex ? GetGroupIndex(nProjectIndex) : nGroupIndex), "");
		}

		FXMakerMain.inst.SaveTooltip();
	}

	void winTopRight(int id)
	{
		if ((FXMakerLayout.m_bMinimizeAll || FXMakerLayout.m_bMinimizeTopMenu) == false)
		{

			Rect	gridRectx		= FXMakerLayout.GetChildVerticalRect(FXMakerLayout.GetMenuTopRightRect(), 0, 2, 0, 1);
			Rect	gridRecty		= FXMakerLayout.GetChildVerticalRect(FXMakerLayout.GetMenuTopRightRect(), 0, 2, 1, 1);
			m_nCameraAngleXIndex	= FXMakerLayout.TooltipSelectionGrid(FXMakerLayout.GetMenuTopRightRect(), gridRectx, m_nCameraAngleXIndex, FXMakerTooltip.GetHcToolEffect_CameraX(), 4);
			m_nCameraAngleYIndex	= FXMakerLayout.TooltipSelectionGrid(FXMakerLayout.GetMenuTopRightRect(), gridRecty, m_nCameraAngleYIndex, FXMakerTooltip.GetHcToolEffect_CameraY(), 4);

			if (GUI.changed)
			{
				FXMakerMain.inst.GetFXMakerMouse().ChangeAngle(FXMakerOption.inst.m_nCameraAangleXValues[m_nCameraAngleXIndex], FXMakerOption.inst.m_nCameraAangleYValues[m_nCameraAngleYIndex]);
				
				// 마지막 선택정보 저장
				UnityEditor.EditorPrefs.SetInt("FXMakerEffect.m_nCameraAngleXIndex", m_nCameraAngleXIndex);
				UnityEditor.EditorPrefs.SetInt("FXMakerEffect.m_nCameraAngleYIndex", m_nCameraAngleYIndex);
			}
		}
		FXMakerMain.inst.SaveTooltip();
	}

	// ==========================================================================================================
	void winEffectList(int id)
	{
		if (GetGroupIndex() < 0)
			return;

		Rect	effectRect	= FXMakerLayout.GetEffectListRect();

		// window desc -----------------------------------------------------------
		FXMakerTooltip.WindowDescription(effectRect, FXMakerLayout.WINDOWID.EFFECT_LIST, null);

		// mini ----------------------------------------------------------------
		m_bMinimize = GUI.Toggle(new Rect(3, 1, FXMakerLayout.m_fMinimizeClickWidth, FXMakerLayout.m_fMinimizeClickHeight), m_bMinimize, "Mini");
		GUI.changed = false;
		if (FXMakerLayout.m_bMinimizeAll || m_bMinimize)
		{
			RenameCurrentPrefab(m_EditingName);
			FXMakerMain.inst.SaveTooltip();
			return;
		}

		// 기능 버튼 -----------------------------------------------------------
		Rect	rect1Row	= new Rect(FXMakerLayout.m_rectInnerMargin.x, 20, effectRect.width-FXMakerLayout.m_rectInnerMargin.x*2, 25);
		Rect	rect2Row	= new Rect(FXMakerLayout.m_rectInnerMargin.x, 50, effectRect.width-FXMakerLayout.m_rectInnerMargin.x*2, 20);
		Rect	rect3Row	= new Rect(FXMakerLayout.m_rectInnerMargin.x, 75, effectRect.width-FXMakerLayout.m_rectInnerMargin.x*2, 12);

		// Add button
		if (m_nEffectCount < FXMakerLayout.m_nMaxPrefabListCount)
		{
			if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(rect1Row, 5, 0, 1), FXMakerTooltip.GetHcToolEffect("New"), IsReadOnlyFolder() == 0))
			{
				RenameCurrentPrefab(m_EditingName);
				ShowNewMenu();
				return;
			}
		}

		if (m_nEffectCount <= 0)
		{
			// right button
			if (Input.GetMouseButtonUp(1))
				ShowRightMenu(-1, false);
			return;
		}

		// Selected state
		bool bEnable = (FXMakerMain.inst.IsCurrentEffectObject() && IsReadOnlyFolder() == 0);

		// Delete button
		if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(rect1Row, 5, 1, 1), FXMakerTooltip.GetHcToolEffect("Del"), bEnable))
		{
			RenameCurrentPrefab(m_EditingName);
			m_bProcessDelete = true;
		}

		if (m_bProcessDelete)
		{
			m_bProcessDelete = FxmPopupManager.inst.ShowModalOkCancelMessage("'" + m_EffectContents[m_nEffectIndex].text + "'\n" + FXMakerTooltip.GetHsToolMessage("DIALOG_DELETEPREFAB", ""));
			if (m_bProcessDelete == false)
			{
				if (FxmPopupManager.inst.GetModalMessageValue() == FXMakerLayout.MODALRETURN_TYPE.MODALRETURN_OK)
				{
		 			NcSpriteAnimation spriteCom = FXMakerMain.inst.GetOriginalEffectPrefab().GetComponent<NcSpriteAnimation>();
					if (spriteCom != null && spriteCom.m_bBuildSpriteObj && spriteCom.renderer.sharedMaterial != null)
						m_bProcessDelSprite = true;
					else {
						GameObject deletePrefab = FXMakerMain.inst.ClearCurrentEffectObject(m_CurrentEffectRoot, true);
						FXMakerAsset.DeleteEffectPrefab(deletePrefab);
						SelectToolbar(m_nProjectIndex, GetGroupIndex(), m_nEffectIndex);
						return;
					}
				}
			}
		}

		if (m_bProcessDelSprite)
		{
			m_bProcessDelSprite = FxmPopupManager.inst.ShowModalOkCancelMessage("'" + m_EffectContents[m_nEffectIndex].text + "'\n" + FXMakerTooltip.GetHsToolMessage("DIALOG_DELETESPRITE", ""));
			if (m_bProcessDelSprite == false)
			{
				if (FxmPopupManager.inst.GetModalMessageValue() == FXMakerLayout.MODALRETURN_TYPE.MODALRETURN_OK)
				{
					// delete material, texture
		 			NcSpriteAnimation spriteCom = FXMakerMain.inst.GetOriginalEffectPrefab().GetComponent<NcSpriteAnimation>();
					if (spriteCom.renderer.sharedMaterial.mainTexture != null)
					{
						string path = AssetDatabase.GetAssetPath(spriteCom.renderer.sharedMaterial.mainTexture);
						AssetDatabase.MoveAssetToTrash(path);
//						AssetDatabase.DeleteAsset(path);
					}
					string matpath = AssetDatabase.GetAssetPath(spriteCom.renderer.sharedMaterial);
					AssetDatabase.MoveAssetToTrash(matpath);
//					AssetDatabase.DeleteAsset(matpath);
				}
				// delete prefab
				GameObject deletePrefab = FXMakerMain.inst.ClearCurrentEffectObject(m_CurrentEffectRoot, true);
				FXMakerAsset.DeleteEffectPrefab(deletePrefab);
				SelectToolbar(m_nProjectIndex, GetGroupIndex(), m_nEffectIndex);
				return;
			}
		}

		// Clone button
		if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(rect1Row, 5, 2, 1), FXMakerTooltip.GetHcToolEffect("Clone"), bEnable))
		{
			RenameCurrentPrefab(m_EditingName);
			ClonePrefab();
			return;
		}

		// Capture Thumb button
		if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(rect1Row, 5, 3, 1), FXMakerTooltip.GetHcToolEffect("Thumb"), bEnable))
		{
			RenameCurrentPrefab(m_EditingName);
			ThumbPrefab();
			return;
		}

// 		// History button
// 		if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(rect1Row, 5, 4, 1), FXMakertip.GetHcToolEffect("History"), bEnable))
// 		{
// 			SetActiveEffect(m_nEffectIndex);
// 			return;
// 		}

		// Sprite button
		if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(rect1Row, 5, 4, 1), FXMakerTooltip.GetHcToolEffect("Sprite"), bEnable))
		{
			RenameCurrentPrefab(m_EditingName);
			SpritePrefab();
			return;
		}

		// Selected state
		if (FXMakerMain.inst.IsCurrentEffectObject())
		{
			// ChangeName
			if (FXMakerMain.inst.IsCurrentEffectObject() && 0 <= m_nEffectIndex && m_nEffectIndex < m_nEffectCount)
			{
				GUI.SetNextControlName("TextField");
				FXMakerLayout.GUIEnableBackup(IsReadOnlyFolder() == 0);
// 				FXMakerMain.inst.ToggleGlobalLangSkin(true);
				m_EditingName = GUI.TextField(FXMakerLayout.GetInnerHorizontalRect(rect2Row, 4, 0, 4), m_EditingName, 50);
// 				FXMakerMain.inst.ToggleGlobalLangSkin(false);
				FXMakerLayout.GUIEnableRestore();

				bool bEnterKey = (Event.current.isKey && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter));
				if (bEnterKey || (FXMakerMain.GetPrevWindowFocus() == (int)FXMakerLayout.WINDOWID.EFFECT_LIST && FXMakerMain.GetWindowFocus() != (int)FXMakerLayout.WINDOWID.EFFECT_LIST))
					RenameCurrentPrefab(m_EditingName);
			}
		}

		// Resize --------------------------------------------------------------
		bool bChangeScrollColumn = false;
		m_nScrollColumn		= (int)GUI.HorizontalScrollbar(rect3Row, m_nScrollColumn, 1, 1, m_nMaxObjectColumn+1);
		if (GUI.changed)
		{
			UnityEditor.EditorPrefs.SetInt("FXMakerEffect.Effect_nScrollColumn", m_nScrollColumn);
			bChangeScrollColumn	= true;

			Rect rect	= FXMakerLayout.GetAspectScrollViewRect((int)rect3Row.width, FXMakerLayout.m_fScrollButtonAspect, m_nEffectCount, m_nScrollColumn, false);
			m_EffectListScrollPos.y = rect.height * (m_nEffectIndex-m_nScrollColumn) / (float)m_nEffectCount;
		}
		// Draw line
		Rect lineRect		= rect3Row;
		lineRect.y			= rect3Row.yMax+5;
		lineRect.height		= 3;
		NgGUIDraw.DrawHorizontalLine(new Vector2(lineRect.x, lineRect.y), (int)lineRect.width, new Color(0.1f, 0.1f, 0.1f, 0.7f), 2, false);

		// Effect List ------------------------------------------------------
		Rect	listRect		= FXMakerLayout.GetChildVerticalRect(effectRect, m_nGuiTopHeight, 1, 0, 1);
		Rect	scrollRect		= FXMakerLayout.GetAspectScrollViewRect((int)listRect.width, FXMakerLayout.m_fScrollButtonAspect, m_nEffectCount, m_nScrollColumn, false);
		Rect	gridRect		= FXMakerLayout.GetAspectScrollGridRect((int)listRect.width, FXMakerLayout.m_fScrollButtonAspect, m_nEffectCount, m_nScrollColumn, false);
		m_EffectListScrollPos	= GUI.BeginScrollView(listRect, m_EffectListScrollPos, scrollRect);
// 		FXMakerMain.inst.ToggleGlobalLangSkin(true);
//		int		nEffectIndex	= GUI.SelectionGrid(listRect, m_nEffectIndex, m_EffectNameStrings, m_nScrollColumn);
//		int		nEffectIndex	= GUI.SelectionGrid(gridRect, m_nEffectIndex, m_EffectContents, m_nScrollColumn);
		int		nEffectIndex	= FXMakerLayout.TooltipSelectionGrid(FXMakerLayout.GetOffsetRect(effectRect, 0, -m_EffectListScrollPos.y), listRect, gridRect, m_nEffectIndex, m_EffectContents, m_nScrollColumn);
// 		FXMakerMain.inst.ToggleGlobalLangSkin(false);

		// move key
		if (FXMakerMain.inst.GetFocusInputKey(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.EFFECT_LIST)) != 0)
		{
			switch(FXMakerMain.inst.GetFocusInputKey(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.EFFECT_LIST)))
			{
				case KeyCode.LeftArrow	: nEffectIndex--;					FXMakerMain.inst.SetFocusInputKey(0);	break;
				case KeyCode.RightArrow	: nEffectIndex++;					FXMakerMain.inst.SetFocusInputKey(0);	break;
				case KeyCode.UpArrow	: nEffectIndex-=m_nScrollColumn;	FXMakerMain.inst.SetFocusInputKey(0);	break;
				case KeyCode.DownArrow	: nEffectIndex+=m_nScrollColumn;	FXMakerMain.inst.SetFocusInputKey(0);	break;
			}
			if (nEffectIndex < 0)				nEffectIndex = 0;
			if (m_nEffectCount <= nEffectIndex) nEffectIndex = m_nEffectCount-1;
		}

		// select
		if ((bChangeScrollColumn == false && GUI.changed) || m_nEffectIndex != nEffectIndex)
		{
			RenameCurrentPrefab(m_EditingName);

			// right button
			if (Input.GetMouseButtonUp(1))
				ShowRightMenu(nEffectIndex, true);

			// active
			NgUtil.LogDevelop("changed m_nEffectIndex - id = " + id + ", value = " + m_EffectContents[nEffectIndex].text);
			if (nEffectIndex == m_nEffectIndex && FXMakerMain.inst.IsCurrentEffectObject())
			{
				FXMakerMain.inst.CreateCurrentInstanceEffect(true);
			} else SetActiveEffect(nEffectIndex);
		} else {
			// right button
			if (Input.GetMouseButtonUp(1))
			{
				RenameCurrentPrefab(m_EditingName);
				ShowRightMenu(-1, false);
			}
		}
		GUI.EndScrollView();
		FXMakerMain.inst.SaveTooltip();
	}

	void ShowNewMenu()
	{
// 		if (FxmPopupManager.inst.IsShowPopupExcludeMenu())
// 			return;

		if (IsReadOnlyFolder() != 0)
			return;
		m_bShowNewMenuPopup	= true;
		FxmPopupManager.inst.ShowMenuPopup("New Prefab", new string[]{"Empty", "- Mesh -", "Plane", "- Particle -", "Legacy", "Shuriken"},
				new bool  []{true, false, true, false, true, true});
	}

	void CheckNewMenu()
	{
		if (m_bShowNewMenuPopup)
		{
			if (FxmPopupManager.inst.IsShowCurrentMenuPopup() == false)
			{
				switch (FxmPopupManager.inst.GetSelectedIndex())
				{
					case 0:		NewPrefab(NEW_TYPE.NEW_EMPTY);		break;
					case 1:		break;
					case 2:		NewPrefab(NEW_TYPE.NEW_PLANE);		break;
					case 3:		break;
					case 4:		NewPrefab(NEW_TYPE.NEW_LEGACY);		break;
					case 5:		NewPrefab(NEW_TYPE.NEW_SHURIKEN);	break;
				}
				m_bShowNewMenuPopup	= false;
			}
		}
	}

	void ShowRightMenu(int nObjectIndex, bool bSelected)
	{
		Rect	scrollRect	= FXMakerLayout.GetEffectListRect();
		scrollRect.y		+= m_nGuiTopHeight;
		scrollRect.height	-= m_nGuiTopHeight;

		if (FxmPopupManager.inst.IsShowPopupExcludeMenu() || scrollRect.Contains(FXMakerLayout.GetGUIMousePosition()) == false)
			return;

		m_bShowRightMenuPopup	= true;
		m_nIndexRightMenuPopup	= nObjectIndex;
		bool bNotReadOnly = (IsReadOnlyFolder() == 0);
		FxmPopupManager.inst.ShowMenuPopup("Prefab", new string[]{"PingPrefab", "Copy", "Cut", "Paste", "-", "New", "Delete", "Clone", "Thumb", "BuildSprite", "Export", "ExportAll"},
				new bool  []{bSelected, bSelected, bSelected&&bNotReadOnly, FXMakerClipboard.inst.IsPrefab()&&bNotReadOnly, false, true&&bNotReadOnly, bSelected&&bNotReadOnly, bSelected&&bNotReadOnly, bSelected&&bNotReadOnly, bSelected&&bNotReadOnly, bSelected, (0<m_nEffectCount)});
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
					case 1:		CopyPrefab();				break;
					case 2:		CutPrefab();				break;
					case 3:		PastePrefab();				break;
					case 4:		break;
//					case 5:		NewPrefab(NEW_TYPE.NEW_EMPTY);	break;
					case 5:		ShowNewMenu();				break;
					case 6:		m_bProcessDelete = true;	break;
					case 7:		ClonePrefab();				break;
					case 8:		ThumbPrefab();				break;
					case 9:		SpritePrefab();				break;
					case 10:	ExportPrefab(false);		break;
					case 11:	ExportPrefab(true);			break;
				}
				m_bShowRightMenuPopup	= false;
			}
		}
	}

	void PingPrefab()
	{
//		if (IsLoadingProject() == false)
//			if (Input.GetMouseButtonUp(1))
		FXMakerAsset.SetPingObject(FXMakerMain.inst.GetOriginalEffectPrefab());
	}

	string GetCurrentDirPath()
	{
		string groupName = m_GroupFolderContents[GetGroupIndex()].text;
		if (groupName.Contains("Res.."))
		{
			return NgFile.CombinePath("Assets/Resources/", groupName  + "/");
		} else
		return NgFile.CombinePath(FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.PROJECTS), m_ProjectFolerContents[m_nProjectIndex].text + "/" + m_GroupFolderContents[GetGroupIndex()].text  + "/");
	}

	void CopyPrefab()
	{
		SaveProject();
		FXMakerClipboard.inst.SetClipboardPrefab(FXMakerMain.inst.GetOriginalEffectPrefab(), false);
	}

	void CutPrefab()
	{
		SaveProject();
		FXMakerClipboard.inst.SetClipboardPrefab(FXMakerMain.inst.GetOriginalEffectPrefab(), true);
	}

	void PastePrefab()
	{
		SaveProject();
		string createPath = FXMakerClipboard.inst.PasteClipboardPrefab(GetCurrentDirPath());
		SelectToolbar(m_nProjectIndex, GetGroupIndex(), createPath);
	}

	void NewPrefab(NEW_TYPE newType)
	{
		SaveProject();
		GameObject	newPrefab;
		switch (newType)
		{
			case NEW_TYPE.NEW_EMPTY		: newPrefab	= m_DefaultEmptyPrefab;		break;
			case NEW_TYPE.NEW_PLANE		: newPrefab	= m_DefaultPlanePrefab;		break;
			case NEW_TYPE.NEW_LEGACY	: newPrefab	= m_DefaultLegacyPrefab;	break;
			case NEW_TYPE.NEW_SHURIKEN	: newPrefab	= m_DefaultShurikenPrefab;	break;
			default :
				{
					NgUtil.LogMessage("NEW_TYPE error !!!");
					return;
				}
		}
		string createPath = NgAsset.CreateDefaultUniquePrefab(newPrefab, GetCurrentDirPath() + "NewEffect.prefab");
		SetChangePrefab();
		SaveProject();
		SelectToolbar(m_nProjectIndex, GetGroupIndex(), createPath);
	}

	void ClonePrefab()
	{
		SaveProject();
		string createPath = FXMakerAsset.CloneEffectPrefab(FXMakerMain.inst.GetOriginalEffectPrefab());
		SelectToolbar(m_nProjectIndex, GetGroupIndex(), createPath);
	}

	void ThumbPrefab()
	{
// 		string filename	= NgFile.CombinePath(FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.TEMPPREVIEWTEXTURE), NgAsset.GetPrefabThumbFilename(FXMakerMain.inst.GetOriginalEffectPrefab()));
// 
// 		if (AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(FXMakerMain.inst.GetOriginalEffectPrefab().renderer.sharedMaterial.mainTexture), filename))
// 			NgAsset.CaptureResize(filename);
// 		return;
// 		SaveProject();
		FXMakerCapture.StartSaveEffectThumb();
	}

	void SpritePrefab()
	{
		FxmPopupManager.inst.ShowSpritePopup();
	}

	void ExportPrefab(bool bAllExport)
	{
		SaveProject();
		string	savePath = EditorUtility.SaveFilePanel("ExportPackage", "", (bAllExport ? m_GroupFolderContents[GetGroupIndex()].text : FXMakerMain.inst.GetOriginalEffectPrefab().name), "unitypackage");

		if (savePath != "")
		{
			string			prefabPath;
			List<string>	exportList	= new List<string>();

			if (bAllExport)
			{
				for (int n=0; n < m_nEffectCount; n++)
				{
					if (m_EffectPrefabs[n] != null)
					{
						prefabPath = AssetDatabase.GetAssetPath(m_EffectPrefabs[n]);
						exportList.Add(prefabPath);
					}
				}
			} else {
	 			prefabPath	= AssetDatabase.GetAssetPath(FXMakerMain.inst.GetOriginalEffectPrefab());
				exportList.Add(prefabPath);
			}
			exportList.Add(FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.SCRIPTS));

 			AssetDatabase.ExportPackage(exportList.ToArray(), savePath, ExportPackageOptions.IncludeDependencies|ExportPackageOptions.Recurse);
//			EditorUtility.OpenWithDefaultApp(NgFile.TrimFilenameExt(savePath));
		}
	}

	public void RenameCurrentPrefab(string newName)
	{
		if (FXMakerMain.inst.IsCurrentEffectObject() && 0 <= m_nEffectIndex && m_nEffectIndex < m_nEffectCount)
		{
			if (newName != m_EffectContents[m_nEffectIndex].text)
			{
				newName = newName.Trim();
				if (newName != "")
				{
					SaveProject();
					m_EffectContents[m_nEffectIndex].text	= newName;
					m_EffectPrefabs[m_nEffectIndex].name	= newName;
					FXMakerMain.inst.GetOriginalEffectObject().name = newName;
					string changePath = FXMakerAsset.RenameEffectPrefab(FXMakerMain.inst.GetOriginalEffectObject(), FXMakerMain.inst.GetOriginalEffectPrefab(), newName);
					LoadProject(changePath);
				}
			}
		}
	}
}
#endif
