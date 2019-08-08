using System;

namespace QF.GraphDesigner
{
    public class NodeValidator<TNode>
    {
        public string Message { get; set; }
        public ValidatorType Type { get; set; }
        public Func<TNode, bool> Validate { get; set; }
    }
}