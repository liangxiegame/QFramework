

namespace MoonSharp.Interpreter
{
	/// <summary>
	/// Enumeration of possible data types in MoonSharp
	/// </summary>
	public enum DataType
	{
		// DO NOT MODIFY ORDER - IT'S SIGNIFICANT

		/// <summary>
		/// A nil value, as in Lua
		/// </summary>
		Nil,
		/// <summary>
		/// A place holder for no value
		/// </summary>
		Void,
		/// <summary>
		/// A Lua boolean
		/// </summary>
		Boolean,
		/// <summary>
		/// A Lua number
		/// </summary>
		Number,
		/// <summary>
		/// A Lua string
		/// </summary>
		String,
		/// <summary>
		/// A Lua function
		/// </summary>
		Function,

		/// <summary>
		/// A Lua table
		/// </summary>
		Table,
		/// <summary>
		/// A set of multiple values
		/// </summary>
		Tuple,
		/// <summary>
		/// A userdata reference - that is a wrapped CLR object
		/// </summary>
		UserData,
		/// <summary>
		/// A coroutine handle
		/// </summary>
		Thread,

		/// <summary>
		/// A callback function
		/// </summary>
		ClrFunction,

		/// <summary>
		/// A request to execute a tail call
		/// </summary>
		TailCallRequest,
		/// <summary>
		/// A request to coroutine.yield
		/// </summary>
		YieldRequest,
	}

	/// <summary>
	/// Extension methods to DataType
	/// </summary>
	public static class LuaTypeExtensions
	{
		internal const DataType MaxMetaTypes = DataType.Table;
		internal const DataType MaxConvertibleTypes = DataType.ClrFunction;

		/// <summary>
		/// Determines whether this data type can have type metatables.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public static bool CanHaveTypeMetatables(this DataType type)
		{
			return (int)type < (int)MaxMetaTypes;
		}

		/// <summary>
		/// Converts the DataType to the string returned by the "type(...)" Lua function
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		/// <exception cref="ScriptRuntimeException">The DataType is not a Lua type</exception>
		public static string ToErrorTypeString(this DataType type)
		{
			switch (type)
			{
				case DataType.Void:
					return "no value";
				case DataType.Nil:
					return "nil";
				case DataType.Boolean:
					return "boolean";
				case DataType.Number:
					return "number";
				case DataType.String:
					return "string";
				case DataType.Function:
					return "function";
				case DataType.ClrFunction:
					return "function";
				case DataType.Table:
					return "table";
				case DataType.UserData:
					return "userdata";
				case DataType.Thread:
					return "coroutine";
				case DataType.Tuple:
				case DataType.TailCallRequest:
				case DataType.YieldRequest:
				default:
					return string.Format("internal<{0}>", type.ToLuaDebuggerString());
			}
		}

		/// <summary>
		/// Converts the DataType to the string returned by the "type(...)" Lua function, with additional values
		/// to support debuggers
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		/// <exception cref="ScriptRuntimeException">The DataType is not a Lua type</exception>
		public static string ToLuaDebuggerString(this DataType type)
		{
			return type.ToString().ToLowerInvariant();
		}


		/// <summary>
		/// Converts the DataType to the string returned by the "type(...)" Lua function
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		/// <exception cref="ScriptRuntimeException">The DataType is not a Lua type</exception>
		public static string ToLuaTypeString(this DataType type)
		{
			switch (type)
			{
				case DataType.Void:
				case DataType.Nil:
					return "nil";
				case DataType.Boolean:
					return "boolean";
				case DataType.Number:
					return "number";
				case DataType.String:
					return "string";
				case DataType.Function:
					return "function";
				case DataType.ClrFunction:
					return "function";
				case DataType.Table:
					return "table";
				case DataType.UserData:
					return "userdata";
				case DataType.Thread:
					return "thread";
				case DataType.Tuple:
				case DataType.TailCallRequest:
				case DataType.YieldRequest:
				default:
					throw new ScriptRuntimeException("Unexpected LuaType {0}", type);
			}
		}
	}
}
