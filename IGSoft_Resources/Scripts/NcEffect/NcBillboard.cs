// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------


using UnityEngine;
using System.Collections;

public class NcBillboard : NcEffectBehaviour
{
	// Attribute ------------------------------------------------------------------------
	public		bool			m_bCameraLookAt;
	public		bool			m_bFixedObjectUp;
	public		bool			m_bFixedStand;
	public		enum AXIS_TYPE	{AXIS_FORWARD, AXIS_BACK, AXIS_RIGHT, AXIS_LEFT, AXIS_UP, AXIS_DOWN};
	public		AXIS_TYPE		m_FrontAxis;
	public		enum ROTATION	{NONE, RND, ROTATE}
	public		ROTATION		m_RatationMode;
	public		enum AXIS		{X=0, Y, Z};
	public		AXIS			m_RatationAxis		= AXIS.Z;
	public		float			m_fRotationValue	= 180;

	protected	float			m_fRndValue;
	protected	float			m_fTotalRotationValue;
	protected	Quaternion		m_qOiginal;

	// Property -------------------------------------------------------------------------
#if UNITY_EDITOR
	public override string CheckProperty()
	{
		if (1 < gameObject.GetComponents(GetType()).Length)
			return "SCRIPT_WARRING_DUPLICATE";

		return "";	// no error
	}
#endif

	// Loop Function --------------------------------------------------------------------
	void Awake()
	{
	}

	void OnEnable()
	{
#if UNITY_EDITOR
		if (IsCreatingEditObject() == false)
			UpdateBillboard();
#else
 		UpdateBillboard();
#endif
	}

	public void UpdateBillboard()
	{
		m_fRndValue	= Random.Range(0,360.0f);
		if (enabled)
			Update();
	}

	void Start()
	{
		m_qOiginal	= transform.rotation;
	}

	void Update()
	{
		if (Camera.main == null)
			return;
		Vector3		vecUp;

		// 카메라 업벡터를 무시하고 오젝의 업벡터를 유지한다
		if (m_bFixedObjectUp)
//  			vecUp		= m_qOiginal * Vector3.up;
			vecUp		= transform.up;
		else vecUp		= Camera.main.transform.rotation * Vector3.up;

		if (m_bCameraLookAt)
			transform.LookAt(Camera.main.transform,	vecUp);
		else transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.back, vecUp);

		switch (m_FrontAxis)
		{
			case AXIS_TYPE.AXIS_FORWARD:	break;
			case AXIS_TYPE.AXIS_BACK:		transform.Rotate(transform.up,		180, Space.World);		break;
			case AXIS_TYPE.AXIS_RIGHT:		transform.Rotate(transform.up,		270, Space.World);		break;
			case AXIS_TYPE.AXIS_LEFT:		transform.Rotate(transform.up,		 90, Space.World);		break;
			case AXIS_TYPE.AXIS_UP:			transform.Rotate(transform.right,	 90, Space.World);		break;
			case AXIS_TYPE.AXIS_DOWN:		transform.Rotate(transform.right,	270, Space.World);		break;
		}

		if (m_bFixedStand)
			transform.rotation  = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z));

		if (m_RatationMode == ROTATION.RND)
			transform.localRotation	*= Quaternion.Euler((m_RatationAxis == AXIS.X ? m_fRndValue : 0), (m_RatationAxis == AXIS.Y ? m_fRndValue : 0), (m_RatationAxis == AXIS.Z ? m_fRndValue : 0));
		if (m_RatationMode == ROTATION.ROTATE)
		{
			float	fRotValue = GetEngineDeltaTime() * m_fRotationValue;
			transform.Rotate((m_RatationAxis == AXIS.X ? fRotValue : 0), (m_RatationAxis == AXIS.Y ? fRotValue : 0), (m_RatationAxis == AXIS.Z ? fRotValue : 0), Space.Self);
// 			float	fRotValue = m_fTotalRotationValue + GetEngineDeltaTime() * m_fRotationValue;
// 			transform.Rotate((m_RatationAxis == AXIS.X ? fRotValue : 0), (m_RatationAxis == AXIS.Y ? fRotValue : 0), (m_RatationAxis == AXIS.Z ? fRotValue : 0), Space.Self);
// 			m_fTotalRotationValue = fRotValue;
		}

//		transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.back,
//			Camera.main.transform.rotation * Vector3.up);
	}
	// Control Function -----------------------------------------------------------------
	// Event Function -------------------------------------------------------------------
}


