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

public class FXMakerClipboard : MonoBehaviour
{
	// Attribute ------------------------------------------------------------------------
	public		static FXMakerClipboard	inst;

	public		enum OBJECT_TYPE			{NONE, PREFAB, TRANSFORM, GAMEOBJECT, COMPONENT, MATERIAL, ANIMATIONCLIP};
	public		OBJECT_TYPE					m_ObjectType;
	public		GameObject					m_CopyCloneGameObject;
	public		Object						m_CopyCloneObject;
	public		Color						m_CopyColor;

	public		bool						m_bPrefabCutCopy;
	public		GameObject					m_PrefabObject;

	protected	static NcTransformTool			m_CopyTransform;

	//	[HideInInspector]

	// Property -------------------------------------------------------------------------
	public void ClearClipboard()
	{
		m_ObjectType	= OBJECT_TYPE.NONE;
		if (m_CopyCloneGameObject != null)
			DestroyImmediate(m_CopyCloneGameObject);
		m_CopyCloneObject = null;
		m_PrefabObject	= null;
	}

	public bool IsPrefab()
	{
		return (m_PrefabObject != null && m_ObjectType == OBJECT_TYPE.PREFAB);
	}

	public bool IsObject()
	{
		return (m_CopyCloneObject != null && m_ObjectType != OBJECT_TYPE.PREFAB);
	}

	public bool IsTransform()
	{
		return (m_CopyCloneObject is Transform);
	}

	public bool IsGameObject()
	{
		return (m_CopyCloneObject is GameObject);
	}

	public bool IsComponent()
	{
		return (m_CopyCloneObject is Component);
	}

	public bool IsMaterial()
	{
		return (m_CopyCloneObject is Material);
	}

	public bool IsAnimationClip()
	{
		return (m_CopyCloneObject is AnimationClip);
	}

	public string GetName()
	{
		if (IsObject())
			return m_CopyCloneObject.ToString();
		return "";
	}

	public Object GetObject()
	{
		return m_CopyCloneObject;
	}

	// Control --------------------------------------------------------------------------
	public void SetClipboardColor(Color color)
	{
		m_CopyColor	= color;
	}

	public void PasteClipboardColor(Transform tarTransform, int tarMatIndex, Material tarMat)
	{
		if (NgMaterial.IsMaterialColor(tarMat))
		{
//			NgMaterial.SetMaterialColor(tarMat, m_CopyColor);
			FxmPopupManager.inst.ChangeMaterialColor(tarTransform, tarMatIndex, m_CopyColor);
		}
	}

	public void SetClipboardPrefab(GameObject setPrefabObj, bool bCutCopy)
	{
		m_ObjectType		= OBJECT_TYPE.PREFAB;
		m_bPrefabCutCopy	= bCutCopy;
		m_PrefabObject		= setPrefabObj;
	}

	public void CheckDeletePrefab(GameObject tarPrefab)
	{
		if (m_PrefabObject == tarPrefab)
			ClearClipboard();
	}

	public string PasteClipboardPrefab(string tarPath)
	{
		string rePath = FXMakerAsset.CopyEffectPrefab(m_PrefabObject, tarPath, m_bPrefabCutCopy);
		ClearClipboard();
		return rePath;
	}

	public void SetClipboardObject(Object setObj)
	{
		ClearClipboard();

		if (setObj is Transform)
		{
			m_ObjectType = OBJECT_TYPE.TRANSFORM;
			SetClipboardTransform((setObj as Transform));
		} else
		if (setObj is GameObject)
		{
			m_ObjectType = OBJECT_TYPE.GAMEOBJECT;
			SetClipboardGameObject((setObj as GameObject));
		} else
		if (setObj is Component)
		{
			m_ObjectType = OBJECT_TYPE.COMPONENT;
			SetClipboardComponent((setObj as Component));
		} else
		if (setObj is Material)
		{
			m_ObjectType = OBJECT_TYPE.MATERIAL;
			m_CopyCloneObject		= setObj;
		} else
		if (setObj is AnimationClip)
		{
			m_ObjectType = OBJECT_TYPE.ANIMATIONCLIP;
			m_CopyCloneObject		= setObj;
		} else {
			Debug.LogWarning("SetClipboardObject() - Invaild type!!!");
			return;
		}
	}

	public Object PasteClipboardObject(GameObject tarGameObject, Object tarComponent, int tarIndex)
	{
		if (IsGameObject())
			return FXMakerHierarchy.inst.AddGameObject(tarGameObject, m_CopyCloneObject as GameObject);

		return PasteObject(m_CopyCloneObject, true, tarGameObject.transform, tarComponent, tarIndex, true, false, false);
	}

	public Object OverwriteClipboardObject(GameObject tarGameObject, Object tarComponent, int tarIndex)
	{
		if (IsGameObject())
		{
			// not used
			GameObject newObj = NgObject.CreateGameObject(m_CopyCloneObject as GameObject);
			NgObject.SetActiveRecursively(newObj, false);
			newObj.transform.parent = tarGameObject.transform; // �����ִ�... �������ϹǷ� ���߿� ó��
			newObj.name = newObj.name.Replace("(Clone)", "");
			newObj.name = newObj.name.Replace("(Original)", "");
			return newObj;
		}

		return PasteObject(m_CopyCloneObject, true, tarGameObject.transform, tarComponent, tarIndex, false, false, false);
	}

	// ---------------------------------------------------------------------------
	void SetClipboardTransform(Transform selTrans)
	{
		m_CopyCloneObject = NgObject.CreateGameObject(gameObject.transform, selTrans.gameObject).transform;
		NgObject.SetActiveRecursively((m_CopyCloneObject as Transform).gameObject, false);
		ObjectCopy(selTrans, (m_CopyCloneObject as Transform), true);

		m_CopyCloneObject.name	= m_CopyCloneObject.name.Replace("(Clone)", "");
		m_CopyCloneObject.name	= m_CopyCloneObject.name.Replace("(Original)", "");
	}

	void SetClipboardGameObject(GameObject selObject)
	{
		m_CopyCloneObject = FXMakerHierarchy.inst.AddGameObject(gameObject, selObject);
	}

	void SetClipboardComponent(Component selCom)
	{
		m_CopyCloneGameObject = NgObject.CreateGameObject(gameObject.transform, selCom.gameObject.name);
		NgObject.SetActiveRecursively(m_CopyCloneGameObject, false);
		m_CopyCloneObject	= NgSerialized.AddComponent(m_CopyCloneGameObject, selCom);
		NgSerialized.CopySerialized(selCom, (m_CopyCloneObject as Component));
	}

	// UpdateLoop -----------------------------------------------------------------------
	FXMakerClipboard()
	{
		inst = this;
	}

	void OnEnable()
	{
		ClearClipboard();
	}
	// Event -------------------------------------------------------------------------
	// Function ----------------------------------------------------------------------
	public static Object PasteObject(Object srcObject, bool bSrcClipboardObj, Transform tarTrans, Object tarObj, int tarIndex, bool bCreateNewTarget, bool bSearchTarComponent, bool bCopyAllTarComs)
	{
		Object retObject = null;

		// move child
		if (srcObject is GameObject)
		{
			GameObject dropObject = srcObject as GameObject;
			dropObject.transform.parent = tarTrans;
			return dropObject;
		}

		// New or Overwrite data
		if (srcObject is Component)
		{
			if (srcObject is Transform)
				retObject = FXMakerClipboard.ObjectCopy((srcObject as Transform), tarTrans, FXMakerOption.inst.m_bDragDrop_WorldSpace);
			else {
				retObject = FXMakerClipboard.ObjectCopy((srcObject as Component), tarTrans, tarObj, bCreateNewTarget, bSearchTarComponent, bCopyAllTarComs);
				if (bSrcClipboardObj == false)
				{
					bool bCutCopy = true;
					if (retObject != null && bCutCopy)
						FXMakerHierarchy.inst.DeleteHierarchyObject(srcObject);
				}
			}
		}

		// New or Overwrite data
		if (srcObject is Material && tarTrans.GetComponent<Renderer>() != null)
			retObject = FXMakerClipboard.ObjectCopy((srcObject as Material), tarTrans, tarIndex);

		// New or Overwrite data
		if (srcObject is AnimationClip && tarTrans.GetComponent<Animation>() != null)
			retObject = FXMakerClipboard.ObjectCopy((srcObject as AnimationClip), tarTrans, tarIndex);

		return retObject;
	}

	public static Transform ObjectCopy(Transform src, Transform dst, bool bWorld)
	{
		if (bWorld)
		{
			dst.position	= src.position;
			dst.rotation	= src.rotation;
			dst.localScale	= Vector3.one;
			dst.localScale = new Vector3((dst.lossyScale.x == 0 ? 0 : src.lossyScale.x / (dst.lossyScale.x)),
										 (dst.lossyScale.y == 0 ? 0 : src.lossyScale.y / (dst.lossyScale.y)),
										 (dst.lossyScale.z == 0 ? 0 : src.lossyScale.z / (dst.lossyScale.z)));
		} else {
			dst.localPosition	= src.localPosition;
			dst.localRotation	= src.localRotation;
			dst.localScale		= dst.localScale;
		}
		return dst;
	}

	public static Component ObjectCopy(Component src, Transform dstSelTrans, Object dstSelObj, bool bCreateNewTarget, bool bSearchTarComponent, bool bCopyAllTarComs)
	{
		Component[] tarComs	= null;
		Component	retCom	= null;

		// bCreateNewTarget ���̿���, bSearchTarComponent ���̸� ���� ��������

		if ((bCreateNewTarget == false) && src.GetType() == dstSelObj.GetType())	// �űԻ��� ������ �ƴϸ�, �����Կ� ������
		{
// 			if (NgAssembly.IsValidCopy(dst, src))
			{
				tarComs		= new Component[1];
				tarComs[0]	= dstSelObj as Component;
			}
		} else {
			if (bSearchTarComponent)
				tarComs = dstSelTrans.GetComponents(src.GetType());
		}
		if (tarComs == null || tarComs.Length <= 0)					// ���� ������
		{
			tarComs		= new Component[1];
			tarComs[0]	= NgSerialized.AddComponent(dstSelTrans.gameObject, (src as Component));
		}
		if (tarComs != null)
		{
			foreach (Component tarcom in tarComs)					// ���� ����
			{
				if (src != tarcom)
				{
					NgSerialized.CopySerialized(src as Component, tarcom);
					FXMakerHierarchy.inst.OnAddComponent(tarcom);
					retCom	= tarcom;
					if (bCopyAllTarComs == false)
						break;
				}
			}
		}
		return retCom;
	}

	public static Material ObjectCopy(Material src, Transform dstSelTrans, int nDstSelIndex)
	{
		if (dstSelTrans.GetComponent<Renderer>() != null)
			return NgMaterial.SetSharedMaterial(dstSelTrans.GetComponent<Renderer>(), nDstSelIndex, src);
		return null;
	}

	public static AnimationClip ObjectCopy(AnimationClip src, Transform dstSelTrans, int nDstSelIndex)
	{
		if (dstSelTrans.GetComponent<Animation>() != null)
			return NgAnimation.SetAnimation(dstSelTrans.GetComponent<Animation>(), nDstSelIndex, src);
		return null;
	}
}
#endif
