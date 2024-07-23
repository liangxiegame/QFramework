using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoonSharp.Interpreter.Serialization
{
	/// <summary>
	/// 
	/// </summary>
	public static class SerializationExtensions
	{
		static HashSet<string> LUAKEYWORDS = new HashSet<string>()
		{
			"and", "break", "do", "else", "elseif", "end", "false", "for", "function", "goto", "if", "in", "local", "nil", "not", "or", "repeat", "return", "then", "true", "until", "while"
		};


		public static string Serialize(this Table table, bool prefixReturn = false, int tabs = 0)
		{
			if (table.OwnerScript != null)
				throw new ScriptRuntimeException("Table is not a prime table.");

			string tabstr = new string('\t', tabs);
			StringBuilder sb = new StringBuilder();

			//sb.Append(tabstr);

			if (prefixReturn)
				sb.Append("return ");

			if (!table.Values.Any())
			{
				sb.Append("${ }");
				return sb.ToString();
			}

			sb.AppendLine("${");

			foreach (TablePair tp in table.Pairs)
			{
				sb.Append(tabstr);

				string key = 
					IsStringIdentifierValid(tp.Key) ? 
					tp.Key.String : "[" + SerializeValue(tp.Key, tabs + 1) +"]";

				sb.AppendFormat("\t{0} = {1},\n",
					key, SerializeValue(tp.Value, tabs + 1));
			}

			sb.Append(tabstr);
			sb.Append("}");

			if (tabs == 0)
				sb.AppendLine();

			return sb.ToString();
		}

		private static bool IsStringIdentifierValid(DynValue dynValue)
		{
			if (dynValue.Type != DataType.String)
				return false;

			if (dynValue.String.Length == 0)
				return false;

			if (LUAKEYWORDS.Contains(dynValue.String))
				return false;

			if (!char.IsLetter(dynValue.String[0]) && (dynValue.String[0] != '_'))
				return false;

			foreach (char c in dynValue.String)
			{
				if (!char.IsLetterOrDigit(c) && c != '_')
					return false;
			}

			return true;
		}

		public static string SerializeValue(this DynValue dynValue, int tabs = 0)
		{
			if (dynValue.Type == DataType.Nil || dynValue.Type == DataType.Void)
				return "nil";
			else if (dynValue.Type == DataType.Tuple)
				return (dynValue.Tuple.Any() ? SerializeValue(dynValue.Tuple[0], tabs) : "nil");
			else if (dynValue.Type == DataType.Number)
				return dynValue.Number.ToString("r");
			else if (dynValue.Type == DataType.Boolean)
				return dynValue.Boolean ? "true" : "false";
			else if (dynValue.Type == DataType.String)
				return EscapeString(dynValue.String ?? "");
			else if (dynValue.Type == DataType.Table && dynValue.Table.OwnerScript == null)
				return Serialize(dynValue.Table, false, tabs);
			else
				throw new ScriptRuntimeException("Value is not a primitive value or a prime table.");
		}

		private static string EscapeString(string s)
		{
			s = s.Replace(@"\", @"\\");
			s = s.Replace("\n", @"\n");
			s = s.Replace("\r", @"\r");
			s = s.Replace("\t", @"\t");
			s = s.Replace("\a", @"\a");
			s = s.Replace("\f", @"\f");
			s = s.Replace("\b", @"\b");
			s = s.Replace("\v", @"\v");
			s = s.Replace("\"", "\\\"");
			s = s.Replace("\'", @"\'");
			return "\"" + s + "\"";
		}

	}
}
