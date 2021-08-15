////////////////////////////////////////////////////////////////////////////////

using System.Text;
using UnityEngine;

namespace MG.MDV
{
    public class LayoutBuilder : IBuilder
    {
        ////////////////////////////////////////////////////////////////////////////////
        // IMarkdownInterface

        public void Text( string text, Style style, string link, string tooltip )
        {
            if( mCurrentContent == null )
            {
                NewContentBlock();
            }

            mContext.Apply( style );

            if( style.Size > 0 )
            {
                if( mCurrentContent.ID == null )
                {
                    mCurrentContent.ID = "#";
                }
                else
                {
                    mCurrentContent.ID += "-";
                }

                mCurrentContent.ID += text.Trim().Replace( ' ', '-' ).ToLower();
            }

            mStyle   = style;
            mLink    = link;
            mTooltip = tooltip;

            for( var i = 0; i < text.Length; i++ )
            {
                var ch = text[i];

                if( ch == '\n' )
                {
                    AddWord();
                    NewLine();
                }
                else if( char.IsWhiteSpace( ch ) )
                {
                    mWord.Append( ' ' );
                    AddWord();
                }
                else
                {
                    mWord.Append( ch );
                }
            }

            AddWord();
        }


        //------------------------------------------------------------------------------

        public void Image( string url, string alt, string title )
        {
            var payload = new GUIContent();
            var content = new ContentImage( payload, mStyle, mLink );

            content.URL     = url;
            content.Alt     = alt;
            payload.tooltip = !string.IsNullOrEmpty( title ) ? title : alt;

            AddContent( content );
        }

        //------------------------------------------------------------------------------

        public void NewLine()
        {
            if( mCurrentContent != null && mCurrentContent.IsEmpty )
            {
                return;
            }

            NewContentBlock();
        }

        public void Space()
        {
            if( CurrentBlock is BlockSpace || CurrentBlock is BlockContainer )
            {
                return;
            }

            AddBlock( new BlockSpace( mIndent ) );
        }

        public void HorizontalLine()
        {
            if( CurrentBlock is BlockLine )
            {
                return;
            }

            AddBlock( new BlockLine( mIndent ) );
        }


        //------------------------------------------------------------------------------

        public void Indent()
        {
            NewLine();

            mIndent += mContext.IndentSize;

            if( mCurrentContent != null )
            {
                mCurrentContent.Indent = mIndent;
            }
        }

        public void Outdent()
        {
            NewLine();

            mIndent = Mathf.Max( mIndent - mContext.IndentSize, 0.0f );

            if( mCurrentContent != null )
            {
                mCurrentContent.Indent = mIndent;
            }
        }

        public void Prefix( string text, Style style )
        {
            mContext.Apply( style );

            if( mCurrentContent == null )
            {
                return;
            }

            var payload = new GUIContent( text );
            var content = new ContentText( payload, style, null );
            content.Location.size = mContext.CalcSize( payload );

            mCurrentContent.Prefix( content );
        }


        //------------------------------------------------------------------------------

        public void StartBlock( bool quoted )
        {
            Space();
            mCurrentContainer = AddBlock( new BlockContainer( mIndent ) { Highlight = true, Quoted = quoted } );
            CurrentBlock = null;
        }

        public void EndBlock()
        {
            mCurrentContainer.RemoveTrailingSpace();
            mCurrentContainer = mCurrentContainer.Parent as BlockContainer ?? mDocument;
            CurrentBlock = null;

            Space();
        }


        ////////////////////////////////////////////////////////////////////////////////
        // private


        Context         mContext;

        Style           mStyle;
        string          mLink;
        string          mTooltip;
        StringBuilder   mWord;
        float           mIndent;

        BlockContainer  mDocument;
        BlockContainer  mCurrentContainer;
        Block           mCurrentBlock;
        BlockContent    mCurrentContent;

        Block CurrentBlock
        {
            get
            {
                return mCurrentBlock;
            }

            set
            {
                mCurrentBlock   = value;
                mCurrentContent = mCurrentBlock as BlockContent;
            }
        }


        //------------------------------------------------------------------------------

        public LayoutBuilder( Context context )
        {
            mContext          = context;

            mStyle            = new Style();
            mLink             = null;
            mTooltip          = null;
            mWord             = new StringBuilder( 1024 );

            mIndent           = 0.0f;

            mDocument         = new BlockContainer( mIndent );
            mCurrentContainer = mDocument;
            mCurrentBlock     = null;
            mCurrentContent   = null;
        }

        public Layout GetLayout()
        {
            return new Layout( mContext, mDocument );
        }

        //------------------------------------------------------------------------------

        void AddContent( Content content )
        {
            if( mCurrentContent == null )
            {
                NewContentBlock();
            }

            mCurrentContent.Add( content );
        }

        T AddBlock<T>( T block ) where T : Block
        {
            CurrentBlock = mCurrentContainer.Add( block );
            return block;
        }

        void NewContentBlock()
        {
            AddBlock( new BlockContent( mIndent ) );
 
            mStyle.Clear();
            mContext.Apply( mStyle );
        }

        void AddWord()
        {
            if( mWord.Length == 0 )
            {
                return;
            }

            var payload = new GUIContent( mWord.ToString(), mTooltip );
            var content = new ContentText( payload, mStyle, mLink );
            content.CalcSize( mContext );

            AddContent( content );

            mWord.Length = 0;
        }
    }
}
