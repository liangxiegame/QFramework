////////////////////////////////////////////////////////////////////////////////

namespace MG.MDV
{
    public class Layout
    {
        Context mContext;
        BlockContainer mDocument;

        public Layout( Context context, BlockContainer doc )
        {
            mContext  = context;
            mDocument = doc;
        }

        public float Height { get { return mDocument.Rect.height; } }

        public Block Find( string id )
        {
            return mDocument.Find( id );
        }

        public void Arrange(float maxWidth, float leftWidth)
        {
            mContext.Reset();
            mDocument.Arrange( mContext, MarkdownViewer.Margin, maxWidth,leftWidth );
        }

        public void Draw()
        {
            mContext.Reset();
            mDocument.Draw( mContext );
        }
    }
}

