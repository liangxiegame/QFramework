// /****************************************************************************
//  * Copyright (c) 2021 Karsion(拖鞋)
//  * Date: 2021-09-01 11:57
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
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace QFramework
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(RectTransform))]
	internal class RectTransformInspector : CustomCustomEditor
	{
		private float scaleAll = 1;
		private SerializedProperty spAnchoredPosition;
		private SerializedProperty spLocalPositionZ;
		private SerializedProperty spLocalRotation;
		private SerializedProperty spLocalScale;
		private SerializedProperty spPivot;
		private SerializedProperty spSizeDelta;

		internal RectTransformInspector() : base("RectTransformEditor") { }

		protected override void OnEnable()
		{
			base.OnEnable();
			spAnchoredPosition = serializedObject.FindProperty("m_AnchoredPosition");
			spLocalPositionZ = serializedObject.FindProperty("m_LocalPosition.z");
			spSizeDelta = serializedObject.FindProperty("m_SizeDelta");
			spLocalRotation = serializedObject.FindProperty("m_LocalRotation");
			spLocalScale = serializedObject.FindProperty("m_LocalScale");
			spPivot = serializedObject.FindProperty("m_Pivot");
			scaleAll = spLocalScale.FindPropertyRelative("x").floatValue;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			Contents sContents = Contents.instance;
			TransformInspector.Contents sContents2 = TransformInspector.Contents.instance;

			#region Keyboard Move

			Event e = Event.current;
			if (e != null)
			{
				if (e.type == EventType.KeyDown && e.control)
				{
					int nUnit = e.shift ? 10 : 1;
					switch (e.keyCode)
					{
						case KeyCode.UpArrow:
							Undo.RecordObjects(targets, "UpArrow");
							MoveTargetAnchoredPosition(Vector2.up * nUnit);
							e.Use();
							break;
						case KeyCode.DownArrow:
							Undo.RecordObjects(targets, "DownArrow");
							MoveTargetAnchoredPosition(Vector2.down * nUnit);
							e.Use();
							break;
						case KeyCode.LeftArrow:
							Undo.RecordObjects(targets, "LeftArrow");
							MoveTargetAnchoredPosition(Vector2.left * nUnit);
							e.Use();
							break;
						case KeyCode.RightArrow:
							Undo.RecordObjects(targets, "RightArrow");
							MoveTargetAnchoredPosition(Vector2.right * nUnit);
							e.Use();
							break;
					}
				}
			}

			TransformInspector.EventProcessing(targets);

			#endregion

			bool isAnLayout = EditorPrefs.GetBool("RectTransformEditor.showAnchorProperties");
			bool isAnyDrivenBy = false;
			foreach (RectTransform rectTransform in targets)
			{
				isAnyDrivenBy |= typeof(RectTransform).GetProperty("drivenByObject",
					                                      BindingFlags.GetProperty | BindingFlags.Instance |
					                                      BindingFlags.NonPublic)
				                                      .GetValue(rectTransform) != null;
			}

			Rect rect = new Rect();
			float start = EditorGUIUtility.wideMode
				? EditorGUIUtility.labelWidth - 2
				: EditorGUIUtility.currentViewWidth - 212 - 2;

			GUILayoutUtility.GetRect(new GUIContent(), GUIStyle.none);
			rect = GUILayoutUtility.GetLastRect();
			float currentViewWidth = rect.width;
			float up = -EditorGUIUtility.singleLineHeight + 3;
			GUILayout.Space(up);

			#region Reset Position Size

			rect.width = 18;
			rect.x = start;
#if UNITY_2017 || UNITY_2018
			rect.y += EditorGUIUtility.singleLineHeight + 2;
#elif UNITY_2019_1_OR_NEWER
			rect.y += EditorGUIUtility.singleLineHeight + 4;
#endif

			if (isAnyDrivenBy)
			{
#if UNITY_2017 || UNITY_2018
				rect.y += EditorGUIUtility.singleLineHeight + 9;
#elif UNITY_2019_1_OR_NEWER
				rect.y += EditorGUIUtility.singleLineHeight + 4;
#endif
			}

			bool enable = spAnchoredPosition.vector2Value != Vector2.zero || e.alt && spLocalPositionZ.floatValue != 0;
			using (new EditorGUI.DisabledGroupScope(!enable))
			{
				if (GUI.Button(rect, sContents2.anchoredPositionReset, sContents2.resetStyle))
				{
					spAnchoredPosition.vector2Value = Vector2.zero;
					if (e.alt) { spLocalPositionZ.floatValue = 0; }
				}
			}

			rect.y += EditorGUIUtility.singleLineHeight * 2;

			using (new EditorGUI.DisabledGroupScope(spSizeDelta.vector2Value == Vector2.zero))
			{
				if (GUI.Button(rect, sContents2.deltaSizeReset, sContents2.resetStyle))
				{
					spSizeDelta.vector2Value = Vector2.zero;
				}
			}

			#endregion

			#region Pivot

			float w = 14;
			Rect rect2 = new Rect(currentViewWidth - 32,
				rect.y + EditorGUIUtility.singleLineHeight + 1, w, w);
			if (GUI.Button(rect2, "◤", sContents.stylePivotSetup)) { spPivot.vector2Value = new Vector2(0, 1); }

			rect2.x += w;
			if (GUI.Button(rect2, EditorGUIUtility.TrTempContent(""), sContents.stylePivotSetup))
			{
				spPivot.vector2Value = new Vector2(0.5f, 1);
			}

			rect2.x += w;
			if (GUI.Button(rect2, "◥", sContents.stylePivotSetup)) { spPivot.vector2Value = new Vector2(1, 1); }

			rect2.y += w;
			if (GUI.Button(rect2, EditorGUIUtility.TrTempContent(""), sContents.stylePivotSetup))
			{
				spPivot.vector2Value = new Vector2(1, 0.5f);
			}

			rect2.x -= w;
			if (GUI.Button(rect2, "+", sContents.stylePivotSetup)) { spPivot.vector2Value = new Vector2(0.5f, 0.5f); }

			rect2.x -= w;
			if (GUI.Button(rect2, EditorGUIUtility.TrTempContent(""), sContents.stylePivotSetup))
			{
				spPivot.vector2Value = new Vector2(0, 0.5f);
			}

			rect2.y += w;
			if (GUI.Button(rect2, "◣", sContents.stylePivotSetup)) { spPivot.vector2Value = new Vector2(0, 0); }

			rect2.x += w;
			if (GUI.Button(rect2, EditorGUIUtility.TrTempContent(""), sContents.stylePivotSetup))
			{
				spPivot.vector2Value = new Vector2(0.5f, 0);
			}

			rect2.x += w;
			if (GUI.Button(rect2, "◢", sContents.stylePivotSetup)) { spPivot.vector2Value = new Vector2(1, 0); }

			#endregion

			#region Reset Rotation Scale

#if UNITY_2017 || UNITY_2018
			rect.y += EditorGUIUtility.singleLineHeight * 4 - 2;
#elif UNITY_2019_1_OR_NEWER
			rect.y += EditorGUIUtility.singleLineHeight * 4;
#endif
			rect.x = 15;
			if (isAnLayout) { rect.y += EditorGUIUtility.singleLineHeight * 2; }

			using (new EditorGUI.DisabledGroupScope(spLocalRotation.quaternionValue == Quaternion.identity))
			{
				if (GUI.Button(rect, sContents2.rotationReset, sContents2.resetStyle))
				{
					Undo.RecordObjects(targets, "rotationContent");
					MethodInfo method =
						typeof(Transform).GetMethod("SetLocalEulerAngles",
							BindingFlags.Instance | BindingFlags.NonPublic);
					object[] clear = {Vector3.zero, 0,};
					for (int i = 0; i < targets.Length; i++) { method.Invoke(targets[i], clear); }

					Event.current.type = EventType.Used;
				}
			}

			rect.y += EditorGUIUtility.singleLineHeight + 2;
			using (new EditorGUI.DisabledGroupScope(spLocalScale.vector3Value == Vector3.one))
			{
				if (GUI.Button(rect, sContents2.rotationReset, sContents2.resetStyle))
				{
					scaleAll = 1;
					spLocalScale.vector3Value = Vector3.one;
					Event.current.type = EventType.Used;
				}
			}

			#endregion

			EditorGUI.indentLevel = 1;
			base.OnInspectorGUI();
			EditorGUI.indentLevel = 0;

			#region Scale All

			GUILayout.BeginHorizontal();
			{
				EditorGUIUtility.labelWidth = 64;
				float newScale = EditorGUILayout.FloatField(sContents2.scale, scaleAll);
				if (!Mathf.Approximately(scaleAll, newScale))
				{
					scaleAll = newScale;
					spLocalScale.vector3Value = Vector3.one * scaleAll;
				}

				if (GUILayout.Button(sContents2.roundRect, GUILayout.Width(54)))
				{
					Vector2 v2 = spAnchoredPosition.vector2Value;
					spAnchoredPosition.vector2Value = new Vector2(Mathf.Round(v2.x), Mathf.Round(v2.y));
					v2 = spSizeDelta.vector2Value;
					spSizeDelta.vector2Value = new Vector2(Mathf.Round(v2.x), Mathf.Round(v2.y));
				}
			}

			GUILayout.EndHorizontal();

			#endregion

			#region Copy Paste

			GUILayout.BeginHorizontal();
			Color c = GUI.color;
			GUI.color = new Color(1f, 1f, 0.5f, 1f);
			if (GUILayout.Button(sContents.contentCopy, sContents.styleButtonLeft))
			{
				ComponentUtility.CopyComponent(target as RectTransform);
			}

			GUI.color = new Color(1f, 0.5f, 0.5f, 1f);
			if (GUILayout.Button(sContents.contentPaste, sContents.styleButtonMid))
			{
				foreach (Object item in targets) { ComponentUtility.PasteComponentValues(item as RectTransform); }
			}

			GUI.color = c;

			if (GUILayout.Button(sContents.contentFilled, sContents.styleButtonMid))
			{
				Undo.RecordObjects(targets, "F");
				foreach (Object item in targets)
				{
					RectTransform rtf = item as RectTransform;
					rtf.anchorMax = Vector2.one;
					rtf.anchorMin = Vector2.zero;
					rtf.offsetMax = Vector2.zero;
					rtf.offsetMin = Vector2.zero;
				}
			}

			if (GUILayout.Button(sContents.contentUnfilled, sContents.styleButtonRight))
			{
				Undo.RecordObjects(targets, "N");
				foreach (Object item in targets)
				{
					RectTransform rtf = item as RectTransform;
					Rect rectTemp = rtf.rect;
					rtf.anchorMax = new Vector2(0.5f, 0.5f);
					rtf.anchorMin = new Vector2(0.5f, 0.5f);
					rtf.sizeDelta = rectTemp.size;
				}
			}

			GUILayout.Label(sContents.contentReadme);
			GUILayout.EndHorizontal();

			#endregion

			TransformInspector.DrawBottomPanel(target, targets);
			serializedObject.ApplyModifiedProperties();
		}

		private void MoveTargetAnchoredPosition(Vector2 v2Unit)
		{
			foreach (Object item in targets)
			{
				RectTransform rtf = item as RectTransform;
				rtf.anchoredPosition += v2Unit;
			}
		}

		private class Contents : Singleton<Contents>
		{
			public readonly GUIContent contentReadme =
				new GUIContent("Help", "Ctrl+Arrow key move rectTransform\nCtrl: 1px    Shift: 10px");

			public readonly GUIContent contentUnfilled = new GUIContent("Unfilled", "Change to normal sizeDelta mode");
			public readonly GUIContent contentFilled = new GUIContent("Filled", "Fill the parent RectTransform");
			public readonly GUIContent contentPaste = new GUIContent("Paste", "Paste component value");
			public readonly GUIContent contentCopy = new GUIContent("Copy", "Copy component value");

			public readonly GUIStyle styleButtonLeft = new GUIStyle("ButtonLeft");
			public readonly GUIStyle styleButtonMid = new GUIStyle("ButtonMid");
			public readonly GUIStyle styleButtonRight = new GUIStyle("ButtonRight");
			public readonly GUIStyle stylePivotSetup;

			public Contents() =>
				stylePivotSetup = new GUIStyle("PreButton")
				{
					normal = new GUIStyle("CN Box").normal,
					active = new GUIStyle("AppToolbar").normal,
					overflow = new RectOffset(),
					padding = new RectOffset(1, -1, -1, 1),
					fontSize = 15,
					fixedWidth = 15,
					fixedHeight = 15,
				};
		}
	}
}