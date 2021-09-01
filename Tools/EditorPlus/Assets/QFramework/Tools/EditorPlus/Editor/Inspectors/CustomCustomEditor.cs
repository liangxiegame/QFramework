// /****************************************************************************
//  * Copyright (c) 2021 Karsion(拖鞋)
//  * Date: 2021-09-01 10:00
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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace QFramework
{
	/// <summary>
	///     A base class for creating editors that decorate Unity's built-in editor types.
	/// </summary>
	public abstract class CustomCustomEditor : Editor
	{
		// empty array for invoking methods using reflection
		private static readonly object[] EMPTY_ARRAY = new object[0];

		private static readonly Dictionary<string, MethodInfo> decoratedMethods = new Dictionary<string, MethodInfo>();

		private static readonly Assembly editorAssembly = Assembly.GetAssembly(typeof(Editor));

		protected CustomCustomEditor(string editorTypeName)
		{
			decoratedEditorType = editorAssembly.GetTypes().FirstOrDefault(t => t.Name == editorTypeName);
			Init();

			// Check CustomEditor types.
			Type originalEditedType = GetCustomEditorType(decoratedEditorType);
			if (originalEditedType != editedObjectType)
			{
				throw new ArgumentException(
					$"Type {editedObjectType} does not match the editor {editorTypeName} type {originalEditedType}");
			}
		}

		protected Editor EditorInstance
		{
			get
			{
				if (editorInstance == null && targets != null && targets.Length > 0)
				{
					editorInstance = CreateEditor(targets, decoratedEditorType);
				}

				if (editorInstance == null) { Debug.LogError("Could not create editor !"); }

				return editorInstance;
			}
		}

		private static Type GetCustomEditorType(Type type)
		{
			const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;

			CustomEditor[] attributes = type.GetCustomAttributes(typeof(CustomEditor), true) as CustomEditor[];

			FieldInfo field = attributes?.Select(editor => editor.GetType().GetField("m_InspectedType", flags)).First();
			return field?.GetValue(attributes[0]) as Type;
		}

		private void Init() { editedObjectType = GetCustomEditorType(GetType()); }

		protected virtual void OnEnable()
		{
			if (editorInstance) { CallInspectorMethod("OnEnable"); }
		}

		protected virtual void OnDisable()
		{
			if (editorInstance != null) { DestroyImmediate(editorInstance); }
		}

		protected void CallFieldMethod(string fieldName, string methodName, Type[] types, params object[] parameters)
		{
			Type type = EditorInstance.GetType();
			FieldInfo fieldInfo = type.GetField(fieldName,
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			type = fieldInfo.FieldType;
			MethodInfo methodInfo = type.GetMethod(methodName, types);
			methodInfo.Invoke(fieldInfo.GetValue(EditorInstance), parameters);
		}

		protected void SetFieldValue(string fieldName, string valueName, object value)
		{
			Type type = EditorInstance.GetType();
			FieldInfo fieldInfo = type.GetField(fieldName,
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			type = fieldInfo.FieldType;
			FieldInfo fieldInfo2 = type.GetField(valueName,
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			fieldInfo2.SetValue(fieldInfo.GetValue(EditorInstance), value);
		}

		/// <summary>
		///     Delegates a method call with the given name to the decorated editor instance.
		/// </summary>
		protected void CallInspectorMethod(string methodName, Editor editor)
		{
			MethodInfo method = null;

			// Add MethodInfo to cache
			if (!decoratedMethods.ContainsKey(methodName))
			{
				const BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic |
				                           BindingFlags.Public;

				method = decoratedEditorType.GetMethod(methodName, flags);

				if (method != null) { decoratedMethods[methodName] = method; }
				else { Debug.LogError($"Could not find method {(MethodInfo) null}"); }
			}
			else { method = decoratedMethods[methodName]; }

			method?.Invoke(editor, EMPTY_ARRAY);
		}

		protected void CallInspectorMethod(string methodName) { CallInspectorMethod(methodName, editorInstance); }

		protected virtual void OnSceneGUI()
		{
			if (editorInstance) { CallInspectorMethod("OnSceneGUI"); }
		}

		protected override void OnHeaderGUI()
		{
			if (editorInstance) { CallInspectorMethod("OnHeaderGUI"); }
		}

		public override void OnInspectorGUI() { EditorInstance.OnInspectorGUI(); }

		public override void DrawPreview(Rect previewArea) { EditorInstance.DrawPreview(previewArea); }

		public override string GetInfoString() => EditorInstance.GetInfoString();

		public override GUIContent GetPreviewTitle() => EditorInstance.GetPreviewTitle();

		public override bool HasPreviewGUI() => EditorInstance.HasPreviewGUI();

		public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
		{
			EditorInstance.OnInteractivePreviewGUI(r, background);
		}

		public override void OnPreviewGUI(Rect r, GUIStyle background) { EditorInstance.OnPreviewGUI(r, background); }

		public override void OnPreviewSettings() { EditorInstance.OnPreviewSettings(); }

		public override void ReloadPreviewInstances() { EditorInstance.ReloadPreviewInstances(); }

		public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height) =>
			EditorInstance.RenderStaticPreview(assetPath, subAssets, width, height);

		public override bool RequiresConstantRepaint() => EditorInstance.RequiresConstantRepaint();

		public override bool UseDefaultMargins() => EditorInstance.UseDefaultMargins();

		#region Editor Fields

		/// <summary>
		///     Type object for the internally used (decorated) editor.
		/// </summary>
		private readonly Type decoratedEditorType;

		/// <summary>
		///     Type object for the object that is edited by this editor.
		/// </summary>
		private Type editedObjectType;

		private Editor editorInstance;

		#endregion
	}
}