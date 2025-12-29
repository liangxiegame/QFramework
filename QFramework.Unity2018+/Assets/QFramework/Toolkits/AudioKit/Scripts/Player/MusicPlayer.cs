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
    public class MusicPlayer : AbstractAudioPlayer
    {
        public MusicPlayer(BindableProperty<float> volume,bool isLoop = true)
        {
            OnInit(volume);
            IsLoop = isLoop;
        }

        protected override void OnPlayStarted()
        {
            
        }

        internal override bool CanPlayAudio() => true;

        protected override void OnBeforeStop()
        {
            
        }

        protected override void OnStop()
        {
            
        }

        public void Deinit()
        {
            OnDeinit();
        }
    }
}