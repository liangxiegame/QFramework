// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

// [AddComponentMenu("FXMaker/NcAutoDestruct	%#F")]

public class NcChangeAlpha : NcEffectBehaviour
{
	// Attribute ------------------------------------------------------------------------
	public		enum				TARGET_TYPE				{MeshColor, MaterialColor};
	public		TARGET_TYPE			m_TargetType			= TARGET_TYPE.MeshColor;
	public		float				m_fDelayTime			= 2;
	public		float				m_fChangeTime			= 1;
	public		bool				m_bRecursively			= true;
	public		enum				CHANGE_MODE				{FromTo};
	public		CHANGE_MODE			m_ChangeMode			= CHANGE_MODE.FromTo;
	public		float				m_fFromAlphaValue		= 1;
	public		float				m_fToMeshValue			= 0;
	public		bool				m_bAutoDeactive			= true;

	// read only
	protected	float				m_fStartTime			= 0;
	protected	float				m_fStartChangeTime		= 0;

	// Create ---------------------------------------------------------------------------
	public static NcChangeAlpha SetChangeTime(GameObject baseGameObject, float fLifeTime, float fChangeTime, float fFromMeshAlphaValue, float fToMeshAlphaValue)
	{
		NcChangeAlpha ncChangeAlpha = baseGameObject.AddComponent<NcChangeAlpha>();
		ncChangeAlpha.SetChangeTime(fLifeTime, fChangeTime, fFromMeshAlphaValue, fToMeshAlphaValue);
		return ncChangeAlpha;
	}

	// Property -------------------------------------------------------------------------
#if UNITY_EDITOR
	public override string CheckProperty()
	{
		if (1 < gameObject.GetComponents(GetType()).Length)
			return "SCRIPT_WARRING_DUPLICATE";

		return "";	// no error
	}
#endif

	public void SetChangeTime(float fDelayTime, float fChangeTime, float fFromAlphaValue, float fToAlphaValue)
	{
		m_fDelayTime		= fDelayTime;
		m_fChangeTime		= fChangeTime;
		m_fFromAlphaValue	= fFromAlphaValue;
		m_fToMeshValue		= fToAlphaValue;
		if (IsActive(gameObject))
		{
			Start();
			Update();
		}
	}

	public void Restart()
	{
		m_fStartTime		= GetEngineTime();
		m_fStartChangeTime	= 0;
		ChangeToAlpha(0);
	}

	// Loop Function --------------------------------------------------------------------
	void Awake()
	{
		m_fStartTime		= 0;
		m_fStartChangeTime	= 0;
	}

	void Start()
	{
		Restart();
	}

	void Update()
	{
		if (0 < m_fStartChangeTime)
		{
			if (0 < m_fChangeTime)
			{
				float fElapsedRate = ((GetEngineTime() - m_fStartChangeTime) / m_fChangeTime);
				if (1 < fElapsedRate)
				{
					fElapsedRate = 1;
					// m_bAutoDeactive
					if (m_bAutoDeactive && m_fToMeshValue <= 0)
						SetActiveRecursively(gameObject, false);
				}
				ChangeToAlpha(fElapsedRate);
			} else {
				ChangeToAlpha(1);
			}
		} else {
			// Time
// 			if (0 < m_fStartTime && m_fLifeTime != 0)
			if (0 < m_fStartTime)
			{
				if (m_fStartTime + m_fDelayTime <= GetEngineTime())
					StartChange();
			}
		}
	}

	// Control Function -----------------------------------------------------------------
	void StartChange()
	{
		m_fStartChangeTime	= GetEngineTime();
	}

	void ChangeToAlpha(float fElapsedRate)
	{
		float	fAlphaValue = Mathf.Lerp(m_fFromAlphaValue, m_fToMeshValue, fElapsedRate);
		if (m_TargetType == TARGET_TYPE.MeshColor)
		{
			MeshFilter[] meshFilters;
			if (m_bRecursively)
				 meshFilters = transform.GetComponentsInChildren<MeshFilter>(true);
			else meshFilters = transform.GetComponents<MeshFilter>();
			Color		color;
			for (int n = 0; n < meshFilters.Length; n++)
			{
				Color[]	colors	= meshFilters[n].mesh.colors;
				if (colors.Length == 0)
				{
					if (meshFilters[n].mesh.vertices.Length == 0)
						NcSpriteFactory.CreateEmptyMesh(meshFilters[n]);
					colors = new Color[meshFilters[n].mesh.vertices.Length];
					for (int c = 0; c < colors.Length; c++)
						colors[c] = Color.white;
				}
				for (int c = 0; c < colors.Length; c++)
				{
					color		= colors[c];
					color.a		= fAlphaValue;
					colors[c]	= color;
				}
				meshFilters[n].mesh.colors	= colors;
			}
		} else {
			Renderer[] rens;
			if (m_bRecursively)
				 rens = transform.GetComponentsInChildren<Renderer>(true);
			else rens = transform.GetComponents<Renderer>();
			for (int n = 0; n < rens.Length; n++)
			{
				Renderer	ren		= rens[n];
				string		colName	= GetMaterialColorName(ren.sharedMaterial);

				if (colName != null)
				{
					Color col = ren.material.GetColor(colName);
					col.a		= fAlphaValue;
					ren.material.SetColor(colName, col);
				}
			}
		}

		if (fElapsedRate == 1 && fAlphaValue == 0)
		{
			SetActiveRecursively(gameObject, false);
		}
	}

	// Event Function -------------------------------------------------------------------
	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		m_fDelayTime		/= fSpeedRate;
		m_fChangeTime		/= fSpeedRate;
	}

	public override void OnSetReplayState()
	{
		base.OnSetReplayState();
		// Backup InitColor
		m_NcEffectInitBackup = new NcEffectInitBackup();
		if (m_TargetType == TARGET_TYPE.MeshColor)
			 m_NcEffectInitBackup.BackupMeshColor(gameObject, m_bRecursively);
		else m_NcEffectInitBackup.BackupMaterialColor(gameObject, m_bRecursively);
	}

	public override void OnResetReplayStage(bool bClearOldParticle)
	{
		base.OnResetReplayStage(bClearOldParticle);
		m_fStartTime		= GetEngineTime();
		m_fStartChangeTime	= 0;

		// Restore InitColor
		if (m_NcEffectInitBackup != null)
		{
			if (m_TargetType == TARGET_TYPE.MeshColor)
				 m_NcEffectInitBackup.RestoreMeshColor();
			else m_NcEffectInitBackup.RestoreMaterialColor();
		}
	}

}
