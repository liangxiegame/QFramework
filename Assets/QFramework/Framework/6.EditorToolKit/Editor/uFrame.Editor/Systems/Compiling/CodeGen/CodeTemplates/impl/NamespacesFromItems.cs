using System;
using System.Linq;
using System.Reflection;

namespace QF.GraphDesigner
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class NamespacesFromItems : TemplateAttribute
    {
        public override int Priority
        {
            get { return -3; }
        }

  

        public string Namespace { get; set; }
        public override void Modify(object templateInstance, MemberInfo info, TemplateContext ctx)
        {
            base.Modify(templateInstance, info, ctx);
            foreach (var property in ((IDiagramNode) ctx.DataObject).PersistedItems.OfType<GenericTypedChildItem>())
            {
                var type = property.Type;
                if (type == null) continue;

                ctx.TryAddNamespace(type.Namespace);
            }
        }
    }
}