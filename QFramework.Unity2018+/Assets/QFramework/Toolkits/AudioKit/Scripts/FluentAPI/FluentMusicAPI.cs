/****************************************************************************
 * Copyright (c) 2015 - 2025 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 * AudioKit v1.0: use QFramework.cs architecture
 ****************************************************************************/

using UnityEngine;

namespace QFramework
{
    public class FluentMusicAPI : IPoolable, IPoolType
    {
        enum PrepareModes
        {
            ByName,
            ByClip
        }
        public bool IsRecycled { get; set; }
        

        private string mName = null;
        private AudioClip mClip = null;
        private bool mLoop = true;
        private float mVolumeScale = 1;
        private PrepareModes mPrepareMode = PrepareModes.ByName;

        public FluentMusicAPI WithName(string name)
        {
            mName = name;
            mPrepareMode = PrepareModes.ByName;
            return this;
        }

        public FluentMusicAPI WithAudioClip(AudioClip clip)
        {
            mClip = clip;
            mPrepareMode = PrepareModes.ByClip;
            return this;
        }

        public FluentMusicAPI Loop(bool loop)
        {
            mLoop = loop;
            return this;
        }

        public FluentMusicAPI VolumeScale(float volumeScale)
        {
            mVolumeScale = volumeScale;
            return this;
        }
        
        public void Play()
        {
            if (mPrepareMode == PrepareModes.ByName)
            {
                AudioKit.PlayMusic(mName, mLoop, onEndCallback: Recycle2Cache, volume: mVolumeScale);
            }
            else
            {
                AudioKit.PlayMusic(mClip, mLoop, onEndCallback: Recycle2Cache, volume: mVolumeScale);
            }
        }
        
        public static FluentMusicAPI Allocate() => SafeObjectPool<FluentMusicAPI>.Instance.Allocate();

        public void OnRecycled()
        {
            mName = null;
            mClip = null;
            mLoop = true;
            mVolumeScale = 1;
        }


        public void Recycle2Cache() => SafeObjectPool<FluentMusicAPI>.Instance.Recycle(this);

    }
}