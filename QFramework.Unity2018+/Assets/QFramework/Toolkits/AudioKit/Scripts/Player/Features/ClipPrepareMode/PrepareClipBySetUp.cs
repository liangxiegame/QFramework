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
    internal class PrepareClipBySetUp : IClipPrepareMode
    {
        private AudioClip mClip;

        internal void SetClipForPrepare(AudioClip clip)
        {
            mClip = clip;
        }
        
        public void PrepareClip(AbstractAudioPlayer audioPlayer, GameObject root, string name, bool loop)
        {
            audioPlayer.AudioSourceProxy.CreateOrUpdateAudioSource(root, name);

            audioPlayer.ClearDataAndStop();
            audioPlayer.AudioName = name;
            audioPlayer.AudioSourceProxy.SetLoop(loop);
            audioPlayer.AudioSourceProxy.SetClip(mClip);
            audioPlayer.OnClipPrepareFinished(true, mClip);
            mClip = null;
        }
        

        public void UnPrepareClip()
        {
        }
    }
}