// -----------------------------------------------------------------------
// <copyright file="VoiceCodec.cs" company="Exit Games GmbH">
//   Photon Voice API Framework for Photon - Copyright (C) 2017 Exit Games GmbH
// </copyright>
// <summary>
//   Photon data streaming support.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Photon.Voice
{
    public enum FrameFlags : byte
    {
        Config = 1,
        KeyFrame = 2,
        PartialFrame = 4,
        EndOfStream = 8
    }

    /// <summary>Generic encoder interface.</summary>
    /// <remarks>
    /// Depending on implementation, encoder should either call Output on eaach data frame or return next data frame in DequeueOutput() call.
    /// </remarks>
    public interface IEncoder : IDisposable
    {
        /// <summary>If not null, the object is in invalid state.</summary>
        string Error { get; }
        /// <summary>Set callback encoder calls on each encoded data frame (if such output supported).</summary>
        Action<ArraySegment<byte>, FrameFlags> Output { set; }
        /// <summary>Returns next encoded data frame (if such output supported).</summary>
        ArraySegment<byte> DequeueOutput(out FrameFlags flags);
        /// <summary>Forces an encoder to flush and produce frame with EndOfStream flag (in output queue).</summary>
        void EndOfStream();
        /// <summary>Returns an platform-specific interface.</summary>
        I GetPlatformAPI<I>() where I : class;
    }

    /// <summary>Interface for an encoder which consumes input data via explicit call.</summary>
    public interface IEncoderDirect<B> : IEncoder
    {
        /// <summary>Consumes the given raw data.</summary>
        /// <param name="buf">Array containing raw data (e.g. audio samples).</param>
        void Input(B buf);
    }

    /// <summary>Interface for an encoder which consumes images via explicit call.</summary>
    public interface IEncoderDirectImage : IEncoderDirect<ImageBufferNative>
    {
        /// <summary>Recommended encoder input image format. Encoder may support other formats.</summary>
        ImageFormat ImageFormat { get; }
    }

    /// <summary>Generic decoder interface.</summary>
    public interface IDecoder : IDisposable
    {
        /// <summary>Open (initialize) the decoder.</summary>
        /// <param name="info">Properties of the data stream to decode.</param>
        void Open(VoiceInfo info);
        /// <summary>If not null, the object is in invalid state.</summary>
        string Error { get; }
        /// <summary>Consumes the given encoded data.</summary>
        /// <remarks>
        /// The callee can call buf.Retain() to prevent the caller from disposing the buffer.
        /// In this case, the callee should call buf.Release() when buffer is no longer needed.
        /// </remarks>
        void Input(ref FrameBuffer buf);
    }

    /// <summary>Interface for an decoder which outputs data via explicit call.</summary>
    public interface IDecoderDirect<B> : IDecoder
    {
        /// <summary>Callback to call when a new decoded data buffer is available.</summary>
        Action<B> Output { get; set; }
    }

    /// <summary>Exception thrown if an unsupported audio sample type is encountered.</summary>
    /// <remarks>
    /// PhotonVoice generally supports 32-bit floating point ("float") or 16-bit signed integer ("short") audio,
    /// but it usually won't be converted automatically due to the high CPU overhead (and potential loss of precision) involved.
    /// </remarks>
    class UnsupportedSampleTypeException : Exception
    {
        /// <summary>Create a new UnsupportedSampleTypeException.</summary>
        /// <param name="t">The sample type actually encountered.</param>
        public UnsupportedSampleTypeException(Type t) : base("[PV] unsupported sample type: " + t) { }
    }

    /// <summary>Exception thrown if an unsupported codec is encountered.</summary>
    class UnsupportedCodecException : Exception
    {
        /// <summary>Create a new UnsupportedCodecException.</summary>
        /// <param name="info">The info prepending standard message.</param>
        /// <param name="codec">The codec actually encountered.</param>
        public UnsupportedCodecException(string info, Codec codec) : base("[PV] " + info + ": unsupported codec: " + codec) { }
    }

    /// <summary>Exception thrown if an unsupported platform is encountered.</summary>
    class UnsupportedPlatformException : Exception
    {
        /// <summary>Create a new UnsupportedPlatformException.</summary>
        /// <param name="subject">The info prepending standard message.</param>
        /// /// <param name="platform">Optional platform name.</param>
        public UnsupportedPlatformException(string subject, string platform = null) : base("[PV] " + subject + " does not support " + (platform == null ? "current" : platform) + " platform") { }
    }

    /// <summary>Enum for Media Codecs supported by PhotonVoice.</summary>
    /// <remarks>Transmitted in <see cref="VoiceInfo"></see>. Do not change the values of this Enum!</remarks>
    public enum Codec
    {
        Raw = 1,
        /// <summary>OPUS audio</summary>
        AudioOpus = 11,
#if PHOTON_VOICE_VIDEO_ENABLE
        VideoVP8 = 21,
        VideoVP9 = 22,
        VideoH264 = 31,
#endif
    }

    public enum ImageFormat
    {
        Undefined,
        I420, // native vpx (no format conversion before encodong)
        YV12, // native vpx (no format conversion before encodong)
        Android420,
        ABGR,
        BGRA,
        ARGB,
        NV12,
    }

    public enum Rotation
    {
        Undefined = -1,
        Rotate0 = 0,      // No rotation.
        Rotate90 = 90,    // Rotate 90 degrees clockwise.
        Rotate180 = 180,  // Rotate 180 degrees.
        Rotate270 = 270,  // Rotate 270 degrees clockwise.
    }

    public struct Flip
    {
        public bool IsVertical { get; private set; }
        public bool IsHorizontal { get; private set; }

        public static bool operator ==(Flip f1, Flip f2)
        {
            return f1.IsVertical == f2.IsVertical && f1.IsHorizontal == f2.IsHorizontal;
        }

        public static bool operator !=(Flip f1, Flip f2)
        {
            return f1.IsVertical != f2.IsVertical || f1.IsHorizontal != f2.IsHorizontal;
        }

        // trivial implementation to avoid warnings CS0660 and CS0661 about missing overrides when == and != defined 
        public override bool Equals(object obj) { return base.Equals(obj); } 
        public override int GetHashCode() { return base.GetHashCode(); }

        public static Flip operator *(Flip f1, Flip f2)
        {
            return new Flip
            {
                IsVertical = f1.IsVertical != f2.IsVertical,
                IsHorizontal = f1.IsHorizontal != f2.IsHorizontal,
            };
        }

        public static Flip None;
        public static Flip Vertical = new Flip() { IsVertical = true };
        public static Flip Horizontal = new Flip() { IsHorizontal = true };
        public static Flip Both = Vertical * Horizontal;
    }

    // Image buffer pool support
    public struct ImageBufferInfo
    {
        [StructLayout(LayoutKind.Sequential)] // the struct instance may be used where IntPtr[] expected by native method
        public struct StrideSet
        {
            private int stride0;
            private int stride1;
            private int stride2;
            private int stride3;

            public StrideSet(int length, int s0 = 0, int s1 = 0, int s2 = 0, int s3 = 0)
            {
                Length = length;
                stride0 = s0;
                stride1 = s1;
                stride2 = s2;
                stride3 = s3;
            }
            public int this[int key]
            {
                get
                {
                    switch (key)
                    {
                        case 0: return stride0;
                        case 1: return stride1;
                        case 2: return stride2;
                        case 3: return stride3;
                        default: return 0;
                    }
                }

                set
                {
                    switch (key)
                    {
                        case 0: stride0 = value; break;
                        case 1: stride1 = value; break;
                        case 2: stride2 = value; break;
                        case 3: stride3 = value; break;
                    }
                }
            }

            public int Length { get; private set; }
        }

        public int Width { get; }
        public int Height { get; }
        public StrideSet Stride { get; }
        public ImageFormat Format { get; }
        public Rotation Rotation { get; set; }
        public Flip Flip { get; set; }
        public ImageBufferInfo(int width, int height, StrideSet stride, ImageFormat format)
        {
            Width = width;
            Height = height;
            Stride = stride;
            Format = format;
            Rotation = Rotation.Rotate0;
            Flip = Flip.None;
        }
    }

    public class ImageBufferNative
    {
        [StructLayout(LayoutKind.Sequential)] // the struct instance may be used where IntPtr[] expected by native method (does not work on Mac, so we use intermediate IntPtr[] to pass planes)
        public struct PlaneSet
        {
            private IntPtr plane0;
            private IntPtr plane1;
            private IntPtr plane2;
            private IntPtr plane3;

            public PlaneSet(int length, IntPtr p0 = default(IntPtr), IntPtr p1 = default(IntPtr), IntPtr p2 = default(IntPtr), IntPtr p3 = default(IntPtr))
            {
                Length = length;
                plane0 = p0;
                plane1 = p1;
                plane2 = p2;
                plane3 = p3;
            }
            public IntPtr this[int key]
            {
                get
                {
                    switch (key)
                    {
                        case 0: return plane0;
                        case 1: return plane1;
                        case 2: return plane2;
                        case 3: return plane3;
                        default: return IntPtr.Zero;
                    }
                }

                set
                {
                    switch (key)
                    {
                        case 0: plane0 = value; break;
                        case 1: plane1 = value; break;
                        case 2: plane2 = value; break;
                        case 3: plane3 = value; break;
                    }
                }
            }

            public int Length { get; private set; }
        }

        public ImageBufferNative(ImageBufferInfo info)
        {
            Info = info;
            Planes = new PlaneSet(info.Stride.Length);
        }

        public ImageBufferNative(IntPtr buf, int width, int height, int stride, ImageFormat imageFormat)
        {
            Info = new ImageBufferInfo(width, height, new ImageBufferInfo.StrideSet(1, stride), imageFormat);
            Planes = new PlaneSet(1, buf);
        }
        public ImageBufferInfo Info;
        public PlaneSet Planes; // operator[] setter does not compile if this member is a property (because [] applies to a copy of the property)

        // Release resources for dispose or reuse.
        public virtual void Release() { }
        public virtual void Dispose() { }

    }

    // Allocates native buffers for planes
    // Supports releasing to image pool with allocation reuse
    public class ImageBufferNativeAlloc : ImageBufferNative, IDisposable
    {
        ImageBufferNativePool<ImageBufferNativeAlloc> pool;
        public ImageBufferNativeAlloc(ImageBufferNativePool<ImageBufferNativeAlloc> pool, ImageBufferInfo info) : base(info)
        {
            this.pool = pool;
            
            for (int i = 0; i < info.Stride.Length; i++)
            {
                Planes[i] = Marshal.AllocHGlobal(info.Stride[i] * info.Height);
            }
        }

        public override void Release()
        {
            if (pool != null)
            {
                pool.Release(this);
            }
        }

        public override void Dispose()
        {
            for (int i = 0; i < Info.Stride.Length; i++)
            {
                Marshal.FreeHGlobal(Planes[i]);
            }
        }
    }

    // Acquires byte[] plane via GHandle. Optimized for single plane images.
    // Supports releasing to image pool after freeing GHandle (object itself reused only)
    public class ImageBufferNativeGCHandleSinglePlane : ImageBufferNative, IDisposable
    {
        ImageBufferNativePool<ImageBufferNativeGCHandleSinglePlane> pool;
        GCHandle planeHandle;
        public ImageBufferNativeGCHandleSinglePlane(ImageBufferNativePool<ImageBufferNativeGCHandleSinglePlane> pool, ImageBufferInfo info) : base(info)
        {
            if (info.Stride.Length != 1)
            {
                throw new Exception("ImageBufferNativeGCHandleSinglePlane wrong plane count " + info.Stride.Length);
            }
            this.pool = pool;
        }
        public void PinPlane(byte[] plane)
        {
            planeHandle = GCHandle.Alloc(plane, GCHandleType.Pinned);
            Planes[0] = planeHandle.AddrOfPinnedObject();
        }

        public override void Release()
        {
            planeHandle.Free();
            if (pool != null)
            {
                pool.Release(this);
            }
        }

        public override void Dispose()
        {
        }
    }
}