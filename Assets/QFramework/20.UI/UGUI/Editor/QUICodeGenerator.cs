/****************************************************************************
 * Copyright (c) 2017 xiaojun@putao.com
 * Copyright (c) 2017 maoling@putao.com
 * Copyright (c) 2017 liangxie
****************************************************************************/

/****************************************************************************
change log:
maoling@putao.com
Auto serialize QUIMark objects to *Components.cs which is auto generated after Unity finishing compling script. 
change log:
liqingyun@putao.com
auto add panel's monobehaivour
****************************************************************************/

namespace QFramework
{
	using System;
	using UnityEngine;
	using System.Collections.Generic;
	using UnityEditor;
	using System.IO;

	public class PanelCodeData
	{
		public Dictionary<string, string> DicNameToFullName = new Dictionary<string, string>();
		public Dictionary<string, IUIMark> DicNameToIUIData = new Dictionary<string, IUIMark>();
		public List<ElementCodeData> ElementCodeDatas = new List<ElementCodeData>();
	}

	public class ElementCodeData
	{
		public string BehaviourName;
		public Dictionary<string, string> DicNameToFullName = new Dictionary<string, string>();
		public Dictionary<string, IUIMark> DicNameToIUIData = new Dictionary<string, IUIMark>();
		public List<ElementCodeData> ElementCodeDatas = new List<ElementCodeData>();
	}

	public class QUICodeGenerator
	{
		[MenuItem("Assets/QUIFramework - Create UICode")]
		public static void CreateUICode()
		{
			UnityEngine.Object[] objs = Selection.GetFiltered(typeof(GameObject), SelectionMode.Assets | SelectionMode.TopLevel);
			bool displayProgress = objs.Length > 1;
			if (displayProgress) EditorUtility.DisplayProgressBar("", "Create UIPrefab Code...", 0);
			for (int i = 0; i < objs.Length; i++)
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
			if (null != obj)
			{
				PrefabType prefabType = PrefabUtility.GetPrefabType(obj);
				if (PrefabType.Prefab != prefabType)
				{
					return;
				}
				GameObject clone = PrefabUtility.InstantiatePrefab(obj) as GameObject;
				if (null == clone)
				{
					return;
				}

				mPanelCodeData = new PanelCodeData();

				FindAllMarkTrans(clone.transform, "");
				CreateUIPanelCode(obj, uiPrefabPath);

				AddSerializeUIPrefab(obj, mPanelCodeData);

				GameObject.DestroyImmediate(clone);
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="rootTrans"></param>
		/// <param name="curTrans"></param>
		/// <param name="transFullName"></param>
		private void FindAllMarkTrans(Transform curTrans, string transFullName, ElementCodeData parentElementCodeData = null)
		{
			foreach (Transform childTrans in curTrans)
			{
				IUIMark uiMark = childTrans.GetComponent<IUIMark>();

				if (null != uiMark)
				{
					if (null == parentElementCodeData)
					{
						if (!mPanelCodeData.DicNameToIUIData.ContainsKey(uiMark.Transform.name))
						{
							mPanelCodeData.DicNameToIUIData.Add(uiMark.Transform.name, uiMark);
							mPanelCodeData.DicNameToFullName.Add(uiMark.Transform.name, transFullName + childTrans.name);
						}
						else
						{
							Debug.LogError("Repeat key: " + childTrans.name);
						}
					}
					else
					{
						if (!parentElementCodeData.DicNameToIUIData.ContainsKey(uiMark.Transform.name))
						{
							parentElementCodeData.DicNameToIUIData.Add(uiMark.Transform.name, uiMark);
							parentElementCodeData.DicNameToFullName.Add(uiMark.Transform.name, transFullName + childTrans.name);
						}
						else
						{
							Debug.LogError("Repeat key: " + childTrans.name);
						}
					}
				}

				if (uiMark is QUIElement)
				{
					ElementCodeData elementCodeData = new ElementCodeData();
					elementCodeData.BehaviourName = uiMark.ComponentName;
					if (null == parentElementCodeData)
					{
						mPanelCodeData.ElementCodeDatas.Add(elementCodeData);
					}
					else
					{
						parentElementCodeData.ElementCodeDatas.Add(elementCodeData);
					}

					FindAllMarkTrans(childTrans, transFullName + childTrans.name + "/", elementCodeData);
				}
				else
				{
					FindAllMarkTrans(childTrans, transFullName + childTrans.name + "/", parentElementCodeData);
				}
			}
		}

		private void CreateUIPanelCode(GameObject uiPrefab, string uiPrefabPath)
		{
			if (null == uiPrefab)
				return;

			string behaviourName = uiPrefab.name;
			string strFilePath = uiPrefabPath.Replace(QFrameworkConfigData.Load().UIPrefabDir, GetScriptsPath());
			strFilePath.Replace(uiPrefab.name + ".prefab", "").CreateDirIfNotExists();
			strFilePath = strFilePath.Replace(".prefab", ".cs");

			if (File.Exists(strFilePath) == false)
			{
				UIPanelCodeTemplate.Generate(strFilePath, behaviourName, GetProjectNamespace());
			}

			CreateUIPanelComponentsCode(behaviourName, strFilePath);
			Debug.Log(">>>>>>>Success Create UIPrefab Code: " + behaviourName);
		}

		private void CreateUIPanelComponentsCode(string behaviourName, string uiUIPanelfilePath)
		{
			string dir = uiUIPanelfilePath.Replace(behaviourName + ".cs", "");
			string generateFilePath = dir + behaviourName + "Components.cs";

			UIPanelComponentsCodeTemplate.Generate(generateFilePath, behaviourName, GetProjectNamespace(), mPanelCodeData);

			foreach (var elementCodeData in mPanelCodeData.ElementCodeDatas)
			{
				string elementDir = IOExtension.CreateDirIfNotExists(dir + behaviourName + "/");
				CreateUIElementCode(elementDir, elementCodeData);
			}
		}

		private void CreateUIElementCode(string generateDirPath, ElementCodeData elementCodeData)
		{
			UIElementCodeTemplate.Generate(generateDirPath + elementCodeData.BehaviourName + "Components.cs",
				elementCodeData.BehaviourName, GetProjectNamespace(), elementCodeData);


			foreach (var childElementCodeData in elementCodeData.ElementCodeDatas)
			{
				string elementDir = IOExtension.CreateDirIfNotExists(generateDirPath + elementCodeData.BehaviourName + "/");
				CreateUIElementCode(elementDir, childElementCodeData);
			}
		}

		private void AddSerializeUIPrefab(GameObject uiPrefab, PanelCodeData panelData)
		{
			string prefabPath = AssetDatabase.GetAssetPath(uiPrefab);
			if (string.IsNullOrEmpty(prefabPath))
				return;

			string pathStr = EditorPrefs.GetString("AutoGenUIPrefabPath");
			if (string.IsNullOrEmpty(pathStr))
			{
				pathStr = prefabPath;
			}
			else
			{
				pathStr += ";" + prefabPath;
			}

			EditorPrefs.SetString("AutoGenUIPrefabPath", pathStr);
		}

		[UnityEditor.Callbacks.DidReloadScripts]
		private static void SerializeUIPrefab()
		{
			string pathStr = EditorPrefs.GetString("AutoGenUIPrefabPath");
			if (string.IsNullOrEmpty(pathStr))
				return;

			EditorPrefs.DeleteKey("AutoGenUIPrefabPath");
			Debug.Log(">>>>>>>SerializeUIPrefab: " + pathStr);
			 
			var assembly = ReflectionExtension.GetAssemblyCSharp();

			string[] paths = pathStr.Split(new char[] {';'}, StringSplitOptions.RemoveEmptyEntries);
			bool displayProgress = paths.Length > 3;
			if (displayProgress) EditorUtility.DisplayProgressBar("", "Serialize UIPrefab...", 0);
			for (int i = 0; i < paths.Length; i++)
			{
				GameObject uiPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(paths[i]);
				AttachSerializeObj(uiPrefab, uiPrefab.name, assembly);

				// uibehaviour
				string className = GetProjectNamespace() + "." + uiPrefab.name;
				var t = assembly.GetType(className);
				var com = uiPrefab.GetComponent(t);
				if (null == com)
					com = uiPrefab.AddComponent(t);

				if (displayProgress)
					EditorUtility.DisplayProgressBar("", "Serialize UIPrefab..." + uiPrefab.name, (float) (i + 1) / paths.Length);
				Debug.Log(">>>>>>>Success Serialize UIPrefab: " + uiPrefab.name);
			}

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			if (displayProgress) EditorUtility.ClearProgressBar();
		}

		private static void AttachSerializeObj(GameObject obj, string behaviourName, System.Reflection.Assembly assembly,
			List<IUIMark> processedMarks = null)
		{
			if (null == processedMarks)
			{
				processedMarks = new List<IUIMark>();
			}

			string className = GetProjectNamespace() + "." + behaviourName + "Components";
			Type t = assembly.GetType(className);
			var com = obj.GetComponent(t);
			if (com == null)
				com = obj.AddComponent(t);
			SerializedObject sObj = new SerializedObject(com);
			QUIElement[] uiMarks = obj.GetComponentsInChildren<QUIElement>(true);

			foreach (QUIElement elementMark in uiMarks)
			{
				if (processedMarks.Contains(elementMark))
				{
					continue;
				}

				processedMarks.Add(elementMark);

				string uiType = elementMark.ComponentName;
				string propertyName = string.Format("{0}", elementMark.Transform.gameObject.name);

				if (sObj.FindProperty(propertyName) == null)
				{
					Log.I("sObj is Null:{0}", propertyName);
					continue;
				}

				sObj.FindProperty(propertyName).objectReferenceValue = elementMark.Transform.gameObject;
				AttachSerializeObj(elementMark.Transform.gameObject, elementMark.ComponentName, assembly, processedMarks);
			}

			QUIMark[] marks = obj.GetComponentsInChildren<QUIMark>(true);
			foreach (QUIMark elementMark in marks)
			{
				if (processedMarks.Contains(elementMark))
				{
					continue;
				}

				processedMarks.Add(elementMark);

				string uiType = elementMark.ComponentName;
				string propertyName = string.Format("{0}", elementMark.Transform.gameObject.name);
				Log.I(propertyName);
				sObj.FindProperty(propertyName).objectReferenceValue = elementMark.Transform.gameObject;
			}

			sObj.ApplyModifiedPropertiesWithoutUndo();
		}

		private static string GetScriptsPath()
		{
			return QFrameworkConfigData.Load().UIScriptDir;
		}

		private static string GetProjectNamespace()
		{
			return QFrameworkConfigData.Load().Namespace;
		}

		private PanelCodeData mPanelCodeData = null;

		static private QUICodeGenerator mInstance = new QUICodeGenerator();
	}
}