// Disable warnings about XML documentation
#pragma warning disable 1591

using System;
using lua_Integer = System.Int32;

namespace MoonSharp.Interpreter.Interop.LuaStateInterop
{
	/// <summary>
	/// Classes using the classic interface should inherit from this class.
	/// This class defines only static methods and is really meant to be used only
	/// from C# and not other .NET languages. 
	/// 
	/// For easier operation they should also define:
	///		using ptrdiff_t = System.Int32;
	///		using lua_Integer = System.Int32;
	///		using LUA_INTFRM_T = System.Int64;
	///		using UNSIGNED_LUA_INTFRM_T = System.UInt64;
	/// </summary>
	public partial class LuaBase
	{
		protected const int LUA_TNONE = -1;
		protected const int LUA_TNIL = 0;
		protected const int LUA_TBOOLEAN = 1;
		protected const int LUA_TLIGHTUSERDATA = 2;
		protected const int LUA_TNUMBER = 3;
		protected const int LUA_TSTRING = 4;
		protected const int LUA_TTABLE = 5;
		protected const int LUA_TFUNCTION = 6;
		protected const int LUA_TUSERDATA = 7;
		protected const int LUA_TTHREAD = 8;
		
		protected const int LUA_MULTRET = -1;

		protected const string LUA_INTFRMLEN = "l";

		protected static DynValue GetArgument(LuaState L, lua_Integer pos)
		{
			return L.At(pos);
		}

		protected static DynValue ArgAsType(LuaState L, lua_Integer pos, DataType type, bool allowNil = false)
		{
			return GetArgument(L, pos).CheckType(L.FunctionName, type, pos - 1, allowNil ? TypeValidationFlags.AllowNil | TypeValidationFlags.AutoConvert : TypeValidationFlags.AutoConvert);
		}

		protected static lua_Integer LuaType(LuaState L, lua_Integer p)
		{
			switch (GetArgument(L, p).Type)
			{
				case DataType.Void:
					return LUA_TNONE;
				case DataType.Nil:
					return LUA_TNIL;
				case DataType.Boolean:
					return LUA_TNIL;
				case DataType.Number:
					return LUA_TNUMBER;
				case DataType.String:
					return LUA_TSTRING;
				case DataType.Function:
					return LUA_TFUNCTION;
				case DataType.Table:
					return LUA_TTABLE;
				case DataType.UserData:
					return LUA_TUSERDATA;
				case DataType.Thread:
					return LUA_TTHREAD;
				case DataType.ClrFunction:
					return LUA_TFUNCTION;
				case DataType.TailCallRequest:
				case DataType.YieldRequest:
				case DataType.Tuple:
				default:
					throw new ScriptRuntimeException("Can't call LuaType on any type");
			}
		}

		protected static string LuaLCheckLString(LuaState L, lua_Integer argNum, out uint l)
		{
			string str = ArgAsType(L, argNum, DataType.String, false).String;
			l = (uint)str.Length;
			return str;
		}

		protected static void LuaPushInteger(LuaState L, lua_Integer val)
		{
			L.Push(DynValue.NewNumber(val));
		}

		protected static lua_Integer LuaToBoolean(LuaState L, lua_Integer p)
		{
			return GetArgument(L, p).CastToBool() ? 1 : 0;
		}

		protected static string LuaToLString(LuaState luaState, lua_Integer p, out uint l)
		{
			return LuaLCheckLString(luaState, p, out l);
		}

		protected static string LuaToString(LuaState luaState, lua_Integer p)
		{
			uint l;
			return LuaLCheckLString(luaState, p, out l);
		}

		protected static void LuaLAddValue(LuaLBuffer b)
		{
			b.StringBuilder.Append(b.LuaState.Pop().ToPrintString());
		}

		protected static void LuaLAddLString(LuaLBuffer b, CharPtr s, uint p)
		{
			b.StringBuilder.Append(s.ToString((int)p));
		}

		protected static void LuaLAddString(LuaLBuffer b, string s)
		{
			b.StringBuilder.Append(s.ToString());
		}


		protected static lua_Integer LuaLOptInteger(LuaState L, lua_Integer pos, lua_Integer def)
		{
			DynValue v = ArgAsType(L, pos, DataType.Number, true);

			if (v.IsNil())
				return def;
			else
				return (int)v.Number;
		}

		protected static lua_Integer LuaLCheckInteger(LuaState L, lua_Integer pos)
		{
			DynValue v = ArgAsType(L, pos, DataType.Number, false);
			return (int)v.Number;
		}

		protected static void LuaLArgCheck(LuaState L, bool condition, lua_Integer argNum, string message)
		{
			if (!condition)
				LuaLArgError(L, argNum, message);
		}

		protected static lua_Integer LuaLCheckInt(LuaState L, lua_Integer argNum)
		{
			return LuaLCheckInteger(L, argNum);
		}

		protected static lua_Integer LuaGetTop(LuaState L)
		{
			return L.Count;
		}

		protected static lua_Integer LuaLError(LuaState luaState, string message, params object[] args)
		{
			throw new ScriptRuntimeException(message, args);
		}

		protected static void LuaLAddChar(LuaLBuffer b, char p)
		{
			b.StringBuilder.Append(p);
		}

		protected static void LuaLBuffInit(LuaState L, LuaLBuffer b)
		{
		}

		protected static void LuaPushLiteral(LuaState L, string literalString)
		{
			L.Push(DynValue.NewString(literalString));
		}

		protected static void LuaLPushResult(LuaLBuffer b)
		{
			LuaPushLiteral(b.LuaState, b.StringBuilder.ToString());
		}

		protected static void LuaPushLString(LuaState L, CharPtr s, uint len)
		{
			string ss = s.ToString((int)len);
			L.Push(DynValue.NewString(ss));
		}

		protected static void LuaLCheckStack(LuaState L, lua_Integer n, string message)
		{
			// nop ?
		}

		protected static string LUA_QL(string p)
		{
			return "'" + p + "'";
		}


		protected static void LuaPushNil(LuaState L)
		{
			L.Push(DynValue.Nil);
		}

		protected static void LuaAssert(bool p)
		{
			// ??! 
			// A lot of KopiLua methods fall here in valid state!

			//if (!p)
			//	throw new InternalErrorException("LuaAssert failed!");
		}

		protected static string LuaLTypeName(LuaState L, lua_Integer p)
		{
			return L.At(p).Type.ToErrorTypeString();
		}

		protected static lua_Integer LuaIsString(LuaState L, lua_Integer p)
		{
			var v = L.At(p);
			return (v.Type == DataType.String || v.Type == DataType.Number) ? 1 : 0;
		}

		protected static void LuaPop(LuaState L, lua_Integer p)
		{
			for (int i = 0; i < p; i++)
				L.Pop();
		}

		protected static void LuaGetTable(LuaState L, lua_Integer p)
		{
			// DEBT: this should call metamethods, now it performs raw access
			DynValue key = L.Pop();
			DynValue table = L.At(p);

			if (table.Type != DataType.Table)
				throw new NotImplementedException();

			var v = table.Table.Get(key);
			L.Push(v);
		}

		protected static int LuaLOptInt(LuaState L, lua_Integer pos, lua_Integer def)
		{
			return LuaLOptInteger(L, pos, def);
		}

		protected static CharPtr LuaLCheckString(LuaState L, lua_Integer p)
		{
			uint dummy;
			return LuaLCheckLString(L, p, out dummy);
		}

		protected static string LuaLCheckStringStr(LuaState L, lua_Integer p)
		{
			uint dummy;
			return LuaLCheckLString(L, p, out dummy);
		}

		protected static void LuaLArgError(LuaState L, lua_Integer arg, string p)
		{
			throw ScriptRuntimeException.BadArgument(arg - 1, L.FunctionName, p);
		}

		protected static double LuaLCheckNumber(LuaState L, lua_Integer pos)
		{
			DynValue v = ArgAsType(L, pos, DataType.Number, false);
			return v.Number;
		}

		protected static void LuaPushValue(LuaState L, lua_Integer arg)
		{
			DynValue v = L.At(arg);
			L.Push(v);
		}


		/// <summary>
		/// Calls a function.
		/// To call a function you must use the following protocol: first, the function to be called is pushed onto the stack; then,
		/// the arguments to the function are pushed in direct order; that is, the first argument is pushed first. Finally you call
		/// lua_call; nargs is the number of arguments that you pushed onto the stack. All arguments and the function value are
		/// popped from the stack when the function is called. The function results are pushed onto the stack when the function
		/// returns. The number of results is adjusted to nresults, unless nresults is LUA_MULTRET. In this case, all results from
		/// the function are pushed. Lua takes care that the returned values fit into the stack space. The function results are
		/// pushed onto the stack in direct order (the first result is pushed first), so that after the call the last result is on
		/// the top of the stack.
		/// </summary>
		/// <param name="L">The LuaState</param>
		/// <param name="nargs">The number of arguments.</param>
		/// <param name="nresults">The number of expected results.</param>
		/// <exception cref="System.NotImplementedException"></exception>
		protected static void LuaCall(LuaState L, lua_Integer nargs, lua_Integer nresults = LUA_MULTRET)
		{
			DynValue[] args = L.GetTopArray(nargs);

			L.Discard(nargs);

			DynValue func = L.Pop();

			DynValue ret = L.ExecutionContext.Call(func, args);

			if (nresults != 0)
			{
				if (nresults == -1)
				{
					nresults = (ret.Type == DataType.Tuple) ? ret.Tuple.Length : 1;
				}

				DynValue[] vals = (ret.Type == DataType.Tuple) ? ret.Tuple : new DynValue[1] { ret };

				int copied = 0;

				for (int i = 0; i < vals.Length && copied < nresults; i++, copied++)
				{
					L.Push(vals[i]);
				}

				while (copied < nresults)
				{
					L.Push(DynValue.Nil);
				}
			}
		}
	}
}
