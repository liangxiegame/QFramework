using ExitGames.Client.Photon;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using static Photon.Voice.Unity.Recorder;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System;

namespace QFramework.NetworkExtension 
{
    public class NetworkingVoiceUtility : PhotonVoiceNetwork, INetworkingVoiceUtility
    {
        public event Action<string> onInVoiceRoomEvent;

        protected override void OnVoiceStateChanged(ClientState fromState, ClientState toState)
        {
            base.OnVoiceStateChanged(fromState, toState);

            if (this.Client.InRoom && this.Client.CurrentRoom.Name.Equals(GetVoiceRoomName()))
                onInVoiceRoomEvent?.Invoke(Client.CurrentRoom.Name);

        }

        public void Init()
        {

        }


        private void OnEnable()
        {
            this.PrimaryRecorder = gameObject.AddComponent<Recorder>();
            this.PrimaryRecorder.TransmitEnabled = true;
            this.PrimaryRecorder.AutoStart = true;
            this.PrimaryRecorder.MicrophoneType = MicType.Unity;
            this.LogLevel = DebugLevel.ERROR;
            this.GlobalRecordersLogLevel = DebugLevel.ERROR;
            this.GlobalSpeakersLogLevel = DebugLevel.ERROR;
        }


        public void SetInterestGroups(byte[] disableGroups, byte[] enableGroups)
        {
            if (this.Client.InRoom == false)
            {
                Debug.LogError(this.Client + "没有链接成功");
                return;
            }
            this.Client.OpChangeGroups(disableGroups, enableGroups);
        }

        public void SetTargetGroup(byte targetGroup)
        {
            this.PrimaryRecorder.InterestGroup = targetGroup;
        }

        public void SetVoiceTransmitEnabled(bool isEnabled)
        {
            this.PrimaryRecorder.TransmitEnabled = isEnabled;
        }

        internal static string GetVoiceRoomName()
        {
            if (PhotonNetwork.InRoom)
            {
                return string.Format("{0}{1}", PhotonNetwork.CurrentRoom.Name, VoiceRoomNameSuffix);
            }
            return null;
        }
    }
}


