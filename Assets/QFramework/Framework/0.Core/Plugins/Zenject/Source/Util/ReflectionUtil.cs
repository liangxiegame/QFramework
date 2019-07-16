using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ModestTree
{
    public static class ReflectionUtil
    {
        public static Array CreateArray(Type elementType, List<object> instances)
        {
            var array = Array.CreateInstance(elementType, instances.Count);

            for (int i = 0; i < instances.Count; i++)
            {
                var instance = instances[i];

                if (instance != null)
                {
                    Assert.That(instance.GetType().DerivesFromOrEqual(elementType),
                        "Wrong type when creating array, expected something assignable from '"+ elementType +"', but found '" + instance.GetType() + "'");
                }

                array.SetValue(instance, i);
            }

            return array;
        }

        public static IList CreateGenericList(Type elementType, List<object> instances)
        {
            var genericType = typeof(List<>).MakeGenericType(elementType);

            var list = (IList)Activator.CreateInstance(genericType);

            for (int i = 0; i < instances.Count; i++)
            {
                var instance = instances[i];

                if (instance != null)
                {
                    Assert.That(instance.GetType().DerivesFromOrEqual(elementType),
                        "Wrong type when creating generic list, expected something assignable from '"+ elementType +"', but found '" + instance.GetType() + "'");
                }

                list.Add(instance);
            }

            return list;
        }

        public static string ToDebugString(this MethodInfo method)
        {
            return "{0}.{1}".Fmt(method.DeclaringType.PrettyName(), method.Name);
        }

        public static string ToDebugString(this Action action)
        {
#if UNITY_WSA && ENABLE_DOTNET && !UNITY_EDITOR
            return action.ToString();
#else
            return action.Method.ToDebugString();
#endif
        }

        public static string ToDebugString<TParam1>(this Action<TParam1> action)
        {
#if UNITY_WSA && ENABLE_DOTNET && !UNITY_EDITOR
            return action.ToString();
#else
            return action.Method.ToDebugString();
#endif
        }

        public static string ToDebugString<TParam1, TParam2>(this Action<TParam1, TParam2> action)
        {
#if UNITY_WSA && ENABLE_DOTNET && !UNITY_EDITOR
            return action.ToString();
#else
            return action.Method.ToDebugString();
#endif
        }

        public static string ToDebugString<TParam1, TParam2, TParam3>(this Action<TParam1, TParam2, TParam3> action)
        {
#if UNITY_WSA && ENABLE_DOTNET && !UNITY_EDITOR
            return action.ToString();
#else
            return action.Method.ToDebugString();
#endif
        }

        public static string ToDebugString<TParam1, TParam2, TParam3, TParam4>(this Action<TParam1, TParam2, TParam3, TParam4> action)
        {
#if UNITY_WSA && ENABLE_DOTNET && !UNITY_EDITOR
            return action.ToString();
#else
            return action.Method.ToDebugString();
#endif
        }

        public static string ToDebugString<TParam1, TParam2, TParam3, TParam4, TParam5>(this
#if NET_4_6
            Action<TParam1, TParam2, TParam3, TParam4, TParam5> action)
#else
            ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5> action)
#endif
        {
#if UNITY_WSA && ENABLE_DOTNET && !UNITY_EDITOR
            return action.ToString();
#else
            return action.Method.ToDebugString();
#endif
        }

        public static string ToDebugString<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>(this
#if NET_4_6
            Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6> action)
#else
            ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6> action)
#endif
        {
#if UNITY_WSA && ENABLE_DOTNET && !UNITY_EDITOR
            return action.ToString();
#else
            return action.Method.ToDebugString();
#endif
        }

        public static string ToDebugString<TParam1>(this Func<TParam1> func)
        {
#if UNITY_WSA && ENABLE_DOTNET && !UNITY_EDITOR
            return func.ToString();
#else
            return func.Method.ToDebugString();
#endif
        }

        public static string ToDebugString<TParam1, TParam2>(this Func<TParam1, TParam2> func)
        {
#if UNITY_WSA && ENABLE_DOTNET && !UNITY_EDITOR
            return func.ToString();
#else
            return func.Method.ToDebugString();
#endif
        }

        public static string ToDebugString<TParam1, TParam2, TParam3>(this Func<TParam1, TParam2, TParam3> func)
        {
#if UNITY_WSA && ENABLE_DOTNET && !UNITY_EDITOR
            return func.ToString();
#else
            return func.Method.ToDebugString();
#endif
        }

        public static string ToDebugString<TParam1, TParam2, TParam3, TParam4>(this Func<TParam1, TParam2, TParam3, TParam4> func)
        {
#if UNITY_WSA && ENABLE_DOTNET && !UNITY_EDITOR
            return func.ToString();
#else
            return func.Method.ToDebugString();
#endif
        }
    }
}
