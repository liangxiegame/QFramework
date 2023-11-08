using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoonSharp.Interpreter.Debugging
{
	/// <summary>
	/// Enumeration of capabilities for a debugger
	/// </summary>
	[Flags]
	public enum DebuggerCaps
	{
		/// <summary>
		/// Flag set if the debugger can debug source code
		/// </summary>
		CanDebugSourceCode = 0x1,
		/// <summary>
		/// Flag set if the can debug VM bytecode
		/// </summary>
		CanDebugByteCode = 0x2,
		/// <summary>
		/// Flag set if the debugger uses breakpoints based on lines instead of tokens
		/// </summary>
		HasLineBasedBreakpoints = 0x4
	}
}
