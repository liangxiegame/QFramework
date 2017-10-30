using System.CodeDom;
using System.IO;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework.ResSystem
{
	public  class QBundleInfoGenerator
	{

		public static void WriteClass (TextWriter writer, string ns, List<AssetBundleInfo> assetBundleInfos,string projectTag=null)
		{

			var compileUnit = new CodeCompileUnit ();
			var codeNamespace = new CodeNamespace (ns);
			compileUnit.Namespaces.Add (codeNamespace);

			for(int i=0;i<assetBundleInfos.Count;i++){
				AssetBundleInfo assetBundleInfo = assetBundleInfos [i];
				string className = assetBundleInfo.name;
				string bundleName = className.Substring (0, 1).ToUpper () + className.Substring (1);
				int firstNumber;
				if (int.TryParse(bundleName[0].ToString(), out firstNumber))
				{
					continue;
				}

				className = className.Substring(0,1).ToUpper()+className.Substring(1).Replace("/","_").Replace("@","_").Replace("!","_");
				if (!string.IsNullOrEmpty (projectTag)) {
					className = className.Replace ("_project_" + projectTag, "");
					bundleName = bundleName.Replace ("_project_" + projectTag, "");
				}

				var codeType = new CodeTypeDeclaration (className);
				codeNamespace.Types.Add (codeType);

				CodeMemberField bundleNameField = new CodeMemberField();
				bundleNameField.Attributes = MemberAttributes.Public|MemberAttributes.Const;
				bundleNameField.Name = "BundleName";
				bundleNameField.Type = new CodeTypeReference(typeof(System.String));
				codeType.Members.Add(bundleNameField);
				bundleNameField.InitExpression  =new CodePrimitiveExpression(bundleName.ToUpperInvariant());

				Dictionary<string,string> checkRepeatDict = new Dictionary<string, string> ();
				foreach (var asset in assetBundleInfo.assets) {
						CodeMemberField assetField = new CodeMemberField ();
						assetField.Attributes = MemberAttributes.Public | MemberAttributes.Const;

						string content = Path.GetFileNameWithoutExtension (asset).ToUpperInvariant ();
						assetField.Name = content.Replace ("@", "_").Replace ("!", "_");
						assetField.Type = new CodeTypeReference (typeof(System.String));
					if (!assetField.Name.StartsWith ("[") && !assetField.Name.StartsWith (" [") && !checkRepeatDict.ContainsKey(assetField.Name)) {
						checkRepeatDict.Add (assetField.Name, asset);
						codeType.Members.Add (assetField);
					}
					assetField.InitExpression = new CodePrimitiveExpression (content);
					
				}
				checkRepeatDict.Clear ();
			}

			var provider = new CSharpCodeProvider ();
			var options = new CodeGeneratorOptions ();
			options.BlankLinesBetweenMembers = false;
			options.BracingStyle = "C";

			provider.GenerateCodeFromCompileUnit (compileUnit, writer, options);

		}


	}
}