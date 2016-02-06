// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class NsSharedManager : MonoBehaviour
{
	// Attribute ------------------------------------------------------------------------
	protected	static NsSharedManager	_inst;
	protected	List<GameObject>		m_SharedPrefabs			= new List<GameObject>();
	protected	List<GameObject>		m_SharedGameObjects		= new List<GameObject>();

	// AudioSource
	protected	List<AudioClip>			m_SharedAudioClip		= new List<AudioClip>();
	protected	List<List<AudioSource>>	m_SharedAudioSources	= new List<List<AudioSource>>();

	public static NsSharedManager inst
	{
		get
		{
			if (_inst == null)
				_inst = NcEffectBehaviour.GetRootInstanceEffect().AddComponent<NsSharedManager>();
			return _inst;
		}
	}

	// Property -------------------------------------------------------------------------
	// Loop Function --------------------------------------------------------------------
	// Control Function -----------------------------------------------------------------
	// Particle
	public GameObject GetSharedParticleGameObject(GameObject originalParticlePrefab)
	{
		int nIndex = m_SharedPrefabs.IndexOf(originalParticlePrefab);

		if (nIndex < 0 || m_SharedGameObjects[nIndex] == null)
		{
			if (NcEffectBehaviour.IsSafe() == false)
				return null;
			GameObject	sharedObj = (GameObject)Object.Instantiate(originalParticlePrefab);
			sharedObj.transform.parent = NcEffectBehaviour.GetRootInstanceEffect().transform;
			if (0 <= nIndex)
			{
				m_SharedGameObjects[nIndex] = sharedObj;
			} else {
				m_SharedPrefabs.Add(originalParticlePrefab);
				m_SharedGameObjects.Add(sharedObj);
			}
			// Init sharedObj
			NcParticleSystem ps = sharedObj.GetComponent<NcParticleSystem>();
			if (ps)
				ps.enabled = false;
			if (sharedObj.GetComponent<ParticleEmitter>())
			{
				sharedObj.GetComponent<ParticleEmitter>().emit			= false;
				sharedObj.GetComponent<ParticleEmitter>().useWorldSpace	= true;
				ParticleAnimator paAni = sharedObj.GetComponent<ParticleAnimator>();
				if (paAni)
					paAni.autodestruct = false;
			}
			NcParticleSystem ncPsCom = sharedObj.GetComponent<NcParticleSystem>();
			if (ncPsCom)
				ncPsCom.m_bBurst = false;
			ParticleSystem psCom = sharedObj.GetComponent<ParticleSystem>();
			if (psCom)
			{
				psCom.enableEmission = false;
			}

			return sharedObj;
		} else {
			return m_SharedGameObjects[nIndex];
		}
	}

	public void EmitSharedParticleSystem(GameObject originalParticlePrefab, int nEmitCount, Vector3 worldPos)
	{
		GameObject sharedObj = GetSharedParticleGameObject(originalParticlePrefab);
		if (sharedObj == null)
			return;
		sharedObj.transform.position = worldPos;
		if (sharedObj.GetComponent<ParticleEmitter>() != null)
			 sharedObj.GetComponent<ParticleEmitter>().Emit(nEmitCount);
		else {
				ParticleSystem ps = sharedObj.GetComponent<ParticleSystem>();
			if (ps != null)
				ps.Emit(nEmitCount);
		}
	}

	// AudioSource
	public AudioSource GetSharedAudioSource(AudioClip audioClip, int nPriority, bool bLoop, float fVolume, float fPitch)
	{
		int nIndex = m_SharedAudioClip.IndexOf(audioClip);

		if (nIndex < 0)
		{
			if (NcEffectBehaviour.IsSafe() == false)
				return null;

			List<AudioSource>	sourceList  = new List<AudioSource>();
			m_SharedAudioClip.Add(audioClip);
			m_SharedAudioSources.Add(sourceList);

			return AddAudioSource(sourceList, audioClip, nPriority, bLoop, fVolume, fPitch);
		} else {
			foreach (AudioSource audioSource in m_SharedAudioSources[nIndex])
			{
				if (audioSource.volume == fVolume && audioSource.pitch == fPitch && audioSource.loop == bLoop && audioSource.priority == nPriority)
					return audioSource;
			}
			// add
			return AddAudioSource(m_SharedAudioSources[nIndex], audioClip, nPriority, bLoop, fVolume, fPitch);
		}
	}

	AudioSource AddAudioSource(List<AudioSource> sourceList, AudioClip audioClip, int nPriority, bool bLoop, float fVolume, float fPitch)
	{
		AudioSource			audioSource = gameObject.AddComponent<AudioSource>();
		sourceList.Add(audioSource);

		// Init AudioSource
		audioSource.clip		= audioClip;
		audioSource.priority	= nPriority;
		audioSource.loop		= bLoop;
		audioSource.volume		= fVolume;
		audioSource.pitch		= fPitch;
		audioSource.playOnAwake	= false;

		return audioSource;
	}

	public void PlaySharedAudioSource(bool bUniquePlay, AudioClip audioClip, int nPriority, bool bLoop, float fVolume, float fPitch)
	{
		AudioSource	audioSource = GetSharedAudioSource(audioClip, nPriority, bLoop, fVolume, fPitch);
		if (audioSource == null)
			return;
		if (audioSource.isPlaying)
		{
			if (bUniquePlay)
				return;
			audioSource.Stop();
		}
		audioSource.Play();
	}

	// Event Function -------------------------------------------------------------------
}


