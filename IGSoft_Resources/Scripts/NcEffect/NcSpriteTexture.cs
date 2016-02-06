// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class NcSpriteTexture : NcEffectBehaviour
{
	// Attribute ------------------------------------------------------------------------
	public		GameObject						m_NcSpriteFactoryPrefab	= null;
	protected	NcSpriteFactory					m_NcSpriteFactoryCom	= null;
	public		NcSpriteFactory.NcFrameInfo[]	m_NcSpriteFrameInfos	= null;
	public		float							m_fUvScale				= 1;
	public		int								m_nSpriteFactoryIndex	= 0;
	public		int								m_nFrameIndex			= 0;

	public		NcSpriteFactory.MESH_TYPE		m_MeshType				= NcSpriteFactory.MESH_TYPE.BuiltIn_Plane;
	public		NcSpriteFactory.ALIGN_TYPE		m_AlignType				= NcSpriteFactory.ALIGN_TYPE.CENTER;
	public		float							m_fShowRate				= 1.0f;

	protected	GameObject						m_EffectObject			= null;

	// Property -------------------------------------------------------------------------
#if UNITY_EDITOR
	public override string CheckProperty()
	{
		if (1 < gameObject.GetComponents(GetType()).Length)
			return "SCRIPT_WARRING_DUPLICATE";
		if ((m_NcSpriteFactoryPrefab == null || m_NcSpriteFactoryPrefab.GetComponent<NcSpriteFactory>() == null) && (GetComponent<NcSpriteFactory>() == null))
			return "SCRIPT_EMPTY_SPRITEFACTORY";
		if (1 < GetEditingUvComponentCount())
			return "SCRIPT_DUPERR_EDITINGUV";
		if (GetComponent<Renderer>() == null || GetComponent<Renderer>().sharedMaterial == null)
			return "SCRIPT_EMPTY_MATERIAL";

		return "";	// no error
	}
#endif

	// Loop Function --------------------------------------------------------------------
	void Awake()
	{
		if (m_MeshFilter == null)
		{
			m_MeshFilter = gameObject.GetComponent<MeshFilter>();
#if UNITY_EDITOR
			if (IsCreatingEditObject() == false)
#endif
				if (m_MeshFilter == null)
					m_MeshFilter = gameObject.AddComponent<MeshFilter>();
		}

		if (m_NcSpriteFactoryPrefab == null && gameObject.GetComponent<NcSpriteFactory>() != null)
			m_NcSpriteFactoryPrefab = gameObject;
		UpdateFactoryInfo(m_nSpriteFactoryIndex);
	}

	void Start()
	{
		UpdateSpriteTexture(true);
	}

// 	void Update()
// 	{
// 	}

	// Control Function -----------------------------------------------------------------
	public void SetSpriteFactoryIndex(string spriteName, int nFrameIndex, bool bRunImmediate)
	{
		if (m_NcSpriteFactoryCom == null)
		{
			if (m_NcSpriteFactoryPrefab && m_NcSpriteFactoryPrefab.GetComponent<NcSpriteFactory>() != null)
				m_NcSpriteFactoryCom = m_NcSpriteFactoryPrefab.GetComponent<NcSpriteFactory>();
			else return;
		}
		m_nSpriteFactoryIndex = m_NcSpriteFactoryCom.GetSpriteNodeIndex(spriteName);
		SetSpriteFactoryIndex(m_nSpriteFactoryIndex, nFrameIndex, bRunImmediate);
	}

	public void SetSpriteFactoryIndex(int nSpriteFactoryIndex, int nFrameIndex, bool bRunImmediate)
	{
		if (UpdateFactoryInfo(nSpriteFactoryIndex) == false)
			return;
		SetFrameIndex(nFrameIndex);
		if (bRunImmediate)
			UpdateSpriteTexture(bRunImmediate);
	}

	public void SetFrameIndex(int nFrameIndex)
	{
		m_nFrameIndex = (0 <= nFrameIndex ? nFrameIndex : m_nFrameIndex);
		if (m_NcSpriteFrameInfos == null)
			return;
		m_nFrameIndex = (m_NcSpriteFrameInfos.Length == 0 ? 0 : (m_NcSpriteFrameInfos.Length <= m_nFrameIndex ? m_NcSpriteFrameInfos.Length - 1 : m_nFrameIndex));
	}

	public void SetShowRate(float fShowRate)
	{
		m_fShowRate = fShowRate;
		UpdateSpriteTexture(true);
	}

	bool UpdateFactoryInfo(int nSpriteFactoryIndex)
	{
		m_nSpriteFactoryIndex	= nSpriteFactoryIndex;
		if (m_NcSpriteFactoryCom == null)
		{
			if (m_NcSpriteFactoryPrefab && m_NcSpriteFactoryPrefab.GetComponent<NcSpriteFactory>() != null)
				m_NcSpriteFactoryCom = m_NcSpriteFactoryPrefab.GetComponent<NcSpriteFactory>();
			else return false;
		}
		if (m_NcSpriteFactoryCom.IsValidFactory() == false)
			return false;
		m_NcSpriteFrameInfos	= m_NcSpriteFactoryCom.GetSpriteNode(m_nSpriteFactoryIndex).m_FrameInfos;
		m_fUvScale				= m_NcSpriteFactoryCom.m_fUvScale;
		return true;
	}

	void UpdateSpriteTexture(bool bShowEffect)
	{
		if (UpdateSpriteMaterial() == false)
			return;
		if (m_NcSpriteFactoryCom.IsValidFactory() == false)
			return;
		if (m_NcSpriteFrameInfos.Length == 0)
			SetSpriteFactoryIndex(m_nSpriteFactoryIndex, m_nFrameIndex, false);

		if (m_MeshFilter == null)
		{
			if (gameObject.GetComponent<MeshFilter>() != null)
				 m_MeshFilter = gameObject.GetComponent<MeshFilter>();
			else m_MeshFilter = gameObject.AddComponent<MeshFilter>();
		}
		NcSpriteFactory.CreatePlane(m_MeshFilter, m_fUvScale, m_NcSpriteFrameInfos[m_nFrameIndex], false, m_AlignType, m_MeshType, m_fShowRate);
  		NcSpriteFactory.UpdateMeshUVs(m_MeshFilter, m_NcSpriteFrameInfos[m_nFrameIndex].m_TextureUvOffset, m_AlignType, m_fShowRate);
// 		{
// // 			Debug.Log("m_Renderer.material");
// 			renderer.material.mainTextureScale	= new Vector2(m_fTilingX, m_fTilingY);
// 			renderer.material.mainTextureOffset	= new Vector2(m_fOffsetX, m_fOffsetY);
// 		}

		if (bShowEffect)
			m_EffectObject = m_NcSpriteFactoryCom.CreateSpriteEffect(m_nSpriteFactoryIndex, transform);
	}

	public bool UpdateSpriteMaterial()
	{
		if (m_NcSpriteFactoryPrefab == null)
			return false;
		if (m_NcSpriteFactoryPrefab.GetComponent<Renderer>() == null || m_NcSpriteFactoryPrefab.GetComponent<Renderer>().sharedMaterial == null || m_NcSpriteFactoryPrefab.GetComponent<Renderer>().sharedMaterial.mainTexture == null)
			return false;
		if (GetComponent<Renderer>() == null)
			return false;
		
		if (m_NcSpriteFactoryCom == null)
			return false;
		if (m_nSpriteFactoryIndex < 0 || m_NcSpriteFactoryCom.GetSpriteNodeCount() <= m_nSpriteFactoryIndex)
			return false;
		if (m_NcSpriteFactoryCom.m_SpriteType != NcSpriteFactory.SPRITE_TYPE.NcSpriteTexture && m_NcSpriteFactoryCom.m_SpriteType != NcSpriteFactory.SPRITE_TYPE.Auto)
			return false;
		GetComponent<Renderer>().sharedMaterial = m_NcSpriteFactoryPrefab.GetComponent<Renderer>().sharedMaterial;
		return true;
	}

	// Event Function -------------------------------------------------------------------
	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
	}

	public override void OnUpdateToolData()
	{
	}
}

