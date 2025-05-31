using System;
using System.Linq;

namespace MoonSharp.Interpreter.Loaders
{
	/// <summary>
	/// A base implementation of IScriptLoader, offering resolution of module names.
	/// </summary>
	public abstract class ScriptLoaderBase : IScriptLoader
	{
		/// <summary>
		/// Checks if a script file exists. 
		/// </summary>
		/// <param name="name">The script filename.</param>
		/// <returns></returns>
		public abstract bool ScriptFileExists(string name);

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
		public abstract object LoadFile(string file, Table globalContext);


		/// <summary>
		/// Resolves the name of a module on a set of paths.
		/// </summary>
		/// <param name="modname">The modname.</param>
		/// <param name="paths">The paths.</param>
		/// <returns></returns>
		protected virtual string ResolveModuleName(string modname, string[] paths)
		{
			if (paths == null) 
				return null;

			modname = modname.Replace('.', '/');

			foreach (string path in paths)
			{
				string file = path.Replace("?", modname);

				if (ScriptFileExists(file))
					return file;
			}

			return null;
		}

		/// <summary>
		/// Resolves the name of a module to a filename (which will later be passed to OpenScriptFile).
		/// The resolution happens first on paths included in the LUA_PATH global variable (if and only if
		/// the IgnoreLuaPathGlobal is false), and - if the variable does not exist - by consulting the
		/// ScriptOptions.ModulesPaths array. Override to provide a different behaviour.
		/// </summary>
		/// <param name="modname">The modname.</param>
		/// <param name="globalContext">The global context.</param>
		/// <returns></returns>
		public virtual string ResolveModuleName(string modname, Table globalContext)
		{
			if (!this.IgnoreLuaPathGlobal)
			{
				DynValue s = globalContext.RawGet("LUA_PATH");

				if (s != null && s.Type == DataType.String)
					return ResolveModuleName(modname, UnpackStringPaths(s.String));
			}

			return ResolveModuleName(modname, this.ModulePaths);
		}

		/// <summary>
		/// Gets or sets the modules paths used by the "require" function. If null, the default paths are used (using
		/// environment variables etc.). 
		/// </summary>
		public string[] ModulePaths { get; set; }

		/// <summary>
		/// Unpacks a string path in a form like "?;?.lua" to an array
		/// </summary>
		public static string[] UnpackStringPaths(string str)
		{
			return str.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
				.Select(s => s.Trim())
				.Where(s => !string.IsNullOrEmpty(s))
				.ToArray();
		}

		/// <summary>
		/// Gets the default environment paths.
		/// </summary>
		public static string[] GetDefaultEnvironmentPaths()
		{
			string[] modulePaths = null;

			if (modulePaths == null)
			{
				string env = Script.GlobalOptions.Platform.GetEnvironmentVariable("MOONSHARP_PATH");
				if (!string.IsNullOrEmpty(env)) modulePaths = UnpackStringPaths(env);

				if (modulePaths == null)
				{
					env = Script.GlobalOptions.Platform.GetEnvironmentVariable("LUA_PATH");
					if (!string.IsNullOrEmpty(env)) modulePaths = UnpackStringPaths(env);
				}

				if (modulePaths == null)
					modulePaths = UnpackStringPaths("?;?.lua");
			}

			return modulePaths;
		}



		/// <summary>
		/// Resolves a filename [applying paths, etc.]
		/// </summary>
		/// <param name="filename">The filename.</param>
		/// <param name="globalContext">The global context.</param>
		/// <returns></returns>
		public virtual string ResolveFileName(string filename, Table globalContext)
		{
			return filename;
		}

		/// <summary>
		/// Gets or sets a value indicating whether the LUA_PATH global is checked or not to get the path where modules are contained.
		/// If true, the LUA_PATH global is NOT checked.
		/// </summary>
		public bool IgnoreLuaPathGlobal { get; set; }
	}
}
