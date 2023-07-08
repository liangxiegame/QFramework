#if DOTNET_CORE || (!(PCL || ENABLE_DOTNET || NETFX_CORE))
using System.IO;

namespace MoonSharp.Interpreter.Loaders
{
	/// <summary>
	/// A script loader loading scripts directly from the file system (does not go through platform object)
	/// </summary>
	public class FileSystemScriptLoader : ScriptLoaderBase
	{
		/// <summary>
		/// Checks if a script file exists.
		/// </summary>
		/// <param name="name">The script filename.</param>
		/// <returns></returns>
		public override bool ScriptFileExists(string name)
		{
			return File.Exists(name);
		}

		/// <summary>
		/// Opens a file for reading the script code.
		/// It can return either a string, a byte[] or a Stream.
		/// If a byte[] is returned, the content is assumed to be a serialized (dumped) bytecode. If it's a string, it's
		/// assumed to be either a script or the output of a string.dump call. If a Stream, autodetection takes place.
		/// </summary>
		/// <param name="file">The file.</param>
		/// <param name="globalContext">The global context.</param>
		/// <returns>
		/// A string, a byte[] or a Stream.
		/// </returns>
		public override object LoadFile(string file, Table globalContext)
		{
			return new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		}
	}
}
#endif