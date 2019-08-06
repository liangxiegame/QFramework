using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using QF.GraphDesigner;
using Invert.Data;
using QF.MVVM;
using UnityEngine;

namespace QF.GraphDesigner
{

    public abstract class DiagramNodeViewModel<TData> : DiagramNodeViewModel where TData : IDiagramNode
    {
        protected DiagramNodeViewModel()
        {
        }

        protected DiagramNodeViewModel(TData graphItemObject, DiagramViewModel diagramViewModel)
            : base(graphItemObject, diagramViewModel)
        {

        }

        public override void DataObjectChanged()
        {
            base.DataObjectChanged();
            DiagramViewModel.ClearConnectors(Connectors);
            GetConnectors(Connectors);
            DiagramViewModel.AddConnectors(Connectors);
            DiagramViewModel.RefreshConnectors();
        }

        protected override void CreateContent()
        {

            base.CreateContent();

            foreach (var item in GraphItem.DisplayedItems)
            {
                var vm = GetDataViewModel(item);

                if (vm == null)
                {
                    InvertApplication.LogError(string.Format("Couldn't find view-model for {0}", item.GetType()));
                    continue;
                }
                vm.DiagramViewModel = DiagramViewModel;
                ContentItems.Add(vm);
            }



            AddPropertyFields();
        }

        public void AddPropertyFields(string headerText = null)
        {
            var ps = GraphItem.GetPropertiesWithAttribute<NodeProperty>().ToArray();
            if (ps.Length < 1) return;

            if (!string.IsNullOrEmpty(headerText))
                ContentItems.Add(new SectionHeaderViewModel()
                {
                    Name = headerText,
                });

            foreach (var property in ps)
            {
                PropertyInfo property1 = property.Key;
                var vm = new PropertyFieldViewModel(this)
                {
                    Type = property.Key.PropertyType,
                    Name = property.Key.Name,
                    InspectorType = property.Value.InspectorType,
                    CustomDrawerType = property.Value.CustomDrawerType,
                    Getter = () => property1.GetValue(GraphItem, null),
                    DataObject = GraphItem,
                    DisableInputs = true,
                    DisableOutputs = true,
                    Setter = (d,v) =>
                    {
                        property1.SetValue(d, v, null);

                    }
                };
                ContentItems.Add(vm);
            }
        }

        protected GraphItemViewModel GetDataViewModel(IGraphItem item)
        {
            var vm = InvertGraphEditor.Container.ResolveRelation<ItemViewModel>(item.GetType(), item, this) as GraphItemViewModel;
            vm.DiagramViewModel = DiagramViewModel;

            return vm;
        }

        public TData GraphItem
        {
            get { return (TData)GraphItemObject; }
        }
    }

    public abstract class DiagramNodeViewModel : GraphItemViewModel
    {
        private bool _isSelected = false;

        public IDiagramNode GraphItemObject
        {
            get { return DataObject as IDiagramNode; }
            set { DataObject = value; }
        }

        public virtual bool IsEditable
        {
            get { return true; }
        }
        public DiagramNodeViewModel(IDiagramNode graphItemObject, DiagramViewModel diagramViewModel)
            : this()
        {
            ColumnSpan = 2;
            DiagramViewModel = diagramViewModel;
            GraphItemObject = graphItemObject;

            OutputConnectorType = graphItemObject.GetType();
            InputConnectorType = graphItemObject.GetType();
    
        }
        
        public override void PropertyChanged(IDataRecord record, string name, object previousValue, object nextValue)
        {
            base.PropertyChanged(record, name, previousValue, nextValue);

            if (record == DataObject || record.IsNear(DataObject as IDataRecord))
            {
                DataObjectChanged();
                return;
            }

            //foreach (var item in ContentItems)
            //{
            //    if (item.DataObject == record)
            //    {
            //        DataObjectChanged();
            //    }
            //}


        }

        public override void RecordInserted(IDataRecord record)
        {
            base.RecordInserted(record);
            var nodeItem = DataObject as IDiagramNode;
            if (nodeItem != null)
            {
                if (record.IsNear(nodeItem))
                {
                    DataObjectChanged();
                    return;
                }
            }
            foreach (var item in ContentItems)
            {
                if (record.IsNear(item.DataObject as IDataRecord))
                {
                    DataObjectChanged();
                    return;
                }
            }
        }

        public override void RecordRemoved(IDataRecord record)
        {
            base.RecordRemoved(record);

            var nodeItem = DataObject as IDiagramNode;
            if (nodeItem != null)
            {
                if (record.IsNear(nodeItem))
                {
                    DataObjectChanged();
                    return;
                }
            }
            foreach (var item in ContentItems)
            {
                if (record.IsNear(item.DataObject as IDataRecord))
                {
                    DataObjectChanged();
                    return;
                }
            }
        }

        public bool IsExternal { get; set; }
        public string TagsString
        {
            get { return string.Join(" | ", Tags.ToArray()); }
        }
        public virtual Type ExportGraphType
        {
            get { return null; }
        }

        public override ConnectorViewModel InputConnector
        {
            get
            {
                if (!HasInputs)
                {
                    return null;
                }
                return base.InputConnector;
            }
        }

        public override ConnectorViewModel OutputConnector
        {
            get
            {
                if (!HasOutputs)
                    return null;
                return base.OutputConnector;
            }
        }

        public virtual bool HasInputs
        {
            get { return true; }
        }
        public virtual bool HasOutputs
        {
            get { return true; }
        }
        protected override ConnectorViewModel CreateInputConnector()
        {

            return base.CreateInputConnector();
        }

        protected DiagramNodeViewModel()
        {

        }

        public MVVM.ObservableCollection<GraphItemViewModel> PropertyViewModels { get; set; }

        public override Vector2 Position
        {
            get
            {
                if (!DiagramViewModel.FilterItems.ContainsKey(GraphItemObject.Identifier)) return new Vector2(45, 45);
                return DiagramViewModel.FilterItems[GraphItemObject.Identifier].Position;
            }
            set
            {
                DiagramViewModel.FilterItems[GraphItemObject.Identifier].Position = value;
            }
        }

        public virtual NodeColor Color
        {
            get
            {
                return NodeColor.LightGray;
            }
        }
        //public bool Dirty { get; set; }
        public override bool IsSelected
        {
            get
            {
                return GraphItemObject.IsSelected;
            }
            set
            {
                if (value == false)
                    IsEditing = false;
                GraphItemObject.IsSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        public override void GetConnectors(List<ConnectorViewModel> list)
        {
            // base.GetConnectors(list);

            foreach (var item in ContentItems)
            {

                item.GetConnectors(list);
                if (IsCollapsed)
                {
                    if (item.InputConnector != null)
                    {
                        item.InputConnector.Disabled = true;
                        item.InputConnector.ConnectorFor = this;
                    }

                    if (item.OutputConnector != null)
                    {
                        item.OutputConnector.Disabled = true;
                        item.OutputConnector.ConnectorFor = this;
                    }
                }
            }

            if (InputConnector != null)
                list.Add(InputConnector);
            if (OutputConnector != null)
                list.Add(OutputConnector);



        }


        public virtual bool IsCollapsed
        {
            get
            {


                if (AllowCollapsing)
                {
                    if (!DiagramViewModel.FilterItems.ContainsKey(GraphItemObject.Identifier)) return false;
                    return DiagramViewModel.FilterItems[GraphItemObject.Identifier].Collapsed;
                }

                return true;

            }
            set
            {
                if (!DiagramViewModel.FilterItems.ContainsKey(GraphItemObject.Identifier)) return;
                DiagramViewModel.FilterItems[GraphItemObject.Identifier].Collapsed = value;
                OnPropertyChanged("IsCollapsed");
                IsDirty = true;
                DataObjectChanged();
            }
        }

        public virtual bool ShowSubtitle { get { return false; } }

        public virtual float HeaderSize
        {
            get
            {
                return 27;
            }
        }

        public virtual bool AllowCollapsing
        {
            get { return ContentItems.Count > 0; }
        }

        public override void DataObjectChanged()
        {
            base.DataObjectChanged();
            ContentItems.Clear();

            //IsLocal = DiagramViewModel == null || DiagramViewModel.CurrentRepository.NodeItems.Contains(GraphItemObject);
            //var time = DateTime.Now;
            //if (!IsCollapsed)
            //{
            CreateContent();
            //}

            //InvertApplication.Log(this.GetType().Name + ": " + DateTime.Now.Subtract(time).TotalSeconds.ToString());
            if (GraphItemObject.IsEditing)
            {
                BeginEditing();
            }
            IsDirty = true;
            IsExternal = GraphItemObject.Graph.Identifier != DiagramViewModel.GraphData.Identifier;

        }

        protected virtual void CreateContent()
        {

        }
        public bool IsLocal { get; set; }
        public bool IsEditing
        {
            get { return GraphItemObject.IsEditing; }
            set
            {
                if (value == false)
                    EndEditing();
                GraphItemObject.IsEditing = value;

            }
        }

        public string FullLabel
        {
            get { return GraphItemObject.FullLabel; }
        }

        public IEnumerable<IDiagramNodeItem> Items
        {
            get { return GraphItemObject.DisplayedItems; }
        }

        public virtual string SubTitle
        {
            get
            {
                return GraphItemObject.SubTitle;
            }
        }

        public override string Name
        {
            get
            {
                if (IsEditing)
                {
                    return editText;
                }
                
                return GraphItemObject.Name;
            }
            set
            {
                Rename(value);
                OnPropertyChanged("Name");
            }
        }

        private INodeStyleSchema _normalStyleSchema;
        private INodeStyleSchema _minimalisticStyleSchema;
        private INodeStyleSchema _boldStyleSchema;

        public virtual string HeaderBaseImage
        {
            get
            {
                return "Header3";
            }
        }

        public virtual INodeStyleSchema NormalStyleSchema
        {
            get
            {
                return _normalStyleSchema ?? (_normalStyleSchema = CachedStyles.NodeStyleSchemaNormal);
            }
            set { _normalStyleSchema = value; }
        }
        public virtual INodeStyleSchema MinimalisticStyleSchema
        {
            get
            {
                return _minimalisticStyleSchema ?? (_minimalisticStyleSchema = CachedStyles.NodeStyleSchemaMinimalistic);
            }
            set { _minimalisticStyleSchema = value; }
        }

        public virtual INodeStyleSchema BoldStyleSchema
        {
            get
            {
                return _boldStyleSchema ?? (_boldStyleSchema = CachedStyles.NodeStyleSchemaBold);
            }
            set { _boldStyleSchema = value; }
        }

        public virtual INodeStyleSchema StyleSchema
        {
            get
            {
                if (IsCurrentFilter) return BoldStyleSchema;
                return NormalStyleSchema;
            }
        }

        public virtual string IconName
        {
            get { return "CommandIcon"; }
            set { }
        }

        public virtual Color IconTint
        {
            get { return HeaderColor + new Color(0.2f, 0.2f, 0.2f, -0.1f); }
        }

        public virtual Color HeaderColor
        {
            get { return CachedStyles.GetColor(Color); }
        }

        public string Label
        {
            get { return Name; }
        }

        //public bool IsSelected
        //{
        //    get { return _isSelected; }
        //    set
        //    {
        //        SetProperty(ref _isSelected, value, IsSelectedProperty);
        //    }
        //}
        public virtual Type CommandsType
        {
            get { return typeof(IDiagramNode); }
        }

        public override string ToString()
        {
            return GraphItemObject.Identifier;
        }

        public override void Select()
        {

            DiagramViewModel.Select(this);
            base.Select();


        }

        public override bool Enabled
        {
            get { return !this.IsExternal; }
        }


        public string editText = string.Empty;
        private string _headerBaseImage;

        public void Rename(string newText)
        {
            editText = newText;
        }

        public void EndEditing()
        {
            if (!IsEditable) return;
            if (!IsEditing) return;
            InvertApplication.Execute(new ApplyRenameCommand() { Item = GraphItemObject, Name = editText, Old = GraphItemObject.Name });

            Dirty = true;
        }

        public bool Dirty { get; set; }

        public virtual bool IsFilter
        {
            get { return InvertGraphEditor.IsFilter(GraphItemObject.GetType()); }
        }

        public IEnumerable<OutputGenerator> CodeGenerators
        {
            get
            {
                return InvertGraphEditor.GetAllCodeGenerators(InvertGraphEditor.Container.Resolve<IGraphConfiguration>(),
                    new IDataRecord[] { GraphItemObject });
            }
        }

        public bool HasFilterItems
        {
            get
            {
                var filter = GraphItemObject as IGraphFilter;
                if (filter == null)
                {
                    return false;
                }
                return filter.FilterItems.Any();
            }
        }
        public IEnumerable<IDiagramNode> ContainedItems
        {
            get
            {
                var filter = GraphItemObject as IGraphFilter;
                if (filter == null)
                {
                    yield break;
                }
                foreach (var item in filter.FilterNodes)
                {
                    yield return item;
                }
            }
        }
        public virtual IEnumerable<string> Tags
        {
            get { yield break; }
        }

        public virtual ErrorInfo[] Issues
        {
            get { return GraphItemObject.Errors; }
        }

        public bool SaveImage { get; set; }

        public bool IsCurrentFilter
        {
            get { return GraphItemObject.Graph.CurrentFilter == GraphItemObject; }

        }


        public void BeginEditing()
        {
            if (!IsEditable) return;
            editText = GraphItemObject.Name;
            GraphItemObject.BeginEditing();
        }

        public void Remove()
        {
            GraphItemObject.RemoveFromDiagram();
        }

        //public void Hide()
        //{
        //    DiagramViewModel.GraphData.CurrentFilter.HideInFilter(GraphItemObject);
        //}


        public virtual void CtrlClicked()
        {
            //InvertApplication.Execute(() =>
            //{
            //    var fileGenerator = this.CodeGenerators.OfType<CodeGenerator>().FirstOrDefault(p => !p.IsDesignerFile);
            //    if (fileGenerator != null)
            //    {
            //        var filePath = fileGenerator.FullPathName;
            //        //var filename = repository.GetControllerCustomFilename(this.Name);
            //        InvertGraphEditor.Platform.OpenScriptFile("Assets" + fileGenerator.UnityPath);

            //    }
            //});
        }

        public void CtrlShiftClicked()
        {
            //InvertApplication.Execute(() =>
            //{
            //    var fileGenerator = this.CodeGenerators.OfType<CodeGenerator>().LastOrDefault(p => !p.IsDesignerFile);
            //    if (fileGenerator != null)
            //    {

            //        InvertGraphEditor.Platform.OpenScriptFile("Assets" + fileGenerator.UnityPath);

            //    }
            //});
        }


    }
}