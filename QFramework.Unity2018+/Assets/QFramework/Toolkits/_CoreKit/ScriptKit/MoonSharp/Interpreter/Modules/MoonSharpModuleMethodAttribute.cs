using System;

namespace MoonSharp.Interpreter
{
	/// <summary>
	/// In a module type, mark methods or fields with this attribute to have them exposed as module functions.
	/// Methods must have the signature "public static DynValue ...(ScriptExecutionContextCallbackArguments)".
	/// Fields must be static or const strings, with an anonymous Lua function inside.
	/// 
	/// See <see cref="MoonSharpModuleAttribute"/> for more information about modules.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
	public sealed class MoonSharpModuleMethodAttribute : Attribute
	{
		/// <summary>
		/// Gets or sets the name of the function in the module (defaults to member name)
		/// </summary>
		public string Name { get; set; }
	}
}
