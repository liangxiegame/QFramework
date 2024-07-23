
namespace MoonSharp.Interpreter.Interop.BasicDescriptors
{
	/// <summary>
	/// Base interface to describe access to members of a given type.
	/// While it's not infrastructural to implement custom type descriptors, it's needed for 
	/// classes extending <see cref="DispatchingUserDataDescriptor"/>.
	/// </summary>
	public interface IMemberDescriptor
	{
		/// <summary>
		/// Gets a value indicating whether the described member is static.
		/// </summary>
		bool IsStatic { get; }
		/// <summary>
		/// Gets the name of the member
		/// </summary>
		string Name { get; }
		/// <summary>
		/// Gets the types of access supported by this member
		/// </summary>
		MemberDescriptorAccess MemberAccess { get; }
		/// <summary>
		/// Gets the value of this member as a <see cref="DynValue"/> to be exposed to scripts.
		/// Implementors should raise exceptions if the value cannot be read or if access to an
		/// instance member through a static userdata is attempted.
		/// </summary>
		/// <param name="script">The script.</param>
		/// <param name="obj">The object owning this member, or null if static.</param>
		/// <returns>The value of this member as a <see cref="DynValue"/>.</returns>
		DynValue GetValue(Script script, object obj);
		/// <summary>
		/// Sets the value of this member from a <see cref="DynValue"/>.
		/// Implementors should raise exceptions if the value cannot be read or if access to an
		/// instance member through a static userdata is attempted.
		/// </summary>
		/// <param name="script">The script.</param>
		/// <param name="obj">The object owning this member, or null if static.</param>
		/// <param name="value">The value to be set.</param>
		void SetValue(Script script, object obj, DynValue value);
	}


	/// <summary>
	/// Extension methods for <see cref="IMemberDescriptor" /> and <see cref="MemberDescriptorAccess"/> .
	/// </summary>
	public static class MemberDescriptor
	{
		/// <summary>
		/// Determines whether the specified MemberDescriptorAccess has ALL the specified flags.
		/// </summary>
		/// <param name="access">The access.</param>
		/// <param name="flag">The flag.</param>
		/// <returns></returns>
		public static bool HasAllFlags(this MemberDescriptorAccess access, MemberDescriptorAccess flag)
		{
			return (access & flag) == flag;
		}

		/// <summary>
		/// Determines whether this instance can be read
		/// </summary>
		/// <param name="desc">The descriptor instance.</param>
		/// <returns></returns>
		public static bool CanRead(this IMemberDescriptor desc)
		{
			return desc.MemberAccess.HasAllFlags(MemberDescriptorAccess.CanRead);
		}

		/// <summary>
		/// Determines whether this instance can be written to
		/// </summary>
		/// <param name="desc">The descriptor instance.</param>
		/// <returns></returns>
		public static bool CanWrite(this IMemberDescriptor desc)
		{
			return desc.MemberAccess.HasAllFlags(MemberDescriptorAccess.CanWrite);
		}

		/// <summary>
		/// Determines whether this instance can be executed (called as a function)
		/// </summary>
		/// <param name="desc">The descriptor instance.</param>
		/// <returns></returns>
		public static bool CanExecute(this IMemberDescriptor desc)
		{
			return desc.MemberAccess.HasAllFlags(MemberDescriptorAccess.CanExecute);
		}

		/// <summary>
		/// Gets the getter of the member as a DynValue containing a callback
		/// </summary>
		/// <param name="desc">The descriptor instance.</param>
		/// <param name="script">The script.</param>
		/// <param name="obj">The object.</param>
		/// <returns></returns>
		public static DynValue GetGetterCallbackAsDynValue(this IMemberDescriptor desc, Script script, object obj)
		{
			return DynValue.NewCallback((p1, p2) => desc.GetValue(script, obj));
		}

		/// <summary>
		/// Returns the specified descriptor if it supports all the specified access modes, otherwise returns null.
		/// </summary>
		/// <param name="desc">The descriptor instance.</param>
		/// <param name="access">The access mode(s).</param>
		/// <returns></returns>
		public static IMemberDescriptor WithAccessOrNull(this IMemberDescriptor desc, MemberDescriptorAccess access)
		{
			if (desc == null)
				return null;

			if (desc.MemberAccess.HasAllFlags(access))
				return desc;

			return null;
		}

		/// <summary>
		/// Raises an appropriate ScriptRuntimeException if the specified access is not supported.
		/// Checks are made for the MemberDescriptorAccess permissions AND for the access of instance
		/// members through static userdatas.
		/// </summary>
		/// <param name="desc">The desc.</param>
		/// <param name="access">The access.</param>
		/// <param name="obj">The object to be checked for access.</param>
		public static void CheckAccess(this IMemberDescriptor desc, MemberDescriptorAccess access, object obj)
		{
			if (!desc.IsStatic && obj == null)
				throw ScriptRuntimeException.AccessInstanceMemberOnStatics(desc);

			if (access.HasAllFlags(MemberDescriptorAccess.CanExecute) && !desc.CanExecute())
				throw new ScriptRuntimeException("userdata member {0} cannot be called.", desc.Name);

			if (access.HasAllFlags(MemberDescriptorAccess.CanWrite) && !desc.CanWrite())
				throw new ScriptRuntimeException("userdata member {0} cannot be assigned to.", desc.Name);

			if (access.HasAllFlags(MemberDescriptorAccess.CanRead) && !desc.CanRead())
				throw new ScriptRuntimeException("userdata member {0} cannot be read from.", desc.Name);
		}




	}

}
