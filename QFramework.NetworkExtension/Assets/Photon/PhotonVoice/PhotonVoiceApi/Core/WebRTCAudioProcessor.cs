#if (UNITY_IOS && !UNITY_EDITOR) || __IOS__
#define DLL_IMPORT_INTERNAL
#endif

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace Photon.Voice
{
    public class WebRTCAudioProcessor : WebRTCAudioLib, IProcessor<short>
    {
        const int REVERSE_BUFFER_POOL_CAPACITY = 50;

        int reverseStreamDelayMs;
        bool aec = false;
        bool aecHighPass = true;
        bool aecm = false;
        bool highPass = false;
        bool ns = false;
        bool agc = true;
        int agcCompressionGain = 9;
        int agcTargetLevel = 3;
        bool agc2 = false;
        bool vad;

        bool reverseStreamThreadRunning = false;
        Queue<short[]> reverseStreamQueue = new Queue<short[]>();
        AutoResetEvent reverseStreamQueueReady = new AutoResetEvent(false);
        FactoryPrimitiveArrayPool<short> reverseBufferFactory;

        public int AECStreamDelayMs { set { if (reverseStreamDelayMs != value) { reverseStreamDelayMs = value; if (proc != IntPtr.Zero) setParam(Param.REVERSE_STREAM_DELAY_MS, value); } } }
        public bool AEC
        {
            set
            {
                if (aec != value)
                {
                    aec = value;
                    InitReverseStream();
                    if (proc != IntPtr.Zero) setParam(Param.AEC, aec ? 1 : 0);
                    aecm = aec ? false : aecm;
                }
            }
        }

        public bool AECHighPass { set { if (aecHighPass != value) { aecHighPass = value; if (proc != IntPtr.Zero) setParam(Param.AEC_HIGH_PASS_FILTER, value ? 1 : 0); } } }

        public bool AECMobile
        {
            set
            {
                if (aecm != value)
                {
                    aecm = value;
                    InitReverseStream();
                    if (proc != IntPtr.Zero) setParam(Param.AECM, aecm ? 1 : 0);
                    aec = aecm ? false : aec;
                }
            }
        }

        public bool HighPass { set { if (highPass != value) { highPass = value; if (proc != IntPtr.Zero) setParam(Param.HIGH_PASS_FILTER, value ? 1 : 0); } } }
        public bool NoiseSuppression { set { if (ns != value) { ns = value; if (proc != IntPtr.Zero) setParam(Param.NS, value ? 1 : 0); } } }
        public bool AGC { set { if (agc != value) { agc = value; if (proc != IntPtr.Zero) setParam(Param.AGC, value ? 1 : 0); } } }
        public int AGCCompressionGain
        {
            set
            {
                if (agcCompressionGain != value)
                {
                    if (value < 0 || value > 90)
                    {
                        logger.LogError("[PV] WebRTCAudioProcessor: new AGCCompressionGain value {0} not in range [0..90]", value);
                    }
                    else 
                    { 
                        agcCompressionGain = value;
                        if (proc != IntPtr.Zero)
                        {
                            setParam(Param.AGC_COMPRESSION_GAIN, value);
                        }
                    }
                }
            }
        }
        public int AGCTargetLevel
        {
            set
            {
                if (agcTargetLevel != value)
                {
                    if (value > 31 || value < 0)
                    {
                        logger.LogError("[PV] WebRTCAudioProcessor: new AGCTargetLevel value {0} not in range [0..31]", value);
                    }
                    else
                    {
                        agcTargetLevel = value;
                        if (proc != IntPtr.Zero)
                            setParam(Param.AGC_TARGET_LEVEL_DBFS, value);
                    }
                }
            }
        }
        public bool AGC2 { set { if (agc2 != value) { agc2 = value; if (proc != IntPtr.Zero) setParam(Param.AGC2, value ? 1 : 0); } } }
        public bool VAD { set { if (vad != value) { vad = value; if (proc != IntPtr.Zero) setParam(Param.VAD, value ? 1 : 0); } } }
        public bool Bypass
        {
            set
            {
                if (bypass != value) logger.LogInfo("[PV] WebRTCAudioProcessor: setting bypass=" + value);
                bypass = value;
            }
            private get { return bypass; }
        }
        bool bypass = false;

        int inFrameSize; // frames passed to Process
        int processFrameSize; // frames passed to webrtc_audio_processor_process
        int samplingRate; // input sampling rate (the same for Process and webrtc_audio_processor_process)
        int channels;
        IntPtr proc;
        bool disposed;

        Framer<float> reverseFramer;
        int reverseSamplingRate;
        int reverseChannels;
        ILogger logger;

        // audio parameters supported by webrtc
        const int supportedFrameLenMs = 10;
        public static readonly int[] SupportedSamplingRates = { 8000, 16000, 32000, 48000 };

        public WebRTCAudioProcessor(ILogger logger, int frameSize, int samplingRate, int channels, int reverseSamplingRate, int reverseChannels)
        {
            bool ok = false;
            foreach (var s in SupportedSamplingRates)
            {
                if (samplingRate == s)
                {
                    ok = true;
                    break;
                }
            }
            if (!ok)
            {
                logger.LogError("[PV] WebRTCAudioProcessor: input sampling rate ({0}) must be 8000, 16000, 32000 or 48000", samplingRate);
                disposed = true;
                return;
            }

            this.logger = logger;
            this.inFrameSize = frameSize;
            this.processFrameSize = samplingRate * supportedFrameLenMs / 1000;
            if (this.inFrameSize / this.processFrameSize * this.processFrameSize != this.inFrameSize)
            {
                logger.LogError("[PV] WebRTCAudioProcessor: input frame size ({0} samples / {1} ms) must be equal to or N times more than webrtc processing frame size ({2} samples / 10 ms)", this.inFrameSize, 1000f * this.inFrameSize / samplingRate, processFrameSize);
                disposed = true;
                return;
            }
            this.samplingRate = samplingRate;
            this.channels = channels;
            this.reverseSamplingRate = reverseSamplingRate;
            this.reverseChannels = reverseChannels;
            this.proc = webrtc_audio_processor_create(samplingRate, channels, this.processFrameSize, samplingRate /* reverseSamplingRate to be converted */, reverseChannels);
            webrtc_audio_processor_init(this.proc);
            logger.LogInfo("[PV] WebRTCAudioProcessor create sampling rate {0}, channels{1}, frame size {2}, frame samples {3}, reverseChannels {4}", samplingRate, channels, this.processFrameSize, this.inFrameSize / this.channels, this.reverseChannels);
        }

        bool aecInited;
        private void InitReverseStream()
        {
            lock (this)
            {
                if (!aecInited)
                {
                    if (disposed)
                    {
                        return;
                    }

                    int size = processFrameSize * reverseSamplingRate / samplingRate * reverseChannels;
                    reverseFramer = new Framer<float>(size);
                    reverseBufferFactory = new FactoryPrimitiveArrayPool<short>(REVERSE_BUFFER_POOL_CAPACITY, "WebRTCAudioProcessor Reverse Buffers", this.inFrameSize);

                    logger.LogInfo("[PV] WebRTCAudioProcessor Init reverse stream: frame size {0}, reverseSamplingRate {1}, reverseChannels {2}", size, reverseSamplingRate, reverseChannels);

                    if (!reverseStreamThreadRunning)
                    {
#if NETFX_CORE
                        Windows.System.Threading.ThreadPool.RunAsync((x) =>
                        {
                            ReverseStreamThread();
                        });
#else
                        var t = new Thread(ReverseStreamThread);
                        t.Start();
                        Util.SetThreadName(t, "[PV] WebRTCProcRevStream");
#endif
                    }

                    if (reverseSamplingRate != samplingRate)
                    {
                        logger.LogWarning("[PV] WebRTCAudioProcessor AEC: output sampling rate {0} != {1} capture sampling rate. For better AEC, set audio source (microphone) and audio output samping rates to the same value.", reverseSamplingRate, samplingRate);
                    }
                    aecInited = true;
                }
            }
        }

        public short[] Process(short[] buf)
        {
            if (Bypass) return buf;
            if (disposed) return buf;
            if (proc == IntPtr.Zero) return buf;

            if (buf.Length != this.inFrameSize)
            {
                this.logger.LogError("[PV] WebRTCAudioProcessor Process: frame size expected: {0}, passed: {1}", this.inFrameSize, buf);
                return buf;
            }
            bool voiceDetected = false;
            for (int offset = 0; offset < inFrameSize; offset += processFrameSize)
            {
                bool vd = true;
                int err = webrtc_audio_processor_process(proc, buf, offset, out vd);
                if (vd)
                    voiceDetected = true;
                if (lastProcessErr != err)
                {
                    lastProcessErr = err;
                    this.logger.LogError("[PV] WebRTCAudioProcessor Process: webrtc_audio_processor_process() error {0}", err);
                    return buf;
                }
            }
            if (vad && !voiceDetected)
            {
                return null;
            }
            else
            {
                return buf;
            }

        }
        int lastProcessErr = 0;
        int lastProcessReverseErr = 0;

        public void OnAudioOutFrameFloat(float[] data)
        {
            if (disposed) return;
            if (!aecInited) return;
            if (proc == IntPtr.Zero) return;
            foreach (var reverseBufFloat in reverseFramer.Frame(data))
            {
                var reverseBuf = reverseBufferFactory.New();
                if (reverseBufFloat.Length != reverseBuf.Length)
                {
                    AudioUtil.ResampleAndConvert(reverseBufFloat, reverseBuf, reverseBuf.Length, this.reverseChannels);
                }
                else
                {
                    AudioUtil.Convert(reverseBufFloat, reverseBuf, reverseBuf.Length);
                }

                lock (reverseStreamQueue)
                {
                    if (reverseStreamQueue.Count < REVERSE_BUFFER_POOL_CAPACITY - 1)
                    {
                        reverseStreamQueue.Enqueue(reverseBuf);
                        reverseStreamQueueReady.Set();
                    }
                    else
                    {
                        this.logger.LogError("[PV] WebRTCAudioProcessor Reverse stream queue overflow");
                        this.reverseBufferFactory.Free(reverseBuf);
                    }
                }
            }
        }

        private void ReverseStreamThread()
        {
            logger.LogInfo("[PV] WebRTCAudioProcessor: Starting reverse stream thread");
            reverseStreamThreadRunning = true;
            try
            {
                while (!disposed)
                {
                    reverseStreamQueueReady.WaitOne(); // Wait until data is pushed to the queue or Dispose signals.

                    //#if UNITY_5_3_OR_NEWER
                    //                    UnityEngine.Profiling.Profiler.BeginSample("Encoder");
                    //#endif

                    while (true) // Dequeue and process while the queue is not empty
                    {
                        short[] reverseBuf = null;
                        lock (reverseStreamQueue)
                        {
                            if (reverseStreamQueue.Count > 0)
                            {
                                reverseBuf = reverseStreamQueue.Dequeue();
                            }
                        }
                        if (reverseBuf != null)
                        {
                            int err = webrtc_audio_processor_process_reverse(proc, reverseBuf, reverseBuf.Length);
                            this.reverseBufferFactory.Free(reverseBuf);
                            if (lastProcessReverseErr != err)
                            {
                                lastProcessReverseErr = err;
                                this.logger.LogError("[PV] WebRTCAudioProcessor: OnAudioOutFrameFloat: webrtc_audio_processor_process_reverse() error {0}", err);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                this.logger.LogError("[PV] WebRTCAudioProcessor: ReverseStreamThread Exceptions: " + e);
            }
            finally
            {
                logger.LogInfo("[PV] WebRTCAudioProcessor: Exiting reverse stream thread");
                reverseStreamThreadRunning = false;
            }
        }

        private int setParam(Param param, int v)
        {
            if (disposed) return 0;
            logger.LogInfo("[PV] WebRTCAudioProcessor: setting param " + param + "=" + v);
            return webrtc_audio_processor_set_param(proc, (int)param, v);
        }

        public void Dispose()
        {
            lock (this)
            {
                if (!disposed)
                {
                    disposed = true;
                    logger.LogInfo("[PV] WebRTCAudioProcessor: destroying...");
                    reverseStreamQueueReady.Set();
                    if (proc != IntPtr.Zero)
                    {
                        while (reverseStreamThreadRunning)
                        {
#if WINDOWS_UWP || ENABLE_WINMD_SUPPORT
                            System.Threading.Tasks.Task.Delay(1).Wait();
#else
                            Thread.Sleep(1);
#endif
                        }
                        webrtc_audio_processor_destroy(proc);
                        logger.LogInfo("[PV] WebRTCAudioProcessor: destroyed");
                    }
                }
            }
        }
    }

    public class WebRTCAudioLib
    {
#if DLL_IMPORT_INTERNAL
        const string lib_name = "__Internal";
#else
        const string lib_name = "webrtc-audio";
#endif

        [DllImport(lib_name, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr webrtc_audio_processor_create(int samplingRate, int channels, int frameSize, int revSamplingRate, int revChannels);
        [DllImport(lib_name, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int webrtc_audio_processor_init(IntPtr proc);
        [DllImport(lib_name, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int webrtc_audio_processor_set_param(IntPtr proc, int param, int v);
        [DllImport(lib_name, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int webrtc_audio_processor_process(IntPtr proc, short[] buffer, int offset, out bool voiceDetected);
        [DllImport(lib_name, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int webrtc_audio_processor_process_reverse(IntPtr proc, short[] buffer, int bufferSize);
        [DllImport(lib_name, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern void webrtc_audio_processor_destroy(IntPtr proc);

        // library methods return webrtc error codes
        public enum Error
        {
            // Fatal errors.
            kNoError = 0,
            kUnspecifiedError = -1,
            kCreationFailedError = -2,
            kUnsupportedComponentError = -3,
            kUnsupportedFunctionError = -4,
            kNullPointerError = -5,
            kBadParameterError = -6,
            kBadSampleRateError = -7,
            kBadDataLengthError = -8,
            kBadNumberChannelsError = -9,
            kFileError = -10,
            kStreamParameterNotSetError = -11,
            kNotEnabledError = -12,

            // Warnings are non-fatal.
            // This results when a set_stream_ parameter is out of range. Processing
            // will continue, but the parameter may have been truncated.
            kBadStreamParameterWarning = -13
        };

        public enum Param
        {
            REVERSE_STREAM_DELAY_MS = 1,

            AEC = 10,
            AEC_HIGH_PASS_FILTER = 11,

            AECM = 20,

            HIGH_PASS_FILTER = 31,

            NS = 41,
            NS_LEVEL = 42,

            AGC = 51,
//            AGC_MODE = 52,
            AGC_TARGET_LEVEL_DBFS = 55,
            AGC_COMPRESSION_GAIN = 56,
            AGC_LIMITER = 57,

            VAD = 61,
            VAD_FRAME_SIZE_MS = 62,
            VAD_LIKELIHOOD = 63,

            AGC2 = 71,
        }
    }
}