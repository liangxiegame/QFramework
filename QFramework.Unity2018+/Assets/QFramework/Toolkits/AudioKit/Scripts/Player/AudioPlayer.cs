/****************************************************************************
 * Copyright (c) 2015 - 2025 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 * AudioKit v1.0: use QFramework.cs architecture
 ****************************************************************************/

namespace QFramework
{
    /// <summary>
    /// 有对象池的音频管理器
    /// </summary>
    public class AudioPlayer : AbstractAudioPlayer, IPoolable, IPoolType
    {
        internal AudioKit.PlaySoundModes PlaySoundMode { get; private set; } = AudioKit.PlaySoundModes.EveryOne;

        internal AudioPlayer SetPlaySoundMode(AudioKit.PlaySoundModes? playSoundMode)
        {
            if (playSoundMode != null)
            {
                PlaySoundMode = playSoundMode.Value;
            }

            return this;
        }

        internal static AudioPlayer Allocate(BindableProperty<float> volume)
        {
            var player = SafeObjectPool<AudioPlayer>.Instance.Allocate();
            player.PlaySoundMode = AudioKit.PlaySoundMode;
            player.OnInit(volume);
            return player;
        }

        public bool IsRecycled { get; set; } = false;

        public void OnRecycled()
        {
            OnDeinit();
        }

        public void Recycle2Cache()
        {
            if (!SafeObjectPool<AudioPlayer>.Instance.Recycle(this))
            {
                AudioSourceProxy.OnParentRecycled();
            }
        }

        string mNameForPool = null;

        protected override void OnPlayStarted()
        {
            if (mPlayedAudio)
            {
                mNameForPool = AudioName;
                Architecture.PlayingSoundPoolModel.AddSoundPlayer2Pool(mNameForPool, this);
            }
        }

        internal override bool CanPlayAudio()
        {
            return Architecture.PlaySoundChannelSystem.CanPlaySound(this) && AudioKit.Settings.IsSoundOn.Value;
        }

        protected override void OnBeforeStop()
        {
            if (mPlayedAudio)
            {
                Architecture.PlayingSoundPoolModel.RemoveSoundPlayerFromPool(mNameForPool, this);
            }
        }

        protected override void OnStop()
        {
            Recycle2Cache();
        }

        internal override void OnPlayFinished()
        {
            if (mPlayedAudio && !AudioSourceProxy.Loop)
            {
                Architecture.PlaySoundChannelSystem.SoundFinish(this);
            }

            base.OnPlayFinished();
        }
    }
}