using System;
using System.Collections.Generic;
using System.Linq;
using QF.GraphDesigner;

namespace QF.GraphDesigner
{
    public class DocumentationDefaultProvider : IDocumentationProvider
    {
        private WorkspaceService _projectService;

        public WorkspaceService WorkspaceService
        {
            get { return _projectService ?? (_projectService = InvertApplication.Container.Resolve<WorkspaceService>()); }
            set { _projectService = value; }
        }

        public virtual Type RootPageType
        {
            get { return null; }
        }

        public virtual void GetPages(List<DocumentationPage> rootPages)
        {
            //var pages = InvertApplication.GetDerivedTypes<DocumentationPage>(false, false)
            //    .Where(p=>RootPageType == null || RootPageType.IsAssignableFrom(p))
            //    .Where(p => p.Name != "DocumentationPageTemplate")
            //    .Select(Activator.CreateInstance).OfType<DocumentationPage>().ToArray();

            //GetValue(pages, rootPages, null);
        }

        private static void GetValue(DocumentationPage[] allPages, List<DocumentationPage> pages, DocumentationPage parentPage)
        {
            foreach (var page in allPages)
            {
                //  foreach (var page in allPages
//                .Where(p => p.ParentPage == (parentPage == null ? null : parentPage.GetType())))
          
                if (parentPage != null && page.ParentPage != null && page.ParentPage.IsAssignableFrom(parentPage.GetType()))
                {
                    pages.Add(page);
                    GetValue(allPages, page.ChildPages, page);
                }
                else if (parentPage == null && page.ParentPage == null)
                {
                    pages.Add(page);
                    GetValue(allPages, page.ChildPages, page);
                }
            }
        }


        public virtual void GetDocumentation(IDocumentationBuilder node)
        {

        }
    }
}