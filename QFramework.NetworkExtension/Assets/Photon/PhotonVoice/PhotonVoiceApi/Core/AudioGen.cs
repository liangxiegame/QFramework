using System;

#if NETFX_CORE
using Windows.UI.Xaml;
using TimeObject = System.Object;
#else
using TimeObject = System.Timers.ElapsedEventArgs;
#endif

namespace Photon.Voice
{
    public static partial class AudioUtil
    {
        /// <summary>IAudioReader that provides a constant tone signal.</summary>
        /// Because of current resampling algorithm, the tone is distorted if SamplingRate does not equal encoder sampling rate.
        public class ToneAudioReader<T> : IAudioReader<T>
        {
            /// <summary>Create a new ToneAudioReader instance</summary>
            /// <param name="clockSec">Function to get current time in seconds. In Unity, pass in '() => AudioSettings.dspTime' for better results.</param>
            /// <param name="frequency">Frequency of the generated tone (in Hz).</param>
            /// <param name="samplingRate">Sampling rate of the audio signal (in Hz).</param>
            /// <param name="channels">Number of channels in the audio signal.</param>
            public ToneAudioReader(Func<double> clockSec = null, double frequency = 440, int samplingRate = 48000, int channels = 2)
            {                
                this.clockSec = clockSec == null ? () => DateTime.Now.Ticks / 10000000.0 : clockSec;
                this.samplingRate = samplingRate;
                this.channels = channels;
                k = 2 * Math.PI * frequency / SamplingRate;
            }

            /// <summary>Number of channels in the audio signal.</summary>
            public int Channels { get { return channels; } }

            /// <summary>Sampling rate of the audio signal (in Hz).</summary>
            public int SamplingRate { get { return samplingRate; } }
            /// <summary>If not null, audio object is in invalid state.</summary>
            public string Error { get; private set; }
            
            public void Dispose()
            {
            }
            double k;
            long timeSamples;
            Func<double> clockSec;
            int samplingRate;
            int channels;
            public bool Read(T[] buf)
            {
                var bufSamples = buf.Length / Channels;
                var t = (long)(clockSec() * SamplingRate);

                var deltaTimeSamples = t - timeSamples;
                if (Math.Abs(deltaTimeSamples) > SamplingRate / 4) // when started or Read has not been called for a while
                {
                    deltaTimeSamples = bufSamples;
                    timeSamples = t - bufSamples;
                }

                if (deltaTimeSamples < bufSamples)
                {
                    return false;
                }
                else
                {
                    int x = 0;

                    if (buf is float[])
                    {
                        for (int i = 0; i < bufSamples; i++)
                        {
                            var b = buf as float[];
                            var v = (float)(System.Math.Sin(timeSamples++ * k) * 0.2f);
                            for (int j = 0; j < Channels; j++)
                                b[x++] = v;
                        }
                    }
                    else if (buf is short[])
                    {
                        var b = buf as short[];
                        for (int i = 0; i < bufSamples; i++)
                        {
                            var v = (short)(System.Math.Sin(timeSamples++ * k) * (0.2f * short.MaxValue));
                            for (int j = 0; j < Channels; j++)
                                b[x++] = v;
                        }
                    }
                    return true;
                }
            }
        }

        /// <summary>IAudioPusher that provides a constant tone signal.</summary>
        // Helpful for debug but does not compile for UWP because of System.Timers.Timer.
        public class ToneAudioPusher<T> : IAudioPusher<T>
        {
            /// <summary>Create a new ToneAudioReader instance</summary>
            /// <param name="frequency">Frequency of the generated tone (in Hz).</param>
            /// <param name="bufSizeMs">Size of buffers to push (in milliseconds).</param>
            /// <param name="samplingRate">Sampling rate of the audio signal (in Hz).</param>
            /// <param name="channels">Number of channels in the audio signal.</param>
            public ToneAudioPusher(int frequency = 440, int bufSizeMs = 100, int samplingRate = 48000, int channels = 2)
            {
                this.samplingRate = samplingRate;
                this.channels = channels;
                this.bufSizeSamples = bufSizeMs * SamplingRate / 1000;
                k = 2 * Math.PI * frequency/ SamplingRate;                
            }
            double k;
#if NETFX_CORE
            DispatcherTimer timer;
#else
            System.Timers.Timer timer;
#endif
            Action<T[]> callback;
            ObjectFactory<T[], int> bufferFactory;

            /// <summary>Set the callback function used for pushing data</summary>
            /// <param name="callback">Callback function to use</param>
            /// <param name="bufferFactory">Buffer factory used to create the buffer that is pushed to the callback</param>
            public void SetCallback(Action<T[]> callback, ObjectFactory<T[], int> bufferFactory)
            {
                if (timer != null)
                {
                    Dispose();
                }

                this.callback = callback;
                this.bufferFactory = bufferFactory;
                // Hook up the Elapsed event for the timer.
#if NETFX_CORE
                timer = new DispatcherTimer();
                timer.Tick += OnTimedEvent;
                timer.Interval = new TimeSpan(10000000 * bufSizeSamples / SamplingRate); // ticks (10 000 000 per sec) in single buffer
#else
                timer = new System.Timers.Timer(1000.0 * bufSizeSamples / SamplingRate);
                timer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedEvent);
                timer.Enabled = true;
#endif
            }

            private void OnTimedEvent(object source, TimeObject e)
            {
                var buf = bufferFactory.New(bufSizeSamples * Channels);
                int x = 0;                
                if (buf is float[])
                {
                    var b = buf as float[];
                    for (int i = 0; i < bufSizeSamples; i++)
                    {
                        var v = (float)(System.Math.Sin((posSamples + i) * k) / 2);
                        for (int j = 0; j < Channels; j++)
                            b[x++] = v;
                    }
                }
                else if (buf is short[])
                {
                    var b = buf as short[];
                    for (int i = 0; i < bufSizeSamples; i++)
                    {
                        var v = (short)(System.Math.Sin((posSamples + i) * k) * short.MaxValue / 2);
                        for (int j = 0; j < Channels; j++)
                            b[x++] = v;
                    }
                }

                cntFrame++;
                posSamples += bufSizeSamples;
                this.callback(buf);
            }
            int cntFrame;
            int posSamples;
            int bufSizeSamples;
            int samplingRate;
            int channels;

            public int Channels { get { return channels; } }
            public int SamplingRate { get { return samplingRate; } }
            public string Error { get; private set; }

            public void Dispose()
            {
                if (timer != null)
                {
#if NETFX_CORE
                    timer.Stop();
#else
                    timer.Close();
#endif
                }
            }
        }
    }
}