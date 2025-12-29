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
#if UNITY_EDITOR
    [ClassAPI("09.AudioKit", "AudioKit", 0, "AudioKit")]
    [APIDescriptionCN("音频管理方案")]
    [APIDescriptionEN("Audio Managements Solution")]
#endif
    public static class AudioKit
    {
        public static MusicPlayer MusicPlayer => AudioManager.Instance.MusicPlayer;
        public static MusicPlayer VoicePlayer => AudioManager.Instance.VoicePlayer;

        public static AudioLoaderPoolModel Config => Architecture.LoaderPoolModel;


#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("播放背景音乐")]
        [APIDescriptionEN("Play Background Music")]
        [APIExampleCode(@"
AudioKit.PlayMusic(""HomeBg"");


// loop = false
AudioKit.PlayMusic(""HomeBg"",false);

AudioKit.PlayMusic(homeBgClip);

")]
#endif
        public static void PlayMusic(string musicName, bool loop = true, Action onBeganCallback = null,
            Action onEndCallback = null, float volume = 1f) =>
            PlayMusicCommand.Execute(musicName, loop, onBeganCallback, onEndCallback, volume);

#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("停止背景音乐播放")]
        [APIDescriptionEN("Stop Background Music")]
        [APIExampleCode(@"
AudioKit.StopMusic();
")]
#endif
        public static void StopMusic() => StopMusicCommand.Execute();


#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("暂停背景音乐播放")]
        [APIDescriptionEN("Pause Background Music")]
        [APIExampleCode(@"
AudioKit.PauseMusic();
")]
#endif
        public static void PauseMusic() => MusicPlayer.Pause();


#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("继续背景音乐播放")]
        [APIDescriptionEN("Resume Background Music")]
        [APIExampleCode(@"
AudioKit.ResumeMusic();
")]
#endif
        public static void ResumeMusic() => ResumeMusicCommand.Execute();


#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("播放人声")]
        [APIDescriptionEN("Play Voice")]
        [APIExampleCode(@"
AudioKit.PlayVoice(""SentenceA"");
AudioKit.PlayVoice(SentenceAClip);
")]
#endif
        public static void PlayVoice(string voiceName, bool loop = false, Action onBeganCallback = null,
            Action onEndedCallback = null) =>
            PlayVoiceCommand.Execute(voiceName, loop, onBeganCallback, onEndedCallback);

#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("暂停人声")]
        [APIDescriptionEN("Pause Voice")]
        [APIExampleCode(@"
AudioKit.PauseVoice();
")]
#endif
        public static void PauseVoice() => PauseVoiceCommand.Execute();

#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("继续人声")]
        [APIDescriptionEN("Resume Voice")]
        [APIExampleCode(@"
AudioKit.ResumeVoice();
")]
#endif
        public static void ResumeVoice() => ResumeVoiceCommand.Execute();

#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("停止人声")]
        [APIDescriptionEN("Stop Voice")]
        [APIExampleCode(@"
AudioKit.StopVoice();
")]
#endif
        public static void StopVoice() => StopVoiceCommand.Execute();


        #region PlaySoundMode System API

        public enum PlaySoundModes
        {
            EveryOne,
            IgnoreSameSoundInGlobalFrames,
            IgnoreSameSoundInSoundFrames
        }


        public static PlaySoundModes PlaySoundMode
        {
            get => Architecture.PlaySoundChannelSystem.DefaultPlaySoundMode;
            set => Architecture.PlaySoundChannelSystem.DefaultPlaySoundMode = value;
        }

        public static int SoundFrameCountForIgnoreSameSound
        {
            get => Architecture.PlaySoundChannelSystem.IgnoreInSoundFramesChannel
                .SoundFrameCountForIgnoreSameSound;
            set => Architecture.PlaySoundChannelSystem.IgnoreInSoundFramesChannel
                .SoundFrameCountForIgnoreSameSound = value;
        }

        public static int GlobalFrameCountForIgnoreSameSound
        {
            get => Architecture.PlaySoundChannelSystem.IgnoreInGlobalFramesChannel
                .GlobalFrameCountForIgnoreSameSound;
            set => Architecture.PlaySoundChannelSystem.IgnoreInGlobalFramesChannel
                    .GlobalFrameCountForIgnoreSameSound =
                value;
        }

        #endregion


#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("播放声音")]
        [APIDescriptionEN("Play Sound")]
        [APIExampleCode(@"
AudioKit.PlaySound(""EnemyDie"");
AudioKit.PlaySound(EnemyDieClip);
")]
#endif
        public static AudioPlayer PlaySound(string soundName, bool loop = false, Action<AudioPlayer> callBack = null,
            float volume = 1.0f, float pitch = 1,PlaySoundModes? playSoundMode = null) => PlaySoundCommand.Execute(soundName, loop, callBack, volume, pitch,playSoundMode);

        public static AudioPlayer PlaySound(AudioClip clip, bool loop = false, Action<AudioPlayer> callBack = null,
            float volume = 1.0f, float pitch = 1,PlaySoundModes? playSoundMode = null) =>
            PlaySoundWithClipCommand.Execute(clip, loop, callBack, volume,pitch,playSoundMode);
        
#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("停止播放全部声音")]
        [APIDescriptionEN("Stop All Sound")]
        [APIExampleCode(@"
AudioKit.StopAllSound();
")]
#endif
        public static void StopAllSound()
        {
            StopAllSoundCommand.Execute();
        }

        public static void PlayMusic(AudioClip clip, bool loop = true, Action onBeganCallback = null,
            Action onEndCallback = null, float volume = 1f) =>
            PlayMusicWithClipCommand.Execute(clip, loop, onBeganCallback, onEndCallback, volume);


        public static void PlayVoice(AudioClip clip, bool loop = false, Action onBeganCallback = null,
            Action onEndedCallback = null, float volumeScale = 1.0f) =>
            PlayVoiceWithClip.Execute(clip, loop, onBeganCallback, onEndedCallback, volumeScale);




#if UNITY_EDITOR
        [PropertyAPI]
        [APIDescriptionCN("音频相关设置")]
        [APIDescriptionEN("AudioKit Setting")]
        [APIExampleCode(@"

// Switch
// 开关
btnSoundOn.onClick.AddListener(() => { AudioKit.Settings.IsSoundOn.Value = true; });

btnSoundOff.onClick.AddListener(() => { AudioKit.Settings.IsSoundOn.Value = false; });

btnMusicOn.onClick.AddListener(() => { AudioKit.Settings.IsMusicOn.Value = true; });

btnMusicOff.onClick.AddListener(() => { AudioKit.Settings.IsMusicOn.Value = false; });

btnVoiceOn.onClick.AddListener(() => { AudioKit.Settings.IsVoiceOn.Value = true; });

btnVoiceOff.onClick.AddListener(() => { AudioKit.Settings.IsVoiceOn.Value = false; });

// Volume Control
// 音量控制
AudioKit.Settings.MusicVolume.RegisterWithInitValue(v => musicVolumeSlider.value = v);
AudioKit.Settings.VoiceVolume.RegisterWithInitValue(v => voiceVolumeSlider.value = v);
AudioKit.Settings.SoundVolume.RegisterWithInitValue(v => soundVolumeSlider.value = v);
            
// 监听音量变更
musicVolumeSlider.onValueChanged.AddListener(v => { AudioKit.Settings.MusicVolume.Value = v; });
voiceVolumeSlider.onValueChanged.AddListener(v => { AudioKit.Settings.VoiceVolume.Value = v; });
soundVolumeSlider.onValueChanged.AddListener(v => { AudioKit.Settings.SoundVolume.Value = v; });
")]
#endif
        public static AudioKitSettingsModel Settings => Architecture.SettingsModel;

        #region Fluent API

        public static FluentMusicAPI Music() => FluentMusicAPI.Allocate();

        public static FluentSoundAPI Sound() => FluentSoundAPI.Allocate();

        #endregion
    }
}