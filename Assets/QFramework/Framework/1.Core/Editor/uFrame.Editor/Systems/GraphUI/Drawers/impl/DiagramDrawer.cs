using System;
using System.Collections.Generic;
using System.Linq;
using Invert.Common;
using UnityEngine;
using NotifyCollectionChangedEventArgs = QF.MVVM.NotifyCollectionChangedEventArgs;

namespace QF.GraphDesigner
{    
    public class DiagramDrawer : Drawer, IInputHandler
    {
        public delegate void SelectionChangedEventArgs(IDiagramNode oldData, IDiagramNode newData);
        public event SelectionChangedEventArgs SelectionChanged;

        private IDrawer mNodeDrawerAtMouse;
        private SelectionRectHandler mSelectionRectHandler;
        private IDrawer[] mCachedChildren = { };
        private Dictionary<IGraphFilter, Vector2> mCachedPaths;

        public static float Scale
        {
            get { return InvertGraphEditor.CurrentDiagramViewModel.Scale; }
        }

        public IDiagramNode CurrentMouseOverNode { get; set; }

        public DiagramViewModel DiagramViewModel
        {
            get { return this.DataContext as DiagramViewModel; }
            set { this.DataContext = value; }
        }


        public Rect Rect { get; set; }



        public DiagramDrawer(DiagramViewModel viewModel)
        {
            DiagramViewModel = viewModel;
        }

        public static Rect CreateSelectionRect(Vector2 start, Vector2 current)
        {
            if (current.x > start.x)
            {
                if (current.y > start.y)
                {
                    return new Rect(start.x, start.y,
                        current.x - start.x, current.y - start.y);
                }
                else
                {
                    return new Rect(
                        start.x, current.y, current.x - start.x, start.y - current.y);
                }
            }
            else
            {
                if (current.y > start.y)
                {
                    // x is less and y is greater
                    return new Rect(
                        current.x, start.y, start.x - current.x, current.y - start.y);
                }
                else
                {
                    // both are less
                    return new Rect(
                        current.x, current.y, start.x - current.x, start.y - current.y);
                }
            }
        }


        public void DrawTabs(IPlatformDrawer platform, Rect tabsRect)
        {
            var designerWindow = DiagramViewModel.NavigationViewModel.DesignerWindow;
            if (designerWindow != null && designerWindow.Designer != null)
            {
                var x = 1f;
                foreach (var tab in DiagramViewModel.NavigationViewModel.Tabs.ToArray())
                {
                    if (tab == null) continue;
                    if (tab.Title == null)
                        continue;

                    var textSize = platform.CalculateTextSize(tab.Title, CachedStyles.TabTitleStyle);
                    
                    var buttonRect=  new Rect()
                        .AlignAndScale(tabsRect)
                        .WithWidth(Math.Max(textSize.x + 21 + 16,60))
                        .Translate(x,0);

                    var buttonBoxRect = new Rect().AlignAndScale(buttonRect)
                        .WithWidth(textSize.x + 10);
                    
                    var textRect = new Rect()
                        .AlignAndScale(buttonRect)
                        .Pad(7, 0, 7, 0);

                    var closeButton = new Rect()
                        .WithSize(16, 16)
                        .AlignTopRight(buttonRect)
                        .AlignHorisonallyByCenter(buttonRect)
                        .Translate(-7,1);
                    

                    platform.DrawStretchBox(buttonRect,tab.State == NavigationItemState.Current ? CachedStyles.TabBoxActiveStyle : CachedStyles.TabBoxStyle,10);

                    platform.DrawLabel(textRect,tab.Title,CachedStyles.TabTitleStyle);

                    var tab1 = tab;
                    platform.DoButton(buttonBoxRect,"",CachedStyles.ClearItemStyle, m =>
                    {
                        if (tab1.NavigationAction != null) tab1.NavigationAction(m);
                    }, m =>
                    {
                        if (tab1.NavigationActionSecondary != null) tab1.NavigationActionSecondary(m);
                    });

                    platform.DoButton(closeButton,"",CachedStyles.TabCloseButton, m =>
                    {
                        if (tab1.CloseAction != null) tab1.CloseAction(m);
                    });

                    x += buttonRect.width+2;
                }

                var newTabButtonRect =
                    new Rect().WithSize(27, 27).Align(tabsRect).AlignHorisonallyByCenter(tabsRect).Translate(x+2, 0);

                platform.SetTooltipForRect(newTabButtonRect,"Create or import new graphs");

                platform.DoButton(newTabButtonRect,"",ElementDesignerStyles.WizardActionButtonStyleSmall,()=>{ InvertApplication.SignalEvent<INewTabRequested>(_=>_.NewTabRequested());});
                
                platform.DrawImage(newTabButtonRect.PadSides(6),"PlusIcon_Micro",true);
            }
        }

        public void DrawBreadcrumbs(IPlatformDrawer platform,  float y)
        {

            var navPanelRect = new Rect(4, y, 60, 30f);
            var breadcrumbsRect = new Rect(64, y, Bounds.width-44, 30f);
            platform.DrawRect(Bounds.WithOrigin(0,y).WithHeight(30), InvertGraphEditor.Settings.BackgroundColor);

            var back = new Rect().WithSize(30, 30).PadSides(2).CenterInsideOf(navPanelRect.LeftHalf());
            platform.DoButton(back, "", ElementDesignerStyles.WizardActionButtonStyleSmall,
                () =>
                {
                    InvertApplication.Execute(new NavigateBackCommand());
                });
            platform.DrawImage(back.PadSides(4), "BackIcon", true);

            var forward = new Rect().WithSize(30, 30).PadSides(2).CenterInsideOf(navPanelRect.RightHalf());
            platform.DoButton(forward, "", ElementDesignerStyles.WizardActionButtonStyleSmall,
                () =>
                {
                    InvertApplication.Execute(new NavigateForwardCommand());
                });
            platform.DrawImage(forward.PadSides(4),"ForwardIcon",true);

            var x = 1f;

            var styles = DiagramViewModel.NavigationViewModel.BreadcrumbsStyle;
            var iconsTine = new Color(1, 1, 1, 0.5f);

            foreach (var usitem in DiagramViewModel.NavigationViewModel.Breadcrubs.ToArray())
            {
                var item = usitem;
                var textSize = platform.CalculateTextSize(usitem.Title, CachedStyles.BreadcrumbTitleStyle);
                float buttonContentPadding = 5;
                float buttonIconsPadding= 5;
                bool useSpecIcon = !string.IsNullOrEmpty(item.SpecializedIcon);
                var buttonWidth = textSize.x + buttonContentPadding*2 + 8;
                if (!string.IsNullOrEmpty(item.Icon)) buttonWidth += buttonIconsPadding + 16;
                if (useSpecIcon) buttonWidth += buttonIconsPadding + 16;
               
                var buttonRect = new Rect()
                    .AlignAndScale(breadcrumbsRect)
                    .WithWidth(buttonWidth)
                    .PadSides(3)
                    .Translate(x, 0);

                var icon1Rect = new Rect()
                    .WithSize(16, 16)
                    .AlignTopRight(buttonRect)
                    .AlignHorisonallyByCenter(buttonRect)
                    .Translate(-buttonContentPadding, 0);

                var icon2Rect = new Rect()
                    .WithSize(16, 16)
                    .Align(buttonRect)
                    .AlignHorisonallyByCenter(buttonRect)
                    .Translate(buttonContentPadding, 0);

                var textRect = new Rect()
                    .WithSize(textSize.x, textSize.y)
                    .Align(useSpecIcon ? icon2Rect : buttonRect)
                    .AlignHorisonallyByCenter(buttonRect)
                    .Translate(useSpecIcon ? buttonIconsPadding + 16 : buttonContentPadding, -1);

                var dotRect = new Rect()
                    .WithSize(16, 16)
                    .RightOf(buttonRect)
                    .AlignHorisonallyByCenter(buttonRect)
                    .Translate(-3,0);

                platform.DoButton(buttonRect, "", item.State == NavigationItemState.Current ? CachedStyles.BreadcrumbBoxActiveStyle : CachedStyles.BreadcrumbBoxStyle, item.NavigationAction);
                platform.DrawLabel(textRect, item.Title, CachedStyles.BreadcrumbTitleStyle, DrawingAlignment.MiddleCenter);
                platform.DrawImage(icon1Rect, styles.GetIcon(item.Icon,iconsTine), true);

                if (useSpecIcon) platform.DrawImage(icon2Rect, styles.GetIcon(item.SpecializedIcon, iconsTine), true);
                if (item.State != NavigationItemState.Current) platform.DrawImage(dotRect, styles.GetIcon("DotIcon", iconsTine), true);

                x += buttonRect.width + 16 - 6;

            }



        }

        public override void Draw(IPlatformDrawer platform, float scale)
        {
#if UNITY_EDITOR
            if (UpgradeOldProject()) return;
#endif
            //// Draw the title box
            //GUI.Box(new Rect(0, 0f, DiagramSize.width, 30f), DiagramViewModel.Title , ElementDesignerStyles.DiagramTitle);
            if (DiagramViewModel.LastMouseEvent != null)
            {
                var handler = DiagramViewModel.LastMouseEvent.CurrentHandler;
                if (!(handler is DiagramDrawer))
                    handler.Draw(platform, scale);
            }
            
            // Draw all of our drawers
            foreach (var drawer in CachedChildren)
            {
                if (drawer.Dirty)
                {
                    drawer.Refresh((IPlatformDrawer)platform,drawer.Bounds.position,false);
                    drawer.Dirty = false;
                }
                drawer.Draw(platform, Scale);

            }
          //  platform.DrawLabel(new Rect(5f, 5f, 200f, 100f), DiagramViewModel.Title, CachedStyles.GraphTitleLabel);


            DrawErrors();
            DrawHelp();
        }
        public override void OnMouseDoubleClick(MouseEvent mouseEvent)
        {
           
            DiagramViewModel.LastMouseEvent = mouseEvent;
            if (DrawersAtMouse == null)
            {

                DrawersAtMouse = GetDrawersAtPosition(this, mouseEvent.MousePosition).ToArray();
            }
            base.OnMouseDoubleClick(mouseEvent);
            if (DrawersAtMouse.Length < 1)
            {
                if (mouseEvent.ModifierKeyStates.Alt)
                {
                   // DiagramViewModel.ShowContainerDebug();
                }
                else
                {
                    DiagramViewModel.ShowQuickAdd();
                }

                return;
            }
            if (!BubbleEvent(d => d.OnMouseDoubleClick(mouseEvent), mouseEvent))
            {

                return;
            }
            else
            {
               
            }

            DiagramViewModel.Navigate();

            Refresh((IPlatformDrawer)InvertGraphEditor.PlatformDrawer);
            //Refresh((IPlatformDrawer)InvertGraphEditor.PlatformDrawer);
     

        }

        public override void OnMouseEnter(MouseEvent e)
        {
            base.OnMouseEnter(e);
            BubbleEvent(d => d.OnMouseEnter(e), e);
        }
        public override void OnMouseExit(MouseEvent e)
        {
            base.OnMouseExit(e);
            DiagramViewModel.LastMouseEvent = e;
            BubbleEvent(d => d.OnMouseExit(e), e);
        }

        public override void OnMouseDown(MouseEvent mouseEvent)
        {
            base.OnMouseDown(mouseEvent);
            DiagramViewModel.LastMouseEvent = mouseEvent;
            if (DrawersAtMouse == null) return;
            if (!DrawersAtMouse.Any())
            {
                DiagramViewModel.NothingSelected();
                if (mouseEvent.ModifierKeyStates.Ctrl)
                {
                    DiagramViewModel.ShowQuickAdd();
                }
                mouseEvent.Begin(SelectionRectHandler);
            }
            else
            {
                BubbleEvent(d => d.OnMouseDown(mouseEvent), mouseEvent);
            }
        }

        public bool BubbleEvent(Action<IDrawer> action, MouseEvent e)
        {
            if (DrawersAtMouse == null) return true;

            foreach (var item in DrawersAtMouse.OrderByDescending(p => p.ZOrder))
            {
               // if (!item.Enabled) continue;
                action(item);
                if (e.NoBubble)
                {
                    e.NoBubble = false;
                    return false;

                    break;
                }
            }
            return true;
        }

        public override void OnMouseMove(MouseEvent e)
        {
            base.OnMouseMove(e);
            DiagramViewModel.LastMouseEvent = e;
            if (e.IsMouseDown && e.MouseButton == 0 && !e.ModifierKeyStates.Any)
            {
                foreach (var item in Children.OfType<DiagramNodeDrawer>())
                {
                    if (item.ViewModelObject.IsSelected)
                    {
#if UNITY_EDITOR
                        if (DiagramViewModel.Settings.Snap)
                        {
                            item.ViewModel.Position += e.MousePositionDeltaSnapped;
                            item.ViewModel.Position = item.ViewModel.Position.Snap(DiagramViewModel.Settings.SnapSize);
                        }
                        else
                        {
#endif
                            item.ViewModel.Position += e.MousePositionDelta;
#if UNITY_EDITOR
                        }
#endif
                        if (item.ViewModel.Position.x < 0)
                        {
                            item.ViewModel.Position = new Vector2(0f, item.ViewModel.Position.y);
                        }
                        if (item.ViewModel.Position.y < 0)
                        {
                            item.ViewModel.Position = new Vector2(item.ViewModel.Position.x, 0f);
                        }
                        item.Dirty = true;
                        //item.Refresh((IPlatformDrawer)InvertGraphEditor.PlatformDrawer,item.Bounds.position,false);
                    }
                }
            }
            else
            {

                var nodes = GetDrawersAtPosition(this, e.MousePosition).ToArray();

                //NodeDrawerAtMouse = nodes.FirstOrDefault();

                if (DrawersAtMouse != null)
                    foreach (var item in nodes)
                    {
                        var alreadyInside = DrawersAtMouse.Contains(item);
                        if (!alreadyInside)
                        {
                            item.OnMouseEnter(e);
                        }
                    }
                if (DrawersAtMouse != null)
                    foreach (var item in DrawersAtMouse)
                    {
                        if (!nodes.Contains(item))
                        {
                            item.OnMouseExit(e);
                        }
                    }

                DrawersAtMouse = nodes;
                foreach (var node in DrawersAtMouse)
                {
                    node.OnMouseMove(e);
                }
            }
        }

        public IDrawer[] DrawersAtMouse { get; set; }

        public IEnumerable<IDrawer> GetDrawersAtPosition(IDrawer parent, Vector2 point)
        {
            foreach (var child in parent.Children)
            {
                if (child.Bounds.Contains(point))
                {
                    if (child.Children != null && child.Children.Count > 0)
                    {
                        var result = GetDrawersAtPosition(child, point);
                        foreach (var item in result)
                        {
                            yield return item;
                        }
                    }
                    yield return child;
                }
            }
        }
        public override void OnMouseUp(MouseEvent mouseEvent)
        {
            DiagramViewModel.LastMouseEvent = mouseEvent;
            BubbleEvent(d => d.OnMouseUp(mouseEvent), mouseEvent);

        }

        public override void OnRightClick(MouseEvent mouseEvent)
        {
            DiagramViewModel.LastMouseEvent = mouseEvent;
            BubbleEvent(d => d.OnRightClick(mouseEvent), mouseEvent);
            if (DrawersAtMouse == null)
            {
                ShowAddNewContextMenu(mouseEvent);
                return;

            }
            //var item = DrawersAtMouse.OrderByDescending(p=>p.ZOrder).FirstOrDefault();
            IDrawer item = DrawersAtMouse.OfType<ConnectorDrawer>().FirstOrDefault();
            if (item != null)
            {
                InvertApplication.SignalEvent<IShowContextMenu>(_=>_.Show(mouseEvent,item.ViewModelObject));
                return;
            }
            item = DrawersAtMouse.OfType<ItemDrawer>().FirstOrDefault();
            if (item != null)
            {
                if (item.Enabled)
                ShowItemContextMenu(mouseEvent);
                return;
            }
            item = DrawersAtMouse.OfType<DiagramNodeDrawer>().FirstOrDefault();
            if (item == null)
                item = DrawersAtMouse.OfType<HeaderDrawer>().FirstOrDefault();
            if (item != null)
            {
                if (!item.ViewModelObject.IsSelected)
                item.ViewModelObject.Select();
                ShowContextMenu(mouseEvent);
                return;
            }
            ShowAddNewContextMenu(mouseEvent);
        }

        public override void Refresh(IPlatformDrawer platform, Vector2 position, bool hardRefresh = true)
        {
            base.Refresh(platform, position, hardRefresh);
            // Eventually it will all be viewmodels
            if (DiagramViewModel == null) return;
            Dictionary<IGraphFilter, Vector2> dictionary = new Dictionary<IGraphFilter, Vector2>();
            
            var first = true;
            foreach (var filter in new [] {DiagramViewModel.GraphData.RootFilter}.Concat(this.DiagramViewModel.GraphData.GetFilterPath()).Reverse())
            {


                var name = first ? filter.Name : "< " + filter.Name;
                dictionary.Add(filter, platform.CalculateTextSize(name, first ? CachedStyles.GraphTitleLabel : CachedStyles.ItemTextEditingStyle));
                first = false;
            }
                
            mCachedPaths = dictionary;

            Children.Clear();
            DiagramViewModel.Load(hardRefresh);
            Children.Add(SelectionRectHandler);
            Dirty = true;
            //_cachedChildren = Children.OrderBy(p => p.ZOrder).ToArray();
        }

        public void Save()
        {
            DiagramViewModel.Save();
        }

        public void ShowAddNewContextMenu(MouseEvent mouseEvent)
        {
            InvertApplication.SignalEvent<IShowContextMenu>(_ => _.Show(mouseEvent, DiagramViewModel));
        }

        public void ShowContextMenu(MouseEvent mouseEvent)
        {
            InvertApplication.SignalEvent<IShowContextMenu>(_ => _.Show(mouseEvent, DiagramViewModel.SelectedNodes.Cast<object>().ToArray()));
        }

        public void ShowItemContextMenu(MouseEvent mouseEvent)
        {
            InvertApplication.SignalEvent<IShowContextMenu>(_ => _.Show(mouseEvent, DiagramViewModel.SelectedNodeItem));
        }

        protected override void DataContextChanged()
        {
            base.DataContextChanged();
            DiagramViewModel.GraphItems.CollectionChanged += GraphItemsOnCollectionChangedWith;
        }



        protected virtual void OnSelectionChanged(IDiagramNode olddata, IDiagramNode newdata)
        {
            SelectionChangedEventArgs handler = SelectionChanged;
            if (handler != null) handler(olddata, newdata);
        }

        private static void DrawHelp()
        {
            // TODO implement platform stuff
#if UNITY_EDITOR
            //if (InvertGraphEditor.Settings.ShowHelp)
            //{
            //    var rect = new Rect(Screen.width - 275f, 10f, 250f, InvertGraphEditor.KeyBindings.Length * 20f);
            //    GUI.Box(rect, string.Empty);

            //    GUILayout.BeginArea(rect);
            //    foreach (var keyBinding in InvertGraphEditor.KeyBindings.Select(p => p.Name + ": " + p.ToString()).Distinct())
            //    {
            //        EditorGUILayout.LabelField(keyBinding);
            //    }
            //    EditorGUILayout.LabelField("Open Code: Ctrl+Click");
            //    GUILayout.EndArea();

            //}
#endif
        }

        private void DrawErrors()
        {
#if UNITY_EDITOR
            if (DiagramViewModel.HasErrors)
            {
                GUI.Box(Rect, DiagramViewModel.Errors.Message + Environment.NewLine + DiagramViewModel.Errors.StackTrace);
            }
#endif
        }

        public SelectionRectHandler SelectionRectHandler
        {
            get { return mSelectionRectHandler ?? (mSelectionRectHandler = new SelectionRectHandler(DiagramViewModel)); }
            set { mSelectionRectHandler = value; }
        }

        public static bool IsEditingField { get; set; }

        public IEnumerable<IDrawer> CachedChildren
        {
            get
            {
                return mCachedChildren;
            }
        }

        private void GraphItemsOnCollectionChangedWith(object sender, MVVM.NotifyCollectionChangedEventArgs e)
        {
            GraphItemsOnCollectionChangedWith(e);
        }
        private void GraphItemsOnCollectionChangedWith(MVVM.NotifyCollectionChangedEventArgs changeArgs)
        {
            if (changeArgs.NewItems != null)
                foreach (var item in changeArgs.NewItems.OfType<ViewModel>())
                {
            
                    if (item == null) InvertApplication.Log("Graph Item is null");
                 
                    var drawer = InvertGraphEditor.Container.CreateDrawer<IDrawer>(item);
                    
                    if (drawer == null) InvertApplication.Log("Drawer is null");
       
                    Children.Add(drawer);

                    mCachedChildren = Children.OrderBy(p => p.ZOrder).ToArray();
                    drawer.Refresh((IPlatformDrawer)InvertGraphEditor.PlatformDrawer);
                
                }
            if (changeArgs.OldItems != null && changeArgs.OldItems.Count > 0)
            {
                var c = Children.Count;
                Children.RemoveAll(p => changeArgs.OldItems.Contains(p.ViewModelObject));
                var d = Children.Count;
                if (c != d)
                {
                    mCachedChildren = Children.OrderBy(p => p.ZOrder).ToArray();
                }
            }
        }
#if UNITY_EDITOR
        private bool UpgradeOldProject()
        {
            if (DiagramViewModel.NeedsUpgrade)
            {
                var rect = new Rect(50f, 50f, 200f, 75f);
                GUI.Box(rect, string.Empty);
                GUILayout.BeginArea(rect);
                GUILayout.Label("You need to upgrade to the new " + Environment.NewLine +
                                "file format for future compatability.");
                if (GUILayout.Button("Upgrade Now"))
                {
                    DiagramViewModel.UpgradeProject();
                    return true;
                }
                GUILayout.EndArea();
            }
            return false;
        }
#endif
    }
}