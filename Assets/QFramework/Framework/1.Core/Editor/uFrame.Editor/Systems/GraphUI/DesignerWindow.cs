using System;
using System.Collections.Generic;
using System.Linq;
using Invert.Data;
using QF.GraphDesigner.Systems.GraphUI;
using UnityEngine;

namespace QF.GraphDesigner
{
    /// <summary>
    /// 这个应该是主要的设计窗口
    /// </summary>
    public class DesignerWindow : DiagramPlugin,
        IGraphWindow,
        IDrawUFrameWindow,
        ICommandExecuted,
        INodeItemEvents,
        IDataRecordInserted,
        IDataRecordRemoved,
        IDataRecordPropertyChanged,
        IDataRecordManagerRefresh,
        IToolbarQuery
    {
        private DesignerViewModel mDesignerViewModel;

        private bool mDrawToolbar = true;
        
        private WorkspaceService mWorkspaceService;

        public Toolbars Toolbars
        {
            get { return Container.Resolve<Toolbars>(); }
        }

        public IPlatformDrawer Drawer
        {
            get { return InvertGraphEditor.PlatformDrawer; }
        }
        
        public DesignerViewModel Designer
        {
            get
            {
                if (Workspace == null)
                    return null;
                if (mDesignerViewModel == null)
                {
                    mDesignerViewModel = new DesignerViewModel()
                    {
                        Data = Workspace
                    };
                }
                return mDesignerViewModel;
            }
            set { mDesignerViewModel = value; }
        }

        public DiagramDrawer DiagramDrawer { get; set; }

        public Rect DiagramRect { get; set; }

        public DiagramViewModel DiagramViewModel
        {
            get
            {
                if (DiagramDrawer == null) return null;
                return DiagramDrawer.DiagramViewModel;
            }
        }

        public bool DrawToolbar
        {
            get { return mDrawToolbar; }
            set { mDrawToolbar = value; }
        }



        public float Scale
        {
            get { return 1f; }
            set
            {
            }
        }

        public ToolbarUI Toolbar
        {
            get { return Toolbars.ToolbarUI; }
        }

        public Workspace Workspace
        {
            get { return WorkspaceService.CurrentWorkspace; }
            //set { InvertApplication.Execute();<IOpenWorkspace>(_=>_.OpenWorkspace(value)); }
        }

        public WorkspaceService WorkspaceService
        {
            get { return mWorkspaceService ?? (mWorkspaceService = InvertGraphEditor.Container.Resolve<WorkspaceService>()); }
            set { mWorkspaceService = value; }
        }

        public override void Loaded(QFrameworkContainer container)
        {
            base.Loaded(container);
            InvertGraphEditor.DesignerWindow = this;
        }

        private bool mShouldProcessInputFromDiagram = true;

        private DateTime mLastFpsUpdate;
        private int mFramesLastSec;
        private int mFpsShown;
        public void Draw(float width, float height, Vector2 scrollPosition, float scale)
        {
            DiagramDrawer.IsEditingField = false;
            if (Drawer == null)
            {
                InvertApplication.Log("DRAWER IS NULL");
                return;
            }
            var diagramRect = new Rect();
            if (DrawToolbar)
            {
                var toolbarTopRect = new Rect(0, 0, width, 18);
                var tabsRect = new Rect(0, toolbarTopRect.height, width, 31);
                var breadCrumbsRect = new Rect(0, tabsRect.y + tabsRect.height, width, 30);

                diagramRect = new Rect(0f, breadCrumbsRect.y + breadCrumbsRect.height, width,
                    height - (toolbarTopRect.height * 2) - breadCrumbsRect.height - 31);
                var toolbarBottomRect = new Rect(0f, diagramRect.y + diagramRect.height, width,
                    toolbarTopRect.height);


                var modalItems = new List<DesignerWindowModalContent>();
                Signal<IQueryDesignerWindowModalContent>(_ => _.QueryDesignerWindowModalContent(modalItems));

                var overlayItems = new List<DesignerWindowOverlayContent>();
                Signal<IQueryDesignerWindowOverlayContent>(_ => _.QueryDesignerWindowOverlayContent(overlayItems));

                //Disable diagram input if any modal content presents or if mouse is over overlay content
                mShouldProcessInputFromDiagram = !modalItems.Any() && overlayItems.All(i => !i.Drawer.CalculateBounds(diagramRect).Contains(Event.current.mousePosition));

                Drawer.DrawStretchBox(toolbarTopRect, CachedStyles.Toolbar, 0f);

                Drawer.DoToolbar(toolbarTopRect, this, ToolbarPosition.Left);
                //drawer.DoToolbar(toolbarTopRect, this, ToolbarPosition.Right);

                Drawer.DrawRect(tabsRect, InvertGraphEditor.Settings.GridLinesColor);

                //Drawer.DoTabs(Drawer,tabsRect, this); 
                DiagramRect = diagramRect;

                /* 
                 * DRAW DIAGRAM 
                 * Using GUI.color hack to avoid transparent diagram on disabled input (Thanks Unity :( )
                 */

                if (!mShouldProcessInputFromDiagram) Drawer.DisableInput();

                if (DiagramDrawer != null)
                {
                    DiagramDrawer.DrawTabs(Drawer, tabsRect);
                    DiagramDrawer.DrawBreadcrumbs(Drawer, breadCrumbsRect.y);
                }

                var fpsCounterRect = tabsRect.WithWidth(80).RightOf(tabsRect).Translate(-100,0);

                if (ShowFPS)
                {
                    if ((DateTime.Now - mLastFpsUpdate).TotalMilliseconds > 1000)
                    {
                        mFpsShown = mFramesLastSec;
                        mLastFpsUpdate = DateTime.Now;
                        mFramesLastSec = 0;
                    }
                    else
                    {
                        mFramesLastSec++;
                    }
                    Drawer.DrawLabel(fpsCounterRect, string.Format("FPS: {0}", mFpsShown), CachedStyles.WizardSubBoxTitleStyle);

                }

                Drawer.DrawRect(diagramRect, InvertGraphEditor.Settings.BackgroundColor);

                DiagramRect = diagramRect;

                DrawDiagram(Drawer, scrollPosition, scale, diagramRect);

                Drawer.EnableInput();

                /*
                 * DRAW OVERLAY CONTENT
                 */

                if (modalItems.Any()) Drawer.DisableInput();

                foreach (var item in overlayItems)
                {
                    var bounds = item.Drawer.CalculateBounds(diagramRect);
                    var isMouseOver = bounds.Contains(Event.current.mousePosition);

                    var colorCache = GUI.color;

                    if (!isMouseOver && !item.DisableTransparency) GUI.color = new Color(colorCache.r, colorCache.g, colorCache.b, colorCache.a / 4);

                    item.Drawer.Draw(bounds);

                    GUI.color = colorCache;
                }

                Drawer.EnableInput();

                /*
                 * DRAW MODAL CONTENT
                 */

                if (modalItems.Any())
                {
                    var modalBackgroundRect = new Rect().Cover(breadCrumbsRect, tabsRect, diagramRect);
                    var modalContentRect = new Rect().WithSize(800, 500).CenterInsideOf(modalBackgroundRect);
                    var activeModal = modalItems.OrderBy(i => i.ZIndex).Last();

                    Drawer.DisableInput();

                    foreach (var source in modalItems.OrderBy(i => i.ZIndex).Except(new[] { activeModal }))
                    {
                        source.Drawer(modalContentRect);
                    }

                    Drawer.EnableInput();

                    Drawer.DrawRect(modalBackgroundRect, new Color(0, 0, 0, 0.8f));

                    activeModal.Drawer(modalContentRect);
                }


                DrawToolip(breadCrumbsRect);

                Drawer.DoToolbar(toolbarBottomRect, this, ToolbarPosition.BottomLeft);
                //drawer.DoToolbar(toolbarBottomRect, this, ToolbarPosition.BottomRight);
            }
            else
            {
                diagramRect = new Rect(0f, 0f, width, height);
                DiagramRect = diagramRect;
                DrawDiagram(Drawer, scrollPosition, scale, diagramRect);
            }

            InvertApplication.SignalEvent<IDesignerWindowEvents>(_ => _.AfterDrawDesignerWindow(new Rect(0, 0, width, height)));
            InvertApplication.SignalEvent<IDesignerWindowEvents>(_ => _.DrawComplete());
        }

        private void DrawToolip(Rect alignmentRect)
        {
            var tooltip = Drawer.GetTooltip();
            if (!string.IsNullOrEmpty(tooltip))
            {
                var tooltipHeight = Drawer.CalculateTextHeight(tooltip, CachedStyles.ListItemTitleStyle, 350);

                var infoRect =
                    new Rect().WithSize(350, Math.Max(80, tooltipHeight + 60)).AlignTopRight(alignmentRect).Below(alignmentRect).Pad(0, 15, 15, 0).Translate(-20,0);

                var imageRect =
                    new Rect().WithSize(37, 37).AlignTopRight(infoRect).AlignHorisonallyByCenter(infoRect).Translate(-10, 0);

                Drawer.DrawStretchBox(infoRect, CachedStyles.TooltipBoxStyle, 13);
                Drawer.DrawLabel(infoRect.Pad(15, 15, 15 + 41 + 15, 30), tooltip, CachedStyles.ListItemTitleStyle);
                Drawer.DrawImage(imageRect, "InfoIcon", true);

            }
            Drawer.ClearTooltip();
        }

        public void LoadDiagram(IGraphData diagram)
        {
            InvertGraphEditor.DesignerWindow = this;
            if (diagram == null) return;
            try
            {

                DiagramDrawer = new DiagramDrawer(new DiagramViewModel(diagram));
                DiagramDrawer.Dirty = true;
                //DiagramDrawer.Data.ApplyFilter();
                DiagramDrawer.Refresh(InvertGraphEditor.PlatformDrawer);

            }
            catch (Exception ex)
            {
                InvertApplication.LogException(ex);
                InvertApplication.Log("Either a plugin isn't installed or the file could no longer be found. See Exception error");
            }
        }

        public void ProjectChanged(Workspace project)
        {
            mDesignerViewModel = null;

            mWorkspaceService = null;

            DiagramDrawer = null;

            if (project.CurrentGraph != null)
            {
                LoadDiagram(project.CurrentGraph);
            }
        }

        public void RefreshContent()
        {

            if (Workspace == null) return;
            if (Workspace.CurrentGraph == null) return;

            if (Workspace.CurrentGraph != DiagramDrawer.DiagramViewModel.DataObject)
                LoadDiagram(Workspace.CurrentGraph);
            else
            {
                if (DiagramViewModel != null)
                {
                    //TODO Micah, please check if it is valid to refresh it here
                    //Doing this on Load does not handle shit like renaming
                    DiagramViewModel.NavigationViewModel.Refresh();
                }

                if (DiagramDrawer != null)
                {
                    DiagramDrawer.Refresh(InvertGraphEditor.PlatformDrawer);
                }
            }


        }

        public void SwitchDiagram(IGraphData data)
        {
            if (data != null)
            {
                Designer.OpenTab(data);
                LoadDiagram(Workspace.CurrentGraph);
            }
        }


        private CachedLineItem[] mCachedLines;
        private Vector2 mCachedScroll;
        private Vector2 mCachedScreen;
        private List<IDataRecord> mChangedRecords;

        private bool DrawDiagram(IPlatformDrawer drawer, Vector2 scrollPosition, float scale, Rect diagramRect)
        {

            var screen = new Vector2(Screen.width, Screen.height);


            if (DiagramDrawer == null)
            {
                if (Workspace != null)
                {
                    if (Workspace.CurrentGraph != null)
                    {
                        LoadDiagram(Workspace.CurrentGraph);
                    }
                }
            }

            if (DiagramDrawer != null && DiagramViewModel != null && InvertGraphEditor.Settings.UseGrid)
            {
                if (mCachedLines == null || mCachedScroll != scrollPosition || mCachedScreen != screen)
                {

                    var lines = new List<CachedLineItem>();

                    var softColor = InvertGraphEditor.Settings.GridLinesColor;
                    var hardColor = InvertGraphEditor.Settings.GridLinesColorSecondary;
                    var x = -scrollPosition.x;

                    var every10 = 0;

                    while (x < DiagramRect.x + DiagramRect.width + scrollPosition.x)
                    {
                        var color = softColor;
                        if (every10 == 10)
                        {
                            color = hardColor;
                            every10 = 0;
                        }
                        if (x > diagramRect.x)
                        {
                            lines.Add(new CachedLineItem()
                            {
                                Lines = new[] { new Vector3(x, diagramRect.y), new Vector3(x, diagramRect.x + diagramRect.height + scrollPosition.y + 85) },
                                Color = color
                            });
                        }

                        x += DiagramViewModel.Settings.SnapSize * scale;
                        every10++;
                    }
                    var y = -scrollPosition.y + 80;
                    every10 = 10;
                    while (y < DiagramRect.y + DiagramRect.height + scrollPosition.y)
                    {
                        var color = softColor;
                        if (every10 == 10)
                        {
                            color = hardColor;
                            every10 = 0;
                        }
                        if (y > diagramRect.y)
                        {

                            lines.Add(new CachedLineItem()
                            {
                                Lines = new[] { new Vector3(diagramRect.x, y), new Vector3(diagramRect.x + diagramRect.width + scrollPosition.x, y) },
                                Color = color
                            });
                        }

                        y += DiagramViewModel.Settings.SnapSize * scale;
                        every10++;
                    }
                    mCachedLines = lines.ToArray();
                    mCachedScreen = screen;
                    mCachedScroll = scrollPosition;
                }

                for (int i = 0; i < mCachedLines.Length; i++)
                {
                    Drawer.DrawLine(mCachedLines[i].Lines, mCachedLines[i].Color);
                }

            }
            if (DiagramDrawer != null)
            {
                InvertApplication.SignalEvent<IDesignerWindowEvents>(_ => _.BeforeDrawGraph(diagramRect));
                DiagramDrawer.Bounds = new Rect(0f, 0f, diagramRect.width, diagramRect.height);

                DiagramDrawer.Draw(drawer, 1f);

                if (mShouldProcessInputFromDiagram) InvertApplication.SignalEvent<IDesignerWindowEvents>(_ => _.ProcessInput());
                InvertApplication.SignalEvent<IDesignerWindowEvents>(_ => _.AfterDrawGraph(DiagramDrawer.Bounds));
            }
            return false;
        }

        public void CommandExecuted(ICommand command)
        {
            if (command is UndoCommand || command is RedoCommand)
            {
                refresh = true;
            }
            if (refresh)
            {
                refresh = false;
                RefreshContent(); 
            } 
        }

        public void Renamed(IDiagramNodeItem nodeItem, string editText, string name)
        {
            RefreshContent();
        }

        public List<IDataRecord> ChangedRecords
        {
            get { return mChangedRecords ?? (mChangedRecords = new List<IDataRecord>()); }
            set { mChangedRecords = value; }
        }

        public void RecordInserted(IDataRecord record)
        {
            if (DiagramDrawer == null || DiagramDrawer.DiagramViewModel == null || DiagramDrawer.DiagramViewModel.IsLoading) return;
            if (record is FilterStackItem || record is WorkspaceGraph || record is FilterItem)
            {
                refresh = true;
            }
            DiagramViewModel.RecordInserted(record);
        }

        public bool refresh = false;
        public void RecordRemoved(IDataRecord record)
        {
            if (DiagramDrawer == null || DiagramDrawer.DiagramViewModel == null || DiagramDrawer.DiagramViewModel.IsLoading) return;

            if (record is FilterStackItem || record is WorkspaceGraph)
            {
                refresh = true;
            }

            DiagramViewModel.RecordRemoved(record);
        }

        public void PropertyChanged(IDataRecord record, string name, object previousValue, object nextValue)
        {
            
            if (DiagramDrawer ==null || DiagramDrawer.DiagramViewModel == null || DiagramDrawer.DiagramViewModel.IsLoading) return;
            if (name == "IsDirty")
            {
                DiagramViewModel.NavigationViewModel.Refresh();
                return;
            }
            DiagramViewModel.PropertyChanged(record, name, previousValue, nextValue);
            if (record is Workspace || record is InvertGraph || record is FilterStackItem)
            {
                refresh = true;
                return;
            }
            
            //RefreshByData(record);
        }

        private void RefreshByData(IDataRecord record)
        {
            List<IDrawer> drawers = new List<IDrawer>();
            LoopDrawers((drawer, vm, data) =>
            {
                if (record.IsNear(data))
                {
                    drawer.Children.Clear();
                    vm.DataObjectChanged();
                    drawers.Add(drawer);
                }
            });
            RefreshDrawerList(drawers);
        }

        private void RefreshDrawerList(List<IDrawer> drawers)
        {
            if (drawers.Count < 1) return;
            foreach (var drawer in drawers)
            {
                drawer.Refresh(InvertGraphEditor.PlatformDrawer, drawer.Bounds.position, true);
            }
            foreach (var drawer in drawers)
            {
                drawer.OnLayout();
            }
           RefreshConnections();
        }

        private void RefreshConnections()
        {
            DiagramDrawer.Children.RemoveAll(p => p is ConnectorDrawer);
            DiagramDrawer.Children.RemoveAll(p => p is ConnectionDrawer);
            DiagramDrawer.DiagramViewModel.RefreshConnectors();
        }

        private void LoopDrawers(Action<IDrawer, GraphItemViewModel, IDataRecord> action)
        {
            if (DiagramDrawer == null || DiagramDrawer.Children == null) return;
            LoopDrawers(DiagramDrawer.Children, action);
        }

        private void LoopDrawers(List<IDrawer> drawers, Action<IDrawer, GraphItemViewModel, IDataRecord> action)
        {
            for (int index = 0; index < drawers.Count; index++)
            {
                var item = drawers[index];
                if (item.ViewModelObject == null) continue;
                var dataObject = item.ViewModelObject.DataObject as IDataRecord;
                var vm = item.ViewModelObject;
                if (dataObject != null && vm != null && item != null)
                    action(item, vm, dataObject);
                LoopDrawers(item.Children, action);
            }
        }

        public void ManagerRefreshed(IDataRecordManager manager)
        {
            RefreshContent();
        }

        public bool ShowFPS
        {
            get { return InvertGraphEditor.Prefs.GetBool("ShowFPSInDiagramDesigner", false); }
            set { InvertGraphEditor.Prefs.SetBool("ShowFPSInDiagramDesigner",value); }
        }

        public void QueryToolbarCommands(ToolbarUI ui)
        {
            ui.AddCommand(new ToolbarItem()
            {
                Title = "Toggle FPS",
                Checked = ShowFPS,
                Command = new LambdaCommand("Toggle Fps", () =>
                {
                    ShowFPS = !ShowFPS;
                }),
                Position = ToolbarPosition.BottomRight
            });
        }
    }
}