using System.Linq;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    [CustomEditor(typeof(Bind), true)]
    public class BindInspector : UnityEditor.Editor
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


        private Bind mBindScript
        {
            get { return target as Bind; }
        }

        private VerticalLayout mRootLayout;
        private HorizontalLayout mComponentLine;
        private HorizontalLayout mClassnameLine;

        private void OnEnable()
        {
            mRootLayout = new VerticalLayout("box");

            EasyIMGUI.Space()
                .AddTo(mRootLayout);

            var markTypeLine = new HorizontalLayout()
                .AddTo(mRootLayout);

            EasyIMGUI.Label().Text(LocaleText.MarkType)
                .FontSize(12)
                .Width(60)
                .AddTo(markTypeLine);

            var enumPopupView = new EnumPopupView(mBindScript.MarkType)
                .AddTo(markTypeLine);

            enumPopupView.ValueProperty.Bind(newValue =>
            {
                mBindScript.MarkType = (BindType) newValue;

                OnRefresh();
            });


            EasyIMGUI.Space()
                .AddTo(mRootLayout);

            EasyIMGUI.Custom().OnGUI(() =>
            {
                if (mBindScript.CustomComponentName == null ||
                    string.IsNullOrEmpty(mBindScript.CustomComponentName.Trim()))
                {
                    mBindScript.CustomComponentName = mBindScript.name;
                }
            }).AddTo(mRootLayout);


            mComponentLine = new HorizontalLayout();

            EasyIMGUI.Label().Text(LocaleText.Type)
                .Width(60)
                .FontSize(12)
                .AddTo(mComponentLine);

            if (mBindScript.MarkType == BindType.DefaultUnityElement)
            {
                var components = mBindScript.GetComponents<Component>();

                var componentNames = components.Where(c => c.GetType() != typeof(Bind))
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

                new PopupView(componentNameIndex, componentNames)
                    .AddTo(mComponentLine)
                    .IndexProperty.Bind((index) => { mBindScript.ComponentName = componentNames[index]; });
            }

            mComponentLine.AddTo(mRootLayout);

            EasyIMGUI.Space()
                .AddTo(mRootLayout);

            var belongsTo = new HorizontalLayout()
                .AddTo(mRootLayout);

            EasyIMGUI.Label().Text(LocaleText.BelongsTo)
                .Width(60)
                .FontSize(12)
                .AddTo(belongsTo);

            EasyIMGUI.Label().Text(CodeGenUtil.GetBindBelongs2(target as Bind))
                .Width(200)
                .FontSize(12)
                .AddTo(belongsTo);


            EasyIMGUI.Button()
                .Text(LocaleText.Select)
                .OnClick(() =>
                {
                    Selection.objects = new[]
                    {
                        CodeGenUtil.GetBindBelongs2GameObject(target as Bind)
                    };
                })
                .Width(60)
                .AddTo(belongsTo);

            mClassnameLine = new HorizontalLayout();

            EasyIMGUI.Label().Text(LocaleText.ClassName)
                .Width(60)
                .FontSize(12)
                .AddTo(mClassnameLine);

            EasyIMGUI.TextField().Text(mBindScript.CustomComponentName)
                .AddTo(mClassnameLine)
                .Content.Bind(newValue => { mBindScript.CustomComponentName = newValue; });

            mClassnameLine.AddTo(mRootLayout);

            EasyIMGUI.Space()
                .AddTo(mRootLayout);

            EasyIMGUI.Label().Text(LocaleText.Comment)
                .FontSize(12)
                .AddTo(mRootLayout);

            EasyIMGUI.Space()
                .AddTo(mRootLayout);

            EasyIMGUI.TextArea()
                .Text(mBindScript.Comment)
                .Height(100)
                .AddTo(mRootLayout)
                .Content.Bind(newValue => mBindScript.CustomComment = newValue);

            var bind = target as Bind;
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
                        var rootPrefabObj = PrefabUtility.GetPrefabParent(rootGameObj);
                        UICodeGenerator.DoCreateCode(new[] {rootPrefabObj});
                    })
                    .Height(30)
                    .AddTo(mRootLayout);
            }
            else if (rootGameObj.transform.IsViewController())
            {
                EasyIMGUI.Button()
                    .Text(LocaleText.Generate + " " + CodeGenUtil.GetBindBelongs2(bind))
                    .OnClick(() => { CreateViewControllerCode.DoCreateCodeFromScene(bind.gameObject); })
                    .Height(30)
                    .AddTo(mRootLayout);
            }


            OnRefresh();
        }

        private void OnRefresh()
        {
            if (mBindScript.MarkType == BindType.DefaultUnityElement)
            {
                mComponentLine.Show();
                mClassnameLine.Hide();
            }
            else
            {
                mClassnameLine.Show();
                mComponentLine.Hide();
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