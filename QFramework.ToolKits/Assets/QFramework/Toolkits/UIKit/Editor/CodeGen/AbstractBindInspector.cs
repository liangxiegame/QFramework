/****************************************************************************
 * Copyright (c) 2017 ~ 2021.1 liangxie
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

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
        class LocaleText
        {
            public static string MarkType
            {
                get { return Language.IsChinese ? " 标记类型:" : " Mark Type:"; }
            }

            public static string Type
            {
                get { return Language.IsChinese ? " 类型:" : " Type:"; }
            }

            public static string ClassName
            {
                get { return Language.IsChinese ? " 生成类名:" : " Generate Class Name:"; }
            }

            public static string Comment
            {
                get { return Language.IsChinese ? " 注释" : " Comment"; }
            }

            public static string BelongsTo
            {
                get { return Language.IsChinese ? " 属于:" : " Belongs 2:"; }
            }

            public static string Select
            {
                get { return Language.IsChinese ? "选择" : "Select"; }
            }

            public static string Generate
            {
                get { return Language.IsChinese ? " 生成代码" : " Generate Code"; }
            }
        }


        private AbstractBind mBindScript
        {
            get { return target as AbstractBind; }
        }


        private string[] mComponentNames;
        private int mComponentNameIndex;
        private void OnEnable()
        {
            var components = mBindScript.GetComponents<Component>();

            mComponentNames = components.Where(c => !(c is AbstractBind))
                .Select(c => c.GetType().FullName)
                .ToArray();
            
            mComponentNameIndex = mComponentNames.ToList()
                .FindIndex((componentName) => componentName.Contains(mBindScript.ComponentName));

            if (mComponentNameIndex == -1 || mComponentNameIndex >= mComponentNames.Length)
            {
                mComponentNameIndex = 0;
            }
        }

        private Lazy<GUIStyle> mLabel12 = new Lazy<GUIStyle>(() => new GUIStyle(GUI.skin.label)
        {
            fontSize = 12
        });

        public override void OnInspectorGUI()
        {
            GUILayout.BeginVertical("box");
            GUILayout.Space(10);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label(LocaleText.MarkType,mLabel12.Value,GUILayout.Width(60));

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
                
                GUILayout.Label(LocaleText.Type,mLabel12.Value,GUILayout.Width(60));
                
                mComponentNameIndex = EditorGUILayout.Popup(mComponentNameIndex, mComponentNames);
                mBindScript.ComponentName = mComponentNames[mComponentNameIndex];

                
                GUILayout.EndHorizontal();
            }
            
            
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Label(LocaleText.BelongsTo,mLabel12.Value,GUILayout.Width(60));

            GUILayout.Label(CodeGenUtil.GetBindBelongs2(mBindScript),mLabel12.Value,GUILayout.Width(200));

            if (GUILayout.Button(LocaleText.Select, GUILayout.Width(60)))
            {
                Selection.objects = new Object[]
                {
                    CodeGenUtil.GetBindBelongs2GameObject(target as AbstractBind)
                };
            }
            
            GUILayout.EndHorizontal();
            
            if (mBindScript.MarkType != BindType.DefaultUnityElement)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(LocaleText.ClassName,mLabel12.Value,GUILayout.Width(60));
                mBindScript.CustomComponentName =  EditorGUILayout.TextField(mBindScript.CustomComponentName);

                GUILayout.EndHorizontal();
            }
            
            GUILayout.Space(10);

            GUILayout.Label(LocaleText.Comment,mLabel12.Value);
            
            GUILayout.Space(10);
            
            mBindScript.CustomComment = EditorGUILayout.TextArea(mBindScript.Comment, GUILayout.Height(100));

            var rootGameObj = CodeGenUtil.GetBindBelongs2GameObject(mBindScript);
            if (rootGameObj.transform.GetComponent("ILKitBehaviour"))
            {
            }
            else if (rootGameObj.transform.IsUIPanel())
            {
                if (GUILayout.Button(LocaleText.Generate + " " + CodeGenUtil.GetBindBelongs2(mBindScript),GUILayout.Height(30)))
                {
                    var rootPrefabObj = PrefabUtility.GetCorrespondingObjectFromSource<Object>(rootGameObj);
                    UICodeGenerator.DoCreateCode(new[] {rootPrefabObj});
                }
            }
            else if (rootGameObj.transform.IsViewController())
            {
                if (GUILayout.Button(LocaleText.Generate + " " + CodeGenUtil.GetBindBelongs2(mBindScript),
                    GUILayout.Height(30)))
                {
                    CreateViewControllerCode.DoCreateCodeFromScene(mBindScript.gameObject);
                }
            }

            GUILayout.EndVertical();
            
            
            base.OnInspectorGUI();
        }

  

        private void OnDisable()
        {
        }


    }
}