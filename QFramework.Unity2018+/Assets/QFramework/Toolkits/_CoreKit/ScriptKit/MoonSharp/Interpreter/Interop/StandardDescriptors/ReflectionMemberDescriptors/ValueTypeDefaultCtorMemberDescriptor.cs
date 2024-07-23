using System;
using MoonSharp.Interpreter.Compatibility;
using MoonSharp.Interpreter.Interop.BasicDescriptors;
using MoonSharp.Interpreter.Interop.Converters;

namespace MoonSharp.Interpreter.Interop
{
	/// <summary>
	/// Member descriptor for the default constructor of value types.
	/// </summary>
	public class ValueTypeDefaultCtorMemberDescriptor : IOverloadableMemberDescriptor,
		IWireableDescriptor
	{
		/// <summary>
		/// Gets a value indicating whether the described method is static.
		/// </summary>
		public bool IsStatic { get { return true; } }
		/// <summary>
		/// Gets the name of the described method
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// This property is equal to the value type to be constructed.
		/// </summary>
		public Type ValueTypeDefaultCtor { get; private set; }

		/// <summary>
		/// Gets the type of the arguments of the underlying CLR function
		/// </summary>
		public ParameterDescriptor[] Parameters { get; private set; }


		/// <summary>
		/// Gets the type which this extension method extends, null if this is not an extension method.
		/// </summary>
		public Type ExtensionMethodType
		{
			get { return null; }
		}

		/// <summary>
		/// Gets a value indicating the type of the ParamArray parameter of a var-args function. If the function is not var-args,
		/// null is returned.
		/// </summary>
		public Type VarArgsArrayType
		{
			get { return null; }
		}

		/// <summary>
		/// Gets a value indicating the type of the elements of the ParamArray parameter of a var-args function. If the function is not var-args,
		/// null is returned.
		/// </summary>
		public Type VarArgsElementType
		{
			get { return null; }
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="MethodMemberDescriptor" /> class
		/// representing the default empty ctor for a value type.
		/// </summary>
		/// <param name="valueType">Type of the value.</param>
		/// <exception cref="System.ArgumentException">valueType is not a value type</exception>
		public ValueTypeDefaultCtorMemberDescriptor(Type valueType)
		{
			if (!Framework.Do.IsValueType(valueType))
				throw new ArgumentException("valueType is not a value type");

			this.Name = "__new";
			this.Parameters = new ParameterDescriptor[0];

			ValueTypeDefaultCtor = valueType;
		}


		/// <summary>
		/// Invokes the member from script.
		/// Implementors should raise exceptions if the value cannot be executed or if access to an
		/// instance member through a static userdata is attempted.
		/// </summary>
		/// <param name="script">The script.</param>
		/// <param name="obj">The object.</param>
		/// <param name="context">The context.</param>
		/// <param name="args">The arguments.</param>
		/// <returns></returns>
		public DynValue Execute(Script script, object obj, ScriptExecutionContext context, CallbackArguments args)
		{
			this.CheckAccess(MemberDescriptorAccess.CanRead, obj);

			object vto = Activator.CreateInstance(ValueTypeDefaultCtor);
			return ClrToScriptConversions.ObjectToDynValue(script, vto);
		}


		/// <summary>
		/// Gets a sort discriminant to give consistent overload resolution matching in case of perfectly equal scores
		/// </summary>
		public string SortDiscriminant
		{
			get { return "@.ctor"; }
		}


		/// <summary>
		/// Gets the types of access supported by this member
		/// </summary>
		public MemberDescriptorAccess MemberAccess
		{
			get { return MemberDescriptorAccess.CanRead | MemberDescriptorAccess.CanExecute; }
		}

		/// <summary>
		/// Gets the value of this member as a 
		/// <see cref="DynValue" /> to be exposed to scripts.
		/// Implementors should raise exceptions if the value cannot be read or if access to an
		/// instance member through a static userdata is attempted.
		/// </summary>
		/// <param name="script">The script.</param>
		/// <param name="obj">The object owning this member, or null if static.</param>
		/// <returns>
		/// The value of this member as a <see cref="DynValue" />.
		/// </returns>
		public DynValue GetValue(Script script, object obj)
		{
			this.CheckAccess(MemberDescriptorAccess.CanRead, obj);

			object vto = Activator.CreateInstance(ValueTypeDefaultCtor);
			return ClrToScriptConversions.ObjectToDynValue(script, vto);
		}

		/// <summary>
		/// Sets the value of this member from a 
		/// <see cref="DynValue" />.
		/// Implementors should raise exceptions if the value cannot be read or if access to an
		/// instance member through a static userdata is attempted.
		/// </summary>
		/// <param name="script">The script.</param>
		/// <param name="obj">The object owning this member, or null if static.</param>
		/// <param name="value">The value to be set.</param>
		public void SetValue(Script script, object obj, DynValue value)
		{
			this.CheckAccess(MemberDescriptorAccess.CanWrite, obj);
		}

		/// <summary>
		/// Prepares the descriptor for hard-wiring.
		/// The descriptor fills the passed table with all the needed data for hardwire generators to generate the appropriate code.
		/// </summary>
		/// <param name="t">The table to be filled</param>
		public void PrepareForWiring(Table t)
		{
			t.Set("class", DynValue.NewString(this.GetType().FullName));
			t.Set("type", DynValue.NewString(this.ValueTypeDefaultCtor.FullName));
			t.Set("name", DynValue.NewString(this.Name));
		}
	}
}
