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
using System.Collections.Generic;

public class FxmInfoIndexing : MonoBehaviour
{
	// Attribute ------------------------------------------------------------------------
	public		Transform				m_OriginalTrans;
	public		bool					m_bRuntimeCreatedObj;

	public		bool					m_bSelected		= false;
	public		bool					m_bRootSelected	= false;

	// picking
	public		bool					m_bPicking		= true;
	public		Collider				m_Collider;

	// wireframe
	public		bool					m_bBounds		= false;
	public		bool					m_bWireframe	= false;
	public		bool					m_bRoot			= false;
	public		FXMakerWireframe		m_WireObject;

	//	[HideInInspector]

	// static init ------------------------------------------------------------------------
	public static void CreateInstanceIndexing(Transform oriTrans, Transform insTrans, bool bSameValueRecursively, bool bRuntimeCreatedObj)
	{
		if (insTrans == null)
			return;

		FxmInfoIndexing com = insTrans.gameObject.AddComponent<FxmInfoIndexing>();
		com.SetOriginal(oriTrans, bRuntimeCreatedObj);
		com.OnAutoInitValue();

		// Dup process
		NcDuplicator dupCom = com.GetComponent<NcDuplicator>();
		if (dupCom != null)
		{
			GameObject clone = dupCom.GetCloneObject();
			if (clone != null)
				CreateInstanceIndexing(oriTrans, clone.transform, bSameValueRecursively, false);
		}

		if (bSameValueRecursively)
		{
			for (int n = 0; n < insTrans.childCount; n++)
				CreateInstanceIndexing(oriTrans, insTrans.GetChild(n), bSameValueRecursively, true);
		} else {
			for (int n = 0; n < oriTrans.childCount; n++)
			{
				if (n < insTrans.childCount)
					CreateInstanceIndexing(oriTrans.GetChild(n), insTrans.GetChild(n), bSameValueRecursively, bRuntimeCreatedObj);
			}
		}
	}

	public static FxmInfoIndexing FindInstanceIndexing(Transform oriTrans, bool bIncludeRuntimeCreatedObj)
	{
		GameObject			parentObj = FXMakerMain.inst.GetInstanceRoot();
		FxmInfoIndexing[]	coms = parentObj.GetComponentsInChildren<FxmInfoIndexing>(true);

		foreach (FxmInfoIndexing fxmInfoIndexing in coms)
		{
			if (fxmInfoIndexing.m_OriginalTrans == oriTrans && (bIncludeRuntimeCreatedObj || fxmInfoIndexing.m_bRuntimeCreatedObj == false))
				return fxmInfoIndexing;
		}
		return null;
	}

	public static List<FxmInfoIndexing> FindInstanceIndexings(Transform oriTrans, bool bIncludeRuntimeCreatedObj)
	{
		GameObject				parentObj = FXMakerMain.inst.GetInstanceRoot();
		FxmInfoIndexing[]		coms = parentObj.GetComponentsInChildren<FxmInfoIndexing>(true);
		List<FxmInfoIndexing>	list = new List<FxmInfoIndexing>();

		foreach (FxmInfoIndexing fxmInfoIndexing in coms)
		{
			if (oriTrans == null || fxmInfoIndexing.m_OriginalTrans == oriTrans && (bIncludeRuntimeCreatedObj || fxmInfoIndexing.m_bRuntimeCreatedObj == false))
				list.Add(fxmInfoIndexing);
		}
		return list;
	}
	
	public void OnAutoInitValue()
	{
		NcEffectBehaviour[] oriComs = m_OriginalTrans.GetComponents<NcEffectBehaviour>();
		foreach (NcEffectBehaviour effect in oriComs)
			effect.OnUpdateToolData();
		NcEffectBehaviour[] insComs = transform.GetComponents<NcEffectBehaviour>();
		foreach (NcEffectBehaviour effect in insComs)
			effect.OnUpdateToolData();

		// Set particleSystem.speed
// 		{
// 			NcParticleSystem	ncParticleScaleOri	= (m_OriginalTrans.GetComponent<NcParticleSystem>());
// 			NcParticleSystem	ncParticleScaleIns	= (transform.GetComponent<NcParticleSystem>());
// 			if (ncParticleScaleOri != null && ncParticleScaleIns != null && ncParticleScaleOri.particleSystem != null)
// 			{
// 				ncParticleScaleOri.SaveShurikenSpeed();
// 				ncParticleScaleIns.SaveShurikenSpeed();
// 			}
// 		}

		// Update bWorldSpace
		{
			NcParticleSystem	ncParticleScaleOri	= (m_OriginalTrans.GetComponent<NcParticleSystem>());
			NcParticleSystem	ncParticleScaleIns	= (transform.GetComponent<NcParticleSystem>());
			if (ncParticleScaleOri != null && ncParticleScaleIns != null)
				ncParticleScaleIns.m_bWorldSpace = ncParticleScaleOri.m_bWorldSpace = NgSerialized.GetSimulationSpaceWorld(ncParticleScaleOri.transform);
		}

		// Set particleEmitter.m_MinNormalVelocity, m_MaxNormalVelocity
		{
			NcParticleSystem	ncParticleScaleOri	= (m_OriginalTrans.GetComponent<NcParticleSystem>());
			NcParticleSystem	ncParticleScaleIns	= (transform.GetComponent<NcParticleSystem>());
			if (ncParticleScaleOri != null && ncParticleScaleOri.enabled && ncParticleScaleIns != null && ncParticleScaleOri.GetComponent<ParticleEmitter>() != null && ncParticleScaleOri.m_bScaleWithTransform && NgSerialized.IsMeshParticleEmitter(ncParticleScaleOri.GetComponent<ParticleEmitter>()))
			{
				float	fSetMinValue;
				float	fSetMaxValue;
				NgSerialized.GetMeshNormalVelocity(ncParticleScaleOri.GetComponent<ParticleEmitter>(), out fSetMinValue, out fSetMaxValue);
				if (fSetMinValue != ncParticleScaleOri.GetScaleMinMeshNormalVelocity() || fSetMaxValue != ncParticleScaleOri.GetScaleMaxMeshNormalVelocity())
				{
					NgSerialized.SetMeshNormalVelocity(ncParticleScaleOri.GetComponent<ParticleEmitter>(), ncParticleScaleOri.GetScaleMinMeshNormalVelocity(), ncParticleScaleOri.GetScaleMaxMeshNormalVelocity());
					NgSerialized.SetMeshNormalVelocity(ncParticleScaleIns.GetComponent<ParticleEmitter>(), ncParticleScaleOri.GetScaleMinMeshNormalVelocity(), ncParticleScaleOri.GetScaleMaxMeshNormalVelocity());
				}
			}
		}

	}

	// Property -------------------------------------------------------------------------
	public void SetSelected(bool bSelected, bool bRoot)
	{
		m_bSelected		= bSelected;
		m_bRootSelected	= bRoot;
	}

	public void SetOriginal(Transform originalTrans, bool bRuntimeCreatedObj)
	{
		m_OriginalTrans			= originalTrans;
		m_bRuntimeCreatedObj	= bRuntimeCreatedObj;
	}

	public bool IsParticle()
	{
		return (GetComponent<ParticleEmitter>() != null || GetComponent<ParticleSystem>() != null);
	}

	public bool IsMesh()
	{
		MeshFilter	filter	= GetComponent<MeshFilter>();
		return (filter != null && filter.sharedMesh != null);
	}

	public int GetParticleCount()
	{
		if (IsParticle() == false)
			return 0;

		if (m_WireObject == null)
			return 0;

		return m_WireObject.GetParticleCount();
	}

	// Control --------------------------------------------------------------------------
	public bool IsPickingParticle(Ray ray)
	{
		if (IsParticle() == false)
			return false;

		if (m_WireObject == null)
			return false;

		List<Vector4>	listPos = m_WireObject.GetLastParticlePostions();
		foreach (Vector4 pos in listPos)
		{
			Bounds	bounds = new Bounds(new Vector3(pos.x, pos.y, pos.z), new Vector3(pos.w, pos.w, pos.w));
			if (bounds.IntersectRay(ray))
				return true;
		}

		return false;
	}

	// UpdateLoop -----------------------------------------------------------------------
	void Start() 
	{
		if (m_bPicking)
			InitPicking(m_bPicking);
	}

	void OnDrawGizmos()
	{
		if (m_bBounds)
		{
			if (IsParticle())
			{
				Bounds			allBounds = new Bounds();
				List<Vector4>	listPos = m_WireObject.GetLastParticlePostions();
				for (int n=0; n < listPos.Count; n++)
				{
					Vector4 pos		= listPos[n];
					Bounds	bounds	= new Bounds(new Vector3(pos.x, pos.y, pos.z), new Vector3(pos.w, pos.w, pos.w));
//					Bounds	bounds = new Bounds(new Vector3(pos.x, pos.y, pos.z), new Vector3(0, 0, 0));
//					Gizmos.DrawCube(bounds.center, new Vector3(0.1f, 0.1f, 0.1f));
					if (n == 0)
						 allBounds = bounds;
					else allBounds.Encapsulate(bounds);
				}
				Gizmos.matrix = Matrix4x4.identity;
				DrawBox(allBounds, (m_bRoot ? FXMakerOption.inst.m_ColorRootBoundsBox : FXMakerOption.inst.m_ColorChildBoundsBox));
			} else
			if (GetComponent<Renderer>() != null)
			{
				Gizmos.matrix = Matrix4x4.identity;
				DrawBox(GetComponent<Renderer>().bounds, (m_bRoot ? FXMakerOption.inst.m_ColorRootBoundsBox : FXMakerOption.inst.m_ColorChildBoundsBox));
			}
		}
	}

	void DrawBox(Bounds	bounds, Color color)
	{
		Color	oldColor = Gizmos.color;
		Gizmos.color = color;
		Gizmos.DrawWireCube(bounds.center, bounds.size);
		Gizmos.color = oldColor;
	}

	// Event -------------------------------------------------------------------------
	// Function ----------------------------------------------------------------------
	bool InitPicking(bool bPicking)
	{
		if (GetComponent<Renderer>() == null || bPicking == false)
			return false;

		// particle
		if (IsParticle())
		{
			SetBoundsWire(m_bBounds, m_bWireframe, m_bRoot);
			return true;
		}

		// mesh
		if ((GetComponent<MeshFilter>() != null && GetComponent<MeshFilter>().sharedMesh != null))
		{
			Mesh	mesh	= GetComponent<MeshFilter>().sharedMesh;
			bool	bPlane	= (mesh.bounds.size.x < 0.1f || mesh.bounds.size.y < 0.1f || mesh.bounds.size.z < 0.1f);


			m_bPicking	= true;
			m_Collider	= GetComponent<Collider>();
			if (m_Collider == null)
			{
				if (bPlane)
				{
					m_Collider	= gameObject.AddComponent<BoxCollider>();
				} else {
					m_Collider	= gameObject.AddComponent<MeshCollider>();
				}
			}
			return true;
		}
		return false;
	}

	public void SetBoundsWire(bool bBounds, bool bWireframe, bool bRoot)
	{
		CreateWireObject();
		m_bWireframe = m_WireObject.InitWireframe(transform, bWireframe, true, bRoot);
		m_bBounds	= bBounds;
		m_bRoot		= bRoot;
	}

	void CreateWireObject()
	{
		if (m_WireObject == null)
		{
			GameObject newObj = NgObject.CreateGameObject(gameObject, "WireObject");
			m_WireObject = newObj.AddComponent<FXMakerWireframe>();
			m_WireObject.m_ParentName = gameObject.name;
		}
	}
}

#endif
