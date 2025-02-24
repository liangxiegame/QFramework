/****************************************************************************
 * Copyright (c) 2015 ~ 2025 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace QFramework
{
    public class ViewControllerCodeTemplate
    {
        public static void Generate(CodeGenTask task)
        {
            var viewController = task.GameObject.GetComponent<ViewController>();

            var root = new RootCode()
                .Using("UnityEngine")
                .Using("QFramework")
                .EmptyLine();

            if (CodeGenKit.Setting.IsDefaultNamespace)
            {
                root.Custom("// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间");
                root.Custom("// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改");
            }

            var namespaceName = (string.IsNullOrWhiteSpace(task.Namespace))
                ? CodeGenKit.Setting.Namespace
                : task.Namespace;

            root.Namespace(namespaceName, ns =>
            {
                var hasParent = viewController.ViewControllerFullTypeName.IsNotNullAndEmpty();
                var parentClassName = hasParent ? viewController.ViewControllerFullTypeName : "ViewController";
                ns.Class(task.ClassName, parentClassName, true, false, cs =>
                {
                    if (parentClassName == "ViewController")
                    {
                        cs.CustomScope("void Start()", false, scope => { scope.Custom("// Code Here"); });
                    }

                    if (hasParent)
                    {
                        var parentType = ViewControllerInspector.SearchAllViewControllerTypes().FirstOrDefault(t =>
                            t.FullName == viewController.ViewControllerFullTypeName);

                        if (parentType != null)
                        {
                            if (parentType.IsAbstract)
                            {
                                foreach (var property in parentType.GetProperties(BindingFlags.Instance |
                                                 BindingFlags.NonPublic | BindingFlags.Public)
                                             .Where(p =>
                                                 p.GetGetMethod() != null && p.GetGetMethod().IsAbstract ||
                                                 p.GetGetMethod(true) != null && p.GetGetMethod(true).IsAbstract ||
                                                 p.GetSetMethod() != null && p.GetSetMethod().IsAbstract ||
                                                 p.GetSetMethod(true) != null && p.GetSetMethod(true).IsAbstract))
                                {
                                    var getMethod = property.GetGetMethod() ?? property.GetGetMethod(true);
                                    var setMethod = property.GetSetMethod() ?? property.GetSetMethod(true);
                                    if (getMethod != null && setMethod != null)
                                    {
                                        if (GetVisibility(getMethod) == GetVisibility(setMethod))
                                        {
                                            cs.Custom(
                                                $"{GetVisibility(getMethod)} override {GetPrettyTypeName(property.PropertyType)} {property.Name} {{ get; set; }}");
                                        }
                                        else
                                        {
                                            if (getMethod.IsPublic || getMethod.IsAssembly && setMethod.IsFamily)
                                            {
                                                cs.Custom(
                                                    $"{GetVisibility(getMethod)} override {GetPrettyTypeName(property.PropertyType)} {property.Name} {{ get; {GetVisibility(setMethod)} set; }}");
                                            }
                                            else if (setMethod.IsPublic || setMethod.IsAssembly && getMethod.IsFamily)
                                            {
                                                cs.Custom(
                                                    $"{GetVisibility(setMethod)} override {GetPrettyTypeName(property.PropertyType)} {property.Name} {{ {GetVisibility(getMethod)} get; set; }}");
                                            }
                                            else
                                            {
                                            }
                                        }
                                    }
                                    else if (getMethod != null)
                                    {
                                        cs.Custom(
                                            $"{GetVisibility(getMethod)} override {GetPrettyTypeName(property.PropertyType)} {property.Name} {{ get; }}");
                                    }
                                    else if (setMethod != null)
                                    {
                                        cs.Custom(
                                            $"{GetVisibility(setMethod)} override {GetPrettyTypeName(property.PropertyType)} {property.Name} {{ set; }}");
                                    }

                                    cs.EmptyLine();
                                }
                            }
                        }


                        if (parentType != null)
                        {
                            if (parentType.IsAbstract)
                            {
                                foreach (var method in parentType.GetMethods(BindingFlags.Instance |
                                                                             BindingFlags.Public |
                                                                             BindingFlags.NonPublic)
                                             .Where(m => m.IsAbstract && !m.Name.StartsWith("get_") &&
                                                         !m.Name.StartsWith("set_")))
                                {
                                    cs.EmptyLine();
                                    var parameterString = method.GetParameters()
                                        .Select(p => $"{GetPrettyTypeName(p.ParameterType)} {p.Name}").StringJoin(",");

                                    cs.CustomScope(
                                        $"{GetVisibility(method)} override {GetPrettyTypeName(method.ReturnType)} {method.Name}({parameterString})",
                                        false,
                                        scope => { scope.Custom("throw new System.NotImplementedException();"); });
                                }
                            }
                        }
                    }
                });
            });


            var scriptFile = string.Format(task.ScriptsFolder + "/{0}.cs", (task.ClassName));

            if (!File.Exists(scriptFile))
            {
                scriptFile.GetFolderPath().CreateDirIfNotExists();
                using (var fileWriter = File.CreateText(scriptFile))
                {
                    root.Gen(new FileCodeWriter(fileWriter));
                }
            }
        }

        static string GetPrettyTypeName(Type type)
        {
            if (type == typeof(float)) return "float";
            else if (type == typeof(int)) return "int";
            else if (type == typeof(long)) return "long";
            else if (type == typeof(double)) return "double";
            else if (type == typeof(string)) return "string";
            else if (type == typeof(bool)) return "bool";
            else if (type.FullName == "System.Void") return "void";
            else return type.FullName;
        }


        static string GetVisibility(MethodInfo m)
        {
            string visibility = "";
            if (m.IsPublic)
                return "public";
            else if (m.IsPrivate)
                return "private";
            else if (m.IsFamily)
                visibility = "protected";
            else if (m.IsAssembly)
                visibility += "internal";
            return visibility;
        }
    }
}
#endif