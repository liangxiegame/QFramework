using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using QFramework.CodeGen;
using Invert.Data;
using UnityEngine;

namespace QFramework.CodeGen
{
    public abstract class DiagramNodeViewModel : GraphItemViewModel
    {
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
                    return;
                }
            }
            foreach (var item in ContentItems)
            {
                if (record.IsNear(item.DataObject as IDataRecord))
                {
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
                    return;
                }
            }
            foreach (var item in ContentItems)
            {
                if (record.IsNear(item.DataObject as IDataRecord))
                {
                    return;
                }
            }
        }

        public bool IsExternal { get; set; }


        protected DiagramNodeViewModel()
        {

        }


        public virtual Vector2 Position
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
            }
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
                IsDirty = true;
            }
        }

        public virtual bool AllowCollapsing
        {
            get { return ContentItems.Count > 0; }
        }


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
            }
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