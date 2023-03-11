using System;
using UnityEngine;

namespace QFramework.Example
{
    public class CustomAudioLoaderExample : MonoBehaviour
    {
        /// <summary>
        /// 定义从 Resources 加载音频
        /// </summary>
        class ResourcesAudioLoaderPool : AbstractAudioLoaderPool
        {
            protected override IAudioLoader CreateLoader()
            {
                return new ResourcesAudioLoader();
            }
        }

        class ResourcesAudioLoader : IAudioLoader
        {
            private AudioClip mClip;
        
            public AudioClip Clip => mClip;

            public AudioClip LoadClip(AudioSearchKeys panelSearchKeys)
            {
                mClip = Resources.Load<AudioClip>(panelSearchKeys.AssetName);
                return mClip;
            }

            public void LoadClipAsync(AudioSearchKeys audioSearchKeys, Action<bool,AudioClip> onLoad)
            {
                var resourceRequest = Resources.LoadAsync<AudioClip>(audioSearchKeys.AssetName);
                resourceRequest.completed += operation =>
                {
                    var clip = resourceRequest.asset as AudioClip;
                    onLoad(clip, clip);
                };
            }

            public void Unload()
            {
                Resources.UnloadAsset(mClip);
            }
        }
        
        
        void Start()
        {
            // 启动时需要调用一次
            AudioKit.Config.AudioLoaderPool = new ResourcesAudioLoaderPool();
        }
    }
}
