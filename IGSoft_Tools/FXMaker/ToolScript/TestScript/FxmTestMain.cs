// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;

public class FxmTestMain : MonoBehaviour
{
	// -------------------------------------------------------------------------------------------
	public		static FxmTestMain		inst;

	public		GUISkin					m_GuiMainSkin;

	// --------------------------------
 	public		FxmTestMouse			m_FXMakerMouse;
	public		FxmTestControls			m_FXMakerControls;
	public		AnimationCurve			m_SimulateArcCurve;

	// --------------------------------
	public		GameObject				m_GroupList;
	public		int						m_CurrentGroupIndex;
	public		GameObject				m_PrefabList;
	public		int						m_CurrentPrefabIndex;

	public		bool					m_bAutoChange			= true;
	public		bool					m_bAutoSetting			= true;
	protected	GameObject				m_OriginalEffectObject	= null;
	protected	GameObject				m_InstanceEffectObject	= null;

	// -------------------------------------------------------------------------------------------
	public FxmTestMouse GetFXMakerMouse()
	{
		if (m_FXMakerMouse == null)
			m_FXMakerMouse = GetComponentInChildren<FxmTestMouse>();
		return m_FXMakerMouse;
	}

	public FxmTestControls GetFXMakerControls()
	{
		if (m_FXMakerControls == null)
			m_FXMakerControls = GetComponent<FxmTestControls>();
		return m_FXMakerControls;
	}

	// -------------------------------------------------------------------------------------------
	FxmTestMain()
	{
		inst = this;
	}

	void Awake()
	{
		NgUtil.LogDevelop("Awake - FXMakerMain");
		GetFXMakerControls().enabled		= true;
	}

	void OnEnable()
	{
		NgUtil.LogDevelop("OnEnable - FXMakerMain");
	}

	void Start()
	{
		if (0 < m_GroupList.transform.childCount)
			m_PrefabList = m_GroupList.transform.GetChild(0).gameObject;
		if (m_PrefabList != null && 0 < m_PrefabList.transform.childCount)
		{
			m_OriginalEffectObject = m_PrefabList.transform.GetChild(0).gameObject;
			CreateCurrentInstanceEffect(true);
		}
	}

	void Update()
	{
	}

	// -------------------------------------------------------------------------------------------
	public void OnGUI()
	{
		// Gizmo menu ----------------------------------------------------------
		GUI.skin = m_GuiMainSkin;
// 		FXMakerGizmo fxmGizmo = GetComponentInChildren<FXMakerGizmo>();
// 		if (fxmGizmo != null && fxmGizmo.enabled)
// 			fxmGizmo.OnGUIGizmo();

		float fButtonWidth	= Screen.width/7;
		float fButtonHeight	= Screen.height/10;

		// Child Window -------------------------------------------------------
		m_FXMakerControls.OnGUIControl();

		// Change Effect ------------------------------------------------------
 		if (GUI.Button(new Rect(0, 0, fButtonWidth, fButtonHeight), "GPrev"))
			ChangeGroup(false);
 		if (GUI.Button(new Rect(fButtonWidth+10, 0, fButtonWidth, fButtonHeight), "GNext"))
			ChangeGroup(true);

		GUI.Box(new Rect(0, fButtonHeight+10, fButtonWidth*2+10, 20), m_GroupList.transform.GetChild(m_CurrentGroupIndex).name, GUI.skin.FindStyle("Hierarchy_Button"));

 		if (GUI.Button(new Rect(Screen.width-fButtonWidth*2-10, 0, fButtonWidth, fButtonHeight), "EPrev"))
			ChangeEffect(false);
 		if (GUI.Button(new Rect(Screen.width-fButtonWidth, 0, fButtonWidth, fButtonHeight), "ENext"))
			ChangeEffect(true);

		// auto toggle
 		m_bAutoChange		= (GUI.Toggle(new Rect(Screen.width-fButtonWidth, fButtonHeight+10, fButtonWidth, 20), m_bAutoChange, "AutoChange"));
 		bool bAutoSetting	= (GUI.Toggle(new Rect(Screen.width-fButtonWidth*2-10, fButtonHeight+10, fButtonWidth, 20), m_bAutoSetting, "AutoSetting"));
		if (bAutoSetting != m_bAutoSetting)
		{
			m_bAutoSetting = bAutoSetting;
			if (bAutoSetting == false)
				m_FXMakerControls.SetDefaultSetting();
		}

		// distance
		float dist = GUI.VerticalSlider(new Rect(10, fButtonHeight+10+30, 25, Screen.height - (fButtonHeight+10+50) - GetFXMakerControls().GetActionToolbarRect().height), GetFXMakerMouse().m_fDistance, GetFXMakerMouse().m_fDistanceMin, GetFXMakerMouse().m_fDistanceMax);
		if (dist != GetFXMakerMouse().m_fDistance)
			GetFXMakerMouse().SetDistance(dist);
	}

	public void ChangeEffect(bool bNext)
	{
		if (m_PrefabList == null)
			return;

		if (bNext)
		{
			if (m_CurrentPrefabIndex < m_PrefabList.transform.childCount-1)
				m_CurrentPrefabIndex++;
			else {
				ChangeGroup(true);
				return;
			}
		} else {
			if (m_CurrentPrefabIndex == 0)
			{
				ChangeGroup(false);
				return;
			} else m_CurrentPrefabIndex--;
		}

		m_OriginalEffectObject = m_PrefabList.transform.GetChild(m_CurrentPrefabIndex).gameObject;
		CreateCurrentInstanceEffect(true);
	}

	public bool ChangeGroup(bool bNext)
	{
		if (bNext)
		{
			if (m_CurrentGroupIndex < m_GroupList.transform.childCount-1)
				m_CurrentGroupIndex++;
			else m_CurrentGroupIndex = 0;
		} else {
			if (m_CurrentGroupIndex == 0)
				m_CurrentGroupIndex = m_GroupList.transform.childCount-1;
			else m_CurrentGroupIndex--;
		}
		m_PrefabList = m_GroupList.transform.GetChild(m_CurrentGroupIndex).gameObject;
		if (m_PrefabList != null && 0 < m_PrefabList.transform.childCount)
		{
			m_CurrentPrefabIndex	= 0;
			m_OriginalEffectObject	= m_PrefabList.transform.GetChild(m_CurrentPrefabIndex).gameObject;
			CreateCurrentInstanceEffect(true);
			return true;
		} else {
			return true;
		}
	}

	// -------------------------------------------------------------------------------------------
	public bool IsCurrentEffectObject()
	{
		return (m_OriginalEffectObject != null);
	}

	public GameObject GetOriginalEffectObject()
	{
		return m_OriginalEffectObject;
	}

	public void ChangeRoot_OriginalEffectObject(GameObject newRoot)
	{
		m_OriginalEffectObject = newRoot;
	}

	public void ChangeRoot_InstanceEffectObject(GameObject newRoot)
	{
		m_InstanceEffectObject = newRoot;
	}

	public GameObject GetInstanceEffectObject()
	{
		return m_InstanceEffectObject;
	}

	public void ClearCurrentEffectObject(GameObject effectRoot, bool bClearEventObject)
	{
		if (bClearEventObject)
		{
			GameObject	parentObj = GetInstanceRoot();
			if (parentObj != null)
				NgObject.RemoveAllChildObject(parentObj, true);
		}

		NgObject.RemoveAllChildObject(effectRoot, true);
		m_OriginalEffectObject	= null;
		CreateCurrentInstanceEffect(null);
	}

	// -------------------------------------------------------------------------------------------
	public void CreateCurrentInstanceEffect(bool bRunAction)
	{
		// auto test setting
		FxmTestSetting setting = m_PrefabList.GetComponent<FxmTestSetting>();
		if (m_bAutoSetting && setting != null)
			m_FXMakerControls.AutoSetting(setting.m_nPlayIndex, setting.m_nTransIndex, setting.m_nTransAxis, setting.m_fDistPerTime, setting.m_nRotateIndex, setting.m_nMultiShotCount, setting.m_fTransRate, setting.m_fStartPosition);

		NgUtil.LogDevelop("CreateCurrentInstanceEffect() - bRunAction - " + bRunAction);
 		bool bTrue = CreateCurrentInstanceEffect(m_OriginalEffectObject);
		if (bTrue && bRunAction)
			m_FXMakerControls.RunActionControl();
	}

	public GameObject GetInstanceRoot()
	{
		return NcEffectBehaviour.GetRootInstanceEffect();
	}

	bool CreateCurrentInstanceEffect(GameObject gameObj)
	{
		NgUtil.LogDevelop("CreateCurrentInstanceEffect() - gameObj - " + gameObj);
		GameObject parentObj = GetInstanceRoot();

		// 이전거 삭제
		NgObject.RemoveAllChildObject(parentObj, true);

		// 새로 생성
		if (gameObj != null)
		{
			GameObject	createObj = (GameObject)Instantiate(gameObj);
			NsEffectManager.PreloadResource(createObj);

			createObj.transform.parent = parentObj.transform;
			m_InstanceEffectObject = createObj;
#if (!UNITY_3_5)
			NgObject.SetActiveRecursively(createObj, true);
#endif
			m_FXMakerControls.SetStartTime();

			return true;
		}
		m_InstanceEffectObject = null;
		return false;
	}

}


