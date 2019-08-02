using System;

namespace QF.GraphDesigner
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NodeProperty : InspectorProperty
    {
        public NodeProperty()
        {
        }

        public NodeProperty(InspectorType inspectorType)
            : base(inspectorType)
        {
        }

        public NodeProperty(string tip) : base(tip)
        {
        }
    }
}