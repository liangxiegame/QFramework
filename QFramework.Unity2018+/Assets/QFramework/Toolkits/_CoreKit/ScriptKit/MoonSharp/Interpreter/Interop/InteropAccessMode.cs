
namespace MoonSharp.Interpreter
{
	/// <summary>
	/// Enumerations of the possible strategies to marshal CLR objects to MoonSharp userdata and functions
	/// when using automatic descriptors.
	/// Note that these are "hints" and MoonSharp is free to ignore the access mode specified (if different from
	/// HideMembers) and downgrade the access mode to "Reflection". 
	/// This particularly happens when running on AOT platforms like iOS.
	/// See also : <see cref="CallbackFunction"/> and <see cref="UserData"/> .
	/// </summary>
	public enum InteropAccessMode
	{
		/// <summary>
		/// Optimization is not performed and reflection is used everytime to access members.
		/// This is the slowest approach but saves a lot of memory if members are seldomly used.
		/// </summary>
		Reflection,
		/// <summary>
		/// Optimization is done on the fly the first time a member is accessed.
		/// This saves memory for all members that are never accessed, at the cost of an increased script execution time.
		/// </summary>
		LazyOptimized,
		/// <summary>
		/// Optimization is done at registration time.
		/// </summary>
		Preoptimized,
		/// <summary>
		/// Optimization is done in a background thread which starts at registration time. 
		/// If a member is accessed before optimization is completed, reflection is used.
		/// </summary>
		BackgroundOptimized,
		/// <summary>
		/// Use the hardwired descriptor(s)
		/// </summary>
		Hardwired,
		/// <summary>
		/// No optimization is done, and members are not accessible at all.
		/// </summary>
		HideMembers,
		/// <summary>
		/// No reflection is allowed, nor code generation. This is used as a safeguard when registering types which should not
		/// use a standard reflection based descriptor - for example for types implementing <see cref="MoonSharp.Interpreter.Interop.IUserDataType" />
		/// </summary>
		NoReflectionAllowed,
		/// <summary>
		/// Use the default access mode
		/// </summary>
		Default
	}
}
