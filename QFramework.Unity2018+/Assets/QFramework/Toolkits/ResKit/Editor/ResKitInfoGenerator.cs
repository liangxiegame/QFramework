/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System.CodeDom;
using System.IO;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    public static class ResDataCodeGenerator
    {
        public static void WriteClass(TextWriter writer, string ns)
        {
            var assetBundleInfos = new List<AssetBundleInfo>();

            var assetBundleNames = AssetDatabase.GetAllAssetBundleNames();
            foreach (var assetBundleName in assetBundleNames)
            {
                var assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName);


                assetBundleInfos.Add(new AssetBundleInfo(assetBundleName)
                {
                    assets = assetPaths
                        .Select(assetName => Path.GetFileNameWithoutExtension(assetName))
                        .ToArray()
                });
            }

            var compileUnit = new CodeCompileUnit();
            var codeNamespace = new CodeNamespace(ns);
            compileUnit.Namespaces.Add(codeNamespace);

            foreach (var assetBundleInfo in assetBundleInfos)
            {
                var className = assetBundleInfo.Name;
                var bundleName = className.Substring(0, 1).ToLower() + className.Substring(1);
                if (int.TryParse(bundleName[0].ToString(), out _))
                {
                    continue;
                }

                className = className.Substring(0, 1).ToUpper() +
                            className.Substring(1)
                                .RemoveInvalidateChars();

                var codeType = new CodeTypeDeclaration(className);
                codeNamespace.Types.Add(codeType);

                var bundleNameField = new CodeMemberField
                {
                    Attributes = MemberAttributes.Public | MemberAttributes.Const,
                    Name = "BundleName",
                    Type = new CodeTypeReference(typeof(System.String))
                };
                codeType.Members.Add(bundleNameField);
                bundleNameField.InitExpression = new CodePrimitiveExpression(bundleName.ToLowerInvariant());

                var checkRepeatDict = new Dictionary<string, string>();
                foreach (var asset in assetBundleInfo.assets)
                {
                    var assetField = new CodeMemberField
                        { Attributes = MemberAttributes.Const | MemberAttributes.Public };

                    var content = Path.GetFileNameWithoutExtension(asset);

                    if (ResKitView.GenerateClassNameStyle == ResKitView.GENERATE_NAME_STYLE_UPPERCASE)
                    {
                        assetField.Name = content.ToUpperInvariant()
                            .RemoveInvalidateChars();
                    } else if (ResKitView.GenerateClassNameStyle == ResKitView.GENERATE_NAME_STYLE_KeepOriginal)
                    {
                        assetField.Name = content.RemoveInvalidateChars();
                    }

                    assetField.Type = new CodeTypeReference(typeof(System.String));
                    if (!assetField.Name.StartsWith("[") && !assetField.Name.StartsWith(" [") &&
                        !checkRepeatDict.ContainsKey(assetField.Name))
                    {
                        checkRepeatDict.Add(assetField.Name, asset);
                        codeType.Members.Add(assetField);
                    }

                    assetField.InitExpression = new CodePrimitiveExpression(content);
                }

                checkRepeatDict.Clear();
            }

            var provider = new CSharpCodeProvider();
            var options = new CodeGeneratorOptions
            {
                BlankLinesBetweenMembers = false,
                BracingStyle = "C"
            };

            provider.GenerateCodeFromCompileUnit(compileUnit, writer, options);
        }

        static string RemoveInvalidateChars(this string name)
        {
            return name.Replace("/", "")
                .Replace("@", "")
                .Replace("!", "")
                .Replace(" ", "_")
                .Replace("__", "_")
                .Replace("__", "_")
                .Replace("__", "_")
                .Replace("&", "")
                .Replace("-", "")
                .Replace("(", "")
                .Replace(")", "")
                .Replace("#", "");
        }
    }
}