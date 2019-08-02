using System;
using System.Text;

namespace QF.GraphDesigner.Unity
{
    public class MarkdownDocsBuilder : IDocumentationBuilder
    {
        public StringBuilder _ = new StringBuilder();
        public void BeginArea(string id)
        {
            
        }

        public void EndArea()
        {
            
        }

        public void BeginSection(string id)
        {
            
        }

        public void EndSection()
        {
            
        }

        public void PushIndent()
        {
            
        }

        public void PopIndent()
        {
            
        }

        public void LinkToNode(IDiagramNodeItem node, string text = null)
        {
            
        }

        public void NodeImage(GraphNode node)
        {
            
        }

        public void Paragraph(string text, params object[] args)
        {
            _.AppendLine(text).AppendLine();
        }

        public string EditableParagraph(string text)
        {
            return null;
        }

        public void Break()
        {
            _.AppendLine().AppendLine();
        }

        public void Lines(params string[] lines)
        {
            
        }

        public void Title(string text, params object[] args)
        {
            _.AppendFormat("#{0}", string.Format(text, args));
        }

        public void Title2(string text, params object[] args)
        {
            _.AppendFormat("##{0}", string.Format(text, args));
        }

        public void Title3(string text, params object[] args)
        {
            _.AppendFormat("###{0}", string.Format(text, args));
        }

        public void Note(string text, params object[] args)
        {
            _.AppendFormat("> {0}", string.Format(text, args));
        }

        public void TemplateLink()
        {
            
        }

        public void Literal(string text, params object[] args)
        {
            _.AppendFormat("{0}", string.Format(text, args));
        }

        public void Section(string text, params object[] args)
        {
            
        }

        public void Rows(params System.Action[] actions)
        {
            
        }

        public void Columns(params System.Action[] actions)
        {
            
        }

        public void YouTubeLink(string id)
        {
            
        }

        public void TemplateExample<TTemplate, TData>(TData data, bool isDesignerFile = true, params string[] members) where TTemplate : class, IClassTemplate<TData>, new() where TData : class, IDiagramNodeItem
        {
            
        }

        public void ShowGist(string id, string filename, string userId = "micahosborne")
        {
            
        }

        public bool ShowTutorialStep(ITutorialStep step, Action<IDocumentationBuilder> stepContent = null)
        {
            return false;
        }

        public void BeginTutorial(string walkthrough)
        {
            
        }

        public InteractiveTutorial EndTutorial()
        {
            return null;
        }

        public void ImageByUrl(string empty, string description = null)
        {
            _.AppendFormat("![{0}]({1})", description, empty).AppendLine();
        }

        public void CodeSnippet(string code)
        {
            _.AppendLine("```cs").AppendLine();
            _.AppendLine(code).AppendLine();
            _.AppendLine("```").AppendLine();
        }

        public void ToggleContentByNode<TNode>(string name)
        {
            
        }

        public void ToggleContentByPage<TPage>(string name)
        {
            
        }

        public void ContentByNode<TNode>()
        {
            
        }

        public void ContentByPage<TPage>()
        {
            
        }

        public void LinkToPage<T>()
        {
            
        }

        public void AlsoSeePages(params Type[] type)
        {
            
        }
    }
}