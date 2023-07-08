#if NETFX_CORE && !DOTNET_CORE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MoonSharp.Interpreter.Compatibility.Frameworks
{
	class FrameworkCurrent : FrameworkReflectionBase
	{
		public override TypeInfo GetTypeInfoFromType(Type t)
		{
			return t.GetTypeInfo();
		}

		private T[] SafeArray<T>(IEnumerable<T> prop) 
		{
			return prop != null ? prop.ToArray() : new T[0];
		}

		public override MethodInfo GetAddMethod(EventInfo ei)
		{
			return ei.AddMethod;
		}

		public override ConstructorInfo[] GetConstructors(Type type)
		{
			return SafeArray(GetTypeInfoFromType(type).DeclaredConstructors);
		}

		public override EventInfo[] GetEvents(Type type)
		{
			return SafeArray(type.GetRuntimeEvents());
		}

		public override FieldInfo[] GetFields(Type type)
		{
			return SafeArray(type.GetRuntimeFields());
		}

		public override Type[] GetGenericArguments(Type type)
		{
			return type.GetTypeInfo().GenericTypeArguments;
		}

		public override MethodInfo GetGetMethod(PropertyInfo pi)
		{
			return pi.GetMethod;
		}

		public override Type GetInterface(Type type, string name)
		{
			return type.GetTypeInfo().ImplementedInterfaces.FirstOrDefault(t => t.Name == name);
		}

		public override Type[] GetInterfaces(Type t)
		{
			return SafeArray(GetTypeInfoFromType(t).ImplementedInterfaces);
		}

		public override MethodInfo GetMethod(Type type, string name)
		{
			return type.GetRuntimeMethods().FirstOrDefault(mi => mi.Name == name);
		}

		public override MethodInfo[] GetMethods(Type type)
		{
			return SafeArray(type.GetRuntimeMethods());
		}

		public override Type[] GetNestedTypes(Type type)
		{
			return SafeArray(GetTypeInfoFromType(type).DeclaredNestedTypes.Select(ti => ti.AsType()));
		}

		public override PropertyInfo[] GetProperties(Type type)
		{
			return SafeArray(type.GetRuntimeProperties());
		}

		public override PropertyInfo GetProperty(Type type, string name)
		{
			return type.GetRuntimeProperty(name);
		}

		public override MethodInfo GetRemoveMethod(EventInfo ei)
		{
			return ei.RemoveMethod;
		}

		public override MethodInfo GetSetMethod(PropertyInfo pi)
		{
			return pi.SetMethod;
		}


		public override bool IsAssignableFrom(Type current, Type toCompare)
		{
			return current.GetTypeInfo().IsAssignableFrom(toCompare.GetTypeInfo());
		}

		public override bool IsDbNull(object o)
		{
			return o != null && o.GetType().FullName.StartsWith("System.DBNull");
		}

		public override bool IsInstanceOfType(Type t, object o)
		{
			if (o == null)
				return false;

			return t.GetTypeInfo().IsAssignableFrom(o.GetType().GetTypeInfo());
		}

		public override bool StringContainsChar(string str, char chr)
		{
			return str.Contains(chr);
		}

		public override MethodInfo GetMethod(Type resourcesType, string name, Type[] types)
		{
			return resourcesType.GetRuntimeMethod(name, types);
		}

		public override Type[] GetAssemblyTypes(Assembly asm)
		{
			return SafeArray(asm.DefinedTypes.Select(ti => ti.AsType()));
		}

	}
}
#endif
