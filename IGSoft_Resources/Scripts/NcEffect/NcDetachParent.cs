// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

// [AddComponentMenu("EsayTool/NcDetachParent	%#D")]

public class NcDetachParent : NcEffectBehaviour
{
	// Attribute ------------------------------------------------------------------------
	public		bool				m_bFollowParentTransform	= true;
	public		bool				m_bParentHideToStartDestroy	= true;
	public		float				m_fSmoothDestroyTime		= 2;
	public		bool				m_bDisableEmit				= true;
	public		bool				m_bSmoothHide				= true;
	public		bool				m_bMeshFilterOnlySmoothHide	= false;

	protected	bool				m_bStartDetach				= false;
	protected	float				m_fStartDestroyTime;						// parent �Ǵ� this�� deactive �� �ð� (m_ReadonlybIsPlayed ���̵� �ð�)
	protected	GameObject			m_ParentGameObject;
	protected	NcDetachObject		m_ncDetachObject			= null;
	protected	NcTransformTool		m_OriginalPos				= new NcTransformTool();

	// Property -------------------------------------------------------------------------
#if UNITY_EDITOR
	public override string CheckProperty()
	{
		if (1 < gameObject.GetComponents(GetType()).Length)
			return "SCRIPT_WARRING_DUPLICATE";

		if (m_bFollowParentTransform && (GetComponent("NcCurveAnimation") != null || GetComponent("NcRotation") != null || GetComponent("Animation") != null) || GetComponent<Rigidbody>() != null)
			return "SCRIPT_WARRING_NCDETACHPARENT";
		return "";	// no error
	}
#endif

	// Function --------------------------------------------------------------------
	public void SetDestroyValue(bool bParentHideToStart, bool bStartDisableEmit, float fSmoothDestroyTime, bool bSmoothHide, bool bMeshFilterOnlySmoothHide)
	{
		m_bParentHideToStartDestroy	= bParentHideToStart;
		m_bDisableEmit				= bStartDisableEmit;
		m_bSmoothHide				= bSmoothHide;
		m_fSmoothDestroyTime		= fSmoothDestroyTime;
		m_bMeshFilterOnlySmoothHide	= bMeshFilterOnlySmoothHide;
	}

	// Loop Function --------------------------------------------------------------------
	protected override void OnDestroy()
	{
		if (m_ncDetachObject != null)
			Destroy(m_ncDetachObject);
		base.OnDestroy();
	}

	void Update()
	{
		if (m_bStartDetach == false)
		{
			m_bStartDetach = true;

			if (transform.parent != null)
			{
				m_ParentGameObject = transform.parent.gameObject;
				m_ncDetachObject	= NcDetachObject.Create(m_ParentGameObject, transform.gameObject);
			}

			// Detach Parent
			GameObject	parentObj = GetRootInstanceEffect();

			if (m_bFollowParentTransform)
			{
				m_OriginalPos.SetLocalTransform(transform);
				ChangeParent(parentObj.transform, transform, false, null);
				m_OriginalPos.CopyToLocalTransform(transform);
			} else {
				ChangeParent(parentObj.transform, transform, false, null);
			}

			if (m_bParentHideToStartDestroy == false)
				StartDestroy();
		}

		// Live time
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
					Object.Destroy(gameObject);
			}
		} else {
			if (m_bParentHideToStartDestroy)
				if (m_ParentGameObject == null || IsActive(m_ParentGameObject) == false)
					StartDestroy();
		}

		// follow parent transform
 		if (m_bFollowParentTransform && m_ParentGameObject != null && m_ParentGameObject.transform != null)
 		{
 			NcTransformTool	tmp = new NcTransformTool();
 			tmp.SetTransform(m_OriginalPos);
 			tmp.AddTransform(m_ParentGameObject.transform);
 			tmp.CopyToLocalTransform(transform);
 		}
	}
	// Control Function -----------------------------------------------------------------
	void StartDestroy()
	{
		m_fStartDestroyTime	= GetEngineTime();
		if (m_bDisableEmit)
			DisableEmit();
	}

	// Event Function -------------------------------------------------------------------
	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		m_fSmoothDestroyTime	/= fSpeedRate;
	}
}
