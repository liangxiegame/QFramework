// -----------------------------------------------------------------------
// <copyright file="LoadBalancingTransport.cs" company="Exit Games GmbH">
//   Photon Voice API Framework for Photon - Copyright (C) 2015 Exit Games GmbH
// </copyright>
// <summary>
//   Extends Photon Realtime API with media streaming functionality.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Realtime;

namespace Photon.Voice
{
    class VoiceEvent
    {
        /// <summary>
        /// Single event used for voice communications.
        /// </summary>
        /// Change if it conflicts with other event codes used in the same Photon room.
        public const byte Code = 202; // all photon voice events use single event code
        public const byte FrameCode = 203; // LoadBalancingTransport2 uses separate code for frame event serialized as byte[]
    }

    /// <summary>
    /// Extends LoadBalancingClient with media streaming functionality.
    /// </summary>
    /// <remarks>
    /// Use your normal LoadBalancing workflow to join a Voice room. 
    /// All standard LoadBalancing features are available.
    /// Use <see cref="VoiceClient"/> to work with media streams.
    /// </remarks>
    public class LoadBalancingTransport : LoadBalancingClient, IVoiceTransport, ILogger, IDisposable
    {
        internal const int VOICE_CHANNEL = 0;

        /// <summary>The <see cref="VoiceClient"></see> implementation associated with this LoadBalancingTransport.</summary>
        public VoiceClient VoiceClient { get { return this.voiceClient; } }

        protected VoiceClient voiceClient;
        private PhotonTransportProtocol protocol;

        public void LogError(string fmt, params object[] args) { this.DebugReturn(DebugLevel.ERROR, string.Format(fmt, args)); }
        public void LogWarning(string fmt, params object[] args) { this.DebugReturn(DebugLevel.WARNING, string.Format(fmt, args)); }
        public void LogInfo(string fmt, params object[] args) { this.DebugReturn(DebugLevel.INFO, string.Format(fmt, args)); }
        public void LogDebug(string fmt, params object[] args) { this.DebugReturn(DebugLevel.ALL, string.Format(fmt, args)); }

        // send different media type to different channels for efficiency
        internal byte photonChannelForCodec(Codec c)
        {
            return (byte)(1 + Array.IndexOf(Enum.GetValues(typeof(Codec)), c));
        }

        public bool IsChannelJoined(int channelId) { return this.State == ClientState.Joined; }

        /// <summary>
        /// Initializes a new <see cref="LoadBalancingTransport"/>.
        /// </summary>
        /// <param name="logger">ILogger instance. If null, this instance LoadBalancingClient.DebugReturn implementation is used.<see cref="ConnectionProtocol"></see></param>
        /// <param name="connectionProtocol">Connection protocol (UDP or TCP). <see cref="ConnectionProtocol"></see></param>
        public LoadBalancingTransport(ILogger logger = null, ConnectionProtocol connectionProtocol = ConnectionProtocol.Udp) : base(connectionProtocol)
        {
            if (logger == null)
            {
                logger = this;
            }
            base.EventReceived += onEventActionVoiceClient;
            base.StateChanged += onStateChangeVoiceClient;
            this.voiceClient = new VoiceClient(this, logger);
            var voiceChannelsCount = Enum.GetValues(typeof(Codec)).Length + 1; // channel per stream type, channel 0 is for user events
            if (LoadBalancingPeer.ChannelCount < voiceChannelsCount)
            {
                this.LoadBalancingPeer.ChannelCount = (byte)voiceChannelsCount;
            }
            this.protocol = new PhotonTransportProtocol(voiceClient, logger);
        }

        /// <summary>
        /// This method dispatches all available incoming commands and then sends this client's outgoing commands.
        /// Call this method regularly (2 to 20 times a second).
        /// </summary>
        new public void Service()
        {
            base.Service();
            this.voiceClient.Service();
        }

        [Obsolete("Use LoadBalancingPeer::OpChangeGroups().")]
        public virtual bool ChangeAudioGroups(byte[] groupsToRemove, byte[] groupsToAdd)
        {
            return this.LoadBalancingPeer.OpChangeGroups(groupsToRemove, groupsToAdd);
        }

        [Obsolete("Use GlobalInterestGroup.")]
        public byte GlobalAudioGroup
        {
            get { return GlobalInterestGroup; }
            set { GlobalInterestGroup = value; }
        }
        /// <summary>
        /// Set global interest group for this client. This call sets InterestGroup for existing local voices and for created later to given value.
        /// Client set as listening to this group only until LoadBalancingPeer.OpChangeGroups() called. This method can be called any time.
        /// </summary>
        /// <see cref="LocalVoice.InterestGroup"/>
        /// <see cref="LoadBalancingPeer.OpChangeGroups(byte[], byte[])"/>
        public byte GlobalInterestGroup
        {
            get { return this.voiceClient.GlobalInterestGroup; }
            set
            {
                this.voiceClient.GlobalInterestGroup = value;
                if (this.State == ClientState.Joined)
                {
                    if (this.voiceClient.GlobalInterestGroup != 0)
                    {
                        this.LoadBalancingPeer.OpChangeGroups(new byte[0], new byte[] { this.voiceClient.GlobalInterestGroup });
                    }
                    else
                    {
                        this.LoadBalancingPeer.OpChangeGroups(new byte[0], null);
                    }
                }                
            }
        }


        #region nonpublic

        public void SendVoicesInfo(IEnumerable<LocalVoice> voices, int channelId, int targetPlayerId)
        {
            foreach (var codecVoices in voices.GroupBy(v => v.Info.Codec))
            {
                object content = protocol.buildVoicesInfo(codecVoices, true);

                var sendOpt = new SendOptions()
                {
                    Reliability = true,
                    Channel = photonChannelForCodec(codecVoices.Key),
                };

                var opt = new RaiseEventOptions();
                if (targetPlayerId == -1)
                {
                    opt.TargetActors = new int[] { this.LocalPlayer.ActorNumber };
                }
                else if (targetPlayerId != 0)
                {
                    opt.TargetActors = new int[] { targetPlayerId };
                }

                this.OpRaiseEvent(VoiceEvent.Code, content, opt, sendOpt);
            }
        }

        public void SendVoiceRemove(LocalVoice voice, int channelId, int targetPlayerId)
        {
            object content = protocol.buildVoiceRemoveMessage(voice);
            var sendOpt = new SendOptions()
            {
                Reliability = true,
                Channel = photonChannelForCodec(voice.Info.Codec),
            };

            var opt = new RaiseEventOptions();

            if (targetPlayerId == -1)
            {
                opt.TargetActors = new int[] { this.LocalPlayer.ActorNumber };
            }
            else if (targetPlayerId != 0)
            {
                opt.TargetActors = new int[] { targetPlayerId };
            }
            if (voice.DebugEchoMode)
            {
                opt.Receivers = ReceiverGroup.All;
            }

            this.OpRaiseEvent(VoiceEvent.Code, content, opt, sendOpt);
        }

        public virtual void SendFrame(ArraySegment<byte> data, FrameFlags flags, byte evNumber, byte voiceId, int channelId, int targetPlayerId, bool reliable, LocalVoice localVoice)
        {
            object[] content = protocol.buildFrameMessage(voiceId, evNumber, data, flags);

            var sendOpt = new SendOptions()
            {
                Reliability = reliable,
                Channel = photonChannelForCodec(localVoice.Info.Codec),
                Encrypt = localVoice.Encrypt
            };

            var opt = new RaiseEventOptions();
            if (targetPlayerId == -1)
            {
                opt.TargetActors = new int[] { this.LocalPlayer.ActorNumber };
            }
            else if (targetPlayerId != 0)
            {
                opt.TargetActors = new int[] { targetPlayerId };
            }
            if (localVoice.DebugEchoMode)
            {
                opt.Receivers = ReceiverGroup.All;
            }
            opt.InterestGroup = localVoice.InterestGroup;

            this.OpRaiseEvent(VoiceEvent.Code, content, opt, sendOpt);
            while (this.LoadBalancingPeer.SendOutgoingCommands());
        }

        public string ChannelIdStr(int channelId) { return null; }
        public string PlayerIdStr(int playerId) { return null; }

        protected virtual void onEventActionVoiceClient(EventData ev)
        {
            // check for voice event first
            if (ev.Code == VoiceEvent.Code)
            {
                // Payloads are arrays. If first array element is 0 than next is event subcode. Otherwise, the event is data frame with voiceId in 1st element.                    
                protocol.onVoiceEvent(ev[(byte)ParameterCode.CustomEventContent], VOICE_CHANNEL, ev.Sender, ev.Sender == this.LocalPlayer.ActorNumber);
            }
            else
            {
                int playerId;
                switch (ev.Code)
                {
                    case (byte)EventCode.Join:
                        playerId = ev.Sender;
                        if (playerId == this.LocalPlayer.ActorNumber)
                        {
                        }
                        else
                        {
                            this.voiceClient.onPlayerJoin(VOICE_CHANNEL, playerId);                            
                        }
                        break;
                    case (byte)EventCode.Leave:
                        {
                            playerId = ev.Sender;
                            if (playerId == this.LocalPlayer.ActorNumber)
                            {
                                this.voiceClient.onLeaveAllChannels();
                            }
                            else
                            {
                                this.voiceClient.onPlayerLeave(VOICE_CHANNEL, playerId);
                            }
                        }
                        break;
                }
            }
        }

        void onStateChangeVoiceClient(ClientState fromState, ClientState state)
        {
            switch (fromState)
            {
                case ClientState.Joined:
                    this.voiceClient.onLeaveChannel(VOICE_CHANNEL);
                    break;
            }

            switch (state)
            {
                case ClientState.Joined:
                    this.voiceClient.onJoinChannel(VOICE_CHANNEL);
                    if (this.voiceClient.GlobalInterestGroup != 0)
                    {
                        this.LoadBalancingPeer.OpChangeGroups(new byte[0], new byte[] { this.voiceClient.GlobalInterestGroup });
                    }
                    break;
            }
        }        

        #endregion

        /// <summary>
        /// Releases all resources used by the <see cref="LoadBalancingTransport"/> instance.
        /// </summary>
        public void Dispose()
        {
            this.voiceClient.Dispose();
        }
    }
}
