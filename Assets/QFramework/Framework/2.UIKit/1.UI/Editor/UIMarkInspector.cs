using EGO.Framework;
using UnityEditor;

namespace QFramework
{
    [CustomEditor(typeof(UIMark))]
    public class UIMarkInspector : UnityEditor.Editor
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


        private UIMark mUIMarkScript
        {
            get { return target as UIMark; }
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

            var enumPopupView = new EnumPopupView<UIMarkType>(mUIMarkScript.MarkType)
                .AddTo(markTypeLine);

            enumPopupView.ValueProperty.Bind(newValue =>
            {
                mUIMarkScript.MarkType = (UIMarkType) newValue;

                OnRefresh();
            });

            new EGO.Framework.SpaceView()
                .AddTo(mRootLayout);

            new CustomView(() =>
            {
                if (mUIMarkScript.CustomComponentName == null ||
                    string.IsNullOrEmpty(mUIMarkScript.CustomComponentName.Trim()))
                {
                    mUIMarkScript.CustomComponentName = mUIMarkScript.name;
                }
            }).AddTo(mRootLayout);


            mComponentLine = new HorizontalLayout();

            new EGO.Framework.LabelView(LocaleText.Type)
                .Width(100)
                .FontBold()
                .FontSize(12)
                .AddTo(mComponentLine);

            new EGO.Framework.LabelView(mUIMarkScript.ComponentName)
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

            new TextView(mUIMarkScript.CustomComponentName)
                .AddTo(mClassnameLine)
                .Content.Bind(newValue => mUIMarkScript.CustomComponentName = newValue);

            mClassnameLine.AddTo(mRootLayout);

            new EGO.Framework.SpaceView()
                .AddTo(mRootLayout);

            new EGO.Framework.LabelView(LocaleText.Comment)
                .FontSize(12)
                .FontBold()
                .AddTo(mRootLayout);

            new EGO.Framework.SpaceView()
                .AddTo(mRootLayout);

            new TextAreaView(mUIMarkScript.Comment)
                .Height(100)
                .AddTo(mRootLayout)
                .Content.Bind(newValue => mUIMarkScript.CustomComment = newValue);

            OnRefresh();
        }

        private void OnRefresh()
        {
            if (mUIMarkScript.MarkType == UIMarkType.DefaultUnityElement)
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