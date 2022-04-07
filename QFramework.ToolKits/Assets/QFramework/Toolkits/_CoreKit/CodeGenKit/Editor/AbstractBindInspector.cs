/****************************************************************************
 * Copyright (c) 2015 ~ 2022 liangxiegame UNDER MIT LICENSE
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace QFramework
{
    [CustomEditor(typeof(AbstractBind), true)]
    public class AbstractBindInspector : UnityEditor.Editor
    {
        private BindInspectorLocale mLocaleText = new BindInspectorLocale();

        private AbstractBind mBindScript => target as AbstractBind;


        private string[] mComponentNames;
        private int mComponentNameIndex;

        private void OnEnable()
        {
            var components = mBindScript.GetComponents<Component>();

            mComponentNames = components.Where(c => !(c is AbstractBind))
                .Select(c => c.GetType().FullName)
                .ToArray();

            mComponentNameIndex = mComponentNames.ToList()
                .FindIndex((componentName) => componentName.Contains(mBindScript.TypeName));

            if (mComponentNameIndex == -1 || mComponentNameIndex >= mComponentNames.Length)
            {
                mComponentNameIndex = 0;
            }

            mComponentName = serializedObject.FindProperty("mComponentName");
        }

        private Lazy<GUIStyle> mLabel12 = new Lazy<GUIStyle>(() => new GUIStyle(GUI.skin.label)
        {
            fontSize = 12
        });

        private SerializedProperty mComponentName;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            GUILayout.BeginVertical("box");
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label(mLocaleText.Bind, mLabel12.Value, GUILayout.Width(60));

            mBindScript.MarkType = (BindType)EditorGUILayout.EnumPopup(mBindScript.MarkType);

            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            if (mBindScript.CustomComponentName == null ||
                string.IsNullOrEmpty(mBindScript.CustomComponentName.Trim()))
            {
                mBindScript.CustomComponentName = mBindScript.name;
            }

            if (mBindScript.MarkType == BindType.DefaultUnityElement)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label(mLocaleText.Type, mLabel12.Value, GUILayout.Width(60));

                EditorGUI.BeginChangeCheck();
                mComponentNameIndex = EditorGUILayout.Popup(mComponentNameIndex, mComponentNames);
                if (EditorGUI.EndChangeCheck())
                {
                    
                    mComponentName.stringValue = mComponentNames[mComponentNameIndex];
                    EditorUtility.SetDirty(target);
                }

                GUILayout.EndHorizontal();
            }


            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Label(mLocaleText.BelongsTo, mLabel12.Value, GUILayout.Width(60));

            GUILayout.Label(CodeGenHelper.GetBindBelongs2(mBindScript), mLabel12.Value, GUILayout.Width(200));

            if (GUILayout.Button(mLocaleText.Select, GUILayout.Width(60)))
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
                GUILayout.Label(mLocaleText.ClassName, mLabel12.Value, GUILayout.Width(60));
                mBindScript.CustomComponentName = EditorGUILayout.TextField(mBindScript.CustomComponentName);

                GUILayout.EndHorizontal();
            }

            GUILayout.Space(10);

            GUILayout.Label(mLocaleText.Comment, mLabel12.Value);

            GUILayout.Space(10);

            mBindScript.CustomComment = EditorGUILayout.TextArea(mBindScript.Comment, GUILayout.Height(100));

            var rootGameObj = CodeGenHelper.GetBindBelongs2GameObject(mBindScript);

            if (rootGameObj)
            {
                if (GUILayout.Button(mLocaleText.Generate + " (" + rootGameObj.name + ")",
                        GUILayout.Height(30)))
                {
                    CodeGenKit.Generate(rootGameObj.GetComponent<IBindGroup>());
                }
            }
            
            GUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
            
            base.OnInspectorGUI();
        }
    }
}
#endif