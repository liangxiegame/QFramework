using System;
using System.IO;
using System.Text;

namespace MoonSharp.Interpreter.Platforms
{
	/// <summary>
	/// An abstract class which offers basic services on top of IPlatformAccessor to provide easier implementation of platforms.
	/// </summary>
	public abstract class PlatformAccessorBase : IPlatformAccessor
	{
		/// <summary>
		/// Gets the platform name prefix
		/// </summary>
		/// <returns></returns>
		public abstract string GetPlatformNamePrefix();

		/// <summary>
		/// Gets the name of the platform (used for debug purposes).
		/// </summary>
		/// <returns>
		/// The name of the platform (used for debug purposes)
		/// </returns>
		public string GetPlatformName()
		{
			string suffix = null;

			if (PlatformAutoDetector.IsRunningOnUnity)
			{
				if (PlatformAutoDetector.IsUnityNative)
				{
					suffix = "unity." + GetUnityPlatformName().ToLower() + "." + GetUnityRuntimeName();
				}
				else
				{
					if (PlatformAutoDetector.IsRunningOnMono)
						suffix = "unity.dll.mono";
					else
						suffix = "unity.dll.unknown";
				}
			}
			else if (PlatformAutoDetector.IsRunningOnMono)
				suffix = "mono";
			else
				suffix = "dotnet";

			if (PlatformAutoDetector.IsPortableFramework)
				suffix = suffix + ".portable";
			
			if (PlatformAutoDetector.IsRunningOnClr4)
				suffix = suffix + ".clr4";
			else
				suffix = suffix + ".clr2";

#if DOTNET_CORE
			suffix += ".netcore";
#endif

			if (PlatformAutoDetector.IsRunningOnAOT)
				suffix = suffix + ".aot";

			return GetPlatformNamePrefix() + "." + suffix;
		}

		private string GetUnityRuntimeName()
		{
#if ENABLE_MONO
	return "mono";
#elif ENABLE_IL2CPP
	return "il2cpp";
#elif ENABLE_DOTNET
	return "dotnet";
#else
	return "unknown";
#endif
		}

		private string GetUnityPlatformName()
		{
#if UNITY_STANDALONE_OSX
			return "OSX";
#elif UNITY_STANDALONE_WIN
			return "WIN";
#elif UNITY_STANDALONE_LINUX
			return "LINUX";
#elif UNITY_STANDALONE
			return "STANDALONE";
#elif UNITY_WII
			return "WII";
#elif UNITY_IOS
			return "IOS";
#elif UNITY_IPHONE
			return "IPHONE";
#elif UNITY_ANDROID
			return "ANDROID";
#elif UNITY_PS3
			return "PS3";
#elif UNITY_PS4
			return "PS4";
#elif UNITY_SAMSUNGTV
			return "SAMSUNGTV";
#elif UNITY_XBOX360
			return "XBOX360";
#elif UNITY_XBOXONE
			return "XBOXONE";
#elif UNITY_TIZEN
			return "TIZEN";
#elif UNITY_TVOS
			return "TVOS";
#elif UNITY_WP_8_1
			return "WP_8_1";
#elif UNITY_WSA_10_0
			return "WSA_10_0";
#elif UNITY_WSA_8_1
			return "WSA_8_1";
#elif UNITY_WSA
			return "WSA";
#elif UNITY_WINRT_10_0
			return "WINRT_10_0";
#elif UNITY_WINRT_8_1
			return "WINRT_8_1";
#elif UNITY_WINRT
			return "WINRT";
#elif UNITY_WEBGL
			return "WEBGL";
#else
			return "UNKNOWNHW";
#endif
		}

		/// <summary>
		/// Default handler for 'print' calls. Can be customized in ScriptOptions
		/// </summary>
		/// <param name="content">The content.</param>
		public abstract void DefaultPrint(string content);

		/// <summary>
		/// DEPRECATED.
		/// This is kept for backward compatibility, see the overload taking a prompt as an input parameter.
		/// 
		/// Default handler for interactive line input calls. Can be customized in ScriptOptions.
		/// If an inheriting class whants to give a meaningful implementation, this method MUST be overridden.
		/// </summary>
		/// <returns>null</returns>
		[Obsolete("Replace with DefaultInput(string)")]
		public virtual string DefaultInput()
		{
			return null;
		}

		/// <summary>
		/// Default handler for interactive line input calls. Can be customized in ScriptOptions.
		/// If an inheriting class whants to give a meaningful implementation, this method MUST be overridden.
		/// </summary>
		/// <returns>null</returns>
		public virtual string DefaultInput(string prompt)
		{
#pragma warning disable 618
			return DefaultInput();
#pragma warning restore 618
		}

		/// <summary>
		/// A function used to open files in the 'io' module. 
		/// Can have an invalid implementation if 'io' module is filtered out.
		/// It should return a correctly initialized Stream for the given file and access
		/// </summary>
		/// <param name="script"></param>
		/// <param name="filename">The filename.</param>
		/// <param name="encoding">The encoding.</param>
		/// <param name="mode">The mode (as per Lua usage - e.g. 'w+', 'rb', etc.).</param>
		/// <returns></returns>
		public abstract Stream IO_OpenFile(Script script, string filename, Encoding encoding, string mode);


		/// <summary>
		/// Gets a standard stream (stdin, stdout, stderr).
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public abstract Stream IO_GetStandardStream(StandardFileType type);


		/// <summary>
		/// Gets a temporary filename. Used in 'io' and 'os' modules.
		/// Can have an invalid implementation if 'io' and 'os' modules are filtered out.
		/// </summary>
		/// <returns></returns>
		public abstract string IO_OS_GetTempFilename();


		/// <summary>
		/// Exits the process, returning the specified exit code.
		/// Can have an invalid implementation if the 'os' module is filtered out.
		/// </summary>
		/// <param name="exitCode">The exit code.</param>
		public abstract void OS_ExitFast(int exitCode);


		/// <summary>
		/// Checks if a file exists. Used by the 'os' module.
		/// Can have an invalid implementation if the 'os' module is filtered out.
		/// </summary>
		/// <param name="file">The file.</param>
		/// <returns>
		/// True if the file exists, false otherwise.
		/// </returns>
		public abstract bool OS_FileExists(string file);


		/// <summary>
		/// Deletes the specified file. Used by the 'os' module.
		/// Can have an invalid implementation if the 'os' module is filtered out.
		/// </summary>
		/// <param name="file">The file.</param>
		public abstract void OS_FileDelete(string file);


		/// <summary>
		/// Moves the specified file. Used by the 'os' module.
		/// Can have an invalid implementation if the 'os' module is filtered out.
		/// </summary>
		/// <param name="src">The source.</param>
		/// <param name="dst">The DST.</param>
		public abstract void OS_FileMove(string src, string dst);


		/// <summary>
		/// Executes the specified command line, returning the child process exit code and blocking in the meantime.
		/// Can have an invalid implementation if the 'os' module is filtered out.
		/// </summary>
		/// <param name="cmdline">The cmdline.</param>
		/// <returns></returns>
		public abstract int OS_Execute(string cmdline);


		/// <summary>
		/// Filters the CoreModules enumeration to exclude non-supported operations
		/// </summary>
		/// <param name="module">The requested modules.</param>
		/// <returns>
		/// The requested modules, with unsupported modules filtered out.
		/// </returns>
		public abstract CoreModules FilterSupportedCoreModules(CoreModules module);

		/// <summary>
		/// Gets an environment variable. Must be implemented, but an implementation is allowed
		/// to always return null if a more meaningful implementation cannot be achieved or is
		/// not desired.
		/// </summary>
		/// <param name="envvarname">The envvarname.</param>
		/// <returns>
		/// The environment variable value, or null if not found
		/// </returns>
		public abstract string GetEnvironmentVariable(string envvarname);

		/// <summary>
		/// Determines whether the application is running in AOT (ahead-of-time) mode
		/// </summary>
		public virtual bool IsRunningOnAOT()
		{
			return PlatformAutoDetector.IsRunningOnAOT;
		}
	}
}
