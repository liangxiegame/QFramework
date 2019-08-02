using System;
using System.CodeDom;
using System.Reflection;

namespace QF.GraphDesigner
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AsStatic : TemplateAttribute
    {
        public override void Modify(object templateInstance, MemberInfo info, TemplateContext ctx)
        {
            base.Modify(templateInstance, info, ctx);
            if (ctx.CurrentMember != null)
            {
                ctx.CurrentMember.Attributes |= MemberAttributes.Static;
            }
            else
            {
                ctx.CurrentDeclaration.Attributes |= MemberAttributes.Static;
            }
            
        }
    }
}