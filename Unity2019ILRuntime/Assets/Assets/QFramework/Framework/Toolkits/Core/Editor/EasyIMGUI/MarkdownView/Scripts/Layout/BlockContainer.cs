using System;
using System.Collections.Generic;
using UnityEngine;

namespace MG.MDV
{
    public class BlockContainer : Block
    {
        public bool Quoted    = false;
        public bool Highlight = false;

        List<Block> mBlocks = new List<Block>();

        public BlockContainer( float indent ) : base( indent ) { }

        public Block Add( Block block )
        {
            block.Parent = this;
            mBlocks.Add( block );
            return block;
        }

        public override Block Find( string id )
        {
            if( id.Equals( ID, StringComparison.Ordinal ) )
            {
                return this;
            }

            foreach( var block in mBlocks )
            {
                var match = block.Find( id );

                if( match != null )
                {
                    return match;
                }
            }

            return null;
        }

        public override void Arrange(Context context, Vector2 pos, float maxWidth, float leftWidth)
        {
            Rect.position = new Vector2( pos.x + Indent, pos.y );
            Rect.width = maxWidth - Indent - context.IndentSize;

            var paddingBottom = 0.0f;

            if( Highlight )
            {
                var style = GUI.skin.GetStyle( Quoted ? "blockquote" : "blockcode" );

                pos.x += style.padding.left;
                pos.y += style.padding.top;

                maxWidth -= style.padding.horizontal;
                paddingBottom = style.padding.bottom;
            }

            foreach( var block in mBlocks )
            {
                block.Arrange( context, pos, maxWidth,leftWidth );
                pos.y += block.Rect.height;
            }

            Rect.height = pos.y - Rect.position.y + paddingBottom;
        }

        public override void Draw( Context context )
        {
            if( Highlight && !Quoted )
            {
                GUI.Box( Rect, string.Empty, GUI.skin.GetStyle( "blockcode" ) );
            }

            mBlocks.ForEach( block => block.Draw( context ) );

            if( Highlight && Quoted )
            {
                GUI.Box( Rect, string.Empty, GUI.skin.GetStyle( "blockquote" ) );
            }
        }

        public void RemoveTrailingSpace()
        {
            if( mBlocks.Count > 0 && mBlocks[ mBlocks.Count - 1 ] is BlockSpace )
            {
                mBlocks.RemoveAt( mBlocks.Count - 1 );
            }
        }
    }
}
