using System;

namespace MoonSharp.Interpreter.Diagnostics.PerformanceCounters
{
	internal interface IPerformanceStopwatch
	{
		IDisposable Start();
		PerformanceResult GetResult();
	}
}
