namespace QFramework.PackageKit
{
 public class PackageListHeaderView : HorizontalLayout
    {
        public PackageListHeaderView()
        {
            HorizontalStyle = "box";

            new LabelView(LocaleText.PackageName)
                .Width(200)
                .FontSize(12)
                .FontBold()
                .AddTo(this);

            new LabelView(LocaleText.ServerVersion)
                .Width(80)
                .TextMiddleCenter()
                .FontSize(12)
                .FontBold()
                .AddTo(this);

            new LabelView(LocaleText.LocalVersion)
                .Width(80)
                .TextMiddleCenter()
                .FontSize(12)
                .FontBold()
                .AddTo(this);

            new LabelView(LocaleText.AccessRight)
                .Width(50)
                .TextMiddleCenter()
                .FontSize(12)
                .FontBold()
                .AddTo(this);

            // new LabelView(LocaleText.Doc)
            //     .Width(40)
            //     .TextMiddleCenter()
            //     .FontSize(12)
            //     .FontBold()
            //     .AddTo(this);

            new LabelView(LocaleText.Action)
                .Width(100)
                .TextMiddleCenter()
                .FontSize(12)
                .FontBold()
                .AddTo(this);

            new LabelView(LocaleText.ReleaseNote)
                .Width(100)
                .TextMiddleCenter()
                .FontSize(12)
                .FontBold()
                .AddTo(this);
            
            new LabelView(LocaleText.AuthorName)
                .Width(140)
                .TextMiddleCenter()
                .FontSize(12)
                .FontBold()
                .AddTo(this);
        }

        class LocaleText
        {
            public static string PackageName
            {
                get { return Language.IsChinese ? " 模块名" : " Package Name"; }
            }
            
            public static string AuthorName
            {
                get { return Language.IsChinese ? " 作者" : " Author"; }
            }

            public static string ServerVersion
            {
                get { return Language.IsChinese ? "服务器版本" : "Server Version"; }
            }

            public static string LocalVersion
            {
                get { return Language.IsChinese ? "本地版本" : "Local Version"; }
            }

            public static string AccessRight
            {
                get { return Language.IsChinese ? "访问权限" : "Access Right"; }
            }

            public static string Doc
            {
                get { return Language.IsChinese ? "文档" : "Doc"; }
            }

            public static string Action
            {
                get { return Language.IsChinese ? "动作" : "Action"; }
            }

            public static string ReleaseNote
            {
                get { return Language.IsChinese ? "版本说明" : "ReleaseNote Note"; }
            }
        }
    }
}