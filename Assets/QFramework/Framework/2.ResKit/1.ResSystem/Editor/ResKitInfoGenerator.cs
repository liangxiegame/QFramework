/****************************************************************************
 * Copyright (c) 2017 ~ 2019.1 liangxie
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

using System.CodeDom;
using System.IO;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Collections.Generic;
using System.Linq;
using QF.Extensions;

namespace QF.Res
{
	public static class ResDataCodeGenerator
	{
		public static void WriteClass(TextWriter writer, string ns)
		{
			EditorRuntimeAssetDataCollector.BuildDataTable();
			var allAssetDataGroups = ResDatas.Instance.AllAssetDataGroups;
			
			var assetBundleInfos = new List<AssetBundleInfo>();
			
			allAssetDataGroups.ForEach(assetDataGroup =>
			{
				var assetDatas = assetDataGroup.AssetBundleDatas;

				assetDatas.ForEach(abUnit =>
				{
					assetBundleInfos.Add(new AssetBundleInfo(abUnit.abName)
					{
						assets = assetDataGroup.AssetDatas
							.Where(assetData=>assetData.OwnerBundleName == abUnit.abName)
							.Select(assetData=>assetData.AssetName)
							.ToArray()
					});	
				});
			});
			
			var compileUnit = new CodeCompileUnit();
			var codeNamespace = new CodeNamespace(ns);
			compileUnit.Namespaces.Add(codeNamespace);

			foreach (var assetBundleInfo in assetBundleInfos)
			{
				var className = assetBundleInfo.Name;
				var bundleName = className.Substring(0, 1).ToLower() + className.Substring(1);
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
				bundleNameField.InitExpression = new CodePrimitiveExpression(bundleName.ToLowerInvariant());

				var checkRepeatDict = new Dictionary<string, string>();
				foreach (var asset in assetBundleInfo.assets)
				{
					var assetField = new CodeMemberField {Attributes = MemberAttributes.Public | MemberAttributes.Const};

					var content = Path.GetFileNameWithoutExtension(asset);
					assetField.Name = content.ToUpperInvariant().Replace("@", "_").Replace("!", "_");
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