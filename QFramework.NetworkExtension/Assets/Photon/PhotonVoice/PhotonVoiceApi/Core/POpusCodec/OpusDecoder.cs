using System;
using POpusCodec.Enums;
using System.Runtime.InteropServices;
using Photon.Voice;

namespace POpusCodec
{
    public class OpusDecoder<T> : IDisposable
    {
        private const bool UseInbandFEC = true;

        private bool TisFloat;
        private int sizeofT;
        
        private IntPtr _handle = IntPtr.Zero;
        private const int MaxFrameSize = 5760;

        private int _channelCount;

        private static readonly T[] EmptyBuffer = new T[] { };

        private Bandwidth? _previousPacketBandwidth = null;

        public Bandwidth? PreviousPacketBandwidth
        {
            get
            {
                return _previousPacketBandwidth;
            }
        }

        public OpusDecoder(SamplingRate outputSamplingRateHz, Channels numChannels)
        {
            TisFloat = default(T) is float;
            sizeofT = Marshal.SizeOf(default(T));

            if ((outputSamplingRateHz != SamplingRate.Sampling08000)
                && (outputSamplingRateHz != SamplingRate.Sampling12000)
                && (outputSamplingRateHz != SamplingRate.Sampling16000)
                && (outputSamplingRateHz != SamplingRate.Sampling24000)
                && (outputSamplingRateHz != SamplingRate.Sampling48000))
            {
                throw new ArgumentOutOfRangeException("outputSamplingRateHz", "Must use one of the pre-defined sampling rates (" + outputSamplingRateHz + ")");
            }
            if ((numChannels != Channels.Mono)
                && (numChannels != Channels.Stereo))
            {
                throw new ArgumentOutOfRangeException("numChannels", "Must be Mono or Stereo");
            }

            _channelCount = (int)numChannels;
            _handle = Wrapper.opus_decoder_create(outputSamplingRateHz, numChannels);

            if (_handle == IntPtr.Zero)
            {
                throw new OpusException(OpusStatusCode.AllocFail, "Memory was not allocated for the encoder");
            }
        }
        
        private T[] buffer; // allocated for exactly 1 frame size as first valid frame received
        private FrameBuffer prevPacketData;
        bool prevPacketInvalid; // maybe false if prevPacket us null

        // pass null to indicate packet loss
        public T[] DecodePacket(ref FrameBuffer packetData)
        {
            if (this.buffer == null && packetData.Array == null)
            {
                return EmptyBuffer;
            }
            
            int numSamplesDecoded = 0;

            if (this.buffer == null)
            {
                // on the first call we don't know frame size, use temporal buffer of maximal length
                this.buffer = new T[MaxFrameSize * _channelCount];                
            }

            bool packetInvalid;
            if (packetData.Array == null)
            {
                packetInvalid = true;
            }
            else
            {
                int bandwidth = Wrapper.opus_packet_get_bandwidth(packetData.Ptr);
                packetInvalid = bandwidth == (int)OpusStatusCode.InvalidPacket;
            }

            bool regularDecode = false;
            if (UseInbandFEC)
            {
                if (prevPacketInvalid)
                {
                    if (packetInvalid)
                    {
                        // no fec data, conceal previous frame
                        numSamplesDecoded = TisFloat ?
                            Wrapper.opus_decode(_handle, new FrameBuffer(), this.buffer as float[], 0, _channelCount) :
                            Wrapper.opus_decode(_handle, new FrameBuffer(), this.buffer as short[], 0, _channelCount);
                        //UnityEngine.Debug.Log("======================= Conceal");
                    }
                    else
                    {
                        // error correct previous frame with the help of the current
                        numSamplesDecoded = TisFloat ?
                            Wrapper.opus_decode(_handle, packetData, this.buffer as float[], 1, _channelCount) :
                            Wrapper.opus_decode(_handle, packetData, this.buffer as short[], 1, _channelCount);
                        //UnityEngine.Debug.Log("======================= FEC");
                    }
                }
                else
                {
                    // decode previous frame
                    if (prevPacketData.Array != null) // is null on 1st call
                    {
                        numSamplesDecoded = TisFloat ?
                            Wrapper.opus_decode(_handle, prevPacketData, this.buffer as float[], 0, _channelCount) :
                            Wrapper.opus_decode(_handle, prevPacketData, this.buffer as short[], 0, _channelCount);                        
                        // prevPacketData is disposed below before copying packetData to it
                        regularDecode = true;
                    }
                }

                prevPacketData.Release();
                prevPacketData = packetData;
                packetData.Retain();
                prevPacketInvalid = packetInvalid;
            }
            else
            {
                #pragma warning disable 162
                // decode or conceal current frame
                numSamplesDecoded = TisFloat ?
                    Wrapper.opus_decode(_handle, packetData, this.buffer as float[], 0, _channelCount) :
                    Wrapper.opus_decode(_handle, packetData, this.buffer as short[], 0, _channelCount);
                regularDecode = true;
                #pragma warning restore 162
            }

            if (numSamplesDecoded == 0)
                return EmptyBuffer;
            
            if (this.buffer.Length != numSamplesDecoded * _channelCount)
            {
                if (!regularDecode)
                {
                    // wait for regular valid frame to imitialize the size
                    return EmptyBuffer;
                }
                // now that we know the frame size, allocate the buffer and copy data from temporal buffer
                var tmp = this.buffer;
                this.buffer = new T[numSamplesDecoded * _channelCount];
                Buffer.BlockCopy(tmp, 0, this.buffer, 0, numSamplesDecoded * sizeofT);
            }
            return this.buffer;
        }

        public T[] DecodeEndOfStream()
        {
            int numSamplesDecoded = 0;
            if (UseInbandFEC && !prevPacketInvalid)
            {
                // follow the same buffer initializatiopn pattern as in DecodeFrame() though buffer is already initialized most likely
                if (this.buffer == null)
                {
                    // on the first call we don't know frame size, use temporal buffer of maximal length
                    this.buffer = new T[MaxFrameSize * _channelCount];
                }

                // decode previous frame
                if (prevPacketData.Array != null) // is null on 1st call
                {
                    numSamplesDecoded = TisFloat ?
                    Wrapper.opus_decode(_handle, prevPacketData, this.buffer as float[], 1, _channelCount) :
                    Wrapper.opus_decode(_handle, prevPacketData, this.buffer as short[], 1, _channelCount);
                }
                prevPacketData.Release();
                prevPacketData = new FrameBuffer();
                prevPacketInvalid = false;

                if (numSamplesDecoded == 0)
                {
                    return EmptyBuffer;
                }
                else
                {
                    // follow the same buffer initializatiopn pattern as in DecodeFrame() 
                    if (this.buffer.Length != numSamplesDecoded * _channelCount)
                    {
                        // now that we know the frame size, allocate the buffer and copy data from temporal buffer
                        var tmp = this.buffer;
                        this.buffer = new T[numSamplesDecoded * _channelCount];
                        Buffer.BlockCopy(tmp, 0, this.buffer, 0, numSamplesDecoded * sizeofT);
                    }

                    return this.buffer;
                }
            }
            else
            {
                prevPacketData.Release();
                prevPacketData = new FrameBuffer();
                prevPacketInvalid = false;
                return EmptyBuffer;
            }
        }

        public void Dispose()
        {
            prevPacketData.Release();
            if (_handle != IntPtr.Zero)
            {
                Wrapper.opus_decoder_destroy(_handle);
                _handle = IntPtr.Zero;
            }
        }
    }
}
