using System;
using MoonSharp.Interpreter.Interop.BasicDescriptors;
using MoonSharp.Interpreter.Interop.Converters;

namespace MoonSharp.Interpreter.Interop
{
	/// <summary>
	/// Member descriptor which allows to define new members which behave similarly to class instance members
	/// </summary>
	public class ObjectCallbackMemberDescriptor : FunctionMemberDescriptorBase
	{
		Func<object, ScriptExecutionContext, CallbackArguments, object> m_CallbackFunc;


		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectCallbackMemberDescriptor"/> class.
		/// A new member descriptor is defined, which is a function taking no parameters and returning void, doing nothing.
		/// </summary>
		/// <param name="funcName">Name of the function.</param>
		public ObjectCallbackMemberDescriptor(string funcName)
			: this(funcName, (o, c, a) => DynValue.Void, new ParameterDescriptor[0])
		{ }


		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectCallbackMemberDescriptor"/> class.
		/// The function described by descriptors created by this callback are defined as if they take no parameters
		/// and thus don't support overload resolution well (unless they really take no parameters) but can freely be
		/// used if no overload resolution is required.
		/// </summary>
		/// <param name="funcName">Name of the function.</param>
		/// <param name="callBack">The callback function.</param>
		public ObjectCallbackMemberDescriptor(string funcName, Func<object, ScriptExecutionContext, CallbackArguments, object> callBack)
			: this(funcName, callBack, new ParameterDescriptor[0])
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectCallbackMemberDescriptor"/> class.
		/// Members defined with this constructor will support overload resolution.
		/// </summary>
		/// <param name="funcName">Name of the function.</param>
		/// <param name="callBack">The call back.</param>
		/// <param name="parameters">The parameters.</param>
		public ObjectCallbackMemberDescriptor(string funcName, Func<object, ScriptExecutionContext, CallbackArguments, object> callBack, ParameterDescriptor[] parameters)
		{
			m_CallbackFunc = callBack;
			Initialize(funcName, false, parameters, false);
		}

		/// <summary>
		/// The internal callback which actually executes the method
		/// </summary>
		/// <param name="script">The script.</param>
		/// <param name="obj">The object.</param>
		/// <param name="context">The context.</param>
		/// <param name="args">The arguments.</param>
		/// <returns></returns>
		public override DynValue Execute(Script script, object obj, ScriptExecutionContext context, CallbackArguments args)
		{
			if (m_CallbackFunc != null)
			{
				object retv = m_CallbackFunc(obj, context, args);
				return ClrToScriptConversions.ObjectToDynValue(script, retv);
			}
			else
			{
				return DynValue.Void;
			}
		}
	}
}
