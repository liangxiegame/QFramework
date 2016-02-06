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

public class FxmPopup : MonoBehaviour
{
	// Attribute ------------------------------------------------------------------------
	// const
	protected		const int			m_nButtonHeight		= 24;
	protected		int					m_nMarginUnfocus	= 20;

	// popup
	protected		Vector2				m_PopupPosition;
	protected		Transform			m_SelectedTransform;
	protected		GameObject			m_AddGameObject;
	protected		Object				m_AddComObject;
	protected		bool				m_bOldGUIEnable;

	// Property -------------------------------------------------------------------------
	public virtual void ClosePopup()
	{
		FxmPopupManager.inst.OnClosePopup(this);
		if (enabled == false)
			return;
		enabled = false;
	}

	public virtual void ClosePopup(bool bSave)
	{
		FxmPopupManager.inst.OnClosePopup(this);
		if (enabled == false)
			return;
		if (bSave)
		{
			if (m_AddGameObject != null || m_AddComObject != null)
				FXMakerHierarchy.inst.SetActiveComponent(m_AddGameObject, m_AddComObject, true);
			FXMakerMain.inst.CreateCurrentInstanceEffect(true);
		} else {
			// 저장 상관없는 것은 ClosePopup()으로 변경 후 살릴 수 있음
// 			Debug.Log("Undo");
// 			UndoObject();
		}
		enabled = false;
	}

	public bool IsShowPopup()
	{
		return (enabled);
	}

	public virtual bool ShowPopupWindow(Object selObj)
	{
		if (selObj != null)
		{
			Debug.LogError("Invaild Call ShowPopupWindow() -------------");
		}
		SetAddObject(null, null);
		return enabled;
	}

	protected virtual void UndoObject()
	{
	}

	protected void SetAddObject(GameObject addGameObject, Object addComObject)
	{
		m_AddGameObject	= addGameObject;
		m_AddComObject	= addComObject;
	}

	public virtual Rect GetPopupRect()
	{
		return new Rect(0,0,0,0);
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

	public virtual void OnGUIPopup()
	{
	}

	// ==========================================================================================================

	// ----------------------------------------------------------------------------------------------------------
	protected bool UnwindowClose()
	{
		if ((new Rect(0, 0, Screen.width, Screen.height)).Contains(FXMakerLayout.GetGUIMousePosition()) == false)
		{
			ClosePopup(true);
			return true;
		}
		return false;
	}

	protected bool UnscreenClose()
	{
		if ((new Rect(0, 0, Screen.width, Screen.height)).Contains(FXMakerLayout.GetGUIMousePosition()) == false)
		{
			ClosePopup(true);
			return true;
		}
		return false;
	}

	protected bool UnfocusClose(Rect rectPopup, int offsetLeft, int offsetTop, int offsetRight, int offsetBottom)
	{
		if (FXMakerLayout.GetOffsetRect(rectPopup, -m_nMarginUnfocus+offsetLeft, -m_nMarginUnfocus+offsetTop, m_nMarginUnfocus+offsetRight, m_nMarginUnfocus+offsetBottom).Contains(FXMakerLayout.GetGUIMousePosition()) == false)
		{
			ClosePopup(true);
			return true;
		}
		return false;
	}

	protected Rect GetPopupRectTop(int nWidth, int nHeight)
	{
		Vector2	pos		= m_PopupPosition;
//		Rect	rect	= new Rect(pos.x-nWidth*0.99f, pos.y-nHeight/2, nWidth, nHeight);
		Rect	rect	= new Rect(pos.x, pos.y-nHeight-10, nWidth, nHeight);

		rect = ClampWindow(rect, 0);
		return rect;
	}

	protected Rect GetPopupRectBottom(int nWidth, int nHeight)
	{
		Vector2	pos		= m_PopupPosition;
		Rect	rect	= new Rect(pos.x-nWidth/2, pos.y+20, nWidth, nHeight);

		rect = ClampWindow(rect, 0);
		return rect;
	}

	protected Rect GetPopupRectLeftBottom(int nWidth, int nHeight)
	{
		Vector2	pos		= m_PopupPosition;
		Rect	rect	= new Rect(pos.x-nWidth, pos.y+20, nWidth, nHeight);

		rect = ClampWindow(rect, 0);
		return rect;
	}

	protected Rect GetPopupRectRight(int nWidth, int nHeight)
	{
		Vector2	pos		= m_PopupPosition;
		Rect	rect	= new Rect(pos.x+20, pos.y-nHeight/2, nWidth, nHeight);

		rect = ClampWindow(rect, 30);
		return rect;
	}

	protected Rect GetPopupRectRight2(int nWidth, int nHeight)
	{
		Vector2	pos		= m_PopupPosition;
		Rect	rect	= new Rect(pos.x+5, pos.y-nHeight/2, nWidth, nHeight);

		rect = ClampWindow(rect, 30);
		return rect;
	}

	protected Rect GetPopupRectCenter(int nWidth, int nHeight)
	{
		Vector2	pos		= m_PopupPosition;
		Rect	rect	= new Rect(pos.x-nWidth*0.65f, pos.y-nHeight/2, nWidth, nHeight);

		rect = ClampWindow(rect, 30);
		return rect;
	}

	Rect ClampWindow(Rect popupRect, float fMarginBottom)
	{
		if (popupRect.x < 0) popupRect.x = 0;
		if (popupRect.y < 0) popupRect.y = 0;
		if (Screen.width  < popupRect.xMax) popupRect.x -= popupRect.xMax - Screen.width;
		if (Screen.height < popupRect.yMax+fMarginBottom) popupRect.y -= (popupRect.yMax+fMarginBottom) - Screen.height;
		return popupRect;
	}

	protected Rect GetPopupListRect(int nColumn, int nRow, int nCellWidth, int nCellHeight)
	{
		return new Rect(nColumn*nCellWidth, nRow*nCellHeight, nCellWidth, nCellHeight);
	}

	protected void SetGUIEnable(bool bEnable)
	{
		m_bOldGUIEnable	= GUI.enabled;
		GUI.enabled		= bEnable;
	}

	protected void RestoreGUIEnable()
	{
		GUI.enabled = m_bOldGUIEnable;
	}

	// Control Function -----------------------------------------------------------------

	// Event Function -------------------------------------------------------------------
}
#endif

