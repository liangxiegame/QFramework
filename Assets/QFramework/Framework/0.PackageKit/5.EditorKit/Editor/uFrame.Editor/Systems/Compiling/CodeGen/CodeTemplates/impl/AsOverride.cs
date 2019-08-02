using System;
using System.CodeDom;
using System.Reflection;

namespace QF.GraphDesigner
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class AsOverride : TemplateAttribute
    {
        public override int Priority
        {
            get { return 5; }
        }

        public override void Modify(object templateInstance, MemberInfo info, TemplateContext ctx)
        {
            base.Modify(templateInstance, info, ctx);
            if (ctx.CurrentMember != null)
            {
                ctx.CurrentMember.Attributes |= MemberAttributes.Override;
            }
        }
    }
}