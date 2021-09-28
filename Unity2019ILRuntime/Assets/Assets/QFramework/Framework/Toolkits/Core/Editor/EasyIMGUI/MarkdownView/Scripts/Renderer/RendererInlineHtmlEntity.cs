////////////////////////////////////////////////////////////////////////////////

using Markdig.Renderers;
using Markdig.Syntax.Inlines;

namespace MG.MDV
{
    ////////////////////////////////////////////////////////////////////////////////
    /// <see cref="Markdig.Renderers.Html.Inlines.HtmlEntityInlineRenderer"/>
    /// <see cref="Markdig.Renderers.Normalize.Inlines.NormalizeHtmlEntityInlineRenderer"/>

    public class RendererInlineHtmlEntity : MarkdownObjectRenderer<RendererMarkdown, HtmlEntityInline>
    {
        protected override void Write( RendererMarkdown renderer, HtmlEntityInline node )
        {
            renderer.Text( node.Transcoded.ToString() );
        }
    }
}
