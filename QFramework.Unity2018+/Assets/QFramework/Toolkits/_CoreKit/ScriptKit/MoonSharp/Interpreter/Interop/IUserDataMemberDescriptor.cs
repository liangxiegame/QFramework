using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoonSharp.Interpreter.Interop
{
	/// <summary>
	/// Interface used by standard descriptors to access members of a given type from scripts.
	/// </summary>
	public interface IUserDataMemberDescriptor
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
		/// Gets the value of the member
		/// </summary>
		/// <param name="script">The script.</param>
		/// <param name="obj">The object.</param>
		/// <returns></returns>
		DynValue GetValue(Script script, object obj);
		/// <summary>
		/// Sets the value of the member
		/// </summary>
		/// <param name="script">The script.</param>
		/// <param name="obj">The object.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		bool SetValue(Script script, object obj, DynValue value);
		/// <summary>
		/// Gets the type of the member.
		/// </summary>
		/// <value>
		/// The type of the member.
		/// </value>
		UserDataMemberType MemberType { get; }


		void Optimize();



		bool IsStatic { get; }
	}
}
