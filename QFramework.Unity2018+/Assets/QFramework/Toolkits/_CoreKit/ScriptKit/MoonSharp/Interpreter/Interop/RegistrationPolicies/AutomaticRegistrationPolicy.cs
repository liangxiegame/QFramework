using System;

namespace MoonSharp.Interpreter.Interop.RegistrationPolicies
{
	/// <summary>
	/// Similar to <see cref="DefaultRegistrationPolicy"/>, but with automatic type registration is disabled.
	/// </summary>
	public class AutomaticRegistrationPolicy : DefaultRegistrationPolicy
	{
		/// <summary>
		/// Allows type automatic registration for the specified type.
		/// NOTE: automatic type registration is NOT recommended.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>
		/// True to register the type automatically, false otherwise.
		/// </returns>
		public override bool AllowTypeAutoRegistration(Type type)
		{
			return true;
		}
	}
}
