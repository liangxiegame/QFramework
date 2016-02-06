// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

//[AddComponentMenu("EsayTool/NcAttachPrefab	%#A")]

public class NcParticleEmit : NcEffectBehaviour
{
	// Attribute ------------------------------------------------------------------------
 	public		enum			AttachType				{Active, Destroy};
	public		AttachType		m_AttachType			= AttachType.Active;
	public		float			m_fDelayTime;
	public		float			m_fRepeatTime;
	public		int				m_nRepeatCount;

	public		GameObject		m_ParticlePrefab;
	public		int				m_EmitCount				= 10;
	public		Vector3			m_AddStartPos			= Vector3.zero;
	public		Vector3			m_RandomRange			= Vector3.zero;

	protected	float			m_fStartTime;
	protected	int				m_nCreateCount			= 0;
	protected	bool			m_bStartAttach			= false;
	protected	GameObject		m_CreateGameObject;
	protected	bool			m_bEnabled;
	protected	ParticleSystem	m_ps;

	// Property -------------------------------------------------------------------------
#if UNITY_EDITOR
	public override string CheckProperty()
	{
		if (m_ParticlePrefab == null || (m_ParticlePrefab.GetComponent<ParticleEmitter>() == null && m_ParticlePrefab.GetComponent<ParticleSystem>() == null))
			return "SCRIPT_EMPTY_PARTICLEPREFAB";
		return "";	// no error
	}
#endif

 	public override int GetAnimationState()
	{
		if (m_bEnabled && (enabled && IsActive(gameObject)) && m_ParticlePrefab != null)
		{
			if (m_AttachType == AttachType.Active && 
				((m_nRepeatCount == 0 && m_nCreateCount < 1) ||
				(0 < m_fRepeatTime && m_nRepeatCount == 0) ||
				(0 < m_nRepeatCount && m_nCreateCount < m_nRepeatCount)))
				return 1;
			if (m_AttachType == AttachType.Destroy)
				return 1;
		}
		return 0;
	}

	public void UpdateImmediately()
	{
// 		if (m_bEnabled && (enabled/* && IsActive(gameObject)*/) && m_ParticlePrefab != null)
			Update();
	}

	public GameObject EmitSharedParticle()
	{
		return CreateAttachSharedParticle();
	}

	public GameObject GetInstanceObject()
	{
		if (m_CreateGameObject == null)
			UpdateImmediately();
		return m_CreateGameObject;
	}

	public void SetEnable(bool bEnable)
	{
		m_bEnabled = bEnable;
	}

	// Loop Function --------------------------------------------------------------------
	void Awake()
	{
#if UNITY_EDITOR
		if (IsCreatingEditObject() == true)
			m_bEnabled = false;
		else
#endif
		m_bEnabled = (enabled && IsActive(gameObject) && GetComponent<NcDontActive>() == null);

	}

	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		if (m_ParticlePrefab == null)
			return;

		if (m_AttachType == AttachType.Active)
		{
			if (m_bStartAttach == false)
			{
				m_fStartTime	= GetEngineTime();
				m_bStartAttach	= true;
			}

			if (m_fStartTime + m_fDelayTime <= GetEngineTime())
			{
				CreateAttachPrefab();
				if ((0 < m_fRepeatTime && m_nRepeatCount == 0) || (m_nCreateCount < m_nRepeatCount))
				{
					m_fStartTime	= GetEngineTime();
					m_fDelayTime	= m_fRepeatTime;
				} else {
					enabled = false;
				}
			}
		}
	}

	protected override void OnDestroy()
	{
//  	Debug.Log("OnDestroy");
		if (m_bEnabled && IsSafe() && m_AttachType == AttachType.Destroy && m_ParticlePrefab != null)
			CreateAttachPrefab();
		base.OnDestroy();
	}

	// Control Function -----------------------------------------------------------------
	void CreateAttachPrefab()
	{
		m_nCreateCount++;

		CreateAttachSharedParticle();

		if ((0 == m_fRepeatTime || m_AttachType == AttachType.Destroy) && 0 < m_nRepeatCount && m_nCreateCount < m_nRepeatCount)
			CreateAttachPrefab();
	}

	GameObject CreateAttachSharedParticle()
	{
		if (m_CreateGameObject == null)
			m_CreateGameObject = NsSharedManager.inst.GetSharedParticleGameObject(m_ParticlePrefab);
		if (m_CreateGameObject == null)
			return null;

		// sync Layer
//		Ng_ChangeLayerWithChild(m_CreateGameObject, gameObject.layer);

		// Random pos, AddStartPos
		Vector3	newPos = transform.position + m_AddStartPos + m_ParticlePrefab.transform.position;
		m_CreateGameObject.transform.position = new Vector3(Random.Range(-m_RandomRange.x, m_RandomRange.x)+newPos.x, Random.Range(-m_RandomRange.y, m_RandomRange.y)+newPos.y, Random.Range(-m_RandomRange.z, m_RandomRange.z)+newPos.z);
		// m_AccumStartRot - unsupported
		// PrefabAdjustSpeed - unsupported
		// m_bDetachParent - unsupported

		if (m_CreateGameObject.GetComponent<ParticleEmitter>() != null)
			 m_CreateGameObject.GetComponent<ParticleEmitter>().Emit(m_EmitCount);
// 			 m_CreateGameObject.particleEmitter.Emit(newPos, Vector3.zero, 1, 1, Color.white);
		else {
			if (m_ps == null)
				m_ps = m_CreateGameObject.GetComponent<ParticleSystem>();
			if (m_ps != null)
				m_ps.Emit(m_EmitCount);
		}
		return m_CreateGameObject;
	}

	// Event Function -------------------------------------------------------------------
	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		m_fDelayTime		/= fSpeedRate;
		m_fRepeatTime		/= fSpeedRate;
	}

	// utility fonction ----------------------------------------------------------------
}

