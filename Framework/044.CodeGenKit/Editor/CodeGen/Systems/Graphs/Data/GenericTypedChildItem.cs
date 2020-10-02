using System;
using System.Linq;
using QFramework.CodeGen;
using Invert.Data;

public class GenericTypedChildItem : ITypedItem
{
    protected string _type = string.Empty;

    public virtual Type Type
    {
        get
        {
            if (string.IsNullOrEmpty(RelatedType)) return null;

            return InvertApplication.FindType(RelatedType) ?? InvertApplication.FindTypeByName(RelatedType) ;
        }
    }

    public string RelatedType
    {
        get { return _type; }
        set
        {

            this.Changed("RelatedType", ref _type, value);
        }
    }

    public virtual string DefaultTypeName
    {
        get { return typeof(string).FullName; }
    }

    public virtual string RelatedTypeName
    {
        get
        {
            var outputClass = RelatedTypeNode;
            if (outputClass != null)
            {
                return outputClass.ClassName;
            }

            var relatedNode = this.RelatedNode();

            if (relatedNode != null)
                return relatedNode.Name;

            return string.IsNullOrEmpty(RelatedType) ? DefaultTypeName : Type.Name;
        }
    }

    public virtual IClassTypeNode RelatedTypeNode
    {
        get
        {
            return this.Repository.AllOf<IClassTypeNode>().FirstOrDefault(p => p.Identifier == RelatedType);
        }
    }


    public string Identifier { get; set; }
    public IRepository Repository { get; set; }
    public bool Changed { get; set; }
    public string Name { get; set; }
    public string Namespace { get; private set; }
    public string NodeId { get; set; }
    public void NodeRemoved(IDiagramNode nodeData)
    {
        
    }

    public int Order { get; set; }
}