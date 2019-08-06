using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using QF.GraphDesigner;
using Invert.Data;
using QF.GraphDesigner.Unity;
using QF;
using UnityEditor;

using UnityEngine;

namespace QF.GraphDesigner {
    public class NavigationSystem : DiagramPlugin
        , IExecuteCommand<NavigateToNodeCommand>
        , IExecuteCommand<FilterBySelectionCommand>
        , IExecuteCommand<NavigateByNameCommand>
        , IExecuteCommand<NavigateByIdCommand>
        , IExecuteCommand<NavigateForwardCommand>
        , IExecuteCommand<NavigateBackCommand>
        , IExecuteCommand<NavigateCommand>
        , IExecuteCommand<NavigateByHistoryItemCommand>
        , IExecuteCommand<SaveNavigationHistoryStepCommand>
        , IWorkspaceChanged
        , IDataRecordPropertyChanged 
        , IMouseDown
        , IKeyUp
    {
        private IRepository _repository;
        private GraphSystem _graphSystem;
        private WorkspaceService _workspaceService;

        public IRepository Repository
        {
            get { return _repository ?? (_repository = InvertApplication.Container.Resolve<IRepository>()); }
            set { _repository = value; }
        }

        public GraphSystem GraphSystem
        {
            get { return _graphSystem ?? (_graphSystem = InvertApplication.Container.Resolve<GraphSystem>()); }
            set { _graphSystem = value; }
        }

        public WorkspaceService WorkspaceService
        {
            get { return _workspaceService ?? (_workspaceService = InvertApplication.Container.Resolve<WorkspaceService>()); }
            set { _workspaceService = value; }
        }

        public void Execute(NavigateToNodeCommand nodeCommand)
        {

            var graph = nodeCommand.Node.Graph;
            var workspace = WorkspaceService.Workspaces.FirstOrDefault(p => p.Graphs.Any(x => x.Identifier == graph.Identifier));

            if (workspace == null)
            {
                Signal<INotify>(_=>_.Notify(string.Format("Workspace not found for {0} graph",graph.Name),NotificationIcon.Error));
                return;
            }

            if (WorkspaceService.CurrentWorkspace.Identifier != workspace.Identifier)
            {
                WorkspaceService.Execute(new OpenWorkspaceCommand()
                {
                    Workspace = workspace
                });    
            }



            if (WorkspaceService.CurrentWorkspace.CurrentGraphId != graph.Identifier)
            {
                WorkspaceService.CurrentWorkspace.CurrentGraphId = graph.Identifier;
            }
          
            
            var filterPath = nodeCommand.Node.FilterPath().ToArray();
            graph.PopToFilter(graph.RootFilter);
            foreach (var item in filterPath)
            {
                if (item == graph.RootFilter) continue;
                graph.PushFilter(item);
            }


            if (nodeCommand.Select)
                nodeCommand.Node.IsSelected = true;


            var graphNode = nodeCommand.Node as GraphNode;

            if (graphNode != null && filterPath.Any())
            {
                var filter = graphNode.FilterLocations.FirstOrDefault(p => p.FilterId == filterPath.Last().Identifier);
                if (filter != null)
                {

                    var position = filter.Position;

                    Execute(new ScrollGraphCommand()
                    {
                        Position = position
                    });

                }
            }

            _saveOnNextUpdate = true;
            //SaveNavSnapshot();

        }

        public void Execute(FilterBySelectionCommand command)
        {

            var diagramViewModel = InvertGraphEditor.CurrentDiagramViewModel;
            if (diagramViewModel == null) return;

            if (diagramViewModel.SelectedNode == null) return;
            if (diagramViewModel.SelectedNode.IsFilter)
            {

                if (diagramViewModel.SelectedNode.GraphItemObject == diagramViewModel.GraphData.CurrentFilter)
                {
                    diagramViewModel.GraphData.PopFilter();

                }
                else
                {
                    var graphFilter = diagramViewModel.SelectedNode.GraphItemObject as IGraphFilter;
                    diagramViewModel.GraphData.PushFilter(graphFilter);
                }
             //   if (command.SaveInHistory) SaveNewStep(null);

            }

            _saveOnNextUpdate = true;
        }




        public void Execute(NavigateByNameCommand command)
        {
            var graphItem = Repository.AllOf<IDiagramNodeItem>().FirstOrDefault(p => p.Name == command.ItemName);
            if (graphItem == null) return;
            var node = graphItem.Node;
            if (node != null)
            {
                Execute<NavigateToNodeCommand>(new NavigateToNodeCommand()
                {
                    Node = node,
                });
            }
        }

        public void Execute(NavigateByIdCommand command)
        {
            var graphItem = Repository.AllOf<IDiagramNodeItem>().FirstOrDefault(p => p.Identifier== command.Identifier);
            if (graphItem == null) return;
            var node = graphItem.Node;
            if (node != null)
            {
                Execute<NavigateToNodeCommand>(new NavigateToNodeCommand()
                {
                    Node = node,
                });
            }
        }

        public void Execute(NavigateForwardCommand command)
        {
            UnityEngine.Profiling.Profiler.BeginSample("Load Navigation Step");

            var navHistoryItems = Repository.All<NavHistoryItem>().ToList();
            var activeHistoryItem = navHistoryItems.FirstOrDefault(i => i.IsActive);
            if (activeHistoryItem == null) return;
            var closesHistoryItem = navHistoryItems.Where(i=>i.Time > activeHistoryItem.Time).OrderBy(i=>i.Time).FirstOrDefault();
            if (closesHistoryItem == null) return;

            activeHistoryItem.IsActive = false;
            closesHistoryItem.IsActive = true;

            Execute(new NavigateCommand()
            {
                ItemId = closesHistoryItem.ItemId,
                GraphId = closesHistoryItem.GraphId,
                Workspaceid = closesHistoryItem.WorkspaceId,
                Scroll = closesHistoryItem.Scroll,
                FilterId = closesHistoryItem.FilterId,
            });
            UnityEngine.Profiling.Profiler.EndSample();
            _saveOnNextUpdate = false;
        }

        public void Execute(NavigateBackCommand command)
        {
            UnityEngine.Profiling.Profiler.BeginSample("Load Navigation Step");
            var navHistoryItems = Repository.All<NavHistoryItem>().ToList();
            var activeHistoryItem = navHistoryItems.FirstOrDefault(i => i.IsActive);
            if (activeHistoryItem == null) return;
            var closesHistoryItem = navHistoryItems.Where(i => i.Time < activeHistoryItem.Time).OrderByDescending(i => i.Time).FirstOrDefault();
            if (closesHistoryItem == null) return;

            activeHistoryItem.IsActive = false;
            closesHistoryItem.IsActive = true;

            Execute(new NavigateCommand()
            {
                ItemId = closesHistoryItem.ItemId,
                GraphId = closesHistoryItem.GraphId,
                Workspaceid = closesHistoryItem.WorkspaceId,
                FilterId = closesHistoryItem.FilterId,
                Scroll = closesHistoryItem.Scroll,
            });

            _saveOnNextUpdate = false;
            UnityEngine.Profiling.Profiler.EndSample();
//            var filter =
//                Repository.AllOf<IGraphFilter>().Where(f => f.Identifier == closesHistoryItem.FilterId).FirstOrDefault();
//
        }

        private void SaveNavSnapshot()
        {

            UnityEngine.Profiling.Profiler.BeginSample("Save Navigation Step");

            if (WorkspaceService == null || WorkspaceService.CurrentWorkspace == null ||
                WorkspaceService.CurrentWorkspace.CurrentGraph == null || WorkspaceService.CurrentWorkspace.CurrentGraph.CurrentFilter == null) return;
            
            var navHistoryItems = Repository.All<NavHistoryItem>().OrderBy(i => i.Time).ToList();
            var activeHistoryItem = navHistoryItems.FirstOrDefault(i => i.IsActive);
            if (activeHistoryItem != null) 
            {
                activeHistoryItem.IsActive = false;
                Repository.RemoveAll<NavHistoryItem>(i => i.Time > activeHistoryItem.Time);
            }

            Vector2 scroll = Vector2.zero;
            Signal<IQueryDiagramScroll>(_=>_.QueryDiagramScroll(ref scroll));

            var newHistoryItem = new NavHistoryItem()
            {
                IsActive = true,
                Time = DateTime.Now,
                //ItemId = itemId,
                Scroll = scroll,
                FilterId = WorkspaceService.CurrentWorkspace.CurrentGraph.CurrentFilter.Identifier,
                WorkspaceId = WorkspaceService.CurrentWorkspace.Identifier,
                GraphId = WorkspaceService.CurrentWorkspace.CurrentGraphId
            };

            Repository.Add(newHistoryItem);

            if (navHistoryItems.Count > 30)
            {
                var oldestItem = navHistoryItems.FirstOrDefault();
                if(oldestItem != null) Repository.Remove(oldestItem);
            }
        


            UnityEngine.Profiling.Profiler.EndSample();

        }

        public void Execute(NavigateCommand command)
        {
            IGraphData graph = null;
            GraphNode filter = null;
           // IDiagramNodeItem item = null;
            IGraphFilter[] filterPath = null;

            //if(!string.IsNullOrEmpty(command.ItemId))
           //     item = Repository.GetById<IDiagramNodeItem>(command.ItemId);
           // if (!string.IsNullOrEmpty(command.FilterId))
            filter = Repository.GetById<GraphNode>(command.FilterId);
           // if (!string.IsNullOrEmpty(command.GraphId))
            graph = Repository.GetById<IGraphData>(command.GraphId);
            //if (!string.IsNullOrEmpty(command.Workspaceid))
           

//            if (item == null && graph != null)
//            {
//                workspace = workspace ?? WorkspaceService.Workspaces.FirstOrDefault(p => p.Graphs.Any(x => x.Identifier == graph.Identifier));
//                filter = filter ?? graph.RootFilter as GraphNode;
//            }
//            else if(item != null)
//            {
//                graph = graph ?? item.Graph;
//                workspace = workspace ?? WorkspaceService.Workspaces.FirstOrDefault(p => p.Graphs.Any(x => x.Identifier == graph.Identifier));
//                if(item.Node != null) filter = filter ?? item.Node;
//            }
//            else if(filter == null)
//            {
//                throw new Exception("Cannot navigate: Invalid address");
//            }

//            if (workspace == null || filter == null || graph == null)
//            {
//                var ws = workspace == null ? "UnknownWorkspace" : workspace.Name;
//                var gp = graph == null ? "UnknownGraph" : graph.Name;
//                var f = filter == null ? "UnknownFilter" : filter.Name;
//                Signal<INotify>(_ => _.Notify(string.Format("Invalid Address: {0}/{1}/{2}", ws, gp, f), NotificationIcon.Error));
//
//                return;
//            };

            //Open Workspace

            if (WorkspaceService.CurrentWorkspace.Identifier != command.Workspaceid)
            {
                var workspace = Repository.GetById<Workspace>(command.Workspaceid);
                WorkspaceService.Execute(new OpenWorkspaceCommand()
                {
                    Workspace = workspace
                });    
            }


            if (WorkspaceService.CurrentWorkspace.CurrentGraphId != command.GraphId)
            {

                WorkspaceService.CurrentWorkspace.CurrentGraphId = command.GraphId;
                
            }


//            filterPath = filter.FilterPath().ToArray();
//            graph.PopToFilter(graph.RootFilter);
//            foreach (var f in filterPath)
//            {
//                if (f == graph.RootFilter) continue;
//                graph.PushFilter(f);
//            }
//            if(filter != graph.RootFilter && !filterPath.Contains(filter)) graph.PushFilter(filter);

            filterPath = filter.FilterPath().ToArray();
            graph.PopToFilter(graph.RootFilter);
            foreach (var item in filterPath)
            {
                if (item == graph.RootFilter) continue;
                graph.PushFilter(item);
            }
            if (filterPath.LastOrDefault() != filter) graph.PushFilter(filter);

            Execute(new ScrollGraphCommand()
            {
                Position = command.Scroll
            });

        
        }

        public override void Loaded(QFrameworkContainer container)
        {
            base.Loaded(container);
            EditorApplication.update -= SaveSnapshotIfNeeded;
            EditorApplication.update += SaveSnapshotIfNeeded;
        }

        private void SaveSnapshotIfNeeded()
        {
            if (_saveOnNextUpdate)
            {
                _saveOnNextUpdate = false;
                SaveNavSnapshot();
            }
        }


        public void Execute(NavigateByHistoryItemCommand command)
        {
            var navHistoryItems = Repository.All<NavHistoryItem>().ToList();
            var activeHistoryItem = navHistoryItems.FirstOrDefault(i => i.IsActive);
            if (activeHistoryItem == null) return;

            activeHistoryItem.IsActive = false;
            command.Item.IsActive = true;

            Execute(new NavigateCommand()
            {
                ItemId = command.Item.ItemId,
                GraphId = command.Item.GraphId,
                Workspaceid = command.Item.WorkspaceId,
                FilterId = command.Item.FilterId,
            });

            _saveOnNextUpdate = false;
        }

        public void CommandExecuted(ICommand command)
        {
        }

        public void Execute(SaveNavigationHistoryStepCommand command)
        {
            _saveOnNextUpdate = true;
        }

        private bool _saveOnNextUpdate = false;

        public void PropertyChanged(IDataRecord record, string name, object previousValue, object nextValue)
        {
            if (record is Workspace && name == "CurrentGraphId")
            {
                _saveOnNextUpdate = true;
            }
        }

        public void WorkspaceChanged(Workspace workspace)
        {
            _saveOnNextUpdate = true;
        }

        public void MouseDown(MouseEvent mouse)
        {
            //InvertApplication.Log(mouse.MouseButton.ToString());
        }

        public bool KeyUp(bool control, bool alt, bool shift, KeyCode character)
        {
            if (character == KeyCode.Minus)
            {
                Execute(new NavigateBackCommand());
                return true;
            }
            if (character == KeyCode.Plus)
            {
                Execute(new NavigateForwardCommand());
                return true;
            }
            return false;
        }
    }

    public class NavigateToNodeCommand : Command
    {
        public IDiagramNode Node;
        public bool Select { get; set; }
        public string FilterId { get; set; }

    }
}
