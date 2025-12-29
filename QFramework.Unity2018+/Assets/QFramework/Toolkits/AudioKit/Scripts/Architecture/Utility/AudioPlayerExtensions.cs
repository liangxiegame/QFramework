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
    public static class AudioPlayerExtensions
    {
        public static T OnStart<T>(this T self, Action onStart) where T : AbstractAudioPlayer
        {
            self.LifeCycle.RegisterOnStartOnce(onStart);
            return self;
        }

        public static T OnFinish<T>(this T self, Action onFinish) where T : AbstractAudioPlayer
        {
            self.LifeCycle.RegisterOnFinishOnce(onFinish);
            return self;
        }
        

        public static T Pitch<T>(this T self, float pitch) where T : AbstractAudioPlayer
        {
            self.AudioSourceProxy.SetPitch(pitch);
            return self;
        }

        public static T VolumeScale<T>(this T self, float volumeScale) where T : AbstractAudioPlayer
        {
            self.AudioSourceProxy.SetVolumeScale(volumeScale);
            return self;
        }

        internal static AbstractAudioPlayer PrepareByNameAsyncAndPlay(this AbstractAudioPlayer self, GameObject root,
            string name, bool loop)
        {
            if (string.IsNullOrEmpty(name) || self.AudioName == name)
            {
                return self;
            }

            self.PrepareModeController.ChangePrepareMode(self.PrepareModeController.ByLoaderAsync);
            self.PrepareModeController.PrepareMode.PrepareClip(self, root, name, loop);
            return self;
        }
        
        internal static AbstractAudioPlayer PrepareByNameSyncAndPlay(this AbstractAudioPlayer self, GameObject root,
            string name, bool loop)
        {
            if (string.IsNullOrEmpty(name) || self.AudioName == name)
            {
                return self;
            }

            self.PrepareModeController.ChangePrepareMode(self.PrepareModeController.ByLoaderSync);
            self.PrepareModeController.PrepareMode.PrepareClip(self, root, name, loop);
            return self;
        }

        internal static T PrepareByClipAndPlay<T>(this T self, GameObject root, AudioClip clip, string name, bool loop)
            where T : AbstractAudioPlayer
        {
            if (clip == null || self.AudioName == name)
            {
                return self;
            }

            self.PrepareModeController.BySetUp.SetClipForPrepare(clip);
            self.PrepareModeController.ChangePrepareMode(self.PrepareModeController.BySetUp);
            self.PrepareModeController.PrepareMode.PrepareClip(self, root, name, loop);
            return self;
        }
        

        public static void Pause(this AbstractAudioPlayer self)
        {
            if (self.IsPause)
            {
                return;
            }

            self.AudioSourceProxy.Pause();
        }

        public static void Resume(this AbstractAudioPlayer self)
        {
            if (!self.IsPause)
            {
                return;
            }

            self.LifeCycle.CallOnStartOnce();
            self.AudioSourceProxy.Play(self.OnPlayFinished);
        }
    }
}