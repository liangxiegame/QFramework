using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace QF.GraphDesigner
{
    public class MultiInputSlot<TFor> : GenericSlot, IMultiSlot
    {
        public override bool AllowMultipleInputs
        {
            get { return true; }
        }

        public override bool AllowMultipleOutputs
        {
            get { return false; }
        }

        [Browsable(false)]
        public IEnumerable<TFor> Items
        {
            get { return Inputs.Select(p=>p.Output).OfType<TFor>(); }
        }
        public override bool Validate(IDiagramNodeItem a, IDiagramNodeItem b)
        {
            if (a.Node == b.Node) return false;
            return base.Validate(a, b);
        }
    }
}