using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using QF.GraphDesigner;

namespace QF.GraphDesigner
{
    public class GraphDesignerNavigationViewModel
    {
        private List<NavigationItem> _tabs;
        private List<NavigationItem> _breadcrubs;
        private DesignerWindow _designerWindow;
        private WorkspaceService _workspaceService;


        public IBreadcrumbsStyleSchema BreadcrumbsStyle
        {
            get { return CachedStyles.DefaultBreadcrumbsStyleSchema; }
            set { }
        }

        public WorkspaceService WorkspaceService
        {
            get { return _workspaceService ?? (_workspaceService = InvertGraphEditor.Container.Resolve<WorkspaceService>()); }
            set { _workspaceService = value; }
        }

        public List<NavigationItem> Tabs
        {
            get { return _tabs ?? (_tabs = new List<NavigationItem>()); }
            set { _tabs = value; }
        }

        public List<NavigationItem> Breadcrubs
        {
            get { return _breadcrubs ?? (_breadcrubs = new List<NavigationItem>()); }
            set { _breadcrubs = value; }
        }


        public DesignerWindow DesignerWindow
        {
            get
            {
                return _designerWindow ?? (_designerWindow = InvertApplication.Container.Resolve<DesignerWindow>());
            }
            set { _designerWindow = value; }
        }

        public void Refresh()
        {
            Tabs.Clear();

            foreach (var tab in DesignerWindow.Designer.Tabs)
            {
                var tab1 = tab;
                var navigationItem = new NavigationItem()
                {
                    Icon = "CommandIcon",
                    SpecializedIcon = null,

                    //State = DesignerWindow.Designer.CurrentTab.Graph == tab ? NavigationItemState.Current : NavigationItemState.Regular,
                    Title = tab.Title + (tab.IsDirty ? "*" : string.Empty),
                    NavigationAction = x =>
                    {
                        InvertApplication.Execute(new LambdaCommand("Change Current Graph", () =>
                        {
                            WorkspaceService.CurrentWorkspace.CurrentGraphId = tab1.Identifier;
                            //DesignerWindow.SwitchDiagram(WorkspaceService.CurrentWorkspace.Graphs.FirstOrDefault(p => p.Identifier == tab.Identifier));
                        }));
                        

                    },
                    CloseAction = x =>
                    {
                        InvertApplication.Execute(new LambdaCommand("Close Graph", () =>
                        {
                            this.DiagramViewModel.CurrentRepository.RemoveAll<WorkspaceGraph>(p => p.GraphId == tab1.Identifier);
                        } ));
                    }
                };

                if (DesignerWindow.Workspace != null && DesignerWindow.Workspace.CurrentGraph != null &&
                    tab.Identifier == DesignerWindow.Workspace.CurrentGraph.Identifier)
                {
                    navigationItem.State = NavigationItemState.Current;
                }
                else
                {
                    navigationItem.State = NavigationItemState.Regular;
                }

                Tabs.Add(navigationItem);

            }

            Breadcrubs.Clear();

            foreach (var filter in new[] { DiagramViewModel.GraphData.RootFilter }.Concat(this.DiagramViewModel.GraphData.GetFilterPath()))
            {
                var filter1 = filter;
                var navigationItem = new NavigationItem()
                {
                    Icon = "CommandIcon",
                    Title = filter.Name,
                    State = DiagramViewModel.GraphData != null && DiagramViewModel.GraphData.CurrentFilter == filter ? NavigationItemState.Current : NavigationItemState.Regular,
                    NavigationAction = x =>
                    {
                        InvertApplication.Execute(new LambdaCommand("Back", () => { DiagramViewModel.GraphData.PopToFilter(filter1); }));
                    }       
                };

                if (filter == DiagramViewModel.GraphData.RootFilter) navigationItem.SpecializedIcon = "RootFilterIcon";

                Breadcrubs.Add(navigationItem);
            }
        }

        public DiagramViewModel DiagramViewModel { get; set; }

    }
}