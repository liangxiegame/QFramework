/****************************************************************************
 * Copyright (c) 2018 ~ 2022 liangxiegame UNDER MIT License
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace QFramework
{
    /// <summary>
    /// Used by the injection container to determine if a property or field should be injected.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InjectAttribute : Attribute
    {
        public InjectAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public InjectAttribute()
        {
        }
    }

    public interface IQFrameworkContainer
    {
        /// <summary>
        /// Clears all type mappings and instances.
        /// </summary>
        void Clear();

        /// <summary>
        /// Injects registered types/mappings into an object
        /// </summary>
        /// <param name="obj"></param>
        void Inject(object obj);

        /// <summary>
        /// Injects everything that is registered at once
        /// </summary>
        void InjectAll();

        /// <summary>
        /// Register a type mapping
        /// </summary>
        /// <typeparam name="TSource">The base type.</typeparam>
        /// <typeparam name="TTarget">The concrete type</typeparam>
        void Register<TSource, TTarget>(string name = null);

        void RegisterRelation<TFor, TBase, TConcrete>();

        /// <summary>
        /// Register an instance of a type.
        /// </summary>
        /// <typeparam name="TBase"></typeparam>
        /// <param name="default"></param>
        /// <param name="injectNow"></param>
        /// <returns></returns>
        void RegisterInstance<TBase>(TBase @default, bool injectNow) where TBase : class;

        /// <summary>
        /// Register an instance of a type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="default"></param>
        /// <param name="injectNow"></param>
        /// <returns></returns>
        void RegisterInstance(Type type, object @default, bool injectNow);

        /// <summary>
        /// Register a named instance
        /// </summary>
        /// <param name="baseType">The type to register the instance for.</param>
        /// <param name="name">The name for the instance to be resolved.</param>
        /// <param name="instance">The instance that will be resolved be the name</param>
        /// <param name="injectNow">Perform the injection immediately</param>
        void RegisterInstance(Type baseType, object instance = null, string name = null, bool injectNow = true);

        void RegisterInstance<TBase>(TBase instance, string name, bool injectNow = true) where TBase : class;

        void RegisterInstance<TBase>(TBase instance) where TBase : class;

        /// <summary>
        ///  If an instance of T exist then it will return that instance otherwise it will create a new one based off mappings.
        /// </summary>
        /// <typeparam name="T">The type of instance to resolve</typeparam>
        /// <returns>The/An instance of 'instanceType'</returns>
        T Resolve<T>(string name = null, bool requireInstance = false, params object[] args) where T : class;

        TBase ResolveRelation<TBase>(Type tfor, params object[] arg);

        TBase ResolveRelation<TFor, TBase>(params object[] arg);

        /// <summary>
        /// Resolves all instances of TType or subclasses of TType.  Either named or not.
        /// </summary>
        /// <typeparam name="TType">The Type to resolve</typeparam>
        /// <returns>List of objects.</returns>
        IEnumerable<TType> ResolveAll<TType>();

        //IEnumerable<object> ResolveAll(Type type);
        void Register(Type source, Type target, string name = null);

        /// <summary>
        /// Resolves all instances of TType or subclasses of TType.  Either named or not.
        /// </summary>
        /// <typeparam name="TType">The Type to resolve</typeparam>
        /// <returns>List of objects.</returns>
        IEnumerable<object> ResolveAll(Type type);

        TypeMappingCollection Mappings { get; set; }
        TypeInstanceCollection Instances { get; set; }
        TypeRelationCollection RelationshipMappings { get; set; }

        /// <summary>
        /// If an instance of instanceType exist then it will return that instance otherwise it will create a new one based off mappings.
        /// </summary>
        /// <param name="baseType">The type of instance to resolve</param>
        /// <param name="name">The type of instance to resolve</param>
        /// <param name="requireInstance">If true will return null if an instance isn't registered.</param>
        /// <returns>The/An instance of 'instanceType'</returns>
        object Resolve(Type baseType, string name = null, bool requireInstance = false,
            params object[] constructorArgs);

        object ResolveRelation(Type tfor, Type tbase, params object[] arg);
        void RegisterRelation(Type tfor, Type tbase, Type tconcrete);
        object CreateInstance(Type type, params object[] args);
    }

    /// <summary>
    /// A ViewModel Container and a factory for Controllers and commands.
    /// </summary>
    public class QFrameworkContainer : IQFrameworkContainer
    {
        public enum MemberSearchModes
        {
            OnlyPublic,
            OnlyNonPublic,
            All,
        }

        private MemberSearchModes mMemberSearchMode = MemberSearchModes.OnlyPublic; 
        
        private TypeInstanceCollection mInstances;
        private TypeMappingCollection mMappings;
        
        public TypeMappingCollection Mappings
        {
            get => mMappings ?? (mMappings = new TypeMappingCollection());
            set => mMappings = value;
        }

        public TypeInstanceCollection Instances
        {
            get => mInstances ?? (mInstances = new TypeInstanceCollection());
            set => mInstances = value;
        }

        public TypeRelationCollection RelationshipMappings
        {
            get => mRelationshipMappings;
            set => mRelationshipMappings = value;
        }

        public IEnumerable<TType> ResolveAll<TType>()
        {
            foreach (var obj in ResolveAll(typeof(TType)))
            {
                yield return (TType)obj;
            }
        }

        /// <summary>
        /// Resolves all instances of TType or subclasses of TType.  Either named or not.
        /// </summary>
        /// <typeparam name="TType">The Type to resolve</typeparam>
        /// <returns>List of objects.</returns>
        public IEnumerable<object> ResolveAll(Type type)
        {
            foreach (KeyValuePair<Tuple<Type, string>, object> kv in Instances)
            {
                if (kv.Key.Item1 == type && !string.IsNullOrEmpty(kv.Key.Item2))
                    yield return kv.Value;
            }

            foreach (KeyValuePair<Tuple<Type, string>, Type> kv in Mappings)
            {
                if (!string.IsNullOrEmpty(kv.Key.Item2))
                {
                    var condition = type.IsAssignableFrom(kv.Key.Item1);
                    
                    if (condition)
                    {
                        var item = Activator.CreateInstance(kv.Value);
                        Inject(item);
                        yield return item;
                    }
                }
            }
        }

        /// <summary>
        /// Clears all type-mappings and instances.
        /// </summary>
        public void Clear()
        {
            Instances.Clear();
            Mappings.Clear();
            RelationshipMappings.Clear();
        }

        private MemberInfo[] GetMembers(object obj)
        {
            switch (mMemberSearchMode)
            {
                case MemberSearchModes.OnlyPublic:
                    return obj.GetType().GetMembers();
                case MemberSearchModes.OnlyNonPublic:
                    return obj.GetType().GetMembers(BindingFlags.Instance | BindingFlags.NonPublic);
                case MemberSearchModes.All:
                    return obj.GetType().GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                default:
                    return obj.GetType().GetMembers();
            }
        }

        /// <summary>
        /// Injects registered types/mappings into an object
        /// </summary>
        /// <param name="obj"></param>
        public void Inject(object obj)
        {
            if (obj == null) return;

            var members = GetMembers(obj);
            
            foreach (var memberInfo in members)
            {
                var injectAttribute =
                    memberInfo.GetCustomAttributes(typeof(InjectAttribute), true).FirstOrDefault() as InjectAttribute;
                if (injectAttribute != null)
                {
                    if (memberInfo is PropertyInfo)
                    {
                        var propertyInfo = memberInfo as PropertyInfo;
                        propertyInfo.SetValue(obj, Resolve(propertyInfo.PropertyType, injectAttribute.Name), null);
                    }
                    else if (memberInfo is FieldInfo)
                    {
                        var fieldInfo = memberInfo as FieldInfo;
                        fieldInfo.SetValue(obj, Resolve(fieldInfo.FieldType, injectAttribute.Name));
                    }
                }
            }
        }
        
        public void Register<TSource>(string name = null) => Mappings[typeof(TSource), name] = typeof(TSource);
        
        public void Register<TSource, TTarget>(string name = null) => Mappings[typeof(TSource), name] = typeof(TTarget);

        public void Register(Type source, Type target, string name = null) => Mappings[source, name] = target;

        public void RegisterInstance(Type baseType, object instance = null, bool injectNow = true) => RegisterInstance(baseType, instance, null, injectNow);
        
        public virtual void RegisterInstance(Type baseType, object instance = null, string name = null,
            bool injectNow = true)
        {
            Instances[baseType, name] = instance;
            if (injectNow)
            {
                Inject(instance);
            }
        }

        public void RegisterInstance<TBase>(TBase instance) where TBase : class => RegisterInstance<TBase>(instance, true);

        public void RegisterInstance<TBase>(TBase instance, bool injectNow) where TBase : class => RegisterInstance<TBase>(instance, null, injectNow);

        public void RegisterInstance<TBase>(TBase instance, string name, bool injectNow = true) where TBase : class => RegisterInstance(typeof(TBase), instance, name, injectNow);
        
        public T Resolve<T>(string name = null, bool requireInstance = false, params object[] args) where T : class => (T)Resolve(typeof(T), name, requireInstance, args);
        
        public object Resolve(Type baseType, string name = null, bool requireInstance = false,
            params object[] constructorArgs)
        {
            // Look for an instance first
            var item = Instances[baseType, name];
            if (item != null)
            {
                return item;
            }

            if (requireInstance)
                return null;
            // Check if there is a mapping of the type
            var namedMapping = Mappings[baseType, name];
            if (namedMapping != null)
            {
                var obj = CreateInstance(namedMapping, constructorArgs);
                //Inject(obj);
                return obj;
            }

            return null;
        }

        public object CreateInstance(Type type, params object[] constructorArgs)
        {
            if (constructorArgs != null && constructorArgs.Length > 0)
            {
                //return Activator.CreateInstance(type,BindingFlags.Public | BindingFlags.Instance,Type.DefaultBinder, constructorArgs,CultureInfo.CurrentCulture);
                var obj2 = Activator.CreateInstance(type, constructorArgs);
                Inject(obj2);
                return obj2;
            }
            
            var constructor = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            if (constructor.Length < 1)
            {
                var obj2 = Activator.CreateInstance(type);
                Inject(obj2);
                return obj2;
            }

            var maxParameters = constructor.First().GetParameters();

            foreach (var c in constructor)
            {
                var parameters = c.GetParameters();
                if (parameters.Length > maxParameters.Length)
                {
                    maxParameters = parameters;
                }
            }

            var args = maxParameters.Select(p =>
            {
                if (p.ParameterType.IsArray)
                {
                    return ResolveAll(p.ParameterType);
                }

                return Resolve(p.ParameterType) ?? Resolve(p.ParameterType, p.Name);
            }).ToArray();

            var obj = Activator.CreateInstance(type, args);
            Inject(obj);
            return obj;
        }

        public TBase ResolveRelation<TBase>(Type tfor, params object[] args)
        {
            try
            {
                return (TBase)ResolveRelation(tfor, typeof(TBase), args);
            }
            catch (InvalidCastException castIssue)
            {
                throw new Exception(
                    $"Resolve Relation couldn't cast  to {typeof(TBase).Name} from {tfor.Name}",
                    castIssue);
            }
        }

        public void InjectAll()
        {
            foreach (var instance in Instances.Values)
            {
                Inject(instance);
            }
        }

        private TypeRelationCollection mRelationshipMappings = new TypeRelationCollection();

        public QFrameworkContainer(MemberSearchModes memberSearchMode = MemberSearchModes.OnlyPublic) => mMemberSearchMode = memberSearchMode;

        public void RegisterRelation<TFor, TBase, TConcrete>() => RelationshipMappings[typeof(TFor), typeof(TBase)] = typeof(TConcrete);

        public void RegisterRelation(Type tfor, Type tbase, Type tconcrete) => RelationshipMappings[tfor, tbase] = tconcrete;

        public object ResolveRelation(Type tfor, Type tbase, params object[] args)
        {
            var concreteType = RelationshipMappings[tfor, tbase];

            if (concreteType == null)
            {
                return null;
            }

            var result = CreateInstance(concreteType, args);
            //Inject(result);
            return result;
        }

        public TBase ResolveRelation<TFor, TBase>(params object[] arg) => (TBase)ResolveRelation(typeof(TFor), typeof(TBase), arg);
    }

    // http://stackoverflow.com/questions/1171812/multi-key-dictionary-in-c
    public class Tuple<T1, T2> //FUCKING Unity: struct is not supported in Mono
    {
        public readonly T1 Item1;
        public readonly T2 Item2;

        public Tuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        public override bool Equals(object obj)
        {
            var p = obj as Tuple<T1, T2>;
            if (obj == null) return false;

            if (Item1 == null)
            {
                if (p.Item1 != null) return false;
            }
            else
            {
                if (p.Item1 == null || !Item1.Equals(p.Item1)) return false;
            }

            if (Item2 == null)
            {
                if (p.Item2 != null) return false;
            }
            else
            {
                if (p.Item2 == null || !Item2.Equals(p.Item2)) return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            var hash = 0;
            if (Item1 != null)
                hash ^= Item1.GetHashCode();
            if (Item2 != null)
                hash ^= Item2.GetHashCode();
            return hash;
        }
    }

    // Kanglai: Using Dictionary rather than List!
    public class TypeMappingCollection : Dictionary<Tuple<Type, string>, Type>
    {
        public Type this[Type from, string name = null]
        {
            get
            {
                var key = new Tuple<Type, string>(from, name);
                return TryGetValue(key, out var mapping) ? mapping : null;
            }
            set
            {
                var key = new Tuple<Type, string>(from, name);
                this[key] = value;
            }
        }
    }

    public class TypeInstanceCollection : Dictionary<Tuple<Type, string>, object>
    {
        public object this[Type from, string name = null]
        {
            get
            {
                var key = new Tuple<Type, string>(from, name);
                return TryGetValue(key, out var mapping) ? mapping : null;
            }
            set
            {
                var key = new Tuple<Type, string>(from, name);
                this[key] = value;
            }
        }
    }

    public class TypeRelationCollection : Dictionary<Tuple<Type, Type>, Type>
    {
        public Type this[Type from, Type to]
        {
            get
            {
                var key = new Tuple<Type, Type>(from, to);
                return TryGetValue(key, out var mapping) ? mapping : null;
            }
            set
            {
                var key = new Tuple<Type, Type>(from, to);
                this[key] = value;
            }
        }
    }
}