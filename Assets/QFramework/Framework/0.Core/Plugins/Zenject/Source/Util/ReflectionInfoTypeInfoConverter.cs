//#define ZEN_DO_NOT_USE_COMPILED_EXPRESSIONS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ModestTree;
#if !NOT_UNITY3D
using UnityEngine;
#endif

namespace Zenject.Internal
{
    public static class ReflectionInfoTypeInfoConverter
    {
        public static InjectTypeInfo.InjectMethodInfo ConvertMethod(
            ReflectionTypeInfo.InjectMethodInfo injectMethod)
        {
            var methodInfo = injectMethod.MethodInfo;
            var action = TryCreateActionForMethod(methodInfo);

            if (action == null)
            {
                action = (obj, args) => methodInfo.Invoke(obj, args);
            }

            return new InjectTypeInfo.InjectMethodInfo(
                action,
                injectMethod.Parameters.Select(x => x.InjectableInfo).ToArray(),
                methodInfo.Name);
        }

        public static InjectTypeInfo.InjectConstructorInfo ConvertConstructor(
            ReflectionTypeInfo.InjectConstructorInfo injectConstructor, Type type)
        {
            return new InjectTypeInfo.InjectConstructorInfo(
                TryCreateFactoryMethod(type, injectConstructor),
                injectConstructor.Parameters.Select(x => x.InjectableInfo).ToArray());
        }

        public static InjectTypeInfo.InjectMemberInfo ConvertField(
            Type parentType, ReflectionTypeInfo.InjectFieldInfo injectField)
        {
            return new InjectTypeInfo.InjectMemberInfo(
                GetSetter(parentType, injectField.FieldInfo), injectField.InjectableInfo);
        }

        public static InjectTypeInfo.InjectMemberInfo ConvertProperty(
            Type parentType, ReflectionTypeInfo.InjectPropertyInfo injectProperty)
        {
            return new InjectTypeInfo.InjectMemberInfo(
                GetSetter(parentType, injectProperty.PropertyInfo), injectProperty.InjectableInfo);
        }

        static ZenFactoryMethod TryCreateFactoryMethod(
            Type type, ReflectionTypeInfo.InjectConstructorInfo reflectionInfo)
        {
#if !NOT_UNITY3D
            if (type.DerivesFromOrEqual<Component>())
            {
                return null;
            }
#endif

            if (type.IsAbstract())
            {
                Assert.That(reflectionInfo.Parameters.IsEmpty());
                return null;
            }

            var constructor = reflectionInfo.ConstructorInfo;

            var factoryMethod = TryCreateFactoryMethodCompiledLambdaExpression(type, constructor);

            if (factoryMethod == null)
            {
                if (constructor == null)
                {
                    // No choice in this case except to use the slow Activator.CreateInstance
                    // as far as I know
                    // This should be rare though and only seems to occur when instantiating
                    // structs on platforms that don't support lambda expressions
                    // Non-structs should always have a default constructor
                    factoryMethod = args =>
                    {
                        Assert.That(args.Length == 0);
                        return Activator.CreateInstance(type, new object[0]);
                    };
                }
                else
                {
                    factoryMethod = constructor.Invoke;
                }
            }

            return factoryMethod;
        }

        static ZenFactoryMethod TryCreateFactoryMethodCompiledLambdaExpression(
            Type type, ConstructorInfo constructor)
        {
#if NET_4_6 && !ENABLE_IL2CPP && !ZEN_DO_NOT_USE_COMPILED_EXPRESSIONS

            if (type.ContainsGenericParameters)
            {
                return null;
            }

            ParameterExpression param = Expression.Parameter(typeof(object[]));

            if (constructor == null)
            {
                return Expression.Lambda<ZenFactoryMethod>(
                    Expression.Convert(
                        Expression.New(type), typeof(object)), param).Compile();
            }

            ParameterInfo[] par = constructor.GetParameters();
            Expression[] args = new Expression[par.Length];

            for (int i = 0; i != par.Length; ++i)
            {
                args[i] = Expression.Convert(
                    Expression.ArrayIndex(
                        param, Expression.Constant(i)), par[i].ParameterType);
            }

            return Expression.Lambda<ZenFactoryMethod>(
                Expression.Convert(
                    Expression.New(constructor, args), typeof(object)), param).Compile();
#else
            return null;
#endif
        }

        static ZenInjectMethod TryCreateActionForMethod(MethodInfo methodInfo)
        {
#if NET_4_6 && !ENABLE_IL2CPP && !ZEN_DO_NOT_USE_COMPILED_EXPRESSIONS

            if (methodInfo.DeclaringType.ContainsGenericParameters)
            {
                return null;
            }

            ParameterInfo[] par = methodInfo.GetParameters();

            if (par.Any(x => x.ParameterType.ContainsGenericParameters))
            {
                return null;
            }

            Expression[] args = new Expression[par.Length];
            ParameterExpression argsParam = Expression.Parameter(typeof(object[]));
            ParameterExpression instanceParam = Expression.Parameter(typeof(object));

            for (int i = 0; i != par.Length; ++i)
            {
                args[i] = Expression.Convert(
                    Expression.ArrayIndex(
                        argsParam, Expression.Constant(i)), par[i].ParameterType);
            }

            return Expression.Lambda<ZenInjectMethod>(
                Expression.Call(
                    Expression.Convert(instanceParam, methodInfo.DeclaringType), methodInfo, args),
                instanceParam, argsParam).Compile();
#else
            return null;
#endif
        }

#if !(UNITY_WSA && ENABLE_DOTNET) || UNITY_EDITOR
        static IEnumerable<FieldInfo> GetAllFields(Type t, BindingFlags flags)
        {
            if (t == null)
            {
                return Enumerable.Empty<FieldInfo>();
            }

            return t.GetFields(flags).Concat(GetAllFields(t.BaseType, flags)).Distinct();
        }

        static ZenMemberSetterMethod GetOnlyPropertySetter(
            Type parentType,
            string propertyName)
        {
            Assert.That(parentType != null);
            Assert.That(!string.IsNullOrEmpty(propertyName));

            var allFields = GetAllFields(
                parentType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy).ToList();

            var writeableFields = allFields.Where(f => f.Name == string.Format("<" + propertyName + ">k__BackingField", propertyName)).ToList();

            if (!writeableFields.Any())
            {
                throw new ZenjectException(string.Format(
                    "Can't find backing field for get only property {0} on {1}.\r\n{2}",
                    propertyName, parentType.FullName, string.Join(";", allFields.Select(f => f.Name).ToArray())));
            }

            return (injectable, value) => writeableFields.ForEach(f => f.SetValue(injectable, value));
        }
#endif

        static ZenMemberSetterMethod GetSetter(Type parentType, MemberInfo memInfo)
        {
            var setterMethod = TryGetSetterAsCompiledExpression(parentType, memInfo);

            if (setterMethod != null)
            {
                return setterMethod;
            }

            var fieldInfo = memInfo as FieldInfo;
            var propInfo = memInfo as PropertyInfo;

            if (fieldInfo != null)
            {
                return ((injectable, value) => fieldInfo.SetValue(injectable, value));
            }

            Assert.IsNotNull(propInfo);

#if UNITY_WSA && ENABLE_DOTNET && !UNITY_EDITOR
            return ((object injectable, object value) => propInfo.SetValue(injectable, value, null));
#else
            if (propInfo.CanWrite)
            {
                return ((injectable, value) => propInfo.SetValue(injectable, value, null));
            }

            return GetOnlyPropertySetter(parentType, propInfo.Name);
#endif
        }

        static ZenMemberSetterMethod TryGetSetterAsCompiledExpression(Type parentType, MemberInfo memInfo)
        {
#if NET_4_6 && !ENABLE_IL2CPP && !ZEN_DO_NOT_USE_COMPILED_EXPRESSIONS

            if (parentType.ContainsGenericParameters)
            {
                return null;
            }

            var fieldInfo = memInfo as FieldInfo;
            var propInfo = memInfo as PropertyInfo;

            // It seems that for readonly fields, we have to use the slower approach below
            // As discussed here: https://www.productiverage.com/trying-to-set-a-readonly-autoproperty-value-externally-plus-a-little-benchmarkdotnet
            // We have to skip value types because those can only be set by reference using an lambda expression
            if (!parentType.IsValueType() && (fieldInfo == null || !fieldInfo.IsInitOnly) && (propInfo == null || propInfo.CanWrite))
            {
                Type memberType = fieldInfo != null
                    ? fieldInfo.FieldType : propInfo.PropertyType;

                var typeParam = Expression.Parameter(typeof(object));
                var valueParam = Expression.Parameter(typeof(object));

                return Expression.Lambda<ZenMemberSetterMethod>(
                    Expression.Assign(
                        Expression.MakeMemberAccess(Expression.Convert(typeParam, parentType), memInfo),
                        Expression.Convert(valueParam, memberType)),
                        typeParam, valueParam).Compile();
            }
#endif

            return null;
        }
    }
}
