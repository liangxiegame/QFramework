using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ModestTree;
using Zenject.ReflectionBaking.Mono.Cecil;
using Zenject.ReflectionBaking.Mono.Collections.Generic;
using ICustomAttributeProvider = Zenject.ReflectionBaking.Mono.Cecil.ICustomAttributeProvider;

namespace Zenject.ReflectionBaking
{
    public static class CecilExtensions
    {
        public static Type TryGetActualType(this TypeReference typeRef, Assembly assembly)
        {
            var reflectionName = GetReflectionName(typeRef);
            return assembly.GetType(reflectionName);
        }

        static string GetReflectionName(TypeReference type)
        {
            if (type.IsGenericInstance)
            {
                var genericInstance = (GenericInstanceType)type;

                return string.Format(
                    "{0}.{1}[{2}]", genericInstance.Namespace, type.Name,
                    String.Join(",", genericInstance.GenericArguments.Select(p => GetReflectionName(p)).ToArray()));
            }

            return type.FullName;
        }

        public static List<TypeDefinition> LookupAllTypes(this ModuleDefinition module)
        {
            var allTypes = new List<TypeDefinition>();

            foreach (var type in module.Types)
            {
                LookupAllTypesInternal(type, allTypes);
            }

            return allTypes;
        }

        static void LookupAllTypesInternal(TypeDefinition type, List<TypeDefinition> buffer)
        {
            buffer.Add(type);

            foreach (var nestedType in type.NestedTypes)
            {
                LookupAllTypesInternal(nestedType, buffer);
            }
        }

        public static TypeReference ImportType<T>(this ModuleDefinition module)
        {
            return module.ImportType(typeof(T));
        }

        public static TypeReference ImportType(this ModuleDefinition module, Type type)
        {
            return module.Import(type);
        }

        public static MethodReference ImportMethod<T>(this ModuleDefinition module, string methodName)
        {
            return module.ImportMethod(typeof(T), methodName);
        }

        public static MethodReference ImportMethod(
            this ModuleDefinition module, Type type, string methodName)
        {
            return module.Import(
                module.ImportType(type).Resolve().GetMethod(methodName));
        }

        public static MethodReference ImportMethod<T>(
            this ModuleDefinition module, string methodName, int numArgs)
        {
            return module.ImportMethod(typeof(T), methodName, numArgs);
        }

        public static MethodReference ImportMethod(
            this ModuleDefinition module, Type type, string methodName, int numArgs)
        {
            return module.Import(
                module.ImportType(type).Resolve().GetMethod(methodName, numArgs));
        }

        public static MethodDefinition GetMethod(this TypeDefinition instance, string name)
        {
            for (int i = 0; i < instance.Methods.Count; i++)
            {
                MethodDefinition methodDef = instance.Methods[i];

                if (string.CompareOrdinal(methodDef.Name, name) == 0)
                {
                    return methodDef;
                }
            }
            return null;
        }

        public static MethodDefinition GetMethod(this TypeDefinition instance, string name, params Type[] parameterTypes)
        {
            for (int i = 0; i < instance.Methods.Count; i++)
            {
                MethodDefinition methodDefinition = instance.Methods[i];

                if (!string.Equals(methodDefinition.Name, name, StringComparison.Ordinal) ||
                    parameterTypes.Length != methodDefinition.Parameters.Count)
                {
                    continue;
                }

                MethodDefinition result = methodDefinition;
                for (int x = methodDefinition.Parameters.Count - 1; x >= 0; x--)
                {
                    ParameterDefinition parameter = methodDefinition.Parameters[x];
                    if (!string.Equals(parameter.ParameterType.Name, parameterTypes[x].Name, StringComparison.Ordinal))
                    {
                        break;
                    }

                    if (x == 0)
                    {
                        return result;
                    }
                }
            }
            return null;
        }

        public static MethodDefinition GetMethod(this TypeDefinition instance, string name, params TypeReference[] parameterTypes)
        {
            if (instance.Methods != null)
            {
                for (int i = 0; i < instance.Methods.Count; i++)
                {
                    MethodDefinition methodDefinition = instance.Methods[i];
                    if (string.Equals(methodDefinition.Name, name, StringComparison.Ordinal) // Names Match
                        && parameterTypes.Length == methodDefinition.Parameters.Count) // The same number of parameters
                    {
                        MethodDefinition result = methodDefinition;
                        for (int x = methodDefinition.Parameters.Count - 1; x >= 0; x--)
                        {
                            ParameterDefinition parameter = methodDefinition.Parameters[x];
                            if (!string.Equals(parameter.ParameterType.Name, parameterTypes[x].Name, StringComparison.Ordinal))
                            {
                                break;
                            }

                            if (x == 0)
                            {
                                return result;
                            }
                        }
                    }
                }
            }
            return null;
        }

        public static MethodDefinition GetMethod(this TypeDefinition instance, string name, int argCount)
        {
            for (int i = 0; i < instance.Methods.Count; i++)
            {
                MethodDefinition methodDef = instance.Methods[i];

                if (string.CompareOrdinal(methodDef.Name, name) == 0 && methodDef.Parameters.Count == argCount)
                {
                    return methodDef;
                }
            }
            return null;
        }

        public static PropertyDefinition GetPropertyDefinition(this TypeDefinition instance, string name)
        {
            for (int i = 0; i < instance.Properties.Count; i++)
            {
                PropertyDefinition preopertyDef = instance.Properties[i];

                // Properties can only have one argument or they are an indexer.
                if (string.CompareOrdinal(preopertyDef.Name, name) == 0 && preopertyDef.Parameters.Count == 0)
                {
                    return preopertyDef;
                }
            }
            return null;
        }

        public static bool HasCustomAttribute<T>(this ICustomAttributeProvider instance)
        {
            if (!instance.HasCustomAttributes)
            {
                return false;
            }

            Collection<CustomAttribute> attributes = instance.CustomAttributes;

            for(int i = 0;  i < attributes.Count; i++)
            {
                if (attributes[i].AttributeType.FullName.Equals(typeof(T).FullName, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        public static MethodReference ChangeDeclaringType(
            this MethodReference methodDef, TypeReference typeRef)
        {
            var newMethodRef = new MethodReference(
                methodDef.Name, methodDef.ReturnType, typeRef);

            newMethodRef.HasThis = methodDef.HasThis;

            foreach (var arg in methodDef.Parameters)
            {
                var paramDef = new ParameterDefinition(arg.ParameterType);

                newMethodRef.Parameters.Add(paramDef);
            }

            return newMethodRef;
        }

        public static FieldReference ChangeDeclaringType(
            this FieldReference fieldDef, TypeReference typeRef)
        {
            return new FieldReference(
                fieldDef.Name, fieldDef.FieldType, typeRef);
        }

        public static CustomAttribute GetCustomAttribute<T>(this ICustomAttributeProvider instance)
        {
            if (!instance.HasCustomAttributes)
            {
                return null;
            }

            Collection<CustomAttribute> attributes = instance.CustomAttributes;

            for (int i = 0; i < attributes.Count; i++)
            {
                if (attributes[i].AttributeType.FullName.Equals(typeof(T).FullName, StringComparison.Ordinal))
                {
                    return attributes[i];
                }
            }
            return null;
        }

        public static IEnumerable<TypeReference> GetSpecificBaseTypesAndSelf(
            this TypeReference specificTypeRef)
        {
            yield return specificTypeRef;

            foreach (var ancestor in specificTypeRef.GetSpecificBaseTypesAndSelf())
            {
                yield return ancestor;
            }
        }

        public static IEnumerable<TypeReference> GetSpecificBaseTypes(
            this TypeReference specificTypeRef)
        {
            var specificBaseTypeRef = specificTypeRef.TryGetSpecificBaseType();

            if (specificBaseTypeRef != null)
            {
                yield return specificBaseTypeRef;

                foreach (var ancestor in GetSpecificBaseTypes(specificBaseTypeRef))
                {
                    yield return ancestor;
                }
            }
        }

        public static IEnumerable<TypeReference> AllNestParentsAndSelf(this TypeReference specificTypeRef)
        {
            yield return specificTypeRef;

            foreach (var ancestor in specificTypeRef.AllNestParents())
            {
                yield return ancestor;
            }
        }

        public static IEnumerable<TypeReference> AllNestParents(this TypeReference specificTypeRef)
        {
            if (specificTypeRef.DeclaringType != null)
            {
                yield return specificTypeRef.DeclaringType;

                foreach (var ancestor in specificTypeRef.DeclaringType.AllNestParents())
                {
                    yield return ancestor;
                }
            }
        }

        public static TypeReference TryResolve(this TypeReference typeRef)
        {
            try
            {
                return typeRef.Resolve();
            }
            catch
            {
                return null;
            }
        }

        public static TypeReference TryGetSpecificBaseType(this TypeReference specificTypeRef)
        {
            var typeDef = specificTypeRef.Resolve();

            if (typeDef.BaseType == null
                || typeDef.BaseType.FullName == "System.Object")
            {
                return null;
            }

            var specificBaseTypeRef = typeDef.BaseType;

            if (specificBaseTypeRef.ContainsGenericParameter)
            {
                var genericArgMap = new Dictionary<string, TypeReference>();

                foreach (var ancestor in specificTypeRef.AllNestParentsAndSelf())
                {
                    var specificTypeRefGenericInstance = ancestor as GenericInstanceType;

                    if (specificTypeRefGenericInstance != null)
                    {
                        for (int i = 0; i < typeDef.GenericParameters.Count; i++)
                        {
                            genericArgMap[typeDef.GenericParameters[i].Name] = specificTypeRefGenericInstance.GenericArguments[i];
                        }
                    }
                }

                specificBaseTypeRef = FillInGenericParameters(specificBaseTypeRef, genericArgMap);
            }

            return specificBaseTypeRef;
        }

        public static TypeReference FillInGenericParameters(
            TypeReference type, Dictionary<string, TypeReference> genericArgMap)
        {
            var genericType = type as GenericInstanceType;
            Assert.IsNotNull(genericType);

            var genericTypeClone = new GenericInstanceType(type.Resolve());

            for (int i = 0; i < genericType.GenericArguments.Count; i++)
            {
                var arg = genericType.GenericArguments[i];

                if (arg.IsGenericParameter)
                {
                    Assert.That(genericArgMap.ContainsKey(arg.Name), "Could not find key '{0}' for type '{1}'", arg.Name, type.FullName);

                    genericTypeClone.GenericArguments.Add(genericArgMap[arg.Name]);
                }
                else
                {
                    genericTypeClone.GenericArguments.Add(arg);
                }
            }

            return genericTypeClone;
        }
    }
}
