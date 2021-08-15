////////////////////////////////////////////////////////////////////////////////

using Markdig.Renderers;
using Markdig.Syntax.Inlines;

namespace MG.MDV
{
    /// <see cref="Markdig.Renderers.Html.Inlines.LiteralInlineRenderer"/>
    /// <see cref="Markdig.Renderers.Normalize.Inlines.LiteralInlineRenderer"/>

    public class RendererInlineLiteral : MarkdownObjectRenderer<RendererMarkdown, LiteralInline>
    {
        protected override void Write( RendererMarkdown renderer, LiteralInline node )
        {
            renderer.Text( node.Content.ToString() );
        }
    }
}
