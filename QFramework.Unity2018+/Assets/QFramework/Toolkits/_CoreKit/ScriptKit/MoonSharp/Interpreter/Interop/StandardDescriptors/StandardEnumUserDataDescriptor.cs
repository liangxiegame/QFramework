using System;
using System.Linq;
using MoonSharp.Interpreter.Compatibility;
using MoonSharp.Interpreter.Interop.BasicDescriptors;

namespace MoonSharp.Interpreter.Interop
{
	/// <summary>
	/// Standard descriptor for Enum values
	/// </summary>
	public class StandardEnumUserDataDescriptor : DispatchingUserDataDescriptor
	{
		/// <summary>
		/// Gets the underlying type of the enum.
		/// </summary>
		public Type UnderlyingType { get; private set; }
		/// <summary>
		/// Gets a value indicating whether underlying type of the enum is unsigned.
		/// </summary>
		public bool IsUnsigned { get; private set; }
		/// <summary>
		/// Gets a value indicating whether this instance describes a flags enumeration.
		/// </summary>
		public bool IsFlags { get; private set; }

		Func<object, ulong> m_EnumToULong = null;
		Func<ulong, object> m_ULongToEnum = null;
		Func<object, long> m_EnumToLong = null;
		Func<long, object> m_LongToEnum = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="StandardEnumUserDataDescriptor"/> class.
		/// </summary>
		/// <param name="enumType">Type of the enum.</param>
		/// <param name="friendlyName">Name of the friendly.</param>
		/// <exception cref="System.ArgumentException">enumType must be an enum!</exception>
		public StandardEnumUserDataDescriptor(Type enumType, string friendlyName = null,
			string[] names = null, object[] values = null, Type underlyingType = null)
			: base(enumType, friendlyName)
		{
			if (!Framework.Do.IsEnum(enumType))
				throw new ArgumentException("enumType must be an enum!");

			UnderlyingType = underlyingType ?? Enum.GetUnderlyingType(enumType);
			IsUnsigned = ((UnderlyingType == typeof(byte)) || (UnderlyingType == typeof(ushort)) || (UnderlyingType == typeof(uint)) || (UnderlyingType == typeof(ulong)));

			names = names ?? Enum.GetNames(this.Type);
			values = values ?? Enum.GetValues(this.Type).OfType<object>().ToArray();

			FillMemberList(names, values);
		}

		/// <summary>
		/// Fills the member list.
		/// </summary>
		private void FillMemberList(string[] names, object[] values)
		{

			for (int i = 0; i < names.Length; i++)
			{
				string name = names[i];
				object value = values.GetValue(i);
				DynValue cvalue = UserData.Create(value, this);

				base.AddDynValue(name, cvalue);
			}

			var attrs = Framework.Do.GetCustomAttributes(this.Type, typeof(FlagsAttribute), true);

			if (attrs != null && attrs.Length > 0)
			{
				IsFlags = true;

				AddEnumMethod("flagsAnd", DynValue.NewCallback(Callback_And));
				AddEnumMethod("flagsOr", DynValue.NewCallback(Callback_Or));
				AddEnumMethod("flagsXor", DynValue.NewCallback(Callback_Xor));
				AddEnumMethod("flagsNot", DynValue.NewCallback(Callback_BwNot));
				AddEnumMethod("hasAll", DynValue.NewCallback(Callback_HasAll));
				AddEnumMethod("hasAny", DynValue.NewCallback(Callback_HasAny));
			}
		}



		/// <summary>
		/// Adds an enum method to the object
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="dynValue">The dyn value.</param>
		private void AddEnumMethod(string name, DynValue dynValue)
		{
			if (!HasMember(name))
				AddDynValue(name, dynValue);

			if (!HasMember("__" + name))
				AddDynValue("__" + name, dynValue);
		}


		/// <summary>
		/// Gets the value of the enum as a long
		/// </summary>
		private long GetValueSigned(DynValue dv)
		{
			CreateSignedConversionFunctions();

			if (dv.Type == DataType.Number)
				return (long)dv.Number;

			if ((dv.Type != DataType.UserData) || (dv.UserData.Descriptor != this) || (dv.UserData.Object == null))
				throw new ScriptRuntimeException("Enum userdata or number expected, or enum is not of the correct type.");

			return m_EnumToLong(dv.UserData.Object);
		}

		/// <summary>
		/// Gets the value of the enum as a ulong
		/// </summary>
		private ulong GetValueUnsigned(DynValue dv)
		{
			CreateUnsignedConversionFunctions();

			if (dv.Type == DataType.Number)
				return (ulong)dv.Number;

			if ((dv.Type != DataType.UserData) || (dv.UserData.Descriptor != this) || (dv.UserData.Object == null))
				throw new ScriptRuntimeException("Enum userdata or number expected, or enum is not of the correct type.");

			return m_EnumToULong(dv.UserData.Object);
		}

		/// <summary>
		/// Creates an enum value from a long
		/// </summary>
		private DynValue CreateValueSigned(long value)
		{
			CreateSignedConversionFunctions();
			return UserData.Create(m_LongToEnum(value), this);
		}

		/// <summary>
		/// Creates an enum value from a ulong
		/// </summary>
		private DynValue CreateValueUnsigned(ulong value)
		{
			CreateUnsignedConversionFunctions();
			return UserData.Create(m_ULongToEnum(value), this);
		}

		/// <summary>
		/// Creates conversion functions for signed enums
		/// </summary>
		private void CreateSignedConversionFunctions()
		{
			if (m_EnumToLong == null || m_LongToEnum == null)
			{
				if (UnderlyingType == typeof(sbyte))
				{
					m_EnumToLong = o => (long)((sbyte)o);
					m_LongToEnum = o => (sbyte)(o);
				}
				else if (UnderlyingType == typeof(short))
				{
					m_EnumToLong = o => (long)((short)o);
					m_LongToEnum = o => (short)(o);
				}
				else if (UnderlyingType == typeof(int))
				{
					m_EnumToLong = o => (long)((int)o);
					m_LongToEnum = o => (int)(o);
				}
				else if (UnderlyingType == typeof(long))
				{
					m_EnumToLong = o => (long)(o);
					m_LongToEnum = o => (long)(o);
				}
				else throw new ScriptRuntimeException("Unexpected enum underlying type : {0}", UnderlyingType.FullName);
			}
		}

		/// <summary>
		/// Creates conversion functions for unsigned enums
		/// </summary>
		private void CreateUnsignedConversionFunctions()
		{
			if (m_EnumToULong == null || m_ULongToEnum == null)
			{
				if (UnderlyingType == typeof(byte))
				{
					m_EnumToULong = o => (ulong)((byte)o);
					m_ULongToEnum = o => (byte)(o);
				}
				else if (UnderlyingType == typeof(ushort))
				{
					m_EnumToULong = o => (ulong)((ushort)o);
					m_ULongToEnum = o => (ushort)(o);
				}
				else if (UnderlyingType == typeof(uint))
				{
					m_EnumToULong = o => (ulong)((uint)o);
					m_ULongToEnum = o => (uint)(o);
				}
				else if (UnderlyingType == typeof(ulong))
				{
					m_EnumToULong = o => (ulong)(o);
					m_ULongToEnum = o => (ulong)(o);
				}
				else throw new ScriptRuntimeException("Unexpected enum underlying type : {0}", UnderlyingType.FullName);
			}
		}

		private DynValue PerformBinaryOperationS(string funcName, ScriptExecutionContext ctx, CallbackArguments args, Func<long, long, DynValue> operation)
		{
			if (args.Count != 2)
				throw new ScriptRuntimeException("Enum.{0} expects two arguments", funcName);

			long v1 = GetValueSigned(args[0]);
			long v2 = GetValueSigned(args[1]);
			return operation(v1, v2);
		}

		private DynValue PerformBinaryOperationU(string funcName, ScriptExecutionContext ctx, CallbackArguments args, Func<ulong, ulong, DynValue> operation)
		{
			if (args.Count != 2)
				throw new ScriptRuntimeException("Enum.{0} expects two arguments", funcName);

			ulong v1 = GetValueUnsigned(args[0]);
			ulong v2 = GetValueUnsigned(args[1]);
			return operation(v1, v2);
		}

		private DynValue PerformBinaryOperationS(string funcName, ScriptExecutionContext ctx, CallbackArguments args, Func<long, long, long> operation)
		{
			return PerformBinaryOperationS(funcName, ctx, args, (v1, v2) => CreateValueSigned(operation(v1, v2)));
		}

		private DynValue PerformBinaryOperationU(string funcName, ScriptExecutionContext ctx, CallbackArguments args, Func<ulong, ulong, ulong> operation)
		{
			return PerformBinaryOperationU(funcName, ctx, args, (v1, v2) => CreateValueUnsigned(operation(v1, v2)));
		}

		private DynValue PerformUnaryOperationS(string funcName, ScriptExecutionContext ctx, CallbackArguments args, Func<long, long> operation)
		{
			if (args.Count != 1)
				throw new ScriptRuntimeException("Enum.{0} expects one argument.", funcName);

			long v1 = GetValueSigned(args[0]);
			long r = operation(v1);
			return CreateValueSigned(r);
		}

		private DynValue PerformUnaryOperationU(string funcName, ScriptExecutionContext ctx, CallbackArguments args, Func<ulong, ulong> operation)
		{
			if (args.Count != 1)
				throw new ScriptRuntimeException("Enum.{0} expects one argument.", funcName);

			ulong v1 = GetValueUnsigned(args[0]);
			ulong r = operation(v1);
			return CreateValueUnsigned(r);
		}

		internal DynValue Callback_Or(ScriptExecutionContext ctx, CallbackArguments args)
		{
			if (IsUnsigned)
				return PerformBinaryOperationU("or", ctx, args, (v1, v2) => v1 | v2);
			else
				return PerformBinaryOperationS("or", ctx, args, (v1, v2) => v1 | v2);
		}

		internal DynValue Callback_And(ScriptExecutionContext ctx, CallbackArguments args)
		{
			if (IsUnsigned)
				return PerformBinaryOperationU("and", ctx, args, (v1, v2) => v1 & v2);
			else
				return PerformBinaryOperationS("and", ctx, args, (v1, v2) => v1 & v2);
		}

		internal DynValue Callback_Xor(ScriptExecutionContext ctx, CallbackArguments args)
		{
			if (IsUnsigned)
				return PerformBinaryOperationU("xor", ctx, args, (v1, v2) => v1 ^ v2);
			else
				return PerformBinaryOperationS("xor", ctx, args, (v1, v2) => v1 ^ v2);
		}

		internal DynValue Callback_BwNot(ScriptExecutionContext ctx, CallbackArguments args)
		{
			if (IsUnsigned)
				return PerformUnaryOperationU("not", ctx, args, v1 => ~v1);
			else
				return PerformUnaryOperationS("not", ctx, args, v1 => ~v1);
		}

		internal DynValue Callback_HasAll(ScriptExecutionContext ctx, CallbackArguments args)
		{
			if (IsUnsigned)
				return PerformBinaryOperationU("hasAll", ctx, args, (v1, v2) => DynValue.NewBoolean((v1 & v2) == v2));
			else
				return PerformBinaryOperationS("hasAll", ctx, args, (v1, v2) => DynValue.NewBoolean((v1 & v2) == v2));
		}

		internal DynValue Callback_HasAny(ScriptExecutionContext ctx, CallbackArguments args)
		{
			if (IsUnsigned)
				return PerformBinaryOperationU("hasAny", ctx, args, (v1, v2) => DynValue.NewBoolean((v1 & v2) != 0));
			else
				return PerformBinaryOperationS("hasAny", ctx, args, (v1, v2) => DynValue.NewBoolean((v1 & v2) != 0));
		}

		/// <summary>
		/// Determines whether the specified object is compatible with the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="obj">The object.</param>
		/// <returns></returns>
		public override bool IsTypeCompatible(Type type, object obj)
		{
			if (obj != null)
				return (Type == type);

			return base.IsTypeCompatible(type, obj);
		}

		/// <summary>
		/// Gets a "meta" operation on this userdata. 
		/// In this specific case, only the concat operator is supported, only on flags enums and it implements the
		/// 'or' operator.
		/// </summary>
		/// <param name="script"></param>
		/// <param name="obj"></param>
		/// <param name="metaname"></param>
		/// <returns></returns>
		public override DynValue MetaIndex(Script script, object obj, string metaname)
		{
			if (metaname == "__concat" && IsFlags)
				return DynValue.NewCallback(Callback_Or);

			return null;
		}
	}
}
