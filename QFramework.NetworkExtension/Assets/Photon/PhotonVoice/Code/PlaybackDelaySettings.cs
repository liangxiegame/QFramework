namespace Photon.Voice.Unity
{
    /// <summary> Playback delay configuration container. </summary>
    [System.Serializable]
    public struct PlaybackDelaySettings
    {
        public const int DEFAULT_LOW = 200;
        public const int DEFAULT_HIGH = 400;
        public const int DEFAULT_MAX = 1000;

        /// <summary> ms: Audio player tries to keep the delay above this value. </summary>
        public int MinDelaySoft;
        /// <summary> ms: Audio player tries to keep the delay below this value. </summary>
        public int MaxDelaySoft;
        /// <summary> ms: Audio player guarantees that the delay never exceeds this value. </summary>
        public int MaxDelayHard;

        public override string ToString()
        {
            return string.Format("[low={0}ms,high={1}ms,max={2}ms]", this.MinDelaySoft, this.MaxDelaySoft, this.MaxDelayHard);
        }
    }
}