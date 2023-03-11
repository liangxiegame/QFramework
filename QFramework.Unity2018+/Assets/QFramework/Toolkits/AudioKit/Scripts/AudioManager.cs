/****************************************************************************
* Copyright (c) 2017 snowcold
* Copyright (c) 2017 ~ 2022 liangxie
*
* https://qframework.cn
* https://github.com/liangxiegame/QFramework
* https://gitee.com/liangxiegame/QFramework
****************************************************************************/

using System;
using System.Linq;

namespace QFramework
{
    using System.Collections.Generic;
    using UnityEngine;
    
    [MonoSingletonPath("[Audio]/AudioManager")]
    public partial class AudioManager : MonoBehaviour, ISingleton
    {

        public AudioPlayer MusicPlayer { get; private set; }

        public AudioPlayer VoicePlayer { get; private set; }

        public void OnSingletonInit()
        {

            SafeObjectPool<AudioPlayer>.Instance.Init(10, 1);
            MusicPlayer = AudioPlayer.Allocate();
            MusicPlayer.usedCache = false;
            VoicePlayer = AudioPlayer.Allocate();
            VoicePlayer.usedCache = false;

            CheckAudioListener();

            gameObject.transform.position = Vector3.zero;

            AudioKit.Settings.MusicVolume.Register(volume => { MusicPlayer.SetVolume(volume); })
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            AudioKit.Settings.VoiceVolume.Register(volume => { VoicePlayer.SetVolume(volume); })
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            AudioKit.Settings.IsMusicOn.Register(musicOn =>
            {
                if (musicOn)
                {
                    if (!string.IsNullOrEmpty(CurrentMusicName))
                    {
                        AudioKit.PlayMusic(CurrentMusicName);
                    }
                }
                else
                {
                    MusicPlayer.Stop();
                }
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            AudioKit.Settings.IsVoiceOn.Register(voiceOn =>
            {
                if (voiceOn)
                {
                    if (!string.IsNullOrEmpty(CurrentVoiceName))
                    {
                        AudioKit.PlayVoice(CurrentVoiceName);
                    }
                }
                else
                {
                    VoicePlayer.Stop();
                }
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            AudioKit.Settings.IsSoundOn.Register(soundOn =>
            {
                if (soundOn)
                {
                }
                else
                {
                    ForEachAllSound(player => player.Stop());
                }
            }).UnRegisterWhenGameObjectDestroyed(gameObject);


            AudioKit.Settings.SoundVolume.Register(soundVolume =>
            {
                ForEachAllSound(player => player.SetVolume(soundVolume));
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private static Dictionary<string, List<AudioPlayer>> mSoundPlayerInPlaying =
            new Dictionary<string, List<AudioPlayer>>(30);


        public void ForEachAllSound(Action<AudioPlayer> operation)
        {
            foreach (var audioPlayer in mSoundPlayerInPlaying.SelectMany(keyValuePair => keyValuePair.Value))
            {
                operation(audioPlayer);
            }
        }

        public void AddSoundPlayer2Pool(AudioPlayer audioPlayer)
        {
            if (mSoundPlayerInPlaying.ContainsKey(audioPlayer.Name))
            {
                mSoundPlayerInPlaying[audioPlayer.Name].Add(audioPlayer);
            }
            else
            {
                mSoundPlayerInPlaying.Add(audioPlayer.Name, new List<AudioPlayer> { audioPlayer });
            }
        }

        public void RemoveSoundPlayerFromPool(AudioPlayer audioPlayer)
        {
            mSoundPlayerInPlaying[audioPlayer.Name].Remove(audioPlayer);
        }
        
        #region 对外接口

        public void Init()
        {
            Debug.Log("AudioManager.Init");
        }

        private AudioListener mAudioListener;

        public void CheckAudioListener()
        {
            // 确保有一个AudioListener
            if (!mAudioListener)
            {
                mAudioListener = FindObjectOfType<AudioListener>();
            }

            if (!mAudioListener)
            {
                mAudioListener = gameObject.AddComponent<AudioListener>();
            }
        }

        public string CurrentMusicName { get; set; }

        public string CurrentVoiceName { get; set; }

        #endregion




        public static void PlayVoiceOnce(string voiceName)
        {

            if (string.IsNullOrEmpty(voiceName))
            {
                return;
            }

            var unit = SafeObjectPool<AudioPlayer>.Instance.Allocate();
            unit.SetAudio(Instance.gameObject, voiceName, false);
        }

        #region 单例实现

        public static AudioManager Instance
        {
            get { return MonoSingletonProperty<AudioManager>.Instance; }
        }


        #endregion

        public void ClearAllPlayingSound()
        {
            mSoundPlayerInPlaying.Clear();
        }
    }
}