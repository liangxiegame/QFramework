// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

// --------------------------------------------------------------------------------------
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;

[CustomEditor(typeof(NcParticleSystem))]

public class NcParticleSystemEditor : FXMakerEditor
{
	// Attribute ------------------------------------------------------------------------
	protected	NcParticleSystem	m_Sel;
	protected	FxmPopupManager		m_FxmPopupManager;

	// Property -------------------------------------------------------------------------
	// Event Function -------------------------------------------------------------------
    void OnEnable()
    {
 		m_Sel = target as NcParticleSystem;
		m_UndoManager	= new FXMakerUndoManager(m_Sel, "NcParticleSystem");
    }

    void OnDisable()
    {
		if (m_FxmPopupManager != null && m_FxmPopupManager.IsShowByInspector())
			m_FxmPopupManager.CloseNcPrefabPopup();
    }

	public override void OnInspectorGUI()
	{
		AddScriptNameField(m_Sel);
		m_UndoManager.CheckUndo();

		m_FxmPopupManager = GetFxmPopupManager();

		bool bClickButton	= false;
		EditorGUI.BeginChangeCheck();
		{
			m_Sel.m_fUserTag = EditorGUILayout.FloatField(GetCommonContent("m_fUserTag"), m_Sel.m_fUserTag);

			m_Sel.m_fStartDelayTime				= EditorGUILayout.FloatField(GetHelpContent("m_fStartDelayTime")				, m_Sel.m_fStartDelayTime);
			// --------------------------------------------------------------------------------------------------------------------------------------------
			if (IsParticleEmitterOneShot())
			{
				Rect butRect = EditorGUILayout.BeginHorizontal(GUILayout.Height(m_fButtonHeight));
				{
					if (FXMakerLayout.GUIButton(butRect, GetHelpContent("Convert: OneShot To FXMakerBurst"), true))
					{
						ConvertOneShotToFXMakerBurst();
						return;
					}
					GUILayout.Label("");
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();
			}
			// --------------------------------------------------------------------------------------------------------------------------------------------
			m_Sel.m_bBurst						= EditorGUILayout.Toggle(GetHelpContent("m_bBurst")								, m_Sel.m_bBurst);
			if (m_Sel.m_bBurst)
			{
				m_Sel.m_fBurstRepeatTime		= EditorGUILayout.FloatField(GetHelpContent("m_fBurstRepeatTime")				, m_Sel.m_fBurstRepeatTime);
				m_Sel.m_nBurstRepeatCount		= EditorGUILayout.IntField	(GetHelpContent("m_nBurstRepeatCount")				, m_Sel.m_nBurstRepeatCount);
				m_Sel.m_fBurstEmissionCount		= EditorGUILayout.IntField  (GetHelpContent("m_fBurstEmissionCount")			, m_Sel.m_fBurstEmissionCount);
				SetMinValue(ref m_Sel.m_fBurstRepeatTime, 0.01f);
			} else {
				m_Sel.m_fEmitTime				= EditorGUILayout.FloatField(GetHelpContent("m_fEmitTime")						, m_Sel.m_fEmitTime);
				m_Sel.m_fSleepTime				= EditorGUILayout.FloatField(GetHelpContent("m_fSleepTime")						, m_Sel.m_fSleepTime);
			}

			m_Sel.m_bScaleWithTransform			= EditorGUILayout.Toggle	(GetHelpContent("m_bScaleWithTransform")			, m_Sel.m_bScaleWithTransform);
			// --------------------------------------------------------------------------------------------------------------------------------------------
			if (m_Sel.GetComponent<ParticleEmitter>() != null && m_Sel.m_bScaleWithTransform)	//  && m_Sel.transform.lossyScale != Vector3.one
			{
				Rect butRect = EditorGUILayout.BeginHorizontal(GUILayout.Height(m_fButtonHeight));
				{
					if (FXMakerLayout.GUIButton(butRect, GetHelpContent("Convert To Static Scale"), true))
					{
						ConvertToStaticScale(m_Sel.GetComponent<ParticleEmitter>(), m_Sel.GetComponent<ParticleAnimator>());
						m_Sel.m_bScaleWithTransform	= false;
						return;
					}
					GUILayout.Label("");
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();
			}

			// --------------------------------------------------------------------------------------------------------------------------------------------
			bool	bWorldSpace					= EditorGUILayout.Toggle	(GetHelpContent("m_bWorldSpace")					, m_Sel.m_bWorldSpace);
			// Set bWorldSpace
			if (m_Sel.m_bWorldSpace != bWorldSpace)
			{
				m_Sel.m_bWorldSpace	= bWorldSpace;
 				NgSerialized.SetSimulationSpaceWorld(m_Sel.transform, bWorldSpace);
			}
 			// Update bWorldSpace
 			if (m_Sel.m_bWorldSpace != NgSerialized.GetSimulationSpaceWorld(m_Sel.transform))
 				m_Sel.m_bWorldSpace	= !m_Sel.m_bWorldSpace;
			// --------------------------------------------------------------------------------------------------------------------------------------------

			m_Sel.m_fStartSizeRate				= EditorGUILayout.FloatField(GetHelpContent("m_fStartSizeRate")					, m_Sel.m_fStartSizeRate);
			m_Sel.m_fStartLifeTimeRate			= EditorGUILayout.FloatField(GetHelpContent("m_fStartLifeTimeRate")				, m_Sel.m_fStartLifeTimeRate);
			m_Sel.m_fStartEmissionRate			= EditorGUILayout.FloatField(GetHelpContent("m_fStartEmissionRate")				, m_Sel.m_fStartEmissionRate);
			m_Sel.m_fStartSpeedRate				= EditorGUILayout.FloatField(GetHelpContent("m_fStartSpeedRate")				, m_Sel.m_fStartSpeedRate);
			m_Sel.m_fRenderLengthRate			= EditorGUILayout.FloatField(GetHelpContent("m_fRenderLengthRate")				, m_Sel.m_fRenderLengthRate);

			if (m_Sel.GetComponent<ParticleEmitter>() != null && NgSerialized.IsMeshParticleEmitter(m_Sel.GetComponent<ParticleEmitter>()))
			{
				m_Sel.m_fLegacyMinMeshNormalVelocity= EditorGUILayout.FloatField(GetHelpContent("m_fLegacyMinMeshNormalVelocity")	, m_Sel.m_fLegacyMinMeshNormalVelocity);
				m_Sel.m_fLegacyMaxMeshNormalVelocity= EditorGUILayout.FloatField(GetHelpContent("m_fLegacyMaxMeshNormalVelocity")	, m_Sel.m_fLegacyMaxMeshNormalVelocity);
			}

			if (m_Sel.GetComponent<ParticleSystem>() != null)
			{
				float	fShurikenSpeedRate		= EditorGUILayout.FloatField(GetHelpContent("m_fShurikenSpeedRate")				, m_Sel.m_fShurikenSpeedRate);
				// Set particleSystem.speed
				if (m_Sel.m_fShurikenSpeedRate != fShurikenSpeedRate)
				{
					m_Sel.m_fShurikenSpeedRate = fShurikenSpeedRate;
					m_Sel.SaveShurikenSpeed();
				}
			}

			// m_ParticleDestruct
			m_Sel.m_ParticleDestruct			= (NcParticleSystem.ParticleDestruct)EditorGUILayout.EnumPopup(GetHelpContent("m_ParticleDestruct"), m_Sel.m_ParticleDestruct, GUILayout.MaxWidth(Screen.width));
			if (m_Sel.m_ParticleDestruct != NcParticleSystem.ParticleDestruct.NONE)
			{
				if (m_Sel.m_ParticleDestruct == NcParticleSystem.ParticleDestruct.COLLISION)
				{
					m_Sel.m_CollisionLayer		= LayerMaskField				(GetHelpContent("m_CollisionLayer")				, m_Sel.m_CollisionLayer);
					m_Sel.m_fCollisionRadius	= EditorGUILayout.FloatField	(GetHelpContent("m_fCollisionRadius")			, m_Sel.m_fCollisionRadius);
				}
				if (m_Sel.m_ParticleDestruct == NcParticleSystem.ParticleDestruct.WORLD_Y)
				{
					m_Sel.m_fDestructPosY		= EditorGUILayout.FloatField	(GetHelpContent("m_fDestructPosY")				, m_Sel.m_fDestructPosY);
				}
				m_Sel.m_AttachPrefab			= (GameObject)EditorGUILayout.ObjectField(GetHelpContent("m_AttachPrefab")		, m_Sel.m_AttachPrefab, typeof(GameObject), false, null);
				m_Sel.m_fPrefabScale			= EditorGUILayout.FloatField	(GetHelpContent("m_fPrefabScale")				, m_Sel.m_fPrefabScale);
				m_Sel.m_fPrefabSpeed			= EditorGUILayout.FloatField	(GetHelpContent("m_fPrefabSpeed")				, m_Sel.m_fPrefabSpeed);
				m_Sel.m_fPrefabLifeTime			= EditorGUILayout.FloatField	(GetHelpContent("m_fPrefabLifeTime")			, m_Sel.m_fPrefabLifeTime);

				SetMinValue(ref m_Sel.m_fCollisionRadius, 0.01f);
				SetMinValue(ref m_Sel.m_fPrefabScale, 0.01f);
				SetMinValue(ref m_Sel.m_fPrefabSpeed, 0.01f);
				SetMinValue(ref m_Sel.m_fPrefabLifeTime, 0);
				
				// --------------------------------------------------------------
				EditorGUILayout.Space();
				Rect attRect = EditorGUILayout.BeginHorizontal(GUILayout.Height(m_fButtonHeight));
				{
					if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(attRect, 3, 0, 1), GetHelpContent("Select Prefab"), (m_FxmPopupManager != null)))
						m_FxmPopupManager.ShowSelectPrefabPopup(m_Sel, true, 0);
					if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(attRect, 3, 1, 1), GetHelpContent("Clear Prefab"), (m_Sel.m_AttachPrefab != null)))
					{
						bClickButton = true;
						m_Sel.m_AttachPrefab = null;
					}
					if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerHorizontalRect(attRect, 3, 2, 1), GetHelpContent("Open Prefab"), (m_FxmPopupManager != null) && (m_Sel.m_AttachPrefab != null)))
					{
						bClickButton = true;
						GetFXMakerMain().OpenPrefab(m_Sel.m_AttachPrefab);
						return;
					}
					GUILayout.Label("");
				}
				EditorGUILayout.EndHorizontal();
			}


			// ---------------------------------------------------------------------
			// current ParticleSystem
			EditorGUILayout.Space();
			Component	currentParticle = null;
			Rect		rect = EditorGUILayout.BeginHorizontal(GUILayout.Height(m_fButtonHeight * 3));
			{
				if ((currentParticle = m_Sel.gameObject.GetComponent<ParticleSystem>()) != null)
				{
					ParticleSystemRenderer	psr = m_Sel.gameObject.GetComponent<ParticleSystemRenderer>();
					if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerVerticalRect(rect, 3, 0, 2), GetHelpContent("Delete Shuriken Components"), true))
					{
						bClickButton = true;
						Object.DestroyImmediate(currentParticle);
						if (psr != null)
							Object.DestroyImmediate(psr);
					}
					if (psr == null)
					{
			 			FXMakerLayout.GUIColorBackup(FXMakerLayout.m_ColorHelpBox);
						if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerVerticalRect(rect, 3, 2, 1), GetHelpContent("Add ParticleSystemRenderer"), true))
						{
							bClickButton = true;
							m_Sel.gameObject.AddComponent<ParticleSystemRenderer>();
						}
 						FXMakerLayout.GUIColorRestore();
					} else FXMakerLayout.GUIButton(FXMakerLayout.GetInnerVerticalRect(rect, 3, 2, 1),"ParticleSystemRenderer", false);
				} else {
					if ((currentParticle = m_Sel.gameObject.GetComponent<ParticleEmitter>()) != null)
					{
						ParticleAnimator	pa = m_Sel.gameObject.GetComponent<ParticleAnimator>();
						ParticleRenderer	pr = m_Sel.gameObject.GetComponent<ParticleRenderer>();

						if (currentParticle.ToString().Contains("EllipsoidParticleEmitter"))
						{
							if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerVerticalRect(rect, 3, 0, 1), GetHelpContent("Delete Legacy(Ellipsoid) Components"), true))
							{
								bClickButton = true;
								Object.DestroyImmediate(currentParticle);
								if (pa != null)	Object.DestroyImmediate(pa);
								if (pr != null)	Object.DestroyImmediate(pr);
							}
						} else {
							if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerVerticalRect(rect, 3, 0, 1), GetHelpContent("Delete Legacy(Mesh) Components"), true))
							{
								bClickButton = true;
								Object.DestroyImmediate(currentParticle);
								if (pa != null)	Object.DestroyImmediate(pa);
								if (pr != null)	Object.DestroyImmediate(pr);
							}
						}

						if (pa == null)
						{
			 				FXMakerLayout.GUIColorBackup(FXMakerLayout.m_ColorHelpBox);
							if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerVerticalRect(rect, 3, 1, 1), GetHelpContent("Add ParticleAnimator"), true))
							{
								bClickButton = true;
								m_Sel.gameObject.AddComponent<ParticleAnimator>();
							}
 							FXMakerLayout.GUIColorRestore();
						} else FXMakerLayout.GUIButton(FXMakerLayout.GetInnerVerticalRect(rect, 3, 1, 1),"ParticleAnimator", false);
						if (pr == null)
						{
			 				FXMakerLayout.GUIColorBackup(FXMakerLayout.m_ColorHelpBox);
							if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerVerticalRect(rect, 3, 2, 1), GetHelpContent("Add ParticleRenderer"), true))
							{
								bClickButton = true;
								m_Sel.gameObject.AddComponent<ParticleRenderer>();
							}
 							FXMakerLayout.GUIColorRestore();
						} else FXMakerLayout.GUIButton(FXMakerLayout.GetInnerVerticalRect(rect, 3, 2, 1),"ParticleRenderer", false);
					}
				}

				// ---------------------------------------------------------------------
				// Create ParticleSystem
				if (currentParticle == null)
				{
					if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerVerticalRect(rect, 3, 0, 1), GetHelpContent("Add Shuriken Components"), true))
					{
						bClickButton = true;
						m_Sel.gameObject.AddComponent<ParticleSystem>();
						if (m_Sel.gameObject.GetComponent<ParticleSystemRenderer>() == null)
							m_Sel.gameObject.AddComponent<ParticleSystemRenderer>();
					}
					if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerVerticalRect(rect, 3, 1, 1), GetHelpContent("Add Legacy(Ellipsoid) Components"), true))
					{
						bClickButton = true;
						m_Sel.gameObject.AddComponent<EllipsoidParticleEmitter>();
						if (m_Sel.gameObject.GetComponent<ParticleAnimator>() == null)
							m_Sel.gameObject.AddComponent<ParticleAnimator>();
						if (m_Sel.gameObject.GetComponent<ParticleRenderer>() == null)
							m_Sel.gameObject.AddComponent<ParticleRenderer>();
					}
					if (FXMakerLayout.GUIButton(FXMakerLayout.GetInnerVerticalRect(rect, 3, 2, 1), GetHelpContent("Add Legacy(Mesh) Components"), true))
					{
						bClickButton = true;
						m_Sel.gameObject.AddComponent<MeshParticleEmitter>();
						if (m_Sel.gameObject.GetComponent<ParticleAnimator>() == null)
							m_Sel.gameObject.AddComponent<ParticleAnimator>();
						if (m_Sel.gameObject.GetComponent<ParticleRenderer>() == null)
							m_Sel.gameObject.AddComponent<ParticleRenderer>();
					}
				}
				GUILayout.Label("");
			}
			EditorGUILayout.EndHorizontal();
		}
		m_UndoManager.CheckDirty();
		// ---------------------------------------------------------------------
		if ((EditorGUI.EndChangeCheck() || bClickButton) && GetFXMakerMain())
			OnEditComponent();
		// ---------------------------------------------------------------------
		if (GUI.tooltip != "")
			m_LastTooltip	= GUI.tooltip;
		HelpBox(m_LastTooltip);
	}

// --------------------------------------------------------------------------------------
	// Legacy Only...
	public static void ConvertToStaticScale(ParticleEmitter pe, ParticleAnimator pa)
	{
		if (pe == null)
			return;

		Vector3 vecVelScale	= (pe.transform.lossyScale);
		float fVelScale		= (NcTransformTool.GetTransformScaleMeanValue(pe.transform));
		float fScale		= (NcTransformTool.GetTransformScaleMeanValue(pe.transform));

		pe.minSize					*= fScale;
		pe.maxSize					*= fScale;

		pe.worldVelocity			=  Vector3.Scale(pe.worldVelocity, vecVelScale);
		pe.localVelocity			=  Vector3.Scale(pe.localVelocity, vecVelScale);
		pe.rndVelocity				=  Vector3.Scale(pe.rndVelocity, vecVelScale);
		pe.angularVelocity			*= fVelScale;
		pe.rndAngularVelocity		*= fVelScale;
		pe.emitterVelocityScale		*= fVelScale;

		if (pa != null)
		{
			pa.rndForce					=  Vector3.Scale(pa.rndForce, vecVelScale);
			pa.force					=  Vector3.Scale(pa.force, vecVelScale);
//			pa.damping					*= fScale;
		}
		Vector3		ellipsoid;
		float		minEmitterRange;

		NgSerialized.GetEllipsoidSize(pe, out ellipsoid, out minEmitterRange);
		NgSerialized.SetEllipsoidSize(pe, ellipsoid * fScale, minEmitterRange * fScale);
// 		NgAssembly.LogFieldsPropertis(pe);
	}

	protected bool IsParticleEmitterOneShot()
	{
		ParticleEmitter  pe = m_Sel.GetComponent<ParticleEmitter>();
		if (pe == null)
			return false;
		bool	bOneShot = (bool)NgSerialized.GetPropertyValue(new SerializedObject(pe as ParticleEmitter), "m_OneShot");
		return (bOneShot && m_Sel.m_bBurst == false);
	}

	protected void ConvertOneShotToFXMakerBurst()
	{
		ParticleEmitter  pe = m_Sel.GetComponent<ParticleEmitter>();
		ParticleAnimator pa = m_Sel.GetComponent<ParticleAnimator>();
		bool	bOneShot = (bool)NgSerialized.GetPropertyValue(new SerializedObject(pe as ParticleEmitter), "m_OneShot");
		if (bOneShot && m_Sel.m_bBurst == false)
		{
			m_Sel.m_bBurst				= true;
			m_Sel.m_fBurstRepeatTime	= pe.maxEnergy;
			m_Sel.m_nBurstRepeatCount	= 1;
			m_Sel.m_fBurstEmissionCount	= (int)Random.Range(pe.minEmission, pe.maxEmission+1);

			pe.emit = false;
			NgSerialized.SetPropertyValue(new SerializedObject(pe as ParticleEmitter), "m_OneShot", false, true);
			if (pa != null)
				pa.autodestruct = false;
		}
	}

	// ----------------------------------------------------------------------------------
	protected GUIContent GetHelpContent(string tooltip)
	{
		string caption	= tooltip;
		string text		= FXMakerTooltip.GetHsEditor_NcParticleSystem(tooltip);
		return GetHelpContent(caption, text);
	}

	protected override void HelpBox(string caption)
	{
		string	str	= caption;
		if (caption == "" || caption == "Script")
			str = FXMakerTooltip.GetHsEditor_NcParticleSystem("");
		base.HelpBox(str);
	}
}

