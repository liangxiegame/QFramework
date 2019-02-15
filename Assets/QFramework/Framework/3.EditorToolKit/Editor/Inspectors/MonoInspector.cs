// /****************************************************************************
//  * Copyright (c) 2018 Karsion(拖鞋)
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
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace QFramework
{
    [CustomEditor(typeof(MonoBehaviour), true, isFallback = true)]
    [CanEditMultipleObjects]
    internal class MonoInspector : UnityEditor.Editor
    {
        private class Styles
        {
            public readonly GUIContent iconPlus = EditorGUIUtility.IconContent("Toolbar Plus", "|Add");
            public readonly GUIContent iconMinus = EditorGUIUtility.IconContent("Toolbar Minus", "|Remove");
            public readonly GUIContent iconTrash = EditorGUIUtility.IconContent("TreeEditor.Trash", "|Clear");
            public readonly GUIStyle listItem = new GUIStyle("ListToggle");
            public readonly GUIStyle preButton = "RL FooterButton";
            public readonly GUIStyle button = new GUIStyle("Label");
            public readonly GUIStyle endBackground = "VCS_StickyNote";

            public readonly GUIStyle list = "OL box NoExpand";

            public Styles()
            {
                button.stretchWidth = false;
                listItem.margin = new RectOffset(0, 0, 2, 0);
            }
        }

        private static Styles sStyles;
        private int nListId;
        private int nObjectId;
        private ButtonExAttributeDrawer buttonExAttributeDrawer;


        public override void OnInspectorGUI()
        {
            if (!target)
            {
                return;
            }

            if (buttonExAttributeDrawer == null)
            {
                buttonExAttributeDrawer = new ButtonExAttributeDrawer(target);
            }

            buttonExAttributeDrawer.OnInspectorGUI();
            nListId = -1;
            nObjectId = serializedObject.targetObject.GetInstanceID();
            CustomInspectorGUI(serializedObject, target, this);
        }

        private static void CustomInspectorGUI(SerializedObject serializedObject, Object target, MonoInspector editor)
        {
            //这部分是使用了反编译Editor的代码
            EditorGUI.BeginChangeCheck();
            serializedObject.Update();
            SerializedProperty iterator = serializedObject.GetIterator();
            for (bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
            {
                using (new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath))
                {
                    FieldInfo fieldInfos = target.GetType().GetField(iterator.name);
                    if (fieldInfos == null)
                    {
                        EditorGUILayout.PropertyField(iterator, true);
                        continue;
                    }

                    //检查ShowIf特性
                    if (CheckShowIf(iterator, target))
                    {
                        //准备style
                        if (sStyles == null)
                        {
                            sStyles = new Styles();
                        }

                        if (iterator.isArray && iterator.propertyType == SerializedPropertyType.Generic)
                        {
                            //绘制Array、List<>
                            ListField(iterator, editor);
                        }
                        else
                        {
                            EditorGUILayout.PropertyField(iterator, true);
                        }
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
            EditorGUI.EndChangeCheck();
        }

        private static bool CheckShowIf(SerializedProperty iterator, Object target)
        {
            FieldInfo fieldInfos = target.GetType().GetField(iterator.name);
            if (fieldInfos == null)
            {
                return true;
            }

            Type tButtonExAttribute = typeof(ShowIfAttribute);
            if (Attribute.IsDefined(fieldInfos, tButtonExAttribute, true))
            {
                ShowIfAttribute[] exAttributes =
                    fieldInfos.GetCustomAttributes(tButtonExAttribute, true) as ShowIfAttribute[];
                return ShowIfAttributeDrawer.CheckShowTargets(iterator, exAttributes[0]);
            }

            return true;
        }

        //internal static float GetHeaderAttributeHeight(SerializedProperty serializedProperty)
        //{
        //    Object target = serializedProperty.serializedObject.targetObject;
        //    FieldInfo fieldInfo = target.GetType().GetField(serializedProperty.name);
        //    if (fieldInfo == null)
        //    {
        //        return 0;
        //    }

        //    Type tButtonExAttribute = typeof(HeaderAttribute);
        //    if (Attribute.IsDefined(fieldInfo, tButtonExAttribute, true))
        //    {
        //        return fieldInfo.GetCustomAttributes(tButtonExAttribute, true).Length * 24f;
        //    }

        //    return 0;
        //}

        private static void ListField(SerializedProperty iterator, MonoInspector editor)
        {
            editor.nListId++;
            int dragId = editor.nListId + editor.nObjectId;
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(iterator,
                new GUIContent(string.Format("[{0}] {1}", iterator.arraySize, iterator.displayName)), false);
            GUILayout.EndHorizontal();
            Rect position = GUILayoutUtility.GetLastRect();
            position.x = 2;
            position.width = 10;
            float heightDelta = position.height - 16;
            if (heightDelta > 0)
            {
                position.height = 16;
                position.y += heightDelta;
            }

            //上移16高度绘制“+”、“清空”按钮
            GUILayout.Space(-18);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayoutPressButton(sStyles.iconPlus, sStyles.preButton, GUILayout.Width(16)) ==
                ButtonPressType.Mouse0)
            {
                //添加一个元素
                iterator.isExpanded = true;
                iterator.InsertArrayElementAtIndex(iterator.arraySize);
                Event.current.type = EventType.Used;
                editor.Repaint();
            }

            Color currColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.red;
            if (GUILayoutPressButton(sStyles.iconTrash, sStyles.preButton, GUILayout.Width(16)) ==
                ButtonPressType.Mouse0)
            {
                //清空列表
                iterator.isExpanded = true;
                iterator.ClearArray();
                MonoInspectorDrag.EndDrag(false);
                Event.current.type = EventType.Used;
                editor.Repaint();
            }

            GUI.backgroundColor = currColor;
            GUILayout.EndHorizontal();

            //显示折叠
            if (!iterator.isExpanded || iterator.serializedObject == null)
            {
                return;
            }

            //为拖拽准备一个IList
            IList iList = null;
            int minListLen = int.MaxValue;
            Type type = editor.target.GetType();
            FieldInfo fInfo = type.GetField(iterator.name,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            while (fInfo == null && type != typeof(Object))
            {
                type = type.BaseType;
                if (type == null)
                {
                    break;
                }

                fInfo = type.GetField(iterator.name,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            }

            for (int index = 0; index < editor.targets.Length; index++)
            {
                Object o = editor.targets[index];
                IList li = fInfo.GetValue(o) as IList;
                int l = li == null ? 0 : li.Count;
                if (l >= minListLen)
                {
                    continue;
                }

                iList = li;
                minListLen = l;
            }

            //绘制列表元素
            Undo.RecordObject(iterator.serializedObject.targetObject, iterator.serializedObject.targetObject.name);
            if (Event.current.type == EventType.DragPerform || Event.current.type == EventType.DragExited ||
                Event.current.type == EventType.DragUpdated)
            {
                for (int i = 0; i < iterator.arraySize; i++)
                {
                    SerializedProperty spArrayElement = iterator.GetArrayElementAtIndex(i);
                    EditorGUILayout.PropertyField(spArrayElement, spArrayElement.hasChildren); // Property
                }

                return;
            }

            EditorGUILayout.BeginVertical(sStyles.list);
            for (int i = 0; i < iterator.arraySize; i++)
            {
                SerializedProperty spArrayElement = iterator.GetArrayElementAtIndex(i);
                EditorGUILayout.BeginHorizontal();
                float currLabelW = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth -= 25;
                bool wasEnabled = GUI.enabled;
                if (editor.targets.Length > 1)
                {
                    GUI.enabled = false;
                }

                //按方块开始拖拽
                ButtonPressType bpt = GUILayoutPressButton(GUIContent.none, sStyles.listItem);
                if (bpt == ButtonPressType.Mouse1)
                {
                    MonoInspectorDrag.StartDrag(dragId, editor, iList, i);
                }

                GUI.enabled = wasEnabled;
                if (spArrayElement.hasVisibleChildren)
                {
                    GUILayout.Label(i.ToString(), sStyles.button);
                    GUILayout.Space(10);
                }

                EditorGUILayout.PropertyField(spArrayElement, spArrayElement.hasChildren); // Property
                if (GUILayout.Button(sStyles.iconPlus, sStyles.preButton, GUILayout.Width(16)))
                {
                    iterator.InsertArrayElementAtIndex(i);
                    EditorGUILayout.EndHorizontal();
                    break;
                }

                if (GUILayout.Button(sStyles.iconMinus, sStyles.preButton, GUILayout.Width(16)))
                {
                    if (spArrayElement.propertyType == SerializedPropertyType.ObjectReference &&
                        spArrayElement.objectReferenceValue != null)
                    {
                        iterator.DeleteArrayElementAtIndex(i);
                    }

                    iterator.DeleteArrayElementAtIndex(i);
                    EditorGUILayout.EndHorizontal();
                    break;
                }

                EditorGUIUtility.labelWidth = currLabelW;
                EditorGUILayout.EndHorizontal();
                if (MonoInspectorDrag.Drag(dragId, iList, i).outcome == DragResultType.Accepted)
                {
                    GUI.changed = true;
                }
            }

            EditorGUILayout.EndHorizontal();

            if (iterator.isExpanded)
            {
                GUILayout.Space(3);
                GUILayout.Box(string.Empty, sStyles.endBackground, GUILayout.ExpandWidth(true), GUILayout.Height(2));
            }
        }

        private static int activePressButtonId;
        private static FieldInfo fiLastControlId;

        private enum ButtonPressType
        {
            None,
            Mouse0,
            Mouse1
        }

        private static ButtonPressType GUIPressButton(Rect position, GUIContent content, GUIStyle guiStyle)
        {
            Event eventCurrent = Event.current;
            if (GUI.enabled && eventCurrent.type == EventType.MouseUp && activePressButtonId != -1)
            {
                activePressButtonId = -1;
                GUIUtility.hotControl = 0;
                eventCurrent.Use();
            }

            GUI.Button(position, content, guiStyle);
            int lastControlId = GetLastControlId();
            int hotControl = GUIUtility.hotControl;
            bool flag = GUI.enabled && hotControl > 1 &&
                        position.Contains(eventCurrent.mousePosition);
            if (flag && activePressButtonId == -1 && activePressButtonId != lastControlId)
            {
                GUIUtility.hotControl = lastControlId;
                activePressButtonId = lastControlId;
                return eventCurrent.button == 0 ? ButtonPressType.Mouse0 : ButtonPressType.Mouse1;
            }

            if (!flag && hotControl < 1)
            {
                activePressButtonId = -1;
            }

            return ButtonPressType.None;
        }

        //按钮区分鼠标左右键的
        private static ButtonPressType GUILayoutPressButton(GUIContent content, GUIStyle guiStyle,
            params GUILayoutOption[] options)
        {
            Event eventCurrent = Event.current;
            if (GUI.enabled && eventCurrent.type == EventType.MouseUp && activePressButtonId != -1)
            {
                activePressButtonId = -1;
                GUIUtility.hotControl = 0;
                eventCurrent.Use();
            }

            GUILayout.Button(content, guiStyle, options);
            int lastControlId = GetLastControlId();
            int hotControl = GUIUtility.hotControl;
            bool flag = GUI.enabled && hotControl > 1 &&
                        GUILayoutUtility.GetLastRect().Contains(eventCurrent.mousePosition);
            if (flag && activePressButtonId == -1 && activePressButtonId != lastControlId)
            {
                GUIUtility.hotControl = lastControlId;
                activePressButtonId = lastControlId;
                return eventCurrent.button == 0 ? ButtonPressType.Mouse0 : ButtonPressType.Mouse1;
            }

            if (!flag && hotControl < 1)
            {
                activePressButtonId = -1;
            }

            return ButtonPressType.None;
        }

        //反射获取最后一个控件的ID
        private static int GetLastControlId()
        {
            if (fiLastControlId == null)
            {
                fiLastControlId =
                    typeof(EditorGUIUtility).GetField("s_LastControlID", BindingFlags.Static | BindingFlags.NonPublic);
                if (fiLastControlId == null)
                {
                    fiLastControlId =
                        typeof(EditorGUI).GetField("lastControlID", BindingFlags.Static | BindingFlags.NonPublic);
                }
            }

            return (int) fiLastControlId.GetValue(null);
        }
    }
}