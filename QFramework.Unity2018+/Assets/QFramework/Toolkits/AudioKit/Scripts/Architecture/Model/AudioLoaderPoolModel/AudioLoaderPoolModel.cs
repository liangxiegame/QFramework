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
    public class AudioLoaderPoolModel : AbstractModel
    {
        private IAudioLoaderPool mAudioLoaderPool = new DefaultAudioLoaderPool();

        public IAudioLoaderPool AudioLoaderPool
        {
            get => mAudioLoaderPool;
            set
            {
                LogKit.I("RegisterAudioLoaderPool:" + value.GetType().Name);
                mAudioLoaderPool = value;
            }
        }

        protected override void OnInit()
        {
            LogKit.I("CurrentAudioLoaderPool:" + mAudioLoaderPool.GetType().Name);
        }
    }
}