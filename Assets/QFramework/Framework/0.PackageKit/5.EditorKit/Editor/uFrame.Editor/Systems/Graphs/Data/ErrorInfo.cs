using System;
using QF.GraphDesigner;
using Invert.Data;

namespace QF.GraphDesigner
{
    public class ErrorInfo : IItem
    {
        protected bool Equals(ErrorInfo other)
        {
            return string.Equals(Identifier, other.Identifier) && string.Equals(Message, other.Message);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ErrorInfo) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Identifier != null ? Identifier.GetHashCode() : 0)*397) ^ (Message != null ? Message.GetHashCode() : 0);
            }
        }

        public IDataRecord Record { get; set; }
        public string Identifier { get; set; }
        public string Message { get; set; }
        public System.Action AutoFix { get; set; }
        public ValidatorType Siverity { get; set; }

        public GraphNode SourceNode
        {
            get
            {
                return Record as GraphNode;
            }
        }

        public string Title
        {
            get
            {
                //GraphNode
                //GenericSlot
                string name = null;

                if (SourceNode != null)
                {
                    var filter = SourceNode.Filter;
                    if (filter != null)
                    name = filter.Name ;
                }
                return name == null ? Message : string.Format("{0} : {1}",name,Message);
            }
        }

        public string Group { get; private set; }
        public string SearchTag { get; private set; }
        public string Description { get; set; }
    }
}