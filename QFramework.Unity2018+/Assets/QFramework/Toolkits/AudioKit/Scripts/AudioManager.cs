/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 ~ 2025 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using UnityEngine;

namespace QFramework
{
    
    [MonoSingletonPath("QFramework/AudioKit/AudioManager")]
    public class AudioManager : MonoBehaviour, ISingleton,IController
    {
        public AudioPlayer MusicPlayer { get; private set; }

        public AudioPlayer VoicePlayer { get; private set; }

        public void OnSingletonInit()
        {
            SafeObjectPool<AudioPlayer>.Instance.Init(10, 1);
            MusicPlayer = AudioPlayer.Allocate(AudioKit.Settings.MusicVolume);
            MusicPlayer.UsedCache = false;
            MusicPlayer.IsLoop = true;
            VoicePlayer = AudioPlayer.Allocate(AudioKit.Settings.VoiceVolume);
            VoicePlayer.UsedCache = false;

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
                    AudioKitArchitecture.ForEachAllSound(player => player.Stop());
                }
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
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

        public static AudioManager Instance => MonoSingletonProperty<AudioManager>.Instance;

        #endregion



        public IArchitecture GetArchitecture()
        {
            return AudioKitArchitecture.Interface;
        }
    }
}