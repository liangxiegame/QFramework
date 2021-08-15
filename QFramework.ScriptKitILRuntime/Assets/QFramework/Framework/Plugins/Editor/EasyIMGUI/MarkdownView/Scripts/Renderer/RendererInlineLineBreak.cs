////////////////////////////////////////////////////////////////////////////////

using Markdig.Renderers;
using Markdig.Syntax.Inlines;

namespace MG.MDV
{
    ////////////////////////////////////////////////////////////////////////////////
    // <br/>
    /// <see cref="Markdig.Renderers.Html.Inlines.LineBreakInlineRenderer"/>

    public class RendererInlineLineBreak : MarkdownObjectRenderer<RendererMarkdown, LineBreakInline>
    {
        protected override void Write( RendererMarkdown renderer, LineBreakInline node )
        {
            if( node.IsHard )
            {
                renderer.FinishBlock();
            }
            else
            {
                renderer.Text( " " );
            }
        }
    }
}
