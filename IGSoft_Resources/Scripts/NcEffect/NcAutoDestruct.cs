// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

// [AddComponentMenu("FXMaker/NcAutoDestruct	%#F")]

public class NcAutoDestruct : NcEffectBehaviour
{
	// Attribute ------------------------------------------------------------------------
	public		float				m_fLifeTime					= 2;
	public		float				m_fSmoothDestroyTime		= 0;
	public		bool				m_bDisableEmit				= true;
	public		bool				m_bSmoothHide				= true;
	public		bool				m_bMeshFilterOnlySmoothHide	= false;
 	protected	bool				m_bEndNcCurveAnimation		= false;

	public		enum CollisionType	{NONE, COLLISION, WORLD_Y};
	public		CollisionType		m_CollisionType				= CollisionType.NONE;
	public		LayerMask			m_CollisionLayer			= -1;
	public		float				m_fCollisionRadius			= 0.3f;
	public		float				m_fDestructPosY				= 0.2f;

	// read only
	protected	float				m_fStartTime				= 0;
	protected	float				m_fStartDestroyTime;
	protected	NcCurveAnimation	m_NcCurveAnimation;

	// Create ---------------------------------------------------------------------------
	public static NcAutoDestruct CreateAutoDestruct(GameObject baseGameObject, float fLifeTime, float fDestroyTime, bool bSmoothHide, bool bMeshFilterOnlySmoothHide)
	{
		NcAutoDestruct ncAutoDestruct = baseGameObject.AddComponent<NcAutoDestruct>();
		ncAutoDestruct.m_fLifeTime					= fLifeTime;
		ncAutoDestruct.m_fSmoothDestroyTime			= fDestroyTime;
		ncAutoDestruct.m_bSmoothHide				= bSmoothHide;
		ncAutoDestruct.m_bMeshFilterOnlySmoothHide	= bMeshFilterOnlySmoothHide;
		if (IsActive(baseGameObject))
		{
			ncAutoDestruct.Start();
			ncAutoDestruct.Update();
		}
		return ncAutoDestruct;
	}

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
 		m_bEndNcCurveAnimation	= false;	// disable

		m_fStartTime			= 0;
		m_NcCurveAnimation		= null;
	}

	void Start()
	{
		m_fStartTime = GetEngineTime();
		if (m_bEndNcCurveAnimation)
			m_NcCurveAnimation = GetComponent<NcCurveAnimation>();
	}

	void Update()
	{
		if (0 < m_fStartDestroyTime)
		{
			if (0 < m_fSmoothDestroyTime)
			{
				if (m_bSmoothHide)
				{
					float fAlphaRate = 1 - ((GetEngineTime() - m_fStartDestroyTime) / m_fSmoothDestroyTime);
					if (fAlphaRate < 0)
						fAlphaRate = 0;

					if (m_bMeshFilterOnlySmoothHide)
					{
						// Recursively
						MeshFilter[] meshFilters = transform.GetComponentsInChildren<MeshFilter>(true);
						Color		color;
						for (int n = 0; n < meshFilters.Length; n++)
						{
							Color[]	colors	= meshFilters[n].mesh.colors;
							if (colors.Length == 0)
							{
								colors = new Color[meshFilters[n].mesh.vertices.Length];
								for (int c = 0; c < colors.Length; c++)
									colors[c] = Color.white;
							}
							for (int c = 0; c < colors.Length; c++)
							{
								color		= colors[c];
								color.a		= Mathf.Min(color.a, fAlphaRate);
								colors[c]	= color;
							}
							meshFilters[n].mesh.colors	= colors;
						}
					} else {
						// Recursively
						Renderer[] rens = transform.GetComponentsInChildren<Renderer>(true);
						for (int n = 0; n < rens.Length; n++)
						{
							Renderer	ren		= rens[n];
							string		colName	= GetMaterialColorName(ren.sharedMaterial);

							if (colName != null)
							{
								Color col	= ren.material.GetColor(colName);
								col.a		= Mathf.Min(col.a, fAlphaRate);
								ren.material.SetColor(colName, col);
// 								AddRuntimeMaterial(ren.material);
							}
						}
					}
				}
				if (m_fStartDestroyTime + m_fSmoothDestroyTime < GetEngineTime())
					AutoDestruct();
			}
		} else {
			// Time
// 			if (0 < m_fStartTime && m_fLifeTime != 0)
			if (0 < m_fStartTime)
			{
				if (m_fStartTime + m_fLifeTime <= GetEngineTime())
					StartDestroy();
			}

			// event
			if (m_bEndNcCurveAnimation && m_NcCurveAnimation != null)
				if (1 <= m_NcCurveAnimation.GetElapsedRate())
					StartDestroy();
		}
	}

	void FixedUpdate()
	{
		if (0 < m_fStartDestroyTime)
			return;
		bool bDestect = false;

		if (m_CollisionType == CollisionType.NONE)
			return;

		if (m_CollisionType == CollisionType.COLLISION)
		{
#if UNITY_EDITOR
			Collider[]	colls = Physics.OverlapSphere(transform.position, m_fCollisionRadius, m_CollisionLayer);
			foreach (Collider coll in colls)
			{
				if (coll.gameObject.GetComponent("FxmInfoIndexing") != null)
					continue;
				bDestect = true;
				break;
			}
#else
			if (Physics.CheckSphere(transform.position, m_fCollisionRadius, m_CollisionLayer))
				bDestect = true;
#endif
		} else
		if (m_CollisionType == CollisionType.WORLD_Y && transform.position.y <= m_fDestructPosY)
			bDestect = true;

		if (bDestect)
			StartDestroy();
	}

	// Control Function -----------------------------------------------------------------
	void StartDestroy()
	{
		if (m_fSmoothDestroyTime <= 0)
			AutoDestruct();
		else {
			m_fStartDestroyTime	= GetEngineTime();
			if (m_bDisableEmit)
				DisableEmit();
		}
	}

	// Event Function -------------------------------------------------------------------
	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		m_fLifeTime				/= fSpeedRate;
		m_fSmoothDestroyTime	/= fSpeedRate;
	}

	public override void OnSetReplayState()
	{
		base.OnSetReplayState();

		// Backup InitColor
		if (0 < m_fSmoothDestroyTime && m_bSmoothHide)
		{
			m_NcEffectInitBackup = new NcEffectInitBackup();
			if (m_bMeshFilterOnlySmoothHide)
				 m_NcEffectInitBackup.BackupMeshColor(gameObject, true);
			else m_NcEffectInitBackup.BackupMaterialColor(gameObject, true);
		}
	}

	public override void OnResetReplayStage(bool bClearOldParticle)
	{
		base.OnResetReplayStage(bClearOldParticle);
		m_fStartTime		= GetEngineTime();
		m_fStartDestroyTime	= 0;

		// Restore InitColor
		if (0 < m_fSmoothDestroyTime && m_bSmoothHide && m_NcEffectInitBackup != null)
		{
			if (m_bMeshFilterOnlySmoothHide)
				 m_NcEffectInitBackup.RestoreMeshColor();
			else m_NcEffectInitBackup.RestoreMaterialColor();
		}
	}

	void AutoDestruct()
	{
		if (m_bReplayState)
			 SetActiveRecursively(gameObject, false);
		else DestroyObject(gameObject);
	}
}
