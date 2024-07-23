
namespace MoonSharp.Interpreter.Diagnostics
{
	/// <summary>
	/// Enumeration of the possible performance counters
	/// </summary>
	public enum PerformanceCounter
	{
		/// <summary>
		/// Measures the time spent parsing the source creating the AST
		/// </summary>
		AstCreation,
		/// <summary>
		/// Measures the time spent converting ASTs in bytecode
		/// </summary>
		Compilation,
		/// <summary>
		/// Measures the time spent in executing scripts
		/// </summary>
		Execution,
		/// <summary>
		/// Measures the on the fly creation/compilation of functions in userdata descriptors
		/// </summary>
		AdaptersCompilation,

		/// <summary>
		/// Sentinel value to get the enum size
		/// </summary>
		LastValue
	}
}
