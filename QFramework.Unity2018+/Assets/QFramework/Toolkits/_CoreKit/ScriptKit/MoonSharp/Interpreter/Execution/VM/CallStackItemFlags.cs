using System;

namespace MoonSharp.Interpreter.Execution.VM
{
	[Flags]
	internal enum CallStackItemFlags
	{
		None = 0,

		EntryPoint = 1,
		ResumeEntryPoint = 3,
		CallEntryPoint = 5,

		TailCall = 0x10,
		MethodCall = 0x20,
	}
}
