/****************************************************************************
* Copyright (c) 2017 snowcold
* Copyright (c) 2017 ~ 2018.12 liangxie
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
using QF.Action;
using QF.Extensions;
using QFramework;

namespace QF.Res
{
    using UnityEngine.Events;
    using System.Collections.Generic;
    using UnityEngine;
    using UniRx;

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

    #region 消息体定义

    public class AudioMsgWithBool : QMsg
    {
        public bool on;

        public AudioMsgWithBool(ushort eventId, bool on) : base(eventId)
        {
            this.on = on;
        }
    }

    public class AduioMsgPlayVoiceLoop : QMsg
    {
        public string      VoiceName;
        public UnityAction OnVoiceBeganCallback;
        public UnityAction OnVoiceEndedCallback;
    }

    public class AudioMsgWithNode : QMsg
    {
        public IAction Node;

        public AudioMsgWithNode(IAction node) : base((int) AudioEvent.PlayNode)
        {
            Node = node;
        }
    }

    #endregion

    /// <summary>
    /// TODO:目前,不支持本地化
    /// </summary>
    [QMonoSingletonPath("[Audio]/AudioManager")]
    public class AudioManager : QMgrBehaviour, ISingleton
    {
        #region Audio设置数据

// 用来存储的Key
        const string KEY_AUDIO_MANAGER_SOUND_ON = "KEY_AUDIO_MANAGER_SOUND_ON";

        const string KEY_AUDIO_MANAGER_MUSIC_ON     = "KEY_AUDIO_MANAGER_MUSIC_ON";
        const string KEY_AUDIO_MANAGER_VOICE_ON     = "KEY_AUDIO_MANAGER_VOICE_ON";
        const string KEY_AUDIO_MANAGER_VOICE_VOLUME = "KEY_AUDIO_MANAGER_VOICE_VOLUME";
        const string KEY_AUDIO_MANAGER_SOUND_VOLUME = "KEY_AUDIO_MANAGER_SOUND_VOLUME";
        const string KEY_AUDIO_MANAGER_MUSIC_VOLUME = "KEY_AUDIO_MANAGER_MUSIC_VOLUME";

        /// <summary>
        /// 读取音频数据
        /// </summary>
        void ReadAudioSetting()
        {
            SoundOn.Value = PlayerPrefs.GetInt(KEY_AUDIO_MANAGER_SOUND_ON, 1) == 1 ? true : false;
            IsMusicOn = PlayerPrefs.GetInt(KEY_AUDIO_MANAGER_MUSIC_ON, 1) == 1 ? true : false;
            IsVoiceOn = PlayerPrefs.GetInt(KEY_AUDIO_MANAGER_VOICE_ON, 1) == 1 ? true : false;

            SoundVolume = PlayerPrefs.GetFloat(KEY_AUDIO_MANAGER_SOUND_VOLUME, 1.0f);
            MusicVolume = PlayerPrefs.GetFloat(KEY_AUDIO_MANAGER_MUSIC_VOLUME, 1.0f);
            VoiceVolume = PlayerPrefs.GetFloat(KEY_AUDIO_MANAGER_VOICE_VOLUME, 1.0f);
        }

        /// <summary>
        /// 保存音频数据
        /// </summary>
        void SaveAudioSetting()
        {
            PlayerPrefs.SetInt(KEY_AUDIO_MANAGER_SOUND_ON, SoundOn.Value == true ? 1 : 0);
            PlayerPrefs.SetInt(KEY_AUDIO_MANAGER_MUSIC_ON, IsMusicOn == true ? 1 : 0);
            PlayerPrefs.SetInt(KEY_AUDIO_MANAGER_VOICE_ON, IsVoiceOn == true ? 1 : 0);
            PlayerPrefs.SetFloat(KEY_AUDIO_MANAGER_SOUND_VOLUME, SoundVolume);
            PlayerPrefs.SetFloat(KEY_AUDIO_MANAGER_MUSIC_VOLUME, MusicVolume);
            PlayerPrefs.SetFloat(KEY_AUDIO_MANAGER_VOICE_VOLUME, VoiceVolume);
        }

        void OnApplicationQuit()
        {
            SaveAudioSetting();
        }

        void OnApplicationFocus(bool focus)
        {
            SaveAudioSetting();
        }

        void OnApplicationPause(bool pause)
        {
            SaveAudioSetting();
        }

        #endregion

        #region 消息处理

        protected AudioUnit mMainUnit;
        protected AudioUnit mVoiceUnit;

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

            SafeObjectPool<AudioUnit>.Instance.Init(10, 1);
            mMainUnit = AudioUnit.Allocate();
            mMainUnit.usedCache = false;
            mVoiceUnit = AudioUnit.Allocate();
            mVoiceUnit.usedCache = false;

            CheckAudioListener();

            gameObject.transform.position = Vector3.zero;

// 读取存储
            ReadAudioSetting();
        }

        public void Dispose()
        {
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
                    AudioMsgWithBool soundSwitchMsg = msg as AudioMsgWithBool;
                    SoundOn.Value = soundSwitchMsg.on;
                    break;
                case (int) AudioEvent.MusicSwitch:
                    AudioMsgWithBool musicSwitchMsg = msg as AudioMsgWithBool;
                    IsMusicOn = musicSwitchMsg.on;
                    if (!IsMusicOn)
                    {
                        StopMusic();
                    }

                    break;
                case (int) AudioEvent.PlayMusic:
                    Debug.LogFormat("play music msg: {0}, is musicOn: ", AudioEvent.PlayMusic.ToString(), MusicOn);
                    PlayMusic(msg as AudioMusicMsg);
                    break;
                case (int) AudioEvent.StopMusic:
                    StopMusic();
                    break;
                case (int) AudioEvent.PlaySound:
                    AudioSoundMsg audioSoundMsg = msg as AudioSoundMsg;
                    PlaySound(audioSoundMsg);
                    break;

                case (int) AudioEvent.PlayVoice:
                    PlayVoice(msg as AudioVoiceMsg);
                    break;
                case (int) AudioEvent.StopVoice:
                    StopVoice();
                    break;
                case (int) AudioEvent.PlayNode:
                    IAction msgPlayNode = (msg as AudioMsgWithNode).Node;
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
                    PauseMusic();
                    break;
                case (int) AudioEvent.ResumeMusic:
                    ResumeMusic();
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

        public static bool IsOn
        {
            get { return IsSoundOn && IsMusicOn && IsVoiceOn; }
        }

        public static void On()
        {
            SetSoundOn();
            SetMusicOn();
            SetVoiceOn();
        }

        public static void Off()
        {
            SetSoundOff();
            SetMusicOff();
            SetVoiceOff();
        }

        public static void SetSoundOn()
        {
            IsSoundOn = true;
        }

        public static void SetSoundOff()
        {
            IsSoundOn = false;
        }

        public static void SetVoiceOn()
        {
            IsVoiceOn = true;
        }

        public static void SetVoiceOff()
        {
            IsVoiceOn = false;
        }

        private string mCurMusicName;

        public static void SetMusicOn()
        {
            IsMusicOn = true;

            var self = Instance;

            if (self.mCurMusicName.IsNotNullAndEmpty())
            {
                self.SendMsg(new AudioMusicMsg(self.mCurMusicName, true));
            }
        }

        public static void SetMusicOff()
        {
            IsMusicOn = false;
            StopMusic();
        }

        public static bool IsSoundOn { get; private set; }

        public static bool IsMusicOn { get; private set; }

        public static bool IsVoiceOn { get; private set; }


        public BoolReactiveProperty SoundOn = new BoolReactiveProperty();

        [Obsolete("please use IsMusicOn")]
        public bool MusicOn
        {
            get { return IsMusicOn; }
            private set { IsMusicOn = value; }
        }

        [Obsolete("please use IsVoiceOn")]
        public bool VoiceOn
        {
            get { return IsVoiceOn; }
            private set { IsVoiceOn = value; }
        }

        public float SoundVolume { get; private set; }
        public float MusicVolume { get; private set; }
        public float VoiceVolume { get; private set; }

        #endregion

        #region 内部实现

        int mCurSourceIndex;


        /// <summary>
        /// 播放音乐
        /// </summary>
        void PlayMusic(AudioMusicMsg musicMsg)
        {

            if (!MusicOn && musicMsg.allowMusicOff)
            {
                musicMsg.onMusicBeganCallback.InvokeGracefully();

                musicMsg.onMusicEndedCallback.InvokeGracefully();
                return;
            }

            Log.I(">>>>>> Start Play Music");

// TODO: 需要按照这个顺序去 之后查一下原因
//需要先注册事件，然后再play
            mMainUnit.SetOnStartListener(delegate(AudioUnit musicUnit)
            {
                musicMsg.onMusicBeganCallback.InvokeGracefully();

//调用完就置为null，否则应用层每注册一个而没有注销，都会调用
                mMainUnit.SetOnStartListener(null);
            });

            mMainUnit.SetAudio(gameObject, musicMsg.MusicName, musicMsg.Loop);

            mMainUnit.SetOnFinishListener(delegate(AudioUnit musicUnit)
            {
                musicMsg.onMusicEndedCallback.InvokeGracefully();

//调用完就置为null，否则应用层每注册一个而没有注销，都会调用
                mMainUnit.SetOnFinishListener(null);
            });
        }

        public static void PlayMusic(string musicName, bool loop = true, System.Action onBeganCallback = null,
            System.Action onEndCallback = null, bool allowMusicOff = true,float volume = 1.0f)
        {
            var self = Instance;
            self.mCurMusicName = musicName;

            if (!IsMusicOn && allowMusicOff)
            {
                onBeganCallback.InvokeGracefully();
                onEndCallback.InvokeGracefully();
                return;
            }

            Log.I(">>>>>> Start Play Music");

// TODO: 需要按照这个顺序去 之后查一下原因
//需要先注册事件，然后再play
            self.mMainUnit.SetOnStartListener(musicUnit =>
            {
                onBeganCallback.InvokeGracefully();

                self.mMainUnit.SetVolume(volume);
//调用完就置为null，否则应用层每注册一个而没有注销，都会调用
                self.mMainUnit.SetOnStartListener(null);
            });

            self.mMainUnit.SetAudio(self.gameObject, musicName, loop);

            self.mMainUnit.SetOnFinishListener(musicUnit =>
            {
                onEndCallback.InvokeGracefully();

//调用完就置为null，否则应用层每注册一个而没有注销，都会调用
                self.mMainUnit.SetOnFinishListener(null);
            });
        }

        private void SetVolume(AudioUnit audioUnit, VolumeLevel volumeLevel)
        {
            switch (volumeLevel)
            {
                case VolumeLevel.Max:
                    audioUnit.SetVolume(1.0f);
                    break;
                case VolumeLevel.Normal:
                    audioUnit.SetVolume(0.5f);
                    break;
                case VolumeLevel.Min:
                    audioUnit.SetVolume(0.2f);
                    break;
            }
        }

        public static AudioUnit PlaySound(string soundName, bool loop = false, Action<AudioUnit> callBack = null,
            int customEventID = -1)
        {
            if (soundName.IsNullOrEmpty())
            {
                return null;
            }

            var unit = SafeObjectPool<AudioUnit>.Instance.Allocate();
            unit.SetAudio(Instance.gameObject, soundName, loop);
            unit.SetOnFinishListener(callBack);
            unit.customEventID = customEventID;

            return unit;
        }

        /// <summary>
        /// 停止播放音乐
        /// </summary>
        public static void StopMusic()
        {
            Instance.mMainUnit.Stop();
        }
        
        public static void StopVoice()
        {
            if (Instance.mVoiceUnit.IsNotNull())
            {
                Instance.mVoiceUnit.Stop();
            }
        }

        public static void PauseMusic()
        {
            if (Instance.mMainUnit != null)
            {
                Instance.mMainUnit.Pause();
            }
        }

        public static void ResumeMusic()
        {
            if (Instance.mMainUnit != null)
            {
                Instance.mMainUnit.Resume();
            }
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        void PlaySound(AudioSoundMsg soundMsg)
        {
            if (SoundOn.Value)
            {
                AudioUnit unit = SafeObjectPool<AudioUnit>.Instance.Allocate();


                unit.SetOnStartListener(delegate(AudioUnit soundUnit)
                {
                    soundMsg.onSoundBeganCallback.InvokeGracefully();

                    unit.SetVolume(soundMsg.Volume);

                    unit.SetOnStartListener(null);
                });

                unit.SetAudio(gameObject, soundMsg.SoundName, false);

                unit.SetOnFinishListener(delegate(AudioUnit soundUnit)
                {
                    soundMsg.onSoundEndedCallback.InvokeGracefully();

                    unit.SetOnFinishListener(null);
                });
            }
        }

        /// <summary>
        /// 播放语音
        /// </summary>
        void PlayVoice(AudioVoiceMsg msg)
        {
            mVoiceUnit.SetOnStartListener(delegate(AudioUnit musicUnit)
            {
//                SetVolume(mVoiceUnit, VolumeLevel.Min);

                msg.onVoiceBeganCallback.InvokeGracefully();

                mVoiceUnit.SetOnStartListener(null);
            });

            mVoiceUnit.SetAudio(gameObject, msg.voiceName, msg.loop);

            mVoiceUnit.SetOnFinishListener(delegate(AudioUnit musicUnit)
            {
//                SetVolume(mVoiceUnit, VolumeLevel.Max);

                msg.onVoiceEndedCallback.InvokeGracefully();

                mVoiceUnit.SetOnFinishListener(null);
            });
        }

        public static void PlayVoice(string soundName, bool loop = false)
        {
            if (soundName.IsNullOrEmpty())
            {
                return;
            }

            var unit = SafeObjectPool<AudioUnit>.Instance.Allocate();
            unit.SetAudio(Instance.gameObject, soundName, loop);
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

        protected List<string> mRetainAudioNames;

        /// <summary>
        /// 添加常驻音频资源，建议尽早添加，不要在用的时候再添加
        /// </summary>
        private void AddRetainAudio(string audioName)
        {
            if (mRetainResLoader == null)
                mRetainResLoader = ResLoader.Allocate();
            if (mRetainAudioNames == null)
                mRetainAudioNames = new List<string>();

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


        #region 留给脚本绑定的 API

        public static void PlayMusic(string musicName)
        {
            PlayMusic(musicName, true);
        }
        

        #endregion
    }
}