////////////////////////////////////////////////////////////////////////////////

using Markdig.Renderers;
using Markdig.Syntax.Inlines;

namespace MG.MDV
{
    ////////////////////////////////////////////////////////////////////////////////
    // <img src="..." /> || <a href="...">
    /// <see cref="Markdig.Renderers.Html.Inlines.LinkInlineRenderer"/>

    public class RendererInlineLink : MarkdownObjectRenderer<RendererMarkdown, LinkInline>
    {
        protected override void Write( RendererMarkdown renderer, LinkInline node )
        {
            var url = node.GetDynamicUrl != null ? node.GetDynamicUrl() : node.Url;

            if( node.IsImage )
            {
                renderer.Layout.Image( url, renderer.GetContents( node ), node.Title );
            }
            else
            {
                renderer.Link = url;

                if( string.IsNullOrEmpty( node.Title ) == false )
                {
                    renderer.ToolTip = node.Title;
                }

                renderer.WriteChildren( node );

                renderer.ToolTip = null;
                renderer.Link    = null;
            }
        }
    }
}
