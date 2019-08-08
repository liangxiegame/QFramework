using System;
using System.Reflection;

namespace QF.GraphDesigner
{
    public class IfNot : TemplateAttribute
    {
        public override int Priority
        {
            get { return -2; }
        }

        public string ConditionMemberName { get; set; }

        public IfNot(string conditionMemberName)
        {
            ConditionMemberName = conditionMemberName;
        }

        public override bool CanGenerate(object templateInstance, MemberInfo info, TemplateContext ctx)
        {
            try
            {
                var condition = (bool)ctx.GetTemplateProperty(templateInstance, ConditionMemberName);
                return !condition;
            }
            catch (InvalidCastException ex)
            {
                throw new TemplateException(string.Format("Condition {0} is not a valid condition, make sure it is of type boolean.", ConditionMemberName), ex);
            }
        }
    }
}