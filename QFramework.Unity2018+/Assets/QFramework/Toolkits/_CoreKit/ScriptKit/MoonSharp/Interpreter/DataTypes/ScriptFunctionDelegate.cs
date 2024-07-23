
namespace MoonSharp.Interpreter
{
	/// <summary>
	/// A Delegate type which can wrap a script function
	/// </summary>
	/// <param name="args">The arguments.</param>
	/// <returns>The return value of the script function</returns>
	public delegate object ScriptFunctionDelegate(params object[] args);
	/// <summary>
	/// A Delegate type which can wrap a script function with a generic typed return value
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="args">The arguments.</param>
	/// <returns>The return value of the script function</returns>
	public delegate T ScriptFunctionDelegate<T>(params object[] args);
}
