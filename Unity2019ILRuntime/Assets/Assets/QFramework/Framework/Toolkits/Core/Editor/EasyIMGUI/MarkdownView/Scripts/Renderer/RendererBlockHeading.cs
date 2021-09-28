////////////////////////////////////////////////////////////////////////////////

using Markdig.Renderers;
using Markdig.Syntax;

namespace MG.MDV
{
    ////////////////////////////////////////////////////////////////////////////////
    // <h1>...</h1>
    /// <see cref="Markdig.Renderers.Html.HeadingRenderer"/>

    public class RendererBlockHeading : MarkdownObjectRenderer<RendererMarkdown, HeadingBlock>
    {
        protected override void Write( RendererMarkdown renderer, HeadingBlock block )
        {
            var prevStyle = renderer.Style.Size;
            renderer.Style.Size = block.Level;
            renderer.WriteLeafBlockInline( block );
            renderer.Style.Size = prevStyle;

            if( block.Level == 1 )
            {
                renderer.Layout.HorizontalLine();
                renderer.FinishBlock( true );
            }
            else
            {
                renderer.FinishBlock();
            }
        }
    }
}
