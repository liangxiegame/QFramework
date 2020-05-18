using System;

namespace QFramework
{
    public class AudioKit
    {
        /// <summary>
        /// 音频相关的设置
        /// </summary>
        public readonly static AudioKitSettings Settings = new AudioKitSettings();

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
        public static void PlayMusic(string musicName, bool loop = true, Action onBeganCallback = null,
            Action onEndCallback = null, bool allowMusicOff = true, float volume = -1f)
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

            MusicPlayer.SetOnFinishListener(musicUnit =>
            {
                onEndCallback.InvokeGracefully();

                // 调用完就置为null，否则应用层每注册一个而没有注销，都会调用
                MusicPlayer.SetOnFinishListener(null);
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

        public static void PlayVoice(string voiceName, bool loop = false, Action onBeganCallback = null,
            Action onEndedCallback = null)
        {
            var audioMgr = AudioManager.Instance;

            audioMgr.CurrentVoiceName = voiceName;

            if (!Settings.IsVoiceOn.Value)
            {
                return;
            }


            VoicePlayer.SetOnStartListener(musicUnit =>
            {
                onBeganCallback.InvokeGracefully();

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

            soundPlayer.SetAudio(AudioManager.Instance.gameObject, soundName, loop);
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

        public static void StopAllSound()
        {
            AudioManager.Instance.ForEachAllSound(player => player.Stop());

            AudioManager.Instance.ClearAllPlayingSound();
        }
    }
}