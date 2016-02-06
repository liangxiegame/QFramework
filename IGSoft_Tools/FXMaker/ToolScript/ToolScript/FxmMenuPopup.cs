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

public class FxmMenuPopup : FxmPopup
{
	// Attribute ------------------------------------------------------------------------
	// popup
	protected		string		m_TitleName			= "Popup Menu";
	protected		string[]	m_DisplayedMenu;
	protected		bool[]		m_EnabledMenu;
	protected		int			m_nSelectedIndex;
	protected		int			m_nLineCount;

	// Property -------------------------------------------------------------------------
	public bool ShowPopupWindow(string titleName, string[] displayedMenu)
	{
		m_TitleName	= titleName;
		bool[]	enabledMenu = new bool[displayedMenu.Length];
		for (int n = 0; n < enabledMenu.Length; n++)
			enabledMenu[n] = true;
		return ShowPopupWindow(titleName, displayedMenu, enabledMenu);
	}

	public bool ShowPopupWindow(string titleName, string[] displayedMenu, bool[] eabledMenu)
	{
		m_TitleName			= titleName;
		m_DisplayedMenu		= displayedMenu;
		m_EnabledMenu		= eabledMenu;
		m_nSelectedIndex	= -1;
		m_nLineCount		= GetLineCount();
		m_PopupPosition		= FXMakerLayout.GetGUIMousePosition();
		enabled				= true;

		base.ShowPopupWindow(null);
		return enabled;
	}

	public int GetSelectedIndex()
	{
		return m_nSelectedIndex;
	}

	int GetLineCount()
	{
		int			nDrawCount	= 0;

		for (int n = 0; n < m_DisplayedMenu.Length; n++)
		{
			if (m_DisplayedMenu[n] == "-")
				 nDrawCount += 1;
			else nDrawCount += 2;
		}
		return nDrawCount;
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
		FXMakerMain.inst.PopupFocusWindow(FXMakerLayout.GetWindowId(FXMakerLayout.WINDOWID.MENUPOPUP), GetPopupRect(), winPopup, m_TitleName);
	}

	// ==========================================================================================================
	void winPopup(int id)
	{
		Rect		baseRect	= GetPopupRect();
		Rect		lineRect;

		if (UnfocusClose(baseRect, 5, 0, 0, 0))
			return;

		baseRect = FXMakerLayout.GetChildVerticalRect(baseRect, 0, 1, 0, 1);

		int			nLineCount	= m_nLineCount;
		int			nDrawCount	= 0;

		for (int n = 0; n < m_DisplayedMenu.Length; n++)
		{
			if (m_DisplayedMenu[n] == "-")
			{
				// Draw line
				lineRect = FXMakerLayout.GetInnerVerticalRect(baseRect, nLineCount, nDrawCount, 1);
				NgGUIDraw.DrawHorizontalLine(new Vector2(lineRect.x, lineRect.y+lineRect.height/2), (int)lineRect.width, Color.gray, 2, false);
				nDrawCount += 1;
			} else {
				if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerVerticalRect(baseRect, nLineCount, nDrawCount, 2), GetHelpContent(m_DisplayedMenu[n]), m_EnabledMenu[n]))
				{
					m_nSelectedIndex = n;
					ClosePopup(false);
				}
				nDrawCount += 2;
			}
		}
		FXMakerMain.inst.SaveTooltip();
	}

	// ----------------------------------------------------------------------------------------------------------
	public override Rect GetPopupRect()
	{
		return GetPopupRectRight2(110, m_nLineCount*m_nButtonHeight/2 + 22);
	}

	// Control Function -----------------------------------------------------------------

	// Event Function -------------------------------------------------------------------

	// -------------------------------------------------------------------------------------------
	GUIContent GetHelpContent(string text)
	{
		return FXMakerTooltip.GetGUIContentNoTooltip(text);
// 		return NgTooltip.GetHcPopup_GameObject(text, "");
	}

	GUIContent GetHelpContent(string text, string arg)
	{
		return FXMakerTooltip.GetGUIContentNoTooltip(text);
// 		return NgTooltip.GetHcPopup_GameObject(text, arg);
	}
}

#endif