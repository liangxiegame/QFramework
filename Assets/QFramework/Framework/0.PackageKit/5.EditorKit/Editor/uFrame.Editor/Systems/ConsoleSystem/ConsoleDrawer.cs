using System;
using Invert.Common;
using UnityEngine;

namespace QF.GraphDesigner.Unity.WindowsPlugin
{
    public class ConsoleDrawer : Drawer<ConsoleViewModel>
    {
        public ConsoleDrawer(ConsoleViewModel viewModelObject) : base(viewModelObject)
        {

        }

        public override void Draw(IPlatformDrawer platform, float scale)
        {
            base.Draw(platform, scale);

            var messageRect = new Rect(5, 5, 100, 15);

            foreach (var messages in ViewModel.Messages)
            {

                var message = string.Format("{0} : {1}", Enum.GetName(typeof (MessageType), messages.MessageType), 
                    messages.Message);

                platform.DrawLabel(messageRect,message,CachedStyles.HeaderTitleStyle);
        
                messageRect = new Rect(messageRect)
                {
                    y = messageRect.y + messageRect.height
                };
            }


            var typeRect = new Rect(250, 5, 150, 24);

            platform.DoButton(typeRect, "All", ElementDesignerStyles.ButtonStyle, () =>
            {
                ViewModel.SelectFilterType(null);
            });
            foreach (var type in ViewModel.AvailableTypes)
            {

                typeRect = new Rect(typeRect)
                {
                    y = typeRect.y + typeRect.height
                };

                var type1 = type;
                platform.DoButton(typeRect, type.Name, ElementDesignerStyles.ButtonStyle, () =>
                {
                    ViewModel.SelectFilterType(type1);
                });
            }


        }
    }
}