using UnityEngine;

namespace QFramework
{
    public class AdvertisementItemView : HorizontalLayout
    {
        public AdvertisementItemView(string title, string link)
        {
            Box();
            
            EasyIMGUI.Label()
                .Text(title)
                .FontBold()
                .AddTo(this);
            
            EasyIMGUI.FlexibleSpace().AddTo(this);

            EasyIMGUI.Button()
                .Text(LocalText.Open)
                .OnClick(() => { Application.OpenURL(link); })
                .Width(200)
                .AddTo(this);
        }

        class LocalText
        {
            public static string Open
            {
                get { return Language.IsChinese ? "打开" : "Open"; }
            }
        }
    }
}