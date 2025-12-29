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
    public static class PlayVoiceWithClip
    {
        public static void Execute(AudioClip clip, bool loop = false, Action onBeganCallback = null,
            Action onEndedCallback = null, float volumeScale = 1.0f)
        {
            AudioManager.Instance.CheckAudioListener();
            var audioMgr = AudioManager.Instance;

            audioMgr.CurrentVoiceName = "voice" + clip.GetHashCode();

            if (!AudioKit.Settings.IsVoiceOn.Value)
            {
                return;
            }

            AudioKit.VoicePlayer.VolumeScale(volumeScale)
                .OnStart(onBeganCallback);
            AudioKit.VoicePlayer.PrepareByClipAndPlay(AudioManager.Instance.gameObject, clip, audioMgr.CurrentVoiceName, loop);
            AudioKit.VoicePlayer.OnFinish(onEndedCallback);
        }
    }
}