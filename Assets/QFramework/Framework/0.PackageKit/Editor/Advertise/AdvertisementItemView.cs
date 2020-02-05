using UnityEngine;

namespace QFramework.PackageKit
{
    public class AdvertisementItemView : HorizontalLayout
    {
        public AdvertisementItemView(string title,string link) : base("box")
        {
            new LabelView(title).FontBold().AddTo(this);

            new FlexibaleSpaceView().AddTo(this);
            
            new ButtonView(LocalText.Open, () => { Application.OpenURL(link); })
                .Width(200)
                .AddTo(this);
        }

        public class LocalText
        {
            public static string Open
            {
                get { return Language.IsChinese ? "打开" : "Open"; }
            }
        }
    }
}