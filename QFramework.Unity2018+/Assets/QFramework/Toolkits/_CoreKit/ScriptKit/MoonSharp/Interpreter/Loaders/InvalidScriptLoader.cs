using System;

namespace MoonSharp.Interpreter.Loaders
{
	/// <summary>
	/// A script loader used for platforms we cannot initialize in any better way..
	/// </summary>
	internal class InvalidScriptLoader : IScriptLoader
	{
		string m_Error;

		internal InvalidScriptLoader(string frameworkname)
		{
			m_Error = string.Format(
@"Loading scripts from files is not automatically supported on {0}. 
Please implement your own IScriptLoader (possibly, extending ScriptLoaderBase for easier implementation),
use a preexisting loader like EmbeddedResourcesScriptLoader or UnityAssetsScriptLoader or load scripts from strings.", frameworkname);
		}

		public object LoadFile(string file, Table globalContext)
		{
			throw new PlatformNotSupportedException(m_Error);
		}

		public string ResolveFileName(string filename, Table globalContext)
		{
			return filename;
		}

		public string ResolveModuleName(string modname, Table globalContext)
		{
			throw new PlatformNotSupportedException(m_Error);
		}
	}
}
