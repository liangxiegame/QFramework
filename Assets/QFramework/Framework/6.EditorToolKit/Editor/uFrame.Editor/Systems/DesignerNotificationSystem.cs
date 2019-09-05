using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Invert.Common;
using QF.GraphDesigner;
using QF.GraphDesigner.Systems;
using UnityEditor;
using UnityEngine;

namespace QF.GraphDesigner.Unity.Notifications
{

    public class DesignerNotificationSystem : DiagramPlugin, INotify, IDesignerWindowEvents
    {
        private List<NotificationItem> _items;
        private IStyleProvider _styleProvider;

        private List<NotificationItem> Items
        {
            get { return _items ?? (_items = new List<NotificationItem>()); }
            set { _items = value; }
        }

        public IStyleProvider StyleProvider
        {
            get { return _styleProvider ?? (_styleProvider = InvertApplication.Container.Resolve<IStyleProvider>()); }
            set { _styleProvider = value; }
        }
        
        public IPlatformDrawer PlatformDrawer
        {
            get { return _platformDrawer ?? (_platformDrawer = InvertApplication.Container.Resolve<IPlatformDrawer>()); }
            set { _platformDrawer = value; }
        }

        public void Notify(string message, string icon, int time = 5000)
        {
            Notify(message,icon,time,false,null);
            //Add notification item to the queue
        }

        protected void Notify(string message, string icon, int time, bool requireClose, NotifyActionItem[] actions)
        {
            var index = Enumerable.Range(0, 10).First(i => Items.All(n => n.Index != i));
            if (time == 0) requireClose = true;
            time += 800;
            var notificationItem = new NotificationItem()
            {
                AnimatedX = 0,
                Icon = StyleProvider.GetImage(icon),
                Message = message,
                TimeLeft = time,
                Time = time,
                Index = index,
                RequireClose = requireClose,
                Actions = actions
            };

            EditorApplication.delayCall +=
                () => {
                          Items.Add(notificationItem);
                };

            Signal<IDebugWindowEvents>(_ => _.RegisterInspectedItem(notificationItem, notificationItem.Message, true));
            //Add notification item to the queue
        }

        public void Notify(string message, NotificationIcon icon, int time = 5000)
        {

            Notify(message, GetCodeByIcon(icon), time);
        }

        protected string GetCodeByIcon(NotificationIcon icon)
        {
            string iconCode = "InfoIcon";
            switch (icon)
            {
                case NotificationIcon.Info:
                    iconCode = "InfoIcon";
                    break;
                case NotificationIcon.Error:
                    iconCode = "ErrorIcon";
                    break;
                case NotificationIcon.Warning:
                    iconCode = "WarningIcon";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("icon", icon, null);
            }
            return iconCode;
        }

        public void NotifyWithActions(string message, NotificationIcon icon, params NotifyActionItem[] actions)
        {
        
            Notify(message,GetCodeByIcon(icon),0,true,actions);
        }

        public void AfterDrawGraph(Rect diagramRect)
        {

         
        }

        public void BeforeDrawGraph(Rect diagramRect)
        {
        }

        public void AfterDrawDesignerWindow(Rect windowRect)
        {
            _deltaTime = (float)(DateTime.Now - _lastUpdate).TotalMilliseconds;

            var firstUnpaddedItemRect = windowRect.WithSize(300, 150).InnerAlignWithBottomRight(windowRect).Translate(-15,-70);
            
            foreach (var item in Items.ToArray())
            {

                var unpaddedItemRect = firstUnpaddedItemRect.AboveAll(firstUnpaddedItemRect, item.Index);

                if (item.TimeLeft > item.Time - 400)
                {
                    item.TimeLeft -= _deltaTime;
                    item.AnimatedX = Mathf.Lerp(400, 0, (item.Time - item.TimeLeft) / 400);
                }
                else if (!item.RequireClose && item.TimeLeft <= 400)
                {
                    item.TimeLeft -= _deltaTime;
                    if (item.TimeLeft < 0)
                    {
                        Items.Remove(item);
                        continue;
                    }
                    item.AnimatedX = Mathf.Lerp(400, 0, item.TimeLeft/400);
                }


                if (!item.RequireClose)
                {
                    item.TimeLeft -= _deltaTime;
                }
                //else item.AnimatedX = 0;
                
                var paddedItemRect = unpaddedItemRect.WithOrigin(unpaddedItemRect.x+item.AnimatedX, unpaddedItemRect.y).PadSides(15);

                PlatformDrawer.DrawStretchBox(paddedItemRect,CachedStyles.TooltipBoxStyle,14);
                if (item.RequireClose){
                    var closeButtonRect = new Rect().WithSize(16, 16).InnerAlignWithUpperRight(paddedItemRect).Translate(-12,12);
                        PlatformDrawer.DrawImage(closeButtonRect, "CloseIcon", true);
                    var item1 = item;
                    PlatformDrawer.DoButton(closeButtonRect.PadSides(-3),"",CachedStyles.ClearItemStyle,m=>
                    {
                        item1.RequireClose = false;
                    });
                }
                
                paddedItemRect = paddedItemRect.PadSides(15);
                PlatformDrawer.DrawLabel(paddedItemRect.Pad(0,0,40,0),item.Message,CachedStyles.BreadcrumbTitleStyle,item.Actions == null || item.Actions.Length == 0 ? DrawingAlignment.MiddleLeft : DrawingAlignment.TopLeft);
                PlatformDrawer.DrawImage(paddedItemRect.WithSize(33,33).InnerAlignWithBottomRight(paddedItemRect).AlignHorisonallyByCenter(paddedItemRect),item.Icon,true);

                if (item.Actions != null && item.Actions.Length > 0)
                {
                    Rect lastItemRect = new Rect().WithSize(0, 27).InnerAlignWithBottomLeft(paddedItemRect).Translate(0, 10);
                    foreach (var actionItem in item.Actions)
                    {
                        var itemRect =
                            lastItemRect.WithSize(
                                PlatformDrawer.CalculateTextSize(actionItem.Title, ElementDesignerStyles.ButtonStyle)
                                    .x+15, 27).RightOf(lastItemRect);

                       PlatformDrawer.DoButton(itemRect,actionItem.Title,ElementDesignerStyles.ButtonStyle,actionItem.Action);
                        lastItemRect = itemRect;
                    }
                }

           


            }

            _lastUpdate = DateTime.Now;
        }

        public void DrawComplete()
        {
        }

        public void ProcessInput()
        {
        }

        private DateTime _lastUpdate;
        private float _deltaTime;
        private IPlatformDrawer _platformDrawer;

        internal class NotificationItem
        {
            public object Icon { get; set; }
            public string Message { get; set; }
            public float AnimatedX { get; set; }
            public float TimeLeft { get; set; }
            public int Index { get; set; }
            public int Time { get; set; }
            public bool RequireClose { get; set; }
            public NotifyActionItem[] Actions { get; set; }
        }

      
    }

    


}
