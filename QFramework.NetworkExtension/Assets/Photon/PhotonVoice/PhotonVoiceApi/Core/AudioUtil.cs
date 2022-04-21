using System;

namespace Photon.Voice
{
    /// <summary>Collection of Audio Utility functions and classes.</summary>
    public static partial class AudioUtil
    {
        /// <summary>Resample audio data so that the complete src buffer fits into dstCount samples in the dst buffer.</summary>
        /// This implements a primitive nearest-neighbor resampling algorithm for an arbitrary number of channels.
        /// <param name="src">Source buffer.</param>
        /// <param name="dst">Destination buffer.</param>
        /// <param name="dstCount">Target size of destination buffer (in samples per channel).</param>
        /// <param name="channels">Number of channels in the signal (1=mono, 2=stereo). Must be > 0.</param>
        public static void Resample<T>(T[] src, T[] dst, int dstCount, int channels)
        {
            if (channels == 1)
            {
                for (int i = 0; i < dstCount; i++)
                {
                    dst[i] = src[i * src.Length / dstCount];
                }
            }
            else if (channels == 2)
            {
                for (int i = 0; i < dstCount / 2; i++)
                {
                    var srcI = i * src.Length / dstCount;
                    var dstCh0I = i * 2;
                    var srcCh0I = srcI * 2;
                    dst[dstCh0I++] = src[srcCh0I++];
                    dst[dstCh0I] = src[srcCh0I];
                }
            }
            else
            {
                for (int i = 0; i < dstCount / channels; i++)
                {
                    var srcI = i * src.Length / dstCount;
                    var dstCh0I = i * channels;
                    var srcCh0I = srcI * channels;
                    for (int ch = 0; ch < channels; ch++)
                    {
                        dst[dstCh0I++] = src[srcCh0I++];
                    }
                }
            }
        }

        public static void Resample<T>(T[] src, int srcOffset, int srcCount, T[] dst, int dstOffset, int dstCount, int channels)
        {
            if (channels == 1)
            {
                for (int i = 0; i < dstCount; i++)
                {
                    dst[dstOffset + i] = src[srcOffset + i * srcCount / dstCount];
                }
            }
            else if (channels == 2)
            {
                for (int i = 0; i < dstCount / 2; i++)
                {
                    var srcI = i * srcCount / dstCount;
                    var dstCh0I = i * 2;
                    var srcCh0I = srcI * 2;
                    dst[dstOffset + dstCh0I++] = src[srcOffset + srcCh0I++];
                    dst[dstOffset + dstCh0I] = src[srcOffset + srcCh0I];
                }
            }
            else
            {
                for (int i = 0; i < dstCount / channels; i++)
                {
                    var srcI = i * srcCount / dstCount;
                    var dstCh0I = i * channels;
                    var srcCh0I = srcI * channels;
                    for (int ch = 0; ch < channels; ch++)
                    {
                        dst[dstOffset + dstCh0I++] = src[srcOffset + srcCh0I++];
                    }
                }
            }
        }

        // does not sum channel values but only maps channel to channel (it's not possible to apply math to generic type T)
        public static void Resample<T>(T[] src, int srcOffset, int srcCount, int srcChannels, T[] dst, int dstOffset, int dstCount, int dstChannels)
        {
            if (srcChannels == dstChannels)
            {
                Resample<T>(src, srcOffset, srcCount, dst, dstOffset, dstCount, dstChannels);
                return;
            }

            if (srcChannels == 1 && dstChannels == 2)
            {
                for (int i = 0, j = 0; i < dstCount / 2; i++)
                {
                    var v = src[srcOffset + i * srcCount * 2 / dstCount];
                    dst[dstOffset + j++] = v;
                    dst[dstOffset + j++] = v;
                }
            }
            else if (srcChannels == 2 && dstChannels == 1)
            {
                for (int i = 0; i < dstCount; i++)
                {
                    dst[dstOffset + i] = src[srcOffset + i * srcCount / dstCount / 2 * 2];
                }

            }
            else
            {
                for (int i = 0, j = 0; i < dstCount / dstChannels; i++)
                {
                    var srcI = srcOffset + i * srcCount * dstChannels / dstCount / srcChannels * srcChannels;
                    if (srcChannels >= dstChannels)
                    {
                        for (int ch = 0; ch < dstChannels; ch++)
                        {
                            dst[dstOffset + j++] = src[srcI + ch];
                        }
                    }
                    else
                    {
                        for (int ch = 0; ch < srcChannels; ch++)
                        {
                            dst[dstOffset + j++] = src[srcI + ch];
                        }
                        j += dstChannels - srcChannels;
                    }
                }
            }
        }
        /// <summary>Resample audio data so that the complete src buffer fits into dstCount samples in the dst buffer,
        /// and convert short to float samples along the way.</summary>
        /// This implements a primitive nearest-neighbor resampling algorithm for an arbitrary number of channels.
        /// <param name="src">Source buffer.</param>
        /// <param name="dst">Destination buffer.</param>
        /// <param name="dstCount">Target size of destination buffer (in samples per channel).</param>
        /// <param name="channels">Number of channels in the signal (1=mono, 2=stereo). Must be > 0.</param>
        public static void ResampleAndConvert(short[] src, float[] dst, int dstCount, int channels)
        {
            if (channels == 1)
            {
                for (int i = 0; i < dstCount; i++)
                {
                    dst[i] = src[i * src.Length / dstCount] / (float)short.MaxValue;
                }
            }
            else if (channels == 2)
            {
                for (int i = 0; i < dstCount / 2; i++)
                {
                    var srcI = i * src.Length / dstCount;
                    var dstCh0I = i * 2;
                    var srcCh0I = srcI * 2;
                    dst[dstCh0I++] = src[srcCh0I++] / (float)short.MaxValue;
                    dst[dstCh0I] = src[srcCh0I] / (float)short.MaxValue;
                }
            }
            else
            {
                for (int i = 0; i < dstCount / channels; i++)
                {
                    var srcI = i * src.Length / dstCount;
                    var dstCh0I = i * channels;
                    var srcCh0I = srcI * channels;
                    for (int ch = 0; ch < channels; ch++)
                    {
                        dst[dstCh0I++] = src[srcCh0I++] / (float)short.MaxValue;
                    }
                }
            }
        }

        /// <summary>Resample audio data so that the complete src buffer fits into dstCount samples in the dst buffer,
        /// and convert float to short samples along the way.</summary>
        /// This implements a primitive nearest-neighbor resampling algorithm for an arbitrary number of channels.
        /// <param name="src">Source buffer.</param>
        /// <param name="dst">Destination buffer.</param>
        /// <param name="dstCount">Target size of destination buffer (in samples per channel).</param>
        /// <param name="channels">Number of channels in the signal (1=mono, 2=stereo). Must be > 0.</param>
        public static void ResampleAndConvert(float[] src, short[] dst, int dstCount, int channels)
        {
            if (channels == 1)
            {
                for (int i = 0; i < dstCount; i++)
                {
                    dst[i] = (short)(src[i * src.Length / dstCount] * (float)short.MaxValue);
                }
            }
            else if (channels == 2)
            {
                for (int i = 0; i < dstCount / 2; i++)
                {
                    var srcI = i * src.Length / dstCount;
                    var dstCh0I = i * 2;
                    var srcCh0I = srcI * 2;
                    dst[dstCh0I++] = (short)(src[srcCh0I++] * (float)short.MaxValue);
                    dst[dstCh0I] = (short)(src[srcCh0I] * (float)short.MaxValue);
                }
            }
            else
            {
                for (int i = 0; i < dstCount / channels; i++)
                {
                    var srcI = i * src.Length / dstCount;
                    var dstCh0I = i * channels;
                    var srcCh0I = srcI * channels;
                    for (int ch = 0; ch < channels; ch++)
                    {
                        dst[dstCh0I++] = (short)(src[srcCh0I++] * (float)short.MaxValue);
                    }
                }
            }
        }

        /// <summary>Convert audio buffer from float to short samples.</summary>
        /// <param name="src">Source buffer.</param>
        /// <param name="dst">Destination buffer.</param>
        /// <param name="dstCount">Size of destination buffer (in total samples), source buffer must be of same length or longer.</param>
        public static void Convert(float[] src, short[] dst, int dstCount)
        {
            for (int i = 0; i < dstCount; i++)
            {
                dst[i] = (short)(src[i] * (float)short.MaxValue);
            }
        }

        /// <summary>Convert audio buffer from short to float samples.</summary>
        /// <param name="src">Source buffer.</param>
        /// <param name="dst">Destination buffer.</param>
        /// <param name="dstCount">Size of destination buffer (in total samples), source buffer must be of same length or longer.</param>
        public static void Convert(short[] src, float[] dst, int dstCount)
        {
            for (int i = 0; i < dstCount; i++)
            {
                dst[i] = src[i] / (float)short.MaxValue;
            }
        }


        /// <summary>Convert audio buffer with arbitrary number of channels to stereo.</summary>
        /// For mono sources (srcChannels==1), the signal will be copied to both Left and Right stereo channels.
        /// For all others, the first two available channels will be used, any other channels will be discarded.
        /// <param name="src">Source buffer.</param>
        /// <param name="dst">Destination buffer.</param>
        /// <param name="srcChannels">Number of (interleaved) channels in src.</param>
        public static void ForceToStereo<T>(T[] src, T[] dst, int srcChannels)
        {
            for (int i = 0, j = 0; j < dst.Length - 1; i += srcChannels, j += 2)
            {
                dst[j] = src[i];
                dst[j + 1] = srcChannels > 1 ? src[i + 1] : src[i];
            }
        }

        internal static string tostr<T>(T[] x, int lim = 10)
        {
            System.Text.StringBuilder b = new System.Text.StringBuilder();
            for (var i = 0; i < (x.Length < lim ? x.Length : lim); i++)
            {
                b.Append("-");
                b.Append(x[i]);
            }
            return b.ToString();
        }

        public class TempoUp<T>
        {
            readonly int sizeofT = System.Runtime.InteropServices.Marshal.SizeOf(default(T));
            int channels;
            int skipGroup;

            int skipFactor;
            int sign = 0;
            int waveCnt;
            bool skipping;

            public void Begin(int channels, int changePerc, int skipGroup)
            {
                this.channels = channels;
                this.skipFactor = 100 / changePerc;
                this.skipGroup = skipGroup;
                sign = 0;
                skipping = false;
                waveCnt = 0;
            }

            public int Process(T[] s, T[] d)
            {
                if (sizeofT == 2)
                {
                    return processShort(s as short[], d as short[]);
                }
                else
                {
                    return processFloat(s as float[], d as float[]);
                }
            }

            // returns the number of samples required to skip in order to complete currently skipping wave
            public int End(T[] s)
            {
                if (!skipping)
                {
                    return 0;
                }
                if (sizeofT == 2)
                {
                    return endShort(s as short[]);
                }
                else
                {
                    return endFloat(s as float[]);
                }
            }
            
            int processFloat(float[] s, float[] d)
            {
                int dPos = 0;
                if (channels == 1)
                {
                    for (int i = 0; i < s.Length; i++)
                    {
                        if (s[i] < 0)
                        {
                            sign = -1;
                        }
                        else if (sign < 0)
                        {
                            waveCnt++;
                            skipping = waveCnt % (skipGroup * skipFactor) < skipGroup;
                            sign = 0;
                        }

                        if (!skipping)
                        {
                            d[dPos++] = s[i];
                        }
                    }
                }

                else if (channels == 2)
                {
                    for (int i = 0; i < s.Length; i += 2)
                    {
                        if (s[i] + s[i + 1] < 0)
                        {
                            sign = -1;
                        }
                        else if (sign < 0)
                        {
                            waveCnt++;
                            skipping = waveCnt % (skipGroup * skipFactor) < skipGroup;
                            sign = 0;
                        }

                        if (!skipping)
                        {
                            d[dPos++] = s[i];
                            d[dPos++] = s[i + 1];
                        }
                    }
                }

                else
                {
                    for (int i = 0; i < s.Length; i += channels)
                    {
                        var x = s[i] + s[i + 1];
                        for (int j = 2; i < channels; j++)
                        {
                            x += s[i + j];
                        }
                        if (x < 0)
                        {
                            sign = -1;
                        }
                        else if (sign < 0)
                        {
                            waveCnt++;
                            skipping = waveCnt % (skipGroup * skipFactor) < skipGroup;
                            sign = 0;
                        }

                        if (!skipping)
                        {
                            d[dPos++] = s[i];
                            d[dPos++] = s[i + 1];
                            for (int j = 2; i < channels; j++)
                            {
                                d[dPos++] += s[i + j];
                            }
                        }
                    }
                }

                return dPos / channels;
            }

            public int endFloat(float[] s)
            {
                if (channels == 1)
                {
                    for (int i = 0; i < s.Length; i++)
                    {
                        if (s[i] < 0)
                        {
                            sign = -1;
                        }
                        else if (sign < 0)
                        {
                            waveCnt++;
                            skipping = waveCnt % (skipGroup * skipFactor) < skipGroup;
                            if (!skipping)
                            {
                                return i;
                            }
                            sign = 0;
                        }
                    }
                }

                else if (channels == 2)
                {
                    for (int i = 0; i < s.Length; i += 2)
                    {
                        if (s[i] + s[i + 1] < 0)
                        {
                            sign = -1;
                        }
                        else if (sign < 0)
                        {
                            waveCnt++;
                            skipping = waveCnt % (skipGroup * skipFactor) < skipGroup;
                            if (!skipping)
                            {
                                return i / 2;
                            }
                            sign = 0;
                        }
                    }
                }

                else
                {
                    for (int i = 0; i < s.Length; i += channels)
                    {
                        var x = s[i] + s[i + 1];
                        for (int j = 2; i < channels; j++)
                        {
                            x += s[i + j];
                        }
                        if (x < 0)
                        {
                            sign = -1;
                        }
                        else if (sign < 0)
                        {
                            waveCnt++;
                            skipping = waveCnt % (skipGroup * skipFactor) < skipGroup;
                            if (!skipping)
                            {
                                return i / channels;
                            }
                            sign = 0;
                        }
                    }
                }
                return 0;
            }

            int processShort(short[] s, short[] d)
            {
                int dPos = 0;
                if (channels == 1)
                {
                    for (int i = 0; i < s.Length; i++)
                    {
                        if (s[i] < 0)
                        {
                            sign = -1;
                        }
                        else if (sign < 0)
                        {
                            waveCnt++;
                            skipping = waveCnt % (skipGroup * skipFactor) < skipGroup;
                            sign = 0;
                        }

                        if (!skipping)
                        {
                            d[dPos++] = s[i];
                        }
                    }
                }

                else if (channels == 2)
                {
                    for (int i = 0; i < s.Length; i += 2)
                    {
                        if (s[i] + s[i + 1] < 0)
                        {
                            sign = -1;
                        }
                        else if (sign < 0)
                        {
                            waveCnt++;
                            skipping = waveCnt % (skipGroup * skipFactor) < skipGroup;
                            sign = 0;
                        }

                        if (!skipping)
                        {
                            d[dPos++] = s[i];
                            d[dPos++] = s[i + 1];
                        }
                    }
                }

                else
                {
                    for (int i = 0; i < s.Length; i += channels)
                    {
                        var x = s[i] + s[i + 1];
                        for (int j = 2; i < channels; j++)
                        {
                            x += s[i + j];
                        }
                        if (x < 0)
                        {
                            sign = -1;
                        }
                        else if (sign < 0)
                        {
                            waveCnt++;
                            skipping = waveCnt % (skipGroup * skipFactor) < skipGroup;
                            sign = 0;
                        }

                        if (!skipping)
                        {
                            d[dPos++] = s[i];
                            d[dPos++] = s[i + 1];
                            for (int j = 2; i < channels; j++)
                            {
                                d[dPos++] += s[i + j];
                            }
                        }
                    }
                }

                return dPos / channels;
            }

            public int endShort(short[] s)
            {
                if (channels == 1)
                {
                    for (int i = 0; i < s.Length; i++)
                    {
                        if (s[i] < 0)
                        {
                            sign = -1;
                        }
                        else if (sign < 0)
                        {
                            waveCnt++;
                            skipping = waveCnt % (skipGroup * skipFactor) < skipGroup;
                            if (!skipping)
                            {
                                return i;
                            }
                            sign = 0;
                        }
                    }
                }

                else if (channels == 2)
                {
                    for (int i = 0; i < s.Length; i += 2)
                    {
                        if (s[i] + s[i + 1] < 0)
                        {
                            sign = -1;
                        }
                        else if (sign < 0)
                        {
                            waveCnt++;
                            skipping = waveCnt % (skipGroup * skipFactor) < skipGroup;
                            if (!skipping)
                            {
                                return i / 2;
                            }
                            sign = 0;
                        }
                    }
                }

                else
                {
                    for (int i = 0; i < s.Length; i += channels)
                    {
                        var x = s[i] + s[i + 1];
                        for (int j = 2; i < channels; j++)
                        {
                            x += s[i + j];
                        }
                        if (x < 0)
                        {
                            sign = -1;
                        }
                        else if (sign < 0)
                        {
                            waveCnt++;
                            skipping = waveCnt % (skipGroup * skipFactor) < skipGroup;
                            if (!skipping)
                            {
                                return i / channels;
                            }
                            sign = 0;
                        }
                    }
                }
                return 0;
            }
        }

        /// <summary>Sample-rate conversion Audio Processor.</summary>
        /// This processor converts the sample-rate of the source stream. Internally, it uses <see cref="AudioUtil.Resample{T}(T[], T[], int, int)"></see>.
        public class Resampler<T> : IProcessor<T>
        {
            protected T[] frameResampled;
            int channels;

            /// <summary>Create a new Resampler instance.</summary>
            /// <param name="dstSize">Frame size of a destination frame. Determins output rate.</param>
            /// <param name="channels">Number of audio channels expected in both in- and output.</param>
            public Resampler(int dstSize, int channels)
            {
                this.frameResampled = new T[dstSize];
                this.channels = channels;
            }
            public T[] Process(T[] buf)
            {
                AudioUtil.Resample(buf, this.frameResampled, this.frameResampled.Length, channels);
                return this.frameResampled;
            }
            public void Dispose()
            {
            }

        }

        /// <summary>Audio Level Metering interface.</summary>
        public interface ILevelMeter
        {
            /// <summary>
            /// Average amplitude value over last half second.
            /// </summary>
            float CurrentAvgAmp { get; }

            /// <summary>
            /// Maximum amplitude value over last half second sec.
            /// </summary>
            float CurrentPeakAmp
            {
                get;
            }

            /// <summary>
            /// Average of CurrentPeakAmps since last reset.
            /// </summary>
            float AccumAvgPeakAmp { get; }

            /// <summary>
            /// Reset <see cref="AccumAvgPeakAmp"></see>.
            /// </summary>
            void ResetAccumAvgPeakAmp();
        }

        /// <summary>Dummy Audio Level Meter that doesn't actually do anything.</summary>
        public class LevelMeterDummy : ILevelMeter
        {
            public float CurrentAvgAmp { get { return 0; } }
            public float CurrentPeakAmp { get { return 0; } }
            public float AccumAvgPeakAmp { get { return 0; } }
            public void ResetAccumAvgPeakAmp() { }
        }

        /// <summary>
        /// Audio Level Meter.
        /// </summary>
        abstract public class LevelMeter<T> : IProcessor<T>, ILevelMeter
        {
            // sum of all values in buffer
            protected float ampSum;
            // max of values from start buffer to current pos
            protected float ampPeak;
            protected int bufferSize;
            protected float[] prevValues;
            protected int prevValuesHead;

            protected float accumAvgPeakAmpSum;
            protected int accumAvgPeakAmpCount;
            protected float currentPeakAmp;
            protected float norm;

            internal LevelMeter(int samplingRate, int numChannels)
            {
                this.bufferSize = samplingRate * numChannels / 2; // 1/2 sec
                this.prevValues = new float[this.bufferSize];
            }

            public float CurrentAvgAmp { get { return ampSum / this.bufferSize * norm; } }
            public float CurrentPeakAmp
            {
                get { return currentPeakAmp * norm; }
                protected set { currentPeakAmp = value / norm; }
            }

            public float AccumAvgPeakAmp { get { return this.accumAvgPeakAmpCount == 0 ? 0 : accumAvgPeakAmpSum / this.accumAvgPeakAmpCount * norm; } }

            public void ResetAccumAvgPeakAmp() { this.accumAvgPeakAmpSum = 0; this.accumAvgPeakAmpCount = 0; ampPeak = 0; }

            public abstract T[] Process(T[] buf);

            public void Dispose()
            {
            }
        }

        /// <summary>
        /// LevelMeter specialization for float audio.
        /// </summary>
        public class LevelMeterFloat : LevelMeter<float>
        {

            /// <summary>Create new LevelMeterFloat instance.</summary>
            /// <param name="samplingRate">Sampling rate of the audio signal (in Hz).</param>
            /// <param name="numChannels">Number of channels in the audio signal.</param>
            public LevelMeterFloat(int samplingRate, int numChannels) : base(samplingRate, numChannels)
            {
                norm = 1.0f;
            }

            public override float[] Process(float[] buf)
            {
                foreach (var v in buf)
                {
                    var a = v;
                    if (a < 0)
                    {
                        a = -a;
                    }
                    ampSum = ampSum + a - this.prevValues[this.prevValuesHead];
                    this.prevValues[this.prevValuesHead] = a;

                    if (ampPeak < a)
                    {
                        ampPeak = a;
                    }
                    if (this.prevValuesHead == 0)
                    {
                        currentPeakAmp = ampPeak;
                        ampPeak = 0;
                        accumAvgPeakAmpSum += currentPeakAmp;
                        accumAvgPeakAmpCount++;
                    }
                    this.prevValuesHead = (this.prevValuesHead + 1) % this.bufferSize;
                }
                return buf;
            }
        }

        /// <summary>
        /// LevelMeter specialization for short audio.
        /// </summary>
        public class LevelMeterShort : LevelMeter<short>
        {
            /// <summary>Create new LevelMeterShort instance.</summary>
            /// <param name="samplingRate">Sampling rate of the audio signal (in Hz).</param>
            /// <param name="numChannels">Number of channels in the audio signal.</param>
            public LevelMeterShort(int samplingRate, int numChannels) : base(samplingRate, numChannels)
            {
                norm = 1.0f / short.MaxValue;
            }

            public override short[] Process(short[] buf)
            {
                foreach (var v in buf)
                {
                    var a = v;
                    if (a < 0)
                    {
                        a = (short)-a;
                    }
                    ampSum = ampSum + a - this.prevValues[this.prevValuesHead];
                    this.prevValues[this.prevValuesHead] = a;

                    if (ampPeak < a)
                    {
                        ampPeak = a;
                    }
                    if (this.prevValuesHead == 0)
                    {
                        currentPeakAmp = ampPeak;
                        ampPeak = 0;
                        accumAvgPeakAmpSum += currentPeakAmp;
                        accumAvgPeakAmpCount++;
                    }
                    this.prevValuesHead = (this.prevValuesHead + 1) % this.bufferSize;
                }
                return buf;
            }
        }

        /// <summary>Voice Activity Detector interface.</summary>
        public interface IVoiceDetector
        {
            /// <summary>If true, voice detection enabled.</summary>
            bool On { get; set; }

            /// <summary>Voice detected as soon as signal level exceeds threshold.</summary>
            float Threshold { get; set; }

            /// <summary>If true, voice detected.</summary>
            bool Detected { get; }

            /// <summary>Last time when switched to detected state.</summary>
            DateTime DetectedTime { get; }

            /// <summary>Called when switched to detected state.</summary>
            event Action OnDetected;

            /// <summary>Keep detected state during this time after signal level dropped below threshold.</summary>
            int ActivityDelayMs { get; set; }
        }

        /// <summary>Calibration Utility for Voice Detector</summary>.
        /// Using this audio processor, you can calibrate the <see cref="IVoiceDetector.Threshold"></see>.
        public class VoiceDetectorCalibration<T> : IProcessor<T>
        {
            IVoiceDetector voiceDetector;
            ILevelMeter levelMeter;
            int valuesPerSec;
            public bool IsCalibrating { get { return calibrateCount > 0; } }
            protected int calibrateCount;
            private Action<float> onCalibrated;

            /// <summary>Create new VoiceDetectorCalibration instance.</summary>
            /// <param name="voiceDetector">Voice Detector to calibrate.</param>
            /// <param name="levelMeter">Level Meter to look at for calibration.</param>
            /// <param name="samplingRate">Sampling rate of the audio signal (in Hz).</param>
            /// <param name="channels">Number of channels in the audio signal.</param>
            public VoiceDetectorCalibration(IVoiceDetector voiceDetector, ILevelMeter levelMeter, int samplingRate, int channels)
            {
                this.valuesPerSec = samplingRate * channels;
                this.voiceDetector = voiceDetector;
                this.levelMeter = levelMeter;
            }

            /// <summary>Start calibration.</summary>
            /// <param name="durationMs">Duration of the calibration procedure (in milliseconds).</param>
            /// <param name="onCalibrated">Optional callback that is called after calibration is complete.</param>
            /// <remarks>
            /// This activates the Calibration process. 
            /// It will reset the given LevelMeter's AccumAvgPeakAmp (accumulated average peak amplitude),
            /// and when the duration has passed, use it for the VoiceDetector's detection threshold.
            /// </remarks>
            public void Calibrate(int durationMs, Action<float> onCalibrated = null)
            {
                this.calibrateCount = valuesPerSec * durationMs / 1000;
                this.onCalibrated = onCalibrated;
                levelMeter.ResetAccumAvgPeakAmp();
            }
            public T[] Process(T[] buf)
            {

                if (this.calibrateCount != 0)
                {
                    this.calibrateCount -= buf.Length;
                    if (this.calibrateCount <= 0)
                    {
                        this.calibrateCount = 0;
                        this.voiceDetector.Threshold = levelMeter.AccumAvgPeakAmp * 2;
                        if (this.onCalibrated != null) this.onCalibrated(this.voiceDetector.Threshold);
                    }
                }
                return buf;
            }

            public void Dispose()
            {
            }
        }

        /// <summary>Dummy VoiceDetector that doesn't actually do anything.</summary>
        public class VoiceDetectorDummy : IVoiceDetector
        {
            public bool On { get { return false; } set { } }
            public float Threshold { get { return 0; } set { } }
            public bool Detected { get { return false; } }
            public int ActivityDelayMs { get { return 0; } set { } }
            public DateTime DetectedTime { get; private set; }

            public event Action OnDetected { add { } remove { } } // Disabling Warning CS0067 The event 'AudioUtil.VoiceDetectorDummy.OnDetected' is never used.
        }


        /// <summary>
        /// Simple voice activity detector triggered by signal level.
        /// </summary>
        abstract public class VoiceDetector<T> : IProcessor<T>, IVoiceDetector
        {
            /// <summary>If true, voice detection enabled.</summary>
            public bool On { get; set; }

            /// <summary>Voice detected as soon as signal level exceeds threshold.</summary>
            public float Threshold { get { return threshold * norm; } set { threshold = value / norm; } }

            protected float norm;
            protected float threshold;
            bool detected;

            /// <summary>If true, voice detected.</summary>
            public bool Detected
            {
                get { return detected; }
                protected set
                {
                    if (detected != value)
                    {
                        detected = value; DetectedTime = DateTime.Now;
                        if (detected && OnDetected != null) OnDetected();
                    }
                }
            }

            /// <summary>Last time when switched to detected state.</summary>
            public DateTime DetectedTime { get; private set; }

            /// <summary>Keep detected state during this time after signal level dropped below threshold.</summary>
            public int ActivityDelayMs
            {
                get { return this.activityDelay; }
                set
                {
                    this.activityDelay = value;
                    this.activityDelayValuesCount = value * valuesCountPerSec / 1000;
                }
            }

            /// <summary>Called when switched to detected state.</summary>
            public event Action OnDetected;

            protected int activityDelay;
            protected int autoSilenceCounter = 0;
            protected int valuesCountPerSec;
            protected int activityDelayValuesCount;

            internal VoiceDetector(int samplingRate, int numChannels)
            {
                this.valuesCountPerSec = samplingRate * numChannels;
                this.ActivityDelayMs = 500;
                this.On = true;
            }

            public abstract T[] Process(T[] buf);

            public void Dispose()
            {
            }
        }

        /// <summary>VoiceDetector specialization for float audio.</summary>
        public class VoiceDetectorFloat : VoiceDetector<float>
        {
            /// <summary>Create a new VoiceDetectorFloat instance.</summary>
            /// <param name="samplingRate">Sampling rate of the audio signal (in Hz).</param>
            /// <param name="numChannels">Number of channels in the audio signal.</param>
            public VoiceDetectorFloat(int samplingRate, int numChannels) : base(samplingRate, numChannels)
            {
                norm = 1f;
            }

            public override float[] Process(float[] buffer)
            {
                if (this.On)
                {
                    foreach (var s in buffer)
                    {
                        if (s > this.threshold)
                        {
                            this.Detected = true;
                            this.autoSilenceCounter = 0;
                        }
                        else
                        {
                            this.autoSilenceCounter++;
                        }
                    }
                    if (this.autoSilenceCounter > this.activityDelayValuesCount)
                    {
                        this.Detected = false;
                    }
                    return Detected ? buffer : null;
                }
                else
                {
                    return buffer;
                }
            }
        }

        /// <summary>VoiceDetector specialization for float audio.</summary>
        public class VoiceDetectorShort : VoiceDetector<short>
        {
            /// <summary>Create a new VoiceDetectorFloat instance</summary>
            /// <param name="samplingRate">Sampling rate of the audio signal (in Hz).</param>
            /// <param name="numChannels">Number of channels in the audio signal.</param>
            public VoiceDetectorShort(int samplingRate, int numChannels) : base(samplingRate, numChannels)
            {
                norm = 1.0f / short.MaxValue;
            }

            public override short[] Process(short[] buffer)
            {
                if (this.On)
                {
                    foreach (var s in buffer)
                    {
                        if (s > this.threshold)
                        {
                            this.Detected = true;
                            this.autoSilenceCounter = 0;
                        }
                        else
                        {
                            this.autoSilenceCounter++;
                        }
                    }
                    if (this.autoSilenceCounter > this.activityDelayValuesCount)
                    {
                        this.Detected = false;
                    }
                    return Detected ? buffer : null;
                }
                else
                {
                    return buffer;
                }
            }
        }

        /// <summary>Utility Audio Processor Voice Detection Calibration.</summary>
        /// Encapsulates level meter, voice detector and voice detector calibrator in single instance.
        public class VoiceLevelDetectCalibrate<T> : IProcessor<T>
        {
            /// <summary>The LevelMeter in use.</summary>
            public ILevelMeter LevelMeter { get; private set; }

            /// <summary>The VoiceDetector in use</summary>
            public IVoiceDetector VoiceDetector { get; private set; }

            /// <summary>The VoiceDetectorCalibration in use.</summary>
            VoiceDetectorCalibration<T> calibration;

            /// <summary>Create new VoiceLevelDetectCalibrate instance</summary>
            /// <param name="samplingRate">Sampling rate of the audio signal (in Hz).</param>
            /// <param name="channels">Number of channels in the audio signal.</param>
            public VoiceLevelDetectCalibrate(int samplingRate, int channels)
            {
                var x = new T[1];
                if (x[0] is float)
                {
                    LevelMeter = new LevelMeterFloat(samplingRate, channels);
                    VoiceDetector = new VoiceDetectorFloat(samplingRate, channels);
                }
                else if (x[0] is short)
                {
                    LevelMeter = new LevelMeterShort(samplingRate, channels);
                    VoiceDetector = new VoiceDetectorShort(samplingRate, channels);
                }
                else
                {
                    throw new Exception("VoiceLevelDetectCalibrate: type not supported: " + x[0].GetType());
                }
                calibration = new VoiceDetectorCalibration<T>(VoiceDetector, LevelMeter, samplingRate, channels);
            }

            /// <summary>Start calibration</summary>
            /// <param name="durationMs">Duration of the calibration procedure (in milliseconds).</param>
            /// <param name="onCalibrated">Called when calibration is complete. Parameter is new threshold value.</param>
            /// This activates the Calibration process. 
            /// It will reset the given LevelMeter's AccumAvgPeakAmp (accumulated average peak amplitude),
            /// and when the duration has passed, use it for the VoiceDetector's detection threshold.
            public void Calibrate(int durationMs, Action<float> onCalibrated = null)
            {
                calibration.Calibrate(durationMs, onCalibrated);
            }

            public bool IsCalibrating { get { return calibration.IsCalibrating; } }

            public T[] Process(T[] buf)
            {
                buf = (LevelMeter as IProcessor<T>).Process(buf);
                buf = (calibration as IProcessor<T>).Process(buf);
                buf = (VoiceDetector as IProcessor<T>).Process(buf);
                return buf;
            }

            public void Dispose()
            {
                (LevelMeter as IProcessor<T>).Dispose();
                (VoiceDetector as IProcessor<T>).Dispose();
                calibration.Dispose();
            }
        }
    }
}