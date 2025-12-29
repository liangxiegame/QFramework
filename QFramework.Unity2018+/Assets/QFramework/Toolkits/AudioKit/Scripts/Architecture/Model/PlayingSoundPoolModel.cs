/****************************************************************************
 * Copyright (c) 2015 - 2025 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 * AudioKit v1.0: use QFramework.cs architecture
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

namespace QFramework
{
    internal class PlayingSoundPoolModel : AbstractModel
    {
        internal Dictionary<string, List<AudioPlayer>> SoundPlayerInPlaying =
            new Dictionary<string, List<AudioPlayer>>(30);

        protected override void OnInit()
        {
        }

        internal void ClearAllPlayingSound() => SoundPlayerInPlaying.Clear();

        internal void RemoveSoundPlayerFromPool(string nameForPool,AudioPlayer audioPlayer)
        {
            if (SoundPlayerInPlaying.TryGetValue(nameForPool, out var list))
            {
                list.Remove(audioPlayer);
            }
        }

        internal void AddSoundPlayer2Pool(string nameForPool,AudioPlayer audioPlayer)
        {
            if (SoundPlayerInPlaying.TryGetValue(nameForPool, out var list))
            {
                list.Add(audioPlayer);
            }
            else
            {
                SoundPlayerInPlaying.Add(nameForPool, new List<AudioPlayer> { audioPlayer });
            }
        }

        public void ForEachAllSound(Action<AudioPlayer> operation)
        {
            var soundPlayerInPlaying =
                SoundPlayerInPlaying.SelectMany(keyValuePair => keyValuePair.Value);

            var pool = ListPool<AudioPlayer>.Get((SoundPlayerInPlaying.Count / 8 + 1) * 16);

            foreach (var audioPlayer in soundPlayerInPlaying)
            {
                pool.Add(audioPlayer);
            }

            foreach (var audioPlayer in pool)
            {
                operation(audioPlayer);
            }

            pool.Release2Pool();
        }
    }
}