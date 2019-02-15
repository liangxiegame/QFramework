using System;
using System.Reflection;
using QFramework;

namespace QFramework.GraphDesigner
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple = true)]
    public class RequiresNamespace : TemplateAttribute
    {
        public override int Priority
        {
            get { return -3; }
        }

        public RequiresNamespace(string ns)
        {
            Namespace = ns;
        }

        public string Namespace { get; set; }
        public override void Modify(object templateInstance, MemberInfo info, TemplateContext ctx)
        {
            base.Modify(templateInstance, info, ctx);
            ctx.TryAddNamespace(Namespace);
        }
    }
}