// Disable warnings about XML documentation
#pragma warning disable 1591


namespace MoonSharp.Interpreter.CoreLib
{
	/// <summary>
	/// Class implementing metatable related Lua functions (xxxmetatable and rawxxx).
	/// </summary>
	[MoonSharpModule]
	public class MetaTableModule
	{
		// setmetatable (table, metatable)
		// -------------------------------------------------------------------------------------------------------------------
		// Sets the metatable for the given table. (You cannot change the metatable of other 
		// types from Lua, only from C.) If metatable is nil, removes the metatable of the given table. 
		// If the original metatable has a "__metatable" field, raises an error ("cannot change a protected metatable").
		// This function returns table. 
		[MoonSharpModuleMethod]
		public static DynValue setmetatable(ScriptExecutionContext executionContext, CallbackArguments args)  
		{
			DynValue table = args.AsType(0, "setmetatable", DataType.Table);
			DynValue metatable = args.AsType(1, "setmetatable", DataType.Table, true);

			DynValue curmeta = executionContext.GetMetamethod(table, "__metatable");

			if (curmeta != null)
			{
				throw new ScriptRuntimeException("cannot change a protected metatable");
			}

			table.Table.MetaTable = metatable.Table;
			return table;
		}

		// getmetatable (object)
		// -------------------------------------------------------------------------------------------------------------------
		// If object does not have a metatable, returns nil. Otherwise, if the object's metatable 
		// has a "__metatable" field, returns the associated value. Otherwise, returns the metatable of the given object. 
		[MoonSharpModuleMethod]
		public static DynValue getmetatable(ScriptExecutionContext executionContext, CallbackArguments args)  
		{
			DynValue obj = args[0];
			Table meta = null;

			if (obj.Type.CanHaveTypeMetatables())
			{
				meta = executionContext.GetScript().GetTypeMetatable(obj.Type);
			}
			
			
			if (obj.Type == DataType.Table)
			{
				meta = obj.Table.MetaTable;
			}

			if (meta == null)
				return DynValue.Nil;
			else if (meta.RawGet("__metatable") != null)
				return meta.Get("__metatable");
			else
				return DynValue.NewTable(meta);
		}

		// rawget (table, index)
		// -------------------------------------------------------------------------------------------------------------------
		// Gets the real value of table[index], without invoking any metamethod. table must be a table; index may be any value.
		[MoonSharpModuleMethod]
		public static DynValue rawget(ScriptExecutionContext executionContext, CallbackArguments args)  
		{
			DynValue table = args.AsType(0, "rawget", DataType.Table);
			DynValue index = args[1];

			return table.Table.Get(index);
		}

		// rawset (table, index, value)
		// -------------------------------------------------------------------------------------------------------------------
		// Sets the real value of table[index] to value, without invoking any metamethod. table must be a table, 
		// index any value different from nil and NaN, and value any Lua value.
		// This function returns table. 
		[MoonSharpModuleMethod]
		public static DynValue rawset(ScriptExecutionContext executionContext, CallbackArguments args)  
		{
			DynValue table = args.AsType(0, "rawset", DataType.Table);
			DynValue index = args[1];

			table.Table.Set(index, args[2]);

			return table;
		}

		// rawequal (v1, v2)
		// -------------------------------------------------------------------------------------------------------------------
		// Checks whether v1 is equal to v2, without invoking any metamethod. Returns a boolean. 
		[MoonSharpModuleMethod]
		public static DynValue rawequal(ScriptExecutionContext executionContext, CallbackArguments args)  
		{
			DynValue v1 = args[0];
			DynValue v2 = args[1];

			return DynValue.NewBoolean(v1.Equals(v2)); 
		}

		//rawlen (v)
		// -------------------------------------------------------------------------------------------------------------------
		//Returns the length of the object v, which must be a table or a string, without invoking any metamethod. Returns an integer number.	
		[MoonSharpModuleMethod]
		public static DynValue rawlen(ScriptExecutionContext executionContext, CallbackArguments args) 
		{
			if (args[0].Type != DataType.String && args[0].Type != DataType.Table)
			{
				throw ScriptRuntimeException.BadArgument(0, "rawlen", "table or string", args[0].Type.ToErrorTypeString(), false);
			}

			return args[0].GetLength();
		}



	}
}
