using MoonSharp.Interpreter.Serialization.Json;

namespace MoonSharp.Interpreter.CoreLib
{
	[MoonSharpModule(Namespace = "json")]
	public class JsonModule
	{
		[MoonSharpModuleMethod]
		public static DynValue parse(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			try
			{
				DynValue vs = args.AsType(0, "parse", DataType.String, false);
				Table t = JsonTableConverter.JsonToTable(vs.String, executionContext.GetScript());
				return DynValue.NewTable(t);
			}
			catch (SyntaxErrorException ex)
			{
				throw new ScriptRuntimeException(ex);
			}
		}

		[MoonSharpModuleMethod]
		public static DynValue serialize(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			try
			{
				DynValue vt = args.AsType(0, "serialize", DataType.Table, false);
				string s = JsonTableConverter.TableToJson(vt.Table);
				return DynValue.NewString(s);
			}
			catch (SyntaxErrorException ex)
			{
				throw new ScriptRuntimeException(ex);
			}
		}
		[MoonSharpModuleMethod]
		public static DynValue isnull(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			DynValue vs = args[0];
			return DynValue.NewBoolean((JsonNull.IsJsonNull(vs)) || (vs.IsNil()));
		}

		[MoonSharpModuleMethod]
		public static DynValue @null(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return JsonNull.Create();
		}
	}
}
