/****************************************************************************
 * Copyright (c) 2015 - 2025 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 * AudioKit v1.0: use QFramework.cs architecture
 ****************************************************************************/

using System;

namespace QFramework
{
    internal static class PlayVoiceCommand
    {
        internal static void Execute(string voiceName, bool loop = false, Action onBeganCallback = null,
            Action onEndedCallback = null)
        {
            var audioMgr = AudioManager.Instance;
            AudioManager.Instance.CheckAudioListener();
            audioMgr.CurrentVoiceName = voiceName;

            if (!AudioKit.Settings.IsVoiceOn.Value)
            {
                return;
            }
            
            AudioKit.VoicePlayer.OnStart(onBeganCallback);
            AudioKit.VoicePlayer.PrepareByNameAsyncAndPlay(AudioManager.Instance.gameObject, voiceName, loop);
            AudioKit.VoicePlayer.OnFinish(onEndedCallback);
        }
        
    }
}