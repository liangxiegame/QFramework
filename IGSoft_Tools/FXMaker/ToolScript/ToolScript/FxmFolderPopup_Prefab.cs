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
using System.Collections.Generic;

// 	ShowAddPrefabPopup(Transform trans)
// 	ShowSavePrefabPopup(Transform trans)
// 	ShowSelectPrefabPopup(NcAttachPrefab selCom)
// 	ShowSelectMeshPopup(Component selCom)

public class FxmFolderPopup_Prefab : FxmFolderPopup
{
	// Attribute ------------------------------------------------------------------------
	// const

	protected		enum DIALOG_TYPE	{ADD, SAVE, SELECT};
	// popup
	public			string				m_DefaultPrefabProjectName	= "[EffectMesh]";
	public			string				m_DefaultMeshProjectName	= "[Resources]";
	public			string				m_DefaultMeshGroupName		= "[Mesh]";
	protected		string				m_CaptionName;
// 	protected		GameObject[]		m_PrefabObjects;
	protected		GameObject			m_AddPrefabObject;

	protected		DIALOG_TYPE			m_bDialogType;
	protected		NcEffectBehaviour	m_BaseSelectPrefab;
	protected		int					m_SelectnArgIndex;
	protected		int					m_SelectnSubArgIndex;
	protected		GameObject			m_OldSelectPrefab;

	protected		bool				m_bSelectMeshDialog;
	protected		Component			m_SelectMeshComponent;
	protected		Mesh				m_OldSelectMesh;
	protected		List<GameObject>	m_AddGameObjectList;
	protected		NgEnum.PREFAB_TYPE	m_LoadPrefabType;

	// Property -------------------------------------------------------------------------
	public virtual bool ShowPrefabPopupWindow(Transform selTrans, bool bSaveDialog)
	{
		m_LoadPrefabType = NgEnum.PREFAB_TYPE.All;
		if (bSaveDialog)
		{
			m_CaptionName		= "Save Prefab - " + m_LoadPrefabType.ToString();
			m_PrefsName			= "SavePrefab";
			SetShowOption(selTrans, DIALOG_TYPE.SAVE, false);
		} else {
			m_CaptionName		= "Add Prefab - " + m_LoadPrefabType.ToString();
			m_PrefsName			= "AddPrefab";
			SetShowOption(selTrans, DIALOG_TYPE.ADD, false);
		}

		return base.ShowPopupWindow(selTrans, bSaveDialog);
	}

	public virtual bool ShowSelectPopupWindow(NcEffectBehaviour selObj, int nArgIndex, int nSubArgIndex)
	{
		m_BaseSelectPrefab		= selObj;
		m_SelectnArgIndex		= nArgIndex;
		m_SelectnSubArgIndex	= nSubArgIndex;
		m_OldSelectPrefab		= GetAttachPrefab();
		m_CaptionName			= "Select Prefab - " + m_LoadPrefabType.ToString();
		m_PrefsName				= "SelectPrefab";

		SetShowOption(m_BaseSelectPrefab.transform, DIALOG_TYPE.SELECT, false);
		return base.ShowPopupWindow(m_BaseSelectPrefab.transform, false);
	}

	public virtual bool ShowSelectMeshPopupWindow(Object selObj)
	{
		m_LoadPrefabType		= NgEnum.PREFAB_TYPE.All;
		m_SelectMeshComponent	= selObj as Component;
		m_OldSelectMesh			= NgSerialized.GetMesh(m_SelectMeshComponent, true) as Mesh;
		m_CaptionName			= "Select Mesh - " + m_LoadPrefabType.ToString();
		m_PrefsName				= "SelectMesh";

		SetShowOption(m_SelectMeshComponent.transform, DIALOG_TYPE.SELECT, true);
		return base.ShowPopupWindow(m_SelectMeshComponent.transform, false);
	}

	void SetShowOption(Transform selTrans, DIALOG_TYPE dialogType, bool bMeshDialog)
	{
		// SaveDialog - Disable m_bOptionRecursively
		if (dialogType == DIALOG_TYPE.SAVE)
		{
			m_SaveFilename					= selTrans.name;
			m_bFixedOptionRecursively		= true;
			m_bOptionRecursively			= false;
		} else m_bFixedOptionRecursively	= false;


		m_DefaultProjectName	= (bMeshDialog ? m_DefaultMeshProjectName : m_DefaultPrefabProjectName);
		m_BaseDefaultGroupName	= (bMeshDialog ? m_DefaultMeshGroupName : "");
		m_bDialogType			= dialogType;
		m_bSelectMeshDialog		= bMeshDialog;

		// Save Hint Box
		m_bDrawRedProject		= dialogType == DIALOG_TYPE.SAVE;
		m_bDrawRedGroup			= dialogType == DIALOG_TYPE.SAVE;
		m_bDrawRedBottom		= dialogType == DIALOG_TYPE.SAVE;

		// Common Init
		m_nOriMaxObjectColumn	= 3;
		m_fButtonAspect			= FXMakerLayout.m_fScrollButtonAspect;
		m_SelectedTransform		= selTrans;

		// Init
		m_AddGameObjectList		= new List<GameObject>();
	}

	public override void ClosePopup(bool bSave)
	{
		if (m_bDialogType == DIALOG_TYPE.SELECT)
			base.ClosePopup(false);
		else base.ClosePopup(bSave);
	}

	GameObject GetObjectNodePrefab(int nObjIndex)
	{
		if (m_ObjectNodes[nObjIndex].m_Object != null)
			return (GameObject)m_ObjectNodes[nObjIndex].m_Object;
		m_ObjectNodes[nObjIndex].m_Object = AssetDatabase.LoadAssetAtPath(m_ObjectNodes[nObjIndex].m_AssetPath, typeof(GameObject));
		return (GameObject)m_ObjectNodes[nObjIndex].m_Object;
	}

	// -------------------------------------------------------------------------------------------
	void Awake()
	{
		m_nObjectColumn	= 5;
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
//		FXMakerMain.inst.PopupFocusWindow(NgLayout.GetWindowId(NgLayout.WINDOWID.POPUP), GetPopupRect(), winPopup, m_CaptionName);
		FXMakerMain.inst.ModalMsgWindow(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.POPUP), GetPopupRect(), winPopup, m_CaptionName);
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
			bool bPreviewEnable = false;
			// image
			if (m_SelObjectContent == null)
			{
				guiCon = new GUIContent("[Not Selected]");
				// original mesh preview
				if (m_bDialogType == DIALOG_TYPE.SELECT && m_bSelectMeshDialog && m_OldSelectMesh != null)
				{
					Texture2D preview = NgAsset.GetAssetPreview(m_OldSelectMesh);
					guiCon	= new GUIContent(m_OldSelectMesh.name, preview, GetMeshInfo(m_OldSelectMesh));
					bPreviewEnable = true;
				}
			} else {
				guiCon = new GUIContent("", m_SelObjectContent.image, m_SelObjectContent.tooltip);
				bPreviewEnable = (m_SelObjectContent != null);
			}

			if (FXMakerLayout.GUIButton(imageRect, guiCon, GUI.skin.GetStyle("PopupBottom_ImageButton"), bPreviewEnable))
			{
				if (m_bDialogType == DIALOG_TYPE.SELECT)
				{
					if (m_bSelectMeshDialog)
					{
						Object pingObj;
						Object pingMesh;
						if (m_AddPrefabObject != null)
						{
							pingObj		= m_AddPrefabObject;
							pingMesh	= m_AddPrefabObject.GetComponent<MeshFilter>().sharedMesh;
						} else {
							pingObj		= m_OldSelectMesh;
							pingMesh	= m_OldSelectMesh;
						}

						if (Input.GetMouseButtonUp(0))
						{
							FXMakerAsset.SetPingObject(pingObj);
							FXMakerMain.inst.CreateCurrentInstanceEffect(true);
						}
						if (Input.GetMouseButtonUp(1))
							FXMakerAsset.SetPingObject(pingMesh);
					} else {
						FXMakerMain.inst.CreateCurrentInstanceEffect(true);
					}
				} else {
					if (Input.GetMouseButtonUp(0))
					{
						FXMakerAsset.SetPingObject(m_AddGameObject);
						FXMakerMain.inst.CreateCurrentInstanceEffect(true);
					}
					if (Input.GetMouseButtonUp(1))
						FXMakerAsset.SetPingObject(GetObjectNodePrefab(m_nObjectIndex));
				}
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
				SaveCurrentObjectToPrefabFile();
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
			if (FXMakerLayout.GUIButton(buttonRect, GetHelpContent("Undo"), (m_AddGameObject != null)))
				UndoObject();

			// close
			buttonRect.x		+= buttonRect.width + 5;
			buttonRect.width	= baseRect.width - buttonRect.x;
			if (GUI.Button(buttonRect, GetHelpContent("Close")))
				ClosePopup(true);
		}
	}

	protected override void UndoObject()
	{
		if (m_bDialogType == DIALOG_TYPE.SELECT)
		{
			if (m_bSelectMeshDialog)
			{
				NgSerialized.SetMesh(m_SelectMeshComponent, m_OldSelectMesh, true);
				FXMakerHierarchy.inst.UpdateMeshList();
			} else SetAttachPrefab(m_OldSelectPrefab, true);
			m_AddGameObject		= null;
		} else {
			m_AddGameObjectList.Remove(m_AddGameObject);
			DestroyImmediate(m_AddGameObject);
			if (0 < m_AddGameObjectList.Count)
				m_AddGameObject	= m_AddGameObjectList[0];
		}
		m_nObjectIndex			= -1;
		m_nSelObjectGroupIndex	= -1;
		m_nSelObjectProjectIndex= -1;
		m_SelObjectContent		= null;
		m_AddPrefabObject		= null;

		if (m_bDialogType == DIALOG_TYPE.SELECT)
		{
			FXMakerMain.inst.CreateCurrentInstanceEffect(true);
		} else {
			if (FXMakerHierarchy.inst.SetActiveGameObject(m_SelectedTransform.gameObject) == false)
				FXMakerMain.inst.CreateCurrentInstanceEffect(true);
		}
	}

	// ----------------------------------------------------------------------------------------------------------
	protected override void LoadObjects()
	{
// 		Debug.Log("call LoadObjects");

		m_ObjectNodes		= null;
		m_ObjectContents	= null;
		m_nObjectCount		= 0;
		m_nObjectIndex		= -1;

		if (0 < m_nGroupCount)
		{
			string	loaddir;
			if (m_bOptionRecursively)
				 loaddir	= NgFile.CombinePath(FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.PROJECTS), m_ProjectFolerContents[m_nProjectIndex].text + "/");
			else loaddir	= GetCurrentDirPath(m_nGroupIndex);
			AddObjects(loaddir);
		}
	}

	string GetCurrentDirPath(int nGroupIndex)
	{
		string groupName = m_GroupFolderContents[nGroupIndex].text;
		if (groupName.Contains("Res.."))
		{
			return NgFile.CombinePath("Assets/Resources/", groupName  + "/");
		} else
		return NgFile.CombinePath(FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.PROJECTS), m_ProjectFolerContents[m_nProjectIndex].text + "/" + m_GroupFolderContents[nGroupIndex].text  + "/");
	}

	void AddObjects(string loaddir)
	{
		m_LoadDirectory	 = NgFile.PathSeparatorNormalize(loaddir);
// 		if (m_bSelectMeshDialog)
// 			 m_PrefabObjects	= NgAsset.GetMeshList(m_LoadDirectory, true, FXMakerLayout.m_nMaxPrefabListCount, out m_nObjectCount);
// 		else m_PrefabObjects	= NgAsset.GetPrefabList(m_LoadDirectory, false, true, FXMakerLayout.m_nMaxPrefabListCount, out m_nObjectCount);
		if (m_bSelectMeshDialog)
			 m_ObjectNodes	= NgAsset.GetMeshList(m_LoadDirectory, true, FXMakerLayout.m_nMaxPrefabListCount, out m_nObjectCount);
		else m_ObjectNodes	= NgAsset.GetPrefabList(m_LoadDirectory, false, true, FXMakerLayout.m_nMaxPrefabListCount, (m_LoadPrefabType == NgEnum.PREFAB_TYPE.All ? true : false), m_LoadPrefabType, out m_nObjectCount);

		m_ObjectContents = new GUIContent[m_nObjectCount];

		// Current Select
		if (m_AddGameObject != null)
		{
			for (int n=0; n < m_nObjectCount; n++)
				if (m_ObjectNodes[n].m_AssetPath == AssetDatabase.GetAssetPath(m_AddPrefabObject))
					m_nObjectIndex = n;
		}
		BuildContents();
	}

	void BuildContents()
	{
// 		Debug.Log("call BuildContents");

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
			m_nNotLoadCount++;
		}
	}
	void AyncLoadObject()
	{
		if (enabled == false)
			return;
		NcTickTimerTool	tickTimer = new NcTickTimerTool();

		for (int n=0; n < m_nObjectCount; n++)
		{
			if (m_ObjectNodes[n].m_Object == null || m_ObjectContents[n].image == null)
			{
				if (m_ObjectNodes[n].m_Object == null)
				{
					m_ObjectNodes[n].m_Object	= GetObjectNodePrefab(n);
				}
				if (m_ObjectContents[n].image == null)
 					m_ObjectContents[n].image	= FXMakerMain.inst.GetPrefabThumbTexture(GetObjectNodePrefab(n));
				if (m_ObjectContents[n].image != null)
				{
					string subDir = AssetDatabase.GetAssetPath(GetObjectNodePrefab(n));
					subDir = NgFile.PathSeparatorNormalize(subDir).Replace(m_LoadDirectory, "");
					m_ObjectContents[n].tooltip	= FXMakerTooltip.Tooltip(GetObjectInfo(GetObjectNodePrefab(n), subDir));
					m_ObjectContents[n].tooltip	+= FXMakerTooltip.AddPopupPreview(m_ObjectContents[n].image);
					m_nNotLoadCount--;
				}

				if (300 < tickTimer.GetStartedTickCount())
					return;
			}
		}
	}

	string GetObjectInfo(GameObject currentObj, string subDir)
	{
		if (m_bSelectMeshDialog)
		{
			int			nVertices;
			int			nTriangles;
			int			nMeshCount;
			MeshFilter	meshFilter	= currentObj.GetComponent<MeshFilter>();
			Vector3		size		= meshFilter.sharedMesh.bounds.extents * 2;

			NgObject.GetMeshInfo(currentObj, false, out nVertices, out nTriangles, out nMeshCount);
			return string.Format("- {0}\n- MeshCount = {1}\n Mesh: Triangles = {2} Vertices = {3}\n- size: {4},{5},{6}", subDir, nMeshCount, nTriangles, nVertices, size.x.ToString("0.0"), size.y.ToString("0.0"), size.z.ToString("0.0"));
		} else {
			return currentObj.name + "\n" + subDir;
		}
	}

	string GetMeshInfo(Mesh mesh)
	{
		if (m_bSelectMeshDialog)
		{
			int			nVertices;
			int			nTriangles;
			int			nMeshCount;
			Vector3		size		= mesh.bounds.extents * 2;

			NgObject.GetMeshInfo(mesh, out nVertices, out nTriangles, out nMeshCount);
			return string.Format("- {0}\n- MeshCount = {1}\n Mesh: Triangles = {2} Vertices = {3}\n- size: {4},{5},{6}", "Current Mesh", nMeshCount, nTriangles, nVertices, size.x.ToString("0.0"), size.y.ToString("0.0"), size.z.ToString("0.0"));
		}
		return "";
	}

	// ----------------------------------------------------------------------------------------------------------
	protected override void SetActiveObject(int nObjectIndex)
	{
		if (nObjectIndex < 0 || m_nObjectCount <= nObjectIndex)
			return;

		bool		bAppend	= false;
		GameObject	selPrefab = GetObjectNodePrefab(nObjectIndex);

		// right button, image Ping and not select
		if (Input.GetMouseButtonUp(1))
		{
			if (m_bDialogType == DIALOG_TYPE.ADD && FXMakerOption.inst.m_PrefabDlg_RightClick == FXMakerOption.DLG_RIGHTCLICK.APPEND)
			{
				bAppend	= true;
			} else {
//	 			if (m_bDialogType == DIALOG_TYPE.SELECT == false)
					FXMakerAsset.SetPingObject(selPrefab);
				return;
			}
		}

		// 같은것 재클릭
		if (m_nObjectIndex == nObjectIndex)
		{
//			if (m_bDialogType != DIALOG_TYPE.SELECT)
				FXMakerAsset.SetPingObject(m_AddGameObject);
			FXMakerMain.inst.CreateCurrentInstanceEffect(true);
			return;
		}

		if (m_bDialogType == DIALOG_TYPE.SELECT)
		{
			m_AddPrefabObject	= selPrefab;
			m_AddGameObject		= selPrefab;

			if (m_bSelectMeshDialog)
			{
				MeshFilter meshFilter = m_AddPrefabObject.GetComponent<MeshFilter>();
				if (meshFilter != null && meshFilter.sharedMesh != null)
				{
					NgSerialized.SetMesh(m_SelectMeshComponent, meshFilter.sharedMesh, true);
					FXMakerHierarchy.inst.UpdateMeshList();
				}
			} else {
				SetAttachPrefab(selPrefab, false);
			}
			FXMakerMain.inst.CreateCurrentInstanceEffect(true);
		} else {
			if (bAppend == false)
			{
				// 기존것 삭제
				if (m_AddGameObject != null)
				{
					m_AddGameObjectList.Remove(m_AddGameObject);
					DestroyImmediate(m_AddGameObject);
					m_AddGameObject = null;
				}
			}

			// create
			m_AddPrefabObject	= selPrefab;
			m_AddGameObject		= FXMakerHierarchy.inst.AddGameObject(m_SelectedTransform.gameObject, selPrefab);
			m_AddGameObjectList.Insert(0, m_AddGameObject);

			if (FXMakerHierarchy.inst.SetActiveGameObject(m_AddGameObject) == false)
				FXMakerMain.inst.CreateCurrentInstanceEffect(true);
		}

		m_nObjectIndex			= nObjectIndex;
		m_nSelObjectProjectIndex= m_nProjectIndex;
		m_nSelObjectGroupIndex	= (m_bOptionRecursively ? -1 : m_nGroupIndex);
		m_SelObjectContent		= new GUIContent(m_ObjectContents[nObjectIndex].text, m_ObjectContents[nObjectIndex].image, m_ObjectContents[nObjectIndex].tooltip);
	}

	void SaveCurrentObjectToPrefabFile()
	{
		if (m_nProjectIndex < 0)
			return;

		if (FXMakerEffect.inst.m_DefaultEmptyPrefab == null)
		{
			Debug.LogError("FXMakerMain.m_DefaultEmptyPrefab is null");
			return;
		}

		string dstPath = NgFile.CombinePath(m_LoadDirectory, m_SaveFilename + ".prefab");

		dstPath = NgAsset.CreateDefaultUniquePrefab(FXMakerEffect.inst.m_DefaultEmptyPrefab, dstPath);

		GameObject			newPrefab	= NgAsset.LoadPrefab(dstPath);
		PrefabUtility.ReplacePrefab(m_SelectedTransform.gameObject, newPrefab);
		FXMakerAsset.AssetDatabaseSaveAssets();
	}

	// Control Function -----------------------------------------------------------------
	GameObject GetAttachPrefab()
	{
		m_LoadPrefabType = NgEnum.PREFAB_TYPE.All;
		if (m_BaseSelectPrefab is NcAttachPrefab)
			return (m_BaseSelectPrefab as NcAttachPrefab).m_AttachPrefab;
		if (m_BaseSelectPrefab is NcParticleSystem)
			return (m_BaseSelectPrefab as NcParticleSystem).m_AttachPrefab;
		if (m_BaseSelectPrefab is NcSpriteFactory)
		{
// 			if (m_SelectnSubArgIndex == 0)
			return (m_BaseSelectPrefab as NcSpriteFactory).m_SpriteList[m_SelectnArgIndex].m_EffectPrefab;
		}
		if (m_BaseSelectPrefab is NcSpriteTexture)
		{
			m_LoadPrefabType = NgEnum.PREFAB_TYPE.NcSpriteFactory;
			return (m_BaseSelectPrefab as NcSpriteTexture).m_NcSpriteFactoryPrefab;
		}
		if (m_BaseSelectPrefab is NcSpriteAnimation)
		{
			m_LoadPrefabType = NgEnum.PREFAB_TYPE.NcSpriteFactory;
			return (m_BaseSelectPrefab as NcSpriteAnimation).m_NcSpriteFactoryPrefab;
		}
		if (m_BaseSelectPrefab is NcParticleSpiral)
		{
			m_LoadPrefabType = NgEnum.PREFAB_TYPE.LegacyParticle;
			return (m_BaseSelectPrefab as NcParticleSpiral).m_ParticlePrefab;
		}
		if (m_BaseSelectPrefab is NcParticleEmit)
		{
			m_LoadPrefabType = NgEnum.PREFAB_TYPE.ParticleSystem;
			return (m_BaseSelectPrefab as NcParticleEmit).m_ParticlePrefab;
		}
		if (m_BaseSelectPrefab is FxmInfoBackground)
			return null;
		Debug.LogError("GetAttachPrefab() error");
		return null;
	}

	void SetAttachPrefab(GameObject attachPrefab, bool bUndo)
	{
		if (bUndo == false && m_BaseSelectPrefab is NcAttachPrefab)
		{
			if (FXMakerHierarchy.inst.GetSelectedGameObject() != null && m_BaseSelectPrefab.gameObject != FXMakerHierarchy.inst.GetSelectedGameObject())
			{
				NcAttachPrefab ncAttCom = FXMakerHierarchy.inst.GetSelectedGameObject().GetComponent<NcAttachPrefab>();
				if (ncAttCom != null)
				{
					m_BaseSelectPrefab		= ncAttCom;
					m_OldSelectPrefab		= GetAttachPrefab();
				}
			}
		}

		if (m_BaseSelectPrefab is NcAttachPrefab)
			(m_BaseSelectPrefab as NcAttachPrefab).m_AttachPrefab = attachPrefab;
		if (m_BaseSelectPrefab is NcParticleSystem)
			(m_BaseSelectPrefab as NcParticleSystem).m_AttachPrefab = attachPrefab;
		if (m_BaseSelectPrefab is NcSpriteFactory)
		{
//			if (m_SelectnSubArgIndex == 0)
			(m_BaseSelectPrefab as NcSpriteFactory).m_SpriteList[m_SelectnArgIndex].m_EffectPrefab = attachPrefab;
		}
		if (m_BaseSelectPrefab is NcSpriteTexture)
			(m_BaseSelectPrefab as NcSpriteTexture).m_NcSpriteFactoryPrefab = attachPrefab;
		if (m_BaseSelectPrefab is NcSpriteAnimation)
			(m_BaseSelectPrefab as NcSpriteAnimation).m_NcSpriteFactoryPrefab = attachPrefab;
		if (m_BaseSelectPrefab is NcParticleSpiral)
			(m_BaseSelectPrefab as NcParticleSpiral).m_ParticlePrefab = attachPrefab;
		if (m_BaseSelectPrefab is NcParticleEmit)
			(m_BaseSelectPrefab as NcParticleEmit).m_ParticlePrefab = attachPrefab;
		if (m_BaseSelectPrefab is FxmInfoBackground)
			(m_BaseSelectPrefab as FxmInfoBackground).SetReferenceObject(m_SelectnArgIndex, attachPrefab);
		return;
	}

	// Event Function -------------------------------------------------------------------


	// -------------------------------------------------------------------------------------------
}
#endif
