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
    internal static class PlayMusicCommand
    {
        internal static void Execute(string musicName, bool loop = true, Action onBeganCallback = null,
            Action onEndCallback = null, float volume = 1f)
        {
            AudioManager.Instance.CheckAudioListener();
            var audioMgr = AudioManager.Instance;
            audioMgr.CurrentMusicName = musicName;
            
            AudioKit.MusicPlayer.VolumeScale(volume)
                .OnStart(onBeganCallback)
                .PrepareByNameAsyncAndPlay(audioMgr.gameObject, musicName, loop)
                .OnFinish(onEndCallback);
        }
    }
}