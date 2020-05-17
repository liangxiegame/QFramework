/****************************************************************************
* Copyright (c) 2017 snowcold
* Copyright (c) 2017 ~ 2020.5 liangxie
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
using System.Linq;

namespace QFramework
{
    using System.Collections.Generic;
    using UnityEngine;

    #region 消息id定义

    public enum AudioEvent
    {
        Began = QMgrID.Audio,
        SoundSwitch,
        MusicSwitch,
        VoiceSwitch,
        SetSoundVolume,
        SetMusicVolume,
        SetVoiceVolume,
        PlayMusic,
        PlaySound,
        PlayVoice,
        PauseMusic,
        ResumeMusic,
        StopMusic,
        PauseVoice,
        StopVoice,
        StopAllSound,
        PlayNode,
        AddRetainAudio,
        RemoveRetainAudioAudio,
        Ended
    }

    #endregion

    /// <summary>
    /// TODO:目前,不支持本地化
    /// </summary>
    [MonoSingletonPath("[Audio]/AudioManager")]
    public partial class AudioManager : QMgrBehaviour, ISingleton
    {
        #region 消息处理

        public AudioPlayer MusicPlayer { get; private set; }

        public AudioPlayer VoicePlayer { get; private set; }

        public void OnSingletonInit()
        {
            Log.I("AudioManager OnSingletonInit");
            RegisterEvents(
                AudioEvent.SoundSwitch,
                AudioEvent.MusicSwitch,
                AudioEvent.VoiceSwitch,
                AudioEvent.SetSoundVolume,
                AudioEvent.SetMusicVolume,
                AudioEvent.SetVoiceVolume,
                AudioEvent.PlayMusic,
                AudioEvent.PlaySound,
                AudioEvent.PlayVoice,
                AudioEvent.PlayNode,
                AudioEvent.AddRetainAudio,
                AudioEvent.RemoveRetainAudioAudio
            );

            SafeObjectPool<AudioPlayer>.Instance.Init(10, 1);
            MusicPlayer = AudioPlayer.Allocate();
            MusicPlayer.usedCache = false;
            VoicePlayer = AudioPlayer.Allocate();
            VoicePlayer.usedCache = false;

            CheckAudioListener();

            gameObject.transform.position = Vector3.zero;

            AudioKit.Settings.MusicVolume.Bind(volume => { MusicPlayer.SetVolume(volume); }).AddTo(this);

            AudioKit.Settings.VoiceVolume.Bind(volume => { VoicePlayer.SetVolume(volume); }).AddTo(this);

            AudioKit.Settings.IsMusicOn.Bind(musicOn =>
            {
                if (musicOn)
                {
                    if (CurrentMusicName.IsNotNullAndEmpty())
                    {
                        AudioKit.PlayMusic(CurrentMusicName);
                    }
                }
                else
                {
                    MusicPlayer.Stop();
                }
            }).AddTo(this);

            AudioKit.Settings.IsVoiceOn.Bind(voiceOn =>
            {
                if (voiceOn)
                {
                    if (CurrentVoiceName.IsNotNullAndEmpty())
                    {
                        AudioKit.PlayVoice(CurrentVoiceName);
                    }
                }
                else
                {
                    VoicePlayer.Stop();
                }
            }).AddTo(this);

            AudioKit.Settings.IsSoundOn.Bind(soundOn =>
            {
                if (soundOn)
                {

                }
                else
                {
                    ForEachAllSound(player => player.Stop());
                }
            }).AddTo(this);


            AudioKit.Settings.SoundVolume.Bind(soundVolume =>
            {
                ForEachAllSound(player => player.SetVolume(soundVolume));
            }).AddTo(this);
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
                mSoundPlayerInPlaying.Add(audioPlayer.Name,new List<AudioPlayer> {audioPlayer});
            }
        }

        public void RemoveSoundPlayerFromPool(AudioPlayer audioPlayer)
        {
            mSoundPlayerInPlaying[audioPlayer.Name].Remove(audioPlayer);
        }

        public override int ManagerId
        {
            get { return QMgrID.Audio; }
        }

        protected override void ProcessMsg(int key, QMsg msg)
        {
            switch (msg.EventID)
            {
                case (int) AudioEvent.SoundSwitch:
                    var soundSwitchMsg = msg as AudioMsgWithBool;
                    AudioKit.Settings.IsSoundOn.Value = soundSwitchMsg.on;
                    break;
                case (int) AudioEvent.MusicSwitch:
                    var musicSwitchMsg = msg as AudioMsgWithBool;
                    AudioKit.Settings.IsMusicOn.Value = musicSwitchMsg.on;
                    if (!AudioKit.Settings.IsMusicOn.Value)
                    {
                        AudioKit.StopMusic();
                    }

                    break;
                case (int) AudioEvent.PlayMusic:
                    Log.I("play music msg: {0}, is musicOn: {1}", AudioEvent.PlayMusic.ToString(),
                        AudioKit.Settings.IsMusicOn.Value);
                    var musicMsg = msg as AudioMusicMsg;
                    AudioKit.PlayMusic(musicMsg.MusicName, musicMsg.Loop, musicMsg.onMusicBeganCallback,
                        musicMsg.onMusicEndedCallback, musicMsg.allowMusicOff);
                    break;
                case (int) AudioEvent.StopMusic:
                    AudioKit.StopMusic();
                    break;
                case (int) AudioEvent.PlaySound:
                    AudioSoundMsg audioSoundMsg = msg as AudioSoundMsg;
                    PlaySound(audioSoundMsg);
                    break;

                case (int) AudioEvent.PlayVoice:
                    var voiceMsg = msg as AudioVoiceMsg;
                    AudioKit.PlayVoice(voiceMsg.voiceName, voiceMsg.loop, voiceMsg.onVoiceBeganCallback,
                        voiceMsg.onVoiceEndedCallback);
                    break;
                case (int) AudioEvent.StopVoice:
                    AudioKit.StopVoice();
                    break;
                case (int) AudioEvent.PlayNode:
                    var msgPlayNode = (msg as AudioMsgWithNode).Node;
                    StartCoroutine(msgPlayNode.Execute());
                    break;
                case (int) AudioEvent.AddRetainAudio:
                    AddRetainAudioMsg addRetainAudioMsg = msg as AddRetainAudioMsg;
                    AddRetainAudio(addRetainAudioMsg.AudioName);
                    break;
                case (int) AudioEvent.RemoveRetainAudioAudio:
                    RemoveRetainAudioMsg removeRetainAudioMsg = msg as RemoveRetainAudioMsg;
                    RemoveRetainAudio(removeRetainAudioMsg.AudioName);
                    break;
                case (int) AudioEvent.PauseMusic:
                    AudioKit.PauseMusic();
                    break;
                case (int) AudioEvent.ResumeMusic:
                    AudioKit.ResumeMusic();
                    break;
            }
        }

        #endregion


        #region 对外接口

        public override void Init()
        {
            Log.I("AudioManager.Init");
        }

        public void CheckAudioListener()
        {
            // 确保有一个AudioListener
            if (FindObjectOfType<AudioListener>() == null)
            {
                gameObject.AddComponent<AudioListener>();
            }
        }

        public string CurrentMusicName { get; set; }

        public string CurrentVoiceName { get; set; }

        #endregion

        #region 内部实现

        int mCurSourceIndex;



        /// <summary>
        /// 播放音效
        /// </summary>
        void PlaySound(AudioSoundMsg soundMsg)
        {
            if (AudioKit.Settings.IsSoundOn.Value)
            {
                var unit = SafeObjectPool<AudioPlayer>.Instance.Allocate();

                unit.SetOnStartListener(soundUnit =>
                {
                    soundMsg.onSoundBeganCallback.InvokeGracefully();

                    unit.SetVolume(soundMsg.Volume);

                    soundUnit.SetOnStartListener(null);
                });

                unit.SetAudio(gameObject, soundMsg.SoundName, false);

                unit.SetOnFinishListener(soundUnit =>
                {
                    soundMsg.onSoundEndedCallback.InvokeGracefully();

                    soundUnit.SetOnFinishListener(null);
                });
            }
        }


        public static void PlayVoiceOnce(string voiceName)
        {
            if (voiceName.IsNullOrEmpty())
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

        void Example()
        {
            // 按钮点击音效
            SendMsg(new AudioSoundMsg("Sound.CLICK"));

            //播放背景音乐
            SendMsg(new AudioMusicMsg("music", true));

            //停止播放音乐
            SendMsg(new QMsg((ushort) AudioEvent.StopMusic));

            SendMsg(new AudioVoiceMsg("Sound.CLICK", delegate { }, delegate { }));
        }

        #endregion

        //常驻内存不卸载音频资源
        protected ResLoader mRetainResLoader;

        protected HashSet<string> mRetainAudioNames;

        /// <summary>
        /// 添加常驻音频资源，建议尽早添加，不要在用的时候再添加
        /// </summary>
        private void AddRetainAudio(string audioName)
        {
            if (mRetainResLoader == null)
                mRetainResLoader = ResLoader.Allocate();
            if (mRetainAudioNames == null)
                mRetainAudioNames = new HashSet<string>();

            if (!mRetainAudioNames.Contains(audioName))
            {
                mRetainAudioNames.Add(audioName);
                mRetainResLoader.Add2Load(audioName);
                mRetainResLoader.LoadAsync();
            }
        }

        /// <summary>
        /// 删除常驻音频资源
        /// </summary>
        private void RemoveRetainAudio(string audioName)
        {
            if (mRetainAudioNames != null && mRetainAudioNames.Remove(audioName))
            {
                mRetainResLoader.ReleaseRes(audioName);
            }
        }

        #endregion

        public void ClearAllPlayingSound()
        {
            mSoundPlayerInPlaying.Clear();
        }
    }
}