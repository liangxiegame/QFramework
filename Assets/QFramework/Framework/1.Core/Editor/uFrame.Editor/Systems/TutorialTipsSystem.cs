using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QF.GraphDesigner;

namespace QF.GraphDesigner
{
    public class TutorialTipsSystem : DiagramPlugin
        , IShowTutorialTip
    {
        private IPlatformOperations _platform;
        private const string PrefCode = "ShowTutorialTips";

        [InspectorProperty]
        public bool ShowTutorialTips {
            get { return InvertGraphEditor.Prefs.GetBool(PrefCode, true); }
            set { InvertGraphEditor.Prefs.SetBool(PrefCode,value); }
        }


        public IPlatformOperations Platform
        {
            get { return _platform ?? (_platform = InvertApplication.Container.Resolve<IPlatformOperations>()); }
            set { _platform = value; }
        }

        public void ShowTutorialTip(TutorialTips tip)
        {
            string message = null;
            List<NotifyActionItem> items = new List<NotifyActionItem>();

            switch (tip)
            {
                case TutorialTips.DatabasesScreen:
                    message = "This is a database management screen.";
                    items.Add(new NotifyActionItem()
                    {
                        Title = "Learn more...",
                        Action = () =>
                        {
                            Platform.OpenLink("http://google.com");
                        }
                    });
                    break;
                case TutorialTips.WorkspacesScreen:
                    break;
                case TutorialTips.GraphsScreen:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("tip", tip, null);
            }

            items.Add(new NotifyActionItem()
            {
                Title = "Disable Tips",
                Action = () =>
                {
                    ShowTutorialTips = false;
                }
            });

            Signal<INotify>(_ => _.NotifyWithActions(message, NotificationIcon.Info, items.ToArray()));
        }
    }
}
