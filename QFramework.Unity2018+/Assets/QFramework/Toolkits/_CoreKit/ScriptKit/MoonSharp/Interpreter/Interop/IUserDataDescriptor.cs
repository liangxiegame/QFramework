using System;

namespace MoonSharp.Interpreter.Interop
{
	/// <summary>
	/// Interface used by MoonSharp to access objects of a given type from scripts.
	/// </summary>
	public interface IUserDataDescriptor
	{
		/// <summary>
		/// Gets the name of the descriptor (usually, the name of the type described).
		/// </summary>
		string Name { get; }
		/// <summary>
		/// Gets the type this descriptor refers to
		/// </summary>
		Type Type { get; }
		/// <summary>
		/// Performs an "index" "get" operation.
		/// </summary>
		/// <param name="script">The script originating the request</param>
		/// <param name="obj">The object (null if a static request is done)</param>
		/// <param name="index">The index.</param>
		/// <param name="isDirectIndexing">If set to true, it's indexed with a name, if false it's indexed through brackets.</param>
		/// <returns></returns>
		DynValue Index(Script script, object obj, DynValue index, bool isDirectIndexing);
		/// <summary>
		/// Performs an "index" "set" operation.
		/// </summary>
		/// <param name="script">The script originating the request</param>
		/// <param name="obj">The object (null if a static request is done)</param>
		/// <param name="index">The index.</param>
		/// <param name="value">The value to be set</param>
		/// <param name="isDirectIndexing">If set to true, it's indexed with a name, if false it's indexed through brackets.</param>
		/// <returns></returns>
		bool SetIndex(Script script, object obj, DynValue index, DynValue value, bool isDirectIndexing);
		/// <summary>
		/// Converts this userdata to string
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <returns></returns>
		string AsString(object obj);
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
		/// <param name="obj">The object (null if a static request is done)</param>
		/// <param name="metaname">The name of the metamember.</param>
		/// <returns></returns>
		DynValue MetaIndex(Script script, object obj, string metaname);
		/// <summary>
		/// Determines whether the specified object is compatible with the specified type.
		/// Unless a very specific behaviour is needed, the correct implementation is a 
		/// simple " return type.IsInstanceOfType(obj); "
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="obj">The object.</param>
		/// <returns></returns>
		bool IsTypeCompatible(Type type, object obj);
	}
}
