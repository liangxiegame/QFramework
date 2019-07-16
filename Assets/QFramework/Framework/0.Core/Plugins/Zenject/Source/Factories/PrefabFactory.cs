#if !NOT_UNITY3D

using ModestTree;
using UnityEngine;

namespace Zenject
{
    // This factory type can be useful if you want to control where the prefab comes from at runtime
    // rather than from within the installers

    //No parameters
    public class PrefabFactory<T> : IFactory<UnityEngine.Object, T>
        //where T : Component
    {
        [Inject]
        readonly DiContainer _container = null;

        public DiContainer Container
        {
            get { return _container; }
        }

        public virtual T Create(UnityEngine.Object prefab)
        {
            Assert.That(prefab != null,
               "Null prefab given to factory create method when instantiating object with type '{0}'.", typeof(T));

            return _container.InstantiatePrefabForComponent<T>(prefab);
        }

        // Note: We can't really validate here without access to the prefab
        // We could validate the class directly with the current container but that fails when the
        // class is inside a GameObjectContext
    }

    // One parameter
    public class PrefabFactory<P1, T> : IFactory<UnityEngine.Object, P1, T>
        //where T : Component
    {
        [Inject]
        readonly DiContainer _container = null;

        public DiContainer Container
        {
            get { return _container; }
        }

        public virtual T Create(UnityEngine.Object prefab, P1 param)
        {
            Assert.That(prefab != null,
               "Null prefab given to factory create method when instantiating object with type '{0}'.", typeof(T));

            return (T)_container.InstantiatePrefabForComponentExplicit(
                typeof(T), prefab, InjectUtil.CreateArgListExplicit(param));
        }
    }

    // Two parameters
    public class PrefabFactory<P1, P2, T> : IFactory<UnityEngine.Object, P1, P2, T>
        //where T : Component
    {
        [Inject]
        readonly DiContainer _container = null;

        public DiContainer Container
        {
            get { return _container; }
        }

        public virtual T Create(UnityEngine.Object prefab, P1 param, P2 param2)
        {
            Assert.That(prefab != null,
               "Null prefab given to factory create method when instantiating object with type '{0}'.", typeof(T));

            return (T)_container.InstantiatePrefabForComponentExplicit(
                typeof(T), prefab, InjectUtil.CreateArgListExplicit(param, param2));
        }
    }

    // Three parameters
    public class PrefabFactory<P1, P2, P3, T> : IFactory<UnityEngine.Object, P1, P2, P3, T>
        //where T : Component
    {
        [Inject]
        readonly DiContainer _container = null;

        public DiContainer Container
        {
            get { return _container; }
        }

        public virtual T Create(UnityEngine.Object prefab, P1 param, P2 param2, P3 param3)
        {
            Assert.That(prefab != null,
               "Null prefab given to factory create method when instantiating object with type '{0}'.", typeof(T));

            return (T)_container.InstantiatePrefabForComponentExplicit(
                typeof(T), prefab, InjectUtil.CreateArgListExplicit(param, param2, param3));
        }
    }

    // Four parameters
    public class PrefabFactory<P1, P2, P3, P4, T> : IFactory<UnityEngine.Object, P1, P2, P3, P4, T>
        //where T : Component
    {
        [Inject]
        readonly DiContainer _container = null;

        public DiContainer Container
        {
            get { return _container; }
        }

        public virtual T Create(UnityEngine.Object prefab, P1 param, P2 param2, P3 param3, P4 param4)
        {
            Assert.That(prefab != null,
               "Null prefab given to factory create method when instantiating object with type '{0}'.", typeof(T));

            return (T)_container.InstantiatePrefabForComponentExplicit(
                typeof(T), prefab, InjectUtil.CreateArgListExplicit(param, param2, param3, param4));
        }
    }
}

#endif


