using System.CodeDom;
using System.IO;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Collections.Generic;

namespace QFramework
{
	public static class QBundleInfoGenerator
	{
		public static void WriteClass(TextWriter writer, string ns, List<AssetBundleInfo> assetBundleInfos)
		{
			var compileUnit = new CodeCompileUnit();
			var codeNamespace = new CodeNamespace(ns);
			compileUnit.Namespaces.Add(codeNamespace);

			foreach (var assetBundleInfo in assetBundleInfos)
			{
				var className = assetBundleInfo.Name;
				var bundleName = className.Substring(0, 1).ToUpper() + className.Substring(1);
				int firstNumber;
				if (int.TryParse(bundleName[0].ToString(), out firstNumber))
				{
					continue;
				}

				className = className.Substring(0, 1).ToUpper() +
				            className.Substring(1).Replace("/", "_").Replace("@", "_").Replace("!", "_");

				var codeType = new CodeTypeDeclaration(className);
				codeNamespace.Types.Add(codeType);

				var bundleNameField = new CodeMemberField
				{
					Attributes = MemberAttributes.Public | MemberAttributes.Const,
					Name = "BundleName",
					Type = new CodeTypeReference(typeof(System.String))
				};
				codeType.Members.Add(bundleNameField);
				bundleNameField.InitExpression = new CodePrimitiveExpression(bundleName.ToUpperInvariant());

				var checkRepeatDict = new Dictionary<string, string>();
				foreach (var asset in assetBundleInfo.assets)
				{
					var assetField = new CodeMemberField {Attributes = MemberAttributes.Public | MemberAttributes.Const};

					var content = Path.GetFileNameWithoutExtension(asset).ToUpperInvariant();
					assetField.Name = content.Replace("@", "_").Replace("!", "_");
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
	}
}