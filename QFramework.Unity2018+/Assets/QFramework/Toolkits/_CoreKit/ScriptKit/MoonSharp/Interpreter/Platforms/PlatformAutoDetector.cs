using System;
using System.Linq;
using MoonSharp.Interpreter.Interop;
using MoonSharp.Interpreter.Loaders;

namespace MoonSharp.Interpreter.Platforms
{
	/// <summary>
	/// A static class offering properties for autodetection of system/platform details
	/// </summary>
	public static class PlatformAutoDetector
	{
		private static bool? m_IsRunningOnAOT = null;

		private static bool m_AutoDetectionsDone = false;

		/// <summary>
		/// Gets a value indicating whether this instance is running on mono.
		/// </summary>
		public static bool IsRunningOnMono { get; private set; }
		/// <summary>
		/// Gets a value indicating whether this instance is running on a CLR4 compatible implementation
		/// </summary>
		public static bool IsRunningOnClr4 { get; private set; }
		/// <summary>
		/// Gets a value indicating whether this instance is running on Unity-3D
		/// </summary>
		public static bool IsRunningOnUnity { get; private set; }
		/// <summary>
		/// Gets a value indicating whether this instance has been built as a Portable Class Library
		/// </summary>
		public static bool IsPortableFramework { get; private set; }
		/// <summary>
		/// Gets a value indicating whether this instance has been compiled natively in Unity (as opposite to importing a DLL).
		/// </summary>
		public static bool IsUnityNative { get; private set; }
		/// <summary>
		/// Gets a value indicating whether this instance has been compiled natively in Unity AND is using IL2CPP
		/// </summary>
		public static bool IsUnityIL2CPP { get; private set; }


		/// <summary>
		/// Gets a value indicating whether this instance is running a system using Ahead-Of-Time compilation 
		/// and not supporting JIT.
		/// </summary>
		public static bool IsRunningOnAOT
		{
			// We do a lazy eval here, so we can wire out this code by not calling it, if necessary..
			get
			{
#if UNITY_WEBGL || UNITY_IOS || UNITY_TVOS || ENABLE_IL2CPP
				return true;
#else

				if (!m_IsRunningOnAOT.HasValue)
				{
					try
					{
						System.Linq.Expressions.Expression e = System.Linq.Expressions.Expression.Constant(5, typeof(int));
						var lambda = System.Linq.Expressions.Expression.Lambda<Func<int>>(e);
						lambda.Compile();
						m_IsRunningOnAOT = false;
					}
					catch (Exception)
					{
						m_IsRunningOnAOT = true;
					}
				}

				return m_IsRunningOnAOT.Value;
#endif
			}
		}

		private static void AutoDetectPlatformFlags()
		{
			if (m_AutoDetectionsDone)
				return;
#if PCL
			IsPortableFramework = true;
#if ENABLE_DOTNET
			IsRunningOnUnity = true;
			IsUnityNative = true;
#endif
#else
#if UNITY_5
			IsRunningOnUnity = true;
			IsUnityNative = true;

	#if ENABLE_IL2CPP
					IsUnityIL2CPP = true;
	#endif
	#elif !(NETFX_CORE)
			IsRunningOnUnity = AppDomain.CurrentDomain
				.GetAssemblies()
				.SelectMany(a => a.SafeGetTypes())
				.Any(t => t.FullName.StartsWith("UnityEngine."));
	#endif
#endif

			IsRunningOnMono = (Type.GetType("Mono.Runtime") != null);

			IsRunningOnClr4 = (Type.GetType("System.Lazy`1") != null);

			m_AutoDetectionsDone = true;
		}



		internal static IPlatformAccessor GetDefaultPlatform()
		{
			AutoDetectPlatformFlags();

#if PCL || ENABLE_DOTNET
			return new LimitedPlatformAccessor();
#else
			if (IsRunningOnUnity)
				return new LimitedPlatformAccessor();

#if DOTNET_CORE
			return new DotNetCorePlatformAccessor();
#else
			return new StandardPlatformAccessor();
#endif
#endif
		}

		internal static IScriptLoader GetDefaultScriptLoader()
		{
			AutoDetectPlatformFlags();

			if (IsRunningOnUnity)
				return new UnityAssetsScriptLoader();
			else
			{
#if (DOTNET_CORE)
				return new FileSystemScriptLoader();
#elif (PCL || ENABLE_DOTNET || NETFX_CORE)
				return new InvalidScriptLoader("Portable Framework");
#else
				return new FileSystemScriptLoader();
#endif
			}
		}
	}
}
