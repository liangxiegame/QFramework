using EGO.Framework;
using UnityEditor;

namespace QFramework
{
    [CustomEditor(typeof(Bind),true)]
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

            new EGO.Framework.SpaceView()
                .AddTo(mRootLayout);

            var markTypeLine = new HorizontalLayout()
                .AddTo(mRootLayout);

            new EGO.Framework.LabelView(LocaleText.MarkType)
                .FontBold()
                .FontSize(12)
                .Width(100)
                .AddTo(markTypeLine);

            var enumPopupView = new EnumPopupView(mBindScript.MarkType)
                .AddTo(markTypeLine);

            enumPopupView.ValueProperty.Bind(newValue =>
            {
                mBindScript.MarkType = (BindType)newValue;

                OnRefresh();
            });

            new EGO.Framework.SpaceView()
                .AddTo(mRootLayout);

            new CustomView(() =>
            {
                if (mBindScript.CustomComponentName == null ||
                    string.IsNullOrEmpty(mBindScript.CustomComponentName.Trim()))
                {
                    mBindScript.CustomComponentName = mBindScript.name;
                }
            }).AddTo(mRootLayout);


            mComponentLine = new HorizontalLayout();

            new EGO.Framework.LabelView(LocaleText.Type)
                .Width(100)
                .FontBold()
                .FontSize(12)
                .AddTo(mComponentLine);

            new EGO.Framework.LabelView(mBindScript.ComponentName)
                .FontBold()
                .FontSize(12)
                .AddTo(mComponentLine);

            mComponentLine.AddTo(mRootLayout);

            mClassnameLine = new HorizontalLayout();

            new EGO.Framework.LabelView(LocaleText.ClassName)
                .Width(100)
                .FontBold()
                .FontSize(12)
                .AddTo(mClassnameLine);

            new TextView(mBindScript.CustomComponentName)
                .AddTo(mClassnameLine)
                .Content.Bind(newValue =>
                {
                    mBindScript.CustomComponentName = newValue;
                });

            mClassnameLine.AddTo(mRootLayout);

            new EGO.Framework.SpaceView()
                .AddTo(mRootLayout);

            new EGO.Framework.LabelView(LocaleText.Comment)
                .FontSize(12)
                .FontBold()
                .AddTo(mRootLayout);

            new EGO.Framework.SpaceView()
                .AddTo(mRootLayout);

            new TextAreaView(mBindScript.Comment)
                .Height(100)
                .AddTo(mRootLayout)
                .Content.Bind(newValue => mBindScript.CustomComment = newValue);

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