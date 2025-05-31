#if (PCL) || (UNITY_5) || NETFX_CORE
// Dummy implementation for PCL and Unity targets
using System;
using System.IO;
using System.Text;

namespace MoonSharp.Interpreter.Platforms
{
	/// <summary>
	/// Class providing the IPlatformAccessor interface for standard full-feaured implementations.
	/// </summary>
	public class StandardPlatformAccessor : PlatformAccessorBase
	{
		public override void DefaultPrint(string content)
		{
			throw new NotImplementedException();
		}

		public override CoreModules FilterSupportedCoreModules(CoreModules module)
		{
			throw new NotImplementedException();
		}

		public override string GetEnvironmentVariable(string envvarname)
		{
			throw new NotImplementedException();
		}

		public override string GetPlatformNamePrefix()
		{
			throw new NotImplementedException();
		}

		public override Stream IO_GetStandardStream(StandardFileType type)
		{
			throw new NotImplementedException();
		}

		public override Stream IO_OpenFile(Script script, string filename, Encoding encoding, string mode)
		{
			throw new NotImplementedException();
		}

		public override string IO_OS_GetTempFilename()
		{
			throw new NotImplementedException();
		}

		public override int OS_Execute(string cmdline)
		{
			throw new NotImplementedException();
		}

		public override void OS_ExitFast(int exitCode)
		{
			throw new NotImplementedException();
		}

		public override void OS_FileDelete(string file)
		{
			throw new NotImplementedException();
		}

		public override bool OS_FileExists(string file)
		{
			throw new NotImplementedException();
		}

		public override void OS_FileMove(string src, string dst)
		{
			throw new NotImplementedException();
		}
	}
}
#else
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace MoonSharp.Interpreter.Platforms
{
	/// <summary>
	/// Class providing the IPlatformAccessor interface for standard full-feaured implementations.
	/// </summary>
	public class StandardPlatformAccessor : PlatformAccessorBase
	{
		/// <summary>
		/// Converts a Lua string access mode to a FileAccess enum
		/// </summary>
		/// <param name="mode">The mode.</param>
		/// <returns></returns>
		public static FileAccess ParseFileAccess(string mode)
		{
			mode = mode.Replace("b", "");

			if (mode == "r")
				return FileAccess.Read;
			else if (mode == "r+")
				return FileAccess.ReadWrite;
			else if (mode == "w")
				return FileAccess.Write;
			else if (mode == "w+")
				return FileAccess.ReadWrite;
			else
				return FileAccess.ReadWrite;
		}

		/// <summary>
		/// Converts a Lua string access mode to a ParseFileMode enum
		/// </summary>
		/// <param name="mode">The mode.</param>
		/// <returns></returns>
		public static FileMode ParseFileMode(string mode)
		{
			mode = mode.Replace("b", "");

			if (mode == "r")
				return FileMode.Open;
			else if (mode == "r+")
				return FileMode.OpenOrCreate;
			else if (mode == "w")
				return FileMode.Create;
			else if (mode == "w+")
				return FileMode.Truncate;
			else
				return FileMode.Append;
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
		public override Stream IO_OpenFile(Script script, string filename, Encoding encoding, string mode)
		{
			return new FileStream(filename, ParseFileMode(mode), ParseFileAccess(mode), FileShare.ReadWrite | FileShare.Delete);
		}

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
			return Environment.GetEnvironmentVariable(envvarname);
		}

		/// <summary>
		/// Gets a standard stream (stdin, stdout, stderr).
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentException">type</exception>
		public override Stream IO_GetStandardStream(StandardFileType type)
		{
			switch (type)
			{
				case StandardFileType.StdIn:
					return Console.OpenStandardInput();
				case StandardFileType.StdOut:
					return Console.OpenStandardOutput();
				case StandardFileType.StdErr:
					return Console.OpenStandardError();
				default:
					throw new ArgumentException("type");
			}
		}

		/// <summary>
		/// Default handler for 'print' calls. Can be customized in ScriptOptions
		/// </summary>
		/// <param name="content">The content.</param>
		public override void DefaultPrint(string content)
		{
			Console.WriteLine(content);
		}


		/// <summary>
		/// Gets a temporary filename. Used in 'io' and 'os' modules.
		/// Can have an invalid implementation if 'io' and 'os' modules are filtered out.
		/// </summary>
		/// <returns></returns>
		public override string IO_OS_GetTempFilename()
		{
			return Path.GetTempFileName();
		}

		/// <summary>
		/// Exits the process, returning the specified exit code.
		/// Can have an invalid implementation if the 'os' module is filtered out.
		/// </summary>
		/// <param name="exitCode">The exit code.</param>
		public override void OS_ExitFast(int exitCode)
		{
			Environment.Exit(exitCode);
		}

		/// <summary>
		/// Checks if a file exists. Used by the 'os' module.
		/// Can have an invalid implementation if the 'os' module is filtered out.
		/// </summary>
		/// <param name="file">The file.</param>
		/// <returns>
		/// True if the file exists, false otherwise.
		/// </returns>
		public override bool OS_FileExists(string file)
		{
			return File.Exists(file);
		}

		/// <summary>
		/// Deletes the specified file. Used by the 'os' module.
		/// Can have an invalid implementation if the 'os' module is filtered out.
		/// </summary>
		/// <param name="file">The file.</param>
		public override void OS_FileDelete(string file)
		{
			File.Delete(file);
		}

		/// <summary>
		/// Moves the specified file. Used by the 'os' module.
		/// Can have an invalid implementation if the 'os' module is filtered out.
		/// </summary>
		/// <param name="src">The source.</param>
		/// <param name="dst">The DST.</param>
		public override void OS_FileMove(string src, string dst)
		{
#if (!PCL) && ((!UNITY_5) || UNITY_STANDALONE)
				File.Move(src, dst);
#endif
		}

		/// <summary>
		/// Executes the specified command line, returning the child process exit code and blocking in the meantime.
		/// Can have an invalid implementation if the 'os' module is filtered out.
		/// </summary>
		/// <param name="cmdline">The cmdline.</param>
		/// <returns></returns>
		public override int OS_Execute(string cmdline)
		{
			// This is windows only!
			ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", string.Format("/C {0}", cmdline));
			psi.ErrorDialog = false;

			Process proc = Process.Start(psi);
			proc.WaitForExit();
			return proc.ExitCode;
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
			return module;
		}

		/// <summary>
		/// Gets the platform name prefix
		/// </summary>
		/// <returns></returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public override string GetPlatformNamePrefix()
		{
			return "std";
		}
	}
}
#endif