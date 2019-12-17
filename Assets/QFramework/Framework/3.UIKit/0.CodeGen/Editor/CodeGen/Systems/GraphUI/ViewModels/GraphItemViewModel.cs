using System;
using System.Collections.Generic;
using Invert.Data;
using UnityEngine;

namespace QFramework.CodeGen
{
    public abstract class GraphItemViewModel : ViewModel,IDataRecordInserted, IDataRecordRemoved, IDataRecordPropertyChanged
    {
        public virtual object DataObject
        {
            get { return null; }
            set
            {
                IsDirty = true;
            }
        }

        public abstract string Name { get; set; }

        public DiagramViewModel DiagramViewModel { get; set; }
        public Rect Bounds
        {
            get { return _bounds; }
            set
            {
                _bounds = value;
                
            }
        }

        public virtual bool Enabled
        {
            get { return true; }
        }

        private bool _isSelected = false;
        private List<ConnectorViewModel> _connectors;
        private readonly List<GraphItemViewModel> _contentItems = new List<GraphItemViewModel>();

        public const string IsSelectedProperty = "IsSelected";


        public virtual bool IsSelected
        {
            get
            {

                return _isSelected;
            }
            set
            {
                if (value == false)
                {
                    foreach (var item in ContentItems)
                    {
                        item.IsSelected = false;
                    }
                }
                _isSelected = value;
                //SetProperty(ref _isSelected, value, IsSelectedProperty);
                if (value) OnSelected();
                else OnDeselected();
            }
        }

        public virtual void OnSelected()
        {
        }

        public virtual void OnDeselected()
        {

        }
        public int Column { get; set; }
        public virtual void Select()
        {
            
            IsSelected = true;

        }

        public override string ToString()
        {
            return GetHashCode().ToString();
        }

        public virtual void PropertyChanged(IDataRecord record, string name, object previousValue, object nextValue)
        {
            
        }

        public virtual void RecordRemoved(IDataRecord record)
        {
            
        }

        public virtual void RecordInserted(IDataRecord record)
        {
            

        }

        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                _isDirty = value;
            }
        }

        public bool IsMouseOver
        {
            get { return _isMouseOver; }
            set
            {
                _isMouseOver = value;
            }
        }

        public List<ConnectorViewModel> Connectors
        {
            get { return _connectors ?? (_connectors = new List<ConnectorViewModel>()); }
            set { _connectors = value; }
        }

        public List<GraphItemViewModel> ContentItems
        {
            get { return _contentItems; }
        
        }

        public virtual Type InputConnectorType { get; set; }
        public virtual Type OutputConnectorType { get; set; }
        
        private ConnectorViewModel _inputConnector;
        public virtual ConnectorViewModel InputConnector
        {
            get
            {
                if (DataObject == null) return null;
                var connectable = DataObject as IConnectable;
                if (connectable != null && !connectable.AllowInputs) 
                    return null;
                return _inputConnector ?? (_inputConnector = CreateInputConnector());
            }
            set { throw new NotImplementedException(); }
        }

        public bool DisableInputs { get; set; }
        public bool DisableOutputs { get; set; }
        protected virtual ConnectorViewModel CreateInputConnector()
        {
            if (DisableInputs) return null;
            return new ConnectorViewModel()
            {
                DataObject = DataObject,
                Direction = ConnectorDirection.Input,
                ConnectorFor = this,
                DiagramViewModel = DiagramViewModel,
                ConnectorForType = InputConnectorType ?? DataObject.GetType(),
                Side = ConnectorSide.Left,
                SidePercentage = 0.5f,
                //Validator = InputValidator
            };
        }

        private ConnectorViewModel _outputConnector;
        private Rect _bounds;
        private Rect _connectorBounds;
        private Func<IDiagramNodeItem, IDiagramNodeItem, bool> _inputValidator;
        private Func<IDiagramNodeItem, IDiagramNodeItem, bool> _outputValidator;
        private bool _isMouseOver;
        private bool _showHelp;
        private bool _isDirty;

        public virtual ConnectorViewModel OutputConnector
        {
            get
            {
                if (DataObject == null) return null;
                var connectable = DataObject as IConnectable;
                if (connectable != null && !connectable.AllowOutputs)
                    return null;
                return _outputConnector ?? (_outputConnector = CreateOutputConnector());
            }
            set { throw new NotImplementedException(); }
        }

        protected virtual ConnectorViewModel CreateOutputConnector()
        {
            if (DisableOutputs) return null;
            return new ConnectorViewModel()
            {
                DataObject = DataObject,
                Direction = ConnectorDirection.Output,
                ConnectorFor = this,
                DiagramViewModel = DiagramViewModel,
                ConnectorForType = OutputConnectorType ?? DataObject.GetType(),
                Side = ConnectorSide.Right,
                SidePercentage = 0.5f,
                //Validator = OutputValidator
            };
        }

        /// <summary>
        /// This is the bounds used to calculate the position of connectors.  Since it is a struct
        /// setting Bounds automatically sets this value.  If you need a custom Connector Bounds position
        /// you'll need to set this after Bounds is set.
        /// </summary>
        public Rect ConnectorBounds
        {
            get
            {
                if (_connectorBounds.width < 1f)
                {
                    return Bounds;
                }
                return _connectorBounds;
            }
            set { _connectorBounds = value; }
        }

        public bool ShowHelp
        {
            get
            {
                // TODO return _ShowHelp instead
                return IsScreenshot ||  _showHelp;
            }
            set { _showHelp = value; }
        }

        public bool IsScreenshot { get; set; }
        public int ColumnSpan { get; set; }
        public int Row { get; set; }

        
        public virtual void GetConnectors(List<ConnectorViewModel> list)
        {
            if (InputConnector != null)
                list.Add(InputConnector);
            if (OutputConnector != null)
                list.Add(OutputConnector);
            
            foreach (var item in ContentItems)
            {
                item.GetConnectors(list);
            }
        }
    }
}