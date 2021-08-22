////////////////////////////////////////////////////////////////////////////////

using Markdig.Renderers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace MG.MDV
{
    ////////////////////////////////////////////////////////////////////////////////
    /// <see cref="Markdig.Renderers.HtmlRenderer"/>
    /// <see cref="Markdig.Renderers.Normalize.NormalizeRenderer"/>

    public class RendererMarkdown : RendererBase
    {
        internal LayoutBuilder  Layout;
        internal Style          Style   = new Style();
        internal string         ToolTip = null;
        internal string         Link
        {
            get
            {
                return mLink;
            }

            set
            {
                mLink = value;
                Style.Link = !string.IsNullOrEmpty( mLink );
            }
        }

        public bool ConsumeSpace = false;

        private string mLink = null;

        internal void Text( string text ) { Layout.Text( text, Style, Link, ToolTip ); }


        //------------------------------------------------------------------------------

        public override object Render( MarkdownObject document )
        {
            Write( document );
            return this;
        }

        public RendererMarkdown( LayoutBuilder doc )
        {
            Layout = doc;

            ObjectRenderers.Add( new RendererBlockCode() );
            ObjectRenderers.Add( new RendererBlockList() );
            ObjectRenderers.Add( new RendererBlockHeading() );
            ObjectRenderers.Add( new RendererBlockHtml() );
            ObjectRenderers.Add( new RendererBlockParagraph() );
            ObjectRenderers.Add( new RendererBlockQuote() );
            ObjectRenderers.Add( new RendererBlockThematicBreak() );

            ObjectRenderers.Add( new RendererInlineLink() );
            ObjectRenderers.Add( new RendererInlineAutoLink() );
            ObjectRenderers.Add( new RendererInlineCode() );
            ObjectRenderers.Add( new RendererInlineDelimiter() );
            ObjectRenderers.Add( new RendererInlineEmphasis() );
            ObjectRenderers.Add( new RendererInlineLineBreak() );
            ObjectRenderers.Add( new RendererInlineHtml() );
            ObjectRenderers.Add( new RendererInlineHtmlEntity() );
            ObjectRenderers.Add( new RendererInlineLiteral() );
        }


        ////////////////////////////////////////////////////////////////////////////////

        /// <see cref="Markdig.Renderers.TextRendererBase.WriteLeafInline"/>

        internal void WriteLeafBlockInline( LeafBlock block )
        {
            var inline = block.Inline as Inline;

            while( inline != null )
            {
                Write( inline );
                inline = inline.NextSibling;
            }
        }

        /// <summary>
        /// Output child nodes as raw text
        /// </summary>
        /// <see cref="Markdig.Renderers.HtmlRenderer.WriteLeafRawLines"/>

        internal void WriteLeafRawLines( LeafBlock block )
        {
            if( block.Lines.Lines == null )
            {
                return;
            }

            var lines  = block.Lines;
            var slices = lines.Lines;

            for( int i = 0; i < lines.Count; i++ )
            {
                Text( slices[ i ].ToString() );
                Layout.NewLine();
            }
        }

        internal string GetContents( ContainerInline node )
        {
            if( node == null )
            {
                return string.Empty;
            }

            /// <see cref="Markdig.Renderers.RendererBase.WriteChildren(ContainerInline)"/>
            
            var inline  = node.FirstChild;
            var content = string.Empty;

            while( inline != null )
            {
                var lit = inline as LiteralInline;

                if( lit != null )
                {
                    content += lit.Content.ToString();
                }

                inline = inline.NextSibling;
            }

            return content;
        }

        //------------------------------------------------------------------------------

        internal void FinishBlock( bool space = false )
        {
            if( space && !ConsumeSpace )
            {
                Layout.Space();
            }
            else
            {
                Layout.NewLine();
            }
        }
    }
}
