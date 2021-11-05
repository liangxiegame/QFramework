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
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using Object = UnityEngine.Object;

namespace QFramework.Editor
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(Transform), true)]
	internal class TransformInspector : CustomCustomEditor
	{
		internal struct TransformData
		{
			private Transform transform;
			private Vector3 position;
			private Quaternion rotation;
			private Vector3 scale;

			internal TransformData RecordTransform(Transform transform)
			{
				this.transform = transform;
				position = transform.localPosition;
				rotation = transform.localRotation;
				scale = transform.localScale;
				return this;
			}

			internal void SetTransform()
			{
				if (transform)
				{
					Undo.RecordObject(transform, "SetTransform");
					transform.localPosition = position;
					transform.localRotation = rotation;
					transform.localScale = scale;
					EditorUtility.SetDirty(transform);
				}
			}
		}

		private float fScale = 1;
		private SerializedProperty spLocalPosition;
		private SerializedProperty spLocalRotation;
		private SerializedProperty spLocalScale;
		private static MethodInfo miSetLocalEulerAngles;

		internal TransformInspector()
			: base("TransformInspector") { }

		protected override void OnSceneGUI() { }

		protected override void OnEnable()
		{
			base.OnEnable();
			spLocalPosition = serializedObject.FindProperty("m_LocalPosition");
			spLocalRotation = serializedObject.FindProperty("m_LocalRotation");
			spLocalScale = serializedObject.FindProperty("m_LocalScale");
			fScale = spLocalScale.FindPropertyRelative("x").floatValue;
			if (dictTransformData == null) { dictTransformData = new Dictionary<int, TransformData>(); }
		}

		internal static Dictionary<int, TransformData> dictTransformData;

		private static void TransformRuntimeCopyClear() { dictTransformData.Clear(); }

		private static void TransformRuntimeCopy(Transform transform)
		{
			int insID = transform.GetInstanceID();
			if (!dictTransformData.ContainsKey(insID))
			{
				dictTransformData.Add(transform.GetInstanceID(), new TransformData().RecordTransform(transform));
			}
			else { dictTransformData[insID].RecordTransform(transform); }
		}

		private static void TransformRuntimeCopy(bool withChildren)
		{
			for (int i = 0; i < Selection.gameObjects.Length; i++)
			{
				Transform transform = Selection.gameObjects[i].transform;
				TransformRuntimeCopy(transform);
				if (withChildren) { transform.ActionRecursion(TransformRuntimeCopy); }
			}
		}

		private static void TransformRuntimePaste()
		{
			foreach (KeyValuePair<int, TransformData> transformData in dictTransformData)
			{
				transformData.Value.SetTransform();
			}
		}

		private static Vector3 Round(Vector3 v3Value, int nDecimalPoint = 0)
		{
			int nScale = 1;
			for (int i = 0; i < nDecimalPoint; i++) { nScale *= 10; }

			v3Value *= nScale;
			v3Value.x = Mathf.RoundToInt(v3Value.x);
			v3Value.y = Mathf.RoundToInt(v3Value.y);
			v3Value.z = Mathf.RoundToInt(v3Value.z);
			return v3Value / nScale;
		}

		/// <summary>
		///     Draw the inspector widget.
		/// </summary>
		public override void OnInspectorGUI()
		{
#if UNITY_2017 || UNITY_2018
			GUILayoutUtility.GetRect(new GUIContent(), GUIStyle.none);
			float up = -EditorGUIUtility.singleLineHeight + 3;
			GUILayout.Space(up);
			float startY = GUILayoutUtility.GetLastRect().y + up + 2;
			float startX = 10;
#elif UNITY_2019_1_OR_NEWER
			float startY = 6;
			float startX = 15;
#endif
			float labelWidthNew = 80;
			Rect rect = new Rect(startX, startY, 18, EditorGUIUtility.singleLineHeight);
			Contents sContents = Contents.instance;
			using (new EditorGUI.DisabledGroupScope(spLocalPosition.vector3Value == Vector3.zero))
			{
				if (GUI.Button(rect, sContents.positionReset, sContents.resetStyle))
				{
					spLocalPosition.vector3Value = Vector3.zero;
					Event.current.type = EventType.Used;
				}
			}

			rect.y += EditorGUIUtility.singleLineHeight + 2;
			using (new EditorGUI.DisabledGroupScope(spLocalRotation.quaternionValue.Equals(Quaternion.identity)))
			{
				if (GUI.Button(rect, sContents.rotationReset, sContents.resetStyle))
				{
					Undo.RecordObjects(targets, "rotationContent");
					if (miSetLocalEulerAngles == null)
					{
						miSetLocalEulerAngles = typeof(Transform).GetMethod("SetLocalEulerAngles",
							BindingFlags.Instance | BindingFlags.NonPublic);
					}

					object[] clear = {Vector3.zero, 0,};
					for (int i = 0; i < targets.Length; i++) { miSetLocalEulerAngles.Invoke(targets[i], clear); }

					Event.current.type = EventType.Used;
				}
			}

			rect.y += EditorGUIUtility.singleLineHeight + 2;
			using (new EditorGUI.DisabledGroupScope(spLocalScale.vector3Value == Vector3.one))
			{
				if (GUI.Button(rect, sContents.scaleReset, sContents.resetStyle))
				{
					fScale = 1;
					spLocalScale.vector3Value = Vector3.one;
					Event.current.type = EventType.Used;
				}
			}

			float labelWidthBac = EditorGUIUtility.labelWidth;
			EditorGUI.indentLevel = 1;
			bool wideMode = EditorGUIUtility.wideMode;
			EditorGUIUtility.labelWidth = labelWidthNew;
			EditorGUIUtility.wideMode = true;
			base.OnInspectorGUI();
			EditorGUI.indentLevel = 0;

			//Round
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUIUtility.labelWidth = 38;
				float newScale = EditorGUILayout.FloatField(sContents.scale, fScale);
				if (!Mathf.Approximately(fScale, newScale))
				{
					fScale = newScale;
					spLocalScale.vector3Value = Vector3.one * fScale;
				}

				EditorGUILayout.LabelField(sContents.round, GUILayout.Width(42f));
				bool isAlt = Event.current.alt;
				if (GUILayout.Button(".", "MiniButtonLeft", GUILayout.Width(16)))
				{
					Undo.RecordObjects(targets, "Round");
					for (int i = 0; i < targets.Length; i++)
					{
						Transform o = targets[i] as Transform;
						o.localPosition = Round(o.localPosition);
						if (isAlt) { o.localScale = Round(o.localScale); }

						o.localEulerAngles = Round(o.localEulerAngles);
					}
				}

				if (GUILayout.Button(".0", "MiniButtonMid", GUILayout.Width(24)))
				{
					Undo.RecordObjects(targets, "Round");
					for (int i = 0; i < targets.Length; i++)
					{
						Transform o = targets[i] as Transform;
						o.localPosition = Round(o.localPosition, 1);
						if (isAlt) { o.localScale = Round(o.localScale, 1); }

						o.localEulerAngles = Round(o.localEulerAngles, 1);
					}
				}

				if (GUILayout.Button(".00", "MiniButtonRight", GUILayout.Width(32)))
				{
					Undo.RecordObjects(targets, "Round");
					for (int i = 0; i < targets.Length; i++)
					{
						Transform o = targets[i] as Transform;
						o.localPosition = Round(o.localPosition, 2);
						if (isAlt) { o.localScale = Round(o.localScale, 2); }

						o.localEulerAngles = Round(o.localEulerAngles, 2);
					}
				}

				if (GUILayout.Button("Drop", "MiniButton"))
				{
					Undo.RecordObjects(targets, "Drop");
					for (int i = 0; i < targets.Length; i++)
					{
						Transform o = targets[i] as Transform;
						bool bRes = Physics.Raycast(o.position, Vector3.down, out RaycastHit hit);
						if (bRes) { o.position = hit.point; }
					}
				}
			}
			EditorGUILayout.EndHorizontal();

			int nSelectionCount = Selection.transforms.Length;
			if (Application.isPlaying || dictTransformData.Count > 0)
			{
				EditorGUILayout.BeginHorizontal();
				{
					if (GUILayout.Button("Record", "ButtonLeft")) { TransformRuntimeCopy(false); }

					if (GUILayout.Button("Record whit children", "ButtonLeft")) { TransformRuntimeCopy(true); }

					if (GUILayout.Button("Set", "ButtonRight")) { TransformRuntimePaste(); }

					GUILayout.Label(dictTransformData.Count.ToString());
					if (GUILayout.Button("Clear")) { TransformRuntimeCopyClear(); }
				}
				EditorGUILayout.EndHorizontal();
			}

			// Copy
			EditorGUILayout.BeginHorizontal();
			{
				Color c = GUI.color;
				GUI.color = new Color(1f, 1f, 0.5f, 1f);
				using (new EditorGUI.DisabledScope(nSelectionCount != 1))
				{
					if (GUILayout.Button("Copy", "ButtonLeft"))
					{
						TransformInspectorCopyData.localPositionCopy = spLocalPosition.vector3Value;
						TransformInspectorCopyData.localRotationCopy = spLocalRotation.quaternionValue;
						TransformInspectorCopyData.loacalScaleCopy = spLocalScale.vector3Value;
						Transform t = target as Transform;
						TransformInspectorCopyData.positionCopy = t.position;
						TransformInspectorCopyData.rotationCopy = t.rotation;
					}
				}

				bool isGlobal = Tools.pivotRotation == PivotRotation.Global;
				GUI.color = new Color(1f, 0.5f, 0.5f, 1f);
				if (GUILayout.Button("Paste", "ButtonMid"))
				{
					Undo.RecordObjects(targets, "Paste Local");
					if (isGlobal)
					{
						PastePosition();
						PasteRotation();
					}
					else
					{
						spLocalPosition.vector3Value = TransformInspectorCopyData.localPositionCopy;
						spLocalRotation.quaternionValue = TransformInspectorCopyData.localRotationCopy;
						spLocalScale.vector3Value = TransformInspectorCopyData.loacalScaleCopy;
					}
				}

				if (GUILayout.Button("PPos", "ButtonMid"))
				{
					Undo.RecordObjects(targets, "PPos");
					if (isGlobal) { PastePosition(); }
					else { spLocalPosition.vector3Value = TransformInspectorCopyData.localPositionCopy; }
				}

				if (GUILayout.Button("PRot", "ButtonMid"))
				{
					Undo.RecordObjects(targets, "PRot");
					if (isGlobal) { PasteRotation(); }
					else { spLocalRotation.quaternionValue = TransformInspectorCopyData.localRotationCopy; }
				}

				using (new EditorGUI.DisabledScope(isGlobal))
				{
					if (GUILayout.Button("PSca", "ButtonMid"))
					{
						Undo.RecordObjects(targets, "PSca");
						spLocalScale.vector3Value = TransformInspectorCopyData.loacalScaleCopy;
					}
				}

				//GUI.color = new Color(1f, 0.75f, 0.5f, 1f);
				GUIContent pivotRotationContent = sContents.pivotPasteGlobal;
				pivotRotationContent.text = "Global";
				if (Tools.pivotRotation == PivotRotation.Local)
				{
					pivotRotationContent = sContents.pivotPasteLocal;
					pivotRotationContent.text = "Local";
				}

				GUI.color = c;
				if (GUILayout.Button(pivotRotationContent, "ButtonRight", GUILayout.MaxHeight(18)))
				{
					Tools.pivotRotation = Tools.pivotRotation == PivotRotation.Local
						? PivotRotation.Global
						: PivotRotation.Local;
				}
			}
			EditorGUILayout.EndHorizontal();

			EditorGUIUtility.labelWidth = labelWidthBac;
			EditorGUIUtility.wideMode = wideMode;
			DrawBottomPanel(target, targets);
			serializedObject.ApplyModifiedProperties();
		}

		private void PasteRotation()
		{
			foreach (Object o in targets)
			{
				Transform t = (Transform) o;
				t.rotation = TransformInspectorCopyData.rotationCopy;
				EditorUtility.SetDirty(t);
			}
		}

		private void PastePosition()
		{
			foreach (Object o in targets)
			{
				Transform t = (Transform) o;
				t.position = TransformInspectorCopyData.positionCopy;
				EditorUtility.SetDirty(t);
			}
		}

		private class BottomPanelContents : Singleton<BottomPanelContents>
		{
			public readonly GUIContent name = new GUIContent("Name Smart",
				"Name the gameObject using the name of the first component (non-Transform)");

			public readonly GUIContent ping = new GUIContent("Ping",
				"PingObject");
		}

		internal static void DrawBottomPanel(Object target, IEnumerable<Object> targets)
		{
			GUILayout.BeginHorizontal();
			if (GUILayout.Button(BottomPanelContents.instance.name)) { NameGameObject(targets); }

			if (GUILayout.Button(BottomPanelContents.instance.ping, GUILayout.Width(128)))
			{
				EditorGUIUtility.PingObject(target);
			}

			GUILayout.EndHorizontal();
		}

		internal static void EventProcessing(IEnumerable<Object> targets)
		{
			Event e = Event.current;
			if (e != null)
			{
				if (e.type == EventType.MouseDown)
				{
					if (e.button == 2)
					{
						if (e.alt)
						{
							AutoReferencer.FindReferences(targets);
							e.Use();
							return;
						}

						SelectionHelper.ToggleGameObjcetActiveSelf();
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

		internal static void NameGameObject(IEnumerable<Object> targets)
		{
			//sb用来生成代码并复制到剪贴板
			StringBuilder sb = new StringBuilder(128);
			foreach (Object item in targets)
			{
				Transform t = item as Transform;
				Component[] coms = t.GetComponents<Component>(); //没有Image的话
				if (coms.Length > 1) //只有Transform的话排除
				{
					Undo.RecordObject(t.gameObject, "Rename");
					Component com = coms[1];
					Type comType = com.GetType();

					if (comType == typeof(SpriteRenderer))
					{
						SpriteRenderer sr = com as SpriteRenderer;
						if (sr.sprite) { t.name = "sp" + sr.sprite.name; }
						else { t.name = "spNull"; }
					}
#if UNITY_2017_1_OR_NEWER
					else if (comType == typeof(PlayableDirector))
					{
						PlayableDirector pd = com as PlayableDirector;
						t.name = pd.playableAsset ? pd.playableAsset.name : "PlayableDirector";
					}
#endif
					else if (comType == typeof(ParticleSystem))
					{
						ParticleSystemRenderer component = com.GetComponent<ParticleSystemRenderer>();
						if (component.sharedMaterial) { t.name = "ps" + component.sharedMaterial.name; }
					}
					else if (comType == typeof(Light)) { t.name = (com as Light).type + " light"; }
					else if (comType == typeof(MeshFilter))
					{
						MeshFilter meshFilter = com as MeshFilter;
						if (meshFilter.sharedMesh) { t.name = meshFilter.sharedMesh.name; }
					}
					else
					{
						MethodInfo methodInfo = comType.GetMethod("Name",
							BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
						if (methodInfo == null)
						{
							t.name = comType.Name;
							return;
						}

						methodInfo.Invoke(t, null);
					}

					string strType = comType.Name;
					string strValue = strType[0].ToString().ToLower() + strType.Substring(1);
					sb.Append("public ").Append(strType).Append(" ").Append(strValue).AppendLine(";");
				}

				if (Event.current.alt) { t.name = $"{t.name} ({t.GetSiblingIndex()})"; }
			}

			GUIUtility.systemCopyBuffer = sb.ToString();
		}

		internal class Contents : Singleton<Contents>
		{
			public Contents()
			{
				resetStyle = new GUIStyle();
				resetStyle.fixedWidth = 16;
				resetStyle.fixedHeight = 16;
				resetStyle.margin = new RectOffset(0, 0, 0, 0);
				Texture image = EditorGUIUtility.IconContent("Refresh").image;
				positionReset = new GUIContent(image, "Reset the position.");
				anchoredPositionReset = new GUIContent(image, "Reset the xy.\nHold Alt reset the z.");
				deltaSizeReset = new GUIContent(image, "Reset the deltaSize.");
				scaleReset = new GUIContent(image, "Reset the scale.");
				rotationReset = new GUIContent(image, "Reset the rotation.");
			}

			public readonly GUIStyle resetStyle;
			public readonly GUIContent positionReset;
			public readonly GUIContent anchoredPositionReset;
			public readonly GUIContent deltaSizeReset;
			public readonly GUIContent rotationReset;
			public readonly GUIContent scaleReset;

			public readonly GUIContent scale = new GUIContent("Scale", "Scale all axis.");
			public readonly GUIContent round = new GUIContent("Round", "Round all position value.");
			public readonly GUIContent roundRect = new GUIContent("Round", "Round all position and deltaSize value.");

			public readonly GUIContent pivotPasteLocal =
				EditorGUIUtility.IconContent("ToolHandleLocal", "Local|Tool handles are in local paste.");

			public readonly GUIContent pivotPasteGlobal =
				EditorGUIUtility.IconContent("ToolHandleGlobal", "Global|Tool handles are in global paste.");
		}
	}
}