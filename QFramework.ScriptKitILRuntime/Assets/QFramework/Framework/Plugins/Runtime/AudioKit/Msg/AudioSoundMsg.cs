/****************************************************************************
 * Copyright (c) 2017 liangxie
****************************************************************************/

using QFramework;

namespace QFramework
{
	using System;
	
	public class AudioSoundMsg : QMsg
	{
		public string SoundName;
		public float Volume = 1.0f;
		public System.Action onSoundBeganCallback;
		public System.Action onSoundEndedCallback;

		
		public AudioSoundMsg(string soundName) : base((int) AudioEvent.PlaySound)
		{
			SoundName = soundName;
		}

		public AudioSoundMsg(
			string soundName,
			System.Action onSoundBeganCallback = null,
			System.Action onSoundEndedCallback = null) : base((int) AudioEvent.PlaySound)
		{
			SoundName = soundName;
			this.onSoundBeganCallback = onSoundBeganCallback;
			this.onSoundEndedCallback = onSoundEndedCallback;
		}
	}
}