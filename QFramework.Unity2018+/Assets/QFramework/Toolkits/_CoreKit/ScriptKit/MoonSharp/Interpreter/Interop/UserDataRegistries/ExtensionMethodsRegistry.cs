using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using MoonSharp.Interpreter.Compatibility;
using MoonSharp.Interpreter.DataStructs;
using MoonSharp.Interpreter.Interop.BasicDescriptors;

namespace MoonSharp.Interpreter.Interop.UserDataRegistries
{
	/// <summary>
	/// Registry of all extension methods. Use UserData statics to access these.
	/// </summary>
	internal class ExtensionMethodsRegistry
	{
		private static object s_Lock = new object();
		private static MultiDictionary<string, IOverloadableMemberDescriptor> s_Registry = new MultiDictionary<string, IOverloadableMemberDescriptor>();
		private static MultiDictionary<string, UnresolvedGenericMethod> s_UnresolvedGenericsRegistry = new MultiDictionary<string, UnresolvedGenericMethod>();
		private static int s_ExtensionMethodChangeVersion = 0;

		private class UnresolvedGenericMethod
		{
			public readonly MethodInfo Method;
			public readonly InteropAccessMode AccessMode;
			public readonly HashSet<Type> AlreadyAddedTypes = new HashSet<Type>();

			public UnresolvedGenericMethod(MethodInfo mi, InteropAccessMode mode)
			{
				AccessMode = mode;
				Method = mi;
			}
		}

		/// <summary>
		/// Registers an extension Type (that is a type containing extension methods)
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="mode">The InteropAccessMode.</param>
		public static void RegisterExtensionType(Type type, InteropAccessMode mode = InteropAccessMode.Default)
		{
			lock (s_Lock)
			{
				bool changesDone = false;

				foreach (MethodInfo mi in Framework.Do.GetMethods(type).Where(_mi => _mi.IsStatic))
				{
					if (mi.GetCustomAttributes(typeof(ExtensionAttribute), false).Count() == 0)
						continue;

					if (mi.ContainsGenericParameters)
					{
						s_UnresolvedGenericsRegistry.Add(mi.Name, new UnresolvedGenericMethod(mi, mode));
						changesDone = true;
						continue;
					}

					if (!MethodMemberDescriptor.CheckMethodIsCompatible(mi, false))
						continue;

					var desc = new MethodMemberDescriptor(mi, mode);

					s_Registry.Add(mi.Name, desc);
					changesDone = true;
				}

				if (changesDone)
					++s_ExtensionMethodChangeVersion;
			}
		}

		private static object FrameworkGetMethods()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets all the extension methods which can match a given name
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public static IEnumerable<IOverloadableMemberDescriptor> GetExtensionMethodsByName(string name)
		{
			lock (s_Lock)
				return new List<IOverloadableMemberDescriptor>(s_Registry.Find(name));
		}

		/// <summary>
		/// Gets a number which gets incremented everytime the extension methods registry changes.
		/// Use this to invalidate caches based on extension methods
		/// </summary>
		/// <returns></returns>
		public static int GetExtensionMethodsChangeVersion()
		{
			return s_ExtensionMethodChangeVersion;
		}


		/// <summary>
		/// Gets all the extension methods which can match a given name and extending a given Type
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="extendedType">The extended type.</param>
		/// <returns></returns>
		public static List<IOverloadableMemberDescriptor> GetExtensionMethodsByNameAndType(string name, Type extendedType)
		{
			List<UnresolvedGenericMethod> unresolvedGenerics = null;

			lock (s_Lock)
			{
				unresolvedGenerics = s_UnresolvedGenericsRegistry.Find(name).ToList();
			}

			foreach (UnresolvedGenericMethod ugm in unresolvedGenerics)
			{
				ParameterInfo[] args = ugm.Method.GetParameters();
				if (args.Length == 0) continue;
				Type extensionType = args[0].ParameterType;

				Type genericType = GetGenericMatch(extensionType, extendedType);

				if (ugm.AlreadyAddedTypes.Add(genericType))
				{
					if (genericType != null)
					{
						MethodInfo mi = InstantiateMethodInfo(ugm.Method, extensionType, genericType, extendedType);
						if (mi != null)
						{
							if (!MethodMemberDescriptor.CheckMethodIsCompatible(mi, false))
								continue;

							var desc = new MethodMemberDescriptor(mi, ugm.AccessMode);

							s_Registry.Add(ugm.Method.Name, desc);
							++s_ExtensionMethodChangeVersion;
						}
					}
				}
			}

			return s_Registry.Find(name)
				.Where(d => d.ExtensionMethodType != null && Framework.Do.IsAssignableFrom(d.ExtensionMethodType, extendedType))
				.ToList();
		}

		private static MethodInfo InstantiateMethodInfo(MethodInfo mi, Type extensionType, Type genericType, Type extendedType)
		{
			Type[] defs = mi.GetGenericArguments();
			Type[] tdefs = Framework.Do.GetGenericArguments(genericType);

			if (tdefs.Length == defs.Length)
			{
				return mi.MakeGenericMethod(tdefs);
			}

			return null;
		}

		private static Type GetGenericMatch(Type extensionType, Type extendedType)
		{
			if (!extensionType.IsGenericParameter)
			{
				extensionType = extensionType.GetGenericTypeDefinition();

				foreach (Type t in extendedType.GetAllImplementedTypes())
				{
					if (Framework.Do.IsGenericType(t) && t.GetGenericTypeDefinition() == extensionType)
					{
						return t;
					}
				}
			}

			return null;
		}

	}
}
