using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace QF.GraphDesigner
{
    public class MultiOutputSlot<TFor> : GenericSlot, IMultiSlot
    {
        public override bool AllowMultipleInputs
        {
            get { return false; }
        }

        public override bool AllowMultipleOutputs
        {
            get { return true; }
        }
        [Browsable(false)]
        public IEnumerable<TFor> Items
        {
            get { return Outputs.Select(p => p.Input).OfType<TFor>(); }
        }

        public override bool Validate(IDiagramNodeItem a, IDiagramNodeItem b)
        {
            if (a.Node == b.Node) return false;
            return base.Validate(a, b);
        }
    }
}