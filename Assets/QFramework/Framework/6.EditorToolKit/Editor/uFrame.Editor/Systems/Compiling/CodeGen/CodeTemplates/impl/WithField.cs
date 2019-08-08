using System;
using System.CodeDom;
using System.Linq;
using System.Reflection;

namespace QF.GraphDesigner
{
    [AttributeUsage(AttributeTargets.Property)]
    public class WithField : TemplateAttribute
    {
        private readonly Type[] _customAttributes;

        public override int Priority
        {
            get { return 2; }
        }

        public WithField()
        {
        }
        public bool ManualSetter { get; set; }

        public WithField(string defaultExpression)
        {
            DefaultExpression = defaultExpression;
        }

        public WithField(Type fieldType, params Type[] customAttributes)
        {
            _customAttributes = customAttributes;
            FieldType = fieldType;
        }

        public Type FieldType { get; set; }
        public string DefaultExpression { get; set; }
        
        public sealed override void Modify(object templateInstance, MemberInfo info, TemplateContext ctx)
        {
            base.Modify(templateInstance, info, ctx);
            CreateField(ctx);
            Apply(ctx);
        }

        protected virtual void Apply(TemplateContext ctx)
        {
            ctx.CurrentProperty.GetStatements._("return {0}", Field.Name);
            if (!ManualSetter)
                ctx.CurrentProperty.SetStatements._("{0} = value", Field.Name);
            if (DefaultExpression != null)
                Field.InitExpression = new CodeSnippetExpression(DefaultExpression);
        }

        private void CreateField(TemplateContext ctx)
        {
            if (FieldType != null)
            {
                Field = ctx.CurrentDeclaration._private_(ctx.ProcessType(FieldType), "m_{0}", ctx.CurrentProperty.Name.Clean());
            }
            else
            {
                Field = ctx.CurrentDeclaration._private_(ctx.CurrentProperty.Type, "m_{0}", ctx.CurrentProperty.Name.Clean());
            }
            if (_customAttributes != null)
                Field.CustomAttributes.AddRange(_customAttributes.Select(p=>new CodeAttributeDeclaration(new CodeTypeReference(p))).ToArray());
        }

        public CodeMemberField Field { get; set; }
    }
}