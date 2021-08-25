/****************************************************************************
* Copyright (c) 2017 ~ 2021.1 liangxie
*
* http://qframework.io
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
            var audioMgr = AudioManager.Instance;
            audioMgr.CurrentMusicName = musicName;

            if (!Settings.IsMusicOn.Value && allowMusicOff)
            {
                onBeganCallback.InvokeGracefully();
                onEndCallback.InvokeGracefully();
                return;
            }

            Log.I(">>>>>> Start Play Music");

            // TODO: 需要按照这个顺序去 之后查一下原因
            // 需要先注册事件，然后再play
            MusicPlayer.SetOnStartListener(musicUnit =>
            {
                onBeganCallback.InvokeGracefully();

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
                onEndCallback.InvokeGracefully();

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

            audioMgr.CurrentVoiceName = voiceName;

            if (!Settings.IsVoiceOn.Value)
            {
                return;
            }


            VoicePlayer.SetOnStartListener(player =>
            {
                onBeganCallback.InvokeGracefully();
                
                player.SetVolume(Settings.VoiceVolume.Value);
                
                // 调用完就置为null，否则应用层每注册一个而没有注销，都会调用
                VoicePlayer.SetOnStartListener(null);
            });

            VoicePlayer.SetAudio(AudioManager.Instance.gameObject, voiceName, loop);

            VoicePlayer.SetOnFinishListener(musicUnit =>
            {
                onEndedCallback.InvokeGracefully();

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
            var audioMgr = AudioManager.Instance;
            audioMgr.CurrentMusicName = "music" + clip.GetHashCode();

            if (!Settings.IsMusicOn.Value && allowMusicOff)
            {
                onBeganCallback.InvokeGracefully();
                onEndCallback.InvokeGracefully();
                return;
            }

            Log.I(">>>>>> Start Play Music");

            // TODO: 需要按照这个顺序去 之后查一下原因
            // 需要先注册事件，然后再play
            MusicPlayer.SetOnStartListener(musicUnit =>
            {
                onBeganCallback.InvokeGracefully();

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
                onEndCallback.InvokeGracefully();

                // 调用完就置为null，否则应用层每注册一个而没有注销，都会调用
                MusicPlayer.SetOnFinishListener(null);
            });
        }


        public static void PlayVoice(AudioClip clip, bool loop = false, System.Action onBeganCallback = null,
            System.Action onEndedCallback = null)
        {
            var audioMgr = AudioManager.Instance;

            audioMgr.CurrentVoiceName = "voice" + clip.GetHashCode();

            if (!Settings.IsVoiceOn.Value)
            {
                return;
            }


            VoicePlayer.SetOnStartListener(musicUnit =>
            {
                onBeganCallback.InvokeGracefully();

                VoicePlayer.SetOnStartListener(null);
            });

            VoicePlayer.SetAudioExt(AudioManager.Instance.gameObject, clip, audioMgr.CurrentVoiceName, loop);

            VoicePlayer.SetOnFinishListener(musicUnit =>
            {
                onEndedCallback.InvokeGracefully();

                VoicePlayer.SetOnFinishListener(null);
            });
        }

        public static AudioPlayer PlaySound(AudioClip clip, bool loop = false, Action<AudioPlayer> callBack = null,
            int customEventId = -1)
        {
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
}