using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace CSCore
{
    //http://msdn.microsoft.com/en-us/library/windows/hardware/ff536383(v=vs.85).aspx
    //http://msdn.microsoft.com/en-us/library/windows/hardware/gg463006.aspx
    /// <summary>
    ///     Defines the format of waveform-audio data for formats having more than two channels or higher sample resolutions
    ///     than allowed by <see cref="WaveFormat" />.
    ///     Can be used to define any format that can be defined by <see cref="WaveFormat" />.
    ///     For more information see <see href="http://msdn.microsoft.com/en-us/library/windows/hardware/gg463006.aspx" /> and
    ///     <see href="http://msdn.microsoft.com/en-us/library/windows/hardware/ff536383(v=vs.85).aspx" />.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 2, CharSet = CharSet.Ansi)]
    public class WaveFormatExtensible : WaveFormat
    {
        internal const int WaveFormatExtensibleExtraSize = 22; //2(WORD) + 4(DWORD) + 16(GUID)

        private short _samplesUnion;

        private ChannelMask _channelMask;
        private Guid _subFormat;


        /// <summary>
        ///     Returns the SubType-Guid of a <paramref name="waveFormat" />. If the specified <paramref name="waveFormat" /> does
        ///     not contain a SubType-Guid, the <see cref="WaveFormat.WaveFormatTag" /> gets converted to the equal SubType-Guid
        ///     using the <see cref="AudioSubTypes.SubTypeFromEncoding" /> method.
        /// </summary>
        /// <param name="waveFormat"><see cref="WaveFormat" /> which gets used to determine the SubType-Guid.</param>
        /// <returns>SubType-Guid of the specified <paramref name="waveFormat" />.</returns>
        public static Guid SubTypeFromWaveFormat(WaveFormat waveFormat)
        {
            if (waveFormat == null)
                throw new ArgumentNullException("waveFormat");
            if (waveFormat is WaveFormatExtensible)
                return ((WaveFormatExtensible) waveFormat).SubFormat;
            return AudioSubTypes.SubTypeFromEncoding(waveFormat.WaveFormatTag);
        }

        /// <summary>
        ///     Gets the number of bits of precision in the signal.
        ///     Usually equal to <see cref="WaveFormat.BitsPerSample" />. However, <see cref="WaveFormat.BitsPerSample" /> is the
        ///     container size and must be a multiple of 8, whereas <see cref="ValidBitsPerSample" /> can be any value not
        ///     exceeding the container size. For example, if the format uses 20-bit samples,
        ///     <see cref="WaveFormat.BitsPerSample" /> must be at least 24, but <see cref="ValidBitsPerSample" /> is 20.
        /// </summary>
        public int ValidBitsPerSample
        {
            get { return _samplesUnion; }
            internal protected set { _samplesUnion = (short) value; }
        }

        /// <summary>
        ///     Gets the number of samples contained in one compressed block of audio data. This value is used in buffer
        ///     estimation. This value is used with compressed formats that have a fixed number of samples within each block. This
        ///     value can be set to 0 if a variable number of samples is contained in each block of compressed audio data. In this
        ///     case, buffer estimation and position information needs to be obtained in other ways.
        /// </summary>
        public int SamplesPerBlock
        {
            get { return _samplesUnion; }
            internal protected set { _samplesUnion = (short) value; }
        }

        /// <summary>
        ///     Gets a bitmask specifying the assignment of channels in the stream to speaker positions.
        /// </summary>
        public ChannelMask ChannelMask
        {
            get { return _channelMask; }
            internal protected set { _channelMask = value; }
        }

        /// <summary>
        ///     Subformat of the data, such as <see cref="AudioSubTypes.Pcm" />. The subformat information is similar to
        ///     that provided by the tag in the <see cref="WaveFormat" /> class's <see cref="WaveFormat.WaveFormatTag" /> member.
        /// </summary>
        public Guid SubFormat
        {
            get { return _subFormat; }
            internal protected set { _subFormat = value; }
        }

        internal WaveFormatExtensible()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="WaveFormatExtensible" /> class.
        /// </summary>
        /// <param name="sampleRate">
        ///     Samplerate of the waveform-audio. This value will get applied to the
        ///     <see cref="WaveFormat.SampleRate" /> property.
        /// </param>
        /// <param name="bits">
        ///     Bits per sample of the waveform-audio. This value will get applied to the
        ///     <see cref="WaveFormat.BitsPerSample" /> property and the <see cref="ValidBitsPerSample" /> property.
        /// </param>
        /// <param name="channels">
        ///     Number of channels of the waveform-audio. This value will get applied to the
        ///     <see cref="WaveFormat.Channels" /> property.
        /// </param>
        /// <param name="subFormat">Subformat of the data. This value will get applied to the <see cref="SubFormat" /> property.</param>
        public WaveFormatExtensible(int sampleRate, int bits, int channels, Guid subFormat)
            : base(sampleRate, bits, channels, AudioEncoding.Extensible, WaveFormatExtensibleExtraSize)
        {
            _samplesUnion = (short) bits;
            _subFormat = SubTypeFromWaveFormat(this);
            int cm = 0;
            for (int i = 0; i < channels; i++)
            {
                cm |= (1 << i);
            }

            _channelMask = (ChannelMask) cm;
            _subFormat = subFormat;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="WaveFormatExtensible" /> class.
        /// </summary>
        /// <param name="sampleRate">
        ///     Samplerate of the waveform-audio. This value will get applied to the
        ///     <see cref="WaveFormat.SampleRate" /> property.
        /// </param>
        /// <param name="bits">
        ///     Bits per sample of the waveform-audio. This value will get applied to the
        ///     <see cref="WaveFormat.BitsPerSample" /> property and the <see cref="ValidBitsPerSample" /> property.
        /// </param>
        /// <param name="channels">
        ///     Number of channels of the waveform-audio. This value will get applied to the
        ///     <see cref="WaveFormat.Channels" /> property.
        /// </param>
        /// <param name="subFormat">Subformat of the data. This value will get applied to the <see cref="SubFormat" /> property.</param>
        /// <param name="channelMask">
        ///     Bitmask specifying the assignment of channels in the stream to speaker positions. Thie value
        ///     will get applied to the <see cref="ChannelMask" /> property.
        /// </param>
        public WaveFormatExtensible(int sampleRate, int bits, int channels, Guid subFormat, ChannelMask channelMask)
            : this(sampleRate, bits, channels, subFormat)
        {
            Array totalChannelMaskValues = Enum.GetValues(typeof (ChannelMask));
            int valuesSet = 0;
            for (int i = 0; i < totalChannelMaskValues.Length; i++)
            {
                if ((channelMask & (ChannelMask) totalChannelMaskValues.GetValue(i)) ==
                    (ChannelMask) totalChannelMaskValues.GetValue(i))
                    valuesSet++;
            }

            if (channels != valuesSet)
                throw new ArgumentException("Channels has to equal the set flags in the channelmask.");

            _channelMask = channelMask;
        }

        /// <summary>
        ///     Converts the <see cref="WaveFormatExtensible" /> instance to a raw <see cref="WaveFormat" /> instance by converting
        ///     the <see cref="SubFormat" /> to the equal <see cref="WaveFormat.WaveFormatTag" />.
        /// </summary>
        /// <returns>A simple <see cref="WaveFormat"/> instance.</returns>
        public WaveFormat ToWaveFormat()
        {
            return new WaveFormat(SampleRate, BitsPerSample, Channels, AudioSubTypes.EncodingFromSubType(SubFormat));
        }

        /// <summary>
        /// Creates a new <see cref="WaveFormat" /> object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A copy of the current instance.
        /// </returns>
        public override object Clone()
        {
            //var waveFormat = new WaveFormatExtensible(SampleRate, BitsPerSample, Channels, SubFormat, ChannelMask);
            //waveFormat._samplesUnion = _samplesUnion;
            //return waveFormat;
            return MemberwiseClone();
        }

        internal override void SetWaveFormatTagInternal(AudioEncoding waveFormatTag)
        {
            SubFormat = AudioSubTypes.SubTypeFromEncoding(waveFormatTag);
        }

        /// <summary>
        ///     Returns a string which describes the <see cref="WaveFormatExtensible" />.
        /// </summary>
        /// <returns>A string which describes the <see cref="WaveFormatExtensible" />.</returns>
        [DebuggerStepThrough]
        public override string ToString()
        {
            var stringBuilder = new StringBuilder(base.ToString());
            stringBuilder.Append("|SubFormat: " + SubFormat);
            stringBuilder.Append("|ChannelMask: " + ChannelMask);
            return stringBuilder.ToString();
        }
    }
}	
