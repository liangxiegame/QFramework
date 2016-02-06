// ----------------------------------------------------------------------------------
//
// FXMaker
// Modify by ismoon - 2012 - ismoonto@gmail.com
//
// reference source - http://wiki.unity3d.com/index.php/MeleeWeaponTrail
//
// ----------------------------------------------------------------------------------

#define USE_INTERPOLATION	// Unfinished
//
// By Anomalous Underdog, 2011
//
// Based on code made by Forest Johnson (Yoggy) and xyber
//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
// [RequireComponent (typeof(MeshRenderer))]
 
public class NcTrailTexture : NcEffectBehaviour
{
	// Attribute ------------------------------------------------------------------------
	public		float		m_fDelayTime;
	public		float		m_fEmitTime				= 0.00f;
	public		bool		m_bSmoothHide			= true;
	protected	bool		m_bEmit					= true;
	protected	float		m_fStartTime;
	protected	float		m_fStopTime;
	public		float		m_fLifeTime				= 0.70f;
	public		enum AXIS_TYPE	{AXIS_FORWARD, AXIS_BACK, AXIS_RIGHT, AXIS_LEFT, AXIS_UP, AXIS_DOWN};
	public		AXIS_TYPE	m_TipAxis				= AXIS_TYPE.AXIS_BACK;
	public		float		m_fTipSize				= 1.0f;
	public		bool		m_bCenterAlign			= false;
	public		bool		m_UvFlipHorizontal		= false;
	public		bool		m_UvFlipVirtical		= false;

	public		int			m_nFadeHeadCount		= 2;
	public		int			m_nFadeTailCount		= 2;

	public		Color[]		m_Colors;
	public		float[]		m_SizeRates;

#if USE_INTERPOLATION
	public		bool		m_bInterpolation		= false;
	public		int			m_nMaxSmoothCount		= 10;
	public		int			m_nSubdivisions			= 4;
	protected	List<Point>	m_SmoothedPoints		= new List<Point>();
#endif

	public		float		m_fMinVertexDistance	= 0.20f;
	public		float		m_fMaxVertexDistance	= 10.00f;
	public		float		m_fMaxAngle				= 3.00f;

	public		bool		m_bAutoDestruct			= false;

	protected	List<Point> m_Points				= new List<Point>();

	protected	Transform	m_base;
	protected	GameObject	m_TrialObject;
	protected	Mesh		m_TrailMesh;
	protected	Vector3		m_LastPosition;
	protected	Vector3		m_LastCameraPosition1;
	protected	Vector3		m_LastCameraPosition2;
	protected	bool		m_bLastFrameEmit		= true;

 	public class Point
	{
		public	float		timeCreated		= 0.00f;
		public	Vector3		basePosition;
		public	Vector3		tipPosition;
		public	bool		lineBreak		= false;
	}

	// Property -------------------------------------------------------------------------
	public void SetEmit(bool bEmit)
	{
		m_bEmit = bEmit;
		m_fStartTime	= GetEngineTime();
		m_fStopTime		= 0;
	}

#if UNITY_EDITOR
	public override string CheckProperty()
	{
// 		if (1 < gameObject.GetComponents(GetType()).Length)
// 			return "SCRIPT_WARRING_DUPLICATE";
		if (GetComponent<Renderer>() == null)
			return "SCRIPT_EMPTY_MESHRENDERER";
		if (GetComponent<Renderer>().sharedMaterial == null)
			return "SCRIPT_EMPTY_MATERIAL";

		return "";	// no error
	}
#endif

 	public override int GetAnimationState()
	{
		if ((enabled && IsActive(gameObject)))
		{
			if (GetEngineTime() < m_fStartTime + m_fDelayTime + 0.1f)
				return 1;
			return -1;
		}
		return -1;
	}

	// Event Function -------------------------------------------------------------------
	void OnDisable()
	{
		if (m_TrialObject != null)
			NcAutoDestruct.CreateAutoDestruct(m_TrialObject, 0, m_fLifeTime/2.0f, true, true);
	}

	void Start()
	{
		if (GetComponent<Renderer>() == null || GetComponent<Renderer>().sharedMaterial == null)
		{
			enabled = false;
			return;
		}

		if (0 < m_fDelayTime)
		{
			m_fStartTime = GetEngineTime();
		} else {
			InitTrailObject();
		}
	}

	void InitTrailObject()
	{
		m_base			= transform;
		m_fStartTime	= GetEngineTime();

		m_LastPosition	= transform.position;
		m_TrialObject	= new GameObject("Trail");
		m_TrialObject.transform.position = Vector3.zero;
		m_TrialObject.transform.rotation = Quaternion.identity;
		m_TrialObject.transform.localScale = transform.localScale;
		m_TrialObject.AddComponent(typeof(MeshFilter));
		m_TrialObject.AddComponent(typeof(MeshRenderer));
// 		m_TrialObject.AddComponent<Nc>();
		m_TrialObject.GetComponent<Renderer>().sharedMaterial = GetComponent<Renderer>().sharedMaterial;
		m_TrailMesh = m_TrialObject.GetComponent<MeshFilter>().mesh;
		CreateEditorGameObject(m_TrialObject);
	}

	Vector3 GetTipPoint()
	{
		switch (m_TipAxis)
		{
			case AXIS_TYPE.AXIS_FORWARD:	return m_base.position + m_base.forward;
			case AXIS_TYPE.AXIS_BACK:		return m_base.position + m_base.forward * -1;
			case AXIS_TYPE.AXIS_RIGHT:		return m_base.position + m_base.right;
			case AXIS_TYPE.AXIS_LEFT:		return m_base.position + m_base.right * -1;
			case AXIS_TYPE.AXIS_UP:			return m_base.position + m_base.up;
			case AXIS_TYPE.AXIS_DOWN:		return m_base.position + m_base.up * -1;
		}
		return m_base.position + m_base.forward;
	}

	void Update()
	{
		if (GetComponent<Renderer>() == null || GetComponent<Renderer>().sharedMaterial == null)
		{
			enabled = false;
			return;
		}

		if (0 < m_fDelayTime)
		{
			if (GetEngineTime() < m_fStartTime + m_fDelayTime)
				return;
			m_fDelayTime = 0;
			m_fStartTime = 0;
			InitTrailObject();
		}

		if (m_bEmit && 0 < m_fEmitTime && m_fStopTime == 0)
		{
			if (m_fStartTime + m_fEmitTime < GetEngineTime())
			{
				if (m_bSmoothHide)
					m_fStopTime = GetEngineTime();
				else m_bEmit = false;
			}
		}
		if (0 < m_fStopTime && m_fLifeTime < (GetEngineTime() - m_fStopTime))
			m_bEmit = false;
 
		if (!m_bEmit && m_Points.Count == 0 && m_bAutoDestruct)
		{
			Destroy(m_TrialObject);
			Destroy(gameObject);
		}
 
// 		// early out if there is no camera
// 		if (!Camera.main) return;

		// if we have moved enough, create a new vertex and make sure we rebuild the mesh
		float theDistance = (m_LastPosition - transform.position).magnitude;
		if (m_bEmit)
		{
			if (theDistance > m_fMinVertexDistance)
			{
				bool make = false;
				if (m_Points.Count < 3)
				{
					make = true;
				}
				else
				{
					//Vector3 l1 = m_Points[m_Points.Count - 2].basePosition - m_Points[m_Points.Count - 3].basePosition;
					//Vector3 l2 = m_Points[m_Points.Count - 1].basePosition - m_Points[m_Points.Count - 2].basePosition;
					Vector3 l1 = m_Points[m_Points.Count - 2].basePosition - m_Points[m_Points.Count - 3].basePosition;
					Vector3 l2 = m_Points[m_Points.Count - 1].basePosition - m_Points[m_Points.Count - 2].basePosition;
					if (Vector3.Angle(l1, l2) > m_fMaxAngle || theDistance > m_fMaxVertexDistance) make = true;
				}
 
				if (make)
				{
					Point p = new Point();
					p.basePosition	= m_base.position;
					p.tipPosition	= GetTipPoint();
					if (0 < m_fStopTime)
					{
						p.timeCreated	= GetEngineTime() - (GetEngineTime() - m_fStopTime);
					} else {
						p.timeCreated	= GetEngineTime();
					}
					m_Points.Add(p);
					m_LastPosition = transform.position;
 
 
					if (m_bInterpolation)
					{
						if (m_Points.Count == 1)
						{
							m_SmoothedPoints.Add(p);
						}
						else if (1 < m_Points.Count)
						{
							// add 1+m_nSubdivisions for every possible pair in the m_Points
							for (int n = 0; n < 1+m_nSubdivisions; ++n)
								m_SmoothedPoints.Add(p);
						}
	 
						// we use 4 control points for the smoothing
						int nMinSmoothCount = 2;
						if (nMinSmoothCount <= m_Points.Count)
						{
							int nSampleCount = Mathf.Min(m_nMaxSmoothCount, m_Points.Count);
							Vector3[] tipPoints = new Vector3[nSampleCount];
							for (int n = 0; n < nSampleCount; n++)
								tipPoints[n] = m_Points[m_Points.Count - (nSampleCount - n)].basePosition;
	 
	//						IEnumerable<Vector3> smoothTip = NcInterpolate.NewBezier(NcInterpolate.Ease(NcInterpolate.EaseType.Linear), tipPoints, m_nSubdivisions);
							IEnumerable<Vector3> smoothTip = NgInterpolate.NewCatmullRom(tipPoints, m_nSubdivisions, false);
	 
							Vector3[] basePoints = new Vector3[nSampleCount];
							for (int n = 0; n < nSampleCount; n++)
								basePoints[n] = m_Points[m_Points.Count - (nSampleCount - n)].tipPosition;
	 
	//						IEnumerable<Vector3> smoothBase = NcInterpolate.NewBezier(NcInterpolate.Ease(NcInterpolate.EaseType.Linear), basePoints, m_nSubdivisions);
							IEnumerable<Vector3> smoothBase = NgInterpolate.NewCatmullRom(basePoints, m_nSubdivisions, false);
	 
							List<Vector3> smoothTipList = new List<Vector3>(smoothTip);
							List<Vector3> smoothBaseList = new List<Vector3>(smoothBase);
	 
							float firstTime = m_Points[m_Points.Count - nSampleCount].timeCreated;
							float secondTime = m_Points[m_Points.Count - 1].timeCreated;
	 
							//Debug.Log(" smoothTipList.Count: " + smoothTipList.Count);
	 
							for (int n = 0; n < smoothTipList.Count; ++n)
							{
	 
								int idx = m_SmoothedPoints.Count - (smoothTipList.Count-n);
								// there are moments when the m_SmoothedPoints are lesser
								// than what is required, when elements from it are removed
								if (-1 < idx && idx < m_SmoothedPoints.Count)
								{
									Point sp = new Point();
									sp.tipPosition = smoothBaseList[n];
									sp.basePosition = smoothTipList[n];
									sp.timeCreated = Mathf.Lerp(firstTime, secondTime, n/(float)(smoothTipList.Count));
									m_SmoothedPoints[idx] = sp;
								}
								//else
								//{
								//	Debug.LogError(idx + "/" + m_SmoothedPoints.Count);
								//}
							}
						}
					}
				}
				else
				{
					m_Points[m_Points.Count - 1].tipPosition = GetTipPoint();
					m_Points[m_Points.Count - 1].basePosition = m_base.position;
					//m_Points[m_Points.Count - 1].timeCreated = GetEngineTime();
 
					if (m_bInterpolation)
					{
						m_SmoothedPoints[m_SmoothedPoints.Count - 1].tipPosition = GetTipPoint();
						m_SmoothedPoints[m_SmoothedPoints.Count - 1].basePosition = m_base.position;
					}
				}
			}
			else
			{
				if (m_Points.Count > 0)
				{
					m_Points[m_Points.Count - 1].tipPosition = GetTipPoint();
					m_Points[m_Points.Count - 1].basePosition = m_base.position;
					//m_Points[m_Points.Count - 1].timeCreated = GetEngineTime();
				}
 
				if (m_bInterpolation)
				{
					if (m_SmoothedPoints.Count > 0)
					{
						m_SmoothedPoints[m_SmoothedPoints.Count - 1].tipPosition = GetTipPoint();
						m_SmoothedPoints[m_SmoothedPoints.Count - 1].basePosition = m_base.position;
					}
				}
			}
		}
 
		if (!m_bEmit && m_bLastFrameEmit && m_Points.Count > 0)
			m_Points[m_Points.Count - 1].lineBreak = true;
 
		m_bLastFrameEmit = m_bEmit;
 
 
 
 
		List<Point> remove = new List<Point>();
		foreach (Point p in m_Points)
		{
			// cull old points first
			if (GetEngineTime() - p.timeCreated > m_fLifeTime)
			{
				remove.Add(p);
			}
		}
		foreach (Point p in remove)
		{
			m_Points.Remove(p);
		}
 
		if (m_bInterpolation)
		{
			remove = new List<Point>();
			foreach (Point p in m_SmoothedPoints)
			{
				// cull old points first
				if (GetEngineTime() - p.timeCreated > m_fLifeTime)
				{
					remove.Add(p);
				}
			}
			foreach (Point p in remove)
			{
				m_SmoothedPoints.Remove(p);
			}
		}
 
		List<Point> pointsToUse;
		if (m_bInterpolation)
			 pointsToUse = m_SmoothedPoints;
		else pointsToUse = m_Points;
 
		if (pointsToUse.Count > 1)
		{
			Vector3[] newVertices = new Vector3[pointsToUse.Count * 2];
			Vector2[] newUV = new Vector2[pointsToUse.Count * 2];
			int[] newTriangles = new int[(pointsToUse.Count - 1) * 6];
			Color[] newColors = new Color[pointsToUse.Count * 2];
 
			for (int n = 0; n < pointsToUse.Count; ++n)
			{
				Point p = pointsToUse[n];
				float time = (GetEngineTime() - p.timeCreated) / m_fLifeTime;
 
				Color color = Color.Lerp(Color.white, Color.clear, time);
				if (m_Colors != null && m_Colors.Length > 0)
				{
					float colorTime = time * (m_Colors.Length - 1);
					float min = Mathf.Floor(colorTime);
					float max = Mathf.Clamp(Mathf.Ceil(colorTime), 1, m_Colors.Length - 1);
					float lerp = Mathf.InverseLerp(min, max, colorTime);
					if (min >= m_Colors.Length) min = m_Colors.Length - 1; if (min < 0) min = 0;
					if (max >= m_Colors.Length) max = m_Colors.Length - 1; if (max < 0) max = 0;
					color = Color.Lerp(m_Colors[(int)min], m_Colors[(int)max], lerp);
				}
 
				Vector3 lineDirection = p.basePosition - p.tipPosition;
				float size = m_fTipSize;

				if (m_SizeRates != null && m_SizeRates.Length > 0)
				{
					float sizeTime = time * (m_SizeRates.Length - 1);
					float min = Mathf.Floor(sizeTime);
					float max = Mathf.Clamp(Mathf.Ceil(sizeTime), 1, m_SizeRates.Length - 1);
					float lerp = Mathf.InverseLerp(min, max, sizeTime);
					if (min >= m_SizeRates.Length) min = m_SizeRates.Length - 1; if (min < 0) min = 0;
					if (max >= m_SizeRates.Length) max = m_SizeRates.Length - 1; if (max < 0) max = 0;
					size *= Mathf.Lerp(m_SizeRates[(int)min], m_SizeRates[(int)max], lerp);
				}
 
				if (m_bCenterAlign)
				{
					newVertices[n * 2]			= p.basePosition - (lineDirection * (size * 0.5f));
					newVertices[(n * 2) + 1]	= p.basePosition + (lineDirection * (size * 0.5f));
				} else {
					newVertices[n * 2]			= p.basePosition - (lineDirection * size);
					newVertices[(n * 2) + 1]	= p.basePosition;
				}

				// FadeInOut
				int		nFadeTailCount	= (m_bInterpolation ? m_nFadeTailCount * m_nSubdivisions : m_nFadeTailCount);
				int		nFadeHeadCount	= (m_bInterpolation ? m_nFadeHeadCount * m_nSubdivisions : m_nFadeHeadCount);
				if (0 < nFadeTailCount && n <= nFadeTailCount)
					color.a = color.a * n / (float)nFadeTailCount;
				if (0 < nFadeHeadCount && pointsToUse.Count-(n+1) <= nFadeHeadCount)
					color.a = color.a * (pointsToUse.Count-(n+1)) / (float)nFadeHeadCount;

				newColors[n * 2] = newColors[(n * 2) + 1] = color;
 
				float uvRatio = (float)n/pointsToUse.Count;

				newUV[n * 2] = new Vector2((m_UvFlipHorizontal ? 1-uvRatio : uvRatio), (m_UvFlipVirtical ? 1 : 0));
				newUV[(n * 2) + 1] = new Vector2((m_UvFlipHorizontal ? 1-uvRatio : uvRatio), (m_UvFlipVirtical ? 0 : 1));
 
				if (n > 0 /*&& !pointsToUse[n - 1].lineBreak*/)
				{
					newTriangles[(n - 1) * 6] = (n * 2) - 2;
					newTriangles[((n - 1) * 6) + 1] = (n * 2) - 1;
					newTriangles[((n - 1) * 6) + 2] = n * 2;
 
					newTriangles[((n - 1) * 6) + 3] = (n * 2) + 1;
					newTriangles[((n - 1) * 6) + 4] = n * 2;
					newTriangles[((n - 1) * 6) + 5] = (n * 2) - 1;
				}
			}
 
			m_TrailMesh.Clear();
			m_TrailMesh.vertices = newVertices;
			m_TrailMesh.colors = newColors;
			m_TrailMesh.uv = newUV;
			m_TrailMesh.triangles = newTriangles;
 		} else {
			m_TrailMesh.Clear();
 		}
	}

	// Event Function -------------------------------------------------------------------
	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		m_fDelayTime	/= fSpeedRate;
		m_fEmitTime		/= fSpeedRate;
		m_fLifeTime		/= fSpeedRate;
	}
}
