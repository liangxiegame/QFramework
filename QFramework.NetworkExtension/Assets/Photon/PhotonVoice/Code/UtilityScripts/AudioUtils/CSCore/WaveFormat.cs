using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace CSCore
{
    // ReSharper disable ConvertToAutoProperty
    /// <summary>
    ///     Defines the format of waveform-audio data.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public class WaveFormat : ICloneable, IEquatable<WaveFormat>
    {
        private AudioEncoding _encoding;
        private short _channels;
        private int _sampleRate;
        private int _bytesPerSecond;

        private short _blockAlign;
        private short _bitsPerSample;
        private short _extraSize;

        /// <summary>
        ///     Gets the number of channels in the waveform-audio data. Mono data uses one channel and stereo data uses two
        ///     channels.
        /// </summary>
        public virtual int Channels
        {
            get { return _channels; }
            protected internal set
            {
                _channels = (short) value;
                UpdateProperties();
            }
        }

        /// <summary>
        ///     Gets the sample rate, in samples per second (hertz).
        /// </summary>
        public virtual int SampleRate
        {
            get { return _sampleRate; }
            protected internal set
            {
                _sampleRate = value;
                UpdateProperties();
            }
        }

        /// <summary>
        ///     Gets the required average data transfer rate, in bytes per second. For example, 16-bit stereo at 44.1 kHz has an
        ///     average data rate of 176,400 bytes per second (2 channels — 2 bytes per sample per channel — 44,100 samples per
        ///     second).
        /// </summary>
        public virtual int BytesPerSecond
        {
            get { return _bytesPerSecond; }
            protected internal set { _bytesPerSecond = value; }
        }

        /// <summary>
        ///     Gets the block alignment, in bytes. The block alignment is the minimum atomic unit of data. For PCM data, the block
        ///     alignment is the number of bytes used by a single sample, including data for both channels if the data is stereo.
        ///     For example, the block alignment for 16-bit stereo PCM is 4 bytes (2 channels x 2 bytes per sample).
        /// </summary>
        public virtual int BlockAlign
        {
            get { return _blockAlign; }
            protected internal set { _blockAlign = (short) value; }
        }

        /// <summary>
        ///     Gets the number of bits, used to store one sample.
        /// </summary>
        public virtual int BitsPerSample
        {
            get { return _bitsPerSample; }
            protected internal set
            {
                _bitsPerSample = (short) value;
                UpdateProperties();
            }
        }

        /// <summary>
        ///     Gets the size (in bytes) of extra information. This value is mainly used for marshalling.
        /// </summary>
        public virtual int ExtraSize
        {
            get { return _extraSize; }
            protected internal set { _extraSize = (short) value; }
        }

        /// <summary>
        ///     Gets the number of bytes, used to store one sample.
        /// </summary>
        public virtual int BytesPerSample
        {
            get { return BitsPerSample / 8; }
        }

        /// <summary>
        ///     Gets the number of bytes, used to store one block. This value equals <see cref="BytesPerSample" /> multiplied with
        ///     <see cref="Channels" />.
        /// </summary>
        public virtual int BytesPerBlock
        {
            get { return BytesPerSample * Channels; }
        }

        /// <summary>
        ///     Gets the waveform-audio format type.
        /// </summary>
        public virtual AudioEncoding WaveFormatTag
        {
            get { return _encoding; }
            protected internal set { _encoding = value; }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="WaveFormat" /> class with a sample rate of 44100 Hz, bits per sample
        ///     of 16 bit, 2 channels and PCM as the format type.
        /// </summary>
        public WaveFormat()
            : this(44100, 16, 2)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="WaveFormat" /> class with PCM as the format type.
        /// </summary>
        /// <param name="sampleRate">Samples per second.</param>
        /// <param name="bits">Number of bits, used to store one sample.</param>
        /// <param name="channels">Number of channels in the waveform-audio data.</param>
        public WaveFormat(int sampleRate, int bits, int channels)
            : this(sampleRate, bits, channels, AudioEncoding.Pcm)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="WaveFormat" /> class.
        /// </summary>
        /// <param name="sampleRate">Samples per second.</param>
        /// <param name="bits">Number of bits, used to store one sample.</param>
        /// <param name="channels">Number of channels in the waveform-audio data.</param>
        /// <param name="encoding">Format type or encoding of the wave format.</param>
        public WaveFormat(int sampleRate, int bits, int channels, AudioEncoding encoding)
            : this(sampleRate, bits, channels, encoding, 0)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="WaveFormat" /> class.
        /// </summary>
        /// <param name="sampleRate">Samples per second.</param>
        /// <param name="bits">Number of bits, used to store one sample.</param>
        /// <param name="channels">Number of channels in the waveform-audio data.</param>
        /// <param name="encoding">Format type or encoding of the wave format.</param>
        /// <param name="extraSize">Size (in bytes) of extra information. This value is mainly used for marshalling.</param>
        public WaveFormat(int sampleRate, int bits, int channels, AudioEncoding encoding, int extraSize)
        {
            if (sampleRate < 1)
                throw new ArgumentOutOfRangeException("sampleRate");
            if (bits < 0)
                throw new ArgumentOutOfRangeException("bits");
            if (channels < 1)
                throw new ArgumentOutOfRangeException("channels", "Number of channels has to be bigger than 0.");

            _sampleRate = sampleRate;
            _bitsPerSample = (short) bits;
            _channels = (short) channels;
            _encoding = encoding;
            _extraSize = (short) extraSize;

// ReSharper disable once DoNotCallOverridableMethodsInConstructor
            UpdateProperties();
        }

        /// <summary>
        ///     Converts a duration in milliseconds to a duration in bytes.
        /// </summary>
        /// <param name="milliseconds">Duration in millisecond to convert to a duration in bytes.</param>
        /// <returns>Duration in bytes.</returns>
        public long MillisecondsToBytes(double milliseconds)
        {
            var result = (long) ((BytesPerSecond / 1000.0) * milliseconds);
            result -= result % BlockAlign;
            return result;
        }

        /// <summary>
        ///     Converts a duration in bytes to a duration in milliseconds.
        /// </summary>
        /// <param name="bytes">Duration in bytes to convert to a duration in milliseconds.</param>
        /// <returns>Duration in milliseconds.</returns>
        public double BytesToMilliseconds(long bytes)
        {
            bytes -= bytes % BlockAlign;
            var result = ((bytes / (double) BytesPerSecond) * 1000.0);
            return result;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">The <see cref="WaveFormat"/> to compare with this <see cref="WaveFormat"/>.</param>
        /// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
        public virtual bool Equals(WaveFormat other)
        {
            return Channels == other.Channels &&
                   SampleRate == other.SampleRate &&
                   BytesPerSecond == other.BytesPerSecond &&
                   BlockAlign == other.BlockAlign &&
                   BitsPerSample == other.BitsPerSample &&
                   ExtraSize == other.ExtraSize &&
                   WaveFormatTag == other.WaveFormatTag;
        }

        /// <summary>
        ///     Returns a string which describes the <see cref="WaveFormat" />.
        /// </summary>
        /// <returns>A string which describes the <see cref="WaveFormat" />.</returns>
        public override string ToString()
        {
            return GetInformation().ToString();
        }

        /// <summary>
        ///     Creates a new <see cref="WaveFormat" /> object that is a copy of the current instance.
        /// </summary>
        /// <returns>A copy of the current instance.</returns>
        public virtual object Clone()
        {
            return MemberwiseClone(); //since there are value types MemberWiseClone is enough.
        }

        internal virtual void SetWaveFormatTagInternal(AudioEncoding waveFormatTag)
        {
            WaveFormatTag = waveFormatTag;
        }

        internal virtual void SetBitsPerSampleAndFormatProperties(int bitsPerSample)
        {
            BitsPerSample = bitsPerSample;
            UpdateProperties();
        }

        /// <summary>
        /// Updates the <see cref="BlockAlign"/>- and the <see cref="BytesPerSecond"/>-property.
        /// </summary>
        internal protected virtual void UpdateProperties()
        {
            BlockAlign = (BitsPerSample / 8) * Channels;
            BytesPerSecond = BlockAlign * SampleRate;
        }

        [DebuggerStepThrough]
        private StringBuilder GetInformation()
        {
            var builder = new StringBuilder();
            builder.Append("ChannelsAvailable: " + Channels);
            builder.Append("|SampleRate: " + SampleRate);
            builder.Append("|Bps: " + BytesPerSecond);
            builder.Append("|BlockAlign: " + BlockAlign);
            builder.Append("|BitsPerSample: " + BitsPerSample);
            builder.Append("|Encoding: " + _encoding);

            return builder;
        }
    }

    // ReSharper restore ConvertToAutoProperty
}