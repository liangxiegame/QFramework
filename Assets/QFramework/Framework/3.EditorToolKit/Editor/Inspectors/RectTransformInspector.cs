// /****************************************************************************
//  * Copyright (c) 2018 Karsion(拖鞋)
//  * Date: 2018-06-07 18:30
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

using System.Reflection;
using QF.Extensions;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

namespace QFramework
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(RectTransform))]
    public class RectTransformInspector : CustomCustomEditor
    {
        private const string strButtonLeft = "ButtonLeft";
        private const string strButtonMid = "ButtonMid";
        private const string strButtonRight = "ButtonRight";

        private static GUIStyle styleMove;
        private static GUIStyle stylePivotSetup;
        private bool autoSetNativeSize;
        private Image image;

        private float scaleAll = 1;
        private SerializedProperty spAnchoredPosition;
        private SerializedProperty spLocalRotation;
        private SerializedProperty spLocalScale;
        private SerializedProperty spPivot;
        private SerializedProperty spSizeDelta;

        public RectTransformInspector()
            : base("RectTransformEditor")
        {
        }

        private void MoveTargetAnchoredPosition(Vector2 v2Unit)
        {
            foreach (Object item in targets)
            {
                RectTransform rtf = item as RectTransform;
                rtf.anchoredPosition += v2Unit;
            }
        }

        private void OnEnable()
        {
            spAnchoredPosition = serializedObject.FindProperty("m_AnchoredPosition");
            spSizeDelta = serializedObject.FindProperty("m_SizeDelta");
            spLocalRotation = serializedObject.FindProperty("m_LocalRotation");
            spLocalScale = serializedObject.FindProperty("m_LocalScale");
            spPivot = serializedObject.FindProperty("m_Pivot");
            scaleAll = spLocalScale.FindPropertyRelative("x").floatValue;
            image = (target as RectTransform).GetComponent<Image>();
            if (image)
            {
                autoSetNativeSize = !image.sprite;
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (s_Contents == null)
            {
                s_Contents = new Contents();
            }

            serializedObject.Update();
            Event e = Event.current;
            if (e != null)
            {
                if (image) //Auto set Image's native size if Image.sprite is not null
                {
                    if (e.type == EventType.DragPerform || e.type == EventType.MouseDown)
                    {
                        autoSetNativeSize = !image.sprite;
                    }

                    if (autoSetNativeSize && image.sprite && image.type == Image.Type.Simple)
                    {
                        autoSetNativeSize = false;
                        RectTransform tf = target as RectTransform;
                        float x = image.sprite.rect.width/image.pixelsPerUnit;
                        float y = image.sprite.rect.height/image.pixelsPerUnit;
                        tf.anchorMax = tf.anchorMin;
                        tf.sizeDelta = new Vector2(x, y);
                    }
                }

                //Keyboard control move by pixel style like Photoshop's move tool
                if (e.type == EventType.KeyDown && e.control)
                {
                    int nUnit = e.shift ? 10 : 1;
                    switch (e.keyCode)
                    {
                        case KeyCode.UpArrow:
                            Undo.RecordObjects(targets, "UpArrow");
                            MoveTargetAnchoredPosition(Vector2.up*nUnit);
                            e.Use();
                            break;
                        case KeyCode.DownArrow:
                            Undo.RecordObjects(targets, "DownArrow");
                            MoveTargetAnchoredPosition(Vector2.down*nUnit);
                            e.Use();
                            break;
                        case KeyCode.LeftArrow:
                            Undo.RecordObjects(targets, "LeftArrow");
                            MoveTargetAnchoredPosition(Vector2.left*nUnit);
                            e.Use();
                            break;
                        case KeyCode.RightArrow:
                            Undo.RecordObjects(targets, "RightArrow");
                            MoveTargetAnchoredPosition(Vector2.right*nUnit);
                            e.Use();
                            break;
                    }
                }
            }

            const float fButtonWidth = 21;
            if (stylePivotSetup == null)
            {
                stylePivotSetup = new GUIStyle("PreButton")
                {
                    normal = new GUIStyle("CN Box").normal,
                    active = new GUIStyle("AppToolbar").normal,
                    overflow = new RectOffset(),
                    padding = new RectOffset(0, 0, -1, 0),
                    fixedWidth = 19
                };

                styleMove = new GUIStyle(stylePivotSetup)
                {
                    padding = new RectOffset(0, 0, -2, 0)
                };
            }

            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginVertical();
                {
                    #region Tools
                    GUILayout.BeginHorizontal();
                    {
                        EditorGUIUtility.labelWidth = 64;
                        float newScale = EditorGUILayout.FloatField(s_Contents.scaleContent, scaleAll);
                        if (!Mathf.Approximately(scaleAll, newScale))
                        {
                            scaleAll = newScale;
                            spLocalScale.vector3Value = Vector3.one*scaleAll;
                        }
                    }

                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button(s_Contents.anchoredPosition0Content, strButtonLeft, GUILayout.Width(fButtonWidth)))
                        {
                            foreach (Object item in targets)
                            {
                                RectTransform rtf = item as RectTransform;
                                rtf.LocalPositionIdentity();
                            }
                        }

                        if (GUILayout.Button(s_Contents.deltaSize0Content, strButtonMid, GUILayout.Width(fButtonWidth)))
                        {
                            spSizeDelta.vector2Value = Vector2.zero;
                        }

                        if (GUILayout.Button(s_Contents.rotation0Content, strButtonMid, GUILayout.Width(fButtonWidth)))
                        {
                            Undo.RecordObjects(targets, "rotationContent");
                            MethodInfo method =
                                typeof(Transform).GetMethod("SetLocalEulerAngles", BindingFlags.Instance | BindingFlags.NonPublic);
                            object[] clear = { Vector3.zero, 0 };
                            for (int i = 0; i < targets.Length; i++)
                            {
                                method.Invoke(targets[i], clear);
                            }

                            Event.current.type = EventType.Used;
                        }

                        if (GUILayout.Button(s_Contents.scale0Content, strButtonRight, GUILayout.Width(fButtonWidth)))
                        {
                            spLocalScale.vector3Value = Vector3.one;
                            scaleAll = spLocalScale.FindPropertyRelative("x").floatValue;
                        }

                        if (GUILayout.Button(s_Contents.roundContent))
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
                    if (GUILayout.Button(s_Contents.copyContent, strButtonLeft, GUILayout.Width(fButtonWidth)))
                    {
                        ComponentUtility.CopyComponent(target as RectTransform);
                    }

                    GUI.color = new Color(1f, 0.5f, 0.5f, 1f);
                    if (GUILayout.Button(s_Contents.pasteContent, strButtonMid, GUILayout.Width(fButtonWidth)))
                    {
                        foreach (Object item in targets)
                        {
                            ComponentUtility.PasteComponentValues(item as RectTransform);
                        }
                    }

                    GUI.color = c;
                    if (GUILayout.Button(s_Contents.fillParentContent, strButtonMid, GUILayout.Width(fButtonWidth)))
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

                    if (GUILayout.Button(s_Contents.normalSizeDeltaContent, strButtonRight, GUILayout.Width(fButtonWidth)))
                    {
                        Undo.RecordObjects(targets, "N");
                        foreach (Object item in targets)
                        {
                            RectTransform rtf = item as RectTransform;
                            Rect rect = rtf.rect;
                            rtf.anchorMax = new Vector2(0.5f, 0.5f);
                            rtf.anchorMin = new Vector2(0.5f, 0.5f);
                            rtf.sizeDelta = rect.size;
                        }
                    }

                    GUILayout.Label(s_Contents.readmeContent);
                    GUILayout.EndHorizontal();
                    #endregion
                }
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                {
                    #region Pivot
                    GUILayout.Label("Pivot", "ProfilerPaneSubLabel"); //┌─┐
                    GUILayout.BeginHorizontal(); //│┼│
                    {
                        //└─┘
                        if (GUILayout.Button("◤", stylePivotSetup))
                        {
                            spPivot.vector2Value = new Vector2(0, 1);
                        }

                        if (GUILayout.Button("", stylePivotSetup))
                        {
                            spPivot.vector2Value = new Vector2(0.5f, 1);
                        }

                        if (GUILayout.Button("◥", stylePivotSetup))
                        {
                            spPivot.vector2Value = new Vector2(1, 1);
                        }
                    }

                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("", stylePivotSetup))
                        {
                            spPivot.vector2Value = new Vector2(0, 0.5f);
                        }

                        if (GUILayout.Button("+", styleMove))
                        {
                            spPivot.vector2Value = new Vector2(0.5f, 0.5f);
                        }

                        if (GUILayout.Button("", stylePivotSetup))
                        {
                            spPivot.vector2Value = new Vector2(1, 0.5f);
                        }
                    }

                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("◣", stylePivotSetup))
                        {
                            spPivot.vector2Value = new Vector2(0, 0);
                        }

                        if (GUILayout.Button("", stylePivotSetup))
                        {
                            spPivot.vector2Value = new Vector2(0.5f, 0);
                        }

                        if (GUILayout.Button("◢", stylePivotSetup))
                        {
                            spPivot.vector2Value = new Vector2(1, 0);
                        }
                    }

                    GUILayout.EndHorizontal();
                }
                #endregion

                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();

            TransformInspector.DrawBottomPanel(target, targets);
            serializedObject.ApplyModifiedProperties();
        }

        private static Contents s_Contents;

        private class Contents
        {
            public readonly GUIContent anchoredPosition0Content = new GUIContent("P", "AnchoredPosition 0");
            public readonly GUIContent scaleContent = new GUIContent("Scale …", "Scale all axis");
            public readonly GUIContent scale0Content = new GUIContent("S", "Scale 0");
            public readonly GUIContent deltaSize0Content = new GUIContent("D", "DeltaSize 0");
            public readonly GUIContent rotation0Content = new GUIContent("R", "Rotation 0");
            public readonly GUIContent roundContent = new GUIContent("Round", "AnchoredPosition DeltaSize round to int");
            public readonly GUIContent copyContent = new GUIContent("C", "Copy component value");
            public readonly GUIContent pasteContent = new GUIContent("P", "Paste component value");
            public readonly GUIContent fillParentContent = new GUIContent("F", "Fill the parent RectTransform");
            public readonly GUIContent normalSizeDeltaContent = new GUIContent("N", "Change to normal sizeDelta mode");
            public readonly GUIContent readmeContent = new GUIContent("Readme", "Ctrl+Arrow key move rectTransform\nCtrl: 1px    Shift: 10px");
        }
    }
}