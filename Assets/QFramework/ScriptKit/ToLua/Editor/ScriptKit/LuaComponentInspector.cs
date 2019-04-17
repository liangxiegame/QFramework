using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using UnityEditor.SceneManagement;
using System.Text.RegularExpressions;
using Object = UnityEngine.Object;

namespace QFramework
{
	[CustomEditor(typeof(LuaComponent))]
	public class LuaComponentInspectorEditor : UnityEditor.Editor
	{
		private SerializedObject obj;

		// 添加TestInspector组件的GameObject被选中时触发该函数
		private void OnEnable()
		{
			if (obj.IsNull())
			{
				obj = new SerializedObject(target);
			}
		}

		// 重写Inspector检视面板
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			var path = obj.FindProperty("LuaPath").stringValue;
			path = path ?? "";

			EditorGUILayout.LabelField("LuaPath:");

			var boxRect = EditorGUILayout.GetControlRect();
			boxRect.height *= 1.2f;
			GUI.Box(boxRect, path);
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("请将 Lua 脚本文件拖到下边区域");
			var sfxPathRect = EditorGUILayout.GetControlRect();
			sfxPathRect.height = 200;
			GUI.Box(sfxPathRect, string.Empty);
			EditorGUILayout.LabelField(string.Empty, GUILayout.Height(185));
			if (
				Event.current.type == EventType.DragUpdated
				&& sfxPathRect.Contains(Event.current.mousePosition)
			)
			{
				//改变鼠标的外表  
				DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
				if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
				{
					if (DragAndDrop.paths[0] != "")
					{
						var newPath = DragAndDrop.paths[0];
						var resultString = Regex.Split(newPath, "/Lua/", RegexOptions.IgnoreCase);
						newPath = resultString[1];

						newPath = newPath.Replace(".lua", "");
						newPath = newPath.Replace("/", ".");

						obj.FindProperty("LuaPath").stringValue = newPath;
						obj.FindProperty("LuaFilePath").stringValue = DragAndDrop.paths[0];
						obj.ApplyModifiedPropertiesWithoutUndo();
						AssetDatabase.SaveAssets();
						AssetDatabase.Refresh();
						EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
					}
				}
			}

			if (obj.FindProperty("LuaFilePath").stringValue.IsNotNullAndEmpty())
			{
				if (GUILayout.Button("选择脚本"))
				{
					Selection.activeObject =
						AssetDatabase.LoadAssetAtPath<Object>(obj.FindProperty("LuaFilePath").stringValue);


				}
			}
		}
	}
}