/****************************************************************************
 * Copyright (c) 2015 - 2025 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 * AudioKit v1.0: use QFramework.cs architecture
 ****************************************************************************/

using System.Linq;
using UnityEngine;

namespace QFramework
{
    internal class ConsoleModuleSystem : AbstractSystem
    {
        private readonly FluentGUIStyle mSoundItemStyle = FluentGUIStyle.Label()
            .FontSize(12);
        protected override void OnInit()
        {
            var scrollPos = Vector2.zero;
            ConsoleKit.AddModule(new ConsoleModule()
            {
                Title = "AudioKit",

                OnDrawGUI = () =>
                {
                    scrollPos = GUILayout.BeginScrollView(scrollPos);
                    GUILayout.Label("Music Player:" + (AudioKit.MusicPlayer.AudioName ?? "No Music") + " Paused:" +
                                    AudioKit.MusicPlayer.IsPause);
                    GUILayout.Label("Voice Player:" + (AudioKit.VoicePlayer?.AudioName ?? "No Music"));

                    GUILayout.Label("Default Play Sound Mode:" +
                                    Architecture.PlaySoundChannelSystem.DefaultPlaySoundMode);

                    GUILayout.Label("Sound Player Count In Playing:" + Architecture.PlayingSoundPoolModel
                        .SoundPlayerInPlaying.SelectMany(kv => kv.Value).Count());

                    foreach (var kv in Architecture.PlayingSoundPoolModel.SoundPlayerInPlaying)
                    {
                        GUILayout.Label("  Sound Name:" + kv.Key + " Count:" + kv.Value.Count);

                        foreach (var audioPlayer in kv.Value)
                        {
                            GUILayout.BeginVertical("box");

                            GUILayout.Label("    Volume:" + audioPlayer.Volume + " Pos:" +
                                            audioPlayer?.AudioSourceProxy?.AudioSource?.gameObject?.Position() + " Mode:" + audioPlayer.PlaySoundMode,mSoundItemStyle);
                            GUILayout.EndVertical();

                        }
                    }
                    
                    GUILayout.EndScrollView();
                }
            });
        }
    }
}