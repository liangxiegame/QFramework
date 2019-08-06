using QF.GraphDesigner;
using Invert.uFrame.Editor.ViewModels;
using UnityEngine;

namespace QF.GraphDesigner
{
    public class GenericItemHeaderViewModel : GraphItemViewModel
    {
        public override bool IsNewLine
        {
            get { return true; }
            set { base.IsNewLine = value; }
        }

        public override bool Enabled
        {
            get { return NodeViewModel.Enabled; }
        }

        public bool AllowConnections { get; set; }
        public override ConnectorViewModel InputConnector
        {
            get
            {
                if (!AllowConnections) return null;
                return base.InputConnector;
            }
            set {  }
        }

        public override ConnectorViewModel OutputConnector
        {
            get
            {
                if (!AllowConnections) return null;
                return base.OutputConnector;
            }
            set { }
        }

        public override Vector2 Position { get; set; }
        public override string Name { get; set; }
        public GraphItemViewModel NodeViewModel { get; set; }
        public ICommand AddCommand { get; set; }
        public NodeConfigBase NodeConfig { get; set; }
        public NodeConfigSectionBase SectionConfig { get; set; }
        public bool IsBig { get; set; }

        public void Add()
        {
            InvertApplication.Execute(AddCommand);
        }
    }
}