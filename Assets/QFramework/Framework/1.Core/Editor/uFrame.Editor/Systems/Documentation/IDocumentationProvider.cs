using System.Collections.Generic;

namespace QF.GraphDesigner
{
    public interface IDocumentationProvider
    {
        void GetDocumentation(IDocumentationBuilder node);
        void GetPages(List<DocumentationPage> rootPages);

    }
}