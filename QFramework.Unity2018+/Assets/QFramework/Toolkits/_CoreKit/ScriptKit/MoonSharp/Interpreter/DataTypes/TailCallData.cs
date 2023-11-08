
namespace MoonSharp.Interpreter
{
	/// <summary>
	/// Class used to support "tail" continuations - a way for C# / Lua interaction which supports 
	/// coroutine yielding (at the expense of a LOT of added complexity in calling code).
	/// </summary>
	public class TailCallData
	{
		/// <summary>
		/// Gets or sets the function to call
		/// </summary>
		public DynValue Function { get; set; }
		/// <summary>
		/// Gets or sets the arguments to the function
		/// </summary>
		public DynValue[] Args { get; set; }
		/// <summary>
		/// Gets or sets the callback to be used as a continuation.
		/// </summary>
		public CallbackFunction Continuation { get; set; }
		/// <summary>
		/// Gets or sets the callback to be used in case of errors.
		/// </summary>
		public CallbackFunction ErrorHandler { get; set; }
		/// <summary>
		/// Gets or sets the error handler to be called before stack unwinding
		/// </summary>
		public DynValue ErrorHandlerBeforeUnwind { get; set; }
	}
}
