using System;

namespace MoonSharp.Interpreter.Diagnostics
{
	/// <summary>
	/// The result of a performance counter
	/// </summary>
	public class PerformanceResult
	{
		/// <summary>
		/// Gets the name of the performance counter which generated this result.
		/// </summary>
		public string Name { get; internal set; }
		/// <summary>
		/// Gets the quantity monitored - see Type to understand what this field contains
		/// </summary>
		public long Counter { get; internal set; }
		/// <summary>
		/// Gets the number of instances which led to the specified counter being incremented - e.g. the times a specific
		/// code is executed, or object instanced
		/// </summary>
		public int Instances { get; internal set; }
		/// <summary>
		/// Gets a value indicating whether this <see cref="PerformanceResult"/> is global or relative to the resource
		/// for which it's called.
		/// </summary>
		public bool Global { get; internal set; }
		/// <summary>
		/// Gets the unit of measure of the Counter field.
		/// </summary>
		public PerformanceCounterType Type { get; internal set; }

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		public override string ToString()
		{
			return string.Format("{0}{1} : {2} times / {3} {4}",
				Name,
				Global ? "(g)" : "",
				Instances,
				Counter,
				PerformanceCounterTypeToString(Type));
		}

		/// <summary>
		/// Converts a PerformanceCounterType to a string.
		/// </summary>
		/// <param name="Type">The type.</param>
		public static string PerformanceCounterTypeToString(PerformanceCounterType Type)
		{
			switch (Type)
			{
				case PerformanceCounterType.MemoryBytes:
					return "bytes";
				case PerformanceCounterType.TimeMilliseconds:
					return "ms";
				default:
					throw new InvalidOperationException("PerformanceCounterType has invalid value " + Type.ToString());
			}
		}
	}
}
