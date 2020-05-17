using System;
using UnityEngine;

namespace QFramework
{
    public class AudioPlayer : IPoolable, IPoolType
    {
        private ResLoader mLoader;
        private AudioSource mAudioSource;
        private string mName;

        public string Name
        {
            get { return mName; }
        }

        private bool mIsLoop;
        private AudioClip mAudioClip;
        private TimeItem mTimeItem;
        private bool mUsedCache = true;
        private bool mIsCache = false;

        private Action<AudioPlayer> mOnStartListener;
        private Action<AudioPlayer> mOnFinishListener;
        private bool mIsPause = false;
        private float mLeftDelayTime = -1;
        private int mPlayCount = 0;
        private int mCustomEventID;

        public AudioSource AudioSource
        {
            get { return mAudioSource; }
        }

        public int customEventID
        {
            get { return mCustomEventID; }
            set { mCustomEventID = -1; }
        }

        public static AudioPlayer Allocate()
        {
            return SafeObjectPool<AudioPlayer>.Instance.Allocate();
        }

        public void SetOnStartListener(Action<AudioPlayer> l)
        {
            mOnStartListener = l;
        }

        public void SetOnFinishListener(Action<AudioPlayer> l)
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

            if (mAudioSource == null)
            {
                mAudioSource = root.AddComponent<AudioSource>();
            }

            //防止卸载后立马加载的情况
            var preLoader = mLoader;

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
                mTimeItem = QFramework.Timer.Instance.Post2Scale(OnResumeTimeTick, mLeftDelayTime);
            }

            mIsPause = false;

            mAudioSource.Play();
        }

        public void SetVolume(float volume)
        {
            if (null != mAudioSource)
            {
                mAudioSource.volume = volume;
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
            if (mAudioSource == null || mAudioClip == null)
            {
                Release();
                return;
            }

            mAudioSource.clip = mAudioClip;
            mAudioSource.loop = mIsLoop;
            mAudioSource.volume = 1.0f;

            int loopCount = 1;
            if (mIsLoop)
            {
                loopCount = -1;
            }

            mTimeItem = Timer.Instance.Post2Scale(OnSoundPlayFinish, mAudioClip.length, loopCount);

            if (null != mOnStartListener)
            {
                mOnStartListener(this);
            }

            mAudioSource.Play();
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

            if (mAudioSource != null)
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
            if (!SafeObjectPool<AudioPlayer>.Instance.Recycle(this))
            {
                if (mAudioSource != null)
                {
                    GameObject.Destroy(mAudioSource);
                    mAudioSource = null;
                }
            }
        }
    }
}