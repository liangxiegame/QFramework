// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------


using UnityEngine;
using System.Reflection; 
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

using System.Collections;

public class NcParticleSystem : NcEffectBehaviour
{
	// Attribute ------------------------------------------------------------------------
	protected	bool					m_bDisabledEmit					= false;
	public		float					m_fStartDelayTime				= 0;
	public		bool					m_bBurst						= false;
							
	public		float					m_fBurstRepeatTime				= 0.5f;
	public		int						m_nBurstRepeatCount				= 0;
	public		int						m_fBurstEmissionCount			= 10;
							
	public		float					m_fEmitTime						= 0;
	public		float					m_fSleepTime					= 0;
							
	public		bool					m_bScaleWithTransform			= false;
	public		bool					m_bWorldSpace					= true;
	public		float					m_fStartSizeRate				= 1;
	public		float					m_fStartLifeTimeRate			= 1;
	public		float					m_fStartEmissionRate			= 1;
	public		float					m_fStartSpeedRate				= 1;
	public		float					m_fRenderLengthRate				= 1;
							
	public		float					m_fLegacyMinMeshNormalVelocity	= 10;
	public		float					m_fLegacyMaxMeshNormalVelocity	= 10;
							
	public		float					m_fShurikenSpeedRate			= 1.0f;
	protected	bool					m_bStart						= false;
	protected	Vector3					m_OldPos						= Vector3.zero;
	protected	bool					m_bLegacyRuntimeScale			= true;

	public		enum ParticleDestruct	{NONE, COLLISION, WORLD_Y};
	public		ParticleDestruct		m_ParticleDestruct				= ParticleDestruct.NONE;
	public		LayerMask				m_CollisionLayer				= -1;
	public		float					m_fCollisionRadius				= 0.3f;
	public		float					m_fDestructPosY					= 0.2f;
	public		GameObject				m_AttachPrefab;
	public		float					m_fPrefabScale					= 1.0f;
	public		float					m_fPrefabSpeed					= 1;
	public		float					m_fPrefabLifeTime				= 2;
							
	protected	bool					m_bSleep						= false;
	protected	float					m_fStartTime					= 0;
	protected	float					m_fDurationStartTime			= 0;
	protected	float					m_fEmitStartTime				= 0;
	protected	int						m_nCreateCount					= 0;
	protected	bool					m_bScalePreRender				= false;
	protected	bool					m_bMeshParticleEmitter			= false;

 	protected	ParticleSystem			m_ps;
	protected	ParticleEmitter			m_pe;
	protected	ParticleAnimator		m_pa;
	protected	ParticleRenderer		m_pr;

	protected	ParticleSystem.Particle[]	m_BufPsParts;
	protected	ParticleSystem.Particle[]	m_BufColliderOriParts;
	protected	ParticleSystem.Particle[]	m_BufColliderConParts;
	protected	const int					m_nAllocBufCount	= 50;


	// Property -------------------------------------------------------------------------
	public void SetDisableEmit()
	{
		m_bDisabledEmit = true;
	}

	public bool IsShuriken()
	{
		return GetComponent<ParticleSystem>() != null;
	}

	public bool IsLegacy()
	{
		return GetComponent<ParticleEmitter>() != null && GetComponent<ParticleEmitter>().enabled;
	}

#if UNITY_EDITOR
	public override string CheckProperty()
	{
		if (1 < gameObject.GetComponents(GetType()).Length)
			return "SCRIPT_WARRING_DUPLICATE";

		if (GetComponent<ParticleSystem>() == null && GetComponent<ParticleEmitter>() == null)
			return "SCRIPT_EMPTY_PARTICLE";

// 		if (m_ParticleDestruct != ParticleDestruct.NONE && m_AttachPrefab == null)
// 			return "SCRIPT_EMPTY_ATTACHPREFAB";

		return "";	// no error
	}
#endif

	public override int GetAnimationState()
	{
		if (enabled == false || IsActive(gameObject) == false)
			return -1;

		if (m_bBurst)
		{
			if (0 < m_nBurstRepeatCount)
			{
				if (m_nCreateCount < m_nBurstRepeatCount)
					 return 1;
				else return 0;
			}
			return 1;
		} else {
			if (0 < m_fStartDelayTime)
				return 1;
			if (0 < m_fEmitTime && m_fSleepTime <= 0)
			{
				if (m_nCreateCount < 1)
					return 1;
				else return 0;
			}
		}
		return -1;	// loop
	}

	public bool IsMeshParticleEmitter()
	{
		return m_bMeshParticleEmitter;
	}

	public void ResetParticleEmit(bool bClearOldParticle)
	{
		m_bDisabledEmit		= false;
		m_OldPos			= transform.position;
		Init();

		if (bClearOldParticle)
		{
			// 			if (m_ps != null)
			// 				m_ps.Clear(false);
			if (m_pe != null)
				m_pe.ClearParticles();
		}
		if (m_bBurst || 0 < m_fStartDelayTime)
			SetEnableParticle(false);
		else SetEnableParticle(true);
	}

	// Loop Function --------------------------------------------------------------------
	void Awake()
	{
// 		particleEmitter.emit = false;
		if (IsShuriken())
		{
			m_ps = GetComponent<ParticleSystem>();
		} else {
			m_pe = GetComponent<ParticleEmitter>();
			m_pa = GetComponent<ParticleAnimator>();
			m_pr = GetComponent<ParticleRenderer>();

			if (m_pe != null)
				m_bMeshParticleEmitter	= (m_pe.ToString().Contains("MeshParticleEmitter"));
		}
	}

	void OnEnable()
	{
		if (m_bScaleWithTransform)
		{
// 			if (Camera.main == null)
// 				return;
			AddRenderEventCall();
		}
		m_OldPos = transform.position;
	}

	void OnDisable()
	{
		if (m_bScaleWithTransform)
			RemoveRenderEventCall();
	}

	void Init()
	{
		m_bSleep			= false;
		m_nCreateCount		= 0;
		m_fStartTime		= GetEngineTime();
		m_fEmitStartTime	= 0;
	}

	void Start()
	{
#if UNITY_EDITOR
		Debug.LogWarning("Waring!!! FXMaker - NcParticleSystem.cs->m_bScaleWithTransform : This option is very slow. (see : Assets/IGSoft_Tools/FXMaker/Readme/Optimization)");
#endif

		if (m_bDisabledEmit)
			return;

		m_bStart = true;
		Init();

		if (IsShuriken())
			ShurikenInitParticle();
		else LegacyInitParticle();
		if (m_bBurst || 0 < m_fStartDelayTime)
			SetEnableParticle(false);
// 		if (m_bBurst == false && m_fStartDelayTime == 0)
// 			SetEnableParticle(true);
	}

	void Update()
	{
		if (m_bDisabledEmit)
			return;

		if (m_fEmitStartTime == 0)
		{
			// delay start
			if (0 < m_fStartDelayTime)
			{
				if (m_fStartTime + m_fStartDelayTime <= GetEngineTime())
				{
					m_fEmitStartTime	= GetEngineTime();
					m_fDurationStartTime= GetEngineTime();
					SetEnableParticle(true);
				}
				return;
			} else {
				m_fEmitStartTime	= GetEngineTime();
				m_fDurationStartTime= GetEngineTime();
			}
		}

		// burst repeat
		if (m_bBurst)
		{
			if (m_fDurationStartTime <= GetEngineTime())
			{
				if ((m_nBurstRepeatCount == 0 || m_nCreateCount < m_nBurstRepeatCount))		// check repeat count
				{
//					SetEnableParticle(true);
					m_fDurationStartTime = m_fBurstRepeatTime + GetEngineTime();
					m_nCreateCount++;
					if (IsShuriken())
						m_ps.Emit(m_fBurstEmissionCount);
					else if (m_pe != null) m_pe.Emit(m_fBurstEmissionCount);
				}
			} else {
//				SetEnableParticle(false);
			}
		} else {
			if (m_bSleep)
			{
				if (m_fEmitStartTime + m_fEmitTime + m_fSleepTime < GetEngineTime())
				{
					SetEnableParticle(true);
					m_fEmitStartTime = GetEngineTime();
					m_bSleep = false;
				}
			} else {
				// Emit End
				if (0 < m_fEmitTime && m_fEmitStartTime + m_fEmitTime < GetEngineTime())
				{
					m_nCreateCount++;
					SetEnableParticle(false);
					if (0 < m_fSleepTime)
						m_bSleep = true;
					else SetDisableEmit();
				}
			}
		}
	}

	void FixedUpdate()
	{
		// Check Collider
		if (m_ParticleDestruct != ParticleDestruct.NONE)
		{
			bool	bUpdate		= false;
			Vector3	pos;

			if (IsShuriken())
			{
				if (m_ps != null)
				{
					AllocateParticleSystem(ref m_BufColliderOriParts);
					AllocateParticleSystem(ref m_BufColliderConParts);
					m_ps.GetParticles(m_BufColliderOriParts);
					m_ps.GetParticles(m_BufColliderConParts);
					ShurikenScaleParticle(m_BufColliderConParts, m_ps.particleCount, m_bScaleWithTransform, true);

					// Check Collider
					for (int n = 0; n < m_ps.particleCount; n++)
					{
						bool bDestect = false;
						if (m_bWorldSpace)
							 pos = m_BufColliderConParts[n].position;
						else pos = transform.TransformPoint(m_BufColliderConParts[n].position);

						if (m_ParticleDestruct == ParticleDestruct.COLLISION)
						{
#if UNITY_EDITOR
							Collider[]	colls = Physics.OverlapSphere(pos, m_fCollisionRadius, m_CollisionLayer);
							foreach (Collider coll in colls)
							{
								if (coll.gameObject.GetComponent("FxmInfoIndexing") != null)
									continue;
								bDestect = true;
								break;
							}
#else
							if (Physics.CheckSphere(pos, m_fCollisionRadius, m_CollisionLayer))
								bDestect = true;
#endif
						} else
						if (m_ParticleDestruct == ParticleDestruct.WORLD_Y && pos.y <= m_fDestructPosY)
							bDestect = true;

						if (bDestect && 0 < m_BufColliderOriParts[n].lifetime)
						{
							m_BufColliderOriParts[n].lifetime = 0.0f;
							bUpdate				 = true;
							CreateAttachPrefab(pos, m_BufColliderConParts[n].size * m_fPrefabScale);
						}
					}
					if (bUpdate)
						m_ps.SetParticles(m_BufColliderOriParts, m_ps.particleCount);
				}
			} else {
				if (m_pe != null)
				{
					Particle[]	oriParts = m_pe.particles;
					Particle[]	conParts = m_pe.particles;
					LegacyScaleParticle(conParts, m_bScaleWithTransform, true);

					// Check Collider
					for (int n = 0; n < conParts.Length; n++)
					{
						bool bDestect = false;
						if (m_bWorldSpace)
							 pos = conParts[n].position;
						else pos = transform.TransformPoint(conParts[n].position);

						if (m_ParticleDestruct == ParticleDestruct.COLLISION)
						{
#if UNITY_EDITOR
							Collider[]	colls = Physics.OverlapSphere(pos, m_fCollisionRadius, m_CollisionLayer);
							foreach (Collider coll in colls)
							{
								if (coll.gameObject.GetComponent("FxmInfoIndexing") != null)
									continue;
								bDestect = true;
								break;
							}
#else
							if (Physics.CheckSphere(pos, m_fCollisionRadius, m_CollisionLayer))
								bDestect = true;
#endif
						} else
						if (m_ParticleDestruct == ParticleDestruct.WORLD_Y && pos.y <= m_fDestructPosY)
							bDestect = true;

						if (bDestect && 0 < oriParts[n].energy)
						{
							oriParts[n].energy	= 0.0f;
							bUpdate				= true;
							CreateAttachPrefab(pos, conParts[n].size * m_fPrefabScale);
						}
					}
					if (bUpdate)
						m_pe.particles = oriParts;
				}
			}
		}
	}

	void OnPreRender()
	{
		if (!m_bStart)
			return;
		if (m_bScaleWithTransform)
		{
			m_bScalePreRender = true;
			if (IsShuriken())
				 ShurikenSetRuntimeParticleScale(true);
			else LegacySetRuntimeParticleScale(true);
		}
	}

	void OnPostRender()
	{
		if (!m_bStart)
			return;
		if (m_bScalePreRender)
		{
			if (IsShuriken())
				 ShurikenSetRuntimeParticleScale(false);
			else LegacySetRuntimeParticleScale(false);
		}
		m_OldPos = transform.position;
		m_bScalePreRender = false;
	}

	// Control Function -----------------------------------------------------------------
	void CreateAttachPrefab(Vector3 position, float size)
	{
		if (m_AttachPrefab == null)
			return;
		GameObject createGameObject = (GameObject)CreateGameObject(m_AttachPrefab, m_AttachPrefab.transform.position + position, m_AttachPrefab.transform.rotation);
		if (createGameObject == null)
			return;
		// Change Parent
		ChangeParent(GetRootInstanceEffect().transform, createGameObject.transform, false, null);
		NcTransformTool.CopyLossyToLocalScale(createGameObject.transform.lossyScale * size, createGameObject.transform);

		// PrefabAdjustSpeed
		NsEffectManager.AdjustSpeedRuntime(createGameObject, m_fPrefabSpeed);

		// m_fPrefabLifeTime
		if (0 < m_fPrefabLifeTime)
		{
			NcAutoDestruct	ncAd = createGameObject.GetComponent<NcAutoDestruct>();
			if (ncAd == null)
				ncAd = createGameObject.AddComponent<NcAutoDestruct>();
			ncAd.m_fLifeTime = m_fPrefabLifeTime;
		}
	}

	void AddRenderEventCall()
	{
		foreach (Camera cam in Camera.allCameras)
		{
			NsRenderManager nsr = cam.GetComponent<NsRenderManager>();
			if (nsr == null)
				nsr = cam.gameObject.AddComponent<NsRenderManager>();
			nsr.AddRenderEventCall(this);
		}
	}

	void RemoveRenderEventCall()
	{
		foreach (Camera cam in Camera.allCameras)
		{
			NsRenderManager nsr = cam.GetComponent<NsRenderManager>();
			if (nsr != null)
				nsr.RemoveRenderEventCall(this);
		}
	}

	// Common ----------------------------------------------------------
	void SetEnableParticle(bool bEnable)
	{
// 		Debug.Log("SetEnableParticle");
		if (m_ps != null)
			m_ps.enableEmission = bEnable;
		if (m_pe != null)
			m_pe.emit = bEnable;
	}

	void ClearParticle()
	{
		if (m_ps != null)
			m_ps.Clear(false);
		if (m_pe != null)
			m_pe.ClearParticles();
	}

	// Legacy ----------------------------------------------------------
	public float GetScaleMinMeshNormalVelocity()
	{
		return m_fLegacyMinMeshNormalVelocity * (m_bScaleWithTransform ? NcTransformTool.GetTransformScaleMeanValue(transform) : 1);
	}

	public float GetScaleMaxMeshNormalVelocity()
	{
		return m_fLegacyMaxMeshNormalVelocity * (m_bScaleWithTransform ? NcTransformTool.GetTransformScaleMeanValue(transform) : 1);
	}

	void LegacyInitParticle()
	{
		if (m_pe != null)
			LegacySetParticle();
	}

	void LegacySetParticle()
	{
		ParticleEmitter		pe = m_pe;
		ParticleAnimator	pa = m_pa;
		ParticleRenderer	pr = m_pr;

		if (pe == null || pr == null)
			return;

		if (m_bLegacyRuntimeScale)
		{
			Vector3 vecVelScale	= Vector3.one * m_fStartSpeedRate;
			float fVelScale		= m_fStartSpeedRate;

			pe.minSize					*= m_fStartSizeRate;
			pe.maxSize					*= m_fStartSizeRate;
			pe.minEnergy				*= m_fStartLifeTimeRate;
			pe.maxEnergy				*= m_fStartLifeTimeRate;
			pe.minEmission				*= m_fStartEmissionRate;
			pe.maxEmission				*= m_fStartEmissionRate;

			pe.worldVelocity			=  Vector3.Scale(pe.worldVelocity, vecVelScale);
			pe.localVelocity			=  Vector3.Scale(pe.localVelocity, vecVelScale);
			pe.rndVelocity				=  Vector3.Scale(pe.rndVelocity, vecVelScale);
			pe.angularVelocity			*= fVelScale;
			pe.rndAngularVelocity		*= fVelScale;
			pe.emitterVelocityScale		*= fVelScale;

//  		NgAssembly.LogFieldsPropertis(pe);

			if (pa != null)
			{
				pa.rndForce					=  Vector3.Scale(pa.rndForce, vecVelScale);
				pa.force					=  Vector3.Scale(pa.force, vecVelScale);
//				pa.damping					*= fScale;
			}

// 			pr.velocityScale			*= fVelScale;
			pr.lengthScale				*= m_fRenderLengthRate;
		} else {
			Vector3 vecVelScale	= (m_bScaleWithTransform ? pe.transform.lossyScale : Vector3.one) * m_fStartSpeedRate;
			float fVelScale		= (m_bScaleWithTransform ? NcTransformTool.GetTransformScaleMeanValue(pe.transform) : 1) * m_fStartSpeedRate;
			float fScale		= (m_bScaleWithTransform ? NcTransformTool.GetTransformScaleMeanValue(pe.transform) : 1) * m_fStartSizeRate;

			pe.minSize					*= fScale;
			pe.maxSize					*= fScale;
			pe.minEnergy				*= m_fStartLifeTimeRate;
			pe.maxEnergy				*= m_fStartLifeTimeRate;
			pe.minEmission				*= m_fStartEmissionRate;
			pe.maxEmission				*= m_fStartEmissionRate;

			pe.worldVelocity			=  Vector3.Scale(pe.worldVelocity, vecVelScale);
			pe.localVelocity			=  Vector3.Scale(pe.localVelocity, vecVelScale);
			pe.rndVelocity				=  Vector3.Scale(pe.rndVelocity, vecVelScale);
			pe.angularVelocity			*= fVelScale;
			pe.rndAngularVelocity		*= fVelScale;
			pe.emitterVelocityScale		*= fVelScale;

//  		NgAssembly.LogFieldsPropertis(pe);

			if (pa != null)
			{
				pa.rndForce					=  Vector3.Scale(pa.rndForce, vecVelScale);
				pa.force					=  Vector3.Scale(pa.force, vecVelScale);
//				pa.damping					*= fScale;
			}

// 			pr.velocityScale			*= fVelScale;
			pr.lengthScale				*= m_fRenderLengthRate;
		}
	}

	void LegacyParticleSpeed(float fSpeed)
	{
		ParticleEmitter		pe = m_pe;
		ParticleAnimator	pa = m_pa;
		ParticleRenderer	pr = m_pr;

		if (pe == null || pr == null)
			return;

		Vector3 vecSpeed	= Vector3.one * fSpeed;

		pe.minEnergy				/= fSpeed;
		pe.maxEnergy				/= fSpeed;

		pe.worldVelocity			=  Vector3.Scale(pe.worldVelocity, vecSpeed);
		pe.localVelocity			=  Vector3.Scale(pe.localVelocity, vecSpeed);
		pe.rndVelocity				=  Vector3.Scale(pe.rndVelocity, vecSpeed);
		pe.angularVelocity			*= fSpeed;
		pe.rndAngularVelocity		*= fSpeed;
		pe.emitterVelocityScale		*= fSpeed;

		if (pa != null)
		{
			pa.rndForce				=  Vector3.Scale(pa.rndForce, vecSpeed);
			pa.force				=  Vector3.Scale(pa.force, vecSpeed);
//			pa.damping				*= fScale;
		}
//		pr.velocityScale			*= fSpeed;
	}

	// Runtime Scale ----------------------------------------------
	void LegacySetRuntimeParticleScale(bool bScale)
	{
		if (m_bLegacyRuntimeScale == false)
			return;

		if (m_pe != null)
		{
			Particle[]	parts = m_pe.particles;
			m_pe.particles = LegacyScaleParticle(parts, bScale, true);
		}
	}

	public Particle[] LegacyScaleParticle(Particle[] parts, bool bScale, bool bPosUpdate)
	{
		float fScale;

		if (bScale)
			 fScale = NcTransformTool.GetTransformScaleMeanValue(transform);
		else fScale = 1 / NcTransformTool.GetTransformScaleMeanValue(transform);

		for (int n = 0; n < parts.Length; n++)
		{
 			if (IsMeshParticleEmitter() == false)
			{
				if (m_bWorldSpace)
				{
					if (bPosUpdate)
					{
						Vector3 move =  (m_OldPos - transform.position);
						if (bScale)
							parts[n].position -= move * (1 - 1/fScale);
					}
					parts[n].position -= transform.position;
					parts[n].position *= fScale;
					parts[n].position += transform.position;
				} else {
	 				parts[n].position *= fScale;
				}
			}
			parts[n].angularVelocity *= fScale;
			parts[n].velocity *= fScale;
			parts[n].size *= fScale;
		}
		return parts;
	}

	// Shuriken ---------------------------------------------------------------------------
	void ShurikenInitParticle()
	{
		if (m_ps != null)
		{
// 			m_ps.startDelay		+= m_fStartDelayTime;
			m_ps.startSize		*= m_fStartSizeRate;
			m_ps.startLifetime	*= m_fStartLifeTimeRate;
			m_ps.emissionRate	*= m_fStartEmissionRate;
			m_ps.startSpeed		*= m_fStartSpeedRate;

//			iphone error!!!
// 			ParticleSystemRenderer	ren = GetComponent<ParticleSystemRenderer>();
// 			if (ren != null)
// 			{
// 				float fRenderLength	= (float)Ng_GetProperty(ren, "lengthScale");
// 				Ng_SetProperty(ren, "lengthScale", fRenderLength * m_fRenderLengthRate);
// 			}
		}
	}

	void AllocateParticleSystem(ref ParticleSystem.Particle[] tmpPsParts)
	{
		if (tmpPsParts == null || tmpPsParts.Length < m_ps.particleCount)
			tmpPsParts = new ParticleSystem.Particle[m_ps.particleCount + m_nAllocBufCount];
	}

	void ShurikenSetRuntimeParticleScale(bool bScale)
	{
		if (m_ps != null)
		{
			AllocateParticleSystem(ref m_BufPsParts);
//			ParticleSystem.Particle[]	m_tmpPsParts = new ParticleSystem.Particle[m_ps.particleCount];

			m_ps.GetParticles(m_BufPsParts);

			m_BufPsParts = ShurikenScaleParticle(m_BufPsParts, m_ps.particleCount, bScale, true);
			m_ps.SetParticles(m_BufPsParts, m_ps.particleCount);
		}
	}

	public ParticleSystem.Particle[] ShurikenScaleParticle(ParticleSystem.Particle[] parts, int nCount, bool bScale, bool bPosUpdate)
	{
		float fScale;
		if (bScale)
			fScale = NcTransformTool.GetTransformScaleMeanValue(transform);
		else fScale = 1 / NcTransformTool.GetTransformScaleMeanValue(transform);

		for (int n = 0; n < nCount; n++)
		{
			if (m_bWorldSpace)
			{
				if (bPosUpdate)
				{
					Vector3 move =  (m_OldPos - transform.position);
					if (bScale)
						parts[n].position -= move * (1 - 1/fScale);
				}
				parts[n].position -= transform.position;
				parts[n].position *= fScale;
				parts[n].position += transform.position;

			} else parts[n].position *= fScale;
// 			parts[n].angularVelocity *= fScale;
			parts[n].size *= fScale;
// 			parts[n].velocity *= fScale;
		}
		return parts;
	}

	// Event Function -------------------------------------------------------------------
	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		m_fStartDelayTime		/= fSpeedRate;
		m_fBurstRepeatTime		/= fSpeedRate;
		m_fEmitTime				/= fSpeedRate;
		m_fSleepTime			/= fSpeedRate;
		m_fShurikenSpeedRate	*= fSpeedRate;
		LegacyParticleSpeed(fSpeedRate);
//	 	m_fStartLifeTimeRate	= 1;
// 		m_fStartEmissionRate	= 1;
//		m_fStartSpeedRate		= 1;

		m_fPrefabLifeTime		/= fSpeedRate;
		m_fPrefabSpeed			*= fSpeedRate;

#if UNITY_EDITOR
		if (bRuntime == false)
			SaveShurikenSpeed();
#endif
	}

	public override void OnSetReplayState()
	{
		base.OnSetReplayState();
		m_pe = GetComponent<ParticleEmitter>();
		m_pa = GetComponent<ParticleAnimator>();
		if (m_pa != null)
			m_pa.autodestruct	= false;
	}

	public override void OnResetReplayStage(bool bClearOldParticle)
	{
		base.OnResetReplayStage(bClearOldParticle);
		ResetParticleEmit(bClearOldParticle);
	}

	// utility fonction ----------------------------------------------------------------
	public static void Ng_SetProperty(object srcObj, string fieldName, object newValue)
	{
		PropertyInfo info = srcObj.GetType().GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
		if (info != null && info.CanWrite)
			info.SetValue(srcObj, newValue, null);
		else Debug.LogWarning(fieldName + " could not be write.");
	}

	public static object Ng_GetProperty(object srcObj, string fieldName)
	{
		object ret = null;
		PropertyInfo info = srcObj.GetType().GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
		if (info != null && info.CanRead && info.GetIndexParameters().Length == 0)
		{
			ret = info.GetValue(srcObj, null);
		} else Debug.LogWarning(fieldName + " could not be read.");
		return ret;
	}

	// sync fonction ----------------------------------------------------------------
#if UNITY_EDITOR
	public void SaveShurikenSpeed()
	{
		// Set particleSystem.speed
		if (GetComponent<ParticleSystem>() != null)
		{
			SerializedObject sysSo = new SerializedObject(GetComponent<ParticleSystem>());
			SetPropertyValue(sysSo, "speed", m_fShurikenSpeedRate, true);
		}
	}

	public static void SetPropertyValue(SerializedObject serObj, string propertyName, object value, bool bUpdate)
	{
		SerializedProperty sp = serObj.FindProperty(propertyName);
		if (sp == null)
		{
			Debug.LogError("SetPropertyValue error - " + propertyName);
			return;
		}
		SetPropertyValue(sp, value);
		if (bUpdate)
		{
 			if (serObj.ApplyModifiedProperties() == false)
 			{
//				Debug.LogWarning("Failed - ApplyModifiedProperties " + propertyName);		// dup
			}
		}
	}

	public static void SetPropertyValue(SerializedProperty sp, object value)
	{
		switch (sp.propertyType)
		{
			case SerializedPropertyType.Integer			:	sp.intValue		= (int)value;			 break;
			case SerializedPropertyType.Boolean			:	sp.boolValue	= (bool)value;			 break;
			case SerializedPropertyType.Float			:	sp.floatValue	= (float)value;			 break;
			case SerializedPropertyType.String			:	sp.stringValue	= (string)value;		 break;
			case SerializedPropertyType.Color			:	sp.colorValue	= (Color)value;			 break;
			case SerializedPropertyType.AnimationCurve	:	sp.animationCurveValue	= (AnimationCurve)value;	break;
			case SerializedPropertyType.ObjectReference	:	sp.objectReferenceValue	= (Object)value;			break;
			case SerializedPropertyType.Enum			:	sp.enumValueIndex = (int)value;			 break;
			case SerializedPropertyType.Vector2			:	sp.vector2Value	= (Vector2)value;		 break;
			case SerializedPropertyType.Vector3			:	sp.vector3Value	= (Vector3)value;		 break;
			case SerializedPropertyType.Rect			:	sp.rectValue	= (Rect)value;			 break;
			case SerializedPropertyType.Bounds			:	sp.boundsValue	= (Bounds)value;		 break;
// 			default: Debug.Log("Undefile propertyType - SetPropertyValue " + sp.name); break;
		}
	}

#endif
}
