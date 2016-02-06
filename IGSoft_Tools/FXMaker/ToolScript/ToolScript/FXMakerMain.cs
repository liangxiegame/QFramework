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

public class FXMakerMain : MonoBehaviour
{
	// -------------------------------------------------------------------------------------------
	public		static FXMakerMain		inst;

	public		GUISkin					m_GuiMainEngSkin;
	public		GUISkin					m_GuiMainGloSkin;
	public		GUISkin					m_GuiMsgSkin;
	public		GUISkin					m_GuiPopupSkin;
	public		GUISkin					m_GuiTooltipSkin;
	protected	GUISkin					m_OldGuiSkin;

	// --------------------------------
	public		Material				m_LineBackMaterial;
	public		Material				m_LineColorMaterial;
	public		GameObject				m_FXMakerSpritePrefab;

	// --------------------------------
	protected	Camera					m_GrayscaleCamera;
	protected	FXMakerMouse			m_FXMakerMouse;
	protected	FXMakerControls			m_FXMakerControls;
	protected	FXMakerQuickMenu		m_FXMakerQuickMenu;
	protected	FXMakerHierarchy		m_FXMakerHierarchy;

	// --------------------------------
	protected	GameObject				m_OriginalEffectPrefab	= null;
	protected	GameObject				m_OriginalEffectObject	= null;
	protected	GameObject				m_InstanceEffectObject	= null;

	// --------------------------------
	protected	bool					m_bLoadingProject;
	protected	int						m_nToolIndex			= 1;
	protected	string					m_Tooltip				= "";
	protected	string					m_EmptyTooltip			= "";
	protected	string					m_PriorityTooltip		= "";
	protected	int						m_nOnGUICallCount		= 0;
	protected	int						m_nUnityFrameCount		= 0;
	protected	int						m_nTempRefreshFrameIndex;
	protected	bool					m_bFrameCreateInstance	= false;

	public		enum UNITYWINDOW		{ None=0, GameView, ProjectWindow, HierarchyWindow, InspectorWindow, ConsoleWindow, ColorPicker, ParticlsSystemWindow, Count };
	protected	bool[]					m_bFocusUnityWindows	= new bool[(int)(UNITYWINDOW.Count)];
	protected	static int				m_nPrevFocusWindow;
	protected	static int				m_nLastFocusWindow;
	protected	bool					m_bPrevPopupWindow;
	protected	bool					m_bLastPopupWindow;
	protected	bool					m_bStartTooMain;

	protected	FxmSpritePopup.CAPTURE_TYPE	m_nSpriteCaptureType;
	protected	int						m_nStartThumbCapture;
	protected	int						m_nSpriteCaptureCount;
	protected	int						m_nSpriteCaptureCurrent;
	protected	float					m_fSpriteCaptureStartTime;
	protected	float					m_fSpriteCaptureInterval;
	protected	bool					m_bApplicationQuit		= false;
	protected	float					m_fOldTimeScale			= 1;

	protected	string							m_ModalMessage			= "";
	protected	FXMakerLayout.MODAL_TYPE		m_ModalType				= FXMakerLayout.MODAL_TYPE.MODAL_NONE;
	protected	FXMakerLayout.MODALRETURN_TYPE	m_nModalMessageValue	= FXMakerLayout.MODALRETURN_TYPE.MODALRETURN_CANCEL;

	// input key
	protected	KeyCode					m_keyLastInput;

	// directroy
	public		string					m_IGSoft_ResourcesDir		= "Assets/IGSoft_Resources";
	public		enum TOOLDIR_TYPE		{DEFAULTSHADERMATERIALS=0, TEMPPREVIEWTEXTURE, BACKGROUNDPREFABS, BACKGROUNDRESOURCES, PROJECTS, SCRIPTS, SCREENSHOTSAVEDIR, SPRITE_TOOL, SPRITE_USER, EXPORTSPLITDIR};
	protected	string					m_DefaultShaderMaterialsDir	= "ToolResources/DefaultShaderMaterials";
	protected	string					m_TempPreviewTextureDir		= "ToolResources/TempPreviewTexture";
	protected	string					m_BackgroundPrefabsDir		= "Backgrounds/BackgroundPrefabs";
	protected	string					m_BackgroundResourcesDir	= "Backgrounds/BackgroundResources";
	protected	string					m_ProjectsDir				= "Projects";
	protected	string					m_ScriptsDir				= "Scripts";
	protected	string					m_ScreenShotSaveDir			= "ScreenShot";
	protected	string					m_SplitTextureDir			= "SplitTexture";

	// -------------------------------------------------------------------------------------------
	public string GetResourceDir(TOOLDIR_TYPE tooldirType)
	{
		string subDir = "";

		switch (tooldirType)
		{
			case TOOLDIR_TYPE.DEFAULTSHADERMATERIALS:	subDir = m_DefaultShaderMaterialsDir;	break;
			case TOOLDIR_TYPE.TEMPPREVIEWTEXTURE:		subDir = m_TempPreviewTextureDir;		break;
			case TOOLDIR_TYPE.BACKGROUNDPREFABS:		subDir = m_BackgroundPrefabsDir;		break;
			case TOOLDIR_TYPE.BACKGROUNDRESOURCES:		subDir = m_BackgroundResourcesDir;		break;
			case TOOLDIR_TYPE.PROJECTS:					subDir = m_ProjectsDir;					break;
			case TOOLDIR_TYPE.SCRIPTS:					subDir = m_ScriptsDir;					break;
			case TOOLDIR_TYPE.SCREENSHOTSAVEDIR:		return m_ScreenShotSaveDir;
			case TOOLDIR_TYPE.SPRITE_TOOL:				subDir = "Projects/[Resources]/[Sprite]/ToolSprite";	break;
			case TOOLDIR_TYPE.SPRITE_USER:				subDir = "Projects/[Resources]/[Sprite]/USerSprite";	break;
			case TOOLDIR_TYPE.EXPORTSPLITDIR:			return m_SplitTextureDir;
// 			default: Debug.LogError("GetResourceDir(TOOLDIR_TYPE tooldirType) - not define !!!");
		}
		return NgFile.CombinePath(m_IGSoft_ResourcesDir, subDir);
	}

	// -------------------------------------------------------------------------------------------
	public Camera GetGrayscaleCamera()
	{
		if (m_GrayscaleCamera == null)
			m_GrayscaleCamera = GetComponentInChildren<Camera>();
		return m_GrayscaleCamera;
	}

	public FXMakerMouse GetFXMakerMouse()
	{
		if (m_FXMakerMouse == null)
			m_FXMakerMouse = GetComponentInChildren<FXMakerMouse>();
		return m_FXMakerMouse;
	}

		
	public FXMakerControls GetFXMakerControls()
	{
		if (m_FXMakerControls == null)
			m_FXMakerControls = GetComponent<FXMakerControls>();
		return m_FXMakerControls;
	}

	public FXMakerQuickMenu GetFXMakerQuickMenu()
	{
		if (m_FXMakerQuickMenu == null)
		{
			m_FXMakerQuickMenu = GetComponent<FXMakerQuickMenu>();
			if (m_FXMakerQuickMenu == null)
				m_FXMakerQuickMenu = gameObject.AddComponent<FXMakerQuickMenu>();
		}
		return m_FXMakerQuickMenu;
	}

	public FXMakerHierarchy GetFXMakerHierarchy()
	{
 		if (m_FXMakerHierarchy == null)
 			m_FXMakerHierarchy = GetComponent<FXMakerHierarchy>();
		return m_FXMakerHierarchy;
	}

	public void SetEmptyTooltip(string tooltip)
	{
		m_EmptyTooltip = tooltip;
	}

	public int GetUnityFrameCount()
	{
		return m_nUnityFrameCount;
	}

	public int GetOnGUICallCount()
	{
		return m_nOnGUICallCount;
	}

	public bool IsGUIMousePosition()
	{
		if (FxmPopupManager.inst.IsShowPopup())
			return FxmPopupManager.inst.IsGUIMousePosition();
		if (FXMakerLayout.GetMenuGizmoRect().Contains(FXMakerLayout.GetGUIMousePosition()))
			return true;
		return (m_nLastFocusWindow != FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.TOP_RIGHT) && 0 < m_nLastFocusWindow);
	}

	public bool IsFrameCreateInstance()
	{
		return m_bFrameCreateInstance;
	}

	public bool IsStartTooMain()
	{
		return m_bStartTooMain;
	}

	public void StartThumbCapture()
	{
		m_nStartThumbCapture = 1;
	}

	public bool IsSpriteCapture()
	{
		return 0 < m_nSpriteCaptureCount;
	}

	public void StartSpriteCapture(FxmSpritePopup.CAPTURE_TYPE captureType, int nCount, float fCaptureInterval)
	{
		m_nSpriteCaptureType			= captureType;
		m_nSpriteCaptureCount			= nCount;
		m_fSpriteCaptureStartTime		= Time.time;
		m_fSpriteCaptureInterval		= fCaptureInterval;
		m_nSpriteCaptureCurrent			= -1;
		CreateCurrentInstanceEffect(true);
	}

	// -------------------------------------------------------------------------------------------
	FXMakerMain()
	{
		inst = this;
	}

	void Awake()
	{
		m_fOldTimeScale	= Time.timeScale;

		NgUtil.LogDevelop("Awake - FXMakerMain");
		if (FXMakerLayout.m_bDevelopPrefs == false)
			m_nToolIndex	= EditorPrefs.GetInt("FXMakerMain.m_nToolIndex", m_nToolIndex);

		m_bFocusUnityWindows[(int)(UNITYWINDOW.GameView)] = true;
		m_bStartTooMain	= true;

		GetFXMakerControls().enabled	= true;
		GetFXMakerQuickMenu().enabled	= true;
		GetFXMakerHierarchy().enabled	= true;
	}

	void OnEnable()
	{
		NgUtil.LogDevelop("OnEnable - FXMakerMain");
// 		CreateCurrentInstanceEffect(true);
	}

	void Start()
	{
		LoadTool("");
		SetActiveTool(m_nToolIndex);
		m_bStartTooMain	= false;

		// start message
		if (FXMakerOption.inst.m_bAutoSaveAppPause == false || FXMakerOption.inst.m_bAutoSaveAppQuit == false)
			FxmPopupManager.inst.SetDelayBottomMessage(FXMakerTooltip.GetHsToolMessage("WARRING_AUTOSAVE", ""));
	}

	public IEnumerator CaptureSpriteImageCoroutine()
	{
		yield return new WaitForEndOfFrame();

		// capture
		FXMakerCapture.CaptureSprite(m_nSpriteCaptureCount, m_nSpriteCaptureCurrent);
		m_nSpriteCaptureCurrent++;
		if (m_fSpriteCaptureStartTime == 0)
			m_fSpriteCaptureStartTime = Time.time;
		else m_fSpriteCaptureStartTime += m_fSpriteCaptureInterval;
		// end
		if (m_nSpriteCaptureCount == m_nSpriteCaptureCurrent)
		{
			int nCapCount = m_nSpriteCaptureCount;
			m_nSpriteCaptureCount = 0;
			FXMakerCapture.EndSpriteCapture(nCapCount);
		} else {
			// recreate
			if (m_nSpriteCaptureType == FxmSpritePopup.CAPTURE_TYPE.RANDOM)
				CreateCurrentInstanceEffect(true);
		}
	}

	void CaptureSpriteImage()
	{
		// Thumb Capture
		if (0 < m_nSpriteCaptureCount)
		{
			if (0 <= m_nSpriteCaptureCurrent)
			{
				if (m_fSpriteCaptureStartTime + m_fSpriteCaptureInterval <= Time.time)
				{
					StartCoroutine(CaptureSpriteImageCoroutine());
				}
			} else m_nSpriteCaptureCurrent++;
		}
	}

	void Update()
	{
		CaptureSpriteImage();

//  		NgSerialized.LogPropertis(transform, true);

//		Debug.Log(NgAssembly.GetPropertyValue(new SerializedObject(transform), "m_LocalEulerAnglesHint.x"));
// 		NgAssembly.SetPropertyValue(new SerializedObject(transform), "m_LocalEulerAnglesHint.x", (float)1000, true);
// 		Debug.Log(NgAssembly.GetPropertyValue(new SerializedObject(transform), "m_LocalEulerAnglesHint.x"));
// 		if (FXMakerHierarchy.inst.GetSelectedGameObject() != null && FXMakerHierarchy.inst.GetSelectedGameObject().renderer != null)
// 			NgAssembly.GetPropertis(FXMakerHierarchy.inst.GetSelectedGameObject().renderer);

		m_nUnityFrameCount++;
		m_nOnGUICallCount	= 0;
		m_bPrevPopupWindow	= m_bLastPopupWindow;
		SetFocusInputKey(0);		// 사용안된 키값 지우기

		// Original Unactive
		UnactiveOriginalObject();

		// main Camera Check
		if (Camera.main == null)
			FxmPopupManager.inst.ShowToolMessage("MainCamera not Found");

		// ShotKey
		if (Input.GetKey(KeyCode.LeftArrow))
			m_keyLastInput = FXMakerLayout.GetVaildInputKey(KeyCode.LeftArrow, Input.GetKeyDown(KeyCode.LeftArrow));
		if (Input.GetKey(KeyCode.RightArrow))
			m_keyLastInput = FXMakerLayout.GetVaildInputKey(KeyCode.RightArrow, Input.GetKeyDown(KeyCode.RightArrow));
		if (Input.GetKey(KeyCode.UpArrow))
			m_keyLastInput = FXMakerLayout.GetVaildInputKey(KeyCode.UpArrow, Input.GetKeyDown(KeyCode.UpArrow));
		if (Input.GetKey(KeyCode.DownArrow))
			m_keyLastInput = FXMakerLayout.GetVaildInputKey(KeyCode.DownArrow, Input.GetKeyDown(KeyCode.DownArrow));
		if (Input.GetKeyUp(KeyCode.Delete))
			m_keyLastInput = KeyCode.Delete;

		// Thumb Capture
		if (0 < m_nStartThumbCapture)
		{
			if (30 < m_nStartThumbCapture)
			{
				if (m_nToolIndex == 0)
				{
					StartCoroutine(FXMakerCapture.EndSaveBackThumbCoroutine());
				} else {
					StartCoroutine(FXMakerCapture.EndSaveEffectThumbCoroutine());
				}
				m_nStartThumbCapture = 0;
			} else {
				m_nStartThumbCapture++;
			}
		}

// 		if (Input.GetKey(KeyCode.LeftArrow))
// 			SendMessage("aa");
	}

	void OnApplicationPause(bool pause)
	{
		Debug.Log("OnApplicationPause --------------------------------");
		FxmPopupManager.inst.HideAllPopup(true);
		if (FXMakerOption.inst.m_bAutoSaveAppPause)
			SaveTool("AutoSave - OnApplicationPause");
	}

	// Sent to all game objects before the application is quit.
	void OnApplicationQuit()
	{
		if (FXMakerOption.inst.m_bResetTimeScaleAppQuit)
			Time.timeScale	= m_fOldTimeScale;
		FxmPopupManager.inst.HideAllPopup(true);
		m_bApplicationQuit = true;
		Debug.Log("OnApplicationQuit --------------------------------");
		if (FXMakerOption.inst.m_bAutoSaveAppQuit)
			SaveTool("AutoSave - OnApplicationQuit");
	}

	public void ToggleGlobalLangSkin(bool bEnable)
	{
		if (bEnable)
		{
			m_OldGuiSkin= GUI.skin;
			GUI.skin	= m_GuiMainGloSkin;
		} else GUI.skin = m_OldGuiSkin;
	}

	// -------------------------------------------------------------------------------------------
	public void OnGUIStart()
	{
		m_bFrameCreateInstance	= false;
		m_nPrevFocusWindow		= m_nLastFocusWindow;
		m_nLastFocusWindow		= 0;
		m_bLastPopupWindow		= false;
		m_nOnGUICallCount++;

		// msg box -------------------------------------------------------------
		GUI.skin = m_GuiMsgSkin;
		FxmPopupManager.inst.OnGUIToolMessage();

		// modal msg box -------------------------------------------------------
		FxmPopupManager.inst.OnGUIModalMessage();

		// Gizmo menu ----------------------------------------------------------
		GUI.skin = m_GuiMainEngSkin;
		FXMakerGizmo fxmGizmo = GetComponentInChildren<FXMakerGizmo>();
		if (fxmGizmo != null && fxmGizmo.enabled)
			fxmGizmo.OnGUIGizmo();

		// modal Fxm Popup ----------------------------------------------------
		GUI.skin = m_GuiPopupSkin;
		FxmPopupManager.inst.OnGUIFolderPopup();

		// Menu Change UI ------------------------------------------------------
		GUI.skin = m_GuiMainEngSkin;
		AutoFocusWindow(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.TOP_LEFT), FXMakerLayout.GetMenuChangeRect(), winMenuChange, "FXMaker " + FXMakerLayout.m_CurrentVersion);

		// Child Window -------------------------------------------------------
		m_FXMakerControls.OnGUIControl();
		m_FXMakerQuickMenu.OnGUIControl();
		// Child Window -------------------------------------------------------
		if (m_nToolIndex == 1 && GetFXMakerHierarchy() != null)
			GetFXMakerHierarchy().OnGUIHierarchy();

		// Auto Save, Auto Load -----------------------------------------------
		CheckFocusedUnityWindow();
	}

	public void OnGUIEnd()
	{
		// tooltip -------------------------------------------------------------
		GUI.skin = m_GuiTooltipSkin;
		ProcessTooltip();
		FxmPopupManager.inst.OnGUIScriptTooltip();

		if (m_nLastFocusWindow == 0)
			GUI.UnfocusWindow();

		// ShotKey --------------------------------------------------------------
		// screen shot - Ctl + shiift + s
		if (Event.current != null && Event.current.control && Event.current.shift && Event.current.keyCode == KeyCode.S)
			FXMakerCapture.CaptureScreenShot();
	}

	// -------------------------------------------------------------------------------------------
	void winMenuChange(int id)
	{
		int nVirticlaCount = 0;

		if ((FXMakerLayout.m_bMinimizeAll || FXMakerLayout.m_bMinimizeTopMenu) == false)
		{
			// change button
			if (GUI.Button(FXMakerLayout.GetChildVerticalRect(FXMakerLayout.GetMenuChangeRect(), 0, 3, 0, 1), FXMakerTooltip.GetHcToolMain("go "+(m_nToolIndex==0 ? "PrefabTool" : "Background"))))
				SetActiveTool(m_nToolIndex == 0 ? 1 : 0);
			// Capture
			if (GUI.Button(FXMakerLayout.GetChildVerticalRect(FXMakerLayout.GetMenuChangeRect(), 0, 3, 1, 1), FXMakerTooltip.GetHcToolMain("FullCapture", FXMakerCapture.GetCaptureScreenShotDir())))
			{
				if (Input.GetMouseButtonUp(1))
				{
					Debug.Log(FXMakerCapture.GetCaptureScreenShotDir());
					EditorUtility.OpenWithDefaultApp(FXMakerCapture.GetCaptureScreenShotDir());
				}
 				else FXMakerCapture.CaptureScreenShot();
			}
			nVirticlaCount = 2;
		} else nVirticlaCount = 0;

		// Reload Project Data
		if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(FXMakerLayout.GetChildVerticalRect(FXMakerLayout.GetMenuChangeRect(), 0, 1+nVirticlaCount, nVirticlaCount, 1), 2, 0, 1), FXMakerTooltip.GetHcToolMain("LoadPrj")))
			LoadTool("Loaded Project");
		// Save Project Data
		FXMakerEffect fxMakerEffect = GetComponent<FXMakerEffect>();
		bool bEnable = (fxMakerEffect != null && GetComponent<FXMakerEffect>().IsReadOnlyFolder() == 0);
		if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(FXMakerLayout.GetChildVerticalRect(FXMakerLayout.GetMenuChangeRect(), 0, 1+nVirticlaCount, nVirticlaCount, 1), 2, 1, 1), FXMakerTooltip.GetHcToolMain("SavePrj"), bEnable))
			SaveTool("Saved Prjoect", true);
		SaveTooltip();
	}

	void OnHoverCommand_Popup(int nInstanceID)
	{
		FxmPopupManager.inst.OnHoverCommand_Popup(nInstanceID);
	}

	void OnHoverCommand_Button(string cmd)
	{
		if (cmd == "ShowThumbCaptureRect")
			NgGUIDraw.DrawBox(FXMakerLayout.GetOffsetRect(FXMakerCapture.GetThumbCaptureRect(), 5), new Color(0.8f, 0, 0), 2, false);
// 		if (cmd == "ShowSpriteCaptureRect")
// 			NgGUIDraw.DrawBox(FXMakerLayout.GetOffsetRect(FXMakerCapture.GetSpriteCaptureRect(), 5), new Color(0.8f, 0, 0), 2, false);
	}

	void OnRuntimeIntance(object arg)
	{
		NcEffectBehaviour._RuntimeIntance	runtimeIntance	= (NcEffectBehaviour._RuntimeIntance)arg;
		FxmInfoIndexing					indexCom		= runtimeIntance.m_ParentGameObject.GetComponent<FxmInfoIndexing>();
		if (indexCom == null)
			return;
// 		Debug.Log(runtimeIntance.m_ParentGameObject);
// 		Debug.Log(indexCom);
// 		Debug.Log(indexCom.m_OriginalTrans);
		FxmInfoIndexing.CreateInstanceIndexing(indexCom.m_OriginalTrans, runtimeIntance.m_ChildGameObject.transform, true, true);
//		bool bRoot = indexCom.m_OriginalTrans.gameObject == FXMakerHierarchy.inst.GetSelectedGameObject();
//		FXMakerHierarchy.inst.ChangeBoundsBoxWireframe(indexCom.m_OriginalTrans.gameObject, null, bRoot, true);
  		FXMakerHierarchy.inst.OnCreateInstanceEffect(runtimeIntance.m_ChildGameObject, false, runtimeIntance.m_ParentGameObject);

	}

	// -------------------------------------------------------------------------------------------
	public static bool IsWindowFocus()
	{
		return m_nLastFocusWindow != 0;
	}

	public static int GetWindowFocus()
	{
		return m_nLastFocusWindow;
	}

	public static int GetPrevWindowFocus()
	{
		return m_nPrevFocusWindow;
	}

	public KeyCode GetFocusInputKey(int nWindowId)
	{
		if (m_nLastFocusWindow == nWindowId)
			return m_keyLastInput;
		return 0;
	}

	public void SetFocusInputKey(KeyCode key)
	{
		m_keyLastInput = key;
	}


	// -------------------------------------------------------------------------------------------
	public Texture GetPrefabThumbTexture(GameObject effectPrefab)
	{
		return NgAsset.GetPrefabThumb(GetResourceDir(TOOLDIR_TYPE.TEMPPREVIEWTEXTURE), effectPrefab);
	}

	public Texture GetPrefabThumbTexture(string filename)
	{
		return NgAsset.GetThumbImage(GetResourceDir(TOOLDIR_TYPE.TEMPPREVIEWTEXTURE), filename);
	}

	// -------------------------------------------------------------------------------------------
	public bool IsLoadingProject()
	{
		return m_bLoadingProject;
	}

	public void LoadTool(string toolMsg)
	{
		m_bLoadingProject	= true;

 		NgUtil.LogDevelop("LoadTool");
		FxmPopupManager.inst.HideModalMessage();

		if (toolMsg != "")
			FxmPopupManager.inst.ShowToolMessage(toolMsg);

		GetComponent<FXMakerBackground>().LoadProject();
		GetComponent<FXMakerEffect>().LoadProject();
		if (m_nToolIndex == 1)
			ResetCamera();

		m_bLoadingProject = false;
	}

	public void OpenPrefab(GameObject targetPrefab)
	{
		SaveTool("Auto Saved");

		string prefabPath = AssetDatabase.GetAssetPath(targetPrefab);
		FXMakerEffect.inst.LoadProject(prefabPath);
	}

	public void SaveTool(string toolMsg)
	{
		SaveTool(toolMsg, false);
	}

	public void SaveTool(string toolMsg, bool bClickSaveButton)
	{
		NgUtil.LogDevelop("SaveTool");
		FxmPopupManager.inst.HideModalMessage();

		if (toolMsg != "")
			FxmPopupManager.inst.ShowToolMessage(toolMsg);

		switch (m_nToolIndex)
		{
			case 0:
				{
					GetComponent<FXMakerBackground>().SaveProject();
					break;
				}
			case 1:
				{
					GetComponent<FXMakerEffect>().SaveProject(bClickSaveButton);
					break;
				}
		}
	}

	public void SetActiveTool(int nToolIndex)
	{
		switch (nToolIndex)
		{
			case 0:
				{
					if (m_nToolIndex != nToolIndex)
						GetComponent<FXMakerEffect>().SaveProject();
					GetComponent<FXMakerBackground>().enabled	= true;
					GetComponent<FXMakerEffect>().enabled	= false;
					GetComponentInChildren<FXMakerGizmo>().enabled	= false;
					break;
				}
			case 1:
				{
					if (m_nToolIndex != nToolIndex)
						GetComponent<FXMakerBackground>().SaveProject();
					ResetCamera();
					GetComponent<FXMakerBackground>().enabled	= false;
					GetComponent<FXMakerEffect>().enabled	= true;
					GetComponentInChildren<FXMakerGizmo>().enabled	= true;
					break;
				}
		}

		m_nToolIndex = nToolIndex;
		EditorPrefs.SetInt("FXMakerMain.m_nToolIndex", m_nToolIndex);
	}

	public void ResetCamera()
	{
		bool bGray = (GetFXMakerHierarchy() != null) ? FXMakerGizmo.inst.IsGrayscale() : false;

		// Grayscale
		if (bGray)
		{
			int			grayLayer	= LayerMask.NameToLayer("TransparentFX");
			if (Camera.main != null)
			{
				Camera.main.clearFlags	= CameraClearFlags.Depth;
				Camera.main.cullingMask	= (1<<grayLayer);
			}
			if (GetGrayscaleCamera() != null)
			{
				if (Camera.main)
				{
					m_GrayscaleCamera.backgroundColor	= Camera.main.backgroundColor;
					m_GrayscaleCamera.fieldOfView		= Camera.main.fieldOfView;
					m_GrayscaleCamera.nearClipPlane		= Camera.main.nearClipPlane;
					m_GrayscaleCamera.farClipPlane		= Camera.main.farClipPlane;
					m_GrayscaleCamera.orthographicSize	= Camera.main.orthographicSize;
					m_GrayscaleCamera.orthographic		= Camera.main.orthographic;
				}
				m_GrayscaleCamera.enabled = true;
			}
		} else {
			if (Camera.main != null)
			{
				Camera.main.clearFlags	= CameraClearFlags.Skybox;
				Camera.main.cullingMask	= -1;
			}
			if (GetGrayscaleCamera() != null)
				m_GrayscaleCamera.enabled = false;
		}
	}

	// -------------------------------------------------------------------------------------------
	public bool IsCurrentEffectObject()
	{
		return (m_OriginalEffectPrefab != null && m_OriginalEffectObject != null);
	}

	public GameObject GetOriginalEffectPrefab()
	{
		return m_OriginalEffectPrefab;
	}

	public GameObject GetOriginalEffectObject()
	{
		return m_OriginalEffectObject;
	}

	public void ChangeRoot_OriginalEffectObject(GameObject newRoot)
	{
		FXMakerEffect.inst.SetChangePrefab();
		m_OriginalEffectObject = newRoot;
		m_OriginalEffectObject.name = m_OriginalEffectPrefab.name;
		SaveTool("");
	}

	public void ChangeRoot_InstanceEffectObject(GameObject newRoot)
	{
		m_InstanceEffectObject = newRoot;
	}

	public GameObject GetInstanceEffectObject()
	{
		return m_InstanceEffectObject;
	}
	public GameObject ClearCurrentEffectObject(GameObject effectRoot, bool bClearEventObject)
	{
		if (m_bApplicationQuit)
			return null;

		if (bClearEventObject)
		{
			GameObject	parentObj = GetInstanceRoot();
			if (parentObj != null)
				NgObject.RemoveAllChildObject(parentObj, true);
		}

		NgObject.RemoveAllChildObject(effectRoot, true);
		if (m_OriginalEffectObject != null)
			DestroyImmediate(m_OriginalEffectObject);
		CreateCurrentInstanceEffect(null);
		if (GetFXMakerHierarchy() != null)
			GetFXMakerHierarchy().ShowHierarchy(null);
		GameObject backPrefab = m_OriginalEffectPrefab;
		m_OriginalEffectPrefab = null;

		return backPrefab;
	}

	public void SetCurrentEffectPrefab(GameObject setPrefab, GameObject effectRoot, bool bUpdateOnlyPrefab)
	{
		if (m_bApplicationQuit)
			return;

		if (IsStartTooMain() || setPrefab == null)
			return;

		m_OriginalEffectPrefab	= setPrefab;
		if (bUpdateOnlyPrefab)
			return;

		NsEffectManager.PreloadResource(setPrefab);

		m_OriginalEffectObject	= NgAsset.LoadPrefab(setPrefab, effectRoot);
		m_OriginalEffectObject.name = m_OriginalEffectObject.name.Replace("(Clone)", "(Original)");
		NgObject.SetActiveRecursively(m_OriginalEffectObject, false);

		if (IsLoadingProject() == false)
			FXMakerAsset.SetPingObject(m_OriginalEffectObject);

		// EffectHierarchy
		if (GetFXMakerHierarchy() != null)
			GetFXMakerHierarchy().ShowHierarchy(m_OriginalEffectObject);
		// Create ShowObject
		CreateCurrentInstanceEffect(true);
	}

	// -------------------------------------------------------------------------------------------
	public void CreateCurrentInstanceEffect(bool bRunAction)
	{
		NgUtil.LogDevelop("CreateCurrentInstanceEffect() - bRunAction - " + bRunAction);

 		bool bTrue = CreateCurrentInstanceEffect((GetFXMakerHierarchy() != null) ? GetFXMakerHierarchy().GetShowGameObject() : m_OriginalEffectObject);
		if (bTrue && bRunAction)
			m_FXMakerControls.RunActionControl();
	}

	public GameObject GetInstanceRoot()
	{
		return NcEffectBehaviour.GetRootInstanceEffect();
	}

	void RemoveInvaildComponent(GameObject gameObj)
	{
		// MeshParticleEmitter - missing mesh - unity error
		ParticleEmitter[] pes = gameObj.GetComponentsInChildren<ParticleEmitter>(true);

		foreach (ParticleEmitter pe in pes)
		{
			if (NgSerialized.IsMeshParticleEmitter(pe as ParticleEmitter) && NgSerialized.GetMesh(pe, false) == null)
			{
// 				Debug.Log(pe.name + " - MeshParticleEmitter : missing mesh");
				if (pe.gameObject.GetComponent<Renderer>() != null)
					DestroyImmediate(pe.gameObject.GetComponent<Renderer>());
				DestroyImmediate(pe);
			}
		}

		// remove Dup Component - Over process error
		Transform[] transComs = gameObj.GetComponentsInChildren<Transform>(true);
		foreach (Transform trans in transComs)
		{
			NcEffectBehaviour[]	effComs = trans.GetComponents<NcEffectBehaviour>();
			foreach (NcEffectBehaviour eff in effComs)
			{
				if (eff != null && eff.CheckProperty() == "SCRIPT_WARRING_DUPLICATE")
				{
					Component[] dupcoms = eff.GetComponents(eff.GetType());
					for (int n = 0; n < dupcoms.Length; n++)
						if (0 < n)
							DestroyImmediate(dupcoms[n]);
				}
			}
		}

		// remove DisableComponent - AutoRet error
		NcEffectBehaviour[] effDComs = gameObj.GetComponentsInChildren<NcEffectBehaviour>(true);
		for (int n = 0; n < effDComs.Length; n++)
		{
			if (effDComs[n].enabled == false)
				DestroyImmediate(effDComs[n]);
		}
	}

	bool CreateCurrentInstanceEffect(GameObject gameObj)
	{
		NgUtil.LogDevelop("CreateCurrentInstanceEffect() - gameObj - " + gameObj);
		GameObject parentObj = GetInstanceRoot();

		m_bFrameCreateInstance	= true;

		// 이전거 삭제
		NgObject.RemoveAllChildObject(parentObj, true);

		// 순환 참조 prefab 검사
		NsEffectManager.PreloadResource(gameObj);

		// 새로 생성
		if (gameObj != null)
		{
			GameObject	createObj = (GameObject)Instantiate(gameObj);

			createObj.transform.parent = parentObj.transform;
			FxmInfoIndexing.CreateInstanceIndexing(gameObj.transform, createObj.transform, false, false);
			m_InstanceEffectObject = createObj;

			RemoveInvaildComponent(createObj);

			// Grayscale
			GetFXMakerHierarchy().OnCreateInstanceEffect(m_InstanceEffectObject, true, null);

#if (!UNITY_3_5)
			NgObject.SetActiveRecursively(createObj, true);
#endif

			m_FXMakerControls.SetStartTime();

			return true;
		}
		m_InstanceEffectObject = null;
		return false;
	}

	// =============================================================================================
	public Rect AutoFocusWindow(int id, Rect clientRect, GUI.WindowFunction func, string title)
	{
		Rect	rect	= GUI.Window(id, clientRect, func, FXMakerTooltip.GetGUIContentNoTooltip(title));
		Vector2	pos		= FXMakerLayout.GetGUIMousePosition();

		// auto focus
		if (GUI.enabled && (m_bPrevPopupWindow == false && m_bLastPopupWindow == false))
		{
//			if ((GUI.GetNameOfFocusedControl() != "TextField"))
			if (rect.Contains(pos))
			{
				m_nLastFocusWindow	= id;
				GUI.FocusWindow(id);
			}
		}
		return rect;
	}

	public Rect PopupFocusWindow(int id, Rect clientRect, GUI.WindowFunction func, string title)
	{
		GUISkin	oldSkin	= GUI.skin;

		GUI.skin = m_GuiPopupSkin;

		Rect rect = GUI.Window(id, clientRect, func, FXMakerTooltip.GetGUIContentNoTooltip(title));
 		m_bLastPopupWindow = true;
		if (GUI.enabled)
		{
			m_nLastFocusWindow	= id;
			GUI.FocusWindow(id);
			GUI.BringWindowToFront(id);
			SaveTooltip();
		}

		GUI.skin = oldSkin;
		return rect;
	}

// 	public void ModalFocusWindow(Rect clientRect, GUI.WindowFunction func, string title)
// 	{
// 		GUISkin	oldSkin	= GUI.skin;
// 
// 		GUI.skin = m_GuiPopupSkin;
// 		m_bLastPopupWindow = true;
// 		gNcLayout.ModalWindow(clientRect, func, title);
// 		GUI.skin = oldSkin;
// 	}

	public void ModalMsgWindow(int id, Rect clientRect, GUI.WindowFunction func, string title)
	{
//		gNcLayout.ModalWindow(clientRect, func, title);
		GUI.Window(id, clientRect, func, FXMakerTooltip.GetGUIContentNoTooltip(title));
		m_bLastPopupWindow = true;
		if (GUI.enabled)
		{
			m_nLastFocusWindow	= id;
			GUI.FocusWindow(id);
			GUI.BringWindowToFront(id);
		}
	}

	public void SaveTooltip()
	{
		// save Last tooltip
		if (GUI.tooltip != "")
			m_Tooltip = GUI.tooltip;
	}

	public void SaveTooltip(string toolTip)
	{
		m_Tooltip = toolTip;
	}

	public void SavePriorityTooltip(string toolTip)
	{
		m_PriorityTooltip = toolTip;
	}

	public UNITYWINDOW GetFocusUnityWindow()
	{
		for (int n=0; n < m_bFocusUnityWindows.Length; n++)
			if (m_bFocusUnityWindows[n])
				return (UNITYWINDOW)n;
		return UNITYWINDOW.None;
	}

	void CheckFocusedUnityWindow()
	{
		if (UnityEditor.EditorWindow.focusedWindow == null)
			return;

		string	name = UnityEditor.EditorWindow.focusedWindow.ToString();
		bool[]	m_bPrevFocusWindows	= new bool[(int)(UNITYWINDOW.Count)];

		for (int n=0; n < m_bFocusUnityWindows.Length; n++)
			m_bPrevFocusWindows[n]	= m_bFocusUnityWindows[n];

		NgUtil.ClearBools(m_bFocusUnityWindows);

		if (name.Contains(".ProjectWindow"))		m_bFocusUnityWindows[(int)(UNITYWINDOW.ProjectWindow)]			= true;
		if (name.Contains(".HierarchyWindow"))		m_bFocusUnityWindows[(int)(UNITYWINDOW.HierarchyWindow)]		= true;
		if (name.Contains(".InspectorWindow"))		m_bFocusUnityWindows[(int)(UNITYWINDOW.InspectorWindow)]		= true;
		if (name.Contains(".ConsoleWindow"))		m_bFocusUnityWindows[(int)(UNITYWINDOW.ConsoleWindow)]			= true;
		if (name.Contains(".GameView"))				m_bFocusUnityWindows[(int)(UNITYWINDOW.GameView)]				= true;
		if (name.Contains(".ColorPicker"))			m_bFocusUnityWindows[(int)(UNITYWINDOW.ColorPicker)]			= true;
		if (name.Contains(".ParticlsSystemWindow"))	m_bFocusUnityWindows[(int)(UNITYWINDOW.ParticlsSystemWindow)]	= true;

		if (m_bPrevFocusWindows[(int)(UNITYWINDOW.ProjectWindow)] == false && m_bFocusUnityWindows[(int)(UNITYWINDOW.ProjectWindow)] == true)
		{
// 			NgUtil.LogMessage("Focused ProjectWindow !!!");
// 			SaveTool("AutoSave Prjoect");
		}

		if (m_bPrevFocusWindows[(int)(UNITYWINDOW.ProjectWindow)] == true && m_bFocusUnityWindows[(int)(UNITYWINDOW.ProjectWindow)] == false)
		{
// 			NgUtil.LogMessage("Unfocused ProjectWindow !!!");
// 			LoadTool("AutoReloaded Prjoect");
		}

		if (m_bPrevFocusWindows[(int)(UNITYWINDOW.GameView)] == true && m_bFocusUnityWindows[(int)(UNITYWINDOW.GameView)] == false)
		{
// 			NgUtil.LogMessage("Unfocused GameView !!!");
			FxmPopupManager.inst.HideAllPopup(true);
		}

		if (m_bPrevFocusWindows[(int)(UNITYWINDOW.GameView)] == false && m_bFocusUnityWindows[(int)(UNITYWINDOW.GameView)] == true)
 		{
// 			NgUtil.LogMessage("Focused GameView !!!");
			FXMakerHierarchy.inst.OnActiveHierarchy();
 			CreateCurrentInstanceEffect(true);
		}

		if ((m_bPrevFocusWindows[(int)(UNITYWINDOW.HierarchyWindow)] || m_bPrevFocusWindows[(int)(UNITYWINDOW.InspectorWindow)]) && m_bFocusUnityWindows[(int)(UNITYWINDOW.GameView)] == true)
		{
			// enable save
			FXMakerEffect.inst.SetChangePrefab();

			// save background
			if (m_nToolIndex == 0)
				SaveTool("AutoSave");


//			gUtil.LogMessage("Unfocused HierarchyWindow or InspectorWindow !!!");
//			ShowToolMessage("Update FxmHierarchy");
//			GetFXMakerHierarchy().ShowHierarchy(null);
		}
	}

	// tooltip -------------------------------------------------------------
	void ProcessTooltip()
	{
		if (FxmPopupManager.inst.IsShowModalMessage() == false)
		{
			if (m_PriorityTooltip != "")
				m_Tooltip = m_PriorityTooltip;

			if (m_Tooltip != "")
			{
				if (0 < m_Tooltip.Trim().Length)
				{
					string tooltip = "";

					// HintRect
					tooltip = FXMakerTooltip.GetTooltip(FXMakerTooltip.TOOLTIPSPLIT.HintRect, m_Tooltip);
					if (tooltip != "")
						if (FXMakerOption.inst.m_bHintRedBox)
							FxmPopupManager.inst.ShowHintRect(FXMakerTooltip.GetHintRect(tooltip));

					// HoverCommand_Button
					tooltip = FXMakerTooltip.GetTooltip(FXMakerTooltip.TOOLTIPSPLIT.HoverCommand_Button, m_Tooltip);
					if (tooltip != "")
						OnHoverCommand_Button(tooltip);

					// HoverCommand_Popup Object
					tooltip = FXMakerTooltip.GetTooltip(FXMakerTooltip.TOOLTIPSPLIT.HoverCommand_Popup, m_Tooltip);
					if (tooltip != "")
						OnHoverCommand_Popup(System.Convert.ToInt32(tooltip));
					else OnHoverCommand_Popup(0);

					// Cursor Shot Tooltip
					tooltip = FXMakerTooltip.GetTooltip(FXMakerTooltip.TOOLTIPSPLIT.CursorTooltip, m_Tooltip);
					if (tooltip != "")
						FxmPopupManager.inst.ShowCursorTooltip(tooltip);

					// Bottom Long Tooltip
					tooltip = FXMakerTooltip.GetTooltip(FXMakerTooltip.TOOLTIPSPLIT.Tooltip, m_Tooltip);
					if (tooltip != "")
						FxmPopupManager.inst.ShowBottomTooltip(tooltip);

					FxmPopupManager.inst.UpdateBringWindow();
				}

				if (m_nOnGUICallCount == 2)
				{
					m_Tooltip = "";
					m_PriorityTooltip = "";
				}
			} else {
// 				OnHoverCommand_Hierarchy(0);
				string msg = m_EmptyTooltip;
				FXMakerEffect fxMakerEffect = GetComponent<FXMakerEffect>();
				if (fxMakerEffect != null && 0 < GetComponent<FXMakerEffect>().IsReadOnlyFolder())
					msg += "\n" + FXMakerTooltip.GetHsToolMessage("FOLDER_READONLY_BOTTOM", "");
				GUI.Label(FXMakerLayout.GetTooltipRect(), msg);
			}
		}
	}

	// ------------------------------------------------------------------
	void UnactiveOriginalObject()
	{
		FXMakerEffect effectToolMain = GetComponent<FXMakerEffect>();
		if (effectToolMain != null && effectToolMain.m_CurrentEffectRoot != null && GetOriginalEffectObject() != null)
		{
			// Original - CheckParent
			if (GetOriginalEffectObject().transform.parent == null)
				GetOriginalEffectObject().transform.parent = effectToolMain.m_CurrentEffectRoot.transform;

			// Unactive
			Transform[] trans = GetOriginalEffectObject().GetComponentsInChildren<Transform>(true);
			foreach (Transform tran in trans)
				NgObject.SetActive(tran.gameObject, false);
		}
	}
}
#endif
