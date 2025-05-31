using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

#if DOTNET_CORE
	using TTypeInfo = System.Reflection.TypeInfo;
#elif NETFX_CORE
	using TTypeInfo = System.Reflection.TypeInfo;
#else
	using TTypeInfo = System.Type;
#endif

namespace MoonSharp.Interpreter.Compatibility.Frameworks
{
	abstract class FrameworkReflectionBase : FrameworkBase
	{
		public abstract TTypeInfo GetTypeInfoFromType(Type t);

		public override Assembly GetAssembly(Type t)
		{
			return GetTypeInfoFromType(t).Assembly;
		}

		public override Type GetBaseType(Type t)
		{
			return GetTypeInfoFromType(t).BaseType;
		}


		public override bool IsValueType(Type t)
		{
			return GetTypeInfoFromType(t).IsValueType;
		}

		public override bool IsInterface(Type t)
		{
			return GetTypeInfoFromType(t).IsInterface;
		}

		public override bool IsNestedPublic(Type t)
		{
			return GetTypeInfoFromType(t).IsNestedPublic;
		}
		public override bool IsAbstract(Type t)
		{
			return GetTypeInfoFromType(t).IsAbstract;
		}

		public override bool IsEnum(Type t)
		{
			return GetTypeInfoFromType(t).IsEnum;
		}

		public override bool IsGenericTypeDefinition(Type t)
		{
			return GetTypeInfoFromType(t).IsGenericTypeDefinition;
		}

		public override bool IsGenericType(Type t)
		{
			return GetTypeInfoFromType(t).IsGenericType;
		}

		public override Attribute[] GetCustomAttributes(Type t, bool inherit)
		{
			return GetTypeInfoFromType(t).GetCustomAttributes(inherit).OfType<Attribute>().ToArray();
		}

		public override Attribute[] GetCustomAttributes(Type t, Type at, bool inherit)
		{
			return GetTypeInfoFromType(t).GetCustomAttributes(at, inherit).OfType<Attribute>().ToArray();
		}


	}
}
