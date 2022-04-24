#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_SWITCH || UNITY_IOS
#define PHOTON_AUDIO_CHANGE_IN_NOTIFIER
#endif

namespace Photon.Voice.Unity
{
    using Voice;
    using UnityEngine;

    /// <summary>
    /// This component is useful to handle audio device and config changes.
    /// </summary>
    [RequireComponent(typeof(Recorder))]
    public class AudioChangesHandler : VoiceComponent
    {
        private IAudioInChangeNotifier photonMicChangeNotifier;
        private AudioConfiguration audioConfiguration;
        private Recorder recorder;

        /// <summary>
        /// Try to start recording when we get devices change notification and recording is not started.
        /// </summary>
        /// <remarks>
        /// On some platforms we can't make sure that a device change notification could mean that at least a microphone device is now available.
        /// Besides, the auto start of the recording might not happen if other necessary conditions set in Recorder are not met:
        /// e.g. <see cref="Recorder.RecordOnlyWhenEnabled"/> or <see cref="Recorder.RecordOnlyWhenJoined"/> etc.
        /// or if the Recorder has been stopped explicitly via <see cref="Recorder.StopRecording"/> call or <see cref="Recorder.IsRecording"/> set to false.
        /// </remarks>
        [Tooltip("Try to start recording when we get devices change notification and recording is not started.")]
        public bool StartWhenDeviceChange = true;
        /// <summary>
        /// Try to react to device change notification when Recorder is started.
        /// </summary>
        /// <remarks>
        /// This requires <see cref="UseNativePluginChangeNotifier"/> or <see cref="UseOnAudioConfigurationChanged"/> to be true.
        /// </remarks>
        [Tooltip("Try to react to device change notification when Recorder is started.")]
        public bool HandleDeviceChange = true;
        /// <summary>
        /// Try to react to audio config change notification when Recorder is started.
        /// </summary>
        /// <remarks>
        /// This requires <see cref="UseOnAudioConfigurationChanged"/> to be true.
        /// </remarks>
        [Tooltip("Try to react to audio config change notification when Recorder is started.")]
        public bool HandleConfigChange = true;
        /// <summary>
        /// Whether or not to make use of Photon's AudioInChangeNotifier native plugin.
        /// </summary>
        /// <remarks>
        /// This may disable <see cref="HandleDeviceChange"/> if this and <see cref="UseOnAudioConfigurationChanged"/> are both false.
        /// </remarks>
        [Tooltip("Whether or not to make use of Photon's AudioInChangeNotifier native plugin.")]
        public bool UseNativePluginChangeNotifier = true;
        /// <summary>
        /// Whether or not to make use of Unity's OnAudioConfigurationChanged.
        /// </summary>
        /// <remarks>
        /// This is needed for <see cref="HandleConfigChange"/> and may also disable
        /// <see cref="HandleDeviceChange"/> if this and <see cref="UseNativePluginChangeNotifier"/> are both false.
        /// </remarks>
        [Tooltip("Whether or not to make use of Unity's OnAudioConfigurationChanged.")]
        public bool UseOnAudioConfigurationChanged = true;

        #if UNITY_EDITOR || UNITY_ANDROID
        /// <summary>
        /// If the recorder is set to use microphone as source with type Photon, audio device changes are handled within the native plugin by default.
        /// If you set this to true, it will also be handled via this component logic.
        /// </summary>
        [Tooltip("If the recorder is set to use microphone as source with type Photon, audio device changes are handled within the native plugin by default. " +
                 "If you set this to true, it will also be handled via this component logic.")]
        public bool Android_AlwaysHandleDeviceChange;
        #endif
        #if UNITY_EDITOR || UNITY_IOS
        /// <summary>
        /// If the recorder is set to use microphone as source with type Photon, audio device changes are handled within the native plugin by default.
        /// If you set this to true, it will also be handled via this component logic.
        /// </summary>
        [Tooltip("If the recorder is set to use microphone as source with type Photon, audio device changes are handled within the native plugin by default. " +
                 "If you set this to true, it will also be handled via this component logic.")]
        public bool iOS_AlwaysHandleDeviceChange;
        #endif

        private bool subscribedToSystemChangesPhoton, subscribedToSystemChangesUnity;

        protected override void Awake()
        {
            base.Awake();
            this.recorder = this.GetComponent<Recorder>();
            this.recorder.ReactOnSystemChanges = false;
            //if (this.recorder.AutoStart) {
            //    if (this.AutoStartWhenMicrophoneMightBeAvailable && this.CheckIfMicrophoneIsAvailable) {
            //        this.recorder.AutoStart = false;
            //        if (this.micAvailable) {
            //            this.recorder.CheckAndAutoStart(true);
            //        }
            //    } else if (this.recorder.IsRecording) {
            //        this.recorder.AutoStart = false;
            //    }
            //}
            this.audioConfiguration = AudioSettings.GetConfiguration();
            this.SubscribeToSystemChanges();
        }

        private void OnDestroy()
        {
            this.UnsubscribeFromSystemChanges();
        }

        #if PHOTON_AUDIO_CHANGE_IN_NOTIFIER
        private void PhotonMicrophoneChangeDetected() 
        {
            if (this.Logger.IsDebugEnabled)
            {
                this.Logger.LogDebug("Microphones change detected by Photon native plugin.");
            }
            if (!this.recorder.MicrophoneDeviceChangeDetected && this.UseNativePluginChangeNotifier)
            {
                this.OnDeviceChange();
            }
        }
        #endif

        private void OnDeviceChange()
        {
            if (!this.recorder.IsRecording)
            {
                if (this.StartWhenDeviceChange)
                {
                    this.recorder.MicrophoneDeviceChangeDetected = true;
                    if (this.Logger.IsInfoEnabled)
                    {
                        this.Logger.LogInfo("An attempt to auto start recording should follow shortly.");
                    }
                }
                else if (this.Logger.IsInfoEnabled)
                {
                    this.Logger.LogInfo("Device change detected but will not try to start recording as StartWhenDeviceChange is false.");
                }
            }
            else if (this.HandleDeviceChange)
            {
                #if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
                if (this.recorder.SourceType == Recorder.InputSourceType.Microphone && this.recorder.MicrophoneType == Recorder.MicType.Photon)
                {
                    #if UNITY_ANDROID
                    if (!this.Android_AlwaysHandleDeviceChange)
                    {
                    #elif UNITY_IOS
                    if (!this.iOS_AlwaysHandleDeviceChange)
                    {
                    #endif
                        if (this.Logger.IsInfoEnabled)
                        {
                            this.Logger.LogInfo("Device change notification ignored when using Photon microphone type as this is handled internally for iOS and Android via native plugins.");
                        }
                        return;
                    }
                }
                #endif
                this.recorder.MicrophoneDeviceChangeDetected = true;
            }
            else if (this.Logger.IsInfoEnabled)
            {
                this.Logger.LogInfo("Device change detected but will not try to handle this as HandleDeviceChange is false.");
            }
        }

        private void SubscribeToSystemChanges()
        {
            if (this.Logger.IsDebugEnabled)
            {
                this.Logger.LogDebug("Subscribing to system (audio) changes.");
            }
            #if PHOTON_AUDIO_CHANGE_IN_NOTIFIER
            if (this.subscribedToSystemChangesPhoton)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Already subscribed to audio changes via Photon.");
                }
            } 
            else 
            {
                this.photonMicChangeNotifier = Platform.CreateAudioInChangeNotifier(this.PhotonMicrophoneChangeDetected, this.Logger);
                if (this.photonMicChangeNotifier.IsSupported)
                {
                    if (this.photonMicChangeNotifier.Error == null)
                    {
                        this.subscribedToSystemChangesPhoton = true;
                        if (this.Logger.IsInfoEnabled) 
                        {
                            this.Logger.LogInfo("Subscribed to audio in change notifications via Photon plugin.");
                        }
                    }
                    if (this.Logger.IsErrorEnabled)
                    {
                        this.Logger.LogError("Error creating instance of photonMicChangeNotifier: {0}", this.photonMicChangeNotifier.Error);
                    }
                } 
                else if (this.Logger.IsErrorEnabled)
                {
                    this.Logger.LogError("Unexpected: Photon's AudioInChangeNotifier not supported on current platform: {0}", CurrentPlatform);
                }
                if (!this.subscribedToSystemChangesPhoton)
                {
                    this.photonMicChangeNotifier.Dispose();
                    this.photonMicChangeNotifier = null;
                }
            }
            #else
            if (this.Logger.IsInfoEnabled)
            {
                this.Logger.LogInfo("Skipped subscribing to audio change notifications via Photon's AudioInChangeNotifier as not supported on current platform: {0}", CurrentPlatform);
            }
            if (this.subscribedToSystemChangesPhoton)
            {
                if (this.Logger.IsErrorEnabled)
                {
                    this.Logger.LogError("Unexpected: subscribedToSystemChangesPhoton is set to true while platform is not supported!.");
                }
            }
            #endif
            if (this.subscribedToSystemChangesUnity)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Already subscribed to audio changes via Unity OnAudioConfigurationChanged callback.");
                }
            }
            else
            {
                AudioSettings.OnAudioConfigurationChanged += this.OnAudioConfigChanged;
                this.subscribedToSystemChangesUnity = true;
                if (this.Logger.IsInfoEnabled)
                {
                    this.Logger.LogInfo("Subscribed to audio configuration changes via Unity OnAudioConfigurationChanged callback.");
                }
            }
        }

        private void OnAudioConfigChanged(bool deviceWasChanged)
        {
            if (this.Logger.IsInfoEnabled)
            {
                this.Logger.LogInfo("OnAudioConfigurationChanged: {0}", deviceWasChanged ? "Device was changed." : "AudioSettings.Reset was called.");
            }
            AudioConfiguration config = AudioSettings.GetConfiguration();
            bool audioConfigChanged = false;
            if (config.dspBufferSize != this.audioConfiguration.dspBufferSize)
            {
                if (this.Logger.IsInfoEnabled)
                {
                    this.Logger.LogInfo("OnAudioConfigurationChanged: dspBufferSize old={0} new={1}", this.audioConfiguration.dspBufferSize, config.dspBufferSize);
                }
                audioConfigChanged = true;
            }
            if (config.numRealVoices != this.audioConfiguration.numRealVoices)
            {
                if (this.Logger.IsInfoEnabled)
                {
                    this.Logger.LogInfo("OnAudioConfigurationChanged: numRealVoices old={0} new={1}", this.audioConfiguration.numRealVoices, config.numRealVoices);
                }
                audioConfigChanged = true;
            }
            if (config.numVirtualVoices != this.audioConfiguration.numVirtualVoices)
            {
                if (this.Logger.IsInfoEnabled)
                {
                    this.Logger.LogInfo("OnAudioConfigurationChanged: numVirtualVoices old={0} new={1}", this.audioConfiguration.numVirtualVoices, config.numVirtualVoices);
                }
                audioConfigChanged = true;
            }
            if (config.sampleRate != this.audioConfiguration.sampleRate)
            {
                if (this.Logger.IsInfoEnabled)
                {
                    this.Logger.LogInfo("OnAudioConfigurationChanged: sampleRate old={0} new={1}", this.audioConfiguration.sampleRate, config.sampleRate);
                }
                audioConfigChanged = true;
            }
            if (config.speakerMode != this.audioConfiguration.speakerMode)
            {
                if (this.Logger.IsInfoEnabled)
                {
                    this.Logger.LogInfo("OnAudioConfigurationChanged: speakerMode old={0} new={1}", this.audioConfiguration.speakerMode, config.speakerMode);
                }
                audioConfigChanged = true;
            }
            if (audioConfigChanged)
            {
                this.audioConfiguration = config;
            }
            if (!this.recorder.MicrophoneDeviceChangeDetected)
            {
                if (audioConfigChanged)
                {
                    if (this.recorder.IsRecording)
                    {
                        if (this.HandleConfigChange)
                        {
                            if (this.Logger.IsInfoEnabled)
                            {
                                this.Logger.LogInfo("Config change detected; an attempt to auto start recording should follow shortly.");
                            }
                            this.recorder.MicrophoneDeviceChangeDetected = true;
                        }
                        else if (this.Logger.IsInfoEnabled)
                        {
                            this.Logger.LogInfo("Config change detected but will not try to handle this as HandleConfigChange is false.");
                        }
                    }
                    else if (this.Logger.IsInfoEnabled)
                    {
                        this.Logger.LogInfo("Config change detected but ignored as recording not started.");
                    }
                }
                else if (deviceWasChanged)
                {
                    if (this.UseOnAudioConfigurationChanged)
                    {
                        this.OnDeviceChange();
                    }
                    else if (this.Logger.IsInfoEnabled)
                    {
                        this.Logger.LogInfo("Device change detected but will not try to handle this as UseOnAudioConfigurationChanged is false.");
                    }
                }
            }
        }

        private void UnsubscribeFromSystemChanges()
        {
            if (this.subscribedToSystemChangesUnity)
            {
                AudioSettings.OnAudioConfigurationChanged -= this.OnAudioConfigChanged;
                this.subscribedToSystemChangesUnity = false;
                if (this.Logger.IsInfoEnabled)
                {
                    this.Logger.LogInfo("Unsubscribed from audio changes via Unity OnAudioConfigurationChanged callback.");
                }
            }
            if (this.subscribedToSystemChangesPhoton)
            {
                if (this.photonMicChangeNotifier == null)
                {
                    if (this.Logger.IsErrorEnabled)
                    {
                        this.Logger.LogError("Unexpected: photonMicChangeNotifier is null while subscribedToSystemChangesPhoton is true.");
                    }
                }
                else
                {
                    this.photonMicChangeNotifier.Dispose();
                    this.photonMicChangeNotifier = null;
                }
                this.subscribedToSystemChangesPhoton = false;
                if (this.Logger.IsInfoEnabled)
                {
                    this.Logger.LogInfo("Unsubscribed from audio in change notifications via Photon plugin.");
                }
            }
        }
    }
}