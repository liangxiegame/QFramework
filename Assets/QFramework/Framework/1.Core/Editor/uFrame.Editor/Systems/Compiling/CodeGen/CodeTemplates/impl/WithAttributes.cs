using System;
using System.CodeDom;
using System.Reflection;

namespace QF.GraphDesigner
{
    public class WithAttributes : TemplateAttribute
    {
        public WithAttributes(params Type[] attributes)
        {
            Attributes = attributes;
        }

        public Type[] Attributes { get; set; }
        public override void Modify(object templateInstance, MemberInfo info, TemplateContext ctx)
        {
            base.Modify(templateInstance, info, ctx);
            foreach (var attribute in Attributes)
            {
                ctx.CurrentMember.CustomAttributes.Add(
                    new CodeAttributeDeclaration(new CodeTypeReference(attribute)));
            }
        }
    }
}