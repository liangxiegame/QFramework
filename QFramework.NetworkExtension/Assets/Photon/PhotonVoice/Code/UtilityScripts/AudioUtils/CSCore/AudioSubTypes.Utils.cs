using System;

namespace CSCore
{
    /// <summary>
    /// Defines <see cref="AudioSubTypes"/> and provides methods to convert between <see cref="AudioEncoding"/>-values and <see cref="AudioSubTypes"/>-values.
    /// </summary>
    /// <remarks><see cref="AudioSubTypes"/> are used by the <see cref="WaveFormatExtensible"/>, the <see cref="MFMediaType"/> and the <see cref="MediaType"/> class.</remarks>
    public static partial class AudioSubTypes
    {
        /// <summary>
        /// Converts a <see cref="AudioSubTypes"/>-value to a <see cref="AudioEncoding"/>-value.
        /// </summary>
        /// <param name="audioSubType">The <see cref="AudioSubTypes"/>-value to convert to the equivalent <see cref="AudioEncoding"/>-value.</param>
        /// <returns>The <see cref="AudioEncoding"/> which belongs to the specified <paramref name="audioSubType"/>.</returns>
        public static AudioEncoding EncodingFromSubType(Guid audioSubType)
        {
            var bytes = audioSubType.ToByteArray();
            int value = BitConverter.ToInt32(bytes, 0);
            if (Enum.IsDefined(typeof(AudioEncoding), (short)value))
                return (AudioEncoding)value;

            throw new ArgumentException("Invalid audioSubType.", "audioSubType");
        }

        /// <summary>
        /// Converts a <see cref="AudioEncoding"/> value to a <see cref="AudioSubTypes"/>-value.
        /// </summary>
        /// <param name="audioEncoding">The <see cref="AudioEncoding"/> to convert to the equivalent <see cref="AudioSubTypes"/>-value.</param>
        /// <returns>The <see cref="AudioSubTypes"/>-value which belongs to the specified <paramref name="audioEncoding"/>.</returns>
        public static Guid SubTypeFromEncoding(AudioEncoding audioEncoding)
        {
            if (Enum.IsDefined(typeof(AudioEncoding), (short)audioEncoding))
                return new Guid((int)audioEncoding, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

            throw new ArgumentException("Invalid encoding.", "audioEncoding");
        }

        /// <summary>
        /// The Major Type for <c>Audio</c> media types.
        /// </summary>
        public static readonly Guid MediaTypeAudio = new Guid("73647561-0000-0010-8000-00AA00389B71");

        ///// <summary>
        ///// FLAC
        ///// </summary>
        //public static readonly Guid WAVE_FORMAT_FLAC = new Guid("0000f1ac-0000-0010-8000-00aa00389b71");
    }
}
