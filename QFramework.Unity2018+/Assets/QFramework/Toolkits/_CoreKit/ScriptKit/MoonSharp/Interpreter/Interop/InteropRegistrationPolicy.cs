using System;
using MoonSharp.Interpreter.Interop.RegistrationPolicies;

namespace MoonSharp.Interpreter.Interop
{
	/// <summary>
	/// Collection of the standard policies to handle UserData type registrations.
	/// Provided mostly for compile-time backward compatibility with old code.
	/// See also : <see cref="IRegistrationPolicy"/> .
	/// </summary>
	public static class InteropRegistrationPolicy
	{
		/// <summary>
		/// The default registration policy used by MoonSharp unless explicitely replaced.
		/// Deregistrations are allowed, but registration of a new descriptor are not allowed
		/// if a descriptor is already registered for that type.
		/// 
		/// Types must be explicitly registered.
		/// </summary>
		public static IRegistrationPolicy Default { get { return new DefaultRegistrationPolicy(); }}

		/// <summary>
		/// The default registration policy used by MoonSharp unless explicitely replaced.
		/// Deregistrations are allowed, but registration of a new descriptor are not allowed
		/// if a descriptor is already registered for that type.
		/// 
		/// Types must be explicitly registered.
		/// </summary>
		[Obsolete("Please use InteropRegistrationPolicy.Default instead.")]
		public static IRegistrationPolicy Explicit { get { return new DefaultRegistrationPolicy(); } }
		/// <summary>
		/// Types are automatically registered if not found in the registry. This is easier to use but potentially unsafe.
		/// </summary>
		public static IRegistrationPolicy Automatic { get { return new AutomaticRegistrationPolicy(); } }
		
	}
}
