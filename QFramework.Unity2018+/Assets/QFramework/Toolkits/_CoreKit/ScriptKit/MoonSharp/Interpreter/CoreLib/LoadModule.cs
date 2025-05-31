// Disable warnings about XML documentation
#pragma warning disable 1591


namespace MoonSharp.Interpreter.CoreLib
{
	/// <summary>
	/// Class implementing loading Lua functions like 'require', 'load', etc.
	/// </summary>
	[MoonSharpModule]
	public class LoadModule
	{
		public static void MoonSharpInit(Table globalTable, Table ioTable)
		{
			DynValue package = globalTable.Get("package");

			if (package.IsNil())
			{
				package = DynValue.NewTable(globalTable.OwnerScript);
				globalTable["package"] = package;
			}
			else if (package.Type != DataType.Table)
			{
				throw new InternalErrorException("'package' global variable was found and it is not a table");
			}

#if PCL || ENABLE_DOTNET || NETFX_CORE 
			string cfg = "\\\n;\n?\n!\n-\n";
#else
			string cfg = System.IO.Path.DirectorySeparatorChar + "\n;\n?\n!\n-\n";
#endif

			package.Table.Set("config", DynValue.NewString(cfg));
		}



		// load (ld [, source [, mode [, env]]])
		// ----------------------------------------------------------------
		// Loads a chunk.
		// 
		// If ld is a string, the chunk is this string. 
		// 
		// If there are no syntactic errors, returns the compiled chunk as a function; 
		// otherwise, returns nil plus the error message.
		// 
		// source is used as the source of the chunk for error messages and debug 
		// information (see §4.9). When absent, it defaults to ld, if ld is a string, 
		// or to "=(load)" otherwise.
		// 
		// The string mode is ignored, and assumed to be "t"; 
		[MoonSharpModuleMethod]
		public static DynValue load(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return load_impl(executionContext, args, null);
		}

		// loadsafe (ld [, source [, mode [, env]]])
		// ----------------------------------------------------------------
		// Same as load, except that "env" defaults to the current environment of the function
		// calling load, instead of the actual global environment.
		[MoonSharpModuleMethod]
		public static DynValue loadsafe(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return load_impl(executionContext, args, GetSafeDefaultEnv(executionContext));
		}

		public static DynValue load_impl(ScriptExecutionContext executionContext, CallbackArguments args, Table defaultEnv)
		{
			try
			{
				Script S = executionContext.GetScript();
				DynValue ld = args[0];
				string script = "";

				if (ld.Type == DataType.Function)
				{
					while (true)
					{
						DynValue ret = executionContext.GetScript().Call(ld);
						if (ret.Type == DataType.String && ret.String.Length > 0)
							script += ret.String;
						else if (ret.IsNil())
							break;
						else
							return DynValue.NewTuple(DynValue.Nil, DynValue.NewString("reader function must return a string"));
					}
				}
				else if (ld.Type == DataType.String)
				{
					script = ld.String;
				}
				else
				{
					args.AsType(0, "load", DataType.Function, false);
				}

				DynValue source = args.AsType(1, "load", DataType.String, true);
				DynValue env = args.AsType(3, "load", DataType.Table, true);

				DynValue fn = S.LoadString(script,
					!env.IsNil() ? env.Table : defaultEnv,
					!source.IsNil() ? source.String : "=(load)");

				return fn;
			}
			catch (SyntaxErrorException ex)
			{
				return DynValue.NewTuple(DynValue.Nil, DynValue.NewString(ex.DecoratedMessage ?? ex.Message));
			}
		}

		// loadfile ([filename [, mode [, env]]])
		// ----------------------------------------------------------------
		// Similar to load, but gets the chunk from file filename or from the standard input, 
		// if no file name is given. INCOMPAT: stdin not supported, mode ignored
		[MoonSharpModuleMethod]
		public static DynValue loadfile(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return loadfile_impl(executionContext, args, null);
		}

		// loadfile ([filename [, mode [, env]]])
		// ----------------------------------------------------------------
		// Same as loadfile, except that "env" defaults to the current environment of the function
		// calling load, instead of the actual global environment.
		[MoonSharpModuleMethod]
		public static DynValue loadfilesafe(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			return loadfile_impl(executionContext, args, GetSafeDefaultEnv(executionContext));
		}



		private static DynValue loadfile_impl(ScriptExecutionContext executionContext, CallbackArguments args, Table defaultEnv)
		{
			try
			{
				Script S = executionContext.GetScript();
				DynValue filename = args.AsType(0, "loadfile", DataType.String, false);
				DynValue env = args.AsType(2, "loadfile", DataType.Table, true);

				DynValue fn = S.LoadFile(filename.String, env.IsNil() ? defaultEnv : env.Table);

				return fn;
			}
			catch (SyntaxErrorException ex)
			{
				return DynValue.NewTuple(DynValue.Nil, DynValue.NewString(ex.DecoratedMessage ?? ex.Message));
			}
		}


		private static Table GetSafeDefaultEnv(ScriptExecutionContext executionContext)
		{
			Table env = executionContext.CurrentGlobalEnv;

			if (env == null)
				throw new ScriptRuntimeException("current environment cannot be backtracked.");

			return env;
		}

		//dofile ([filename])
		//--------------------------------------------------------------------------------------------------------------
		//Opens the named file and executes its contents as a Lua chunk. When called without arguments, 
		//dofile executes the contents of the standard input (stdin). Returns all values returned by the chunk. 
		//In case of errors, dofile propagates the error to its caller (that is, dofile does not run in protected mode). 
		[MoonSharpModuleMethod]
		public static DynValue dofile(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			try
			{
				Script S = executionContext.GetScript();
				DynValue v = args.AsType(0, "dofile", DataType.String, false);

				DynValue fn = S.LoadFile(v.String);

				return DynValue.NewTailCallReq(fn); // tail call to dofile
			}
			catch (SyntaxErrorException ex)
			{
				throw new ScriptRuntimeException(ex);
			}
		}

		//require (modname)
		//----------------------------------------------------------------------------------------------------------------
		//Loads the given module. The function starts by looking into the package.loaded table to determine whether 
		//modname is already loaded. If it is, then require returns the value stored at package.loaded[modname]. 
		//Otherwise, it tries to find a loader for the module.
		//
		//To find a loader, require is guided by the package.loaders array. By changing this array, we can change 
		//how require looks for a module. The following explanation is based on the default configuration for package.loaders.
		//
		//First require queries package.preload[modname]. If it has a value, this value (which should be a function) 
		//is the loader. Otherwise require searches for a Lua loader using the path stored in package.path. 
		//If that also fails, it searches for a C loader using the path stored in package.cpath. If that also fails, 
		//it tries an all-in-one loader (see package.loaders).
		//
		//Once a loader is found, require calls the loader with a single argument, modname. If the loader returns any value, 
		//require assigns the returned value to package.loaded[modname]. If the loader returns no value and has not assigned 
		//any value to package.loaded[modname], then require assigns true to this entry. In any case, require returns the 
		//final value of package.loaded[modname].
		//
		//If there is any error loading or running the module, or if it cannot find any loader for the module, then require 
		//signals an error. 
		[MoonSharpModuleMethod]
		public static DynValue __require_clr_impl(ScriptExecutionContext executionContext, CallbackArguments args)
		{
			Script S = executionContext.GetScript();
			DynValue v = args.AsType(0, "__require_clr_impl", DataType.String, false);

			DynValue fn = S.RequireModule(v.String);

			return fn; // tail call to dofile
		}


		[MoonSharpModuleMethod]
		public const string require = @"
function(modulename)
	if (package == nil) then package = { }; end
	if (package.loaded == nil) then package.loaded = { }; end

	local m = package.loaded[modulename];

	if (m ~= nil) then
		return m;
	end

	local func = __require_clr_impl(modulename);

	local res = func(modulename);

	if (res == nil) then
		res = true;
	end

	package.loaded[modulename] = res;

	return res;
end";



	}
}
