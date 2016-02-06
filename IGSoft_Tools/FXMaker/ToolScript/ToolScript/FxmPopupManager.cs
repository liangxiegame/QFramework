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

public class FxmPopupManager : MonoBehaviour
{
	// Attribute ------------------------------------------------------------------------
	public		static FxmPopupManager			inst;
	protected	float							m_fMessageTime			= 3;

	public		GameObject						m_ToolPopupGameObject;

	// --------------------------------
	protected	FxmPopup						m_CurrentHierarchyPopup;
	protected	FxmMenuPopup					m_CurrentMenuPopup;
	protected	FxmSpritePopup					m_CurrentSpritePopup;
	protected	FxmFolderPopup					m_CurrentFolderPopup;

	// --------------------------------
	protected	bool							m_bShowByInspector		= false;
	protected	string							m_ToolMessage			= "";
	protected	int								m_nHintRectCount		= 0;
	protected	bool							m_bCursorTooltip		= false;
	protected	string							m_ScriptTooltip			= "";
	protected	string							m_StaticMessage			= "";

	protected	string							m_ModalMessage			= "";
	protected	FXMakerLayout.MODAL_TYPE		m_ModalType				= FXMakerLayout.MODAL_TYPE.MODAL_NONE;
	protected	FXMakerLayout.MODALRETURN_TYPE	m_nModalMessageValue	= FXMakerLayout.MODALRETURN_TYPE.MODALRETURN_CANCEL;

	// Property -------------------------------------------------------------------------
	public void RecreateInstanceEffect()
	{
		FXMakerMain.inst.CreateCurrentInstanceEffect(true);
	}

	public bool IsShowByInspector()
	{
		return m_bShowByInspector;
	}

	// Loop Function --------------------------------------------------------------------
	FxmPopupManager()
	{
		inst = this;
	}

	void Awake()
	{
		if (m_ToolPopupGameObject == null)
		{
			Debug.LogError("m_ToolPopupGameObject is null !!!");
		}
		FxmFolderPopup[] coms = m_ToolPopupGameObject.GetComponents<FxmFolderPopup>();
		foreach (FxmFolderPopup com in coms)
			com.enabled = false;
	}

	void OnEnable()
	{
	}

	void Start()
	{
	}

	void Update()
	{
		m_nHintRectCount		= 0;
		m_bCursorTooltip		= false;
	}

	// OnGUI ----------------------------------------------------------------------------------
	public void OnGUIToolMessage()
	{
		FxmPopupManager.inst.ShowToolMessage(m_ToolMessage);
		if (m_ToolMessage != "")
			GUI.Box(FXMakerLayout.GetToolMessageRect(), m_ToolMessage);
	}

	public void OnGUIModalMessage()
	{
		if (FxmPopupManager.inst.IsShowModalMessage())
		{
			FXMakerMain.inst.ModalMsgWindow(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.MODAL_MSG), FXMakerLayout.GetModalMessageRect(), winModalMessage, "");
 			GUI.enabled = false;
		}
	}

	public void OnGUIScriptTooltip()
	{
		if (m_ScriptTooltip != "")
			ShowScriptTooltip(m_ScriptTooltip);
		if (m_StaticMessage != "")
			ShowStaticMessage(m_StaticMessage);
	}

	public void OnGUIFolderPopup()
	{
		bool bDisable = false;
		if (m_CurrentFolderPopup != null && m_CurrentFolderPopup.IsShowPopup())
		{
			m_CurrentFolderPopup.OnGUIPopup();
			bDisable = true;
		}
		if (m_CurrentMenuPopup != null && m_CurrentMenuPopup.IsShowPopup())
		{
			m_CurrentMenuPopup.OnGUIPopup();
			bDisable = true;
		}
		if (m_CurrentSpritePopup != null && m_CurrentSpritePopup.IsShowPopup())
		{
			m_CurrentSpritePopup.OnGUIPopup();
			bDisable = true;
		}
		if (bDisable)
			GUI.enabled = false;
	}

	public void OnGUIHierarchyPopup()
	{
		if (m_CurrentHierarchyPopup && m_CurrentHierarchyPopup.IsShowPopup())
			m_CurrentHierarchyPopup.OnGUIPopup();
	}

	void winEmpty(int id)
	{
	}

	// ToolMessage window -----------------------------------------------------------------
	public void ShowToolMessage(string msg)
	{
		if (m_ToolMessage == msg)
			return;

		CancelInvoke("HideToolMessage");
		NgUtil.LogMessage("ToolMessage - " + msg);
		m_ToolMessage = msg;
		Invoke("HideToolMessage", m_fMessageTime*Time.timeScale);
	}

	public void ShowToolMessage(string msg, float time)
	{
		if (m_ToolMessage == msg)
			return;

		CancelInvoke("HideToolMessage");
		Debug.Log("ToolMessage - " + msg);
		m_ToolMessage = msg;
		Invoke("HideToolMessage", time*Time.timeScale);
	}

	public void ShowWarningMessage(string msg, float time)
	{
		if (m_ToolMessage == msg)
			return;

		CancelInvoke("HideToolMessage");
		 Debug.LogWarning("ToolMessage - " + msg);
		m_ToolMessage = msg;
		Invoke("HideToolMessage", time*Time.timeScale);
	}

	public void HideToolMessage()
	{
		m_ToolMessage = "";
	}

	// Tooltip window -------------------------------------------------------------------------
	public void ShowHintRect(Rect hintRect)
	{
//		NgGUIDraw.DrawBox(hintRect, NgLayout.m_ColorHelpBox, 1, false);
		GUIStyle style = GUI.skin.GetStyle("HintRect_Window");
		GUI.Window(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.HINTRECT)+m_nHintRectCount, hintRect, winEmpty, "", style);
		GUI.BringWindowToFront(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.HINTRECT)+m_nHintRectCount);
		m_nHintRectCount++;
	}

	public void ShowCursorTooltip(string toolTip)
	{
		if (FXMakerOption.inst.m_bShowCursorTooltip == false)
			return;

		GUIStyle style = GUI.skin.GetStyle("TooltipCursor_Wnd");
		bool bOldEnable = GUI.enabled;
		GUI.enabled = true;
		Vector2	size = style.CalcSize(new GUIContent(toolTip));
		GUI.Window(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.TOOLIP_CURSOR), FXMakerLayout.GetCursorTooltipRect(size), winEmpty, toolTip, style);
		m_bCursorTooltip = true;
		GUI.enabled = bOldEnable;
	}

	public void ShowBottomTooltip(string toolTip)
	{
		if (m_ScriptTooltip != "")
			return;
		if (FXMakerOption.inst.m_bShowBottomTooltip)
		{
			GUIStyle	tooltipStyle	= GUI.skin.GetStyle("TooltipBottom_Label");

			if (IsShowCurrentFolderPopup() && Screen.width/5*2 < GetCurrentFolderPopup().GetPopupRect().width)
				 tooltipStyle.alignment = TextAnchor.MiddleLeft;
			else tooltipStyle.alignment = TextAnchor.MiddleCenter;
			GUI.Label(FXMakerLayout.GetTooltipRect(), toolTip, tooltipStyle);
		}
	}

	public void SetStaticBottomMessage(string msg)
	{
		CancelInvoke("ClearStaticBottomMessage");
		m_StaticMessage	= msg;
// 		NgUtil.LogMessage("ToolMessage - " + msg);
		Invoke("ClearStaticBottomMessage", m_fMessageTime);
	}

	void ClearStaticBottomMessage()
	{
		m_StaticMessage = "";
	}

	public void SetDelayBottomMessage(string toolTip)
	{
		CancelInvoke("ClearDelayBottomMessage");
		m_ScriptTooltip	= toolTip;
		NgUtil.LogMessage("ToolMessage - " + toolTip);
		Invoke("ClearDelayBottomMessage", m_fMessageTime);
	}

	void ClearDelayBottomMessage()
	{
		m_ScriptTooltip = "";
	}

	public void SetScriptTooltip(string toolTip)
	{
		m_ScriptTooltip	= toolTip;
	}

	void ShowScriptTooltip(string toolTip)
	{
		GUIStyle	tooltipStyle	= GUI.skin.GetStyle("TooltipBottom_LeftLabel");
		GUI.Label(FXMakerLayout.GetTooltipRect(), toolTip, tooltipStyle);
	}

	void ShowStaticMessage(string toolTip)
	{
		GUIStyle	tooltipStyle	= GUI.skin.GetStyle("TooltipBottom_Label");
		GUI.Label(FXMakerLayout.GetToolMessageRect(), toolTip, tooltipStyle);
	}

	public void UpdateBringWindow()
	{
		if (m_bCursorTooltip)
		{
			if (IsShowCurrentFolderPopup() || IsShowHierarchyPopup())
				GUI.BringWindowToFront(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.POPUP));
			if (IsShowCurrentMenuPopup())
				GUI.BringWindowToFront(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.MENUPOPUP));
			if (IsShowSpritePopup())
				GUI.BringWindowToFront(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.SPRITEPOPUP));
			GUI.BringWindowToFront(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.TOOLIP_CURSOR));
		}
	}

	// All Popup -----------------------------------------------------------------
	public bool IsShowPopup()
	{
		return (IsShowCurrentFolderPopup() || IsShowHierarchyPopup() || IsShowCurrentMenuPopup() || IsShowSpritePopup());
	}

	public bool IsShowPopupExcludeMenu()
	{
		return (IsShowCurrentFolderPopup() || IsShowHierarchyPopup() || IsShowSpritePopup());
	}

	public void HideAllPopup(bool bSave)
	{
		// Close AutoFocus Popup
		HideHierarchyPopup(bSave);
		// Close ModalFocus Popup
		HideFolderPopup(bSave);

		HideSpritePopup();
	}

	public bool IsGUIMousePosition()
	{
		Rect rect = new Rect();

		if (IsShowCurrentFolderPopup())
			rect = FxmPopupManager.inst.GetCurrentFolderPopup().GetPopupRect();
		if (IsShowHierarchyPopup())
			rect = m_CurrentHierarchyPopup.GetPopupRect();
		if (IsShowCurrentMenuPopup())
			rect = m_CurrentMenuPopup.GetPopupRect();
		if (IsShowSpritePopup())
			rect = m_CurrentSpritePopup.GetPopupRect();

		if (rect.Contains(FXMakerLayout.GetGUIMousePosition()))
			return true;
		return false;
	}

	// Menu Popup window -----------------------------------------------------------------
	public FxmMenuPopup GetCurrentMenuPopup()
	{
		return m_CurrentMenuPopup;
	}

	public bool IsShowCurrentMenuPopup()
	{
		return (m_CurrentMenuPopup != null && m_CurrentMenuPopup.enabled);
	}

	public void ShowMenuPopup(string popupName, string[] displayedMenu)
	{
		ShowMenuPopup(popupName, displayedMenu, null);
	}

	public void ShowMenuPopup(string popupName, string[] displayedMenu, bool[] eabledMenu)
	{
		m_CurrentMenuPopup	= m_ToolPopupGameObject.GetComponent<FxmMenuPopup>();
		if (m_CurrentMenuPopup == null)
			m_CurrentMenuPopup	= m_ToolPopupGameObject.AddComponent<FxmMenuPopup>();
		if (m_CurrentMenuPopup != null)
		{
			if (eabledMenu == null)
				 m_CurrentMenuPopup.ShowPopupWindow(popupName, displayedMenu);
			else m_CurrentMenuPopup.ShowPopupWindow(popupName, displayedMenu, eabledMenu);
		}
	}

	public int GetSelectedIndex()
	{
		if (m_CurrentMenuPopup != null)
			return m_CurrentMenuPopup.GetSelectedIndex();
		return -1;
	}

	public void HideMenuPopup(bool bSave)
	{
		if (m_CurrentMenuPopup != null && m_CurrentMenuPopup.IsShowPopup())
			m_CurrentMenuPopup.ClosePopup(bSave);
	}

	// Sprite Popup -----------------------------------------------------------------
	public bool IsShowSpritePopup()
	{
		return (m_CurrentSpritePopup && m_CurrentSpritePopup.IsShowPopup());
	}

	public FxmSpritePopup GetSpritePopup()
	{
		return m_CurrentSpritePopup;
	}

	public void ShowSpritePopup()
	{
		m_CurrentSpritePopup	= m_ToolPopupGameObject.GetComponent<FxmSpritePopup>();
		if (m_CurrentSpritePopup == null)
			m_CurrentSpritePopup	= m_ToolPopupGameObject.AddComponent<FxmSpritePopup>();
		if (m_CurrentSpritePopup != null)
		{
			m_CurrentSpritePopup.ShowPopupWindow();
		}
	}

	public void HideSpritePopup()
	{
		if (m_CurrentSpritePopup != null && m_CurrentSpritePopup.IsShowPopup())
			m_CurrentSpritePopup.ClosePopup(false);
	}

	// Hierarchy Popup -----------------------------------------------------------------
	public FxmPopup GetCurrentHierarchyPopup()
	{
		if (m_CurrentHierarchyPopup && m_CurrentHierarchyPopup.IsShowPopup())
			return m_CurrentHierarchyPopup;
		return null;
	}

	public bool IsContainsHierarchyPopup(Vector2 mousePos)
	{
		return (IsShowHierarchyPopup() && m_CurrentHierarchyPopup.GetPopupRect().Contains(mousePos) == true);
	}

	public bool IsShowHierarchyPopup()
	{
		return (m_CurrentHierarchyPopup && m_CurrentHierarchyPopup.IsShowPopup());
	}

	public void ShowHierarchyRightPopup(FXMakerHierarchy.OBJECT_TYPE selObjType, Transform baseTransform, Object selObj, int selIndex)
	{
		FxmPopup_Object popupObject = m_ToolPopupGameObject.GetComponent<FxmPopup_Object>();
		if (popupObject == null)
			popupObject	= m_ToolPopupGameObject.AddComponent<FxmPopup_Object>();
		m_CurrentHierarchyPopup	 = popupObject;
		if (popupObject != null)
			popupObject.ShowPopupWindow(selObjType, baseTransform, selObj, selIndex);
	}

	public void ShowHierarchyObjectPopup(string scriptFilename, Object selObj)
	{
		if (scriptFilename != null)
		{
			m_CurrentHierarchyPopup	= m_ToolPopupGameObject.GetComponent(scriptFilename) as FxmPopup;
			if (m_CurrentHierarchyPopup == null)
				m_CurrentHierarchyPopup	= UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(m_ToolPopupGameObject, "Assets/IGSoft_Tools/FXMaker/ToolScript/ToolScript/FxmPopupManager.cs (418,31)", scriptFilename) as FxmPopup;
			if (m_CurrentHierarchyPopup != null)
				m_CurrentHierarchyPopup.ShowPopupWindow(selObj);
		}
	}

	public void HideHierarchyPopup(bool bSave)
	{
		if (m_CurrentHierarchyPopup != null && m_CurrentHierarchyPopup.IsShowPopup())
			m_CurrentHierarchyPopup.ClosePopup(bSave);
	}

	// Folder Popup window -----------------------------------------------------------------
	public FxmFolderPopup GetCurrentFolderPopup()
	{
		return m_CurrentFolderPopup;
	}

	public bool IsShowCurrentFolderPopup()
	{
		return (m_CurrentFolderPopup != null && m_CurrentFolderPopup.enabled);
	}

	public void HideFolderPopup(bool bSave)
	{
		if (m_CurrentFolderPopup != null && m_CurrentFolderPopup.IsShowPopup())
			m_CurrentFolderPopup.ClosePopup(bSave);
	}

	// Show Folder Popup -----------------------------------------------------------------
	public void ShowAddPrefabPopup(Transform trans)
	{
		if (SetFxmFolderPopup(typeof(FxmFolderPopup_Prefab)))
			(m_CurrentFolderPopup as FxmFolderPopup_Prefab).ShowPrefabPopupWindow(trans, false);
	}

	public void ShowSavePrefabPopup(Transform trans)
	{
		if (SetFxmFolderPopup(typeof(FxmFolderPopup_Prefab)))
			(m_CurrentFolderPopup as FxmFolderPopup_Prefab).ShowPrefabPopupWindow(trans, true);
	}

	public void	ShowSelectPrefabPopup(NcEffectBehaviour selCom, bool bShowByInspector, int nArgIndex)
	{
		if (SetFxmFolderPopup(typeof(FxmFolderPopup_Prefab)))
		{
			m_bShowByInspector = bShowByInspector;
			(m_CurrentFolderPopup as FxmFolderPopup_Prefab).ShowSelectPopupWindow(selCom, nArgIndex, 0);
		}
	}

	public void	ShowSelectPrefabPopup(NcEffectBehaviour selCom, int nArgIndex, int nSubArgIndex, bool bShowByInspector)
	{
		if (SetFxmFolderPopup(typeof(FxmFolderPopup_Prefab)))
		{
			m_bShowByInspector = bShowByInspector;
			(m_CurrentFolderPopup as FxmFolderPopup_Prefab).ShowSelectPopupWindow(selCom, nArgIndex, nSubArgIndex);
		}
	}

	public void ShowSelectMeshPopup(Component selCom)
	{
		if (SetFxmFolderPopup(typeof(FxmFolderPopup_Prefab)))
			(m_CurrentFolderPopup as FxmFolderPopup_Prefab).ShowSelectMeshPopupWindow(selCom);
	}

	public void ShowSelectAudioClipPopup(NcAttachSound selCom)
	{
	}

	public void ShowMaterialPopup(Transform selTrans, int nSelMatIndex)
	{
		if (SetFxmFolderPopup(typeof(FxmFolderPopup_Texture)))
			(m_CurrentFolderPopup as FxmFolderPopup_Texture).ShowPopupWindow(selTrans, nSelMatIndex);
	}

	public void ChangeMaterialColor(Transform selTrans, int nSelMatIndex, Color color)
	{
		if (SetFxmFolderPopup(typeof(FxmFolderPopup_Texture)))
			(m_CurrentFolderPopup as FxmFolderPopup_Texture).ChangeMaterialColor(selTrans, nSelMatIndex, color);
	}

	public void ShowNcCurveAnimationPopup(NcCurveAnimation selObj, bool bSaveDialog)
	{
		if (SetFxmFolderPopup(typeof(FxmFolderPopup_NcCurveAnimation)))
			(m_CurrentFolderPopup as FxmFolderPopup_NcCurveAnimation).ShowPopupWindow(selObj, bSaveDialog);
	}

	public void ShowNcInfoCurvePopup(NcCurveAnimation selObj, int nSelIndex, bool bSaveDialog)
	{
		if (SetFxmFolderPopup(typeof(FxmFolderPopup_NcInfoCurve)))
			(m_CurrentFolderPopup as FxmFolderPopup_NcInfoCurve).ShowPopupWindow(selObj, nSelIndex, bSaveDialog);
	}

	public void CloseNcPrefabPopup()
	{
		if (m_CurrentFolderPopup != null && m_CurrentFolderPopup.IsShowPopup())
			if (m_CurrentFolderPopup is FxmFolderPopup_Prefab || m_CurrentFolderPopup is FxmFolderPopup_Prefab)
				m_CurrentFolderPopup.ClosePopup(false);
	}

	public void CloseNcCurvePopup()
	{
		if (m_CurrentFolderPopup != null && m_CurrentFolderPopup.IsShowPopup())
			if (m_CurrentFolderPopup is FxmFolderPopup_NcCurveAnimation || m_CurrentFolderPopup is FxmFolderPopup_NcInfoCurve)
				m_CurrentFolderPopup.ClosePopup(false);
	}

	// MessageBox window -----------------------------------------------------------------
	public bool ShowModalOkCancelMessage(string msg)	// show state return
	{
		// ó�� ������
		if (msg != m_ModalMessage)
		{
			m_ModalMessage			= msg;
			m_ModalType				= FXMakerLayout.MODAL_TYPE.MODAL_OKCANCEL;
			m_nModalMessageValue	= FXMakerLayout.MODALRETURN_TYPE.MODALRETURN_SHOW;
			return true;
		} else {	// �ݺ���
			if (m_nModalMessageValue == FXMakerLayout.MODALRETURN_TYPE.MODALRETURN_SHOW)
				return true;
			m_ModalMessage	= "";
			m_ModalType		= FXMakerLayout.MODAL_TYPE.MODAL_NONE;
			return false;	// hide
		}
	}

	// ShowModalOkCancelMessage�� false �϶��� ����
	public FXMakerLayout.MODALRETURN_TYPE GetModalMessageValue()
	{
		return m_nModalMessageValue;
	}

	public bool IsShowModalMessage()
	{
		return m_ModalType != FXMakerLayout.MODAL_TYPE.MODAL_NONE;
	}

	public void HideModalMessage()
	{
		switch (m_ModalType)	// default return value
		{
			case FXMakerLayout.MODAL_TYPE.MODAL_NONE		:
			case FXMakerLayout.MODAL_TYPE.MODAL_MSG		:
			case FXMakerLayout.MODAL_TYPE.MODAL_OK		: m_nModalMessageValue = FXMakerLayout.MODALRETURN_TYPE.MODALRETURN_OK;		break;
			case FXMakerLayout.MODAL_TYPE.MODAL_YESNO		:
			case FXMakerLayout.MODAL_TYPE.MODAL_OKCANCEL	: m_nModalMessageValue = FXMakerLayout.MODALRETURN_TYPE.MODALRETURN_CANCEL;	break;
		}
		m_ModalType	= FXMakerLayout.MODAL_TYPE.MODAL_NONE;
	}

	void winModalMessage(int id)
	{
		Rect baseRect	= FXMakerLayout.GetModalMessageRect();
		Rect msgRect	= FXMakerLayout.GetChildVerticalRect(baseRect, 0, 10, 1, 5);
		Rect buttonRect	= FXMakerLayout.GetChildVerticalRect(baseRect, 0, 10, 6, 3);

		switch (m_ModalType)
		{
			case FXMakerLayout.MODAL_TYPE.MODAL_NONE		: break;
			case FXMakerLayout.MODAL_TYPE.MODAL_MSG		: break;
			case FXMakerLayout.MODAL_TYPE.MODAL_OK		: break;
			case FXMakerLayout.MODAL_TYPE.MODAL_YESNO		: break;
			case FXMakerLayout.MODAL_TYPE.MODAL_OKCANCEL	:
				{
					GUI.Label(FXMakerLayout.GetInnerHorizontalRect(msgRect, 10, 1, 9), m_ModalMessage);
					if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(buttonRect, 14, 2, 4), "OK"))
						m_nModalMessageValue = FXMakerLayout.MODALRETURN_TYPE.MODALRETURN_OK;
					if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(buttonRect, 14, 8, 4), "CANCEL"))
						m_nModalMessageValue = FXMakerLayout.MODALRETURN_TYPE.MODALRETURN_CANCEL;
					break;
				}
		}
	}

	// ---------------------------------------------------------------------------------
	bool SetFxmFolderPopup(System.Type fxmFolderPopup)
	{
		if (m_CurrentFolderPopup != null && m_CurrentFolderPopup.IsShowPopup())
			m_CurrentFolderPopup.ClosePopup(true);

		m_CurrentFolderPopup = m_ToolPopupGameObject.GetComponent(fxmFolderPopup) as FxmFolderPopup;
		if (m_CurrentFolderPopup == null)
			Debug.LogError("m_CurrentFolderPopup is null");
		return (m_CurrentFolderPopup != null);
	}

	// Event Function -------------------------------------------------------------------
	public void OnHoverCommand_Popup(int nInstanceID)
	{
		if (GetCurrentFolderPopup() == null)
			return;
		if (nInstanceID != 0)
		{
			Texture2D	selObj = EditorUtility.InstanceIDToObject(nInstanceID) as Texture2D;
			if (selObj != null)
				GetCurrentFolderPopup().SetPreviewTexture(selObj);
		} else GetCurrentFolderPopup().SetPreviewTexture(null);
	}

	public void OnClosePopup(FxmPopup popup)
	{
		m_bShowByInspector	= false;
	}
}
#endif
