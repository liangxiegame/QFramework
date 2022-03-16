using System;
using UnityEngine;

namespace QFramework
{

    public class AudioKitWithResKitInit : MonoBehaviour
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
            
            Lazy<ResLoader> mResLoader = new Lazy<ResLoader>(()=>ResLoader.Allocate());

            public AudioClip Clip => mClip;
            private AudioClip mClip;
            
            public AudioClip LoadClip(AudioSearchKeys audioSearchKeys)
            {
                 mClip = mResLoader.Value.LoadSync<AudioClip>(audioSearchKeys.AssetName);

                 return mClip;
            }

            public void LoadClipAsync(AudioSearchKeys audioSearchKeys, Action<bool, AudioClip> onLoad)
            {
                mResLoader.Value.Add2Load<AudioClip>(audioSearchKeys.AssetName,(b, res) =>
                {
                    mClip = res.Asset as AudioClip;
                    onLoad(b,res.Asset as AudioClip);
                });

                mResLoader.Value.LoadAsync();
            }

            public void Unload()
            {
                mClip = null;
                mResLoader.Value.Recycle2Cache();
            }
        }
        protected override IAudioLoader CreateLoader()
        {
            return new ResKitAudioLoader();
        }
    }
}
