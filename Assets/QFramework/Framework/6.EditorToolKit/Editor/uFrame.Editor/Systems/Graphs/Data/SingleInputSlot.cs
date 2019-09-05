using System.ComponentModel;
using System.Linq;

namespace QF.GraphDesigner
{
    public class SingleInputSlot<TFor> : GenericSlot
    {
        public override bool AllowMultipleInputs
        {
            get { return false; }
        }

        public override bool AllowMultipleOutputs
        {
            get { return false; }
        }
        [Browsable(false)]
        public TFor Item
        {
            get { return Inputs.Select(p => p.Output).OfType<TFor>().FirstOrDefault(); }
        }
        public override bool Validate(IDiagramNodeItem a, IDiagramNodeItem b)
        {
            if (a.Node == b.Node) return false;
            return base.Validate(a, b);
        }

    }
}