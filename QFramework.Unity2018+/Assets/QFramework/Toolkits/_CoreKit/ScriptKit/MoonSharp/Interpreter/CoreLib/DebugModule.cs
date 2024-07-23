// Disable warnings about XML documentation
#pragma warning disable 1591

using System;
using System.Text;
using MoonSharp.Interpreter.Debugging;
using MoonSharp.Interpreter.REPL;

namespace MoonSharp.Interpreter.CoreLib
{
	/// <summary>
	/// Class implementing debug Lua functions. Support for the debug module is partial. 
	/// </summary>
	[MoonSharpModule(Namespace = "debug")]
	public class DebugModule
	{
		[MoonSharpModuleMethod]
		public static DynValue debug(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			Script script = executionContext.GetScript();

			if (script.Options.DebugInput == null)
				throw new ScriptRuntimeException("debug.debug not supported on this platform/configuration");

			ReplInterpreter interpreter = new ReplInterpreter(script)
				{
					HandleDynamicExprs = false,
					HandleClassicExprsSyntax = true
				};

			while (true)
			{
				string s = script.Options.DebugInput(interpreter.ClassicPrompt + " ");

				try
				{
					DynValue result = interpreter.Evaluate(s);

					if (result != null && result.Type != DataType.Void)
						script.Options.DebugPrint(string.Format("{0}", result));
				}
				catch (InterpreterException ex)
				{
					script.Options.DebugPrint(string.Format("{0}", ex.DecoratedMessage ?? ex.Message));
				}
				catch (Exception ex)
				{
					script.Options.DebugPrint(string.Format("{0}", ex.Message));
				}
			}
		}

		[MoonSharpModuleMethod]
		public static DynValue getuservalue(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue v = args[0];

			if (v.Type != DataType.UserData)
				return DynValue.Nil;

			return v.UserData.UserValue ?? DynValue.Nil;
		}

		[MoonSharpModuleMethod]
		public static DynValue setuservalue(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue v = args.AsType(0, "setuservalue", DataType.UserData, false);
			DynValue t = args.AsType(0, "setuservalue", DataType.Table, true);

			return v.UserData.UserValue = t;
		}

		[MoonSharpModuleMethod]
		public static DynValue getregistry(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return DynValue.NewTable(executionContext.GetScript().Registry);
		}

		[MoonSharpModuleMethod]
		public static DynValue getmetatable(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue v = args[0];
			Script S = executionContext.GetScript();

			if (v.Type.CanHaveTypeMetatables())
				return DynValue.NewTable(S.GetTypeMetatable(v.Type));
			else if (v.Type == DataType.Table)
				return DynValue.NewTable(v.Table.MetaTable);
			else
				return DynValue.Nil;
		}

		[MoonSharpModuleMethod]
		public static DynValue setmetatable(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue v = args[0];
			DynValue t = args.AsType(1, "setmetatable", DataType.Table, true);
			Table m = (t.IsNil()) ? null : t.Table;
			Script S = executionContext.GetScript();

			if (v.Type.CanHaveTypeMetatables())
				S.SetTypeMetatable(v.Type, m);
			else if (v.Type == DataType.Table)
				v.Table.MetaTable = m;
			else
				throw new ScriptRuntimeException("cannot debug.setmetatable on type {0}", v.Type.ToErrorTypeString());

			return v;
		}

		[MoonSharpModuleMethod]
		public static DynValue getupvalue(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			var index = (int)args.AsType(1, "getupvalue", DataType.Number, false).Number - 1;

			if (args[0].Type == DataType.ClrFunction)
				return DynValue.Nil;

			var fn = args.AsType(0, "getupvalue", DataType.Function, false).Function;

			var closure = fn.ClosureContext;

			if (index < 0 || index >= closure.Count)
				return DynValue.Nil;

			return DynValue.NewTuple(
				DynValue.NewString(closure.Symbols[index]),
				closure[index]);
		}


		[MoonSharpModuleMethod]
		public static DynValue upvalueid(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			var index = (int)args.AsType(1, "getupvalue", DataType.Number, false).Number - 1;

			if (args[0].Type == DataType.ClrFunction)
				return DynValue.Nil;

			var fn = args.AsType(0, "getupvalue", DataType.Function, false).Function;

			var closure = fn.ClosureContext;

			if (index < 0 || index >= closure.Count)
				return DynValue.Nil;

			return DynValue.NewNumber(closure[index].ReferenceID);
		}


		[MoonSharpModuleMethod]
		public static DynValue setupvalue(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			var index = (int)args.AsType(1, "setupvalue", DataType.Number, false).Number - 1;

			if (args[0].Type == DataType.ClrFunction)
				return DynValue.Nil;

			var fn = args.AsType(0, "setupvalue", DataType.Function, false).Function;

			var closure = fn.ClosureContext;

			if (index < 0 || index >= closure.Count)
				return DynValue.Nil;

			closure[index].Assign(args[2]);

			return DynValue.NewString(closure.Symbols[index]);
		}


		[MoonSharpModuleMethod]
		public static DynValue upvaluejoin(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue f1 = args.AsType(0, "upvaluejoin", DataType.Function, false);
			DynValue f2 = args.AsType(2, "upvaluejoin", DataType.Function, false);
			int n1 = args.AsInt(1, "upvaluejoin") - 1;
			int n2 = args.AsInt(3, "upvaluejoin") - 1;

			Closure c1 = f1.Function;
			Closure c2 = f2.Function;

			if (n1 < 0 || n1 >= c1.ClosureContext.Count)
				throw ScriptRuntimeException.BadArgument(1, "upvaluejoin", "invalid upvalue index");
			
			if (n2 < 0 || n2 >= c2.ClosureContext.Count)
				throw ScriptRuntimeException.BadArgument(3, "upvaluejoin", "invalid upvalue index");

			c2.ClosureContext[n2] = c1.ClosureContext[n1];

			return DynValue.Void;
		}


		[MoonSharpModuleMethod]
		public static DynValue traceback(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			StringBuilder sb = new StringBuilder();

			DynValue vmessage = args[0];
			DynValue vlevel = args[1];

			double defaultSkip = 1.0;

			Coroutine cor = executionContext.GetCallingCoroutine();

			if (vmessage.Type == DataType.Thread)
			{
				cor = vmessage.Coroutine;
				vmessage = args[1];
				vlevel = args[2];
				defaultSkip = 0.0;
			}

			if (vmessage.IsNotNil() && vmessage.Type != DataType.String && vmessage.Type != DataType.Number)
			{
				return vmessage;
			}

			string message = vmessage.CastToString();

			int skip = (int)((vlevel.CastToNumber()) ?? defaultSkip);

			WatchItem[] stacktrace = cor.GetStackTrace(Math.Max(0, skip));

			if (message != null)
				sb.AppendLine(message);

			sb.AppendLine("stack traceback:");

			foreach (WatchItem wi in stacktrace)
			{
				string name;

				if (wi.Name == null)
					if (wi.RetAddress < 0)
						name = "main chunk";
					else
						name = "?";
				else
					name = "function '" + wi.Name + "'";

				string loc = wi.Location != null ? wi.Location.FormatLocation(executionContext.GetScript()) : "[clr]";
				sb.AppendFormat("\t{0}: in {1}\n", loc, name);
			}

			return DynValue.NewString(sb);
		}

		//[MoonSharpModuleMethod]
		//public static DynValue getlocal(ScriptExecutionContext executionContext, CallbackArguments args)
		//{
		//	Coroutine c;
		//	int funcIdx;
		//	Closure f;
		//	int nextArg = ParseComplexArgs("getlocal", executionContext, args, out c, out f, out funcIdx);

		//	int localIdx = args.AsInt(nextArg, "getlocal");

		//	if (f != null)
		//	{
				
		//	}
		//	else
		//	{

		//	}



		//}

		//private static int ParseComplexArgs(string funcname, ScriptExecutionContext executionContext, CallbackArguments args, out Coroutine c, out Closure f, out int funcIdx)
		//{
		//	DynValue arg1 = args[0];
		//	int argbase = 0;
		//	c = null;

		//	if (arg1.Type == DataType.Thread)
		//	{
		//		c = arg1.Coroutine;
		//		argbase = 1;
		//	}

		//	if (args[argbase].Type == DataType.Number)
		//	{
		//		funcIdx = (int)args[argbase].Number;
		//		f = null;
		//	}
		//	else
		//	{
		//		funcIdx = -1;
		//		f = args.AsType(argbase, funcname, DataType.Function, false).Function;
		//	}

		//	return argbase + 1;
		//}


		//[MoonSharpMethod]
		//public static DynValue getinfo(ScriptExecutionContext executionContext, CallbackArguments args)
		//{
		//	Coroutine cor = executionContext.GetCallingCoroutine();
		//	int vfArgIdx = 0;

		//	if (args[0].Type == DataType.Thread)
		//		cor = args[0].Coroutine;

		//	DynValue vf = args[vfArgIdx+0];
		//	DynValue vwhat = args[vfArgIdx+1];

		//	args.AsType(vfArgIdx + 1, "getinfo", DataType.String, true);
			
		//	string what = vwhat.CastToString() ?? "nfSlu";

		//	DynValue vt = DynValue.NewTable(executionContext.GetScript());
		//	Table t = vt.Table;

		//	if (vf.Type == DataType.Function)
		//	{
		//		Closure f = vf.Function;
		//		executionContext.GetInfoForFunction
		//	}
		//	else if (vf.Type == DataType.ClrFunction)
		//	{

		//	}
		//	else if (vf.Type == DataType.Number || vf.Type == DataType.String)
		//	{

		//	}
		//	else
		//	{
		//		args.AsType(vfArgIdx + 0, "getinfo", DataType.Number, true);
		//	}

		//	return vt;


		//}

	}
}
