using System;

namespace MoonSharp.Interpreter.Interop.BasicDescriptors
{
	/// <summary>
	/// Permissions for members access
	/// </summary>
	[Flags]
	public enum MemberDescriptorAccess
	{
		/// <summary>
		/// The member can be read from
		/// </summary>
		CanRead = 1,
		/// <summary>
		/// The member can be written to
		/// </summary>
		CanWrite = 2,
		/// <summary>
		/// The can be invoked
		/// </summary>
		CanExecute = 4
	}







}
