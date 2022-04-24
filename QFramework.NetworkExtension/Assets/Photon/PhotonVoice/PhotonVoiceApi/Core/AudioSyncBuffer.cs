using System.Collections.Generic;

namespace Photon.Voice
{
    // Keeps buffer size within given bounds (discards or repeats samples) even if numbers of pushed and read samples per second are different
    public class AudioSyncBuffer<T> : IAudioOut<T>
    {
        private int curPlayingFrameSamplePos;

        private int sampleRate;
        private int channels;
        private int frameSamples;
        private int frameSize;
        private bool started;

        private int maxDevPlayDelaySamples;
        private int targetPlayDelaySamples;

        int playDelayMs;
        private readonly ILogger logger;
        private readonly string logPrefix;
        private readonly bool debugInfo;

        private readonly int elementSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(T));

        private T[] emptyFrame;

        public AudioSyncBuffer(int playDelayMs, ILogger logger, string logPrefix, bool debugInfo)
        {
            this.playDelayMs = playDelayMs;
            this.logger = logger;
            this.logPrefix = logPrefix;
            this.debugInfo = debugInfo;
        }
        public int Lag 
        { 
            get 
            {
                lock (this)
                {
                    return (int)((float)this.frameQueue.Count * this.frameSamples * 1000 / sampleRate);
                }
            } 
        }

        public bool IsPlaying
        {
            get 
            {
                lock (this)
                {
                    return this.started;
                }
            }
        }

        // Can be called on runnig AudioSyncBuffer to reuse it for other parameters
        public void Start(int sampleRate, int channels, int frameSamples)
        {
            lock (this)
            {
                this.started = false;

                this.sampleRate = sampleRate;

                // this.sampleRate = (int)(sampleRate * 1.2); // underrun test
                // this.sampleRate = (int)(sampleRate / 1.2); // overrun test

                this.channels = channels;
                this.frameSamples = frameSamples;
                this.frameSize = frameSamples * channels;

                int playDelaySamples = playDelayMs * sampleRate / 1000 + frameSamples;
                this.maxDevPlayDelaySamples = playDelaySamples / 2;
                this.targetPlayDelaySamples = playDelaySamples + maxDevPlayDelaySamples;

                if (this.framePool.Info != this.frameSize)
                {
                    this.framePool.Init(this.frameSize);
                }

                //frameQueue = new Queue<T[]>();
                while (this.frameQueue.Count > 0)
                {
                    dequeueFrameQueue();
                }

                // it's important to change 'emptyFrame' value after frameQueue cleaned up, otherwise ' != this.emptyFrame' check in dequeueFrameQueue() will not work
                this.emptyFrame = new T[this.frameSize];

                // initial sync
                int framesCnt = targetPlayDelaySamples / this.frameSamples;
                this.curPlayingFrameSamplePos = targetPlayDelaySamples % this.frameSamples;
                while (this.frameQueue.Count < framesCnt)
                {
                    this.frameQueue.Enqueue(emptyFrame);
                }

                this.started = true;
            }
        }

        Queue<T[]> frameQueue = new Queue<T[]>();
        public const int FRAME_POOL_CAPACITY = 50;
        PrimitiveArrayPool<T> framePool = new PrimitiveArrayPool<T>(FRAME_POOL_CAPACITY, "AudioSyncBuffer");

        public void Service()
        {
        }
        
        public void Read(T[] outBuf, int outChannels, int outSampleRate)
        {
            lock (this)
            {
                if (this.started)
                {
                    int outPos = 0;
                    // enough data in remaining frames to fill entire out buffer
                    // framesElemRem / this.sampleRate >= outElemRem / outSampleRate
                    while ((this.frameQueue.Count * this.frameSamples - this.curPlayingFrameSamplePos) * this.channels * outSampleRate >= (outBuf.Length - outPos) * this.sampleRate) 
                    {
                        int playingFramePos = this.curPlayingFrameSamplePos * this.channels;
                        var frame = frameQueue.Peek();
                        int outElemRem = outBuf.Length - outPos;
                        int frameElemRem = frame.Length - playingFramePos;

                        // enough data in the current frame to fill entire out buffer and some will remain for the next call: keeping this frame
                        // frameElemRem / (frCh * frRate) > outElemRem / (outCh * outRate)
                        if (frameElemRem * outChannels * outSampleRate > outElemRem * this.channels * this.sampleRate)
                        {
                            // frame remainder is large enough to fill outBuf remainder, keep this frame and return
                            //int framePosDelta = this.channels * outChannels * this.sampleRate / (outElemRem * outSampleRate);
                            int framePosDelta = outElemRem * this.channels* this.sampleRate / (outChannels * outSampleRate);
                            if (this.sampleRate == outSampleRate && this.channels == outChannels)
                            {
                                System.Buffer.BlockCopy(frame, playingFramePos * elementSize, outBuf, outPos * elementSize, outElemRem * elementSize);
                            }
                            else
                            {
                                AudioUtil.Resample(frame, playingFramePos, framePosDelta, this.channels, outBuf, outPos, outElemRem, outChannels);
                            }
                            this.curPlayingFrameSamplePos += framePosDelta / this.channels;

                            return;
                        }
                        // discarding current frame because it fills exactly out buffer or next frame required to do so
                        else
                        {
                            int outPosDelta = frameElemRem * outChannels * outSampleRate / (this.channels * this.sampleRate);
                            if (this.sampleRate == outSampleRate && this.channels == outChannels)
                            {
                                System.Buffer.BlockCopy(frame, playingFramePos * elementSize, outBuf, outPos * elementSize, frameElemRem * elementSize);
                            }
                            else
                            {
                                AudioUtil.Resample(frame, playingFramePos, frameElemRem, this.channels, outBuf, outPos, outPosDelta, outChannels);
                            }
                            outPos += outPosDelta;

                            this.curPlayingFrameSamplePos = 0;

                            dequeueFrameQueue();

                            if (outPosDelta == outElemRem)
                            {
                                return;
                            }
                        }
                    }
                }
            }
        }

        // may be called on any thread
        public void Push(T[] frame)
        {
            lock (this)
            {
                if (this.started)
                {
                    if (frame.Length == 0)
                    {
                        return;
                    }

                    if (frame.Length != this.frameSize)
                    {
                        logger.LogError("{0} AudioSyncBuffer audio frames are not of size: {1} != {2}", this.logPrefix, frame.Length, frameSize);
                        return;
                    }

                    //TODO: call framePool.AcquireOrCreate(frame.Length) and test
                    if (framePool.Info != frame.Length)
                    {
                        framePool.Init(frame.Length);
                    }
                    T[] b = framePool.AcquireOrCreate();
                    System.Buffer.BlockCopy(frame, 0, b, 0, System.Buffer.ByteLength(frame));

                    lock (this)
                    {
                        frameQueue.Enqueue(b);

                        syncFrameQueue();
                    }
                }
            }
        }

        public void Flush()
        {
        }

        public void Stop()
        {
            lock (this)
            {
                this.started = false;
            }
        }

        // call inside lock (this) { ... }
        private void dequeueFrameQueue()
        {
            var f = this.frameQueue.Dequeue();
            if (f != this.emptyFrame)
            {
                this.framePool.Release(f, f.Length);
            }

        }

        // call inside lock (this) { ... }
        private void syncFrameQueue()
        {
            var lagSamples = this.frameQueue.Count * this.frameSamples - this.curPlayingFrameSamplePos;
            if (lagSamples > targetPlayDelaySamples + maxDevPlayDelaySamples)
            {
                int framesCnt = targetPlayDelaySamples / this.frameSamples;
                this.curPlayingFrameSamplePos = targetPlayDelaySamples % this.frameSamples;
                while (frameQueue.Count > framesCnt)
                {
                    dequeueFrameQueue();
                }
                if (this.debugInfo)
                {
                    this.logger.LogWarning("{0} AudioSynctBuffer overrun {1} {2} {3} {4}", this.logPrefix, targetPlayDelaySamples - maxDevPlayDelaySamples, targetPlayDelaySamples + maxDevPlayDelaySamples, lagSamples, framesCnt, this.curPlayingFrameSamplePos);
                }
            }
            else if (lagSamples < targetPlayDelaySamples - maxDevPlayDelaySamples)
            {
                int framesCnt = targetPlayDelaySamples / this.frameSamples;
                this.curPlayingFrameSamplePos = targetPlayDelaySamples % this.frameSamples;
                while (frameQueue.Count < framesCnt)
                {
                    frameQueue.Enqueue(emptyFrame);
                }

                if (this.debugInfo)
                {
                    this.logger.LogWarning("{0} AudioSyncBuffer underrun {1} {2} {3} {4}", this.logPrefix, targetPlayDelaySamples - maxDevPlayDelaySamples, targetPlayDelaySamples + maxDevPlayDelaySamples, lagSamples, framesCnt, this.curPlayingFrameSamplePos);
                }
            }
        }
    }
}