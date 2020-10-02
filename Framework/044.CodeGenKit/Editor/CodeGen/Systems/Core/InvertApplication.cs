using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace QFramework.CodeGen
{
    public static class InvertApplication
    {
        private static QFrameworkContainer mContainer;
        private static Dictionary<Type, ISignalManager> mEventManagers;
        private static List<Assembly> mTypeAssemblies;

        public static List<Assembly> CachedAssemblies { get; set; }

        static InvertApplication()
        {
            CachedAssemblies = new List<Assembly>
            {
                typeof (int).Assembly, typeof (List<>).Assembly
            };

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.FullName.StartsWith("Invert"))
                {
                    CachedAssembly(assembly);
                }
            }
        }

        public static QFrameworkContainer Container
        {
            get
            {
                if (mContainer != null) return mContainer;
                mContainer = new QFrameworkContainer();
                InitializeContainer(mContainer);
                return mContainer;
            }
        }

        public static Type FindType(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            foreach (var assembly in CachedAssemblies)
            {
                var t = assembly.GetType(name);
                if (t != null)
                {
                    return t;
                }
            }
            return null;
        }

        public static Type FindTypeByName(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            foreach (var assembly in CachedAssemblies)
            {
                try
                {
                    foreach (var item in assembly.GetTypes())
                    {
                        if (item.Name == name)
                            return item;    
                        if (item.FullName == name)
                            return item;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning(ex);
                    continue;
                }

            }
            return null;
        }


        private static void InitializeContainer(IQFrameworkContainer container)
        {
            container.RegisterInstance(container);

            container.InjectAll();
        }

        private static Dictionary<Type, ISignalManager> EventManagers
        {
            get { return mEventManagers ?? (mEventManagers = new Dictionary<Type, ISignalManager>()); }
            set { mEventManagers = value; }
        }

        /// <summary>
        /// Signals and event to all listeners
        /// </summary>
        /// <typeparam name="TEvents">The lambda that invokes the action.</typeparam>
        public static void SignalEvent<TEvents>(Action<TEvents> action) where TEvents : class
        {
            ISignalManager manager;
            if (!EventManagers.TryGetValue(typeof(TEvents), out manager))
            {
                EventManagers.Add(typeof(TEvents), manager = new SignalManager<TEvents>());
            }
            var m = manager as SignalManager<TEvents>;
            m.Signal(action);
        }

        public static IEnumerable<KeyValuePair<PropertyInfo, TAttribute>> GetPropertiesWithAttributeByType<TAttribute>(this Type type, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance) where TAttribute : Attribute
        {
            foreach (var source in type.GetProperties(flags).ToArray())
            {
                var attribute = source.GetCustomAttributes(typeof (TAttribute), true).OfType<TAttribute>().FirstOrDefault();
                if (attribute == null) continue;
                yield return new KeyValuePair<PropertyInfo, TAttribute>(source, (TAttribute)attribute);
            }
        }

        public static IEnumerable<KeyValuePair<MethodInfo, TAttribute>> GetMethodsWithAttribute<TAttribute>(this Type type, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance) where TAttribute : Attribute
        {
            foreach (var source in type.GetMethods(flags))
            {
                var attribute = source.GetCustomAttributes(typeof(TAttribute), true).OfType<TAttribute>().FirstOrDefault();
                if (attribute == null) continue;
                yield return new KeyValuePair<MethodInfo, TAttribute>(source, attribute);
            }
        }


        public static Type GetGenericParameter(this Type type)
        {
            var t = type;
            while (t != null)
            {
                if (t.IsGenericType)
                {
                    return t.GetGenericArguments().FirstOrDefault();
                }
                t = t.BaseType;
            }
            return null;
        }

        public static void CachedAssembly(Assembly assembly)
        {
            if (CachedAssemblies.Contains(assembly)) return;
            CachedAssemblies.Add(assembly);
        }
    }
}