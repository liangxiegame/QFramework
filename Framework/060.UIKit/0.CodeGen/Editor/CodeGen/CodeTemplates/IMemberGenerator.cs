using System.CodeDom;

namespace QFramework.CodeGen
{
    public interface IMemberGenerator
    {
        TemplateLocation MemberLocation { get; set; }
        
        CodeTypeMember Create(CodeTypeDeclaration decleration, object data, bool isDesignerFile);
    }
    public interface IMemberGenerator<in TData> : IMemberGenerator
    {


        CodeTypeMember Create(TData data, bool isDesignerFile);

    }
}