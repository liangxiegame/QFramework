using System.CodeDom;
using System.Reflection;

namespace QF.GraphDesigner
{
    public class GenerateMethod : GenerateMember
    {
        private bool _callBase = true;

        public GenerateMethod(TemplateLocation location)
            : base(location)
        {
        }

        public GenerateMethod()
            : base()
        {
        }

        public GenerateMethod(TemplateLocation location, bool callBase)
            : base(location)
        {
            CallBase = callBase;
        }

        public GenerateMethod(string nameFormat, TemplateLocation location, bool callBase)
            : base(location)
        {
            _callBase = callBase;
            NameFormat = nameFormat;
        }

        public bool CallBase
        {
            get { return _callBase; }
            set { _callBase = value; }
        }


        public override void Modify(object templateInstance, MemberInfo info, TemplateContext ctx)
        {
            base.Modify(templateInstance, info, ctx);
            var methodInfo = info as MethodInfo;
            var t = ctx.ProcessType(methodInfo.ReturnType);
            if (t != null)
            {
                ctx.CurrentMethod.ReturnType = new CodeTypeReference(t);
            }
            var prms = methodInfo.GetParameters();
            for (int index = 0; index < prms.Length; index++)
            {
                var parameter = prms[index];
                var templateParameter = ctx.CurrentMethod.Parameters[index];
                var x = ctx.ProcessType(parameter.ParameterType);
                if (x != null)
                {
                    templateParameter.Type = new CodeTypeReference(x);
                }

            }
            var isOverried = false;
            if (!ctx.IsDesignerFile && ctx.CurrentMember.Attributes != MemberAttributes.Final && ctx.CurrentAttribute.Location == TemplateLocation.Both)
            {
                ctx.CurrentMethod.Attributes |= MemberAttributes.Override;
                isOverried = true;
            }
            if ((methodInfo.IsVirtual && !ctx.IsDesignerFile) || (methodInfo.IsOverride() && !methodInfo.GetBaseDefinition().IsAbstract && ctx.IsDesignerFile))
            {
                if ((ctx.CurrentAttribute as GenerateMethod).CallBase)
                {
                    //if (!info.IsOverride() || !info.GetBaseDefinition().IsAbstract && IsDesignerFile)
                    //{ 
                    ctx.CurrentMethod.invoke_base(true);
                    //}

                }
            }

        }
    }
}