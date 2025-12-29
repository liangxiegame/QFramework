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
    internal static class PlayMusicWithClipCommand
    {
        internal static void Execute(AudioClip clip, bool loop = true, Action onBeganCallback = null,
            Action onEndCallback = null, float volume = 1f)
        {
            AudioManager.Instance.CheckAudioListener();
            var audioMgr = AudioManager.Instance;
            audioMgr.CurrentMusicName = "music" + clip.GetHashCode();

            AudioKit.MusicPlayer.VolumeScale(volume)
                .OnStart(onBeganCallback)
                .PrepareByClipAndPlay(audioMgr.gameObject, clip, audioMgr.CurrentMusicName, loop)
                .OnFinish(onEndCallback);
        }
    }
}