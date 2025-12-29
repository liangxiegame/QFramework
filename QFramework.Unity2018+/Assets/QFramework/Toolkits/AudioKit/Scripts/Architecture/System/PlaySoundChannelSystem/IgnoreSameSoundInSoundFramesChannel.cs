/****************************************************************************
 * Copyright (c) 2015 - 2025 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 * AudioKit v1.0: use QFramework.cs architecture
 ****************************************************************************/

using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    internal class IgnoreSameSoundInSoundFramesChannel : PlaySoundChannel
    {
        private readonly Dictionary<string, int> mSoundFrameCountForName = new Dictionary<string, int>();

        internal int SoundFrameCountForIgnoreSameSound = 10;

        internal override bool CanPlaySound(string soundName)
        {
            if (mSoundFrameCountForName.TryGetValue(soundName, out var frames))
            {
                if (Time.frameCount - frames <= SoundFrameCountForIgnoreSameSound)
                {
                    return false;
                }

                mSoundFrameCountForName[soundName] = Time.frameCount;
            }
            else
            {
                mSoundFrameCountForName.Add(soundName, Time.frameCount);
            }

            return true;
        }

        internal override void SoundFinish(string soundName)
        {
            mSoundFrameCountForName.Remove(soundName);
        }
    }
}