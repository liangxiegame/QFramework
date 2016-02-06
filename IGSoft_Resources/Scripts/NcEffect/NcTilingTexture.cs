// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class NcTilingTexture : NcEffectBehaviour
{
	// Attribute ------------------------------------------------------------------------
	public		float		m_fTilingX			= 2;
	public		float		m_fTilingY			= 2;
	public		float		m_fOffsetX			= 0;
	public		float		m_fOffsetY			= 0;

	public		bool		m_bFixedTileSize	= false;

	protected	Vector3		m_OriginalScale		= new Vector3();
	protected	Vector2		m_OriginalTiling	= new Vector2();

	// Property -------------------------------------------------------------------------
#if UNITY_EDITOR
	public override string CheckProperty()
	{
		if (1 < gameObject.GetComponents(GetType()).Length)
			return "SCRIPT_WARRING_DUPLICATE";
		if (1 < GetEditingUvComponentCount())
			return "SCRIPT_DUPERR_EDITINGUV";
		if (GetComponent<Renderer>() == null || GetComponent<Renderer>().sharedMaterial == null)
			return "SCRIPT_EMPTY_MATERIAL";

		return "";	// no error
	}
#endif

	// Loop Function --------------------------------------------------------------------
	void Start()
	{
		if (GetComponent<Renderer>() != null && GetComponent<Renderer>().material != null)
		{
			GetComponent<Renderer>().material.mainTextureScale	= new Vector2(m_fTilingX, m_fTilingY);
			GetComponent<Renderer>().material.mainTextureOffset = new Vector2(m_fOffsetX - ((int)m_fOffsetX), m_fOffsetY - ((int)m_fOffsetY));
			AddRuntimeMaterial(GetComponent<Renderer>().material);
		}
	}

	void Update()
	{
		if (m_bFixedTileSize)
		{
			if (m_OriginalScale.x != 0)
				m_fTilingX = m_OriginalTiling.x * (transform.lossyScale.x / m_OriginalScale.x);
			if (m_OriginalScale.y != 0)
				m_fTilingY = m_OriginalTiling.y * (transform.lossyScale.y / m_OriginalScale.y);
			GetComponent<Renderer>().material.mainTextureScale	= new Vector2(m_fTilingX, m_fTilingY);
		}
	}

	// Control Function -----------------------------------------------------------------
	// Event Function -------------------------------------------------------------------
	public override void OnUpdateToolData()
	{
		m_OriginalScale		= transform.lossyScale;
		m_OriginalTiling.x	= m_fTilingX;
		m_OriginalTiling.y	= m_fTilingY;
	}
}

