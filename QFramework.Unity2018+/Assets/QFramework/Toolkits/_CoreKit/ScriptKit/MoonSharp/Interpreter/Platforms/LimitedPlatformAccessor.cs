using System;
using System.Text;

namespace MoonSharp.Interpreter.Platforms
{
	/// <summary>
	/// A class implementing all the bits needed to have a minimal support of a platform.
	/// This does not support the 'io'/'file' modules and has partial support of the 'os' module.
	/// </summary>
	public class LimitedPlatformAccessor : PlatformAccessorBase
	{
		/// <summary>
		/// Gets an environment variable. Must be implemented, but an implementation is allowed
		/// to always return null if a more meaningful implementation cannot be achieved or is
		/// not desired.
		/// </summary>
		/// <param name="envvarname">The envvarname.</param>
		/// <returns>
		/// The environment variable value, or null if not found
		/// </returns>
		public override string GetEnvironmentVariable(string envvarname)
		{
			return null;
		}

		/// <summary>
		/// Filters the CoreModules enumeration to exclude non-supported operations
		/// </summary>
		/// <param name="module">The requested modules.</param>
		/// <returns>
		/// The requested modules, with unsupported modules filtered out.
		/// </returns>
		public override CoreModules FilterSupportedCoreModules(CoreModules module)
		{
			return module & (~(CoreModules.IO | CoreModules.OS_System));
		}

		/// <summary>
		/// A function used to open files in the 'io' module. 
		/// LimitedPlatformAccessorBase does NOT offer a meaningful implementation of this method and
		/// thus does not support 'io' and 'os' modules.
		/// </summary>
		/// <param name="script"></param>
		/// <param name="filename">The filename.</param>
		/// <param name="encoding">The encoding.</param>
		/// <param name="mode">The mode (as per Lua usage - e.g. 'w+', 'rb', etc.).</param>
		/// <returns></returns>
		/// <exception cref="System.NotImplementedException">The current platform accessor does not support 'io' and 'os' operations. Provide your own implementation of platform to work around this limitation, if needed.</exception>
		public override System.IO.Stream IO_OpenFile(Script script, string filename, Encoding encoding, string mode)
		{
			throw new NotImplementedException("The current platform accessor does not support 'io' and 'os' operations. Provide your own implementation of platform to work around this limitation, if needed.");
		}

		/// <summary>
		/// Gets a standard stream (stdin, stdout, stderr).
		/// LimitedPlatformAccessorBase does NOT offer a meaningful implementation of this method and
		/// thus does not support 'io' and 'os' modules.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		/// <exception cref="System.NotImplementedException">The current platform accessor does not support 'io' and 'os' operations. Provide your own implementation of platform to work around this limitation, if needed.</exception>
		public override System.IO.Stream IO_GetStandardStream(StandardFileType type)
		{
			throw new NotImplementedException("The current platform accessor does not support 'io' and 'os' operations. Provide your own implementation of platform to work around this limitation, if needed.");
		}

		/// <summary>
		/// Gets a temporary filename. Used in 'io' and 'os' modules.
		/// LimitedPlatformAccessorBase does NOT offer a meaningful implementation of this method and
		/// thus does not support 'io' and 'os' modules.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="System.NotImplementedException">The current platform accessor does not support 'io' and 'os' operations. Provide your own implementation of platform to work around this limitation, if needed.</exception>
		public override string IO_OS_GetTempFilename()
		{
			throw new NotImplementedException("The current platform accessor does not support 'io' and 'os' operations. Provide your own implementation of platform to work around this limitation, if needed.");
		}

		/// <summary>
		/// Exits the process, returning the specified exit code.
		/// LimitedPlatformAccessorBase does NOT offer a meaningful implementation of this method and
		/// thus does not support 'io' and 'os' modules.
		/// </summary>
		/// <param name="exitCode">The exit code.</param>
		/// <exception cref="System.NotImplementedException">The current platform accessor does not support 'io' and 'os' operations. Provide your own implementation of platform to work around this limitation, if needed.</exception>
		public override void OS_ExitFast(int exitCode)
		{
			throw new NotImplementedException("The current platform accessor does not support 'io' and 'os' operations. Provide your own implementation of platform to work around this limitation, if needed.");
		}

		/// <summary>
		/// Checks if a file exists. Used by the 'os' module.
		/// LimitedPlatformAccessorBase does NOT offer a meaningful implementation of this method and
		/// thus does not support 'io' and 'os' modules.
		/// </summary>
		/// <param name="file">The file.</param>
		/// <returns>
		/// True if the file exists, false otherwise.
		/// </returns>
		/// <exception cref="System.NotImplementedException">The current platform accessor does not support 'io' and 'os' operations. Provide your own implementation of platform to work around this limitation, if needed.</exception>
		public override bool OS_FileExists(string file)
		{
			throw new NotImplementedException("The current platform accessor does not support 'io' and 'os' operations. Provide your own implementation of platform to work around this limitation, if needed.");
		}

		/// <summary>
		/// Deletes the specified file. Used by the 'os' module.
		/// LimitedPlatformAccessorBase does NOT offer a meaningful implementation of this method and
		/// thus does not support 'io' and 'os' modules.
		/// </summary>
		/// <param name="file">The file.</param>
		/// <exception cref="System.NotImplementedException">The current platform accessor does not support 'io' and 'os' operations. Provide your own implementation of platform to work around this limitation, if needed.</exception>
		public override void OS_FileDelete(string file)
		{
			throw new NotImplementedException("The current platform accessor does not support 'io' and 'os' operations. Provide your own implementation of platform to work around this limitation, if needed.");
		}

		/// <summary>
		/// Moves the specified file. Used by the 'os' module.
		/// LimitedPlatformAccessorBase does NOT offer a meaningful implementation of this method and
		/// thus does not support 'io' and 'os' modules.
		/// </summary>
		/// <param name="src">The source.</param>
		/// <param name="dst">The DST.</param>
		/// <exception cref="System.NotImplementedException">The current platform accessor does not support 'io' and 'os' operations. Provide your own implementation of platform to work around this limitation, if needed.</exception>
		public override void OS_FileMove(string src, string dst)
		{
			throw new NotImplementedException("The current platform accessor does not support 'io' and 'os' operations. Provide your own implementation of platform to work around this limitation, if needed.");
		}

		/// <summary>
		/// Executes the specified command line, returning the child process exit code and blocking in the meantime.
		/// LimitedPlatformAccessorBase does NOT offer a meaningful implementation of this method and
		/// thus does not support 'io' and 'os' modules.
		/// </summary>
		/// <param name="cmdline">The cmdline.</param>
		/// <returns></returns>
		/// <exception cref="System.NotImplementedException">The current platform accessor does not support 'io' and 'os' operations. Provide your own implementation of platform to work around this limitation, if needed.</exception>
		public override int OS_Execute(string cmdline)
		{
			throw new NotImplementedException("The current platform accessor does not support 'io' and 'os' operations. Provide your own implementation of platform to work around this limitation, if needed.");
		}

		/// <summary>
		/// Gets the platform name prefix
		/// </summary>
		/// <returns></returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public override string GetPlatformNamePrefix()
		{
			return "limited";
		}

		/// <summary>
		/// Default handler for 'print' calls. Can be customized in ScriptOptions
		/// </summary>
		/// <param name="content">The content.</param>
		/// <exception cref="System.NotImplementedException"></exception>
		public override void DefaultPrint(string content)
		{
			System.Diagnostics.Debug.WriteLine(content);
		}
	}
}
