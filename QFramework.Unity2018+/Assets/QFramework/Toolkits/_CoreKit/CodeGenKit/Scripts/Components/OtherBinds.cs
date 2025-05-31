/****************************************************************************
 * Copyright (c) 2015 ~ 2024 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace QFramework
{
    [System.Serializable]
    public class OtherBind
    {
        public string MemberName;
        public Object Object;
    }

    public class OtherBindComparer : IComparer<OtherBind>
    {
        public int Compare(OtherBind a, OtherBind b)
        {
            return string.Compare(a.MemberName, b.MemberName, StringComparison.Ordinal);
        }
    }

    [RequireComponent(typeof(ViewController))]
    public class OtherBinds : MonoBehaviour
    {
        public List<OtherBind> Binds = new List<OtherBind>();
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(OtherBinds))]
    public class ReferenceBindsEditor : Editor
    {
        private OtherBinds mOtherBinds;

        private void DelNullReference()
        {
            var dataProperty = serializedObject.FindProperty("Binds");
            for (int i = dataProperty.arraySize - 1; i >= 0; i--)
            {
                var gameObjectProperty = dataProperty.GetArrayElementAtIndex(i).FindPropertyRelative("Object");
                if (gameObjectProperty.objectReferenceValue == null)
                {
                    dataProperty.DeleteArrayElementAtIndex(i);
                    EditorUtility.SetDirty(mOtherBinds);
                    serializedObject.ApplyModifiedProperties();
                    serializedObject.UpdateIfRequiredOrScript();
                }
            }
        }

        private void OnEnable()
        {
            mOtherBinds = (OtherBinds)target;
        }

        public override void OnInspectorGUI()
        {
            Undo.RecordObject(mOtherBinds, "Changed Settings");
            var dataProperty = serializedObject.FindProperty("Binds");
            EditorGUILayout.BeginHorizontal();

            GUILayout.EndHorizontal();
            EditorGUILayout.Space();

            var delList = new List<int>();
            SerializedProperty property;
            for (int i = mOtherBinds.Binds.Count - 1; i >= 0; i--)
            {
                GUILayout.BeginHorizontal();
                property = dataProperty.GetArrayElementAtIndex(i).FindPropertyRelative("MemberName");
                property.stringValue = EditorGUILayout.TextField(property.stringValue, GUILayout.Width(150));
                property = dataProperty.GetArrayElementAtIndex(i).FindPropertyRelative("Object");
                property.objectReferenceValue =
                    EditorGUILayout.ObjectField(property.objectReferenceValue, typeof(Object), true);

                if (property.objectReferenceValue is Component component)
                {
                    var objects = new List<Object>();
                    objects.AddRange(component.gameObject.GetComponents<Component>());
                    objects.Add(component.gameObject);

                    var index = objects.FindIndex(c => c.GetType() == property.objectReferenceValue.GetType());
                    var newIndex = EditorGUILayout.Popup(index, objects.Select(c => c.GetType().FullName).ToArray());
                    if (index != newIndex)
                    {
                        property.objectReferenceValue = objects[newIndex];
                    }
                }
                else if (property.objectReferenceValue is GameObject gameObject)
                {
                    var objects = BindSearchHelper.GetSelectableBindTypeOnGameObject(gameObject);
                    var index = objects.FindIndex(c => c.GetType() == property.objectReferenceValue.GetType());
                    var newIndex = EditorGUILayout.Popup(index, objects.Select(c => c.GetType().FullName).ToArray());
                    if (index != newIndex)
                    {
                        property.objectReferenceValue = objects[newIndex];
                    }
                }

                if (GUILayout.Button("X"))
                {
                    //将元素添加进删除list
                    delList.Add(i);
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.Label(LocaleKitEditor.IsCN.Value
                ? "将其他需要生成变量的 Object 拖拽至此"
                : " Drag other Object bellow to generate member variables");
            var sfxPathRect = EditorGUILayout.GetControlRect();
            sfxPathRect.height = 50;
            GUI.Box(sfxPathRect, string.Empty);
            EditorGUILayout.LabelField(string.Empty, GUILayout.Height(35));
            if (
                (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform)
                && sfxPathRect.Contains(Event.current.mousePosition)
            )
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (Event.current.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    foreach (var o in DragAndDrop.objectReferences)
                    {
                        if (o.name == target.name)
                        {
                            AddReference(dataProperty, "Self" + o.GetType().Name, o);
                        }
                        else
                        {
                            AddReference(dataProperty, o.name.RemoveString(" ", "-", "@"), o);
                        }
                    }
                }

                Event.current.Use();
            }

            GUILayout.BeginHorizontal();

            EditorGUILayout.EndHorizontal();

            foreach (var i in delList)
            {
                dataProperty.DeleteArrayElementAtIndex(i);
            }

            serializedObject.ApplyModifiedProperties();
            serializedObject.UpdateIfRequiredOrScript();
        }

        private void AddReference(SerializedProperty dataProperty, string key, Object obj)
        {
            int index = dataProperty.arraySize;
            dataProperty.InsertArrayElementAtIndex(index);
            var element = dataProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("MemberName").stringValue = key;
            element.FindPropertyRelative("Object").objectReferenceValue = obj;
        }
    }
#endif
}