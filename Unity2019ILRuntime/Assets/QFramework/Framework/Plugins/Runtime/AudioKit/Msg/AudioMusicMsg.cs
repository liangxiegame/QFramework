/****************************************************************************
 * Copyright (c) 2017 liangxie
****************************************************************************/

using QFramework;

namespace QFramework
{
    using System;

	public enum VolumeLevel
	{
		Max,
		Normal,
		Min,
	}
	/// <summary>
	/// 播放音乐用的消息
	/// </summary>
	public class AudioMusicMsg : QMsg
	{
		public string MusicName;
		public bool Loop = true;
		public VolumeLevel volumeLevel = VolumeLevel.Normal;

		/// <summary>
		/// 是否受MusicOn(bool)管理
		/// </summary>
		public bool allowMusicOff = true;

		public System.Action onMusicBeganCallback;
        public System.Action onMusicEndedCallback;

		public AudioMusicMsg(string musicName, bool loop = true, bool allowMusicOff = true,
                             System.Action onMusicBeganCallback = null,
                             System.Action onMusicEndedCallback = null) : base((ushort) AudioEvent.PlayMusic)
		{
			this.MusicName = musicName;
			this.Loop = loop;
			this.allowMusicOff = allowMusicOff;
			this.onMusicBeganCallback = onMusicBeganCallback;
			this.onMusicEndedCallback = onMusicEndedCallback;
		}
	}

	public class AudioStopMusicMsg : QMsg
	{
		public AudioStopMusicMsg() : base((ushort)AudioEvent.StopMusic){}
	}
}