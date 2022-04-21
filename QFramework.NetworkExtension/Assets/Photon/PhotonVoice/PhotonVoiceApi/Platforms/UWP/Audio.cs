#if WINDOWS_UWP || ENABLE_WINMD_SUPPORT
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Windows.Media.MediaProperties;

namespace Photon.Voice.UWP
{
    public class AudioInPusher : IAudioPusher<short>
    {
        ILogger logger;
        int samplingRate;
        int channels;
        CaptureDevice device = null;
        ObjectFactory<short[], int> bufferFactory;

        public AudioInPusher(ILogger logger, int samplingRate, int channels, string deviceID)
        {
            this.logger = logger;
            this.samplingRate = samplingRate;
            this.channels = channels;
            device = new CaptureDevice(logger, CaptureDevice.Media.Audio, deviceID);
        }

        void init()
        {
            try
            {
                device.Initialize();
                device.CaptureFailed += Device_CaptureFailed;
            }
            catch (AggregateException e)
            {
                logger.LogError("[PV] [AI] Device initialization Error: (HResult=" + e.HResult + ") " + e);
                e.Handle((x) =>
                {
                    if (x is UnauthorizedAccessException)
                    {
                        ErrorAccess = true;
                    }
                    Error = x.Message;
                    logger.LogError("[PV] [AI] Device initialization Error (Inner Level 2): (HResult=" + x.HResult + ") " + x);
                    if (x is AggregateException)
                    {
                        (x as AggregateException).Handle((y) =>
                        {
                            Error = y.Message;
                            logger.LogError("[PV] [AI] Device initialization Error (Inner Level 3): (HResult=" + y.HResult + ") " + y);
                            return true;
                        });
                    }
                    return true;
                });
            }
            catch (Exception e)
            {
                Error = e.Message;
                logger.LogError("[PV] [AI] Device initialization Error: " + e);
            }

            if (Error == null)
            {
                logger.LogInfo("[PV] [AI] AudioIn successfully created");
            }
        }

        private void Device_CaptureFailed(object sender, Windows.Media.Capture.MediaCaptureFailedEventArgs e)
        {
            Error = e.Message;
            logger.LogError("[PV] [AI] Error: " + Error);
        }

        public int SamplingRate { get { return samplingRate; } }

        /// <summary>Number of channels in the audio signal.</summary>
        public int Channels { get { return channels; } }

        public void SetCallback(Action<short[]> callback, ObjectFactory<short[], int> bufferFactory)
        {
            init();
            if (Error != null)
            {
                return;
            }
            // Use the MP4 preset to an obtain H.264 video encoding profile
            //            var mep = new MediaEncodingProfile();
            var mep = new MediaEncodingProfile();
            mep.Audio = AudioEncodingProperties.CreatePcm((uint)samplingRate, (uint)channels, 16);
            mep.Video = null;
            mep.Container = null;

            device.StartRecordingAsync(mep, (buf, flags) =>
            {
                    //                    logger.LogInfo("[PV] [AI] " + buf.Length + ": " + BitConverter.ToString(buf, 0, buf.Length > 20 ? 20 : buf.Length));
                if (buf != null)
                {
                    var sb = bufferFactory.New(buf.Length / 2);
                    Buffer.BlockCopy(buf, 0, sb, 0, buf.Length);
                    callback(sb);
                }
            }).ContinueWith((t) =>
            {
                if (t.Exception == null)
                {
                    logger.LogInfo("[PV] [AI] Recording successfully started");
                }
                else
                {
                    t.Exception.Handle((x) =>
                    {
                        Error = x.Message;
                        logger.LogError("[PV] [AI] Recording starting Error: " + Error);
                        return true;
                    });
                }
            });
        }

        private static readonly ArraySegment<byte> EmptyBuffer = new ArraySegment<byte>(new byte[] { });
        public ArraySegment<byte> DequeueOutput(out FrameFlags flags)
        {
            flags = 0;
            return EmptyBuffer;
        }

        public string Error { get; private set; }
        public bool ErrorAccess { get; private set; }

        public void EndOfStream()
        {
        }

        public I GetPlatformAPI<I>() where I : class
        {
            return null;
        }

        public void Dispose()
        {
            device.StopRecordingAsync().ContinueWith((t) =>
            {
                logger.LogInfo("[PV] [AI] AudioIn disposed");
            });
        }

    }
}
#endif