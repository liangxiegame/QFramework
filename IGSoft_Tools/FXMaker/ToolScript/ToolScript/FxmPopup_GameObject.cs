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

public class FxmPopup_GameObject : FxmPopup
{
	// Attribute ------------------------------------------------------------------------
	// popup
	protected		Vector2				m_PopupScrollPos;

	protected		int					m_nGroupIndex;
	protected		GUIContent[]		m_GroupContents;
	protected		int					m_nPopupListIndex;
	protected		string[]			m_ScriptScrings;
	protected		GUIContent[]		m_ScriptContents;

	// Property -------------------------------------------------------------------------
	public override bool ShowPopupWindow(Object selObj)
	{
		if ((selObj is GameObject))
		{
			m_PopupPosition		= FXMakerLayout.GetGUIMousePosition();
			m_SelectedTransform	= (selObj as GameObject).transform;
			enabled	= true;

			LoadScriptList();
		} else {
			enabled	= false;
		}
		base.ShowPopupWindow(null);
		return enabled;
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
		// Popup Window ---------------------------------------------------------
		FXMakerMain.inst.PopupFocusWindow(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.POPUP), GetPopupRect(), winPopup, "Edit - " + m_SelectedTransform.name);
	}

	// ==========================================================================================================
	void winPopup(int id)
	{
		Rect		baseRect	= GetPopupRect();
		GUIStyle	styleList	= GUI.skin.GetStyle("List_Box");

		if (UnfocusClose(baseRect, 0, 0, 0, 0))
			return;

		// --------------------------------------------------------------------------------
		int		nMargin		= 5;
		int		nTopHeight	= 25;
		Rect	topRect		= FXMakerLayout.GetChildTopRect(baseRect, -nMargin, nTopHeight);
		Rect	bottomRect	= FXMakerLayout.GetChildVerticalRect(baseRect, nTopHeight, 1, 0, 1);
		Rect	leftRect	= FXMakerLayout.GetInnerHorizontalRect(bottomRect, 6, 0, 6);

		// top - Name ----------------------------------------------------------------------------
		topRect = FXMakerLayout.GetOffsetRect(topRect, -nMargin/2);
		GUI.Box(FXMakerLayout.GetOffsetRect(topRect, 0, -2, 0, 2), "");
		Rect nameRect = FXMakerLayout.GetInnerVerticalRect(topRect, 2, 0, 2);
		GUI.Label(FXMakerLayout.GetInnerHorizontalRect(nameRect, 5, 0, 1), "Name");
		GUI.SetNextControlName("TextField");
// 		FXMakerMain.inst.ToggleGlobalLangSkin(true);
		string newName = FXMakerLayout.GUITextField(FXMakerLayout.GetInnerHorizontalRect(nameRect, 5, 1, 4), m_SelectedTransform.name, m_SelectedTransform.gameObject != FXMakerMain.inst.GetOriginalEffectObject());
// 		FXMakerMain.inst.ToggleGlobalLangSkin(false);
		if (m_SelectedTransform.name != newName)
		{
			m_SelectedTransform.name = newName;
			if (newName.Trim() != "" && m_SelectedTransform.gameObject == FXMakerMain.inst.GetOriginalEffectObject())
				FXMakerEffect.inst.RenameCurrentPrefab(newName);
		}
		if (Event.current.type == EventType.KeyDown && Event.current.character == '\n')
			ClosePopup(true);

		// left ----------------------------------------------------------------------------
		leftRect = FXMakerLayout.GetOffsetRect(leftRect, -nMargin/2);
		GUI.Box(leftRect, "");
		leftRect = FXMakerLayout.GetOffsetRect(leftRect, -nMargin);
		Rect	scrollRect		= FXMakerLayout.GetInnerVerticalRect(leftRect, 20, 2, 15);
		int		scrollBarWidth	= 13;
		int		nCellHeight		= 18;

		scrollRect.width -= scrollBarWidth;
		GUI.Box(scrollRect, "");

		// folder list
//		m_nGroupIndex		= GUI.SelectionGrid(FXMakerLayout.GetInnerVerticalRect(leftRect, 20, 0, 2), m_nGroupIndex, m_GroupContents, m_GroupContents.Length);
		m_nGroupIndex		= FXMakerLayout.TooltipSelectionGrid(GetPopupRect(), FXMakerLayout.GetInnerVerticalRect(leftRect, 20, 0, 2), m_nGroupIndex, m_GroupContents, m_GroupContents.Length);

		if (GUI.changed)
			LoadScriptList();

		// script list
		Rect	listRect	= new Rect(0, 0, scrollRect.width-1, nCellHeight*m_ScriptScrings.Length);
		m_PopupScrollPos	= GUI.BeginScrollView(scrollRect, m_PopupScrollPos, listRect);
		GUI.changed = false;
//		m_nPopupListIndex	= GUI.SelectionGrid(listRect, m_nPopupListIndex, m_ScriptContents, 1, styleList);
		m_nPopupListIndex	= FXMakerLayout.TooltipSelectionGrid(FXMakerLayout.GetOffsetRect(GetPopupRect(), 0, -m_PopupScrollPos.y), scrollRect, listRect, m_nPopupListIndex, m_ScriptContents, 1, styleList);

		if (GUI.changed && Input.GetMouseButtonUp(1))
			AddScript(m_ScriptScrings[m_nPopupListIndex]);

		GUI.EndScrollView();

		// Add script button
		if (0 <= m_nPopupListIndex && m_nPopupListIndex < m_ScriptContents.Length)
		{
			if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerVerticalRect(leftRect, 20, 17, 3), GetHelpContent("Add Component "), (m_ScriptScrings[m_nPopupListIndex] != "")))
			{
				AddScript(m_ScriptScrings[m_nPopupListIndex]);
				if (Input.GetMouseButtonUp(0))
					ClosePopup(true);
			}
		}

		FXMakerMain.inst.SaveTooltip();
	}

	// ----------------------------------------------------------------------------------------------------------
	public override Rect GetPopupRect()
	{
		return GetPopupRectTop(280, 375);
	}

	void LoadScriptList()
	{
		int		nScriptCount;
		int		nFindFolderCount;
		int		nCount = 0;
		string	scriptDir = FXMakerMain.inst.GetResourceDir(FXMakerMain.TOOLDIR_TYPE.SCRIPTS);

		// load folder
		string[] groupStrings		= NgAsset.GetFolderList(scriptDir, null, "Hide", 0, out nFindFolderCount);
		m_GroupContents = NgConvert.StringsToContents(groupStrings);

		if (m_nGroupIndex < 0 || m_GroupContents.Length <= m_nGroupIndex)
			m_nGroupIndex	= 0;
		if (m_GroupContents.Length == 0)
			m_nGroupIndex	= -1;

		// load file
		if (m_GroupContents[m_nGroupIndex].text == "Unity")
		{
			List<string>	addList = new List<string>();

			// add UnityComponent
			addList.Add("MeshFilter");
			addList.Add("MeshRenderer");
			addList.Add("");
			addList.Add("ParticleSystem");
			addList.Add("ParticleSystemRenderer");
			addList.Add("TrailRenderer");
			addList.Add("LineRenderer");
			addList.Add("");
			addList.Add("EllipsoidParticleEmitter");
			addList.Add("MeshParticleEmitter");
			addList.Add("ParticleAnimator");
			addList.Add("ParticleRenderer");
			addList.Add("");
			addList.Add("Rigidbody");
			addList.Add("BoxCollider");
			addList.Add("SphereCollider");
			addList.Add("CapsuleCollider");
			addList.Add("MeshCollider");
			addList.Add("WorldParticleCollider");
			addList.Add("");
			addList.Add("Animation");
			addList.Add("AudioSource");

			m_ScriptScrings		= new string[addList.Count];
			m_ScriptContents	= new GUIContent[addList.Count];

			for (int n = 0; n < addList.Count; n++)
			{
				m_ScriptContents[n]	= GetHelpContentScript(addList[n]);
				m_ScriptScrings[n]	= addList[n];
			}
		} else {
			string			dir				= (m_nGroupIndex < 0 ? scriptDir : NgFile.CombinePath(scriptDir, m_GroupContents[m_nGroupIndex].text + "/"));
			string[]		scriptScrings	= NgAsset.GetFileList(dir, 0, out nScriptCount);

			m_ScriptScrings		= new string[scriptScrings.Length];
			m_ScriptContents	= new GUIContent[scriptScrings.Length];

			for (int n = 0; n < scriptScrings.Length; n++)
			{
// 				if (scriptScrings[n].Contains("Behaviour."))
// 					continue;

				string ext = Path.GetExtension(scriptScrings[n]);
				ext = ext.ToLower();
				if (ext == ".cs" || ext == ".js")
				{
					m_ScriptContents[nCount]	= GetHelpContentScript(NgFile.TrimFileExt(scriptScrings[n]));
					m_ScriptScrings[nCount]		= NgFile.TrimFileExt(scriptScrings[n]);
					nCount++;
				}
			}
			m_ScriptScrings		= NgConvert.ResizeArray(m_ScriptScrings, nCount);
 			m_ScriptContents	= NgConvert.ResizeArray(m_ScriptContents, nCount);
		}
	}

	void AddScript(string scriptName)
	{
		Component com = UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(m_SelectedTransform.gameObject, "Assets/IGSoft_Tools/FXMaker/ToolScript/ToolScript/FxmPopup_GameObject.cs (233,19)", scriptName);
		if (com != null)
		{
			FxmPopupManager.inst.ShowToolMessage("success... Add script - " + scriptName);
			SetAddObject(m_SelectedTransform.gameObject, com);
			FXMakerHierarchy.inst.OnAddComponent(com);
		} else FxmPopupManager.inst.ShowToolMessage("Failed... Add script - " + scriptName);
	}

	// Control Function -----------------------------------------------------------------

	// -------------------------------------------------------------------------------------------
	GUIContent GetHelpContent(string text)
	{
		return FXMakerTooltip.GetHcPopup_GameObject(text, "");
	}

	GUIContent GetHelpContent(string text, string arg)
	{
		return FXMakerTooltip.GetHcPopup_GameObject(text, arg);
	}

	GUIContent GetHelpContentScript(string text)
	{
		return FXMakerTooltip.GetHcPopup_EffectScript(text);
	}
}

#endif
