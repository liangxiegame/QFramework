using System;
using System.Collections.Generic;

namespace MoonSharp.Interpreter.Interop.Converters
{
	/// <summary>
	/// Static functions to handle conversions of numeric types
	/// </summary>
	internal static class NumericConversions
	{
		static NumericConversions()
		{
			NumericTypesOrdered = new Type[] 
			{
				typeof(double),
				typeof(decimal), 
				typeof(float), 
				typeof(long), 
				typeof(int), 
				typeof(short), 
				typeof(sbyte), 
				typeof(ulong), 
				typeof(uint), 
				typeof(ushort), 
				typeof(byte), 
			};
			NumericTypes = new HashSet<Type>(NumericTypesOrdered);
		}

		/// <summary>
		/// HashSet of numeric types
		/// </summary>
		internal static readonly HashSet<Type> NumericTypes;
		/// <summary>
		/// Array of numeric types in order used for some conversions
		/// </summary>
		internal static readonly Type[] NumericTypesOrdered;

		/// <summary>
		/// Converts a double to another type
		/// </summary>
		internal static object DoubleToType(Type type, double d)
		{
			type = Nullable.GetUnderlyingType(type) ?? type;

			if (type == typeof(double)) return d;
			if (type == typeof(sbyte)) return (sbyte)d;
			if (type == typeof(byte)) return (byte)d;
			if (type == typeof(short)) return (short)d;
			if (type == typeof(ushort)) return (ushort)d;
			if (type == typeof(int)) return (int)d;
			if (type == typeof(uint)) return (uint)d;
			if (type == typeof(long)) return (long)d;
			if (type == typeof(ulong)) return (ulong)d;
			if (type == typeof(float)) return (float)d;
			if (type == typeof(decimal)) return (decimal)d;
			return d;
		}

		/// <summary>
		/// Converts a type to double
		/// </summary>
		internal static double TypeToDouble(Type type, object d)
		{
			if (type == typeof(double)) return (double)d;
			if (type == typeof(sbyte)) return (double)(sbyte)d;
			if (type == typeof(byte)) return (double)(byte)d;
			if (type == typeof(short)) return (double)(short)d;
			if (type == typeof(ushort)) return (double)(ushort)d;
			if (type == typeof(int)) return (double)(int)d;
			if (type == typeof(uint)) return (double)(uint)d;
			if (type == typeof(long)) return (double)(long)d;
			if (type == typeof(ulong)) return (double)(ulong)d;
			if (type == typeof(float)) return (double)(float)d;
			if (type == typeof(decimal)) return (double)(decimal)d;
			return (double)d;
		}



	}
}
