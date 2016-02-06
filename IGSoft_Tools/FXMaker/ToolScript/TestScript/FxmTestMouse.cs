// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;

public class FxmTestMouse : MonoBehaviour
{
	// Attribute ------------------------------------------------------------------------
    public		Transform	m_TargetTrans;
	public		Camera		m_GrayscaleCamara;
	public		Shader		m_GrayscaleShader;

	protected	bool		m_bRaycastHit;
	protected	const float	m_fDefaultDistance	= 8.0f;
	protected	const float	m_fDefaultWheelSpeed= 5.0f;
    public		float		m_fDistance			= m_fDefaultDistance;
    public		float		m_fXSpeed			= 350.0f;
    public		float		m_fYSpeed			= 300.0f;
	public		float		m_fWheelSpeed		= m_fDefaultWheelSpeed;

    public		float		m_fYMinLimit		= -90f;
    public		float		m_fYMaxLimit		= 90f;
   
    public		float		m_fDistanceMin		= 1.0f;
    public		float		m_fDistanceMax		= 50;

	public		int			m_nMoveInputIndex	= 1;
	public		int			m_nRotInputIndex	= 0;
 
    public		float		m_fXRot				= 0.0f;
    public		float		m_fYRot				= 0.0f;

	// HandControl
	protected	bool		m_bHandEnable		= true;
	protected	Vector3		m_MovePostion;
	protected	Vector3		m_OldMousePos;
	protected	bool		m_bLeftClick;
	protected	bool		m_bRightClick;

	// -----------------------------------------------------------------
	public void ChangeAngle(float angle)
	{
		m_fYRot			= angle;
		m_fXRot			= 0;
		m_MovePostion	= Vector3.zero;
	}

	public void SetHandControl(bool bEnable)
	{
		m_bHandEnable = bEnable;
	}

	public void SetDistance(float fDistance)
	{
		m_fDistance = fDistance;
		PlayerPrefs.SetFloat("FxmTestMouse.m_fDistance", m_fDistance);
		UpdateCamera(true);
	}

	// -----------------------------------------------------------------
	void OnEnable()
	{
		m_fDistance	= PlayerPrefs.GetFloat("FxmTestMouse.m_fDistance", m_fDistance);
	}

    void Start()
	{
		if (Camera.main == null)
			return;

//         Vector3 angles = Camera.main.transform.eulerAngles;
//         m_fXRot = angles.y;
//         m_fYRot = angles.x;

        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;
    }

	bool IsGUIMousePosition()
	{
		Vector2 pos = NgLayout.GetGUIMousePosition();
		if (FxmTestMain.inst.GetFXMakerControls().GetActionToolbarRect().Contains(pos))
			return true;
		if (new Rect(0, 0, Screen.width, Screen.height/10+30).Contains(pos))
			return true;
		if (new Rect(0, 0, 40, Screen.height).Contains(pos))
			return true;
		return false;
	}

	void Update()
	{
		if (IsGUIMousePosition() && (m_bLeftClick == false && m_bRightClick == false))
			return;

		UpdateCamera(false);
	}

	public void UpdateCamera(bool bOnlyZoom)
	{
		if (Camera.main == null)
			return;

		if (m_fWheelSpeed < 0)
			m_fWheelSpeed = m_fDefaultWheelSpeed;

		float fDistRate		= m_fDistance / m_fDefaultDistance;
		float fOldDistance	= m_fDistance;

		if (m_TargetTrans)
		{
			m_fDistance = Mathf.Clamp(m_fDistance - Input.GetAxis("Mouse ScrollWheel")*m_fWheelSpeed*fDistRate, m_fDistanceMin, m_fDistanceMax);

			// 오소방식이면.. 화면 사이즈를 조정하자
			if (Camera.main.orthographic)
			{
				Camera.main.orthographicSize = m_fDistance*0.60f;
				if (m_GrayscaleCamara != null)
					m_GrayscaleCamara.orthographicSize = m_fDistance*0.60f;
			}

			if (!bOnlyZoom && m_bRightClick && Input.GetMouseButton(m_nRotInputIndex))
			{
				m_fXRot += Input.GetAxis("Mouse X") * m_fXSpeed * 0.02f;// * m_fDistance * 0.02f;
				m_fYRot -= Input.GetAxis("Mouse Y") * m_fYSpeed * 0.02f;
			}

			if (!bOnlyZoom && Input.GetMouseButtonDown(m_nRotInputIndex))
				m_bRightClick	= true;
			if (!bOnlyZoom && Input.GetMouseButtonUp(m_nRotInputIndex))
				m_bRightClick	= false;

			m_fYRot = ClampAngle(m_fYRot, m_fYMinLimit, m_fYMaxLimit);
	       
			Quaternion rotation = Quaternion.Euler(m_fYRot, m_fXRot, 0);
	 
			if (m_bRaycastHit)
			{
				RaycastHit hit;
				if (Physics.Linecast (m_TargetTrans.position, Camera.main.transform.position, out hit)) {
						m_fDistance -=  hit.distance;
				}
			}

			Vector3 negDistance = new Vector3(0.0f, 0.0f, -m_fDistance);
			Vector3 position = rotation * negDistance + m_TargetTrans.position;
	 
			Camera.main.transform.rotation = rotation;
			Camera.main.transform.position = position;
			UpdatePosition(Camera.main.transform);
			if (m_GrayscaleCamara != null)
			{
				m_GrayscaleCamara.transform.rotation = Camera.main.transform.rotation;
				m_GrayscaleCamara.transform.position = Camera.main.transform.position;
			}

			// save
			if (fOldDistance != m_fDistance)
				PlayerPrefs.SetFloat("FxmTestMouse.m_fDistance", m_fDistance);
		}
	}

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)	angle += 360F;
        if (angle > 360F)	angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }

	void UpdatePosition(Transform camera)
	{
		if (m_bHandEnable)
		{
			if (Input.GetMouseButtonDown(m_nMoveInputIndex))
			{
				m_OldMousePos	= Input.mousePosition;
				m_bLeftClick	= true;
			}

			if (m_bLeftClick && Input.GetMouseButton(m_nMoveInputIndex))
			{
				Vector3 currMousePos	= Input.mousePosition;
				float	worldlen		= NgLayout.GetWorldPerScreenPixel(m_TargetTrans.transform.position);
				
				m_MovePostion += (m_OldMousePos - currMousePos) * worldlen;
				m_OldMousePos = currMousePos;
			}
			if (Input.GetMouseButtonUp(m_nMoveInputIndex))
				m_bLeftClick = false;
		}

		camera.Translate(m_MovePostion, Space.Self);
	}
}
