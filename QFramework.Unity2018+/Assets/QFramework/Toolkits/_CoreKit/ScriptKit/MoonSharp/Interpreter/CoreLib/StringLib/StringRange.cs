// Disable warnings about XML documentation
#pragma warning disable 1591


namespace MoonSharp.Interpreter.CoreLib.StringLib
{
	internal class StringRange
	{
		public int Start { get; set; }
		public int End { get; set; }

		public StringRange()
		{
			Start = 0;
			End = 0;
		}

		public StringRange(int start, int end)
		{
			Start = start;
			End = end;
		}

		public static StringRange FromLuaRange(DynValue start, DynValue end, int? defaultEnd = null)
		{
			int i = start.IsNil() ? 1 : (int)start.Number;
			int j = end.IsNil() ? (defaultEnd ?? i) : (int)end.Number;

			return new StringRange(i, j);
		}


		// Returns the substring of s that starts at i and continues until j; i and j can be negative. 
		// If, after the translation of negative indices, i is less than 1, it is corrected to 1. 
		// If j is greater than the string length, it is corrected to that length. 
		// If, after these corrections, i is greater than j, the function returns the empty string. 		
		public string ApplyToString(string value)
		{
			int i = Start < 0 ? Start + value.Length + 1 : Start;
			int j = End < 0 ? End + value.Length + 1 : End;

			if (i < 1) i = 1;
			if (j > value.Length) j = value.Length;

			if (i > j)
				return string.Empty;

			return value.Substring(i - 1, j - i + 1);
		}

		public int Length()
		{
			return (End - Start) + 1;
		}
	}
}
