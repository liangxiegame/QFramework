#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_IOS || UNITY_ANDROID || UNITY_WSA
#define WEBRTC_AUDIO_DSP_SUPPORTED_PLATFORM
#endif

#if UNITY_EDITOR_WIN || UNITY_EDITOR_OSX
#define WEBRTC_AUDIO_DSP_SUPPORTED_EDITOR
#endif

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Photon.Voice.Unity
{
    [RequireComponent(typeof(Recorder))]
    [DisallowMultipleComponent]
    public class WebRtcAudioDsp : VoiceComponent
    {
        #region Private Fields

        [SerializeField]
        private bool aec = true;
        
        [SerializeField]
        private bool aecHighPass;

        [SerializeField]
        private bool agc = true;

        [SerializeField]
        private int agcCompressionGain = 9;

        [SerializeField]
        private bool vad = true;

        [SerializeField]
        private bool highPass;

        [SerializeField]
        private bool bypass;

        [SerializeField]
        private bool noiseSuppression;

        [SerializeField]
        private int reverseStreamDelayMs = 120;

        private int reverseChannels;
        private WebRTCAudioProcessor proc;

        private AudioListener audioListener;
        private AudioOutCapture audioOutCapture;
        private bool aecStarted;
        private bool autoDestroyAudioOutCapture;

        private static readonly Dictionary<AudioSpeakerMode, int> channelsMap = new Dictionary<AudioSpeakerMode, int>
        {
            #if !UNITY_2019_2_OR_NEWER
            {AudioSpeakerMode.Raw, 0},
            #endif
            {AudioSpeakerMode.Mono, 1},
            {AudioSpeakerMode.Stereo, 2},
            {AudioSpeakerMode.Quad, 4},
            {AudioSpeakerMode.Surround, 5},
            {AudioSpeakerMode.Mode5point1, 6},
            {AudioSpeakerMode.Mode7point1, 8},
            {AudioSpeakerMode.Prologic, 2}
        };

        private LocalVoiceAudioShort localVoice;
        private int outputSampleRate;

        private Recorder recorder;

        #if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
        [FormerlySerializedAs("forceNormalAecInMobile")]
        public bool ForceNormalAecInMobile;
        #endif

        [SerializeField]
        private bool aecOnlyWhenEnabled = true;

        public bool AutoRestartOnAudioChannelsMismatch = true;

        private object threadSafety = new object();

        #endregion

        #region Properties

        public bool AEC
        {
            get
            {
                lock (this.threadSafety)
                {
                    if (this.IsInitialized && (!this.aecOnlyWhenEnabled || this.isActiveAndEnabled))
                    {
                        return this.aecStarted;
                    }
                }
                return this.aec;
            }
            set
            {
                if (value == this.aec)
                {
                    return;
                }
                this.aec = value;
                lock (this.threadSafety)
                {
                    this.ToggleAec();
                }
            }
        }

        [Obsolete("Use AEC instead on all platforms, internally according AEC will be used either mobile or not.")]
        public bool AECMobile // echo control mobile
        {
            get { return this.AEC; }
            set
            {
                this.AEC = value;
            }
        }

        [Obsolete("Obsolete as it's not recommended to set this to true. https://forum.photonengine.com/discussion/comment/48017/#Comment_48017")]
        public bool AECMobileComfortNoise;

        public bool AecHighPass
        {
            get { return this.aecHighPass; }
            set
            {
                if (value == this.aecHighPass)
                {
                    return;
                }
                this.aecHighPass = value;
                lock (this.threadSafety)
                {
                    if (this.IsInitialized)
                    {
                        this.proc.AECHighPass = this.aecHighPass;
                    }
                }
            }
        }

        public int ReverseStreamDelayMs
        {
            get { return this.reverseStreamDelayMs; }
            set
            {
                if (this.reverseStreamDelayMs == value)
                {
                    return;
                }
                this.reverseStreamDelayMs = value;
                lock (this.threadSafety)
                {
                    if (this.IsInitialized)
                    {
                        this.proc.AECStreamDelayMs = this.reverseStreamDelayMs;
                    } 
                }
            }
        }

        public bool NoiseSuppression
        {
            get { return this.noiseSuppression; }
            set
            {
                if (value == this.noiseSuppression)
                {
                    return;
                }
                this.noiseSuppression = value;
                lock (this.threadSafety)
                {
                    if (this.IsInitialized)
                    {
                        this.proc.NoiseSuppression = this.noiseSuppression;
                    }
                }
            }
        }

        public bool HighPass
        {
            get { return this.highPass; }
            set
            {
                if (value == this.highPass)
                {
                    return;
                }
                this.highPass = value;
                lock (this.threadSafety)
                {
                    if (this.IsInitialized)
                    {
                        this.proc.HighPass = this.highPass;
                    }
                }
            }
        }

        public bool Bypass
        {
            get { return this.bypass; }
            set
            {
                if (value == this.bypass)
                {
                    return;
                }
                this.bypass = value;
                if (this.IsInitialized)
                {
                    this.proc.Bypass = this.bypass;
                }
            }
        }

        public bool AGC
        {
            get { return this.agc; }
            set
            {
                if (value == this.agc)
                {
                    return;
                }
                this.agc = value;
                lock (this.threadSafety)
                {
                    if (this.IsInitialized)
                    {
                        this.proc.AGC = this.agc;
                    }
                }
            }
        }

        public int AgcCompressionGain
        {
            get
            {
                return this.agcCompressionGain;
            }
            set
            {
                if (this.agcCompressionGain == value)
                {
                    return;
                }
                if (value < 0 || value > 90)
                {
                    if (this.Logger.IsErrorEnabled)
                    {
                        this.Logger.LogError("AgcCompressionGain value {0} not in range [0..90]", value);
                    }
                    return;
                }
                this.agcCompressionGain = value;
                lock (this.threadSafety)
                {
                    if (this.IsInitialized)
                    {
                        this.proc.AGCCompressionGain = this.agcCompressionGain;
                    }
                }
            }
        }

        public bool VAD
        {
            get { return this.vad; }
            set
            {
                if (value == this.vad)
                {
                    return;
                }
                this.vad = value;
                lock (this.threadSafety)
                {
                    if (this.IsInitialized)
                    {
                        this.proc.VAD = this.vad;
                    }
                }
            }
        }

        public bool IsInitialized
        {
            get
            {
                return this.proc != null;
            }
        }

        public bool AecOnlyWhenEnabled
        {
            get
            {
                return this.aecOnlyWhenEnabled;
            }
            set
            {
                if (this.aecOnlyWhenEnabled != value)
                {
                    this.aecOnlyWhenEnabled = value;
                    lock (this.threadSafety)
                    {
                        this.ToggleAec();
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        protected override void Awake()
        {
            base.Awake();
            AudioSettings.OnAudioConfigurationChanged += this.OnAudioConfigurationChanged;
            if (this.SupportedPlatformCheck())
            {
                this.recorder = this.GetComponent<Recorder>();
                if (ReferenceEquals(null, this.recorder) || !this.recorder)
                {
                    if (this.Logger.IsErrorEnabled)
                    {
                        this.Logger.LogError("A Recorder component needs to be attached to the same GameObject");
                    }
                    this.enabled = false;
                    return;
                }
                if (!this.IgnoreGlobalLogLevel)
                {
                    this.LogLevel = this.recorder.LogLevel;
                }
            }
        }

        private void OnEnable()
        {
            lock (this.threadSafety)
            {
                if (this.SupportedPlatformCheck())
                {
                    if (this.IsInitialized)
                    {
                        this.ToggleAec();
                    } 
                    else if (this.recorder.IsRecording)
                    {
                        if (this.Logger.IsWarningEnabled)
                        {
                            this.Logger.LogWarning("WebRtcAudioDsp is added after recording has started, restarting recording to take effect");
                        }
                        this.recorder.RestartRecording(true);
                    }
                }
            }
        }

        private void OnDisable()
        {
            lock (this.threadSafety)
            {
                if (this.aecOnlyWhenEnabled && this.aecStarted)
                {
                    this.ToggleAecOutputListener(false);
                }
            }
        }

        private bool SupportedPlatformCheck()
        {
            #if WEBRTC_AUDIO_DSP_SUPPORTED_PLATFORM
            return true;
            #elif WEBRTC_AUDIO_DSP_SUPPORTED_EDITOR
            if (this.Logger.IsWarningEnabled)
            {
                this.Logger.LogWarning("WebRtcAudioDsp is not supported on this target platform {0}. The component will be disabled in build.", CurrentPlatform);
            }
            return true;
            #else
            if (this.Logger.IsErrorEnabled)
            {
                this.Logger.LogError("WebRtcAudioDsp is not supported on this platform {0}. The component will be disabled.", CurrentPlatform);
            }
            this.enabled = false;
            return false;
            #endif
        }

        private void ToggleAec()
        {
            if (this.IsInitialized && (!this.aecOnlyWhenEnabled || this.isActiveAndEnabled) && this.aec != this.aecStarted)
            {
                if (this.Logger.IsDebugEnabled)
                {
                    this.Logger.LogDebug("Toggling AEC to {0}", this.aec);
                }
                if (!this.ToggleAecOutputListener(this.aec))
                {
                    if (this.Logger.IsErrorEnabled)
                    {
                        this.Logger.LogError("AEC failed to be toggled to {0}", this.aec);
                    }
                }
                else if (this.Logger.IsDebugEnabled)
                {
                    this.Logger.LogDebug("AEC successfully toggled to {0}", this.aec);
                }
            }
        }

        private bool ToggleAecOutputListener(bool on)
        {
            if (on != this.aecStarted)
            {
                if (on)
                {
                    if (this.aecOnlyWhenEnabled && !this.isActiveAndEnabled)
                    {
                        if (this.Logger.IsErrorEnabled)
                        {
                            this.Logger.LogError("Could not start AEC because AecOnlyWhenEnabled is true and isActiveAndEnabled is false");
                        }
                        return false;
                    }
                    if (ReferenceEquals(null, this.audioOutCapture) || !this.audioOutCapture)
                    {
                        if (!this.InitAudioOutCapture())
                        {
                            if (this.Logger.IsErrorEnabled)
                            {
                                this.Logger.LogError("Could not start AEC OutputListener because a valid AudioOutCapture could not be set.");
                            }
                            return false;
                        }
                    }
                    else
                    {
                        if (!this.AudioOutCaptureChecks(this.audioOutCapture, true))
                        {
                            if (this.Logger.IsErrorEnabled)
                            {
                                this.Logger.LogError("Could not start AEC OutputListener because AudioOutCapture provided is not valid.");
                            }
                            return false;
                        }
                        AudioListener listener = this.audioOutCapture.GetComponent<AudioListener>();
                        if (this.audioListener != listener)
                        {
                            if (this.Logger.IsWarningEnabled)
                            {
                                this.Logger.LogWarning("Unexpected: AudioListener changed but AudioOutCapture did not.");
                            }
                            this.audioListener = listener;
                        }
                    }
                    if (this.IsInitialized) 
                    {
                        this.StartAec();
                    }
                }
                else 
                {
                    if (this.UnsubscribeFromAudioOutCapture(this.autoDestroyAudioOutCapture))
                    {
                        if (this.Logger.IsDebugEnabled)
                        {
                            this.Logger.LogDebug("AEC OutputListener stopped.");
                        }
                    }
                    else if (this.Logger.IsWarningEnabled)
                    {
                        this.Logger.LogWarning("Unexpected: AudioOutCapture is null but aecStarted == true");
                    }
                    if (this.IsInitialized)
                    {
                        this.proc.AEC = false;
                        this.proc.AECMobile = false;
                    }
                    else if (this.Logger.IsWarningEnabled)
                    {
                        this.Logger.LogWarning("Unexpected: proc is null but aecStarted was true.");
                    }
                    this.aecStarted = false;
                }
                return true;
            }
            return false;
        }

        private void StartAec()
        {
            this.proc.AECStreamDelayMs = this.reverseStreamDelayMs;
            this.proc.AECHighPass = this.aecHighPass;
            #if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
            this.proc.AEC = this.ForceNormalAecInMobile;
            this.proc.AECMobile = !this.ForceNormalAecInMobile;
            #else
            this.proc.AEC = true;
            this.proc.AECMobile = false;
            #endif
            this.aecStarted = true;
            this.audioOutCapture.OnAudioFrame += this.OnAudioOutFrameFloat;
            if (this.Logger.IsDebugEnabled)
            {
                this.Logger.LogDebug("AEC OutputListener started.");
            }
        }

        private void OnAudioConfigurationChanged(bool deviceWasChanged)
        {
            lock (this.threadSafety)
            {
                if (this.IsInitialized)
                {
                    bool restart = false;
                    if (this.outputSampleRate != AudioSettings.outputSampleRate)
                    {
                        if (this.Logger.IsInfoEnabled)
                        {
                            this.Logger.LogInfo("AudioConfigChange: outputSampleRate from {0} to {1}. WebRtcAudioDsp will be restarted.", this.outputSampleRate, AudioSettings.outputSampleRate);
                        }
                        this.outputSampleRate = AudioSettings.outputSampleRate;
                        restart = true;
                    }
                    if (this.reverseChannels != channelsMap[AudioSettings.speakerMode])
                    {
                        if (this.Logger.IsInfoEnabled)
                        {
                            this.Logger.LogInfo("AudioConfigChange: speakerMode channels from {0} to {1}. WebRtcAudioDsp will be restarted.", this.reverseChannels, channelsMap[AudioSettings.speakerMode]);
                        }
                        this.reverseChannels = channelsMap[AudioSettings.speakerMode];
                        restart = true;
                    }
                    if (restart)
                    {
                        this.Restart();
                    }
                }
            }
        }

        // triggered by OnAudioFilterRead which is called on a different thread from the main thread (namely the audio thread)
        // so calling into many Unity functions from this function is not allowed (if you try, a warning shows up at run time)
        private void OnAudioOutFrameFloat(float[] data, int outChannels)
        {
            lock (this.threadSafety)
            {
                if (!this.IsInitialized)
                {
                    if (this.Logger.IsErrorEnabled)
                    {
                        this.Logger.LogError("Unexpected: OnAudioOutFrame called while WebRtcAudioDsp is not initialized (proc == null).");
                    }
                    return;
                }
                if (!this.aecStarted)
                {
                    if (this.Logger.IsErrorEnabled)
                    {
                        this.Logger.LogError("Unexpected: OnAudioOutFrame called while aecStarted is false.");
                    }
                }
                if (outChannels != this.reverseChannels)
                {
                    if (this.Logger.IsErrorEnabled)
                    {
                        this.Logger.LogError("Unexpected: OnAudioOutFrame channel count {0} != initialized {1}. Switching channels and restarting.", outChannels, this.reverseChannels);
                    }
                    if (this.AutoRestartOnAudioChannelsMismatch)
                    {
                        this.reverseChannels = outChannels;
                        this.Restart();
                    }
                    return;
                }
                this.proc.OnAudioOutFrameFloat(data);
            }
        }

        // Unity message sent by Recorder
        private void PhotonVoiceCreated(PhotonVoiceCreatedParams p)
        {
            lock (this.threadSafety)
            {
                if (!this.enabled)
                {
                    if (this.Logger.IsInfoEnabled)
                    {
                        this.Logger.LogInfo("Skipped PhotonVoiceCreated message because component is disabled.");
                    }
                    return;
                }
                if (this.recorder != null && this.recorder.SourceType != Recorder.InputSourceType.Microphone)
                {
                    if (this.Logger.IsWarningEnabled)
                    {
                        this.Logger.LogWarning("WebRtcAudioDsp is better suited to be used with Microphone as Recorder Input Source Type.");
                    }
                }
                if (p.Voice.Info.Channels != 1)
                {
                    if (this.Logger.IsErrorEnabled)
                    {
                        this.Logger.LogError("Only mono audio signals supported. WebRtcAudioDsp component will be disabled.");
                    }
                    this.enabled = false;
                    return;
                }
                if (p.Voice is LocalVoiceAudioShort voice)
                {
                    this.localVoice = voice;
                    this.reverseChannels = channelsMap[AudioSettings.speakerMode];
                    this.outputSampleRate = AudioSettings.outputSampleRate;
                    this.Init();
                    this.localVoice.AddPostProcessor(this.proc);
                    this.ToggleAec();
                }
                else
                {
                    if (this.Logger.IsErrorEnabled)
                    {
                        this.Logger.LogError("Only short audio voice supported. WebRtcAudioDsp component will be disabled.");
                    }
                    this.enabled = false;
                }
            }
        }

        // Unity message sent by Recorder
        private void PhotonVoiceRemoved()
        {
            this.StopAllProcessing();
        }

        private void OnDestroy()
        {
            this.StopAllProcessing();
            AudioSettings.OnAudioConfigurationChanged -= this.OnAudioConfigurationChanged;
        }

        private void StopAllProcessing()
        {
            lock (this.threadSafety)
            {
                this.ToggleAecOutputListener(false);
                if (this.IsInitialized)
                {
                    this.proc.Dispose();
                    this.proc = null;
                }
                this.localVoice = null;   
            }
        }

        // called from different thread, do not call any Unity API
        private void Restart()
        {
            if (this.Logger.IsDebugEnabled)
            {
                this.Logger.LogDebug("Restarting");
            }
            if (this.IsInitialized)
            {
                bool aecWasStarted = false;
                if (this.aecStarted)
                {
                    if (this.UnsubscribeFromAudioOutCapture(false))
                    {
                        if (this.Logger.IsDebugEnabled)
                        {
                            this.Logger.LogDebug("AEC OutputListener stopped.");
                        }
                        aecWasStarted = true;
                        this.aecStarted = false;
                    }
                    else if (this.Logger.IsWarningEnabled)
                    {
                        this.Logger.LogWarning("Unexpected: AudioOutCapture is null but aecStarted == true");
                    }
                }
                this.proc.Dispose();
                this.proc = null;
                if (this.Init())
                {
                    this.localVoice.AddPostProcessor(this.proc);
                    if (aecWasStarted)
                    {
                        this.StartAec();
                    }
                    if (this.Logger.IsInfoEnabled)
                    {
                        this.Logger.LogInfo("Restart complete successfully.");
                    }
                }
                else if (this.Logger.IsErrorEnabled)
                {
                    this.Logger.LogError("Restart failed because processor could not be re initialized.");
                }
            } 
            else if (this.Logger.IsErrorEnabled)
            {
                this.Logger.LogError("Cannot restart if not initialized.");
            }
        }

        private bool Init()
        {
            if (this.IsInitialized)
            {
                if (this.Logger.IsErrorEnabled)
                {
                    this.Logger.LogError("Already initialized");
                }
                return false;
            }
            this.proc = new WebRTCAudioProcessor(this.Logger, this.localVoice.Info.FrameSize, this.localVoice.Info.SamplingRate,
                this.localVoice.Info.Channels, this.outputSampleRate, this.reverseChannels);
            this.proc.HighPass = this.highPass;
            this.proc.NoiseSuppression = this.noiseSuppression;
            this.proc.AGC = this.agc;
            this.proc.AGCCompressionGain = this.agcCompressionGain;
            this.proc.VAD = this.vad;
            this.proc.Bypass = this.bypass;
            if (this.Logger.IsInfoEnabled)
            {
                this.Logger.LogInfo("Initialized");
            }
            return true;
        }

        private bool SetOrSwitchAudioListener(AudioListener listener, bool extraChecks, bool log = true)
        {
            if (extraChecks && !this.AudioListenerChecks(listener))
            {
                return false;
            }
            // multiple AudioOutCapture could be added to same GameObject
            AudioOutCapture[] captures = listener.GetComponents<AudioOutCapture>();
            if (captures.Length > 1)
            {
                if (this.Logger.IsDebugEnabled)
                {
                    this.Logger.LogDebug("{0} AudioOutCapture components attached to the same GameObject, is this expected?", captures.Length);
                }
            }
            for (int i = 0; i < captures.Length; i++)
            {
                if (this.SetOrSwitchAudioOutCapture(captures[i], false, false))
                {
                    this.autoDestroyAudioOutCapture = false;
                    return true;
                }
            }
            // in case we fail to set any available AudioOutCapture, let's add a new one
            AudioOutCapture capture = listener.gameObject.AddComponent<AudioOutCapture>();
            if (this.SetOrSwitchAudioOutCapture(capture, false, log))
            {
                if (this.Logger.IsDebugEnabled)
                {
                    this.Logger.LogDebug("AudioOutCapture component added to same GameObject as AudioListener.");
                }
                this.autoDestroyAudioOutCapture = true;
                return true;
            }
            Destroy(capture);
            return false;
        }

        private bool SetOrSwitchAudioOutCapture(AudioOutCapture capture, bool extraChecks, bool log = true)
        {
            if (!this.AudioOutCaptureChecks(capture, extraChecks, log))
            {
                return false;
            }
            bool aecWasStarted = this.aecStarted;
            bool audioOutSwitched = false;
            if (!ReferenceEquals(null, this.audioOutCapture) && this.audioOutCapture)
            {
                if (this.audioOutCapture != capture)
                {
                    if (!this.UnsubscribeFromAudioOutCapture(this.autoDestroyAudioOutCapture))
                    {
                        if (this.Logger.IsErrorEnabled) 
                        {
                            this.Logger.LogError("Could not unsubscribe from previous AudioOutCapture. Switching to a new one won't happen.");
                        }
                        return false;
                    }
                    audioOutSwitched = true;
                }
                else if (extraChecks)
                {
                    if (log && this.Logger.IsErrorEnabled)
                    {
                        this.Logger.LogError("The same AudioOutCapture is being used already");
                    }
                    return false;
                }
            }
            this.audioOutCapture = capture;
            this.audioListener = capture.GetComponent<AudioListener>();
            if (aecWasStarted && audioOutSwitched)
            {
                this.audioOutCapture.OnAudioFrame += this.OnAudioOutFrameFloat;
            }
            return true;
        }

        private bool InitAudioOutCapture()
        {
            if (!ReferenceEquals(null, this.audioOutCapture) && this.audioOutCapture)
            {
                if (this.Logger.IsErrorEnabled)
                {
                    this.Logger.LogError("AudioOutCapture is already initialized.");
                }
                return false;
            }
            if (ReferenceEquals(null, this.audioListener))
            {
                AudioOutCapture[] audioOutCaptures = FindObjectsOfType<AudioOutCapture>();
                if (audioOutCaptures.Length > 1)
                {
                    if (this.Logger.IsDebugEnabled)
                    {
                        this.Logger.LogDebug("{0} AudioOutCapture components found, is this expected?", audioOutCaptures.Length);
                    }
                }
                for(int i=0; i < audioOutCaptures.Length; i++)
                {
                    AudioOutCapture capture = audioOutCaptures[i];
                    if (this.SetOrSwitchAudioOutCapture(capture, true, false))
                    {
                        this.autoDestroyAudioOutCapture = false;
                        return true;
                    }
                }
                AudioListener[] audioListeners = FindObjectsOfType<AudioListener>();
                if (audioListeners.Length == 0)
                {
                    if (this.Logger.IsErrorEnabled)
                    {
                        this.Logger.LogError("No AudioListener component found, is this expected?");
                    }
                }
                else if (audioListeners.Length > 1 && this.Logger.IsDebugEnabled)
                {
                    this.Logger.LogDebug("{0} AudioListener components found, is this expected?", audioListeners.Length);
                }
                for(int i=0; i < audioListeners.Length; i++)
                {
                    AudioListener listener = audioListeners[i];
                    if (this.SetOrSwitchAudioListener(listener, true, false))
                    {
                        return true;
                    }
                }
                if (this.Logger.IsErrorEnabled) 
                {
                    this.Logger.LogError("AudioListener and AudioOutCapture components are required for AEC to work.");
                }
                return false;
            }
            return this.SetOrSwitchAudioListener(this.audioListener, true);
        }

        private bool UnsubscribeFromAudioOutCapture(bool destroy)
        {
            if (!ReferenceEquals(null, this.audioOutCapture))
            {
                if (this.aecStarted)
                {
                    this.audioOutCapture.OnAudioFrame -= this.OnAudioOutFrameFloat;
                    if (this.Logger.IsDebugEnabled)
                    {
                        this.Logger.LogDebug("OnAudioFrame event unsubscribed.");
                    }
                }
                if (destroy)
                {
                    Destroy(this.audioOutCapture);
                    if (this.Logger.IsDebugEnabled)
                    {
                        this.Logger.LogDebug("AudioOutCapture component destroyed.");
                    }
                    this.audioOutCapture = null;
                }
                return true;
            } 
            if (this.aecStarted && this.Logger.IsErrorEnabled)
            {
                this.Logger.LogError("Unexpected: audioOutCapture is null but aecStarted is true");
            }
            return false;
        }
        
        private bool AudioListenerChecks(AudioListener listener, bool log = true)
        {
            if (ReferenceEquals(listener, null))
            {
                if (log && this.Logger.IsErrorEnabled) 
                {
                    this.Logger.LogError("AudioListener is null.");
                }
                return false;
            }
            if (!listener)
            {
                if (log && this.Logger.IsErrorEnabled) 
                {
                    this.Logger.LogError("AudioListener is destroyed.");
                }
                return false;
            }
            if (!listener.gameObject.activeInHierarchy) 
            {
                if (log && this.Logger.IsErrorEnabled) 
                {
                    this.Logger.LogError("The GameObject to which the AudioListener is attached is not active in hierarchy.");
                }
                return false;
            }
            if (!listener.enabled) 
            {
                if (log && this.Logger.IsErrorEnabled) 
                {
                    this.Logger.LogError("AudioListener is disabled.");
                }
                return false;
            }
            return true;
        }

        private bool AudioOutCaptureChecks(AudioOutCapture capture, bool listenerChecks, bool log = true)
        {
            if (ReferenceEquals(capture, null))
            {
                if (log && this.Logger.IsErrorEnabled) 
                {
                    this.Logger.LogError("AudioOutCapture is null.");
                }
                return false;
            }
            if (!capture)
            {
                if (log && this.Logger.IsErrorEnabled) 
                {
                    this.Logger.LogError("AudioOutCapture is destroyed.");
                }
                return false;
            }
            if (!listenerChecks && !capture.gameObject.activeInHierarchy) 
            {
                if (log && this.Logger.IsErrorEnabled) 
                {
                    this.Logger.LogError("The GameObject to which the AudioOutCapture is attached is not active in hierarchy.");
                }
                return false;
            }
            if (!capture.enabled) 
            {
                if (log && this.Logger.IsErrorEnabled) 
                {
                    this.Logger.LogError("AudioOutCapture is disabled.");
                }
                return false;
            }
            return !listenerChecks || this.AudioListenerChecks(capture.GetComponent<AudioListener>(), log);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Set the AudioListener to be used with this WebRtcAudioDsp. Needed for Acoustic Echo Cancellation.
        /// </summary>
        /// <param name="listener">The audioListener to be used</param>
        /// <returns>Success or failure</returns>
        public bool SetOrSwitchAudioListener(AudioListener listener)
        {
            lock (this.threadSafety)
            {
                return this.SetOrSwitchAudioListener(listener, true);
            }
        }

        /// <summary>
        /// Set the AudioOutCapture to be used with this WebRtcAudioDsp. Needed for Acoustic Echo Cancellation.
        /// </summary>
        /// <param name="capture">The audioOutCapture to be used</param>
        /// <returns>Success or failure</returns>
        public bool SetOrSwitchAudioOutCapture(AudioOutCapture capture)
        {
            lock (this.threadSafety)
            {
                if (this.SetOrSwitchAudioOutCapture(capture, true))
                {
                    this.autoDestroyAudioOutCapture = false;
                    return true;
                }
                return false;
            }
        }

        #endregion
    }
}