using System;
using System.Collections.Generic;

namespace QF.GraphDesigner
{
    public class NodeConfigSection<TNode> : NodeConfigSectionBase where TNode : GenericNode
    {
       

        public Func<TNode, IEnumerable<IGraphItem>> Selector
        {
            get
            {
                if (GenericSelector == null) return null;
                return p => GenericSelector(p);
            }
            set { GenericSelector = p => value(p as TNode); }
        }

      //  public bool HasPredefinedOptions { get; set; }

    }
}