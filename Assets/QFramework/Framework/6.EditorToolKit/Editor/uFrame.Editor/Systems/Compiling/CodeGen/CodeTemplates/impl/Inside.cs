using System.Reflection;

namespace QF.GraphDesigner
{
    public class Inside : TemplateAttribute
    {
        public TemplateLocation TemplateLocation { get; set; }

        public Inside(TemplateLocation location)
        {
            TemplateLocation = location;
        }

        public override bool CanGenerate(object templateInstance, MemberInfo info, TemplateContext ctx)
        {
            ctx.CurrentAttribute.Location = (TemplateLocation)((int)TemplateLocation);
            if (TemplateLocation == TemplateLocation.Both)
            {
                return true;
            }
            if (ctx.IsDesignerFile && TemplateLocation == TemplateLocation.DesignerFile)
            {
                return true;
            } 
            if(!ctx.IsDesignerFile && TemplateLocation == TemplateLocation.EditableFile)
            {
                return true;
            }
            return false;
        }
        
    }
}