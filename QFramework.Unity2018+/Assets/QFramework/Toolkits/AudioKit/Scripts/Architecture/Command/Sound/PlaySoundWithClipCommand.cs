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
    internal static class PlaySoundWithClipCommand
    {
        internal static AudioPlayer Execute(AudioClip clip, bool loop = false, Action<AudioPlayer> callBack = null,
            float volume = 1.0f, float pitch = 1,AudioKit.PlaySoundModes? playSoundModes = null)
        {
            AudioManager.Instance.CheckAudioListener();

            var soundName = clip.name;
            if (soundName.IsTrimNotNullAndEmpty())
            {
                soundName = "AudioClip:" + clip.GetHashCode();
            }

            var soundPlayer = AudioPlayer.Allocate(AudioKit.Settings.SoundVolume);
            soundPlayer.SetPlaySoundMode(playSoundModes);
            soundPlayer.OnFinish(() => callBack?.Invoke(soundPlayer));
            
            soundPlayer.VolumeScale(volume)
                .PrepareByClipAndPlay(AudioManager.Instance.gameObject, clip, soundName, loop)
                .Pitch(pitch);
            
            return soundPlayer;
        }
    }
}