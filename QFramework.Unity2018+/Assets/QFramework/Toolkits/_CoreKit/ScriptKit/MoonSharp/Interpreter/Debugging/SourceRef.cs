using System;

namespace MoonSharp.Interpreter.Debugging
{
	/// <summary>
	/// Class representing a reference to source code interval
	/// </summary>
	public class SourceRef
	{
		/// <summary>
		/// Gets a value indicating whether this location is inside CLR .
		/// </summary>
		public bool IsClrLocation { get; private set; }

		/// <summary>
		/// Gets the index of the source.
		/// </summary>
		public int SourceIdx { get; private set; }
		/// <summary>
		/// Gets from which column the source code ref starts
		/// </summary>
		public int FromChar { get; private set; }
		/// <summary>
		/// Gets to which column the source code ref ends
		/// </summary>
		public int ToChar { get; private set; }
		/// <summary>
		/// Gets from which line the source code ref starts
		/// </summary>
		public int FromLine { get; private set; }
		/// <summary>
		/// Gets to which line the source code ref ends
		/// </summary>
		public int ToLine { get; private set; }
		/// <summary>
		/// Gets a value indicating whether this instance is a stop "step" in source mode
		/// </summary>
		public bool IsStepStop { get; private set; }

		/// <summary>
		/// Gets a value indicating whether this instance is a breakpoint
		/// </summary>
		public bool Breakpoint;
		/// <summary>
		/// Gets a value indicating whether this instance cannot be set as a breakpoint
		/// </summary>
		public bool CannotBreakpoint { get; private set; }

		internal static SourceRef GetClrLocation()
		{
			return new SourceRef(0, 0, 0, 0, 0, false) { IsClrLocation = true };
		}

		public SourceRef(SourceRef src, bool isStepStop)
		{
			SourceIdx = src.SourceIdx;
			FromChar = src.FromChar;
			ToChar = src.ToChar;
			FromLine = src.FromLine;
			ToLine = src.ToLine;
			IsStepStop = isStepStop;
		}


		public SourceRef(int sourceIdx, int from, int to, int fromline, int toline, bool isStepStop)
		{
			SourceIdx = sourceIdx;
			FromChar = from;
			ToChar = to;
			FromLine = fromline;
			ToLine = toline;
			IsStepStop = isStepStop;
		}

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String" /> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return string.Format("[{0}]{1} ({2}, {3}) -> ({4}, {5})",
				SourceIdx, IsStepStop ? "*" : " ",
				FromLine, FromChar,
				ToLine, ToChar);
		}

		internal int GetLocationDistance(int sourceIdx, int line, int col)
		{
			const int PER_LINE_FACTOR = 1600; // we avoid computing real lines length and approximate with heuristics..

			if (sourceIdx != SourceIdx)
				return int.MaxValue;

			if (FromLine == ToLine)
			{
				if (line == FromLine)
				{
					if (col >= FromChar && col <= ToChar)
						return 0;
					else if (col < FromChar)
						return FromChar - col;
					else
						return col - ToChar;
				}
				else
				{
					return Math.Abs(line - FromLine) * PER_LINE_FACTOR;
				}
			}
			else if (line == FromLine)
			{
				if (col < FromChar)
					return FromChar - col;
				else
					return 0;
			}
			else if (line == ToLine)
			{
				if (col > ToChar)
					return col - ToChar;
				else
					return 0;
			}
			else if (line > FromLine && line < ToLine)
			{
				return 0;
			}
			else if (line < FromLine)
			{
				return (FromLine - line) * PER_LINE_FACTOR;
			}
			else
			{
				return (line - ToLine) * PER_LINE_FACTOR;
			}
		}

		/// <summary>
		/// Gets whether the source ref includes the specified location
		/// </summary>
		/// <param name="sourceIdx">Index of the source.</param>
		/// <param name="line">The line.</param>
		/// <param name="col">The column.</param>
		/// <returns></returns>
		public bool IncludesLocation(int sourceIdx, int line, int col)
		{
			if (sourceIdx != SourceIdx || line < FromLine || line > ToLine)
				return false;

			if (FromLine == ToLine)
				return col >= FromChar && col <= ToChar;
			if (line == FromLine)
				return col >= FromChar;
			if (line == ToLine)
				return col <= ToChar;

			return true;
		}

		/// <summary>
		/// Sets the CannotBreakpoint flag.
		/// </summary>
		/// <returns></returns>
		public SourceRef SetNoBreakPoint()
		{
			CannotBreakpoint = true;
			return this;
		}

		/// <summary>
		/// Formats the location according to script preferences
		/// </summary>
		/// <param name="script">The script.</param>
		/// <param name="forceClassicFormat">if set to <c>true</c> the classic Lua format is forced.</param>
		/// <returns></returns>
		public string FormatLocation(Script script, bool forceClassicFormat = false)
		{
			SourceCode sc = script.GetSourceCode(this.SourceIdx);

			if (this.IsClrLocation)
				return "[clr]";

			if (script.Options.UseLuaErrorLocations || forceClassicFormat)
			{
				return string.Format("{0}:{1}", sc.Name, this.FromLine);
			}
			else if (this.FromLine == this.ToLine)
			{
				if (this.FromChar == this.ToChar)
				{
					return string.Format("{0}:({1},{2})", sc.Name, this.FromLine, this.FromChar, this.ToLine, this.ToChar);
				}
				else
				{
					return string.Format("{0}:({1},{2}-{4})", sc.Name, this.FromLine, this.FromChar, this.ToLine, this.ToChar);
				}
			}
			else
			{
				return string.Format("{0}:({1},{2}-{3},{4})", sc.Name, this.FromLine, this.FromChar, this.ToLine, this.ToChar);
			}
		}
	}
}
