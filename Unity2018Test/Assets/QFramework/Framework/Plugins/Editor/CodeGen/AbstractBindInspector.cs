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

using System.Linq;
using UnityEditor;
using UnityEngine;

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

        private VerticalLayout mRootLayout;
        private IHorizontalLayout mComponentLine;
        private HorizontalLayout mClassnameLine;

        private void OnEnable()
        {
            mRootLayout = new VerticalLayout("box");

            EasyIMGUI.Space()
                .Parent(mRootLayout);

            var markTypeLine = EasyIMGUI.Horizontal()
                .Parent(mRootLayout);

            EasyIMGUI.Label().Text(LocaleText.MarkType)
                .FontSize(12)
                .Width(60)
                .Parent(markTypeLine);

            var enumPopupView = new EnumPopupView(mBindScript.MarkType)
                .Parent(markTypeLine);

            enumPopupView.ValueProperty.Bind(newValue =>
            {
                mBindScript.MarkType = (BindType) newValue;

                OnRefresh();
            });


            EasyIMGUI.Space()
                .Parent(mRootLayout);

            EasyIMGUI.Custom().OnGUI(() =>
            {
                if (mBindScript.CustomComponentName == null ||
                    string.IsNullOrEmpty(mBindScript.CustomComponentName.Trim()))
                {
                    mBindScript.CustomComponentName = mBindScript.name;
                }
            }).Parent(mRootLayout);


            mComponentLine = EasyIMGUI.Horizontal();

            EasyIMGUI.Label().Text(LocaleText.Type)
                .Width(60)
                .FontSize(12)
                .Parent(mComponentLine);

            if (mBindScript.MarkType == BindType.DefaultUnityElement)
            {
                var components = mBindScript.GetComponents<Component>();

                var componentNames = components.Where(c => !(c is AbstractBind))
                    .Select(c => c.GetType().FullName)
                    .ToArray();

                var componentNameIndex = 0;

                componentNameIndex = componentNames.ToList()
                    .FindIndex((componentName) => componentName.Contains(mBindScript.ComponentName));

                if (componentNameIndex == -1 || componentNameIndex >= componentNames.Length)
                {
                    componentNameIndex = 0;
                }

                mBindScript.ComponentName = componentNames[componentNameIndex];

                PopupView.Create()
                    .WithIndexAndMenus(componentNameIndex, componentNames)
                    .OnIndexChanged(index => { mBindScript.ComponentName = componentNames[index]; })
                    .Parent(mComponentLine);
            }

            mComponentLine.Parent(mRootLayout);

            EasyIMGUI.Space()
                .Parent(mRootLayout);

            var belongsTo = EasyIMGUI.Horizontal()
                .Parent(mRootLayout);

            EasyIMGUI.Label().Text(LocaleText.BelongsTo)
                .Width(60)
                .FontSize(12)
                .Parent(belongsTo);

            EasyIMGUI.Label().Text(CodeGenUtil.GetBindBelongs2(target as AbstractBind))
                .Width(200)
                .FontSize(12)
                .Parent(belongsTo);


            EasyIMGUI.Button()
                .Text(LocaleText.Select)
                .OnClick(() =>
                {
                    Selection.objects = new Object[]
                    {
                        CodeGenUtil.GetBindBelongs2GameObject(target as AbstractBind)
                    };
                })
                .Width(60)
                .Parent(belongsTo);

            mClassnameLine = new HorizontalLayout();

            EasyIMGUI.Label().Text(LocaleText.ClassName)
                .Width(60)
                .FontSize(12)
                .Parent(mClassnameLine);

            EasyIMGUI.TextField().Text(mBindScript.CustomComponentName)
                .Parent(mClassnameLine)
                .Content.Bind(newValue => { mBindScript.CustomComponentName = newValue; });

            mClassnameLine.Parent(mRootLayout);

            EasyIMGUI.Space()
                .Parent(mRootLayout);

            EasyIMGUI.Label().Text(LocaleText.Comment)
                .FontSize(12)
                .Parent(mRootLayout);

            EasyIMGUI.Space()
                .Parent(mRootLayout);

            EasyIMGUI.TextArea()
                .Text(mBindScript.Comment)
                .Height(100)
                .Parent(mRootLayout)
                .Content.Bind(newValue => mBindScript.CustomComment = newValue);

            var bind = target as AbstractBind;
            var rootGameObj = CodeGenUtil.GetBindBelongs2GameObject(bind);


            if (rootGameObj.transform.GetComponent("ILKitBehaviour"))
            {
            }
            else if (rootGameObj.transform.IsUIPanel())
            {
                EasyIMGUI.Button()
                    .Text(LocaleText.Generate + " " + CodeGenUtil.GetBindBelongs2(bind))
                    .OnClick(() =>
                    {
                        var rootPrefabObj = PrefabUtility.GetCorrespondingObjectFromSource<Object>(rootGameObj);
                        UICodeGenerator.DoCreateCode(new[] {rootPrefabObj});
                    })
                    .Height(30)
                    .Parent(mRootLayout);
            }
            else if (rootGameObj.transform.IsViewController())
            {
                EasyIMGUI.Button()
                    .Text(LocaleText.Generate + " " + CodeGenUtil.GetBindBelongs2(bind))
                    .OnClick(() => { CreateViewControllerCode.DoCreateCodeFromScene(bind.gameObject); })
                    .Height(30)
                    .Parent(mRootLayout);
            }


            OnRefresh();
        }

        private void OnRefresh()
        {
            if (mBindScript.MarkType == BindType.DefaultUnityElement)
            {
                mComponentLine.Visible = true;
                mClassnameLine.Visible = false;
            }
            else
            {
                mClassnameLine.Visible = true;
                mComponentLine.Visible = false;
            }
        }

        private void OnDisable()
        {
            mRootLayout.Clear();
            mRootLayout = null;
        }

        public override void OnInspectorGUI()
        {
            mRootLayout.DrawGUI();
            base.OnInspectorGUI();
        }
    }
}