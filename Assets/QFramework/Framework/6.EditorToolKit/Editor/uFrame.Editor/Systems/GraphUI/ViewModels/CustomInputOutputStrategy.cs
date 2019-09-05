using UnityEngine;

namespace QF.GraphDesigner
{
    public class CustomInputOutputStrategy<TOutput, TInput> : DefaultConnectionStrategy<TOutput, TInput>
        where TOutput : IConnectable
        where TInput : IConnectable
    {
        public NodeConfigSectionBase Configuration { get; set; }

        private Color _connectionColor = Color.white;

        public CustomInputOutputStrategy(Color connectionColor)
        {
            _connectionColor = connectionColor;
        }

        protected override bool CanConnect(TOutput output, TInput input)
        {
            if (output.CanOutputTo(input) && input.CanInputFrom(output))
            {
                return base.CanConnect(output, input);
            }
            return false;
        }

        public override Color ConnectionColor
        {
            get { return _connectionColor; }
        }

        public override void Remove(ConnectorViewModel output, ConnectorViewModel input)
        {
            
            InvertGraphEditor.DesignerWindow.DiagramViewModel.GraphData.RemoveConnection(output.DataObject as IConnectable, input.DataObject as IConnectable);
        }
    }
}