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

public class FXMakerGizmo : MonoBehaviour
{
	// Attribute ------------------------------------------------------------------------
	public		static			FXMakerGizmo		inst;

	// const
	protected	const float		m_fActiveDist			= 10;
	protected	enum AXIS		{NONE, X, Y, Z, A};
	protected	enum GIZMO_TYPE	{HAND, POSITION, ROTATION, SCALE, NONE};


	public		Texture			m_iconHandActive;
	public		Texture			m_iconPosActive;
	public		Texture			m_iconRotActive;
	public		Texture			m_iconScaleActive;
	public		Texture			m_iconNoneActive;
	public		Texture			m_iconHandNormal;
	public		Texture			m_iconPosNormal;
	public		Texture			m_iconRotNormal;
	public		Texture			m_iconScaleNormal;
	public		Texture			m_iconNoneNormal;

	public		float			m_fCapSizeRatio			= 1.0f;
	public		float			m_fWorldLineRatio		= 1.0f;
	public		float			m_fLocalLineRatio		= 0.5f;
	protected	float			m_fHandAlpha			= 0.6f;

	protected	Transform		m_SelectedTransform;
	protected	float			m_fGizmoLinePerScreen	= 0.1f;		// per screen

	protected	int				m_nGizmoTypeIndex		= 1;
	protected	string			m_GizmoTooltip			= "";
	protected	bool			m_bActiveLocal			= true;
	protected	AXIS			m_nActiveAxis			= AXIS.NONE;
	protected	Vector3			m_OldMousePos;
	protected	bool			m_bClick;
	protected	Vector3			m_OldOriScale;
	protected	Vector3			m_OldInsScale;
	protected	Vector3			m_SaveRotate;
	protected	float			m_OldGizmoLineLength;
	protected	bool			m_bUnityGizmos;
	protected	bool			m_bWorldSpace			= true;
	protected	bool			m_bFixedSide			= false;

	public		GameObject		m_GridXYGameObject;
	public		GameObject		m_GridXZGameObject;
	public		GameObject		m_GridYZGameObject;
	protected	bool			m_bGridXY				= false;
	protected	bool			m_bGridXZ				= true;
	protected	bool			m_bGridYZ				= false;
	protected	bool			m_bBackground			= true;
	protected	bool			m_bGM					= false;

	protected	bool			m_bShowGameObjectOptionGray			= false;
	protected	bool			m_bShowGameObjectOptionBounds		= false;
	protected	bool			m_bShowGameObjectOptionWireframe	= false;


	protected	GUIContent[]	m_HcGizmoContents;

	// -------------------------------------------------------------------------------------------
	void LoadPrefs()
	{
		if (FXMakerLayout.m_bDevelopPrefs == false)
		{
			m_nGizmoTypeIndex	= EditorPrefs.GetInt("FXMakerGizmo.m_nGizmoTypeIndex", m_nGizmoTypeIndex);
			m_bWorldSpace		= EditorPrefs.GetBool("FXMakerGizmo.m_bWorldSpace", m_bWorldSpace);
			m_bFixedSide		= EditorPrefs.GetBool("FXMakerGizmo.m_bFixedSide", m_bFixedSide);

			m_bGridXY		= EditorPrefs.GetBool("FXMakerGizmo.m_bGridXY", m_bGridXY);
			m_bGridXZ		= EditorPrefs.GetBool("FXMakerGizmo.m_bGridXZ", m_bGridXZ);
			m_bGridYZ		= EditorPrefs.GetBool("FXMakerGizmo.m_bGridYZ", m_bGridYZ);
			m_bBackground	= EditorPrefs.GetBool("FXMakerGizmo.m_bBackground", m_bBackground);
			m_bGM			= EditorPrefs.GetBool("FXMakerGizmo.m_bGM", m_bGM);

			m_bShowGameObjectOptionGray		= EditorPrefs.GetBool("FXMakerGizmo.m_bShowGameObjectOptionGray"		, m_bShowGameObjectOptionGray);
			m_bShowGameObjectOptionBounds	= EditorPrefs.GetBool("FXMakerGizmo.m_bShowGameObjectOptionBounds"		, m_bShowGameObjectOptionBounds);
			m_bShowGameObjectOptionWireframe= EditorPrefs.GetBool("FXMakerGizmo.m_bShowGameObjectOptionWireframe"	, m_bShowGameObjectOptionWireframe);

			UpdateGrid(true);
		}
		SetGizmoType(m_nGizmoTypeIndex);
	}

	// -------------------------------------------------------------------------------------------
	FXMakerGizmo()
	{
		inst = this;
	}

	// -------------------------------------------------------------------------------------------
	public bool IsActiveAxis()
	{
		return m_nActiveAxis != AXIS.NONE;
	}

	bool IsLocalSpace()
	{
		return (!m_bWorldSpace || ((GIZMO_TYPE)m_nGizmoTypeIndex) == GIZMO_TYPE.SCALE);
	}

	public bool IsGrayscale()
	{
		return (m_bShowGameObjectOptionGray);
	}

	public bool IsBoundsBox()
	{
		return (m_bShowGameObjectOptionBounds);
	}

	public bool IsWireframe()
	{
		return (m_bShowGameObjectOptionWireframe);
	}

	public bool IsGridMove()
	{
		return m_bGM;
	}

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
		NgAssembly.SetGizmosVisible(false);
	}

	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{

		m_bUnityGizmos = false;

		// check shotkey
		if (GUI.GetNameOfFocusedControl() != "TextField")
		{
			if (Input.GetKeyDown(KeyCode.Q)) SetGizmoType(0);
			if (Input.GetKeyDown(KeyCode.W)) SetGizmoType(1);
			if (Input.GetKeyDown(KeyCode.E)) SetGizmoType(2);
			if (Input.GetKeyDown(KeyCode.R)) SetGizmoType(3);
			if (Input.GetKeyDown(KeyCode.T)) SetGizmoType(4);
			if (Input.GetKeyDown(KeyCode.Y)) m_bWorldSpace = !m_bWorldSpace;
		}

		GameObject selObj = FXMakerMain.inst.GetFXMakerHierarchy().GetSelectedGameObject();
		if (selObj == null)
			return;
		m_SelectedTransform	= selObj.transform;

		// check click
		if (m_nActiveAxis != AXIS.NONE)
		{
			if (Input.GetMouseButtonDown(0))
			{
				m_OldMousePos			= Input.mousePosition;
				m_OldOriScale			= m_SelectedTransform.localScale;
				m_OldInsScale			= (GetInstanceObject() == null ? Vector3.one : GetInstanceObject().transform.localScale);
				m_OldGizmoLineLength	= NgLayout.GetWorldPerScreenPixel(FXMakerEffect.inst.m_CurrentEffectRoot.transform.position) * (Screen.width * m_fGizmoLinePerScreen * m_fWorldLineRatio);
				m_SaveRotate			= Vector3.zero;
				m_bClick				= true;

				if (m_nGizmoTypeIndex == (int)GIZMO_TYPE.POSITION || m_nGizmoTypeIndex == (int)GIZMO_TYPE.ROTATION || m_nGizmoTypeIndex == (int)GIZMO_TYPE.SCALE)
					FXMakerEffect.inst.SetChangePrefab();
			}
			if (m_bClick && Input.GetMouseButton(0))
			{
				Vector3 currMousePos	= Input.mousePosition;
				Vector3 prevWorldPos	= NgLayout.GetScreenToWorld(m_SelectedTransform.transform.position, m_OldMousePos);
				Vector3 currWorldPos	= NgLayout.GetScreenToWorld(m_SelectedTransform.transform.position, currMousePos);
				
				if (IsLocalSpace())
				{
					Transform	tempTrans	= GetTempTransform();
					tempTrans.rotation		= m_SelectedTransform.rotation;
					currWorldPos = tempTrans.InverseTransformPoint(currWorldPos);
					prevWorldPos = tempTrans.InverseTransformPoint(prevWorldPos);
				}

				switch (((GIZMO_TYPE)m_nGizmoTypeIndex))
				{
					case GIZMO_TYPE.POSITION:
						{
							switch (m_nActiveAxis)
							{
								case AXIS.X: AddTranslate(currWorldPos.x - prevWorldPos.x, 0, 0, m_bActiveLocal); break;
								case AXIS.Y: AddTranslate(0, currWorldPos.y - prevWorldPos.y, 0, m_bActiveLocal); break;
								case AXIS.Z:
									{
										if (m_bFixedSide)
										{
											AddTranslate(currWorldPos.x - prevWorldPos.x, 0, 0, m_bActiveLocal);
											AddTranslate(0, currWorldPos.y - prevWorldPos.y, 0, m_bActiveLocal);
										} else {
											AddTranslate(0, 0, currWorldPos.z - prevWorldPos.z, m_bActiveLocal);
										}
										break;
									}
							}
							break;
						}
					case GIZMO_TYPE.ROTATION:
						{
							switch (m_bFixedSide ? AXIS.Z : m_nActiveAxis)
							{
								case AXIS.X: AddRotation(currWorldPos.z - prevWorldPos.z, 0, 0, m_bActiveLocal); break;
								case AXIS.Y: AddRotation(0, currWorldPos.x - prevWorldPos.x, 0, m_bActiveLocal); break;
								case AXIS.Z: AddRotation(0, 0, prevWorldPos.x - currWorldPos.x, m_bActiveLocal); break;
							}
							break;
						}
					case GIZMO_TYPE.SCALE:
						{
							float	fScaleDist;
							switch (m_nActiveAxis)
							{
								case AXIS.X: fScaleDist = currWorldPos.x - prevWorldPos.x;	AddScale(fScaleDist/m_OldGizmoLineLength, 0, 0);	break;
								case AXIS.Y: fScaleDist = currWorldPos.y - prevWorldPos.y;	AddScale(0, fScaleDist/m_OldGizmoLineLength, 0);	break;
								case AXIS.Z: fScaleDist = currWorldPos.z - prevWorldPos.z;	AddScale(0, 0, fScaleDist/m_OldGizmoLineLength);	break;
								case AXIS.A:
									{
										fScaleDist = (currMousePos.x - m_OldMousePos.x) * NgLayout.GetWorldPerScreenPixel(selObj.transform.position);
										if (m_bFixedSide)
											 AddScale(fScaleDist/m_OldGizmoLineLength, fScaleDist/m_OldGizmoLineLength, 0);
										else AddScale(fScaleDist/m_OldGizmoLineLength, fScaleDist/m_OldGizmoLineLength, fScaleDist/m_OldGizmoLineLength);
										break;
									}
							}
							break;
						}
				}
				m_OldMousePos = currMousePos;
			}
			if (Input.GetMouseButtonUp(0))
			{
				m_nActiveAxis	= AXIS.NONE;
				m_bClick		= false;
				if (IsGridMove() /*&& FXMakerOption.inst.m_bGizmoGridMoveUnit*/)
					UpdateGridMove();
			}
			FXMakerMain.inst.GetFXMakerMouse().SetHandControl(m_bClick == false);
		}
	}

	public void UpdateGridMove()
	{
		if (m_SelectedTransform != null)
		{
			Vector3 pos = m_SelectedTransform.position;
			pos.x = GridMovePos(pos.x);
			pos.y = GridMovePos(pos.y);
			pos.z = GridMovePos(pos.z);
			MoveTranslate(pos);
// 			FXMakerMain.inst.CreateCurrentInstanceEffect(true);
		}
	}

	float GridMovePos(float pos)
	{
		if (Mathf.Approximately(pos, 0))
			return 0;
		int		nCount	= 0;
		float	fSign	= (0 < pos ? 1 : -1);

		pos = Mathf.Abs(pos);
		while (true)
		{
			if (FXMakerOption.inst.m_fGizmoGridMoveUnit * (nCount+1) < pos)
				nCount++;
			else break;
		}
		if (FXMakerOption.inst.m_fGizmoGridMoveUnit / 2 < pos - FXMakerOption.inst.m_fGizmoGridMoveUnit * nCount)
			nCount++;
		pos = FXMakerOption.inst.m_fGizmoGridMoveUnit * nCount;
		return pos * fSign;
	}

	// ==========================================================================================================
	void OnDrawGizmos()
	{
		m_bUnityGizmos = true;

		if (enabled == false)
			return;

		if (Camera.main == null)
			return;

		if (m_SelectedTransform == null)
			return;

		DrawOriginalGizmo(m_SelectedTransform, IsLocalSpace());

 		FxmInfoIndexing com = FxmInfoIndexing.FindInstanceIndexing(m_SelectedTransform, false);
 		if (com != null && !IsLocalSpace())
 			DrawLocalGizmo(com.transform);
	}

	bool DrawOriginalGizmo(Transform selTrans, bool bLocal)
	{
		float		fLineLen	= NgLayout.GetWorldPerScreenPixel(FXMakerEffect.inst.m_CurrentEffectRoot.transform.position) * (Screen.width * m_fGizmoLinePerScreen * m_fWorldLineRatio);
		float		fCapSize	= fLineLen * 0.07f * m_fCapSizeRatio;
		Vector3		cubeSize	= Vector3.one * fCapSize;
		bool		bSelected	= false;

		// check active
		if ((((GIZMO_TYPE)m_nGizmoTypeIndex) != GIZMO_TYPE.HAND && ((GIZMO_TYPE)m_nGizmoTypeIndex) != GIZMO_TYPE.NONE) && m_bClick == false)
		{
			AXIS nActiveAxis = AXIS.NONE;
			float	fStartClickLen	= fLineLen/4.0f;

			if (HandleUtility.DistanceToLine(GetPosition(selTrans) + GetDirect(selTrans, AXIS.X, bLocal) * fStartClickLen, GetPosition(selTrans) + GetDirect(selTrans, AXIS.X, bLocal) * fLineLen) < m_fActiveDist)
				nActiveAxis = AXIS.X;
			if (HandleUtility.DistanceToLine(GetPosition(selTrans) + GetDirect(selTrans, AXIS.Y, bLocal) * fStartClickLen, GetPosition(selTrans) + GetDirect(selTrans, AXIS.Y, bLocal) * fLineLen) < m_fActiveDist)
				nActiveAxis = AXIS.Y;
			if (HandleUtility.DistanceToLine(GetPosition(selTrans) + GetDirect(selTrans, AXIS.Z, bLocal) * fStartClickLen, GetPosition(selTrans) + GetDirect(selTrans, AXIS.Z, bLocal) * fLineLen) < m_fActiveDist)
				nActiveAxis = AXIS.Z;
			if (((GIZMO_TYPE)m_nGizmoTypeIndex) == GIZMO_TYPE.SCALE)
				if (HandleUtility.DistanceToLine(GetPosition(selTrans), GetPosition(selTrans)) < m_fActiveDist)
					nActiveAxis = AXIS.A;

			if (nActiveAxis != AXIS.NONE)
			{
				m_nActiveAxis	= nActiveAxis;
				m_bActiveLocal	= bLocal;
				bSelected		= true;
			} else {
				m_nActiveAxis	= AXIS.NONE;
			}
		}

		// draw Selected gizmo
		DrawGizmoAxis(AXIS.X, bLocal, fLineLen, fCapSize);
		DrawGizmoAxis(AXIS.Y, bLocal, fLineLen, fCapSize);
		DrawGizmoAxis(AXIS.Z, bLocal, fLineLen, fCapSize);

		// World Center Sphere
		Gizmos.color = Color.yellow;
		if (((GIZMO_TYPE)m_nGizmoTypeIndex) == GIZMO_TYPE.HAND)
			Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, m_fHandAlpha);
		Gizmos.DrawSphere(Vector3.zero, cubeSize.x/2.0f);

		// selected center
		if (((GIZMO_TYPE)m_nGizmoTypeIndex) == GIZMO_TYPE.SCALE)
		{
			if (m_nActiveAxis == AXIS.A && m_bActiveLocal == bLocal)
				 Gizmos.color = Color.white;
			else Gizmos.color = Color.cyan;
			Gizmos.DrawCube(GetPosition(selTrans), cubeSize);
		}

		// Gizmo tooltip, Camera Pos
		if (IsLocalSpace())
			 m_GizmoTooltip = Camera.main.transform.position +  " , GizmosLength = " + fLineLen.ToString("0.000") + " , long=Local";
		else m_GizmoTooltip = Camera.main.transform.position +  " , GizmosLength = " + fLineLen.ToString("0.000") + " , long=World, shot=Local";
		FXMakerMain.inst.SetEmptyTooltip(m_GizmoTooltip);

		return bSelected;
	}

	void DrawGizmoAxis(AXIS	drawAxis, bool bLocal, float fLineLen, float fCapSize)
	{
		Transform	selTrans	= m_SelectedTransform.transform;
		Vector3		vecEuler	= Vector3.zero;
		Vector3		vecDirect	= Vector3.zero;

		// set color
		if (m_nActiveAxis != drawAxis || m_bActiveLocal != bLocal)
		{
			switch (drawAxis)
			{
				case AXIS.X: Handles.color = Color.red;			break;
				case AXIS.Y: Handles.color = Color.green;		break;
				case AXIS.Z: Handles.color = Color.blue;		break;
			}
		} else Handles.color = Color.white;
		if (((GIZMO_TYPE)m_nGizmoTypeIndex) == GIZMO_TYPE.HAND)
			Handles.color = new Color(Handles.color.r, Handles.color.g, Handles.color.b, m_fHandAlpha);

		// axis color
		Gizmos.color = Handles.color;

		vecDirect	= GetDirect(selTrans, drawAxis, bLocal);
		vecEuler	= GetEuler(selTrans, drawAxis, bLocal);

		// draw line
//		Handles.DrawLine(GetPosition(selTrans), GetPosition(selTrans) + vecDirect * fLineLen);
		if (((GIZMO_TYPE)m_nGizmoTypeIndex) == GIZMO_TYPE.SCALE)
		{
			float	scale = 1;
			if (m_bClick)
			{
				switch (drawAxis)
				{
					case AXIS.X: scale = selTrans.localScale.x / m_OldOriScale.x; break;
					case AXIS.Y: scale = selTrans.localScale.y / m_OldOriScale.y; break;
					case AXIS.Z: scale = selTrans.localScale.z / m_OldOriScale.z; break;

				}
			}

			Gizmos.DrawRay(GetPosition(selTrans), vecDirect * fLineLen * scale);
		} else {
			Gizmos.DrawRay(GetPosition(selTrans), vecDirect * fLineLen);
		}

		// draw cap
		switch (((GIZMO_TYPE)m_nGizmoTypeIndex))
		{
			case GIZMO_TYPE.POSITION:	Handles.ConeCap(0, GetPosition(selTrans) + vecDirect * fLineLen, Quaternion.Euler(vecEuler), fCapSize*1.6f);		break;
			case GIZMO_TYPE.ROTATION:	Handles.CylinderCap(0, GetPosition(selTrans) + vecDirect * fLineLen, Quaternion.Euler(vecEuler), fCapSize*2f);		break;
			case GIZMO_TYPE.SCALE:		Handles.CubeCap(0, GetPosition(selTrans) + vecDirect * fLineLen, Quaternion.Euler(vecEuler), fCapSize);				break;
		}
		
// 		// draw rot line
// 		if (((GIZMO_TYPE)m_nGizmoTypeIndex) == GIZMO_TYPE.ROTATION)
// 		{
// 			if (m_bClick)
// 			{
//				transform.position	= GetPosition(selTrans);
//				transform.rotation	= Quaternion.Euler(m_SaveRotate);
// 
// 				switch (drawAxis)
// 				{
// 					case AXIS.X: Gizmos.color = Color.red;		Gizmos.DrawLine(GetPosition(selTrans), selTrans.right		* fLineLen * 1.3f);	break;
// 					case AXIS.Y: Gizmos.color = Color.green;	Gizmos.DrawLine(GetPosition(selTrans), selTrans.up			* fLineLen * 1.3f);	break;
// 					case AXIS.Z: Gizmos.color = Color.blue;		Gizmos.DrawLine(GetPosition(selTrans), selTrans.forward		* fLineLen * 1.3f);	break;
// 				}
// 				if (m_nActiveAxis != drawAxis && m_bActiveLocal == bLocal)
// 				{
// 					Gizmos.color = Color.white;
// 					switch (drawAxis)
// 					{
// 						case AXIS.X: Gizmos.DrawLine(transform.position, transform.right	* fLineLen * 2);	break;
// 						case AXIS.Y: Gizmos.DrawLine(transform.position, transform.up		* fLineLen * 2);	break;
// 						case AXIS.Z: Gizmos.DrawLine(transform.position, transform.forward	* fLineLen * 2);	break;
// //						case AXIS.X: Handles.DrawSolidArc(transform.position, transform.right, -transform.up, m_SaveRotate.z, fCapSize*10); break;
// 					}
// 				}
// 			}
// 		}
	}

	void DrawLocalGizmo(Transform selTrans)
	{
		float		fLineLen	= NgLayout.GetWorldPerScreenPixel(FXMakerEffect.inst.m_CurrentEffectRoot.transform.position) * (Screen.width * m_fGizmoLinePerScreen * m_fLocalLineRatio);

		// draw Selected gizmo
		DrawGizmoAxis(selTrans, AXIS.X, fLineLen, true);
		DrawGizmoAxis(selTrans, AXIS.Y, fLineLen, true);
		DrawGizmoAxis(selTrans, AXIS.Z, fLineLen, true);
	}

	void DrawGizmoAxis(Transform selTrans, AXIS drawAxis, float fLineLen, bool bDrawLocalAxis)
	{
		Vector3		vecDirect	= Vector3.zero;
		Vector3		vecPos		= Vector3.zero;

		if (Camera.main == null)
			return;

		// set color
		switch (drawAxis)
		{
			case AXIS.X: Handles.color = Color.red;			break;
			case AXIS.Y: Handles.color = Color.green;		break;
			case AXIS.Z: Handles.color = Color.blue;		break;
		}

		// set alpha
		if (((GIZMO_TYPE)m_nGizmoTypeIndex) == GIZMO_TYPE.HAND)
			Handles.color = new Color(Handles.color.r, Handles.color.g, Handles.color.b, m_fHandAlpha);

		// axis color
		Gizmos.color = Handles.color;

		// draw line
		vecPos		= GetPosition(selTrans);
		vecDirect	= GetDirect(selTrans, drawAxis, bDrawLocalAxis);
		Gizmos.DrawRay(vecPos, vecDirect * fLineLen);
		float		fCapSize	= fLineLen * 0.06f * m_fCapSizeRatio;
		Gizmos.DrawSphere(vecPos + vecDirect * fLineLen, fCapSize);
//		Handles.SphereCap(0, vecPos + vecDirect * fLineLen, Quaternion.identity, fCapSize*2);
	}

	void DrawGuiGizmo()
	{
		float		fLineLen	= NgLayout.GetWorldPerScreenPixel(FXMakerEffect.inst.m_CurrentEffectRoot.transform.position) * (Screen.width * m_fGizmoLinePerScreen * m_fLocalLineRatio);

		// draw Selected gizmo
		DrawGuiGizmoAxis(AXIS.Z, fLineLen, false);
		DrawGuiGizmoAxis(AXIS.X, fLineLen, false);
		DrawGuiGizmoAxis(AXIS.Y, fLineLen, false);
	}

	void DrawGuiGizmoAxis(AXIS drawAxis, float fLineLen, bool bDrawLocalAxis)
	{
		Vector3		vecDirect	= Vector3.zero;
		Vector3		vecPos		= Vector3.zero;
		Color		color		= Color.white;

		if (Camera.main == null)
			return;

		// set color
		switch (drawAxis)
		{
			case AXIS.X: color = Color.red;			break;
			case AXIS.Y: color = Color.green;		break;
			case AXIS.Z: color = Color.blue;		break;
		}

		// draw line
		float		fDist		= FXMakerMain.inst.GetFXMakerMouse().m_fDistance;
		Rect		baseRect	= FXMakerLayout.GetEffectHierarchyRect();
		Vector3		guiCenPos	= new Vector3(Screen.width/2, Screen.height/2, fDist);
		Vector3		guiPos		= new Vector3(baseRect.x-50, baseRect.y+30) - guiCenPos;
		Transform	tempTrans	= GetTempTransform();

		tempTrans.position	= Camera.main.ScreenToWorldPoint(guiCenPos);
		vecDirect			= tempTrans.position + GetDirect(tempTrans, drawAxis, true) * fDist/20;
		vecPos				= Camera.main.WorldToScreenPoint(vecDirect);
		vecPos.y = Screen.height - vecPos.y;
 		NgGUIDraw.DrawLine(guiPos+guiCenPos, guiPos+vecPos, color, 3, true);
	}

	// ==========================================================================================================
	void AddTranslate(float x, float y, float z, bool bLocal)
	{
// 		Debug.Log(bLocal);
// 		Debug.Log(x);
		List<FxmInfoIndexing>	indexComs = FxmInfoIndexing.FindInstanceIndexings(m_SelectedTransform.transform, false);
		foreach (FxmInfoIndexing indexCom in indexComs)
			indexCom.transform.Translate(x, y, z, (bLocal ? Space.Self : Space.World));
		m_SelectedTransform.Translate(x, y, z, (bLocal ? Space.Self : Space.World));
	}

	void MoveTranslate(Vector3 moveTo)
	{
// 		Debug.Log(bLocal);
// 		Debug.Log(x);
		List<FxmInfoIndexing>	indexComs = FxmInfoIndexing.FindInstanceIndexings(m_SelectedTransform.transform, false);
		foreach (FxmInfoIndexing indexCom in indexComs)
			indexCom.transform.position = moveTo;
		m_SelectedTransform.position = moveTo;
	}

	void AddRotation(float x, float y, float z, bool bLocal)
	{
		float scale =  NgLayout.GetWorldPerScreenPixel(m_SelectedTransform.transform.position);
		m_SaveRotate += new Vector3(x/scale, y/scale, z/scale);
		List<FxmInfoIndexing>	indexComs = FxmInfoIndexing.FindInstanceIndexings(m_SelectedTransform.transform, false);
		foreach (FxmInfoIndexing indexCom in indexComs)
			indexCom.transform.Rotate(x/scale, y/scale, z/scale, (bLocal ? Space.Self : Space.World));
		m_SelectedTransform.Rotate(x/scale, y/scale, z/scale, (bLocal ? Space.Self : Space.World));
	}

	void AddScale(float x, float y, float z)
	{
		List<FxmInfoIndexing>	indexComs = FxmInfoIndexing.FindInstanceIndexings(m_SelectedTransform.transform, false);
		foreach (FxmInfoIndexing indexCom in indexComs)
			indexCom.transform.localScale += new Vector3(m_OldInsScale.x*x, m_OldInsScale.y*y, m_OldInsScale.z*z);
		m_SelectedTransform.localScale += new Vector3(m_OldOriScale.x*x, m_OldOriScale.y*y, m_OldOriScale.z*z);
	}

	Transform GetInstanceObject()
	{
		List<FxmInfoIndexing>	indexComs = FxmInfoIndexing.FindInstanceIndexings(m_SelectedTransform.transform, false);
		foreach (FxmInfoIndexing indexCom in indexComs)
			return indexCom.transform;
		return null;
	}

	Vector3 GetPosition(Transform trans)
	{
		return trans.position;
	}

	Quaternion GetRotation(Transform trans)
	{
		return trans.rotation;
	}

	Vector3 GetScale(Transform trans)
	{
		return trans.lossyScale;
	}

	// ==========================================================================================================
	public void OnGUIGizmo()
	{
		Rect		baseRect	= FXMakerLayout.GetMenuGizmoRect();
		Rect		boxRect;
		Rect		subRect;
		GUIStyle	toolStyle	= GUI.skin.GetStyle("GizmoToolbar");
		int			nColCount	= 15;

		// -----------------------------------------------------------------------------------------------------------------------------------
		GUI.Box(FXMakerLayout.GetInnerHorizontalRect(baseRect, nColCount, 0, 7), new GUIContent(" ", " "));
//		m_nGizmoTypeIndex	= GUI.SelectionGrid(FXMakerLayout.GetInnerHorizontalRect(baseRect, nColCount, 0, 5), m_nGizmoTypeIndex, GetGizmoContents(), 5, toolStyle);
		m_nGizmoTypeIndex	= FXMakerLayout.TooltipSelectionGrid(baseRect, FXMakerLayout.GetInnerHorizontalRect(baseRect, nColCount, 0, 5), m_nGizmoTypeIndex, GetGizmoContents(), 5, toolStyle);

		// -----------------------------------------------------------------------------------------------------------------------------------
		boxRect				= FXMakerLayout.GetInnerHorizontalRect(baseRect, nColCount, 5, 2);
		subRect				= FXMakerLayout.GetOffsetRect(boxRect, 4, 0, -2, 0);
		m_bWorldSpace		= GUI.Toggle(new Rect(subRect.x, subRect.y-3, subRect.width, subRect.height-5), m_bWorldSpace, FXMakerTooltip.GetHcEffectGizmo("World"));
		m_bFixedSide		= GUI.Toggle(new Rect(subRect.x, subRect.y+10, subRect.width, subRect.height-6), m_bFixedSide, FXMakerTooltip.GetHcEffectGizmo("Fixed-Z"));

		// -----------------------------------------------------------------------------------------------------------------------------------
		boxRect			= FXMakerLayout.GetInnerHorizontalRect(baseRect, nColCount, 7, 4);
		boxRect			= FXMakerLayout.GetOffsetRect(boxRect, 2, 0, -2, 0);
		GUI.Box(boxRect, new GUIContent(" ", " "));
		subRect			= FXMakerLayout.GetOffsetRect(boxRect, 1, -3, -4, -8);
		m_bGridXY		= GUI.Toggle(FXMakerLayout.GetInnerHorizontalRect(subRect, 3, 0, 1), m_bGridXY, FXMakerTooltip.GetHcEffectGizmo("XY"));
		m_bGridXZ		= GUI.Toggle(FXMakerLayout.GetInnerHorizontalRect(subRect, 3, 1, 1), m_bGridXZ, FXMakerTooltip.GetHcEffectGizmo("XZ"));
		m_bGridYZ		= GUI.Toggle(FXMakerLayout.GetInnerHorizontalRect(subRect, 3, 2, 1), m_bGridYZ, FXMakerTooltip.GetHcEffectGizmo("YZ"));

		subRect			= FXMakerLayout.GetOffsetRect(boxRect, 1, 10, -4, 3);
		m_bBackground	= GUI.Toggle(FXMakerLayout.GetInnerHorizontalRect(subRect, 3, 0, 2), m_bBackground, FXMakerTooltip.GetHcEffectGizmo("Background"));
 		m_bGM			= GUI.Toggle(FXMakerLayout.GetInnerHorizontalRect(subRect, 3, 2, 1), m_bGM, FXMakerTooltip.GetHcEffectGizmo("GM"));

		// -----------------------------------------------------------------------------------------------------------------------------------
		boxRect			= FXMakerLayout.GetInnerHorizontalRect(baseRect, nColCount, 11, 4);
		GUI.Box(boxRect, new GUIContent(" ", " "));
		subRect			= FXMakerLayout.GetOffsetRect(boxRect, 0, 2, 0, -2);
		// Grayscale
		bool bGray		= (GUI.Toggle(FXMakerLayout.GetInnerHorizontalRect(subRect, 3, 0, 1), m_bShowGameObjectOptionGray, FXMakerTooltip.GetHcEffectGizmo("Gray")));
		if (m_bShowGameObjectOptionGray != bGray)
		{
			m_bShowGameObjectOptionGray = bGray;
			UnityEditor.EditorPrefs.SetBool("FXMakerGizmo.m_bShowGameObjectOptionGray", m_bShowGameObjectOptionGray);
			FXMakerMain.inst.ResetCamera();
			FXMakerMain.inst.CreateCurrentInstanceEffect(true);
		}
		// BoundsBox
		bool bBounds	= (GUI.Toggle(FXMakerLayout.GetInnerHorizontalRect(subRect, 3, 1, 1), m_bShowGameObjectOptionBounds, FXMakerTooltip.GetHcEffectGizmo("Box")));
		if (m_bShowGameObjectOptionBounds != bBounds)
		{
			m_bShowGameObjectOptionBounds = bBounds;
			UnityEditor.EditorPrefs.SetBool("FXMakerGizmo.m_bShowGameObjectOptionBounds", m_bShowGameObjectOptionBounds);
			FXMakerHierarchy.inst.ChangeBoundsBoxWireframe(FXMakerHierarchy.inst.GetSelectedGameObject(), FXMakerMain.inst.GetOriginalEffectObject(), true, true);
		}
		// wireframe
		bool bWireframe = (GUI.Toggle(FXMakerLayout.GetInnerHorizontalRect(subRect, 3, 2, 1), m_bShowGameObjectOptionWireframe, FXMakerTooltip.GetHcEffectGizmo("Wire")));
		if (m_bShowGameObjectOptionWireframe != bWireframe)
		{
			m_bShowGameObjectOptionWireframe = bWireframe;
			UnityEditor.EditorPrefs.SetBool("FXMakerGizmo.m_bShowGameObjectOptionWireframe", m_bShowGameObjectOptionWireframe);
			FXMakerHierarchy.inst.ChangeBoundsBoxWireframe(FXMakerHierarchy.inst.GetSelectedGameObject(), FXMakerMain.inst.GetOriginalEffectObject(), true, true);
		}

		// -----------------------------------------------------------------------------------------------------------------------------------
		if (GUI.changed)
		{
			SetGizmoType(m_nGizmoTypeIndex);

			EditorPrefs.SetBool("FXMakerGizmo.m_bWorldSpace", m_bWorldSpace);
			EditorPrefs.SetBool("FXMakerGizmo.m_bFixedSide", m_bFixedSide);
			EditorPrefs.SetBool("FXMakerGizmo.m_bGridXY", m_bGridXY);
			EditorPrefs.SetBool("FXMakerGizmo.m_bGridXZ", m_bGridXZ);
			EditorPrefs.SetBool("FXMakerGizmo.m_bGridYZ", m_bGridYZ);
			EditorPrefs.SetBool("FXMakerGizmo.m_bBackground", m_bBackground);
			EditorPrefs.SetBool("FXMakerGizmo.m_bGM", m_bGM);

			UpdateGrid(true);
			FXMakerBackground.inst.UpdateBackground();
		}

//  		DrawGuiGizmo();
 
 		FXMakerMain.inst.SaveTooltip();
	}

	void SetGizmoType(int gtype)
	{
		NgAssembly.SetGizmosVisible(((GIZMO_TYPE)gtype) != GIZMO_TYPE.NONE);

		m_nGizmoTypeIndex = gtype;
// 		FXMakerMain.inst.GetFXMakerMouse().SetHandControl(((GIZMO_TYPE)m_nGizmoTypeIndex) == GIZMO_TYPE.HAND);
		FXMakerMain.inst.GetFXMakerMouse().SetHandControl(true);

		EditorPrefs.SetInt("FXMakerGizmo.m_nGizmoTypeIndex", m_nGizmoTypeIndex);
		if (((GIZMO_TYPE)m_nGizmoTypeIndex) == GIZMO_TYPE.NONE)
			FXMakerMain.inst.SetEmptyTooltip("");
	}

	Vector3 GetDirect(Transform selTrans, AXIS axis, bool bLocal)
	{
		if (bLocal)
		{
			switch (axis)
			{
				case AXIS.X: return selTrans.right;
				case AXIS.Y: return selTrans.up;
				case AXIS.Z: return selTrans.forward;
			}
		} else {
			switch (axis)
			{
				case AXIS.X: return Vector3.right;
				case AXIS.Y: return Vector3.up;
				case AXIS.Z: return Vector3.forward;
			}
		}
		return Vector3.forward;
	}

	Vector3 GetEuler(Transform selTrans, AXIS axis, bool bLocal)
	{
		if (bLocal)
		{
			Quaternion quat = Quaternion.identity;
			switch (axis)
			{
				case AXIS.X: quat.SetLookRotation(selTrans.right);		break;
				case AXIS.Y: quat.SetLookRotation(selTrans.up);			break;
				case AXIS.Z: quat.SetLookRotation(selTrans.forward);	break;
			}
			return quat.eulerAngles;
		} else {
			switch (axis)
			{
				case AXIS.X: return new Vector3(0, 90, 0);
				case AXIS.Y: return new Vector3(-90, 0, 0);
				case AXIS.Z: return new Vector3(0, 0, 0);
			}
		}
		return Vector3.forward;
	}

	Transform GetTempTransform()
	{
		Transform	tempTrans	= transform;
		tempTrans.position	= Vector3.zero;
		tempTrans.rotation	= Quaternion.identity;
		tempTrans.localScale= Vector3.one;
		tempTrans.parent.localScale = Vector3.one;
		return tempTrans;
	}

	// Property -------------------------------------------------------------------------
	public void SetSpriteCaptureState(bool bEnabled)
	{
		if (bEnabled)
		{
			UpdateGrid(false);
			FXMakerBackground.inst.ShowBackground(false, false);
			enabled = false;
		} else {
			UpdateGrid(true);
			FXMakerBackground.inst.UpdateBackground();
			enabled = true;
		}
	}

	// Control Function -----------------------------------------------------------------
	void UpdateGrid(bool bShow)
	{
		float	fSize		= FXMakerOption.inst.m_GridSize / FXMakerOption.inst.m_GridUnit;
		Vector2	texSize		= new Vector3(FXMakerOption.inst.m_GridSize, FXMakerOption.inst.m_GridSize, 0);
		Vector2	texUnit		= new Vector2(fSize, fSize);

		if (m_GridXYGameObject != null)
		{
			NgObject.SetActive(m_GridXYGameObject, (bShow && m_bGridXY));
			m_GridXYGameObject.transform.localScale	= texSize;
			m_GridXYGameObject.GetComponent<Renderer>().sharedMaterial.mainTextureScale	= texUnit;
		}
		if (m_GridXZGameObject != null)
		{
			NgObject.SetActive(m_GridXZGameObject, (bShow && m_bGridXZ));
			m_GridXZGameObject.transform.localScale	= texSize;
			m_GridXZGameObject.GetComponent<Renderer>().sharedMaterial.mainTextureScale	= texUnit;
		}
		if (m_GridYZGameObject != null)
		{
			NgObject.SetActive(m_GridYZGameObject, (bShow && m_bGridYZ));
			m_GridYZGameObject.transform.localScale	= texSize;
			m_GridYZGameObject.GetComponent<Renderer>().sharedMaterial.mainTextureScale	= texUnit;
		}
	}

	// Event Function -------------------------------------------------------------------

	// -----------------------------------------------------------------------------------
	GUIContent[] GetGizmoContents()
	{
		if (m_HcGizmoContents == null)
		{
			m_HcGizmoContents = FXMakerTooltip.GetHcGizmo();

			m_HcGizmoContents[0].image	= (m_nGizmoTypeIndex == 0) ? m_iconHandActive	: m_iconHandNormal;
			m_HcGizmoContents[1].image	= (m_nGizmoTypeIndex == 1) ? m_iconPosActive	: m_iconPosNormal;
			m_HcGizmoContents[2].image	= (m_nGizmoTypeIndex == 2) ? m_iconRotActive	: m_iconRotNormal;
			m_HcGizmoContents[3].image	= (m_nGizmoTypeIndex == 3) ? m_iconScaleActive	: m_iconScaleNormal;
			m_HcGizmoContents[4].image	= (m_nGizmoTypeIndex == 4) ? m_iconNoneActive	: m_iconNoneNormal;
		}
		return m_HcGizmoContents;
	}
}

#endif
