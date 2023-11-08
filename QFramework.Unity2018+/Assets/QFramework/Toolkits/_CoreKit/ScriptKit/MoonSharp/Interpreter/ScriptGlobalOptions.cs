using MoonSharp.Interpreter.Interop;
using MoonSharp.Interpreter.Platforms;

namespace MoonSharp.Interpreter
{
	/// <summary>
	/// Class containing script global options, that is options which cannot be customized per-script.
	/// <see cref="Script.GlobalOptions"/>
	/// </summary>
	public class ScriptGlobalOptions
	{
		internal ScriptGlobalOptions()
		{
			Platform = PlatformAutoDetector.GetDefaultPlatform();
			CustomConverters = new CustomConvertersCollection();
		}

		/// <summary>
		/// Gets or sets the custom converters.
		/// </summary>
		public CustomConvertersCollection CustomConverters { get; set; }

		/// <summary>
		/// Gets or sets the platform abstraction to use.
		/// </summary>
		/// <value>
		/// The current platform abstraction.
		/// </value>
		public IPlatformAccessor Platform { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether interpreter exceptions should be 
		/// re-thrown as nested exceptions.
		/// </summary>
		public bool RethrowExceptionNested { get; set; }
	}
}
