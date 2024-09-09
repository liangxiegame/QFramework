using UnityEngine;

namespace QFramework
{
    public class FluentMusicAPI : IPoolable, IPoolType
    {
        public static FluentMusicAPI Allocate()
        {
            return SafeObjectPool<FluentMusicAPI>.Instance.Allocate();
        }

        public void OnRecycled()
        {
            mName = null;
            mClip = null;
            mLoop = true;
            mVolumeScale = 1;
        }

        public bool IsRecycled { get; set; }

        public void Recycle2Cache()
        {
            SafeObjectPool<FluentMusicAPI>.Instance.Recycle(this);
        }


        private string mName = null;
        private AudioClip mClip = null;
        private bool mLoop = true;
        private float mVolumeScale = 1;

        public FluentMusicAPI WithName(string name)
        {
            mName = name;
            return this;
        }

        public FluentMusicAPI WithAudioClip(AudioClip clip)
        {
            mClip = clip;
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
            if (mName != null)
            {
                AudioKit.PlayMusic(mName, mLoop, onEndCallback: Recycle2Cache, volume: mVolumeScale);
            }
            else
            {
                AudioKit.PlayMusic(mClip, mLoop, onEndCallback: Recycle2Cache, volume: mVolumeScale);
            }
        }
    }
}