using System;
using System.CodeDom;
using System.Collections.Generic;

namespace QF.GraphDesigner
{
    public interface ITemplateClassGenerator
    {
        CodeNamespace Namespace { get; }
        Predicate<IDiagramNodeItem> ItemFilter { get; set; }
        CodeCompileUnit Unit { get; }
        
        Type TemplateType { get; }
        
        Type GeneratorFor { get; }
        
        TemplateContext Context { get; }
        
        string Filename { get; }
        
        List<TemplateMemberResult> Results { get; set; }

        void Initialize(CodeFileGenerator codeFileGenerator);

        IClassTemplate Template { get; } 
    }
}