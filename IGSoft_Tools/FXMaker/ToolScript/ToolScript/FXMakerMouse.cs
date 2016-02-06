// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Collections;

public class FXMakerMouse : MonoBehaviour
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
   
    public		float		m_fDistanceMin		= 0.5f;
    public		float		m_fDistanceMax		= 200f;
     
    protected	float		m_fXRot				= 0.0f;
    protected	float		m_fYRot				= 0.0f;

	// HandControl
	protected	bool		m_bHandEnable		= true;
	protected	Vector3		m_MovePostion;
	protected	Vector3		m_OldMousePos;
	protected	bool		m_bLeftClick;
	protected	bool		m_bRightClick;

	// -----------------------------------------------------------------
	public void ChangeAngle(float xAngle, float yAngle)
	{
		m_fYRot			= xAngle;
		m_fXRot			= yAngle;
	}

	public void ChangeAngle(float angle)
	{
		m_fYRot			= angle;
		m_fXRot			= 0;
		m_MovePostion	= Vector3.zero;
	}

	public void ChangeAngleH(float angle)
	{
		m_fYRot			= 0;
		m_fXRot			= angle;
		m_MovePostion	= new Vector3(0, 1.2f, 0);
	}

	public void SetHandControl(bool bEnable)
	{
		m_bHandEnable = bEnable;
	}

	public void SetDistance(float fDistance)
	{
		m_fDistance *= fDistance;
		UnityEditor.EditorPrefs.SetFloat("FXMakerMouse.m_fDistance", m_fDistance);
	}

	// -----------------------------------------------------------------
	void OnEnable()
	{
		if (FXMakerLayout.m_bDevelopPrefs == false)
			m_fDistance	= EditorPrefs.GetFloat("FXMakerMouse.m_fDistance", m_fDistance);
	}

    void Start()
	{
		if (Camera.main == null)
			return;

        Vector3 angles = Camera.main.transform.eulerAngles;
        m_fXRot = angles.y;
        m_fYRot = angles.x;

        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;
    }

	void Update()
	{
// 		if (m_GrayscaleCamara != null)
// 		{
// 			if (m_GrayscaleShader != null)
// 			{
// 				m_GrayscaleCamara.ResetReplacementShader();
// 				Camera.main.SetReplacementShader(m_GrayscaleShader, "");
// 			} else
// 			{
// 				m_GrayscaleCamara.ResetReplacementShader();
// 				Camera.main.ResetReplacementShader();
// 			}
// 
// 		}

		if (FXMakerMain.inst.IsGUIMousePosition() && (m_bLeftClick == false && m_bRightClick == false))
			return;

		if (EditorWindow.mouseOverWindow != null)
		{
			try
			{
				string wndName = EditorWindow.mouseOverWindow.ToString();
				if (wndName != null && wndName.Contains(".GameView") == false)
					return;
			}
			catch (System.Exception ex)
			{
				Debug.Log(ex.Message);
				return;
			}
		}

		UpdateCamera();
	}

	public void UpdateCamera()
	{
		if (Camera.main == null)
			return;

		if (m_fWheelSpeed < 0)
			m_fWheelSpeed = m_fDefaultWheelSpeed;

		float fDistRate		= m_fDistance / m_fDefaultDistance;
		float fOldDistance	= m_fDistance;

		if (m_TargetTrans)
		{
			if (m_fDistanceMin < 0.1f)
				m_fDistanceMin = 0.1f;
			if (FXMakerMain.inst.IsGUIMousePosition() == false || FXMakerMain.IsWindowFocus() == false)
				m_fDistance = Mathf.Clamp(m_fDistance - Input.GetAxis("Mouse ScrollWheel")*m_fWheelSpeed*fDistRate, m_fDistanceMin, m_fDistanceMax);

			// 오소방식이면.. 화면 사이즈를 조정하자
			if (Camera.main.orthographic)
			{
				Camera.main.orthographicSize = m_fDistance*0.60f;
				if (m_GrayscaleCamara != null)
					m_GrayscaleCamara.orthographicSize = m_fDistance*0.60f;
			}

			if (GetRightButtonDown())
				m_bRightClick	= true;
			if (GetRightButtonUp())
				m_bRightClick	= false;

			if (GetRightButton())
			{
				m_fXRot += Input.GetAxis("Mouse X") * m_fXSpeed * 0.02f;// * m_fDistance * 0.02f;
				m_fYRot -= Input.GetAxis("Mouse Y") * m_fYSpeed * 0.02f;
			}
	 
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
				UnityEditor.EditorPrefs.SetFloat("FXMakerMouse.m_fDistance", m_fDistance);
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
			if (GetLeftButtonDown())
			{
				m_OldMousePos	= Input.mousePosition;
				m_bLeftClick	= true;
			}

			if (m_bLeftClick && GetLeftButton())
			{
				Vector3 currMousePos	= Input.mousePosition;
				float	worldlen		= FXMakerLayout.GetWorldPerScreenPixel(m_TargetTrans.transform.position);
				
				m_MovePostion += (m_OldMousePos - currMousePos) * worldlen;
				m_OldMousePos = currMousePos;
			}
			if (GetLeftButtonUp())
				m_bLeftClick = false;
		}

		camera.Translate(m_MovePostion, Space.Self);
	}

	bool GetLeftButtonDown()
	{
		return (Input.GetMouseButtonDown(0) && !(Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)));
	}

	bool GetLeftButtonUp()
	{
		return !GetLeftButton();
	}

	bool GetLeftButton()
	{
		return (Input.GetMouseButton(0) && !(Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)));
	}

	bool GetRightButtonDown()
	{
		return (Input.GetMouseButtonDown(1) || (Input.GetMouseButtonDown(0) && (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))));
	}

	bool GetRightButtonUp()
	{
		return !GetRightButton();
	}

	bool GetRightButton()
	{
		return (Input.GetMouseButton(1) || (Input.GetMouseButton(0) && (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))));
	}
}
#endif
