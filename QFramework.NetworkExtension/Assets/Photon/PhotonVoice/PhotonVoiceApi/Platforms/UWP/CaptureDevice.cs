#if WINDOWS_UWP || ENABLE_WINMD_SUPPORT
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;

namespace Photon.Voice.UWP
{
    public delegate void MediaCaptureInitConmpleted(MediaCapture mediaCpture, bool ok);
    
    class CaptureDevice
    {        
        public enum Media
        {
            Audio,
            Video
        }
        private Media media;
        private string deviceID;
        // Media capture object
        private MediaCapture mediaCapture;
        // Custom media sink
        private MediaExtensions.MediaSinkProxy mediaSink;
        // Flag indicating if recording to custom sink has started
        private bool recordingStarted = false;
        private bool forwardEvents = false;
        private ILogger logger;

        internal MediaCapture MediaCapture { get { return mediaCapture; } }

        // Wraps the capture failed and media sink incoming connection events
        public event EventHandler<MediaCaptureFailedEventArgs> CaptureFailed;

        public CaptureDevice(ILogger logger, Media media, string deviceID)
        {
            this.logger = logger;
            this.media = media;
            this.deviceID = deviceID;
        }

        /// <summary>
        ///  Handler for the wrapped MediaCapture object's Failed event. It just wraps and forward's MediaCapture's 
        ///  Failed event as own CaptureFailed event
        /// </summary>
        private void mediaCapture_Failed(MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs)
        {
            if (CaptureFailed != null && forwardEvents) CaptureFailed(this, errorEventArgs);
        }

        /// <summary>
        /// Cleans up the resources.
        /// </summary>
        private void CleanupSink()
        {
            if (mediaSink != null)
            {
                mediaSink.Dispose();
                mediaSink = null;
                recordingStarted = false;
            }
        }

        private void DoCleanup()
        {
            if (mediaCapture != null)
            {
                mediaCapture.Failed -= mediaCapture_Failed;
                mediaCapture = null;
            }

            CleanupSink();
        }

        public void Initialize()
        {
            InitializeAsync();
        }

        public void InitializeAsync()
        {
            try
            {
                var settings = new MediaCaptureInitializationSettings();
                if (media == Media.Video)
                {
                    settings.StreamingCaptureMode = StreamingCaptureMode.Video;
                    settings.VideoDeviceId = deviceID;
                }
                else
                {
                    settings.StreamingCaptureMode = StreamingCaptureMode.Audio;
                    settings.AudioDeviceId = deviceID;
                }

                forwardEvents = true;

                if (mediaCapture != null)
                {
                    throw new InvalidOperationException("Camera is already initialized");
                }

                mediaCapture = new MediaCapture();
                mediaCapture.Failed += mediaCapture_Failed;

                var t = mediaCapture.InitializeAsync(settings);
                t.AsTask().Wait();
                lock (mediaCaptureInitedLock)
                {
                    mediaCaptureInited = true;
                    lastMediaCaptureInitStatus = t.Status == AsyncStatus.Completed;
                    if (MediaCaptureInitCompleted != null)
                    {
                        MediaCaptureInitCompleted(mediaCapture, t.Status == AsyncStatus.Completed);
                    }
                }
            }
            catch (Exception e)
            {
                DoCleanup();
                throw e;
            }
        }

        internal event MediaCaptureInitConmpleted MediaCaptureInitCompleted;
        object mediaCaptureInitedLock = new object();
        bool mediaCaptureInited;
        bool lastMediaCaptureInitStatus;
        internal void MediaCaptureInitCompletedAdd(MediaCaptureInitConmpleted x)
        {
            lock (mediaCaptureInitedLock)
            {
                if (mediaCaptureInited)
                {
                    x.Invoke(mediaCapture, lastMediaCaptureInitStatus);
                }
                MediaCaptureInitCompleted += x;
            }
        }
        /// <summary>
        /// Asynchronous method cleaning up resources and stopping recording if necessary.
        /// </summary>
        public async Task CleanUpAsync()
        {
            try
            {
                forwardEvents = true;

                if (mediaCapture == null && mediaSink == null) return;

                if (recordingStarted)
                {
                    await mediaCapture.StopRecordAsync();
                }

                DoCleanup();
            }
            catch (Exception)
            {
                DoCleanup();
            }
        }

        /// <summary>
        /// Creates url object from MediaCapture
        /// </summary>
        public MediaCapture CaptureSource
        {
            get { return mediaCapture; }
        }

        /// <summary>
        /// Allow selection of camera settings.
        /// </summary>
        /// <param name="mediaStreamType" type="Windows.Media.Capture.MediaStreamType">
        /// Type of a the media stream.
        /// </param>
        /// <param name="filterSettings" type="Func<Windows.Media.MediaProperties.IMediaEncodingProperties, bool>">
        /// A predicate function, which will be called to filter the correct settings.
        /// </param>
        public async Task<IMediaEncodingProperties> SelectPreferredCameraStreamSettingAsync(MediaStreamType mediaStreamType, Func<IMediaEncodingProperties, bool> filterSettings)
        {
            IMediaEncodingProperties previewEncodingProperties = null;

            if (mediaStreamType == MediaStreamType.Audio || mediaStreamType == MediaStreamType.Photo)
            {
                throw new ArgumentException("mediaStreamType value of MediaStreamType.Audio or MediaStreamType.Photo is not supported", "mediaStreamType");
            }
            if (filterSettings == null)
            {
                throw new ArgumentNullException("filterSettings");
            }

            var properties = mediaCapture.VideoDeviceController.GetAvailableMediaStreamProperties(mediaStreamType);
            var filterredProperties = properties.Where(filterSettings);
            var preferredSettings = filterredProperties.ToArray();

            Array.Sort<IMediaEncodingProperties>(preferredSettings, (x, y) =>
            {
                return (int)(((x as VideoEncodingProperties).Width) -
                    (y as VideoEncodingProperties).Width);
            });

            if (preferredSettings.Length > 0)
            {
                previewEncodingProperties = preferredSettings[0];
                await mediaCapture.VideoDeviceController.SetMediaStreamPropertiesAsync(mediaStreamType, preferredSettings[0]);
            }

            return previewEncodingProperties;
        }

        /// <summary>
        /// Starts media recording asynchronously
        /// </summary>
        /// <param name="encodingProfile">
        /// Encoding profile used for the recording session
        /// </param>
        public async Task StartRecordingAsync(MediaEncodingProfile encodingProfile, Action<byte[], FrameFlags> encoderCallback)
        {
            try
            {
                // We cannot start recording twice.
                if (mediaSink != null && recordingStarted)
                {
                    throw new InvalidOperationException("Recording already started.");
                }

                // Release sink if there is one already.
                CleanupSink();

                // Create new sink
                mediaSink = new MediaExtensions.MediaSinkProxy();
                if (encoderCallback != null)
                {
                    mediaSink.OutgoingPacketEvent += (object sender, MediaExtensions.Packet p) =>
                    {
                        encoderCallback(p.Buffer, p.Keyframe ? FrameFlags.KeyFrame : 0);
                    };
                }

                var mfExtension = await mediaSink.InitializeAsync(encodingProfile.Audio, encodingProfile.Video);
                await mediaCapture.StartRecordToCustomSinkAsync(encodingProfile, mfExtension);

                //var file = await Windows.Storage.KnownFolders.CameraRoll.CreateFileAsync("pop.mp4", Windows.Storage.CreationCollisionOption.GenerateUniqueName);
                //await mediaCapture.StartRecordToStorageFileAsync(encodingProfile, file);

                recordingStarted = true;
            }
            catch (Exception e)
            {
                CleanupSink();
                throw e;
            }
        }

        /// <summary>
        /// Stops recording asynchronously
        /// </summary>
        public async Task StopRecordingAsync()
        {
            if (recordingStarted)
            {
                try
                {
                    await mediaCapture.StopRecordAsync();
                    CleanupSink();
                }
                catch (Exception)
                {
                    CleanupSink();
                }
            }
        }

        public static async Task<bool> CheckForRecordingDeviceAsync()
        {
            var cameraFound = false;

            var devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            if (devices.Count > 0)
            {
                cameraFound = true;
            }

            return cameraFound;
        }
    }

    public class DeviceEnumerator : DeviceEnumeratorBase
    {
        Windows.Devices.Enumeration.DeviceClass deviceClass;

        public DeviceEnumerator(ILogger logger, Windows.Devices.Enumeration.DeviceClass deviceClass) : base(logger)
        {
            this.deviceClass = deviceClass;
            Refresh();
        }

        public override void Refresh()
        {
            var op = Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(deviceClass);
            op.AsTask().Wait();
            if (op.Status == Windows.Foundation.AsyncStatus.Error)
            {
                Error = op.ErrorCode.Message;
                return;
            }
            var r = op.GetResults();
            devices = new System.Collections.Generic.List<DeviceInfo>();
            for (int i = 0; i < r.Count; i++)
            {
                devices.Add(new DeviceInfo(r[i].Id, r[i].Name));
            }
        }

        public override void Dispose()
        {
        }
    }

    public class AudioInEnumerator : DeviceEnumerator
    {
        public AudioInEnumerator(ILogger logger) 
            : base(logger, Windows.Devices.Enumeration.DeviceClass.AudioCapture)
        {
        }
    }

    public class VideoInEnumerator : DeviceEnumerator
    {
        public VideoInEnumerator(ILogger logger)
            : base(logger, Windows.Devices.Enumeration.DeviceClass.VideoCapture)
        {
        }
    }
}
#endif
