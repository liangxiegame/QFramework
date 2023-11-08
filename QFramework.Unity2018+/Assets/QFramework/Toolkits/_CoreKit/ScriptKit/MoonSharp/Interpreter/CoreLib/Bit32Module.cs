// Disable warnings about XML documentation
#pragma warning disable 1591

using System;

namespace MoonSharp.Interpreter.CoreLib
{
	/// <summary>
	/// Class implementing bit32 Lua functions 
	/// </summary>
	[MoonSharpModule(Namespace = "bit32")]
	public class Bit32Module
	{
		static readonly uint[] MASKS = new uint[] { 
									 0x1, 0x3, 0x7, 0xF,
									 0x1F, 0x3F, 0x7F, 0xFF,
									 0x1FF, 0x3FF, 0x7FF, 0xFFF,
									 0x1FFF, 0x3FFF, 0x7FFF, 0xFFFF,
									 0x1FFFF, 0x3FFFF, 0x7FFFF, 0xFFFFF,
									 0x1FFFFF, 0x3FFFFF, 0x7FFFFF, 0xFFFFFF,
									 0x1FFFFFF, 0x3FFFFFF, 0x7FFFFFF, 0xFFFFFFF,
									 0x1FFFFFFF, 0x3FFFFFFF, 0x7FFFFFFF, 0xFFFFFFFF, };

		static uint ToUInt32(DynValue v)
		{
			double d = v.Number;
			d = Math.IEEERemainder(d, Math.Pow(2.0, 32.0));
			return (uint)d;
		}

		static int ToInt32(DynValue v)
		{
			double d = v.Number;
			d = Math.IEEERemainder(d, Math.Pow(2.0, 32.0));
			return (int)d;
		}

		static uint NBitMask(int bits)
		{

			if (bits <= 0)
				return 0;
			if (bits >= 32)
				return MASKS[31];

			return MASKS[bits - 1];
		}

		public static uint Bitwise(string funcName, CallbackArguments args, Func<uint, uint, uint> accumFunc)
		{
			uint accum = ToUInt32(args.AsType(0, funcName, DataType.Number, false));

			for (int i = 1; i < args.Count; i++)
			{
				uint vv = ToUInt32(args.AsType(i, funcName, DataType.Number, false));
				accum = accumFunc(accum, vv);
			}

			return accum;
		}


		[MoonSharpModuleMethod]
		public static DynValue extract(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue v_v = args.AsType(0, "extract", DataType.Number);
			uint v = ToUInt32(v_v);

			DynValue v_pos = args.AsType(1, "extract", DataType.Number);
			DynValue v_width = args.AsType(2, "extract", DataType.Number, true);

			int pos = (int)v_pos.Number;
			int width = (v_width).IsNilOrNan() ? 1 : (int)v_width.Number;

			ValidatePosWidth("extract", 2, pos, width);

			uint res = (v >> pos) & NBitMask(width);
			return DynValue.NewNumber(res);
		}


		[MoonSharpModuleMethod]
		public static DynValue replace(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue v_v = args.AsType(0, "replace", DataType.Number);
			uint v = ToUInt32(v_v);

			DynValue v_u = args.AsType(1, "replace", DataType.Number);
			uint u = ToUInt32(v_u);
			DynValue v_pos = args.AsType(2, "replace", DataType.Number);
			DynValue v_width = args.AsType(3, "replace", DataType.Number, true);

			int pos = (int)v_pos.Number;
			int width = (v_width).IsNilOrNan() ? 1 : (int)v_width.Number;

			ValidatePosWidth("replace", 3, pos, width);

			uint mask = NBitMask(width) << pos;
			v = v & (~mask);
			u = u & (mask);
			v = v | u;

			return DynValue.NewNumber(v);
		}


		private static void ValidatePosWidth(string func, int argPos, int pos, int width)
		{
			if (pos > 31 || (pos + width) > 31)
				throw new ScriptRuntimeException("trying to access non-existent bits");

			if (pos < 0)
				throw new ScriptRuntimeException("bad argument #{1} to '{0}' (field cannot be negative)", func, argPos);

			if (width <= 0)
				throw new ScriptRuntimeException("bad argument #{1} to '{0}' (width must be positive)", func, argPos+1);
		}


		[MoonSharpModuleMethod]
		public static DynValue arshift(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue v_v = args.AsType(0, "arshift", DataType.Number);
			int v = ToInt32(v_v);

			DynValue v_a = args.AsType(1, "arshift", DataType.Number);

			int a = (int)v_a.Number;

			if (a < 0)
				v = v << -a;
			else
				v = v >> a;

			return DynValue.NewNumber(v);
		}


		[MoonSharpModuleMethod]
		public static DynValue rshift(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue v_v = args.AsType(0, "rshift", DataType.Number);
			uint v = ToUInt32(v_v);

			DynValue v_a = args.AsType(1, "rshift", DataType.Number);

			int a = (int)v_a.Number;

			if (a < 0)
				v = v << -a;
			else
				v = v >> a;

			return DynValue.NewNumber(v);
		}


		[MoonSharpModuleMethod]
		public static DynValue lshift(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue v_v = args.AsType(0, "lshift", DataType.Number);
			uint v = ToUInt32(v_v);

			DynValue v_a = args.AsType(1, "lshift", DataType.Number);

			int a = (int)v_a.Number;

			if (a < 0)
				v = v >> -a;
			else
				v = v << a;

			return DynValue.NewNumber(v);
		}

		[MoonSharpModuleMethod]
		public static DynValue band(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return DynValue.NewNumber(Bitwise("band", args, (x, y) => x & y));
		}

		[MoonSharpModuleMethod]
		public static DynValue btest(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return DynValue.NewBoolean(0 != Bitwise("btest", args, (x, y) => x & y));
		}

		[MoonSharpModuleMethod]
		public static DynValue bor(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return DynValue.NewNumber(Bitwise("bor", args, (x, y) => x | y));
		}

		[MoonSharpModuleMethod]
		public static DynValue bnot(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue v_v = args.AsType(0, "bnot", DataType.Number);
			uint v = ToUInt32(v_v);
			return DynValue.NewNumber(~v);
		}

		[MoonSharpModuleMethod]
		public static DynValue bxor(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return DynValue.NewNumber(Bitwise("bxor", args, (x, y) => x ^ y));
		}

		[MoonSharpModuleMethod]
		public static DynValue lrotate(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue v_v = args.AsType(0, "lrotate", DataType.Number);
			uint v = ToUInt32(v_v);

			DynValue v_a = args.AsType(1, "lrotate", DataType.Number);

			int a = ((int)v_a.Number) % 32;

			if (a < 0)
				v = (v >> (-a)) | (v << (32 + a));
			else
				v = (v << a) | (v >> (32 - a));

			return DynValue.NewNumber(v);
		}

		[MoonSharpModuleMethod]
		public static DynValue rrotate(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue v_v = args.AsType(0, "rrotate", DataType.Number);
			uint v = ToUInt32(v_v);

			DynValue v_a = args.AsType(1, "rrotate", DataType.Number);

			int a = ((int)v_a.Number) % 32;

			if (a < 0)
				v = (v << (-a)) | (v >> (32 + a));
			else
				v = (v >> a) | (v << (32 - a));

			return DynValue.NewNumber(v);
		}

	}
}
