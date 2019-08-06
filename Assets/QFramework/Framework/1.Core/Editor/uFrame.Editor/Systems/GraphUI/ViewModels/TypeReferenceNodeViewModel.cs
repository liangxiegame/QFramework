using System.Collections.Generic;

namespace QF.GraphDesigner
{
    public class TypeReferenceNodeViewModel : DiagramNodeViewModel<TypeReferenceNode>
    {
        public TypeReferenceNodeViewModel(TypeReferenceNode graphItemObject, DiagramViewModel diagramViewModel) : base(graphItemObject, diagramViewModel)
        {
        }

        public override INodeStyleSchema StyleSchema
        {
            get { return CachedStyles.NodeStyleSchemaMinimalistic; }
        }

        public override IEnumerable<string> Tags
        {
            get { yield return "Type Reference"; }
        }

        //public override bool IsEditable
        //{
        //    get { return false; }
        //}

        public override void DataObjectChanged()
        {
            base.DataObjectChanged();

        }
    }
}