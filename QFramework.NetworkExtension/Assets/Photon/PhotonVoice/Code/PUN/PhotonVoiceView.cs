// ----------------------------------------------------------------------------
// <copyright file="PhotonVoiceView.cs" company="Exit Games GmbH">
// Photon Voice - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
// Component that should be attached to a networked PUN prefab that has 
// PhotonView. It will bind remote Recorder with local Speaker of the same 
// networked prefab. This component makes automatic voice stream routing easy 
// for players' characters/avatars.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

namespace Photon.Voice.PUN
{
    using Pun;
    using UnityEngine;
    using Unity;

    /// <summary>
    /// Component that should be attached to a networked PUN prefab that has <see cref="PhotonView"/>. 
    /// It will bind remote <see cref="Recorder"/> with local <see cref="Speaker"/> of the same networked prefab. 
    /// This component makes automatic voice stream routing easy for players' characters/avatars.
    /// </summary>
    [AddComponentMenu("Photon Voice/Photon Voice View")]
    [RequireComponent(typeof(PhotonView))]
    [HelpURL("https://doc.photonengine.com/en-us/voice/v2/getting-started/voice-for-pun")]
    public class PhotonVoiceView : VoiceComponent
    {
        #region Private Fields

        private PhotonView photonView;

        [SerializeField]
        private Recorder recorderInUse;

        [SerializeField]
        private Speaker speakerInUse;

        private bool onEnableCalledOnce;
        
        #endregion

        #region Public Fields

        /// <summary> If true, a Recorder component will be added to the same GameObject if not found already. </summary>
        public bool AutoCreateRecorderIfNotFound;
        /// <summary> If true, PhotonVoiceNetwork.PrimaryRecorder will be used by this PhotonVoiceView </summary>
        public bool UsePrimaryRecorder;
        /// <summary> If true, a Speaker component will be setup to be used for the DebugEcho mode </summary>
        public bool SetupDebugSpeaker;

        #endregion

        #region Properties

        /// <summary> The Recorder component currently used by this PhotonVoiceView </summary>
        public Recorder RecorderInUse
        {
            get
            {
                return this.recorderInUse;
            }
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
                else if (this.IsPhotonViewReady)
                {
                    if (this.Logger.IsWarningEnabled)
                    {
                        this.Logger.LogWarning("No need to set Recorder as the PhotonView does not belong to local player");
                    }
                }
            }
        }

        /// <summary> The Speaker component currently used by this PhotonVoiceView </summary>
        public Speaker SpeakerInUse
        {
            get
            {
                return this.speakerInUse;
            }
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
                else if (this.IsPhotonViewReady)
                {
                    if (this.Logger.IsWarningEnabled)
                    {
                        this.Logger.LogWarning("Speaker not set because the PhotonView does not belong to a remote player or SetupDebugSpeaker is disabled");
                    }
                }
            }
        }

        /// <summary> If true, this PhotonVoiceView is setup and ready to be used </summary>
        public bool IsSetup
        {
            get { return this.IsPhotonViewReady && (!this.RequiresRecorder || this.IsRecorder) && (!this.RequiresSpeaker || this.IsSpeaker); }
        }
        /// <summary> If true, this PhotonVoiceView has a Speaker setup for playback of received audio frames from remote audio source </summary>
        public bool IsSpeaker { get; private set; }
        /// <summary> If true, this PhotonVoiceView has a Speaker that is currently playing received audio frames from remote audio source </summary>
        public bool IsSpeaking
        {
            get { return this.IsSpeaker && this.SpeakerInUse.IsPlaying; }
        }
        /// <summary> If true, this PhotonVoiceView has a Recorder setup for transmission of audio stream from local audio source </summary>
        public bool IsRecorder { get; private set; }
        /// <summary> If true, this PhotonVoiceView has a Recorder that is currently transmitting audio stream from local audio source </summary>
        public bool IsRecording
        {
            get { return this.IsRecorder && this.RecorderInUse.IsCurrentlyTransmitting; }
        }
        /// <summary> If true, the SpeakerInUse is linked to the remote voice stream </summary>
        public bool IsSpeakerLinked
        {
            get { return this.IsSpeaker && this.SpeakerInUse.IsLinked; }
        }
        /// <summary> If true, the PhotonView attached to the same GameObject has a valid ViewID > 0 </summary>
        public bool IsPhotonViewReady
        {
            get { return !ReferenceEquals(null, this.photonView) && this.photonView && this.photonView.ViewID > 0; }
        }
        
        internal bool RequiresSpeaker
        {
            get { return this.SetupDebugSpeaker || this.IsPhotonViewReady && !this.photonView.IsMine; }
        }

        internal bool RequiresRecorder
        {
            get { return this.IsPhotonViewReady && this.photonView.IsMine; }
        }

        #endregion

        #region Private Methods

        protected override void Awake()
        {
            base.Awake();
            this.photonView = this.GetComponent<PhotonView>();
            this.Init();
        }

        private void OnEnable()
        {
            if (this.onEnableCalledOnce)
            {
                this.Init();
            }
            else
            {
                this.onEnableCalledOnce = true;
            }
        }

        private void Start()
        {
            this.Init();
        }
        
        private void CheckLateLinking()
        {
            if (PhotonVoiceNetwork.Instance.Client.InRoom)
            {
                if (this.IsSpeaker)
                {
                    if (!this.IsSpeakerLinked)
                    {
                        PhotonVoiceNetwork.Instance.CheckLateLinking(this.SpeakerInUse, this.photonView.ViewID);
                    }
                    else if (this.Logger.IsDebugEnabled)
                    {
                        this.Logger.LogDebug("Speaker already linked");
                    }
                } 
                else if (this.Logger.IsDebugEnabled)
                {
                    this.Logger.LogDebug("PhotonVoiceView does not have a Speaker and may not need late linking check");
                }
            }
            else if (this.Logger.IsDebugEnabled)
            {
                this.Logger.LogDebug("Voice client is still not in a room, skipping late linking check");
            }
        }

        internal void Setup()
        {
            if (this.IsSetup)
            {
                if (this.Logger.IsDebugEnabled)
                {
                    this.Logger.LogDebug("PhotonVoiceView already setup");
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
                    if (!ReferenceEquals(null, PhotonVoiceNetwork.Instance.PrimaryRecorder) && PhotonVoiceNetwork.Instance.PrimaryRecorder)
                    {
                        this.recorderInUse = PhotonVoiceNetwork.Instance.PrimaryRecorder;
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
            if (!this.IsPhotonViewReady)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Recorder setup cannot be done before assigning a valid ViewID to the PhotonView attached to the same GameObject as the PhotonVoiceView.");
                }
                return false;
            }
            recorder.UserData = this.photonView.ViewID;
            if (!recorder.IsInitialized)
            {
                this.RecorderInUse.Init(PhotonVoiceNetwork.Instance);
            }
            if (recorder.RequiresRestart)
            {
                recorder.RestartRecording();
            }
            return recorder.IsInitialized && recorder.UserData is int && this.photonView.ViewID == (int) recorder.UserData;
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
                    if (!ReferenceEquals(null, PhotonVoiceNetwork.Instance.SpeakerPrefab))
                    {
                        GameObject go = Instantiate(PhotonVoiceNetwork.Instance.SpeakerPrefab, this.transform, false);
                        speakers = go.GetComponentsInChildren<Speaker>(true);
                        if (speakers.Length > 0)
                        {
                            this.speakerInUse = speakers[0];
                            if (speakers.Length > 1 && this.Logger.IsWarningEnabled)
                            {
                                this.Logger.LogWarning("Multiple Speaker components found attached to the GameObject (PhotonVoiceNetwork.SpeakerPrefab) or its children. Using the first one we found.");
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
                        if (!PhotonVoiceNetwork.Instance.AutoCreateSpeakerIfNotFound)
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
            if (ReferenceEquals(null, speaker))
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Cannot setup a null Speaker");
                }
                return false;
            }
            if (!speaker)
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
                if (this.IsPhotonViewReady)
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
                    this.Logger.LogWarning("Recorder not setup for PhotonVoiceView: playback may not work properly.");
                }
            } 
            else 
            {
                if (!this.RecorderInUse.IsRecording && !this.RecorderInUse.AutoStart)
                {
                    if (this.Logger.IsWarningEnabled)
                    {
                        this.Logger.LogWarning("PhotonVoiceView.RecorderInUse.AutoStart is false, don't forget to start recording manually using recorder.StartRecording() or recorder.IsRecording = true.");
                    }
                }
                if (!this.RecorderInUse.TransmitEnabled)
                {
                    if (this.Logger.IsWarningEnabled)
                    {
                        this.Logger.LogWarning("PhotonVoiceView.RecorderInUse.TransmitEnabled is false, don't forget to set it to true to enable transmission.");
                    }
                }
                if (!this.RecorderInUse.isActiveAndEnabled && this.RecorderInUse.RecordOnlyWhenEnabled)
                {
                    if (this.Logger.IsWarningEnabled)
                    {
                        this.Logger.LogWarning("PhotonVoiceView.RecorderInUse may not work properly as RecordOnlyWhenEnabled is set to true and recorder is disabled or attached to an inactive GameObject.");
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
                if (this.IsPhotonViewReady)
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
                    this.Logger.LogWarning("Speaker not setup for PhotonVoiceView: voice chat will not work.");
                }
            }
            else
            {
                this.CheckLateLinking();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes this PhotonVoiceView for Voice usage based on the PhotonView, Recorder and Speaker components.
        /// </summary>
        /// <remarks>
        /// The initialization should happen automatically.
        /// Call this method explicitly if this does not succeed.
        /// The initialization is a two steps operation: step one is the setup of Recorder and Speaker to be used.
        /// Step two is the late-linking -if needed- of the SpeakerInUse and corresponding remote voice info -if any- via ViewID.
        /// </remarks>
        public void Init()
        {
            if (this.IsPhotonViewReady)
            {
                this.Setup();
                this.CheckLateLinking();
            }
            else if (this.Logger.IsDebugEnabled)
            {
                this.Logger.LogDebug("Tried to initialize PhotonVoiceView but PhotonView does not have a valid allocated ViewID yet.");
            }
        }        

        #endregion
    }
}
