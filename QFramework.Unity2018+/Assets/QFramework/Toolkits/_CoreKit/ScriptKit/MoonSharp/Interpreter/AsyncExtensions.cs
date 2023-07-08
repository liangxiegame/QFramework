#if HASDYNAMIC
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MoonSharp.Interpreter.REPL;

namespace MoonSharp.Interpreter
{
	/// <summary>
	/// This class contains extension methods providing async wrappers of many methods.
	/// Asynchronous execution is performed by scheduling the method on the thread pool (through a Task.Factory.StartNew).
	/// 
	/// This type is supported only on .NET 4.x and .NET 4.x PCL targets.
	/// </summary>
	public static class AsyncExtensions
	{
		private static Task<T> ExecAsync<T>(Func<T> func)
		{
			return Task.Factory.StartNew<T>(func);
		}

		private static Task ExecAsyncVoid(Action func)
		{
			return Task.Factory.StartNew(func);
		}



		/// <summary>
		/// Asynchronously calls this function with the specified args
		/// 
		/// This method is supported only on .NET 4.x and .NET 4.x PCL targets.
		/// </summary>
		/// <param name="function">The function.</param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentException">Thrown if function is not of DataType.Function</exception>
		public static Task<DynValue> CallAsync(this Closure function)
		{
			return ExecAsync(() => function.Call());
		}

		/// <summary>
		/// Asynchronously calls this function with the specified args
		/// 
		/// This method is supported only on .NET 4.x and .NET 4.x PCL targets.
		/// </summary>
		/// <param name="function">The function.</param>
		/// <param name="args">The arguments to pass to the function.</param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentException">Thrown if function is not of DataType.Function</exception>
		public static Task<DynValue> CallAsync(this Closure function, params object[] args)
		{
			return ExecAsync(() => function.Call(args));
		}

		/// <summary>
		/// Asynchronously calls this function with the specified args
		/// 
		/// This method is supported only on .NET 4.x and .NET 4.x PCL targets.
		/// </summary>
		/// <param name="function">The function.</param>
		/// <param name="args">The arguments to pass to the function.</param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentException">Thrown if function is not of DataType.Function</exception>
		public static Task<DynValue> CallAsync(this Closure function, params DynValue[] args)
		{
			return ExecAsync(() => function.Call(args));
		}

		/// <summary>
		/// Asynchronously loads and executes a string containing a Lua/MoonSharp script.
		/// 
		/// This method is supported only on .NET 4.x and .NET 4.x PCL targets.
		/// </summary>
		/// <param name="script">The script.</param>
		/// <param name="code">The code.</param>
		/// <param name="globalContext">The global context.</param>
		/// <param name="codeFriendlyName">Name of the code - used to report errors, etc. Also used by debuggers to locate the original source file.</param>
		/// <returns>
		/// A DynValue containing the result of the processing of the loaded chunk.
		/// </returns>
		public static Task<DynValue> DoStringAsync(this Script script, string code, Table globalContext = null, string codeFriendlyName = null)
		{
			return ExecAsync(() => script.DoString(code, globalContext, codeFriendlyName));
		}


		/// <summary>
		/// Asynchronously loads and executes a stream containing a Lua/MoonSharp script.
		/// 
		/// This method is supported only on .NET 4.x and .NET 4.x PCL targets.
		/// </summary>
		/// <param name="script">The script.</param>
		/// <param name="stream">The stream.</param>
		/// <param name="globalContext">The global context.</param>
		/// <param name="codeFriendlyName">Name of the code - used to report errors, etc. Also used by debuggers to locate the original source file.</param>
		/// <returns>
		/// A DynValue containing the result of the processing of the loaded chunk.
		/// </returns>
		public static Task<DynValue> DoStreamAsync(this Script script, Stream stream, Table globalContext = null, string codeFriendlyName = null)
		{
			return ExecAsync(() => script.DoStream(stream, globalContext, codeFriendlyName));
		}


		/// <summary>
		/// Asynchronously loads and executes a file containing a Lua/MoonSharp script.
		/// 
		/// This method is supported only on .NET 4.x and .NET 4.x PCL targets.
		/// </summary>
		/// <param name="script">The script.</param>
		/// <param name="filename">The filename.</param>
		/// <param name="globalContext">The global context.</param>
		/// <param name="codeFriendlyName">Name of the code - used to report errors, etc. Also used by debuggers to locate the original source file.</param>
		/// <returns>
		/// A DynValue containing the result of the processing of the loaded chunk.
		/// </returns>
		public static Task<DynValue> DoFileAsync(this Script script, string filename, Table globalContext = null, string codeFriendlyName = null)
		{
			return ExecAsync(() => script.DoFile(filename, globalContext, codeFriendlyName));
		}

		/// <summary>
		/// Asynchronously loads a string containing a Lua/MoonSharp function.
		/// 
		/// This method is supported only on .NET 4.x and .NET 4.x PCL targets.
		/// </summary>
		/// <param name="script">The script.</param>
		/// <param name="code">The code.</param>
		/// <param name="globalTable">The global table to bind to this chunk.</param>
		/// <param name="funcFriendlyName">Name of the function used to report errors, etc.</param>
		/// <returns>
		/// A DynValue containing a function which will execute the loaded code.
		/// </returns>
		public static Task<DynValue> LoadFunctionAsync(this Script script, string code, Table globalTable = null, string funcFriendlyName = null)
		{
			return ExecAsync(() => script.LoadFunction(code, globalTable, funcFriendlyName));
		}



		/// <summary>
		/// Asynchronously loads a string containing a Lua/MoonSharp script.
		/// 
		/// This method is supported only on .NET 4.x and .NET 4.x PCL targets.
		/// </summary>
		/// <param name="script">The script.</param>
		/// <param name="code">The code.</param>
		/// <param name="globalTable">The global table to bind to this chunk.</param>
		/// <param name="codeFriendlyName">Name of the code - used to report errors, etc.</param>
		/// <returns>
		/// A DynValue containing a function which will execute the loaded code.
		/// </returns>
		public static Task<DynValue> LoadStringAsync(this Script script, string code, Table globalTable = null, string codeFriendlyName = null)
		{
			return ExecAsync(() => script.LoadString(code, globalTable, codeFriendlyName));
		}



		/// <summary>
		/// Asynchronously loads a Lua/MoonSharp script from a System.IO.Stream. NOTE: This will *NOT* close the stream!
		/// 
		/// This method is supported only on .NET 4.x and .NET 4.x PCL targets.
		/// </summary>
		/// <param name="script">The script.</param>
		/// <param name="stream">The stream containing code.</param>
		/// <param name="globalTable">The global table to bind to this chunk.</param>
		/// <param name="codeFriendlyName">Name of the code - used to report errors, etc.</param>
		/// <returns>
		/// A DynValue containing a function which will execute the loaded code.
		/// </returns>
		public static Task<DynValue> LoadStreamAsync(this Script script, Stream stream, Table globalTable = null, string codeFriendlyName = null)
		{
			return ExecAsync(() => script.LoadStream(stream, globalTable, codeFriendlyName));
		}


		/// <summary>
		/// Asynchronously dumps a function on the specified stream.
		/// This method is supported only on .NET 4.x and .NET 4.x PCL targets.
		/// </summary>
		/// <param name="script">The script.</param>
		/// <param name="function">The function.</param>
		/// <param name="stream">The stream.</param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentException">function arg is not a function!
		/// or
		/// stream is readonly!
		/// or
		/// function arg has upvalues other than _ENV</exception>
		public static Task DumpAsync(this Script script, DynValue function, Stream stream)
		{
			return ExecAsyncVoid(() => script.Dump(function, stream));
		}



		/// <summary>
		/// Asynchronously loads a string containing a Lua/MoonSharp script.
		/// This method is supported only on .NET 4.x and .NET 4.x PCL targets.
		/// </summary>
		/// <param name="script">The script.</param>
		/// <param name="filename">The code.</param>
		/// <param name="globalContext">The global table to bind to this chunk.</param>
		/// <param name="friendlyFilename">The filename to be used in error messages.</param>
		/// <returns>
		/// A DynValue containing a function which will execute the loaded code.
		/// </returns>
		public static Task<DynValue> LoadFileAsync(this Script script, string filename, Table globalContext = null, string friendlyFilename = null)
		{
			return ExecAsync(() => script.LoadFile(filename, globalContext, friendlyFilename));
		}



		/// <summary>
		/// Calls the specified function.
		/// This method is supported only on .NET 4.x and .NET 4.x PCL targets.
		/// </summary>
		/// <param name="script">The script.</param>
		/// <param name="function">The Lua/MoonSharp function to be called</param>
		/// <returns>
		/// The return value(s) of the function call.
		/// </returns>
		/// <exception cref="System.ArgumentException">Thrown if function is not of DataType.Function</exception>
		public static Task<DynValue> CallAsync(this Script script, DynValue function)
		{
			return ExecAsync(() => script.Call(function));
		}


		/// <summary>
		/// Asynchronously calls the specified function.
		/// This method is supported only on .NET 4.x and .NET 4.x PCL targets.
		/// </summary>
		/// <param name="script">The script.</param>
		/// <param name="function">The Lua/MoonSharp function to be called</param>
		/// <param name="args">The arguments to pass to the function.</param>
		/// <returns>
		/// The return value(s) of the function call.
		/// </returns>
		/// <exception cref="System.ArgumentException">Thrown if function is not of DataType.Function</exception>
		public static Task<DynValue> CallAsync(this Script script, DynValue function, params DynValue[] args)
		{
			return ExecAsync(() => script.Call(function, args));
		}



		/// <summary>
		/// Asynchronously calls the specified function.
		/// This method is supported only on .NET 4.x and .NET 4.x PCL targets.
		/// </summary>
		/// <param name="script">The script.</param>
		/// <param name="function">The Lua/MoonSharp function to be called</param>
		/// <param name="args">The arguments to pass to the function.</param>
		/// <returns>
		/// The return value(s) of the function call.
		/// </returns>
		/// <exception cref="System.ArgumentException">Thrown if function is not of DataType.Function</exception>
		public static Task<DynValue> CallAsync(this Script script, DynValue function, params object[] args)
		{
			return ExecAsync(() => script.Call(function, args));
		}



		/// <summary>
		/// Asynchronously calls the specified function.
		/// This method is supported only on .NET 4.x and .NET 4.x PCL targets.
		/// </summary>
		/// <param name="script">The script.</param>
		/// <param name="function">The Lua/MoonSharp function to be called</param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentException">Thrown if function is not of DataType.Function</exception>
		public static Task<DynValue> CallAsync(this Script script, object function)
		{
			return ExecAsync(() => script.Call(function));
		}


		/// <summary>
		/// Asynchronously calls the specified function.
		/// 
		/// This method is supported only on .NET 4.x and .NET 4.x PCL targets.
		/// </summary>
		/// <param name="script">The script.</param>
		/// <param name="function">The Lua/MoonSharp function to be called</param>
		/// <param name="args">The arguments to pass to the function.</param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentException">Thrown if function is not of DataType.Function</exception>
		public static Task<DynValue> CallAsync(this Script script, object function, params object[] args)
		{
			return ExecAsync(() => script.Call(function, args));
		}

		/// <summary>
		/// Asynchronously creates a new dynamic expression.
		/// 
		/// This method is supported only on .NET 4.x and .NET 4.x PCL targets.
		/// </summary>
		/// <param name="script">The script.</param>
		/// <param name="code">The code of the expression.</param>
		/// <returns></returns>
		public static Task<DynamicExpression> CreateDynamicExpressionAsync(this Script script, string code)
		{
			return ExecAsync(() => script.CreateDynamicExpression(code));
		}

		/// <summary>
		/// Asynchronously evaluates a REPL command.
		/// This method returns the result of the computation, or null if more input is needed for having valid code.
		/// In case of errors, exceptions are propagated to the caller.
		/// 
		/// This method is supported only on .NET 4.x and .NET 4.x PCL targets.
		/// </summary>
		/// <param name="interpreter">The interpreter.</param>
		/// <param name="input">The input.</param>
		/// <returns>
		/// This method returns the result of the computation, or null if more input is needed for a computation.
		/// </returns>
		public static Task<DynValue> EvaluateAsync(this ReplInterpreter interpreter, string input)
		{
			return ExecAsync(() => interpreter.Evaluate(input));
		}

		/// <summary>
		/// Resumes the coroutine.
		/// Only non-CLR coroutines can be resumed with this overload of the Resume method. Use the overload accepting a ScriptExecutionContext instead.
		/// 
		/// This method is supported only on .NET 4.x and .NET 4.x PCL targets.
		/// </summary>
		/// <param name="cor">The coroutine</param>
		/// <param name="args">The arguments.</param>
		/// <returns></returns>
		/// <exception cref="System.InvalidOperationException">Only non-CLR coroutines can be resumed with this overload of the Resume method. Use the overload accepting a ScriptExecutionContext instead</exception>
		public static Task<DynValue> ResumeAsync(this Coroutine cor, params DynValue[] args)
		{
			return ExecAsync(() => cor.Resume(args));
		}


		/// <summary>
		/// Resumes the coroutine.
		/// 
		/// This method is supported only on .NET 4.x and .NET 4.x PCL targets.
		/// </summary>
		/// <param name="cor">The coroutine</param>
		/// <param name="context">The ScriptExecutionContext.</param>
		/// <param name="args">The arguments.</param>
		/// <returns></returns>
		public static Task<DynValue> ResumeAsync(this Coroutine cor, ScriptExecutionContext context, params DynValue[] args)
		{
			return ExecAsync(() => cor.Resume(context, args));
		}

		/// <summary>
		/// Resumes the coroutine.
		/// Only non-CLR coroutines can be resumed with this overload of the Resume method. Use the overload accepting a ScriptExecutionContext instead.
		/// 
		/// This method is supported only on .NET 4.x and .NET 4.x PCL targets.
		/// </summary>
		/// <param name="cor">The coroutine</param>
		/// <returns></returns>
		/// <exception cref="System.InvalidOperationException">Only non-CLR coroutines can be resumed with this overload of the Resume method. Use the overload accepting a ScriptExecutionContext instead</exception>
		public static Task<DynValue> ResumeAsync(this Coroutine cor)
		{
			return ExecAsync(() => cor.Resume());
		}


		/// <summary>
		/// Resumes the coroutine.
		/// 
		/// This method is supported only on .NET 4.x and .NET 4.x PCL targets.
		/// </summary>
		/// <param name="cor">The coroutine</param>
		/// <param name="context">The ScriptExecutionContext.</param>
		/// <returns></returns>
		public static Task<DynValue> ResumeAsync(this Coroutine cor, ScriptExecutionContext context)
		{
			return ExecAsync(() => cor.Resume(context));
		}

		/// <summary>
		/// Resumes the coroutine.
		/// Only non-CLR coroutines can be resumed with this overload of the Resume method. Use the overload accepting a ScriptExecutionContext instead.
		/// 
		/// This method is supported only on .NET 4.x and .NET 4.x PCL targets.
		/// </summary>
		/// <param name="cor">The coroutine</param>
		/// <param name="args">The arguments.</param>
		/// <returns></returns>
		/// <exception cref="System.InvalidOperationException">Only non-CLR coroutines can be resumed with this overload of the Resume method. Use the overload accepting a ScriptExecutionContext instead.</exception>
		public static Task<DynValue> ResumeAsync(this Coroutine cor, params object[] args)
		{
			return ExecAsync(() => cor.Resume(args));
		}


		/// <summary>
		/// Resumes the coroutine
		/// 
		/// This method is supported only on .NET 4.x and .NET 4.x PCL targets.
		/// </summary>
		/// <param name="cor">The coroutine</param>
		/// <param name="context">The ScriptExecutionContext.</param>
		/// <param name="args">The arguments.</param>
		/// <returns></returns>
		public static Task<DynValue> ResumeAsync(this Coroutine cor, ScriptExecutionContext context, params object[] args)
		{
			return ExecAsync(() => cor.Resume(context, args));
		}
	}
}
#endif
