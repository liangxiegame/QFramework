using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using QF;

namespace QF.GraphDesigner
{
    public class GenericNodeChildItem : DiagramNodeItem
    {

        public override string FullLabel
        {
            get { return Name; }
        }

        public override string Label
        {
            get { return Name; }
        }

        public override void Remove(IDiagramNode diagramNode)
        {

        }

        public override void OnConnectedToInput(IConnectable input)
        {
            base.OnConnectedToInput(input);
            
        }
    }
}