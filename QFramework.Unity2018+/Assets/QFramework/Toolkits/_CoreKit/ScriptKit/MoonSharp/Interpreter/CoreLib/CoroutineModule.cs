// Disable warnings about XML documentation
#pragma warning disable 1591

using System.Collections.Generic;

namespace MoonSharp.Interpreter.CoreLib
{
	/// <summary>
	/// Class implementing coroutine Lua functions 
	/// </summary>
	[MoonSharpModule(Namespace = "coroutine")]
	public class CoroutineModule
	{
		[MoonSharpModuleMethod]
		public static DynValue create(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			if (args[0].Type != DataType.Function && args[0].Type != DataType.ClrFunction)
				args.AsType(0, "create", DataType.Function); // this throws

			return executionContext.GetScript().CreateCoroutine(args[0]);
		}

		[MoonSharpModuleMethod]
		public static DynValue wrap(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			if (args[0].Type != DataType.Function && args[0].Type != DataType.ClrFunction)
				args.AsType(0, "wrap", DataType.Function); // this throws

			DynValue v = create(executionContext, args);
			DynValue c = DynValue.NewCallback(__wrap_wrapper);
			c.Callback.AdditionalData = v;
			return c;
		}

		public static DynValue __wrap_wrapper(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue handle = (DynValue)executionContext.AdditionalData;
			return handle.Coroutine.Resume(args.GetArray());
		}

		[MoonSharpModuleMethod]
		public static DynValue resume(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue handle = args.AsType(0, "resume", DataType.Thread);

			try
			{
				DynValue ret = handle.Coroutine.Resume(args.GetArray(1));

				List<DynValue> retval = new List<DynValue>();
				retval.Add(DynValue.True);

				if (ret.Type == DataType.Tuple)
				{
					for (int i = 0; i < ret.Tuple.Length; i++)
					{
						var v = ret.Tuple[i];

						if ((i == ret.Tuple.Length - 1) && (v.Type == DataType.Tuple))
						{
							retval.AddRange(v.Tuple);
						}
						else
						{
							retval.Add(v);
						}
					}
				}
				else
				{
					retval.Add(ret);
				}

				return DynValue.NewTuple(retval.ToArray());
			}
			catch (ScriptRuntimeException ex)
			{
				return DynValue.NewTuple(
					DynValue.False,
					DynValue.NewString(ex.Message));
			}
		}

		[MoonSharpModuleMethod]
		public static DynValue yield(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return DynValue.NewYieldReq(args.GetArray());
		}



		[MoonSharpModuleMethod]
		public static DynValue running(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			Coroutine C = executionContext.GetCallingCoroutine();
			return DynValue.NewTuple(DynValue.NewCoroutine(C), DynValue.NewBoolean(C.State == CoroutineState.Main));
		}

		[MoonSharpModuleMethod]
		public static DynValue status(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue handle = args.AsType(0, "status", DataType.Thread);
			Coroutine running = executionContext.GetCallingCoroutine();
			CoroutineState cs = handle.Coroutine.State;

			switch (cs)
			{
				case CoroutineState.Main:
				case CoroutineState.Running:
					return (handle.Coroutine == running) ?
						DynValue.NewString("running") :
						DynValue.NewString("normal");
				case CoroutineState.NotStarted:
				case CoroutineState.Suspended:
				case CoroutineState.ForceSuspended:
					return DynValue.NewString("suspended");
				case CoroutineState.Dead:
					return DynValue.NewString("dead");
				default:
					throw new InternalErrorException("Unexpected coroutine state {0}", cs);
			}
	
		}


	}
}
