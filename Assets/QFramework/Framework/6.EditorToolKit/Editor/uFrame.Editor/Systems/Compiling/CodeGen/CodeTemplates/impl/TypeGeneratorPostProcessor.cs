using System;
using System.CodeDom;

namespace QF.GraphDesigner
{
    
    public abstract class TypeGeneratorPostProcessor<TCodeGenerator> : ITypeGeneratorPostProcessor where TCodeGenerator : CodeGenerator
    {
        public Type For
        {
            get { return typeof(TCodeGenerator); }
        }

        public CodeGenerator Generator { get; set; }

        public CodeTypeDeclaration Declaration { get; set; }

        public TCodeGenerator CodeGenerator
        {
            get { return Generator as TCodeGenerator; }
        }

        public bool IsDesignerFile
        {
            get { return CodeGenerator.IsDesignerFile; }
        }

        public string Filename
        {
            get { return CodeGenerator.Filename; }
        }

        public CodeNamespace Namespace
        {
            get { return CodeGenerator.Namespace; }
        }
        public CodeCompileUnit Unit
        {
            get { return CodeGenerator.Unit; }
        }
        public object ObjectData
        {
            get { return CodeGenerator.ObjectData; }
        }
        public Type RelatedType
        {
            get { return CodeGenerator.RelatedType; }
        }

        public abstract void Apply();
    }
}