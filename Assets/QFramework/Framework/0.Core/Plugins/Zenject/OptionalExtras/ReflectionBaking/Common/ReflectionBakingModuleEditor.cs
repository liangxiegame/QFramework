using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ModestTree;
using Zenject.Internal;
using Zenject.ReflectionBaking.Mono.Cecil;
using Zenject.ReflectionBaking.Mono.Cecil.Cil;
using Zenject.ReflectionBaking.Mono.Collections.Generic;
using MethodAttributes = Zenject.ReflectionBaking.Mono.Cecil.MethodAttributes;

namespace Zenject.ReflectionBaking
{
    public class ReflectionBakingModuleEditor
    {
        readonly Assembly _assembly;
        readonly ModuleDefinition _module;
        readonly List<Regex> _namespaceRegexes;

        MethodReference _zenjectTypeInfoConstructor;
        MethodReference _injectableInfoConstructor;
        MethodReference _injectMethodInfoConstructor;
        MethodReference _injectMemberInfoConstructor;
        MethodReference _constructorInfoConstructor;
        MethodReference _getTypeFromHandleMethod;
        MethodReference _funcConstructor;
        MethodReference _funcPostInject;
        MethodReference _funcMemberSetter;
        MethodReference _preserveConstructor;

        TypeReference _injectMethodInfoType;
        TypeReference _injectMemberInfoType;
        TypeReference _injectableInfoType;
        TypeReference _objectArrayType;
        TypeReference _zenjectTypeInfoType;

        ReflectionBakingModuleEditor(
            ModuleDefinition module, Assembly assembly, List<string> namespacePatterns)
        {
            _module = module;
            _assembly = assembly;
            _namespaceRegexes = namespacePatterns.Select(CreateRegex).ToList();
            _namespaceRegexes.Add(CreateRegex("^Zenject"));
        }

        public static int WeaveAssembly(
            ModuleDefinition module, Assembly assembly)
        {
            return WeaveAssembly(module, assembly, new List<string>());
        }

        public static int WeaveAssembly(
            ModuleDefinition module, Assembly assembly, List<string> namespacePatterns)
        {
            return new ReflectionBakingModuleEditor(module, assembly, namespacePatterns).Run();
        }

        int Run()
        {
            SaveImports();

            int numTypesEditted = 0;

            var allTypes = _module.LookupAllTypes();

            foreach (var typeDef in allTypes)
            {
                if (_namespaceRegexes.Any() && !_namespaceRegexes.Any(x => x.IsMatch(typeDef.FullName)))
                {
                    continue;
                }

                var actualType = typeDef.TryGetActualType(_assembly);

                if (actualType == null)
                {
                    Log.Warn("Could not find actual type for type '{0}', skipping", typeDef.FullName);
                    continue;
                }

                if (TryEditType(typeDef, actualType))
                {
                    numTypesEditted++;
                }
            }

            return numTypesEditted;
        }

        Regex CreateRegex(string regexStr)
        {
            return new Regex(regexStr, RegexOptions.Compiled);
        }

        void SaveImports()
        {
            _zenjectTypeInfoType = _module.ImportType<InjectTypeInfo>();
            _zenjectTypeInfoConstructor = _module.ImportMethod<InjectTypeInfo>(".ctor");

            _injectableInfoConstructor = _module.ImportMethod<InjectableInfo>(".ctor");

            _getTypeFromHandleMethod = _module.ImportMethod<Type>("GetTypeFromHandle", 1);

            _injectMethodInfoType = _module.ImportType<InjectTypeInfo.InjectMethodInfo>();
            _injectMethodInfoConstructor = _module.ImportMethod<InjectTypeInfo.InjectMethodInfo>(".ctor");

            _injectMemberInfoType = _module.ImportType<InjectTypeInfo.InjectMemberInfo>();
            _injectMemberInfoConstructor = _module.ImportMethod<InjectTypeInfo.InjectMemberInfo>(".ctor");

            _preserveConstructor = _module.ImportMethod<Zenject.Internal.PreserveAttribute>(".ctor");
            _constructorInfoConstructor = _module.ImportMethod<InjectTypeInfo.InjectConstructorInfo>(".ctor");

            _injectableInfoType = _module.ImportType<InjectableInfo>();

            _objectArrayType = _module.Import(typeof(object[]));

            _funcConstructor = _module.ImportMethod<ZenFactoryMethod>(".ctor", 2);

            _funcPostInject = _module.ImportMethod<ZenInjectMethod>(".ctor", 2);

            _funcMemberSetter = _module.ImportMethod<ZenMemberSetterMethod>(".ctor", 2);
        }

        public bool TryEditType(TypeDefinition typeDef, Type actualType)
        {
            if (actualType.IsEnum || actualType.IsValueType || actualType.IsInterface
                || actualType.HasAttribute<NoReflectionBakingAttribute>()
                || IsStaticClass(actualType) || actualType.DerivesFromOrEqual<Delegate>() || actualType.DerivesFromOrEqual<Attribute>())
            {
                return false;
            }

            // Allow running on the same dll multiple times without causing problems
            if (IsTypeProcessed(typeDef))
            {
                return false;
            }

            try
            {
                var typeInfo = ReflectionTypeAnalyzer.GetReflectionInfo(actualType);

                var factoryMethod = TryAddFactoryMethod(typeDef, typeInfo);
                var genericTypeDef = CreateGenericInstanceWithParameters(typeDef);
                var fieldSetMethods = AddFieldSetters(typeDef, genericTypeDef, typeInfo);
                var propertySetMethods = AddPropertySetters(typeDef, genericTypeDef, typeInfo);
                var postInjectMethods = AddPostInjectMethods(typeDef, genericTypeDef, typeInfo);

                CreateGetInfoMethod(
                    typeDef, genericTypeDef, typeInfo,
                    factoryMethod, fieldSetMethods, propertySetMethods, postInjectMethods);
            }
            catch (Exception e)
            {
                Log.ErrorException("Error when modifying type '{0}'".Fmt(actualType), e);
                throw;
            }

            return true;
        }

        static bool IsStaticClass(Type type)
        {
            // Apparently this is unique to static classes
            return type.IsAbstract && type.IsSealed;
        }

        // We are already processed if our static constructor calls TypeAnalyzer
        bool IsTypeProcessed(TypeDefinition typeDef)
        {
            return typeDef.GetMethod(TypeAnalyzer.ReflectionBakingGetInjectInfoMethodName) != null;
        }

        void EmitCastOperation(ILProcessor processor, Type type, Collection<GenericParameter> genericParams)
        {
            if (type.IsGenericParameter)
            {
                processor.Emit(OpCodes.Unbox_Any, genericParams[type.GenericParameterPosition]);
            }
            else if (type.IsEnum)
            {
                processor.Emit(OpCodes.Unbox_Any, _module.TypeSystem.Int32);
            }
            else if (type.IsValueType)
            {
                processor.Emit(OpCodes.Unbox_Any, _module.ImportType(type));
            }
            else
            {
                processor.Emit(OpCodes.Castclass, CreateGenericInstanceIfNecessary(type, genericParams));
            }
        }

        TypeReference CreateGenericInstanceWithParameters(TypeDefinition typeDef)
        {
            if (typeDef.GenericParameters.Any())
            {
                var genericInstance = new GenericInstanceType(typeDef);

                foreach (var parameter in typeDef.GenericParameters)
                {
                    genericInstance.GenericArguments.Add(parameter);
                }

                return genericInstance;
            }

            return typeDef;
        }

        MethodDefinition TryAddFactoryMethod(
            TypeDefinition typeDef, ReflectionTypeInfo typeInfo)
        {
            if (typeInfo.Type.GetParentTypes().Any(x => x.FullName == "UnityEngine.Component"))
            {
                Assert.That(typeInfo.InjectConstructor.Parameters.IsEmpty());
                return null;
            }

            if (typeInfo.InjectConstructor.ConstructorInfo == null)
            {
                // static classes, abstract types
                return null;
            }

            var factoryMethod = new MethodDefinition(
                TypeAnalyzer.ReflectionBakingFactoryMethodName,
                MethodAttributes.Private | MethodAttributes.HideBySig |
                MethodAttributes.Static,
                _module.TypeSystem.Object);

            var p1 = new ParameterDefinition(_objectArrayType);
            p1.Name = "P_0";
            factoryMethod.Parameters.Add(p1);

            var body = factoryMethod.Body;
            body.InitLocals = true;

            var processor = body.GetILProcessor();

            var returnValueVar = new VariableDefinition(_module.TypeSystem.Object);
            body.Variables.Add(returnValueVar);

            processor.Emit(OpCodes.Nop);

            Assert.IsNotNull(typeInfo.InjectConstructor);

            var args = typeInfo.InjectConstructor.Parameters;

            for (int i = 0; i < args.Count; i++)
            {
                var arg = args[i];

                processor.Emit(OpCodes.Ldarg_0);
                processor.Emit(OpCodes.Ldc_I4, i);
                processor.Emit(OpCodes.Ldelem_Ref);

                EmitCastOperation(
                    processor, arg.ParameterInfo.ParameterType, typeDef.GenericParameters);
            }

            processor.Emit(OpCodes.Newobj, _module.Import(typeInfo.InjectConstructor.ConstructorInfo));

            processor.Emit(OpCodes.Stloc_0);
            processor.Emit(OpCodes.Ldloc_S, returnValueVar);
            processor.Emit(OpCodes.Ret);

            typeDef.Methods.Add(factoryMethod);

            return factoryMethod;
        }

        void AddPostInjectMethodBody(
            ILProcessor processor, ReflectionTypeInfo.InjectMethodInfo postInjectInfo, TypeDefinition typeDef, TypeReference genericTypeDef)
        {
            processor.Emit(OpCodes.Nop);

            TypeReference declaringTypeDef;
            MethodReference actualMethodDef;

            if (!TryFindLocalMethod(
                genericTypeDef, postInjectInfo.MethodInfo.Name, out declaringTypeDef, out actualMethodDef))
            {
                throw Assert.CreateException();
            }

            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Castclass, declaringTypeDef);

            for (int k = 0; k < postInjectInfo.Parameters.Count; k++)
            {
                var injectInfo = postInjectInfo.Parameters[k];

                processor.Emit(OpCodes.Ldarg_1);
                processor.Emit(OpCodes.Ldc_I4, k);
                processor.Emit(OpCodes.Ldelem_Ref);

                EmitCastOperation(processor, injectInfo.ParameterInfo.ParameterType, typeDef.GenericParameters);
            }

            processor.Emit(OpCodes.Callvirt, actualMethodDef);
            processor.Emit(OpCodes.Ret);
        }

        MethodDefinition AddPostInjectMethod(
            string name, ReflectionTypeInfo.InjectMethodInfo postInjectInfo, TypeDefinition typeDef, TypeReference genericTypeDef)
        {
            var methodDef = new MethodDefinition(
                name,
                MethodAttributes.Private | MethodAttributes.HideBySig |
                MethodAttributes.Static,
                _module.TypeSystem.Void);

            var p1 = new ParameterDefinition(_module.TypeSystem.Object);
            p1.Name = "P_0";
            methodDef.Parameters.Add(p1);

            var p2 = new ParameterDefinition(_objectArrayType);
            p2.Name = "P_1";
            methodDef.Parameters.Add(p2);

            var body = methodDef.Body;
            var processor = body.GetILProcessor();

            AddPostInjectMethodBody(processor, postInjectInfo, typeDef, genericTypeDef);

            typeDef.Methods.Add(methodDef);

            return methodDef;
        }

        List<MethodDefinition> AddPostInjectMethods(
            TypeDefinition typeDef, TypeReference genericTypeDef, ReflectionTypeInfo typeInfo)
        {
            var postInjectMethods = new List<MethodDefinition>();

            for (int i = 0; i < typeInfo.InjectMethods.Count; i++)
            {
                postInjectMethods.Add(
                    AddPostInjectMethod(
                        TypeAnalyzer.ReflectionBakingInjectMethodPrefix + i, typeInfo.InjectMethods[i], typeDef, genericTypeDef));
            }

            return postInjectMethods;
        }

        void EmitSetterMethod(
            ILProcessor processor, MemberInfo memberInfo, TypeDefinition typeDef, TypeReference genericTypeDef)
        {
            processor.Emit(OpCodes.Nop);

            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Castclass, genericTypeDef);

            processor.Emit(OpCodes.Ldarg_1);

            if (memberInfo is FieldInfo)
            {
                var fieldInfo = (FieldInfo)memberInfo;

                EmitCastOperation(processor, fieldInfo.FieldType, typeDef.GenericParameters);

                processor.Emit(OpCodes.Stfld, FindLocalField(genericTypeDef, fieldInfo.Name));
            }
            else
            {
                var propertyInfo = (PropertyInfo)memberInfo;

                EmitCastOperation(processor, propertyInfo.PropertyType, typeDef.GenericParameters);

                processor.Emit(OpCodes.Callvirt, FindLocalPropertySetMethod(genericTypeDef, propertyInfo.Name));
            }

            processor.Emit(OpCodes.Ret);
        }

        MethodDefinition AddSetterMethod(
            string name, MemberInfo memberInfo, TypeDefinition typeDef, TypeReference genericTypeDef)
        {
            var methodDef = new MethodDefinition(
                name,
                MethodAttributes.Private | MethodAttributes.HideBySig |
                MethodAttributes.Static,
                _module.TypeSystem.Void);

            var p1 = new ParameterDefinition(_module.TypeSystem.Object);
            p1.Name = "P_0";
            methodDef.Parameters.Add(p1);

            var p2 = new ParameterDefinition(_module.TypeSystem.Object);
            p2.Name = "P_1";
            methodDef.Parameters.Add(p2);

            methodDef.Body.InitLocals = true;

            EmitSetterMethod(
                methodDef.Body.GetILProcessor(), memberInfo, typeDef, genericTypeDef);

            typeDef.Methods.Add(methodDef);

            return methodDef;
        }

        List<MethodDefinition> AddPropertySetters(
            TypeDefinition typeDef, TypeReference genericTypeDef, ReflectionTypeInfo typeInfo)
        {
            var methodDefs = new List<MethodDefinition>();

            for (int i = 0; i < typeInfo.InjectProperties.Count; i++)
            {
                methodDefs.Add(
                    AddSetterMethod(
                        TypeAnalyzer.ReflectionBakingPropertySetterPrefix + i,
                        typeInfo.InjectProperties[i].PropertyInfo, typeDef, genericTypeDef));
            }

            return methodDefs;
        }

        List<MethodDefinition> AddFieldSetters(
            TypeDefinition typeDef, TypeReference genericTypeDef, ReflectionTypeInfo typeInfo)
        {
            var methodDefs = new List<MethodDefinition>();

            for (int i = 0; i < typeInfo.InjectFields.Count; i++)
            {
                methodDefs.Add(
                    AddSetterMethod(
                        TypeAnalyzer.ReflectionBakingFieldSetterPrefix + i,
                        typeInfo.InjectFields[i].FieldInfo, typeDef, genericTypeDef));
            }

            return methodDefs;
        }

        void CreateGetInfoMethod(
            TypeDefinition typeDef, TypeReference genericTypeDef, ReflectionTypeInfo typeInfo,
            MethodDefinition factoryMethod, List<MethodDefinition> fieldSetMethods,
            List<MethodDefinition> propertySetMethods, List<MethodDefinition> postInjectMethods)
        {
            var getInfoMethodDef = new MethodDefinition(
                TypeAnalyzer.ReflectionBakingGetInjectInfoMethodName,
                MethodAttributes.Private | MethodAttributes.HideBySig |
                MethodAttributes.Static,
                _zenjectTypeInfoType);

            typeDef.Methods.Add(getInfoMethodDef);

            getInfoMethodDef.CustomAttributes.Add(
                new CustomAttribute(_preserveConstructor));

            var returnValueVar = new VariableDefinition(_module.TypeSystem.Object);

            var body = getInfoMethodDef.Body;

            body.Variables.Add(returnValueVar);
            body.InitLocals = true;

            var instructions = new List<Instruction>();

            instructions.Add(Instruction.Create(OpCodes.Ldtoken, genericTypeDef));
            instructions.Add(Instruction.Create(OpCodes.Call, _getTypeFromHandleMethod));

            if (factoryMethod == null)
            {
                instructions.Add(Instruction.Create(OpCodes.Ldnull));
            }
            else
            {
                instructions.Add(Instruction.Create(OpCodes.Ldnull));
                instructions.Add(Instruction.Create(OpCodes.Ldftn, factoryMethod.ChangeDeclaringType(genericTypeDef)));
                instructions.Add(Instruction.Create(OpCodes.Newobj, _funcConstructor));
            }

            instructions.Add(Instruction.Create(OpCodes.Ldc_I4, typeInfo.InjectConstructor.Parameters.Count));
            instructions.Add(Instruction.Create(OpCodes.Newarr, _injectableInfoType));

            for (int i = 0; i < typeInfo.InjectConstructor.Parameters.Count; i++)
            {
                var injectableInfo = typeInfo.InjectConstructor.Parameters[i].InjectableInfo;

                instructions.Add(Instruction.Create(OpCodes.Dup));
                instructions.Add(Instruction.Create(OpCodes.Ldc_I4, i));

                EmitNewInjectableInfoInstructions(
                    instructions, injectableInfo, typeDef);

                instructions.Add(Instruction.Create(OpCodes.Stelem_Ref));
            }

            instructions.Add(Instruction.Create(OpCodes.Newobj, _constructorInfoConstructor));

            instructions.Add(Instruction.Create(OpCodes.Ldc_I4, typeInfo.InjectMethods.Count));
            instructions.Add(Instruction.Create(OpCodes.Newarr, _injectMethodInfoType));

            Assert.IsEqual(postInjectMethods.Count, typeInfo.InjectMethods.Count);

            for (int i = 0; i < typeInfo.InjectMethods.Count; i++)
            {
                var injectMethodInfo = typeInfo.InjectMethods[i];

                instructions.Add(Instruction.Create(OpCodes.Dup));
                instructions.Add(Instruction.Create(OpCodes.Ldc_I4, i));

                AddInjectableMethodInstructions(
                    instructions, injectMethodInfo, typeDef, genericTypeDef, postInjectMethods[i]);

                instructions.Add(Instruction.Create(OpCodes.Stelem_Ref));
            }

            instructions.Add(Instruction.Create(OpCodes.Ldc_I4, fieldSetMethods.Count + propertySetMethods.Count));
            instructions.Add(Instruction.Create(OpCodes.Newarr, _injectMemberInfoType));

            for (int i = 0; i < fieldSetMethods.Count; i++)
            {
                var injectField = typeInfo.InjectFields[i];

                instructions.Add(Instruction.Create(OpCodes.Dup));
                instructions.Add(Instruction.Create(OpCodes.Ldc_I4, i));

                AddInjectableMemberInstructions(
                    instructions,
                    injectField.InjectableInfo, injectField.FieldInfo.Name,
                    typeDef, genericTypeDef, fieldSetMethods[i]);

                instructions.Add(Instruction.Create(OpCodes.Stelem_Ref));
            }

            for (int i = 0; i < propertySetMethods.Count; i++)
            {
                var injectProperty = typeInfo.InjectProperties[i];

                instructions.Add(Instruction.Create(OpCodes.Dup));
                instructions.Add(Instruction.Create(OpCodes.Ldc_I4, fieldSetMethods.Count + i));

                AddInjectableMemberInstructions(
                    instructions,
                    injectProperty.InjectableInfo,
                    injectProperty.PropertyInfo.Name, typeDef, genericTypeDef,
                    propertySetMethods[i]);

                instructions.Add(Instruction.Create(OpCodes.Stelem_Ref));
            }

            instructions.Add(Instruction.Create(OpCodes.Newobj, _zenjectTypeInfoConstructor));

            instructions.Add(Instruction.Create(OpCodes.Stloc_0));
            instructions.Add(Instruction.Create(OpCodes.Ldloc_S, returnValueVar));
            instructions.Add(Instruction.Create(OpCodes.Ret));

            var processor = body.GetILProcessor();

            foreach (var instruction in instructions)
            {
                processor.Append(instruction);
            }
        }

        MethodReference FindLocalPropertySetMethod(
            TypeReference specificTypeRef, string memberName)
        {
            foreach (var typeRef in specificTypeRef.GetSpecificBaseTypesAndSelf())
            {
                var candidatePropertyDef = typeRef.Resolve().Properties
                    .Where(x => x.Name == memberName).SingleOrDefault();

                if (candidatePropertyDef != null)
                {
                    return candidatePropertyDef.SetMethod.ChangeDeclaringType(typeRef);
                }
            }

            throw Assert.CreateException();
        }

        FieldReference FindLocalField(
            TypeReference specificTypeRef, string fieldName)
        {
            foreach (var typeRef in specificTypeRef.GetSpecificBaseTypesAndSelf())
            {
                var candidateFieldDef = typeRef.Resolve().Fields
                    .Where(x => x.Name == fieldName).SingleOrDefault();

                if (candidateFieldDef != null)
                {
                    return candidateFieldDef.ChangeDeclaringType(typeRef);
                }
            }

            throw Assert.CreateException();
        }

        bool TryFindLocalMethod(
            TypeReference specificTypeRef, string methodName, out TypeReference declaringTypeRef, out MethodReference methodRef)
        {
            foreach (var typeRef in specificTypeRef.GetSpecificBaseTypesAndSelf())
            {
                var candidateMethodDef = typeRef.Resolve().Methods
                    .Where(x => x.Name == methodName).SingleOrDefault();

                if (candidateMethodDef != null)
                {
                    declaringTypeRef = typeRef;
                    methodRef = candidateMethodDef.ChangeDeclaringType(typeRef);
                    return true;
                }
            }

            declaringTypeRef = null;
            methodRef = null;
            return false;
        }

        void AddObjectInstructions(
            List<Instruction> instructions,
            object identifier)
        {
            if (identifier == null)
            {
                instructions.Add(Instruction.Create(OpCodes.Ldnull));
            }
            else if (identifier is string)
            {
                instructions.Add(Instruction.Create(OpCodes.Ldstr, (string)identifier));
            }
            else if (identifier is int)
            {
                instructions.Add(Instruction.Create(OpCodes.Ldc_I4, (int)identifier));
                instructions.Add(Instruction.Create(OpCodes.Box, _module.Import(typeof(int))));
            }
            else if (identifier.GetType().IsEnum)
            {
                instructions.Add(Instruction.Create(OpCodes.Ldc_I4, (int)identifier));
                instructions.Add(Instruction.Create(OpCodes.Box, _module.Import(identifier.GetType())));
            }
            else
            {
                throw Assert.CreateException(
                    "Cannot process values with type '{0}' currently.  Feel free to add support for this and submit a pull request to github.", identifier.GetType());
            }
        }

        TypeReference CreateGenericInstanceIfNecessary(
            Type memberType, Collection<GenericParameter> genericParams)
        {
            if (!memberType.ContainsGenericParameters)
            {
                return _module.Import(memberType);
            }

            if (memberType.IsGenericParameter)
            {
                return genericParams[memberType.GenericParameterPosition];
            }

            if (memberType.IsArray)
            {
                return new ArrayType(
                    CreateGenericInstanceIfNecessary(memberType.GetElementType(), genericParams), memberType.GetArrayRank());
            }

            var genericMemberType = memberType.GetGenericTypeDefinition();

            var genericInstance = new GenericInstanceType(_module.Import(genericMemberType));

            foreach (var arg in memberType.GenericArguments())
            {
                genericInstance.GenericArguments.Add(
                    CreateGenericInstanceIfNecessary(arg, genericParams));
            }

            return genericInstance;
        }

        void AddInjectableMemberInstructions(
            List<Instruction> instructions,
            InjectableInfo injectableInfo, string name,
            TypeDefinition typeDef, TypeReference genericTypeDef,
            MethodDefinition methodDef)
        {
            instructions.Add(Instruction.Create(OpCodes.Ldnull));
            instructions.Add(Instruction.Create(OpCodes.Ldftn, methodDef.ChangeDeclaringType(genericTypeDef)));
            instructions.Add(Instruction.Create(OpCodes.Newobj, _funcMemberSetter));

            EmitNewInjectableInfoInstructions(
                instructions, injectableInfo, typeDef);

            instructions.Add(Instruction.Create(OpCodes.Newobj, _injectMemberInfoConstructor));
        }

        void AddInjectableMethodInstructions(
            List<Instruction> instructions,
            ReflectionTypeInfo.InjectMethodInfo injectMethod,
            TypeDefinition typeDef, TypeReference genericTypeDef,
            MethodDefinition methodDef)
        {
            instructions.Add(Instruction.Create(OpCodes.Ldnull));
            instructions.Add(Instruction.Create(OpCodes.Ldftn, methodDef.ChangeDeclaringType(genericTypeDef)));
            instructions.Add(Instruction.Create(OpCodes.Newobj, _funcPostInject));

            instructions.Add(Instruction.Create(OpCodes.Ldc_I4, injectMethod.Parameters.Count));
            instructions.Add(Instruction.Create(OpCodes.Newarr, _injectableInfoType));

            for (int i = 0; i < injectMethod.Parameters.Count; i++)
            {
                var injectableInfo = injectMethod.Parameters[i].InjectableInfo;

                instructions.Add(Instruction.Create(OpCodes.Dup));
                instructions.Add(Instruction.Create(OpCodes.Ldc_I4, i));

                EmitNewInjectableInfoInstructions(
                    instructions, injectableInfo, typeDef);

                instructions.Add(Instruction.Create(OpCodes.Stelem_Ref));
            }

            instructions.Add(Instruction.Create(OpCodes.Ldstr, injectMethod.MethodInfo.Name));

            instructions.Add(Instruction.Create(OpCodes.Newobj, _injectMethodInfoConstructor));
        }

        void EmitNewInjectableInfoInstructions(
            List<Instruction> instructions,
            InjectableInfo injectableInfo,
            TypeDefinition typeDef)
        {
            if (injectableInfo.Optional)
            {
                instructions.Add(Instruction.Create(OpCodes.Ldc_I4_1));
            }
            else
            {
                instructions.Add(Instruction.Create(OpCodes.Ldc_I4_0));
            }

            AddObjectInstructions(instructions, injectableInfo.Identifier);

            instructions.Add(Instruction.Create(OpCodes.Ldstr, injectableInfo.MemberName));

            instructions.Add(Instruction.Create(OpCodes.Ldtoken, CreateGenericInstanceIfNecessary(injectableInfo.MemberType, typeDef.GenericParameters)));

            instructions.Add(Instruction.Create(OpCodes.Call, _getTypeFromHandleMethod));

            AddObjectInstructions(instructions, injectableInfo.DefaultValue);

            instructions.Add(Instruction.Create(OpCodes.Ldc_I4, (int)injectableInfo.SourceType));

            instructions.Add(Instruction.Create(OpCodes.Newobj, _injectableInfoConstructor));
        }
    }
}
