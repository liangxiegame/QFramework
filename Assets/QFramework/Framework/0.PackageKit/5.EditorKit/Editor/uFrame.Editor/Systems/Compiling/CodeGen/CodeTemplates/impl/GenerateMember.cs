using System.Reflection;
using System.Text.RegularExpressions;

namespace QF.GraphDesigner
{
    public class GenerateMember : TemplateAttribute
    {
        private string _nameFormat;

        public string NameFormat
        {
            get { return _nameFormat; }
            set { _nameFormat = value; }
        }

        public TemplateLocation Location { get; set; }
        public GenerateMember()
        {
        }


        public GenerateMember(TemplateLocation location)
        {
            Location = location;
        }

        public override void Modify(object templateInstance, MemberInfo info, TemplateContext ctx)
        {
            base.Modify(templateInstance, info, ctx);
            string strRegex = @"_(?<name>[a-zA-Z0-9]+)_";
            bool replaced = false;
            var newName = Regex.Replace(info.Name, strRegex, _ =>
            {
                var name = _.Groups["name"].Value;
                try
                {
                    replaced = true;
                    return (string) ctx.GetTemplateProperty(templateInstance, name);
                }
                catch (TemplateException ex)
                {
                    return ctx.Item.Name;
                }
                
            });

            if (!replaced && NameFormat != null)
            {
                ctx.CurrentMember.Name = string.Format(NameFormat, ctx.Item.Name.Clean());
            }
            else
            {
                ctx.CurrentMember.Name = newName.Clean();
            }
            
        }
    }
}