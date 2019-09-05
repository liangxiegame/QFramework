using System.Reflection;

namespace QF.GraphDesigner
{
    public class WithNameFormat : TemplateAttribute
    {
        public override int Priority
        {
            get { return 1; }
        }

        public override void Modify(object templateInstance, MemberInfo info, TemplateContext ctx)
        {
            base.Modify(templateInstance, info, ctx);
            ctx.CurrentMember.Name = string.Format(Format, ctx.Item.Name.Clean());
        }

        public string Format { get; set; }

        public WithNameFormat(string format)
        {
            Format = format;
        }
    }
}