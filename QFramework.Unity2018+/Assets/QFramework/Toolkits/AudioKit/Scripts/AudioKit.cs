/****************************************************************************
 * Copyright (c) 2016 ~ 2024 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
#if UNITY_EDITOR
    [ClassAPI("09.AudioKit", "AudioKit", 0, "AudioKit")]
    [APIDescriptionCN("音频管理方案")]
    [APIDescriptionEN("Audio Managements Solution")]
#endif
    public class AudioKit
    {
        public static AudioPlayer MusicPlayer => AudioManager.Instance.MusicPlayer;


        public static AudioKitConfig Config = new AudioKitConfig();

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
            Action onEndCallback = null, float volume = 1f)
        {
            AudioManager.Instance.CheckAudioListener();
            var audioMgr = AudioManager.Instance;
            audioMgr.CurrentMusicName = musicName;

            Debug.Log(">>>>>> Start Play Music");

            MusicPlayer.VolumeScale(volume)
                .OnStart(onBeganCallback)
                .SetAudio(audioMgr.gameObject, musicName, loop)
                .OnFinish(onEndCallback);
        }

#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("停止背景音乐播放")]
        [APIDescriptionEN("Stop Background Music")]
        [APIExampleCode(@"
AudioKit.StopMusic();
")]
#endif
        public static void StopMusic()
        {
            AudioManager.Instance.MusicPlayer.Stop();
        }


#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("暂停背景音乐播放")]
        [APIDescriptionEN("Pause Background Music")]
        [APIExampleCode(@"
AudioKit.PauseMusic();
")]
#endif
        public static void PauseMusic()
        {
            AudioManager.Instance.MusicPlayer.Pause();
        }


#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("继续背景音乐播放")]
        [APIDescriptionEN("Resume Background Music")]
        [APIExampleCode(@"
AudioKit.ResumeMusic();
")]
#endif
        public static void ResumeMusic()
        {
            AudioManager.Instance.MusicPlayer.Resume();
        }

        public static AudioPlayer VoicePlayer => AudioManager.Instance.VoicePlayer;


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
            Action onEndedCallback = null)
        {
            var audioMgr = AudioManager.Instance;
            AudioManager.Instance.CheckAudioListener();
            audioMgr.CurrentVoiceName = voiceName;

            if (!Settings.IsVoiceOn.Value)
            {
                return;
            }
            
            VoicePlayer.OnStart(onBeganCallback);
            VoicePlayer.SetAudio(AudioManager.Instance.gameObject, voiceName, loop);
            VoicePlayer.OnFinish(onEndedCallback);
        }

#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("暂停人声")]
        [APIDescriptionEN("Pause Voice")]
        [APIExampleCode(@"
AudioKit.PauseVoice();
")]
#endif
        public static void PauseVoice()
        {
            VoicePlayer.Pause();
        }

#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("继续人声")]
        [APIDescriptionEN("Resume Voice")]
        [APIExampleCode(@"
AudioKit.ResumeVoice();
")]
#endif
        public static void ResumeVoice()
        {
            VoicePlayer.Resume();
        }

#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("停止人声")]
        [APIDescriptionEN("Stop Voice")]
        [APIExampleCode(@"
AudioKit.StopVoice();
")]
#endif
        public static void StopVoice()
        {
            VoicePlayer.Stop();
        }

        public static PlaySoundModes PlaySoundMode = PlaySoundModes.EveryOne;
        public static int SoundFrameCountForIgnoreSameSound = 10;
        public static int GlobalFrameCountForIgnoreSameSound = 10;

        private static Dictionary<string, int> mSoundFrameCountForName = new Dictionary<string, int>();
        private static int mGlobalFrameCount;

        public enum PlaySoundModes
        {
            EveryOne,
            IgnoreSameSoundInGlobalFrames,
            IgnoreSameSoundInSoundFrames
        }

        static bool CanPlaySound(string soundName)
        {
            if (PlaySoundMode == PlaySoundModes.EveryOne)
            {
                return true;
            }

            if (PlaySoundMode == PlaySoundModes.IgnoreSameSoundInGlobalFrames)
            {
                if (Time.frameCount - mGlobalFrameCount <= GlobalFrameCountForIgnoreSameSound)
                {
                    if (mSoundFrameCountForName.ContainsKey(soundName))
                    {
                        return false;
                    }

                    mSoundFrameCountForName.Add(soundName, 0);
                }
                else
                {
                    mGlobalFrameCount = Time.frameCount;
                    mSoundFrameCountForName.Clear();
                    mSoundFrameCountForName.Add(soundName, 0);
                }

                return true;
            }

            if (PlaySoundMode == PlaySoundModes.IgnoreSameSoundInSoundFrames)
            {
                if (mSoundFrameCountForName.TryGetValue(soundName, out var frames))
                {
                    if (Time.frameCount - frames <= SoundFrameCountForIgnoreSameSound)
                    {
                        return false;
                    }

                    mSoundFrameCountForName[soundName] = Time.frameCount;
                }
                else
                {
                    mSoundFrameCountForName.Add(soundName, Time.frameCount);
                }
            }

            return true;
        }

        static void SoundFinish(string soundName)
        {
            if (PlaySoundMode == PlaySoundModes.IgnoreSameSoundInSoundFrames)
            {
                mSoundFrameCountForName.Remove(soundName);
            }
        }


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
            float volume = 1.0f,float pitch = 1)
        {
            AudioManager.Instance.CheckAudioListener();
            if (!Settings.IsSoundOn.Value) return null;
            if (!CanPlaySound(soundName)) return null;
            var soundPlayer = AudioPlayer.Allocate(Settings.SoundVolume);
            
            soundPlayer.VolumeScale(volume)
                .SetAudio(AudioManager.Instance.gameObject, soundName, loop);
            
            soundPlayer.Pitch(pitch);
            
            soundPlayer.OnFinish(() =>
            {
                callBack?.Invoke(soundPlayer);
                SoundFinish(soundName);
            });
            
            soundPlayer.OnRelease(() =>
            {
                AudioKitArchitecture.RemoveSoundPlayerFromPool(soundPlayer);
            });

            AudioKitArchitecture.AddSoundPlayer2Pool(soundPlayer);
            return soundPlayer;
        }

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
            AudioKitArchitecture.ForEachAllSound(player => player.Stop());
            AudioKitArchitecture.ClearAllPlayingSound();
        }


        #region 梅川内酷需求

        public static void PlayMusic(AudioClip clip, bool loop = true, Action onBeganCallback = null,
            Action onEndCallback = null, float volume = 1f)
        {
            AudioManager.Instance.CheckAudioListener();
            var audioMgr = AudioManager.Instance;
            audioMgr.CurrentMusicName = "music" + clip.GetHashCode();

            Debug.Log(">>>>>> Start Play Music");
            MusicPlayer.VolumeScale(volume)
                .OnStart(onBeganCallback);
            MusicPlayer.SetAudioExt(audioMgr.gameObject, clip, audioMgr.CurrentMusicName, loop);
            MusicPlayer.OnFinish(onEndCallback);
        }


        public static void PlayVoice(AudioClip clip, bool loop = false, Action onBeganCallback = null,
            Action onEndedCallback = null, float volumeScale = 1.0f)
        {
            AudioManager.Instance.CheckAudioListener();
            var audioMgr = AudioManager.Instance;

            audioMgr.CurrentVoiceName = "voice" + clip.GetHashCode();

            if (!Settings.IsVoiceOn.Value)
            {
                return;
            }

            VoicePlayer.VolumeScale(volumeScale)
                .OnStart(onBeganCallback);
            VoicePlayer.SetAudioExt(AudioManager.Instance.gameObject, clip, audioMgr.CurrentVoiceName, loop);
            VoicePlayer.OnFinish(onEndedCallback);
        }

        public static AudioPlayer PlaySound(AudioClip clip, bool loop = false, Action<AudioPlayer> callBack = null,
            float volume = 1.0f,float pitch = 1)
        {
            AudioManager.Instance.CheckAudioListener();
            if (!Settings.IsSoundOn.Value) return null;
            if (!CanPlaySound(clip.name)) return null;

            var soundPlayer = AudioPlayer.Allocate(Settings.SoundVolume);
            soundPlayer.VolumeScale(volume)
                .SetAudioExt(AudioManager.Instance.gameObject, clip, "sound" + clip.GetHashCode(), loop);

            soundPlayer.Pitch(pitch);
            
            soundPlayer.OnFinish(() =>
            {
                callBack?.Invoke(soundPlayer);
            });
            
            soundPlayer.OnRelease(() =>
            {
                AudioKitArchitecture.RemoveSoundPlayerFromPool(soundPlayer);
            });

            AudioKitArchitecture.AddSoundPlayer2Pool(soundPlayer);
            return soundPlayer;
        }

        #endregion

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
        public static AudioKitSettings Settings { get; } = new AudioKitSettings();

        #region Fluent API

        public static FluentMusicAPI Music() => FluentMusicAPI.Allocate();

        public static FluentSoundAPI Sound() => FluentSoundAPI.Allocate();

        #endregion
    }

    public class AudioKitConfig
    {
        public IAudioLoaderPool AudioLoaderPool = new DefaultAudioLoaderPool();
    }

    public interface IAudioLoader
    {
        AudioClip Clip { get; }
        AudioClip LoadClip(AudioSearchKeys audioSearchKeys);

        void LoadClipAsync(AudioSearchKeys audioSearchKeys, Action<bool, AudioClip> onLoad);
        void Unload();
    }

    public class AudioSearchKeys : IPoolType, IPoolable
    {
        public string AssetBundleName;

        public string AssetName;


        public void OnRecycled()
        {
            AssetBundleName = null;
            AssetName = null;
        }

        public bool IsRecycled { get; set; }


        public override string ToString()
        {
            return
                $"AudioSearchKeys AssetName:{AssetName} AssetBundleName:{AssetBundleName}";
        }

        public static AudioSearchKeys Allocate()
        {
            return SafeObjectPool<AudioSearchKeys>.Instance.Allocate();
        }

        public void Recycle2Cache()
        {
            SafeObjectPool<AudioSearchKeys>.Instance.Recycle(this);
        }
    }

    public interface IAudioLoaderPool
    {
        IAudioLoader AllocateLoader();
        void RecycleLoader(IAudioLoader loader);
    }

    public abstract class AbstractAudioLoaderPool : IAudioLoaderPool
    {
        private Stack<IAudioLoader> mPool = new Stack<IAudioLoader>(16);

        public IAudioLoader AllocateLoader()
        {
            return mPool.Count > 0 ? mPool.Pop() : CreateLoader();
        }

        protected abstract IAudioLoader CreateLoader();

        public void RecycleLoader(IAudioLoader loader)
        {
            mPool.Push(loader);
        }
    }

    public class DefaultAudioLoaderPool : AbstractAudioLoaderPool
    {
        protected override IAudioLoader CreateLoader()
        {
            return new DefaultAudioLoader();
        }
    }

    public class DefaultAudioLoader : IAudioLoader
    {
        private AudioClip mClip;

        public AudioClip Clip => mClip;

        public AudioClip LoadClip(AudioSearchKeys panelSearchKeys)
        {
            mClip = Resources.Load<AudioClip>(panelSearchKeys.AssetName);
            return mClip;
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
            Resources.UnloadAsset(mClip);
        }
    }
}