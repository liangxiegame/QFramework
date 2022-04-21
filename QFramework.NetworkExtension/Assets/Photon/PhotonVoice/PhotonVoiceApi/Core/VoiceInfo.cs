// -----------------------------------------------------------------------
// <copyright file="VoiceInfo.cs" company="Exit Games GmbH">
//   Photon Voice API Framework for Photon - Copyright (C) 2017 Exit Games GmbH
// </copyright>
// <summary>
//   Photon data streaming support.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

using System.Collections.Generic;

namespace Photon.Voice
{
	/// <summary>Describes stream properties.</summary>
	public struct VoiceInfo
    {
        /// <summary>
        /// Create stream info for an Opus audio stream.
        /// </summary>
        /// <param name="samplingRate">Audio sampling rate.</param>
        /// <param name="channels">Number of channels.</param>
        /// <param name="frameDurationUs">Uncompressed frame (audio packet) size in microseconds.</param>
        /// <param name="bitrate">Stream bitrate (in bits/second).</param>
        /// <param name="userdata">Optional user data. Should be serializable by Photon.</param>
        /// <returns>VoiceInfo instance.</returns>
        static public VoiceInfo CreateAudioOpus(POpusCodec.Enums.SamplingRate samplingRate, int channels, OpusCodec.FrameDuration frameDurationUs, int bitrate, object userdata = null)
        {
            return new VoiceInfo()
            {
                Codec = Codec.AudioOpus,
                SamplingRate = (int)samplingRate,
                Channels = channels,
                FrameDurationUs = (int)frameDurationUs,
                Bitrate = bitrate,
                UserData = userdata
            };
        }
		/// <summary>
		/// Create stream info for an audio stream.
		/// </summary>
		/// <param name="codec">Audio codec.</param>
		/// <param name="samplingRate">Audio sampling rate.</param>
		/// <param name="channels">Number of channels.</param>
		/// <param name="frameDurationUs">Uncompressed frame (audio packet) size in microseconds.</param>
		/// <param name="userdata">Optional user data. Should be serializable by Photon.</param>
		/// <returns>VoiceInfo instance.</returns>
		static public VoiceInfo CreateAudio(Codec codec, int samplingRate, int channels, int frameDurationUs, object userdata = null)
		{
			return new VoiceInfo()
			{
				Codec = codec,
				SamplingRate = (int)samplingRate,
				Channels = channels,
				FrameDurationUs = (int)frameDurationUs,
				UserData = userdata
			};
		}

#if PHOTON_VOICE_VIDEO_ENABLE
        /// <summary>
        /// Create stream info for a video stream.
        /// </summary>
        /// <param name="codec">Video codec.</param>
        /// <param name="bitrate">Stream bitrate.</param>
        /// <param name="width">Streamed video width. If 0, width and height of video source used (no rescaling).</param>
        /// <param name="heigth">Streamed video height. If -1, aspect ratio preserved during rescaling.</param>
        /// <param name="fps">Streamed video frames per second.</param>
        /// <param name="keyFrameInt">Keyframes interval in frames.</param>/// 
        /// <param name="userdata">Optional user data. Should be serializable by Photon.</param>        
        /// <returns>VoiceInfo instance.</returns>
        static public VoiceInfo CreateVideo(Codec codec, int bitrate, int width, int heigth, int fps, int keyFrameInt, object userdata = null)
        {
            return new VoiceInfo()
            {
                Codec = codec,
                Bitrate = bitrate,
                Width = width,
                Height = heigth,
                FPS = fps,
                KeyFrameInt = keyFrameInt,
                UserData = userdata,
            };
        }

#endif
        public override string ToString()
        {
            return "c=" + Codec + " f=" + SamplingRate + " ch=" + Channels + " d=" + FrameDurationUs + " s=" + FrameSize + " b=" + Bitrate + " w=" + Width + " h=" + Height + " fps=" + FPS + " kfi=" + KeyFrameInt + " ud=" + UserData;
        }

        public Codec Codec { get; set; }
        /// <summary>Audio sampling rate (frequency, in Hz).</summary>
        public int SamplingRate { get; set; }
        /// <summary>Number of channels.</summary>
        public int Channels { get; set; }
        /// <summary>Uncompressed frame (audio packet) size in microseconds.</summary>
        public int FrameDurationUs { get; set; }
        /// <summary>Target bitrate (in bits/second).</summary>
        public int Bitrate { get; set; }
        /// <summary>Video width.</summary>
        public int Width { get; set; }
        /// <summary>Video height</summary>
        public int Height { get; set; }
        /// <summary>Video frames per second</summary>
        public int FPS { get; set; }
        /// <summary>Video keyframe interval in frames</summary>
        public int KeyFrameInt { get; set; }
        /// <summary>Optional user data. Should be serializable by Photon.</summary>
        public object UserData { get; set; }

        /// <summary>Uncompressed frame (data packet) size in samples.</summary>
        public int FrameDurationSamples { get { return (int)(this.SamplingRate * (long)this.FrameDurationUs / 1000000); } }
        /// <summary>Uncompressed frame (data packet) array size.</summary>
        public int FrameSize { get { return this.FrameDurationSamples * this.Channels; } }        
    }

    /// <summary>Information about a remote voice (incoming stream).</summary>
    public class RemoteVoiceInfo
    {
        internal RemoteVoiceInfo(int channelId, int playerId, byte voiceId, VoiceInfo info)
        {
            this.ChannelId = channelId;
            this.PlayerId = playerId;
            this.VoiceId = voiceId;
            this.Info = info;
        }
        /// <summary>Remote voice info.</summary>
        public VoiceInfo Info { get; private set; }
        /// <summary>ID of channel used for transmission.</summary>
        public int ChannelId { get; private set; }
        /// <summary>Player ID of voice owner.</summary>
        public int PlayerId { get; private set; }
        /// <summary>Voice ID (unique in the room).</summary>
        public byte VoiceId { get; private set; }
    }
}