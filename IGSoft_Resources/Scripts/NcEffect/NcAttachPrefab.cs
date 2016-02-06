// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

//[AddComponentMenu("EsayTool/NcAttachPrefab	%#A")]

public class NcAttachPrefab : NcEffectBehaviour
{
	// Attribute ------------------------------------------------------------------------
 	public		enum			AttachType				{Active, Destroy};
	public		AttachType		m_AttachType			= AttachType.Active;
	public		float			m_fDelayTime;
	public		float			m_fRepeatTime;
	public		int				m_nRepeatCount;

	public		GameObject		m_AttachPrefab;
 	public		float			m_fPrefabSpeed			= 1;
	public		float			m_fPrefabLifeTime		= 0;
 	public		bool			m_bWorldSpace			= false;
	public		Vector3			m_AddStartPos			= Vector3.zero;
	public		Vector3			m_AccumStartRot			= Vector3.zero;
	public		Vector3			m_RandomRange			= Vector3.zero;

	public		int				m_nSpriteFactoryIndex	= -1;

	[HideInInspector]
	public		bool			m_bDetachParent			= false;

	protected	float			m_fStartTime;
	protected	int				m_nCreateCount			= 0;
	protected	bool			m_bStartAttach			= false;
	protected	GameObject[]	m_CreateGameObjects;
	protected	bool			m_bEnabled;

	// Property -------------------------------------------------------------------------
#if UNITY_EDITOR
	public override string CheckProperty()
	{
		if (m_AttachPrefab == null)
			return "SCRIPT_EMPTY_ATTACHPREFAB";
		return "";	// no error
	}
#endif

 	public override int GetAnimationState()
	{
		if (m_bEnabled && (enabled && IsActive(gameObject)) && m_AttachPrefab != null)
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
//		if (m_bEnabled && (enabled/* && IsActive(gameObject)*/) && m_AttachPrefab != null)
			Update();
	}

	public void CreateAttachInstance()
	{
		CreateAttachGameObject();
	}

	public GameObject GetInstanceObject()
	{
		if (m_CreateGameObjects == null)
			UpdateImmediately();
		return ((m_CreateGameObjects == null || m_CreateGameObjects.Length < 1) ? null : m_CreateGameObjects[0]);
	}

	public virtual GameObject[] GetInstanceObjects()
	{
		if (m_CreateGameObjects == null)
			UpdateImmediately();
		return m_CreateGameObjects;
	}

	public void SetEnable(bool bEnable)
	{
		m_bEnabled = bEnable;
	}

	// Loop Function --------------------------------------------------------------------
	protected virtual void Awake()
	{
#if UNITY_EDITOR
		if (IsCreatingEditObject() == true)
			m_bEnabled = false;
		else
#endif
		m_bEnabled = (enabled && IsActive(gameObject) && GetComponent<NcDontActive>() == null);

	}

	protected virtual void Start()
	{
	}

	// Update is called once per frame
	protected virtual void Update()
	{
		if (m_AttachPrefab == null)
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
		if (m_bEnabled && IsSafe() && m_AttachType == AttachType.Destroy && m_AttachPrefab != null)
			CreateAttachPrefab();
		base.OnDestroy();
	}

	// Control Function -----------------------------------------------------------------
	void CreateAttachPrefab()
	{
		CreateAttachGameObject();

		if ((0 == m_fRepeatTime || m_AttachType == AttachType.Destroy) && 0 < m_nRepeatCount && m_nCreateCount < m_nRepeatCount)
			CreateAttachPrefab();
	}

	void CreateAttachGameObject()
	{
		GameObject createObj = (GameObject)CreateGameObject(GetTargetGameObject(), (GetTargetGameObject() == gameObject ? null : transform), m_AttachPrefab);
		if (m_bReplayState)
			NsEffectManager.SetReplayEffect(createObj);

		if (createObj == null)
			return;

		if (m_AttachType == AttachType.Active)
		{
			if (m_CreateGameObjects == null)
				m_CreateGameObjects = new GameObject[Mathf.Max(1, m_nRepeatCount)];
			for (int n = 0; n < m_CreateGameObjects.Length; n++)
				if (m_CreateGameObjects[n] == null)
				{
					m_CreateGameObjects[n] = createObj;
					break;
				}
		}

		m_nCreateCount++;

		// sync Layer
//		Ng_ChangeLayerWithChild(createObj, gameObject.layer);

		// Random pos, AddStartPos
		Vector3	newPos = createObj.transform.position;
		createObj.transform.position = m_AddStartPos + new Vector3(Random.Range(-m_RandomRange.x, m_RandomRange.x)+newPos.x, Random.Range(-m_RandomRange.y, m_RandomRange.y)+newPos.y, Random.Range(-m_RandomRange.z, m_RandomRange.z)+newPos.z);

		// m_AccumStartRot
		createObj.transform.localRotation	*= Quaternion.Euler(m_AccumStartRot.x*m_nCreateCount, m_AccumStartRot.y*m_nCreateCount, m_AccumStartRot.z*m_nCreateCount);
		createObj.name += " " + m_nCreateCount;
		SetActiveRecursively(createObj, true);

		// PrefabAdjustSpeed
 		NsEffectManager.AdjustSpeedRuntime(createObj, m_fPrefabSpeed);

		// m_fPrefabLifeTime
		if (0 < m_fPrefabLifeTime)
		{
			NcAutoDestruct	ncAd = createObj.GetComponent<NcAutoDestruct>();
			if (ncAd == null)
				ncAd = AddNcComponentToObject<NcAutoDestruct>(createObj);
			ncAd.m_fLifeTime = m_fPrefabLifeTime;
		}

		// m_bDetachParent
		if (m_bDetachParent)
		{
			NcDetachParent	detachCom = createObj.GetComponent<NcDetachParent>();
			if (detachCom == null)
				detachCom = AddNcComponentToObject<NcDetachParent>(createObj);
		}

		// m_nSpriteFactoryIndex
		if (0 <= m_nSpriteFactoryIndex)
		{
			NcSpriteFactory ncFactory = createObj.GetComponent<NcSpriteFactory>();
			if (ncFactory)
				ncFactory.SetSprite(m_nSpriteFactoryIndex, false);
		}

		OnCreateAttachGameObject();
	}

	protected virtual void OnCreateAttachGameObject()
	{
	}

	// -------------------------------------------------------------------------------
	GameObject GetTargetGameObject()
	{
		// 월드좌표 고정형이면..
		if (m_bWorldSpace || m_AttachType == AttachType.Destroy)
			 return GetRootInstanceEffect();
		else return this.gameObject;
	}
	// Event Function -------------------------------------------------------------------
	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		m_fDelayTime		/= fSpeedRate;
		m_fRepeatTime		/= fSpeedRate;
		m_fPrefabLifeTime	/= fSpeedRate;
 		m_fPrefabSpeed		*= fSpeedRate;
	}

	public override void OnSetActiveRecursively(bool bActive)
	{
		if (m_CreateGameObjects == null)
			return;
		for (int n = 0; n < m_CreateGameObjects.Length; n++)
			if (m_CreateGameObjects[n] != null)
				NsEffectManager.SetActiveRecursively(m_CreateGameObjects[n], bActive);
	}

	// utility fonction ----------------------------------------------------------------
	// tag명을 모두 바꾼다.
	public static void Ng_ChangeLayerWithChild(GameObject rootObj, int nLayer)
	{
		if (rootObj == null)
			return;
		rootObj.layer = nLayer;
		for (int n = 0; n < rootObj.transform.childCount; n++)
			Ng_ChangeLayerWithChild(rootObj.transform.GetChild(n).gameObject, nLayer);
	}
}

