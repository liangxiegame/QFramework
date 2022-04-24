// -----------------------------------------------------------------------
// <copyright file="LoadBalancingTransport2.cs" company="Exit Games GmbH">
//   Photon Voice API Framework for Photon - Copyright (C) 2020 Exit Games GmbH
// </copyright>
// <summary>
//   Extends Photon Realtime API with audio streaming functionality.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------


namespace Photon.Voice
{
    using System;
    using ExitGames.Client.Photon;
    using Realtime;


    /// <summary>
    /// Variant of LoadBalancingTransport. Aims to be non-alloc at the cost of breaking compatibility with older clients.
    /// </summary>
    public class LoadBalancingTransport2 : LoadBalancingTransport
    {
        public LoadBalancingTransport2(ILogger logger = null, ConnectionProtocol connectionProtocol = ConnectionProtocol.Udp) : base(logger, connectionProtocol)
        {
            this.LoadBalancingPeer.UseByteArraySlicePoolForEvents = true; // incoming byte[] events can be deserialized to a pooled ByteArraySlice
            this.LoadBalancingPeer.ReuseEventInstance = true;             // this won't store references to the event anyways
        }

		const int DATA_OFFSET = 4;
        public override void SendFrame(ArraySegment<byte> data, FrameFlags flags, byte evNumber, byte voiceId, int channelId, int targetPlayerId, bool reliable, LocalVoice localVoice)
        {
            // this uses a pooled slice, which is released within the send method (here RaiseEvent at the bottom)
            ByteArraySlice frameData = this.LoadBalancingPeer.ByteArraySlicePool.Acquire(data.Count + DATA_OFFSET);
            frameData.Buffer[0] = DATA_OFFSET;
            frameData.Buffer[1] = voiceId;
            frameData.Buffer[2] = evNumber;
            frameData.Buffer[3] = (byte)flags;
            Buffer.BlockCopy(data.Array, 0, frameData.Buffer, DATA_OFFSET, data.Count);
            frameData.Count = data.Count + DATA_OFFSET; // need to set the count, as we manipulated the buffer directly

            SendOptions sendOpt = new SendOptions() { Reliability = reliable, Channel = this.photonChannelForCodec(localVoice.Info.Codec), Encrypt = localVoice.Encrypt };

            RaiseEventOptions opt = new RaiseEventOptions();
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

            this.OpRaiseEvent(VoiceEvent.FrameCode, frameData, opt, sendOpt);

            // each voice has it's own connection? else, we could aggregate voices data in less count of datagrams
            while (this.LoadBalancingPeer.SendOutgoingCommands());
        }


        protected override void onEventActionVoiceClient(EventData ev)
        {
            if (ev.Code == VoiceEvent.FrameCode)
            {
                // Payloads are arrays. If first array element is 0 than next is event subcode. Otherwise, the event is data frame with voiceId in 1st element.
                this.onVoiceFrameEvent(ev[(byte)ParameterCode.CustomEventContent], VOICE_CHANNEL, ev.Sender, this.LocalPlayer.ActorNumber);
            }
            else
            {
                base.onEventActionVoiceClient(ev);
            }
        }

        internal void onVoiceFrameEvent(object content0, int channelId, int playerId, int localPlayerId)
        {
            byte[] content;
            int contentLength;
            int sliceOffset = 0;
            ByteArraySlice slice = content0 as ByteArraySlice;
            if (slice != null)
            {
                content = slice.Buffer;
                contentLength = slice.Count;
                sliceOffset = slice.Offset;
            }
            else
            {
                content = content0 as byte[];
                contentLength = content.Length;
            }

            if (content == null || contentLength < 3)
            {
                this.LogError("[PV] onVoiceFrameEvent did not receive data (readable as byte[]) " + content0);
            }
            else
            {
                byte dataOffset = (byte)content[sliceOffset];
                byte voiceId = (byte)content[sliceOffset + 1];
                byte evNumber = (byte)content[sliceOffset + 2];
                FrameFlags flags = 0;
                if (dataOffset > 3)
                {
                    flags = (FrameFlags)content[3];
                }

                FrameBuffer buffer;
                if (slice != null)
                {
                    buffer = new FrameBuffer(slice.Buffer, slice.Offset + dataOffset, contentLength - dataOffset, flags, slice);
                }
                else
                {
                    buffer = new FrameBuffer(content, dataOffset, contentLength - dataOffset, flags, null);
                }
                
                this.voiceClient.onFrame(channelId, playerId, voiceId, evNumber, ref buffer, playerId == localPlayerId);
                buffer.Release();
            }
        }
    }
}