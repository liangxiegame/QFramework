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
    internal interface IClipPrepareMode
    {
        void PrepareClip(AbstractAudioPlayer audioPlayer, GameObject root, string name, bool loop);

        void UnPrepareClip();
    }
}