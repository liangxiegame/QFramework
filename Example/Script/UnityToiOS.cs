using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public static class UnityToiOS  {

	[DllImport("__Internal")]
	private static extern string _receive(string s1,string s2);

	public static string Receive(string s1,string s2)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			return _receive (s1,s2);
		}

		return "ok";
	}
}
