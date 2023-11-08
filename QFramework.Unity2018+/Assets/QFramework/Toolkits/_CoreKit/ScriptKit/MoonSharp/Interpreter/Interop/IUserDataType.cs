
namespace MoonSharp.Interpreter.Interop
{
	/// <summary>
	/// As a convenience, every type deriving from IUserDataType is "self-described". That is, no descriptor is needed/generated
	/// and the object itself is used to describe the type for interop. See also <see cref="UserData"/>, <see cref="IUserDataDescriptor"/> 
	/// and <see cref="StandardUserDataDescriptor"/> .
	/// </summary>
	public interface IUserDataType
	{
		/// <summary>
		/// Performs an "index" "get" operation.
		/// </summary>
		/// <param name="script">The script originating the request</param>
		/// <param name="index">The index.</param>
		/// <param name="isDirectIndexing">If set to true, it's indexed with a name, if false it's indexed through brackets.</param>
		/// <returns></returns>
		DynValue Index(Script script, DynValue index, bool isDirectIndexing);
		/// <summary>
		/// Performs an "index" "set" operation.
		/// </summary>
		/// <param name="script">The script originating the request</param>
		/// <param name="index">The index.</param>
		/// <param name="value">The value to be set</param>
		/// <param name="isDirectIndexing">If set to true, it's indexed with a name, if false it's indexed through brackets.</param>
		/// <returns></returns>
		bool SetIndex(Script script, DynValue index, DynValue value, bool isDirectIndexing);
		/// <summary>
		/// 
		/// Gets a "meta" operation on this userdata. If a descriptor does not support this functionality,
		/// it should return "null" (not a nil). 
		/// 
		/// These standard metamethods can be supported (the return value should be a function accepting the
		/// classic parameters of the corresponding metamethod):
		/// __add, __sub, __mul, __div, __div, __pow, __unm, __eq, __lt, __le, __lt, __len, __concat, 
		/// __pairs, __ipairs, __iterator, __call
		/// 
		/// These standard metamethods are supported through other calls for efficiency:
		/// __index, __newindex, __tostring
		/// 
		/// </summary>
		/// <param name="script">The script originating the request</param>
		/// <param name="metaname">The name of the metamember.</param>
		/// <returns></returns>
		DynValue MetaIndex(Script script, string metaname);
	}
}
