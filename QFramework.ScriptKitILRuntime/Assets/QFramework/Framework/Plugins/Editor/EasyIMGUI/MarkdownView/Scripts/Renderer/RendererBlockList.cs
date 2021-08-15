////////////////////////////////////////////////////////////////////////////////

using Markdig.Renderers;
using Markdig.Syntax;

namespace MG.MDV
{
    ////////////////////////////////////////////////////////////////////////////////
    // <ul><li>...</li></ul>
    /// <see cref="Markdig.Renderers.Html.ListRenderer"/>

    public class RendererBlockList : MarkdownObjectRenderer<RendererMarkdown, ListBlock>
    {
        protected override void Write( RendererMarkdown renderer, ListBlock block )
        {
            var layout = renderer.Layout;

            layout.Space();
            layout.Indent();

            var prevImplicit = renderer.ConsumeSpace;
            renderer.ConsumeSpace = true;

            var prefixStyle = renderer.Style;

            if( !block.IsOrdered )
            {
                prefixStyle.Bold = true;
            }

            for( var i = 0; i < block.Count; i++ )
            {
                layout.Prefix( block.IsOrdered ? (i+1).ToString() + "." : "\u2022", prefixStyle );
                renderer.WriteChildren( block[ i ] as ListItemBlock );
            }

            renderer.ConsumeSpace = prevImplicit;
            layout.Outdent();
            layout.Space();
        }
    }
}
