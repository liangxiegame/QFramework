/****************************************************************************
 * Copyright (c) 2015 - 2025 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 * AudioKit v1.0: use QFramework.cs architecture
 ****************************************************************************/

using UnityEngine;

namespace QFramework
{
    [MonoSingletonPath("QFramework/AudioKit/AudioManager")]
    internal class AudioManager : MonoBehaviour, ISingleton, IController
    {
        internal MusicPlayer MusicPlayer { get; private set; }

        internal MusicPlayer VoicePlayer { get; private set; }

        public void OnSingletonInit()
        {
            SafeObjectPool<AudioPlayer>.Instance.Init(10, 1);
            MusicPlayer = new MusicPlayer(AudioKit.Settings.MusicVolume);
            VoicePlayer = new MusicPlayer(AudioKit.Settings.VoiceVolume, false);

            CheckAudioListener();

            gameObject.transform.position = Vector3.zero;

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
                    Architecture.PlayingSoundPoolModel.ForEachAllSound(p=>p.Stop());
                }
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }


        #region 对外接口

        internal void Init()
        {
            Debug.Log("AudioManager.Init");
        }

        private AudioListener mAudioListener;

        internal void CheckAudioListener()
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

        internal string CurrentMusicName { get; set; }

        internal string CurrentVoiceName { get; set; }

        #endregion


        internal static void PlayVoiceOnce(string voiceName)
        {
            if (string.IsNullOrEmpty(voiceName))
            {
                return;
            }

            SafeObjectPool<AudioPlayer>.Instance
                .Allocate()
                .PrepareByNameAsyncAndPlay(Instance.gameObject, voiceName, false);
        }

        #region 单例实现

        internal static AudioManager Instance => MonoSingletonProperty<AudioManager>.Instance;

        #endregion

        public IArchitecture GetArchitecture() => Architecture.Interface;
    }
}