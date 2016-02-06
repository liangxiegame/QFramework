// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;

public class FXMakerLayout : NgLayout
{
	// Attribute ------------------------------------------------------------------------
	protected	static float	m_fFixedWindowWidth			= -1;
	protected	static float	m_fTopMenuHeight			= -1;
	protected	static bool		m_bLastStateTopMenuMini		= false;
	// 스크린정보
#if UNITY_ANDROID
#endif
#if UNITY_IPHONE
#endif

	// const
	public		static bool		m_bDevelopState				= false;
	public		static bool		m_bDevelopPrefs				= false;
	public		const string	m_CurrentVersion			= "v1.5.0";
	public		const int		m_nMaxResourceListCount		= 100;
	public		const int		m_nMaxPrefabListCount		= 500;
	public		const int		m_nMaxTextureListCount		= 500;
	public		const int		m_nMaxMaterialListCount		= 1000;

	public		const float		m_fScreenShotEffectZoomRate	= 1.0f;
	public		const float		m_fScreenShotBackZoomRate	= 0.6f;
	public		const float		m_fScrollButtonAspect		= 0.55f;
	public		const float		m_fReloadPreviewTime		= 0.5f;
	public		const int		m_nThumbCaptureSize			= 512;
	public		const int		m_nThumbImageSize			= 128;

	// 좌표정보
#if UNITY_EDITOR
	public		static float	GetFixedWindowWidth()		{ if (m_fFixedWindowWidth < 0) m_fFixedWindowWidth = UnityEditor.EditorPrefs.GetFloat("FXMakerOption.m_fFixedWindowWidth", 115); return m_fFixedWindowWidth; }
	public		static float	GetTopMenuHeight()			{ if (m_fTopMenuHeight < 0 || m_bLastStateTopMenuMini != (m_bMinimizeAll||m_bMinimizeTopMenu)) { m_bLastStateTopMenuMini = (m_bMinimizeAll||m_bMinimizeTopMenu); m_fTopMenuHeight = (m_bMinimizeAll||m_bMinimizeTopMenu ? m_MinimizeHeight : UnityEditor.EditorPrefs.GetFloat("FXMakerOption.m_fTopMenuHeight", 92)); } return m_fTopMenuHeight; }
#else
	public		static float	GetFixedWindowWidth()		{ return 115; }
	public		static float	GetTopMenuHeight()			{ return (m_bMinimizeAll||m_bMinimizeTopMenu ? m_MinimizeHeight : 92); }
#endif
	public		static Rect		m_rectOuterMargin			= new Rect( 2, 2, 0, 0);
	public		static Rect		m_rectInnerMargin			= new Rect( 7, 19, 7, 4);	// left, top, right, bottom
	public		static int		m_nSidewindowWidthCount		= 2;
	public		static float	m_fButtonMargin				= 3;
	public		static float	m_fScrollButtonHeight		= 70;

	public		static bool		m_bMinimizeTopMenu			= false;
	public		static bool		m_bMinimizeAll				= false;
	public		static float	m_fMinimizeClickWidth		= 60;
	public		static float	m_fMinimizeClickHeight		= 20;
	public		static float	m_fOriActionToolbarHeight	= 126;
	public		static float	m_fActionToolbarHeight		= m_fOriActionToolbarHeight;
	public		static float	m_MinimizeHeight			= 43;
	public		static float	m_fToolMessageHeight		= 50;
	public		static float	m_fTooltipHeight			= 60;
	public		static float	m_fModalMessageWidth		= 500;
	public		static float	m_fModalMessageHeight		= 200;
	public		static float	m_fTestPanelWidth			= 150;
	public		static float	m_fTestPanelHeight			= 120;
	public		static float	m_fOriTestPanelHeight		= m_fTestPanelHeight;

	// color
// 	public		static Color	m_ColorButtonHover			= new Color(0.25f, 1.0f, 0.37f, 1.0f);
// 	public		static Color	m_ColorButtonActive			= new Color(0.95f, 1.0f, 0.37f, 1.0f);
	// Hierarchy color
	public		static Color	m_ColorButtonHover			= new Color(0.7f, 1.0f, 0.9f, 1.0f);
	public		static Color	m_ColorButtonActive			= new Color(1.0f, 1.0f, 0.6f, 1.0f);
	public		static Color	m_ColorButtonMatNormal		= new Color(0.5f, 0.7f, 0.7f, 1.0f);
	public		static Color	m_ColorButtonUnityEngine	= new Color(0.7f, 0.7f, 0.7f, 1.0f);
	public		static Color	m_ColorDropFocused			= new Color(0.2f, 1.0f, 0.6f, 0.8f);
	public		static Color	m_ColorHelpBox				= new Color(1.0f, 0.1f, 0.1f, 1.0f);

	// wnd
	public		enum WINDOWID	{NONE=0, TOP_LEFT=10, TOP_CENTER, TOP_RIGHT, EFFECT_LIST, EFFECT_HIERARCHY, EFFECT_CONTROLS, PANEL_TEST, TOOLIP_CURSOR, MODAL_MSG, MENUPOPUP, SPRITEPOPUP, POPUP=100, RESOURCE_START=200, HINTRECT=300};
	public static int GetWindowId(WINDOWID id)			{ return (int)id; }

	// -----------------------------------------------------------------------------------------------------------------------------------------------------------
//	protected	static Rect		GetWindowRect(float x, float y, float width, float height)		{ return new Rect(x, y, x+width, y+height); }

	public		static Rect		GetChildTopRect(Rect rectParent, int topMargin, int nHeight)								{ return new Rect(m_rectInnerMargin.x, topMargin+m_rectInnerMargin.y, rectParent.width-m_rectInnerMargin.x-m_rectInnerMargin.width, nHeight);	}
	public		static Rect		GetChildBottomRect(Rect rectParent, int nHeight)											{ return new Rect(m_rectInnerMargin.x, rectParent.height-nHeight-m_rectInnerMargin.height, rectParent.width-m_rectInnerMargin.x-m_rectInnerMargin.width, nHeight);	}
	public		static Rect		GetChildVerticalRect(Rect rectParent, int topMargin, int count, int pos, int sumCount)		{ return new Rect(m_rectInnerMargin.x, topMargin+m_rectInnerMargin.y+(rectParent.height-topMargin-m_rectInnerMargin.y-m_rectInnerMargin.height)/count*pos, rectParent.width-m_rectInnerMargin.x-m_rectInnerMargin.width, (rectParent.height-topMargin-m_rectInnerMargin.y-m_rectInnerMargin.height)/count*sumCount-m_fButtonMargin);	}
	public		static Rect		GetInnerVerticalRect(Rect rectBase, int count, int pos, int sumCount)						{ return new Rect(rectBase.x, rectBase.y+(rectBase.height+m_fButtonMargin)/count*pos, rectBase.width, (rectBase.height+m_fButtonMargin)/count*sumCount-m_fButtonMargin); }
	public		static Rect		GetChildHorizontalRect(Rect rectParent, int topMargin, int count, int pos, int sumCount)	{ return new Rect(m_rectInnerMargin.x+(rectParent.width-m_rectInnerMargin.x-m_rectInnerMargin.width)/count*pos, topMargin+m_rectInnerMargin.y, (rectParent.width-m_rectInnerMargin.x-m_rectInnerMargin.width)/count*sumCount-m_fButtonMargin, rectParent.height-m_rectInnerMargin.y-m_rectInnerMargin.height); }
	public		static Rect		GetInnerHorizontalRect(Rect rectBase, int count, int pos, int sumCount)						{ return new Rect(rectBase.x+(rectBase.width+m_fButtonMargin)/count*pos, rectBase.y, (rectBase.width+m_fButtonMargin)/count*sumCount-m_fButtonMargin, rectBase.height); }

	public		static Rect		GetMenuChangeRect()					{ return new Rect(m_rectOuterMargin.x, m_rectOuterMargin.y, GetFixedWindowWidth(), GetTopMenuHeight());	}
	public		static Rect		GetMenuToolbarRect()				{ return new Rect(GetMenuChangeRect().xMax+m_rectOuterMargin.x, m_rectOuterMargin.y, Screen.width-GetMenuChangeRect().width-GetMenuTopRightRect().width-m_rectOuterMargin.x*4, GetTopMenuHeight());	}
	public		static Rect		GetMenuTopRightRect()				{ return new Rect(Screen.width - GetFixedWindowWidth() - m_rectOuterMargin.x, m_rectOuterMargin.y, GetFixedWindowWidth(), GetTopMenuHeight()); }
	public		static Rect		GetResListRect(int nIndex)			{ return new Rect((m_rectOuterMargin.x+(GetFixedWindowWidth()+m_rectOuterMargin.x)*nIndex), GetMenuChangeRect().yMax+m_rectOuterMargin.y, GetFixedWindowWidth(), Screen.height-GetMenuChangeRect().yMax-m_rectOuterMargin.y*2);	}
	public		static Rect		GetEffectListRect()					{ return new Rect(m_rectOuterMargin.x, GetMenuChangeRect().yMax+m_rectOuterMargin.y, GetFixedWindowWidth()*m_nSidewindowWidthCount+m_rectOuterMargin.x, Screen.height-GetMenuChangeRect().yMax-m_rectOuterMargin.y*2); }
	public		static Rect		GetEffectHierarchyRect()			{ return new Rect((Screen.width-(GetFixedWindowWidth()+m_rectOuterMargin.x)*m_nSidewindowWidthCount), GetMenuChangeRect().yMax+m_rectOuterMargin.y, GetFixedWindowWidth()*m_nSidewindowWidthCount+m_rectOuterMargin.x, Screen.height-GetMenuChangeRect().yMax-m_rectOuterMargin.y*2);	}
	public		static Rect		GetActionToolbarRect()				{ return new Rect(m_rectOuterMargin.x*3+GetFixedWindowWidth()*m_nSidewindowWidthCount, Screen.height-m_fActionToolbarHeight-m_rectOuterMargin.y, Screen.width-GetMenuChangeRect().width*4-m_rectOuterMargin.x*6, m_fActionToolbarHeight); }
	public		static Rect		GetToolMessageRect()				{ return new Rect(GetFixedWindowWidth()*2.1f, Screen.height-m_fActionToolbarHeight-m_rectOuterMargin.y-m_fToolMessageHeight-m_fTooltipHeight, Screen.width-GetFixedWindowWidth()*m_nSidewindowWidthCount*2-m_rectOuterMargin.x*2-m_fTestPanelWidth, m_fToolMessageHeight); }
	public		static Rect		GetTooltipRect()					{ return new Rect(m_rectOuterMargin.x*3+GetFixedWindowWidth()*m_nSidewindowWidthCount, Screen.height-m_fActionToolbarHeight-m_rectOuterMargin.y-m_fTooltipHeight, Screen.width-GetMenuChangeRect().width*4-m_rectOuterMargin.x*6-m_fTestPanelWidth, m_fTooltipHeight); }
	public		static Rect		GetCursorTooltipRect(Vector2 size)	{ return FXMakerLayout.ClampWindow(new Rect(Input.mousePosition.x+15, (Screen.height - Input.mousePosition.y + 80), size.x, size.y)); }
	public		static Rect		GetModalMessageRect()				{ return new Rect((Screen.width-m_fModalMessageWidth)/2, (Screen.height-m_fModalMessageHeight-m_fModalMessageHeight/8)/2, m_fModalMessageWidth, m_fModalMessageHeight); }
	public		static Rect		GetMenuGizmoRect()					{ return new Rect(m_rectOuterMargin.x*3+GetFixedWindowWidth()*m_nSidewindowWidthCount, GetTopMenuHeight()+m_rectOuterMargin.y, 35*14, 26); }
	public		static Rect		GetMenuTestPanelRect()				{ return new Rect(Screen.width-GetFixedWindowWidth()*2-m_fTestPanelWidth-m_rectOuterMargin.x*2, Screen.height-m_fActionToolbarHeight-m_rectOuterMargin.y-m_fTestPanelHeight, m_fTestPanelWidth, m_fTestPanelHeight); }
	public		static Rect		GetClientRect()						{ return new Rect(m_rectOuterMargin.x*3+GetFixedWindowWidth()*m_nSidewindowWidthCount, GetTopMenuHeight()+m_rectOuterMargin.y, Screen.width-(m_rectOuterMargin.x*3+GetFixedWindowWidth()*m_nSidewindowWidthCount)*2, Screen.height-m_fActionToolbarHeight-m_rectOuterMargin.y*3-GetTopMenuHeight()); }


	public		static Rect		GetScrollViewRect(int nWidth, int nButtonCount, int nColumn)		{ return new Rect(0, 0, nWidth-2, (m_fScrollButtonHeight) * (nButtonCount/nColumn + (0 < nButtonCount%nColumn ? 1 : 0)) + 25); }
	public		static Rect		GetScrollGridRect(int nWidth, int nButtonCount, int nColumn)		{ return new Rect(0, 0, nWidth-2, (m_fScrollButtonHeight) * (nButtonCount/nColumn + (0 < nButtonCount%nColumn ? 1 : 0))); }
	public		static Rect		GetAspectScrollViewRect(int nWidth, float fAspect, int nButtonCount, int nColumn, bool bImageOnly)	{ return new Rect(0, 0, nWidth-4, ((nWidth-4)/nColumn*fAspect+(bImageOnly?0:10)) * (nButtonCount/nColumn + (0 < nButtonCount%nColumn ? 1 : 0)) + 25); }
	public		static Rect		GetAspectScrollGridRect(int nWidth, float fAspect, int nButtonCount, int nColumn, bool bImageOnly)	{ return new Rect(0, 0, nWidth-4, ((nWidth-4)/nColumn*fAspect+(bImageOnly?0:10)) * (nButtonCount/nColumn + (0 < nButtonCount%nColumn ? 1 : 0))); }

	// -----------------------------------------------------------------------------------------------------------------------------------------------------------
	// -----------------------------------------------------------------------------------------------------------------------------------------------------------
	protected	static float	m_fArrowIntervalStartTime	= 0.20f;
	protected	static float	m_fArrowIntervalRepeatTime	= 0.10f;
	protected	static float	m_fKeyLastTime;
	public static KeyCode GetVaildInputKey(KeyCode key, bool bPress)
	{
		if (bPress || (m_fKeyLastTime + (m_fArrowIntervalRepeatTime * Time.timeScale) < Time.time))
		{
			m_fKeyLastTime = (bPress ? Time.time+m_fArrowIntervalStartTime : Time.time);
			return key;
		}
		return 0;
	}

	// -----------------------------------------------------------------------------------------------------------------------------------------------------------
	public static int GetGridHoverIndex(Rect windowRect, Rect listRect, Rect gridRect, int nCount, int nColumn, GUIStyle style)
	{
		int		nMargin			= (style == null ? 0 : style.margin.left);
		int		nRow			= (nCount/nColumn + (0 < (nCount%nColumn) ? 1 : 0));
		float	fButtonWidth	= (gridRect.width / nColumn);
		float	fButtonHeight	= (gridRect.height / nRow);

		Vector2	mpos	= GetGUIMousePosition() - new Vector2(windowRect.x, windowRect.y);
		if (listRect.Contains(mpos) == false)
			return -1;

		for (int n = 0; n < nCount; n++)
		{
			Rect buttonRect = new Rect(listRect.x + fButtonWidth*(n%nColumn)+nMargin, listRect.y + fButtonHeight*(n/nColumn)+nMargin, fButtonWidth-nMargin*2, fButtonHeight-nMargin*2);
			if (buttonRect.Contains(mpos))
			{
// 				Debug.Log(buttonRect);
// 				Debug.Log(n);
				return n;
			}
		}
		return -1;
	}
	public static int TooltipToolbar(Rect windowRect, Rect gridRect, int nGridIndex, GUIContent[] cons)
	{
		return TooltipToolbar(windowRect, gridRect, nGridIndex, cons, null);
	}
	public static int TooltipToolbar(Rect windowRect, Rect gridRect, int nGridIndex, GUIContent[] cons, GUIStyle style)
	{
		int nGroupIndex	= GUI.Toolbar(gridRect, nGridIndex, cons, style);
 		int nHoverIndex	= GetGridHoverIndex(windowRect, gridRect, gridRect, cons.Length, cons.Length, null);
		if (0 <= nHoverIndex)
			GUI.tooltip = cons[nHoverIndex].tooltip;
// 			GUI.tooltip = FXMakerTooltip.Tooltip((cons[nHoverIndex].text == null ? cons[nHoverIndex].tooltip : cons[nHoverIndex].text));
		return nGroupIndex;
	}
	public static int TooltipSelectionGrid(Rect windowRect, Rect listRect, int nGridIndex, GUIContent[] cons, int nColumCount)
	{
		return TooltipSelectionGrid(windowRect, listRect, listRect, nGridIndex, cons, nColumCount, null);
	}
	public static int TooltipSelectionGrid(Rect windowRect, Rect listRect, int nGridIndex, GUIContent[] cons, int nColumCount, GUIStyle style)
	{
		return TooltipSelectionGrid(windowRect, listRect, listRect, nGridIndex, cons, nColumCount, null);
	}
	public static int TooltipSelectionGrid(Rect windowRect, Rect listRect, Rect gridRect, int nGridIndex, GUIContent[] cons, int nColumCount)
	{
		return TooltipSelectionGrid(windowRect, listRect, gridRect, nGridIndex, cons, nColumCount, null);
	}
	public static int TooltipSelectionGrid(Rect windowRect, Rect listRect, Rect gridRect, int nGridIndex, GUIContent[] cons, int nColumCount, GUIStyle style)
	{
		int nGroupIndex	= GUI.SelectionGrid(gridRect, nGridIndex, cons, nColumCount, style);
 		int nHoverIndex	= GetGridHoverIndex(windowRect, listRect, gridRect, cons.Length, nColumCount, null);
		if (0 <= nHoverIndex)
			GUI.tooltip = cons[nHoverIndex].tooltip;
// 			GUI.tooltip = FXMakerTooltip.Tooltip((cons[nHoverIndex].text == null ? cons[nHoverIndex].tooltip : cons[nHoverIndex].text));
		return nGroupIndex;
	}
// 		// custom tooltip
//  		int nHoverIndex		= FXMakerLayout.GetGridHoverIndex(popupRect, FXMakerLayout.GetChildVerticalRect(popupRect, 0, 10, 0, 3), m_ProjectFolerContents.Length, m_ProjectFolerContents.Length, null);
// 		if (0 <= nHoverIndex)
// 			GUI.tooltip = m_ProjectFolerContents[nHoverIndex].tooltip;

	// -----------------------------------------------------------------------------------------------------------------------------------------------------------
	public enum MODAL_TYPE			{MODAL_NONE=0, MODAL_MSG, MODAL_OK, MODAL_YESNO, MODAL_OKCANCEL};
	public enum MODALRETURN_TYPE	{MODALRETURN_SHOW, MODALRETURN_OK, MODALRETURN_CANCEL};

	public static void ModalWindow(Rect rect, GUI.WindowFunction func, string title)
	{
		GUI.Window(GUIUtility.GetControlID(FocusType.Passive), rect, (int id) =>
		 {
			 GUI.depth = 0;
			 // first get a control id. every subsequent call to GUI control function will get a larger id 
			 int min = GUIUtility.GetControlID(FocusType.Native);
			 // we can use the id to check if current control is inside our window
			 if (GUIUtility.hotControl < min)
				 setHotControl(0);
			 // draw the window
			 func(id);
			 int max = GUIUtility.GetControlID(FocusType.Native);
			 if (GUIUtility.hotControl < min || (GUIUtility.hotControl > max && max != -1))
				 setHotControl(-1);
 			 GUI.FocusWindow(id);
			 GUI.BringWindowToFront(id);
		 }, title);
	}

	static void setHotControl(int id)
	{
		if (new Rect(0, 0, Screen.width, Screen.height).Contains(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)))
			GUIUtility.hotControl = id;
	}

//	[HideInInspector]

	// Property -------------------------------------------------------------------------
	// Control --------------------------------------------------------------------------
	// Event -------------------------------------------------------------------------
// 	void OnDrawGizmos()
// 	{
// 	}

}
