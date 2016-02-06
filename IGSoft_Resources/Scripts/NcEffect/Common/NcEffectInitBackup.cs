// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class NcEffectInitBackup
{
	// Attribute ------------------------------------------------------------------------
	protected	enum SAVE_TYPE		{NONE, ONE, RECURSIVELY};

	protected	SAVE_TYPE			m_SavedMaterialColor;
	protected	Renderer			m_MaterialColorRenderer;
	protected	string				m_MaterialColorColorName;
	protected	Vector4				m_MaterialColorSaveValue;
	protected	Renderer[]			m_MaterialColorRenderers;
	protected	string[]			m_MaterialColorColorNames;
	protected	Vector4[]			m_MaterialColorSaveValues;

	protected	SAVE_TYPE			m_SavedMeshColor;
	protected	MeshFilter			m_MeshColorMeshFilter;
	protected	Vector4				m_MeshColorSaveValue;
	protected	MeshFilter[]		m_MeshColorMeshFilters;
	protected	Vector4[]			m_MeshColorSaveValues;

	protected	NcUvAnimation		m_NcUvAnimation;
	protected	Vector2				m_UvAniSaveValue;

	protected	Transform			m_Transform;
	protected	NcTransformTool		m_NcTansform;


	// Property -------------------------------------------------------------------------
	// Loop Function --------------------------------------------------------------------
	// Control Function -----------------------------------------------------------------
	public void BackupTransform(Transform targetTrans)
	{
		m_Transform		= targetTrans;
		m_NcTansform	= new NcTransformTool(m_Transform);
	}

	public void RestoreTransform()
	{
		m_NcTansform.CopyToLocalTransform(m_Transform);
	}

	public void BackupMaterialColor(GameObject targetObj, bool bRecursively)
	{
		if (targetObj == null)
			return;
		if (bRecursively)
			 m_SavedMaterialColor = SAVE_TYPE.RECURSIVELY;
		else m_SavedMaterialColor = SAVE_TYPE.ONE;

		Transform targetTrans = targetObj.transform;

		if (m_SavedMaterialColor == SAVE_TYPE.RECURSIVELY)
		{
			// Recursively
			m_MaterialColorRenderers			= targetTrans.GetComponentsInChildren<Renderer>(true);
			m_MaterialColorColorNames			= new string[m_MaterialColorRenderers.Length];
			m_MaterialColorSaveValues	= new Vector4[m_MaterialColorRenderers.Length];

			for (int n = 0; n < m_MaterialColorRenderers.Length; n++)
			{
				Renderer ren			= m_MaterialColorRenderers[n];
				m_MaterialColorColorNames[n]	= GetMaterialColorName(ren.sharedMaterial);

				if (m_MaterialColorColorNames[n] != null)
					m_MaterialColorSaveValues[n] = ren.material.GetColor(m_MaterialColorColorNames[n]);
			}
		} else {
			// this Only
			m_MaterialColorRenderer	= targetTrans.GetComponent<Renderer>();
			if (m_MaterialColorRenderer != null)
			{
				m_MaterialColorColorName		= GetMaterialColorName(m_MaterialColorRenderer.sharedMaterial);
				if (m_MaterialColorColorName != null)
					m_MaterialColorSaveValue = m_MaterialColorRenderer.material.GetColor(m_MaterialColorColorName);
			}
		}
	}

	public void RestoreMaterialColor()
	{
		if (m_SavedMaterialColor == SAVE_TYPE.NONE)
			return;

		if (m_SavedMaterialColor == SAVE_TYPE.RECURSIVELY)
		{
			// Recursively
			for (int n = 0; n < m_MaterialColorRenderers.Length; n++)
			{
				if (m_MaterialColorRenderers[n] != null && m_MaterialColorColorNames[n] != null)
					m_MaterialColorRenderers[n].material.SetColor(m_MaterialColorColorNames[n], m_MaterialColorSaveValues[n]);
			}
		} else {
			// this Only
			if (m_MaterialColorRenderers != null)
			{
				m_MaterialColorColorName		= GetMaterialColorName(m_MaterialColorRenderer.sharedMaterial);
				if (m_MaterialColorColorName != null)
					m_MaterialColorRenderer.material.SetColor(m_MaterialColorColorName, m_MaterialColorSaveValue);
			}
		}
	}

	public void BackupUvAnimation(NcUvAnimation uvAniCom)
	{
		if (uvAniCom == null)
			return;
		m_NcUvAnimation		= uvAniCom;
		m_UvAniSaveValue	= new Vector2(m_NcUvAnimation.m_fScrollSpeedX, m_NcUvAnimation.m_fScrollSpeedY);
	}

	public void RestoreUvAnimation()
	{
		if (m_NcUvAnimation == null)
			return;
		m_NcUvAnimation.m_fScrollSpeedX	= m_UvAniSaveValue.x;
		m_NcUvAnimation.m_fScrollSpeedY	= m_UvAniSaveValue.y;
	}

	public void BackupMeshColor(GameObject targetObj, bool bRecursively)
	{
		if (targetObj == null)
			return;
		if (bRecursively)
			 m_SavedMeshColor = SAVE_TYPE.RECURSIVELY;
		else m_SavedMeshColor = SAVE_TYPE.ONE;

		if (m_SavedMeshColor == SAVE_TYPE.RECURSIVELY)
		{
			// Recursively
			m_MeshColorMeshFilters		= targetObj.GetComponentsInChildren<MeshFilter>(true);
			m_MeshColorSaveValues	= new Vector4[m_MeshColorMeshFilters.Length];

			if (m_MeshColorMeshFilters == null || m_MeshColorMeshFilters.Length < 0)
				return;
			for (int arrayIndex = 0; arrayIndex < m_MeshColorMeshFilters.Length; arrayIndex++)
				m_MeshColorSaveValues[arrayIndex]	= GetMeshColor(m_MeshColorMeshFilters[arrayIndex]);
		} else {
			// this Only
			m_MeshColorMeshFilter		= targetObj.GetComponent<MeshFilter>();
			m_MeshColorSaveValue	= GetMeshColor(m_MeshColorMeshFilter);
		}
	}

	public void RestoreMeshColor()
	{
		if (m_SavedMeshColor == SAVE_TYPE.NONE)
			return;
		if (m_SavedMeshColor == SAVE_TYPE.RECURSIVELY)
		{
			// Recursively
			if (m_MeshColorMeshFilters == null || m_MeshColorMeshFilters.Length < 0)
				return;
			for (int arrayIndex = 0; arrayIndex < m_MeshColorMeshFilters.Length; arrayIndex++)
				SetMeshColor(m_MeshColorMeshFilters[arrayIndex], m_MeshColorSaveValues[arrayIndex]);
		} else {
			// this Only
			SetMeshColor(m_MeshColorMeshFilter, m_MeshColorSaveValue);
		}
	}

	// ----------------------------------------------------------------------------------
	protected Color GetMeshColor(MeshFilter mFilter)
	{
		if (mFilter == null || mFilter.mesh == null)
			return Color.white;
		Color[]	colors = mFilter.mesh.colors;
		if (colors.Length == 0)
		{
			colors = new Color[mFilter.mesh.vertices.Length];
			for (int c = 0; c < colors.Length; c++)
				colors[c] = Color.white;
			mFilter.mesh.colors	= colors;
			return Color.white;
		}
		return colors[0];
	}

	protected void SetMeshColor(MeshFilter mFilter, Color tarColor)
	{
		if (mFilter == null || mFilter.mesh == null)
			return;
		Color[]	colors = mFilter.mesh.colors;
		if (colors.Length == 0)
		{
			colors = new Color[mFilter.mesh.vertices.Length];
			for (int c = 0; c < colors.Length; c++)
				colors[c] = Color.white;
		}
		for (int c = 0; c < colors.Length; c++)
			colors[c]	= tarColor;
		mFilter.mesh.colors	= colors;
	}

	protected static string GetMaterialColorName(Material mat)
	{
		string[] propertyNames = { "_Color", "_TintColor", "_EmisColor" };

		if (mat != null)
			foreach (string name in propertyNames)
				if (mat.HasProperty(name))
					return name;
		return null;
	}

	// Event Function -------------------------------------------------------------------
}
