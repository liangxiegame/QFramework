// /****************************************************************************
//  * Copyright (c) 2018 Karsion(拖鞋)
//  * Date: 2018-06-07 18:29
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
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace QFramework
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
                    transform.LocalPosition(position).LocalRotation(rotation).LocalScale(scale);
                    EditorUtility.SetDirty(transform);
                }
            }
        }

        private float fScale = 1;
        private SerializedProperty m_LocalPosition;
        private SerializedProperty m_LocalRotation;
        private SerializedProperty m_LocalScale;

        internal TransformInspector()
            : base("TransformInspector")
        {
            if (dictTransformData == null)
            {
                dictTransformData = new Dictionary<int, TransformData>();
            }
        }

        protected override void OnSceneGUI() { }

        protected void OnEnable()
        {
            m_LocalPosition = serializedObject.FindProperty("m_LocalPosition");
            m_LocalRotation = serializedObject.FindProperty("m_LocalRotation");
            m_LocalScale = serializedObject.FindProperty("m_LocalScale");
            fScale = m_LocalScale.FindPropertyRelative("x").floatValue;
            if (s_Contents == null)
            {
                s_Contents = new Contents();
            }

            CallFieldMethod("m_RotationGUI", "OnEnable", new[]
            {
                typeof(SerializedProperty), typeof(GUIContent)
            }, m_LocalRotation, s_Contents.rotationContent);
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
            else
            {
                dictTransformData[insID].RecordTransform(transform);
            }
        }

        private static void TransformRuntimeCopy(bool withChildren)
        {
            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                Transform transform = Selection.gameObjects[i].transform;
                TransformRuntimeCopy(transform);
                if (withChildren)
                {
                    transform.ActionRecursion(TransformRuntimeCopy);
                }
            }
        }

        private static void TransformRuntimePaste()
        {
            foreach (var transformData in dictTransformData)
            {
                transformData.Value.SetTransform();
            }
        }

        private static Vector3 Round(Vector3 v3Value, int nDecimalPoint = 0)
        {
            int nScale = 1;
            for (int i = 0; i < nDecimalPoint; i++)
            {
                nScale *= 10;
            }

            v3Value *= nScale;
            v3Value.x = Mathf.RoundToInt(v3Value.x);
            v3Value.y = Mathf.RoundToInt(v3Value.y);
            v3Value.z = Mathf.RoundToInt(v3Value.z);
            return v3Value / nScale;
        }

        private static GUIStyle style;

        /// <summary>
        ///     Draw the inspector widget.
        /// </summary>
        public override void OnInspectorGUI()
        {
            if (s_Contents == null)
            {
                s_Contents = new Contents();
            }

            serializedObject.Update();

            if (style == null)
            {
                style = new GUIStyle("button");
                style.fixedWidth = 18;
                style.stretchWidth = true;
                style.fixedHeight = 16;
                style.margin = new RectOffset(0, 0, 1, 2);
            }

            EditorGUIUtility.labelWidth = 24f;
            if (!EditorGUIUtility.wideMode)
            {
                EditorGUIUtility.wideMode = true;
            }

            EditorGUILayout.PropertyField(m_LocalPosition, s_Contents.positionContent);
            CallFieldMethod("m_RotationGUI", "RotationField", new Type[0], null);
            EditorGUILayout.PropertyField(m_LocalScale, s_Contents.scaleContent);

            Rect rect = GUILayoutUtility.GetLastRect();
            rect.width = style.fixedWidth;
            rect.y -= 36;
            if (GUI.Button(rect, s_Contents.positionContent, style))
            {
                m_LocalPosition.vector3Value = Vector3.zero;
                Event.current.type = EventType.Used;
            }

            rect.y += 18;
            if (GUI.Button(rect, s_Contents.rotationContent, style))
            {
                Undo.RecordObjects(targets, "rotationContent");
                MethodInfo method =
                    typeof(Transform).GetMethod("SetLocalEulerAngles", BindingFlags.Instance | BindingFlags.NonPublic);
                object[] clear = {Vector3.zero, 0};
                for (int i = 0; i < targets.Length; i++)
                {
                    method.Invoke(targets[i], clear);
                }

                Event.current.type = EventType.Used;
            }

            rect.y += 18;
            if (GUI.Button(rect, s_Contents.scaleContent, style))
            {
                fScale = 1;
                m_LocalScale.vector3Value = Vector3.one;
                Event.current.type = EventType.Used;
            }

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUIUtility.labelWidth = 37f;
                var newScale = EditorGUILayout.FloatField("Scale", fScale);
                if (!Mathf.Approximately(fScale, newScale))
                {
                    fScale = newScale;
                    m_LocalScale.vector3Value = Vector3.one * fScale;
                }

                EditorGUILayout.LabelField("Round", GUILayout.Width(42f));
                if (GUILayout.Button(".", "MiniButtonLeft"))
                {
                    Undo.RecordObjects(targets, "Round");
                    for (int i = 0; i < targets.Length; i++)
                    {
                        Transform o = targets[i] as Transform;
                        o.localPosition = Round(o.localPosition);
                        o.localScale = Round(o.localScale);
                    }
                }

                if (GUILayout.Button(".0", "MiniButtonMid"))
                {
                    Undo.RecordObjects(targets, "Round");
                    for (int i = 0; i < targets.Length; i++)
                    {
                        Transform o = targets[i] as Transform;
                        o.localPosition = Round(o.localPosition, 1);
                        o.localScale = Round(o.localScale, 1);
                    }
                }

                if (GUILayout.Button(".00", "MiniButtonRight"))
                {
                    Undo.RecordObjects(targets, "Round");
                    for (int i = 0; i < targets.Length; i++)
                    {
                        Transform o = targets[i] as Transform;
                        o.localPosition = Round(o.localPosition, 2);
                        o.localScale = Round(o.localScale, 2);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            int nSelectionCount = Selection.transforms.Length;
            if (Application.isPlaying || dictTransformData.Count > 0)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Record", "ButtonLeft"))
                    {
                        TransformRuntimeCopy(false);
                    }

                    if (GUILayout.Button("Record whit children", "ButtonLeft"))
                    {
                        TransformRuntimeCopy(true);
                    }

                    if (GUILayout.Button("Set", "ButtonRight"))
                    {
                        TransformRuntimePaste();
                    }

                    GUILayout.Label(dictTransformData.Count.ToString());
                    if (GUILayout.Button("Clear"))
                    {
                        TransformRuntimeCopyClear();
                    }
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
                        TransformInspectorCopyData.localPositionCopy = m_LocalPosition.vector3Value;
                        TransformInspectorCopyData.localRotationCopy = m_LocalRotation.quaternionValue;
                        TransformInspectorCopyData.loacalScaleCopy = m_LocalScale.vector3Value;
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
                        m_LocalPosition.vector3Value = TransformInspectorCopyData.localPositionCopy;
                        m_LocalRotation.quaternionValue = TransformInspectorCopyData.localRotationCopy;
                        m_LocalScale.vector3Value = TransformInspectorCopyData.loacalScaleCopy;
                    }
                }

                if (GUILayout.Button("PPos", "ButtonMid"))
                {
                    Undo.RecordObjects(targets, "PPos");
                    if (isGlobal)
                    {
                        PastePosition();
                    }
                    else
                    {
                        m_LocalPosition.vector3Value = TransformInspectorCopyData.localPositionCopy;
                    }
                }

                if (GUILayout.Button("PRot", "ButtonMid"))
                {
                    Undo.RecordObjects(targets, "PRot");
                    if (isGlobal)
                    {
                        PasteRotation();
                    }
                    else
                    {
                        m_LocalRotation.quaternionValue = TransformInspectorCopyData.localRotationCopy;
                    }
                }

                using (new EditorGUI.DisabledScope(isGlobal))
                {
                    if (GUILayout.Button("PSca", "ButtonMid"))
                    {
                        Undo.RecordObjects(targets, "PSca");
                        m_LocalScale.vector3Value = TransformInspectorCopyData.loacalScaleCopy;
                    }
                }

                //GUI.color = new Color(1f, 0.75f, 0.5f, 1f);
                GUIContent pivotRotationContent = s_Contents.pivotPasteGlobal;
                pivotRotationContent.text = "Global";
                if (Tools.pivotRotation == PivotRotation.Local)
                {
                    pivotRotationContent = s_Contents.pivotPasteLocal;
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

            DrawBottomPanel(target, targets);

            Transform transform = target as Transform;
            Vector3 position = transform.position;
            if (Mathf.Abs(position.x) > 100000f || Mathf.Abs(position.y) > 100000f || Mathf.Abs(position.z) > 100000f)
            {
                EditorGUILayout.HelpBox(Contents.floatingPointWarning, MessageType.Warning);
            }

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

        private static BottomPanelContents s_BottomPanelContents;

        private class BottomPanelContents
        {
            //public readonly GUIContent calc = new GUIContent("Calc", "Run the system Calc");
            public readonly GUIContent name = new GUIContent("Name",
                "Name the gameObject using the name of the first component (non-Transform)");

            public readonly GUIContent findRef = new GUIContent("Auto Ref", "Auto find references by Property Name");

            public readonly GUIContent calledByEditor = new GUIContent("CalledByEditor()",
                "Find and call a Function \"CalledByEditor()\" using reflection");

            public readonly GUIContent calledByEditorc = new GUIContent("c", "Copy \"CalledByEditor()\" code");
            //public readonly GUIContent button = new GUIContent("b", "Copy \"[Button(\"Test\")]\" code");
        }

        internal static void DrawBottomPanel(Object target, IEnumerable<Object> targets)
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
                }
            }

            if (s_BottomPanelContents == null)
            {
                s_BottomPanelContents = new BottomPanelContents();
            }

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button(s_BottomPanelContents.findRef))
                {
                    AutoReferencer.FindReferences(targets);
                }

                if (GUILayout.Button(s_BottomPanelContents.name))
                {
                    //sb用来生成代码并复制到剪贴板
                    StringBuilder sb = new StringBuilder(128);
                    foreach (Object item in targets)
                    {
                        Transform t = item as Transform;
                        Image img = t.GetComponent<Image>(); //优先给Image命名
                        if (img && img.sprite)
                        {
                            Undo.RecordObject(t.gameObject, "Rename");
                            t.name = "img" + img.sprite.name;
                            sb.Append("public Image ").Append(t.name).AppendLine(";");
                            continue;
                        }

                        Component[] coms = t.GetComponents<Component>(); //没有Image的话
                        if (coms.Length > 1) //只有Transform的话排除
                        {
                            Undo.RecordObject(t.gameObject, "Rename");
                            string strType = coms[1].GetType().Name;
                            string strValue = strType[0].ToString().ToLower() + strType.Substring(1);
                            t.name = strType;
                            sb.Append("public ").Append(strType).Append(" ").Append(strValue).AppendLine(";");
                        }
                    }

                    GUIUtility.systemCopyBuffer = sb.ToString();
                }

                if (GUILayout.Button(s_BottomPanelContents.calledByEditor, "ButtonLeft"))
                {
                    AutoReferencer.CalledByEditor(targets);
                }

                if (GUILayout.Button(s_BottomPanelContents.calledByEditorc, "ButtonRight"))
                {
                    GUIUtility.systemCopyBuffer =
                        @"
#if UNITY_EDITOR
    private void CalledByEditor()
    {
        
    }
#endif
";
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private static Contents s_Contents;

        private class Contents
        {
            public readonly GUIContent positionContent = new GUIContent("P",
                "The local position of this Game Object relative to the parent. Click the button to 0.");

            public readonly GUIContent scaleContent = new GUIContent("S",
                "The local scaling of this Game Object relative to the parent. Click the button to 1.");

            public readonly GUIContent rotationContent = new GUIContent("R",
                "The local rotation of this Game Object relative to the parent. Click the button to 0.");

            public const string floatingPointWarning =
                "Due to floating-point precision limitations, it is recommended to bring the world coordinates of the GameObject within a smaller range.";

            public readonly GUIContent pivotPasteLocal =
                EditorGUIUtility.IconContent("ToolHandleLocal", "Local|Tool handles are in local paste.");

            public readonly GUIContent pivotPasteGlobal =
                EditorGUIUtility.IconContent("ToolHandleGlobal", "Global|Tool handles are in global paste.");
        }
    }
}