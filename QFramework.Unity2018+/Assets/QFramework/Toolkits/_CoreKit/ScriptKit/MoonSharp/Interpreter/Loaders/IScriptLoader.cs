using System;

namespace MoonSharp.Interpreter.Loaders
{
	/// <summary>
	/// Class dictating how requests to read scripts from files are handled.
	/// 
	/// It's recommended that no class implement IScriptLoader directly, and rather extend ScriptLoaderBase.
	/// </summary>
	public interface IScriptLoader 
	{
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
		object LoadFile(string file, Table globalContext);
		/// <summary>
		/// Resolves a filename [applying paths, etc.]
		/// </summary>
		/// <param name="filename">The filename.</param>
		/// <param name="globalContext">The global context.</param>
		/// <returns></returns>
		[Obsolete("This serves almost no purpose. Kept here just to preserve backward compatibility.")]
		string ResolveFileName(string filename, Table globalContext);
		/// <summary>
		/// Resolves the name of a module to a filename (which will later be passed to OpenScriptFile)
		/// </summary>
		/// <param name="modname">The modname.</param>
		/// <param name="globalContext">The global context.</param>
		/// <returns></returns>
		string ResolveModuleName(string modname, Table globalContext);
	}
}
