using System;
using System.Collections.Generic;
using Invert.Data;
using QF;

namespace QF.GraphDesigner
{
  
    public class GraphFilter : IGraphFilter
    {
        private Type[] _allowedTypes;

        private string _identifier;

        public IRepository Repository { get; set; }

        public string Identifier
        {
            get
            {
                if (string.IsNullOrEmpty(_identifier))
                {
                    _identifier = Guid.NewGuid().ToString();
                }
                return _identifier;
            }
            set { _identifier = value; }
        }

        public bool Changed { get; set; }

        public IEnumerable<string> ForeignKeys
        {
            get { yield break; }
        }

        public virtual bool ImportedOnly
        {
            get { return false; }
        }

        public bool IsExplorerCollapsed { get; set; }


        public virtual string Name
        {
            get { return "All"; }
            set { }
        }

        public virtual bool UseStraightLines
        {
            get { return false; }
        }

        public IEnumerable<IDiagramNode> FilterNodes
        {
            get { yield break; }
        }

        public IEnumerable<IFilterItem> FilterItems { get; private set; }

        public bool AllowExternalNodes
        {
            get { return true; }
        }

        public string InfoLabel { get; private set; }


        public virtual bool IsItemAllowed(object item, Type t)
        {
            return IsAllowed(item, t);
        }

        public virtual bool IsAllowed(object item, Type t)
        {
            return true;
        }

    }
}