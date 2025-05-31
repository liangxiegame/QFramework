using System;

namespace MoonSharp.Interpreter
{
	/// <summary>
	/// Marks a CLR type to be a MoonSharp module.
	/// Modules are the fastest way to bring interop between scripts and CLR code, albeit at the cost of a very increased
	/// complexity in writing them. Modules is what's used for the standard library, for maximum efficiency.
	/// 
	/// Modules are basically classes containing only static methods, with the callback function signature.
	/// 
	/// See <see cref="Table"/> and <see cref="ModuleRegister"/> for (extension) methods used to register modules to a 
	/// table.
	/// 
	/// See <see cref="CallbackFunction"/> for information regarding the standard callback signature along with easier ways
	/// to marshal methods.
	/// 
	/// See <see cref="UserData"/> for easier object marshalling.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class MoonSharpModuleAttribute : Attribute
	{
		/// <summary>
		/// Gets or sets the namespace, that is the name of the table which will contain the defined functions.
		/// Can be null to be in the global table.
		/// </summary>
		public string Namespace { get; set; }
	}
}
