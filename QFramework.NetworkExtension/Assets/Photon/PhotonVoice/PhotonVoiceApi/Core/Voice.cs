// -----------------------------------------------------------------------
// <copyright file="Voice.cs" company="Exit Games GmbH">
//   Photon Voice API Framework for Photon - Copyright (C) 2017 Exit Games GmbH
// </copyright>
// <summary>
//   Photon data streaming support.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

using System;

using System.Collections.Generic;
using System.Threading;

namespace Photon.Voice
{
    /// <summary>
    /// Interface for pulling data, in case this is more appropriate than pushing it.
    /// </summary>
    public interface IDataReader<T> : IDisposable
    {
        /// <summary>Fill full given frame buffer with source uncompressed data or return false if not enough such data.</summary>
        /// <param name="buffer">Buffer to fill.</param>
        /// <returns>True if buffer was filled successfully, false otherwise.</returns>
        bool Read(T[] buffer);
    }

    /// <summary>
    /// Interface for classes that want their Service() function to be called regularly in the context of a LocalVoice.
    /// </summary>
    public interface IServiceable
    {
        /// <summary>Service function that should be called regularly.</summary>
        void Service(LocalVoice localVoice);
    }

    public class FrameOut<T>
    {
        public FrameOut(T[] buf, bool endOfStream)
        {
            Set(buf, endOfStream);
        }
        public FrameOut<T> Set(T[] buf, bool endOfStream)
        {
            Buf = buf;
            EndOfStream = endOfStream;
            return this;
        }
        public T[] Buf { get; private set; }
        public bool EndOfStream { get; private set; } // stream interrupted but may be resumed, flush the output
    }

    /// <summary>
    /// Represents outgoing data stream.
    /// </summary>
    public class LocalVoice : IDisposable
    {
        public const int DATA_POOL_CAPACITY = 50; // TODO: may depend on data type and properties, set for average audio stream

        [Obsolete("Use InterestGroup.")]
        public byte Group { get { return InterestGroup; } set { InterestGroup = value; } }
        /// <summary>If InterestGroup != 0, voice's data is sent only to clients listening to this group (if supported by transport).</summary>
        public byte InterestGroup { get; set; }
        /// <summary>Returns Info structure assigned on local voice cration.</summary>
        public VoiceInfo Info { get { return info; } }
        /// <summary>If true, stream data broadcasted.</summary>
        public bool TransmitEnabled 
        { 
            get
            {
                return transmitEnabled;
            }
            set
            {
                if (transmitEnabled != value)
                {
                    if (transmitEnabled)
                    {
                        if (encoder != null && this.voiceClient.transport.IsChannelJoined(this.channelId))
                        {
                            encoder.EndOfStream();
                        }
                    }
                    transmitEnabled = value;
                }
            }
        }
        private bool transmitEnabled = true;

        /// <summary>Returns true if stream broadcasts.</summary>
        public bool IsCurrentlyTransmitting 
        { 
            get { return Environment.TickCount - lastTransmitTime < NO_TRANSMIT_TIMEOUT_MS; } 
        }

        /// <summary>Sent frames counter.</summary>
        public int FramesSent { get; private set; }

        /// <summary>Sent frames bytes counter.</summary>
        public int FramesSentBytes { get; private set; }

        /// <summary>Send data reliable.</summary>
        public bool Reliable { get; set; }

        /// <summary>Send data encrypted.</summary>
        public bool Encrypt { get; set; }

        /// <summary>Optional user object attached to LocalVoice. its Service() will be called at each VoiceClient.Service() call.</summary>
        public IServiceable LocalUserServiceable { get; set; }

        /// <summary>
        /// If true, outgoing stream routed back to client via server same way as for remote client's streams.
        /// Can be swithed any time. OnRemoteVoiceInfoAction and OnRemoteVoiceRemoveAction are triggered if required.
        /// This functionality availability depends on transport.
        /// </summary>
        public bool DebugEchoMode
        {
            get { return debugEchoMode; }
            set
            {
                if (debugEchoMode != value)
                {
                    debugEchoMode = value;
                    if (voiceClient != null && voiceClient.transport != null)
                    {
                        if (voiceClient.transport.IsChannelJoined(this.channelId))
                        {
                            if (debugEchoMode)
                            {
                                voiceClient.sendVoicesInfoAndConfigFrame(new List<LocalVoice>() { this }, channelId, -1);
                            }
                            else
                            {
                                voiceClient.transport.SendVoiceRemove(this, channelId, -1);
                            }
                        }

                    }
                }
            }
        }
        bool debugEchoMode;

        public void SendSpacingProfileStart()
        {
            sendSpacingProfile.Start();
        }

        public string SendSpacingProfileDump { get { return sendSpacingProfile.Dump; } }

        /// <summary>
        /// Logs input frames time spacing profiling results. Do not call frequently.
        /// </summary>
        public int SendSpacingProfileMax { get { return sendSpacingProfile.Max; } }

        public byte ID { get { return id; } }
        public byte EvNumber { get { return evNumber; } }

        #region nonpublic

        protected VoiceInfo info;
        protected IEncoder encoder;
        internal byte id;
        internal int channelId;
        internal byte evNumber = 0; // sequence used by receivers to detect loss. will overflow.
        protected VoiceClient voiceClient;
        protected ArraySegment<byte> configFrame;

        volatile protected bool disposed;
        protected object disposeLock = new object();
        internal LocalVoice() // for dummy voices
        {
        }

        internal LocalVoice(VoiceClient voiceClient, IEncoder encoder, byte id, VoiceInfo voiceInfo, int channelId)
        {
            this.info = voiceInfo;
            this.channelId = channelId;
            this.voiceClient = voiceClient;
            this.id = id;
            if (encoder == null)
            {
                var m = LogPrefix + ": encoder is null";
                voiceClient.logger.LogError(m);
                throw new ArgumentNullException("encoder");
            }
            this.encoder = encoder;
            this.encoder.Output = sendFrame;
        }

        protected string shortName { get { return "v#" + id + "ch#" + voiceClient.channelStr(channelId); } }

        public string Name { get { return "Local " + info.Codec + " v#" + id + " ch#" + voiceClient.channelStr(channelId); } }
        public string LogPrefix { get { return "[PV] " + Name; } }

        private const int NO_TRANSMIT_TIMEOUT_MS = 100; // should be greater than SendFrame() call interval
        private int lastTransmitTime = Environment.TickCount - NO_TRANSMIT_TIMEOUT_MS;
        
        internal virtual void service()
        {
            while (true)
            {
                FrameFlags f;
                var x = encoder.DequeueOutput(out f);
                if (x.Count == 0)
                {
                    break;
                }
                else
                {
                    sendFrame(x, f);
                }
            }
            
            if (LocalUserServiceable != null)
            {
                LocalUserServiceable.Service(this);
            }
        }

        internal void sendConfigFrame(int targetPlayerId)
        {
            if (configFrame.Count != 0)
            {
                this.voiceClient.logger.LogInfo(LogPrefix + " Sending config frame to pl " + targetPlayerId);
                sendFrame0(configFrame, FrameFlags.Config, targetPlayerId, true);
            }
        }

        internal void sendFrame(ArraySegment<byte> compressed, FrameFlags flags)
        {
            if ((flags & FrameFlags.Config) != 0)
            {
                byte[] a = configFrame.Array != null && configFrame.Array.Length >= compressed.Count ? configFrame.Array : new byte[compressed.Count];
                Buffer.BlockCopy(compressed.Array, compressed.Offset, a, 0, compressed.Count);
                configFrame = new ArraySegment<byte>(a, 0, compressed.Count);

                this.voiceClient.logger.LogInfo(LogPrefix + " Got config frame " + configFrame.Count + " bytes");
            }
            if (this.voiceClient.transport.IsChannelJoined(this.channelId) && this.TransmitEnabled)
            {
                sendFrame0(compressed, flags, 0, Reliable);
            }
        }

        internal void sendFrame0(ArraySegment<byte> compressed, FrameFlags flags, int targetPlayerId, bool reliable)
        {
            if ((flags & FrameFlags.Config) != 0)
            {
                reliable = true;
            }
            if ((flags & FrameFlags.KeyFrame) != 0)
            {
                reliable = true;
            }
            // sending reliably breaks timing
            // consider sending multiple EndOfStream packets for reliability
            if ((flags & FrameFlags.EndOfStream) != 0)
            {
                //                reliable = true;
            }

            this.FramesSent++;
            this.FramesSentBytes += compressed.Count;

            this.voiceClient.transport.SendFrame(compressed, flags, evNumber, id, this.channelId, targetPlayerId, reliable, this);
            this.sendSpacingProfile.Update(false, false);
            if (this.DebugEchoMode)
            {
                this.eventTimestamps[evNumber] = Environment.TickCount;
            }
            evNumber++;            

            if (compressed.Count > 0 && (flags & FrameFlags.Config) == 0) // otherwise the frame is config or control (EOS)
            {
                lastTransmitTime = Environment.TickCount;
            }
        }

        internal Dictionary<byte, int> eventTimestamps = new Dictionary<byte, int>();

        SpacingProfile sendSpacingProfile = new SpacingProfile(1000);
        #endregion

        /// <summary>Remove this voice from it's VoiceClient (using VoiceClient.RemoveLocalVoice</summary>
        public void RemoveSelf()
        {
            if (this.voiceClient != null) // dummy voice can try to remove self
            {
                this.voiceClient.RemoveLocalVoice(this);
            }
        }

        public virtual void Dispose()
        {
            if (!disposed)
            {
                if (this.encoder != null)
                {
                    this.encoder.Dispose();
                }
                disposed = true;
            }
        }
    }

    /// <summary>Event Actions and other options for a remote voice (incoming stream).</summary>
    public struct RemoteVoiceOptions
    {
        public RemoteVoiceOptions(ILogger logger, string logPrefix, VoiceInfo voiceInfo)
        {
            this.logger = logger;
            this.logPrefix = logPrefix;
            this.voiceInfo = voiceInfo;
            this.Decoder = null;
            this.OnRemoteVoiceRemoveAction = null;
        }

		/// <summary>
        /// Create default audio decoder and register a method to be called when a data frame is decoded.
        /// </summary>
        public void SetOutput(Action<FrameOut<float>> output)
        {
            if (voiceInfo.Codec == Codec.Raw) // Debug only. Assumes that original data is short[].
            {
                this.Decoder = new RawCodec.Decoder<short>(new RawCodec.ShortToFloat(output as Action<FrameOut<float>>).Output);
                return;
            }
            setOutput<float>(output);
        }

        /// <summary>
        /// Create default audio decoder and register a method to be called when a data frame is decoded.
        /// </summary>
        public void SetOutput(Action<FrameOut<short>> output)
        {
            if (voiceInfo.Codec == Codec.Raw) // Debug only. Assumes that original data is short[].
            {
                this.Decoder = new RawCodec.Decoder<short>(output);
                return;
            }
            setOutput<short>(output);
        }
        
        private void setOutput<T>(Action<FrameOut<T>> output)
        {
            logger.LogInfo(logPrefix + ": Creating default decoder " + voiceInfo.Codec + " for output FrameOut<" + typeof(T) + ">");
            if (voiceInfo.Codec == Codec.AudioOpus)
            {
                this.Decoder = new OpusCodec.Decoder<T>(output, logger);
            }
            else
            {
                logger.LogError(logPrefix + ": FrameOut<" + typeof(T) + "> output set for non-audio decoder " + voiceInfo.Codec);
            }
        }

        /// <summary>
        /// Register a method to be called when the remote voice is removed.
        /// </summary>
        public Action OnRemoteVoiceRemoveAction { get; set; }

        /// <summary>Remote voice data decoder. Use to set decoder options or override it with user decoder.</summary>
        public IDecoder Decoder { get; set; }

        private readonly ILogger logger;
        private readonly VoiceInfo voiceInfo;
        internal string logPrefix { get; }

    }

    internal class RemoteVoice : IDisposable
    {
        // Client.RemoteVoiceInfos support
        internal VoiceInfo Info { get; private set; }
        internal RemoteVoiceOptions options;
        internal int channelId;
        internal int DelayFrames { get; set; }
        private int playerId;
        private byte voiceId;
        volatile private bool disposed;
        object disposeLock = new object();

        internal RemoteVoice(VoiceClient client, RemoteVoiceOptions options, int channelId, int playerId, byte voiceId, VoiceInfo info, byte lastEventNumber)
        {
            this.options = options;
            this.LogPrefix = options.logPrefix;
            this.voiceClient = client;
            this.channelId = channelId;
            this.playerId = playerId;
            this.voiceId = voiceId;
            this.Info = info;
            this.lastEvNumber = lastEventNumber;

            if (this.options.Decoder == null)
            {
                var m = LogPrefix + ": decoder is null (set it with options Decoder property or SetOutput method in OnRemoteVoiceInfoAction)";
                voiceClient.logger.LogError(m);
                disposed = true;
                return;
            }

#if PHOTON_VOICE_THREADING_DISABLE
            voiceClient.logger.LogInfo(LogPrefix + ": Starting decode singlethreaded");
            options.Decoder.Open(Info);
#else
#if NETFX_CORE
            Windows.System.Threading.ThreadPool.RunAsync((x) =>
            {
                decodeThread();
            });
#else
            var t = new Thread(() => decodeThread());
            Util.SetThreadName(t, "[PV] Dec" + shortName);
            t.Start();
#endif
#endif
        }
        private string shortName { get { return "v#" + voiceId + "ch#" + voiceClient.channelStr(channelId) + "p#" + playerId; } }
        public string LogPrefix { get; private set; }
       
        SpacingProfile receiveSpacingProfile = new SpacingProfile(1000);
        
        /// <summary>
        /// Starts input frames time spacing profiling. Once started, it can't be stopped.
        /// </summary>
        public void ReceiveSpacingProfileStart()
        {
            receiveSpacingProfile.Start();
        }

        public string ReceiveSpacingProfileDump { get { return receiveSpacingProfile.Dump; } }

        /// <summary>
        /// Logs input frames time spacing profiling results. Do not call frequently.
        /// </summary>
        public int ReceiveSpacingProfileMax { get { return receiveSpacingProfile.Max; } }

        internal byte lastEvNumber = 0;
        private VoiceClient voiceClient;

        private static byte byteDiff(byte latest, byte last)
        {
            return (byte)(latest - (last + 1));
        }

        internal void receiveBytes(ref FrameBuffer receivedBytes, byte evNumber)
        {            
            // receive-gap detection and compensation
            if (evNumber != this.lastEvNumber) // skip check for 1st event 
            {
                int missing = byteDiff(evNumber, this.lastEvNumber);
                if (missing == 0)
                {
                    this.lastEvNumber = evNumber;
                }
                else if (missing < 127)
                {
                    this.voiceClient.logger.LogWarning(LogPrefix + " evNumer: " + evNumber + " playerVoice.lastEvNumber: " + this.lastEvNumber + " missing: " + missing + " r/b " + receivedBytes.Length);
                    this.voiceClient.FramesLost += missing;
                    this.lastEvNumber = evNumber;
                    // restoring missing frames
                    receiveNullFrames(missing);
                } else {
                    // late (out of order) frames, just ignore them
                    // these frames already counted in FramesLost
                    this.voiceClient.logger.LogWarning(LogPrefix + " evNumer: " + evNumber + " playerVoice.lastEvNumber: " + this.lastEvNumber + " late: " + (255 - missing) + " r/b " + receivedBytes.Length);
                }
            }
            this.receiveFrame(ref receivedBytes);
        }

        Queue<FrameBuffer> frameQueue = new Queue<FrameBuffer>();
        AutoResetEvent frameQueueReady = new AutoResetEvent(false);
        int flushingFramePosInQueue = -1; // if >= 0, we are flushing since the frame at this (dynamic) position got into the queue: process the queue w/o delays until this frame encountered
        FrameBuffer nullFrame = new FrameBuffer();

        void receiveFrame(ref FrameBuffer frame)
        {
#if PHOTON_VOICE_THREADING_DISABLE
            if (disposed) return;

            options.Decoder.Input(ref frame);
            frame.Release();
#else
            lock (disposeLock) // sync with Dispose and decodeThread 'finally'
            {
                if (disposed) return;

                receiveSpacingProfile.Update(false, (frame.Flags & FrameFlags.EndOfStream) != 0);
                lock (frameQueue)
                {
                    frameQueue.Enqueue(frame);
                    frame.Retain();
                    if ((frame.Flags & FrameFlags.EndOfStream) != 0)
                    {
                        flushingFramePosInQueue = frameQueue.Count - 1;
                    }
                }
                frameQueueReady.Set();
            }
#endif
        }

        void receiveNullFrames(int count)
        {
            lock (disposeLock) // sync with Dispose and decodeThread 'finally'
            {
                if (disposed) return;

                
                for (int i = 0; i < count; i++)
                {
                    receiveSpacingProfile.Update(true, false);
                    lock (frameQueue)
                    {
                        frameQueue.Enqueue(nullFrame);
                    }
                }
                frameQueueReady.Set();
            }
        }

        void decodeThread()
        {
            //#if UNITY_5_3_OR_NEWER
            //            UnityEngine.Profiling.Profiler.BeginThreadProfiling("PhotonVoice", LogPrefix);
            //#endif

            voiceClient.logger.LogInfo(LogPrefix + ": Starting decode thread");

            var decoder = this.options.Decoder;

            try
            {
#if UNITY_ANDROID
                UnityEngine.AndroidJNI.AttachCurrentThread();
#endif
                decoder.Open(Info);

                while (!disposed)
                {
                    frameQueueReady.WaitOne(); // Wait until data is pushed to the queue or Dispose signals.

//#if UNITY_5_3_OR_NEWER
//                    UnityEngine.Profiling.Profiler.BeginSample("Decoder");
//#endif

                    while (true) // Dequeue and process while the queue is not empty
                    {
                        if (disposed) break; // early exit to save few resources

                        FrameBuffer f;
                        bool haveFrame = false;
                        lock (frameQueue)
                        {
                            var df = 0;
                            // if flushing, process all frames in the queue
                            // otherwise keep the queue length equal DelayFrames, also check DelayFrames for validity                            
                            if (flushingFramePosInQueue < 0 && DelayFrames > 0 && DelayFrames < 300) // 10 sec. of video or max 3 sec. audio
                            {
                                df = DelayFrames;
                            }
                            if (frameQueue.Count > df)
                            {
                                f = frameQueue.Dequeue();
                                flushingFramePosInQueue--; // -1 if f is flushing frame (f.Flags == FrameFlags.EndOfStream), the next frame will be processed with delay

                                // leave it decrementing to have an idea when the last flush was triggered
                                // but avoid overflow which will happen in 248.5 days for 100 input frames per sec
                                if (flushingFramePosInQueue == Int32.MinValue)
                                {
                                    flushingFramePosInQueue = -1;
                                }
                                haveFrame = true;
                            }
                            else
                            {
                                break;
                            }
                        }
                        if (haveFrame)
                        {
                            decoder.Input(ref f);
                            f.Release();
                        }                        
                    }

                    //#if UNITY_5_3_OR_NEWER
                    //                    UnityEngine.Profiling.Profiler.EndSample();
                    //#endif

                }
            }
            catch (Exception e)
            {
                voiceClient.logger.LogError(LogPrefix + ": Exception in decode thread: " + e);
                throw e;
            }
            finally
            {
                lock (disposeLock) // sync with receiveFrame/receiveNullFrames
                {
                    disposed = true; // set to disposing state if exiting due to exception
                }
                // cleaning up being sure that fields are not updated anymore
#if NETFX_CORE
                frameQueueReady.Dispose();
#else
                frameQueueReady.Close();
#endif
                lock (frameQueue)
                {
                    while (frameQueue.Count > 0)
                    {
                        frameQueue.Dequeue().Release();
                    }
                }
                decoder.Dispose();
#if UNITY_ANDROID
                UnityEngine.AndroidJNI.DetachCurrentThread();
#endif
                voiceClient.logger.LogInfo(LogPrefix + ": Exiting decode thread");

//#if UNITY_5_3_OR_NEWER
//                UnityEngine.Profiling.Profiler.EndThreadProfiling();
//#endif

            }
        }

        internal void removeAndDispose()
        {
            if (options.OnRemoteVoiceRemoveAction != null)
            {
                options.OnRemoteVoiceRemoveAction();
            }
            Dispose();
        }

        public void Dispose()
        {
#if PHOTON_VOICE_THREADING_DISABLE
            if (options.Decoder != null)
            {
                disposed = true;
                options.Decoder.Dispose();
            }
#else
            lock (disposeLock) // sync with receiveFrame/receiveNullFrames
            {
                if (!disposed)
                {
                    disposed = true;
                    frameQueueReady.Set(); // let decodeThread dispose resporces and exit
                }
            }
#endif
        }
    }
}