using System;
using System.Reflection;

namespace QFramework.GraphDesigner
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AsPartial : TemplateAttribute
    {
        public override void Modify(object templateInstance, MemberInfo info, TemplateContext ctx)
        {
            base.Modify(templateInstance, info, ctx);
            ctx.CurrentDeclaration.IsPartial = true;
        }
    }
}