using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CSharp;

namespace QF.GraphDesigner
{
    public static class CodeDomHelpers
    {
        public static string GenerateCodeFromMembers(params CodeTypeMember[] items)
        {
            var collection = new CodeTypeMemberCollection(items);
            return GenerateCodeFromMembers(collection);
        }
        public static string GenerateCodeFromMembers(CodeTypeMemberCollection collection)
        {
            var type = new CodeTypeDeclaration("DUMMY");
            type.Members.AddRange(collection);
            var cp = new CSharpCodeProvider();
            var sb = new StringBuilder();
            var strWriter = new StringWriter(sb);


            var ccu = new CodeCompileUnit();
            var ns = new CodeNamespace();
            ns.Types.Add(type);
            ccu.Namespaces.Add(ns);

            cp.GenerateCodeFromCompileUnit(ccu, strWriter, new CodeGeneratorOptions());
            var adjusted = new[] { "" }.Concat(
                sb.ToString()
                    .Split(new string[] { Environment.NewLine }, StringSplitOptions.None)
                    .Skip(14)
                    .Reverse()
                    .Skip(2)
                    .Reverse()).ToArray();
            return string.Join("\r\n", adjusted);
        }
        public static string Clean(this string str)
        {
            return Regex.Replace(str, @"[^a-zA-Z0-9_\.]+", "");
        }
        public static CodeTypeDeclaration MakeStatic(this CodeTypeDeclaration type)
        {
            type.BaseTypes.Clear();
            type.Attributes = MemberAttributes.Public;
            type.StartDirectives.Add(
                new CodeRegionDirective(CodeRegionMode.Start, "\nstatic"));
            type.EndDirectives.Add(
                new CodeRegionDirective(CodeRegionMode.End, String.Empty));
            return type;
        }
        public static CodeMemberMethod MakeStatic(this CodeMemberMethod type)
        {
   
            type.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            type.StartDirectives.Add(
                new CodeRegionDirective(CodeRegionMode.Start, "\nstatic"));
            type.EndDirectives.Add(
                new CodeRegionDirective(CodeRegionMode.End, String.Empty));
            return type;
        }
        public static CodeTypeDeclaration Base(this CodeTypeDeclaration decleration, string baseType)
        {
            decleration.BaseTypes.Add(baseType);
            return decleration;
        }

        public static CodeTypeDeclaration Base(this CodeTypeDeclaration decleration, Type baseType)
        {
            decleration.BaseTypes.Add(baseType);
            return decleration;
        }

        public static CodeMemberMethod Method(this CodeTypeDeclaration decleration, MemberAttributes attributes,
            Type returnType, string name)
        {
            var method = new CodeMemberMethod() {Name = name, Attributes = attributes};
            decleration.Members.Add(method);
            return method;
        }

        public static CodeMemberMethod Add(this CodeMemberMethod method, string snippet, params object[] args)
        {
            method.Statements.Add(new CodeSnippetExpression(string.Format(snippet, args)));
            return method;
        }

        public static CodeTypeDeclaration ToClassDecleration(this Type type)
        {
            var classDecleration = new CodeTypeDeclaration()
            {
                Name = type.Name
            };

            ToClassDecleration(type, classDecleration);


            return classDecleration;

        }

        public static void ToClassDecleration(Type type, CodeTypeDeclaration classDecleration)
        {
            classDecleration.Name = type.Name;
            classDecleration.IsInterface = type.IsInterface;
            classDecleration.IsEnum = type.IsEnum;
            classDecleration.IsClass = type.IsClass;
            classDecleration.IsStruct = !type.IsClass && !type.IsEnum && !type.IsInterface && !type.IsPrimitive;

            if (type.BaseType != null)
            {
                classDecleration.BaseTypes.Add(new CodeTypeReference(type.BaseType));
            }
            var interfaces = type.GetInterfaces();
            foreach (var item in interfaces)
            {
                if (item.DeclaringType == type)
                    classDecleration.BaseTypes.Add(new CodeTypeReference(item));
            }
            if (type.IsAbstract && type.IsSealed)
            {
                classDecleration.Attributes |= MemberAttributes.Static;
            } else if (type.IsAbstract)
            {
                classDecleration.Attributes |= MemberAttributes.Abstract;
            }

            if (type.IsPublic)
                classDecleration.Attributes |= MemberAttributes.Public;
            else
                classDecleration.Attributes |= MemberAttributes.Private;

            
        }

        public static CodeMemberMethod MethodFromTypeMethod(this Type type, string methodName, bool callBase = true)
        {
            MethodInfo methodInfo;
            return MethodFromTypeMethod(type, methodName, out methodInfo, callBase);
        }

        public static string Value2StringExpression(this string code)
        {
            return "\"" + code + "\"";
        }

        public static CodeConstructor ToCodeConstructor(this MethodInfo constructor)
        {
            var code = new CodeConstructor()
            {
                Name = constructor.Name,
                
            };
            if (constructor.IsPublic)
            {
                code.Attributes = MemberAttributes.Public;
            }
            if (constructor.IsPrivate && !constructor.IsPublic)
            {
                code.Attributes = MemberAttributes.Private;
            }
            if (constructor.IsVirtual && !constructor.IsFinal)
            {
                code.Attributes |= MemberAttributes.Override;
            }
            else
            {
                code.Attributes |= MemberAttributes.Final;
            }

            if (constructor.IsFamily)
            {
                code.Attributes |= MemberAttributes.Family;
            }
            if (constructor.IsFamilyAndAssembly)
            {
                code.Attributes |= MemberAttributes.FamilyAndAssembly;
            }
            if (constructor.IsFamilyOrAssembly)
            {
                code.Attributes |= MemberAttributes.FamilyOrAssembly;
            }
         
            var ps = constructor.GetParameters();
            foreach (var p in ps)
            {
                var parameter = new CodeParameterDeclarationExpression(p.ParameterType, p.Name);
                code.Parameters.Add(parameter);
                //baseCall.Parameters.Add(new CodeVariableReferenceExpression(parameter.Name));
            }
            //constructor.GetParameters()
            return code;
        }

        public static bool IsOverride(this MethodInfo m)
        {
            return m.GetBaseDefinition().DeclaringType != m.DeclaringType;
        }
        public static CodeMemberMethod MethodFromTypeMethod(this Type type, string methodName, out MethodInfo methodInfo, bool callBase = true)
        {
            var m = type.GetMethods( BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance).FirstOrDefault(p => p.Name == methodName);
            methodInfo = m;
            var method = new CodeMemberMethod()
            {
                Name = methodName,
                ReturnType = new CodeTypeReference(m.ReturnType)
            };
            if (m.IsPublic)
            {
                method.Attributes = MemberAttributes.Public;
            }
            if (m.IsFamily)
            {
                method.Attributes = MemberAttributes.Family;
            }
            if (m.IsFamilyAndAssembly)
            {
                method.Attributes = MemberAttributes.FamilyAndAssembly;
            }
            if (m.IsFamilyOrAssembly)
            {
                method.Attributes = MemberAttributes.FamilyOrAssembly;
            }
            if (m.IsPrivate && !m.IsPublic)
            {
                method.Attributes = MemberAttributes.Private;
            }
            if (m.GetBaseDefinition().DeclaringType != m.DeclaringType)
            {
                method.Attributes |= MemberAttributes.Override;
            }
            if (m.IsFinal)
            {
                method.Attributes |= MemberAttributes.Final;
            }
       
           
            var baseCall = new CodeMethodInvokeExpression(new CodeBaseReferenceExpression(), methodName);
            var ps = m.GetParameters();
            foreach (var p in ps)
            {
                var parameter = new CodeParameterDeclarationExpression(p.ParameterType, p.Name);
                method.Parameters.Add(parameter);
                baseCall.Parameters.Add(new CodeVariableReferenceExpression(parameter.Name));
            }
            if (callBase && !m.IsAbstract)
            {
                method.Statements.Add(baseCall);
            }
            return method;
        }

        public static CodeMemberProperty PropertyFromTypeProperty(this Type type, string propertyName, bool callBase = true)
        {
            var property= type.GetProperty(propertyName,BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            var m = property.GetGetMethod(true);
            
            var codeProperty = new CodeMemberProperty()
            {
                Name = propertyName,
                Type = new CodeTypeReference(property.PropertyType),
                Attributes = MemberAttributes.Public,
                HasGet = true
            };
            codeProperty.HasSet = property.GetSetMethod(false) != null;
            
            if (m.IsPublic)
            {
                codeProperty.Attributes |= MemberAttributes.Public;
            } else
            if (m.IsPrivate)
            {
                codeProperty.Attributes |= MemberAttributes.Private;
            }
            if (m.IsFamily)
            {
                codeProperty.Attributes |= MemberAttributes.Family;
            }
            if (m.IsFamilyAndAssembly)
            {
                codeProperty.Attributes |= MemberAttributes.FamilyAndAssembly;
            }
            if (m.IsFamilyOrAssembly)
            {
                codeProperty.Attributes |= MemberAttributes.FamilyOrAssembly;
            }
            if (m.IsFinal || !m.IsVirtual)
            {
               codeProperty.Attributes |= MemberAttributes.Final;
                
            }
            else
            {
               
            }
            if (m.IsOverride())
            {
                codeProperty.Attributes |= MemberAttributes.Override;
            }
            return codeProperty;
        }

        public static CodeAttributeDeclaration AddArgument(this CodeAttributeDeclaration deleration, CodeExpression expression)
        {
            deleration.Arguments.Add(new CodeAttributeArgument(expression));
            return deleration;
        }
        public static CodeAttributeDeclaration AddArgument(this CodeAttributeDeclaration deleration,string name, CodeExpression expression)
        {
            deleration.Arguments.Add(new CodeAttributeArgument(name,expression));
            return deleration;
        }
        public static CodeAttributeDeclaration AddArgument(this CodeAttributeDeclaration deleration, string format, params string[] args)
        {
            deleration.Arguments.Add(new CodeAttributeArgument(new CodeSnippetExpression(string.Format(format,args))));
            return deleration;
        }
        public static CodeMemberProperty SetTypeArgument(this CodeMemberProperty property, object type, params object[] args)
        {
            property.Type.TypeArguments.Clear();

            if (type is CodeTypeReference)
            {
                property.Type.TypeArguments.Add((CodeTypeReference) type);
            }  else if (type is string)
            {
                property.Type.TypeArguments.Add(string.Format((string) type, args));
            }
            else if (type is Type)
            {
                property.Type.TypeArguments.Add((Type) type);
            }
          
            return property;
        }

        public static CodeTypeReference ToCodeReference(this object obj,params object[] args)
        {
            if (obj is CodeTypeReference)
            {
                return obj as CodeTypeReference;
            }
            else if (obj is Type)
            {
                return new CodeTypeReference((Type)obj);
            }
            else if (obj is string)
            {
                return new CodeTypeReference(string.Format((string)obj,args));
            }
            return new CodeTypeReference((Type)obj.GetType());
        }

        public static CodeSnippetExpression ToCodeSnippetExpression(this object obj)
        {
            return new CodeSnippetExpression()
            {
                Value = "\"" + obj + "\""
            };
        }

        public static CodeTypeDeclaration end(this CodeMemberMethod method)
        {
            return method.UserData["Decleration"] as CodeTypeDeclaration;
        }
        public static CodeTypeDeclaration end(this CodeMemberProperty method)
        {
            return method.UserData["Decleration"] as CodeTypeDeclaration;
        }
        public static CodeTypeDeclaration end(this CodeMemberField method)
        {
            return method.UserData["Decleration"] as CodeTypeDeclaration;
        }
        public static CodeTypeDeclaration end(this CodeMemberEvent method)
        {
            return method.UserData["Decleration"] as CodeTypeDeclaration;
        }

        #region methods
        public static CodeMemberMethod private_func(this CodeTypeDeclaration s, object returnType, string methodName, params object[] parameters)
        {
            var method = new CodeMemberMethod()
            {
                
                Attributes = MemberAttributes.Private | MemberAttributes.Final,
                Name = methodName
            };

            if (returnType == null)
            {
                method.ReturnType = new CodeTypeReference(typeof(void));
            }
            if (returnType is Type)
            {
                method.ReturnType = new CodeTypeReference((Type)returnType);
            }
            else if (returnType is CodeTypeReference)
            {
                method.ReturnType = (CodeTypeReference)returnType;
            }
            else if (returnType is string)
            {
                method.ReturnType = new CodeTypeReference((string)returnType);
            }

            for (var i = 0; i < parameters.Length; i+=2)
            {
                if (parameters.Length <= (i + 1)) break;
                var type = parameters[i];
                var name = parameters[i + 1];
                var baseType = type as Type;
                if (baseType != null)
                {
                    method.Parameters.Add(new CodeParameterDeclarationExpression(baseType, (string)name));
                }
                else
                {
                    method.Parameters.Add(new CodeParameterDeclarationExpression(type.ToString(), (string)name));
                }
                
            }
            if (!method.UserData.Contains("Decleration"))
            {
                method.UserData.Add("Decleration", s);
            }
            s.Members.Add(method);
            return method;
        }
        public static CodeMemberMethod protected_func(this CodeTypeDeclaration s, object returnType, string methodName, params object[] parameters)
        {
            var result = private_func(s, returnType, methodName, parameters);
            result.Attributes = MemberAttributes.Family | MemberAttributes.Final;
            return result;
        }

        public static CodeMemberMethod public_func(this CodeTypeDeclaration s, object returnType, string methodName, params object[] parameters)
        {
            var result = private_func(s, returnType, methodName, parameters);
            result.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            return result;
        }

        public static CodeMemberMethod invoke_base(this CodeMemberMethod m, bool insertAtBeginning = false)
        {
            
            var baseInvoke =
                new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeBaseReferenceExpression(),
                    m.Name));
            foreach (CodeParameterDeclarationExpression parameter in m.Parameters)
            {
                baseInvoke.Parameters.Add(new CodeVariableReferenceExpression(parameter.Name));
            }
            if (insertAtBeginning)
            {
                m.Statements.Insert(0, new CodeExpressionStatement(baseInvoke));
            }
            else
            {
                m.Statements.Add(baseInvoke);
            }
            
            return m;
        }
        //public static CodeMemberMethod invoke_base_tovar(this CodeMemberMethod m)
        //{
        //    var baseInvoke =
        //        new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeBaseReferenceExpression(),
        //            m.Name));
        //    foreach (CodeParameterDeclarationExpression parameter in m.Parameters)
        //    {
        //        baseInvoke.Parameters.Add(new CodeVariableReferenceExpression(parameter.Name));
        //    }
        //    m.Statements.Add(baseInvoke);
        //    return m;
        //}
        public static CodeMemberMethod protected_virtual_func(this CodeTypeDeclaration s, object returnType, string methodName, params object[] parameters)
        {
            var result = private_func(s, returnType, methodName, parameters);
            result.Attributes = MemberAttributes.Family;
            return result;
        }

        public static CodeMemberMethod public_virtual_func(this CodeTypeDeclaration s, object returnType, string methodName, params object[] parameters)
        {
            var result = private_func(s, returnType, methodName, parameters);
            result.Attributes = MemberAttributes.Public;
            return result;
        }
        public static CodeMemberMethod protected_override_func(this CodeTypeDeclaration s, object returnType, string methodName, params object[] parameters)
        {
            var result = private_func(s, returnType, methodName, parameters);
            result.Attributes = MemberAttributes.Family | MemberAttributes.Override;
            return result;
        }

        public static CodeMemberMethod public_override_func(this CodeTypeDeclaration s, object returnType, string methodName, params object[] parameters)
        {
            var result = private_func(s, returnType, methodName, parameters);
            result.Attributes = MemberAttributes.Public | MemberAttributes.Override;
            return result;
        }
        #endregion

        #region properties


        public static CodeMemberProperty private_(this CodeTypeDeclaration s, object propertyType, string propertyName, params object[] nameArgs)
        {
            var method = new CodeMemberProperty()
            {
                Attributes = MemberAttributes.Private | MemberAttributes.Final,
                Name=string.Format(propertyName,nameArgs)
            };
            if (propertyType == null)
            {
                method.Type = new CodeTypeReference(typeof(string));
            }
            var type = propertyType as Type;
            if (type != null)
            {
                method.Type = new CodeTypeReference(type);
            }
            else
            {
                var reference = propertyType as CodeTypeReference;
                if (reference != null)
                {
                    method.Type = reference;
                }
                else if (propertyType is string)
                {
                    method.Type = new CodeTypeReference((string)propertyType);
                }
            }

            if (!method.UserData.Contains("Decleration"))
            {
                method.UserData.Add("Decleration",s);
            }
            s.Members.Add(method);
            return method;
        }
        public static CodeMemberProperty protected_(this CodeTypeDeclaration s, object returnType, string propertyName, params object[] nameArgs)
        {
            var result = private_(s, returnType, propertyName, nameArgs);
            result.Attributes = MemberAttributes.Family | MemberAttributes.Final;
            return result;
        }

        public static CodeMemberProperty public_(this CodeTypeDeclaration s, object returnType, string propertyName, params object[] nameArgs)
        {
            var result = private_(s, returnType, propertyName, nameArgs);
            result.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            return result;
        }

        public static CodeMemberProperty protected_virtual_(this CodeTypeDeclaration s, object returnType, string propertyName, params object[] nameArgs)
        {
            var result = private_(s, returnType, propertyName, nameArgs);
            result.Attributes = MemberAttributes.Family;
            return result;
        }

        public static CodeMemberProperty public_virtual_(this CodeTypeDeclaration s, object returnType, string propertyName, params object[] nameArgs)
        {
            var result = private_(s, returnType, propertyName, nameArgs);
            result.Attributes = MemberAttributes.Public;
            return result;
        }
        public static CodeMemberProperty protected_override_(this CodeTypeDeclaration s, object returnType, string propertyName, params object[] nameArgs)
        {
            var result = private_(s, returnType, propertyName, nameArgs);
            result.Attributes = MemberAttributes.Family | MemberAttributes.Override;
            return result;
        }

        public static CodeMemberProperty public_override_(this CodeTypeDeclaration s, object returnType, string propertyName, params object[] nameArgs)
        {
            var result = private_(s, returnType, propertyName, nameArgs);
            result.Attributes = MemberAttributes.Public | MemberAttributes.Override;
            return result;
        }
        #endregion

        #region fields
        public static CodeMemberField _private_(this CodeTypeDeclaration s, object returnType, string fieldName, params object[] nameArgs)
        {
            var method = new CodeMemberField()
            {
                Type = returnType.ToCodeReference(),
                Attributes = MemberAttributes.Private | MemberAttributes.Final,
                Name = string.Format(fieldName,nameArgs)
            };

            if (!method.UserData.Contains("Decleration"))
            {
                method.UserData.Add("Decleration", s);
            }
            s.Members.Add(method);
            return method;
        }
        public static CodeMemberField _protected_(this CodeTypeDeclaration s, object returnType, string fieldName, params object[] nameArgs)
        {
            var result = _private_(s, returnType, fieldName, nameArgs);
            result.Attributes = MemberAttributes.Family | MemberAttributes.Final;
            return result;
        }

        public static CodeMemberField _public_(this CodeTypeDeclaration s, object returnType, string fieldName, params object[] nameArgs)
        {
            var result = _private_(s, returnType, fieldName, nameArgs);
            result.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            return result;
        }

        public static CodeMemberField _protected_virtual_(this CodeTypeDeclaration s, object returnType, string fieldName, params object[] nameArgs)
        {
            var result = _private_(s, returnType, fieldName, nameArgs);
            result.Attributes = MemberAttributes.Family;
            return result;
        }

        public static CodeMemberField _public_virtual_(this CodeTypeDeclaration s, object returnType, string fieldName, params object[] nameArgs)
        {
            var result = _private_(s, returnType, fieldName, nameArgs);
            result.Attributes = MemberAttributes.Public;
            return result;
        }
        public static CodeMemberField _protected_override_(this CodeTypeDeclaration s, object returnType, string fieldName, params object[] nameArgs)
        {
            var result = _private_(s, returnType, fieldName, nameArgs);
            result.Attributes = MemberAttributes.Family;
            return result;
        }

        public static CodeMemberField _public_override_(this CodeTypeDeclaration s, object returnType, string fieldName, params object[] nameArgs)
        {
            var result = _private_(s, returnType, fieldName, nameArgs);
            result.Attributes = MemberAttributes.Public;
            return result;
        }
        #endregion

        #region Statemenets 
        public static CodeConditionStatement _if(this CodeStatementCollection statements, string expression, params object[] args)
        {
            var condition = new CodeConditionStatement(new CodeSnippetExpression(string.Format(expression,args)));
            statements.Add(condition);
            return condition;
        }
        public static CodeMemberMethod _(this CodeMemberMethod method, string expression, params object[] args)
        {
            _(method.Statements, expression, args);
            return method;
        }

        public static CodeStatementCollection _(this CodeStatementCollection statements, string expression, params object[] args)
        {
            statements.Add(new CodeSnippetExpression(string.Format(expression, args)));
            return statements;
        }
        public static CodeMemberProperty _get(this CodeMemberProperty property, string expression, params object[] args)
        {
            property.GetStatements.Add(new CodeSnippetExpression(string.Format(expression, args)));
            property.HasGet = true;
            return property;
        }
        public static CodeMemberProperty _set(this CodeMemberProperty property, string expression, params object[] args)
        {
            property.SetStatements.Add(new CodeSnippetExpression(string.Format(expression, args)));
            property.HasSet = true;
            return property;
        }

        #endregion

        public static CodeTypeDeclaration Declare(this CodeNamespace ns, MemberAttributes attributes, string name)
        {
            var decl = new CodeTypeDeclaration
            {
                Name = name,
                Attributes = MemberAttributes.Public
            };
            ns.Types.Add(decl);
            return decl;
        }

        public static CodeTypeDeclaration Field(this CodeTypeDeclaration decleration, Type fieldType, string name,
            CodeExpression init = null, params CodeAttributeDeclaration[] customAttributes)
        {
            return Field(decleration, MemberAttributes.Private, fieldType, name, init, customAttributes);
        }

        public static CodeTypeDeclaration Field(this CodeTypeDeclaration decleration, MemberAttributes attributes,
            Type fieldType, string name, CodeExpression init = null, params CodeAttributeDeclaration[] customAttributes)
        {
            var field = new CodeMemberField(fieldType, name) {InitExpression = init, Attributes = attributes};
            if (customAttributes != null)
                field.CustomAttributes.AddRange(customAttributes);
            decleration.Members.Add(field);
            return decleration;
        }

        public static CodeTypeDeclaration EncapsulatedField(this CodeTypeDeclaration decleration, Type fieldType,
            string name, string propertyName, CodeExpression lazyValue, bool publicField = false)
        {
            var field = new CodeMemberField(fieldType, name);
            if (publicField)
            {
                field.Attributes = MemberAttributes.Public;
            }
            decleration.Members.Add(field);
            decleration.EncapsulateField(field, propertyName, lazyValue);
            return decleration;
        }

        public static CodeTypeDeclaration EncapsulateField(this CodeTypeDeclaration typeDeclaration,
            CodeMemberField field, string name, CodeExpression lazyValue, CodeExpression lazyCondition = null)
        {
            var p = new CodeMemberProperty
            {
                Name = name,
                Type = field.Type,
                HasGet = true,
                Attributes = MemberAttributes.Public
            };
            typeDeclaration.Members.Add(p);

            var r = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), field.Name);
            var lazyConditionStatement = new CodeConditionStatement();
            CodeExpression finalLazyCondition = lazyCondition;
            if (finalLazyCondition == null)
            {
                var defaultConditionStatement =
                    new CodeBinaryOperatorExpression(
                        r,
                        CodeBinaryOperatorType.ValueEquality, new CodeSnippetExpression("null"));

                finalLazyCondition = defaultConditionStatement;
            }

            lazyConditionStatement.Condition = finalLazyCondition;
            lazyConditionStatement.TrueStatements.Add(new CodeAssignStatement(r, lazyValue));

            p.GetStatements.Add(lazyConditionStatement);
            p.GetStatements.Add(
                new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
                    field.Name)));

            return typeDeclaration;
        }

        public static CodeMemberProperty EncapsulateField(this CodeMemberField field, string name)
        {
            return EncapsulateField(field, name, null, null, true);
        }

        public static CodeMemberProperty EncapsulateField(this CodeMemberField field, string name,
            CodeExpression lazyValue, CodeExpression lazyCondition = null, bool generateSetter = false)
        {
            var p = new CodeMemberProperty
            {
                Name = name,
                Type = field.Type,
                HasGet = true,
                Attributes = MemberAttributes.Public
            };
            var r = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), field.Name);

            if (lazyValue != null)
            {
                var lazyConditionStatement = new CodeConditionStatement();
                CodeExpression finalLazyCondition = lazyCondition;

                if (finalLazyCondition == null)
                {
                    var defaultConditionStatement =
                        new CodeBinaryOperatorExpression(
                            r,
                            CodeBinaryOperatorType.ValueEquality, new CodeSnippetExpression("null"));

                    finalLazyCondition = defaultConditionStatement;
                }

                lazyConditionStatement.Condition = finalLazyCondition;
                lazyConditionStatement.TrueStatements.Add(new CodeAssignStatement(r, lazyValue));
                p.GetStatements.Add(lazyConditionStatement);
            }



            p.GetStatements.Add(
                new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
                    field.Name)));
            if (generateSetter)
            {
                p.HasSet = true;
                p.SetStatements.Add(new CodeSnippetExpression(string.Format("{0} = value", field.Name)));
            }
            return p;
        }
    }
}