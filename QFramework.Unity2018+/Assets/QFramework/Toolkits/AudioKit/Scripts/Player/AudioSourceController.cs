using System;
using UnityEngine;

namespace QFramework
{
    public class AudioSourceController : IPoolable, IPoolType
    {
        private AudioSource mAudioSource;

        public static AudioSourceController Allocate(GameObject root, string name)
        {
            var controller = SafeObjectPool<AudioSourceController>.Instance.Allocate();

            if (!controller.mAudioSource)
            {
                controller.mAudioSource = new GameObject(name)
                    .AddComponent<AudioSource>();
            }

            controller.mAudioSource
                .Parent(root)
                .Name(name)
                .Show();

            return controller;
        }

        public void OnRecycled()
        {
            mAudioSource
                .Parent(AudioManager.Instance)
                .Hide();
        }

        public bool IsRecycled { get; set; }
        public AudioClip Clip => mAudioSource.clip;

        public void Recycle2Cache()
        {
            SafeObjectPool<AudioSourceController>.Instance.Recycle(this);
        }

        public void Update(GameObject root, string name)
        {
            mAudioSource
                .Parent(root)
                .Name(name)
                .Show();
        }

        private bool mPaused = false;

        public void Pause()
        {
            mAudioSource.Pause();
            mPaused = true;
        }

        private bool mPreviousPlayStatus = false;


        private Action mOnSoundPlayFinish = null;


        private IActionController mAction = null;

        void RegisterOnSoundPlayFinish()
        {
            if (mAction != null)
            {
                mAction.Deinit();
                mAction = null;
            }

            mAction = ActionKit.Condition(() => !mPaused && !mAudioSource.isPlaying, () =>
                {
                    mOnSoundPlayFinish?.Invoke();
                    mOnSoundPlayFinish = null;
                    mAction = null;
                })
                .StartGlobal();
        }

        public void Play(Action onSoundPlayFinish)
        {
            mPaused = false;
            mAudioSource.Play();
            mOnSoundPlayFinish = onSoundPlayFinish;
            RegisterOnSoundPlayFinish();
        }

        public void SetPitch(float pitch)
        {
            if (mAudioSource)
            {
                mAudioSource.pitch = pitch;
            }
        }

        public void SetVolume(float volume)
        {
            if (mAudioSource)
            {
                mAudioSource.volume = volume;
            }
        }

        public bool AudioSourceIsNull()
        {
            return mAudioSource == null;
        }

        public void SetClip(AudioClip clip)
        {
            mAudioSource.clip = clip;
        }

        public void SetLoop(bool loop)
        {
            mAudioSource.loop = loop;
        }

        public void Stop()
        {
            mAudioSource.Stop();
        }
    }
}