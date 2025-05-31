/****************************************************************************
 * Copyright (c) 2016 ~ 2025 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using UnityEngine;

namespace QFramework
{
    public class AudioPlayer : IPoolable, IPoolType, IAudioKitOnFinish, IAudioKitOnRelease
    {
        private IAudioLoader mLoader;
        private AudioSourceController mAudioSourceController;
        private GameObject mGameObject;

        private string mName;
        public BindableProperty<float> Volume { get; set; }

        public string GetName => mName;
        private AudioClip mAudioClip;

        private Action mOnStart = null;
        private Action mOnFinish = null;
        private Action mOnRelease = null;

        private bool mIsPause = false;
        private float mVolumeScale = 1.0f;
        private float mVolume;
        private float mPitch = 1;


        internal static AudioPlayer Allocate(BindableProperty<float> volume)
        {
            var player = SafeObjectPool<AudioPlayer>.Instance.Allocate();
            player.Volume = volume;
            player.mOnStart = null;
            player.mOnFinish = null;
            player.mVolume = 1.0f;
            player.mVolumeScale = 1.0f;
            player.mPitch = 1.0f;
            player.Volume.RegisterWithInitValue(player.SetVolume);
            return player;
        }
        
        public AudioPlayer OnStart(Action onStart)
        {
            if (onStart == null) return this;
            if (mOnStart == null)
            {
                mOnStart = onStart;
            }
            else
            {
                mOnStart += onStart;
            }

            return this;
        }

        void IAudioKitOnFinish.OnFinish(Action onFinish)
        {
            if (onFinish == null) return;

            if (mOnFinish == null)
            {
                mOnFinish = onFinish;
            }
            else
            {
                mOnFinish += onFinish;
            }
        }
        
        public bool UsedCache { get; set; } = true;
        public bool IsRecycled { get; set; } = false;
        public bool IsLoop { get; set; }

        void AllocateAudioSourceController(GameObject root, string name)
        {
            if (mAudioSourceController == null)
            {
                mAudioSourceController = AudioSourceController.Allocate(root,name);
            }
            else
            {
                mAudioSourceController.Update(root, name);
            }
        }

        public void SetAudioExt(GameObject root, AudioClip clip, string name, bool loop)
        {
            if (clip == null || mName == name)
            {
                return;
            }

            AllocateAudioSourceController(root, name);

            CleanResources();

            IsLoop = loop;
            mName = name;

            mAudioClip = clip;
            PlayAudioClip();
        }

        public AudioPlayer SetAudio(GameObject root, string name, bool loop)
        {
            if (string.IsNullOrEmpty(name))
            {
                return this;
            }

            if (mName == name)
            {
                return this;
            }

            AllocateAudioSourceController(root, name);

            //防止卸载后立马加载的情况
            var preLoader = mLoader;

            mLoader = null;

            CleanResources();

            mLoader = AudioKit.Config.AudioLoaderPool.AllocateLoader();

            IsLoop = loop;
            mName = name;

            var keys = AudioSearchKeys.Allocate();
            keys.AssetName = name;
            mLoader.LoadClipAsync(keys, OnResLoadFinish);
            keys.Recycle2Cache();

            if (preLoader != null)
            {
                preLoader.Unload();
                AudioKit.Config.AudioLoaderPool.RecycleLoader(preLoader);
                preLoader = null;
            }

            return this;
        }

        public void Stop()
        {
            Release();
        }

        public void Pause()
        {
            if (mIsPause)
            {
                return;
            }
            

            mIsPause = true;

            mAudioSourceController.Pause();
        }

        public void Resume()
        {
            if (!mIsPause)
            {
                return;
            }
            

            mIsPause = false;

            mOnStart?.Invoke();
            mOnStart = null;
            
            mAudioSourceController.Play(OnSoundPlayFinish);
        }

        public AudioPlayer Pitch(float pitch)
        {
            mPitch = pitch;
            UpdatePitch();
            return this;
        }

        void UpdatePitch()
        {
            if (mAudioSourceController != null)
            {
                mAudioSourceController.SetPitch(mPitch);
            }
        }

        public AudioPlayer VolumeScale(float volumeScale)
        {
            mVolumeScale = volumeScale;
            UpdateVolume();
            return this;
        }

        void SetVolume(float volume)
        {
            mVolume = volume;
            UpdateVolume();
        }

        void UpdateVolume()
        {
            if (mAudioSourceController != null)
            {
                mAudioSourceController.SetVolume(mVolume * mVolumeScale);
            }
        }

        private void OnResLoadFinish(bool result, AudioClip clip)
        {
            if (!result)
            {
                Release();
                return;
            }

            mAudioClip = clip;

            if (mAudioClip == null)
            {
                Debug.LogError("Asset Is Invalid AudioClip:" + mName);
                Release();
                return;
            }

            PlayAudioClip();
        }

        private void PlayAudioClip()
        {
            if (mAudioSourceController == null || mAudioSourceController.AudioSourceIsNull() || mAudioClip == null)
            {
                Release();
                return;
            }

            mAudioSourceController.SetClip(mAudioClip);
            mAudioSourceController.SetLoop(IsLoop);
            UpdateVolume();
            UpdatePitch();

            int loopCount = 1;
            if (IsLoop)
            {
                loopCount = -1;
            }
            

            if (mIsPause)
            {
                return;
            }
            
            mOnStart?.Invoke();
            mOnStart = null;

            mAudioSourceController.Play(OnSoundPlayFinish);
        }
        
        

        private void OnSoundPlayFinish()
        {
            if (!IsLoop)
            {
                mOnFinish?.Invoke();
                mOnFinish = null;
                Release();
            }
        }

        private void Release()
        {
            mOnRelease?.Invoke();
            mOnRelease = null;

            CleanResources();

            if (UsedCache)
            {
                Recycle2Cache();
            }
        }

        private void CleanResources()
        {
            mName = null;

            mIsPause = false;
            

            if (mAudioSourceController != null && !mAudioSourceController.AudioSourceIsNull())
            {
                if (mAudioSourceController.Clip == mAudioClip)
                {
                    mAudioSourceController.Stop();
                    mAudioSourceController.SetClip(null);
                }
            }

            mAudioClip = null;

            if (mLoader != null)
            {
                mLoader.Unload();
                AudioKit.Config.AudioLoaderPool.RecycleLoader(mLoader);
                mLoader = null;
            }
        }

        public void OnRecycled()
        {
            Volume?.UnRegister(SetVolume);
            Volume = null;

            CleanResources();
        }

        public void Recycle2Cache()
        {
            if (!SafeObjectPool<AudioPlayer>.Instance.Recycle(this))
            {
                if (mAudioSourceController != null && !mAudioSourceController.AudioSourceIsNull())
                {
                    mAudioSourceController.Recycle2Cache();
                    mAudioSourceController = null;
                }
            }
        }

        void IAudioKitOnRelease.OnRelease(Action onRelease)
        {
            if (mOnRelease == null)
            {
                mOnRelease = onRelease;
            }
            else
            {
                mOnRelease += onRelease;
            }
        }
    }
}