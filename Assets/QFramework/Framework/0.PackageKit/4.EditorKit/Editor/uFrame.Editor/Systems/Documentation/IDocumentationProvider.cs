using System.Collections.Generic;

namespace QFramework.GraphDesigner
{
    public interface IDocumentationProvider
    {
        void GetDocumentation(IDocumentationBuilder node);
        void GetPages(List<DocumentationPage> rootPages);

    }
}