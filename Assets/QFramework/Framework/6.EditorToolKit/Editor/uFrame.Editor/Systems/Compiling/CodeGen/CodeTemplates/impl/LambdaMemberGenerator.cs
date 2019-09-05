using System;
using System.CodeDom;

namespace QF.GraphDesigner
{
    public class LambdaMemberGenerator<TData> : MemberGenerator<TData>
    {
        public LambdaMemberGenerator(Func<LambdaMemberGenerator<TData>, CodeTypeMember> lambda)
        {
            Lambda = lambda;
        }

        public Func<LambdaMemberGenerator<TData>, CodeTypeMember> Lambda { get; set; }
        public bool IsDesignerFile { get; set; }
        public TData Data { get; set; }
        public CodeTypeDeclaration Decleration;
        public override CodeTypeMember Create(CodeTypeDeclaration decleration, TData data, bool isDesignerFile)
        {
            IsDesignerFile = isDesignerFile;
            Data = data;
            Decleration = decleration;
            return Lambda(this);
        }
    }

   
}