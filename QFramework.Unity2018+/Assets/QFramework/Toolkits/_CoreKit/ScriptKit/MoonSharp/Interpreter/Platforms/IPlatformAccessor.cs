using System.IO;
using System.Text;

namespace MoonSharp.Interpreter.Platforms
{
	/// <summary>
	/// Interface to abstract all accesses made to the underlying platform (OS, framework) by the scripting engine.
	/// Can be used both to support "non-standard" platforms (i.e. non-posix, non-windows) and/or to sandbox the behaviour
	/// of the scripting engine.
	/// 
	/// It's recommended that no class implement IPlatformAccessor directly, and rather extend <see cref="PlatformAccessorBase"/>.
	/// </summary>
	public interface IPlatformAccessor
	{
		/// <summary>
		/// Filters the CoreModules enumeration to exclude non-supported operations
		/// </summary>
		/// <param name="module">The requested modules.</param>
		/// <returns>
		/// The requested modules, with unsupported modules filtered out.
		/// </returns>
		CoreModules FilterSupportedCoreModules(CoreModules module);

		/// <summary>
		/// Gets an environment variable. Must be implemented, but an implementation is allowed
		/// to always return null if a more meaningful implementation cannot be achieved or is
		/// not desired.
		/// </summary>
		/// <param name="envvarname">The envvarname.</param>
		/// <returns>
		/// The environment variable value, or null if not found
		/// </returns>
		string GetEnvironmentVariable(string envvarname);

		/// <summary>
		/// Determines whether the application is running in AOT (ahead-of-time) mode
		/// </summary>
		bool IsRunningOnAOT();

		/// <summary>
		/// Gets the name of the platform (used for debug purposes).
		/// </summary>
		/// <returns>The name of the platform (used for debug purposes)</returns>
		string GetPlatformName();

		/// <summary>
		/// Default handler for 'print' calls. Can be customized in ScriptOptions
		/// </summary>
		/// <param name="content">The content.</param>
		void DefaultPrint(string content);

		/// <summary>
		/// Default handler for interactive line input calls. Can be customized in ScriptOptions.
		/// If a meaningful implementation cannot be provided, this method should return null.
		/// </summary>
		/// <returns></returns>
		string DefaultInput(string prompt);

		/// <summary>
		/// A function used to open files in the 'io' module.
		/// Can have an invalid implementation if 'io' module is filtered out.
		/// It should return a correctly initialized Stream for the given file and access
		/// </summary>
		/// <param name="script">The script.</param>
		/// <param name="filename">The filename.</param>
		/// <param name="encoding">The encoding.</param>
		/// <param name="mode">The mode (as per Lua usage - e.g. 'w+', 'rb', etc.).</param>
		/// <returns></returns>
		Stream IO_OpenFile(Script script, string filename, Encoding encoding, string mode);

		/// <summary>
		/// Gets a standard stream (stdin, stdout, stderr).
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		Stream IO_GetStandardStream(StandardFileType type);

		/// <summary>
		/// Gets a temporary filename. Used in 'io' and 'os' modules.
		/// Can have an invalid implementation if 'io' and 'os' modules are filtered out.
		/// </summary>
		/// <returns></returns>
		string IO_OS_GetTempFilename();

		/// <summary>
		/// Exits the process, returning the specified exit code.
		/// Can have an invalid implementation if the 'os' module is filtered out.
		/// </summary>
		/// <param name="exitCode">The exit code.</param>
		void OS_ExitFast(int exitCode);

		/// <summary>
		/// Checks if a file exists. Used by the 'os' module.
		/// Can have an invalid implementation if the 'os' module is filtered out.
		/// </summary>
		/// <param name="file">The file.</param>
		/// <returns>True if the file exists, false otherwise.</returns>
		bool OS_FileExists(string file);

		/// <summary>
		/// Deletes the specified file. Used by the 'os' module.
		/// Can have an invalid implementation if the 'os' module is filtered out.
		/// </summary>
		/// <param name="file">The file.</param>
		void OS_FileDelete(string file);

		/// <summary>
		/// Moves the specified file. Used by the 'os' module.
		/// Can have an invalid implementation if the 'os' module is filtered out.
		/// </summary>
		/// <param name="src">The source.</param>
		/// <param name="dst">The DST.</param>
		void OS_FileMove(string src, string dst);

		/// <summary>
		/// Executes the specified command line, returning the child process exit code and blocking in the meantime.
		/// Can have an invalid implementation if the 'os' module is filtered out.
		/// </summary>
		/// <param name="cmdline">The cmdline.</param>
		/// <returns></returns>
		int OS_Execute(string cmdline);

	}
}
