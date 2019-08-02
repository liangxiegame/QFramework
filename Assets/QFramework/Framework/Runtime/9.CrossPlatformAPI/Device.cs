/****************************************************************************
 * Copyright (c) 2017 liangxie
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

namespace QF
{
	using System.Linq;
	using UnityEngine;
	
#if SLUA_SUPPORT
	using SLua;

	[CustomLuaClassAttribute]
#endif
	/// <summary>
	/// 1.device detect
	/// 2.resolution ratio calculte; Support QUIManager
	/// TODO:// add ResolutionManager Or Util
	/// </summary>
	public class DeviceUtil
	{
		/// <summary>
		/// Vibrateable
		/// TODO: https://github.com/Burnsidious/AndroidVibrateController/tree/c0322985531ba9000e2093779f1456d601bf67bc
		/// TODO: https://github.com/danskidb/Unity3D-UITools/tree/bf1dfdb6e4cc119a56e08883d9a8a8151b9e6677
		/// </summary>
		public static void Vibrate()
		{
			#if UNITY_IOS
			Handheld.Vibrate();
			#endif
		}
		
		/// <summary>
		/// iPhone/iPad/OS X
		/// </summary>
		/// <returns></returns>
		public static bool IsApplePlatform()
		{
			return RuntimePlatform.IPhonePlayer == Application.platform
			       || RuntimePlatform.OSXEditor == Application.platform
			       || RuntimePlatform.OSXPlayer == Application.platform;
		}

		/// <summary>
		/// Android
		/// </summary>
		/// <returns></returns>
		public static bool IsAndroidPlatform()
		{
			return RuntimePlatform.Android == Application.platform;
		}

		/*  DeviceIDs
			Unknown,
			iPhone, 1
			iPhone3G, 2
			iPhone3GS, 3 
			iPodTouch1Gen, 4
			iPodTouch2Gen, 5
			iPodTouch3Gen, 6
			iPad1Gen, 7
			iPhone4, 8
			iPodTouch4Gen, 9 
			iPad2Gen, 10
			iPhone4S, 11
			iPad3Gen, 12
			iPhone5, 13
			iPodTouch5Gen, 14
			iPadMini1Gen, 15
			iPad4Gen, 16
			iPhone5C, 17
			iPhone5S, 18
			iPadAir1, 19
			iPad5Gen = 19,
			iPadMini2Gen, 20
			iPhone6, 21 
			iPhone6Plus, 22
			iPadMini3Gen, 23
			iPadAir2, 24
			iPhone6S, 25
			iPhone6SPlus, 26
			iPadPro1Gen, 27
			iPadMini4Gen, 28
			iPhoneSE1Gen, 29
			iPadPro10Inch1Gen, 30
			iPhone7, 31
			iPhone7Plus, 32
			iPhoneUnknown = 10001,
			iPadUnknown, 10002
			iPodTouchUnknown 10003 
		*/

		public static int[] IPhoneIDs =
		{
			1, 2, 3, 8, 10, 11, 13, 17, 18, 21, 22, 25, 26, 29, 31, 32, 10001
		};

		public static int[] IPadIDs =
		{
			7, 10, 12, 15, 16, 19, 20, 24, 25, 27, 28, 30, 10002
		};

		public static bool IsEditor()
		{
			return RuntimePlatform.OSXEditor == Application.platform
			       || RuntimePlatform.WindowsEditor == Application.platform;
		}

		#region refactor to ResolutionHelper
		/// <summary>
		/// iPhone 5s height/width 1.78 1280x720
		/// </summary>
		/// <returns></returns>
		public static bool PhoneResolution()
		{
			float aspect = Screen.height > Screen.width ? (float)Screen.height / Screen.width : (float)Screen.width / Screen.height;
			return aspect > (16.0f / 9 - 0.05) && aspect < (16.0f / 9 + 0.05);
		}

		/// <summary>
		/// height/width = 1.67  iPhone 4/4s 960x640
		/// </summary>
		/// <returns></returns>
		public static bool Phone167Resolution()
		{
			float aspect = Screen.height > Screen.width ? (float)Screen.height / Screen.width : (float)Screen.width / Screen.height;
			return aspect > (1920.0f / 1152 - 0.05) && aspect < (1920.0f / 1152 + 0.05);
		}

        /// <summary>
        /// height/width = 1.5  iPhone 4/4s 960x640
        /// </summary>
        /// <returns></returns>
        public static bool Phone15Resolution()
        {
            float aspect = Screen.height > Screen.width ? (float)Screen.height / Screen.width : (float)Screen.width / Screen.height;
            return aspect > (960.0f / 640 - 0.05) && aspect < (960.0f / 640+ 0.05);
        }

        /// <summary>
        /// height/width = 1.60  Huawei Pad?  2560 / 1600
        /// </summary>
        /// <returns></returns>
        public static bool Phone160Resolution()
		{
			float aspect = Screen.height > Screen.width ? (float)Screen.height / Screen.width : (float)Screen.width / Screen.height;
			return aspect > (2560.0f / 1600 - 0.05) && aspect < (2560.0f / 1600 + 0.05);
		}

		
		/// <summary>
		/// Pad resolution  iPad   2048 / 1536
		/// </summary>
		/// <returns></returns>
		public static bool PadResolution()
		{
			float aspect = Screen.height > Screen.width ? (float)Screen.height / Screen.width : (float)Screen.width / Screen.height;
			return aspect > (4.0f / 3 - 0.05) && aspect < (4.0f / 3 + 0.05);
		}
		#endregion

		public static bool IsIPhone()
		{
			var deviceID = GetDeviceID();
			return IPhoneIDs.Contains(deviceID);
		}

		public static int GetDeviceID()
		{
#if UNITY_EDITOR || UNITY_IOS
			return (int) UnityEngine.iOS.Device.generation;
#else 
			return -100;
#endif
		}

		public static bool IsIPad()
		{
			var deviceID = GetDeviceID();
			return IPadIDs.Contains(deviceID);
		}
		
		public static string GetOSSuffix()
		{
			if (IsApplePlatform())
				return "ios";
			else
				return "android";
		}

		public static bool IsAndroidPad()
		{
			return IsAndroidPlatform() && PadResolution();
		}

		public static bool IsAndroidPhone()
		{
			return IsAndroidPlatform() && PhoneResolution();
		}
		
		//-- 奇葩的魅族适配
		public static bool IsAndroidPhone167()
		{
			return IsAndroidPlatform() && Phone167Resolution();
		}

		public static bool IsAndroidPhone160()
		{
			return IsAndroidPlatform() && Phone160Resolution();
		}
	}
}