using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using QF;

namespace QF.GraphDesigner
{
    public static class InvertApplication
    {
        public static bool IsTestMode { get; set; }
        public static IDebugLogger Logger
        {
            get { return _logger ?? (_logger = new DefaultLogger()); }
            set { _logger = value; }
        }

        private static QFrameworkContainer _container;
        private static ICorePlugin[] _plugins;
        private static IDebugLogger _logger;
        private static Dictionary<Type, IEventManager> _eventManagers;
        private static List<Assembly> _typeAssemblies;

        public static List<Assembly> CachedAssemblies { get; set; }

        public static List<Assembly> TypeAssemblies
        {
            get { return _typeAssemblies ?? (_typeAssemblies = new List<Assembly>()); }
            set { _typeAssemblies = value; }
        }

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

        public static void LoadPluginsFolder(string pluginsFolder)
        {
            if (!Directory.Exists(pluginsFolder))
            {
                Directory.CreateDirectory(pluginsFolder);
            }
            foreach (var plugin in Directory.GetFiles(pluginsFolder, "*.dll"))
            {
                var assembly = Assembly.LoadFrom(plugin);
                assembly = AppDomain.CurrentDomain.Load(assembly.GetName());
                InvertApplication.CachedAssembly(assembly);
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

        public static IEnumerable<Type> GetDerivedTypes<T>(bool includeAbstract = false, bool includeBase = true)
        {
            var type = typeof(T);
            if (includeBase)
                yield return type;
            if (includeAbstract)
            {
                foreach (var assembly in CachedAssemblies)
                {
                    //if (!assembly.FullName.StartsWith("Invert")) continue;
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
                        foreach (var t in assembly
                            .GetTypes()
                            .Where(x => type.IsAssignableFrom(x) && !x.IsAbstract))
                        {
                            items.Add(t);
                        }
                    }
                    catch (Exception ex)
                    {
                        InvertApplication.Log(ex.Message);
                    }
                }
                foreach (var item in items)
                    yield return item;
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
                    continue;
                }

            }
            return null;
        }
        public static Type FindTypeByNameExternal(string name)
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
                    continue;
                }

            }
            return null;
        }

        public static Type FindRuntimeType(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            foreach (var assembly in TypeAssemblies)
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
                    continue;
                }

            }
            return null;
        }

        

        public static ICorePlugin[] Plugins
        {
            get
            {
                return _plugins ?? (_plugins = Container.ResolveAll<ICorePlugin>().ToArray());
            }
            set { _plugins = value; }
        }
        public static int MainThreadId
        {
            get; set;
        }
        public static bool IsMainThread
        {
            get { return System.Threading.Thread.CurrentThread.ManagedThreadId == MainThreadId; }
        }
        private static void InitializeContainer(IQFrameworkContainer container)
        {
            _plugins = null;
            container.RegisterInstance<IQFrameworkContainer>(container);
            var pluginTypes = GetDerivedTypes<ICorePlugin>(false, false).ToArray();
            // Load all plugins
            foreach (var diagramPlugin in pluginTypes)
            {
                if (pluginTypes.Any(p => p.BaseType == diagramPlugin)) continue;
                var pluginInstance = Activator.CreateInstance((Type) diagramPlugin) as ICorePlugin;
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

            foreach (var diagramPlugin in Plugins.OrderBy(p => p.LoadPriority).Where(p=>!p.Ignore))
            {
               
                if (diagramPlugin.Enabled)
                {
                    var start = DateTime.Now;
                    diagramPlugin.Container = Container;
                    diagramPlugin.Initialize(Container);
                       
                }
                
               
            }

            foreach (var diagramPlugin in Plugins.OrderBy(p => p.LoadPriority).Where(p => !p.Ignore))
            {
            
                if (diagramPlugin.Enabled)
                {
                    var start = DateTime.Now;
                    container.Inject(diagramPlugin);
                    diagramPlugin.Loaded(Container); 
                    diagramPlugin.LoadTime = DateTime.Now.Subtract(start);
                }
                

            }
            SignalEvent<ISystemResetEvents>(_=>_.SystemRestarted());
        }

        private static Dictionary<Type, IEventManager> EventManagers
        {
            get { return _eventManagers ?? (_eventManagers = new Dictionary<Type, IEventManager>()); }
            set { _eventManagers = value; }
        }
        public static System.Action ListenFor(Type eventInterface, object listenerObject)
        {
            var listener = listenerObject;
    
            IEventManager manager;
            if (!EventManagers.TryGetValue(eventInterface, out manager))
            {
                EventManagers.Add(eventInterface, manager = (IEventManager) Activator.CreateInstance(typeof(EventManager<>).MakeGenericType(eventInterface)));
            }
            var m = manager as IEventManager;
            
            
            return m.AddListener(listener);
        }
        /// <summary>
        /// Subscribes to a series of related events defined by an interface.
        /// </summary>
        /// <typeparam name="TEvents">The interface type the describes the events.</typeparam>
        /// <param name="listener">The listener that implements the event interface TEvents.</param>
        public static System.Action ListenFor<TEvents>(TEvents listener) where TEvents : class
        {
            IEventManager manager;
            if (!EventManagers.TryGetValue(typeof (TEvents), out manager))
            {
                EventManagers.Add(typeof (TEvents), manager = new EventManager<TEvents>());
            }
            var m = manager as EventManager<TEvents>;
            if (m.Listeners.Contains(listener))
                return () => m.Listeners.Remove(listener);
            return m.Subscribe(listener);
        }
        /// <summary>
        /// Subscribes to a series of related events defined by an interface.
        /// </summary>
        /// <typeparam name="TEvents">The interface type the describes the events.</typeparam>
        /// <param name="listenerObject">The listener that implements the event interface TEvents.</param>
        public static System.Action ListenFor<TEvents>(object listenerObject) where TEvents : class
        {
            var listener = listenerObject as TEvents;
            if (listener == null)
            {
                throw new Exception(string.Format("Listener object is not of type {0}", typeof(TEvents).Name));
            }
            IEventManager manager;
            if (!EventManagers.TryGetValue(typeof(TEvents), out manager))
            {
                EventManagers.Add(typeof(TEvents), manager = new EventManager<TEvents>());
            }
            var m = manager as EventManager<TEvents>;
            if (m.Listeners.Contains(listener))
                return () => m.Listeners.Remove(listener);
            return m.Subscribe(listener);
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
        public static void Execute(System.Action action)
        {
            Execute(new LambdaCommand("Unknown Command", action));
        }
        public static void Execute<TCommand>(TCommand command) where TCommand : ICommand
        {
            SignalEvent<ICommandExecuting>(_ => _.CommandExecuting(command));
            try
            {
                SignalEvent<IExecuteCommand<TCommand>>(_ => _.Execute(command));
            }
            catch (Exception ex)
            {
                SignalEvent<INotify>(_=>_.NotifyWithActions(ex.Message,NotificationIcon.Error,new NotifyActionItem()
                {
                    Title = "More...",
                    Action = () =>
                    {
                        SignalEvent<IShowExceptionDetails>(__ => __.ShowExceptionDetails(new Problem()
                        {
                            Exception = ex
                        }));
                    }
                }));
#if DEBUG
                LogException(ex);
#endif
            }
            SignalEvent<ICommandExecuted>(_ => _.CommandExecuted(command));
        }

        public static BackgroundTask ExecuteInBackground<TCommand>(TCommand command)
            where TCommand : IBackgroundCommand
        {
            SignalEvent<ICommandExecuting>(_ => _.CommandExecuting(command));

            var cmd = new BackgroundTaskCommand()
            {
                Command = command,
                Action = (c) =>
                {
                       
                    SignalEvent<IExecuteCommand<TCommand>>(_ => _.Execute((TCommand)c));
                       
                }
            };
            SignalEvent<ICommandExecuted>(_ => _.CommandExecuted(command));
            SignalEvent<IExecuteCommand<BackgroundTaskCommand>>(m=>
            {
                m.Execute(cmd);
            });

            return cmd.Task;
        } 

        public static void Execute(ICommand command)
        {
            SignalEvent<ICommandExecuting>(_ => _.CommandExecuting(command));
            var type = typeof (IExecuteCommand<>).MakeGenericType(command.GetType());
            IEventManager manager;
            if (!EventManagers.TryGetValue(type, out manager))
            {
                EventManagers.Add(type, manager = Activator.CreateInstance(typeof(EventManager<>).MakeGenericType(type)) as IEventManager);
            }
            manager.Signal(listener => listener.GetType().GetMethod("Execute",new Type[] {command.GetType()}).Invoke(listener, new object[] { command }));
            SignalEvent<ICommandExecuted>(_ => _.CommandExecuted(command));
        }
        public static void Log(string s)
        {
#if DEBUG
            Logger.Log(s);
            //Debug.Log(s);
#endif
        }

        public static IEnumerable<KeyValuePair<PropertyInfo, TAttribute>> GetPropertiesWithAttribute<TAttribute>(this object obj) where TAttribute : Attribute
        {
            return GetPropertiesWithAttributeByType<TAttribute>(obj.GetType());
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
        public static IEnumerable<KeyValuePair<ConstructorInfo, TAttribute>> GetConstructorsWithAttribute<TAttribute>(this Type type, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance) where TAttribute : Attribute
        {
            foreach (var source in type.GetConstructors(flags))
            {
                var attribute = source.GetCustomAttributes(typeof(TAttribute), true).OfType<TAttribute>().FirstOrDefault();
                if (attribute == null) continue;
                yield return new KeyValuePair<ConstructorInfo, TAttribute>(source, (TAttribute)attribute);
            }
        }
        public static IEnumerable<KeyValuePair<MethodInfo, TAttribute>> GetMethodsWithAttribute<TAttribute>(this Type type, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance) where TAttribute : Attribute
        {
            foreach (var source in type.GetMethods(flags))
            {
                var attribute = source.GetCustomAttributes(typeof(TAttribute), true).OfType<TAttribute>().FirstOrDefault();
                if (attribute == null) continue;
                yield return new KeyValuePair<MethodInfo, TAttribute>(source, (TAttribute)attribute);
            }
        }
        public static IEnumerable<KeyValuePair<FieldInfo, TAttribute>> GetFieldsWithAttribute<TAttribute>(this Type type, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance) where TAttribute : Attribute
        {
            foreach (var source in type.GetFields(flags))
            {
                var attribute = source.GetCustomAttributes(typeof(TAttribute), true).OfType<TAttribute>().FirstOrDefault();
                if (attribute == null) continue;
                yield return new KeyValuePair<FieldInfo, TAttribute>(source, (TAttribute)attribute);
            }
        }
        public static IEnumerable<KeyValuePair<EventInfo, TAttribute>> GetEventsWithAttribute<TAttribute>(this Type type, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance) where TAttribute : Attribute
        {
            foreach (var source in type.GetEvents(flags))
            {
                var attribute = source.GetCustomAttributes(typeof(TAttribute), true).OfType<TAttribute>().FirstOrDefault();
                if (attribute == null) continue;
                yield return new KeyValuePair<EventInfo, TAttribute>(source, (TAttribute)attribute);
            }
        } 
        public static IEnumerable<PropertyInfo> GetPropertiesByAttribute<TAttribute>(this object obj) where TAttribute : Attribute
        {
            return GetPropertiesByAttribute<TAttribute>(obj.GetType());
        }

        public static IEnumerable<PropertyInfo> GetPropertiesByAttribute<TAttribute>(this Type type) where TAttribute : Attribute
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(property => property.GetCustomAttributes(typeof(TAttribute), true).Length > 0);
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

        public static void LogException(Exception exception)
        {
            Logger.LogException(exception);
            
        }

        public static void LogError(string format)
        {
            Logger.Log(format);
        }

        public static void LogIfNull(object obj, string s)
        {
            if (obj == null)
            {
                LogError(string.Format("{0} is NULL!!", s ?? "YUP it "));
            }
        }

        public static void CachedAssembly(Assembly assembly)
        {
            if (CachedAssemblies.Contains(assembly)) return;
            CachedAssemblies.Add(assembly);
        }

        public static void CachedTypeAssembly(Assembly assembly)
        {

            if (TypeAssemblies.Contains(assembly)) return;
            TypeAssemblies.Add(assembly);
        }
    }
}