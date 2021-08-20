using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace DigitalRuby.RainMaker
{
    public class BaseRainScript : MonoBehaviour
    {
        [Tooltip("Camera the rain should hover over, defaults to main camera")]
        public Camera Camera;

        [Tooltip("Light rain looping clip")]
        public AudioClip RainSoundLight;

        [Tooltip("Medium rain looping clip")]
        public AudioClip RainSoundMedium;

        [Tooltip("Heavy rain looping clip")]
        public AudioClip RainSoundHeavy;

        [Tooltip("Intensity of rain (0-1)")]
        [Range(0.0f, 1.0f)]
        public float RainIntensity;

        [Tooltip("Rain particle system")]
        public ParticleSystem RainFallParticleSystem;

        [Tooltip("Particles system for when rain hits something")]
        public ParticleSystem RainExplosionParticleSystem;

        [Tooltip("Particle system to use for rain mist")]
        public ParticleSystem RainMistParticleSystem;

        [Tooltip("The threshold for intensity (0 - 1) at which mist starts to appear")]
        [Range(0.0f, 1.0f)]
        public float RainMistThreshold = 0.5f;

        [Tooltip("Wind looping clip")]
        public AudioClip WindSound;

        [Tooltip("Wind sound volume modifier, use this to lower your sound if it's too loud.")]
        public float WindSoundVolumeModifier = 0.5f;

        [Tooltip("Wind zone that will affect and follow the rain")]
        public WindZone WindZone;

        [Tooltip("X = minimum wind speed. Y = maximum wind speed. Z = sound multiplier. Wind speed is divided by Z to get sound multiplier value. Set Z to lower than Y to increase wind sound volume, or higher to decrease wind sound volume.")]
        public Vector3 WindSpeedRange = new Vector3(50.0f, 500.0f, 500.0f);

        [Tooltip("How often the wind speed and direction changes (minimum and maximum change interval in seconds)")]
        public Vector2 WindChangeInterval = new Vector2(5.0f, 30.0f);

        [Tooltip("Whether wind should be enabled.")]
        public bool EnableWind = true;

        protected LoopingAudioSource audioSourceRainLight;
        protected LoopingAudioSource audioSourceRainMedium;
        protected LoopingAudioSource audioSourceRainHeavy;
        protected LoopingAudioSource audioSourceRainCurrent;
        protected LoopingAudioSource audioSourceWind;
        protected Material rainMaterial;
        protected Material rainExplosionMaterial;
        protected Material rainMistMaterial;

        private float lastRainIntensityValue = -1.0f;
        private float nextWindTime;

        private void UpdateWind()
        {
            if (EnableWind && WindZone != null && WindSpeedRange.y > 1.0f)
            {
                WindZone.gameObject.SetActive(true);
                WindZone.transform.position = Camera.transform.position;
                if (!Camera.orthographic)
                {
                    WindZone.transform.Translate(0.0f, WindZone.radius, 0.0f);
                }
                if (nextWindTime < Time.time)
                {
                    WindZone.windMain = UnityEngine.Random.Range(WindSpeedRange.x, WindSpeedRange.y);
                    WindZone.windTurbulence = UnityEngine.Random.Range(WindSpeedRange.x, WindSpeedRange.y);
                    if (Camera.orthographic)
                    {
                        int val = UnityEngine.Random.Range(0, 2);
                        WindZone.transform.rotation = Quaternion.Euler(0.0f, (val == 0 ? 90.0f : -90.0f), 0.0f);
                    }
                    else
                    {
                        WindZone.transform.rotation = Quaternion.Euler(UnityEngine.Random.Range(-30.0f, 30.0f), UnityEngine.Random.Range(0.0f, 360.0f), 0.0f);
                    }
                    nextWindTime = Time.time + UnityEngine.Random.Range(WindChangeInterval.x, WindChangeInterval.y);
                    audioSourceWind.Play((WindZone.windMain / WindSpeedRange.z) * WindSoundVolumeModifier);
                }
            }
            else
            {
                if (WindZone != null)
                {
                    WindZone.gameObject.SetActive(false);
                }
                audioSourceWind.Stop();
            }

            audioSourceWind.Update();
        }

        private void CheckForRainChange()
        {
            if (lastRainIntensityValue != RainIntensity)
            {
                lastRainIntensityValue = RainIntensity;
                if (RainIntensity <= 0.01f)
                {
                    if (audioSourceRainCurrent != null)
                    {
                        audioSourceRainCurrent.Stop();
                        audioSourceRainCurrent = null;
                    }
                    if (RainFallParticleSystem != null)
                    {
                        ParticleSystem.EmissionModule e = RainFallParticleSystem.emission;
                        e.enabled = false;
                        RainFallParticleSystem.Stop();
                    }
                    if (RainMistParticleSystem != null)
                    {
                        ParticleSystem.EmissionModule e = RainMistParticleSystem.emission;
                        e.enabled = false;
                        RainMistParticleSystem.Stop();
                    }
                }
                else
                {
                    LoopingAudioSource newSource;
                    if (RainIntensity >= 0.67f)
                    {
                        newSource = audioSourceRainHeavy;
                    }
                    else if (RainIntensity >= 0.33f)
                    {
                        newSource = audioSourceRainMedium;
                    }
                    else
                    {
                        newSource = audioSourceRainLight;
                    }
                    if (audioSourceRainCurrent != newSource)
                    {
                        if (audioSourceRainCurrent != null)
                        {
                            audioSourceRainCurrent.Stop();
                        }
                        audioSourceRainCurrent = newSource;
                        audioSourceRainCurrent.Play(1.0f);
                    }
                    if (RainFallParticleSystem != null)
                    {
                        ParticleSystem.EmissionModule e = RainFallParticleSystem.emission;
                        e.enabled = RainFallParticleSystem.GetComponent<Renderer>().enabled = true;
                        RainFallParticleSystem.Play();
                        ParticleSystem.MinMaxCurve rate = e.rateOverTime;
                        rate.mode = ParticleSystemCurveMode.Constant;
                        rate.constantMin = rate.constantMax = RainFallEmissionRate();
                        e.rateOverTime = rate;
                    }
                    if (RainMistParticleSystem != null)
                    {
                        ParticleSystem.EmissionModule e = RainMistParticleSystem.emission;
                        e.enabled = RainMistParticleSystem.GetComponent<Renderer>().enabled = true;
                        RainMistParticleSystem.Play();
                        float emissionRate;
                        if (RainIntensity < RainMistThreshold)
                        {
                            emissionRate = 0.0f;
                        }
                        else
                        {
                            // must have RainMistThreshold or higher rain intensity to start seeing mist
                            emissionRate = MistEmissionRate();
                        }
                        ParticleSystem.MinMaxCurve rate = e.rateOverTime;
                        rate.mode = ParticleSystemCurveMode.Constant;
                        rate.constantMin = rate.constantMax = emissionRate;
                        e.rateOverTime = rate;
                    }
                }
            }
        }

        protected virtual void Start()
        {

#if DEBUG

            if (RainFallParticleSystem == null)
            {
                Debug.LogError("Rain fall particle system must be set to a particle system");
                return;
            }

#endif

            if (Camera == null)
            {
                Camera = Camera.main;
            }

            audioSourceRainLight = new LoopingAudioSource(this, RainSoundLight);
            audioSourceRainMedium = new LoopingAudioSource(this, RainSoundMedium);
            audioSourceRainHeavy = new LoopingAudioSource(this, RainSoundHeavy);
            audioSourceWind = new LoopingAudioSource(this, WindSound);

            if (RainFallParticleSystem != null)
            {
                ParticleSystem.EmissionModule e = RainFallParticleSystem.emission;
                e.enabled = false;
                Renderer rainRenderer = RainFallParticleSystem.GetComponent<Renderer>();
                rainRenderer.enabled = false;
                rainMaterial = new Material(rainRenderer.material);
                rainMaterial.EnableKeyword("SOFTPARTICLES_OFF");
                rainRenderer.material = rainMaterial;
            }
            if (RainExplosionParticleSystem != null)
            {
                ParticleSystem.EmissionModule e = RainExplosionParticleSystem.emission;
                e.enabled = false;
                Renderer rainRenderer = RainExplosionParticleSystem.GetComponent<Renderer>();
                rainExplosionMaterial = new Material(rainRenderer.material);
                rainExplosionMaterial.EnableKeyword("SOFTPARTICLES_OFF");
                rainRenderer.material = rainExplosionMaterial;
            }
            if (RainMistParticleSystem != null)
            {
                ParticleSystem.EmissionModule e = RainMistParticleSystem.emission;
                e.enabled = false;
                Renderer rainRenderer = RainMistParticleSystem.GetComponent<Renderer>();
                rainRenderer.enabled = false;
                rainMistMaterial = new Material(rainRenderer.material);
                if (UseRainMistSoftParticles)
                {
                    rainMistMaterial.EnableKeyword("SOFTPARTICLES_ON");
                }
                else
                {
                    rainMistMaterial.EnableKeyword("SOFTPARTICLES_OFF");
                }
                rainRenderer.material = rainMistMaterial;
            }
        }

        protected virtual void Update()
        {

#if DEBUG

            if (RainFallParticleSystem == null)
            {
                Debug.LogError("Rain fall particle system must be set to a particle system");
                return;
            }

#endif

            CheckForRainChange();
            UpdateWind();
            audioSourceRainLight.Update();
            audioSourceRainMedium.Update();
            audioSourceRainHeavy.Update();
        }

        protected virtual float RainFallEmissionRate()
        {
            return RainFallParticleSystem.main.maxParticles / RainFallParticleSystem.main.startLifetimeMultiplier * RainIntensity;
        }

        protected virtual float MistEmissionRate()
        {
            return RainMistParticleSystem.main.maxParticles / RainMistParticleSystem.main.startLifetimeMultiplier * RainIntensity * RainIntensity;
        }

        protected virtual bool UseRainMistSoftParticles
        {
            get
            {
                return true;
            }
        }
    }

    /// <summary>
    /// Provides an easy wrapper to looping audio sources with nice transitions for volume when starting and stopping
    /// </summary>
    public class LoopingAudioSource
    {
        public AudioSource AudioSource { get; private set; }
        public float TargetVolume { get; private set; }

        public LoopingAudioSource(MonoBehaviour script, AudioClip clip)
        {
            AudioSource = script.gameObject.AddComponent<AudioSource>();
            AudioSource.loop = true;
            AudioSource.clip = clip;
            AudioSource.Stop();
            TargetVolume = 1.0f;
        }

        public void Play(float targetVolume)
        {
            if (!AudioSource.isPlaying)
            {
                AudioSource.volume = 0.0f;
                AudioSource.Play();
            }
            TargetVolume = targetVolume;
        }

        public void Stop()
        {
            TargetVolume = 0.0f;
        }

        public void Update()
        {
            if (AudioSource.isPlaying && (AudioSource.volume = Mathf.Lerp(AudioSource.volume, TargetVolume, Time.deltaTime)) == 0.0f)
            {
                AudioSource.Stop();
            }
        }
    }

}