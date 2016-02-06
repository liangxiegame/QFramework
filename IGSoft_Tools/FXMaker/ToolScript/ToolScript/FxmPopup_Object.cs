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
using System.Reflection; 

public class FxmPopup_Object : FxmPopup
{
	// Attribute ------------------------------------------------------------------------
	// popup
	protected		Object							m_SelectedObject;
	protected		int								m_nSelectedIndex;
	protected		int								m_nButtonCount;
	protected		FXMakerHierarchy.OBJECT_TYPE	m_SelObjectType;

	// Property -------------------------------------------------------------------------
	public bool ShowPopupWindow(FXMakerHierarchy.OBJECT_TYPE selObjType, Transform baseTransform, Object selObj, int selIndex)
	{
		m_SelectedTransform	= baseTransform;
		m_SelObjectType		= selObjType;
		m_nSelectedIndex	= selIndex;
		return ShowPopupWindow(selObj);
	}

	public override bool ShowPopupWindow(Object selObj)
	{
		m_nButtonCount		= 20;
		m_PopupPosition		= FXMakerLayout.GetGUIMousePosition();
		m_SelectedObject	= selObj;
		enabled	= true;

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
		FXMakerMain.inst.PopupFocusWindow(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.POPUP), GetPopupRect(), winPopup, "Right Menu");
	}

	// ==========================================================================================================
	void winPopup(int id)
	{
		Rect		baseRect	= GetPopupRect();
		Rect		buttonRect;
		Rect		lineRect;

		if (UnfocusClose(baseRect, -10, 0, 0, 0))
			return;

		baseRect = FXMakerLayout.GetChildVerticalRect(baseRect, 0, 1, 0, 1);

		Transform	transOriginalRoot = FXMakerMain.inst.GetOriginalEffectObject().transform;
		int			nButtonCount	= m_nButtonCount*2;
		int			nDrawCount		= 0;
		bool		bEnable			= false;

		// Copy
		if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerVerticalRect(baseRect, nButtonCount, nDrawCount, 2), GetHelpContent("Copy"), true))
		{
			FXMakerClipboard.inst.SetClipboardObject(m_SelectedObject);
			ClosePopup(true);
			return;
		}
		nDrawCount += 2;

		// Cut
		switch (m_SelObjectType)
		{
			case FXMakerHierarchy.OBJECT_TYPE.OBJECT_GAMEOBJECT:	bEnable	= ( m_SelectedTransform != transOriginalRoot);	break;
			case FXMakerHierarchy.OBJECT_TYPE.OBJECT_TRANSFORM:		bEnable	= false;	break;
			case FXMakerHierarchy.OBJECT_TYPE.OBJECT_EASYEFFECT:	bEnable	= true;		break;
			case FXMakerHierarchy.OBJECT_TYPE.OBJECT_UNITYENGINE:	bEnable	= true;		break;
			case FXMakerHierarchy.OBJECT_TYPE.OBJECT_OTHER:			bEnable	= true;		break;
			case FXMakerHierarchy.OBJECT_TYPE.OBJECT_MATERIAL:		bEnable	= false;	break;
			case FXMakerHierarchy.OBJECT_TYPE.OBJECT_ANICLIP:		bEnable	= false;	break;
			default : Debug.LogWarning("not declare");	break;
		}
		if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerVerticalRect(baseRect, nButtonCount, nDrawCount, 2), GetHelpContent("Cut"), bEnable))
		{
			FXMakerClipboard.inst.SetClipboardObject(m_SelectedObject);
			FXMakerHierarchy.inst.DeleteHierarchyObject(m_SelectedTransform, m_SelectedObject, m_nSelectedIndex);
			ClosePopup(true);
			return;
		}
		nDrawCount += 2;

		// Paste
		switch (m_SelObjectType)
		{
			case FXMakerHierarchy.OBJECT_TYPE.OBJECT_GAMEOBJECT:
			case FXMakerHierarchy.OBJECT_TYPE.OBJECT_TRANSFORM:
			case FXMakerHierarchy.OBJECT_TYPE.OBJECT_EASYEFFECT:
			case FXMakerHierarchy.OBJECT_TYPE.OBJECT_UNITYENGINE:
			case FXMakerHierarchy.OBJECT_TYPE.OBJECT_OTHER:			bEnable	= FXMakerClipboard.inst.IsObject();	break;
			case FXMakerHierarchy.OBJECT_TYPE.OBJECT_MATERIAL:		bEnable	= false;	break;
			case FXMakerHierarchy.OBJECT_TYPE.OBJECT_ANICLIP:		bEnable	= false;	break;
			default : Debug.LogWarning("not declare");	break;
		}
		if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerVerticalRect(baseRect, nButtonCount, nDrawCount, 2), GetHelpContent("Paste", FXMakerClipboard.inst.GetName()), bEnable))
		{
			Object tarObj = FXMakerClipboard.inst.PasteClipboardObject(m_SelectedTransform.gameObject, m_SelectedObject, m_nSelectedIndex);
			if (tarObj is GameObject)
				 SetAddObject((tarObj as GameObject), tarObj);
			else SetAddObject(null, tarObj);
			ClosePopup(true);
			return;
		}
		nDrawCount += 2;

		// Overwrite
		switch (m_SelObjectType)
		{
			case FXMakerHierarchy.OBJECT_TYPE.OBJECT_GAMEOBJECT:		bEnable	= false;	break;
			case FXMakerHierarchy.OBJECT_TYPE.OBJECT_TRANSFORM:		bEnable	= FXMakerClipboard.inst.IsTransform() && FXMakerClipboard.inst.GetObject().GetType() == m_SelectedObject.GetType();		break;
			case FXMakerHierarchy.OBJECT_TYPE.OBJECT_EASYEFFECT:
			case FXMakerHierarchy.OBJECT_TYPE.OBJECT_UNITYENGINE:
			case FXMakerHierarchy.OBJECT_TYPE.OBJECT_OTHER:			bEnable	= FXMakerClipboard.inst.IsComponent() && FXMakerClipboard.inst.GetObject().GetType() == m_SelectedObject.GetType();		break;
			case FXMakerHierarchy.OBJECT_TYPE.OBJECT_MATERIAL:		bEnable	= FXMakerClipboard.inst.IsMaterial() && FXMakerClipboard.inst.GetObject().GetType() == m_SelectedObject.GetType();		break;
			case FXMakerHierarchy.OBJECT_TYPE.OBJECT_ANICLIP:		bEnable	= FXMakerClipboard.inst.IsAnimationClip() && FXMakerClipboard.inst.GetObject().GetType() == m_SelectedObject.GetType();	break;
			default: Debug.LogWarning("not declare"); break;
		}
		if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerVerticalRect(baseRect, nButtonCount, nDrawCount, 2), GetHelpContent("Overwrite", FXMakerClipboard.inst.GetName()), bEnable))
		{
			FXMakerClipboard.inst.OverwriteClipboardObject(m_SelectedTransform.gameObject, m_SelectedObject, m_nSelectedIndex);
			ClosePopup(true);
			return;
		}
		nDrawCount += 2;

		// Draw line
		lineRect = FXMakerLayout.GetInnerVerticalRect(baseRect, nButtonCount, nDrawCount, 1);
		NgGUIDraw.DrawHorizontalLine(new Vector2(lineRect.x, lineRect.y+lineRect.height/2), (int)lineRect.width, Color.gray, 2, false);
		nDrawCount += 1;

		// Duplicate
		switch (m_SelObjectType)
		{
			case FXMakerHierarchy.OBJECT_TYPE.OBJECT_GAMEOBJECT:	bEnable	= (m_SelectedTransform != transOriginalRoot);	break;
			case FXMakerHierarchy.OBJECT_TYPE.OBJECT_TRANSFORM:		bEnable	= false;	break;
			case FXMakerHierarchy.OBJECT_TYPE.OBJECT_EASYEFFECT:	bEnable	= true;		break;
			case FXMakerHierarchy.OBJECT_TYPE.OBJECT_UNITYENGINE:	bEnable	= true;		break;
			case FXMakerHierarchy.OBJECT_TYPE.OBJECT_OTHER:			bEnable	= true;		break;
			case FXMakerHierarchy.OBJECT_TYPE.OBJECT_MATERIAL:		bEnable	= false;	break;
			case FXMakerHierarchy.OBJECT_TYPE.OBJECT_ANICLIP:		bEnable	= false;	break;
			default: Debug.LogWarning("not declare"); break;
		}
		if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerVerticalRect(baseRect, nButtonCount, nDrawCount, 2), GetHelpContent("Duplicate"), bEnable))
		{
			switch (m_SelObjectType)
			{
				case FXMakerHierarchy.OBJECT_TYPE.OBJECT_GAMEOBJECT:	SetAddObject(FXMakerHierarchy.inst.AddGameObject(m_SelectedTransform.transform.parent.gameObject, m_SelectedTransform.gameObject), null);		break;
				case FXMakerHierarchy.OBJECT_TYPE.OBJECT_TRANSFORM:		break;
				case FXMakerHierarchy.OBJECT_TYPE.OBJECT_EASYEFFECT:
				case FXMakerHierarchy.OBJECT_TYPE.OBJECT_UNITYENGINE:
				case FXMakerHierarchy.OBJECT_TYPE.OBJECT_OTHER:			m_AddComObject = NgSerialized.CloneComponent(m_SelectedObject as Component, (m_SelectedObject as Component).gameObject, false);	break;
				case FXMakerHierarchy.OBJECT_TYPE.OBJECT_MATERIAL:		break;
				case FXMakerHierarchy.OBJECT_TYPE.OBJECT_ANICLIP:		break;
				default: Debug.LogWarning("not declare"); break;
			}
			ClosePopup(true);
			return;
		}
		nDrawCount += 2;

		// Disable
		if (m_SelObjectType == FXMakerHierarchy.OBJECT_TYPE.OBJECT_GAMEOBJECT)
		{
			bool bObjEnable = (m_SelectedTransform.gameObject.GetComponent<NcDontActive>() == null);
			if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerVerticalRect(baseRect, nButtonCount, nDrawCount, 2), (bObjEnable ? GetHelpContent("Disable") : GetHelpContent("Enable")), (m_SelectedTransform != transOriginalRoot)))
			{
				FXMakerHierarchy.inst.SetEnableGameObject(m_SelectedTransform.gameObject, !bObjEnable);
				ClosePopup(true);
				return;
			}
		} else {
			buttonRect = FXMakerLayout.GetInnerVerticalRect(baseRect, nButtonCount, nDrawCount, 2);
			if (m_SelectedObject is Component)
			{
				int	nObjEnable = EditorUtility.GetObjectEnabled(m_SelectedObject);
				if (0 <= nObjEnable)
				{
					if (0 < nObjEnable)
					{
						if (GUI.Button(buttonRect, GetHelpContent("Disable")))
						{
							EditorUtility.SetObjectEnabled(m_SelectedObject, false);
							FXMakerHierarchy.inst.OnEnableComponent(m_SelectedObject as Component, false);
							ClosePopup(true);
							return;
						}
					} else {
						if (GUI.Button(buttonRect, GetHelpContent("Enable")))
						{
							EditorUtility.SetObjectEnabled(m_SelectedObject, true);
							FXMakerHierarchy.inst.OnEnableComponent(m_SelectedObject as Component, true);
							ClosePopup(true);
							return;
						}
					}
				} else {
					FXMakerLayout.GUIButton(buttonRect, GetHelpContent("Disable"), false);
				}
			} else FXMakerLayout.GUIButton(buttonRect, GetHelpContent("Disable"), false);
		}
		nDrawCount += 2;


		// Delete
		switch (m_SelObjectType)
		{
			case FXMakerHierarchy.OBJECT_TYPE.OBJECT_GAMEOBJECT:	bEnable	= ( m_SelectedTransform != transOriginalRoot);	break;
			case FXMakerHierarchy.OBJECT_TYPE.OBJECT_TRANSFORM:		bEnable	= false;	break;
			case FXMakerHierarchy.OBJECT_TYPE.OBJECT_EASYEFFECT:	bEnable	= true;		break;
			case FXMakerHierarchy.OBJECT_TYPE.OBJECT_UNITYENGINE:	bEnable	= true;		break;
			case FXMakerHierarchy.OBJECT_TYPE.OBJECT_OTHER:			bEnable	= true;		break;
			case FXMakerHierarchy.OBJECT_TYPE.OBJECT_MATERIAL:		bEnable	= true;		break;
			case FXMakerHierarchy.OBJECT_TYPE.OBJECT_ANICLIP:		bEnable	= true;		break;
			default: Debug.LogWarning("not declare"); break;
		}
		if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerVerticalRect(baseRect, nButtonCount, nDrawCount, 2), GetHelpContent("Delete"), bEnable))
		{
			FXMakerHierarchy.inst.DeleteHierarchyObject(m_SelectedTransform, m_SelectedObject, m_nSelectedIndex);
			ClosePopup(true);
			return;
		}
		nDrawCount += 2;

		// Draw line
		lineRect = FXMakerLayout.GetInnerVerticalRect(baseRect, nButtonCount, nDrawCount, 1);
		NgGUIDraw.DrawHorizontalLine(new Vector2(lineRect.x, lineRect.y+lineRect.height/2), (int)lineRect.width, Color.gray, 2, false);
		nDrawCount += 1;

		// -------------------------------------------------------------------------------------
		if (m_SelectedObject is NcCurveAnimation)
		{
			// NcCurveAnimation
			NcCurveAnimation	curveCom = m_SelectedObject as NcCurveAnimation;
			if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerVerticalRect(baseRect, nButtonCount, nDrawCount, 2), GetHelpContent("SaveCurves"), 0 < curveCom.GetCurveInfoCount()))
			{
				ClosePopup(true);
				FxmPopupManager.inst.ShowNcCurveAnimationPopup(curveCom, true);
				return;
			}
			nDrawCount += 2;
			if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerVerticalRect(baseRect, nButtonCount, nDrawCount, 2), GetHelpContent("LoadCurves"), true))
			{
				ClosePopup(true);
				FxmPopupManager.inst.ShowNcCurveAnimationPopup(curveCom, false);
				return;
			}
			nDrawCount += 2;
		}
		// -------------------------------------------------------------------------------------
		if (m_SelObjectType == FXMakerHierarchy.OBJECT_TYPE.OBJECT_GAMEOBJECT)
		{
			// Add Child
			if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerVerticalRect(baseRect, nButtonCount, nDrawCount, 2), GetHelpContent("Add Child"), true))
			{
// 				GameObject	newChild	= new GameObject("GameObject");
// 				newChild.transform.parent = m_SelectedTransform;
// 				FXMakerHierarchy.inst.OnAddGameObject(newChild);
// 				SetAddObject(newChild, null);
// 				ClosePopup(true);

				FXMakerHierarchy.inst.ShowAddObjectRightPopup();
				ClosePopup(false);

				return;
			}
			nDrawCount += 2;

			// Add Parent
			if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerVerticalRect(baseRect, nButtonCount, nDrawCount, 2), GetHelpContent("Add Parent"), true))
			{
				GameObject newParent = new GameObject("GameObject");
				newParent.transform.parent	= m_SelectedTransform.parent;
				m_SelectedTransform.parent	= newParent.transform;
				m_SelectedTransform.name	= m_SelectedTransform.name.Replace("(Original)", "");
				if (m_SelectedTransform == transOriginalRoot)
					FXMakerMain.inst.ChangeRoot_OriginalEffectObject(newParent);
				FXMakerHierarchy.inst.OnAddGameObject(newParent);
				SetAddObject(newParent, null);
				ClosePopup(true);
				return;
			}
			nDrawCount += 2;

			// MoveToParent
			if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerVerticalRect(baseRect, nButtonCount, nDrawCount, 2), GetHelpContent("MoveToParent"), (m_SelectedTransform != transOriginalRoot && (m_SelectedTransform.parent != transOriginalRoot) || (m_SelectedTransform.parent == transOriginalRoot && transOriginalRoot.childCount==1))))
			{
				if (m_SelectedTransform.parent == transOriginalRoot && transOriginalRoot.childCount==1)
				{
					FXMakerMain.inst.SaveTool("");
					m_SelectedTransform = FXMakerMain.inst.GetOriginalEffectObject().transform;
					// root swap
					Transform toolRoot	= m_SelectedTransform.parent;
					Transform newParent = m_SelectedTransform.GetChild(0);
					Transform newChild	= m_SelectedTransform;
					newChild.parent		= null;
					newParent.parent	= null;
					newChild.parent		= newParent;
					newParent.parent	= toolRoot;
					m_SelectedTransform = newParent;
					FXMakerMain.inst.ChangeRoot_OriginalEffectObject(m_SelectedTransform.gameObject);
					SetAddObject(null, null);
				} else {
					m_SelectedTransform.parent = m_SelectedTransform.parent.parent;
				}
				ClosePopup(true);
				return;
			}
			nDrawCount += 2;

			// Add Component
			if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerVerticalRect(baseRect, nButtonCount, nDrawCount, 2), GetHelpContent("Add Component"), true))
			{
				ClosePopup(true);
				FxmPopupManager.inst.ShowHierarchyObjectPopup("FxmPopup_GameObject", m_SelectedTransform.gameObject);
				return;
			}
			nDrawCount += 2;

			// Add Prefab
			if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerVerticalRect(baseRect, nButtonCount, nDrawCount, 2), GetHelpContent("Add Prefab"), true))
			{
				FxmPopupManager.inst.ShowAddPrefabPopup(m_SelectedTransform);
				ClosePopup(true);
				return;
			}
			nDrawCount += 2;

			// Save Prefab
			if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerVerticalRect(baseRect, nButtonCount, nDrawCount, 2), GetHelpContent("Save Prefab"), true))
			{
				ClosePopup(true);
				FxmPopupManager.inst.ShowSavePrefabPopup(m_SelectedTransform);
				return;
			}
			nDrawCount += 2;
		}
		// -------------------------------------------------------------------------------------
		if (m_SelObjectType == FXMakerHierarchy.OBJECT_TYPE.OBJECT_MATERIAL)
		{
			bEnable	= NgMaterial.IsMaterialColor(m_SelectedObject as Material);

			// Copy Color
			if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerVerticalRect(baseRect, nButtonCount, nDrawCount, 2), GetHelpContent("Copy Color"), bEnable))
			{
				FXMakerClipboard.inst.SetClipboardColor(NgMaterial.GetMaterialColor(m_SelectedObject as Material));
				ClosePopup(true);
				return;
			}
			if (bEnable)
			{
				Rect colorRect = FXMakerLayout.GetOffsetRect(FXMakerLayout.GetInnerVerticalRect(baseRect, nButtonCount, nDrawCount, 2), -5);
				colorRect.width = colorRect.height;
				EditorGUIUtility.DrawColorSwatch(colorRect, NgMaterial.GetMaterialColor(m_SelectedObject as Material));
			}
			nDrawCount += 2;

			// Paste Color
			if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerVerticalRect(baseRect, nButtonCount, nDrawCount, 2), GetHelpContent("Paste Color"), bEnable))
			{
				FXMakerClipboard.inst.PasteClipboardColor(m_SelectedTransform, m_nSelectedIndex, m_SelectedObject as Material);
				ClosePopup(true);
				return;
			}
			{
				Rect colorRect = FXMakerLayout.GetOffsetRect(FXMakerLayout.GetInnerVerticalRect(baseRect, nButtonCount, nDrawCount, 2), -5);
				colorRect.width = colorRect.height;
				EditorGUIUtility.DrawColorSwatch(colorRect, FXMakerClipboard.inst.m_CopyColor);
			}
			nDrawCount += 2;
		}

		m_nButtonCount = nDrawCount/2;
		FXMakerMain.inst.SaveTooltip();
	}

	// ----------------------------------------------------------------------------------------------------------
	public override Rect GetPopupRect()
	{
		return GetPopupRectRight(110, m_nButtonCount*m_nButtonHeight + 22);
	}

	// Control Function -----------------------------------------------------------------

	// Event Function -------------------------------------------------------------------

	// -------------------------------------------------------------------------------------------
	GUIContent GetHelpContent(string text)
	{
		return FXMakerTooltip.GetHcPopup_GameObject(text, "");
	}

	GUIContent GetHelpContent(string text, string arg)
	{
		return FXMakerTooltip.GetHcPopup_GameObject(text, arg);
	}
}

#endif
