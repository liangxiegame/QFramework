/****************************************************************************
 * Copyright (c) 2017 liangxie
****************************************************************************/

using QFramework;

namespace QF.Res
{
	using  System;
	
	public class AudioVoiceMsg : QMsg
	{
		public string voiceName;
		public System.Action onVoiceBeganCallback;
		public System.Action onVoiceEndedCallback;

		/// <summary>
		/// 是否循环
		/// </summary>
		public bool loop = false;

		/// <summary>
		/// 循环间隔
		/// </summary>
		public float intervalTime = 0.0f;

		public AudioVoiceMsg()
		{
		}

		public AudioVoiceMsg(string voiceName,
			System.Action onVoiceBeganCallback = null,
			System.Action onVoiceEndedCallback = null) : base((int) AudioEvent.PlayVoice)
		{
			this.voiceName = voiceName;
			this.onVoiceBeganCallback = onVoiceBeganCallback;
			this.onVoiceEndedCallback = onVoiceEndedCallback;
		}
	}
}