using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using QF.GraphDesigner;
using Invert.Data;
using Invert.Windows;
using QF.MVVM;
using UnityEngine;

namespace QF.GraphDesigner
{
    public class DiagramViewModel : ViewModel, IDataRecordInserted, IDataRecordRemoved, IDataRecordPropertyChanged
    {
        private MVVM.ObservableCollection<GraphItemViewModel> _graphItems = new MVVM.ObservableCollection<GraphItemViewModel>();
    

        private InspectorViewModel _inspectorViewModel;
        private GraphDesignerNavigationViewModel _navigationViewModel;


        public float SnapSize
        {
            get
            {
                //Settings.SnapSize* Scale;
                return 12f;//
            }
        }
	    ElementDiagramSettings _settings = new ElementDiagramSettings();
        public ElementDiagramSettings Settings
        {
            get { return _settings; }
        }

        public IEnumerable<GraphItemViewModel> AllViewModels
        {
            get
            {
                foreach (var item in GraphItems)
                {
                    foreach (var child in item.ContentItems)
                    {
                        yield return child;
                    }
                    yield return item;
                }
            }
        }
        public IEnumerable<GraphItemViewModel> SelectedGraphItems
        {
            get { return AllViewModels.Where(p => p.IsSelected); }
        }

        public void NavigateTo(IDiagramNode node)
        {
            // TODO 2.0 Rewrite Navigate To
        }

        public float Scale
        {
            get { return InvertGraphEditor.DesignerWindow.Scale; }
        }
        public Rect DiagramBounds
        {
            get
            {
                Rect size = new Rect();
                foreach (var diagramItem in GraphItems)
                {
                    var rect = diagramItem.Bounds.Scale(Scale);

                    if (rect.x < 0)
                        rect.x = 0;
                    if (rect.y < 0)
                        rect.y = 0;

                    if (rect.x + rect.width > size.x + size.width)
                    {
                        size.width = rect.x + rect.width;
                    }
                    if (rect.y + rect.height > size.y + size.height)
                    {
                        size.height = rect.y + rect.height;
                    }
                }
                size.height += ((float)Screen.height/2);
                size.width += ((float)Screen.width / 2);
#if UNITY_EDITOR
                if (size.height < Screen.height)
                {
                    size.height = Screen.height;
                }
                if (size.width < Screen.width)
                {
                    size.width = Screen.width;
                }
#endif
                return size; ;
            }

        }

        public GraphItemViewModel SelectedGraphItem
        {
            get { return SelectedGraphItems.FirstOrDefault(); }
        }

        public DiagramNodeViewModel SelectedNode
        {
            get { return SelectedGraphItems.OfType<DiagramNodeViewModel>().FirstOrDefault(); }
        }
        public IEnumerable<DiagramNodeViewModel> SelectedNodes
        {
            get { return SelectedGraphItems.OfType<DiagramNodeViewModel>(); }
        }
        public IEnumerable<GraphItemViewModel> SelectedNodeItems
        {
            get
            {
                return GraphItems.OfType<DiagramNodeViewModel>().SelectMany(p => p.ContentItems).Where(p => p.IsSelected);
            }
        }

        public GraphItemViewModel SelectedNodeItem
        {
            get { return SelectedNodeItems.FirstOrDefault(); }
        }

        public IGraphData GraphData
        {
            get { return DataObject as IGraphData; }
        }

        //public IEnumerable<OutputGenerator> CodeGenerators
        //{
        //    get
        //    {
        //      
        //        return InvertGraphEditor.GetAllCodeGenerators(
        //            InvertApplication.Container.Resolve<IGraphConfiguration>(), GraphData.Repository.Allof);
        //    }
        //}

        public override void DataObjectChanged()
        {
            base.DataObjectChanged();
            GraphItems.Clear();
        }


        public IRepository CurrentRepository { get; set; }

        public DiagramViewModel(IGraphData diagram)
        {

            if (diagram == null) throw new Exception("Diagram not found");
            CurrentRepository = diagram.Repository;
            DataObject = diagram;

        }

        public void Invalidate()
        {
            foreach (var item in GraphItems)
            {
                item.IsDirty = true;
            }
        }
        public Dictionary<string, IFilterItem> FilterItems { get; set; }


        public void Load(bool async = false)
        {
            GraphItems.Clear();
            //GraphItems.Add(InspectorViewModel);

            // var graphItems = new List<GraphItemViewModel>();
            //// var time = DateTime.Now;
            // foreach (var item in CurrentNodes)
            // {

            //     // Get the ViewModel for the data
            //     //InvertApplication.Log("B-A" + DateTime.Now.Subtract(time).TotalSeconds.ToString());
            //     var mapping = InvertApplication.Container.RelationshipMappings[item.GetType(), typeof(ViewModel)];
            //     if (mapping == null) continue;
            //     var vm = Activator.CreateInstance(mapping, item, this) as GraphItemViewModel;
            //     //var vm = 
            //     //    InvertApplication.Container.ResolveRelation<ViewModel>(item.GetType(), item, this) as
            //     //        GraphItemViewModel;
            //     //InvertApplication.Log("B-B" + DateTime.Now.Subtract(time).TotalSeconds.ToString());
            //     if (vm == null)
            //     {
            //         if (InvertGraphEditor.Platform.MessageBox("Node Error", string.Format("Couldn't find view-model for {0} would you like to remove this item?", item.GetType()), "Yes", "No"))
            //         {
            //             CurrentRepository.Remove(item);
            //         }
            //         continue;
            //     }
            //     vm.DiagramViewModel = this;
            //     GraphItems.Add(vm);
            //     // Clear the connections on the view-model
            //     vm.Connectors.Clear();
            //     vm.GetConnectors(vm.Connectors);
            //     connectors.AddRange(vm.Connectors);
            // }            
            CurrentNodes = GraphData.CurrentFilter.FilterNodes.Distinct().ToArray();
            NavigationViewModel.Refresh();
            //if (async)
            //{
            InvertApplication.SignalEvent<ITaskHandler>(_ => _.BeginBackgroundTask(AddGraphItems(CurrentNodes)));
            //}
            //else
            //{
            //var e = AddGraphItems();
            //while (e.MoveNext())
            //{

            //}
            //}


        }

        public IEnumerator AddGraphItems(IEnumerable<IDiagramNode> items)
        {
            var dictionary = new Dictionary<string, IFilterItem>();
            foreach (var item in GraphData.CurrentFilter.FilterItems)
            {
                if (dictionary.ContainsKey(item.NodeId))
                {
                    item.Repository.Remove(item);
                    continue;
                }
                dictionary.Add(item.NodeId, item);
            }

            FilterItems = dictionary;



            IsLoading = true;
            var connectors = new List<ConnectorViewModel>();
            // var time = DateTime.Now;
            foreach (var item in items)
            {
                
                // Get the ViewModel for the data
                //InvertApplication.Log("B-A" + DateTime.Now.Subtract(time).TotalSeconds.ToString());
                var mapping = InvertApplication.Container.RelationshipMappings[item.GetType(), typeof(ViewModel)];
                if (mapping == null) continue;
                var vm = Activator.CreateInstance(mapping, item, this) as GraphItemViewModel; 
                //var vm = 
                //    InvertApplication.Container.ResolveRelation<ViewModel>(item.GetType(), item, this) as
                //        GraphItemViewModel;
                //InvertApplication.Log("B-B" + DateTime.Now.Subtract(time).TotalSeconds.ToString());
                if (vm == null)
                {
                    if (InvertGraphEditor.Platform.MessageBox("Node Error", string.Format("Couldn't find view-model for {0} would you like to remove this item?", item.GetType()), "Yes", "No"))
                    {
                        CurrentRepository.Remove(item);
                    }
                    continue;
                }
                vm.DiagramViewModel = this;
                GraphItems.Add(vm);
                //// Clear the connections on the view-model
                //vm.Connectors.Clear();
                //vm.GetConnectors(vm.Connectors);
                //connectors.AddRange(vm.Connectors);
                yield return new TaskProgress(string.Format("Loading..."), 95f);
            } 
            IsLoading = false;
            RefreshConnectors();
            //AddConnectors(connectors);
            InvertApplication.SignalEvent<IGraphLoaded>(_=>_.GraphLoaded());
            yield break;
        }
        public InspectorViewModel InspectorViewModel
        {
            get
            {
                return _inspectorViewModel ?? (_inspectorViewModel = new InspectorViewModel()
                    {
                        DiagramViewModel = this
                    });
            }
            set { _inspectorViewModel = value; }
        }

        public GraphDesignerNavigationViewModel NavigationViewModel
        {
            get
            {
                return _navigationViewModel ?? (_navigationViewModel = new GraphDesignerNavigationViewModel()
                    {
                        DiagramViewModel = this
                    });
            }
            set { _navigationViewModel = value; }
        }

        //public void RefreshConnectors()
        //{

        //    var items = GraphItems.OfType<ConnectorViewModel>().ToArray();
        //    var connections = GraphItems.OfType<ConnectionViewModel>().ToArray();

        //    foreach (var item in items)
        //    {
        //        GraphItems.Remove(item);
        //    }
        //    foreach (var item in connections)
        //    {
        //        GraphItems.Remove(item);
        //    }
        //    var connectors = new List<ConnectorViewModel>();
        //    foreach (var item in GraphItems)
        //    {
        //        item.GetConnectors(connectors);
        //    }
        //    AddConnectors(connectors);
        //}
        
        public void AddConnectors(List<ConnectorViewModel> connectors)
        {
            foreach (var item in connectors)
            {
                item.DiagramViewModel = this;
                GraphItems.Add(item);
            }
        }
        public void RefreshConnectors()
        {
            if (IsLoading) return;
            var remove = GraphItems.OfType<ConnectionViewModel>().ToArray();
            foreach (var item in remove) GraphItems.Remove(item);


            var strategies = InvertGraphEditor.ConnectionStrategies;

            var outputs = GraphItems.OfType<ConnectorViewModel>().Where(p=>p.Direction == ConnectorDirection.Output).ToArray();
            var inputs = GraphItems.OfType<ConnectorViewModel>().Where(p => p.Direction != ConnectorDirection.Output).ToArray();

            foreach (var output in outputs)
            {
                foreach (var input in inputs)
                {
                    foreach (var strategy in strategies)
                    {
                        if (strategy.IsConnected(output, input))
                        {
                            var strategy1 = strategy;
                            var output1 = output;
                            var input1 = input;
                            output.HasConnections = true;
                            input.HasConnections = true;
                            GraphItems.Add(new ConnectionViewModel(this)
                            {
                                ConnectorA = output,
                                ConnectorB = input,
                                Color = strategy.ConnectionColor,
                                DataObject = output.DataObject, 
                                Remove = (a) =>
                                {
                                    //a.Remove(a);
                                    strategy1.Remove(output1, input1);
                                }
                            });
                        }
                    }
                }
            }
            var connectors = GraphItems.OfType<ConnectorViewModel>().ToArray();

            foreach (var connection in CurrentRepository.All<ConnectionData>())
            {

                ConnectorViewModel startConnector = null;
                ConnectorViewModel endConnector = null;

                for (int i = 0; i < connectors.Length; i++)
                {
                    if (startConnector != null && endConnector != null) break;
                    var p = connectors[i];
                    if (p.Direction == ConnectorDirection.Output && p.Identifier == connection.OutputIdentifier)
                        startConnector = p;
                    else if (p.Direction == ConnectorDirection.Input && p.Identifier == connection.InputIdentifier)
                        endConnector = p;
                }

//                var startConnector = connectors.FirstOrDefault(p =>  p.Direction == ConnectorDirection.Output && p.Identifier == connection.OutputIdentifier);
//                var endConnector = connectors.FirstOrDefault(p => p.Direction == ConnectorDirection.Input && p.Identifier == connection.InputIdentifier);


                if (startConnector == null || endConnector == null) continue;

                var vm = endConnector.ConnectorFor.DataObject as IDiagramNodeItem;


                startConnector.HasConnections = true;
                endConnector.HasConnections = true;
                var connection1 = connection;
                GraphItems.Add(new ConnectionViewModel(this)
                {
                    ConnectorA = endConnector,
                    ConnectorB = startConnector,
                    Color = startConnector.Color,
                    DataObject = connection,
                    Remove = (a) =>
                    {
                        GraphData.RemoveConnection(a.ConnectorB.DataObject as IConnectable, a.ConnectorA.DataObject as IConnectable);
                    }
                });
            }
            //var endTime = DateTime.Now;
            //var diff = new TimeSpan(endTime.Ticks - startTime.Ticks);
            //Debug.Log(string.Format("{0} took {1} seconds {2} milliseconds", "New Strategy", diff.Seconds, diff.Milliseconds));

            //var connections = new List<ConnectionViewModel>();
            //var connectorInfo = new ConnectorInfo(connectors.ToArray(), this, CurrentRepository);
            //foreach (var strategy in InvertGraphEditor.ConnectionStrategies)
            //{
            //    var startTime = DateTime.Now;
            //    strategy.GetConnections(connections, connectorInfo);
            //    var endTime = DateTime.Now;
            //    var diff = new TimeSpan(endTime.Ticks - startTime.Ticks);
            //    Debug.Log(string.Format("{0} took {1} seconds {2} milliseconds", strategy.GetType().Name, diff.Seconds, diff.Milliseconds));
            //}

            //foreach (var item in connections)
            //{
            //    GraphItems.Add(item);
            //    item.ConnectorA.HasConnections = true;
            //    item.ConnectorB.HasConnections = true;
            //}
        }

        public void ClearConnectors(List<ConnectorViewModel> connectors)
        {
            foreach (var item in connectors) GraphItems.Remove(item);
            var items = GraphItems.OfType<ConnectionViewModel>()
                .Where(p => connectors.Contains(p.ConnectorA) || connectors.Contains(p.ConnectorB)).ToArray();
            foreach (var item in items)
            {
                GraphItems.Remove(item);
            }
            connectors.Clear();

        }
        public Color GetColor(IGraphItem dataObject)
        {
            try
            {
                var item = dataObject as IDiagramNodeItem;
                if (item != null)
                {
                    var node = item.Node as GenericNode;
                    if (node != null)
                    {
                        var color = node.Config.GetColor(node);
                        switch (color)
                        {
                            case NodeColor.Black:
                                return Color.black;
                            case NodeColor.Blue:
                                return new Color(0.25f, 0.25f, 0.65f);
                            case NodeColor.DarkDarkGray:
                                return new Color(0.25f, 0.25f, 0.25f);
                            case NodeColor.DarkGray:
                                return new Color(0.45f, 0.45f, 0.45f);
                            case NodeColor.Gray:
                                return new Color(0.65f, 0.65f, 0.65f);
                            case NodeColor.Green:
                                return new Color(0.00f, 1f, 0f);
                            case NodeColor.LightGray:
                                return new Color(0.75f, 0.75f, 0.75f);
                            case NodeColor.Orange:
                                return new Color(0.059f, 0.98f, 0.314f);
                            case NodeColor.Pink:
                                return new Color(0.059f, 0.965f, 0.608f);
                            case NodeColor.Purple:
                                return new Color(0.02f, 0.318f, 0.659f);
                            case NodeColor.Red:
                                return new Color(1f, 0f, 0f);
                            case NodeColor.Yellow:
                                return new Color(1f, 0.8f, 0f);
                            case NodeColor.YellowGreen:
                                return new Color(0.604f, 0.804f, 0.196f);

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                InvertApplication.LogError(string.Format("Node is null on get color {0} : {1}", dataObject.Label, dataObject.Identifier));
            }
            return Color.white;
        }
        public IDiagramNode[] CurrentNodes { get; set; }

        public MVVM.ObservableCollection<GraphItemViewModel> GraphItems
        {
            get { return _graphItems; }
            set { _graphItems = value; }
        }

        public int RefactorCount
        {
            get { return GraphData.RefactorCount; }
        }

        public string Title
        {
            get { return GraphData.CurrentFilter.Name; }
        }

        public bool HasErrors
        {
            get { return GraphData.Errors; }
        }

        public Exception Errors
        {
            get { return GraphData.Error; }
        }

        public bool NeedsUpgrade
        {
            get
            {
                return false;
                //return string.IsNullOrEmpty(DiagramData.Version) || (Convert.ToDouble(DiagramData.Version) < uFrameVersionProcessor.CURRENT_VERSION_NUMBER && uFrameVersionProcessor.REQUIRE_UPGRADE);
            }
        }

        public void Navigate()
        {


            InvertApplication.Execute(new FilterBySelectionCommand()); 
//
//            if (SelectedNode == null) return;
//            if (SelectedNode.IsFilter)
//            {
//
//                if (SelectedNode.GraphItemObject == GraphData.CurrentFilter)
//                {
//                    GraphData.PopFilter();
//                    GraphData.UpdateLinks();
//
//                }
//                else
//                {
//                    var graphFilter = SelectedNode.GraphItemObject as IGraphFilter;
//                    GraphData.PushFilter(graphFilter);
//                    GraphData.UpdateLinks();
//                }
//                //   if (command.SaveInHistory) SaveNewStep(null);
//
//            }
//            InvertApplication.Execute(new FilterBySelectionCommand()
//            {
//                
//            });
        }

        public void Save()
        {
            CurrentRepository.Commit();
        }

        public void MarkDirty()
        {
            CurrentRepository.MarkDirty(GraphData);
        }

        public void RecordUndo(string title)
        {
            // TODO 2.0 Record Undo??
            //CurrentRepository.RecordUndo(GraphData, title);
        }

        public void DeselectAll()
        {
            if (InspectorViewModel != null)
            {
                InspectorViewModel.TargetViewModel = null;
            }

            foreach (var item in AllViewModels.ToArray())
            {
                var ivm = item as ItemViewModel;
                if (ivm != null)
                {
                    if (ivm.IsEditing)
                    {
                        ivm.EndEditing();
                        break;
                    }

                }
                var nvm = item as DiagramNodeViewModel;
                if (nvm != null)
                {
                    if (nvm.IsEditing)
                    {
                        nvm.EndEditing();
                        break;
                    }

                }


                if (item.IsSelected)
                    item.IsSelected = false;
            }


            InvertApplication.SignalEvent<INothingSelectedEvent>(_ => _.NothingSelected());
#if UNITY_EDITOR
            UnityEngine.GUI.FocusControl("");
#endif
        }

        //public void UpgradeProject()
        //{
        //    uFrameEditor.ExecuteCommand(new ConvertToJSON());
        //}

        public void NothingSelected()
        {
            var items = SelectedNodeItems.OfType<ItemViewModel>().Where(p => p.IsEditing).ToArray();
            if (items.Length > 0)
            {
                InvertApplication.Execute(() =>
                {
                    foreach (var item in items)
                    {
                        item.EndEditing();

                    }
                });
            }

            DeselectAll();

            //InvertGraphEditor.ExecuteCommand(_ => { });
        }

        public void Select(GraphItemViewModel viewModelObject)
        {
            if (viewModelObject == null) return;

            InspectorViewModel.TargetViewModel = viewModelObject;

            if (viewModelObject.IsSelected)
            {
                return;
            }
            if (LastMouseEvent != null && LastMouseEvent.ModifierKeyStates != null && !LastMouseEvent.ModifierKeyStates.Alt)
                DeselectAll();

            viewModelObject.IsSelected = true;
            InvertApplication.SignalEvent<IGraphSelectionEvents>(
                _ => _.SelectionChanged(viewModelObject));
        }

        public IEnumerable<IDiagramNode> GetImportableItems()
        {
            return GraphData.CurrentFilter.GetImportableItems();
        }


        //public void UpgradeProject()
        //{
        //    InvertApplication
        //    InvertGraphEditor.ExecuteCommand((n) =>
        //    {
        //        Process15Uprade();
        //    });

        //}

        //public void Process15Uprade()
        //{

        //}

        public FilterItem AddNode(IDiagramNode newNodeData)
        {
            return AddNode(newNodeData, LastMouseEvent.MouseDownPosition);
        }

        public MouseEvent LastMouseEvent { get; set; }

        public FilterItem AddNode(IDiagramNode newNodeData, Vector2 position)
        {
            newNodeData.GraphId = GraphData.Identifier;
            CurrentRepository.Add(newNodeData);

            if (string.IsNullOrEmpty(newNodeData.Name))
                newNodeData.Name =
                    CurrentRepository.GetUniqueName("New" + newNodeData.GetType().Name.Replace("Data", ""));

            return GraphData.CurrentFilter.ShowInFilter(newNodeData, position);
        }

        public IEnumerable<object> ContextObjects
        {
            get
            {
                yield return this;
                yield return DataObject;

                foreach (var nodeItem in GraphItems.Where(p => p.IsMouseOver || p.IsSelected).OfType<ConnectorViewModel>())
                {
                    yield return nodeItem;
                    yield return nodeItem.DataObject;

                }
                foreach (var nodeItem in GraphItems.SelectMany(p => p.ContentItems).Where(p => p.IsMouseOver || p.IsSelected))
                {
                    if (nodeItem is DiagramNodeViewModel) continue;
                    yield return nodeItem;
                    yield return nodeItem.DataObject;

                }
                foreach (var nodeItem in GraphItems.Where(p => p.IsMouseOver || p.IsSelected).OfType<DiagramNodeViewModel>())
                {
                    yield return nodeItem;
                    yield return nodeItem.DataObject;

                }

            }
        }

        public bool UseStraightLines
        {
            get { return GraphData.CurrentFilter.UseStraightLines; }
        }

        public bool IsLoading { get; set; }



        public void NavigateTo(string identifier)
        {
            InvertApplication.Execute(new NavigateByIdCommand()
            {
                Identifier = identifier
            });
        }
        public void NavigateByName(string name)
        {

          InvertApplication.Execute(new NavigateByNameCommand()
          {
              ItemName = name
          });
        }

        public void ShowQuickAdd()
        {

            //var mousePosition = LastMouseEvent.MouseDownPosition;
            //var items = InvertApplication.Plugins.OfType<IPrefabNodeProvider>().SelectMany(p => p.PrefabNodes(CurrentRepository)).ToArray();

            //InvertGraphEditor.WindowManager.InitItemWindow(items, _ =>
            //{
            //    _.Diagram = this;
            //    _.MousePosition = mousePosition;
            //    _.Action(_);
            //});
        }

        public void ShowContainerDebug()
        {
            var mousePosition = LastMouseEvent.MouseDownPosition;
            var items = InvertApplication.Container.Instances.Select(p => new DefaultItem(string.Format("{0} : {1}", p.Key.Item2, p.Value.GetType().Name), p.Key.Item1.Name));

            InvertGraphEditor.WindowManager.InitItemWindow(items, _ =>
            {

            });
        }

        public void UpgradeProject()
        {

        }

        public void RecordInserted(IDataRecord record)
        {

            if (record is ConnectionData)
            {
                RefreshConnectors();
            }
            else
            {
                var filterItem = record as IFilterItem;
                if (filterItem != null)
                {
                    if (filterItem.FilterId == GraphData.CurrentFilter.Identifier)
                    {
                        var e = AddGraphItems(new[] { filterItem.Node });
                        while (e.MoveNext())
                        {

                        }
                    }
                }
            }

            for (int index = 0; index < GraphItems.Count; index++)
            {
                var item = GraphItems[index];
                item.RecordInserted(record);
            }
        }

        public void RecordRemoved(IDataRecord record)
        {
            if (record == GraphData)
            {
                
                return;
            }
            List<GraphItemViewModel> removeList = new List<GraphItemViewModel>();
            var filterItem = record as FilterItem;
            if (filterItem != null)
            {
                var node = filterItem.Node;
                if (node != null)
                { 
                    foreach (var item in GraphItems)
                    {
                        if (item.DataObject == node)
                        {
                            removeList.Add(item);
                            removeList.AddRange(item.Connectors.OfType<GraphItemViewModel>());
                        }
                    }
                    
                }
                if (FilterItems.ContainsKey(filterItem.NodeId))
                {
                    FilterItems.Remove(filterItem.NodeId);
                }
            }
            else
            {
               
                for (int index = 0; index < GraphItems.Count; index++)
                {
                    var item = GraphItems[index];
                    if (item.DataObject == record)
                    {
                        removeList.Add(item);
                        removeList.AddRange(item.Connectors.OfType<GraphItemViewModel>());
                    }
                    item.RecordRemoved(record);
                }

            
            }
            foreach (var remove in removeList)
                GraphItems.Remove(remove);
          if (removeList.Count > 0)
            RefreshConnectors();
         
        }

        public void PropertyChanged(IDataRecord record, string name, object previousValue, object nextValue)
        {
            //if (record == GraphData)
            //{
            //    Load(true);
            //    return;
            //}
            for (int index = 0; index < GraphItems.Count; index++)
            {
                var item = GraphItems[index];
                item.PropertyChanged(record, name, previousValue, nextValue);
            }
        }
    }
}