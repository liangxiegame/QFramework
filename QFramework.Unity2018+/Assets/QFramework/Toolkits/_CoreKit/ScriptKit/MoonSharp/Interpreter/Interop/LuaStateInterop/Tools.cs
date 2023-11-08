//
// This part taken from KopiLua - https://github.com/NLua/KopiLua
//
// =========================================================================================================
//
// Kopi Lua License
// ----------------
// MIT License for KopiLua
// Copyright (c) 2012 LoDC
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
// associated documentation files (the "Software"), to deal in the Software without restriction,
// including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial
// portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ===============================================================================
// Lua License
// -----------
// Lua is licensed under the terms of the MIT license reproduced below.
// This means that Lua is free software and can be used for both academic
// and commercial purposes at absolutely no cost.
// For details and rationale, see http://www.lua.org/license.html .
// ===============================================================================
// Copyright (C) 1994-2008 Lua.org, PUC-Rio.
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.


#region Usings
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;


#endregion

namespace MoonSharp.Interpreter.Interop.LuaStateInterop
{
	internal static class Tools
	{
		#region Public Methods
		#region IsNumericType
		/// <summary>
		/// Determines whether the specified value is of numeric type.
		/// </summary>
		/// <param name="o">The object to check.</param>
		/// <returns>
		/// 	<c>true</c> if o is a numeric type; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsNumericType(object o)
		{
			return (o is byte ||
				o is sbyte ||
				o is short ||
				o is ushort ||
				o is int ||
				o is uint ||
				o is long ||
				o is ulong ||
				o is float ||
				o is double ||
				o is decimal);
		}
		#endregion
		#region IsPositive
		/// <summary>
		/// Determines whether the specified value is positive.
		/// </summary>
		/// <param name="Value">The value.</param>
		/// <param name="ZeroIsPositive">if set to <c>true</c> treats 0 as positive.</param>
		/// <returns>
		/// 	<c>true</c> if the specified value is positive; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsPositive(object Value, bool ZeroIsPositive)
		{
			Type t = Value.GetType();

			if (t == typeof(sbyte))
				return (ZeroIsPositive ? (sbyte)Value >= 0 : (sbyte)Value > 0);
			if (t == typeof(short))
				return (ZeroIsPositive ? (short)Value >= 0 : (short)Value > 0);
			if (t == typeof(int))
				return (ZeroIsPositive ? (int)Value >= 0 : (int)Value > 0);
			if (t == typeof(long))
				return (ZeroIsPositive ? (long)Value >= 0 : (long)Value > 0);
			if (t == typeof(byte))
				return (ZeroIsPositive ? true : (byte)Value > 0);
			if (t == typeof(ushort))
				return (ZeroIsPositive ? true : (ushort)Value > 0);
			if (t == typeof(uint))
				return (ZeroIsPositive ? true : (uint)Value > 0);
			if (t == typeof(ulong))
				return (ZeroIsPositive ? true : (ulong)Value > 0);
			if (t == typeof(float))
				return (ZeroIsPositive ? (float)Value >= 0 : (float)Value > 0);
			if (t == typeof(double))
				return (ZeroIsPositive ? (double)Value >= 0 : (double)Value > 0);
			if (t == typeof(decimal))
				return (ZeroIsPositive ? (decimal)Value >= 0 : (decimal)Value > 0);
			if (t == typeof(char))
				return (ZeroIsPositive ? true : (char)Value != '\0');

			return ZeroIsPositive;
		}
		#endregion
		#region ToUnsigned
		/// <summary>
		/// Converts the specified values boxed type to its correpsonding unsigned
		/// type.
		/// </summary>
		/// <param name="Value">The value.</param>
		/// <returns>A boxed numeric object whos type is unsigned.</returns>
		public static object ToUnsigned(object Value)
		{
			Type t = Value.GetType();

			if (t == typeof(sbyte))
				return (byte)((sbyte)Value);
			if (t == typeof(short))
				return (ushort)((short)Value);
			if (t == typeof(int))
				return (uint)((int)Value);
			if (t == typeof(long))
				return (ulong)((long)Value);
			if (t == typeof(byte))
				return Value;
			if (t == typeof(ushort))
				return Value;
			if (t == typeof(uint))
				return Value;
			if (t == typeof(ulong))
				return Value;
			if (t == typeof(float))
				return (uint)((float)Value);
			if (t == typeof(double))
				return (ulong)((double)Value);
			if (t == typeof(decimal))
				return (ulong)((decimal)Value);

			return null;
		}
		#endregion
		#region ToInteger
		/// <summary>
		/// Converts the specified values boxed type to its correpsonding integer
		/// type.
		/// </summary>
		/// <param name="Value">The value.</param>
		/// <param name="Round">if set to <c>true</c> [round].</param>
		/// <returns>
		/// A boxed numeric object whos type is an integer type.
		/// </returns>
		public static object ToInteger(object Value, bool Round)
		{
			Type t = Value.GetType();

			if (t == typeof(sbyte))
				return Value;
			if (t == typeof(short))
				return Value;
			if (t == typeof(int))
				return Value;
			if (t == typeof(long))
				return Value;
			if (t == typeof(byte))
				return Value;
			if (t == typeof(ushort))
				return Value;
			if (t == typeof(uint))
				return Value;
			if (t == typeof(ulong))
				return Value;
			if (t == typeof(float))
				return (Round ? (int)Math.Round((float)Value) : (int)((float)Value));
			if (t == typeof(double))
				return (Round ? (long)Math.Round((double)Value) : (long)((double)Value));
			if (t == typeof(decimal))
				return (Round ? Math.Round((decimal)Value) : (decimal)Value);

			return null;
		}
		#endregion
		#region UnboxToLong
		public static long UnboxToLong(object Value, bool Round)
		{
			Type t = Value.GetType();

			if (t == typeof(sbyte))
				return (long)((sbyte)Value);
			if (t == typeof(short))
				return (long)((short)Value);
			if (t == typeof(int))
				return (long)((int)Value);
			if (t == typeof(long))
				return (long)Value;
			if (t == typeof(byte))
				return (long)((byte)Value);
			if (t == typeof(ushort))
				return (long)((ushort)Value);
			if (t == typeof(uint))
				return (long)((uint)Value);
			if (t == typeof(ulong))
				return (long)((ulong)Value);
			if (t == typeof(float))
				return (Round ? (long)Math.Round((float)Value) : (long)((float)Value));
			if (t == typeof(double))
				return (Round ? (long)Math.Round((double)Value) : (long)((double)Value));
			if (t == typeof(decimal))
				return (Round ? (long)Math.Round((decimal)Value) : (long)((decimal)Value));

			return 0;
		}
		#endregion
		#region ReplaceMetaChars
		/// <summary>
		/// Replaces the string representations of meta chars with their corresponding
		/// character values.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <returns>A string with all string meta chars are replaced</returns>
		public static string ReplaceMetaChars(string input)
		{
			return Regex.Replace(input, @"(\\)(\d{3}|[^\d])?", new MatchEvaluator(ReplaceMetaCharsMatch));
		}
		private static string ReplaceMetaCharsMatch(Match m)
		{
			// convert octal quotes (like \040)
			if (m.Groups[2].Length == 3)
				return Convert.ToChar(Convert.ToByte(m.Groups[2].Value, 8)).ToString();
			else
			{
				// convert all other special meta characters
				//TODO: \xhhh hex and possible dec !!
				switch (m.Groups[2].Value)
				{
					case "0":           // null
						return "\0";
					case "a":           // alert (beep)
						return "\a";
					case "b":           // BS
						return "\b";
					case "f":           // FF
						return "\f";
					case "v":           // vertical tab
						return "\v";
					case "r":           // CR
						return "\r";
					case "n":           // LF
						return "\n";
					case "t":           // Tab
						return "\t";
					default:
						// if neither an octal quote nor a special meta character
						// so just remove the backslash
						return m.Groups[2].Value;
				}
			}
		}
		#endregion
		#region fprintf
		public static void fprintf(TextWriter Destination, string Format, params object[] Parameters)
		{
			Destination.Write(Tools.sprintf(Format, Parameters));
		}


		#endregion
		#region sprintf
		internal static Regex r = new Regex(@"\%(\d*\$)?([\'\#\-\+ ]*)(\d*)(?:\.(\d+))?([hl])?([dioxXucsfeEgGpn%])");
		public static string sprintf(string Format, params object[] Parameters)
		{
			#region Variables
			StringBuilder f = new StringBuilder();
			//Regex r = new Regex( @"\%(\d*\$)?([\'\#\-\+ ]*)(\d*)(?:\.(\d+))?([hl])?([dioxXucsfeEgGpn%])" );
			//"%[parameter][flags][width][.precision][length]type"
			Match m = null;
			string w = String.Empty;
			int defaultParamIx = 0;
			int paramIx;
			object o = null;

			bool flagLeft2Right = false;
			bool flagAlternate = false;
			bool flagPositiveSign = false;
			bool flagPositiveSpace = false;
			bool flagZeroPadding = false;
			bool flagGroupThousands = false;

			int fieldLength = 0;
			int fieldPrecision = 0;
			char shortLongIndicator = '\0';
			char formatSpecifier = '\0';
			char paddingCharacter = ' ';
			#endregion

			// find all format parameters in format string
			f.Append(Format);
			m = r.Match(f.ToString());
			while (m.Success)
			{
				#region parameter index
				paramIx = defaultParamIx;
				if (m.Groups[1] != null && m.Groups[1].Value.Length > 0)
				{
					string val = m.Groups[1].Value.Substring(0, m.Groups[1].Value.Length - 1);
					paramIx = Convert.ToInt32(val) - 1;
				};
				#endregion

				#region format flags
				// extract format flags
				flagAlternate = false;
				flagLeft2Right = false;
				flagPositiveSign = false;
				flagPositiveSpace = false;
				flagZeroPadding = false;
				flagGroupThousands = false;
				if (m.Groups[2] != null && m.Groups[2].Value.Length > 0)
				{
					string flags = m.Groups[2].Value;

					flagAlternate = (flags.IndexOf('#') >= 0);
					flagLeft2Right = (flags.IndexOf('-') >= 0);
					flagPositiveSign = (flags.IndexOf('+') >= 0);
					flagPositiveSpace = (flags.IndexOf(' ') >= 0);
					flagGroupThousands = (flags.IndexOf('\'') >= 0);

					// positive + indicator overrides a
					// positive space character
					if (flagPositiveSign && flagPositiveSpace)
						flagPositiveSpace = false;
				}
				#endregion

				#region field length
				// extract field length and 
				// pading character
				paddingCharacter = ' ';
				fieldLength = int.MinValue;
				if (m.Groups[3] != null && m.Groups[3].Value.Length > 0)
				{
					fieldLength = Convert.ToInt32(m.Groups[3].Value);
					flagZeroPadding = (m.Groups[3].Value[0] == '0');
				}
				#endregion

				if (flagZeroPadding)
					paddingCharacter = '0';

				// left2right allignment overrides zero padding
				if (flagLeft2Right && flagZeroPadding)
				{
					flagZeroPadding = false;
					paddingCharacter = ' ';
				}

				#region field precision
				// extract field precision
				fieldPrecision = int.MinValue;
				if (m.Groups[4] != null && m.Groups[4].Value.Length > 0)
					fieldPrecision = Convert.ToInt32(m.Groups[4].Value);
				#endregion

				#region short / long indicator
				// extract short / long indicator
				shortLongIndicator = Char.MinValue;
				if (m.Groups[5] != null && m.Groups[5].Value.Length > 0)
					shortLongIndicator = m.Groups[5].Value[0];
				#endregion

				#region format specifier
				// extract format
				formatSpecifier = Char.MinValue;
				if (m.Groups[6] != null && m.Groups[6].Value.Length > 0)
					formatSpecifier = m.Groups[6].Value[0];
				#endregion

				// default precision is 6 digits if none is specified except
				if (fieldPrecision == int.MinValue &&
					formatSpecifier != 's' &&
					formatSpecifier != 'c' &&
					Char.ToUpper(formatSpecifier) != 'X' &&
					formatSpecifier != 'o')
					fieldPrecision = 6;

				#region get next value parameter
				// get next value parameter and convert value parameter depending on short / long indicator
				if (Parameters == null || paramIx >= Parameters.Length)
					o = null;
				else
				{
					o = Parameters[paramIx];

					if (shortLongIndicator == 'h')
					{
						if (o is int)
							o = (short)((int)o);
						else if (o is long)
							o = (short)((long)o);
						else if (o is uint)
							o = (ushort)((uint)o);
						else if (o is ulong)
							o = (ushort)((ulong)o);
					}
					else if (shortLongIndicator == 'l')
					{
						if (o is short)
							o = (long)((short)o);
						else if (o is int)
							o = (long)((int)o);
						else if (o is ushort)
							o = (ulong)((ushort)o);
						else if (o is uint)
							o = (ulong)((uint)o);
					}
				}
				#endregion

				// convert value parameters to a string depending on the formatSpecifier
				w = String.Empty;
				switch (formatSpecifier)
				{
					#region % - character
					case '%':   // % character
						w = "%";
						break;
					#endregion
					#region d - integer
					case 'd':   // integer
						w = FormatNumber((flagGroupThousands ? "n" : "d"), flagAlternate,
										fieldLength, int.MinValue, flagLeft2Right,
										flagPositiveSign, flagPositiveSpace,
										paddingCharacter, o);
						defaultParamIx++;
						break;
					#endregion
					#region i - integer
					case 'i':   // integer
						goto case 'd';
					#endregion
					#region o - octal integer
					case 'o':   // octal integer - no leading zero
						w = FormatOct("o", flagAlternate,
										fieldLength, int.MinValue, flagLeft2Right,
										paddingCharacter, o);
						defaultParamIx++;
						break;
					#endregion
					#region x - hex integer
					case 'x':   // hex integer - no leading zero
						w = FormatHex("x", flagAlternate,
										fieldLength, fieldPrecision, flagLeft2Right,
										paddingCharacter, o);
						defaultParamIx++;
						break;
					#endregion
					#region X - hex integer
					case 'X':   // same as x but with capital hex characters
						w = FormatHex("X", flagAlternate,
										fieldLength, fieldPrecision, flagLeft2Right,
										paddingCharacter, o);
						defaultParamIx++;
						break;
					#endregion
					#region u - unsigned integer
					case 'u':   // unsigned integer
						w = FormatNumber((flagGroupThousands ? "n" : "d"), flagAlternate,
										fieldLength, int.MinValue, flagLeft2Right,
										false, false,
										paddingCharacter, ToUnsigned(o));
						defaultParamIx++;
						break;
					#endregion
					#region c - character
					case 'c':   // character
						if (IsNumericType(o))
							w = Convert.ToChar(o).ToString();
						else if (o is char)
							w = ((char)o).ToString();
						else if (o is string && ((string)o).Length > 0)
							w = ((string)o)[0].ToString();
						defaultParamIx++;
						break;
					#endregion
					#region s - string
					case 's':   // string
								//string t = "{0" + ( fieldLength != int.MinValue ? "," + ( flagLeft2Right ? "-" : String.Empty ) + fieldLength.ToString() : String.Empty ) + ":s}";
						w = o.ToString();
						if (fieldPrecision >= 0)
							w = w.Substring(0, fieldPrecision);

						if (fieldLength != int.MinValue)
							if (flagLeft2Right)
								w = w.PadRight(fieldLength, paddingCharacter);
							else
								w = w.PadLeft(fieldLength, paddingCharacter);
						defaultParamIx++;
						break;
					#endregion
					#region f - double number
					case 'f':   // double
						w = FormatNumber((flagGroupThousands ? "n" : "f"), flagAlternate,
										fieldLength, fieldPrecision, flagLeft2Right,
										flagPositiveSign, flagPositiveSpace,
										paddingCharacter, o);
						defaultParamIx++;
						break;
					#endregion
					#region e - exponent number
					case 'e':   // double / exponent
						w = FormatNumber("e", flagAlternate,
										fieldLength, fieldPrecision, flagLeft2Right,
										flagPositiveSign, flagPositiveSpace,
										paddingCharacter, o);
						defaultParamIx++;
						break;
					#endregion
					#region E - exponent number
					case 'E':   // double / exponent
						w = FormatNumber("E", flagAlternate,
										fieldLength, fieldPrecision, flagLeft2Right,
										flagPositiveSign, flagPositiveSpace,
										paddingCharacter, o);
						defaultParamIx++;
						break;
					#endregion
					#region g - general number
					case 'g':   // double / exponent
						w = FormatNumber("g", flagAlternate,
										fieldLength, fieldPrecision, flagLeft2Right,
										flagPositiveSign, flagPositiveSpace,
										paddingCharacter, o);
						defaultParamIx++;
						break;
					#endregion
					#region G - general number
					case 'G':   // double / exponent
						w = FormatNumber("G", flagAlternate,
										fieldLength, fieldPrecision, flagLeft2Right,
										flagPositiveSign, flagPositiveSpace,
										paddingCharacter, o);
						defaultParamIx++;
						break;
					#endregion
					#region p - pointer
					case 'p':   // pointer
						if (o is IntPtr)
#if PCL || ENABLE_DOTNET
							w = ( (IntPtr)o ).ToString();
#else
							w = "0x" + ((IntPtr)o).ToString("x");
#endif
						defaultParamIx++;
						break;
					#endregion
					#region n - number of processed chars so far
					case 'n':   // number of characters so far
						w = FormatNumber("d", flagAlternate,
										fieldLength, int.MinValue, flagLeft2Right,
										flagPositiveSign, flagPositiveSpace,
										paddingCharacter, m.Index);
						break;
					#endregion
					default:
						w = String.Empty;
						defaultParamIx++;
						break;
				}

				// replace format parameter with parameter value
				// and start searching for the next format parameter
				// AFTER the position of the current inserted value
				// to prohibit recursive matches if the value also
				// includes a format specifier
				f.Remove(m.Index, m.Length);
				f.Insert(m.Index, w);
				m = r.Match(f.ToString(), m.Index + w.Length);
			}

			return f.ToString();
		}
		#endregion
		#endregion

		#region Private Methods
		#region FormatOCT
		private static string FormatOct(string NativeFormat, bool Alternate,
											int FieldLength, int FieldPrecision,
											bool Left2Right,
											char Padding, object Value)
		{
			string w = String.Empty;
			string lengthFormat = "{0" + (FieldLength != int.MinValue ?
											"," + (Left2Right ?
													"-" :
													String.Empty) + FieldLength.ToString() :
											String.Empty) + "}";

			if (IsNumericType(Value))
			{
				w = Convert.ToString(UnboxToLong(Value, true), 8);

				if (Left2Right || Padding == ' ')
				{
					if (Alternate && w != "0")
						w = "0" + w;
					w = String.Format(lengthFormat, w);
				}
				else
				{
					if (FieldLength != int.MinValue)
						w = w.PadLeft(FieldLength - (Alternate && w != "0" ? 1 : 0), Padding);
					if (Alternate && w != "0")
						w = "0" + w;
				}
			}

			return w;
		}
		#endregion
		#region FormatHEX
		private static string FormatHex(string NativeFormat, bool Alternate,
											int FieldLength, int FieldPrecision,
											bool Left2Right,
											char Padding, object Value)
		{
			string w = String.Empty;
			string lengthFormat = "{0" + (FieldLength != int.MinValue ?
											"," + (Left2Right ?
													"-" :
													String.Empty) + FieldLength.ToString() :
											String.Empty) + "}";
			string numberFormat = "{0:" + NativeFormat + (FieldPrecision != int.MinValue ?
											FieldPrecision.ToString() :
											String.Empty) + "}";

			if (IsNumericType(Value))
			{
				w = String.Format(numberFormat, Value);

				if (Left2Right || Padding == ' ')
				{
					if (Alternate)
						w = (NativeFormat == "x" ? "0x" : "0X") + w;
					w = String.Format(lengthFormat, w);
				}
				else
				{
					if (FieldLength != int.MinValue)
						w = w.PadLeft(FieldLength - (Alternate ? 2 : 0), Padding);
					if (Alternate)
						w = (NativeFormat == "x" ? "0x" : "0X") + w;
				}
			}

			return w;
		}
		#endregion
		#region FormatNumber
		private static string FormatNumber(string NativeFormat, bool Alternate,
											int FieldLength, int FieldPrecision,
											bool Left2Right,
											bool PositiveSign, bool PositiveSpace,
											char Padding, object Value)
		{
			string w = String.Empty;
			string lengthFormat = "{0" + (FieldLength != int.MinValue ?
											"," + (Left2Right ?
													"-" :
													String.Empty) + FieldLength.ToString() :
											String.Empty) + "}";
			string numberFormat = "{0:" + NativeFormat + (FieldPrecision != int.MinValue ?
											FieldPrecision.ToString() :
											"0") + "}";

			if (IsNumericType(Value))
			{
				w = String.Format(CultureInfo.InvariantCulture, numberFormat, Value);

				if (Left2Right || Padding == ' ')
				{
					if (IsPositive(Value, true))
						w = (PositiveSign ?
								"+" : (PositiveSpace ? " " : String.Empty)) + w;
					w = String.Format(lengthFormat, w);
				}
				else
				{
					if (w.StartsWith("-"))
						w = w.Substring(1);
					if (FieldLength != int.MinValue)
						if (PositiveSign)  // xan - change here
							w = w.PadLeft(FieldLength - 1, Padding);
						else
							w = w.PadLeft(FieldLength, Padding);
					if (IsPositive(Value, true))
						w = (PositiveSign ?
								"+" : "") + w;  // xan - change here
					else
						w = "-" + w;
				}
			}

			return w;
		}
		#endregion
		#endregion
	}
}


