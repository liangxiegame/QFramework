using System;
using System.Collections.Generic;
using System.Linq;
using QF.GraphDesigner;

namespace QF.GraphDesigner
{ 
    public class GenericNodeViewModel<TData> : DiagramNodeViewModel<TData> where TData : GenericNode
    {
        protected NodeConfigBase _nodeConfig;

        public override string Comments
        {
            get { return GraphItem != null ? GraphItem.Comments : null; }
        }

        protected override ConnectorViewModel CreateInputConnector()
        {
            var connector = base.CreateInputConnector();
            if (this.DataObject is GenericInheritableNode)
            {
                connector.AlwaysVisible = true;
            }
            return connector;
        }

        public GenericNodeViewModel()
        {
        }

        public GenericNodeViewModel(TData graphItemObject, DiagramViewModel diagramViewModel)
            : base(graphItemObject, diagramViewModel)
        {
        }

        public virtual NodeConfigBase NodeConfig
        {
            get
            {
                return _nodeConfig ?? (
                  _nodeConfig = InvertGraphEditor.Container.GetNodeConfig(DataObject.GetType()) as NodeConfigBase);
            }
        }

        public override IEnumerable<string> Tags
        {
            get
            {
                if (GraphItem.Graph.Identifier != DiagramViewModel.GraphData.Identifier)
                {
                    yield return "->" + GraphItem.Graph.Name;
                }

                if (!StyleSchema.ShowSubtitle)
                {
                    yield return NodeConfig.Name;
                }
                foreach (var item in GraphItem.Flags)
                {
                    yield return item.Name;
                }
                foreach (var item in NodeConfig.Tags)
                {
                    yield return item;
                }
            }
        }

        public override string SubTitle
        {
            get { return NodeConfig.Name; }
        }

        public override ErrorInfo[] Issues
        {
            get { return GraphItem.Errors; }
        }
        
        public override NodeColor Color
        {
            get
            {
                return NodeConfig.GetColor(GraphItem);
                //if (NodeConfig.NodeColor == null) 
                //    return NodeColor.LightGray;
                //return NodeConfig.NodeColor.GetValue(GraphItem);
            }
        }

        public virtual NodeStyle NodeStyle
        {
            get
            {
                return NodeStyle.Normal;
            }
        }

        public override INodeStyleSchema StyleSchema
        {
            get
            {
                if (IsCurrentFilter)return BoldStyleSchema;

                switch (NodeStyle)
                {
                    case NodeStyle.Normal:
                        return NormalStyleSchema;
                    case NodeStyle.Minimalistic:
                        return MinimalisticStyleSchema;
                    case NodeStyle.Bold:
                        return BoldStyleSchema;
                }
                return base.StyleSchema;
            }
        }
        //public override Func<IDiagramNodeItem, IDiagramNodeItem, bool> InputValidator
        //{
        //    get { return GraphItem.ValidateInput; }
        //}

        //public override Func<IDiagramNodeItem, IDiagramNodeItem, bool> OutputValidator
        //{
        //    get { return GraphItem.ValidateOutput; }
        //}

        protected override void CreateContent()
        {
            //base.CreateContent();
            InputConnectorType = NodeConfig.SourceType;
            OutputConnectorType = NodeConfig.SourceType;

            //IsLocal = InvertGraphEditor.CurrentProject.CurrentGraph.NodeItems.Contains(GraphItemObject);
            if (NodeConfig.IsInput)
                ApplyInputConfiguration(NodeConfig,DataObject as IGraphItem,InputConnector,NodeConfig.AllowMultipleInputs);
            if (NodeConfig.IsOutput)
                ApplyOutputConfiguration(NodeConfig, DataObject as IGraphItem, InputConnector, NodeConfig.AllowMultipleOutputs);
            AddPropertyFields();
            CreateContentByConfiguration(NodeConfig.GraphItemConfigurations, GraphItem);

        }



 

        protected void CreateContentByConfiguration(IEnumerable<GraphItemConfiguration> graphItemConfigurations, GenericNode node = null)
        {
            foreach (var item in graphItemConfigurations.OrderBy(p => p.OrderIndex))
            {
                var proxyConfig = item as ConfigurationProxyConfiguration;
                if (proxyConfig != null)
                {
                    if (!IsVisible(proxyConfig.Visibility)) continue;
                    CreateContentByConfiguration(proxyConfig.ConfigSelector(DataObject as GenericNode));
                    continue;
                }
                var inputConfig = item as NodeInputConfig;
                if (inputConfig != null)
                {
                    if (inputConfig.IsOutput)
                    {
                        AddOutput(inputConfig, node);
                    }
                    else if (inputConfig.IsInput)
                    {
                        AddInput(inputConfig, node);
                    }
                }
                var sectionConfig = item as NodeConfigSectionBase;
                if (sectionConfig != null)
                {
                    AddSection(sectionConfig);
                }
            }
        }

        private void AddSection(NodeConfigSectionBase section)
        {
            //if (DiagramViewModel != null && DiagramViewModel.CurrentRepository.CurrentFilter.IsAllowed(null, section.SourceType)) return;
            var section1 = section as NodeConfigSectionBase;
            if (!IsVisible(section.Visibility)) return;

            if (!string.IsNullOrEmpty(section.Name))
            {
                var header = new GenericItemHeaderViewModel()
                {
                    Name = section.Name,
                    NodeViewModel = this,
                    NodeConfig = NodeConfig,
                    SectionConfig = section1,

                };
                //if (section.AttributeInfo != null)
                //{
                //    header.IsNewLine = inputConfig.AttributeInfo.IsNewRow;
                //}
                //else
                //{
                //    header.IsNewLine = true;
                //}

                header.AddCommand =section1.AllowAdding ? new LambdaCommand("Add Item",() =>
                {
                    OnAdd(section, section1, this);
                }) : null; 
                
                
                ContentItems.Add(header);
            }

            if (section1.GenericSelector != null && section1.ReferenceType == null)
            {
                
                foreach (var item in section1.GenericSelector(GraphItem).OfType<IDiagramNodeItem>().OrderBy(p=>p.Order))
                {
                    
                    if (section.SourceType.IsAssignableFrom(item.GetType()))
                    {
                        var vm = GetDataViewModel(item) as GraphItemViewModel;
                        var itemViewModel = vm as ItemViewModel;
                        if (itemViewModel != null)
                        {
                            itemViewModel.IsEditable = section1.IsEditable;
                            ApplyInputConfiguration(section, item, vm.InputConnector, section.AllowMultipleInputs);
                            ApplyOutputConfiguration(section, item, vm.OutputConnector, section.AllowMutlipleOutputs);
                        }
                        
                        if (vm == null)
                        {
                            InvertApplication.LogError(
                                string.Format(
                                    "Couldn't find view-model for {0} in section {1} with child type {2}",
                                    item.GetType(), section1.Name, section1.SourceType.Name));
                            continue;
                        }

                        vm.DiagramViewModel = DiagramViewModel;
                        ContentItems.Add(vm);
                    }
                    else
                    {
                        InvertApplication.LogError(string.Format("Types do not match {0} and {1}", section.SourceType,
                            item.GetType()));
                    }
                }
            }
            else
            {
                foreach (var item in GraphItem.PersistedItems)
                {
                    if (section.SourceType.IsAssignableFrom(item.GetType()))
                    {
                        var vm = GetDataViewModel(item) as ItemViewModel;
                       

                        if (vm == null)
                        {
                            InvertApplication.LogError(string.Format("Couldn't find view-model for {0}", item.GetType()));
                            continue;
                        }
                        vm.IsEditable = section1.IsEditable;
                        vm.DiagramViewModel = DiagramViewModel;
                        if (section1.HasPredefinedOptions)
                        {
                            vm.IsEditable = false;
                        }
                        ApplyInputConfiguration(section, item, vm.InputConnector, section.AllowMultipleInputs);
                        ApplyOutputConfiguration(section, item, vm.OutputConnector, section.AllowMutlipleOutputs);
                        ContentItems.Add(vm);
                    }
                }
            }
        }

        private void OnAdd(NodeConfigSectionBase section, NodeConfigSectionBase section1, DiagramNodeViewModel vm)
        {
            if (section1.AllowAdding && section1.ReferenceType != null && !section1.HasPredefinedOptions)
            {
                SelectReferenceItem(section, section1);
            }
            else
            {
                if (section1.GenericSelector != null && section1.HasPredefinedOptions)
                {
                    SelectFromOptions(section1, vm);
                }
                else
                {
                    CreateNewSectionItem(section1, vm);
                }
            }
        }

        private void CreateNewSectionItem(NodeConfigSectionBase section1, DiagramNodeViewModel vm)
        {
            var item = Activator.CreateInstance(section1.SourceType) as GenericNodeChildItem;
            item.Node = vm.GraphItemObject as GraphNode;
            DiagramViewModel.CurrentRepository.Add(item);
            item.Name = item.Repository.GetUniqueName(section1.Name);
          
            OnAdd(section1, item);

            if (typeof (ITypedItem).IsAssignableFrom(section1.SourceType))
            {
                InvertApplication.Execute(new SelectTypeCommand()
                {
                    PrimitiveOnly = false,
                    AllowNone = false,
                    IncludePrimitives = true,
                    Item = item as ITypedItem,
                    OnSelectionFinished = () =>
                    {
                        item.IsSelected = true;
                        item.IsEditing = true;
                    }
                });
            }
            else
            {
                item.IsEditing = true;
                
            }
       
        }

        private void SelectFromOptions(NodeConfigSectionBase section1, DiagramNodeViewModel vm)
        {
            InvertGraphEditor.WindowManager.InitItemWindow(section1.GenericSelector(GraphItem).ToArray(),
                (selected) =>
                {
                    var item = selected as GenericNodeChildItem;

                    item.Node = vm.GraphItemObject as GraphNode;
                    DiagramViewModel.CurrentRepository.Add(item);
                    if (section1.OnAdd != null)
                        section1.OnAdd(item);
                    else
                    {
                        item.Name = item.Repository.GetUniqueName(section1.Name);
                    }

                    item.IsEditing = true;
                    OnAdd(section1, item);
                });
        }

        private void SelectReferenceItem(NodeConfigSectionBase section, NodeConfigSectionBase section1)
        {
            if (section.AddCommandType != null)
            {
            }
            if (section1.AllowDuplicates)
            {
                InvertGraphEditor.WindowManager.InitItemWindow(section1.GenericSelector(GraphItem).ToArray(),
                    (selected) => { GraphItem.AddReferenceItem(selected, section1); });
            }
            else
            {
                InvertGraphEditor.WindowManager.InitItemWindow(
                    section1.GenericSelector(GraphItem).ToArray()
                        .Where(
                            p =>
                                !GraphItem.PersistedItems.OfType<GenericReferenceItem>()
                                    .Select(x => x.SourceIdentifier)
                                    .Contains(p.Identifier)),
                    (selected) => { GraphItem.AddReferenceItem(selected, section1); });
            }
        }


        private void AddOutput(NodeInputConfig inputConfig, GenericNode node = null)
        {
            if (!IsVisible(inputConfig.Visibility)) return;
            var nodeToUse = node ?? GraphItem;
            var header =  new InputOutputViewModel();
            header.Name = inputConfig.Name.GetValue(node);
            header.DataObject = inputConfig.IsAlias
                ? DataObject
                : inputConfig.GetDataObject(nodeToUse);
            header.OutputConnectorType = inputConfig.SourceType;
            header.IsInput = false;
            header.IsOutput = true;
            if (inputConfig.AttributeInfo != null)
            {
                header.IsNewLine = inputConfig.AttributeInfo.IsNewRow;
            }
            else
            {
                header.IsNewLine = true;
            }
            ContentItems.Add(header);
            ApplyOutputConfiguration(inputConfig, header.DataObject as IGraphItem, header.OutputConnector,  true);
            if (header.InputConnector != null)
            header.OutputConnector.Configuration = inputConfig;
            
        }

        protected static void ApplyOutputConfiguration(GraphItemConfiguration inputConfig, IGraphItem dataItem, ConnectorViewModel connector, bool alwaysVisible = false)
        {
            if (connector != null)
            connector.AlwaysVisible = alwaysVisible;
            //var slot = dataItem as IDiagramNodeItem;
            //if (slot != null)
            //{
            //    connector.Validator = slot.ValidateOutput;
            //}
        }

        private void AddInput(NodeInputConfig inputConfig, GenericNode node = null)
        {
            if (!IsVisible(inputConfig.Visibility)) return;
            var nodeToUse = node ?? GraphItem;
            var header = new InputOutputViewModel();
            header.Name = inputConfig.Name.GetValue(nodeToUse);
            header.DataObject = inputConfig.IsAlias ? DataObject : inputConfig.GetDataObject(nodeToUse);
            header.InputConnectorType = inputConfig.SourceType;
            header.IsInput = true;
            
            if (inputConfig.AttributeInfo != null)
            {
                header.IsNewLine = inputConfig.AttributeInfo.IsNewRow;
            }
            else
            {
                header.IsNewLine = true;
            }
            
            ContentItems.Add(header);
            var g = header.DataObject as IGraphItem;
            if (g != null)
            {
                header.Name = g.Title;
            }
            ApplyInputConfiguration(inputConfig, g,header.InputConnector, true);
            if (header.InputConnector != null)
            header.InputConnector.Configuration = inputConfig;
        }

        protected static void ApplyInputConfiguration(GraphItemConfiguration inputConfig, IGraphItem dataItem, ConnectorViewModel connector,bool alwaysVisible = false)
        {
            if (connector != null)
            connector.AlwaysVisible = alwaysVisible;
            
            //var slot = dataItem as IDiagramNodeItem;
            //if (slot != null)
            //{
            //    connector.Validator = slot.ValidateInput;
            //}
       
        }

        protected bool IsVisible(SectionVisibility section)
        {
            if (DiagramViewModel == null) return true;
            if (section == SectionVisibility.Always) return true;
            if (section == SectionVisibility.WhenNodeIsFilter)
            {
                
                return DiagramViewModel.GraphData.CurrentFilter == GraphItem;
            }
            return DiagramViewModel.GraphData.CurrentFilter != GraphItem;
        }


        protected virtual void OnAdd(NodeConfigSectionBase configSection, GenericNodeChildItem item)
        {

        }
    }



    //public class BulletPointViewModel : GraphItemViewModel
    //{
    //    public GraphItemViewModel AttachedTo { get; set; }

    //    public bool RightJustified { get; set; }

    //    public BulletPointViewModel(GraphItemViewModel nodeViewModel)
    //    {
    //        AttachedTo = nodeViewModel;
    //    }

    //    public override Vector2 Position
    //    {
    //        get
    //        {
    //            return new Vector2();
    //        }
    //    }

    //    public override string Name
    //    {
    //        get { return true; }
    //        set { throw new NotImplementedException(); }
    //    }
    //}

}