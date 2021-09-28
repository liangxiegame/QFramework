/****************************************************************************
 * Copyright (c) 2018 ~ 2020.10 liangxie
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace QFramework
{
    /// <summary>
    /// A ViewModel Container and a factory for Controllers and commands.
    /// </summary>

    public class ILRuntimeIOCContainer
    {
        private TypeInstanceCollection _instances;
        private TypeMappingCollection  _mappings;

        public TypeMappingCollection Mappings
        {
            get { return _mappings ?? (_mappings = new TypeMappingCollection()); }
            set { _mappings = value; }
        }

        public TypeInstanceCollection Instances
        {
            get { return _instances ?? (_instances = new TypeInstanceCollection()); }
            set { _instances = value; }
        }

        public TypeRelationCollection RelationshipMappings
        {
            get { return _relationshipMappings; }
            set { _relationshipMappings = value; }
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

        /// <summary>
        /// Register a type mapping
        /// </summary>
        /// <typeparam name="TSource">The base type.</typeparam>
        public void Register<TSource>(string name = null)
        {
            Mappings[typeof(TSource), name] = typeof(TSource);
        }


        /// <summary>
        /// Register a type mapping
        /// </summary>
        /// <typeparam name="TSource">The base type.</typeparam>
        /// <typeparam name="TTarget">The concrete type</typeparam>
        public void Register<TSource, TTarget>(string name = null)
        {
            Mappings[typeof(TSource), name] = typeof(TTarget);
        }

        public void Register(Type source, Type target, string name = null)
        {
            Mappings[source, name] = target;
        }

        /// <summary>
        /// Register a named instance
        /// </summary>
        /// <param name="baseType">The type to register the instance for.</param>        
        /// <param name="instance">The instance that will be resolved be the name</param>
        /// <param name="injectNow">Perform the injection immediately</param>
        public void RegisterInstance(Type baseType, object instance = null)
        {
            RegisterInstance(baseType, instance, null);
        }

        /// <summary>
        /// Register a named instance
        /// </summary>
        /// <param name="baseType">The type to register the instance for.</param>
        /// <param name="name">The name for the instance to be resolved.</param>
        /// <param name="instance">The instance that will be resolved be the name</param>
        /// <param name="injectNow">Perform the injection immediately</param>
        public virtual void RegisterInstance(Type baseType, object instance = null, string name = null)
        {
            Instances[baseType, name] = instance;
        }
        

        public void UnRegisterInstance<TBase>()
        {
            var key = Instances.Keys.SingleOrDefault(k => k.Item1 == typeof(TBase));
            if (key != null)
            {
                Instances.Remove(key);
            }
        }

        public void RegisterInstance<TBase>(TBase instance)
        {
            RegisterInstance<TBase>(instance, null);
        }

        public void RegisterInstance<TBase>(TBase instance, string name)
        {
            RegisterInstance(typeof(TBase), instance, name);
        }

        /// <summary>
        ///  If an instance of T exist then it will return that instance otherwise it will create a new one based off mappings.
        /// </summary>
        /// <typeparam name="T">The type of instance to resolve</typeparam>
        /// <returns>The/An instance of 'instanceType'</returns>
        public T Resolve<T>(string name = null, bool requireInstance = false, params object[] args) where T : class
        {
            return (T) Resolve(typeof(T), name, requireInstance, args);
        }

        /// <summary>
        /// If an instance of instanceType exist then it will return that instance otherwise it will create a new one based off mappings.
        /// </summary>
        /// <param name="baseType">The type of instance to resolve</param>
        /// <param name="name">The type of instance to resolve</param>
        /// <param name="requireInstance">If true will return null if an instance isn't registered.</param>
        /// <param name="constructorArgs">The arguments to pass to the constructor if any.</param>
        /// <returns>The/An instance of 'instanceType'</returns>
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
                return obj2;
            }
#if !NETFX_CORE
            ConstructorInfo[] constructor = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
#else
        ConstructorInfo[] constructor = type.GetTypeInfo().DeclaredConstructors.ToArray();
#endif

            if (constructor.Length < 1)
            {
                var obj2 = Activator.CreateInstance(type);
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
            

            var obj = Activator.CreateInstance(type);
            return obj;
        }

        public TBase ResolveRelation<TBase>(Type tfor, params object[] args)
        {
            try
            {
                return (TBase) ResolveRelation(tfor, typeof(TBase), args);
            }
            catch (InvalidCastException castIssue)
            {
                throw new Exception(
                    string.Format("Resolve Relation couldn't cast  to {0} from {1}", typeof(TBase).Name, tfor.Name),
                    castIssue);
            }
        }

        private TypeRelationCollection _relationshipMappings = new TypeRelationCollection();

        public void RegisterRelation<TFor, TBase, TConcrete>()
        {
            RelationshipMappings[typeof(TFor), typeof(TBase)] = typeof(TConcrete);
        }

        public void RegisterRelation(Type tfor, Type tbase, Type tconcrete)
        {
            RelationshipMappings[tfor, tbase] = tconcrete;
        }

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

        public TBase ResolveRelation<TFor, TBase>(params object[] arg)
        {
            return (TBase) ResolveRelation(typeof(TFor), typeof(TBase), arg);
        }
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

        public override bool Equals(Object obj)
        {
            Tuple<T1, T2> p = obj as Tuple<T1, T2>;
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
            int hash = 0;
            if (Item1 != null)
                hash ^= Item1.GetHashCode();
            if (Item2 != null)
                hash ^= Item2.GetHashCode();
            return hash;
        }
    }

    // Kanglai: Using Dictionary rather than List!
    public class TypeMappingCollection
    {
        Dictionary<Tuple<Type, string>, Type> mDictionary = new Dictionary<Tuple<Type, string>, Type>();
        public Type this[Type from, string name = null]
        {
            get
            {
                Tuple<Type, string> key = new Tuple<Type, string>(from, name);
                Type mapping = null;
                if (mDictionary.TryGetValue(key, out mapping))
                {
                    return mapping;
                }

                return null;
            }
            set
            {
                Tuple<Type, string> key = new Tuple<Type, string>(from, name);
                mDictionary[key] = value;
            }
        }

        public void Clear()
        {
            mDictionary.Clear();
        }
    }

    public class TypeInstanceCollection
    {
        Dictionary<Tuple<Type, string>, object> mDictionary = new Dictionary<Tuple<Type, string>, object>();

        public object this[Type from, string name = null]
        {
            get
            {
                Tuple<Type, string> key = new Tuple<Type, string>(from, name);
                object mapping = null;
                if (mDictionary.TryGetValue(key, out mapping))
                {
                    return mapping;
                }

                return null;
            }
            set
            {
                Tuple<Type, string> key = new Tuple<Type, string>(from, name);
                mDictionary[key] = value;
            }
        }

        public Dictionary<Tuple<Type, string>, object>.KeyCollection Keys
        {
            get
            {
                return mDictionary.Keys;
            }
        }

        public void Clear()
        {
            mDictionary.Clear();
        }

        public void Remove(Tuple<Type, string> key)
        {
            mDictionary.Remove(key);
        }
    }

    public class TypeRelationCollection  
    {
        Dictionary<Tuple<Type, Type>, Type> mDictionary = new Dictionary<Tuple<Type, Type>, Type>();

        public Type this[Type from, Type to]
        {
            get
            {
                Tuple<Type, Type> key = new Tuple<Type, Type>(from, to);
                Type mapping = null;
                if (mDictionary.TryGetValue(key, out mapping))
                {
                    return mapping;
                }

                return null;
            }
            set
            {
                Tuple<Type, Type> key = new Tuple<Type, Type>(from, to);
                mDictionary[key] = value;
            }
        }

        public void Clear()
        {
            mDictionary.Clear();
        }
    }
}