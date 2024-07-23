/****************************************************************************
* Copyright (c) 2017 snowcold
* Copyright (c) 2017 ~ 2020.5 liangxie
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
using System.Linq;

namespace QFramework
{
    using System.Collections.Generic;
    using UnityEngine;
    

    /// <summary>
    /// TODO:目前,不支持本地化
    /// </summary>
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

            AudioKit.Settings.MusicVolume.Bind(volume => { MusicPlayer.SetVolume(volume); })
                .DisposeWhenGameObjectDestroyed(this);

            AudioKit.Settings.VoiceVolume.Bind(volume => { VoicePlayer.SetVolume(volume); })
                .DisposeWhenGameObjectDestroyed(this);

            AudioKit.Settings.IsMusicOn.Bind(musicOn =>
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
            }).DisposeWhenGameObjectDestroyed(this);

            AudioKit.Settings.IsVoiceOn.Bind(voiceOn =>
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
            }).DisposeWhenGameObjectDestroyed(this);

            AudioKit.Settings.IsSoundOn.Bind(soundOn =>
            {
                if (soundOn)
                {
                }
                else
                {
                    ForEachAllSound(player => player.Stop());
                }
            }).DisposeWhenGameObjectDestroyed(this);


            AudioKit.Settings.SoundVolume.Bind(soundVolume =>
            {
                ForEachAllSound(player => player.SetVolume(soundVolume));
            }).DisposeWhenGameObjectDestroyed(this);
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
                mSoundPlayerInPlaying.Add(audioPlayer.Name, new List<AudioPlayer> {audioPlayer});
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