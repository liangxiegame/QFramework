using System.CodeDom;
using System.Reflection;

namespace QF.GraphDesigner
{
    public class TemplateMemberResult
    {
        public CodeTypeDeclaration Decleration { get; private set; }

        public TemplateMemberResult(ITemplateClassGenerator templateClass, MemberInfo memberInfo, GenerateMember memberAttribute, CodeTypeMember memberOutput, CodeTypeDeclaration decleration)
        {
            Decleration = decleration;
            TemplateClass = templateClass;
            MemberInfo = memberInfo;
            MemberAttribute = memberAttribute;
            MemberOutput = memberOutput;
        }

        public ITemplateClassGenerator TemplateClass { get; set; }
        public MemberInfo MemberInfo { get; set; }
        public GenerateMember MemberAttribute { get; set; }
        public CodeTypeMember MemberOutput { get; set; }
    }
}