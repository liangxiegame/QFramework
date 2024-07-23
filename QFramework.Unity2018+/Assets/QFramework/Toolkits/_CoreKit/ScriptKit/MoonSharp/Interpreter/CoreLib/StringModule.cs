// Disable warnings about XML documentation
#pragma warning disable 1591

using System;
using System.IO;
using System.Text;
using MoonSharp.Interpreter.CoreLib.StringLib;

namespace MoonSharp.Interpreter.CoreLib
{
	/// <summary>
	/// Class implementing string Lua functions 
	/// </summary>
	[MoonSharpModule(Namespace = "string")]
	public class StringModule
	{
		public const string BASE64_DUMP_HEADER = "MoonSharp_dump_b64::";

		public static void MoonSharpInit(Table globalTable, Table stringTable)
		{
			Table stringMetatable = new Table(globalTable.OwnerScript);
			stringMetatable.Set("__index", DynValue.NewTable(stringTable));
			globalTable.OwnerScript.SetTypeMetatable(DataType.String, stringMetatable);
		}


		[MoonSharpModuleMethod]
		public static DynValue dump(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue fn = args.AsType(0, "dump", DataType.Function, false);

			try
			{
				byte[] bytes;
				using (MemoryStream ms = new MemoryStream())
				{
					executionContext.GetScript().Dump(fn, ms);
					ms.Seek(0, SeekOrigin.Begin);
					bytes = ms.ToArray();
				}
				string base64 = Convert.ToBase64String(bytes);
				return DynValue.NewString(BASE64_DUMP_HEADER + base64);
			}
			catch (Exception ex)
			{
				throw new ScriptRuntimeException(ex.Message);
			}
		}


		[MoonSharpModuleMethod]
		public static DynValue @char(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			StringBuilder sb = new StringBuilder(args.Count);

			for (int i = 0; i < args.Count; i++)
			{
				DynValue v = args[i];
				double d = 0d;

				if (v.Type == DataType.String)
				{
					double? nd = v.CastToNumber();
					if (nd == null)
						args.AsType(i, "char", DataType.Number, false);
					else
						d = nd.Value;
				}
				else
				{
					args.AsType(i, "char", DataType.Number, false);
					d = v.Number;
				}

				sb.Append((char)(d));
			}

			return DynValue.NewString(sb.ToString());
		}


		[MoonSharpModuleMethod]
		public static DynValue @byte(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue vs = args.AsType(0, "byte", DataType.String, false);
			DynValue vi = args.AsType(1, "byte", DataType.Number, true);
			DynValue vj = args.AsType(2, "byte", DataType.Number, true);

			return PerformByteLike(vs, vi, vj,
				i => Unicode2Ascii(i));
		}

		[MoonSharpModuleMethod]
		public static DynValue unicode(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue vs = args.AsType(0, "unicode", DataType.String, false);
			DynValue vi = args.AsType(1, "unicode", DataType.Number, true);
			DynValue vj = args.AsType(2, "unicode", DataType.Number, true);

			return PerformByteLike(vs, vi, vj, i => i);
		}

		private static int Unicode2Ascii(int i)
		{
			if (i >= 0 && i < 255)
				return i;

			return (int)'?';
		}

		private static DynValue PerformByteLike(DynValue vs, DynValue vi, DynValue vj, Func<int, int> filter)
		{
            StringRange range = StringRange.FromLuaRange(vi, vj, null);
            string s = range.ApplyToString(vs.String);

            int length = s.Length;
			DynValue[] rets = new DynValue[length];

            for (int i = 0; i < length; ++i)
            {
                rets[i] = DynValue.NewNumber(filter((int)s[i]));
            }

			return DynValue.NewTuple(rets);
		}


		private static int? AdjustIndex(string s, DynValue vi, int defval)
		{
			if (vi.IsNil())
				return defval;

			int i = (int)Math.Round(vi.Number, 0);

			if (i == 0)
				return null;

			if (i > 0)
				return i - 1;

			return s.Length - i;
		}

		[MoonSharpModuleMethod]
		public static DynValue len(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue vs = args.AsType(0, "len", DataType.String, false);
			return DynValue.NewNumber(vs.String.Length);
		}



		[MoonSharpModuleMethod]
		public static DynValue match(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return executionContext.EmulateClassicCall(args, "match", KopiLua_StringLib.str_match);
		}


		[MoonSharpModuleMethod]
		public static DynValue gmatch(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return executionContext.EmulateClassicCall(args, "gmatch", KopiLua_StringLib.str_gmatch);
		}

		[MoonSharpModuleMethod]
		public static DynValue gsub(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return executionContext.EmulateClassicCall(args, "gsub", KopiLua_StringLib.str_gsub);
		}

		[MoonSharpModuleMethod]
		public static DynValue find(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return executionContext.EmulateClassicCall(args, "find",
				KopiLua_StringLib.str_find);
		}


        [MoonSharpModuleMethod]
        public static DynValue lower(ScriptExecutionContext executionContext, CallbackArguments args)
        {
            DynValue arg_s = args.AsType(0, "lower", DataType.String, false);
            return DynValue.NewString(arg_s.String.ToLower());
        }

        [MoonSharpModuleMethod]
        public static DynValue upper(ScriptExecutionContext executionContext, CallbackArguments args)
        {
            DynValue arg_s = args.AsType(0, "upper", DataType.String, false);
            return DynValue.NewString(arg_s.String.ToUpper());
        }

        [MoonSharpModuleMethod]
        public static DynValue rep(ScriptExecutionContext executionContext, CallbackArguments args)
        {
            DynValue arg_s = args.AsType(0, "rep", DataType.String, false);
            DynValue arg_n = args.AsType(1, "rep", DataType.Number, false);
			DynValue arg_sep = args.AsType(2, "rep", DataType.String, true);

            if (String.IsNullOrEmpty(arg_s.String) || (arg_n.Number < 1))
            {
                return DynValue.NewString("");
            }

			string sep = (arg_sep.IsNotNil()) ? arg_sep.String : null;

            int count = (int)arg_n.Number;
            StringBuilder result = new StringBuilder(arg_s.String.Length * count);

            for (int i = 0; i < count; ++i)
            {
				if (i != 0 && sep != null)
					result.Append(sep);

                result.Append(arg_s.String);
            }

            return DynValue.NewString(result.ToString());
        }

		[MoonSharpModuleMethod]
		public static DynValue format(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return executionContext.EmulateClassicCall(args, "format", KopiLua_StringLib.str_format);
		}



        [MoonSharpModuleMethod]
        public static DynValue reverse(ScriptExecutionContext executionContext, CallbackArguments args)
        {
            DynValue arg_s = args.AsType(0, "reverse", DataType.String, false);

            if (String.IsNullOrEmpty(arg_s.String))
            {
                return DynValue.NewString("");
            }

            char[] elements = arg_s.String.ToCharArray();
            Array.Reverse(elements);

            return DynValue.NewString(new String(elements));
        }

        [MoonSharpModuleMethod]
        public static DynValue sub(ScriptExecutionContext executionContext, CallbackArguments args)
        {
            DynValue arg_s = args.AsType(0, "sub", DataType.String, false);
			DynValue arg_i = args.AsType(1, "sub", DataType.Number, true);
            DynValue arg_j = args.AsType(2, "sub", DataType.Number, true);

			StringRange range = StringRange.FromLuaRange(arg_i, arg_j, -1);
            string s = range.ApplyToString(arg_s.String);

            return DynValue.NewString(s);
        }

		[MoonSharpModuleMethod]
		public static DynValue startsWith(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue arg_s1 = args.AsType(0, "startsWith", DataType.String, true);
			DynValue arg_s2 = args.AsType(1, "startsWith", DataType.String, true);

			if (arg_s1.IsNil() || arg_s2.IsNil())
				return DynValue.False;

			return DynValue.NewBoolean(arg_s1.String.StartsWith(arg_s2.String));
		}

		[MoonSharpModuleMethod]
		public static DynValue endsWith(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue arg_s1 = args.AsType(0, "endsWith", DataType.String, true);
			DynValue arg_s2 = args.AsType(1, "endsWith", DataType.String, true);

			if (arg_s1.IsNil() || arg_s2.IsNil())
				return DynValue.False;

			return DynValue.NewBoolean(arg_s1.String.EndsWith(arg_s2.String));
		}

		[MoonSharpModuleMethod]
		public static DynValue contains(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue arg_s1 = args.AsType(0, "contains", DataType.String, true);
			DynValue arg_s2 = args.AsType(1, "contains", DataType.String, true);

			if (arg_s1.IsNil() || arg_s2.IsNil())
				return DynValue.False;

			return DynValue.NewBoolean(arg_s1.String.Contains(arg_s2.String));
		}

	}


}
