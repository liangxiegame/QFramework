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
    internal class ClipPrepareModeController
    {
        private IClipPrepareMode mPrepareMode;
        internal IClipPrepareMode PrepareMode => mPrepareMode ?? (mPrepareMode = ByLoaderAsync);
        internal readonly PrepareClipBySetUp BySetUp = new PrepareClipBySetUp();
        internal readonly PrepareClipByLoaderAsync ByLoaderAsync = new PrepareClipByLoaderAsync();
        internal readonly PrepareClipByLoaderSync ByLoaderSync = new PrepareClipByLoaderSync();

        internal void ChangePrepareMode(IClipPrepareMode mode)
        {
            if (PrepareMode == mode)
            {
                return;
            }

            PrepareMode?.UnPrepareClip();
            mPrepareMode = mode;
        }
    }
}