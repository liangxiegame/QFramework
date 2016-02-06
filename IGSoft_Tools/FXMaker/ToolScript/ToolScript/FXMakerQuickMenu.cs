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
using System;
using System.Collections.Generic;

public class FXMakerQuickMenu : MonoBehaviour
{
	// Attribute ------------------------------------------------------------------------
	public		static			FXMakerQuickMenu		inst;
	protected	bool			m_bMinimize				= false;
	protected	GameObject		m_ReplayObject;

	// -------------------------------------------------------------------------------------------
	void LoadPrefs()
	{
		if (FXMakerLayout.m_bDevelopPrefs == false)
		{
		}
		m_bMinimize		= EditorPrefs.GetBool("FXMakerQuickMenu.m_bMinimize", m_bMinimize);
	}

	// -------------------------------------------------------------------------------------------
	FXMakerQuickMenu()
	{
		inst = this;
	}

	// -------------------------------------------------------------------------------------------
	// -------------------------------------------------------------------------------------------
	void Awake()
	{
		LoadPrefs();
	}

	void OnEnable()
	{
		LoadPrefs();
	}
	
	void OnDisable()
	{
	}

	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}

	// -------------------------------------------------------------------------------------------
	public void OnGUIControl()
	{
		// Selected Info Window -----------------------------------------------
		FXMakerMain.inst.AutoFocusWindow(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.PANEL_TEST), FXMakerLayout.GetMenuTestPanelRect(), winActionToolbar, "");
	}

	// -------------------------------------------------------------------------------------------
	void winActionToolbar(int id)
	{
		Rect		popupRect	= FXMakerLayout.GetMenuTestPanelRect();
		Rect		baseRect;
		int			nRowCount	= 2;
		int			nRowIndex	= 0;

		// window desc -----------------------------------------------------------
		FXMakerTooltip.WindowDescription(popupRect, FXMakerLayout.WINDOWID.PANEL_TEST, null);

		// mini ----------------------------------------------------------------
		m_bMinimize = GUI.Toggle(new Rect(3, 1, popupRect.width, FXMakerLayout.m_fMinimizeClickHeight), m_bMinimize, "MiniQuickMenu");
		if (GUI.changed)
			EditorPrefs.SetBool("FXMakerQuickMenu.m_bMinimize", m_bMinimize);
		GUI.changed = false;
		if (FXMakerLayout.m_bMinimizeAll || m_bMinimize)
		{
			FXMakerLayout.m_fTestPanelHeight = FXMakerLayout.m_MinimizeHeight;

			nRowCount = 1;
			// mesh info -----------------------------------------------------------------
			baseRect = FXMakerLayout.GetChildVerticalRect(popupRect, 0, nRowCount, 0, 1);

			//

			FXMakerMain.inst.SaveTooltip();
			return;
		} else FXMakerLayout.m_fTestPanelHeight = FXMakerLayout.m_fOriTestPanelHeight;

		// info -----------------------------------------------------------------
		nRowCount	= 3;
// 		baseRect = FXMakerLayout.GetChildVerticalRect(popupRect, 0, nRowCount, 0, 2);
// 		if (NcEffectBehaviour.GetRootInstanceEffect())
// 		{
// 			info		= string.Format("P = {0}\nM = {1}\nT = {2}", m_nParticleCount, m_nMeshCount, m_nTriangles);
// 			infotooltip	= string.Format("ParticleCount = {0} MeshCount = {1}\n Mesh: Triangles = {2} Vertices = {3}", m_nParticleCount, m_nMeshCount, m_nTriangles, m_nVertices);
// 		}
// 		GUI.Box(FXMakerLayout.GetInnerHorizontalRect(baseRect, nColCount, 0, 1), new GUIContent(info, FXMakerTooltip.Tooltip(infotooltip)));

		// control button ------------------------------------------------------------
		if (FXMakerMain.inst.IsCurrentEffectObject())
		{
			// Replay ---------------------------------------
			bool bCreatedReplay = (m_ReplayObject != null && m_ReplayObject == FXMakerMain.inst.GetInstanceEffectObject());
			baseRect = FXMakerLayout.GetChildVerticalRect(popupRect, 0, nRowCount, nRowIndex++, 1);
			if (FXMakerLayout.GUIButton(baseRect, "CreateReplayEffect", true))
				CreateReplayEffect();
			baseRect = FXMakerLayout.GetChildVerticalRect(popupRect, 0, nRowCount, nRowIndex++, 1);
			if (FXMakerLayout.GUIButton(baseRect, "Replay", bCreatedReplay))
				RunReplayEffect(false);
			baseRect = FXMakerLayout.GetChildVerticalRect(popupRect, 0, nRowCount, nRowIndex++, 1);
			if (FXMakerLayout.GUIButton(baseRect, "Replay(ClearParticle)", bCreatedReplay))
				RunReplayEffect(true);
		}
		FXMakerMain.inst.SaveTooltip();
	}

	protected bool IsParticleEmitterOneShot(ParticleEmitter pe)
	{
		NcParticleSystem ps = pe.GetComponent<NcParticleSystem>();
		bool	bOneShot = (bool)NgSerialized.GetPropertyValue(new SerializedObject(pe as ParticleEmitter), "m_OneShot");
		return (bOneShot && (ps == null || ps.m_bBurst == false));
	}

	bool CheckNotSupportComponent(GameObject targetObj, System.Type comType)
	{
		FxmInfoIndexing	infoIndexingCom	= null;
		Component		findCom;
		string			comName = "";

		findCom = NsEffectManager.GetComponentInChildren(targetObj, comType);
		if (findCom != null)
		{
			string subString = "UnityEngine.";
			comName = findCom.ToString();
			if (comName.Contains(subString))
				comName = comName.Replace(subString, "");
			infoIndexingCom = findCom.GetComponent<FxmInfoIndexing>();
		}

		if (findCom is ParticleEmitter)
		{
			if (IsParticleEmitterOneShot(findCom as ParticleEmitter))
			{
				if (infoIndexingCom != null)
				{
					NcParticleSystem ncParticleSystem = infoIndexingCom.m_OriginalTrans.GetComponent<NcParticleSystem>();
					FXMakerHierarchy.inst.SetActiveComponent(infoIndexingCom.m_OriginalTrans.gameObject, ncParticleSystem, true); 
					if (ncParticleSystem == null)
					{
						FxmPopupManager.inst.ShowWarningMessage("- Replay Error!!! : Used the ParticleEmitter.OneShot." + comName + ".\nYou must change this to NcParticleSystem.bBurst. First, add NcParticleSystem.", 5);
					} else {
						FxmPopupManager.inst.ShowWarningMessage("- Replay Error!!! : Used the ParticleEmitter.OneShot." + comName + ".\nYou must change this to NcParticleSystem.bBurst. Please click button(Inspector - 'Convert: OneShot To FXMakerBurst')", 5);
					}
				}
			}
		} else
		if (findCom != null)
		{
			FxmPopupManager.inst.ShowToolMessage(" - Replay Error!!! : " + comName + " is included.\nThis component does not support the replay.", 5);
			return false;
		}
		return true;
	}

	bool CheckNotSupportComponents(GameObject targetObj)
	{
		if (CheckNotSupportComponent(targetObj, typeof(NcAttachPrefab)) == false)	return false;
		if (CheckNotSupportComponent(targetObj, typeof(NcDuplicator)) == false)		return false;
		if (CheckNotSupportComponent(targetObj, typeof(NcSpriteFactory)) == false)	return false;
		if (CheckNotSupportComponent(targetObj, typeof(NcDetachParent)) == false)	return false;
		if (CheckNotSupportComponent(targetObj, typeof(NcParticleEmit)) == false)	return false;
		if (CheckNotSupportComponent(targetObj, typeof(NcParticleSpiral)) == false)	return false;
		if (CheckNotSupportComponent(targetObj, typeof(NcTrailTexture)) == false)	return false;
		CheckNotSupportComponent(targetObj, typeof(ParticleEmitter));
		return true;
	}

	void CreateReplayEffect()
	{
		FXMakerMain.inst.GetFXMakerControls().SetDeactiveRepeatPlay();
		FXMakerMain.inst.CreateCurrentInstanceEffect(true);
		m_ReplayObject = FXMakerMain.inst.GetInstanceEffectObject();

		if (m_ReplayObject == null)
			return;

		CheckNotSupportComponents(m_ReplayObject);
// 		if (CheckNotSupportComponents(m_ReplayObject))
// 			m_ReplayObject = null;

		// Init Replay
		NsEffectManager.SetReplayEffect(m_ReplayObject);
		RunReplayEffect(true);
	}

	void RunReplayEffect(bool bClearOldParticle)
	{
		if (m_ReplayObject != null)
		// Run Replay
		NsEffectManager.RunReplayEffect(m_ReplayObject, bClearOldParticle);
	}

	// Property -------------------------------------------------------------------------
	// Control Function -----------------------------------------------------------------
	// Event Function -------------------------------------------------------------------
	// -----------------------------------------------------------------------------------
}

#endif
