using System;
using MoonSharp.Interpreter.Interop.StandardDescriptors.HardwiredDescriptors;

namespace MoonSharp.Interpreter.Interop.RegistrationPolicies
{
	/// <summary>
	/// The default registration policy used by MoonSharp unless explicitely replaced.
	/// Deregistrations are allowed, but registration of a new descriptor are not allowed
	/// if a descriptor is already registered for that type.
	/// 
	/// Automatic type registration is disabled.
	/// </summary>
	public class DefaultRegistrationPolicy : IRegistrationPolicy
	{
		/// <summary>
		/// Called to handle the registration or deregistration of a type descriptor. Must return the type descriptor to be registered, or null to remove the registration.
		/// </summary>
		/// <param name="newDescriptor">The new descriptor, or null if this is a deregistration.</param>
		/// <param name="oldDescriptor">The old descriptor, or null if no descriptor was previously registered for this type.</param>
		/// <returns></returns>
		public IUserDataDescriptor HandleRegistration(IUserDataDescriptor newDescriptor, IUserDataDescriptor oldDescriptor)
		{
			if (newDescriptor == null)
				return null;
			else
				return oldDescriptor ?? newDescriptor;
		}

		/// <summary>
		/// Allows type automatic registration for the specified type.
		/// NOTE: automatic type registration is NOT recommended.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>
		/// True to register the type automatically, false otherwise.
		/// </returns>
		public virtual bool AllowTypeAutoRegistration(Type type)
		{
			return false;
		}
	}
}
