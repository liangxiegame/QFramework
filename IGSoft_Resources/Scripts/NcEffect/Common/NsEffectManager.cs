// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class NsEffectManager : MonoBehaviour
{
	// Attribute ------------------------------------------------------------------------
	// Property -------------------------------------------------------------------------
	// Control Function -----------------------------------------------------------------
	// 
	public static Texture[] PreloadResource(GameObject tarObj)
	{
		if (tarObj == null)
			return new Texture[0];
		List<GameObject>	parentPrefabList = new List<GameObject>();
		parentPrefabList.Add(tarObj);
		return PreloadResource(tarObj, parentPrefabList);
	}

	public static Component GetComponentInChildren(GameObject tarObj, System.Type findType)
	{
		if (tarObj == null)
			return null;
		List<GameObject>	parentPrefabList = new List<GameObject>();
		parentPrefabList.Add(tarObj);
		return GetComponentInChildren(tarObj, findType, parentPrefabList);
	}

	// Replay Function
	public static GameObject CreateReplayEffect(GameObject tarPrefab)
	{
		if (tarPrefab == null)
			return null;
		if (NcEffectBehaviour.IsSafe() == false)
			return null;
		GameObject instanceObj = (GameObject)Instantiate(tarPrefab);
		SetReplayEffect(instanceObj);
		return instanceObj;
	}

	public static void SetReplayEffect(GameObject instanceObj)
	{
		PreloadResource(instanceObj);
		SetActiveRecursively(instanceObj, false);

		NcEffectBehaviour[] ncComs = instanceObj.GetComponentsInChildren<NcEffectBehaviour>(true);
		foreach (NcEffectBehaviour ncCom in ncComs)
		{
			ncCom.OnSetReplayState();
		}
	}

	public static void RunReplayEffect(GameObject instanceObj, bool bClearOldParticle)
	{
		SetActiveRecursively(instanceObj, true);

		NcEffectBehaviour[] ncComs = instanceObj.GetComponentsInChildren<NcEffectBehaviour>(true);
		foreach (NcEffectBehaviour ncCom in ncComs)
		{
			ncCom.OnResetReplayStage(bClearOldParticle);
		}
	}

	// ----------------------------------------------------------------------------------
#if UNITY_EDITOR
	public static void AdjustSpeedEditor(GameObject target, float fSpeedRate)	// support shuriken, legancy, mesh
	{
		NcEffectBehaviour[] coms = target.GetComponentsInChildren<NcEffectBehaviour>(true);
		foreach (NcEffectBehaviour com in coms)
			com.OnUpdateEffectSpeed(fSpeedRate, false);
	}
#endif
	public static void AdjustSpeedRuntime(GameObject target, float fSpeedRate)	// support legancy/mesh , not support shuriken
	{
		NcEffectBehaviour[] coms = target.GetComponentsInChildren<NcEffectBehaviour>(true);
		foreach (NcEffectBehaviour com in coms)
			com.OnUpdateEffectSpeed(fSpeedRate, true);
	}

	// ----------------------------------------------------------------------------------
	public static void SetActiveRecursively(GameObject target, bool bActive)
	{
#if (!UNITY_3_5)
		for (int n = target.transform.childCount-1; 0 <= n; n--)
			if (n < target.transform.childCount)
				SetActiveRecursively(target.transform.GetChild(n).gameObject, bActive);
		target.SetActive(bActive);
#else
		target.SetActiveRecursively(bActive);
#endif
// 		SetActiveRecursivelyEffect(target, bActive);
	}

	public static bool IsActive(GameObject target)
	{
#if (!UNITY_3_5)
		return (target.activeInHierarchy && target.activeSelf);
#else
		return target.active;
#endif
	}

	protected static void SetActiveRecursivelyEffect(GameObject target, bool bActive)
	{
		NcEffectBehaviour[] coms = target.GetComponentsInChildren<NcEffectBehaviour>(true);
		foreach (NcEffectBehaviour com in coms)
			com.OnSetActiveRecursively(bActive);
	}

	// ----------------------------------------------------------------------------------
	protected static Texture[] PreloadResource(GameObject tarObj, List<GameObject> parentPrefabList)
	{
		if (NcEffectBehaviour.IsSafe() == false)
			return null;
		// texture
		Renderer[]		rens = tarObj.GetComponentsInChildren<Renderer>(true);
		List<Texture>	texList = new List<Texture>();
		foreach (Renderer ren in rens)
		{
			if (ren.sharedMaterials == null || ren.sharedMaterials.Length <= 0)
				continue;
			foreach (Material mat in ren.sharedMaterials)
				if (mat != null && mat.mainTexture != null)
					texList.Add(mat.mainTexture);
		}

		// prefab
		NcAttachPrefab[]	prefabs = tarObj.GetComponentsInChildren<NcAttachPrefab>(true);
		foreach (NcAttachPrefab obj in prefabs)
			if (obj.m_AttachPrefab != null)
			{
				Texture[] ret = PreloadPrefab(obj.m_AttachPrefab, parentPrefabList, true);
				if (ret == null)
					obj.m_AttachPrefab = null;	// clear
				else texList.AddRange(ret);
			}
		NcParticleSystem[]	pss = tarObj.GetComponentsInChildren<NcParticleSystem>(true);
		foreach (NcParticleSystem ps in pss)
			if (ps.m_AttachPrefab != null)
			{
				Texture[] ret = PreloadPrefab(ps.m_AttachPrefab, parentPrefabList, true);
				if (ret == null)
					ps.m_AttachPrefab = null;	// clear
				else texList.AddRange(ret);
			}
		NcSpriteTexture[]	sts = tarObj.GetComponentsInChildren<NcSpriteTexture>(true);
		foreach (NcSpriteTexture st in sts)
			if (st.m_NcSpriteFactoryPrefab != null)
			{
				Texture[] ret = PreloadPrefab(st.m_NcSpriteFactoryPrefab, parentPrefabList, false);
				if (ret != null)
					texList.AddRange(ret);
			}
		NcParticleSpiral[]	sps = tarObj.GetComponentsInChildren<NcParticleSpiral>(true);
		foreach (NcParticleSpiral sp in sps)
			if (sp.m_ParticlePrefab != null)
			{
				Texture[] ret = PreloadPrefab(sp.m_ParticlePrefab, parentPrefabList, false);
				if (ret != null)
					texList.AddRange(ret);
			}
		NcParticleEmit[]	ses = tarObj.GetComponentsInChildren<NcParticleEmit>(true);
		foreach (NcParticleEmit se in ses)
			if (se.m_ParticlePrefab != null)
			{
				Texture[] ret = PreloadPrefab(se.m_ParticlePrefab, parentPrefabList, false);
				if (ret != null)
					texList.AddRange(ret);
			}

		// sound
		NcAttachSound[]	ass = tarObj.GetComponentsInChildren<NcAttachSound>(true);
		foreach (NcAttachSound ncas in ass)
			if (ncas.m_AudioClip != null)
				continue;

		// prefab & sound
		NcSpriteFactory[]	sprites = tarObj.GetComponentsInChildren<NcSpriteFactory>(true);
		foreach (NcSpriteFactory sp in sprites)
			if (sp.m_SpriteList != null)
				for (int n = 0; n < sp.m_SpriteList.Count; n++)
					if (sp.m_SpriteList[n].m_EffectPrefab != null)
					{
						Texture[] ret = PreloadPrefab(sp.m_SpriteList[n].m_EffectPrefab, parentPrefabList, true);
						if (ret == null)
							sp.m_SpriteList[n].m_EffectPrefab = null;	// clear
						else texList.AddRange(ret);
						if (sp.m_SpriteList[n].m_AudioClip != null)
							continue;
					}

		return texList.ToArray();
	}

	protected static Texture[] PreloadPrefab(GameObject tarObj, List<GameObject> parentPrefabList, bool bCheckDup)
	{
		if (parentPrefabList.Contains(tarObj))
		{
			if (bCheckDup)
			{
				string str = "";
				for (int n = 0; n < parentPrefabList.Count; n++)
					str += parentPrefabList[n].name + "/";
				Debug.LogWarning("LoadError : Recursive Prefab - " + str + tarObj.name);
				return null;	// error
			} else return null;
		}
		parentPrefabList.Add(tarObj);
		Texture[] ret = PreloadResource(tarObj, parentPrefabList);
		parentPrefabList.Remove(tarObj);
		return ret;
	}

	// ----------------------------------------------------------------------------------
	protected static Component GetComponentInChildren(GameObject tarObj, System.Type findType, List<GameObject> parentPrefabList)
	{
		Component[] coms = tarObj.GetComponentsInChildren(findType, true);
		foreach (Component com in coms)
		{
			if (com.GetComponent<NcDontActive>() == null)
				return com;
		}

		// prefab
		Component			findCom;
		NcAttachPrefab[]	prefabs = tarObj.GetComponentsInChildren<NcAttachPrefab>(true);
		foreach (NcAttachPrefab obj in prefabs)
			if (obj.m_AttachPrefab != null)
			{
				findCom = GetValidComponentInChildren(obj.m_AttachPrefab, findType, parentPrefabList, true);
				if (findCom != null)
					return findCom;
			}
		NcParticleSystem[]	pss = tarObj.GetComponentsInChildren<NcParticleSystem>(true);
		foreach (NcParticleSystem ps in pss)
			if (ps.m_AttachPrefab != null)
			{
				findCom = GetValidComponentInChildren(ps.m_AttachPrefab, findType, parentPrefabList, true);
				if (findCom != null)
					return findCom;
			}
		NcSpriteTexture[]	sts = tarObj.GetComponentsInChildren<NcSpriteTexture>(true);
		foreach (NcSpriteTexture st in sts)
			if (st.m_NcSpriteFactoryPrefab != null)
			{
				findCom = GetValidComponentInChildren(st.m_NcSpriteFactoryPrefab, findType, parentPrefabList, false);
				if (findCom != null)
					return findCom;
			}
		NcParticleSpiral[]	sps = tarObj.GetComponentsInChildren<NcParticleSpiral>(true);
		foreach (NcParticleSpiral sp in sps)
			if (sp.m_ParticlePrefab != null)
			{
				findCom = GetValidComponentInChildren(sp.m_ParticlePrefab, findType, parentPrefabList, false);
				if (findCom != null)
					return findCom;
			}
		NcParticleEmit[]	ses = tarObj.GetComponentsInChildren<NcParticleEmit>(true);
		foreach (NcParticleEmit se in ses)
			if (se.m_ParticlePrefab != null)
			{
				findCom = GetValidComponentInChildren(se.m_ParticlePrefab, findType, parentPrefabList, false);
				if (findCom != null)
					return findCom;
			}

		// prefab & sound
		NcSpriteFactory[]	sprites = tarObj.GetComponentsInChildren<NcSpriteFactory>(true);
		foreach (NcSpriteFactory sp in sprites)
			if (sp.m_SpriteList != null)
				for (int n = 0; n < sp.m_SpriteList.Count; n++)
					if (sp.m_SpriteList[n].m_EffectPrefab != null)
					{
						findCom = GetValidComponentInChildren(sp.m_SpriteList[n].m_EffectPrefab, findType, parentPrefabList, true);
						if (findCom != null)
							return findCom;
					}
		return null;
	}

	protected static Component GetValidComponentInChildren(GameObject tarObj, System.Type findType, List<GameObject> parentPrefabList, bool bCheckDup)
	{
		if (parentPrefabList.Contains(tarObj))
		{
			if (bCheckDup)
			{
				string str = "";
				for (int n = 0; n < parentPrefabList.Count; n++)
					str += parentPrefabList[n].name + "/";
				Debug.LogWarning("LoadError : Recursive Prefab - " + str + tarObj.name);
				return null;	// error
			} else return null;
		}
		parentPrefabList.Add(tarObj);
		Component com = GetComponentInChildren(tarObj, findType, parentPrefabList);
		parentPrefabList.Remove(tarObj);
		return com;
	}
}
