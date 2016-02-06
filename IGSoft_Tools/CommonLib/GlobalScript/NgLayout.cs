// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;

public class NgLayout
{
	// Attribute ------------------------------------------------------------------------
	protected	static Color	m_GuiOldColor;
	protected	static bool		m_GuiOldEnable;

	// Rect -----------------------------------------------------------------------------------------------------------------------------------------------------------
	public		static Rect		GetZeroRect()																				{ return new Rect(0, 0, 0, 0); }
	public		static Rect		GetSumRect(Rect rect1, Rect rect2)															{ return GetOffsetRect(rect1, Mathf.Min(0, rect2.xMin - rect1.xMin), Mathf.Min(0, rect2.yMin - rect1.yMin), Mathf.Max(0, rect2.xMax-rect1.xMax), Mathf.Max(0, rect2.yMax-rect1.yMax)); }
	public		static Rect		GetOffsetRect(Rect rect, float left, float top)												{ return new Rect(rect.x+left, rect.y+top, rect.width, rect.height); }
	public		static Rect		GetOffsetRect(Rect rect, float left, float top, float right, float bottom)					{ return new Rect(rect.x+left, rect.y+top, rect.width-left+right, rect.height-top+bottom); }
	public		static Rect		GetOffsetRect(Rect rect, float fOffset)														{ return new Rect(rect.x-fOffset, rect.y-fOffset, rect.width+fOffset*2, rect.height+fOffset*2); }
	public		static Rect		GetVOffsetRect(Rect rect, float fOffset)													{ return new Rect(rect.x, rect.y-fOffset, rect.width, rect.height+fOffset*2); }
	public		static Rect		GetHOffsetRect(Rect rect, float fOffset)													{ return new Rect(rect.x-fOffset, rect.y, rect.width+fOffset*2, rect.height); }
	public		static Rect		GetOffsetRateRect(Rect rect, float fOffsetRate)												{ return new Rect(rect.x-Mathf.Abs(rect.x)*fOffsetRate, rect.y-Mathf.Abs(rect.y)*fOffsetRate, rect.width+Mathf.Abs(rect.x)*fOffsetRate*2, rect.height+Mathf.Abs(rect.y)*fOffsetRate*2); }
	public		static Rect		GetZeroStartRect(Rect rect)																	{ return new Rect(0, 0, rect.width, rect.height);	}
	public		static Rect		GetLeftRect(Rect rect, float width)															{ return new Rect(rect.x, rect.y, width, rect.height); }
	public		static Rect		GetRightRect(Rect rect, float width)														{ return new Rect(rect.x+rect.width-width, rect.y, width, rect.height); }
	public		static Rect		GetInnerTopRect(Rect rectBase, int topMargin, int nHeight)									{ return new Rect(rectBase.x, topMargin+rectBase.y, rectBase.width, nHeight);	}
	public		static Rect		GetInnerBottomRect(Rect rectBase, int nHeight)												{ return new Rect(rectBase.x, rectBase.y+rectBase.height-nHeight, rectBase.width, nHeight);	}

	// -----------------------------------------------------------------------------------------------------------------------------------------------------------
	public static Vector2 ClampPoint(Rect rect, Vector2 point)
	{
		if (point.x < rect.xMin) point.x = rect.xMin;
		if (point.y < rect.yMin) point.y = rect.yMin;
		if (rect.xMax  < point.x) point.x = rect.xMax;
		if (rect.yMax  < point.y) point.y = rect.yMax;
		return point;
	}

	public static Vector3 ClampPoint(Rect rect, Vector3 point)
	{
		if (point.x < rect.xMin) point.x = rect.xMin;
		if (point.y < rect.yMin) point.y = rect.yMin;
		if (rect.xMax  < point.x) point.x = rect.xMax;
		if (rect.yMax  < point.y) point.y = rect.yMax;
		return point;
	}

	public static Rect ClampWindow(Rect popupRect)
	{
		if (popupRect.y < 0) popupRect.y = 0;
		if (Screen.width  < popupRect.xMax) popupRect.x -= popupRect.xMax - Screen.width;
		if (Screen.height < popupRect.yMax) popupRect.y -= popupRect.yMax - Screen.height;
		return popupRect;
	}

	public static bool GUIToggle(Rect pos, bool bToggle, GUIContent content, bool bEnable)
	{
		bool bOldEnable = GUI.enabled;
		if (bEnable == false)
			GUI.enabled	= false;
		bToggle = GUI.Toggle(pos, bToggle, content);
		GUI.enabled	= bOldEnable;
		return bToggle;
	}

	public static bool GUIButton(Rect pos, string name, bool bEnable)
	{
		bool bOldEnable = GUI.enabled;
		if (bEnable == false)
			GUI.enabled	= false;
		bool bClick = GUI.Button(pos, name);
		GUI.enabled	= bOldEnable;
		return bClick;
	}

	public static bool GUIButton(Rect pos, GUIContent content, bool bEnable)
	{
		bool bOldEnable = GUI.enabled;
		if (bEnable == false)
			GUI.enabled	= false;
		bool bClick = GUI.Button(pos, content);
		GUI.enabled	= bOldEnable;
		return bClick;
	}

	public static bool GUIButton(Rect pos, GUIContent content, GUIStyle style, bool bEnable)
	{
		bool bOldEnable = GUI.enabled;
		if (bEnable == false)
			GUI.enabled	= false;
		bool bClick = GUI.Button(pos, content, style);
		GUI.enabled	= bOldEnable;
		return bClick;
	}

	public static string GUITextField(Rect pos, string name, bool bEnable)
	{
		bool bOldEnable = GUI.enabled;
		if (bEnable == false)
			GUI.enabled	= false;
		string ret = GUI.TextField(pos, name);
		GUI.enabled	= bOldEnable;
		return ret;
	}

	// -----------------------------------------------------------------------------------------------------------------------------------------------------------
	public static bool GUIEnableBackup(bool newEnable)
	{
		m_GuiOldEnable	= GUI.enabled;
		GUI.enabled	= newEnable;
		return m_GuiOldEnable;
	}

	public static void GUIEnableRestore()
	{
		GUI.enabled	= m_GuiOldEnable;
	}

	public static Color GUIColorBackup(Color newColor)
	{
		m_GuiOldColor	= GUI.color;
		GUI.color	= newColor;
		return m_GuiOldColor;
	}

	public static void GUIColorRestore()
	{
		GUI.color	= m_GuiOldColor;
	}

	// -----------------------------------------------------------------------------------------------------------------------------------------------------------
	public static Vector2 GetGUIMousePosition() { return new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y); }

	public static float GetWorldPerScreenPixel(Vector3 worldPoint)
	{
		Camera cam = Camera.main;
		if (cam == null)
			return 0;
		Plane nearPlane = new Plane(cam.transform.forward, cam.transform.position);
		float dist = nearPlane.GetDistanceToPoint(worldPoint);
		float sample = 100;
		return Vector3.Distance(cam.ScreenToWorldPoint(new Vector3(Screen.width/2, Screen.height/2-sample/2, dist)), cam.ScreenToWorldPoint(new Vector3(Screen.width/2, Screen.height/2+sample/2, dist))) / sample;
	}

	public static Vector3 GetScreenToWorld(Vector3 targetWorld, Vector2 screenPos)
	{
		Camera cam = Camera.main;
		if (cam == null)
			return Vector3.zero;
		Plane nearPlane = new Plane(cam.transform.forward, cam.transform.position);
		float dist = nearPlane.GetDistanceToPoint(targetWorld);
		return cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, dist));
	}

	public static Vector3 GetWorldToScreen(Vector3 targetWorld)
	{
		Camera cam = Camera.main;
		if (cam == null)
			return Vector3.zero;
		return cam.WorldToScreenPoint(targetWorld);
	}

}
