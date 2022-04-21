namespace Photon.Voice.Unity
{
    using Voice;
    using System;

    public class RemoteVoiceLink : IEquatable<RemoteVoiceLink>
    {
        public readonly VoiceInfo Info;
        public readonly int PlayerId;
        public readonly int VoiceId;
        public readonly int ChannelId;

        public event Action<FrameOut<float>> FloatFrameDecoded;
        public event Action RemoteVoiceRemoved;

        public RemoteVoiceLink(VoiceInfo info, int playerId, int voiceId, int channelId)
        {
            this.Info = info;
            this.PlayerId = playerId;
            this.VoiceId = voiceId;
            this.ChannelId = channelId;
        }

        public void Init(ref RemoteVoiceOptions options)
        { 
            options.SetOutput(this.OnDecodedFrameFloatAction);
            options.OnRemoteVoiceRemoveAction = this.OnRemoteVoiceRemoveAction;
        }

        private void OnRemoteVoiceRemoveAction()
        {
            if (this.RemoteVoiceRemoved != null)
            {
                this.RemoteVoiceRemoved();
            }
        }

        private void OnDecodedFrameFloatAction(FrameOut<float> floats)
        {
            if (this.FloatFrameDecoded != null)
            {
                this.FloatFrameDecoded(floats);
            }
        }

        private string cached;
        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.cached))
            {
                this.cached = string.Format("[p#:{0},v#:{1},c#:{2},i:{{{3}}}]", this.PlayerId, this.VoiceId, this.ChannelId, this.Info);
            }
            return this.cached;
        }

        public bool Equals(RemoteVoiceLink other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return this.PlayerId == other.PlayerId && this.VoiceId == other.VoiceId || this.Info.UserData == other.Info.UserData;
        }
    }
}
