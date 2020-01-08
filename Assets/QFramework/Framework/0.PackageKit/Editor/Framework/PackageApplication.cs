using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace QFramework.PackageKit
{
 public static class PackageApplication
    {
        public static  List<Assembly>                  CachedAssemblies { get; set; }
        private static Dictionary<Type, IEventManager> mEventManagers;

        private static Dictionary<Type, IEventManager> EventManagers
        {
            get { return mEventManagers ?? (mEventManagers = new Dictionary<Type, IEventManager>()); }
            set { mEventManagers = value; }
        }

        private static QFrameworkContainer mContainer;

        public static QFrameworkContainer Container
        {
            get
            {
                if (mContainer != null) return mContainer;
                mContainer = new QFrameworkContainer();
                InitializeContainer(mContainer);
                return mContainer;
            }
            set
            {
                mContainer = value;
                if (mContainer == null)
                {
                    IEventManager eventManager;
                    EventManagers.TryGetValue(typeof(ISystemResetEvents), out eventManager);
                    EventManagers.Clear();
                    var events = eventManager as EventManager<ISystemResetEvents>;
                    if (events != null)
                    {
                        events.Signal(_ => _.SystemResetting());
                    }
                }
            }
        }

        public static IEnumerable<Type> GetDerivedTypes<T>(bool includeAbstract = false, bool includeBase = true)
        {
            var type = typeof(T);
            if (includeBase)
                yield return type;
            if (includeAbstract)
            {
                foreach (var assembly in CachedAssemblies)
                {
                    foreach (var t in assembly
                        .GetTypes()
                        .Where(x => type.IsAssignableFrom(x)))
                    {
                        yield return t;
                    }
                }
            }
            else
            {
                var items = new List<Type>();
                foreach (var assembly in CachedAssemblies)
                {
                    try
                    {
                        items.AddRange(assembly.GetTypes()
                            .Where(x => type.IsAssignableFrom(x) && !x.IsAbstract));
                    }
                    catch (Exception ex)
                    {
                        Debug.Log(ex.Message);
//						InvertApplication.Log(ex.Message);
                    }
                }

                foreach (var item in items)
                    yield return item;
            }
        }

        public static System.Action ListenFor(Type eventInterface, object listenerObject)
        {
            var listener = listenerObject;

            IEventManager manager;
            if (!EventManagers.TryGetValue(eventInterface, out manager))
            {
                EventManagers.Add(eventInterface,
                    manager = (IEventManager) Activator.CreateInstance(
                        typeof(EventManager<>).MakeGenericType(eventInterface)));
            }

            var m = manager;


            return m.AddListener(listener);
        }

        private static IPackageKitView[] mPlugins;

        public static IPackageKitView[] Plugins
        {
            get { return mPlugins ?? (mPlugins = Container.ResolveAll<IPackageKitView>().ToArray()); }
            set { mPlugins = value; }
        }

        private static void InitializeContainer(IQFrameworkContainer container)
        {
            mPlugins = null;
            container.RegisterInstance(container);
            var pluginTypes = GetDerivedTypes<IPackageKitView>(false, false).ToArray();

            foreach (var diagramPlugin in pluginTypes)
            {
                var pluginInstance = Activator.CreateInstance((Type) diagramPlugin) as IPackageKitView;
                if (pluginInstance == null) continue;
                container.RegisterInstance(pluginInstance, diagramPlugin.Name, false);
                container.RegisterInstance(pluginInstance.GetType(), pluginInstance);
                if (pluginInstance.Enabled)
                {
                    foreach (var item in diagramPlugin.GetInterfaces())
                    {
                        ListenFor(item, pluginInstance);
                    }
                }
            }

            container.InjectAll();

            foreach (var diagramPlugin in Plugins.OrderBy(p => p.RenderOrder).Where(p => !p.Ignore))
            {
                if (diagramPlugin.Enabled)
                {
                    var start = DateTime.Now;
                    diagramPlugin.Container = Container;
                    diagramPlugin.Init(Container);
                }
            }

            foreach (var diagramPlugin in Plugins.OrderBy(p => p.RenderOrder).Where(p => !p.Ignore))
            {
                if (diagramPlugin.Enabled)
                {
                    var start = DateTime.Now;
                    container.Inject(diagramPlugin);
//					diagramPlugin.Loaded(Container);
//					diagramPlugin.LoadTime = DateTime.Now.Subtract(start);
                }
            }

            SignalEvent<ISystemResetEvents>(_ => _.SystemRestarted());
        }

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

        static PackageApplication()
        {
            CachedAssemblies = new List<Assembly>
            {
                typeof(int).Assembly, typeof(List<>).Assembly
            };

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.FullName.StartsWith("QF") || assembly.FullName.StartsWith("Assembly-CSharp-Editor"))
                {
                    CachedAssembly(assembly);
                }
            }
        }

        public static void CachedAssembly(Assembly assembly)
        {
            if (CachedAssemblies.Contains(assembly)) return;
            CachedAssemblies.Add(assembly);
        }
    }
}