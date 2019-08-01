using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ModestTree
{
    public static class TypeExtensions
    {
        static readonly Dictionary<Type, bool> _isClosedGenericType = new Dictionary<Type, bool>();
        static readonly Dictionary<Type, bool> _isOpenGenericType = new Dictionary<Type, bool>();
        static readonly Dictionary<Type, bool> _isValueType = new Dictionary<Type, bool>();
        static readonly Dictionary<Type, Type[]> _interfaces = new Dictionary<Type, Type[]>();

        public static bool DerivesFrom<T>(this Type a)
        {
            return DerivesFrom(a, typeof(T));
        }

        // This seems easier to think about than IsAssignableFrom
        public static bool DerivesFrom(this Type a, Type b)
        {
            return b != a && a.DerivesFromOrEqual(b);
        }

        public static bool DerivesFromOrEqual<T>(this Type a)
        {
            return DerivesFromOrEqual(a, typeof(T));
        }

        public static bool DerivesFromOrEqual(this Type a, Type b)
        {
#if UNITY_WSA && ENABLE_DOTNET && !UNITY_EDITOR
            return b == a || b.GetTypeInfo().IsAssignableFrom(a.GetTypeInfo());
#else
            return b == a || b.IsAssignableFrom(a);
#endif
        }

#if !(UNITY_WSA && ENABLE_DOTNET)
        // TODO: Is it possible to do this on WSA?
        public static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.Interfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                {
                    return true;
                }
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }

            Type baseType = givenType.BaseType;

            if (baseType == null)
            {
                return false;
            }

            return IsAssignableToGenericType(baseType, genericType);
        }
#endif

        public static bool IsEnum(this Type type)
        {
#if UNITY_WSA && ENABLE_DOTNET && !UNITY_EDITOR
            return type.GetTypeInfo().IsEnum;
#else
            return type.IsEnum;
#endif
        }

        public static bool IsValueType(this Type type)
        {
            bool result;
            if (!_isValueType.TryGetValue(type, out result))
            {
#if UNITY_WSA && ENABLE_DOTNET && !UNITY_EDITOR
                result = type.GetTypeInfo().IsValueType;
#else
                result = type.IsValueType;
#endif
                _isValueType[type] = result;
            }
            return result;
        }

        public static MethodInfo[] DeclaredInstanceMethods(this Type type)
        {
#if UNITY_WSA && ENABLE_DOTNET && !UNITY_EDITOR
            return type.GetRuntimeMethods()
                .Where(x => x.DeclaringType == type).ToArray();
#else
            return type.GetMethods(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
#endif
        }

        public static PropertyInfo[] DeclaredInstanceProperties(this Type type)
        {
#if UNITY_WSA && ENABLE_DOTNET && !UNITY_EDITOR
            // There doesn't appear to be an IsStatic member on PropertyInfo
            return type.GetRuntimeProperties()
                .Where(x => x.DeclaringType == type).ToArray();
#else
            return type.GetProperties(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
#endif
        }

        public static FieldInfo[] DeclaredInstanceFields(this Type type)
        {
#if UNITY_WSA && ENABLE_DOTNET && !UNITY_EDITOR
            return type.GetRuntimeFields()
                .Where(x => x.DeclaringType == type && !x.IsStatic).ToArray();
#else
            return type.GetFields(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
#endif
        }

#if UNITY_WSA && ENABLE_DOTNET && !UNITY_EDITOR
        public static bool IsAssignableFrom(this Type a, Type b)
        {
            return a.GetTypeInfo().IsAssignableFrom(b.GetTypeInfo());
        }
#endif

        public static Type BaseType(this Type type)
        {
#if UNITY_WSA && ENABLE_DOTNET && !UNITY_EDITOR
            return type.GetTypeInfo().BaseType;
#else
            return type.BaseType;
#endif
        }

        public static bool IsGenericType(this Type type)
        {
#if UNITY_WSA && ENABLE_DOTNET && !UNITY_EDITOR
            return type.GetTypeInfo().IsGenericType;
#else
            return type.IsGenericType;
#endif
        }
        public static bool IsGenericTypeDefinition(this Type type)
        {
#if UNITY_WSA && ENABLE_DOTNET && !UNITY_EDITOR
            return type.GetTypeInfo().IsGenericTypeDefinition;
#else
            return type.IsGenericTypeDefinition;
#endif
        }

        public static bool IsPrimitive(this Type type)
        {
#if UNITY_WSA && ENABLE_DOTNET && !UNITY_EDITOR
            return type.GetTypeInfo().IsPrimitive;
#else
            return type.IsPrimitive;
#endif
        }

        public static bool IsInterface(this Type type)
        {
#if UNITY_WSA && ENABLE_DOTNET && !UNITY_EDITOR
            return type.GetTypeInfo().IsInterface;
#else
            return type.IsInterface;
#endif
        }

        public static bool ContainsGenericParameters(this Type type)
        {
#if UNITY_WSA && ENABLE_DOTNET && !UNITY_EDITOR
            return type.GetTypeInfo().ContainsGenericParameters;
#else
            return type.ContainsGenericParameters;
#endif
        }

        public static bool IsAbstract(this Type type)
        {
#if UNITY_WSA && ENABLE_DOTNET && !UNITY_EDITOR
            return type.GetTypeInfo().IsAbstract;
#else
            return type.IsAbstract;
#endif
        }

        public static bool IsSealed(this Type type)
        {
#if UNITY_WSA && ENABLE_DOTNET && !UNITY_EDITOR
            return type.GetTypeInfo().IsSealed;
#else
            return type.IsSealed;
#endif
        }

        public static MethodInfo Method(this Delegate del)
        {
#if UNITY_WSA && ENABLE_DOTNET && !UNITY_EDITOR
            return del.GetMethodInfo();
#else
            return del.Method;
#endif
        }

        public static Type[] GenericArguments(this Type type)
        {
#if UNITY_WSA && ENABLE_DOTNET && !UNITY_EDITOR
            return type.GetTypeInfo().GenericTypeArguments;
#else
            return type.GetGenericArguments();
#endif
        }

        public static Type[] Interfaces(this Type type)
        {
            Type[] result;
            if (!_interfaces.TryGetValue(type, out result))
            {
#if UNITY_WSA && ENABLE_DOTNET && !UNITY_EDITOR
                result = type.GetTypeInfo().ImplementedInterfaces.ToArray();
#else
                result = type.GetInterfaces();
#endif
                _interfaces.Add(type, result);
            }
            return result;
        }

        public static ConstructorInfo[] Constructors(this Type type)
        {
#if UNITY_WSA && ENABLE_DOTNET && !UNITY_EDITOR
            return type.GetTypeInfo().DeclaredConstructors.ToArray();
#else
            return type.GetConstructors(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#endif
        }

        public static object GetDefaultValue(this Type type)
        {
            if (type.IsValueType())
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }

        public static bool IsClosedGenericType(this Type type)
        {
            bool result;
            if (!_isClosedGenericType.TryGetValue(type, out result))
            {
                result = type.IsGenericType() && type != type.GetGenericTypeDefinition();
                _isClosedGenericType[type] = result;
            }
            return result;
        }

        public static IEnumerable<Type> GetParentTypes(this Type type)
        {
            if (type == null || type.BaseType() == null || type == typeof(object) || type.BaseType() == typeof(object))
            {
                yield break;
            }

            yield return type.BaseType();

            foreach (var ancestor in type.BaseType().GetParentTypes())
            {
                yield return ancestor;
            }
        }

        public static bool IsOpenGenericType(this Type type)
        {
            bool result;
            if (!_isOpenGenericType.TryGetValue(type, out result))
            {
                result = type.IsGenericType() && type == type.GetGenericTypeDefinition();
                _isOpenGenericType[type] = result;
            }
            return result;
        }

        public static T GetAttribute<T>(this MemberInfo provider)
            where T : Attribute
        {
            return provider.AllAttributes<T>().Single();
        }

        public static T TryGetAttribute<T>(this MemberInfo provider)
            where T : Attribute
        {
            return provider.AllAttributes<T>().OnlyOrDefault();
        }

        public static bool HasAttribute(
            this MemberInfo provider, params Type[] attributeTypes)
        {
            return provider.AllAttributes(attributeTypes).Any();
        }

        public static bool HasAttribute<T>(this MemberInfo provider)
            where T : Attribute
        {
            return provider.AllAttributes(typeof(T)).Any();
        }

        public static IEnumerable<T> AllAttributes<T>(
            this MemberInfo provider)
            where T : Attribute
        {
            return provider.AllAttributes(typeof(T)).Cast<T>();
        }

        public static IEnumerable<Attribute> AllAttributes(
            this MemberInfo provider, params Type[] attributeTypes)
        {
            Attribute[] allAttributes;
#if NETFX_CORE
            allAttributes = provider.GetCustomAttributes<Attribute>(true).ToArray();
#else
            allAttributes = System.Attribute.GetCustomAttributes(provider, typeof(Attribute), true);
#endif
            if (attributeTypes.Length == 0)
            {
                return allAttributes;
            }

            return allAttributes.Where(a => attributeTypes.Any(x => a.GetType().DerivesFromOrEqual(x)));
        }

        // We could avoid this duplication here by using ICustomAttributeProvider but this class
        // does not exist on the WP8 platform
        public static bool HasAttribute(
            this ParameterInfo provider, params Type[] attributeTypes)
        {
            return provider.AllAttributes(attributeTypes).Any();
        }

        public static bool HasAttribute<T>(this ParameterInfo provider)
            where T : Attribute
        {
            return provider.AllAttributes(typeof(T)).Any();
        }

        public static IEnumerable<T> AllAttributes<T>(
            this ParameterInfo provider)
            where T : Attribute
        {
            return provider.AllAttributes(typeof(T)).Cast<T>();
        }

        public static IEnumerable<Attribute> AllAttributes(
            this ParameterInfo provider, params Type[] attributeTypes)
        {
            Attribute[] allAttributes;
#if NETFX_CORE
            allAttributes = provider.GetCustomAttributes<Attribute>(true).ToArray();
#else
            allAttributes = System.Attribute.GetCustomAttributes(provider, typeof(Attribute), true);
#endif
            if (attributeTypes.Length == 0)
            {
                return allAttributes;
            }

            return allAttributes.Where(a => attributeTypes.Any(x => a.GetType().DerivesFromOrEqual(x)));
        }
    }
}
