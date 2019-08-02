using System;
using System.Collections.Generic;

namespace QF
{
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
        object Resolve(Type baseType, string name = null, bool requireInstance = false, params object[] constructorArgs);

        object ResolveRelation(Type tfor, Type tbase, params object[] arg);
        void RegisterRelation(Type tfor, Type tbase, Type tconcrete);
        object CreateInstance(Type type, params object[] args);
    }
}
