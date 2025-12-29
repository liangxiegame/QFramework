/****************************************************************************
 * Copyright (c) 2015 - 2025 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 * AudioKit v1.0: use QFramework.cs architecture
 ****************************************************************************/

using System;

namespace QFramework
{
    internal static class PlaySoundCommand
    {
        internal static AudioPlayer Execute(string soundName, bool loop = false, Action<AudioPlayer> callBack = null,
            float volume = 1.0f, float pitch = 1, AudioKit.PlaySoundModes? playSoundModes = null)
        {
            AudioManager.Instance.CheckAudioListener();
            var soundPlayer = AudioPlayer.Allocate(AudioKit.Settings.SoundVolume);
            
            soundPlayer
                .SetPlaySoundMode(playSoundModes)
                .VolumeScale(volume)
                .PrepareByNameAsyncAndPlay(AudioManager.Instance.gameObject, soundName, loop)
                .Pitch(pitch);
            soundPlayer.OnFinish(() => callBack?.Invoke(soundPlayer));

            return soundPlayer;
        }
    }
}