using System;
using System.Collections.Generic;
using System.Diagnostics;
using ModestTree;
using Zenject.Internal;
using System.Linq;
using TypeExtensions = ModestTree.TypeExtensions;

#if !NOT_UNITY3D
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

#endif

namespace Zenject
{
    internal static class BindingUtil
    {
#if !NOT_UNITY3D

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void AssertIsValidPrefab(UnityEngine.Object prefab)
        {
            Assert.That(!ZenUtilInternal.IsNull(prefab), "Received null prefab during bind command");

#if UNITY_EDITOR
            // Unfortunately we can't do this check because asset bundles return PrefabType.None here
            // as discussed here: https://github.com/svermeulen/Zenject/issues/269#issuecomment-323419408
            //Assert.That(PrefabUtility.GetPrefabType(prefab) == PrefabType.Prefab,
                //"Expected prefab but found game object with name '{0}' during bind command", prefab.name);
#endif
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void AssertIsValidGameObject(GameObject gameObject)
        {
            Assert.That(!ZenUtilInternal.IsNull(gameObject), "Received null game object during bind command");

#if UNITY_EDITOR
            // Unfortunately we can't do this check because asset bundles return PrefabType.None here
            // as discussed here: https://github.com/svermeulen/Zenject/issues/269#issuecomment-323419408
            //Assert.That(PrefabUtility.GetPrefabType(gameObject) != PrefabType.Prefab,
                //"Expected game object but found prefab instead with name '{0}' during bind command", gameObject.name);
#endif
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void AssertIsNotComponent(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                AssertIsNotComponent(type);
            }
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void AssertIsNotComponent<T>()
        {
            AssertIsNotComponent(typeof(T));
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void AssertIsNotComponent(Type type)
        {
            Assert.That(!type.DerivesFrom(typeof(Component)),
                "Invalid type given during bind command.  Expected type '{0}' to NOT derive from UnityEngine.Component", type);
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void AssertDerivesFromUnityObject(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                AssertDerivesFromUnityObject(type);
            }
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void AssertDerivesFromUnityObject<T>()
        {
            AssertDerivesFromUnityObject(typeof(T));
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void AssertDerivesFromUnityObject(Type type)
        {
            Assert.That(type.DerivesFrom<UnityEngine.Object>(),
                "Invalid type given during bind command.  Expected type '{0}' to derive from UnityEngine.Object", type);
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void AssertTypesAreNotComponents(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                AssertIsNotComponent(type);
            }
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void AssertIsValidResourcePath(string resourcePath)
        {
            Assert.That(!string.IsNullOrEmpty(resourcePath), "Null or empty resource path provided");

            // We'd like to validate the path here but unfortunately there doesn't appear to be
            // a way to do this besides loading it
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void AssertIsInterfaceOrScriptableObject(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                AssertIsInterfaceOrScriptableObject(type);
            }
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void AssertIsInterfaceOrScriptableObject<T>()
        {
            AssertIsInterfaceOrScriptableObject(typeof(T));
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void AssertIsInterfaceOrScriptableObject(Type type)
        {
            Assert.That(type.DerivesFrom(typeof(ScriptableObject)) || type.IsInterface(),
                "Invalid type given during bind command.  Expected type '{0}' to either derive from UnityEngine.ScriptableObject or be an interface", type);
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void AssertIsInterfaceOrComponent(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                AssertIsInterfaceOrComponent(type);
            }
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void AssertIsInterfaceOrComponent<T>()
        {
            AssertIsInterfaceOrComponent(typeof(T));
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void AssertIsInterfaceOrComponent(Type type)
        {
            Assert.That(type.DerivesFrom(typeof(Component)) || type.IsInterface(),
                "Invalid type given during bind command.  Expected type '{0}' to either derive from UnityEngine.Component or be an interface", type);
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void AssertIsComponent(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                AssertIsComponent(type);
            }
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void AssertIsComponent<T>()
        {
            AssertIsComponent(typeof(T));
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void AssertIsComponent(Type type)
        {
            Assert.That(type.DerivesFrom(typeof(Component)),
                "Invalid type given during bind command.  Expected type '{0}' to derive from UnityEngine.Component", type);
        }
#else
        public static void AssertTypesAreNotComponents(IEnumerable<Type> types)
        {
        }

        public static void AssertIsNotComponent(Type type)
        {
        }

        public static void AssertIsNotComponent<T>()
        {
        }

        public static void AssertIsNotComponent(IEnumerable<Type> types)
        {
        }
#endif

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void AssertTypesAreNotAbstract(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                AssertIsNotAbstract(type);
            }
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void AssertIsNotAbstract(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                AssertIsNotAbstract(type);
            }
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void AssertIsNotAbstract<T>()
        {
            AssertIsNotAbstract(typeof(T));
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void AssertIsNotAbstract(Type type)
        {
            Assert.That(!type.IsAbstract(),
                "Invalid type given during bind command.  Expected type '{0}' to not be abstract.", type);
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void AssertIsDerivedFromType(Type concreteType, Type parentType)
        {
#if !(UNITY_WSA && ENABLE_DOTNET)
            // TODO: Is it possible to do this on WSA?

            Assert.That(parentType.IsOpenGenericType() == concreteType.IsOpenGenericType(),
                "Invalid type given during bind command.  Expected type '{0}' and type '{1}' to both either be open generic types or not open generic types", parentType, concreteType);

            if (parentType.IsOpenGenericType())
            {
                Assert.That(concreteType.IsOpenGenericType());
                Assert.That(TypeExtensions.IsAssignableToGenericType(concreteType, parentType),
                    "Invalid type given during bind command.  Expected open generic type '{0}' to derive from open generic type '{1}'", concreteType, parentType);
            }
            else
#endif
            {
                Assert.That(concreteType.DerivesFromOrEqual(parentType),
                    "Invalid type given during bind command.  Expected type '{0}' to derive from type '{1}'", concreteType, parentType);
            }
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void AssertConcreteTypeListIsNotEmpty(IEnumerable<Type> concreteTypes)
        {
            Assert.That(concreteTypes.Count() >= 1,
                "Must supply at least one concrete type to the current binding");
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void AssertIsDerivedFromTypes(
            IEnumerable<Type> concreteTypes, IEnumerable<Type> parentTypes, InvalidBindResponses invalidBindResponse)
        {
            if (invalidBindResponse == InvalidBindResponses.Assert)
            {
                AssertIsDerivedFromTypes(concreteTypes, parentTypes);
            }
            else
            {
                Assert.IsEqual(invalidBindResponse, InvalidBindResponses.Skip);
            }
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void AssertIsDerivedFromTypes(IEnumerable<Type> concreteTypes, IEnumerable<Type> parentTypes)
        {
            foreach (var concreteType in concreteTypes)
            {
                AssertIsDerivedFromTypes(concreteType, parentTypes);
            }
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void AssertIsDerivedFromTypes(Type concreteType, IEnumerable<Type> parentTypes)
        {
            foreach (var parentType in parentTypes)
            {
                AssertIsDerivedFromType(concreteType, parentType);
            }
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void AssertInstanceDerivesFromOrEqual(object instance, IEnumerable<Type> parentTypes)
        {
            if (!ZenUtilInternal.IsNull(instance))
            {
                foreach (var baseType in parentTypes)
                {
                    AssertInstanceDerivesFromOrEqual(instance, baseType);
                }
            }
        }

#if ZEN_STRIP_ASSERTS_IN_BUILDS
        [Conditional("UNITY_EDITOR")]
#endif
        public static void AssertInstanceDerivesFromOrEqual(object instance, Type baseType)
        {
            if (!ZenUtilInternal.IsNull(instance))
            {
                Assert.That(instance.GetType().DerivesFromOrEqual(baseType),
                    "Invalid type given during bind command.  Expected type '{0}' to derive from type '{1}'", instance.GetType(), baseType);
            }
        }

        public static IProvider CreateCachedProvider(IProvider creator)
        {
            if (creator.TypeVariesBasedOnMemberType)
            {
                return new CachedOpenTypeProvider(creator);
            }

            return new CachedProvider(creator);
        }
    }
}
