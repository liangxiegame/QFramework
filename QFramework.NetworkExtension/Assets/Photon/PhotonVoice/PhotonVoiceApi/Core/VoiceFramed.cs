// -----------------------------------------------------------------------
// <copyright file="VoiceFramed.cs" company="Exit Games GmbH">
//   Photon Voice API Framework for Photon - Copyright (C) 2017 Exit Games GmbH
// </copyright>
// <summary>
//   Photon data streaming support.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
#if DUMP_TO_FILE
using System.IO;
#endif
using System.Threading;

namespace Photon.Voice
{
    /// <summary>Audio Processor interface.</summary>
    public interface IProcessor<T> : IDisposable
    {
        /// <summary>Process a frame of audio data.</summary>
        /// <param name="buf">Buffer containing input audio data</param>
        /// <returns>Buffer containing output audio data or null if frame has been discarded (VAD)</returns>
        T[] Process(T[] buf);
    }

    /// <summary>Utility class to re-frame audio packets.</summary>
    public class Framer<T>
    {
        T[] frame;

        /// <summary>Create new Framer instance.</summary>
        public Framer(int frameSize)
        {
            this.frame = new T[frameSize];
            var x = new T[1];
            if (x[0] is byte)
                this.sizeofT = sizeof(byte);
            else if (x[0] is short)
                this.sizeofT = sizeof(short);
            else if (x[0] is float)
                this.sizeofT = sizeof(float);
            else
                throw new Exception("Input data type is not supported: " + x[0].GetType());

        }
        int sizeofT;
        int framePos = 0;

        /// <summary>Get the number of frames available after adding bufLen samples.</summary>
        /// <param name="bufLen">Number of samples that would be added.</param>
        /// <returns>Number of full frames available when adding bufLen samples.</returns>
        public int Count(int bufLen)
        {
            return (bufLen + framePos) / frame.Length;
        }

        /// <summary>Append arbitrary-sized buffer and return available full frames.</summary>
        /// <param name="buf">Array of samples to add.</param>
        /// <returns>Enumerator of full frames (might be none).</returns>
        public IEnumerable<T[]> Frame(T[] buf)
        {
            // quick return in trivial case
            if (frame.Length == buf.Length && framePos == 0)
            {
                yield return buf;
            }
            else
            {
                var bufPos = 0;

                while (frame.Length - framePos <= buf.Length - bufPos)
                {
                    var l = frame.Length - framePos;
                    Buffer.BlockCopy(buf, bufPos * sizeofT, frame, framePos * sizeofT, l * sizeofT);
                    //Console.WriteLine("=== Y {0} {1} -> {2} {3} ", bufPos, bufPos + l, sourceFramePos, sourceFramePos + l);
                    bufPos += l;
                    framePos = 0;

                    yield return this.frame;
                }
                if (bufPos != buf.Length)
                {
                    var l = buf.Length - bufPos;
                    Buffer.BlockCopy(buf, bufPos * sizeofT, frame, framePos * sizeofT, l * sizeofT);
                    //Console.WriteLine("=== L {0} {1} -> {2} {3} ", bufPos, bufPos + l, sourceFramePos, sourceFramePos + l);
                    framePos += l;
                }
            }
        }
    }

    /// <summary>
    /// Typed re-framing LocalVoice
    /// </summary>
    /// <remarks>Base class for typed re-framing LocalVoice implementation (<see cref="LocalVoiceFramed{T}"></see>) </remarks>
    public class LocalVoiceFramedBase : LocalVoice
    {
        /// <summary>Data flow will be repacked to frames of this size. May differ from input voiceInfo.FrameSize. Processors should resample in this case.</summary>
        public int FrameSize { get; private set; }

        internal LocalVoiceFramedBase(VoiceClient voiceClient, IEncoder encoder, byte id, VoiceInfo voiceInfo, int channelId, int frameSize)
        : base(voiceClient, encoder, id, voiceInfo, channelId)
        {
            this.FrameSize = frameSize;
        }
    }

    /// <summary>
    /// Typed re-framing LocalVoice
    /// </summary>
    /// <remarks>
    /// Consumes data in array buffers of arbitrary length. Repacks them in frames of constant length for further processing and encoding.
    /// </remarks>
    public class LocalVoiceFramed<T> : LocalVoiceFramedBase
    {
        Framer<T> framer;
#if DUMP_TO_FILE
        FileStream file;
        static int fileCnt = 0;
#endif
        // Optionally process input data. 
        // Should return arrays exactly of info.FrameSize size or null to skip sending
        protected T[] processFrame(T[] buf)
        {
            lock (this.processors)
            {
                foreach (var p in processors)
                {
                    buf = p.Process(buf);
                    if (buf == null)
                    {
                        break;
                    }
                }
            }
            return buf;
        }

        /// <summary>
        /// Adds processors after any built-in processors and everything added with AddPreProcessor.
        /// </summary>
        /// <param name="processors"></param>
        public void AddPostProcessor(params IProcessor<T>[] processors)
        {
            lock (this.processors)
            {
                foreach (var p in processors)
                {
                    this.processors.Add(p);
                }
            }
        }

        int preProcessorsCnt;

        /// <summary>
        /// Adds processors before built-in processors and everything added with AddPostProcessor.
        /// </summary>
        /// <param name="processors"></param>
        public void AddPreProcessor(params IProcessor<T>[] processors)
        {
            lock (this.processors)
            {
                foreach (var p in processors)
                {
                    this.processors.Insert(preProcessorsCnt++, p);
                }
            }
        }

        /// <summary>
        /// Clears all processors in pipeline including built-in resampling.
        /// User should add at least resampler processor after call.
        /// </summary>
        public void ClearProcessors()
        {
            lock (this.processors)
            {
                this.processors.Clear();
                preProcessorsCnt = 0;
            }
        }

        List<IProcessor<T>> processors = new List<IProcessor<T>>();

        internal LocalVoiceFramed(VoiceClient voiceClient, IEncoder encoder, byte id, VoiceInfo voiceInfo, int channelId, int frameSize)
        : base(voiceClient, encoder, id, voiceInfo, channelId, frameSize)
		{
#if DUMP_TO_FILE
            file = File.Open("dump-" + fileCnt++ + ".raw", FileMode.Create);
#endif
			if (frameSize == 0)
			{ 
				throw new Exception(LogPrefix + ": non 0 frame size required for framed stream");
			}
			this.framer = new Framer<T>(FrameSize);

            this.bufferFactory = new FactoryPrimitiveArrayPool<T>(DATA_POOL_CAPACITY, Name + " Data", FrameSize);
        }

        bool dataEncodeThreadStarted;
        Queue<T[]> pushDataQueue = new Queue<T[]>();
        AutoResetEvent pushDataQueueReady = new AutoResetEvent(false);

        public FactoryPrimitiveArrayPool<T> BufferFactory { get { return bufferFactory; } }
        FactoryPrimitiveArrayPool<T> bufferFactory;

        /// <summary>Wether this LocalVoiceFramed has capacity for more data buffers to be pushed asynchronously.</summary>
        public bool PushDataAsyncReady { get { lock (pushDataQueue) return pushDataQueue.Count < DATA_POOL_CAPACITY - 1; } } // 1 slot for buffer currently processed and not contained either by pool or queue

        /// <summary>Asynchronously push data into this stream.</summary>
        // Accepts array of arbitrary size. Automatically splits or aggregates input to buffers of length <see cref="FrameSize"></see>.
        // Expects buf content to be preserved until PushData is called from a worker thread. Releases buffer to <see cref="BufferFactory"></see> then.
        public void PushDataAsync(T[] buf)
        {
            if (disposed) return;

#if PHOTON_VOICE_THREADING_DISABLE
            PushData(buf);
            return;
#endif

            if (!dataEncodeThreadStarted)
            {
                voiceClient.logger.LogInfo(LogPrefix + ": Starting data encode thread");
#if NETFX_CORE
                Windows.System.Threading.ThreadPool.RunAsync((x) =>
                {
                    PushDataAsyncThread();
                });
#else
                var t = new Thread(PushDataAsyncThread);
                t.Start();
                Util.SetThreadName(t, "[PV] EncData " + shortName);
#endif
                dataEncodeThreadStarted = true;
            }

            // Caller should check this asap in general case if packet production is expensive.
            // This is not the case For lightweight audio stream. Also overflow does not happen for audio stream normally.
            // Make sure that queue is not too large even if caller missed the check.
            if (this.PushDataAsyncReady)
            {
                lock (pushDataQueue)
                {
                    pushDataQueue.Enqueue(buf);
                }
                pushDataQueueReady.Set();
            }
            else
            {
                this.bufferFactory.Free(buf, buf.Length);
                if (framesSkipped == framesSkippedNextLog)
                {
                    voiceClient.logger.LogWarning(LogPrefix + ": PushData queue overflow. Frames skipped: " + (framesSkipped + 1));
                    framesSkippedNextLog = framesSkipped + 10;
                }
                framesSkipped++;
            }
        }
        int framesSkippedNextLog;
        int framesSkipped;
        bool exitThread = false;
        private void PushDataAsyncThread()
        {

//#if UNITY_5_3_OR_NEWER
//            UnityEngine.Profiling.Profiler.BeginThreadProfiling("PhotonVoice", LogPrefix);
//#endif

            try
            {
                while (!exitThread)
                {
                    pushDataQueueReady.WaitOne(); // Wait until data is pushed to the queue or Dispose signals.

//#if UNITY_5_3_OR_NEWER
//                    UnityEngine.Profiling.Profiler.BeginSample("Encoder");
//#endif

                    while (true) // Dequeue and process while the queue is not empty
                    {
                        if (exitThread) break; // early exit to save few resources

                        T[] b = null;
                        lock (pushDataQueue)
                        {
                            if (pushDataQueue.Count > 0)
                            {
                                b = pushDataQueue.Dequeue();
                            }
                        }
                        if (b != null)
                        {
                            PushData(b);
                            this.bufferFactory.Free(b, b.Length);
                        }
                        else
                        {
                            break;
                        }
                    }

//#if UNITY_5_3_OR_NEWER
//                    UnityEngine.Profiling.Profiler.EndSample();
//#endif

                }
            }
            catch (Exception e)
            {
                voiceClient.logger.LogError(LogPrefix + ": Exception in encode thread: " + e);
                throw e;
            }
            finally
            {
                Dispose();
                this.bufferFactory.Dispose();

#if NETFX_CORE
                pushDataQueueReady.Dispose();
#else
                pushDataQueueReady.Close();
#endif

                voiceClient.logger.LogInfo(LogPrefix + ": Exiting data encode thread");

//#if UNITY_5_3_OR_NEWER
//                UnityEngine.Profiling.Profiler.EndThreadProfiling();
//#endif

            }
        }


        // counter for detection of first frame for which process() returned null
        int processNullFramesCnt = 0;
        /// <summary>Synchronously push data into this stream.</summary>
        // Accepts array of arbitrary size. Automatically splits or aggregates input to buffers of length <see cref="FrameSize"></see>.
        public void PushData(T[] buf)
        {
            if (this.voiceClient.transport.IsChannelJoined(this.channelId))
            {
                if (this.TransmitEnabled)
                {
                    if (this.encoder is IEncoderDirect<T[]>)
                    {
                        lock (disposeLock)
                        {
                            if (!disposed)
                            {
                                foreach (var framed in framer.Frame(buf))
                                {
                                    var processed = processFrame(framed);
                                    if (processed != null)
                                    {
#if DUMP_TO_FILE
                                        var b = new byte[processed.Length * sizeof(short)];
                                        Buffer.BlockCopy(processed, 0, b, 0, b.Length);
                                        file.Write(b, 0, b.Length);
#endif
                                        processNullFramesCnt = 0;
                                        ((IEncoderDirect<T[]>)this.encoder).Input(processed);
                                    }
                                    else
                                    {
                                        processNullFramesCnt++;
                                        if (processNullFramesCnt == 1)
                                        {
                                            this.encoder.EndOfStream();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new Exception(LogPrefix + ": PushData(T[]) called on encoder of unsupported type " + (this.encoder == null ? "null" : this.encoder.GetType().ToString()));
                    }
                }
            }
        }

        /// <summary>
        /// Releases resources used by the <see cref="LocalVoiceFramed{T}"/> instance. 
        /// Buffers used for asynchronous push will be disposed in encoder thread's 'finally'.
        /// </summary>
        public override void Dispose()
        {
#if DUMP_TO_FILE
            file.Close();
#endif
            exitThread = true;
            lock (disposeLock)
            {
                if (!disposed)
                {
                    lock (this.processors)
                    {
                        foreach (var p in processors)
                        {
                            p.Dispose();
                        }
                    }
                    base.Dispose();
                    pushDataQueueReady.Set(); // let worker exit
                }
            }
            base.Dispose();
        }
    }
}