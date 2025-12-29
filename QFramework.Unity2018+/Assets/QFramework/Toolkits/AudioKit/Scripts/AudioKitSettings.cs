/****************************************************************************
 * Copyright (c) 2015 - 2025 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 * AudioKit v1.0: use QFramework.cs architecture
 ****************************************************************************/

namespace QFramework
{
    /// <summary>
    /// TODO maybe support custom settings storage later
    /// </summary>
    public class AudioKitSettingsModel : AbstractModel
    {
        private const string KeyAudioManagerSoundOn = "KEY_AUDIO_MANAGER_SOUND_ON";
        private const string KeyAudioManagerMusicOn = "KEY_AUDIO_MANAGER_MUSIC_ON";
        private const string KeyAudioManagerVoiceOn = "KEY_AUDIO_MANAGER_VOICE_ON";
        private const string KeyAudioManagerVoiceVolume = "KEY_AUDIO_MANAGER_VOICE_VOLUME";
        private const string KeyAudioManagerSoundVolume = "KEY_AUDIO_MANAGER_SOUND_VOLUME";
        private const string KeyAudioManagerMusicVolume = "KEY_AUDIO_MANAGER_MUSIC_VOLUME";
        

        public PlayerPrefsBooleanProperty IsSoundOn { get; private set; }

        public PlayerPrefsBooleanProperty IsMusicOn { get; private set; }

        public PlayerPrefsBooleanProperty IsVoiceOn { get; private set; }
        
        public PlayerPrefsFloatProperty SoundVolume { get; private set; }
        
        public PlayerPrefsFloatProperty MusicVolume { get; private set; }
        
        public PlayerPrefsFloatProperty VoiceVolume { get; private set; }
        
        public CustomProperty<bool> IsOn { get; private set; }
        protected override void OnInit()
        {
            IsSoundOn = new PlayerPrefsBooleanProperty(KeyAudioManagerSoundOn, true);

            IsMusicOn = new PlayerPrefsBooleanProperty(KeyAudioManagerMusicOn, true);

            IsVoiceOn = new PlayerPrefsBooleanProperty(KeyAudioManagerVoiceOn, true);

            
            IsOn = new CustomProperty<bool>(
                () => IsSoundOn.Value && IsMusicOn.Value && IsVoiceOn.Value,
                isOn =>
                {
                    IsSoundOn.Value = isOn;
                    IsMusicOn.Value = isOn;
                    IsVoiceOn.Value = isOn;
                }
            );

            SoundVolume = new PlayerPrefsFloatProperty(KeyAudioManagerSoundVolume, 1.0f);

            MusicVolume = new PlayerPrefsFloatProperty(KeyAudioManagerMusicVolume, 1.0f);

            VoiceVolume = new PlayerPrefsFloatProperty(KeyAudioManagerVoiceVolume, 1.0f);
        }
    }
}