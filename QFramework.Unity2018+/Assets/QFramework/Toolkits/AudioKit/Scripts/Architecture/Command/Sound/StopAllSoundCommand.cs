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
    internal static class StopAllSoundCommand
    {
        internal static void Execute()
        {
            Architecture.PlayingSoundPoolModel.ForEachAllSound(player=>player.Stop());
            Architecture.PlayingSoundPoolModel.ClearAllPlayingSound();
        }
    }
}