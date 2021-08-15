using UnityEngine;

namespace MG.MDV
{
    public class BlockLine : Block
    {
        public BlockLine( float indent ) : base( indent ) { }

        public override void Draw( Context context )
        {
            var rect = new Rect( Rect.position.x, Rect.center.y, Rect.width, 1.0f );
            GUI.Label( rect, string.Empty, GUI.skin.GetStyle( "hr" ) );
        }

        public override void Arrange( Context context, Vector2 pos, float maxWidth,float leftWidth )
        {
            Rect.position = pos;
            Rect.width = maxWidth;
            Rect.height = 10.0f;
        }
    }
}
