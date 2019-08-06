using System;
using System.CodeDom;

namespace QF.GraphDesigner
{
    [AttributeUsage(AttributeTargets.Property)]
    public class WithLazyField : WithField
    {
        public WithLazyField()
        {
        }

        public WithLazyField(Type fieldType, bool readOnly = false) : base(fieldType)
        {
            ReadOnly = readOnly;
        }

        public WithLazyField(Type fieldType, params Type[] customAttributes) : base(fieldType, customAttributes)
        {
        }

        public bool ReadOnly { get; set; }
        protected override void Apply(TemplateContext ctx)
        {
            //base.Apply(ctx);
            
            ctx.CurrentProperty.GetStatements._if("{0} == null", Field.Name).TrueStatements
                .Add(new CodeAssignStatement(new CodeSnippetExpression(string.Format("{0}", Field.Name)), DefaultExpression == null ? (CodeExpression) new CodeObjectCreateExpression(Field.Type) : new CodeSnippetExpression(DefaultExpression)) );
            ctx.CurrentProperty.GetStatements._("return {0}", Field.Name);
            if (!ReadOnly)
                ctx.CurrentProperty.SetStatements._("{0} = value", Field.Name);
          
        }
        
    }
}