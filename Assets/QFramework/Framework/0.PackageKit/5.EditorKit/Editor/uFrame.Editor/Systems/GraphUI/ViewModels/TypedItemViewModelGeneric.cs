using QF.GraphDesigner;

namespace QF.GraphDesigner
{ 
    public abstract class TypedItemViewModel<TElementItem> : TypedItemViewModel
    {
        public TElementItem ElementItem
        {
            get { return (TElementItem)DataObject; }
        }

        protected TypedItemViewModel(ITypedItem viewModelItem, DiagramNodeViewModel nodeViewModel)
            : base(viewModelItem, nodeViewModel)
        {

        }

        public override string RelatedType
        {
            get
            {
                if (Data.RelatedType == "ENTITY")
                    return "ENTITY";
                return base.RelatedType;
            }
            set { base.RelatedType = value; }
        }

        public override bool IsEditable
        {
            get { return base.IsEditable; }
        }

        public override ConnectorViewModel InputConnector
        {
            get { return null; }
        }
    }
}