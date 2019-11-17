/****************************************************************************
 * Copyright (c) 2017 liangxie
****************************************************************************/

using QFramework;

namespace QF.Res
{
    using System;
    using UnityEngine;
        
    public class AudioUnit : IPoolable, IPoolType
    {
        private ResLoader mLoader;
        private AudioSource mSource;
        private string mName;

        private bool mIsLoop;
        private AudioClip mAudioClip;
        private TimeItem mTimeItem;
        private bool mUsedCache = true;
        private bool mIsCache = false;

        private Action<AudioUnit> mOnStartListener;
        private Action<AudioUnit> mOnFinishListener;
        private bool mIsPause = false;
        private float mLeftDelayTime = -1;
        private int mPlayCount = 0;
        private int mCustomEventID;

        public int customEventID
        {
            get { return mCustomEventID; }
            set { mCustomEventID = -1; }
        }

        public static AudioUnit Allocate()
        {
            return SafeObjectPool<AudioUnit>.Instance.Allocate();
        }

        public void SetOnStartListener(Action<AudioUnit> l)
        {
            mOnStartListener = l;
        }

        public void SetOnFinishListener(Action<AudioUnit> l)
        {
            mOnFinishListener = l;
        }

        public bool usedCache
        {
            get { return mUsedCache; }
            set { mUsedCache = false; }
        }

        public int playCount
        {
            get { return mPlayCount; }
        }

        public bool IsRecycled
        {
            get { return mIsCache; }

            set { mIsCache = value; }
        }

        public void SetAudio(GameObject root, string name, bool loop)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            if (mName == name)
            {
                return;
            }

            if (mSource == null)
            {
                mSource = root.AddComponent<AudioSource>();
            }

            //防止卸载后立马加载的情况
            ResLoader preLoader = mLoader;
            mLoader = null;
            CleanResources();

            mLoader = ResLoader.Allocate();

            mIsLoop = loop;
            mName = name;

            mLoader.Add2Load(name, OnResLoadFinish);

            if (preLoader != null)
            {
                preLoader.Recycle2Cache();
                preLoader = null;
            }

            if (mLoader != null)
            {
                mLoader.LoadAsync();
            }
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
                mLeftDelayTime = mTimeItem.SortScore - QFramework.Timer.Instance.currentScaleTime;
                mTimeItem.Cancel();
                mTimeItem = null;
            }

            mIsPause = true;

            mSource.Pause();
        }

        public void Resume()
        {
            if (!mIsPause)
            {
                return;
            }

            if (mLeftDelayTime >= 0)
            {
                mTimeItem = QFramework.Timer.Instance.Post2Scale(OnResumeTimeTick, mLeftDelayTime);
            }

            mIsPause = false;

            mSource.Play();
        }

        public void SetVolume(float volume)
        {
            if (null != mSource)
            {
                mSource.volume = volume;
            }
        }

        private void OnResLoadFinish(bool result, IRes res)
        {
            if (!result)
            {
                Release();
                return;
            }

            mAudioClip = res.Asset as AudioClip;

            if (mAudioClip == null)
            {
                Log.E("Asset Is Invalid AudioClip:" + mName);
                Release();
                return;
            }

            PlayAudioClip();
        }

        private void PlayAudioClip()
        {
            if (mSource == null || mAudioClip == null)
            {
                Release();
                return;
            }

            mSource.clip = mAudioClip;
            mSource.loop = mIsLoop;
            mSource.volume = 1.0f;

            int loopCount = 1;
            if (mIsLoop)
            {
                loopCount = -1;
            }

            mTimeItem = QFramework.Timer.Instance.Post2Scale(OnSoundPlayFinish, mAudioClip.length, loopCount);


            if (null != mOnStartListener)
            {
                mOnStartListener(this);
            }

            mSource.Play();
        }

        private void OnResumeTimeTick(int repeatCount)
        {
            OnSoundPlayFinish(repeatCount);

            if (mIsLoop)
            {
                mTimeItem = QFramework.Timer.Instance.Post2Scale(OnSoundPlayFinish, mAudioClip.length, -1);
            }
        }

        private void OnSoundPlayFinish(int count)
        {
            ++mPlayCount;

            if (mOnFinishListener != null)
            {
                mOnFinishListener(this);
            }

            if (mCustomEventID > 0)
            {
                QEventSystem.Instance.Send(mCustomEventID, this);
            }

            if (!mIsLoop)
            {
                Release();
            }
        }

        private void Release()
        {
            CleanResources();

            if (mUsedCache)
            {
                Recycle2Cache();
            }
        }

        private void CleanResources()
        {
            mName = null;

            mPlayCount = 0;
            mIsPause = false;
            mOnFinishListener = null;
            mLeftDelayTime = -1;
            mCustomEventID = -1;

            if (mTimeItem != null)
            {
                mTimeItem.Cancel();
                mTimeItem = null;
            }

            if (mSource != null)
            {
                if (mSource.clip == mAudioClip)
                {
                    mSource.Stop();
                    mSource.clip = null;
                }
            }

            mAudioClip = null;

            if (mLoader != null)
            {
                mLoader.Recycle2Cache();
                mLoader = null;
            }
        }

        public void OnRecycled()
        {
            CleanResources();
        }

        public void Recycle2Cache()
        {
            if (!SafeObjectPool<AudioUnit>.Instance.Recycle(this))
            {
                if (mSource != null)
                {
                    GameObject.Destroy(mSource);
                    mSource = null;
                }
            }
        }
    }
}