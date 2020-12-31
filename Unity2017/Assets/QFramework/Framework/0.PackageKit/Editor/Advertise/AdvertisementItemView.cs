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
                .Parent(this);
            
            EasyIMGUI.FlexibleSpace().Parent(this);

            EasyIMGUI.Button()
                .Text(LocalText.Open)
                .OnClick(() => { Application.OpenURL(link); })
                .Width(200)
                .Parent(this);
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