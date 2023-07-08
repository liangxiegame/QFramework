using System;

namespace MoonSharp.Interpreter
{

	/// <summary>
	/// Marks a property as a configruation property
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
	public sealed class MoonSharpPropertyAttribute : Attribute
	{
		/// <summary>
		/// The metamethod name (like '__div', '__ipairs', etc.)
		/// </summary>
		public string Name { get; private set; }


		/// <summary>
		/// Initializes a new instance of the <see cref="MoonSharpPropertyAttribute"/> class.
		/// </summary>
		public MoonSharpPropertyAttribute()
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MoonSharpPropertyAttribute"/> class.
		/// </summary>
		/// <param name="name">The name for this property</param>
		public MoonSharpPropertyAttribute(string name)
		{
			Name = name;
		}
	}

}
