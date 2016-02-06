// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;

public class FxmTestSimulate : MonoBehaviour
{
	// Attribute ------------------------------------------------------------------------
	public	enum	MODE_TYPE			{NONE, MOVE, ARC, ROTATE, TORNADO, SCALE};

	public			MODE_TYPE			m_Mode;
	public			FxmTestControls.AXIS m_nAxis;
	public			float				m_fStartTime	= 0;
	public			Vector3				m_StartPos;
	public			Vector3				m_EndPos;
	public			float				m_fSpeed		= 0;
	public			bool				m_bRotFront;

	public			float				m_fDist;
	public			float				m_fRadius;
	public			float				m_fArcLenRate;

	public			AnimationCurve		m_Curve;
	public			Component			m_FXMakerControls;
	public			int					m_nMultiShotIndex	= 0;
	public			int					m_nMultiShotCount	= 0;
	public			int					m_nCircleCount		= 0;
	public			Vector3				m_PrevPosition		= Vector3.zero;

	protected static int				m_nMultiShotCreate	= 0;

	// --------------------------------------------------------------------------
	// --------------------------------------------------------------------------

	//	[HideInInspector]

	// Property -------------------------------------------------------------------------

	// Control --------------------------------------------------------------------------
	public void Init(Component fxmEffectControls, int nMultiShotCount)
	{
		m_FXMakerControls	= fxmEffectControls;
		m_nMultiShotCount		= nMultiShotCount;
	}

	public void SimulateMove(FxmTestControls.AXIS nTransAxis, float fHalfDist, float fSpeed, bool bRotFront)
	{
		Vector3	pos	= transform.position;
		m_nAxis		= nTransAxis;
		m_StartPos	= pos;
		m_EndPos	= pos;
		m_StartPos[(int)m_nAxis] -= fHalfDist;
		m_EndPos[(int)m_nAxis] += fHalfDist;
		m_fDist		= Vector3.Distance(m_StartPos, m_EndPos);
		m_Mode		= MODE_TYPE.MOVE;
		SimulateStart(m_StartPos, fSpeed, bRotFront);
	}

	public void SimulateArc(float fHalfDist, float fSpeed, bool bRotFront)
	{
		m_Curve = FxmTestMain.inst.m_SimulateArcCurve;

		if (m_Curve == null)
		{
			Debug.LogError("FXMakerOption.m_SimulateArcCurve is null !!!!");
			return;
		}

// 		float	fHeight = 0;
// 		for (int n = 0; n < m_Curve.length; n++)
// 			fHeight = Mathf.Max(m_Curve.keys[n].value);

		Vector3	pos	= transform.position;
		m_StartPos	= new Vector3(pos.x-fHalfDist, pos.y, pos.z);
		m_EndPos	= new Vector3(pos.x+fHalfDist, pos.y, pos.z);
		m_fDist		= Vector3.Distance(m_StartPos, m_EndPos);
		m_Mode		= MODE_TYPE.ARC;
		SimulateStart(m_StartPos, fSpeed, bRotFront);
	}

	public void SimulateFall(float fHeight, float fSpeed, bool bRotFront)
	{
		Vector3	pos	= transform.position;
		m_StartPos	= new Vector3(pos.x, pos.y+fHeight, pos.z);
		m_EndPos	= new Vector3(pos.x, pos.y, pos.z);
		m_fDist		= Vector3.Distance(m_StartPos, m_EndPos);
		m_Mode		= MODE_TYPE.MOVE;
		SimulateStart(m_StartPos, fSpeed, bRotFront);
	}

	public void SimulateRaise(float fHeight, float fSpeed, bool bRotFront)
	{
		Vector3	pos	= transform.position;
		m_StartPos	= new Vector3(pos.x, pos.y, pos.z);
		m_EndPos	= new Vector3(pos.x, pos.y+fHeight, pos.z);
		m_fDist		= Vector3.Distance(m_StartPos, m_EndPos);
		m_Mode		= MODE_TYPE.MOVE;
		SimulateStart(m_StartPos, fSpeed, bRotFront);
	}

	public void SimulateCircle(float fRadius, float fSpeed, bool bRotFront)
	{
		Vector3	pos	= transform.position;
		m_fRadius	= fRadius;
		m_Mode		= MODE_TYPE.ROTATE;
		m_fDist		= 1;
		SimulateStart(new Vector3(pos.x-fRadius, pos.y, pos.z), fSpeed, bRotFront);
	}

	public void SimulateTornado(float fRadius, float fHeight, float fSpeed, bool bRotFront)
	{
		Vector3	pos	= transform.position;
		m_fRadius	= fRadius;
		m_Mode		= MODE_TYPE.TORNADO;

		m_StartPos	= new Vector3(pos.x-fRadius, pos.y, pos.z);
		m_EndPos	= new Vector3(pos.x-fRadius, pos.y+fHeight, pos.z);
		m_fDist		= Vector3.Distance(m_StartPos, m_EndPos);
		SimulateStart(m_StartPos, fSpeed, bRotFront);

	}

	public void SimulateScale(FxmTestControls.AXIS nTransAxis, float fHalfDist, float fStartPosition, float fSpeed, bool bRotFront)
	{
		Vector3	pos	= transform.position;
		m_nAxis		= nTransAxis;
		m_StartPos	= pos;
		m_EndPos	= pos;
		m_StartPos[(int)m_nAxis]	+= fHalfDist * fStartPosition;
		m_EndPos[(int)m_nAxis]		+= fHalfDist*2 + fHalfDist * fStartPosition;
		m_fDist		= Vector3.Distance(m_StartPos, m_EndPos);
		m_Mode		= MODE_TYPE.SCALE;
		SimulateStart(m_StartPos, fSpeed, bRotFront);
	}

	public void Stop()
	{
		m_fSpeed = 0;
	}

	void SimulateStart(Vector3 startPos, float fSpeed, bool bRotFront)
	{
		transform.position	= startPos;
		m_fSpeed			= fSpeed;
		m_bRotFront			= bRotFront;
		m_nCircleCount		= 0;
		m_PrevPosition		= Vector3.zero;

		if (bRotFront && m_Mode == MODE_TYPE.MOVE)
			transform.LookAt(m_EndPos);

		// Multi shot
 		if (m_Mode != MODE_TYPE.SCALE && 1 < m_nMultiShotCount)
		{
			NcDuplicator dupCom = gameObject.AddComponent<NcDuplicator>();
			dupCom.m_fDuplicateTime		= 0.2f;
			dupCom.m_nDuplicateCount	= m_nMultiShotCount;
			dupCom.m_fDuplicateLifeTime= 0;	

			m_nMultiShotCreate	= 0;
			m_nMultiShotIndex	= 0;
		}
		m_fStartTime	= Time.time;
		Update();
	}

	Vector3 GetArcPos(float fTimeRate)
	{
		Vector3	nextPos	= Vector3.Lerp(m_StartPos, m_EndPos, fTimeRate);
		return new Vector3(nextPos.x, m_Curve.Evaluate(fTimeRate) * m_fDist, nextPos.z);
	}

	// UpdateLoop -----------------------------------------------------------------------
	void Awake()
	{
		m_nMultiShotIndex	= m_nMultiShotCreate;
		m_nMultiShotCreate++;
	}

	void Start()
	{
		m_fStartTime	= Time.time;
	}

	void Update()
	{
		if (0 < m_fDist && 0 < m_fSpeed)
		{
			switch (m_Mode)
			{
				case MODE_TYPE.MOVE:
					{
						float	totalTime	= m_fDist / m_fSpeed;
						float	elapsedTime	= Time.time - m_fStartTime;
						transform.position	= Vector3.Lerp(m_StartPos, m_EndPos, elapsedTime/totalTime);
						if (1 < elapsedTime/totalTime)
							OnMoveEnd();
						break;
					}
				case MODE_TYPE.ARC:
					{
						float	totalTime	= m_fDist / m_fSpeed;
						float	elapsedTime	= Time.time - m_fStartTime;
						Vector3	vecNext		= GetArcPos(elapsedTime/totalTime + elapsedTime/totalTime*0.01f);
						transform.position	= GetArcPos(elapsedTime/totalTime);
						if (m_bRotFront)
							transform.LookAt(vecNext);
						if (1 < elapsedTime/totalTime)
							OnMoveEnd();
						break;
					}
				case MODE_TYPE.ROTATE:
					{
						float	fSpeed		= (m_fSpeed / 3.14f * 360);
						transform.RotateAround(Vector3.zero, Vector3.up, Time.deltaTime * (m_fRadius == 0 ? 0 : fSpeed / (m_fRadius*2)));
						if (m_PrevPosition.z < 0 && 0 < transform.position.z)
						{
							if (1 <= m_nCircleCount)
								OnMoveEnd();
							m_nCircleCount++;
						}
						break;
					}
				case MODE_TYPE.TORNADO:
					{
						float	totalTime	= m_fDist / (m_fSpeed / 20.0f);
						float	elapsedTime	= Time.time - m_fStartTime;
						Vector3	nextPos		= Vector3.Lerp(m_StartPos, m_EndPos, elapsedTime/totalTime);
						transform.position	= new Vector3(transform.position.x, nextPos.y, transform.position.z);

						float	fSpeed		= (m_fSpeed / 3.14f * 360);
						transform.RotateAround(new Vector3(0, nextPos.y, 0), Vector3.up, Time.deltaTime * (m_fRadius == 0 ? 0 : fSpeed / (m_fRadius*2)));
						if (1 < elapsedTime/totalTime)
							OnMoveEnd();
						break;
					}
				case MODE_TYPE.SCALE:
					{
						float	totalTime	= m_fDist / m_fSpeed;
						float	elapsedTime	= Time.time - m_fStartTime;
						Vector3	vPos		= new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
						vPos[(int)m_nAxis]	= m_fDist * elapsedTime/totalTime;
						if (vPos[(int)m_nAxis] == 0)
							vPos[(int)m_nAxis] = 0.001f;
						transform.localScale= vPos;

						if (1 < elapsedTime/totalTime)
							OnMoveEnd();
						break;
					}
			}
		}
		m_PrevPosition	= transform.position;
	}

	void FixedUpdate()
	{
	}

	public void LateUpdate()
	{
	}

	// Event -------------------------------------------------------------------------
	void OnMoveEnd()
	{
		m_fSpeed	= 0;

		NgObject.SetActiveRecursively(gameObject, false);
		if (1 < m_nMultiShotCreate && m_nMultiShotIndex < m_nMultiShotCreate-1)
			return;
		if (m_FXMakerControls != null)
			m_FXMakerControls.SendMessage("OnActionTransEnd");
	}

	// Function ----------------------------------------------------------------------
}
