using System.CodeDom;

namespace QF.GraphDesigner
{
    public interface ITypedItem : IDiagramNodeItem
    {
        string RelatedType { get; set; }
        string RelatedTypeName { get; }
        //CodeTypeReference GetFieldType();
        //CodeTypeReference GetPropertyType();
        void RemoveType();
    }
}