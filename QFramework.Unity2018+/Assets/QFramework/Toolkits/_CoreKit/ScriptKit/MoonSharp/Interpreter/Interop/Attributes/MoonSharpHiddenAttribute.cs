using System;

namespace MoonSharp.Interpreter
{
	/// <summary>
	/// Forces a class member visibility to scripts. Can be used to hide public members. Equivalent to MoonSharpVisible(false).
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field
		| AttributeTargets.Constructor | AttributeTargets.Event, Inherited = true, AllowMultiple = false)]
	public sealed class MoonSharpHiddenAttribute : Attribute
	{
	}
}
