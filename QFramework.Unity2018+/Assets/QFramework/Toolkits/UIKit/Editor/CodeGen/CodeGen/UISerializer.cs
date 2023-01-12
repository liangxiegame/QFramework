using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace QFramework
{
    public class UISerializer
    {
	    public static void StartAddComponent2PrefabAfterCompile(GameObject uiPrefab)
	    {
		    var prefabPath = AssetDatabase.GetAssetPath(uiPrefab);
		    if (string.IsNullOrEmpty(prefabPath))
			    return;

		    var pathStr = EditorPrefs.GetString("AutoGenUIPrefabPath");
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
	    
	    [DidReloadScripts]
	    private static void DoAddComponent2Prefab()
	    {
		    var pathStr = EditorPrefs.GetString("AutoGenUIPrefabPath");
		    if (string.IsNullOrEmpty(pathStr))
			    return;

		    EditorPrefs.DeleteKey("AutoGenUIPrefabPath");
		    Debug.Log(">>>>>>>SerializeUIPrefab: " + pathStr);

		    var assembly = GetAssemblyCSharp();

		    var paths = pathStr.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
		    var displayProgress = paths.Length > 3;
		    if (displayProgress) EditorUtility.DisplayProgressBar("", "Serialize UIPrefab...", 0);
			
		    for (var i = 0; i < paths.Length; i++)
		    {
			    var uiPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(paths[i]);
			    SetObjectRef2Property(uiPrefab, uiPrefab.name, assembly);

			    // uibehaviour
			    if (displayProgress)
				    EditorUtility.DisplayProgressBar("", "Serialize UIPrefab..." + uiPrefab.name, (float) (i + 1) / paths.Length);
			    Debug.Log(">>>>>>>Success Serialize UIPrefab: " + uiPrefab.name);
		    }

		    AssetDatabase.SaveAssets();
		    AssetDatabase.Refresh();
			
		    for (var i = 0; i < paths.Length; i++)
		    {
			    var uiPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(paths[i]);
			    SetObjectRef2Property(uiPrefab, uiPrefab.name, assembly);

			    // uibehaviour
			    if (displayProgress)
				    EditorUtility.DisplayProgressBar("", "Serialize UIPrefab..." + uiPrefab.name, (float) (i + 1) / paths.Length);
			    Debug.Log(">>>>>>>Success Serialize UIPrefab: " + uiPrefab.name);
		    }

		    AssetDatabase.SaveAssets();
		    AssetDatabase.Refresh();
			
		    if (displayProgress) EditorUtility.ClearProgressBar();
	    }
	    
	    public static void SetObjectRef2Property(GameObject obj, string behaviourName, Assembly assembly,
			List<IBindOld> processedMarks = null)
		{
			if (null == processedMarks)
			{
				processedMarks = new List<IBindOld>();
			}

			var iBind = obj.GetComponent<IBindOld>();
			var className = string.Empty;

			if (iBind != null)
			{
				var nameSpace = UIKitSettingData.LoadSettingNamespace();
				if (nameSpace != "")
				{
					className = nameSpace + "." + iBind.TypeName;
				}
				else
				{
					className = iBind.TypeName;
                }

				// 这部分
				if (iBind.GetBindType() != BindType.DefaultUnityElement)
				{
					var bind = obj.GetComponent<AbstractBind>();
					if (bind != null)
					{
						Object.DestroyImmediate(bind, true);
					}
				}
			}
			else
			{
                var nameSpace = UIKitSettingData.LoadSettingNamespace();
                if (nameSpace != "")
                {
                    className = nameSpace + "." + behaviourName;
                }
                else
                {
                    className = behaviourName;
                }
            }

			var t = assembly.GetType(className);

			var com = obj.GetComponent(t) ?? obj.AddComponent(t);
			var sObj = new SerializedObject(com);
			var bindScripts = obj.GetComponentsInChildren<IBindOld>(true);

			foreach (var elementMark in bindScripts)
			{
				if (processedMarks.Contains(elementMark) || elementMark.GetBindType() == BindType.DefaultUnityElement)
				{
					continue;
				}

				processedMarks.Add(elementMark);

				var uiType = elementMark.TypeName;
				var propertyName = string.Format("{0}", elementMark.Transform.gameObject.name);

				if (sObj.FindProperty(propertyName) == null)
				{
					Debug.LogFormat("sObj is Null:{0} {1} {2}", propertyName, uiType,sObj);
					continue;
				}

				sObj.FindProperty(propertyName).objectReferenceValue = elementMark.Transform.gameObject;
				SetObjectRef2Property(elementMark.Transform.gameObject, elementMark.TypeName, assembly, processedMarks);
			}

			var marks = obj.GetComponentsInChildren<IBindOld>(true);
			foreach (var elementMark in marks)
			{
				if (processedMarks.Contains(elementMark))
				{
					continue;
				}

				processedMarks.Add(elementMark);

				var propertyName = elementMark.Transform.name;
				sObj.FindProperty(propertyName).objectReferenceValue = elementMark.Transform.gameObject;
			}

			sObj.ApplyModifiedPropertiesWithoutUndo();
		}
	    public static Assembly GetAssemblyCSharp()
	    {
		    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
		    foreach (var a in assemblies)
		    {
			    if (a.FullName.StartsWith("Assembly-CSharp,"))
			    {
				    return a;
			    }
		    }

//            Log.E(">>>>>>>Error: Can\'t find Assembly-CSharp.dll");
		    return null;
	    }
    }
    
    
}