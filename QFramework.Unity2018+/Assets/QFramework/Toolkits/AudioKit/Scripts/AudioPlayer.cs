/****************************************************************************
 * Copyright (c) 2016 ~ 2024 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace QFramework
{
    public class AudioPlayer : IPoolable, IPoolType,IAudioKitOnFinish
    {
        private IAudioLoader mLoader;
        private AudioSource mAudioSource;
        private string mName;
        public BindableProperty<float> Volume { get; set; }

        public string GetName => mName;

        private AudioClip mAudioClip;
        private TimeItem mTimeItem;

        private Action mOnStart = null;
        private Action mOnFinish = null;

        private bool mIsPause = false;
        private float mLeftDelayTime = -1;
        private float mVolumeScale = 1.0f;
        private float mVolume;

        public AudioSource AudioSource => mAudioSource;
        
        internal static AudioPlayer Allocate(BindableProperty<float> volume)
        {
            var player =  SafeObjectPool<AudioPlayer>.Instance.Allocate();
            player.Volume = volume;
            player.OnAllocate();
            return player;
        }

        public void OnAllocate()
        {
            mOnStart = null;
            mOnFinish = null;
            mVolume = 1.0f;
            mVolumeScale = 1.0f;
            Volume.RegisterWithInitValue(SetVolume);
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

        public void SetAudioExt(GameObject root, AudioClip clip, string name, bool loop)
        {
            if (clip == null || mName == name)
            {
                return;
            }

            if (mAudioSource == null)
            {
                mAudioSource = root.AddComponent<AudioSource>();
            }

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

            if (mAudioSource == null)
            {
                mAudioSource = root.AddComponent<AudioSource>();
            }

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

            mLeftDelayTime = -1;
            //暂停
            if (mTimeItem != null)
            {
                mLeftDelayTime = mTimeItem.SortScore - Timer.Instance.CurrentScaleTime;
                mTimeItem.Cancel();
                mTimeItem = null;
            }

            mIsPause = true;

            mAudioSource.Pause();
        }

        public void Resume()
        {
            if (!mIsPause)
            {
                return;
            }

            if (mLeftDelayTime >= 0)
            {
                mTimeItem = Timer.Instance.Post2Scale(OnResumeTimeTick, mLeftDelayTime);
            }

            mIsPause = false;

            mAudioSource.Play();
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
            if (mAudioSource)
            {
                mAudioSource.volume = mVolume * mVolumeScale;
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
            if (mAudioSource == null || mAudioClip == null)
            {
                Release();
                return;
            }

            mAudioSource.clip = mAudioClip;
            mAudioSource.loop = IsLoop;
            UpdateVolume();

            int loopCount = 1;
            if (IsLoop)
            {
                loopCount = -1;
            }

            mTimeItem = Timer.Instance.Post2Scale(OnSoundPlayFinish, mAudioClip.length, loopCount);

            mOnStart?.Invoke();
            mOnStart = null;

            mAudioSource.Play();
        }

        private void OnResumeTimeTick(int repeatCount)
        {
            OnSoundPlayFinish(repeatCount);

            if (IsLoop)
            {
                mTimeItem = Timer.Instance.Post2Scale(OnSoundPlayFinish, mAudioClip.length, -1);
            }
        }

        private void OnSoundPlayFinish(int count)
        {
            mOnFinish?.Invoke();
            mOnFinish = null;

            if (!IsLoop)
            {
                Release();
            }
        }

        private void Release()
        {
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

            mLeftDelayTime = -1;
            
            if (mTimeItem != null)
            {
                mTimeItem.Cancel();
                mTimeItem = null;
            }

            if (mAudioSource)
            {
                if (mAudioSource.clip == mAudioClip)
                {
                    mAudioSource.Stop();
                    mAudioSource.clip = null;
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
                if (mAudioSource != null)
                {
                    Object.Destroy(mAudioSource);
                    mAudioSource = null;
                }
            }
        }
    }
}