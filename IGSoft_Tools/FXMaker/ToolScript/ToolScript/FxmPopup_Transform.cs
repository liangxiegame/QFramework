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

public class FxmPopup_Transform : FxmPopup
{
	// Attribute ------------------------------------------------------------------------
	// popup
	protected		Vector2				m_PopupScrollPos;
	protected		int					m_nWorldLocalSelIndex;

	protected		enum TRANS_TYPE		{POSITION=0, ROTATION, SCALE};
	protected		string[]			m_TransStrings			= {"Position", "Rotation", "Scale"};
	protected		static NcTransformTool	m_CopyTransform;
	protected		NcTransformTool			m_SaveTrans;

	protected		string[,]			m_strFloatInput;

	// Property -------------------------------------------------------------------------
	public override bool ShowPopupWindow(Object selObj)
	{
		if ((selObj is Transform))
		{
			m_PopupPosition		= FXMakerLayout.GetGUIMousePosition();
			m_SelectedTransform	= selObj as Transform;
			if (m_CopyTransform == null)
				m_CopyTransform = new NcTransformTool();
			SaveTransform();
			enabled	= true;
		} else {
			enabled	= false;
		}

		m_nWorldLocalSelIndex	= EditorPrefs.GetInt	("FxmPopup_Transform.m_nWorldLocalSelIndex"		, m_nWorldLocalSelIndex);
		InitFloatInput();

		return enabled;
	}

	void InitFloatInput()
	{
		m_strFloatInput = new string[3,3] {{null,null,null},{null,null,null},{null,null,null}};
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
		GUIStyle	styleBox	= GUI.skin.GetStyle("Transform_Box");

		if (UnfocusClose(baseRect, 0, 0, 0, 20))
			return;

		Rect		lineRect;
		Rect		groupRect;
		Rect		leftRect;
		Rect		rightRect;
		int			nGroup	= 3;
		int			nLine	= 2;
 		string		format	= "";
		
		baseRect	= FXMakerLayout.GetChildVerticalRect(baseRect, 0, 1, 0, 1);
		baseRect	= FXMakerLayout.GetOffsetRect(baseRect, -2);
		GUI.Box(baseRect, " ");
		// --------------------------------------------------------------------------------

// 		// World Position
// 		groupRect	= gNcLayout.GetInnerVerticalRect(baseRect, 15, 0, 5);
// 		groupRect	= gNcLayout.GetOffsetRect(groupRect, -5);
// 		leftRect	= gNcLayout.GetInnerHorizontalRect(groupRect, 6, 0, 2);
// 		rightRect	= gNcLayout.GetInnerHorizontalRect(groupRect, 6, 2, 4);
// 		GUI.Label(gNcLayout.GetInnerHorizontalRect(leftRect, 1, 0, 1), "World Position");
// 		GUI.Box(gNcLayout.GetInnerHorizontalRect(rightRect, 3, 0, 1), trans.position.x.ToString(format), styleBox);
// 		GUI.Box(gNcLayout.GetInnerHorizontalRect(rightRect, 3, 1, 1), trans.position.y.ToString(format), styleBox);
// 		GUI.Box(gNcLayout.GetInnerHorizontalRect(rightRect, 3, 2, 1), trans.position.z.ToString(format), styleBox);
// 		NgGUIDraw.DrawHorizontalLine(new Vector2(groupRect.x, groupRect.yMax+5), (int)groupRect.width, Color.black, 2, false);

		// Clipboard
		groupRect	= FXMakerLayout.GetInnerVerticalRect(baseRect, 35, 0, 8);
		groupRect	= FXMakerLayout.GetOffsetRect(groupRect, -5);
		leftRect	= FXMakerLayout.GetInnerHorizontalRect(groupRect, 6, 0, 2);
		rightRect	= FXMakerLayout.GetInnerHorizontalRect(groupRect, 6, 2, 4);
		for (int n = 0; n < nGroup; n++)
		{
			Vector3		vecValue	= Vector3.zero;
			TRANS_TYPE	nTrans		= (TRANS_TYPE)n;

			switch (nTrans)
			{
				case TRANS_TYPE.POSITION:	vecValue = m_CopyTransform.m_vecPos;				break;
				case TRANS_TYPE.ROTATION:	vecValue = m_CopyTransform.m_vecRotHint;			break;
				case TRANS_TYPE.SCALE:		vecValue = m_CopyTransform.m_vecScale;				break;
			}

			GUI.Label(FXMakerLayout.GetInnerVerticalRect(FXMakerLayout.GetInnerHorizontalRect(leftRect , 1, 0, 1), 3, n, 1), "Clipboard " + m_TransStrings[n]);
			GUI.Box  (FXMakerLayout.GetInnerVerticalRect(FXMakerLayout.GetInnerHorizontalRect(rightRect, 3, 0, 1), 3, n, 1), vecValue.x.ToString(format), styleBox);
			GUI.Box  (FXMakerLayout.GetInnerVerticalRect(FXMakerLayout.GetInnerHorizontalRect(rightRect, 3, 1, 1), 3, n, 1), vecValue.y.ToString(format), styleBox);
			GUI.Box  (FXMakerLayout.GetInnerVerticalRect(FXMakerLayout.GetInnerHorizontalRect(rightRect, 3, 2, 1), 3, n, 1), vecValue.z.ToString(format), styleBox);
		}
		groupRect	= FXMakerLayout.GetInnerVerticalRect(baseRect, 35, 8, 1);
		groupRect	= FXMakerLayout.GetOffsetRect(groupRect, -5);
		NgGUIDraw.DrawHorizontalLine(new Vector2(groupRect.x, groupRect.y-5), (int)groupRect.width, Color.grey, 2, false);

		// --------------------------------------------------------------------------------
		groupRect	= FXMakerLayout.GetInnerVerticalRect(baseRect, 35, 9, 3);
		groupRect	= FXMakerLayout.GetOffsetRect(groupRect, 5, 0, -5, 0);
		GUIContent[]	worldlocalCon = new GUIContent[2] {GetHelpContent("World"), GetHelpContent("Local")};
//		int nWorldLocalIndex = GUI.SelectionGrid(groupRect, m_nWorldLocalSelIndex, worldlocalCon, 2);
		int nWorldLocalIndex = FXMakerLayout.TooltipSelectionGrid(GetPopupRect(), groupRect, m_nWorldLocalSelIndex, worldlocalCon, 2);
		if (m_nWorldLocalSelIndex != nWorldLocalIndex)
		{
			m_nWorldLocalSelIndex = nWorldLocalIndex;
			EditorPrefs.SetInt("FxmPopup_Transform.m_nWorldLocalSelIndex", m_nWorldLocalSelIndex);
			InitFloatInput();
			SaveTransform();
		}

		// --------------------------------------------------------------------------------
		Rect editRect	= FXMakerLayout.GetInnerVerticalRect(baseRect, 35, 12, 18);
		for (int n = 0; n < nGroup; n++)
		{
			Vector3		vecValue	= Vector3.zero;
			TRANS_TYPE	nTrans		= (TRANS_TYPE)n;

			switch (nTrans)
			{
				case TRANS_TYPE.POSITION:	vecValue = m_SaveTrans.m_vecPos;		break;
				case TRANS_TYPE.ROTATION:	vecValue = m_SaveTrans.m_vecRotHint;	break;
				case TRANS_TYPE.SCALE:		vecValue = m_SaveTrans.m_vecScale;		break;
			}
			Vector3 oldValue = vecValue;

			for (int nn = 0; nn < 3; nn++)
				if (m_strFloatInput[n,nn] == null)
					m_strFloatInput[n,nn] = vecValue[nn].ToString();

			groupRect	= FXMakerLayout.GetInnerVerticalRect(editRect, nGroup, n, 1);
			groupRect	= FXMakerLayout.GetOffsetRect(groupRect, -3);
			leftRect	= FXMakerLayout.GetInnerHorizontalRect(groupRect, 6, 0, 2);
			rightRect	= FXMakerLayout.GetInnerHorizontalRect(groupRect, 6, 2, 4);

			// left
			// line 1
			lineRect	= FXMakerLayout.GetInnerVerticalRect(leftRect, nLine, 0, 1);
			GUI.Label(FXMakerLayout.GetInnerHorizontalRect(lineRect, 1, 0, 1), (m_nWorldLocalSelIndex==0 ? "World " : "Local ") + m_TransStrings[n]);
			// line 2
			lineRect	= FXMakerLayout.GetInnerVerticalRect(leftRect, nLine, 1, 1);
			if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(lineRect, 3, 0, 1), GetHelpContent("Reset")))
			{
				ResetTransform(nTrans);
				RecreateInstance();
				InitFloatInput();
				return;
			}
			if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(lineRect, 3, 1, 1), GetHelpContent("Copy")))
			{
				CopyTransform(nTrans);
				InitFloatInput();
				return;
			}
			if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(lineRect, 3, 2, 1), GetHelpContent("Paste")))
			{
				PasteTransform(nTrans);
				InitFloatInput();
				return;
			}

			// left, All Scale
			if (nTrans == TRANS_TYPE.SCALE)
			{
				lineRect	= FXMakerLayout.GetInnerVerticalRect(leftRect, nLine, 0, 1);
				if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(lineRect, 5, 3, 1), GetHelpContent(GetPrevName(nTrans).ToString())))
				{
					SetTransform(TRANS_TYPE.SCALE, GetPrevValue(nTrans, vecValue));
					RecreateInstance();
					InitFloatInput();
					return;
				}
				if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(lineRect, 5, 4, 1), GetHelpContent(GetNextName(nTrans).ToString())))
				{
					SetTransform(TRANS_TYPE.SCALE, GetNextValue(nTrans, vecValue));
					RecreateInstance();
					InitFloatInput();
					return;
				}
			}

			// right
			// line 1
			lineRect	= FXMakerLayout.GetInnerVerticalRect(rightRect, nLine, 0, 1);
			GUI.SetNextControlName("TextField");
			m_strFloatInput[n,0] = GUI.TextField(FXMakerLayout.GetInnerHorizontalRect(lineRect, 3, 0, 1), m_strFloatInput[n,0]);
			m_strFloatInput[n,0] = NgConvert.GetVaildFloatString(m_strFloatInput[n,0], ref vecValue.x);
			GUI.SetNextControlName("TextField");
			m_strFloatInput[n,1] = GUI.TextField(FXMakerLayout.GetInnerHorizontalRect(lineRect, 3, 1, 1), m_strFloatInput[n,1]);
			m_strFloatInput[n,1] = NgConvert.GetVaildFloatString(m_strFloatInput[n,1], ref vecValue.y);
			GUI.SetNextControlName("TextField");
			m_strFloatInput[n,2] = GUI.TextField(FXMakerLayout.GetInnerHorizontalRect(lineRect, 3, 2, 1), m_strFloatInput[n,2]);
			m_strFloatInput[n,2] = NgConvert.GetVaildFloatString(m_strFloatInput[n,2], ref vecValue.z);

// 			if (nTrans == TRANS_TYPE.ROTATION)
// 				vecValue = Vector3.zero;

			// line 2
			lineRect	= FXMakerLayout.GetInnerVerticalRect(rightRect, nLine, 1, 1);
			if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(lineRect, 3*2, 0, 1), GetHelpContent(GetPrevName(nTrans).ToString())))
				vecValue.x = GetPrevValue(nTrans, vecValue.x);
			if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(lineRect, 3*2, 1, 1), GetHelpContent(GetNextName(nTrans).ToString())))
				vecValue.x = GetNextValue(nTrans, vecValue.x);
			if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(lineRect, 3*2, 2, 1), GetHelpContent(GetPrevName(nTrans).ToString())))
				vecValue.y = GetPrevValue(nTrans, vecValue.y);
			if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(lineRect, 3*2, 3, 1), GetHelpContent(GetNextName(nTrans).ToString())))
				vecValue.y = GetNextValue(nTrans, vecValue.y);
			if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(lineRect, 3*2, 4, 1), GetHelpContent(GetPrevName(nTrans).ToString())))
				vecValue.z = GetPrevValue(nTrans, vecValue.z);
			if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(lineRect, 3*2, 5, 1), GetHelpContent(GetNextName(nTrans).ToString())))
				vecValue.z = GetNextValue(nTrans, vecValue.z);

			// Set
			if (vecValue != oldValue)
			{
				SetTransform(nTrans, vecValue);
				RecreateInstance();
				InitFloatInput();
				return;
			}
		}

		// --------------------------------------------------------------------------------
		// World, Local
		groupRect	= FXMakerLayout.GetInnerVerticalRect(baseRect, 35, 30, 1);
		groupRect	= FXMakerLayout.GetOffsetRect(groupRect, -5);
		NgGUIDraw.DrawHorizontalLine(new Vector2(groupRect.x, groupRect.y-6), (int)groupRect.width, Color.grey, 1, false);
		groupRect	= FXMakerLayout.GetInnerVerticalRect(baseRect, 35, 30, 5);
		groupRect	= FXMakerLayout.GetOffsetRect(groupRect, -5);
		leftRect	= FXMakerLayout.GetInnerHorizontalRect(groupRect, 6, 0, 2);
		rightRect	= FXMakerLayout.GetInnerHorizontalRect(groupRect, 6, 2, 4);

		GUI.Label (leftRect, m_nWorldLocalSelIndex==0 ? "World " : "Local ");
		if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(rightRect, 3, 0, 1), GetHelpContent("Reset")))
		{
			ResetTransform(TRANS_TYPE.POSITION);
			ResetTransform(TRANS_TYPE.ROTATION);
			ResetTransform(TRANS_TYPE.SCALE);
			RecreateInstance();
			InitFloatInput();
			return;
		}
		if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(rightRect, 3, 1, 1), GetHelpContent("Copy")))
		{
			CopyTransform(TRANS_TYPE.POSITION);
			CopyTransform(TRANS_TYPE.ROTATION);
			CopyTransform(TRANS_TYPE.SCALE);
			return;
		}
		if (GUI.Button(FXMakerLayout.GetInnerHorizontalRect(rightRect, 3, 2, 1), GetHelpContent("Paste")))
		{
			PasteTransform(TRANS_TYPE.POSITION);
			PasteTransform(TRANS_TYPE.ROTATION);
			PasteTransform(TRANS_TYPE.SCALE);
			RecreateInstance();
			InitFloatInput();
			return;
		}
		FXMakerMain.inst.SaveTooltip();
	}

	// ----------------------------------------------------------------------------------------------------------
	public override Rect GetPopupRect()
	{
		return GetPopupRectTop(400, 300);
	}

	float GetPrevName(TRANS_TYPE nGroupIndex)
	{
		switch (nGroupIndex)
		{
			case TRANS_TYPE.POSITION:	return -1;
			case TRANS_TYPE.ROTATION:	return -90;
			case TRANS_TYPE.SCALE:		return 0.5f;
		}
		return 0;
	}

	float GetNextName(TRANS_TYPE nGroupIndex)
	{
		switch (nGroupIndex)
		{
			case TRANS_TYPE.POSITION:	return 1;
			case TRANS_TYPE.ROTATION:	return 90;
			case TRANS_TYPE.SCALE:		return 2;
		}
		return 0;
	}

	float GetPrevValue(TRANS_TYPE nGroupIndex, float value)
	{
		bool bRightClick = Input.GetMouseButtonUp(1);
		if (bRightClick)
		{
			switch (nGroupIndex)
			{
				case TRANS_TYPE.POSITION:	return value-0.1f;
				case TRANS_TYPE.ROTATION:	return value-15;
				case TRANS_TYPE.SCALE:		return value*0.9f;
			}
		} else {
			switch (nGroupIndex)
			{
				case TRANS_TYPE.POSITION:	return value-1;
				case TRANS_TYPE.ROTATION:	return value-90;
				case TRANS_TYPE.SCALE:		return value*0.5f;
			}
		}
		return value;
	}

	float GetNextValue(TRANS_TYPE nGroupIndex, float value)
	{
		bool bRightClick = Input.GetMouseButtonUp(1);
		if (bRightClick)
		{
			switch (nGroupIndex)
			{
				case TRANS_TYPE.POSITION:	return value+0.1f;
				case TRANS_TYPE.ROTATION:	return value+15;
				case TRANS_TYPE.SCALE:		return value*1.1f;
			}
		} else {
			switch (nGroupIndex)
			{
				case TRANS_TYPE.POSITION:	return value+1;
				case TRANS_TYPE.ROTATION:	return value+90;
				case TRANS_TYPE.SCALE:		return value*2;
			}
		}
		return value;
	}

	Vector3 GetPrevValue(TRANS_TYPE nGroupIndex, Vector3 value)
	{
		value.x = GetPrevValue(nGroupIndex, value.x);
		value.y = GetPrevValue(nGroupIndex, value.y);
		value.z = GetPrevValue(nGroupIndex, value.z);
		return value;
	}

	Vector3 GetNextValue(TRANS_TYPE nGroupIndex, Vector3 value)
	{
		value.x = GetNextValue(nGroupIndex, value.x);
		value.y = GetNextValue(nGroupIndex, value.y);
		value.z = GetNextValue(nGroupIndex, value.z);
		return value;
	}

	void ResetTransform(TRANS_TYPE nGroupIndex)
	{
		switch (nGroupIndex)
		{
			case TRANS_TYPE.POSITION:	SetTransform(TRANS_TYPE.POSITION, Vector3.zero);	break;
			case TRANS_TYPE.ROTATION:	SetTransform(TRANS_TYPE.ROTATION, Vector3.zero);	break;
			case TRANS_TYPE.SCALE:		SetTransform(TRANS_TYPE.SCALE	, Vector3.one);		break;
		}
	}

	void CopyTransform(TRANS_TYPE nGroupIndex)
	{
		if (m_nWorldLocalSelIndex == 0)
		{
			switch (nGroupIndex)
			{
				case TRANS_TYPE.POSITION:	m_CopyTransform.m_vecPos		= m_SelectedTransform.position;					break;
				case TRANS_TYPE.ROTATION:	m_CopyTransform.m_vecRotHint	= m_SelectedTransform.rotation.eulerAngles;		break;
				case TRANS_TYPE.SCALE:		m_CopyTransform.m_vecScale		= m_SelectedTransform.lossyScale;				break;
			}
		} else {
			switch (nGroupIndex)
			{
				case TRANS_TYPE.POSITION:	m_CopyTransform.m_vecPos		= m_SelectedTransform.localPosition;				break;
				case TRANS_TYPE.ROTATION:	m_CopyTransform.m_vecRotHint	= m_SelectedTransform.localRotation.eulerAngles;	break;
				case TRANS_TYPE.SCALE:		m_CopyTransform.m_vecScale		= m_SelectedTransform.localScale;					break;
			}
		}
	}

	void PasteTransform(TRANS_TYPE nGroupIndex)
	{
		switch (nGroupIndex)
		{
			case TRANS_TYPE.POSITION:	SetTransform(TRANS_TYPE.POSITION, m_CopyTransform.m_vecPos);		break;
			case TRANS_TYPE.ROTATION:	SetTransform(TRANS_TYPE.ROTATION, m_CopyTransform.m_vecRotHint);	break;
			case TRANS_TYPE.SCALE:		SetTransform(TRANS_TYPE.SCALE	, m_CopyTransform.m_vecScale);		break;
		}
	}

	void SaveTransform()
	{
		if (m_SaveTrans == null)
			m_SaveTrans = new NcTransformTool();

		int nGroup = 3;

		for (int n = 0; n < nGroup; n++)
		{
			TRANS_TYPE	nTrans		= (TRANS_TYPE)n;

			if (m_nWorldLocalSelIndex == 0)
			{
				switch (nTrans)
				{
					case TRANS_TYPE.POSITION:	m_SaveTrans.m_vecPos	= m_SelectedTransform.position;			break;
					case TRANS_TYPE.ROTATION:	m_SaveTrans.m_vecRotHint= m_SelectedTransform.eulerAngles;		break;
					case TRANS_TYPE.SCALE:		m_SaveTrans.m_vecScale	= m_SelectedTransform.lossyScale;		break;
				}
			} else {
				switch (nTrans)
				{
					case TRANS_TYPE.POSITION:	m_SaveTrans.m_vecPos	= m_SelectedTransform.localPosition;	break;
					case TRANS_TYPE.ROTATION:	m_SaveTrans.m_vecRotHint= m_SelectedTransform.localEulerAngles;	break;
					case TRANS_TYPE.SCALE:		m_SaveTrans.m_vecScale	= m_SelectedTransform.localScale;		break;
				}
			}
		}
	}

	void SetTransform(TRANS_TYPE nGroupIndex, Vector3 vecValue)
	{
		// Set
		if (m_nWorldLocalSelIndex == 0)
		{
			// set Original
			switch (nGroupIndex)
			{
				case TRANS_TYPE.POSITION:	m_SelectedTransform.position	= vecValue; break;
				case TRANS_TYPE.ROTATION:	m_SelectedTransform.rotation	= Quaternion.Euler(vecValue); break;
//				case TRANS_TYPE.ROTATION:	m_SelectedTransform.rotation	= Quaternion.Euler(vecValue); break;
				case TRANS_TYPE.SCALE:		CopyWorldScale(vecValue, m_SelectedTransform); break;
			}

// Nan 문제로 recreate 방식으로 변경
// 			// set Instance
// 			List<FxmInfoIndexing>	indexComs = FxmInfoIndexing.FindInstanceIndexings(m_SelectedTransform);
// 			foreach (FxmInfoIndexing indexCom in indexComs)
// 			{
// 				switch (nGroupIndex)
// 				{
// 					case TRANS_TYPE.POSITION:	indexCom.transform.Translate(vecValue - m_SaveTrans.m_vecPos, Space.World);				break;
// 					case TRANS_TYPE.ROTATION:	indexCom.transform.rotation	*= Quaternion.Euler(vecValue - m_SaveTrans.m_vecRotHint);	break;
// //					case TRANS_TYPE.ROTATION:	indexCom.transform.rotation	*= Quaternion.Euler(vecValue);								break;
// 					case TRANS_TYPE.SCALE:		CopyWorldScale(vecValue - m_SaveTrans.m_vecScale, indexCom.transform);					break;
// 				}
// 			}
		} else {
			// set Original
			switch (nGroupIndex)
			{
				case TRANS_TYPE.POSITION:	m_SelectedTransform.localPosition	= vecValue;						break;
				case TRANS_TYPE.ROTATION:	m_SelectedTransform.localRotation	= Quaternion.Euler(vecValue);	break;
//				case TRANS_TYPE.ROTATION:	m_SelectedTransform.localRotation	*= Quaternion.Euler(vecValue);	break;
				case TRANS_TYPE.SCALE:		m_SelectedTransform.transform.localScale = vecValue;				break;
			}

// 			// set Instance
// 			List<FxmInfoIndexing>	indexComs = FxmInfoIndexing.FindInstanceIndexings(m_SelectedTransform);
// 			foreach (FxmInfoIndexing indexCom in indexComs)
// 			{
// 				switch (nGroupIndex)
// 				{
// 					case TRANS_TYPE.POSITION:	indexCom.transform.Translate(vecValue - m_SaveTrans.m_vecPos, Space.Self);					break;
// 					case TRANS_TYPE.ROTATION:	indexCom.transform.localRotation *= Quaternion.Euler(vecValue - m_SaveTrans.m_vecRotHint);	break;
// 					case TRANS_TYPE.SCALE:		indexCom.transform.localScale = vecValue - m_SaveTrans.m_vecScale;							break;
// 				}
// 			}
		}

		// Last Update
		switch (nGroupIndex)
		{
			case TRANS_TYPE.POSITION:	m_SaveTrans.m_vecPos		= vecValue;		break;
			case TRANS_TYPE.ROTATION:	m_SaveTrans.m_vecRotHint	= vecValue;		break;
			case TRANS_TYPE.SCALE:		m_SaveTrans.m_vecScale		= vecValue;		break;
		}
	}

	void CopyWorldScale(Vector3 srcLossy, Transform tarTrans)
	{
		tarTrans.localScale = Vector3.one;
		tarTrans.localScale = new Vector3((tarTrans.lossyScale.x == 0 ? 0 : srcLossy.x / (tarTrans.lossyScale.x)),
										  (tarTrans.lossyScale.y == 0 ? 0 : srcLossy.y / (tarTrans.lossyScale.y)),
										  (tarTrans.lossyScale.z == 0 ? 0 : srcLossy.z / (tarTrans.lossyScale.z)));
	}

	Vector3 GetLocalEulerAnglesHint(Transform trans)
	{
		return (Vector3)NgSerialized.GetPropertyValue(new SerializedObject(trans), "m_LocalEulerAnglesHint");
	}

	void SetLocalEulerAnglesHint(Transform trans, Vector3 localEulerAngle)
	{
		NgSerialized.SetPropertyValue(new SerializedObject(trans), "m_LocalEulerAnglesHint", localEulerAngle, true);
	}

	// Control Function -----------------------------------------------------------------
	void RecreateInstance()
	{
		FXMakerMain.inst.CreateCurrentInstanceEffect(true);
	}

	// Event Function -------------------------------------------------------------------


	// -------------------------------------------------------------------------------------------
	GUIContent GetHelpContent(string text)
	{
		return FXMakerTooltip.GetHcPopup_Transform(text);
	}
}
#endif
