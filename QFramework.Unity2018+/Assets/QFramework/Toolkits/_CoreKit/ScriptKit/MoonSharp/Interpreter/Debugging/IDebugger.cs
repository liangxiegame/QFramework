using System.Collections.Generic;

namespace MoonSharp.Interpreter.Debugging
{
	/// <summary>
	/// Interface for debuggers to implement, in order to provide debugging facilities to Scripts.
	/// </summary>
	public interface IDebugger
	{
		/// <summary>
		/// Gets the debugger caps.
		/// </summary>
		DebuggerCaps GetDebuggerCaps();
		/// <summary>
		/// Sets the debug service for this debugger
		/// </summary>
		/// <param name="debugService">The debug service.</param>
		void SetDebugService(DebugService debugService);
		/// <summary>
		/// Called by the script engine  when a source code is added or changed.
		/// </summary>
		/// <param name="sourceCode">The source code object.</param>
		void SetSourceCode(SourceCode sourceCode);
		/// <summary>
		/// Called by the script engine  when the bytecode changes.
		/// </summary>
		/// <param name="byteCode">The bytecode source</param>
		void SetByteCode(string[] byteCode);
		/// <summary>
		/// Called by the script engine at execution time to check if a break has 
		/// been requested. Should return pretty fast as it's called A LOT.
		/// </summary>
		bool IsPauseRequested();
		/// <summary>
		/// Called by the script engine when a runtime error occurs. 
		/// The debugger can return true to signal the engine that it wants to break 
		/// into the source of the error. If it does so, it should also return true 
		/// to subsequent calls to IsPauseRequested().
		/// </summary>
		/// <param name="ex">The runtime exception.</param>
		/// <returns>True if this error should break execution.</returns>
		bool SignalRuntimeException(ScriptRuntimeException ex);
		/// <summary>
		/// Called by the script engine to get what action to do next.
		/// </summary>
		/// <param name="ip">The instruction pointer in bytecode.</param>
		/// <param name="sourceref">The source reference.</param>
		/// <returns>T</returns>
		DebuggerAction GetAction(int ip, SourceRef sourceref);
		/// <summary>
		/// Called by the script engine when the execution ends.
		/// </summary>
		void SignalExecutionEnded();
		/// <summary>
		/// Called by the script engine to update watches of a given type. Note 
		/// that this method is not called only for watches in the strictest term, 
		/// but also for the stack, etc.
		/// </summary>
		/// <param name="watchType">Type of the watch.</param>
		/// <param name="items">The items.</param>
		void Update(WatchType watchType, IEnumerable<WatchItem> items);
		/// <summary>
		/// Called by the script engine to get which expressions are active 
		/// watches in the debugger.
		/// </summary>
		/// <returns>A list of watches</returns>
		List<DynamicExpression> GetWatchItems();
		/// <summary>
		/// Called by the script engine to refresh the breakpoint list.
		/// </summary>
		void RefreshBreakpoints(IEnumerable<SourceRef> refs);
	}
}
