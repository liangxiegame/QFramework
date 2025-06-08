/****************************************************************************
 * Copyright (c) 2015 ~ 2025 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace QFramework
{
    [CustomEditor(typeof(AbstractBind), true)]
    [CanEditMultipleObjects]
    public class AbstractBindInspector : UnityEditor.Editor
    {

        private AbstractBind mBindScript => target as AbstractBind;


        private string[] mComponentNames;
        private int mComponentNameIndex;

        private void OnEnable()
        {
            mComponentNames = BindSearchHelper.GetSelectableBindTypeFullNameOnGameObject(mBindScript.gameObject);
            
            mComponentNameIndex = Array.FindIndex(mComponentNames,
                (componentName) => componentName.Contains(mBindScript.TypeName));

            if (mComponentNameIndex == -1 || mComponentNameIndex >= mComponentNames.Length)
            {
                mComponentNameIndex = 0;
            }

            mComponentNameProperty = serializedObject.FindProperty("mComponentName");
            mCustomComponentNameProperty = serializedObject.FindProperty("CustomComponentName");
        }

        private Lazy<GUIStyle> mLabel12 = new Lazy<GUIStyle>(() => new GUIStyle(GUI.skin.label)
        {
            fontSize = 12
        });

        private SerializedProperty mComponentNameProperty;
        private SerializedProperty mCustomComponentNameProperty;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            GUILayout.BeginVertical("box");
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label(LocaleText.Bind, mLabel12.Value, GUILayout.Width(60));


            EditorGUI.BeginChangeCheck();

            mBindScript.MarkType = (BindType)EditorGUILayout.EnumPopup(mBindScript.MarkType);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            if (mCustomComponentNameProperty.stringValue == null ||
                string.IsNullOrEmpty(mCustomComponentNameProperty.stringValue.Trim()))
            {
                mCustomComponentNameProperty.stringValue = mBindScript.name;
            }

            if (mBindScript.MarkType == BindType.DefaultUnityElement)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label(LocaleText.Type, mLabel12.Value, GUILayout.Width(60));

                EditorGUI.BeginChangeCheck();
                mComponentNameIndex = EditorGUILayout.Popup(mComponentNameIndex, mComponentNames);
                if (EditorGUI.EndChangeCheck())
                {
                    mComponentNameProperty.stringValue = mComponentNames[mComponentNameIndex];
                    EditorUtility.SetDirty(target);
                }

                GUILayout.EndHorizontal();
            }


            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Label(LocaleText.BelongsTo, mLabel12.Value, GUILayout.Width(60));

            GUILayout.Label(CodeGenHelper.GetBindBelongs2(mBindScript), mLabel12.Value, GUILayout.Width(200));

            if (GUILayout.Button(LocaleText.Select, GUILayout.Width(60)))
            {
                Selection.objects = new Object[]
                {
                    CodeGenHelper.GetBindBelongs2GameObject(target as AbstractBind)
                };
            }

            GUILayout.EndHorizontal();

            if (mBindScript.MarkType != BindType.DefaultUnityElement)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(LocaleText.ClassName, mLabel12.Value, GUILayout.Width(60));
                mCustomComponentNameProperty.stringValue =
                    EditorGUILayout.TextField(mCustomComponentNameProperty.stringValue);

                GUILayout.EndHorizontal();
            }

            GUILayout.Space(10);

            GUILayout.Label(LocaleText.Comment, mLabel12.Value);

            GUILayout.Space(10);

            mBindScript.CustomComment = EditorGUILayout.TextArea(mBindScript.Comment, GUILayout.Height(100));

            var rootGameObj = CodeGenHelper.GetBindBelongs2GameObject(mBindScript);

            if (rootGameObj)
            {
                if (GUILayout.Button(LocaleText.Generate + " (" + rootGameObj.name + ")",
                        GUILayout.Height(30)))
                {
                    CodeGenKit.Generate(rootGameObj.GetComponent<IBindGroup>());
                }
            }

            GUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }

        private void OnDisable()
        {
            mCustomComponentNameProperty = null;
            mComponentNameProperty = null;
        }

        private static class LocaleText
        {
            // ReSharper disable once InconsistentNaming
            private static bool CN
            {
                get => LocaleKitEditor.IsCN.Value;
                set => LocaleKitEditor.IsCN.Value = value;
            }
        
            public static string Type => CN ? " 类型:" : " Type:";
            public static string Comment => CN ? " 注释" : " Comment";
            public static string BelongsTo => CN ? " 属于:" : " Belongs 2:";
            public static string Select => CN ? "选择" : "Select";
            public static string Generate => CN ? " 生成代码" : " Generate Code";

            public static string Bind => CN ? " 绑定设置" : " Bind Setting";
            public static string ClassName => CN ? "类名" : " Class Name";
        }
    }
}
#endif