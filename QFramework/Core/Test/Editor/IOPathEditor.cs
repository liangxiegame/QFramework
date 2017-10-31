/****************************************************************************
 * Copyright (c) 2017 liangxie
****************************************************************************/

using UnityEngine;
using System.IO;
using UnityEditor;

namespace QFramework
{
    public class IOPathEditor
    {
        [MenuItem("PuTaoTool/Framework/IOPath/Gen Path Asset File")]
        public static void GenPathAssetFile()
        {
			AssetDatabase.SaveAssets ();

			PathConfig data = null;

			IOUtils.CreateDirIfNotExists (EditorPathManager.DefaultPathConfigGenerateForder);

			string newConfigPath = IOEditorPathConfig.IOGeneratorPath + "/NewPathConfig.asset";

			data = AssetDatabase.LoadAssetAtPath<PathConfig>(newConfigPath);
            if (data == null)
            {
				data = ScriptableObject.CreateInstance<PathConfig>();
				AssetDatabase.CreateAsset(data, newConfigPath);
            }

            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
		}

		[MenuItem("PuTaoTool/Framework/IOPath/Gen Path Script")]
		public static void GeneratePathScript() 
		{
			AssetDatabase.SaveAssets ();

			IOUtils.CreateDirIfNotExists (EditorPathManager.DefaultPathScriptGenerateForder);

			string[] fullPathFileNames = Directory.GetFiles(EditorPathManager.DefaultPathConfigGenerateForder, "*PathDefine.asset", SearchOption.AllDirectories);

			foreach(string fullPathFileName in fullPathFileNames) 
			{
				Debug.Log (fullPathFileName);
				if (!fullPathFileName.EndsWith (".meta")) 
				{
					Debug.Log ("gen: " + fullPathFileName);

					PathConfig config = AssetDatabase.LoadAssetAtPath<PathConfig> (fullPathFileName);
					QNamespaceDefine nameSpace = new QNamespaceDefine ();
					nameSpace.Name = string.IsNullOrEmpty (config.NameSpace) ? "QFramework" : config.NameSpace;
					nameSpace.FileName = config.name + ".cs";
					nameSpace.GenerateDir = string.IsNullOrEmpty (config.ScriptGeneratePath) ? EditorPathManager.DefaultPathScriptGenerateForder : IOUtils.CreateDirIfNotExists ("Assets/" + config.ScriptGeneratePath);
					var classDefine = new QClassDefine ();
					classDefine.Comment = config.Description;
					classDefine.Name = config.name;
					nameSpace.Classes.Add (classDefine);
					Debug.Log (nameSpace.GenerateDir);
					foreach (var pathItem in config.List) 
					{
						if (!string.IsNullOrEmpty(pathItem.Name)) 
						{
							var variable = new QVariable (QAccessLimit.Private, QCompileType.Const, QTypeDefine.String,"m_" + pathItem.Name, pathItem.Path);
							classDefine.Variables.Add (variable);

							var property = new QProperty (QAccessLimit.Public, QCompileType.Static, QTypeDefine.String, pathItem.Name, pathItem.PropertyGetCode, pathItem.Description);
							classDefine.Properties.Add (property);
						}
					}
					QCodeGenerator.Generate (nameSpace);

					EditorUtility.SetDirty (config);
					Resources.UnloadAsset (config);

				}
					
			}
				
			AssetDatabase.SaveAssets();
		}
    }
}
