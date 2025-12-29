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
    public abstract class AbstractAudioPlayer
    {
        public BindableProperty<float> Volume { get; protected set; }

        internal bool IsPause => AudioSourceProxy.Paused;

        public bool IsLoop
        {
            get => AudioSourceProxy.Loop;
            set => AudioSourceProxy.SetLoop(value);
        }
        
        public string AudioName { get; internal set; }

        internal readonly AudioPlayerLifeCycle LifeCycle = new AudioPlayerLifeCycle();
        internal readonly AudioSourceProxy AudioSourceProxy = new AudioSourceProxy();
        internal readonly ClipPrepareModeController PrepareModeController = new ClipPrepareModeController();
        
        protected void OnInit(BindableProperty<float> volume)
        {
            Volume = volume;
            LifeCycle.Clear();
            AudioSourceProxy.InitParameters();
            Volume.RegisterWithInitValue(AudioSourceProxy.SetVolume);
        }

        protected void OnDeinit()
        {
            Volume?.UnRegister(AudioSourceProxy.SetVolume);
            Volume = null;

            ClearDataAndStop();
        }
        
        internal void ClearDataAndStop()
        {
            AudioName = null;
            AudioSourceProxy.StopAndClearClip();
            PrepareModeController.PrepareMode.UnPrepareClip();
        }
        
        internal void OnClipPrepareFinished(bool result, AudioClip clip)
        {
            if (!result)
            {
                Stop();
                return;
            }

            AudioSourceProxy.SetClip(clip);

            if (!clip)
            {
                Debug.LogError("Asset Is Invalid AudioClip:" + AudioName);
                Stop();
                return;
            }

            if (AudioSourceProxy.AudioSourceIsNull() || AudioSourceProxy.AudioClip == null)
            {
                Stop();
                return;
            }

            if (IsPause)
            {
                return;
            }
            
            
            LifeCycle.CallOnStartOnce();

            if (CanPlayAudio())
            {
                mPlayedAudio = true;
                OnPlayStarted();
                AudioSourceProxy.Play(OnPlayFinished);
            }
            else
            {
                mPlayedAudio = false;
            }
        }

        protected abstract void OnPlayStarted();

        protected bool mPlayedAudio = false;

        internal abstract bool CanPlayAudio();

        public void Stop()
        {
            OnBeforeStop();
            
            ClearDataAndStop();

            OnStop();
        }

        protected abstract void OnBeforeStop();

        protected abstract void OnStop();
        
        internal virtual void OnPlayFinished()
        {
            if (!AudioSourceProxy.Loop)
            {
                LifeCycle.CallOnFinishOnce();

                Stop();
            }
        }
    }
}