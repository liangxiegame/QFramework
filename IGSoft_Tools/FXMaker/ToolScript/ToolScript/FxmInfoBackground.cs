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

public class FxmInfoBackground : NcEffectBehaviour
{
	// Attribute ------------------------------------------------------------------------
	public enum SETTING_TYPE		{ ModelPrefab=0, GridPrefab, LightPrefab, CameraPrefab, Count };

	public		GameObject[]	m_ChildObjects			= new GameObject[FXMakerBackground.m_CloneTypeCount+FXMakerBackground.m_ReferenceTypeCount];
	public		string[]		m_CloneThumbFilenames	= new string[FXMakerBackground.m_CloneTypeCount];
	public		GameObject[]	m_ReferenceObjects		= new GameObject[FXMakerBackground.m_ReferenceTypeCount];		// 	m_ModelPrefab1, m_ModelPrefab2, 

	//	[HideInInspector]

	// Property -------------------------------------------------------------------------
	public void SetActive()
	{
		NgObject.SetActiveRecursively(gameObject, true);
	}

	public void SetPingObject(int nChildIndex)
	{
		if (nChildIndex < FXMakerBackground.m_CloneTypeCount)
			FXMakerAsset.SetPingObject(m_ChildObjects[nChildIndex]);
		else FXMakerAsset.SetPingObject(m_ReferenceObjects[nChildIndex - FXMakerBackground.m_CloneTypeCount]);
	}

	public GameObject GetChildObject(int nChildIndex)
	{
		return m_ChildObjects[nChildIndex];
	}

	public string GetClildThumbFilename(int nChildIndex)
	{
		if (nChildIndex < FXMakerBackground.m_CloneTypeCount)
			return m_CloneThumbFilenames[nChildIndex];
		return NgAsset.GetPrefabThumbFilename(m_ReferenceObjects[nChildIndex-FXMakerBackground.m_CloneTypeCount]);
	}

	public GameObject GetReferenceObject(int nRefIndex)
	{
		return m_ReferenceObjects[nRefIndex];
	}

	public void ShowBackground(bool bShow)
	{
		for (int nIndex=0; nIndex < m_ReferenceObjects.Length; nIndex++)
		{
			int nChildIndex = FXMakerBackground.m_CloneTypeCount + nIndex;

			// 기존 것 삭제
			if (m_ChildObjects[nChildIndex] != null)
			{
				Object.DestroyImmediate(m_ChildObjects[nChildIndex]);
				m_ChildObjects[nChildIndex]	= null;
			}

			if (bShow && m_ReferenceObjects[nIndex] != null)
			{
				m_ChildObjects[nChildIndex] = NgAsset.LoadPrefab(m_ReferenceObjects[nIndex], gameObject);
				m_ChildObjects[nChildIndex].name = m_ChildObjects[nChildIndex].name.Replace("(Clone)", "");
	 			NgObject.SetActiveRecursively(m_ChildObjects[nChildIndex], true);
			}
		}
	}

	public void SetCloneObject(int nCloneIndex, GameObject prefab)
	{
		// 기존 것 삭제
		if (m_ChildObjects[nCloneIndex] != null)
		{
			Object.DestroyImmediate(m_ChildObjects[nCloneIndex]);
			m_ChildObjects[nCloneIndex]			= null;
			m_CloneThumbFilenames[nCloneIndex]	= "";
		}

		// 새로운 차일드 등록
		if (prefab != null)
		{
			m_ChildObjects[nCloneIndex] = NgAsset.LoadPrefab(prefab, gameObject);
 			NgObject.SetActiveRecursively(m_ChildObjects[nCloneIndex], true);
			m_ChildObjects[nCloneIndex].name = m_ChildObjects[nCloneIndex].name.Replace("(Clone)", "");

			// 사용된 prefab과 GameObject를 선택한다.
			FXMakerAsset.SetPingObject(prefab, m_ChildObjects[nCloneIndex]);

			// thumb 출력을 위해서 원본prefab의 guid를 저장해 둔다.
			m_CloneThumbFilenames[nCloneIndex] = NgAsset.GetPrefabThumbFilename(prefab);
		}
		FXMakerBackground.inst.SaveBackgroundPrefab();
	}

	public void SetReferenceObject(int nRefIndex, GameObject prefab)
	{
		m_ReferenceObjects[nRefIndex]	= prefab;
		ShowBackground(true);
		FXMakerBackground.inst.SaveBackgroundPrefab();
	}

	// Control --------------------------------------------------------------------------

	// UpdateLoop -----------------------------------------------------------------------
	void Awake()
	{
	}

	void Start()
	{
	}

	void Update()
	{
	}

	void FixedUpdate()
	{
	}

	public void LateUpdate()
	{
	}

	// Event -------------------------------------------------------------------------
	void OnDrawGizmos()
	{
	}

	// Function ----------------------------------------------------------------------
}
#endif
