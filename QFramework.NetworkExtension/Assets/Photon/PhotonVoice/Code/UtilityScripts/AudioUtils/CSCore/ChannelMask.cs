using System;

namespace CSCore
{
    /// <summary>
    ///     Channelmask used by <see cref="WaveFormatExtensible" />. For more information see
    ///     http://msdn.microsoft.com/en-us/library/windows/desktop/dd757714(v=vs.85).aspx
    /// </summary>
    [Flags]
    public enum ChannelMask
    {
        /// <summary>
        ///     Front left speaker.
        /// </summary>
        SpeakerFrontLeft = 0x1,

        /// <summary>
        ///     Front right speaker.
        /// </summary>
        SpeakerFrontRight = 0x2,

        /// <summary>
        ///     Front center speaker.
        /// </summary>
        SpeakerFrontCenter = 0x4,

        /// <summary>
        ///     Low frequency speaker.
        /// </summary>
        SpeakerLowFrequency = 0x8,

        /// <summary>
        ///     Back left speaker.
        /// </summary>
        SpeakerBackLeft = 0x10,

        /// <summary>
        ///     Back right speaker.
        /// </summary>
        SpeakerBackRight = 0x20,

        /// <summary>
        ///     Front left of center speaker.
        /// </summary>
        SpeakerFrontLeftOfCenter = 0x40,

        /// <summary>
        ///     Front right of center speaker.
        /// </summary>
        SpeakerFrontRightOfCenter = 0x80,

        /// <summary>
        ///     Back center speaker.
        /// </summary>
        SpeakerBackCenter = 0x100,

        /// <summary>
        ///     Side left speaker.
        /// </summary>
        SpeakerSideLeft = 0x200,

        /// <summary>
        ///     Side right speaker.
        /// </summary>
        SpeakerSideRight = 0x400,

        /// <summary>
        ///     Top center speaker.
        /// </summary>
        SpeakerTopCenter = 0x800,

        /// <summary>
        ///     Top front left speaker.
        /// </summary>
        SpeakerTopFrontLeft = 0x1000,

        /// <summary>
        ///     Top front center speaker.
        /// </summary>
        SpeakerTopFrontCenter = 0x2000,

        /// <summary>
        ///     Top front right speaker.
        /// </summary>
        SpeakerTopFrontRight = 0x4000,

        /// <summary>
        ///     Top back left speaker.
        /// </summary>
        SpeakerTopBackLeft = 0x8000,

        /// <summary>
        ///     Top back center speaker.
        /// </summary>
        SpeakerTopBackCenter = 0x10000,

        /// <summary>
        ///     Top back right speaker.
        /// </summary>
        SpeakerTopBackRight = 0x20000
    }
}