using System;
using System.Collections.Generic;

namespace QF.GraphDesigner
{
    public class NodeConfigSectionBase : GraphItemConfiguration
    {
        public bool IsProxy { get; set; }
        private bool _allowAdding = true;
        private bool _isEditable = true;

        public string Name { get; set; }

        public bool AllowAdding
        {
            get { return _allowAdding; }
            set { _allowAdding = value; }
        }
        public bool AllowMultipleInputs { get; set; }
        public bool AllowMutlipleOutputs { get; set; }

        public bool IsEditable
        {
            get { return _isEditable; }
            set { _isEditable = value; }
        }

        public Func<GenericNode, IEnumerable<IGraphItem>> GenericSelector { get; set; }
       // public Type ReferenceType { get; set; }
        public bool AllowDuplicates { get; set; }

        public NodeConfigSectionBase()
        {
            InputValidator = (a,b) => true;
            OutputValidator = (a, b) => true;
        }

        public Func<IDiagramNodeItem, IDiagramNodeItem, bool> InputValidator { get; set; }
        public Func<IDiagramNodeItem, IDiagramNodeItem, bool> OutputValidator { get; set; }

        public bool HasPredefinedOptions { get; set; }
        public Action<IDiagramNodeItem> OnAdd { get; set; }
        public Type AddCommandType { get; set; }
    }
}