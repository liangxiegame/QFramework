using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MoonSharp.Interpreter.Loaders
{
	/// <summary>
	/// A script loader loading scripts from an assembly resources
	/// </summary>
	public class EmbeddedResourcesScriptLoader : ScriptLoaderBase
	{
		Assembly m_ResourceAssembly;
		HashSet<string> m_ResourceNames;
		string m_Namespace;

		/// <summary>
		/// Initializes a new instance of the <see cref="EmbeddedResourcesScriptLoader"/> class.
		/// </summary>
		/// <param name="resourceAssembly">The assembly containing the scripts as embedded resources or null to use the calling assembly.</param>
		public EmbeddedResourcesScriptLoader(Assembly resourceAssembly = null)
		{
			if (resourceAssembly == null)
			{
#if NETFX_CORE || DOTNET_CORE
				throw new NotSupportedException("Assembly.GetCallingAssembly is not supported on target framework.");
#else
				resourceAssembly = Assembly.GetCallingAssembly();
#endif
			}

			m_ResourceAssembly = resourceAssembly;
			m_Namespace = m_ResourceAssembly.FullName.Split(',').First();
			m_ResourceNames = new HashSet<string>(m_ResourceAssembly.GetManifestResourceNames());
		}

		private string FileNameToResource(string file)
		{
			file = file.Replace('/', '.');
			file = file.Replace('\\', '.');
			return m_Namespace + "." + file;
		}

		/// <summary>
		/// Checks if a script file exists.
		/// </summary>
		/// <param name="name">The script filename.</param>
		/// <returns></returns>
		public override bool ScriptFileExists(string name)
		{
			name = FileNameToResource(name);
			return m_ResourceNames.Contains(name);
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
			file = FileNameToResource(file);
			return m_ResourceAssembly.GetManifestResourceStream(file);
		}
	}
}
