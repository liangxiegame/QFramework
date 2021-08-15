using System;
using UnityEngine;

namespace MG.MDV
{
    public abstract class Block
    {
        public string   ID      = null;
        public Rect     Rect    = new Rect();
        public Block    Parent  = null;
        public float    Indent  = 0.0f;

        public abstract void Arrange( Context context, Vector2 anchor, float maxWidth, float leftWidth);
        public abstract void Draw( Context context );

        public Block( float indent )
        {
            Indent = indent;
        }

        public virtual Block Find( string id )
        {
            return id.Equals( ID, StringComparison.Ordinal ) ? this : null;
        }
    }
}
