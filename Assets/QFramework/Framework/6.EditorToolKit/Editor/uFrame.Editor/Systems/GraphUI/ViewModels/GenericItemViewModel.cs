using System.Linq;
using Invert.uFrame.Editor.ViewModels;

namespace QF.GraphDesigner
{
    public class GenericItemViewModel<TData> : ItemViewModel<TData> where TData : IDiagramNodeItem
    {
        public GenericItemViewModel(TData data, DiagramNodeViewModel nodeViewModel) : base(data, nodeViewModel)
        {
        }

        public GenericItemViewModel(DiagramNodeViewModel nodeViewModel) : base(nodeViewModel)
        {
        }

        public override string Name
        {
            get { return Data.Name; }
            set { Data.Name = value; }
        }


        public override ConnectorViewModel InputConnector
        {
            get
            {
                //if (SectionConfig.IsProxy) return null;
                return base.InputConnector;
            }
        }

        public override ConnectorViewModel OutputConnector
        {
            get
            {
               // if (SectionConfig.IsProxy) return null;
                return base.OutputConnector;
            }
        }
        
    }
}