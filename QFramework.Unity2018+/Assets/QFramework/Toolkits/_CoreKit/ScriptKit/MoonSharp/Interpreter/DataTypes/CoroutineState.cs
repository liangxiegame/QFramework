
namespace MoonSharp.Interpreter
{
	/// <summary>
	/// State of coroutines
	/// </summary>
	public enum CoroutineState
	{
		/// <summary>
		/// This is the main coroutine
		/// </summary>
		Main,
		/// <summary>
		/// Coroutine has not started yet
		/// </summary>
		NotStarted,
		/// <summary>
		/// Coroutine is suspended
		/// </summary>
		Suspended,
		/// <summary>
		/// Coroutine has been forcefully suspended (i.e. auto-yielded)
		/// </summary>
		ForceSuspended,
		/// <summary>
		/// Coroutine is running
		/// </summary>
		Running,
		/// <summary>
		/// Coroutine has terminated
		/// </summary>
		Dead
	}
}
