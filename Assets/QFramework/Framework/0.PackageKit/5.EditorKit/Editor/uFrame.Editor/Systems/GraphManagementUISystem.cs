using System;
using System.Collections.Generic;
using System.Linq;
using Invert.Common;
using QF.GraphDesigner;
using QF.GraphDesigner.Systems;
using QF.GraphDesigner.Systems.GraphUI;
using QF.GraphDesigner.Systems.GraphUI.api;
using Invert.Data;
using QF;
using UnityEngine;

namespace Assets.UnderConstruction.Editor
{
    public class GraphManagementUISystem : DiagramPlugin, IQueryDesignerWindowModalContent, IQueryGraphsActions, INewTabRequested
    {
        private WorkspaceService _workspaceService;
        private IPlatformDrawer _platformDrawer;
        private Vector2 _scrollPos;
        private bool _enableGraphManagementhWizard;

        public void ToggleGraphManagementWizard()
        {
            //RefreshViewModel
        }

        public WorkspaceService WorkspaceService
        {
            get { return _workspaceService ?? (_workspaceService = InvertApplication.Container.Resolve<WorkspaceService>()); }
            set { _workspaceService = value; }
        }

        public IPlatformDrawer PlatformDrawer
        {
            get { return _platformDrawer ?? (_platformDrawer = InvertApplication.Container.Resolve<IPlatformDrawer>()); }
            set { _platformDrawer = value; }
        }

        protected bool EnableGraphManagementhWizard
        {
            get
            {
                if (WorkspaceService.CurrentWorkspace != null && WorkspaceService.CurrentWorkspace.CurrentGraph == null)
                    return true;
                return _enableGraphManagementhWizard;
            }
            set { _enableGraphManagementhWizard = value; }
        }

        public void QueryDesignerWindowModalContent(List<DesignerWindowModalContent> content)
        {
            if (EnableGraphManagementhWizard)
            {
                content.Add(new DesignerWindowModalContent()
                {
                    ZIndex = 1,
                    Drawer = (rect) =>
                    {
                    //    try
                    //    {
                            DrawGraphManagementWizard(rect);
                        //}
                        //catch (Exception ex)
                        //{
                      //      EnableGraphManagementhWizard = false;
                    //        Debug.LogError(ex);
                        //}
                    }
                });
            }
        }
        [Inject]
        public IRepository Repository { get; set; }
        private void DrawGraphManagementWizard(Rect obj)
        {
            var listRect = obj.RightHalf().PadSides(2);
            var controlRect = obj.LeftHalf().BottomHalf().PadSides(2);
            var actionRect = obj.LeftHalf().TopHalf().PadSides(2);

            var actions = new List<ActionItem>();
            Signal<IQueryGraphsActions>(_=>_.QueryGraphsAction(actions));

            DrawGraphsList(listRect, Repository.AllOf<IGraphData>()
                .OrderBy(_ => WorkspaceService.CurrentWorkspace != null && !WorkspaceService.CurrentWorkspace.Graphs.Contains(_))
                .ThenBy(_=>_.Name)
                .ToList());
     
            Signal<IDrawActionsPanel>(_=>_.DrawActionsPanel(PlatformDrawer,actionRect,actions, (i, m) =>
            {
                SelectedAction = i;
            }));

            if (SelectedAction != null)
            {
                Signal<IDrawActionDialog>(_=>_.DrawActionDialog(PlatformDrawer,controlRect,SelectedAction, () =>
                {
                    SelectedAction = null;
                }));
            }

            var closeButtonBounds = new Rect().WithSize(80, 30).InnerAlignWithBottomRight(listRect.PadSides(15));
            PlatformDrawer.DoButton(closeButtonBounds, "Close", ElementDesignerStyles.ButtonStyle, () => EnableGraphManagementhWizard = false);
        }

        public ActionItem SelectedAction { get; set; }

        public void DrawGraphsList(Rect bounds, List<IGraphData> items)
        {

            PlatformDrawer.DrawStretchBox(bounds, CachedStyles.WizardSubBoxStyle, 13);
            
            var scrollBounds = bounds.Translate(15,0).Pad(0,0,15,0);
            
            bounds = bounds.PadSides(15);


            var headerRect = bounds.WithHeight(40);
             
            PlatformDrawer.DrawLabel(headerRect, string.Format("{0} Graphs", WorkspaceService.CurrentWorkspace.Title), CachedStyles.WizardSubBoxTitleStyle, DrawingAlignment.TopCenter);

            var unpaddedItemRect = bounds.Below(headerRect).WithHeight(50);

            var databasesListItems = items.ToArray();
         
            var position = scrollBounds.Below(headerRect).Clip(scrollBounds).Pad(0, 0, 0, 55);
            var usedRect = position.Pad(0, 0, 15, 0).WithHeight((unpaddedItemRect.height + 1)*databasesListItems.Length);
            
            _scrollPos = GUI.BeginScrollView(position, _scrollPos, usedRect);

            foreach (var db in databasesListItems)
            {

                var db1 = db;
                var isGraphInWorkspace = WorkspaceService.CurrentWorkspace != null && WorkspaceService.CurrentWorkspace.Graphs.Contains(db1);
                var cColor = GUI.color;
                var mColor = new Color(cColor.r, cColor.g, cColor.b, 0.3f);
                if (!isGraphInWorkspace) GUI.color = mColor;

                PlatformDrawer.DrawStretchBox(unpaddedItemRect,CachedStyles.WizardListItemBoxStyle,2);
               
                if(isGraphInWorkspace)
                PlatformDrawer.DoButton(unpaddedItemRect.TopHalf(),"",CachedStyles.ClearItemStyle, () =>
                {
                    Execute(new LambdaCommand("Open Graph", () =>
                    {
                        WorkspaceService.CurrentWorkspace.AddGraph(db1);
                        WorkspaceService.CurrentWorkspace.CurrentGraphId = db1.Identifier;
                        EnableGraphManagementhWizard = false;
                    }));
                });
                
                var itemRect = unpaddedItemRect.PadSides(15);
                var titleRect = itemRect.WithHeight(40);

                PlatformDrawer.DrawLabel(titleRect,db.Title,CachedStyles.WizardSubBoxTitleStyle,DrawingAlignment.TopLeft);

                var infoRect = itemRect.Below(titleRect).WithHeight(38);
                //(PlatformDrawer as UnityDrawer).DrawInfo(infoRect, string.Format("Namespace: {0}\nPath: {1}", db.GraphConfiguration.Namespace ?? "-", db.GraphConfiguration.FullPath));


                var openButton = new Rect().WithSize(80,25).InnerAlignWithBottomRight(itemRect).AlignHorisonallyByCenter(itemRect);
                var configButton = openButton.LeftOf(openButton).Translate(-2,0);
                var exportButton = openButton.LeftOf(configButton).Translate(-2, 0);
                var deleteButton = openButton.LeftOf(exportButton).Translate(-2, 0);


                GUI.color = cColor;
                PlatformDrawer.DoButton(openButton, isGraphInWorkspace ? "Open" : "Import", ElementDesignerStyles.DarkButtonStyle, () =>
                {
                    
                    /* OPEN DATABASE */
                    Execute(new LambdaCommand("Open Graph", () =>
                    {
                        WorkspaceService.CurrentWorkspace.AddGraph(db1);
                        WorkspaceService.CurrentWorkspace.CurrentGraphId = db1.Identifier;
                        EnableGraphManagementhWizard = false;
                    }));
                    //DatabaseListWindow.Init(new Vector2(Screen.currentResolution.width / 2 - 200, Screen.currentResolution.height/2- 300));

                });
                //PlatformDrawer.DoButton(configButton, "Config", ElementDesignerStyles.ButtonStyle, () => { /* CONFIG DATABASE */ });
                //PlatformDrawer.DoButton(deleteButton, "Delete", ElementDesignerStyles.ButtonStyle, () => { /* SHOW DATABASE IN EXPLORER */ });
                //PlatformDrawer.DoButton(exportButton, "Export", ElementDesignerStyles.ButtonStyle, () => { /* SHOW DATABASE IN EXPLORER */ });
               
                unpaddedItemRect = unpaddedItemRect.Below(unpaddedItemRect).Translate(0,1);

            }
            GUI.EndScrollView(true);
        }

        public void QueryGraphsAction(List<ActionItem> items)
        {

            var config = WorkspaceService.CurrentConfiguration;
            if (config == null) return;
            foreach (var item in config.GraphTypes)
            {
                items.Add(new ActionItem()
                {
                     Title = item.Title ?? item.GraphType.Name,
                    Command = new CreateGraphCommand()
                    {
                        GraphType = item.GraphType,
                        Name = "New" + item.GraphType.Name
                    },
                    Description = item.Description,
                    Verb = "Create"
                });
            }
            
        }

        public void NewTabRequested()
        {
            EnableGraphManagementhWizard = true;
        }
    }
}


