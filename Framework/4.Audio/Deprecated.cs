namespace QFramework
{
    public partial class AudioManager
    {

        public static bool IsSoundOn
        {
            get { return Instance.Settings.IsSoundOn.Value; }
            set { Instance.Settings.IsSoundOn.Value = value; }
        }

        public static bool IsMusicOn
        {
            get { return Instance.Settings.IsMusicOn.Value; }
            set { Instance.Settings.IsMusicOn.Value = value; }
        }

        public static bool IsVoiceOn
        {
            get { return Instance.Settings.IsVoiceOn.Value; }
            set { Instance.Settings.IsVoiceOn.Value = value; }
        }

        public float SoundVolume
        {
            get { return Instance.Settings.SoundVolume.Value; }
            set { Instance.Settings.SoundVolume.Value = value; }
        }

        public float MusicVolume
        {
            get { return Instance.Settings.MusicVolume.Value; }
            set { Instance.Settings.MusicVolume.Value = value; }
        }

        public float VoiceVolume
        {
            get { return Instance.Settings.VoiceVolume.Value; }
            set { Instance.Settings.VoiceVolume.Value = value; }
        }
    }
}