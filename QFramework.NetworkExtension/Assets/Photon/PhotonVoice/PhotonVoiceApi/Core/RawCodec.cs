using System;

namespace Photon.Voice
{
    public class RawCodec
    {
        public class Encoder<T> : IEncoderDirect<T[]>
        {
            public string Error { get; private set; }

            public Action<ArraySegment<byte>, FrameFlags> Output { set; get; }

            int sizeofT = System.Runtime.InteropServices.Marshal.SizeOf(default(T));
            byte[] byteBuf = new byte[0];
            private static readonly ArraySegment<byte> EmptyBuffer = new ArraySegment<byte>(new byte[] { });

            public ArraySegment<byte> DequeueOutput(out FrameFlags flags)
            {
                flags = 0;
                return EmptyBuffer;
            }

            public void EndOfStream()
            {
            }

            public I GetPlatformAPI<I>() where I : class
            {
                return null;
            }

            public void Dispose()
            {
            }

            public void Input(T[] buf)
            {
                if (Error != null)
                {
                    return;
                }
                if (Output == null)
                {
                    Error = "RawCodec.Encoder: Output action is not set";
                    return;
                }
                if (buf == null)
                {
                    return;
                }
                if (buf.Length == 0)
                {
                    return;
                }

                var s = buf.Length * sizeofT;
                if (byteBuf.Length < s)
                {
                    byteBuf = new byte[s];
                }
                Buffer.BlockCopy(buf, 0, byteBuf, 0, s);
                Output(new ArraySegment<byte>(byteBuf, 0, s), 0);
            }
        }

        public class Decoder<T> : IDecoder
        {
            public string Error { get; private set; }

            public Decoder(Action<FrameOut<T>> output)
            {
                this.output = output;
            }

            public void Open(VoiceInfo info)
            {
            }

            T[] buf = new T[0];
            int sizeofT = System.Runtime.InteropServices.Marshal.SizeOf(default(T));

			public void Input(ref FrameBuffer byteBuf)
            {
                if (byteBuf.Array == null)
                {
                    return;
                }
                if (byteBuf.Length == 0)
                {
                    return;
                }

                var s = byteBuf.Length / sizeofT;
                if (buf.Length < s)
                {
                    buf = new T[s];
                }
                Buffer.BlockCopy(byteBuf.Array, byteBuf.Offset, buf, 0, byteBuf.Length);

                output(new FrameOut<T>((T[])(object)buf, false));
            }
            public void Dispose()
            {
            }

            private Action<FrameOut<T>> output;
        }

        // Adapts FrameOut<float> output to FrameOut<short> decoder
        // new RawCodec.Decoder<short>(new RawCodec.ShortToFloat(output as Action<FrameOut<float>>).Output);
        public class ShortToFloat
        {
            public ShortToFloat(Action<FrameOut<float>> output)
            {
                this.output = output;
            }
            public void Output(FrameOut<short> shortBuf)
            {
                if (buf.Length < shortBuf.Buf.Length)
                {
                    buf = new float[shortBuf.Buf.Length];
                }
                AudioUtil.Convert(shortBuf.Buf, buf, shortBuf.Buf.Length);
                output(new FrameOut<float>((float[])(object)buf, false));
            }

            Action<FrameOut<float>> output;
            float[] buf = new float[0];
        }
    }
}
