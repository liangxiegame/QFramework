/****************************************************************************
 * Copyright (c) 2015 - 2025 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 * AudioKit v1.0: use QFramework.cs architecture
 ****************************************************************************/

using System;
using UnityEngine;

namespace QFramework
{
    internal class AudioSourceProxy
    {
        internal float VolumeScale { get; private set; } = 1.0f;
        internal float Volume { get; private set; } = 1.0f;
        internal float Pitch { get; private set; } = 1.0f;
        internal AudioClip AudioClip { get; private set; } = null;

        internal bool Paused { get; private set; } = false;
        internal bool Loop { get; private set; } = false;

        internal void InitParameters()
        {
            Volume = 1.0f;
            VolumeScale = 1.0f;
            Pitch = 1.0f;
            AudioClip = null;
            Loop = false;
        }

        internal AudioSource AudioSource { get; private set; }
        
        private Action mOnSoundPlayFinish = null;

        private IActionController mAction = null;


        internal void CreateOrUpdateAudioSource(GameObject root, string name)
        {
            if (!AudioSource)
            {
                AudioSource = new GameObject(name)
                    .AddComponent<AudioSource>();
            }

            AudioSource
                .Parent(root)
                .Name(name)
                .Show();
        }

        internal void OnParentRecycled()
        {
            if (!AudioSourceIsNull())
            {
                AudioSource
                    .Parent(AudioManager.Instance)
                    .Hide();
            }
        }

        internal void Pause()
        {
            AudioSource.Pause();
            Paused = true;
        }

        void RegisterOnSoundPlayFinish()
        {
            if (mAction != null)
            {
                mAction.Deinit();
                mAction = null;
            }

            mAction = ActionKit.Condition(() => !Paused && !AudioSource.isPlaying, () =>
                {
                    mOnSoundPlayFinish?.Invoke();
                    mOnSoundPlayFinish = null;
                    mAction = null;
                })
                .StartGlobal();
        }

        internal void ApplyParameters()
        {
            SetClip(AudioClip);
            SetLoop(Loop);
            SetVolume(Volume);
            SetVolumeScale(VolumeScale);
            SetPitch(Pitch);
        }

        internal void Play(Action onSoundPlayFinish)
        {
            ApplyParameters();

            Paused = false;
 
            AudioSource.Play();

            mOnSoundPlayFinish = onSoundPlayFinish;
            RegisterOnSoundPlayFinish();
        }

        internal void SetPitch(float pitch)
        {
            Pitch = pitch;

            if (AudioSource)
            {
                AudioSource.pitch = pitch;
            }
        }

        internal void SetVolumeScale(float volumeScale)
        {
            VolumeScale = volumeScale;
            UpdateVolume();
        }

        internal void SetVolume(float volume)
        {
            Volume = volume;
            UpdateVolume();
        }

        void UpdateVolume()
        {
            if (AudioSource)
            {
                AudioSource.volume = VolumeScale * Volume;
            }
        }

        internal bool AudioSourceIsNull()
        {
            return AudioSource == null;
        }

        internal void SetClip(AudioClip clip)
        {
            AudioClip = clip;

            if (AudioSource)
            {
                AudioSource.clip = clip;
            }
        }

        internal void SetLoop(bool loop)
        {
            Loop = loop;

            if (AudioSource)
            {
                AudioSource.loop = loop;
            }
        }

        internal void Stop()
        {
            if (AudioSource)
            {
                AudioSource.Stop();
            }
        }

        internal void StopAndClearClip()
        {
            Paused = false;
            if (!AudioSourceIsNull())
            {
                if (AudioSource.clip == AudioClip)
                {
                    Stop();
                    SetClip(null);
                }
            }
        }
    }
}