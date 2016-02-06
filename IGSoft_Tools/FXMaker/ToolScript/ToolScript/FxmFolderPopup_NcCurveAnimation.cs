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

public class FxmFolderPopup_NcCurveAnimation : FxmFolderPopup
{
	// Attribute ------------------------------------------------------------------------
	// const
	protected		const string		m_NcCurveAni_FileKeyword	= "NcAniCurve_";

	public			string				m_DefaultGroupName			= "[Animation]";
	public			GameObject			m_DefaultSavePrefab;

	// popup
	protected		NcCurveAnimation	m_OriCurveAnimation;

	protected		GameObject[]		m_CurveAniObjects;
	protected		GameObject			m_SelCurveAniObject;
	protected		NcCurveAnimation	m_CurrentCurveAnimation;

	// Property -------------------------------------------------------------------------
	public override bool ShowPopupWindow(Object selObj, bool bSaveDialog)
	{
		m_CurrentCurveAnimation = selObj as NcCurveAnimation;

		if (m_CurrentCurveAnimation == null)
		{
			Debug.LogError("(selObj as NcCurveAnimation) is null");
			return false;
		}

		// Save Hint Box
		m_bDrawRedProject		= bSaveDialog;
		m_bDrawRedGroup			= bSaveDialog;
		m_bDrawRedBottom		= bSaveDialog;

		// SaveDialog - Disable m_bOptionRecursively
		if (bSaveDialog)
		{
			m_SaveFilename				= "Save FileName";
			m_bFixedOptionRecursively	= true;
			m_bOptionRecursively		= false;
		} else m_bFixedOptionRecursively = false;

		// create backup component
		m_OriCurveAnimation = gameObject.GetComponent<NcCurveAnimation>();
		if (m_OriCurveAnimation == null)
		{
			m_OriCurveAnimation = gameObject.AddComponent<NcCurveAnimation>();
			m_OriCurveAnimation.enabled = false;
		}

		// backup
		m_CurrentCurveAnimation.CopyTo(m_OriCurveAnimation, false);

		m_nOriMaxObjectColumn		= 3;
		m_fButtonAspect				= FXMakerLayout.m_fScrollButtonAspect;
		m_SelectedTransform			= m_CurrentCurveAnimation.transform;
		m_PrefsName					= "NcCurveAnimation";
		m_BaseDefaultGroupName		= m_DefaultGroupName;
		m_SelCurveAniObject			= null;
		return base.ShowPopupWindow(selObj, bSaveDialog);
	}

	// -------------------------------------------------------------------------------------------
	void Awake()
	{
		m_nObjectColumn		= 3;
		m_bOptionShowName	= true;
	}

	void Start()
	{
	}

	void Update()
	{
	}

	public override void OnGUIPopup()
	{
		base.OnGUIPopup();
		// Popup Window ---------------------------------------------------------
		FXMakerMain.inst.ModalMsgWindow(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.POPUP), GetPopupRect(), winPopup, "CurveAnimation");
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

		if (m_bSaveDialog)
		{
			Rect	labelRect		= FXMakerLayout.GetInnerVerticalRect(baseRect, 12, 2, 3);
			GUI.Label(FXMakerLayout.GetLeftRect(labelRect, 100), "Filename:");
			Rect	editRect		= FXMakerLayout.GetInnerVerticalRect(baseRect, 12, 5, 5);
			string	saveFilename	= GUI.TextField(editRect, m_SaveFilename, 50);
			if (saveFilename != m_SaveFilename)
			{
				if (saveFilename.Trim() != "")
					m_SaveFilename = saveFilename;
			}
		} else {
			// image
			if (m_SelObjectContent == null)
				 guiCon = new GUIContent("[Not Selected]");
			else guiCon = new GUIContent("", m_SelObjectContent.image, m_SelObjectContent.tooltip);
			if (FXMakerLayout.GUIButton(imageRect, guiCon, GUI.skin.GetStyle("PopupBottom_ImageButton"), (m_SelObjectContent != null)))
			{
				if (Input.GetMouseButtonUp(0))
				{
					FXMakerAsset.SetPingObject(m_CurrentCurveAnimation);
					FXMakerMain.inst.CreateCurrentInstanceEffect(true);
				}
				if (Input.GetMouseButtonUp(1))
					FXMakerAsset.SetPingObject(m_CurveAniObjects[m_nObjectIndex]);
			}

			// text
			GUI.Label(FXMakerLayout.GetInnerVerticalRect(rightRect, 12, 5, 8), (m_SelObjectContent == null ? "[Not Selected]" : m_SelObjectContent.text));
		}

		if (m_bSaveDialog)
		{
			bool bSaveEnable = (0 <= m_nGroupIndex && 0 < m_nGroupCount);
			bool bReadOnyFolder = false;

			if (bSaveEnable)
			{
				bReadOnyFolder = 0 < IsReadOnlyFolder();
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
				SaveCurveAniToPrefabFile();
				ClosePopup(true);
// 				FXMakerEffect.inst.LoadProject();
			}
			if (m_bDrawRedBottomButtom)
				NgGUIDraw.DrawBox(FXMakerLayout.GetOffsetRect(buttonRect, 3), FXMakerLayout.m_ColorHelpBox, (bSaveEnable ? 2:1), false);
			if (m_bDrawRedBottom)
				NgGUIDraw.DrawBox(FXMakerLayout.GetOffsetRect(baseRect, 2), FXMakerLayout.m_ColorHelpBox, (bSaveEnable ? 2:1), false);
		} else {
			// Undo
			int	nButtonWidht	= 70;
			buttonRect.width	= nButtonWidht/2;
			if (FXMakerLayout.GUIButton(buttonRect, GetHelpContent("Undo"), (m_SelCurveAniObject != null)))
			{
				UndoCurveAni();
				m_nObjectIndex			= -1;
				m_nSelObjectGroupIndex	= -1;
				m_nSelObjectProjectIndex= -1;
				m_SelObjectContent		= null;
				m_SelCurveAniObject		= null;

				FXMakerMain.inst.CreateCurrentInstanceEffect(true);
			}

			// close
			buttonRect.x		+= buttonRect.width + 5;
			buttonRect.width	= baseRect.width - buttonRect.x;
			if (GUI.Button(buttonRect, GetHelpContent("Close")))
				ClosePopup(true);
		}
	}

	// ----------------------------------------------------------------------------------------------------------
	protected override void LoadObjects()
	{
		m_CurveAniObjects	= null;
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
		m_LoadDirectory		= NgFile.PathSeparatorNormalize(loaddir);
		m_CurveAniObjects	= NgAsset.GetCurvePrefabList(m_LoadDirectory, "", true, FXMakerLayout.m_nMaxPrefabListCount, out m_nObjectCount);

		ArrayList curveAnis	= new ArrayList();
		foreach (GameObject obj in m_CurveAniObjects)
		{
			if (obj.GetComponent<NcCurveAnimation>() != null && obj.name.Contains(FxmFolderPopup_NcInfoCurve.m_NcInfoCurve_FileKeyword) == false)
				curveAnis.Add(obj);
		}

		m_CurveAniObjects	= NgConvert.ToArray<GameObject>(curveAnis);
		if (m_CurveAniObjects == null)
			m_CurveAniObjects = new GameObject[0];
		m_nObjectCount		= m_CurveAniObjects.Length;
		m_ObjectContents	= new GUIContent[m_nObjectCount];

		// Current Select
		if (m_SelCurveAniObject != null)
		{
			for (int n=0; n < m_nObjectCount; n++)
			if (m_CurveAniObjects[n] == m_SelCurveAniObject)
				m_nObjectIndex = n;
		}

		BuildContents();
	}

	void BuildContents()
	{
		CancelInvoke("BuildContents");
		if (enabled == false)
			return;

		int nNotLoadPreviewCount = 0;
		for (int n=0; n < m_nObjectCount; n++)
		{
			if (m_ObjectContents[n] == null)
			{
				string subDir = AssetDatabase.GetAssetPath(m_CurveAniObjects[n]);
				subDir = NgFile.PathSeparatorNormalize(subDir).Replace(m_LoadDirectory, "");

				m_ObjectContents[n]			= new GUIContent();
				m_ObjectContents[n].text	= m_CurveAniObjects[n].name;
				m_ObjectContents[n].tooltip	= FXMakerTooltip.Tooltip(m_CurveAniObjects[n].name + "\n" + subDir);
			}
			if (m_ObjectContents[n].image == null)
			{
				m_ObjectContents[n].image	= FXMakerMain.inst.GetPrefabThumbTexture(m_CurveAniObjects[n]);
				if (m_ObjectContents[n].image != null)
					m_ObjectContents[n].tooltip	+= FXMakerTooltip.AddPopupPreview(m_ObjectContents[n].image);
			}
			if (m_ObjectContents[n].image == null)
				nNotLoadPreviewCount++;
		}
		if (0 < nNotLoadPreviewCount)
			Invoke("BuildContents", FXMakerLayout.m_fReloadPreviewTime);
	}

	// ----------------------------------------------------------------------------------------------------------
	protected override void SetActiveObject(int nObjectIndex)
	{
		if (nObjectIndex < 0 || m_nObjectCount <= nObjectIndex)
			return;

		GameObject	selCurveAniObj = m_CurveAniObjects[nObjectIndex];

		// right button, image Ping and not select
		if (Input.GetMouseButtonUp(1))
		{
			FXMakerAsset.SetPingObject(selCurveAniObj);
			return;
		}

		// 같은것 재클릭
		if (m_nObjectIndex == nObjectIndex)
		{
			FXMakerAsset.SetPingObject(m_CurrentCurveAnimation);
			FXMakerMain.inst.CreateCurrentInstanceEffect(true);
			return;
		}

		SetCurveAni(selCurveAniObj);
		FXMakerMain.inst.CreateCurrentInstanceEffect(true);

		m_nObjectIndex			= nObjectIndex;
		m_nSelObjectProjectIndex= m_nProjectIndex;
		m_nSelObjectGroupIndex	= (m_bOptionRecursively ? -1 : m_nGroupIndex);
		m_SelObjectContent		= new GUIContent(m_ObjectContents[nObjectIndex].text, m_ObjectContents[nObjectIndex].image, m_ObjectContents[nObjectIndex].tooltip);
	}

	void SetCurveAni(GameObject selCurveAniObj)
	{
		NcCurveAnimation	ncCurveAni = selCurveAniObj.GetComponent<NcCurveAnimation>();
		m_SelCurveAniObject	= selCurveAniObj;
		// copy
// 		ncCurveAni.CopyTo(m_CurrentCurveAnimation, false);
		// append
		m_OriCurveAnimation.CopyTo(m_CurrentCurveAnimation, false);
		ncCurveAni.AppendTo(m_CurrentCurveAnimation, false);
	}

	void UndoCurveAni()
	{
		// Restore
		m_OriCurveAnimation.CopyTo(m_CurrentCurveAnimation, false);
		m_SelCurveAniObject	= null;
	}

	void SaveCurveAniToPrefabFile()
	{
		if (m_nProjectIndex < 0)
			return;

		if (m_DefaultSavePrefab == null)
		{
			Debug.LogError("FxmFolderPopup_NcCurveAnimation.m_DefaultSavePrefab is null");
			return;
		}

		string dstPath = NgFile.CombinePath(m_LoadDirectory, m_NcCurveAni_FileKeyword + m_SaveFilename + ".prefab");

		dstPath = NgAsset.CreateDefaultUniquePrefab(m_DefaultSavePrefab, dstPath);

		GameObject			newPrefab	= NgAsset.LoadPrefab(dstPath);
		NcCurveAnimation	newCom		= newPrefab.AddComponent<NcCurveAnimation>();
		m_CurrentCurveAnimation.CopyTo(newCom, false);
		FXMakerAsset.AssetDatabaseSaveAssets();
	}

	// Control Function -----------------------------------------------------------------

	// Event Function -------------------------------------------------------------------


	// -------------------------------------------------------------------------------------------
}
#endif
