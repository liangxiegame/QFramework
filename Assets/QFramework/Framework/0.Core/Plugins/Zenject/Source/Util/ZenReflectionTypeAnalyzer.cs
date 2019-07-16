using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ModestTree;
#if !NOT_UNITY3D
using UnityEngine;
#endif

namespace Zenject.Internal
{
    public static class ReflectionTypeAnalyzer
    {
        static readonly HashSet<Type> _injectAttributeTypes;

        static ReflectionTypeAnalyzer()
        {
            _injectAttributeTypes = new HashSet<Type>();
            _injectAttributeTypes.Add(typeof(InjectAttributeBase));
        }

        public static void AddCustomInjectAttribute<T>()
            where T : Attribute
        {
            AddCustomInjectAttribute(typeof(T));
        }

        public static void AddCustomInjectAttribute(Type type)
        {
            Assert.That(type.DerivesFrom<Attribute>());
            _injectAttributeTypes.Add(type);
        }

        public static ReflectionTypeInfo GetReflectionInfo(Type type)
        {
            Assert.That(!type.IsEnum(), "Tried to analyze enum type '{0}'.  This is not supported", type);
            Assert.That(!type.IsArray, "Tried to analyze array type '{0}'.  This is not supported", type);

            var baseType = type.BaseType();

            if (baseType == typeof(object))
            {
                baseType = null;
            }

            return new ReflectionTypeInfo(
                type, baseType, GetConstructorInfo(type), GetMethodInfos(type),
                GetFieldInfos(type), GetPropertyInfos(type));
        }

        static List<ReflectionTypeInfo.InjectPropertyInfo> GetPropertyInfos(Type type)
        {
            return type.DeclaredInstanceProperties()
                .Where(x => _injectAttributeTypes.Any(a => x.HasAttribute(a)))
                .Select(x => new ReflectionTypeInfo.InjectPropertyInfo(
                    x, GetInjectableInfoForMember(type, x))).ToList();
        }

        static List<ReflectionTypeInfo.InjectFieldInfo> GetFieldInfos(Type type)
        {
            return type.DeclaredInstanceFields()
                .Where(x => _injectAttributeTypes.Any(a => x.HasAttribute(a)))
                .Select(x => new ReflectionTypeInfo.InjectFieldInfo(
                    x, GetInjectableInfoForMember(type, x)))
                .ToList();
        }

        static List<ReflectionTypeInfo.InjectMethodInfo> GetMethodInfos(Type type)
        {
            var injectMethodInfos = new List<ReflectionTypeInfo.InjectMethodInfo>();

            // Note that unlike with fields and properties we use GetCustomAttributes
            // This is so that we can ignore inherited attributes, which is necessary
            // otherwise a base class method marked with [Inject] would cause all overridden
            // derived methods to be added as well
            var methodInfos = type.DeclaredInstanceMethods()
                .Where(x => _injectAttributeTypes.Any(a => x.GetCustomAttributes(a, false).Any())).ToList();

            for (int i = 0; i < methodInfos.Count; i++)
            {
                var methodInfo = methodInfos[i];
                var injectAttr = methodInfo.AllAttributes<InjectAttributeBase>().SingleOrDefault();

                if (injectAttr != null)
                {
                    Assert.That(!injectAttr.Optional && injectAttr.Id == null && injectAttr.Source == InjectSources.Any,
                        "Parameters of InjectAttribute do not apply to constructors and methodInfos");
                }

                var injectParamInfos = methodInfo.GetParameters()
                    .Select(x => CreateInjectableInfoForParam(type, x)).ToList();

                injectMethodInfos.Add(
                    new ReflectionTypeInfo.InjectMethodInfo(methodInfo, injectParamInfos));
            }

            return injectMethodInfos;
        }

        static ReflectionTypeInfo.InjectConstructorInfo GetConstructorInfo(Type type)
        {
            var args = new List<ReflectionTypeInfo.InjectParameterInfo>();

            var constructor = TryGetInjectConstructor(type);

            if (constructor != null)
            {
                args.AddRange(constructor.GetParameters().Select(
                    x => CreateInjectableInfoForParam(type, x)));
            }

            return new ReflectionTypeInfo.InjectConstructorInfo(constructor, args);
        }

        static ReflectionTypeInfo.InjectParameterInfo CreateInjectableInfoForParam(
            Type parentType, ParameterInfo paramInfo)
        {
            var injectAttributes = paramInfo.AllAttributes<InjectAttributeBase>().ToList();

            Assert.That(injectAttributes.Count <= 1,
                "Found multiple 'Inject' attributes on type parameter '{0}' of type '{1}'.  Parameter should only have one", paramInfo.Name, parentType);

            var injectAttr = injectAttributes.SingleOrDefault();

            object identifier = null;
            bool isOptional = false;
            InjectSources sourceType = InjectSources.Any;

            if (injectAttr != null)
            {
                identifier = injectAttr.Id;
                isOptional = injectAttr.Optional;
                sourceType = injectAttr.Source;
            }

            bool isOptionalWithADefaultValue = (paramInfo.Attributes & ParameterAttributes.HasDefault) == ParameterAttributes.HasDefault;

            return new ReflectionTypeInfo.InjectParameterInfo(
                paramInfo,
                new InjectableInfo(
                    isOptionalWithADefaultValue || isOptional,
                    identifier,
                    paramInfo.Name,
                    paramInfo.ParameterType,
                    isOptionalWithADefaultValue ? paramInfo.DefaultValue : null,
                    sourceType));
        }

        static InjectableInfo GetInjectableInfoForMember(Type parentType, MemberInfo memInfo)
        {
            var injectAttributes = memInfo.AllAttributes<InjectAttributeBase>().ToList();

            Assert.That(injectAttributes.Count <= 1,
            "Found multiple 'Inject' attributes on type field '{0}' of type '{1}'.  Field should only container one Inject attribute", memInfo.Name, parentType);

            var injectAttr = injectAttributes.SingleOrDefault();

            object identifier = null;
            bool isOptional = false;
            InjectSources sourceType = InjectSources.Any;

            if (injectAttr != null)
            {
                identifier = injectAttr.Id;
                isOptional = injectAttr.Optional;
                sourceType = injectAttr.Source;
            }

            Type memberType = memInfo is FieldInfo
                ? ((FieldInfo)memInfo).FieldType : ((PropertyInfo)memInfo).PropertyType;

            return new InjectableInfo(
                isOptional,
                identifier,
                memInfo.Name,
                memberType,
                null,
                sourceType);
        }

        static ConstructorInfo TryGetInjectConstructor(Type type)
        {
#if !NOT_UNITY3D
            if (type.DerivesFromOrEqual<Component>())
            {
                return null;
            }
#endif

            if (type.IsAbstract())
            {
                return null;
            }

            var constructors = type.Constructors();

#if UNITY_WSA && ENABLE_DOTNET && !UNITY_EDITOR
            // WP8 generates a dummy constructor with signature (internal Classname(UIntPtr dummy))
            // So just ignore that
            constructors = constructors.Where(c => !IsWp8GeneratedConstructor(c)).ToArray();
#endif

            if (constructors.IsEmpty())
            {
                return null;
            }

            if (constructors.HasMoreThan(1))
            {
                var explicitConstructor = (from c in constructors where _injectAttributeTypes.Any(a => c.HasAttribute(a)) select c).SingleOrDefault();

                if (explicitConstructor != null)
                {
                    return explicitConstructor;
                }

                // If there is only one public constructor then use that
                // This makes decent sense but is also necessary on WSA sometimes since the WSA generated
                // constructor can sometimes be private with zero parameters
                var singlePublicConstructor = constructors.Where(x => x.IsPublic).OnlyOrDefault();

                if (singlePublicConstructor != null)
                {
                    return singlePublicConstructor;
                }

                // Choose the one with the least amount of arguments
                // This might result in some non obvious errors like null reference exceptions
                // but is probably the best trade-off since it allows zenject to be more compatible
                // with libraries that don't depend on zenject at all
                // Discussion here - https://github.com/svermeulen/Zenject/issues/416
                return constructors.OrderBy(x => x.GetParameters().Count()).First();
            }

            return constructors[0];
        }

#if UNITY_WSA && ENABLE_DOTNET && !UNITY_EDITOR
        static bool IsWp8GeneratedConstructor(ConstructorInfo c)
        {
            ParameterInfo[] args = c.GetParameters();

            if (args.Length == 1)
            {
                return args[0].ParameterType == typeof(UIntPtr)
                    && (string.IsNullOrEmpty(args[0].Name) || args[0].Name == "dummy");
            }

            if (args.Length == 2)
            {
                return args[0].ParameterType == typeof(UIntPtr)
                    && args[1].ParameterType == typeof(Int64*)
                    && (string.IsNullOrEmpty(args[0].Name) || args[0].Name == "dummy")
                    && (string.IsNullOrEmpty(args[1].Name) || args[1].Name == "dummy");
            }

            return false;
        }
#endif
    }
}
