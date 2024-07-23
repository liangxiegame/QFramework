namespace Photon.Voice.PUN.UtilityScripts
{
    using Pun;
    using Unity;
    using Realtime;
    using UnityEngine;
    using ExitGames.Client.Photon;

    /// <summary> Utility script to be attached next to PhotonVoiceView & PhotonView on the player prefab to be network instantiated.
    /// Call voiceDebugScript.CantHearYou() on the networked object of the remote (or local) player if you can't hear the corresponding player. </summary>
    [RequireComponent(typeof(PhotonVoiceView))]
    public class VoiceDebugScript : MonoBehaviourPun
    {
        private PhotonVoiceView photonVoiceView;

        /// <summary> Make sure recorder.TransmitEnabled and recorder.IsRecording are true. </summary>
        public bool ForceRecordingAndTransmission;

        /// <summary> Audio file to be broadcast when TestUsingAudioClip is enabled. </summary>
        public AudioClip TestAudioClip;

        /// <summary> Broadcast Audio file to make sure transmission over network works if microphone (audio input device/hardware) is not reliable. Requires setting AudioClip in TestAudioClip. </summary>
        public bool TestUsingAudioClip;

        /// <summary> Disable recorder.VoiceDetection for easier testing. </summary>
        public bool DisableVad;

        /// <summary> Set main voice component's log level to ALL (max). </summary>
        public bool IncreaseLogLevels;

        /// <summary> Debug DebugEcho mode (Can't Hear My Self?!). </summary>
        public bool LocalDebug;

        private void Awake()
        {
            this.photonVoiceView = this.GetComponent<PhotonVoiceView>();
        }

        private void Update()
        {
            this.MaxLogs();
            if (this.photonVoiceView.IsRecorder)
            {
                if (this.TestUsingAudioClip)
                {
                    if (ReferenceEquals(null, this.TestAudioClip) || !this.TestAudioClip)
                    {
                        Debug.LogError("Set an AudioClip first");
                    }
                    else
                    {
                        this.photonVoiceView.RecorderInUse.SourceType = Recorder.InputSourceType.AudioClip;
                        this.photonVoiceView.RecorderInUse.AudioClip = this.TestAudioClip;
                        this.photonVoiceView.RecorderInUse.LoopAudioClip = true;
                        if (this.photonVoiceView.RecorderInUse.RequiresRestart)
                        {
                            this.photonVoiceView.RecorderInUse.RestartRecording();
                        }
                        else
                        {
                            this.photonVoiceView.RecorderInUse.StartRecording();
                        }
                        this.photonVoiceView.RecorderInUse.TransmitEnabled = true;
                    }
                }
                if (this.ForceRecordingAndTransmission)
                {
                    this.photonVoiceView.RecorderInUse.IsRecording = true;
                    this.photonVoiceView.RecorderInUse.TransmitEnabled = true;
                }
                if (this.DisableVad)
                {
                    this.photonVoiceView.RecorderInUse.VoiceDetection = false;
                }
            }
        }

        [ContextMenu("CantHearYou")]
        public void CantHearYou()
        {
            if (!PhotonVoiceNetwork.Instance.Client.InRoom)
            {
                Debug.LogError("local voice client is not joined to a voice room");
            }
            else if (!this.photonVoiceView.IsPhotonViewReady)
            {
                Debug.LogError("PhotonView is not ready yet; maybe PUN client is not joined to a room yet or this PhotonView is not valid");
            }
            else if (!this.photonVoiceView.IsSpeaker)
            {
                if (this.photonView.IsMine && !this.photonVoiceView.SetupDebugSpeaker)
                {
                    Debug.LogError("local object does not have SetupDebugSpeaker enabled");
                    if (this.LocalDebug)
                    {
                        Debug.Log("setup debug speaker not enabled, enabling it now (1)");
                        this.photonVoiceView.SetupDebugSpeaker = true;
                        this.photonVoiceView.Setup();
                    }
                }
                else
                {
                    Debug.LogError("locally not speaker (yet?) (1)");
                    this.photonVoiceView.Setup();
                }
            }
            else
            {
                if (!this.photonVoiceView.IsSpeakerLinked)
                {
                    Debug.LogError("locally speaker not linked, trying late linking & asking anyway");
                    // late linking maybe
                    PhotonVoiceNetwork.Instance.CheckLateLinking(this.photonVoiceView.SpeakerInUse, this.photonView.ViewID);
                }
                this.photonView.RPC("CantHearYou", this.photonView.Owner, PhotonVoiceNetwork.Instance.Client.CurrentRoom.Name, PhotonVoiceNetwork.Instance.Client.LoadBalancingPeer.ServerIpAddress, PhotonVoiceNetwork.Instance.Client.AppVersion);
            }
        }

        [PunRPC]
        private void CantHearYou(string roomName, string serverIp, string appVersion, PhotonMessageInfo photonMessageInfo)
        {
            string why;
            if (!PhotonVoiceNetwork.Instance.Client.InRoom)
            {
                why = "voice client not in a room";
            }
            else if (!PhotonVoiceNetwork.Instance.Client.CurrentRoom.Name.Equals(roomName))
            {
                why = string.Format("voice client is on another room {0} != {1}",
                    PhotonVoiceNetwork.Instance.Client.CurrentRoom.Name, roomName);
            }
            else if (!PhotonVoiceNetwork.Instance.Client.LoadBalancingPeer.ServerIpAddress.Equals(serverIp))
            {
                why = string.Format("voice client is on another server {0} != {1}, maybe different Photon Cloud regions",
                    PhotonVoiceNetwork.Instance.Client.LoadBalancingPeer.ServerIpAddress, serverIp);
            }
            else if (!PhotonVoiceNetwork.Instance.Client.AppVersion.Equals(appVersion))
            {
                why = string.Format("voice client uses different AppVersion {0} != {1}",
                    PhotonVoiceNetwork.Instance.Client.AppVersion, appVersion);
            }
            else if (!this.photonVoiceView.IsRecorder)
            {
                why = "recorder not setup (yet?)";
                this.photonVoiceView.Setup();
            }
            else if (!this.photonVoiceView.RecorderInUse.IsRecording)
            {
                why = "recorder is not recording";
                this.photonVoiceView.RecorderInUse.IsRecording = true;
            }
            else if (!this.photonVoiceView.RecorderInUse.TransmitEnabled)
            {
                why = "recorder is not transmitting";
                this.photonVoiceView.RecorderInUse.TransmitEnabled = true;
            }
            else if (this.photonVoiceView.RecorderInUse.InterestGroup != 0)
            {
                why = "recorder.InterestGroup is not zero? is this on purpose? switching it back to zero";
                this.photonVoiceView.RecorderInUse.InterestGroup = 0;
            }
            else if (!(this.photonVoiceView.RecorderInUse.UserData is int) || (int)this.photonVoiceView.RecorderInUse.UserData != this.photonView.ViewID)
            {
                why = string.Format("recorder.UserData ({0}) != photonView.ViewID ({1}), fixing it now", this.photonVoiceView.RecorderInUse.UserData, this.photonView.ViewID);
                this.photonVoiceView.RecorderInUse.UserData = this.photonView.ViewID;
                this.photonVoiceView.RecorderInUse.RestartRecording();
            }
            else if (this.photonVoiceView.RecorderInUse.VoiceDetection && this.DisableVad) // todo: check WebRtcAudioDsp.VAD
            {
                why = "recorder vad is enabled, disable it for testing";
                this.photonVoiceView.RecorderInUse.VoiceDetection = false;
            }
            else if (this.photonView.OwnerActorNr == photonMessageInfo.Sender.ActorNumber)
            {
                if (this.LocalDebug)
                {
                    if (this.photonVoiceView.IsSpeaker)
                    {
                        why = "no idea why!, should be working (1)";
                        this.photonVoiceView.RecorderInUse.RestartRecording(true);
                    }
                    else if (!this.photonVoiceView.SetupDebugSpeaker) // unreachable probably
                    {
                        why = "setup debug speaker not enabled, enabling it now (2)";
                        this.photonVoiceView.SetupDebugSpeaker = true;
                        this.photonVoiceView.Setup();
                    }
                    else if (!this.photonVoiceView.RecorderInUse.DebugEchoMode)
                    {
                        why = "recorder debug echo mode not enabled, enabling it now";
                        this.photonVoiceView.RecorderInUse.DebugEchoMode = true;
                    }
                    else
                    {
                        why = "locally not speaker (yet?) (2)";
                        this.photonVoiceView.Setup();
                    }
                }
                else
                {
                    why = "local object, are you trying to hear yourself? (feedback DebugEcho), LocalDebug is disabled, enable it if you want to diagnose this";
                }
            }
            else
            {
                why = "no idea why!, should be working (2)";
                this.photonVoiceView.RecorderInUse.RestartRecording(true);
            }
            this.Reply(why, photonMessageInfo.Sender);
        }

        private void Reply(string why, Player player)
        {
            this.photonView.RPC("HeresWhy", player, why);
        }

        [PunRPC]
        private void HeresWhy(string why, PhotonMessageInfo photonMessageInfo)
        {
            Debug.LogErrorFormat("Player {0} replied to my CantHearYou message with {1}", photonMessageInfo.Sender, why);
        }

        private void MaxLogs()
        {
            if (this.IncreaseLogLevels)
            {
                this.photonVoiceView.LogLevel = DebugLevel.ALL;
                PhotonVoiceNetwork.Instance.LogLevel = DebugLevel.ALL;
                PhotonVoiceNetwork.Instance.GlobalRecordersLogLevel = DebugLevel.ALL;
                PhotonVoiceNetwork.Instance.GlobalSpeakersLogLevel = DebugLevel.ALL;
                if (this.photonVoiceView.IsRecorder)
                {
                    this.photonVoiceView.RecorderInUse.LogLevel = DebugLevel.ALL;
                }
                if (this.photonVoiceView.IsSpeaker)
                {
                    this.photonVoiceView.SpeakerInUse.LogLevel = DebugLevel.ALL;
                }
            }
        }
    }
}