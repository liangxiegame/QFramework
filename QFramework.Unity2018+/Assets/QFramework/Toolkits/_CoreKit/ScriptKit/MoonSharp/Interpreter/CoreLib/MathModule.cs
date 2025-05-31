// Disable warnings about XML documentation
#pragma warning disable 1591

using System;
using MoonSharp.Interpreter.Interop;

namespace MoonSharp.Interpreter.CoreLib
{
	/// <summary>
	/// Class implementing math Lua functions 
	/// </summary>
	[MoonSharpModule(Namespace = "math")]
	public class MathModule
	{
		[MoonSharpModuleConstant]
		public const double pi = Math.PI;
		[MoonSharpModuleConstant]
		public const double huge = double.MaxValue;

		private static Random GetRandom(Script s)
		{
			DynValue rr = s.Registry.Get("F61E3AA7247D4D1EB7A45430B0C8C9BB_MATH_RANDOM");
			return (rr.UserData.Object as AnonWrapper<Random>).Value;
		}

		private static void SetRandom(Script s, Random random)
		{
			DynValue rr = UserData.Create(new AnonWrapper<Random>(random));
			s.Registry.Set("F61E3AA7247D4D1EB7A45430B0C8C9BB_MATH_RANDOM", rr);
		}


		public static void MoonSharpInit(Table globalTable, Table ioTable)
		{
			SetRandom(globalTable.OwnerScript, new Random());
		}



		private static DynValue exec1(CallbackArguments args, string funcName, Func<double, double> func)
		{
			DynValue arg = args.AsType(0, funcName, DataType.Number, false);
			return DynValue.NewNumber(func(arg.Number));
		}

		private static DynValue exec2(CallbackArguments args, string funcName, Func<double, double, double> func)
		{
			DynValue arg = args.AsType(0, funcName, DataType.Number, false);
			DynValue arg2 = args.AsType(1, funcName, DataType.Number, false);
			return DynValue.NewNumber(func(arg.Number, arg2.Number));
		}
		private static DynValue exec2n(CallbackArguments args, string funcName, double defVal, Func<double, double, double> func)
		{
			DynValue arg = args.AsType(0, funcName, DataType.Number, false);
			DynValue arg2 = args.AsType(1, funcName, DataType.Number, true);

			return DynValue.NewNumber(func(arg.Number, arg2.IsNil() ? defVal : arg2.Number));
		}
		private static DynValue execaccum(CallbackArguments args, string funcName, Func<double, double, double> func)
		{
			double accum = double.NaN;

			if (args.Count == 0)
			{
				throw new ScriptRuntimeException("bad argument #1 to '{0}' (number expected, got no value)", funcName);
			}

			for (int i = 0; i < args.Count; i++)
			{
				DynValue arg = args.AsType(i, funcName, DataType.Number, false);

				if (i == 0)
					accum = arg.Number;
				else
					accum = func(accum, arg.Number);
			}

			return DynValue.NewNumber(accum);
		}


		[MoonSharpModuleMethod]
		public static DynValue abs(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return exec1(args, "abs", d => Math.Abs(d));
		}

		[MoonSharpModuleMethod]
		public static DynValue acos(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return exec1(args, "acos", d => Math.Acos(d));
		}

		[MoonSharpModuleMethod]
		public static DynValue asin(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return exec1(args, "asin", d => Math.Asin(d));
		}

		[MoonSharpModuleMethod]
		public static DynValue atan(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return exec1(args, "atan", d => Math.Atan(d));
		}

		[MoonSharpModuleMethod]
		public static DynValue atan2(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return exec2(args, "atan2", (d1, d2) => Math.Atan2(d1, d2));
		}

		[MoonSharpModuleMethod]
		public static DynValue ceil(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return exec1(args, "ceil", d => Math.Ceiling(d));
		}

		[MoonSharpModuleMethod]
		public static DynValue cos(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return exec1(args, "cos", d => Math.Cos(d));
		}

		[MoonSharpModuleMethod]
		public static DynValue cosh(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return exec1(args, "cosh", d => Math.Cosh(d));
		}

		[MoonSharpModuleMethod]
		public static DynValue deg(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return exec1(args, "deg", d => d * 180.0 / Math.PI);
		}

		[MoonSharpModuleMethod]
		public static DynValue exp(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return exec1(args, "exp", d => Math.Exp(d));
		}

		[MoonSharpModuleMethod]
		public static DynValue floor(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return exec1(args, "floor", d => Math.Floor(d));
		}

		[MoonSharpModuleMethod]
		public static DynValue fmod(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return exec2(args, "fmod", (d1, d2) => Math.IEEERemainder(d1, d2));
		}

		[MoonSharpModuleMethod]
		public static DynValue frexp(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			// http://stackoverflow.com/questions/389993/extracting-mantissa-and-exponent-from-double-in-c-sharp

			DynValue arg = args.AsType(0, "frexp", DataType.Number, false);
			
			double d = arg.Number;

			// Translate the double into sign, exponent and mantissa.
			long bits = BitConverter.DoubleToInt64Bits(d);
			// Note that the shift is sign-extended, hence the test against -1 not 1
			bool negative = (bits < 0);
			int exponent = (int) ((bits >> 52) & 0x7ffL);
			long mantissa = bits & 0xfffffffffffffL;

			// Subnormal numbers; exponent is effectively one higher,
			// but there's no extra normalisation bit in the mantissa
			if (exponent==0)
			{
				exponent++;
			}
			// Normal numbers; leave exponent as it is but add extra
			// bit to the front of the mantissa
			else
			{
				mantissa = mantissa | (1L<<52);
			}

			// Bias the exponent. It's actually biased by 1023, but we're
			// treating the mantissa as m.0 rather than 0.m, so we need
			// to subtract another 52 from it.
			exponent -= 1075;

			if (mantissa == 0) 
			{
				return DynValue.NewTuple(DynValue.NewNumber(0), DynValue.NewNumber(0));
			}

			/* Normalize */
			while((mantissa & 1) == 0) 
			{    /*  i.e., Mantissa is even */
				mantissa >>= 1;
				exponent++;
			}

			double m = (double)mantissa;
			double e = (double)exponent;
			while( m >= 1 )
			{
				m /= 2.0;
				e += 1.0;
			}

			if( negative ) m = -m;

			return DynValue.NewTuple(DynValue.NewNumber(m), DynValue.NewNumber(e));
		}

		[MoonSharpModuleMethod]
		public static DynValue ldexp(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return exec2(args, "ldexp", (d1, d2) => d1 * Math.Pow(2, d2));
		}

		[MoonSharpModuleMethod]
		public static DynValue log(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return exec2n(args, "log", Math.E, (d1, d2) => Math.Log(d1, d2));
		}

		[MoonSharpModuleMethod]
		public static DynValue max(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return execaccum(args, "max", (d1, d2) => Math.Max(d1, d2));
		}

		[MoonSharpModuleMethod]
		public static DynValue min(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return execaccum(args, "min", (d1, d2) => Math.Min(d1, d2));
		}

		[MoonSharpModuleMethod]
		public static DynValue modf(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue arg = args.AsType(0, "modf", DataType.Number, false);
			return DynValue.NewTuple(DynValue.NewNumber(Math.Floor(arg.Number)), DynValue.NewNumber(arg.Number - Math.Floor(arg.Number)));
		}


		[MoonSharpModuleMethod]
		public static DynValue pow(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return exec2(args, "pow", (d1, d2) => Math.Pow(d1, d2));
		}

		[MoonSharpModuleMethod]
		public static DynValue rad(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return exec1(args, "rad", d => d * Math.PI / 180.0);
		}

		[MoonSharpModuleMethod]
		public static DynValue random(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue m = args.AsType(0, "random", DataType.Number, true);
			DynValue n = args.AsType(1, "random", DataType.Number, true);
			Random R = GetRandom(executionContext.GetScript());
			double d;

			if (m.IsNil() && n.IsNil())
			{
				d = R.NextDouble();
			}
			else
			{
				int a = n.IsNil() ? 1 : (int)n.Number;
				int b = (int)m.Number;

				if (a < b)
					d = R.Next(a, b + 1);
				else
					d = R.Next(b, a + 1);
			}

			return DynValue.NewNumber(d);
		}

		[MoonSharpModuleMethod]
		public static DynValue randomseed(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue arg = args.AsType(0, "randomseed", DataType.Number, false);
			var script = executionContext.GetScript();
			SetRandom(script, new Random((int)arg.Number));
			return DynValue.Nil;
		}

		[MoonSharpModuleMethod]
		public static DynValue sin(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return exec1(args, "sin", d => Math.Sin(d));
		}

		[MoonSharpModuleMethod]
		public static DynValue sinh(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return exec1(args, "sinh", d => Math.Sinh(d));
		}

		[MoonSharpModuleMethod]
		public static DynValue sqrt(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return exec1(args, "sqrt", d => Math.Sqrt(d));
		}

		[MoonSharpModuleMethod]
		public static DynValue tan(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return exec1(args, "tan", d => Math.Tan(d));
		}

		[MoonSharpModuleMethod]
		public static DynValue tanh(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return exec1(args, "tanh", d => Math.Tanh(d));
		}


	}
}
