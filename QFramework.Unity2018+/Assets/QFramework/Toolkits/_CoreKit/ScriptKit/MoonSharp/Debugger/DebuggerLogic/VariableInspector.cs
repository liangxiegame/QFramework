#if (!PCL) && ((!UNITY_5) || UNITY_STANDALONE)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoonSharp.Interpreter;
using MoonSharp.VsCodeDebugger.SDK;

namespace MoonSharp.VsCodeDebugger.DebuggerLogic
{
	internal static class VariableInspector
	{
		internal static void InspectVariable(DynValue v, List<Variable> variables)
		{
			variables.Add(new Variable("(value)", v.ToPrintString()));
			variables.Add(new Variable("(type)", v.Type.ToLuaDebuggerString()));
			variables.Add(new Variable("(val #id)", v.ReferenceID.ToString()));

			switch (v.Type)
			{
				case DataType.Tuple:
					for (int i = 0; i < v.Tuple.Length; i++)
						variables.Add(new Variable("[i]", (v.Tuple[i] ?? DynValue.Void).ToDebugPrintString()));
					break;
				case DataType.Function:
					variables.Add(new Variable("(address)", v.Function.EntryPointByteCodeLocation.ToString("X8")));
					variables.Add(new Variable("(upvalues)", v.Function.GetUpvaluesCount().ToString()));
					variables.Add(new Variable("(upvalues type)", v.Function.GetUpvaluesType().ToString()));
					break;
				case DataType.Table:

					if (v.Table.MetaTable != null && (v.Table.OwnerScript == null))
						variables.Add(new Variable("(table type)", "prime table with metatable"));
					else if (v.Table.MetaTable != null)
						variables.Add(new Variable("(table type)", "has metatable"));
					else if (v.Table.OwnerScript == null)
						variables.Add(new Variable("(table type)", "prime table"));
					else
						variables.Add(new Variable("(table type)", "standard"));

					variables.Add(new Variable("(table #id)", v.Table.ReferenceID.ToString()));

					if (v.Table.MetaTable != null)
						variables.Add(new Variable("(metatable #id)", v.Table.MetaTable.ReferenceID.ToString()));

					variables.Add(new Variable("(length)", v.Table.Length.ToString()));

					foreach (TablePair p in v.Table.Pairs)
						variables.Add(new Variable("[" + p.Key.ToDebugPrintString() + "]", p.Value.ToDebugPrintString()));

					break;
				case DataType.UserData:
					if (v.UserData.Descriptor != null)
					{
						variables.Add(new Variable("(descriptor)", v.UserData.Descriptor.Name));
						variables.Add(new Variable("(native type)", v.UserData.Descriptor.Type.ToString()));
					}
					else
					{
						variables.Add(new Variable("(descriptor)", "null!"));
					}

					variables.Add(new Variable("(native object)", v.UserData.Object != null ? v.UserData.Object.ToString() : "(null)"));
					break;
				case DataType.Thread:
					variables.Add(new Variable("(coroutine state)", v.Coroutine.State.ToString()));
					variables.Add(new Variable("(coroutine type)", v.Coroutine.Type.ToString()));
					variables.Add(new Variable("(auto-yield counter)", v.Coroutine.AutoYieldCounter.ToString()));
					break;
				case DataType.ClrFunction:
					variables.Add(new Variable("(name)", v.Callback.Name ?? "(unnamed)"));
					break;
				case DataType.TailCallRequest:
				case DataType.YieldRequest:
				case DataType.Nil:
				case DataType.Void:
				case DataType.Boolean:
				case DataType.Number:
				case DataType.String:
				default:
					break;
			}
		}
	}
}

#endif