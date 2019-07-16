using System;
using System.Collections.Generic;
using System.Reflection;

namespace Zenject.Internal
{
    [NoReflectionBaking]
    public class ReflectionTypeInfo
    {
        public readonly Type Type;
        public readonly Type BaseType;
        public readonly List<InjectPropertyInfo> InjectProperties;
        public readonly List<InjectFieldInfo> InjectFields;
        public readonly InjectConstructorInfo InjectConstructor;
        public readonly List<InjectMethodInfo> InjectMethods;

        public ReflectionTypeInfo(
            Type type,
            Type baseType,
            InjectConstructorInfo injectConstructor,
            List<InjectMethodInfo> injectMethods,
            List<InjectFieldInfo> injectFields,
            List<InjectPropertyInfo> injectProperties)
        {
            Type = type;
            BaseType = baseType;
            InjectFields = injectFields;
            InjectConstructor = injectConstructor;
            InjectMethods = injectMethods;
            InjectProperties = injectProperties;
        }

        [NoReflectionBaking]
        public class InjectFieldInfo
        {
            public readonly FieldInfo FieldInfo;
            public readonly InjectableInfo InjectableInfo;

            public InjectFieldInfo(
                FieldInfo fieldInfo,
                InjectableInfo injectableInfo)
            {
                InjectableInfo = injectableInfo;
                FieldInfo = fieldInfo;
            }
        }

        [NoReflectionBaking]
        public class InjectParameterInfo
        {
            public readonly ParameterInfo ParameterInfo;
            public readonly InjectableInfo InjectableInfo;

            public InjectParameterInfo(
                ParameterInfo parameterInfo,
                InjectableInfo injectableInfo)
            {
                InjectableInfo = injectableInfo;
                ParameterInfo = parameterInfo;
            }
        }

        [NoReflectionBaking]
        public class InjectPropertyInfo
        {
            public readonly PropertyInfo PropertyInfo;
            public readonly InjectableInfo InjectableInfo;

            public InjectPropertyInfo(
                PropertyInfo propertyInfo,
                InjectableInfo injectableInfo)
            {
                InjectableInfo = injectableInfo;
                PropertyInfo = propertyInfo;
            }
        }

        [NoReflectionBaking]
        public class InjectMethodInfo
        {
            public readonly MethodInfo MethodInfo;
            public readonly List<InjectParameterInfo> Parameters;

            public InjectMethodInfo(
                MethodInfo methodInfo,
                List<InjectParameterInfo> parameters)
            {
                MethodInfo = methodInfo;
                Parameters = parameters;
            }
        }

        [NoReflectionBaking]
        public class InjectConstructorInfo
        {
            public readonly ConstructorInfo ConstructorInfo;
            public readonly List<InjectParameterInfo> Parameters;

            public InjectConstructorInfo(
                ConstructorInfo constructorInfo,
                List<InjectParameterInfo> parameters)
            {
                ConstructorInfo = constructorInfo;
                Parameters = parameters;
            }
        }
    }
}

