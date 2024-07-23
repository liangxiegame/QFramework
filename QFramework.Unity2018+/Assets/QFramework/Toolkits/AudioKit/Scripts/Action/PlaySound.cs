/****************************************************************************
 * Copyright (c) 2016 ~ 2024 liangxie
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using UnityEngine;

namespace QFramework
{
    public class PlaySoundAction : AbstractAction<PlaySoundAction>
    {
        public enum Modes
        {
            ByName,
            ByClip
        }

        private Modes mMode;
        private string mSoundName;
        private AudioClip mAudioClip;
        private Action mOnFinish;

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
                AudioKit.PlaySound(mSoundName,callBack:player => this.Finish());
            }
            else if (mMode == Modes.ByClip)
            {
                AudioKit.PlaySound(mAudioClip,callBack:player => this.Finish());
            }
        }
        

        public override void OnFinish()
        {
            mOnFinish?.Invoke();
        }

        protected override void OnDeinit()
        {
            mSoundName = null;
            mAudioClip = null;
            mOnFinish = null;
        }
    }
    
    public static class PlaySoundExtension
    {
        public static ISequence PlaySound(this ISequence self, string soundName)
        {
            return self.Append(QFramework.PlaySoundAction.Allocate(soundName));
        }
        
        public static ISequence PlaySound(this ISequence self,AudioClip clip)
        {
            return self.Append(QFramework.PlaySoundAction.Allocate(clip));
        }
    }
}