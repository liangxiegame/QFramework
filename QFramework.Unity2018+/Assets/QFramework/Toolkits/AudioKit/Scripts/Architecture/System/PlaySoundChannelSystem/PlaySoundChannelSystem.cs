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
    internal class PlaySoundChannelSystem : AbstractSystem
    {
        internal AudioKit.PlaySoundModes DefaultPlaySoundMode = AudioKit.PlaySoundModes.EveryOne;

        internal readonly EveryOneChannel EveryOneChannel = new EveryOneChannel();

        internal readonly IgnoreSameSoundInSoundFramesChannel IgnoreInSoundFramesChannel =
            new IgnoreSameSoundInSoundFramesChannel();

        internal readonly IgnoreSameSoundInGlobalFramesChannel IgnoreInGlobalFramesChannel =
            new IgnoreSameSoundInGlobalFramesChannel();


        private PlaySoundChannel GetChannel(AudioKit.PlaySoundModes mode)
        {
            switch (mode)
            {
                case AudioKit.PlaySoundModes.EveryOne:
                    return EveryOneChannel;
                case AudioKit.PlaySoundModes.IgnoreSameSoundInSoundFrames:
                    return IgnoreInSoundFramesChannel;
                case AudioKit.PlaySoundModes.IgnoreSameSoundInGlobalFrames:
                    return IgnoreInGlobalFramesChannel;
                default:
                    return EveryOneChannel;
            }
        }
        
        internal bool CanPlaySound(AudioPlayer player) => GetChannel(player.PlaySoundMode)
            .CanPlaySound(player.AudioName);

        internal void SoundFinish(AudioPlayer player) => GetChannel(player.PlaySoundMode)
            .SoundFinish(player.AudioName);

        protected override void OnInit()
        {
        }
    }
}