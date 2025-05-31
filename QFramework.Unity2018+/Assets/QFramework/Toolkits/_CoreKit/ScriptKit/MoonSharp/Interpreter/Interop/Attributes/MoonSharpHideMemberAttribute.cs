using System;

namespace MoonSharp.Interpreter
{
	/// <summary>
	/// Lists a userdata member not to be exposed to scripts referencing it by name.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = true, AllowMultiple = true)]
	public sealed class MoonSharpHideMemberAttribute : Attribute
	{
		/// <summary>
		/// Gets the name of the member to be hidden.
		/// </summary>
		public string MemberName { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="MoonSharpHideMemberAttribute"/> class.
		/// </summary>
		/// <param name="memberName">Name of the member to hide.</param>
		public MoonSharpHideMemberAttribute(string memberName)
		{
			MemberName = memberName;
		}
	}
}
