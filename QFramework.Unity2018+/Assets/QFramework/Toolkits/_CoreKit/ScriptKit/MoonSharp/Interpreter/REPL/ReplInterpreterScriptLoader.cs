#if !(PCL || ENABLE_DOTNET || NETFX_CORE)
using System;
using MoonSharp.Interpreter.Loaders;

namespace MoonSharp.Interpreter.REPL
{
	/// <summary>
	/// A script loader loading scripts directly from the file system (does not go through platform object)
	/// AND starts with module paths taken from environment variables (again, not going through the platform object).
	/// 
	/// The paths are preconstructed using :
	///		* The MOONSHARP_PATH environment variable if it exists
	///		* The LUA_PATH_5_2 environment variable if MOONSHARP_PATH does not exists
	///		* The LUA_PATH environment variable if LUA_PATH_5_2 and MOONSHARP_PATH do not exists
	///		* The "?;?.lua" path if all the above fail
	///		
	/// Also, everytime a module is require(d), the "LUA_PATH" global variable is checked. If it exists, those paths
	/// will be used to load the module instead of the global ones.
	/// </summary>
	public class ReplInterpreterScriptLoader : FileSystemScriptLoader
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReplInterpreterScriptLoader"/> class.
		/// </summary>
		public ReplInterpreterScriptLoader()
		{
			string env = Environment.GetEnvironmentVariable("MOONSHARP_PATH");
			if (!string.IsNullOrEmpty(env)) ModulePaths = UnpackStringPaths(env);

			if (ModulePaths == null)
			{
				env = Environment.GetEnvironmentVariable("LUA_PATH_5_2");
				if (!string.IsNullOrEmpty(env)) ModulePaths = UnpackStringPaths(env);
			}

			if (ModulePaths == null)
			{
				env = Environment.GetEnvironmentVariable("LUA_PATH");
				if (!string.IsNullOrEmpty(env)) ModulePaths = UnpackStringPaths(env);
			}

			if (ModulePaths == null)
			{
				ModulePaths = UnpackStringPaths("?;?.lua");
			}
		}

		/// <summary>
		/// Resolves the name of a module to a filename (which will later be passed to OpenScriptFile).
		/// The resolution happens first on paths included in the LUA_PATH global variable, and -
		/// if the variable does not exist - by consulting the
		/// ScriptOptions.ModulesPaths array. Override to provide a different behaviour.
		/// </summary>
		/// <param name="modname">The modname.</param>
		/// <param name="globalContext">The global context.</param>
		/// <returns></returns>
		public override string ResolveModuleName(string modname, Table globalContext)
		{
			DynValue s = globalContext.RawGet("LUA_PATH");

			if (s != null && s.Type == DataType.String)
				return ResolveModuleName(modname, UnpackStringPaths(s.String));

			else
				return base.ResolveModuleName(modname, globalContext);
		}
	}
}


#endif