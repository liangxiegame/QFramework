/****************************************************************************
 * Copyright (c) 2017 liangxie
****************************************************************************/

using QFramework;

namespace QF.Res
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	
	public class AddRetainAudioMsg : QMsg
	{
		public string AudioName;

		public AddRetainAudioMsg(string audioName) : base((ushort) AudioEvent.AddRetainAudio)
		{
			AudioName = audioName;
		}
	}
}