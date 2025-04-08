using System;
using System.Collections.Generic;
using System.Linq;

namespace QFramework
{
    internal class AudioKitArchitecture : Architecture<AudioKitArchitecture>
    {
        internal static AudioPlayerModel AudioPlayerModel => mAudioPlayerModel ?? (mAudioPlayerModel = Interface.GetModel<AudioPlayerModel>());
        private static AudioPlayerModel mAudioPlayerModel = null;
        
        protected override void Init()
        {
            this.RegisterModel(new AudioPlayerModel());
        }
        
        internal static void ClearAllPlayingSound()
        {
            AudioPlayerModel.SoundPlayerInPlaying.Clear();
        }
        
        internal static void RemoveSoundPlayerFromPool(AudioPlayer audioPlayer)
        {
            AudioPlayerModel.SoundPlayerInPlaying[audioPlayer.GetName].Remove(audioPlayer);
        }
        
        internal static void AddSoundPlayer2Pool(AudioPlayer audioPlayer)
        {
            if (AudioPlayerModel.SoundPlayerInPlaying.ContainsKey(audioPlayer.GetName))
            {
                AudioPlayerModel.SoundPlayerInPlaying[audioPlayer.GetName].Add(audioPlayer);
            }
            else
            {
                AudioPlayerModel.SoundPlayerInPlaying.Add(audioPlayer.GetName, new List<AudioPlayer> { audioPlayer });
            }
        }
        
        public static void ForEachAllSound(Action<AudioPlayer> operation)
        {
            var soundPlayerInPlaying =
                AudioPlayerModel.SoundPlayerInPlaying.SelectMany(keyValuePair => keyValuePair.Value);
            
            var pool = ListPool<AudioPlayer>.Get((AudioPlayerModel.SoundPlayerInPlaying.Count / 8 + 1) * 16);
            
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

    internal class AudioPlayerModel : AbstractModel
    {
        public Dictionary<string, List<AudioPlayer>> SoundPlayerInPlaying =
            new Dictionary<string, List<AudioPlayer>>(30);
        
        protected override void OnInit()
        {
            
        }
    }
}