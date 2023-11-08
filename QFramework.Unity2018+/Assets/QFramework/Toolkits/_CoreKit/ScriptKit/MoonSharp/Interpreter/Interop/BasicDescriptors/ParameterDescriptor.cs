using System;
using System.Linq;
using System.Reflection;
using MoonSharp.Interpreter.Compatibility;

namespace MoonSharp.Interpreter.Interop.BasicDescriptors
{
	/// <summary>
	/// Descriptor of parameters used in <see cref="IOverloadableMemberDescriptor"/> implementations.
	/// </summary>
	public sealed class ParameterDescriptor : IWireableDescriptor
	{
		/// <summary>
		/// Gets the name of the parameter
		/// </summary>
		public string Name { get; private set; }
		/// <summary>
		/// Gets the type of the parameter
		/// </summary>
		public Type Type { get; private set; }
		/// <summary>
		/// Gets a value indicating whether this instance has a default value.
		/// </summary>
		public bool HasDefaultValue { get; private set; }
		/// <summary>
		/// Gets the default value
		/// </summary>
		public object DefaultValue { get; private set; }
		/// <summary>
		/// Gets a value indicating whether this instance is an out parameter
		/// </summary>
		public bool IsOut { get; private set; }
		/// <summary>
		/// Gets a value indicating whether this instance is a "ref" parameter
		/// </summary>
		public bool IsRef { get; private set; }
		/// <summary>
		/// Gets a value indicating whether this instance is a variable arguments param
		/// </summary>
		public bool IsVarArgs { get; private set; }
		/// <summary>
		/// Gets a value indicating whether this instance has been restricted.
		/// </summary>
		public bool HasBeenRestricted { get { return m_OriginalType != null; } }
		/// <summary>
		/// Gets the original type of the parameter before any restriction has been applied.
		/// </summary>
		public Type OriginalType { get { return m_OriginalType ?? Type; } }


		/// <summary>
		/// If the type got restricted, the original type before the restriction.
		/// </summary>
		private Type m_OriginalType = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterDescriptor" /> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="type">The type.</param>
		/// <param name="hasDefaultValue">if set to <c>true</c> the parameter has default value.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <param name="isOut">if set to <c>true</c>, is an out param.</param>
		/// <param name="isRef">if set to <c>true</c> is a ref param.</param>
		/// <param name="isVarArgs">if set to <c>true</c> is variable arguments param.</param>
		public ParameterDescriptor(string name, Type type, bool hasDefaultValue = false, object defaultValue = null, bool isOut = false,
			bool isRef = false, bool isVarArgs = false)
		{
			Name = name;
			Type = type;
			HasDefaultValue = hasDefaultValue;
			DefaultValue = defaultValue;
			IsOut = isOut;
			IsRef = isRef;
			IsVarArgs = isVarArgs;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterDescriptor" /> class. 
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="type">The type.</param>
		/// <param name="hasDefaultValue">if set to <c>true</c> the parameter has default value.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <param name="isOut">if set to <c>true</c>, is an out param.</param>
		/// <param name="isRef">if set to <c>true</c> is a ref param.</param>
		/// <param name="isVarArgs">if set to <c>true</c> is variable arguments param.</param>
		/// <param name="typeRestriction">The type restriction, or nll.</param>
		public ParameterDescriptor(string name, Type type, bool hasDefaultValue, object defaultValue, bool isOut,
			bool isRef, bool isVarArgs, Type typeRestriction)
		{
			Name = name;
			Type = type;
			HasDefaultValue = hasDefaultValue;
			DefaultValue = defaultValue;
			IsOut = isOut;
			IsRef = isRef;
			IsVarArgs = isVarArgs;

			if (typeRestriction != null)
			{
				RestrictType(typeRestriction);
			}
		}
		

		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterDescriptor"/> class.
		/// </summary>
		/// <param name="pi">A ParameterInfo taken from reflection.</param>
		public ParameterDescriptor(ParameterInfo pi)
		{
			Name = pi.Name;
			Type = pi.ParameterType;
			HasDefaultValue = !(Framework.Do.IsDbNull(pi.DefaultValue));
			DefaultValue = pi.DefaultValue;
			IsOut = pi.IsOut;
			IsRef = pi.ParameterType.IsByRef;
			IsVarArgs = (pi.ParameterType.IsArray && pi.GetCustomAttributes(typeof(ParamArrayAttribute), true).Any());
		}


		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String" /> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return string.Format("{0} {1}{2}", Type.Name, Name, HasDefaultValue ? " = ..." : "");
		}

		/// <summary>
		/// Restricts the type of this parameter to a tighter constraint.
		/// Restrictions must be applied before the <see cref="IOverloadableMemberDescriptor"/> containing this
		/// parameter is used in any way.
		/// </summary>
		/// <param name="type">The new type.</param>
		/// <exception cref="System.InvalidOperationException">
		/// Cannot restrict a ref/out or varargs param
		/// or
		/// Specified operation is not a restriction
		/// </exception>
		public void RestrictType(Type type)
		{
			if (IsOut || IsRef || IsVarArgs)
				throw new InvalidOperationException("Cannot restrict a ref/out or varargs param");

			if (!Framework.Do.IsAssignableFrom(Type, type))
				throw new InvalidOperationException("Specified operation is not a restriction");

			m_OriginalType = Type;
			Type = type;
		}


		/// <summary>
		/// Prepares the descriptor for hard-wiring.
		/// The descriptor fills the passed table with all the needed data for hardwire generators to generate the appropriate code.
		/// </summary>
		/// <param name="t">The table to be filled</param>
		public void PrepareForWiring(Table table)
		{
			table.Set("name", DynValue.NewString(Name));

			if (Type.IsByRef)
				table.Set("type", DynValue.NewString(Type.GetElementType().FullName));
			else
				table.Set("type", DynValue.NewString(Type.FullName));

			if (OriginalType.IsByRef)
				table.Set("origtype", DynValue.NewString(OriginalType.GetElementType().FullName));
			else
				table.Set("origtype", DynValue.NewString(OriginalType.FullName));

			table.Set("default", DynValue.NewBoolean(HasDefaultValue));
			table.Set("out", DynValue.NewBoolean(IsOut));
			table.Set("ref", DynValue.NewBoolean(IsRef));
			table.Set("varargs", DynValue.NewBoolean(IsVarArgs));
			table.Set("restricted", DynValue.NewBoolean(HasBeenRestricted));
		}
	}
}
