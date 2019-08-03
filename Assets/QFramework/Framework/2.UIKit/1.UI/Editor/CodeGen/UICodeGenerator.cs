/****************************************************************************
 * Copyright (c) 2017 xiaojun、imagicbell
 * Copyright (c) 2017 ~ 2019.7 liangxie 
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

using System.Linq;
using QF.Extensions;
using QF.GraphDesigner;

namespace QFramework
{
	using UnityEngine;
	using UnityEditor;
	using System.IO;

	public class UICodeGenerator
	{
		[MenuItem("Assets/@UI Kit - Create UICode (alt+c) &c")]
		public static void CreateUICode()
		{
			mScriptKitInfo = null;
			var objs = Selection.GetFiltered(typeof(GameObject), SelectionMode.Assets | SelectionMode.TopLevel);
			var displayProgress = objs.Length > 1;
			if (displayProgress) EditorUtility.DisplayProgressBar("", "Create UIPrefab Code...", 0);
			for (var i = 0; i < objs.Length; i++)
			{
				mInstance.CreateCode(objs[i] as GameObject, AssetDatabase.GetAssetPath(objs[i]));
				if (displayProgress)
					EditorUtility.DisplayProgressBar("", "Create UIPrefab Code...", (float) (i + 1) / objs.Length);
			}

			AssetDatabase.Refresh();
			if (displayProgress) EditorUtility.ClearProgressBar();
		}

		private void CreateCode(GameObject obj, string uiPrefabPath)
		{
			if (obj.IsNotNull())
			{
				var prefabType = PrefabUtility.GetPrefabType(obj);
				if (PrefabType.Prefab != prefabType)
				{
					return;
				}

				var clone = PrefabUtility.InstantiatePrefab(obj) as GameObject;
				if (null == clone)
				{
					return;
				}
				

				var panelCodeInfo = new PanelCodeInfo();

				Debug.Log(clone.name);
				panelCodeInfo.GameObjectName = clone.name.Replace("(clone)", string.Empty);
				BindCollector.SearchBinds(clone.transform, "",panelCodeInfo);
				CreateUIPanelCode(obj, uiPrefabPath,panelCodeInfo);
				
				UISerializer.StartAddComponent2PrefabAfterCompile(obj);

				HotScriptBind(obj);
				
				Object.DestroyImmediate(clone);
			}
		}

		private void CreateUIPanelCode(GameObject uiPrefab, string uiPrefabPath,PanelCodeInfo panelCodeInfo)
		{
			if (null == uiPrefab)
				return;

			var behaviourName = uiPrefab.name;

			var strFilePath = CodeGenUtil.GenSourceFilePathFromPrefabPath(uiPrefabPath, behaviourName);
			if(mScriptKitInfo.IsNotNull()){
				if (File.Exists(strFilePath) == false)
				{
					if(mScriptKitInfo.Templates.IsNotNull() && mScriptKitInfo.Templates[0].IsNotNull())
						mScriptKitInfo.Templates[0].Generate(strFilePath, behaviourName, UIKitSettingData.GetProjectNamespace(),null);
				}
			}
			else
			{
				if (File.Exists(strFilePath) == false)
				{
					RegisteredTemplateGeneratorsFactory.RegisterTemplate<PanelCodeInfo,UIPanelDataTemplate>();
					RegisteredTemplateGeneratorsFactory.RegisterTemplate<PanelCodeInfo,UIPanelTemplate>();
					
					var factory = new RegisteredTemplateGeneratorsFactory();
					
					var generators = factory.CreateGenerators(new UIGraph(), panelCodeInfo);
									
					CompilingSystem.GenerateFile(new FileInfo(strFilePath),new CodeFileGenerator(UIKitSettingData.GetProjectNamespace())
					{
						Generators = generators.ToArray()
					});

					RegisteredTemplateGeneratorsFactory.UnRegisterTemplate<PanelCodeInfo>();
				}
			}

			CreateUIPanelDesignerCode(behaviourName, strFilePath,panelCodeInfo);
			Debug.Log(">>>>>>>Success Create UIPrefab Code: " + behaviourName);
		}
		
		private void CreateUIPanelDesignerCode(string behaviourName, string uiUIPanelfilePath,PanelCodeInfo panelCodeInfo)
		{
			var dir = uiUIPanelfilePath.Replace(behaviourName + ".cs", "");
			var generateFilePath = dir + behaviourName + ".Designer.cs";
			if(mScriptKitInfo.IsNotNull())
			{
				if(mScriptKitInfo.Templates.IsNotNull() && mScriptKitInfo.Templates[1].IsNotNull()){
					mScriptKitInfo.Templates[1].Generate(generateFilePath, behaviourName, UIKitSettingData.GetProjectNamespace(), panelCodeInfo);
				}
				mScriptKitInfo.HotScriptFilePath.CreateDirIfNotExists();
				mScriptKitInfo.HotScriptFilePath = mScriptKitInfo.HotScriptFilePath + "/" + behaviourName + mScriptKitInfo.HotScriptSuffix;
				if (File.Exists(mScriptKitInfo.HotScriptFilePath) == false && mScriptKitInfo.Templates.IsNotNull() &&  mScriptKitInfo.Templates[2].IsNotNull()){
					mScriptKitInfo.Templates[2].Generate(mScriptKitInfo.HotScriptFilePath, behaviourName, UIKitSettingData.GetProjectNamespace(), panelCodeInfo);
				}
			}
			else
			{
				RegisteredTemplateGeneratorsFactory.RegisterTemplate<PanelCodeInfo,UIPanelDesignerTemplate>();

				var factory = new RegisteredTemplateGeneratorsFactory();
					
				var generators = factory.CreateGenerators(new UIGraph(), panelCodeInfo);
									
				CompilingSystem.GenerateFile(new FileInfo(generateFilePath),new CodeFileGenerator(UIKitSettingData.GetProjectNamespace())
				{
					Generators = generators.ToArray()
				});
				
				RegisteredTemplateGeneratorsFactory.UnRegisterTemplate<PanelCodeInfo>();
			}

			foreach (var elementCodeData in panelCodeInfo.ElementCodeDatas)
			{
				var elementDir = string.Empty;
				elementDir = elementCodeData.BindInfo.BindScript.GetBindType() == BindType.Element
					? (dir + behaviourName + "/").CreateDirIfNotExists()
					: (Application.dataPath + "/" + UIKitSettingData.GetScriptsPath() + "/Components/").CreateDirIfNotExists();
				CreateUIElementCode(elementDir, elementCodeData);
			}
		}

		private static void CreateUIElementCode(string generateDirPath, ElementCodeInfo elementCodeInfo)
		{
			var panelFilePathWhithoutExt = generateDirPath + elementCodeInfo.BehaviourName;

			if (File.Exists(panelFilePathWhithoutExt + ".cs") == false)
			{
				UIElementCodeTemplate.Generate(panelFilePathWhithoutExt + ".cs",
					elementCodeInfo.BehaviourName, UIKitSettingData.GetProjectNamespace(), elementCodeInfo);
			}

			UIElementCodeComponentTemplate.Generate(panelFilePathWhithoutExt + ".Designer.cs",
				elementCodeInfo.BehaviourName, UIKitSettingData.GetProjectNamespace(), elementCodeInfo);

			foreach (var childElementCodeData in elementCodeInfo.ElementCodeDatas)
			{
				var elementDir = (panelFilePathWhithoutExt + "/").CreateDirIfNotExists();
				CreateUIElementCode(elementDir, childElementCodeData);
			}
		}

		private static readonly UICodeGenerator mInstance = new UICodeGenerator();

		#region ScriptKit 
		public static void CreateScriptUICode(ScriptKitInfo info)
		{
			mScriptKitInfo = info;
			var objs = Selection.GetFiltered(typeof(GameObject), SelectionMode.Assets | SelectionMode.TopLevel);
			var displayProgress = objs.Length > 1;
			if (displayProgress) EditorUtility.DisplayProgressBar("", "<color=#EE6A50>ScriptKit:Create ScriptUI Code...</color>", 0);
			for (var i = 0; i < objs.Length; i++)
			{
				mInstance.CreateCode(objs[i] as GameObject, AssetDatabase.GetAssetPath(objs[i]));
				if (displayProgress)
					EditorUtility.DisplayProgressBar("", "<color=#EE6A50>ScriptKit:Create ScriptUI Code...</color>", (float) (i + 1) / objs.Length);
			}

			AssetDatabase.Refresh();
			if (displayProgress) EditorUtility.ClearProgressBar();
		}	


		private static void HotScriptBind(GameObject uiPrefab){
			if(mScriptKitInfo.IsNotNull() && mScriptKitInfo.CodeBind.IsNotNull())
			{
				mScriptKitInfo.CodeBind.Invoke(uiPrefab,mScriptKitInfo.HotScriptFilePath);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}
		}		

		private static ScriptKitInfo mScriptKitInfo;
		#endregion
	}
}