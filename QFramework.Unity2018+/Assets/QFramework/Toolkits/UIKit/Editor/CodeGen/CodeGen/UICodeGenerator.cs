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
			var objs = Selection.GetFiltered(typeof(GameObject), SelectionMode.Assets | SelectionMode.TopLevel);
			
			DoCreateCode(objs);
		}

		public static void DoCreateCode(Object[] objs)
		{
			mScriptKitInfo = null;

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
			if (obj != null)
			{
#pragma warning disable CS0618
				var prefabType = PrefabUtility.GetPrefabType(obj);
#pragma warning restore CS0618
#pragma warning disable CS0618
				if (PrefabType.Prefab != prefabType)
#pragma warning restore CS0618
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
			if(mScriptKitInfo != null){
				if (File.Exists(strFilePath) == false)
				{
					if(mScriptKitInfo.Templates != null && mScriptKitInfo.Templates[0] != null)
						mScriptKitInfo.Templates[0].Generate(strFilePath, behaviourName, UIKitSettingData.Load().Namespace,null);
				}
			}
			else
			{
				if (File.Exists(strFilePath) == false)
				{
					UIPanelTemplate.Write(behaviourName,strFilePath,UIKitSettingData.Load().Namespace,UIKitSettingData.Load());
				}
			}

			CreateUIPanelDesignerCode(behaviourName, strFilePath,panelCodeInfo);
			Debug.Log(">>>>>>>Success Create UIPrefab Code: " + behaviourName);
		}
		
		private void CreateUIPanelDesignerCode(string behaviourName, string uiUIPanelfilePath,PanelCodeInfo panelCodeInfo)
		{
			var dir = uiUIPanelfilePath.Replace(behaviourName + ".cs", "");
			var generateFilePath = dir + behaviourName + ".Designer.cs";
			if(mScriptKitInfo != null)
			{
				if(mScriptKitInfo.Templates != null && mScriptKitInfo.Templates[1] != null){
					mScriptKitInfo.Templates[1].Generate(generateFilePath, behaviourName, UIKitSettingData.Load().Namespace, panelCodeInfo);
				}
				mScriptKitInfo.HotScriptFilePath.CreateDirIfNotExists();
				mScriptKitInfo.HotScriptFilePath = mScriptKitInfo.HotScriptFilePath + "/" + behaviourName + mScriptKitInfo.HotScriptSuffix;
				if (File.Exists(mScriptKitInfo.HotScriptFilePath) == false && mScriptKitInfo.Templates != null &&  mScriptKitInfo.Templates[2] != null){
					mScriptKitInfo.Templates[2].Generate(mScriptKitInfo.HotScriptFilePath, behaviourName, UIKitSettingData.Load().Namespace, panelCodeInfo);
				}
			}
			else
			{
				UIPanelDesignerTemplate.Write(behaviourName,dir,UIKitSettingData.Load().Namespace,panelCodeInfo,UIKitSettingData.Load());
			}

			foreach (var elementCodeData in panelCodeInfo.ElementCodeDatas)
			{
				var elementDir = string.Empty;
				elementDir = elementCodeData.BindInfo.BindScript.GetBindType() == BindType.Element
					? (dir + behaviourName + "/").CreateDirIfNotExists()
					: (Application.dataPath + "/" + UIKitSettingData.Load().UIScriptDir + "/Components/").CreateDirIfNotExists();
				CreateUIElementCode(elementDir, elementCodeData);
			}
		}

		private static void CreateUIElementCode(string generateDirPath, ElementCodeInfo elementCodeInfo)
		{
			var panelFilePathWhithoutExt = generateDirPath + elementCodeInfo.BehaviourName;

			if (File.Exists(panelFilePathWhithoutExt + ".cs") == false)
			{
				UIElementCodeTemplate.Generate(panelFilePathWhithoutExt + ".cs",
					elementCodeInfo.BehaviourName, UIKitSettingData.Load().Namespace, elementCodeInfo);
			}

			UIElementCodeComponentTemplate.Generate(panelFilePathWhithoutExt + ".Designer.cs",
				elementCodeInfo.BehaviourName, UIKitSettingData.Load().Namespace, elementCodeInfo);

			foreach (var childElementCodeData in elementCodeInfo.ElementCodeDatas)
			{
				var elementDir = (panelFilePathWhithoutExt + "/").CreateDirIfNotExists();
				CreateUIElementCode(elementDir, childElementCodeData);
			}
		}

		private static readonly UICodeGenerator mInstance = new UICodeGenerator();
		


		private static void HotScriptBind(GameObject uiPrefab){
			if(mScriptKitInfo != null && mScriptKitInfo.CodeBind != null)
			{
				mScriptKitInfo.CodeBind.Invoke(uiPrefab,mScriptKitInfo.HotScriptFilePath);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}
		}		

		private static ScriptKitInfo mScriptKitInfo;
	}
}