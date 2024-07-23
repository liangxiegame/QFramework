// ----------------------------------------------------------------------------
// <copyright file="PhotonVoiceNetwork.cs" company="Exit Games GmbH">
// Photon Voice - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
// This class can be used to automatically join/leave Voice rooms when
// Photon Unity Networking (PUN) joins or leaves its rooms. The Voice room
// will use the same name as PUN, but with a "_voice_" postfix.
// It also sets a custom PUN Speaker factory to find the Speaker
// component for a character's voice. For this to work, the voice's UserData
// must be set to the character's PhotonView ID. 
// (see "PhotonVoiceView.cs")
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;

namespace Photon.Voice.PUN
{

    /// <summary>
    /// This class can be used to automatically sync client states between PUN and Voice.
    /// It also sets a custom PUN Speaker factory to find the Speaker component for a character's voice. 
    /// For this to work attach a <see cref="PhotonVoiceView"/> next to the <see cref="PhotonView"/> of your player's prefab.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Photon Voice/Photon Voice Network")]
    [HelpURL("https://doc.photonengine.com/en-us/voice/v2/getting-started/voice-for-pun")]
    public class PhotonVoiceNetwork : VoiceConnection
    {
        #region Public Fields

        /// <summary> Suffix for voice room names appended to PUN room names. </summary>
        public const string VoiceRoomNameSuffix = "_voice_";
        /// <summary> Auto connect voice client and join a voice room when PUN client is joined to a PUN room </summary>
        public bool AutoConnectAndJoin = true;
        /// <summary> Auto disconnect voice client when PUN client is not joined to a PUN room </summary>
        public bool AutoLeaveAndDisconnect = true;
        /// <summary> Whether or not Photon Voice client should follow PUN client if the latter is in offline mode. </summary>
        public bool WorkInOfflineMode = true;
        #endregion

        #region Private Fields

        private EnterRoomParams voiceRoomParams = new EnterRoomParams
        {
            RoomOptions = new RoomOptions { IsVisible = false }
        };
        private bool clientCalledConnectAndJoin;
        private bool clientCalledDisconnect;
        private bool clientCalledConnectOnly;
        private bool internalDisconnect;
        private bool internalConnect;
        private static object instanceLock = new object();
        private static PhotonVoiceNetwork instance;
        private static bool instantiated;

        [SerializeField]
        private bool usePunAppSettings = true;

        [SerializeField]
        private bool usePunAuthValues = true;

        #endregion

        #region Properties

        /// <summary>
        /// Singleton instance for PhotonVoiceNetwork
        /// </summary>
        public static PhotonVoiceNetwork Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (AppQuits)
                    {
                        if (instance.Logger.IsWarningEnabled)
                        {
                            instance.Logger.LogWarning("PhotonVoiceNetwork Instance already destroyed on application quit. Won't create again - returning null.");
                        }
                        return null;
                    }
                    if (!instantiated)
                    {
                        PhotonVoiceNetwork[] objects = FindObjectsOfType<PhotonVoiceNetwork>();
                        if (objects == null || objects.Length < 1)
                        {
                            GameObject singleton = new GameObject();
                            singleton.name = "PhotonVoiceNetwork singleton";
                            instance = singleton.AddComponent<PhotonVoiceNetwork>();
                            if (instance.Logger.IsInfoEnabled)
                            {
                                instance.Logger.LogInfo("An instance of PhotonVoiceNetwork was automatically created in the scene.");
                            }
                        }
                        else if (objects.Length >= 1)
                        {
                            instance = objects[0];
                            if (objects.Length > 1)
                            {
                                if (instance.Logger.IsErrorEnabled)
                                {
                                    instance.Logger.LogError("{0} PhotonVoiceNetwork instances found. Using first one only and destroying all the other extra instances.", objects.Length);
                                }
                                for (int i = 1; i < objects.Length; i++)
                                {
                                    Destroy(objects[i]);
                                }
                            }
                        }
                        instantiated = true;
                        if (instance.Logger.IsDebugEnabled)
                        {
                            instance.Logger.LogDebug("PhotonVoiceNetwork singleton instance is now set.");
                        }
                    }
                    return instance;
                }
            }
            set
            {
                lock (instanceLock)
                {
                    if (ReferenceEquals(null, value) || !value)
                    {
                        if (instantiated)
                        {
                            if (instance.Logger.IsErrorEnabled)
                            {
                                instance.Logger.LogError("Cannot set PhotonVoiceNetwork.Instance to null or destroyed.");
                            }
                        }
                        else
                        {
                            Debug.LogError("Cannot set PhotonVoiceNetwork.Instance to null or destroyed.");
                        }
                        return;
                    }
                    if (instantiated)
                    {
                        if (instance.GetInstanceID() != value.GetInstanceID())
                        {
                            if (instance.Logger.IsErrorEnabled)
                            {
                                instance.Logger.LogError("An instance of PhotonVoiceNetwork is already set. Destroying extra instance.");
                            }
                            Destroy(value);
                        }
                        return;
                    }
                    instance = value;
                    instantiated = true;
                    if (instance.Logger.IsDebugEnabled)
                    {
                        instance.Logger.LogDebug("PhotonVoiceNetwork singleton instance is now set.");
                    }
                }
            }
        }

        /// <summary>
        /// Whether or not to use the same PhotonNetwork.AuthValues in PhotonVoiceNetwork.Instance.Client.AuthValues.
        /// This means that the same UserID will be used in both clients.
        /// If custom authentication is used and setup in PUN app, the same configuration should be done for the Voice app.
        /// </summary>
        public bool UsePunAuthValues
        {
            get
            {
                return this.usePunAuthValues;
            }
            set
            {
                this.usePunAuthValues = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Connect voice client to Photon servers and join a Voice room
        /// </summary>
        /// <returns>If true, connection command send from client</returns>
        public bool ConnectAndJoinRoom()
        {
            if (!PhotonNetwork.InRoom)
            {
                if (this.Logger.IsErrorEnabled)
                {
                    this.Logger.LogError("Cannot connect and join if PUN is not joined.");
                }
                return false;
            }
            if (this.Connect())
            {
                this.clientCalledConnectAndJoin = true;
                this.clientCalledDisconnect = false;
                return true;
            }
            if (this.Logger.IsErrorEnabled)
            {
                this.Logger.LogError("Connecting to server failed.");
            }
            return false;
        }

        /// <summary>
        /// Disconnect voice client from all Photon servers
        /// </summary>
        public void Disconnect()
        {
            if (!this.Client.IsConnected)
            {
                if (this.Logger.IsErrorEnabled)
                {
                    this.Logger.LogError("Cannot Disconnect if not connected.");
                }
                return;
            }
            this.clientCalledDisconnect = true;
            this.clientCalledConnectAndJoin = false;
            this.clientCalledConnectOnly = false;
            this.Client.Disconnect();
        }

        #endregion

        #region Private Methods

        protected override void Awake()
        {
            Instance = this;
            lock (instanceLock)
            {
                if (instantiated && instance.GetInstanceID() == this.GetInstanceID())
                {
                    base.Awake();
                }
            }
        }

        private void OnEnable()
        {
            PhotonNetwork.NetworkingClient.StateChanged += this.OnPunStateChanged;
            this.FollowPun(); // in case this is enabled or activated late
            this.clientCalledConnectAndJoin = false;
            this.clientCalledConnectOnly = false;
            this.clientCalledDisconnect = false;
            this.internalDisconnect = false;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PhotonNetwork.NetworkingClient.StateChanged -= this.OnPunStateChanged;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            lock (instanceLock)
            {
                if (instantiated && instance.GetInstanceID() == this.GetInstanceID())
                {
                    instantiated = false;
                    if (instance.Logger.IsDebugEnabled)
                    {
                        instance.Logger.LogDebug("PhotonVoiceNetwork singleton instance is being reset because destroyed.");
                    }
                    instance = null;
                }
            }
        }

        private void OnPunStateChanged(ClientState fromState, ClientState toState)
        {
            if (this.Logger.IsDebugEnabled)
            {
                this.Logger.LogDebug("OnPunStateChanged from {0} to {1}", fromState, toState);
            }
            this.FollowPun(toState);
        }

        protected override void OnVoiceStateChanged(ClientState fromState, ClientState toState)
        {
            base.OnVoiceStateChanged(fromState, toState);
            if (toState == ClientState.Disconnected)
            {
                if (this.internalDisconnect)
                {
                    this.internalDisconnect = false;
                }
                else if (!this.clientCalledDisconnect)
                {
                    this.clientCalledDisconnect = this.Client.DisconnectedCause == DisconnectCause.DisconnectByClientLogic;
                }
            } 
            else if (toState == ClientState.ConnectedToMasterServer)
            {
                if (this.internalConnect)
                {
                    this.internalConnect = false;
                } 
                else if (!this.clientCalledConnectOnly && !this.clientCalledConnectAndJoin)
                {
                    this.clientCalledConnectOnly = true;
                    this.clientCalledDisconnect = false;
                } 
            }
            this.FollowPun(toState);
        }

        private void FollowPun(ClientState toState)
        {
            switch (toState)
            {
                case ClientState.Joined:
                case ClientState.Disconnected:
                case ClientState.ConnectedToMasterServer:
                    this.FollowPun();
                    break;
            }
        }

        protected override Speaker SimpleSpeakerFactory(int playerId, byte voiceId, object userData)
        {
            if (!(userData is int))
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("UserData ({0}) does not contain PhotonViewId. Remote voice {1}/{2} not linked. Do you have a Recorder not used with a PhotonVoiceView? is this expected?",
                        userData == null ? "null" : userData.ToString(), playerId, voiceId);
                }
                return null;
            }

            int photonViewId = (int)userData;
            PhotonView photonView = PhotonView.Find(photonViewId);
            if (ReferenceEquals(null, photonView) || !photonView)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("No PhotonView with ID {0} found. Remote voice {1}/{2} not linked.", userData, playerId, voiceId);
                }
                return null;
            }

            PhotonVoiceView photonVoiceView = photonView.GetComponent<PhotonVoiceView>();
            if (ReferenceEquals(null, photonVoiceView) || !photonVoiceView)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("No PhotonVoiceView attached to the PhotonView with ID {0}. Remote voice {1}/{2} not linked.", userData, playerId, voiceId);
                }
                return null;
            }
            if (!photonVoiceView.IgnoreGlobalLogLevel)
            {
                photonVoiceView.LogLevel = this.LogLevel;
            }
            if (!photonVoiceView.IsSpeaker)
            {
                photonVoiceView.SetupSpeakerInUse();
            }
            return photonVoiceView.SpeakerInUse;
        }

        internal static string GetVoiceRoomName()
        {
            if (PhotonNetwork.InRoom)
            {
                return string.Format("{0}{1}", PhotonNetwork.CurrentRoom.Name, VoiceRoomNameSuffix);
            }
            return null;
        }

        private void ConnectOrJoin()
        {
            switch (this.ClientState)
            {
                case ClientState.PeerCreated:
                case ClientState.Disconnected:
                    if (this.Logger.IsInfoEnabled)
                    {
                        this.Logger.LogInfo("PUN joined room, now connecting Voice client");
                    }
                    if (!this.Connect())
                    {
                        if (this.Logger.IsErrorEnabled)
                        {
                            this.Logger.LogError("Connecting to server failed.");
                        }
                    }
                    else
                    {
                        this.internalConnect = this.AutoConnectAndJoin && !this.clientCalledConnectOnly && !this.clientCalledConnectAndJoin;
                    }
                    break;
                case ClientState.ConnectedToMasterServer:
                    if (this.Logger.IsInfoEnabled)
                    {
                        this.Logger.LogInfo("PUN joined room, now joining Voice room");
                    }
                    if (!this.JoinRoom(GetVoiceRoomName()))
                    {
                        if (this.Logger.IsErrorEnabled)
                        {
                            this.Logger.LogError("Joining a voice room failed.");
                        }
                    }
                    break;
                default:
                    if (this.Logger.IsWarningEnabled)
                    {
                        this.Logger.LogWarning("PUN joined room, Voice client is busy ({0}). Is this expected?", this.ClientState);
                    }
                    break;
            }
        }

        private bool Connect()
        {
            AppSettings settings = null;

            if (this.usePunAppSettings)
            {
                settings = new AppSettings();
                settings = PhotonNetwork.PhotonServerSettings.AppSettings.CopyTo(settings); // creates an independent copy (cause we need to modify it slightly)
                if (!string.IsNullOrEmpty(PhotonNetwork.CloudRegion))
                {
                    settings.FixedRegion = PhotonNetwork.CloudRegion; // makes sure the voice connection follows into the same cloud region (as PUN uses now).
                }

                this.Client.SerializationProtocol = PhotonNetwork.NetworkingClient.SerializationProtocol;
            }

            // use the same user, authentication, auth-mode and encryption as PUN
            if (this.UsePunAuthValues)
            {
                if (PhotonNetwork.AuthValues != null)
                {
                    if (this.Client.AuthValues == null)
                    {
                        this.Client.AuthValues = new AuthenticationValues();
                    }
                    this.Client.AuthValues = PhotonNetwork.AuthValues.CopyTo(this.Client.AuthValues);
                }
                this.Client.AuthMode = PhotonNetwork.NetworkingClient.AuthMode;
                this.Client.EncryptionMode = PhotonNetwork.NetworkingClient.EncryptionMode;
            }

            return this.ConnectUsingSettings(settings);
        }

        private bool JoinRoom(string voiceRoomName)
        {
            if (string.IsNullOrEmpty(voiceRoomName))
            {
                if (this.Logger.IsErrorEnabled)
                {
                    this.Logger.LogError("Voice room name is null or empty.");
                }
                return false;
            }
            this.voiceRoomParams.RoomName = voiceRoomName;
            return this.Client.OpJoinOrCreateRoom(this.voiceRoomParams);
        }

        // Follow PUN client state
        // In case Voice client disconnects unexpectedly try to reconnect to the same room
        // In case Voice client is connected to the wrong room switch to the correct one
        private void FollowPun()
        {
            if (AppQuits)
            {
                return;
            }
            if (PhotonNetwork.OfflineMode && !this.WorkInOfflineMode)
            {
                return;
            }
            if (PhotonNetwork.NetworkClientState == this.ClientState)
            {
                if (PhotonNetwork.InRoom && this.AutoConnectAndJoin)
                {
                    string expectedRoomName = GetVoiceRoomName();
                    string currentRoomName = this.Client.CurrentRoom.Name;
                    if (!currentRoomName.Equals(expectedRoomName))
                    {
                        if (this.Logger.IsWarningEnabled)
                        {
                            this.Logger.LogWarning(
                                "Voice room mismatch: Expected:\"{0}\" Current:\"{1}\", leaving the second to join the first.",
                                expectedRoomName, currentRoomName);
                        }
                        if (!this.Client.OpLeaveRoom(false))
                        {
                            if (this.Logger.IsErrorEnabled)
                            {
                                this.Logger.LogError("Leaving the current voice room failed.");
                            }
                        }
                    }
                }
                else if (this.ClientState == ClientState.ConnectedToMasterServer && this.AutoLeaveAndDisconnect && !this.clientCalledConnectAndJoin && !this.clientCalledConnectOnly)
                {
                    if (this.Logger.IsWarningEnabled)
                    {
                        this.Logger.LogWarning("Unexpected: PUN and Voice clients have the same client state: ConnectedToMasterServer, Disconnecting Voice client.");
                    }
                    this.internalDisconnect = true;
                    this.Client.Disconnect();
                }
                return;
            }
            if (PhotonNetwork.InRoom)
            {
                if (this.clientCalledConnectAndJoin || this.AutoConnectAndJoin && !this.clientCalledDisconnect)
                {
                    this.ConnectOrJoin();
                }
            }
            else if (this.Client.InRoom && this.AutoLeaveAndDisconnect && !this.clientCalledConnectAndJoin && !this.clientCalledConnectOnly)
            {
                if (this.Logger.IsInfoEnabled)
                {
                    this.Logger.LogInfo("PUN left room, disconnecting Voice");
                }
                this.internalDisconnect = true;
                this.Client.Disconnect();
            }
        }

        internal void CheckLateLinking(Speaker speaker, int viewId)
        {
            if (ReferenceEquals(null, speaker) || !speaker)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Cannot check late linking for null Speaker");
                }
                return;
            }
            if (viewId <= 0)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Cannot check late linking for ViewID = {0} (<= 0)", viewId);
                }
                return;
            }
            if (!this.Client.InRoom)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Cannot check late linking while not joined to a voice room, client state: {0}", System.Enum.GetName(typeof(ClientState), this.ClientState));
                }
                return;
            }
            for (int i = 0; i < this.cachedRemoteVoices.Count; i++)
            {
                RemoteVoiceLink remoteVoice = this.cachedRemoteVoices[i];
                if (remoteVoice.Info.UserData is int)
                {
                    int photonViewId = (int)remoteVoice.Info.UserData;
                    if (viewId == photonViewId)
                    {
                        if (this.Logger.IsInfoEnabled)
                        {
                            this.Logger.LogInfo("Speaker 'late-linking' for the PhotonView with ID {0} with remote voice {1}/{2}.", viewId, remoteVoice.PlayerId, remoteVoice.VoiceId);
                        }
                        this.LinkSpeaker(speaker, remoteVoice);
                        break;
                    }
                }
                else if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("VoiceInfo.UserData should be int/ViewId, received: {0}, do you have a Recorder not used with a PhotonVoiceView? is this expected?", 
                        remoteVoice.Info.UserData == null ? "null" : string.Format("{0} ({1})", remoteVoice.Info.UserData, remoteVoice.Info.UserData.GetType()));
                    if (remoteVoice.PlayerId == viewId / PhotonNetwork.MAX_VIEW_IDS)
                    {
                        this.Logger.LogWarning("Player with ActorNumber {0} has started recording (voice # {1}) too early without setting a ViewId maybe? (before PhotonVoiceView setup)", remoteVoice.PlayerId, remoteVoice.VoiceId);
                    }
                }
            }
        }

        #endregion
    }
}
