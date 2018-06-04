/****************************************************************************
 * Copyright (c) 2017 liangxie
****************************************************************************/

namespace QFramework
{
	using UnityEngine;
	#if UNITY_EDITOR	
	using UnityEditor;
	#endif

	/// <summary>
	/// 平台相关的应该全部扔到这里
	/// </summary>
	public class PlatformUtil
	{	
		public static string GetPlatformName()
		{
	#if UNITY_EDITOR
			return GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
	#else
			return GetPlatformForAssetBundles(Application.platform);
	#endif
		}
	
	#if UNITY_EDITOR
		public static string GetPlatformForAssetBundles(BuildTarget target)
		{
			switch(target)
			{
			case BuildTarget.Android:
				return "Android";
			case BuildTarget.iOS:
				return "iOS";
			case BuildTarget.WebGL:
				return "WebGL";
			case BuildTarget.StandaloneWindows:
			case BuildTarget.StandaloneWindows64:
				return "Windows";
#if !UNITY_2017_3_OR_NEWER 
			case BuildTarget.StandaloneOSXIntel:
			case BuildTarget.StandaloneOSXIntel64:
#elif UNITY_5 
			case BuildTarget.StandaloneOSXUniversal:
#else
            case BuildTarget.StandaloneOSX:
#endif
				return "OSX";
				// Add more build targets for your own.
				// If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
			default:
				return null;
			}
		}
	#endif
	
		static string GetPlatformForAssetBundles(RuntimePlatform platform)
		{
			switch(platform)
			{
			case RuntimePlatform.Android:
				return "Android";
			case RuntimePlatform.IPhonePlayer:
				return "iOS";
			case RuntimePlatform.WebGLPlayer:
				return "WebGL";
			case RuntimePlatform.WindowsPlayer:
				return "Windows";
			case RuntimePlatform.OSXPlayer:
				return "OSX";
				// Add more build targets for your own.
				// If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
			default:
				return null;
			}
		}
	}
}