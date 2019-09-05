using System;
using System.Collections;
using System.Reflection;

namespace QF.GraphDesigner
{
    public class ForEach : TemplateAttribute
    {
        public override int Priority
        {
            get { return -1; }
        }
        public ForEach(string iteratorMemberName)
        {
            IteratorMemberName = iteratorMemberName;
        }

        public string IteratorMemberName { get; set; }
        
        public override bool CanGenerate(object templateInstance, MemberInfo info, TemplateContext ctx)
        {
            ctx.AddMemberIterator(info.Name, _ => ctx.GetTemplateProperty(templateInstance, IteratorMemberName) as IEnumerable);
            //CreateIterator(templateInstance, IteratorMemberName, _));
            return true;
        }
        private static IEnumerable CreateIterator(object instance, string iteratorName, object arg1)
        {
            

            var property = instance.GetType().GetProperty(iteratorName);
            if (property == null && arg1 != null)
            {
                property = arg1.GetType().GetProperty(iteratorName);
                return property.GetValue(arg1, null) as IEnumerable;
            }
            if (property != null)
            {
                return property.GetValue(instance, null) as IEnumerable;
            }

            throw new Exception(string.Format("ForEach on property '{0}' could not be found on the template, or the node.",
                iteratorName));
        }
    }
}