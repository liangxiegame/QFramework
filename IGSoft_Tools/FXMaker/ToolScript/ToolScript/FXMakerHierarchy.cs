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

public class FXMakerHierarchy : MonoBehaviour
{
	// Attribute ------------------------------------------------------------------------
	public			static FXMakerHierarchy		inst;

	// const

	public			enum OBJECT_TYPE	{ OBJECT_GAMEOBJECT, OBJECT_TRANSFORM, OBJECT_EASYEFFECT, OBJECT_UNITYENGINE, OBJECT_ANICLIP, OBJECT_MATERIAL, OBJECT_OTHER };
	public			Texture2D			m_DisableTexture;
	public			Texture2D			m_WarringTexture;

	protected		int					m_nTreeLeftMagin			= 8;
	protected		GameObject			m_CurrentEffectObject;

	// const
	protected		const int			m_nGridButtonHeightCount	= 2;
	protected		const int			m_nGridButtonWidthCount		= 19;
	protected		int					m_nGridCellSize				= 7;
	protected		int					m_nMaxGridCellSize			= 10;
	protected		bool				m_bMinimize					= false;
	protected		bool				m_bShowOption				= true;

	// �ڵ� ����
	protected		int					m_nMaxGridColumn;
	protected		int					m_nMaxGridRow;
	protected		Rect				m_HierarchyRect;
	protected		Vector2				m_HierarchyScrollPos;
	protected		bool				m_bUpdateHierarchyScrollPos;
	protected		GameObject			m_SelectedGameObject;
	protected		Object				m_ActiveComponent;
	protected		Object				m_HoverComponent;
	protected		Transform			m_HoverComponentTrans;
	protected		bool				m_bUpdateActiveComponent;
	protected		Vector2				m_AbsScrollPos;
	protected		float				m_fScriptSpeed	= 0;
	protected		int					m_nLastDrawRow;

	protected		Object				m_DragObject;
	protected		Transform			m_DragObjectTrans;
	protected		int					m_nDragObjectIndex;
	protected		string				m_DragObjectName;

	protected		Dictionary<Component, bool>	m_MeshList;

	// option
	protected		bool[]				m_ShowComponentOptions			= { true, true, true, true, true, true };
	protected		enum 				SHOWCOMPONENT_TYPE				  { TRANSFORM=0, EASYEFFECT, UNITYENGINE, MATERIAL, ANIMATION, OTHER, COUNT };
	protected		string[]			m_ShowComponentStrings			= { "Transform", "FXMaker", "UnityEngine", "Material", "Animation", "Other" };
	protected		int					m_nShowGameObjectOptionIndex	= 0;
	protected		string[]			m_ShowGameObjectStrings			= { "All", "Selected" };

	// key input
	protected		GameObject			m_ArrowMovePrevObj;
	protected		Object				m_ArrowMovePrevCom;

	// gui style
	protected		GUIStyle			m_styleButton;
	protected		GUIStyle			m_styleButtonActive;
	protected		GUIStyle			m_styleButtonAddPrefab;
	protected		GUIStyle			m_styleBox;
	protected		GUIStyle			m_styleBoxActive;
	protected		GUIStyle			m_styleToggle;

	protected		bool				m_bShowNewMenuPopup		= false;

	// -------------------------------------------------------------------------------------------
	void LoadPrefs()
	{
		if (FXMakerLayout.m_bDevelopPrefs == false)
		{
			// Component ShowOption -------------------------------------------------
			for (int n = 0; n < m_ShowComponentOptions.Length; n++)
				m_ShowComponentOptions[n]	= EditorPrefs.GetBool("FXMakerHierarchy.m_ShowComponentOptions"+n			, m_ShowComponentOptions[n]);

			// GameObject ShowOption -------------------------------------------------
			m_nShowGameObjectOptionIndex	= EditorPrefs.GetInt("FXMakerHierarchy.m_nShowGameObjectOptionIndex"			, m_nShowGameObjectOptionIndex);

			m_nGridCellSize	= EditorPrefs.GetInt("FXMakerHierarchy.m_nGridCellSize"	, m_nGridCellSize);
		}
		m_bMinimize		= EditorPrefs.GetBool("FXMakerHierarchy.m_bMinimize", m_bMinimize);
	}

	// -------------------------------------------------------------------------------------------
	FXMakerHierarchy()
	{
		inst = this;
	}

	// -------------------------------------------------------------------------------------------
	public static Rect GetWindowRect()
	{
		return FXMakerLayout.GetEffectHierarchyRect();
	}

	// -------------------------------------------------------------------------------------------
	void Awake()
	{
		NgUtil.LogDevelop("Awake - FXMakerHierarchy");
		LoadPrefs();

		if (m_DisableTexture == null)
			Debug.LogError("m_DisableTexture is null !!! - Disanble.png");
		if (m_WarringTexture == null)
			Debug.LogError("m_WarringTexture is null !!! - Warring.png");
	}

	void OnEnable()
	{
		NgUtil.LogDevelop("OnEnable - FXMakerHierarchy");
		LoadPrefs();
		ShowHierarchy(FXMakerMain.inst.GetOriginalEffectObject());
	}

	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		// enable save
		if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
		{
			Rect rect = FXMakerLayout.GetEffectHierarchyRect();
			if (rect.Contains(NgLayout.GetGUIMousePosition()))
				FXMakerEffect.inst.SetChangePrefab();
		}

		// ShotKey - Delete
		if (FXMakerMain.inst.GetFocusInputKey(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.EFFECT_HIERARCHY)) == KeyCode.Delete || FXMakerMain.inst.GetFocusInputKey(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.NONE)) == KeyCode.Delete)
		{
			if (m_ActiveComponent != null && (m_ActiveComponent is Transform) == false && (m_ActiveComponent is GameObject || m_ActiveComponent is Component))
			{
				GameObject	originalRoot = FXMakerMain.inst.GetOriginalEffectObject();
				if (m_ActiveComponent != originalRoot)
				{
					DeleteHierarchyObject(m_ActiveComponent);
					SetActiveComponent(null, null, true);
				}
			}
			FXMakerMain.inst.SetFocusInputKey(0);
		}

		// ShotKey - View Grid Arrow
		bool bArrowMoved	= false;
		if (m_SelectedGameObject != null)
		{
			if (FXMakerMain.inst.GetFocusInputKey(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.NONE)) == KeyCode.LeftArrow)
			{
				AddTranslate(-(FXMakerOption.inst.m_fArrowGridMoveUnit), 0, 0, false);
				bArrowMoved = true;
			}
			if (FXMakerMain.inst.GetFocusInputKey(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.NONE)) == KeyCode.RightArrow)
			{
				AddTranslate(+(FXMakerOption.inst.m_fArrowGridMoveUnit), 0, 0, false);
				bArrowMoved = true;
			}
			if (FXMakerMain.inst.GetFocusInputKey(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.NONE)) == KeyCode.UpArrow)
			{
				AddTranslate(0, +(FXMakerOption.inst.m_fArrowGridMoveUnit), 0, false);
				bArrowMoved = true;
			}
			if (FXMakerMain.inst.GetFocusInputKey(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.NONE)) == KeyCode.DownArrow)
			{
				AddTranslate(0, -(FXMakerOption.inst.m_fArrowGridMoveUnit), 0, false);
				bArrowMoved = true;
			}
			if (bArrowMoved && FXMakerOption.inst.m_ArrowGridMoveType == FXMakerOption.ARROWMOVE_TYPE.MULTIPLE)
				FXMakerGizmo.inst.UpdateGridMove();
		}

		// Speed
		if (m_fScriptSpeed != 0 && Input.GetMouseButtonUp(0))
		{
			float fSpeedRate = (0 <= m_fScriptSpeed ? m_fScriptSpeed+1 : (m_fScriptSpeed/2.0f+1.0f));
			fSpeedRate = ((int)(fSpeedRate * 100)) / 100.0f;
			ChangeGameObjectSpeed(m_SelectedGameObject, fSpeedRate);
			FXMakerMain.inst.CreateCurrentInstanceEffect(true);
			m_fScriptSpeed = 0f;
		}

		// Check NewGameObject rightMenu
		CheckAddObjectRightPopup();
	}

	void AddTranslate(float x, float y, float z, bool bLocal)
	{
// 		Debug.Log(bLocal);
// 		Debug.Log(x);
		List<FxmInfoIndexing>	indexComs = FxmInfoIndexing.FindInstanceIndexings(m_SelectedGameObject.transform, false);
		foreach (FxmInfoIndexing indexCom in indexComs)
			indexCom.transform.Translate(x, y, z, (bLocal ? Space.Self : Space.World));
		m_SelectedGameObject.transform.Translate(x, y, z, (bLocal ? Space.Self : Space.World));
		FXMakerEffect.inst.SetChangePrefab();
	}


	public void DeleteHierarchyObject(Object delObj)
	{
		DeleteHierarchyObject(null, delObj, 0);
	}

	public void DeleteHierarchyObject(Transform baseTrans, Object delObj, int nSelectedIndex)
	{
		// enable save
		FXMakerEffect.inst.SetChangePrefab();

		if (delObj is Component)
			OnDeleteComponent(delObj as Component);
		if (delObj is Material || delObj is AnimationClip)
			NgMaterial.RemoveSharedMaterial(baseTrans.GetComponent<Renderer>(), nSelectedIndex);
		else Object.DestroyImmediate(delObj);
		FXMakerMain.inst.CreateCurrentInstanceEffect(true);
	}

	public void OnGUIHierarchy()
	{
		m_styleButton			= GUI.skin.GetStyle("Hierarchy_Button");
		m_styleButtonActive		= GUI.skin.GetStyle("Hierarchy_ButtonActive");
		m_styleButtonAddPrefab	= GUI.skin.GetStyle("Hierarchy_ButtonAddPrefab");
		m_styleBox				= GUI.skin.GetStyle("Hierarchy_Box");
		m_styleBoxActive		= GUI.skin.GetStyle("Hierarchy_BoxActive");
		m_styleToggle			= GUI.skin.GetStyle("Hierarchy_Toggle");

		if (FXMakerMain.inst.GetOriginalEffectObject() != m_CurrentEffectObject)
			ShowHierarchy(FXMakerMain.inst.GetOriginalEffectObject());

		// Effect Hierarchy Window ----------------------------------------------
		Rect rect = FXMakerLayout.GetEffectHierarchyRect();
		if (FXMakerLayout.m_bMinimizeAll || m_bMinimize)
			rect.height = FXMakerLayout.m_MinimizeHeight;
		FXMakerMain.inst.AutoFocusWindow(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.EFFECT_HIERARCHY), rect, winEffectHierarchy, "PrefabHierarchy");

		// Hierarchy Popup Window ------------------------------------------------------
		FxmPopupManager.inst.OnGUIHierarchyPopup();

		// Show NcSpriteAnimation Index
		ShowUVIndex(rect);
	}

	void ShowUVIndex(Rect baseRect)
	{
		if (m_SelectedGameObject == null)
			return;

		NcSpriteAnimation com = m_SelectedGameObject.GetComponent<NcSpriteAnimation>();
		if (com == null)
			return;
		GUI.Label(new Rect(baseRect.x-150, baseRect.y, 100, 20), "Sprite: " + com.m_nStartFrame.ToString() + "," + com.m_nFrameCount.ToString());

		List<FxmInfoIndexing>	indexComs = FxmInfoIndexing.FindInstanceIndexings(m_SelectedGameObject.transform, true);
		int nCount = 1;
		foreach (FxmInfoIndexing inCom in indexComs)
		{
			NcSpriteAnimation spCom = inCom.GetComponent<NcSpriteAnimation>();
			if (spCom == null)
				continue;
			Rect rect = new Rect(baseRect.x-150, baseRect.y+(nCount*20), 100, 20);
			GUI.Label(rect, spCom.GetShowIndex().ToString());
			nCount++;

			// frame pos stirng
			int nRelIndex = spCom.GetShowIndex() - spCom.m_nStartFrame;
			string framePos = "";
			for (int n = 0; n < spCom.m_nFrameCount; n++)
				framePos += (n == nRelIndex ? "o" : "-");

			// show nodename
			string nodeTextureName = "";
			NcSpriteFactory spriteList = inCom.GetComponent<NcSpriteFactory>();
			if (spriteList != null && spriteList.GetCurrentSpriteNode() != null)
				nodeTextureName = spriteList.GetCurrentSpriteNode().m_TextureName;

			if (spriteList == null || spriteList.GetCurrentSpriteNode() == null)
				 FxmPopupManager.inst.SetStaticBottomMessage(nRelIndex+1 + " / " + spCom.m_nFrameCount.ToString() + "   " + framePos + (spCom.m_bLoop ? " loop" : " "));
			else FxmPopupManager.inst.SetStaticBottomMessage(nRelIndex+1 + " / " + spCom.m_nFrameCount.ToString() + "   " + framePos + (spriteList.GetCurrentSpriteNode().m_bLoop ? " loop" : " ") + "\n" + nodeTextureName);
		}
	}

	// Property -------------------------------------------------------------------------
	public void ShowHierarchy(GameObject effectObject)
	{
		FxmPopupManager.inst.HideAllPopup(false);

		m_CurrentEffectObject = effectObject;
		UpdateMeshList();
		SetActiveComponent(effectObject, null, true);
		ResetScrollView();
	}

	public GameObject GetSelectedGameObject()
	{
		return m_SelectedGameObject;
	}

	public GameObject GetShowGameObject()
	{
		if (m_nShowGameObjectOptionIndex == 1)
			return m_SelectedGameObject;
		return m_CurrentEffectObject;
	}

	void SetHoverComponent(Transform currentTrans, Object hoverCom)
	{
		if (hoverCom != null)
		{
			m_HoverComponentTrans = currentTrans;
			m_HoverComponent = hoverCom;
		} else {
			m_HoverComponentTrans	= null;
			m_HoverComponent = null;
		}
	}

	public bool SetActiveGameObject(GameObject activeObj)
	{
		return SetActiveComponent(activeObj, null, true);
	}

	public bool SetActiveComponent(GameObject activeObj, Object activeCom, bool bAutoScroll)
	{
// 		Debug.Log("----SetActiveComponent--------------");
// 		Debug.Log(activeObj);
// 		Debug.Log(activeCom);

		GameObject	oldActiveObj = m_SelectedGameObject;

		m_SelectedGameObject		= activeObj;
		m_ActiveComponent			= activeCom;
		m_bUpdateActiveComponent	= true;

		if (m_SelectedGameObject != null && m_ActiveComponent == null)
			m_ActiveComponent = m_SelectedGameObject;

		if (m_ActiveComponent != null)
		{
			m_bUpdateHierarchyScrollPos = bAutoScroll;
			if (FXMakerMain.inst.IsLoadingProject() == false)
			{
				if (m_ActiveComponent is ParticleSystem)
					FXMakerAsset.SetPingObject(m_SelectedGameObject);
				else FXMakerAsset.SetPingObject(m_ActiveComponent);

				// �ӽ� - �ִϸ��̼� ����
				if (activeCom is AnimationClip)
				{
					activeObj.GetComponent<Animation>().clip = (activeCom as AnimationClip);
					FXMakerMain.inst.CreateCurrentInstanceEffect(true);
				}
			}
			if (oldActiveObj != m_SelectedGameObject)
			{
				if (m_nShowGameObjectOptionIndex == 1)
					FXMakerMain.inst.CreateCurrentInstanceEffect(true);
				OnChangeSelectGameObject(GetSelectedGameObject(), oldActiveObj);
				return false; // refresh
			}
		}
		return false;
	}

	public void SetEnableGameObject(GameObject gameObj, bool bEnable)
	{
		if (gameObj == null) return;
		Component com = gameObj.gameObject.GetComponent<NcDontActive>();
		if (bEnable)
		{
			if (com != null)
				DestroyImmediate(com);
		} else {
			if (com == null)
				gameObj.gameObject.AddComponent<NcDontActive>();
		}
		foreach (Transform child in gameObj.transform)
			SetEnableGameObject(child.gameObject, bEnable);
	}

	// ==========================================================================================================
	void winEffectHierarchy(int id)
	{
		int			nTopHeight		= 90;
		int			nTopBox1Height	= 18;
		int			nTopBox2Height	= 39;
		Rect		popupRect		= FXMakerLayout.GetEffectHierarchyRect();
		Rect		baseRect		= FXMakerLayout.GetOffsetRect(FXMakerLayout.GetChildVerticalRect(popupRect, 0, 1, 0, 1), -2);
		Rect		rectTop;

		// window desc -----------------------------------------------------------
		FXMakerTooltip.WindowDescription(popupRect, FXMakerLayout.WINDOWID.EFFECT_HIERARCHY, FxmPopupManager.inst.GetCurrentHierarchyPopup());
//		FxmPopupManager.inst.SetScriptTooltip("");

		// mini ----------------------------------------------------------------
		m_bMinimize = GUI.Toggle(new Rect(3, 1, FXMakerLayout.m_fMinimizeClickWidth, FXMakerLayout.m_fMinimizeClickHeight), m_bMinimize, "Mini");
		if (GUI.changed)
			EditorPrefs.SetBool("FXMakerHierarchy.m_bMinimize", m_bMinimize);
		GUI.changed = false;

		// GameObject ShowOption -------------------------------------------------
		rectTop			= baseRect;
		rectTop.height	= nTopBox1Height;
		Rect leftTop	= FXMakerLayout.GetInnerHorizontalRect(rectTop, 5, 0, 5);
		GUI.Box(leftTop, "");
		GUI.BeginGroup(leftTop);
		leftTop = FXMakerLayout.GetZeroStartRect(leftTop);
		leftTop = FXMakerLayout.GetOffsetRect(leftTop, 2, 1, 0, 0);
		for (int n = 0; n < m_ShowGameObjectStrings.Length; n++)
		{
			Rect	leftTopLine	= FXMakerLayout.GetInnerHorizontalRect(leftTop, 3, n, 1);
			bool	bCheck		= (GUI.Toggle(leftTopLine, (m_nShowGameObjectOptionIndex == n), FXMakerTooltip.GetHcEffectHierarchy(m_ShowGameObjectStrings[n])));
			if (bCheck && m_nShowGameObjectOptionIndex != n)
			{
				m_nShowGameObjectOptionIndex = n;
				EditorPrefs.SetInt("FXMakerHierarchy.m_nShowGameObjectOptionIndex", m_nShowGameObjectOptionIndex);
				FXMakerMain.inst.ResetCamera();
				FXMakerMain.inst.CreateCurrentInstanceEffect(true);
			}
		}
		GUI.EndGroup();

		
		// minimize end  --------------------------------------------------------
		if (FXMakerLayout.m_bMinimizeAll || m_bMinimize)
		{
			FXMakerMain.inst.SaveTooltip();
			return;
		}

		// ShowOption -----------------------------------------------------------
		m_bShowOption = GUI.Toggle(new Rect(popupRect.width-60, 1, FXMakerLayout.m_fMinimizeClickWidth, FXMakerLayout.m_fMinimizeClickHeight), m_bShowOption, "Option");
		if (GUI.changed)
			EditorPrefs.SetBool("FXMakerHierarchy.m_bShowOption", m_bShowOption);
		GUI.changed = false;

		if (m_bShowOption)
		{
			// Component ShowOption -------------------------------------------------
			rectTop			= baseRect;
			rectTop.y		= rectTop.y+nTopBox1Height+5;
			rectTop.height	= nTopBox2Height;
			GUI.Box(rectTop, "");
			rectTop = FXMakerLayout.GetOffsetRect(rectTop, 2, 2, 0, -4);
			for (int n = 0; n < m_ShowComponentOptions.Length; n++)
			{
				Rect	rectTopLine	= FXMakerLayout.GetInnerVerticalRect(rectTop, 2, n/3, 1);
				bool	bCheck		= (GUI.Toggle(FXMakerLayout.GetInnerHorizontalRect(rectTopLine, 3, n%3, 1), m_ShowComponentOptions[n], FXMakerTooltip.GetHcEffectHierarchy(m_ShowComponentStrings[n])));
				if (bCheck != m_ShowComponentOptions[n])
				{
					ResetScrollView();
					UnityEditor.EditorPrefs.SetBool("FXMakerHierarchy.m_ShowComponentOptions"+n, (m_ShowComponentOptions[n] = bCheck));
				}
			}

			// Resize --------------------------------------------------------------
			Rect scrollRect		= baseRect;
			scrollRect.y		= rectTop.yMax+10;
			scrollRect.height	= 12;
			m_nGridCellSize		= (int)GUI.HorizontalScrollbar(scrollRect, m_nGridCellSize, 1, 3, m_nMaxGridCellSize+1);
			if (GUI.changed)
				UnityEditor.EditorPrefs.SetInt("FXMakerHierarchy.m_nGridCellSize", m_nGridCellSize);
		} else {
			nTopHeight	-= nTopBox2Height + 20;
		}

		// Draw line
		Rect lineRect		= baseRect;
		lineRect.y			= nTopHeight+16;
		lineRect.height		= 3;
		NgGUIDraw.DrawHorizontalLine(new Vector2(lineRect.x, lineRect.y), (int)lineRect.width, new Color(0.1f, 0.1f, 0.1f, 0.7f), 2, false);

		// Draw Hierarchy ------------------------------------------------------
		m_HierarchyRect				= FXMakerLayout.GetChildVerticalRect(FXMakerLayout.GetEffectHierarchyRect(), nTopHeight, 1, 0, 1);
		m_HierarchyScrollPos		= GUI.BeginScrollView(m_HierarchyRect, m_HierarchyScrollPos, new Rect(0, 0, m_nMaxGridColumn*m_nGridCellSize, m_nMaxGridRow*m_nGridCellSize+30));
		m_ArrowMovePrevObj			= null;
		m_ArrowMovePrevCom			= null;
		m_bUpdateActiveComponent	= false;
		m_AbsScrollPos				= new Vector2(popupRect.x + m_HierarchyRect.x - m_HierarchyScrollPos.x, popupRect.y + m_HierarchyRect.y - m_HierarchyScrollPos.y);

		if (m_CurrentEffectObject != null)
		{
			int nDrawRow = DrawHierarchy(0, 3, m_CurrentEffectObject.transform, null, null);

			if (m_bUpdateActiveComponent == false)	// �˻��ɼǿ� ���� ������
				SetActiveComponent(m_SelectedGameObject, null, true);

			// Hierarchy���� ����
			if (FXMakerMain.inst.GetFocusUnityWindow() != FXMakerMain.UNITYWINDOW.GameView && (0 < m_nLastDrawRow && nDrawRow != m_nLastDrawRow))
			{
				FXMakerMain.inst.CreateCurrentInstanceEffect(true);
				m_nLastDrawRow = nDrawRow;
			}
		}
 		GUI.EndScrollView();

		// init DropObject
		if (m_DragObject != null && Input.GetMouseButton(0) == false)
			ClearDragObject();
		if (m_DragObject != null)
		{
			Rect rect = GetDragButtonRect();
			GUI.color = FXMakerLayout.m_ColorDropFocused;
			GUI.Box(rect, m_DragObjectName, m_styleBox);
		}

 		FXMakerMain.inst.SaveTooltip();
	}

	void ResetScrollView()
	{
		m_nMaxGridColumn	= 0;
		m_nMaxGridRow		= 0;
	}

	// ==========================================================================================================
	int DrawHierarchy(int nColumn, int nRow, Transform drawTrans, GameObject parentObj, Object parentCom)
	{
		if (drawTrans == null)
			return nRow;

		int			nOldRow;
		string		caption;
		Color		backColor			= GUI.color;
		int			nGameObjectRow		= nRow;
		int			nComponentCount		= 0;
		Component[]	coms;

		// Update Editor Info
		NcDelayActive	NcDelayActive = drawTrans.GetComponent<NcDelayActive>();
		if (NcDelayActive != null)
			NcDelayActive.GetParentDelayTime(false);

		// current caption -----------------------------------------------------------------------------
		if (nColumn != 0)
			DrawLinkHLine(nColumn-m_nGridButtonHeightCount/2, nRow+m_nGridButtonHeightCount/2, m_nGridButtonHeightCount/2);
		if (nColumn == 0)
			caption = drawTrans.name.Replace("(Original)", "");
		else caption = drawTrans.name;

		// show GameObject Box
		if (DrawHierarchyBox(OBJECT_TYPE.OBJECT_GAMEOBJECT, parentObj, parentCom, drawTrans, nColumn, nRow, caption, drawTrans.gameObject, 0, false))
			CheckHierarchyRightPopup(OBJECT_TYPE.OBJECT_GAMEOBJECT, drawTrans, drawTrans.gameObject, 0);

		nRow += m_nGridButtonHeightCount;
		GUI.color = backColor;

		// sort particleSystem
		if (drawTrans.GetComponent<ParticleSystem>() != null)
		{
			coms = drawTrans.GetComponents<Component>();
			for (int n = 0; n < coms.Length; n++)
			{
				if (coms[n] == null || coms[n] is Transform)
					continue;
				if ((coms[n] is ParticleSystem))
				{
					break;
				} else {
					// move to last
					NgSerialized.CloneComponent(coms[n], drawTrans.gameObject, true);
				}
			}
		}

		// Transform Component -------------------------------------------------------------------------
		if (m_ShowComponentOptions[(int)SHOWCOMPONENT_TYPE.TRANSFORM])
		{
			coms = drawTrans.GetComponents<Component>();
			for (int n = 0; n < coms.Length; n++)
			{
				if (coms[n] == null)
					continue;
				caption = coms[n].ToString();

				if (caption.Contains("(UnityEngine.Transform)") == false)
					continue;

				// UnityEngine normal color
				GUI.color = FXMakerLayout.m_ColorButtonUnityEngine;

				// trim
				caption = TrimObjectName(drawTrans.gameObject, caption);

				// show Box
				if (DrawHierarchyBox(OBJECT_TYPE.OBJECT_TRANSFORM, parentObj, parentCom, drawTrans, nColumn, nRow, caption, coms[n], 0, true))
					CheckHierarchyRightPopup(OBJECT_TYPE.OBJECT_TRANSFORM, drawTrans, coms[n], 0);

				nRow += m_nGridButtonHeightCount;
				nComponentCount++;
				GUI.color = backColor;
			}
		}

		// Fxm Effect Component -----------------------------------------------------------------
		if (m_ShowComponentOptions[(int)SHOWCOMPONENT_TYPE.EASYEFFECT])
		{
			coms = drawTrans.GetComponents<Component>();
			for (int n = 0; n < coms.Length; n++)
			{
				if (coms[n] == null) continue;
				caption = coms[n].ToString();

				if (caption.Contains("(Nc") == false || caption.Contains("(NcDontActive"))
					continue;

				// trim
				caption = TrimObjectName(drawTrans.gameObject, caption);

				// show Box
				if (DrawHierarchyBox(OBJECT_TYPE.OBJECT_EASYEFFECT, parentObj, parentCom, drawTrans, nColumn, nRow, caption, coms[n], 0, true))
					CheckHierarchyRightPopup(OBJECT_TYPE.OBJECT_EASYEFFECT, drawTrans, coms[n], 0);

				nRow += m_nGridButtonHeightCount;
				nComponentCount++;
				GUI.color = backColor;
			}
		}

		// UnityEngine Component ---------------------------------------------------------------------
		if (m_ShowComponentOptions[(int)SHOWCOMPONENT_TYPE.UNITYENGINE])
		{
			coms = drawTrans.GetComponents<Component>();
			for (int n = 0; n < coms.Length; n++)
			{
				if (coms[n] == null) continue;
				caption = coms[n].ToString();

				if (caption.Contains("(UnityEngine") == false || caption.Contains("(UnityEngine.Transform)") == true)
					continue;

				// UnityEngine normal color
				GUI.color = FXMakerLayout.m_ColorButtonUnityEngine;

				// trim
				caption = TrimObjectName(drawTrans.gameObject, caption);

				// show Box
				if (DrawHierarchyBox(OBJECT_TYPE.OBJECT_UNITYENGINE, parentObj, parentCom, drawTrans, nColumn, nRow, caption, coms[n], 0, true))
					CheckHierarchyRightPopup(OBJECT_TYPE.OBJECT_UNITYENGINE, drawTrans, coms[n], 0);

				nRow += m_nGridButtonHeightCount;
				nComponentCount++;
				GUI.color = backColor;
			}
		}

		// Material --------------------------------------------------------------------------
		if (m_ShowComponentOptions[(int)SHOWCOMPONENT_TYPE.MATERIAL] == true && drawTrans.GetComponent<Renderer>() != null && drawTrans.GetComponent<Renderer>().sharedMaterials != null)
		{
			for (int n = 0; n < drawTrans.GetComponent<Renderer>().sharedMaterials.Length; n++)
			{
				Material	mat = drawTrans.GetComponent<Renderer>().sharedMaterials[n];

// 				gameObject.animation.clip

				// show Box
				if (DrawHierarchyBox(OBJECT_TYPE.OBJECT_MATERIAL, parentObj, parentCom, drawTrans, nColumn, nRow, (mat==null ? "Missing Material" : (NgMaterial.IsMaskTexture(mat) ? "<M>" + mat.name: mat.name)), mat, n, false))
					CheckHierarchyRightPopup(OBJECT_TYPE.OBJECT_MATERIAL, drawTrans, mat, n);

				nRow += m_nGridButtonHeightCount;
				nComponentCount++;
				GUI.color = backColor;
			}
		}

		// Animation --------------------------------------------------------------------------
		if (m_ShowComponentOptions[(int)SHOWCOMPONENT_TYPE.ANIMATION] == true && drawTrans.GetComponent<Animation>() != null && drawTrans.GetComponent<Animation>().clip != null)
		{
			for (int n = 0; n < drawTrans.GetComponent<Animation>().GetComponent<Animation>().GetClipCount(); n++)
			{
				AnimationState	aniState = NgAnimation.GetAnimationByIndex(drawTrans.GetComponent<Animation>(), n);
				if (aniState == null)
					continue;
				AnimationClip	aniClip = aniState.clip;

				// show Box
				if (DrawHierarchyBox(OBJECT_TYPE.OBJECT_ANICLIP, parentObj, parentCom, drawTrans, nColumn, nRow, aniClip.name, aniClip, n, false))
					CheckHierarchyRightPopup(OBJECT_TYPE.OBJECT_ANICLIP, drawTrans, aniClip, n);

				nRow += m_nGridButtonHeightCount;
				nComponentCount++;
				GUI.color = backColor;
			}
		}

		// Other Component ---------------------------------------------------------------------
		if (m_ShowComponentOptions[(int)SHOWCOMPONENT_TYPE.OTHER])
		{
			coms = drawTrans.GetComponents<Component>();
			for (int n = 0; n < coms.Length; n++)
			{
				if (coms[n] == null) continue;
				caption = coms[n].ToString();

				if (caption.Contains("(Nc") == true)
					continue;
				if (caption.Contains("(UnityEngine") == true)
					continue;

				// color
				GUI.color = FXMakerLayout.m_ColorButtonUnityEngine;

				// trim
				caption = TrimObjectName(drawTrans.gameObject, caption);

				// show Box
				if (DrawHierarchyBox(OBJECT_TYPE.OBJECT_OTHER, parentObj, parentCom, drawTrans, nColumn, nRow, caption, coms[n], 0, true))
					CheckHierarchyRightPopup(OBJECT_TYPE.OBJECT_OTHER, drawTrans, coms[n], 0);

				nRow += m_nGridButtonHeightCount;
				nComponentCount++;
				GUI.color = backColor;
			}
		}

		// drawLine all component
		if (drawTrans.gameObject == m_SelectedGameObject)
		{
			Rect boxRect = FXMakerLayout.GetOffsetRect(GetGridButtonRect(nColumn, nGameObjectRow, nComponentCount+1), 5);
			boxRect = FXMakerLayout.GetOffsetRect(boxRect, 3, 0, -3, 0);
			NgGUIDraw.DrawBox(boxRect, FXMakerLayout.m_ColorButtonActive, 1, false);

			// Update ScrollPos
			if (m_bUpdateHierarchyScrollPos)
			{
				float fTop		= boxRect.y - m_HierarchyScrollPos.y;
				float fBottom	= boxRect.y+boxRect.height - m_HierarchyScrollPos.y;
				if (m_HierarchyRect.height < fBottom)
					m_HierarchyScrollPos.y += fBottom - m_HierarchyRect.height;
				if (fTop < 0)
					m_HierarchyScrollPos.y += fTop;
				m_bUpdateHierarchyScrollPos = false;
			}
		}
		if (m_DragObject != null && drawTrans == m_HoverComponentTrans)
		{
			Rect boxRect = FXMakerLayout.GetOffsetRect(GetGridButtonRect(nColumn, nGameObjectRow, nComponentCount+1), 5);
			boxRect = FXMakerLayout.GetOffsetRect(boxRect, 3, 0, -3, 0);
			NgGUIDraw.DrawBox(boxRect, FXMakerLayout.m_ColorDropFocused, 1, false);
		}

		// Child trans ------------------------------------------------------------------------
		Object lastCom = m_ArrowMovePrevCom;
		nOldRow	 = nRow;
		nColumn += m_nGridButtonHeightCount;
		nRow	+= m_nGridButtonHeightCount + 1;
		for (int n = 0; n < drawTrans.childCount; n++)
		{
			// Draw vertical line
			DrawLinkVLine(nColumn-m_nGridButtonHeightCount/2, nOldRow, (nRow+m_nGridButtonHeightCount/2)-nOldRow);
			nOldRow = (nRow+m_nGridButtonHeightCount/2);

			nRow = DrawHierarchy(nColumn, nRow, drawTrans.GetChild(n), drawTrans.gameObject, lastCom);
		}

		// sve scrollView size
		m_nMaxGridColumn	= Mathf.Max(nColumn, m_nMaxGridColumn);
		m_nMaxGridRow		= Mathf.Max(nRow, m_nMaxGridRow);

		return nRow;
	}

	// Popup CheckHierarchyRightPopup
	void CheckHierarchyRightPopup(OBJECT_TYPE objType, Transform currentTrans, Object selObj, int nSelIndex)
	{
		if (Input.GetMouseButtonDown(1) == false) return;
		FxmPopupManager.inst.ShowHierarchyRightPopup(objType, currentTrans, selObj, nSelIndex);
	}

	// AddOn Button
	void DrawAddOnButton(OBJECT_TYPE objType, Rect objectRect, Transform currentTrans, Object selObj, int selIndex)
	{
		// add child prefab
		GUI.color	= new Color(1, 1, 1, 0.7f);
		Rect addRect = objectRect;
		addRect.x += addRect.width + 1;
		addRect.width = addRect.height;
		addRect.height	-= 1;
		addRect = FXMakerLayout.GetOffsetRect(addRect, -1);

		string scriptFilename = null;

		switch (objType)
		{
			case OBJECT_TYPE.OBJECT_GAMEOBJECT:
				{
					// Component
					if (GUI.Button(addRect, FXMakerTooltip.GetHcEffectHierarchy("Popup_GameObject"), m_styleButtonAddPrefab))
						scriptFilename	= "FxmPopup_GameObject";
					else {
						// Prefab
						addRect.x += addRect.width + 4;
						if (GUI.Button(addRect, FXMakerTooltip.GetHcEffectHierarchy("Popup_AddPrefab"), m_styleButtonAddPrefab))
							FxmPopupManager.inst.ShowAddPrefabPopup(currentTrans);
						return;
					}
					break;
				}
			case OBJECT_TYPE.OBJECT_TRANSFORM:
				{
					if (GUI.Button(addRect, FXMakerTooltip.GetHcEffectHierarchy("Popup_Transform"), m_styleButtonAddPrefab))
						scriptFilename = "FxmPopup_Transform";
					break;
				}
			case OBJECT_TYPE.OBJECT_MATERIAL:
				{
					if (GUI.Button(addRect, FXMakerTooltip.GetHcEffectHierarchy("Popup_Material"), m_styleButtonAddPrefab))
						FxmPopupManager.inst.ShowMaterialPopup(currentTrans, selIndex);

// 					addRect.x += addRect.width + 4;
// 					if (GUI.Button(addRect, NgTooltip.GetHcEffectHierarchy("AddOn_RemoveMaterial"), m_styleButtonAddPrefab))
// 						NgAsset.RemoveSharedMaterial(selObj as Renderer, selIndex);
					return;
				}
			case OBJECT_TYPE.OBJECT_UNITYENGINE:
				{
					if (selObj is MeshFilter)
						if (GUI.Button(addRect, FXMakerTooltip.GetHcEffectHierarchy("Popup_Mesh"), m_styleButtonAddPrefab))
							FxmPopupManager.inst.ShowSelectMeshPopup(selObj as MeshFilter);

					if ((selObj is ParticleEmitter || selObj is ParticleSystem || selObj is ParticleSystemRenderer))
						if (IsMeshFromMeshList(selObj as Component))
							if (GUI.Button(addRect, FXMakerTooltip.GetHcEffectHierarchy("Popup_Mesh"), m_styleButtonAddPrefab))
								FxmPopupManager.inst.ShowSelectMeshPopup(selObj as Component);

					if (selObj is Renderer && (selObj is ParticleSystemRenderer) == false)
						if (GUI.Button(addRect, FXMakerTooltip.GetHcEffectHierarchy("AddOn_AddMaterial"), m_styleButtonAddPrefab))
							NgMaterial.AddSharedMaterial(selObj as Renderer);
					return;
				}
		}

		if (scriptFilename != null)
			FxmPopupManager.inst.ShowHierarchyObjectPopup(scriptFilename, selObj);
	}

	// enable/disable
	void DrawEnableButton(OBJECT_TYPE objType, bool bScriptComponent, Rect objectRect, Transform currentTrans, Object selObj)
	{
		bool bGameObject = objType == OBJECT_TYPE.OBJECT_GAMEOBJECT;

		if (bGameObject || (bScriptComponent && 0 <= EditorUtility.GetObjectEnabled(selObj)))
		{
			bool	bEnable = false;
			if (bGameObject)
			{
				bEnable	= (currentTrans.gameObject.GetComponent<NcDontActive>() == null);
			} else
			if (bScriptComponent)
			{
				bEnable = (0 < EditorUtility.GetObjectEnabled(selObj));
			}

			GUI.color	= new Color(1, 1, 1, 0.7f);
			Rect disRect = objectRect;
			disRect.width = disRect.height;
			disRect = FXMakerLayout.GetOffsetRect(disRect, -1);
			bool ret = (GUI.Toggle(disRect, bEnable, FXMakerTooltip.GetHcEffectHierarchy("Enable"), m_styleToggle));
			if (ret != bEnable)
			{
				if (bGameObject)
					SetEnableGameObject(currentTrans.gameObject, ret);
				else {
//					((Behaviour)selObj).enabled = ret;
					EditorUtility.SetObjectEnabled(selObj, ret);
					OnEnableComponent(selObj as Component, ret);
				}
				FXMakerMain.inst.CreateCurrentInstanceEffect(true);
			}
			if (bGameObject && ret == false)
				GUI.DrawTexture(disRect, m_DisableTexture);
		}
	}

	void DrawWarringIcon(int nColumn, int nRow, string msg)
	{
		Rect	rect = GetGridButtonRect(0, nRow, 1);
		rect.x		= -3;
		rect.width	= rect.height-2;
		rect.height	= rect.height+2;
		GUI.DrawTexture(rect, m_WarringTexture);
		Rect	absRect	= new Rect(m_AbsScrollPos.x, m_AbsScrollPos.y + rect.y, rect.width, rect.height);
		if (absRect.Contains(NgLayout.GetGUIMousePosition()))
			FXMakerMain.inst.SavePriorityTooltip(FXMakerTooltip.Tooltip(msg));
	}

	void SetDragObject(Object dragObj, int dragObjIndex, Transform currentTrans, string name)
	{
		m_DragObject		= dragObj;
		m_nDragObjectIndex	= dragObjIndex;
		m_DragObjectTrans	= currentTrans;
		m_DragObjectName	= name;
	}

	void ClearDragObject()
	{
		m_DragObject		= null;
		m_nDragObjectIndex	= 0;
		m_DragObjectTrans	= null;
		m_DragObjectName	= "";
	}

	void DropObject(Transform currentTrans, Object currentObj, int currentIndex)
	{
		Object tarObj;
		if (m_DragObjectTrans == currentTrans)	// ���� �ٲ�
		{
			if (currentIndex == m_nDragObjectIndex)
				return;
			tarObj = NgMaterial.MoveSharedMaterial(currentTrans.GetComponent<Renderer>(), m_nDragObjectIndex, currentIndex);
		} else {	// ���̱�
			tarObj = FXMakerClipboard.PasteObject(m_DragObject, false, currentTrans, currentObj, currentIndex, false, true, false);
		}

		FXMakerMain.inst.CreateCurrentInstanceEffect(true);
		if (tarObj is GameObject)
			 SetActiveComponent(tarObj as GameObject, tarObj, false);
		else SetActiveComponent(currentTrans.gameObject, tarObj, true);

		ClearDragObject();
		return;
	}

	void CheckMissing(OBJECT_TYPE objType, Transform currentTrans, Object selObj, int nColumn, int nRow, int nSelIndex)
	{
		switch (objType)
		{
			case OBJECT_TYPE.OBJECT_GAMEOBJECT:
				{
					Component[]	coms = currentTrans.GetComponents<Component>();
					for (int n = 0; n < coms.Length; n++)
					{
						if (coms[n] == null)
						{
//							if (FXMakerMain.inst.IsFrameCreateInstance())
// 							Debug.LogWarning("Missage script !!! - GameObject : " + drawTrans.name);
							DrawWarringIcon(nColumn, nRow, "Missing script !!! - Index : " + n);
							continue;
						}
					}
					break;
				}
			case OBJECT_TYPE.OBJECT_TRANSFORM:
				{
					if (currentTrans.GetComponent<ParticleEmitter>() != null || currentTrans.GetComponent<ParticleSystem>() != null)
					{
						if (0.02f < Mathf.Abs(Vector3.Distance(currentTrans.lossyScale, Vector3.one)) && currentTrans.GetComponent<NcParticleSystem>() == null)
							DrawWarringIcon(nColumn, nRow, FXMakerTooltip.GetHsScriptMessage("SCRIPT_NEED_SCALEPARTICLE"));
					}
					break;
				}
			case OBJECT_TYPE.OBJECT_EASYEFFECT:
				{
					NcEffectBehaviour	com		= selObj as NcEffectBehaviour;
					if (com != null)
					{
						string				errKey	= com.CheckProperty();
						if (errKey != "")
							DrawWarringIcon(nColumn, nRow, FXMakerTooltip.GetHsScriptMessage(errKey));
					}
					break;
				}
			case OBJECT_TYPE.OBJECT_UNITYENGINE:
			case OBJECT_TYPE.OBJECT_OTHER:
				{
					Component com = selObj as Component;
					if (IsMeshFromMeshList(com))
						if (GetMeshFromMeshList(com) == false)
							DrawWarringIcon(nColumn, nRow, "Missing Mesh !!!");

					NcEffectBehaviour	ncCom = selObj as NcEffectBehaviour;
					if (ncCom != null)
					{
						string	errKey	= ncCom.CheckProperty();
						if (errKey != "")
							DrawWarringIcon(nColumn, nRow, FXMakerTooltip.GetHsScriptMessage(errKey));
					}
					break;
				}
			case OBJECT_TYPE.OBJECT_MATERIAL:
				{
					Material	mat = selObj as Material;
					if (mat == null)
						DrawWarringIcon(nColumn, nRow, "Missing Material !!! - Index : " + nSelIndex);
					else {
						if (mat.shader == null || mat.shader.name == "")
							DrawWarringIcon(nColumn, nRow, "Missing Shader !!! - Material : " + mat.name);
						if (NgMaterial.GetTexture(mat, false) == null)
							DrawWarringIcon(nColumn, nRow, "Missing Texture !!! - Material : " + mat.name);
						if (NgMaterial.IsMaskTexture(mat) && NgMaterial.GetTexture(mat, true) == null)
							DrawWarringIcon(nColumn, nRow, "Missing MaskTexture !!! - Material : " + mat.name);
					}
					break;
				}
			default: break;
		}
	}

	// show Hierarchy Node
	bool DrawHierarchyBox(OBJECT_TYPE objType, GameObject parentObj, Object parentCom, Transform currentTrans, int nColumn, int nRow, string caption, Object selObj, int selIndex, bool bScriptComponent)
	{
		bool	bClick		= false;
		Rect	boxRect		= GetGridButtonRect(nColumn, nRow, 1);
		bool	bGameObject = objType == OBJECT_TYPE.OBJECT_GAMEOBJECT;
		bool	bVisible	= true;

		// Crop
		if (m_HierarchyScrollPos.y+m_HierarchyRect.height < boxRect.yMin)
			bVisible = false;
		if (boxRect.yMax < m_HierarchyScrollPos.y)
			bVisible = false;

		// Check Missing
 		CheckMissing(objType, currentTrans, selObj, nColumn, nRow, selIndex);

		// Speed ----------------------------------------------------------------------------
		if (bVisible && objType == OBJECT_TYPE.OBJECT_GAMEOBJECT && selObj == m_SelectedGameObject)
		{
			string	speedName;
 			bool	bEnable		= true;
			Rect	speedRect	= GetGridButtonRect(nColumn, nRow-(m_nGridButtonHeightCount+1), 1);
			Rect	sliderRect	= FXMakerLayout.GetOffsetRect(speedRect, 1, 4, -1, 0);
			Rect	buttonRect	= FXMakerLayout.GetOffsetRect(speedRect, speedRect.width+2, 1, speedRect.height*2+5, 3);
			string	warringStr	= "";

			if ((0 < currentTrans.GetComponentsInChildren<Animation>(true).Length || 0 < m_SelectedGameObject.GetComponentsInChildren<ParticleEmitter>(true).Length))
				warringStr += FXMakerTooltip.GetHsScriptMessage("SCRIPT_WARRING_SCALESPEED");
			ParticleSystem[] pss = currentTrans.GetComponentsInChildren<ParticleSystem>(true);
			if (0 < pss.Length)
			{
				foreach (ParticleSystem ps in pss)
				{
					if (ps.GetComponent<NcParticleSystem>() == null)
					{
						warringStr += FXMakerTooltip.GetHsScriptMessage("SCRIPT_WARRING_SCALESPEED_SHURIKEN");
						break;
					}
				}
			}
			if (warringStr != "")
				DrawWarringIcon(nColumn, nRow-m_nGridButtonHeightCount-1, warringStr);

			NgLayout.GUIEnableBackup(bEnable);
			m_fScriptSpeed		= GUI.HorizontalSlider(sliderRect, m_fScriptSpeed, -1.0f, +1.0f);
			NgLayout.GUIEnableRestore();

			if (0 <= m_fScriptSpeed)
				 speedName = (m_fScriptSpeed+1).ToString("0.00") + "x";
			else speedName = (m_fScriptSpeed/2.0f+1).ToString("0.00") + "x";
			NgLayout.GUIButton(buttonRect, new GUIContent(speedName, FXMakerTooltip.GetHcEffectHierarchy("Speed").tooltip), false);
		}

		// check drop target hover
		bool bdropTargets		= (m_DragObject != null && currentTrans == m_HoverComponentTrans && (selObj != null && selObj.GetType() == m_DragObject.GetType()));
		bool bdropTargetHover	= (m_DragObject != null && currentTrans == m_HoverComponentTrans && (selObj != null && selObj == m_HoverComponent && selObj.GetType() == m_DragObject.GetType()));

		// click
//		Rect	absBarRect	= new Rect(m_AbsScrollPos.x + boxRect.x, m_AbsScrollPos.y + boxRect.y, boxRect.width, boxRect.height);
		Rect	absBarRect	= new Rect(m_AbsScrollPos.x, m_AbsScrollPos.y + boxRect.y, m_HierarchyRect.width, boxRect.height);
		float	absStartPos	= FXMakerLayout.GetEffectHierarchyRect().y + m_HierarchyRect.y;
		Vector2	mousePos	= NgLayout.GetGUIMousePosition();

		// hover
//		if (absStartPos < absBarRect.y && absBarRect.Contains(mousePos) && GUI.enabled && (FxmPopupManager.inst.IsContainsHierarchyPopup(mousePos) == false))
		if (absStartPos < absBarRect.y && absBarRect.Contains(mousePos) && GUI.enabled && (FxmPopupManager.inst.IsGUIMousePosition() == false))
		{
			SetHoverComponent(currentTrans, selObj);
			if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
			{
				FxmPopupManager.inst.HideHierarchyPopup(false);
				SetActiveComponent(currentTrans.gameObject, selObj, false);
				bClick = true;
			}
//			FxmPopupManager.inst.SetScriptTooltip(NgTooltip.GetHcPopup_EffectScript(caption).tooltip);
		}

		// arrow move
		CheckInputArrow(parentObj, parentCom, currentTrans.gameObject, selObj);

		// change color
		ChangeActiveColor(selObj, (!bGameObject && !bScriptComponent), bdropTargets, bClick);
          
		// Drag and Drop
		if (absStartPos < absBarRect.y && absBarRect.Contains(mousePos) && GUI.enabled && (FxmPopupManager.inst.IsContainsHierarchyPopup(mousePos) == false))	// bUnityEngine == false && 
		{
			if (m_HoverComponent == selObj && Input.GetMouseButtonDown(0))
				SetDragObject(selObj, selIndex, currentTrans, caption);
			if (m_DragObject != null && m_DragObject != selObj && m_HoverComponent == selObj && Input.GetMouseButtonUp(0))
			{
				DropObject(currentTrans, selObj, selIndex);
				return false;
			}
		}

	 	// show Box
		if (bVisible)
		{
			GUIStyle	style;
			if (bdropTargets)
			{
				if (bGameObject)
					 style = ((bdropTargetHover) ? m_styleButtonActive : m_styleButton);
				else style = ((bdropTargetHover) ? m_styleBoxActive : m_styleButton);
			} else {
				if (bGameObject)
					 style = ((selObj == m_ActiveComponent) ? m_styleButtonActive : m_styleButton);
				else style = ((selObj == m_ActiveComponent) ? m_styleBoxActive : m_styleBox);
			}
			if (objType == OBJECT_TYPE.OBJECT_MATERIAL || objType == OBJECT_TYPE.OBJECT_ANICLIP)
				FXMakerMain.inst.ToggleGlobalLangSkin(true);
//			GUI.Box(boxRect, new GUIContent(caption, FXMakerMain.HoverCommand_Hierarchy(selObj)), style);
			GUI.Box(boxRect, FXMakerTooltip.GetHcEffectHierarchy_Box(caption, bGameObject, bScriptComponent), style);
			if (bGameObject || objType == OBJECT_TYPE.OBJECT_MATERIAL || objType == OBJECT_TYPE.OBJECT_ANICLIP)
			if (objType == OBJECT_TYPE.OBJECT_MATERIAL || objType == OBJECT_TYPE.OBJECT_ANICLIP)
				FXMakerMain.inst.ToggleGlobalLangSkin(false);

			// enable/disable
			DrawEnableButton(objType, bScriptComponent, boxRect, currentTrans, selObj);

			// AddOn Button
			DrawAddOnButton(objType, boxRect, currentTrans, selObj, selIndex);
		}

		return bClick;
	}

	void CheckInputArrow(GameObject parentObj, Object parentCom, GameObject currentObj, Object currentCom)
	{
		GameObject	prevObj = m_ArrowMovePrevObj;
		Object		prevCom = m_ArrowMovePrevCom;
		m_ArrowMovePrevObj	= currentObj;
		m_ArrowMovePrevCom	= currentCom;

		KeyCode	keyLastInputArrow	= FXMakerMain.inst.GetFocusInputKey(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.EFFECT_HIERARCHY));

		if (keyLastInputArrow == 0 || m_ActiveComponent == null)
			return;

		if (keyLastInputArrow == KeyCode.RightArrow && currentCom != null && m_SelectedGameObject == parentObj)
		{
			SetActiveComponent(currentObj, currentCom, true);
			FXMakerMain.inst.SetFocusInputKey(0);
			return;
		}
		if (keyLastInputArrow == KeyCode.LeftArrow && parentCom != null && m_SelectedGameObject == currentObj)
		{
			SetActiveComponent(parentObj, parentCom, true);
			FXMakerMain.inst.SetFocusInputKey(0);
			return;
		}

		if ((keyLastInputArrow == KeyCode.UpArrow || keyLastInputArrow == KeyCode.LeftArrow) && prevCom != null && (m_SelectedGameObject == currentObj && m_ActiveComponent == currentCom))
		{
			SetActiveComponent(prevObj, prevCom, true);
			FXMakerMain.inst.SetFocusInputKey(0);
		} else 
		if ((keyLastInputArrow == KeyCode.DownArrow || keyLastInputArrow == KeyCode.RightArrow) && currentCom != null && (m_SelectedGameObject == prevObj && m_ActiveComponent == prevCom))
		{
			SetActiveComponent(currentObj, currentCom, true);
			FXMakerMain.inst.SetFocusInputKey(0);
		}

		return;
	}

	string TrimObjectName(GameObject currentObj, string caption)
	{
		// UnityEngine
		caption = caption.Replace("(UnityEngine.", "(");
		// trim GameObjectName and ()
		caption = caption.Replace(currentObj.name + " (", "");
		caption = caption.Trim();
		caption = caption.Trim('(');
		caption = caption.Trim(')');
		return caption;
	}

	void ChangeActiveColor(Object obj, bool bMaterial, bool bdropTarget, bool bActive)
	{
		// ǥ�õǰ� �ִ� Ȯ��..
		if (m_ActiveComponent == obj)
			m_bUpdateActiveComponent = true;

		if (GUI.enabled == false)
		{
			if (bMaterial)
				GUI.color = FXMakerLayout.m_ColorButtonMatNormal;
			return;
		}

		// change color
		if (m_DragObject == null && m_HoverComponent == obj)
		{
			if (bActive)
				GUI.color = FXMakerLayout.m_ColorButtonActive;
			else GUI.color = FXMakerLayout.m_ColorButtonHover;
		} else {
			if (bdropTarget)
			{
				GUI.color = FXMakerLayout.m_ColorButtonHover;
			} else {
				if (obj == m_ActiveComponent)
					GUI.color = FXMakerLayout.m_ColorButtonActive;
				else {
					if (bMaterial)
						GUI.color = FXMakerLayout.m_ColorButtonMatNormal;
				}
			}
		}
	}

	Rect GetDragButtonRect()
	{
		Rect	popupRect = FXMakerLayout.GetEffectHierarchyRect();
		int		nDefaultGridCellSize	= 8;
		float	x = Input.mousePosition.x - popupRect.x;
		float	y = Screen.height - Input.mousePosition.y - popupRect.y + 15;

		return new Rect(x, y, nDefaultGridCellSize*m_nGridButtonWidthCount, nDefaultGridCellSize*m_nGridButtonHeightCount);
	}

	Rect GetGridButtonRect(int x, int y, int rowCount)
	{
		return new Rect(m_nTreeLeftMagin + x*m_nGridCellSize, y*m_nGridCellSize, m_nGridCellSize*m_nGridButtonWidthCount, m_nGridCellSize*m_nGridButtonHeightCount*rowCount);
	}

	void DrawLinkHLine(int sx, int sy, int len)
	{
		NgGUIDraw.DrawHorizontalLine(new Vector2(m_nTreeLeftMagin + sx*m_nGridCellSize-2, sy*m_nGridCellSize), len*m_nGridCellSize+2, new Color(0.8f, 0.8f, 0.8f), 2, true);
	}

	void DrawLinkVLine(int sx, int sy, int len)
	{
		NgGUIDraw.DrawVerticalLine(new Vector2(m_nTreeLeftMagin + sx*m_nGridCellSize-3, sy*m_nGridCellSize+2), len*m_nGridCellSize-2, new Color(0.8f, 0.8f, 0.8f), 2, true);
	}

	// Control Function -----------------------------------------------------------------
	void ChangeGameObjectSpeed(GameObject target, float fSpeedRate)
	{
		NsEffectManager.AdjustSpeedEditor(target, fSpeedRate);
	}

	public GameObject AddGameObject(GameObject parent, GameObject addObj)
	{
		GameObject	oriGameObj	= FXMakerMain.inst.GetOriginalEffectObject();
		Transform	rootTrans	= oriGameObj.transform.parent;

		oriGameObj.transform.parent = null;
		GameObject newObj = NgObject.CreateGameObject(parent, addObj);
		oriGameObj.transform.parent = rootTrans;

		NcEffectBehaviour.HideNcDelayActive(newObj);
// 		NgObject.SetActiveRecursively(newObj, false);
		newObj.name = newObj.name.Replace("(Clone)", "");
		newObj.name = newObj.name.Replace("(Original)", "");

		OnAddGameObject(newObj);

		return newObj;
	}

	public void UpdateMeshList()
	{
// 		Debug.Log("UpdateMeshList");
		m_MeshList = new Dictionary<Component, bool>();
		if (m_CurrentEffectObject == null)
			return;

		Component[] coms = m_CurrentEffectObject.GetComponentsInChildren<Component>(true);
		foreach (Component com in coms)
		{
// 			Debug.Log(NgSerialized.IsMesh(com, true));
			if (NgSerialized.IsMesh(com, true) == false)
				continue;

			bool bValid = (NgSerialized.GetMesh(com, true) != null);
// 			Debug.Log(bValid);
			m_MeshList.Add(com, bValid);
		}
// 		Debug.Log(m_MeshList.Count);
	}

	bool IsMeshFromMeshList(Component com)
	{
		if (com == null || m_MeshList == null)
			return false;
		return m_MeshList.ContainsKey(com);
	}

	bool GetMeshFromMeshList(Component com)
	{
		if (com == null || m_MeshList == null)
			return false;
		if (m_MeshList.ContainsKey(com) == false)
			return false;
		return m_MeshList[com];
	}


	public void ShowAddObjectRightPopup()
	{
// 		if (FxmPopupManager.inst.IsShowPopupExcludeMenu())
// 			return;

		if (FXMakerEffect.inst.IsReadOnlyFolder() != 0)
			return;
		m_bShowNewMenuPopup	= true;
		FxmPopupManager.inst.ShowMenuPopup("Add Child", new string[]{"Empty", "- Mesh -", "Plane", "- Particle -", "Legacy", "Shuriken"},
				new bool  []{true, false, true, false, true, true});
	}

	void CheckAddObjectRightPopup()
	{
		if (m_bShowNewMenuPopup)
		{
			if (FxmPopupManager.inst.IsShowCurrentMenuPopup() == false)
			{
				switch (FxmPopupManager.inst.GetSelectedIndex())
				{
					case 0:		NewChildGameObject(FXMakerEffect.NEW_TYPE.NEW_EMPTY);		break;
					case 1:		break;
					case 2:		NewChildGameObject(FXMakerEffect.NEW_TYPE.NEW_PLANE);		break;
					case 3:		break;
					case 4:		NewChildGameObject(FXMakerEffect.NEW_TYPE.NEW_LEGACY);		break;
					case 5:		NewChildGameObject(FXMakerEffect.NEW_TYPE.NEW_SHURIKEN);	break;
				}
				m_bShowNewMenuPopup	= false;
			}
		}
	}

	void NewChildGameObject(FXMakerEffect.NEW_TYPE newType)
	{
		GameObject	newPrefab;
		switch (newType)
		{
			case FXMakerEffect.NEW_TYPE.NEW_EMPTY		: newPrefab	= new GameObject("GameObject");					break;
			case FXMakerEffect.NEW_TYPE.NEW_PLANE		: newPrefab	= NgObject.CreateGameObject(FXMakerEffect.inst.m_DefaultPlanePrefab);		break;
			case FXMakerEffect.NEW_TYPE.NEW_LEGACY		: newPrefab	= NgObject.CreateGameObject(FXMakerEffect.inst.m_DefaultLegacyPrefab);		break;
			case FXMakerEffect.NEW_TYPE.NEW_SHURIKEN	: newPrefab	= NgObject.CreateGameObject(FXMakerEffect.inst.m_DefaultShurikenPrefab);	break;
			default :
				{
					NgUtil.LogMessage("NEW_TYPE error !!!");
					return;
				}
		}
		newPrefab.transform.parent = m_SelectedGameObject.transform;
		OnAddGameObject(newPrefab);
		SetActiveComponent(newPrefab, null, true);
		FXMakerMain.inst.CreateCurrentInstanceEffect(true);
	}

	// Event Function -------------------------------------------------------------------
	public void OnActiveHierarchy()
	{
		UpdateMeshList();
	}

	public void OnAddGameObject(GameObject tarCom)
	{
		UpdateMeshList();
	}

	public void OnAddComponent(Component tarCom)
	{
		UpdateMeshList();

		if (0 < EditorUtility.GetObjectEnabled(tarCom))
			OnEnableComponent(tarCom, (EditorUtility.GetObjectEnabled(tarCom) == 1));

	}

	public void OnDeleteComponent(Component tarCom)
	{
		if (0 < EditorUtility.GetObjectEnabled(tarCom))
			OnEnableComponent(tarCom, false);

	}

	public void OnEnableComponent(Component tarCom, bool bChangedEnable)
	{
		if (tarCom is NcParticleSystem)
		{
			if (tarCom.GetComponent<ParticleEmitter>() != null && NgSerialized.IsMeshParticleEmitter(tarCom.GetComponent<ParticleEmitter>()))
			{
				NcParticleSystem ncParticleScale = (tarCom as NcParticleSystem);
				float			fSetMinValue;
				float			fSetMaxValue;
				NgSerialized.GetMeshNormalVelocity(tarCom.GetComponent<ParticleEmitter>(), out fSetMinValue, out fSetMaxValue);

				if (bChangedEnable == true)
				{
					ncParticleScale.m_fLegacyMinMeshNormalVelocity = fSetMinValue;
					ncParticleScale.m_fLegacyMaxMeshNormalVelocity = fSetMaxValue;
				} else {
					if (fSetMinValue != ncParticleScale.m_fLegacyMinMeshNormalVelocity || fSetMaxValue != ncParticleScale.m_fLegacyMaxMeshNormalVelocity)
						NgSerialized.SetMeshNormalVelocity(tarCom.GetComponent<ParticleEmitter>(), ncParticleScale.m_fLegacyMinMeshNormalVelocity, ncParticleScale.m_fLegacyMaxMeshNormalVelocity);
				}
			}
		}
	}

	public void OnCreateInstanceEffect(GameObject instanceObj, bool bReset, GameObject parentInstanceObj)
	{
		if (instanceObj == null)
			return;
		FxmInfoIndexing		tarIndexCom		= instanceObj.GetComponent<FxmInfoIndexing>();
		GameObject			targetOriObj	= tarIndexCom.m_OriginalTrans.gameObject;

		if (bReset)
		{
			ChangeColorscale(GetSelectedGameObject(), targetOriObj, true);
			ChangeBoundsBoxWireframe(GetSelectedGameObject(), targetOriObj, true, true);
		} else
		if (instanceObj != null)
		{
			FxmInfoIndexing		insIndexObj	= instanceObj.GetComponent<FxmInfoIndexing>();
			bool				bSel		= (GetSelectedGameObject() == null ? false : NgObject.FindTransform(GetSelectedGameObject().transform, insIndexObj.m_OriginalTrans));
			bool				bRoot		= (GetSelectedGameObject() == null ? false : GetSelectedGameObject().transform == insIndexObj.m_OriginalTrans);

			// Grayscale
			if (bSel)
				ChangeColorscale(targetOriObj, null, true);
			else ChangeColorscale(null, targetOriObj, true);

			// BoundsBox, Wireframe
// 			Debug.Log(parentIndexCom.m_bSelected);
			if (bSel)
				ChangeBoundsBoxWireframe(targetOriObj, null, bRoot, true);
			else ChangeBoundsBoxWireframe(null, targetOriObj, bRoot, true);
		}
	}
//			GameObject	rootObj	= FXMakerMain.inst.GetInstanceRoot();

	void OnChangeSelectGameObject(GameObject newOriSelectGameObj, GameObject oldOriSelectGameObj)
	{
		if (newOriSelectGameObj == null)
			return;

		// Grayscale
		ChangeColorscale(newOriSelectGameObj, oldOriSelectGameObj, true);

		// BoundsBox, Wireframe
		ChangeBoundsBoxWireframe(newOriSelectGameObj, oldOriSelectGameObj, true, true);
	}

	void ChangeColorscale(GameObject targetOriGameObj, GameObject oldOriSelectGameObj, bool bRecursively)
	{
		// Grayscale
		if (FXMakerGizmo.inst.IsGrayscale())
		{
			// set default
			if (oldOriSelectGameObj != null)
			{
				SetColorscale(oldOriSelectGameObj, true);
				if (bRecursively)
				{
					Transform[]	transs = NgObject.GetChilds(oldOriSelectGameObj.transform);
					foreach (Transform trans in transs)
						SetColorscale(trans.gameObject, true);
				}
			}

			// set colorLayer
			if (targetOriGameObj != null)
			{
				SetColorscale(targetOriGameObj, false);
				if (bRecursively)
				{
					Transform[]	transs = NgObject.GetChilds(targetOriGameObj.transform);
					foreach (Transform trans in transs)
						SetColorscale(trans.gameObject, false);
				}
			}
		}
	}

	void SetColorscale(GameObject tarOriObj, bool bGrayscale)
	{
		int						colorLayer	= (bGrayscale ? 0 : LayerMask.NameToLayer("TransparentFX"));
		List<FxmInfoIndexing>	indexComs	= FxmInfoIndexing.FindInstanceIndexings(tarOriObj.transform, true);

		foreach (FxmInfoIndexing indexCom in indexComs)
		{
			NgObject.ChangeLayerWithChild(indexCom.gameObject, colorLayer);

			// Dup process
			NcDuplicator dupCom = indexCom.gameObject.GetComponent<NcDuplicator>();
			if (dupCom != null)
			{
				GameObject clone = dupCom.GetCloneObject();
				if (clone != null)
					NgObject.ChangeLayerWithChild(clone, colorLayer);
			}
		}
	}

	public void ChangeBoundsBoxWireframe(GameObject targetOriGameObj, GameObject oldOriSelectGameObj, bool bRoot, bool bRecursively)
	{
		// set default
		if (oldOriSelectGameObj != null)
		{
			SetBoundsBoxWireframe(oldOriSelectGameObj, false, false, false, false);

			if (bRecursively)
			{
				Transform[]	transs = NgObject.GetChilds(oldOriSelectGameObj.transform);
				foreach (Transform trans in transs)
					SetBoundsBoxWireframe(trans.gameObject, false, false, false, false);
			}
		}

		if (targetOriGameObj != null)
		{
			bool bShowBounds = (FXMakerGizmo.inst.IsBoundsBox() && FXMakerMain.inst.IsSpriteCapture() == false);
			bool bShowWire	 = (FXMakerGizmo.inst.IsWireframe() && FXMakerMain.inst.IsSpriteCapture() == false);
			bool bActive	 = targetOriGameObj.GetComponent<NcDontActive>() == null;

			if (bActive == false)
				bShowWire = true;

			SetBoundsBoxWireframe(targetOriGameObj, true, bRoot, bShowBounds, bShowWire);

			if (bRecursively)
			{
				Transform[]	transs = NgObject.GetChilds(targetOriGameObj.transform);
				foreach (Transform trans in transs)
					SetBoundsBoxWireframe(trans.gameObject, true, false, bShowBounds, bShowWire);
			}
		}
	}

	void SetBoundsBoxWireframe(GameObject targetOriGameObj, bool bSelected, bool bRoot, bool bShowBounds, bool bShowWire)
	{
		List<FxmInfoIndexing>	currentComs = FxmInfoIndexing.FindInstanceIndexings(targetOriGameObj.transform, true);

		foreach (FxmInfoIndexing indexCom in currentComs)
		{
			indexCom.SetSelected(bSelected, bRoot);
			indexCom.SetBoundsWire(bShowBounds, bShowWire, bRoot);

			// Dup process
			NcDuplicator dupCom = indexCom.gameObject.GetComponent<NcDuplicator>();
			if (dupCom != null)
			{
				GameObject clone = dupCom.GetCloneObject();
				if (clone != null)
				{
					FxmInfoIndexing cloneidx = clone.GetComponent<FxmInfoIndexing>();
					if (cloneidx != null)
					{
						cloneidx.SetSelected(bSelected, bRoot);
						cloneidx.SetBoundsWire(bShowBounds, bShowWire, bRoot);
					}
				}
			}
		}
	}
}

#endif
