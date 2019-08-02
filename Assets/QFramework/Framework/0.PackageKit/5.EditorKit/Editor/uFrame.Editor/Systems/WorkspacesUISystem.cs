using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Invert.Common;
using QF.GraphDesigner.Systems.GraphUI.api;
using QF.GraphDesigner;
using QF.GraphDesigner.Systems.Wizards.api;
using UnityEngine;

namespace QF.GraphDesigner.Systems.GraphUI
{
    public class WorkspacesUISystem : DiagramPlugin,
        IQueryWorkspacesActions,
        IQueryWorkspacesListItems,
        IDrawWorkspacesList,
        IQueryDesignerWindowModalContent,
        IToolbarQuery,
        IContextMenuQuery
    {
        private WorkspaceService _workspaceService;

        public WorkspaceService WorkspaceService
        {
            get { return _workspaceService ?? (_workspaceService = InvertApplication.Container.Resolve<WorkspaceService>()); }
            set { _workspaceService = value; }
        }

        private DatabaseService _databaseService;
        public DatabaseService DatabaseService
        {
            get { return _databaseService ?? (_databaseService = InvertApplication.Container.Resolve<DatabaseService>()); }
            set { _databaseService = value; }
        }

        private IPlatformDrawer _drawer;
        private Vector2 _scrollPos;
        private bool _enableWizard;

        public IPlatformDrawer Drawer
        {
            get { return _drawer ?? (_drawer = InvertApplication.Container.Resolve<IPlatformDrawer>()); }
            set { _drawer = value; }
        }

        public void QueryWorkspacesActions(List<ActionItem> items)
        {
            if (WorkspaceService != null && WorkspaceService.Configurations != null)
            {
                foreach (var item in WorkspaceService.Configurations)
                {
                    items.Add(new ActionItem()
                    {
                        Title = string.Format("Create {0} Workspace", item.Value.Title),
                        Description = item.Value.Description,
                        Command = new CreateWorkspaceCommand()
                        {
                            Name = string.Format("New{0}Workspace", item.Value.Title),
                            WorkspaceType = item.Key
                        }
                    });


                }
            }
            
           
           
        }

        public void QueryWorkspacesListItems(List<WorkspacesListItem> items)
        {
            items.AddRange(WorkspaceService.Workspaces.Select(workspace => new WorkspacesListItem()
            {
                Workspace = workspace
            }));
        }

        public void DrawWorkspacesList(IPlatformDrawer platform, Rect bounds, List<WorkspacesListItem> items)
        {
            platform.DrawStretchBox(bounds, CachedStyles.WizardSubBoxStyle, 13);

            var scrollBounds = bounds.Translate(15, 0).Pad(0, 0, 15, 0);
            bounds = bounds.PadSides(15);


            var headerRect = bounds.WithHeight(40);

            platform.DrawLabel(headerRect, "Workspaces", CachedStyles.WizardSubBoxTitleStyle, DrawingAlignment.TopCenter);

            var unpaddedItemRect = bounds.Below(headerRect).WithHeight(100);

            var workspaces = items.ToArray();
         
            var position = scrollBounds.Below(headerRect).Clip(scrollBounds).Pad(0, 0, 0, 55);
            var usedRect = position.Pad(0, 0, 15, 0).WithHeight((unpaddedItemRect.height + 1) * workspaces.Length);
            _scrollPos = GUI.BeginScrollView(position, _scrollPos, usedRect);
            foreach (var db in workspaces)
            {
                var workspace = db;
                platform.DrawStretchBox(unpaddedItemRect, CachedStyles.WizardListItemBoxStyle, 2);
                var itemRect = unpaddedItemRect.PadSides(15);
                var titleRect = itemRect.WithHeight(40);

                platform.DrawLabel(titleRect, db.Workspace.Title, CachedStyles.WizardSubBoxTitleStyle, DrawingAlignment.TopLeft);

                var infoRect = itemRect.Below(titleRect).WithHeight(30);
                //(platform as UnityDrawer).DrawInfo(infoRect, string.Format("Namespace: {0}\nPath: {1}", db.GraphConfiguration.Namespace ?? "-", db.GraphConfiguration.FullPath));


                var openButton = new Rect().WithSize(80, 25).InnerAlignWithBottomRight(itemRect);
                var configButton = openButton.LeftOf(openButton).Translate(-2, 0);
                var deleteButton = configButton.LeftOf(configButton).Translate(-2, 0);

                platform.DoButton(openButton, "Open", ElementDesignerStyles.DarkButtonStyle, () =>
                {
                    Execute(new OpenWorkspaceCommand() { Workspace = workspace.Workspace });
                    EnableWizard = false;
                });
                var db1 = db;
                platform.DoButton(configButton, "Config", ElementDesignerStyles.DarkButtonStyle, () => InvokeConfigFor(db1));
                platform.DoButton(deleteButton, "Remove", ElementDesignerStyles.DarkButtonStyle, () => { Execute(new RemoveWorkspaceCommand() { Workspace = workspace.Workspace }); });
                //platform.DoButton(showButton, "Show In Explorer", ElementDesignerStyles.ButtonStyle, () => { });


                unpaddedItemRect = unpaddedItemRect.Below(unpaddedItemRect).Translate(0, 1);

            }

            GUI.EndScrollView(true);

        }

        private void InvokeConfigFor(WorkspacesListItem db)
        {
            SelectedAction = new ActionItem()
            {
                Command = new ConfigureWorkspaceCommand()
                {
                    Name = db.Workspace.Title,
                    Workspace = db.Workspace
                },
                
                Description = "Configuration",
                Title = db.Workspace.Title,
                Verb = "Apply"
            };
        }

        public void DrawWorkspacesWizard( Rect bounds)
        {
            var actions = new List<ActionItem>();
            var items = new List<WorkspacesListItem>();
            var databasesActionsBounds = bounds.LeftHalf().TopHalf().PadSides(2);
            var databasesListBounds = bounds.RightHalf().PadSides(2);
            var databasesActionInspectorBounds = bounds.LeftHalf().BottomHalf().PadSides(2);
            var closeButtonBounds = new Rect().WithSize(80, 30).InnerAlignWithBottomRight(databasesListBounds.PadSides(15));

            Signal<IQueryWorkspacesActions>(_ => _.QueryWorkspacesActions(actions));
            Signal<IQueryWorkspacesListItems>(_ => _.QueryWorkspacesListItems(items));
            Signal<IDrawActionsPanel>(_ => _.DrawActionsPanel(Drawer, databasesActionsBounds, actions, (a, m) => SelectedAction = a));
            Signal<IDrawActionDialog>(_ => _.DrawActionDialog(Drawer, databasesActionInspectorBounds, SelectedAction, () => SelectedAction = null));
            Signal<IDrawWorkspacesList>(_ => _.DrawWorkspacesList(Drawer, databasesListBounds, items));

            Drawer.DoButton(closeButtonBounds, "Close", ElementDesignerStyles.DarkButtonStyle, () =>
            {
                if (WorkspaceService.CurrentWorkspace == null)
                {
                    Signal<INotify>(_ => _.Notify("You need to select or create a Workspace!", NotificationIcon.Info));
                }
                else
                {
                    EnableWizard = false;
                }
            });

        }

        public ActionItem SelectedAction { get; set; }

        public bool EnableWizard
        {
            get
            {
                if (DatabaseService.CurrentConfiguration != null && WorkspaceService.CurrentWorkspace == null)
                {
                    return true;
                }
                return _enableWizard;
            }
            set { _enableWizard = value; }
        }

        public void QueryDesignerWindowModalContent(List<DesignerWindowModalContent> content)
        {
            if (EnableWizard)
                content.Add(new DesignerWindowModalContent()
                {
                    Drawer = DrawWorkspacesWizard,
                    ZIndex = 2
                });
        }

        public override decimal LoadPriority
        {
            get { return 1000; }
        }

        public void QueryToolbarCommands(ToolbarUI ui)
        {

            if (WorkspaceService.Workspaces.Count() < 15)
            {
                ui.AddCommand(new ToolbarItem()
                { 
                    Command = new LambdaCommand("Workspaces", () => EnableWizard = true),
                    Title = "Workspace:",
                    Description = "Click to manage your workspaces."
                });   
          
                foreach (var item in WorkspaceService.Workspaces)
                {
                    ui.AddCommand(new ToolbarItem()
                    {
                        Title = item.Name,
                        Checked = item == WorkspaceService.CurrentWorkspace,
                        Command = new OpenWorkspaceCommand()
                        {
                            Workspace = item
                        }
                    });
                }
               
            }
            else
            {
                ui.AddCommand(new ToolbarItem()
                {
                    Title = WorkspaceService.CurrentWorkspace == null ? "--Choose Workspace--" : WorkspaceService.CurrentWorkspace.Name,
                    IsDropdown = true,
                    Command = new SelectWorkspaceCommand(),
                    Position = ToolbarPosition.Left
                });
            }
        }

        public void QueryContextMenu(ContextMenuUI ui, MouseEvent evt, params object[] obj)
        {
            if (WorkspaceService.Workspaces.Count() >= 8)
            {
                var selectProject = obj.FirstOrDefault() as SelectWorkspaceCommand;
                if (selectProject != null)
                {
                    foreach (var item in WorkspaceService.Workspaces)
                    {
                        ui.AddCommand(new ContextMenuItem()
                        {
                            Title = item.Name,
                            Group = "Workspaces",
                            Checked = item == WorkspaceService.CurrentWorkspace,
                            Command = new OpenWorkspaceCommand()
                            {
                                Workspace = item
                            }
                        });
                    }
                    //if (WorkspaceService.Configurations != null)
                    //{
                    //    ui.AddSeparator();
                    //    foreach (var item in WorkspaceService.Configurations)
                    //    {
                    //        var title = item.Value.Title ?? item.Key.Name;
                    //        ui.AddCommand(new ContextMenuItem()
                    //        {
                    //            Title = string.Format("Create New {0} Workspace", title),
                    //            Command = new CreateWorkspaceCommand()
                    //            {
                    //                Name = string.Format("New {0} Workspace", title),
                    //                Title = string.Format("New {0} Workspace", title),
                    //                WorkspaceType = item.Key,
                    //            }
                    //        });
                    //    }
                    //}
         
                    ui.AddCommand(new ContextMenuItem()
                    {
                        Command = new LambdaCommand("Manage Workspaces", () => EnableWizard = true),
                        Group = "Manage",
                        Title = "Manage"
                    });



                }
            }
           
        }
    }


   
}
