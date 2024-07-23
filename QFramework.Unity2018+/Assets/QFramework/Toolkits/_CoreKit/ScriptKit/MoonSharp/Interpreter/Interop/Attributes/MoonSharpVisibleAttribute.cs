using System;

namespace MoonSharp.Interpreter.Interop
{
	/// <summary>
	/// Forces a class member visibility to scripts. Can be used to hide public members or to expose non-public ones.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field
		| AttributeTargets.Constructor | AttributeTargets.Event, Inherited = true, AllowMultiple = false)]
	public sealed class MoonSharpVisibleAttribute : Attribute
	{
		/// <summary>
		/// Gets a value indicating whether this <see cref="MoonSharpVisibleAttribute"/> is set to "visible".
		/// </summary>
		public bool Visible { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="MoonSharpVisibleAttribute"/> class.
		/// </summary>
		/// <param name="visible">if set to true the member will be exposed to scripts, if false the member will be hidden.</param>
		public MoonSharpVisibleAttribute(bool visible)
		{
			Visible = visible;
		}
	}
}
