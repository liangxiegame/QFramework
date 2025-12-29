/****************************************************************************
 * Copyright (c) 2015 - 2025 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 * AudioKit v1.0: use QFramework.cs architecture
 ****************************************************************************/

using System;
using UnityEngine;

namespace QFramework
{
    public class DefaultAudioLoader : IAudioLoader
    {
        public AudioClip Clip { get; private set; }

        public AudioClip LoadClip(AudioSearchKeys panelSearchKeys)
        {
            Clip = Resources.Load<AudioClip>(panelSearchKeys.AssetName);
            return Clip;
        }

        public void LoadClipAsync(AudioSearchKeys audioSearchKeys, Action<bool, AudioClip> onLoad)
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
            Resources.UnloadAsset(Clip);
            Clip = null;
        }
    }
}