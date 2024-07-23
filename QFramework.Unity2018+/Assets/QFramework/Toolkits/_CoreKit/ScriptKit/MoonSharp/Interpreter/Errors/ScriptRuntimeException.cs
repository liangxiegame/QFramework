using System;
using MoonSharp.Interpreter.Interop;
using MoonSharp.Interpreter.Interop.BasicDescriptors;

namespace MoonSharp.Interpreter
{
	/// <summary>
	/// Exception for all runtime errors. In addition to constructors, it offers a lot of static methods
	/// generating more "standard" Lua errors.
	/// </summary>
#if !(PCL || ((!UNITY_EDITOR) && (ENABLE_DOTNET)) || NETFX_CORE)
	[Serializable]
#endif
	public class ScriptRuntimeException : InterpreterException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ScriptRuntimeException"/> class.
		/// </summary>
		/// <param name="ex">The ex.</param>
		public ScriptRuntimeException(Exception ex)
			: base(ex)
		{
		}       

		/// <summary>
		/// Initializes a new instance of the <see cref="ScriptRuntimeException"/> class.
		/// </summary>
		/// <param name="ex">The ex.</param>
		public ScriptRuntimeException(ScriptRuntimeException ex)
			: base(ex, ex.DecoratedMessage)
		{
			this.DecoratedMessage = Message;
			this.DoNotDecorateMessage = true;
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="ScriptRuntimeException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public ScriptRuntimeException(string message)
			: base(message)
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ScriptRuntimeException"/> class.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <param name="args">The arguments.</param>
		public ScriptRuntimeException(string format, params object[] args)
			: base(format, args)
		{

		}

		/// <summary>
		/// Creates a ScriptRuntimeException with a predefined error message specifying that
		/// an arithmetic operation was attempted on non-numbers
		/// </summary>
		/// <param name="l">The left operand.</param>
		/// <param name="r">The right operand (or null).</param>
		/// <returns>The exception to be raised.</returns>
		/// <exception cref="InternalErrorException">If both are numbers</exception>
		public static ScriptRuntimeException ArithmeticOnNonNumber(DynValue l, DynValue r = null)
		{
			if (l.Type != DataType.Number && l.Type != DataType.String)
				return new ScriptRuntimeException("attempt to perform arithmetic on a {0} value", l.Type.ToLuaTypeString());
			else if (r != null && r.Type != DataType.Number && r.Type != DataType.String)
				return new ScriptRuntimeException("attempt to perform arithmetic on a {0} value", r.Type.ToLuaTypeString());
			else if (l.Type == DataType.String || (r != null && r.Type == DataType.String))
				return new ScriptRuntimeException("attempt to perform arithmetic on a string value");
			else
				throw new InternalErrorException("ArithmeticOnNonNumber - both are numbers");
		}

		/// <summary>
		/// Creates a ScriptRuntimeException with a predefined error message specifying that
		/// a concat operation was attempted on non-strings
		/// </summary>
		/// <param name="l">The left operand.</param>
		/// <param name="r">The right operand.</param>
		/// <returns>The exception to be raised.</returns>
		/// <exception cref="InternalErrorException">If both are numbers or strings</exception>
		public static ScriptRuntimeException ConcatOnNonString(DynValue l, DynValue r)
		{
			if (l.Type != DataType.Number && l.Type != DataType.String)
				return new ScriptRuntimeException("attempt to concatenate a {0} value", l.Type.ToLuaTypeString());
			else if (r != null && r.Type != DataType.Number && r.Type != DataType.String)
				return new ScriptRuntimeException("attempt to concatenate a {0} value", r.Type.ToLuaTypeString());
			else
				throw new InternalErrorException("ConcatOnNonString - both are numbers/strings");
		}

		/// <summary>
		/// Creates a ScriptRuntimeException with a predefined error message specifying that
		/// a len operator was applied on an invalid operand
		/// </summary>
		/// <param name="r">The operand.</param>
		/// <returns>The exception to be raised.</returns>
		public static ScriptRuntimeException LenOnInvalidType(DynValue r)
		{
			return new ScriptRuntimeException("attempt to get length of a {0} value", r.Type.ToLuaTypeString());
		}

		/// <summary>
		/// Creates a ScriptRuntimeException with a predefined error message specifying that
		/// a comparison operator was applied on an invalid combination of operand types
		/// </summary>
		/// <param name="l">The left operand.</param>
		/// <param name="r">The right operand.</param>
		/// <returns>The exception to be raised.</returns>
		public static ScriptRuntimeException CompareInvalidType(DynValue l, DynValue r)
		{
			if (l.Type.ToLuaTypeString() == r.Type.ToLuaTypeString())
				return new ScriptRuntimeException("attempt to compare two {0} values", l.Type.ToLuaTypeString());
			else
				return new ScriptRuntimeException("attempt to compare {0} with {1}", l.Type.ToLuaTypeString(), r.Type.ToLuaTypeString());
		}

		/// <summary>
		/// Creates a ScriptRuntimeException with a predefined error message specifying that
		/// a function was called with a bad argument
		/// </summary>
		/// <param name="argNum">The argument number (0-based).</param>
		/// <param name="funcName">Name of the function generating this error.</param>
		/// <param name="message">The error message.</param>
		/// <returns>The exception to be raised.</returns>
		public static ScriptRuntimeException BadArgument(int argNum, string funcName, string message)
		{
			return new ScriptRuntimeException("bad argument #{0} to '{1}' ({2})", argNum + 1, funcName, message);
		}

		/// <summary>
		/// Creates a ScriptRuntimeException with a predefined error message specifying that
		/// a function was called with a bad userdata argument
		/// </summary>
		/// <param name="argNum">The argument number (0-based).</param>
		/// <param name="funcName">Name of the function generating this error.</param>
		/// <param name="expected">The expected System.Type.</param>
		/// <param name="got">The object which was used.</param>
		/// <param name="allowNil">True if nils were allowed in this call.</param>
		/// <returns>
		/// The exception to be raised.
		/// </returns>
		public static ScriptRuntimeException BadArgumentUserData(int argNum, string funcName, Type expected, object got, bool allowNil)
		{
			return new ScriptRuntimeException("bad argument #{0} to '{1}' (userdata<{2}>{3} expected, got {4})", 
				argNum + 1, 
				funcName,
				expected.Name,
				allowNil ? "nil or " : "",
				got != null ? "userdata<" + got.GetType().Name + ">" : "null"
				);
		}

		/// <summary>
		/// Creates a ScriptRuntimeException with a predefined error message specifying that
		/// a function was called with a bad argument
		/// </summary>
		/// <param name="argNum">The argument number (0-based).</param>
		/// <param name="funcName">Name of the function generating this error.</param>
		/// <param name="expected">The expected data type.</param>
		/// <param name="got">The data type received.</param>
		/// <param name="allowNil">True if nils were allowed in this call.</param>
		/// <returns>
		/// The exception to be raised.
		/// </returns>
		public static ScriptRuntimeException BadArgument(int argNum, string funcName, DataType expected, DataType got, bool allowNil)
		{
			return BadArgument(argNum, funcName, expected.ToErrorTypeString(), got.ToErrorTypeString(), allowNil);
		}

		/// <summary>
		/// Creates a ScriptRuntimeException with a predefined error message specifying that
		/// a function was called with a bad argument
		/// </summary>
		/// <param name="argNum">The argument number (0-based).</param>
		/// <param name="funcName">Name of the function generating this error.</param>
		/// <param name="expected">The expected type description.</param>
		/// <param name="got">The description of the type received.</param>
		/// <param name="allowNil">True if nils were allowed in this call.</param>
		/// <returns>
		/// The exception to be raised.
		/// </returns>
		public static ScriptRuntimeException BadArgument(int argNum, string funcName, string expected, string got, bool allowNil)
		{
			return new ScriptRuntimeException("bad argument #{0} to '{1}' ({2}{3} expected, got {4})",
				argNum + 1, funcName, allowNil ? "nil or " : "", expected, got);
		}

		/// <summary>
		/// Creates a ScriptRuntimeException with a predefined error message specifying that
		/// a function was called with no value when a value was required.
		/// 
		/// This function creates a message like "bad argument #xxx to 'yyy' (zzz expected, got no value)"
		/// while <see cref="BadArgumentValueExpected" /> creates a message like "bad argument #xxx to 'yyy' (value expected)"
		/// </summary>
		/// <param name="argNum">The argument number (0-based).</param>
		/// <param name="funcName">Name of the function generating this error.</param>
		/// <param name="expected">The expected data type.</param>
		/// <returns>
		/// The exception to be raised.
		/// </returns>
		public static ScriptRuntimeException BadArgumentNoValue(int argNum, string funcName, DataType expected)
		{
			return new ScriptRuntimeException("bad argument #{0} to '{1}' ({2} expected, got no value)",
				argNum + 1, funcName, expected.ToErrorTypeString());
		}

		/// <summary>
		/// Creates a ScriptRuntimeException with a predefined error message specifying that
		/// an out of range index was specified
		/// </summary>
		/// <param name="argNum">The argument number (0-based).</param>
		/// <param name="funcName">Name of the function generating this error.</param>
		/// <returns>
		/// The exception to be raised.
		/// </returns>
		public static ScriptRuntimeException BadArgumentIndexOutOfRange(string funcName, int argNum)
		{
			return new ScriptRuntimeException("bad argument #{0} to '{1}' (index out of range)", argNum + 1, funcName);
		}

		/// <summary>
		/// Creates a ScriptRuntimeException with a predefined error message specifying that
		/// a function was called with a negative number when a positive one was expected.
		/// </summary>
		/// <param name="argNum">The argument number (0-based).</param>
		/// <param name="funcName">Name of the function generating this error.</param>
		/// <returns>
		/// The exception to be raised.
		/// </returns>
		public static ScriptRuntimeException BadArgumentNoNegativeNumbers(int argNum, string funcName)
		{
			return new ScriptRuntimeException("bad argument #{0} to '{1}' (not a non-negative number in proper range)",
				argNum + 1, funcName);
		}

		/// <summary>
		/// Creates a ScriptRuntimeException with a predefined error message specifying that
		/// a function was called with no value when a value was required.
		/// This function creates a message like "bad argument #xxx to 'yyy' (value expected)"
		/// while <see cref="BadArgumentNoValue" /> creates a message like "bad argument #xxx to 'yyy' (zzz expected, got no value)"
		/// </summary>
		/// <param name="argNum">The argument number (0-based).</param>
		/// <param name="funcName">Name of the function generating this error.</param>
		/// <returns>
		/// The exception to be raised.
		/// </returns>
		public static ScriptRuntimeException BadArgumentValueExpected(int argNum, string funcName)
		{
			return new ScriptRuntimeException("bad argument #{0} to '{1}' (value expected)",
				argNum + 1, funcName);
		}


		/// <summary>
		/// Creates a ScriptRuntimeException with a predefined error message specifying that
		/// an invalid attempt to index the specified object was made
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <returns>
		/// The exception to be raised.
		/// </returns>
		public static ScriptRuntimeException IndexType(DynValue obj)
		{
			return new ScriptRuntimeException("attempt to index a {0} value", obj.Type.ToLuaTypeString());
		}

		/// <summary>
		/// Creates a ScriptRuntimeException with a predefined error message specifying that
		/// a loop was detected when performing __index over metatables.
		/// </summary>
		/// <returns>
		/// The exception to be raised.
		/// </returns>
		public static ScriptRuntimeException LoopInIndex()
		{
			return new ScriptRuntimeException("loop in gettable");
		}

		/// <summary>
		/// Creates a ScriptRuntimeException with a predefined error message specifying that
		/// a loop was detected when performing __newindex over metatables.
		/// </summary>
		/// <returns>
		/// The exception to be raised.
		/// </returns>
		public static ScriptRuntimeException LoopInNewIndex()
		{
			return new ScriptRuntimeException("loop in settable");
		}

		/// <summary>
		/// Creates a ScriptRuntimeException with a predefined error message specifying that
		/// a loop was detected when performing __call over metatables.
		/// </summary>
		/// <returns>
		/// The exception to be raised.
		/// </returns>
		public static ScriptRuntimeException LoopInCall()
		{
			return new ScriptRuntimeException("loop in call");
		}

		/// <summary>
		/// Creates a ScriptRuntimeException with a predefined error message specifying that
		/// a table indexing operation used nil as the key.
		/// </summary>
		/// <returns>
		/// The exception to be raised.
		/// </returns>
		public static ScriptRuntimeException TableIndexIsNil()
		{
			return new ScriptRuntimeException("table index is nil");
		}

		/// <summary>
		/// Creates a ScriptRuntimeException with a predefined error message specifying that
		/// a table indexing operation used a NaN as the key.
		/// </summary>
		/// <returns>
		/// The exception to be raised.
		/// </returns>
		public static ScriptRuntimeException TableIndexIsNaN()
		{
			return new ScriptRuntimeException("table index is NaN");
		}

		/// <summary>
		/// Creates a ScriptRuntimeException with a predefined error message specifying that
		/// a conversion to number failed.
		/// </summary>
		/// <param name="stage">
		/// Selects the correct error message:
		/// 0 - "value must be a number"
		/// 1 - "'for' initial value must be a number"
		/// 2 - "'for' step must be a number"
		/// 3 - "'for' limit must be a number"
		/// </param>
		/// <returns>
		/// The exception to be raised.
		/// </returns>
		public static ScriptRuntimeException ConvertToNumberFailed(int stage)
		{
			switch (stage)
			{
				case 1:
					return new ScriptRuntimeException("'for' initial value must be a number");
				case 2:
					return new ScriptRuntimeException("'for' step must be a number");
				case 3:
					return new ScriptRuntimeException("'for' limit must be a number");
				default:
					return new ScriptRuntimeException("value must be a number");
			}
		}

		/// <summary>
		/// Creates a ScriptRuntimeException with a predefined error message specifying that
		/// a conversion of a CLR type to a Lua type has failed.
		/// </summary>
		/// <param name="obj">The object which could not be converted.</param>
		/// <returns>
		/// The exception to be raised.
		/// </returns>
		public static ScriptRuntimeException ConvertObjectFailed(object obj)
		{
			return new ScriptRuntimeException("cannot convert clr type {0}", obj.GetType());
		}

		/// <summary>
		/// Creates a ScriptRuntimeException with a predefined error message specifying that
		/// a conversion of a Lua type to a CLR type has failed.
		/// </summary>
		/// <param name="t">The Lua type.</param>
		/// <returns>
		/// The exception to be raised.
		/// </returns>
		public static ScriptRuntimeException ConvertObjectFailed(DataType t)
		{
			return new ScriptRuntimeException("cannot convert a {0} to a clr type", t.ToString().ToLowerInvariant());
		}

		/// <summary>
		/// Creates a ScriptRuntimeException with a predefined error message specifying that
		/// a constrained conversion of a Lua type to a CLR type has failed.
		/// </summary>
		/// <param name="t">The Lua type.</param>
		/// <param name="t2">The expected CLR type.</param>
		/// <returns>
		/// The exception to be raised.
		/// </returns>
		public static ScriptRuntimeException ConvertObjectFailed(DataType t, Type t2)
		{
			return new ScriptRuntimeException("cannot convert a {0} to a clr type {1}", t.ToString().ToLowerInvariant(), t2.FullName);
		}

		/// <summary>
		/// Creates a ScriptRuntimeException with a predefined error message specifying that
		/// a userdata of a specific CLR type was expected and a non-userdata type was passed.
		/// </summary>
		/// <param name="t">The Lua type.</param>
		/// <param name="clrType">The expected CLR type.</param>
		/// <returns>
		/// The exception to be raised.
		/// </returns>
		public static ScriptRuntimeException UserDataArgumentTypeMismatch(DataType t, Type clrType)
		{
			return new ScriptRuntimeException("cannot find a conversion from a MoonSharp {0} to a clr {1}", t.ToString().ToLowerInvariant(), clrType.FullName);
		}

		/// <summary>
		/// Creates a ScriptRuntimeException with a predefined error message specifying that
		/// an attempt to index an invalid member of a userdata was done.
		/// </summary>
		/// <param name="typename">The name of the userdata type.</param>
		/// <param name="fieldname">The field name.</param>
		/// <returns>
		/// The exception to be raised.
		/// </returns>
		public static ScriptRuntimeException UserDataMissingField(string typename, string fieldname)
		{
			return new ScriptRuntimeException("cannot access field {0} of userdata<{1}>", fieldname, typename);
		}

		/// <summary>
		/// Creates a ScriptRuntimeException with a predefined error message specifying that
		/// an attempt resume a coroutine in an invalid state was done.
		/// </summary>
		/// <param name="state">The state of the coroutine.</param>
		/// <returns>
		/// The exception to be raised.
		/// </returns>
		public static ScriptRuntimeException CannotResumeNotSuspended(CoroutineState state)
		{
			if (state == CoroutineState.Dead)
				return new ScriptRuntimeException("cannot resume dead coroutine");
			else
				return new ScriptRuntimeException("cannot resume non-suspended coroutine");
		}

		/// <summary>
		/// Creates a ScriptRuntimeException with a predefined error message specifying that
		/// an attempt to yield across a CLR boundary was made.
		/// </summary>
		/// <returns>
		/// The exception to be raised.
		/// </returns>
		public static ScriptRuntimeException CannotYield()
		{
			return new ScriptRuntimeException("attempt to yield across a CLR-call boundary");
		}

		/// <summary>
		/// Creates a ScriptRuntimeException with a predefined error message specifying that
		/// an attempt to yield from the main coroutine was made.
		/// </summary>
		/// <returns>
		/// The exception to be raised.
		/// </returns>
		public static ScriptRuntimeException CannotYieldMain()
		{
			return new ScriptRuntimeException("attempt to yield from outside a coroutine");
		}

		/// <summary>
		/// Creates a ScriptRuntimeException with a predefined error message specifying that
		/// an attempt to call a non-function was made
		/// </summary>
		/// <param name="type">The lua non-function data type.</param>
		/// <param name="debugText">The debug text to aid location (appears as "near 'xxx'").</param>
		/// <returns></returns>
		public static ScriptRuntimeException AttemptToCallNonFunc(DataType type, string debugText = null)
		{
			string functype = type.ToErrorTypeString();

			if (debugText != null)
				return new ScriptRuntimeException("attempt to call a {0} value near '{1}'", functype, debugText);
			else
				return new ScriptRuntimeException("attempt to call a {0} value", functype);
		}


		/// <summary>
		/// Creates a ScriptRuntimeException with a predefined error message specifying that
		/// an attempt to access a non-static member from a static userdata was made
		/// </summary>
		/// <param name="desc">The member descriptor.</param>
		public static ScriptRuntimeException AccessInstanceMemberOnStatics(IMemberDescriptor desc)
		{
			return new ScriptRuntimeException("attempt to access instance member {0} from a static userdata", desc.Name);
		}

		/// <summary>
		/// Creates a ScriptRuntimeException with a predefined error message specifying that
		/// an attempt to access a non-static member from a static userdata was made
		/// </summary>
		/// <param name="typeDescr">The type descriptor.</param>
		/// <param name="desc">The member descriptor.</param>
		/// <returns></returns>
		public static ScriptRuntimeException AccessInstanceMemberOnStatics(IUserDataDescriptor typeDescr, IMemberDescriptor desc)
		{
			return new ScriptRuntimeException("attempt to access instance member {0}.{1} from a static userdata", typeDescr.Name, desc.Name);
		}

		/// <summary>
		/// Rethrows this instance if 
		/// </summary>
		/// <returns></returns>
		public override void Rethrow()
		{
			if (Script.GlobalOptions.RethrowExceptionNested)
				throw new ScriptRuntimeException(this);
		}

	}
}
