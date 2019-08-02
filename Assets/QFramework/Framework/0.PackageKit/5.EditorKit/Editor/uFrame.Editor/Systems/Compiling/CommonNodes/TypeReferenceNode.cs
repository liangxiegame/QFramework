using System;
using System.Collections.Generic;
using QF.Json;
using QF.GraphDesigner;
using QF;

namespace QF.GraphDesigner
{
    public class TypeReferenceNode : GenericNode, IClassTypeNode, ITypedItem
    {
        
        private string _name1;
        private Type _type;

        public override string InputDescription
        {
            get { return "Plug in any output which requires a Type.\n(For example Property)"; }
        }

        public override string FullName
        {
            get { return Name; }
            set { Name = value; }
        }

        [JsonProperty]
        public override string Name
        {
            get { return _name1 ; }
            set
            {
                _name1 = value;
            }
        }

        public override IEnumerable<IDiagramNodeItem> PersistedItems
        {
            get { yield break; }
            set { base.PersistedItems = value; }
        }

        public string ClassName
        {
            get { return Name; }
            set { Name = value; }
        }

        public Type Type
        {
            get { return _type ?? (_type = InvertApplication.FindTypeByName(Name) ?? InvertApplication.FindRuntimeType(Name)); }
            set { _type = value; }
        }

        public override IEnumerable<IMemberInfo> GetMembers()
        {
            foreach (var item in new SystemTypeInfo(Type).GetMembers())
            {
                yield return item;
            }
        }

        public override void Validate(List<ErrorInfo> errors)
        {
            base.Validate(errors);
           
        }
    }
}