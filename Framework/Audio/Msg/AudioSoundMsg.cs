/****************************************************************************
 * Copyright (c) 2017 liangxie
****************************************************************************/

namespace QFramework
{
	using System;
	
	public class AudioSoundMsg : QMsg
	{
		public string SoundName;
		public Action onSoundBeganCallback;
		public Action onSoundEndedCallback;

		
		public AudioSoundMsg(string soundName) : base((int) AudioEvent.PlaySound)
		{
			SoundName = soundName;
		}

		public AudioSoundMsg(
			string soundName,
			Action onSoundBeganCallback = null,
			Action onSoundEndedCallback = null) : base((int) AudioEvent.PlaySound)
		{
			SoundName = soundName;
			this.onSoundBeganCallback = onSoundBeganCallback;
			this.onSoundEndedCallback = onSoundEndedCallback;
		}
	}
}