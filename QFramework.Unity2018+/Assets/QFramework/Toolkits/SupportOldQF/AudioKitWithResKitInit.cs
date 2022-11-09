using System;
using UnityEngine;

namespace QFramework
{
    public class AudioKitWithResKitInit 
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            AudioKit.Config.AudioLoaderPool = new ResKitAudioLoaderPool();
        }
    }

    public class ResKitAudioLoaderPool : AbstractAudioLoaderPool
    {
        public class ResKitAudioLoader : IAudioLoader
        {
            private ResLoader mResLoader = null;

            public AudioClip Clip => mClip;
            private AudioClip mClip;

            public AudioClip LoadClip(AudioSearchKeys audioSearchKeys)
            {
                if (mResLoader == null)
                {
                    mResLoader = ResLoader.Allocate();
                }

                mClip = mResLoader.LoadSync<AudioClip>(audioSearchKeys.AssetName);

                return mClip;
            }

            public void LoadClipAsync(AudioSearchKeys audioSearchKeys, Action<bool, AudioClip> onLoad)
            {
                if (mResLoader == null)
                {
                    mResLoader = ResLoader.Allocate();
                }

                mResLoader.Add2Load<AudioClip>(audioSearchKeys.AssetName, (b, res) =>
                {
                    mClip = res.Asset as AudioClip;
                    onLoad(b, res.Asset as AudioClip);
                });

                mResLoader.LoadAsync();
            }

            public void Unload()
            {
                mClip = null;
                mResLoader?.Recycle2Cache();
                mResLoader = null;
            }
        }

        protected override IAudioLoader CreateLoader()
        {
            return new ResKitAudioLoader();
        }
    }
}