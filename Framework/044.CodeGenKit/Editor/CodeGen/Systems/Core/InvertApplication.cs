using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace QFramework.CodeGen
{
    public static class InvertApplication
    {
        private static QFrameworkContainer _container;
        private static Dictionary<Type, IEventManager> _eventManagers;
        private static List<Assembly> _typeAssemblies;

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
                if (_container != null) return _container;
                _container = new QFrameworkContainer();
                InitializeContainer(_container);
                return _container;
            }
            set
            {
                _container = value;
                if (_container == null)
                {
                    IEventManager eventManager;
                    EventManagers.TryGetValue(typeof (ISystemResetEvents), out eventManager);
                    EventManagers.Clear();
                    var events = eventManager as EventManager<ISystemResetEvents>;
                    if (events != null)
                    {
                        events.Signal(_=>_.SystemResetting());
                    }
                }
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
            
            SignalEvent<ISystemResetEvents>(_=>_.SystemRestarted());
        }

        private static Dictionary<Type, IEventManager> EventManagers
        {
            get { return _eventManagers ?? (_eventManagers = new Dictionary<Type, IEventManager>()); }
            set { _eventManagers = value; }
        }

        /// <summary>
        /// Signals and event to all listeners
        /// </summary>
        /// <typeparam name="TEvents">The lambda that invokes the action.</typeparam>
        public static void SignalEvent<TEvents>(Action<TEvents> action) where TEvents : class
        {
            IEventManager manager;
            if (!EventManagers.TryGetValue(typeof(TEvents), out manager))
            {
                EventManagers.Add(typeof(TEvents), manager = new EventManager<TEvents>());
            }
            var m = manager as EventManager<TEvents>;
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