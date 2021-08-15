////////////////////////////////////////////////////////////////////////////////

using Markdig.Renderers;
using Markdig.Syntax;

namespace MG.MDV
{
    // <p>...</p>

    /// <see cref="Markdig.Renderers.Html.ParagraphRenderer"/>

    public class RendererBlockParagraph : MarkdownObjectRenderer<RendererMarkdown, ParagraphBlock>
    {
        protected override void Write( RendererMarkdown renderer, ParagraphBlock block )
        {
            renderer.WriteLeafBlockInline( block );
            renderer.FinishBlock( true );
        }
    }
}
