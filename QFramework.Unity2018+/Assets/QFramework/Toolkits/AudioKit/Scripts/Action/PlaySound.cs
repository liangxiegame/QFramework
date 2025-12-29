/****************************************************************************
 * Copyright (c) 2015 - 2025 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 * AudioKit v1.0: use QFramework.cs architecture
 ****************************************************************************/

using System;
using UnityEngine;

namespace QFramework
{
    public class PlaySoundAction : AbstractAction<PlaySoundAction>
    {
        private enum Modes
        {
            ByName,
            ByClip
        }

        private Modes mMode;
        private string mSoundName;
        private AudioClip mAudioClip;
        private Action mOnFinish;
        private AudioPlayer mAudioPlayer;

        public static PlaySoundAction Allocate(string soundName,Action onFinish = null)
        {
            var playSoundAction = Allocate();
            playSoundAction.mMode = Modes.ByName;
            playSoundAction.mSoundName = soundName;
            playSoundAction.mOnFinish = onFinish;
            return playSoundAction;
        }

        public static PlaySoundAction Allocate(AudioClip sound,Action onFinish = null)
        {
            var playSoundAction = Allocate();
            playSoundAction.mMode = Modes.ByClip;
            playSoundAction.mAudioClip = sound;
            playSoundAction.mOnFinish = onFinish;
            return playSoundAction;
        }


        public override void OnStart()
        {
            if (mMode == Modes.ByName)
            {
                mAudioPlayer = AudioKit.PlaySound(mSoundName,callBack:player =>
                {
                    mAudioPlayer = null;
                    this.Finish();
                });
            }
            else if (mMode == Modes.ByClip)
            {
                mAudioPlayer = AudioKit.PlaySound(mAudioClip,callBack:player =>
                {
                    mAudioPlayer = null;
                    this.Finish();
                });
            }
        }

        public override void OnFinish()
        {
            mOnFinish?.Invoke();
        }

        protected override void OnDeinit()
        {
            if (mAudioPlayer != null)
            {
                mAudioPlayer.Stop();
                mAudioPlayer = null;
            }
            
            mSoundName = null;
            mAudioClip = null;
            mOnFinish = null;
        }
    }
    
    public static class PlaySoundExtension
    {
        public static ISequence PlaySound(this ISequence self, string soundName) => self.Append(QFramework.PlaySoundAction.Allocate(soundName));

        public static ISequence PlaySound(this ISequence self,AudioClip clip) => self.Append(QFramework.PlaySoundAction.Allocate(clip));
    }
}