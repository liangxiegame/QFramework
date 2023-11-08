using System;
using System.Diagnostics;

namespace MoonSharp.Interpreter.Diagnostics.PerformanceCounters
{
	/// <summary>
	/// This class is not *really* IDisposable.. it's just use to have a RAII like pattern.
	/// You are free to reuse this instance after calling Dispose.
	/// </summary>
	internal class GlobalPerformanceStopwatch : IPerformanceStopwatch
	{
		private class GlobalPerformanceStopwatch_StopwatchObject : IDisposable
		{
			Stopwatch m_Stopwatch;
			GlobalPerformanceStopwatch m_Parent;

			public GlobalPerformanceStopwatch_StopwatchObject(GlobalPerformanceStopwatch parent)
			{
				m_Parent = parent;
				m_Stopwatch = Stopwatch.StartNew();
			}

			public void Dispose()
			{
				m_Stopwatch.Stop();
				m_Parent.SignalStopwatchTerminated(m_Stopwatch);
			}
		}

		int m_Count = 0;
		long m_Elapsed = 0;
		PerformanceCounter m_Counter;

		public GlobalPerformanceStopwatch(PerformanceCounter perfcounter)
		{
			m_Counter = perfcounter;
		}

		private void SignalStopwatchTerminated(Stopwatch sw)
		{
			m_Elapsed += sw.ElapsedMilliseconds;
			m_Count += 1;
		}

		public IDisposable Start()
		{
			return new GlobalPerformanceStopwatch_StopwatchObject(this);
		}

		public PerformanceResult GetResult()
		{
			return new PerformanceResult()
			{
				Type = PerformanceCounterType.TimeMilliseconds,
				Global = false,
				Name = m_Counter.ToString(),
				Instances = m_Count,
				Counter = m_Elapsed
			};
		}
	}
}
