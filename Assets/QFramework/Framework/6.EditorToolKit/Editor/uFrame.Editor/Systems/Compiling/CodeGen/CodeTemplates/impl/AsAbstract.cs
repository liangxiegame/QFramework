using System;
using System.CodeDom;
using System.Reflection;

namespace QF.GraphDesigner
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Class)]
    public class AsAbstract : TemplateAttribute
    {
        private TemplateLocation _location = TemplateLocation.DesignerFile;

        public TemplateLocation Location
        {
            get { return _location; }
            set { _location = value; }
        }

        public AsAbstract()
        {
        }

        public AsAbstract(TemplateLocation location)
        {
            _location = location;
        }

        public override void Modify(object templateInstance, MemberInfo info, TemplateContext ctx)
        {
            base.Modify(templateInstance, info, ctx);
            if (ctx.IsDesignerFile && _location != (TemplateLocation.Both | TemplateLocation.DesignerFile)) return;
            if (!ctx.IsDesignerFile && _location != (TemplateLocation.Both | TemplateLocation.EditableFile)) return;
            if (ctx.CurrentMember != null)
            {
                ctx.CurrentMember.Attributes |= MemberAttributes.Abstract;
            }
            else
            {
                ctx.CurrentDeclaration.Attributes |= MemberAttributes.Abstract;
            }
        }
    }
}