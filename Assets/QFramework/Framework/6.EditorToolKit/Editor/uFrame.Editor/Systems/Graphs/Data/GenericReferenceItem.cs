using System.ComponentModel;
using Invert.Data;
using QF.Json;
using QF;

namespace QF.GraphDesigner
{
    public class GenericReferenceItem : GenericSlot, ITypedItem, IDataRecordRemoved
    {
        private string _sourceIdentifier;
        [Browsable(false)]
        public override string Label
        {
            get { return SourceItemObject.Name + ": " + base.Label; }
        }

        public override string Name
        {
            get
            {
                if (!string.IsNullOrEmpty(base.Name))
                {
                    return base.Name;
                }
                if (SourceItemObject == null)
                {
                    return "Missing";
                }
                return SourceItemObject.Name;
            }
            set { base.Name = value;
                
            }
        }
        [JsonProperty]
        public string SourceIdentifier
        {
            get { return _sourceIdentifier; }
            set {
                this.Changed("SourceIdentifier",ref _sourceIdentifier, value);
            }
        }
        [Browsable(false)]
        public virtual IDiagramNodeItem SourceItemObject
        {
            get
            {
                return Repository.GetById<IDiagramNodeItem>(SourceIdentifier);
            }
        }



        public string RelatedType
        {
            get
            {
                var source = SourceItemObject;
                if (source == null)
                {
                    return "Missing";
                }
                var classItem = source as IClassTypeNode;
                if (classItem != null)
                {
                    return classItem.ClassName;
                }
                return source.Name;
            }
            set
            {
                
            }
        }

        public string RelatedTypeName
        {
            get
            {
                return RelatedType;
            }
        }

        public void RemoveType()
        {
            Repository.Remove(this);
        }

        public override void RecordRemoved(IDataRecord record)
        {
            if (record.Identifier == this.SourceIdentifier)
            {
                Repository.Remove(this);
            }
        }
    }
}