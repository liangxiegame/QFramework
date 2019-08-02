using System.CodeDom;

namespace QF.GraphDesigner
{
    public abstract class MemberGenerator<TData> : IMemberGenerator
    {
        protected MemberAttributes _attributes = MemberAttributes.Private;
        private CodeAttributeDeclarationCollection _customAttributes;

   
      
        public CodeTypeMember Create(CodeTypeDeclaration decleration, object data, bool isDesignerFile)
        {
            return Create(decleration, (TData) data, isDesignerFile);
        }

        public TemplateLocation MemberLocation { get; set; }
        
        public abstract CodeTypeMember Create(CodeTypeDeclaration decleration, TData data, bool isDesignerFile);

        //public TData Data
        //{
        //    get { return (TData) DataObject; }
        //    set { DataObject = value; }
        //}
        public MemberAttributes Attributes
        {
            get { return _attributes; }
            set { _attributes = value; }
        }

        public CodeAttributeDeclarationCollection CustomAttributes
        {
            get { return _customAttributes ?? (_customAttributes = new CodeAttributeDeclarationCollection()); }
            set { _customAttributes = value; }
        }

    }
}