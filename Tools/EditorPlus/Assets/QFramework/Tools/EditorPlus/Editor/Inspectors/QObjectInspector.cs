// /****************************************************************************
//  * Copyright (c) 2021 Karsion(拖鞋)
//  * Date: 2021-09-01 09:57
//  *
//  * http://qframework.io
//  * https://github.com/liangxiegame/QFramework
//  * 
//  * Permission is hereby granted, free of charge, to any person obtaining a copy
//  * of this software and associated documentation files (the "Software"), to deal
//  * in the Software without restriction, including without limitation the rights
//  * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  * copies of the Software, and to permit persons to whom the Software is
//  * furnished to do so, subject to the following conditions:
//  * 
//  * The above copyright notice and this permission notice shall be included in
//  * all copies or substantial portions of the Software.
//  * 
//  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  * THE SOFTWARE.
//  ****************************************************************************/

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using UEditor = UnityEditor.Editor;

namespace QFramework.Editor
{
	public abstract class QObjectInspector : UEditor
	{
		private class Styles
		{
			public readonly GUIStyle scriptName = new GUIStyle("Button");
		}

		private static Styles sStyles;
		private ButtonExAttributeDrawer buttonExAttributeDrawer;

		public override void OnInspectorGUI()
		{
			if (!target) { return; }

			if (buttonExAttributeDrawer == null)
			{
				buttonExAttributeDrawer = new ButtonExAttributeDrawer(serializedObject);
			}

			//准备style
			if (sStyles == null) { sStyles = new Styles(); }

			CustomInspectorGUI(serializedObject, target, this, buttonExAttributeDrawer);
			EventProcessing();
		}

		private void EventProcessing()
		{
			Event e = Event.current;
			if (e != null)
			{
				if (e.type == EventType.MouseDown)
				{
					if (e.button == 2)
					{
						Undo.RecordObjects(targets, "Active");
						for (int i = 0; i < targets.Length; i++)
						{
							MonoBehaviour mono = targets[i] as MonoBehaviour;
							mono.enabled = !mono.enabled;
						}

						e.Use();
					}
					else if (e.clickCount == 2)
					{
						AutoReferencer.FindReferences(targets);
						e.Use();
					}
				}
			}
		}

		private static bool CheckShowIf(FieldInfo fieldInfos, SerializedProperty iterator, Object target)
		{
			if (fieldInfos == null) { return true; }

			Type tButtonExAttribute = typeof(ShowIfAttribute);
			if (Attribute.IsDefined(fieldInfos, tButtonExAttribute, true))
			{
				ShowIfAttribute[] exAttributes =
					fieldInfos.GetCustomAttributes(tButtonExAttribute, true) as ShowIfAttribute[];
				return ShowIfAttributeDrawer.CheckShowTargets(iterator, exAttributes[0]);
			}

			return true;
		}

		private static float GetAttributeHeight(FieldInfo fieldInfo)
		{
			if (fieldInfo == null) { return 0; }

			float height = 0;
			Type tHeaderAttribute = typeof(HeaderAttribute);
			if (Attribute.IsDefined(fieldInfo, tHeaderAttribute, true))
			{
				height += fieldInfo.GetCustomAttributes(tHeaderAttribute, true).Length * 30f;
			}

			return height;
		}

		private static void CustomInspectorGUI(SerializedObject serializedObject, Object target, QObjectInspector editor,
			ButtonExAttributeDrawer buttonExAttributeDrawer)
		{
			//这部分是使用了反编译Editor的代码
			EditorGUI.BeginChangeCheck();
			serializedObject.Update();
			SerializedProperty iterator = serializedObject.GetIterator();
			Event eventCurrent = Event.current;
			GUILayout.Space(3);
			for (bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
			{
				//using (new EditorGUI.DisabledScope(false))
				//{
				Type type = target.GetType();
				FieldInfo fieldInfos =
					type.GetField(iterator.name, BindingFlags.Instance | BindingFlags.Public |
					                             BindingFlags.NonPublic | BindingFlags.GetField);
				while (fieldInfos == null)
				{
					type = type.BaseType;
					if (type == null) { break; }

					fieldInfos = type.GetField(iterator.name,
						BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);
				}

				if (fieldInfos == null)
				{
					EditorGUILayout.PropertyField(iterator, true);
					GUILayout.Space(-21);
					GUILayout.BeginHorizontal();
					if (GUILayout.Button(EditorGUIUtility.TrTextContent("Name Go"), sStyles.scriptName,
						GUILayout.Width(EditorGUIUtility.labelWidth - 22)))
					{
						Undo.RecordObjects(Selection.objects, "Rename");
						for (int j = 0; j < serializedObject.targetObjects.Length; j++)
						{
							if (ButtonAttributeDrawer.CalledFunc(serializedObject.targetObjects[j], "NameGo"))
							{
								continue;
							}

							serializedObject.targetObjects[j].name = serializedObject.targetObjects[j].GetType().Name;
						}
					}

					//治疗双击脚本不能打开的问题
					//if (GUILayout.Button(EditorGUIUtilityEx.TempContent("Open..."), sStyles.scriptName,
					//	GUILayout.Width(EditorGUIUtility.labelWidth / 2 - 6)))
					//{
					//	AssetDatabase.OpenAsset(iterator.objectReferenceValue);
					//	GUIUtility.ExitGUI();
					//}

					GUILayout.EndHorizontal();
					buttonExAttributeDrawer.OnInspectorGUI();
					continue;
				}

				//检查ShowIf特性
				if (!CheckShowIf(fieldInfos, iterator, target)) { continue; }

				EditorGUILayout.PropertyField(iterator, true);
				Event e = Event.current;
				if (e.type == EventType.KeyDown && e.keyCode == KeyCode.C)
				{
					if (GUILayoutUtility.GetLastRect().Contains(e.mousePosition))
					{
						GUIUtility.systemCopyBuffer = iterator.name;
					}
				}
			}

			serializedObject.ApplyModifiedProperties();
			EditorGUI.EndChangeCheck();
		}
	}
}