#if FUSION_WEAVER
namespace Photon.Voice.Fusion
{
    using global::Fusion;
    using Unity;
    using UnityEngine;
    using ExitGames.Client.Photon;

    public class VoiceNetworkObject : NetworkBehaviour, ILoggableDependent
    {
        #region Private Fields

        private VoiceConnection voiceConnection;

        [SerializeField]
        private Speaker speakerInUse;

        [SerializeField]
        private Recorder recorderInUse;
        
        [SerializeField]
        protected DebugLevel logLevel = DebugLevel.ERROR;

        private VoiceLogger logger;

        [SerializeField, HideInInspector]
        private bool ignoreGlobalLogLevel;

        #endregion

        #region Public Fields

        /// <summary> If true, a Recorder component will be added to the same GameObject if not found already. </summary>
        public bool AutoCreateRecorderIfNotFound;
        /// <summary> If true, VoiceConnection.PrimaryRecorder will be used by this VoiceNetworkObject </summary>
        public bool UsePrimaryRecorder;
        /// <summary> If true, a Speaker component will be setup to be used for the DebugEcho mode </summary>
        public bool SetupDebugSpeaker;

        #endregion

        #region Properties

        public VoiceLogger Logger
        {
            get
            {
                if (this.logger == null)
                {
                    this.logger = new VoiceLogger(this, string.Format("{0}.{1}", this.name, this.GetType().Name), this.logLevel);
                }
                return this.logger;
            }
            protected set => this.logger = value;
        }

        public DebugLevel LogLevel
        {
            get
            {
                if (this.Logger != null)
                {
                    this.logLevel = this.Logger.LogLevel;
                }
                return this.logLevel;
            }
            set
            {
                this.logLevel = value;
                if (this.Logger == null)
                {
                    return;
                }
                this.Logger.LogLevel = this.logLevel;
            }
        }

        public bool IgnoreGlobalLogLevel
        {
            get => this.ignoreGlobalLogLevel;
            set => this.ignoreGlobalLogLevel = value;
        }

        /// <summary> The Recorder component currently used by this VoiceNetworkObject </summary>
        public Recorder RecorderInUse
        {
            get => this.recorderInUse;
            set
            {
                if (value != this.recorderInUse)
                {
                    this.recorderInUse = value;
                    this.IsRecorder = false;
                }
                if (this.RequiresRecorder)
                {
                    this.SetupRecorderInUse();
                }
                else if (this.IsNetworkObjectReady)
                {
                    if (this.Logger.IsWarningEnabled)
                    {
                        this.Logger.LogWarning("No need to set Recorder as this is a remote NetworkObject.");
                    }
                }
            }
        }

        /// <summary> The Speaker component currently used by this VoiceNetworkObject </summary>
        public Speaker SpeakerInUse
        {
            get => this.speakerInUse;
            set
            {
                if (this.speakerInUse != value)
                {
                    this.speakerInUse = value;
                    this.IsSpeaker = false;
                }
                if (this.RequiresSpeaker)
                {
                    this.SetupSpeakerInUse();
                }
                else if (this.IsNetworkObjectReady)
                {
                    if (this.Logger.IsWarningEnabled)
                    {
                        this.Logger.LogWarning("Speaker not set because this is a local NetworkObject and SetupDebugSpeaker is disabled.");
                    }
                }
            }
        }

        /// <summary> If true, this VoiceNetworkObject is setup and ready to be used </summary>
        public bool IsSetup => this.IsNetworkObjectReady && (!this.RequiresRecorder || this.IsRecorder) && (!this.RequiresSpeaker || this.IsSpeaker);

        /// <summary> If true, this VoiceNetworkObject has a Speaker setup for playback of received audio frames from remote audio source </summary>
        public bool IsSpeaker { get; private set; }
        /// <summary> If true, this VoiceNetworkObject has a Speaker that is currently playing received audio frames from remote audio source </summary>
        public bool IsSpeaking => this.IsSpeaker && this.SpeakerInUse.IsPlaying;

        /// <summary> If true, this VoiceNetworkObject has a Recorder setup for transmission of audio stream from local audio source </summary>
        public bool IsRecorder { get; private set; }
        /// <summary> If true, this VoiceNetworkObject has a Recorder that is currently transmitting audio stream from local audio source </summary>
        public bool IsRecording => this.IsRecorder && this.RecorderInUse.IsCurrentlyTransmitting;

        /// <summary> If true, the SpeakerInUse is linked to the remote voice stream </summary>
        public bool IsSpeakerLinked => this.IsSpeaker && this.SpeakerInUse.IsLinked;

        /// <summary> If true, the NetworkObject is found, non null & valid.</summary>
        internal bool IsNetworkObjectReady => this.Object && !ReferenceEquals(null, this.Object) && this.Object && this.Object.IsValid;

        internal bool RequiresSpeaker => this.IsNetworkObjectReady && this.IsPlayer && (this.SetupDebugSpeaker || !this.IsLocal);

        internal bool RequiresRecorder => this.IsNetworkObjectReady && this.IsPlayer && this.IsLocal;

        internal bool IsPlayer => this.Runner.IsPlayer;

        internal bool IsLocal => this.Object.HasInputAuthority;

        #endregion

        #region Private Methods

        internal void Setup()
        {
            if (this.IsSetup)
            {
                if (this.Logger.IsDebugEnabled)
                {
                    this.Logger.LogDebug("VoiceNetworkObject already setup");
                }
                return;
            }
            this.SetupRecorderInUse();
            this.SetupSpeakerInUse();
        }

        private bool SetupRecorder()
        {
            if (ReferenceEquals(null, this.recorderInUse)) // not manually assigned by user
            {
                if (this.UsePrimaryRecorder)
                {
                    if (!ReferenceEquals(null, this.voiceConnection.PrimaryRecorder) && this.voiceConnection.PrimaryRecorder)
                    {
                        this.recorderInUse = this.voiceConnection.PrimaryRecorder;
                        return this.SetupRecorder(this.recorderInUse);
                    } 
                    if (this.Logger.IsErrorEnabled)
                    {
                        this.Logger.LogError("PrimaryRecorder is not set.");
                    }
                }
                Recorder[] recorders = this.GetComponentsInChildren<Recorder>();
                if (recorders.Length > 0)
                {
                    Recorder recorder  = recorders[0];
                    if (recorders.Length > 1 && this.Logger.IsWarningEnabled)
                    {
                        this.Logger.LogWarning("Multiple Recorder components found attached to the GameObject or its children.");
                    }
                    if (!ReferenceEquals(null, recorder) && recorder)
                    {
                        this.recorderInUse = recorder;
                        return this.SetupRecorder(this.recorderInUse);
                    }
                }
                if (!this.AutoCreateRecorderIfNotFound)
                {
                    if (this.Logger.IsWarningEnabled)
                    {
                        this.Logger.LogWarning("No Recorder found to be setup.");
                    }
                    return false;
                }
                this.recorderInUse = this.gameObject.AddComponent<Recorder>();
            }
            return this.SetupRecorder(this.recorderInUse);
        }

        private bool SetupRecorder(Recorder recorder)
        {
            if (ReferenceEquals(null, recorder))
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Cannot setup a null Recorder.");
                }
                return false;
            }
            if (!recorder)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Cannot setup a destroyed Recorder.");
                }
                return false;
            }
            if (!this.IsNetworkObjectReady)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Recorder setup cannot be done as the NetworkObject is not valid or not ready yet.");
                }
                return false;
            }
            recorder.UserData = this.GetUserData();
            if (!recorder.IsInitialized)
            {
                this.RecorderInUse.Init(this.voiceConnection);
            }
            if (recorder.RequiresRestart)
            {
                recorder.RestartRecording();
            }
            return recorder.IsInitialized;
        }

        private bool SetupSpeaker()
        {
            if (ReferenceEquals(null, this.speakerInUse)) // not manually assigned by user
            {
                Speaker[] speakers = this.GetComponentsInChildren<Speaker>(true);
                if (speakers.Length > 0)
                {
                    this.speakerInUse = speakers[0];
                    if (speakers.Length > 1 && this.Logger.IsWarningEnabled)
                    {
                        this.Logger.LogWarning("Multiple Speaker components found attached to the GameObject or its children. Using the first one we found.");
                    }
                }
                if (ReferenceEquals(null, this.speakerInUse))
                {
                    bool instantiated = false;
                    if (!ReferenceEquals(null, this.voiceConnection.SpeakerPrefab))
                    {
                        GameObject go = Instantiate(this.voiceConnection.SpeakerPrefab, this.transform, false);
                        speakers = go.GetComponentsInChildren<Speaker>(true);
                        if (speakers.Length > 0)
                        {
                            this.speakerInUse = speakers[0];
                            if (speakers.Length > 1 && this.Logger.IsWarningEnabled)
                            {
                                this.Logger.LogWarning("Multiple Speaker components found attached to the GameObject (VoiceConnection.SpeakerPrefab) or its children. Using the first one we found.");
                            }
                        }
                        if (ReferenceEquals(null, this.speakerInUse))
                        {
                            if (this.Logger.IsErrorEnabled)
                            {
                                this.Logger.LogError("SpeakerPrefab does not have a component of type Speaker in its hierarchy.");
                            }
                            Destroy(go);
                        }
                        else
                        {
                            instantiated = true;
                        }
                    }
                    if (!instantiated)
                    {
                        if (!this.voiceConnection.AutoCreateSpeakerIfNotFound)
                        {
                            return false;
                        }
                        this.speakerInUse = this.gameObject.AddComponent<Speaker>();
                    }
                }
            }
            return this.SetupSpeaker(this.speakerInUse);
        }

        private bool SetupSpeaker(Speaker speaker)
        {
            if (ReferenceEquals(null, this.speakerInUse))
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Cannot setup a null Speaker");
                }
                return false;
            }
            if (!this.speakerInUse)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Cannot setup a destroyed Speaker");
                }
                return false;
            }
            #if !PHOTON_VOICE_FMOD_ENABLE
            AudioSource audioSource = speaker.GetComponent<AudioSource>();
            if (ReferenceEquals(null, audioSource))
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Unexpected (null?): no AudioSource found attached to the same GameObject as the Speaker component");
                }
                return false;
            }
            if (!audioSource)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Unexpected (destroyed?): no AudioSource found attached to the same GameObject as the Speaker component");
                }
                return false;
            }
            if (audioSource.mute)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("audioSource.mute is true, playback may not work properly");
                }
            }
            if (audioSource.volume <= 0f)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("audioSource.volume is zero, playback may not work properly");
                }
            }
            if (!audioSource.enabled)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("audioSource.enabled is false, playback may not work properly");
                }
            }
            #endif
            return true;
        }

        internal void SetupRecorderInUse()
        {
            if (this.IsRecorder)
            {
                if (this.Logger.IsInfoEnabled)
                {
                    this.Logger.LogInfo("Recorder already setup");
                }
                return;
            }
            if (!this.RequiresRecorder)
            {
                if (this.IsNetworkObjectReady)
                {
                    if (this.Logger.IsInfoEnabled)
                    {
                        this.Logger.LogInfo("Recorder not needed");
                    }
                }
                return;
            }
            this.IsRecorder = this.SetupRecorder();
            if (!this.IsRecorder)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Recorder not setup for VoiceNetworkObject: playback may not work properly.");
                }
            }
            else
            {
                if (!this.RecorderInUse.IsRecording && !this.RecorderInUse.AutoStart)
                {
                    if (this.Logger.IsWarningEnabled)
                    {
                        this.Logger.LogWarning("VoiceNetworkObject.RecorderInUse.AutoStart is false, don't forget to start recording manually using recorder.StartRecording() or recorder.IsRecording = true.");
                    }
                }
                if (!this.RecorderInUse.TransmitEnabled)
                {
                    if (this.Logger.IsWarningEnabled)
                    {
                        this.Logger.LogWarning("VoiceNetworkObject.RecorderInUse.TransmitEnabled is false, don't forget to set it to true to enable transmission.");
                    }
                }
                if (!this.RecorderInUse.isActiveAndEnabled && this.RecorderInUse.RecordOnlyWhenEnabled)
                {
                    if (this.Logger.IsWarningEnabled)
                    {
                        this.Logger.LogWarning("VoiceNetworkObject.RecorderInUse may not work properly as RecordOnlyWhenEnabled is set to true and recorder is disabled or attached to an inactive GameObject.");
                    }
                }
            }
        }

        internal void SetupSpeakerInUse()
        {
            if (this.IsSpeaker)
            {
                if (this.Logger.IsInfoEnabled)
                {
                    this.Logger.LogInfo("Speaker already setup");
                }
                return;
            }
            if (!this.RequiresSpeaker)
            {
                if (this.IsNetworkObjectReady)
                {
                    if (this.Logger.IsInfoEnabled)
                    {
                        this.Logger.LogInfo("Speaker not needed");
                    }
                }
                return;
            }
            this.IsSpeaker = this.SetupSpeaker();
            if (!this.IsSpeaker)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Speaker not setup for VoiceNetworkObject: voice chat will not work.");
                }
            }
            else
            {
                this.CheckLateLinking();
            }
        }
        
        private object GetUserData()
        {
            return this.Object.Id;
        }

        private void CheckLateLinking()
        {
            if (this.voiceConnection.Client.InRoom)
            {
                if (this.IsSpeaker)
                {
                    if (!this.IsSpeakerLinked)
                    {
                        if (this.voiceConnection.TryLateLinkingUsingUserData(this.SpeakerInUse, this.GetUserData()))
                        {
                            if (this.Logger.IsDebugEnabled)
                            {
                                this.Logger.LogDebug("Late linking attempt succeeded.");
                            }
                        }
                        else if (this.Logger.IsDebugEnabled)
                        {
                            this.Logger.LogDebug("Late linking attempt failed.");
                        }
                    }
                    else if (this.Logger.IsDebugEnabled)
                    {
                        this.Logger.LogDebug("Speaker already linked");
                    }
                } 
                else if (this.Logger.IsDebugEnabled)
                {
                    this.Logger.LogDebug("VoiceNetworkObject does not have a Speaker and may not need late linking check");
                }
            }
            else if (this.Logger.IsDebugEnabled)
            {
                this.Logger.LogDebug("Voice client is still not in a room, skipping late linking check");
            }
        }

        public override void Spawned()
        {
            this.voiceConnection = this.Runner.GetComponent<VoiceConnection>();
            this.Setup();
        }

        #endregion
    }
}
#endif