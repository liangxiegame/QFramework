using System;

namespace MoonSharp.Interpreter
{
	/// <summary>
	/// Marks a method as the handler of metamethods of a userdata type
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
	public sealed class MoonSharpUserDataMetamethodAttribute : Attribute
	{
		/// <summary>
		/// The metamethod name (like '__div', '__ipairs', etc.)
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="MoonSharpUserDataMetamethodAttribute"/> class.
		/// </summary>
		/// <param name="name">The metamethod name (like '__div', '__ipairs', etc.)</param>
		public MoonSharpUserDataMetamethodAttribute(string name)
		{
			Name = name;
		}
	}

}
