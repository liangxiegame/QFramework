using UnityEngine;

namespace QFramework
{
    public class AdvertisementItemView : HorizontalLayout
    {
        private readonly IXMLView mView = EasyIMGUI.XMLView();
        
        public AdvertisementItemView(string title, string link)
        {
            Box();
            
            AddChild(mView.LoadXML(Application.dataPath + "/QFramework/Framework/Plugins/PackageKit/SupportItem.xml"));

            mView.GetById<ILabel>("title").Text(title);

            mView.GetById<IButton>("linkBtn")
                .Text(LocalText.Open)
                .OnClick(() =>
                {
                    Application.OpenURL(link);
                });
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