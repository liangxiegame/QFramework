using System;
using System.CodeDom;

namespace QFramework.GraphDesigner
{
    public interface ITypeGeneratorPostProcessor
    {
        Type For { get; }
        CodeGenerator Generator { get; set; }
        CodeTypeDeclaration Declaration { get; set; }
        
        void Apply();
    }
}