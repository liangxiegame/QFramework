using System;
using Invert.uFrame.Editor.ViewModels;
using UnityEngine;

namespace QF.GraphDesigner
{
    public class ConnectorViewModel : GraphItemViewModel
    {
        private IConnectorStyleSchema _styleSchema;

        public ConnectorViewModel()
        {
            //Strategy = strategy;
        }

        public Color Color
        {
            get
            {
                return Data.Color;
            }
        }

        public IConnectable Data
        {
            get { return DataObject as IConnectable; }
        }

        private string _inputDescription;
        private string _outputDescription;

        public string InputDesctiption
        {
            get {
                if (_inputDescription == null && Data.InputDescription != null)
                {
                        _inputDescription = Data.InputDescription;
                } return _inputDescription;
            }
        }

        public string OutputDesctiption
        {
            get
            {
                if (_outputDescription == null && Data.OutputDescription != null)
                {
                        _outputDescription = Data.OutputDescription;
                } return _outputDescription;
            }
        }

        //public IConnectionStrategy Strategy { get; set; }

        public override Vector2 Position { get; set; }

        public override string Name
        {
            get
            {
                if (ConnectorFor == null) return "ConnetorFor is null";
                var item = ConnectorFor.DataObject as IDiagramNodeItem;
                if (item != null && item.Node != item)
                {
                    return string.Format("{0}:{1}", item.Node.Name, item.Name);
                }
                return  ConnectorFor.Name;
            }
            set { }
        }

        public virtual Color TintColor { get; set; }
        public virtual ConnectorStyle Style { get; set; }

        public virtual IConnectorStyleSchema StyleSchema
        {
            get
            {
                if (_styleSchema == null)
                {
                    switch (Style)
                    {
                        case ConnectorStyle.Triangle:
                            return CachedStyles.ConnectorStyleSchemaTriangle;
                        case ConnectorStyle.Circle:
                            return CachedStyles.ConnectorStyleSchemaCircle;
                        default:
                            return CachedStyles.ConnectorStyleSchemaTriangle;
                    }
                }
                return _styleSchema ;
            }
            set { _styleSchema = value; }
        }

        public Action<ConnectionViewModel> ApplyConnection { get; set; }

        public ConnectorDirection Direction { get; set; }

        public GraphItemViewModel ConnectorFor { get; set; }

//        public Func<IDiagramNodeItem, IDiagramNodeItem, bool> Validator { get; set; }

        public NodeInputConfig Configuration { get; set; }

        public ConnectorSide Side { get; set; }  

        /// <summary>
        /// A percentage value from 0-1f on which to calculate the position
        /// </summary>
        public float SidePercentage { get; set; }

        public bool HasConnections { get; set; }
        public Type ConnectorForType { get; set; }

        public bool AlwaysVisible { get; set; }
        public bool Disabled { get; set; }
    }
}