/****************************************************************************
* Copyright (c) 2017 ~ 2021.1 liangxie
*
* https://qframework.cn
* https://github.com/liangxiegame/QFramework
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
****************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public class AudioKit
    {
        /// <summary>
        /// 音频相关的设置
        /// </summary>
        public static readonly AudioKitSettings Settings = new AudioKitSettings();

        public static AudioPlayer MusicPlayer
        {
            get { return AudioManager.Instance.MusicPlayer; }
        }

        public static AudioKitConfig Config = new AudioKitConfig();

        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="musicName"></param>
        /// <param name="onBeganCallback"></param>
        /// <param name="onEndCallback"></param>
        /// <param name="loop"></param>
        /// <param name="allowMusicOff"></param>
        /// <param name="volume"></param>
        public static void PlayMusic(string musicName, bool loop = true, System.Action onBeganCallback = null,
            System.Action onEndCallback = null, bool allowMusicOff = true, float volume = -1f)
        {
            AudioManager.Instance.CheckAudioListener();
            var audioMgr = AudioManager.Instance;
            audioMgr.CurrentMusicName = musicName;

            if (!Settings.IsMusicOn.Value && allowMusicOff)
            {
                onBeganCallback?.Invoke();
                onEndCallback?.Invoke();
                return;
            }

            Debug.Log(">>>>>> Start Play Music");

            // TODO: 需要按照这个顺序去 之后查一下原因
            // 需要先注册事件，然后再play
            MusicPlayer.SetOnStartListener(musicUnit =>
            {
                onBeganCallback?.Invoke();

                if (volume < 0)
                {
                    MusicPlayer.SetVolume(Settings.MusicVolume.Value);
                }
                else
                {
                    MusicPlayer.SetVolume(volume);
                }

                // 调用完就置为null，否则应用层每注册一个而没有注销，都会调用
                MusicPlayer.SetOnStartListener(null);
            });

            MusicPlayer.SetAudio(audioMgr.gameObject, musicName, loop);

            MusicPlayer.SetOnFinishListener(player =>
            {
                onEndCallback?.Invoke();

                // 调用完就置为null，否则应用层每注册一个而没有注销，都会调用
                player.SetOnFinishListener(null);
            });
        }

        public static void StopMusic()
        {
            AudioManager.Instance.MusicPlayer.Stop();
        }

        public static void PauseMusic()
        {
            AudioManager.Instance.MusicPlayer.Pause();
        }

        public static void ResumeMusic()
        {
            AudioManager.Instance.MusicPlayer.Resume();
        }

        public static AudioPlayer VoicePlayer
        {
            get { return AudioManager.Instance.VoicePlayer; }
        }

        public static void PlayVoice(string voiceName, bool loop = false, System.Action onBeganCallback = null,
            System.Action onEndedCallback = null)
        {
            var audioMgr = AudioManager.Instance;
            AudioManager.Instance.CheckAudioListener();
            audioMgr.CurrentVoiceName = voiceName;

            if (!Settings.IsVoiceOn.Value)
            {
                return;
            }


            VoicePlayer.SetOnStartListener(player =>
            {
                onBeganCallback?.Invoke();
                
                player.SetVolume(Settings.VoiceVolume.Value);
                
                // 调用完就置为null，否则应用层每注册一个而没有注销，都会调用
                VoicePlayer.SetOnStartListener(null);
            });

            VoicePlayer.SetAudio(AudioManager.Instance.gameObject, voiceName, loop);

            VoicePlayer.SetOnFinishListener(musicUnit =>
            {
                onEndedCallback?.Invoke();

                VoicePlayer.SetOnFinishListener(null);
            });
        }

        public static void PauseVoice()
        {
            VoicePlayer.Pause();
        }

        public static void ResumeVoice()
        {
            VoicePlayer.Resume();
        }

        public static void StopVoice()
        {
            VoicePlayer.Stop();
        }
        
        public static AudioPlayer PlaySound(string soundName, bool loop = false, Action<AudioPlayer> callBack = null,
            int customEventId = -1)
        {
            AudioManager.Instance.CheckAudioListener();
            if (!Settings.IsSoundOn.Value) return null;

            var soundPlayer = SafeObjectPool<AudioPlayer>.Instance.Allocate();

            soundPlayer.SetOnStartListener(player =>
            {
                player.SetVolume(Settings.SoundVolume.Value);
                soundPlayer.SetOnStartListener(null);
            });
            soundPlayer.SetAudio(AudioManager.Instance.gameObject, soundName, loop);
            soundPlayer.SetOnFinishListener(soundUnit =>
            {
                if (callBack != null)
                {
                    callBack(soundUnit);
                }

                AudioManager.Instance.RemoveSoundPlayerFromPool(soundPlayer);
            });

            soundPlayer.customEventID = customEventId;

            AudioManager.Instance.AddSoundPlayer2Pool(soundPlayer);
            return soundPlayer;
        }

        public static void StopAllSound()
        {
            AudioManager.Instance.ForEachAllSound(player => player.Stop());

            AudioManager.Instance.ClearAllPlayingSound();
        }


        #region 梅川内酷需求
        public static void PlayMusic(AudioClip clip, bool loop = true, System.Action onBeganCallback = null,
            System.Action onEndCallback = null, bool allowMusicOff = true, float volume = -1f)
        {
            AudioManager.Instance.CheckAudioListener();
            var audioMgr = AudioManager.Instance;
            audioMgr.CurrentMusicName = "music" + clip.GetHashCode();

            if (!Settings.IsMusicOn.Value && allowMusicOff)
            {
                onBeganCallback?.Invoke();
                onEndCallback?.Invoke();
                return;
            }

            Debug.Log(">>>>>> Start Play Music");

            // TODO: 需要按照这个顺序去 之后查一下原因
            // 需要先注册事件，然后再play
            MusicPlayer.SetOnStartListener(musicUnit =>
            {
                onBeganCallback?.Invoke();

                if (volume < 0)
                {
                    MusicPlayer.SetVolume(Settings.MusicVolume.Value);
                }
                else
                {
                    MusicPlayer.SetVolume(volume);
                }

                // 调用完就置为null，否则应用层每注册一个而没有注销，都会调用
                MusicPlayer.SetOnStartListener(null);
            });

            MusicPlayer.SetAudioExt(audioMgr.gameObject, clip, audioMgr.CurrentMusicName, loop);

            MusicPlayer.SetOnFinishListener(musicUnit =>
            {
                onEndCallback?.Invoke();

                // 调用完就置为null，否则应用层每注册一个而没有注销，都会调用
                MusicPlayer.SetOnFinishListener(null);
            });
        }


        public static void PlayVoice(AudioClip clip, bool loop = false, System.Action onBeganCallback = null,
            System.Action onEndedCallback = null)
        {
            AudioManager.Instance.CheckAudioListener();
            var audioMgr = AudioManager.Instance;

            audioMgr.CurrentVoiceName = "voice" + clip.GetHashCode();

            if (!Settings.IsVoiceOn.Value)
            {
                return;
            }


            VoicePlayer.SetOnStartListener(musicUnit =>
            {
                onBeganCallback?.Invoke();

                VoicePlayer.SetOnStartListener(null);
            });

            VoicePlayer.SetAudioExt(AudioManager.Instance.gameObject, clip, audioMgr.CurrentVoiceName, loop);

            VoicePlayer.SetOnFinishListener(musicUnit =>
            {
                onEndedCallback?.Invoke();

                VoicePlayer.SetOnFinishListener(null);
            });
        }

        public static AudioPlayer PlaySound(AudioClip clip, bool loop = false, Action<AudioPlayer> callBack = null,
            int customEventId = -1)
        {
            AudioManager.Instance.CheckAudioListener();
            if (!Settings.IsSoundOn.Value) return null;

            var soundPlayer = SafeObjectPool<AudioPlayer>.Instance.Allocate();

            soundPlayer.SetAudioExt(AudioManager.Instance.gameObject, clip,"sound" + clip.GetHashCode(), loop);
            soundPlayer.SetVolume(Settings.SoundVolume.Value);
            soundPlayer.SetOnFinishListener(soundUnit =>
            {
                if (callBack != null)
                {
                    callBack(soundUnit);
                }

                AudioManager.Instance.RemoveSoundPlayerFromPool(soundPlayer);
            });

            soundPlayer.customEventID = customEventId;

            AudioManager.Instance.AddSoundPlayer2Pool(soundPlayer);
            return soundPlayer;
        }
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

        void LoadClipAsync(AudioSearchKeys audioSearchKeys, Action<bool,AudioClip> onLoad);
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
}