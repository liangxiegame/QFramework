// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class NcEffectBehaviour : MonoBehaviour
{
	public class _RuntimeIntance
	{
		public GameObject	m_ParentGameObject;
		public GameObject	m_ChildGameObject;
		public _RuntimeIntance(GameObject	parentGameObject, GameObject childGameObject)
		{
			m_ParentGameObject	= parentGameObject;
			m_ChildGameObject	= childGameObject;
		}
	}

	// Attribute ------------------------------------------------------------------------
	private	static	bool				m_bShuttingDown		= false;
	private	static	GameObject			m_RootInstance;
	public			float				m_fUserTag;
	protected		MeshFilter			m_MeshFilter;
	protected		List<Material>		m_RuntimeMaterials;
	protected		bool				m_bReplayState		= false;
	protected		NcEffectInitBackup	m_NcEffectInitBackup;

	public NcEffectBehaviour()
	{
		m_MeshFilter	= null;
	}

	// Property -------------------------------------------------------------------------
	public static float GetEngineTime()
	{
		if (Time.time == 0)
			return 0.000001f;
		return Time.time;
	}

	public static float GetEngineDeltaTime()
	{
		return Time.deltaTime;
	}

#if UNITY_EDITOR
	public virtual string CheckProperty()
	{
		return "";	// no error
	}

	protected bool IsCreatingEditObject()
	{
		GameObject	main	= GameObject.Find("_FXMaker");
		if (main == null)
			return false;
		GameObject	effroot	= FindRootEditorEffect();
		if (effroot == null)
			return false;
		return (effroot.transform.childCount == 0);
	}

	protected GameObject GetFXMakerGameObject()
	{
		return GameObject.Find("_FXMaker");
	}

	public static GameObject FindRootEditorEffect()
	{
		GameObject	parentObj = GameObject.Find("_CurrentObject");
		return parentObj;
	}

// 	protected FXMakerMain GetFXMakerMain()
// 	{
// 		// find toolmain
// 		GameObject fxMaker = GameObject.Find("_FXMaker");
// 		if (Application.isPlaying && fxMaker != null)
// 			return fxMaker.GetComponent<FXMakerMain>();
// 		return null;
// 	}

	protected int GetEditingUvComponentCount()
	{
		int nCount = 0;
		if (GetComponent<NcSpriteAnimation>() != null)
			nCount++;
		if (GetComponent<NcUvAnimation>() != null)
			nCount++;
		if (GetComponent<NcTilingTexture>() != null)
			nCount++;
		if (GetComponent<NcSpriteTexture>() != null)
			nCount++;
		return nCount;
	}
#endif
	public virtual int GetAnimationState()	// 1 = ing, 0 = stop, -1 = none
	{
		return -1;
	}


	public static GameObject GetRootInstanceEffect()
	{
		if (IsSafe() == false)
			return null;
		if (m_RootInstance == null)
		{
			m_RootInstance = GameObject.Find("_InstanceObject");
			if (m_RootInstance == null)
				m_RootInstance = new GameObject("_InstanceObject");
		}
		return m_RootInstance;
	}

	public static Texture[] PreloadTexture(GameObject tarObj)
	{
		return NsEffectManager.PreloadResource(tarObj);
	}

	protected static void SetActive(GameObject target, bool bActive)
	{
#if (!UNITY_3_5)
		target.SetActive(bActive);
#else
		target.active = bActive;
#endif
	}

	protected static void SetActiveRecursively(GameObject target, bool bActive)
	{
#if (!UNITY_3_5)
		for (int n = target.transform.childCount-1; 0 <= n; n--)
			if (n < target.transform.childCount)
				SetActiveRecursively(target.transform.GetChild(n).gameObject, bActive);
		target.SetActive(bActive);
#else
		target.SetActiveRecursively(bActive);
#endif
	}

	protected static bool IsActive(GameObject target)
	{
#if (!UNITY_3_5)
		return (target.activeInHierarchy && target.activeSelf);
#else
		return target.active;
#endif
	}

	protected static void RemoveAllChildObject(GameObject parent, bool bImmediate)
	{
		for (int n = parent.transform.childCount-1; 0 <= n; n--)
		{
			if (n < parent.transform.childCount)
			{
				Transform	obj = parent.transform.GetChild(n);
				if (bImmediate)
					Object.DestroyImmediate(obj.gameObject);
				else Object.Destroy(obj.gameObject);
			}
// 			obj.parent = null;
// 			Object.Destroy(obj.gameObject);
		}
	}

	public static void HideNcDelayActive(GameObject tarObj)
	{
		SetActiveRecursively(tarObj, false);
/*
		NcDelayActive[]	coms = tarObj.GetComponentsInChildren<NcDelayActive>(true);
		foreach (NcDelayActive com in coms)
			com.CancelDelayActive();
*/
	}

	// RuntimeMaterials
	protected void AddRuntimeMaterial(Material addMaterial)
	{
		if (m_RuntimeMaterials == null)
			m_RuntimeMaterials = new List<Material>();
		if (m_RuntimeMaterials.Contains(addMaterial) == false)
			m_RuntimeMaterials.Add(addMaterial);
	}

	// ----------------------------------------------------------------------------------------------
	public static string GetMaterialColorName(Material mat)
	{
		string[] propertyNames = { "_Color", "_TintColor", "_EmisColor" };

		if (mat != null)
			foreach (string name in propertyNames)
				if (mat.HasProperty(name))
					return name;
		return null;
	}

	protected void DisableEmit()
	{
		NcParticleSystem[] ncpss = gameObject.GetComponentsInChildren<NcParticleSystem>(true);
		foreach (NcParticleSystem ncps in ncpss)
			if (ncps != null)
				ncps.SetDisableEmit();
		NcAttachPrefab[] ncaps = gameObject.GetComponentsInChildren<NcAttachPrefab>(true);
		foreach (NcAttachPrefab ncap in ncaps)
			if (ncap != null)
				ncap.enabled = false;
		ParticleSystem[] pss = gameObject.GetComponentsInChildren<ParticleSystem>(true);
		foreach (ParticleSystem em in pss)
			if (em != null)
				em.enableEmission = false;
		ParticleEmitter[] pes = gameObject.GetComponentsInChildren<ParticleEmitter>(true);
		foreach (ParticleEmitter em in pes)
			if (em != null)
				em.emit = false;
	}

	// SafeCreate -----------------------------------------------------------------------------------
	public static bool IsSafe()
	{
		return (!m_bShuttingDown && !Application.isLoadingLevel);
//		return (NcSafeTool.IsSafe());
	}


	protected GameObject CreateEditorGameObject(GameObject srcGameObj)
	{
		// юс╫ц
		if (srcGameObj.name.Contains("flare 24"))
		{
			return srcGameObj;
		}
#if UNITY_EDITOR
		GameObject	fxmMain = GetFXMakerGameObject();
		if (fxmMain != null && (this is NcDuplicator) == false)
		{
 			_RuntimeIntance	arg = new _RuntimeIntance(gameObject, srcGameObj);
			if (srcGameObj.transform.parent == null)
				ChangeParent(GetRootInstanceEffect().transform, srcGameObj.transform, true, null);
 			fxmMain.SendMessage("OnRuntimeIntance", arg, SendMessageOptions.DontRequireReceiver);
		}
#endif
		return srcGameObj;
	}

	public GameObject CreateGameObject(string name)
	{
		if (IsSafe() == false)
			return null;
		return CreateEditorGameObject(new GameObject(name));
	}

	public GameObject CreateGameObject(GameObject original)
	{
		if (IsSafe() == false)
			return null;
		return CreateEditorGameObject((GameObject)Object.Instantiate(original));
	}

	public GameObject CreateGameObject(GameObject prefabObj, Vector3 position, Quaternion rotation)
	{
		if (IsSafe() == false)
			return null;
		return CreateEditorGameObject((GameObject)Object.Instantiate(prefabObj, position, rotation));
	}

	public GameObject CreateGameObject(GameObject parentObj, GameObject prefabObj)
	{
		if (IsSafe() == false)
			return null;
		GameObject newChild = CreateGameObject(prefabObj);
		if (parentObj != null)
			ChangeParent(parentObj.transform, newChild.transform, true, null);
		return newChild;
	}

// 	public GameObject CreateGameObject(GameObject parentObj, GameObject prefabObj, Vector3 position, Quaternion rotation)
// 	{
// 		if (IsSafe() == false)
// 			return null;
// 		GameObject newChild = CreateEditorGameObject((GameObject)Object.Instantiate(prefabObj, position, rotation));
// 		if (parentObj != null)
// 			ChangeParent(parentObj.transform, newChild.transform, true, null);
// 		return newChild;
// 	}

	public GameObject CreateGameObject(GameObject parentObj, Transform parentTrans, GameObject prefabObj)
	{
		if (IsSafe() == false)
			return null;
		GameObject newChild = CreateGameObject(prefabObj);
		if (parentObj != null)
			ChangeParent(parentObj.transform, newChild.transform, true, parentTrans);
		return newChild;
	}

	protected TT AddNcComponentToObject<TT>(GameObject toObj) where TT : NcEffectBehaviour
	{
		NcEffectBehaviour com = toObj.AddComponent<TT>();
		if (m_bReplayState)
			com.OnSetReplayState();
		return (TT)com;
	}

	protected void ChangeParent(Transform newParent, Transform child, bool bKeepingLocalTransform, Transform addTransform)
	{
		// Keeping transform
		NcTransformTool	trans = null;
		if (bKeepingLocalTransform)
		{
			trans	= new NcTransformTool(child.transform);
			if (addTransform != null)
				trans.AddTransform(addTransform);
		}
		child.parent = newParent;
		if (bKeepingLocalTransform)
			trans.CopyToLocalTransform(child.transform);

		// Update component
		if (bKeepingLocalTransform)
		{
// 			NcEffectBehaviour[]	coms = gameObject.GetComponentsInChildren<NcEffectBehaviour>();
// 			foreach (NcEffectBehaviour com in coms)
// 				com.SendMessage("Update", SendMessageOptions.DontRequireReceiver);
			NcBillboard[]	ncBills = child.GetComponentsInChildren<NcBillboard>();
	 		foreach (NcBillboard ncb in ncBills)
				ncb.UpdateBillboard();
		}
	}

	// ----------------------------------------------------------------------------
	protected void UpdateMeshColors(Color color)
	{
		if (m_MeshFilter == null)
			m_MeshFilter = (MeshFilter)gameObject.GetComponent(typeof(MeshFilter));
		if (m_MeshFilter == null || m_MeshFilter.sharedMesh == null || m_MeshFilter.mesh == null)
			return;

		Color[]	colors = new Color[m_MeshFilter.mesh.vertexCount];
		for (int n = 0; n < colors.Length; n++)
			colors[n] = color;
		m_MeshFilter.mesh.colors = colors;
	}

	// Loop Function --------------------------------------------------------------------
	protected virtual void OnDestroy()
	{
		// RuntimeMaterials
		if (m_RuntimeMaterials != null)
		{
			foreach (Material mat in m_RuntimeMaterials)
				Destroy(mat);
			m_RuntimeMaterials = null;
		}
	}

	// Control Function -----------------------------------------------------------------
	// Event Function -------------------------------------------------------------------
	public void OnApplicationQuit()
	{
		m_bShuttingDown = true;
	}

	public virtual void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
	}

	public virtual void OnSetActiveRecursively(bool bActive)
	{
	}

	public virtual void OnUpdateToolData()
	{
	}

	public virtual void OnSetReplayState()
	{
		m_bReplayState	= true;
	}

	public virtual void OnResetReplayStage(bool bClearOldParticle)
	{
	}

}
