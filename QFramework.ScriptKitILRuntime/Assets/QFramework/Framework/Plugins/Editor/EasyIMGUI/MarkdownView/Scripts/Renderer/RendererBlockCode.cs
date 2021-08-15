////////////////////////////////////////////////////////////////////////////////

using Markdig.Renderers;
using Markdig.Syntax;

namespace MG.MDV
{
    ////////////////////////////////////////////////////////////////////////////////
    // <pre><code>...</code></pre>
    /// <see cref="Markdig.Renderers.Html.CodeBlockRenderer"/>

    public class RendererBlockCode : MarkdownObjectRenderer<RendererMarkdown, CodeBlock>
    {
        protected override void Write( RendererMarkdown renderer, CodeBlock block )
        {
            var fencedCodeBlock = block as FencedCodeBlock;

            if( fencedCodeBlock != null && !string.IsNullOrEmpty( fencedCodeBlock.Info ) )
            {
                // TODO: support for syntax hightlighting ...
                // https://archive.codeplex.com/?p=colorcode
                //UnityEngine.Debug.Log( fencedCodeBlock.Info );
            }


            var prevStyle = renderer.Style;

            renderer.Style.Fixed = true;
            renderer.Style.Block = true;

            renderer.Layout.StartBlock( false );
            renderer.WriteLeafRawLines( block );
            renderer.Layout.EndBlock();

            renderer.Style = prevStyle;

            renderer.FinishBlock( true );
        }
    }
}
