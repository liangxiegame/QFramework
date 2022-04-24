using POpusCodec.Enums;
using POpusCodec;
using System;

namespace Photon.Voice
{
    public class OpusCodec
    {
        static public string Version
        {
            get
            {
                return OpusLib.Version;
            }
        }

        public enum FrameDuration
        {
            Frame2dot5ms = 2500,
            Frame5ms = 5000,
            Frame10ms = 10000,
            Frame20ms = 20000,
            Frame40ms = 40000,
            Frame60ms = 60000
        }

        public static class Factory
        {
            static public IEncoder CreateEncoder<B>(VoiceInfo i, ILogger logger)
            {
                if (typeof(B) == typeof(float[]))
                    return new EncoderFloat(i, logger);
                else if (typeof(B) == typeof(short[]))
                    return new EncoderShort(i, logger);
                else
                    throw new UnsupportedCodecException("Factory.CreateEncoder<" + typeof(B) + ">", i.Codec);
            }
        }

        public static class DecoderFactory
        {
            public static IEncoder Create<T>(VoiceInfo i, ILogger logger)
            {
                var x = new T[1];
                if (x[0].GetType() == typeof(float))
                    return new EncoderFloat(i, logger);
                else if (x[0].GetType() == typeof(short))
                    return new EncoderShort(i, logger);
                else
                    throw new UnsupportedCodecException("EncoderFactory.Create<" + x[0].GetType() + ">", i.Codec);
            }
        }

        abstract public class Encoder<T> : IEncoderDirect<T[]>
        {
            protected OpusEncoder encoder;
            protected bool disposed;
            protected Encoder(VoiceInfo i, ILogger logger)
            {
                try
                {
                    encoder = new OpusEncoder((SamplingRate)i.SamplingRate, (Channels)i.Channels, i.Bitrate, OpusApplicationType.Voip, (Delay)(i.FrameDurationUs * 2 / 1000));
                    logger.LogInfo("[PV] OpusCodec.Encoder created. Opus version " + Version + ". Bitrate " + encoder.Bitrate + ". EncoderDelay " + encoder.EncoderDelay);
                }
                catch (Exception e)
                {
                    Error = e.ToString();
                    if (Error == null) // should never happen but since Error used as validity flag, make sure that it's not null
                    {
                        Error = "Exception in OpusCodec.Encoder constructor";
                    }
                    logger.LogError("[PV] OpusCodec.Encoder: " + Error);
                }
            }

            public string Error { get; private set; }

            public Action<ArraySegment<byte>, FrameFlags> Output { set; get; }

            public void Input(T[] buf)
            {
                if (Error != null)
                {
                    return;
                }
                if (Output == null)
                {
                    Error = "OpusCodec.Encoder: Output action is not set";
                    return;
                }

                lock (this)
                {
                    if (disposed || Error != null) { }
                    else
                    {
                        var res = encodeTyped(buf);
                        if (res.Count != 0)
                        {
                            Output(res, 0);
                        }
                    }
                }
            }

            public void EndOfStream()
            {
                lock (this)
                {
                    if (disposed || Error != null) { }
                    else
                    {
                        Output(EmptyBuffer, FrameFlags.EndOfStream);
                    }
                }
                return;
            }

            private static readonly ArraySegment<byte> EmptyBuffer = new ArraySegment<byte>(new byte[] { });

            public ArraySegment<byte> DequeueOutput(out FrameFlags flags) { flags = 0; return EmptyBuffer; }

            protected abstract ArraySegment<byte> encodeTyped(T[] buf);

            public I GetPlatformAPI<I>() where I : class
            {
                return null;
            }

            public void Dispose()
            {
                lock (this)
                {
                    if (encoder != null)
                    {
                        encoder.Dispose();
                    }
                    disposed = true;
                }
            }
        }

        public class EncoderFloat : Encoder<float>
        {
            internal EncoderFloat(VoiceInfo i, ILogger logger) : base(i, logger) { }

            override protected ArraySegment<byte> encodeTyped(float[] buf)
            {
                return encoder.Encode(buf);
            }
        }
        public class EncoderShort : Encoder<short>
        {
            internal EncoderShort(VoiceInfo i, ILogger logger) : base(i, logger) { }
            override protected ArraySegment<byte> encodeTyped(short[] buf)
            {
                return encoder.Encode(buf);
            }
        }

        public class Decoder<T> : IDecoder
        {
            protected OpusDecoder<T> decoder;
            ILogger logger;
            public Decoder(Action<FrameOut<T>> output, ILogger logger)
            {
                this.output = output;
                this.logger = logger;
            }

            public void Open(VoiceInfo i)
            {
                try
                {
                    decoder = new OpusDecoder<T>((SamplingRate)i.SamplingRate, (Channels)i.Channels);
                    logger.LogInfo("[PV] OpusCodec.Decoder created. Opus version " + Version);
                }
                catch (Exception e)
                {
                    Error = e.ToString();
                    if (Error == null) // should never happen but since Error used as validity flag, make sure that it's not null
                    {
                        Error = "Exception in OpusCodec.Decoder constructor";
                    }
                    logger.LogError("[PV] OpusCodec.Decoder: " + Error);
                }
            }

            public string Error { get; private set; }

            private Action<FrameOut<T>> output;

            public void Dispose()
            {
                if (decoder != null)
                {
                    decoder.Dispose();
                }
            }

            FrameOut<T> frameOut = new FrameOut<T>(null, false);
            public void Input(ref FrameBuffer buf)
            {
                if (Error == null)
                {
                    bool endOfStream = (buf.Flags & FrameFlags.EndOfStream) != 0;
                    if (endOfStream)
                    {
                        T[] res1 = null;
                        T[] res2;
                        // EndOfStream packet may have data
                        // normally we do not send null with EndOfStream flag, but null is still valid here
                        if (buf.Array == null && buf.Length > 0)
                        {
                            res1 = decoder.DecodePacket(ref buf);
                        }
                        // flush decoder
                        res2 = decoder.DecodeEndOfStream();

                        // if res1 is empty, res2 has correct (possible empty) buffer for EndOfStream frame
                        if (res1 != null && res1.Length == 0)
                        {
                            // output cal per res required
                            if (res2 != null && res2.Length != 0)
                            {
                                output(frameOut.Set(res1, false));
                            }
                            else                            
                            {
                                // swap results to reuse the code below
                                res2 = res1;
                            }
                        }
                        output(frameOut.Set(res2, true));
                    }
                    else
                    {
                        T[] res;
                        res = decoder.DecodePacket(ref buf);
                        if (res.Length != 0)
                        {
                            output(frameOut.Set(res, false));
                        }
                    }
                    
                }
            }
        }

        public class Util
        {
            internal static int bestEncoderSampleRate(int f)
            {
                int diff = int.MaxValue;
                int res = (int)SamplingRate.Sampling48000;
                foreach (var x in Enum.GetValues(typeof(SamplingRate)))
                {
                    var d = Math.Abs((int)x - f);
                    if (d < diff)
                    {
                        diff = d;
                        res = (int)x;
                    }
                }
                return res;
            }
        }
    }
}