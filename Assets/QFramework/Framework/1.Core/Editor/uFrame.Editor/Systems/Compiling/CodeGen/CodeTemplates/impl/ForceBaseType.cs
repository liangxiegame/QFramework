using System;
using System.Reflection;

namespace QF.GraphDesigner
{
    /// <summary>
    /// This is only required when for some reason you can't create a template using the actual base class you want
    /// </summary>
    public class ForceBaseType : TemplateAttribute
    {
        public ForceBaseType(Type type)
        {
            Type = type;
        }

        public ForceBaseType(string type)
        {
            StringType = type;
        }

        public Type Type { get; set; }
        public string StringType { get; set; }


        public override void Modify(object templateInstance, MemberInfo info, TemplateContext ctx)
        {
            base.Modify(templateInstance, info, ctx);
            if (ctx.IsDesignerFile)
            {
                if (Type != null)
                    ctx.SetBaseType(Type);
                else
                {
                    ctx.SetBaseType(StringType);
                }
            }
        }
    }
}