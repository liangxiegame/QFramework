using System;

namespace QF.GraphDesigner
{
    public interface IDocumentationBuilder
    {
        void BeginArea(string id);
        void EndArea();
        void BeginSection(string id);
        void EndSection();
        void PushIndent();
        void PopIndent();
        void LinkToNode(IDiagramNodeItem node, string text = null);
        void NodeImage(GraphNode node);
        void Paragraph(string text, params object[] args);
        string EditableParagraph(string text);

        void Break();
        void Lines( params string[] lines);
        void Title(string text, params object[] args);
        void Title2(string text, params object[] args);
        void Title3(string text, params object[] args);
        void Note(string text, params object[] args);
        void TemplateLink();
        void Literal(string text, params object[] args);
        void Section(string text, params object[] args);
        void Rows(params System.Action[] actions);
        void Columns(params System.Action[] actions);
        void YouTubeLink(string id);

        void TemplateExample<TTemplate, TData>(TData data, bool isDesignerFile = true, params string[] members)
            where TTemplate : class, IClassTemplate<TData>, new() where TData : class, IDiagramNodeItem;
        void ShowGist(string id, string filename, string userId = "micahosborne");
        bool ShowTutorialStep(ITutorialStep step, Action<IDocumentationBuilder> stepContent = null);
        void BeginTutorial(string walkthrough);
        InteractiveTutorial EndTutorial();
        void ImageByUrl(string empty, string description = null);
        void CodeSnippet(string code);
        void ToggleContentByNode<TNode>(string name);
        void ToggleContentByPage<TPage>(string name);        
        void ContentByNode<TNode>();
        void ContentByPage<TPage>();

        void LinkToPage<T>();
        void AlsoSeePages(params Type[] type);
    }
}