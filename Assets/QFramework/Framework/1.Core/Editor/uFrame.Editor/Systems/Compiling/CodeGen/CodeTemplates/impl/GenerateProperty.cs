using System.CodeDom;
using System.Reflection;

namespace QF.GraphDesigner
{
    public class GenerateProperty : GenerateMember
    {


        public GenerateProperty(TemplateLocation location)
            : base(location)
        {
          
        }

        public GenerateProperty(string nameFormat)
        {
            NameFormat = nameFormat;
        }

        public GenerateProperty(TemplateLocation location, string nameFormat)
            : base(location)
        {
            NameFormat = nameFormat;
        }



        public GenerateProperty()
            : base()
        {
        }


        public override void Modify(object templateInstance, MemberInfo info, TemplateContext ctx)
        {
            base.Modify(templateInstance, info, ctx);
            var propertyInfo = info as PropertyInfo;
            var t = ctx.ProcessType(propertyInfo.PropertyType);
            if (t != null)
            {
                ctx.CurrentProperty.Type = new CodeTypeReference(t);
            }
        }
    }
}