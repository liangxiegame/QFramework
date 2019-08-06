using System;
using System.CodeDom;
using System.Reflection;
using QF;

namespace QF.GraphDesigner
{
    [AttributeUsage(AttributeTargets.Class)]
    public class WithMetaInfo : TemplateAttribute
    {
        public override int Priority
        {
            get { return -3; }
        }

        public override void Modify(object templateInstance, MemberInfo info, TemplateContext ctx)
        {
            base.Modify(templateInstance, info, ctx);
            ctx.CurrentDeclaration.CustomAttributes.Add(new CodeAttributeDeclaration(
                new CodeTypeReference(typeof(uFrameIdentifier)),
                new CodeAttributeArgument(new CodePrimitiveExpression(ctx.DataObject.Identifier))
                ));
        }
    }
}