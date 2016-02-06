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
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System;

public class FxmFolderPopup_NcInfoCurve : FxmFolderPopup
{
	// Attribute ------------------------------------------------------------------------
	// const
	public			const string					m_NcInfoCurve_FileKeyword	= "NcInfoCurve_";

	// popup
	public			string							m_DefaultFileName			= "[ToolCurve]";
	protected		NcCurveAnimation				m_CurrentCurveAnimation;
	protected		int								m_OriCurveInfoIndex;
	protected		NcCurveAnimation.NcInfoCurve	m_OriCurveInfo;
	protected		int								m_nRightMenuPopup			= -1;
	protected		bool							m_bOnlyCurve				= true;

	protected		GameObject[]					m_CurveAniObjects;
	protected		NcCurveAnimation.NcInfoCurve[]	m_CurveInfos;
	protected		NcCurveAnimation.NcInfoCurve	m_SelCurveInfo;

	// Property -------------------------------------------------------------------------
	public bool ShowPopupWindow(NcCurveAnimation ncCurveAni, int nIndex, bool bSaveDialog)
	{
		m_CurrentCurveAnimation = ncCurveAni;
		m_OriCurveInfoIndex		= nIndex;

		if (m_CurrentCurveAnimation == null)
		{
			Debug.LogError("ncCurveAni is null");
			return false;
		}

		// Save Hint Box
		m_bDrawRedProject		= bSaveDialog;
		m_bDrawRedBottomButtom	= bSaveDialog;

		// backup
		m_OriCurveInfo = m_CurrentCurveAnimation.GetCurveInfo(nIndex).GetClone();

		m_nOriMaxObjectColumn		= 3;
		m_fButtonAspect				= FXMakerLayout.m_fScrollButtonAspect;
		m_SelectedTransform			= m_CurrentCurveAnimation.transform;
		m_SelCurveInfo				= null;
		m_PrefsName					= "NcInfoCurve";
		m_DefaultProjectName		= m_DefaultFileName;
		m_bFixedOptionRecursively	= true;
		m_bOptionRecursively		= false;
		m_bDisableOptionShowName	= true;
		bool reValue = base.ShowPopupWindow(ncCurveAni, bSaveDialog);

		// default nGroupIndex
		if (m_bOnlyCurve == false)
			m_nGroupIndex = (int)m_CurrentCurveAnimation.GetCurveInfo(nIndex).m_ApplyType+1;

		return reValue;
	}

	// -------------------------------------------------------------------------------------------
	void Awake()
	{
		m_nObjectColumn	= 4;
	}

	void Start()
	{
	}

	void Update()
	{
	}

	public override void OnGUIPopup()
	{
		if (m_CurrentCurveAnimation.GetCurveInfo(m_OriCurveInfoIndex) == null)
		{
			ClosePopup(false);
		} else {
			base.OnGUIPopup();
			// Popup Window ---------------------------------------------------------
			FXMakerMain.inst.ModalMsgWindow(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.POPUP), GetPopupRect(), winPopup, "NcInfoCurve");
		}
	}

	// ==========================================================================================================
	protected override void DrawObjectList(Rect baseRect)
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

		// right menu
		CheckRightMenu();

		// Object List ------------------------------------------------------
		Rect		listRect		= baseRect;
		Rect		scrollRect		= FXMakerLayout.GetAspectScrollViewRect((int)listRect.width, m_fButtonAspect, nObjectCount, nColumn, !m_bOptionShowName);
		Rect		gridRect		= FXMakerLayout.GetAspectScrollGridRect((int)listRect.width, m_fButtonAspect, nObjectCount, nColumn, !m_bOptionShowName);
		GUIStyle	styleButton		= GUI.skin.GetStyle("ScrollList_Button");
		m_ObjectListScrollPos		= GUI.BeginScrollView(listRect, m_ObjectListScrollPos, scrollRect);
		styleButton.imagePosition	= (m_bOptionShowName ? ImagePosition.ImageAbove : ImagePosition.ImageOnly);

		bool OldEnabled = GUI.enabled;
// 		if (m_bSaveDialog)
// 			GUI.enabled = false;

		// Draw grid
//		int	nObjectIndex	= GUI.SelectionGrid(gridRect, m_nObjectIndex, m_ObjectContents, nColumn, styleButton);
		int	nObjectIndex	= FXMakerLayout.TooltipSelectionGrid(FXMakerLayout.GetOffsetRect(GetPopupRect(), 0, -m_ObjectListScrollPos.y), listRect, gridRect, m_nObjectIndex, m_ObjectContents, nColumn, styleButton);

		if (GUI.changed)
		{
			// right menu
			if (Input.GetMouseButtonUp(1))
			{
				ShowRightMenu(nObjectIndex);
			} else {
			if (nObjectIndex != m_nObjectIndex)
				this.SetActiveObject(nObjectIndex);
			}
		}

		// Draw Curve
		for (int n = 0; n < m_CurveInfos.Length; n++)
			DrawCurve(n, gridRect, n%nColumn, n/nColumn, m_CurveInfos[n], styleButton);

		if (m_bSaveDialog)
			GUI.enabled = OldEnabled;

		GUI.EndScrollView();
	}

	void ShowRightMenu(int nObjectIndex)
	{
		m_nRightMenuPopup = nObjectIndex;
		FxmPopupManager.inst.ShowMenuPopup("Curve PopMenu", new string[]{"Delete"});
	}

	void CheckRightMenu()
	{
		if (0 <= m_nRightMenuPopup)
		{
			if (FxmPopupManager.inst.IsShowCurrentMenuPopup() == false)
			{
				if (FxmPopupManager.inst.GetSelectedIndex() == 0)
				{
					DeleteCurveInfo(m_nRightMenuPopup);
					m_nRightMenuPopup = -1;
				}
			}
		}
	}

	void DrawCurve(int nIndex, Rect gridRect, int nColumn, int nRow, NcCurveAnimation.NcInfoCurve curveInfo, GUIStyle styleButton)
	{
		int		nMargin			= styleButton.margin.left;
		int		nImageMargin	= 3;
		float	fButtonWidth	= (gridRect.width / m_nObjectColumn);
		float	fButtonHeight	= (m_fButtonAspect * fButtonWidth);

		Rect buttonRect = new Rect(fButtonWidth*nColumn+nMargin, fButtonHeight*nRow+nMargin, fButtonWidth-nMargin*2, fButtonHeight-nMargin*2);
		buttonRect		= FXMakerLayout.GetOffsetRect(buttonRect, -nImageMargin);
 		EditorGUIUtility.DrawCurveSwatch(buttonRect, curveInfo.m_AniCurve, null, Color.green, Color.black, curveInfo.GetFixedDrawRange());
	}

	// ==========================================================================================================
	protected override void DrawBottomRect(Rect baseRect)
	{
		GUI.Box(baseRect, "");

		GUIContent	guiCon;
		Rect	imageRect = baseRect;
		imageRect.width	= FXMakerLayout.GetFixedWindowWidth();
		Rect	rightRect = baseRect;
		rightRect.x		+= imageRect.width;
		rightRect.width	-= imageRect.width;
		rightRect		= FXMakerLayout.GetOffsetRect(rightRect, 5, 3, -5, -3);

		Rect	buttonRect	= FXMakerLayout.GetInnerVerticalRect(rightRect, 12, 0, 5);
		Rect	buttonRect2	= FXMakerLayout.GetInnerVerticalRect(rightRect, 12, 5, 5);

		// image
		if (m_SelObjectContent == null)
			 guiCon = new GUIContent("[Not Selected]");
		else guiCon = new GUIContent("", m_SelObjectContent.image, m_SelObjectContent.tooltip);
		if (FXMakerLayout.GUIButton(imageRect, guiCon, GUI.skin.GetStyle("PopupBottom_ImageButton"), (m_SelObjectContent != null)))
			if (Input.GetMouseButtonUp(0))
				FXMakerMain.inst.CreateCurrentInstanceEffect(true);
		int nImageMargin = 3;
		imageRect		= FXMakerLayout.GetOffsetRect(imageRect, -nImageMargin);
		EditorGUIUtility.DrawCurveSwatch(imageRect, m_CurrentCurveAnimation.GetCurveInfo(m_OriCurveInfoIndex).m_AniCurve, null, Color.green, Color.black, m_CurrentCurveAnimation.GetCurveInfo(m_OriCurveInfoIndex).GetFixedDrawRange());

		// text
// 		GUI.Label(NgLayout.GetInnerVerticalRect(rightRect, 12, 5, 8), (m_SelObjectContent == null ? "[Not Selected]" : m_SelObjectContent.text));

		if (m_bSaveDialog)
		{
			bool bSaveEnable = (0 <= m_nProjectIndex);
			bool bReadOnyFolder = false;

			if (bSaveEnable)
			{
				bReadOnyFolder = IsReadOnlyFolder(m_ProjectFolerContents[m_nProjectIndex].text);
				bSaveEnable = !bReadOnyFolder;
			}

			// Cancel
			if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(buttonRect, 2, 0, 1), GetHelpContent("Cancel"), true))
			{
				ClosePopup(false);
				return;
			}
			// save
			if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(buttonRect, 2, 1, 1), (bReadOnyFolder ? FXMakerTooltip.GetGUIContent("Save", FXMakerTooltip.GetHsToolMessage("READONLY_FOLDER", "")) : GetHelpContent("Save")), bSaveEnable))
			{
				SaveAddCurvePrefab(false);
				ClosePopup(true);
			}
			// overwrite
			if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(buttonRect2, 2, 0, 2), (bReadOnyFolder ? FXMakerTooltip.GetGUIContent("Overwrite", FXMakerTooltip.GetHsToolMessage("READONLY_FOLDER", "")) : GetHelpContent("Overwrite")), (bSaveEnable && 0 <= m_nObjectIndex)))
			{
				SaveAddCurvePrefab(true);
				ClosePopup(true);
			}
			buttonRect.height *= 2;
			if (m_bDrawRedBottomButtom)
				NgGUIDraw.DrawBox(FXMakerLayout.GetOffsetRect(buttonRect, 3), FXMakerLayout.m_ColorHelpBox, (bSaveEnable ? 2:1), false);
		} else {
			// Undo
			if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(buttonRect, 2, 0, 1), GetHelpContent("Undo"), (m_SelCurveInfo != null)))
				UndoCurveAni();
			// close
			if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(buttonRect, 2, 1, 1), GetHelpContent("Close")))
				ClosePopup(true);
		}
	}

	// ----------------------------------------------------------------------------------------------------------
	protected override void LoadProjects()
	{
		string	loaddir = FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.PROJECTS);

		m_LoadDirectory		= NgFile.PathSeparatorNormalize(loaddir);
		m_CurveAniObjects	= NgAsset.GetCurvePrefabList(m_LoadDirectory, m_NcInfoCurve_FileKeyword, true, FXMakerLayout.m_nMaxPrefabListCount, out m_nProjectCount);

// 		Debug.Log(m_nProjectCount);

		// Load FileName
		string[] folderStrings	= new string[m_nProjectCount];
		if (0 < m_nProjectCount)
		{
			for (int n=0; n < m_nProjectCount; n++)
				folderStrings[n] = m_CurveAniObjects[n].name.Replace("NcInfoCurve_", "");
		}
		m_ProjectFolerContents	= NgConvert.StringsToContents(folderStrings);
	}

	protected override void LoadGroups()
	{
		if (m_nProjectCount <= 0)
			return;

		string[] groupStrings;

		if (m_bOnlyCurve)
		{
			const int cnt	= 4;
			m_nGroupCount	= cnt;
			groupStrings	= new string[cnt] {"All", "Low", "Middle", "High"};
		} else {
			if (m_bSaveDialog)
			{
				m_nGroupCount	= 1;
				groupStrings	= new string[m_nGroupCount];
				groupStrings[0]	= m_OriCurveInfo.m_ApplyType.ToString();
			} else {
				// Load GroupName
				m_nGroupCount	= NcCurveAnimation.NcInfoCurve.m_TypeName.Length;
				groupStrings	= new string[m_nGroupCount];
				Array.Copy(NcCurveAnimation.NcInfoCurve.m_TypeName, groupStrings, m_nGroupCount);
				ArrayUtility.Insert<string>(ref groupStrings, 0, "All");
				m_nGroupCount++;
			}
		}
		m_GroupFolderContents	= NgConvert.StringsToContents(groupStrings);
	}

	protected override void LoadObjects()
	{
		ArrayList	infoList = new ArrayList();

		m_CurveInfos		= null;
		m_ObjectContents	= null;
		m_nObjectCount		= 0;
		m_nObjectIndex		= -1;

		if (m_nGroupCount <= 0)
			return;

		GameObject currentObj = m_CurveAniObjects[m_nProjectIndex];

		if (m_bOnlyCurve)
		{
			NcCurveAnimation curveAnis = currentObj.GetComponent<NcCurveAnimation>();
			if (curveAnis != null)
			{
				List<NcCurveAnimation.NcInfoCurve>	curveInfoList = curveAnis.m_CurveInfoList;
				for (int n = 0; n < curveInfoList.Count; n++)
				{
					curveInfoList[n].m_nTag = n;
					if (m_nGroupIndex == 0)
						infoList.Add(curveInfoList[n]);
					else {
						if (m_nGroupIndex == curveInfoList[n].m_nSortGroup)		// 1, 2, 3
							infoList.Add(curveInfoList[n]);
					}
				}
			}
		} else {
			NcCurveAnimation[] curveAnis = currentObj.GetComponents<NcCurveAnimation>();
			foreach (NcCurveAnimation curveAni in curveAnis)
			{
				List<NcCurveAnimation.NcInfoCurve>	curveInfoList = curveAni.m_CurveInfoList;
				for (int n = 0; n < curveInfoList.Count; n++)
				{
					NcCurveAnimation.NcInfoCurve curveInfo	= curveInfoList[n];
					curveInfo.m_nTag = n;
					// Add CurveInfo
					if (m_bSaveDialog)
					{
						if (m_OriCurveInfo.m_ApplyType == curveInfo.m_ApplyType)
							infoList.Add(curveInfo);
					} else {
						if (0 < m_nGroupIndex)
						{
							if (m_nGroupIndex-1 == (int)curveInfo.m_ApplyType)
								infoList.Add(curveInfo);
						} else infoList.Add(curveInfo);
					}
				}
			}
		}

		m_CurveInfos	= NgConvert.ToArray<NcCurveAnimation.NcInfoCurve>(infoList);
		m_nObjectCount	= infoList.Count;

		// build contents
		string	subDir = AssetDatabase.GetAssetPath(m_CurveAniObjects[m_nProjectIndex]);
		subDir = NgFile.PathSeparatorNormalize(subDir).Replace(m_LoadDirectory, "");

		m_ObjectContents = new GUIContent[m_nObjectCount];
		for (int n=0; n < m_nObjectCount; n++)
		{
			if (m_ObjectContents[n] == null)
			{
				m_ObjectContents[n]			= new GUIContent();
				m_ObjectContents[n].text	= "";
				m_ObjectContents[n].tooltip	= FXMakerTooltip.Tooltip(GetCurveInfo(m_CurveInfos[n], subDir));
			}
		}
// 		Debug.Log("m_nObjectCount " + m_nObjectCount);
	}

	string GetCurveInfo(NcCurveAnimation.NcInfoCurve curveInfo, string subDir)
	{
		if (curveInfo != null)
		{
			if (m_bOnlyCurve)
			{
				return string.Format("- SavePrefab: {0}", subDir); 
			} else {
				string option1 = "";
				string option2 = "";

				for (int n=0; n < curveInfo.GetValueCount(); n++)
					option1 += string.Format("{0}({1})  ", curveInfo.GetValueName(n), (curveInfo.m_bApplyOption[n] ? "O" : "  "));
				if (curveInfo.m_ApplyType == NcCurveAnimation.NcInfoCurve.APPLY_TYPE.MATERIAL_COLOR)
					option2 = "To Color: " + curveInfo.m_ToColor.ToString() + "\n- Recursively: " + curveInfo.m_bRecursively.ToString();
				else
				if (curveInfo.m_ApplyType == NcCurveAnimation.NcInfoCurve.APPLY_TYPE.MESH_COLOR)
					option2 = "From Color: " + curveInfo.m_FromColor.ToString() + "To Color: " + curveInfo.m_ToColor.ToString() + "\n- Recursively: " + curveInfo.m_bRecursively.ToString();
				else option2 = "Value Scale: "+curveInfo.m_fValueScale.ToString();
				

				return string.Format("- {0}  {1}\n- {2}\n- SavePrefab: {3}", curveInfo.m_ApplyType.ToString(), option1, option2, subDir); 
			}
		}
		return "";
	}

	// ----------------------------------------------------------------------------------------------------------
	protected override void SetActiveObject(int nObjectIndex)
	{
		if (nObjectIndex < 0 || m_nObjectCount <= nObjectIndex)
			return;

		NcCurveAnimation.NcInfoCurve	selCurveInfo = m_CurveInfos[nObjectIndex];

		if (m_bSaveDialog == false)
		{
			// 같은것 재클릭
			if (m_nObjectIndex == nObjectIndex)
			{
				FXMakerMain.inst.CreateCurrentInstanceEffect(true);
				return;
			}

			SetCurveAni(selCurveInfo);
			FXMakerMain.inst.CreateCurrentInstanceEffect(true);
		}

		m_nObjectIndex			= nObjectIndex;
		m_nSelObjectProjectIndex= m_nProjectIndex;
		m_nSelObjectGroupIndex	= (m_bOptionRecursively ? -1 : m_nGroupIndex);
		m_SelObjectContent		= new GUIContent(m_ObjectContents[nObjectIndex].text, m_ObjectContents[nObjectIndex].image, m_ObjectContents[nObjectIndex].tooltip);
	}

	void SetCurveAni(NcCurveAnimation.NcInfoCurve selCurveInfo)
	{
		// copy
		if (m_bOnlyCurve)
		{
			m_CurrentCurveAnimation.GetCurveInfo(m_OriCurveInfoIndex).m_AniCurve = selCurveInfo.GetClone().m_AniCurve;
			m_SelCurveInfo = selCurveInfo;
		} else {
			m_CurrentCurveAnimation.SetCurveInfo(m_OriCurveInfoIndex, selCurveInfo.GetClone());
			m_SelCurveInfo = selCurveInfo;
		}
	}

	void UndoCurveAni()
	{
		// select init
		m_nObjectIndex			= -1;
		m_nSelObjectGroupIndex	= -1;
		m_nSelObjectProjectIndex= -1;
		m_SelObjectContent		= null;
		m_SelCurveInfo			= null;
		FXMakerMain.inst.CreateCurrentInstanceEffect(true);

		// Restore
		m_CurrentCurveAnimation.SetCurveInfo(m_OriCurveInfoIndex, m_OriCurveInfo.GetClone());
		m_SelCurveInfo = null;
	}

	void SaveAddCurvePrefab(bool bOverwrite)
	{
		if (m_nProjectIndex < 0)
			return;

		GameObject			saveObj = (GameObject)PrefabUtility.InstantiatePrefab(m_CurveAniObjects[m_nProjectIndex]);
		NcCurveAnimation	saveCom;
		int					nSaveIndex;

		// add component
		if (m_bOnlyCurve)
		{
			saveCom = saveObj.GetComponent<NcCurveAnimation>();
			if (saveCom == null)
					saveCom = saveObj.AddComponent<NcCurveAnimation>();
		} else {
			NcCurveAnimation[] curveAnis = saveObj.GetComponents<NcCurveAnimation>();
			if (curveAnis.Length < NcCurveAnimation.NcInfoCurve.m_TypeName.Length)
			{
				for (int n = curveAnis.Length; n < NcCurveAnimation.NcInfoCurve.m_TypeName.Length; n++)
					saveObj.AddComponent<NcCurveAnimation>();
				curveAnis = saveObj.GetComponents<NcCurveAnimation>();
			}
			saveCom = curveAnis[(int)m_OriCurveInfo.m_ApplyType];
		}

		if (bOverwrite)
		{
			// update curve
			nSaveIndex	= m_CurveInfos[m_nObjectIndex].m_nTag;
			saveCom.SetCurveInfo(nSaveIndex, m_OriCurveInfo);
			nSaveIndex = m_nObjectIndex;
		} else {
			// add curve
			nSaveIndex = saveCom.AddCurveInfo(m_OriCurveInfo);
		}
		saveCom.SortCurveInfo();

		// change Enabled
		saveCom.GetCurveInfo(nSaveIndex).m_bEnabled = true;

		PrefabUtility.ReplacePrefab(saveObj, m_CurveAniObjects[m_nProjectIndex]);
		FXMakerAsset.AssetDatabaseSaveAssets();
		DestroyImmediate(saveObj);
	}

	void DeleteCurveInfo(int nIndex)
	{
		GameObject	saveObj	= (GameObject)PrefabUtility.InstantiatePrefab(m_CurveAniObjects[m_nProjectIndex]);

		if (m_bOnlyCurve)
		{
			NcCurveAnimation	saveCom = saveObj.GetComponent<NcCurveAnimation>();
			if (saveCom != null)
			{
				saveCom.DeleteCurveInfo(m_CurveInfos[nIndex].m_nTag);
			}
		} else {
			NcCurveAnimation[]				curveAnis		= saveObj.GetComponents<NcCurveAnimation>();
			NcCurveAnimation.NcInfoCurve	delCurveInfo	= m_CurveInfos[nIndex];

			foreach (NcCurveAnimation curveAni in curveAnis)
			{
				List<NcCurveAnimation.NcInfoCurve>	curveInfoList = curveAni.m_CurveInfoList;
				for (int n = 0; n < curveInfoList.Count; n++)
				{
					NcCurveAnimation.NcInfoCurve curveInfo	= curveInfoList[n];
					if (curveInfo.m_ApplyType == delCurveInfo.m_ApplyType && curveInfo.m_nTag == delCurveInfo.m_nTag)
					{
						curveInfoList.Remove(curveInfo);
						break;
					}
				}
			}
		}

		PrefabUtility.ReplacePrefab(saveObj, m_CurveAniObjects[m_nProjectIndex]);
		FXMakerAsset.AssetDatabaseSaveAssets();
		DestroyImmediate(saveObj);

		// update scrolllist
		int nObjectIndex	= m_nObjectIndex;
		
		if (nIndex < m_nObjectIndex)
			nObjectIndex = m_nObjectIndex-1;
		if (nIndex == m_nObjectIndex)
			nObjectIndex = -1;
		LoadObjects();
		m_nObjectIndex = nObjectIndex;
	}

	// Control Function -----------------------------------------------------------------
	// Event Function -------------------------------------------------------------------
	// -------------------------------------------------------------------------------------------
	protected override GUIContent GetHelpContent(string text)
	{
		return FXMakerTooltip.GetHcFolderPopup_NcInfoCurve(text);
	}
}
#endif
