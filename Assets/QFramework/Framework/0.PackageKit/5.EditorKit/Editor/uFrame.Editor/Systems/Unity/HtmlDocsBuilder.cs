using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Invert.Common.UI;
using UnityEditor;
using UnityEngine;

namespace QF.GraphDesigner.Unity
{
    public class HtmlDocsBuilder : IDocumentationBuilder
    {
        private StringBuilder _output;

        public StringBuilder Output
        {
            get { return _output ?? (_output = new StringBuilder()); }
            set { _output = value; }
        }

        public List<DocumentationPage> Pages { get; set; }

        public HtmlDocsBuilder(List<DocumentationPage> pages, string styleSheet, string screenshotsRelativePath = null)
        {
            Pages = pages;
            ScreenshotsRelativePath = screenshotsRelativePath;
            StyleSheet = styleSheet;
        }

        public string ScreenshotsRelativePath { get; set; }
        public string StyleSheet { get; set; }

        public void OutputPage(DocumentationPage page)
        {
            Output.AppendFormat("<div id='page-{0}' class='page' >", page.Name.Replace(" ", ""));
            page.GetContent(this);
            Output.Append("</div>");
            foreach (var child in page.ChildPages)
            {
                OutputPage(child);
            }
        }
        public void OutputTOC(DocumentationPage page, StringBuilder builder, int indent = 1)
        {
            if (!page.ShowInNavigation) return;
            builder.AppendFormat("<div class='page-toc page-{0}'>", page.Name.Replace(" ", "").ToLower());
            PageLink(page, builder);
            builder.AppendFormat("<div class='page-toc-margin' style='margin-left: {0}px'>", indent * 10);

            foreach (var child in page.ChildPages.OrderBy(p => p.Order))
            {
                OutputTOC(child, builder, indent + 1);
            }
            builder.AppendLine("</div>");
            builder.AppendLine("</div>");

        }

        public override string ToString()
        {
            return ToString(false, true);
        }

        public  string ToString(bool includeHeaders, bool includeStylesheets)
        {

            var finalOutput = new StringBuilder();
            if (includeHeaders)
            {
                finalOutput.AppendLine("<html>");
                finalOutput.AppendLine("<head>");    
            }

            if (includeStylesheets)
            {
                if (this.StyleSheet != null)
                    finalOutput.AppendLine(string.Format("<link rel='stylesheet' type='text/css' href='{0}.css'>", this.StyleSheet));
                finalOutput.AppendLine("<link rel=\"stylesheet\" href=\"http://cdnjs.cloudflare.com/ajax/libs/highlight.js/8.5/styles/default.min.css\">")
                ;
                finalOutput.AppendLine(
                    "<script src=\"http://cdnjs.cloudflare.com/ajax/libs/highlight.js/8.5/highlight.min.js\"></script>");
                finalOutput.AppendLine("<script>hljs.initHighlightingOnLoad();</script>");
                finalOutput.AppendLine(string.Format("<script src=\"https://code.jquery.com/jquery-2.1.4.min.js\"></script>", this.StyleSheet));
                //finalOutput.Append("<script>$(function() { $('.page').hide(); });</script>");
            }




            if (includeHeaders)
            {
                finalOutput.AppendLine("</head>");
                finalOutput.AppendLine("<body>");
            }
          
            finalOutput.Append(Output.ToString());
            if (includeHeaders)
            {
                finalOutput.AppendLine("</body>");
                finalOutput.AppendLine("</html>");
                
            }
          
            return finalOutput.ToString();
        }

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
            Output.AppendLine("<div style='margin-left: 5px;'>");
        }

        public void PopIndent()
        {
            Output.AppendLine("</div>");
        }

        public void LinkToNode(IDiagramNodeItem node, string text = null)
        {

        }

        public void NodeImage(GraphNode node)
        {

        }

        public void Paragraph(string text, params object[] args)
        {
            Output.Append("<p>");
            if (args == null || args.Length == 0)
            {
                Output.AppendLine(text);
            }
            else
            {
                Output.AppendFormat(text, args);
            } Output.Append("</p>");
        }

        public string EditableParagraph(string text)
        {
            Output.Append("<p>");

                Output.AppendLine(text);

            Output.Append("</p>");
            return text;
        }

        public void Break()
        {
            Output.Append("<br/>");
        }

        public void Lines(params string[] lines)
        {

        }

        public void Title(string text, params object[] args)
        {
            Output.Append("<h1>");
            if (args == null || args.Length == 0)
            {

                Output.Append(text);
            }
            else
            {
                Output.AppendFormat(text, args);
            }
            Output.Append("</h1>");
        }

        public void Title2(string text, params object[] args)
        {
            Output.Append("<h2>");
            if (args == null || args.Length == 0)
            {

                Output.Append(text);
            }
            else
            {
                Output.AppendFormat(text, args);
            }
            Output.Append("</h2>");
        }

        public void Title3(string text, params object[] args)
        {
            Output.Append("<h3>");
            if (args == null || args.Length == 0)
            {

                Output.Append(text);
            }
            else
            {
                Output.AppendFormat(text, args);
            }
            Output.Append("</h3>");
        }

        public void Note(string text, params object[] args)
        {
            Output.Append("<div class='note'><b>Note: </b>");
            if (args == null || args.Length == 0)
            {

                Output.Append(text);
            }
            else
            {
                Output.AppendFormat(text, args);
            }
            Output.Append("</div>");
        }

        public void TemplateLink()
        {

        }

        public void Literal(string text, params object[] args)
        {

            if (args == null || args.Length == 0)
            {

                Output.Append(text);
            }
            else
            {
                Output.AppendFormat(text, args);
            }
            Output.Append("<div class='note'>");
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
            Output.AppendFormat("<div class='youtube-link-container'><a class='youtube-link' href='https://www.youtube.com/watch?v={0}'><img src='http://img.youtube.com/vi/{1}/mqdefault.jpg' /></a></div>", id);
        }

        public void TemplateExample<TTemplate, TData>(TData data, bool isDesignerFile = true, params string[] members)
            where TTemplate : class, IClassTemplate<TData>, new()
            where TData : class, IDiagramNodeItem
        {
            //var tempProject = ScriptableObject.CreateInstance<TemporaryProjectRepository>();
            //tempProject.CurrentGraph = data.Node.Graph;
            //tempProject.Graphs = new[] { data.Node.Graph };

            //var template = new TemplateClassGenerator<TData, TTemplate>()
            //{
            //    Data = data,
            //    IsDesignerFile = isDesignerFile,
            //    // If we don't have any make sure its null
            //    FilterToMembers = members != null && members.Length > 0 ? members : null
            //};
            //var name = "Example " + data.Name;
            //if (members != null)
            //{
            //    name += string.Join(", ", members);
            //}
            //if (isDesignerFile)
            //{
            //    name += ".designer";
            //}
            //name += ".cs";

            //var codeFileGenerator = new CodeFileGenerator
            //{
            //    Generators = new OutputGenerator[] { template },
            //    RemoveComments = true
            //};

            //Title3(name);
            //CodeSnippet(codeFileGenerator.ToString());

        }

        public void ShowGist(string id, string filename, string userId = "micahosborne")
        {
            Title3(filename);
            Output.AppendFormat("<script src=\"https://gist.github.com/{0}/{1}.js\"></script>", userId, id);
        }

        public InteractiveTutorial CurrentTutorial { get; set; }

        public bool ShowTutorialStep(ITutorialStep step, Action<IDocumentationBuilder> stepContent = null)
        {
            CurrentTutorial.Steps.Add(step);
            if (stepContent != null)
            {
                step.StepContent = stepContent;
            }
            return step.IsComplete == null;

        }

        public void BeginTutorial(string walkthrough)
        {
            CurrentTutorial = new InteractiveTutorial(walkthrough);
        }

        public InteractiveTutorial EndTutorial()
        {
            foreach (var step in CurrentTutorial.Steps)
            {
                Title2(step.Name);
                if (step.StepContent != null)
                {
                    step.StepContent(this);
                }
            }
            return CurrentTutorial;
        }

        public void ImageByUrl(string empty, string description = null)
        {
            Output.AppendFormat("<div class='img-container'><img src='{0}' /></div>", empty);
        }

        public void CodeSnippet(string code)
        {
            Output.Append("<pre><code class='csharp'>");

            Output.Append(code.Replace("<", "&lt;").Replace(">", "&gt;"));

            Output.Append("</code></pre>");
        }

        public void ToggleContentByNode<TNode>(string name)
        {
            var page = FindPage(Pages, p => p.RelatedNodeType == typeof(TNode));
            if (page != null)
                PageLink(page, Output);

        }

        public void ToggleContentByPage<TPage>(string name)
        {
            LinkToPage<TPage>();
        }

        public void ContentByNode<TNode>()
        {
            var page = FindPage(Pages, p => p.RelatedNodeType == typeof(TNode));
            if (page != null)
                page.GetContent(this);
        }

        public void ContentByPage<TPage>()
        {
            var page = FindPage(Pages, p => p.GetType() == typeof(TPage));
            if (page != null)
                page.GetContent(this);
        }
        public static DocumentationPage FindPage(IEnumerable<DocumentationPage> inside, Predicate<DocumentationPage> predicate)
        {
            foreach (var page in inside)
            {
                if (predicate(page))
                {
                    return page;
                }
                var result = FindPage(page.ChildPages, predicate);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
        public void LinkToPage<T>()
        {
            var page = FindPage(Pages, p => p.GetType() == typeof(T));
            if (page != null)
            {
                PageLink(page, Output);


            }

        }

        public void AlsoSeePages(params Type[] type)
        {
            Title2("Also See:");
            foreach (var t in type)
            {
                var t1 = t;
                var page = FindPage(Pages, p => p.GetType() == t1);
                if (page != null)
                {
                    PageLink(page, Output);
                }
            }
        }
        public Func<DocumentationPage,string> PageLinkHandler { get; set; }

        private void PageLink(DocumentationPage page, StringBuilder builder)
        {
            if (PageLinkHandler != null)
            {
                builder.Append(PageLinkHandler(page));
            }
            else
            {
                builder.AppendFormat("<a href='{0}.html' target='content-frame'>{1}</a>",
                    page.Name.Replace(" ", ""), page.Name).AppendLine();    
            }
            //onclick=\"$('.page').hide(); $('#page-{0}').show();\"
            
        }
    }
}