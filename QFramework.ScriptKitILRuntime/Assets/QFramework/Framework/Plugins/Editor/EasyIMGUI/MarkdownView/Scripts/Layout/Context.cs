using UnityEngine;

namespace MG.MDV
{
    public class Context
    {
        public Context( GUISkin skin, HandlerImages images, HandlerNavigate navigate )
        {
            mStyleConverter = new StyleConverter( skin );
            mImages         = images;
            mNagivate       = navigate;

            Apply( Style.Default );
        }

        StyleConverter      mStyleConverter;
        GUIStyle            mStyleGUI;
        HandlerImages       mImages;
        HandlerNavigate     mNagivate;

        public void     SelectPage( string path )       { mNagivate.SelectPage( path ); }
        public Texture  FetchImage( string url )        { return mImages.FetchImage( url ); }

        public float    LineHeight                      { get { return mStyleGUI.lineHeight; } }
        public float    MinWidth                        { get { return LineHeight * 2.0f; } }
        public float    IndentSize                      { get { return LineHeight * 1.5f; } }

        public void     Reset()                         { Apply( Style.Default ); }
        public GUIStyle Apply( Style style )            { mStyleGUI = mStyleConverter.Apply( style ); return mStyleGUI; }
        public Vector2  CalcSize( GUIContent content )  { return mStyleGUI.CalcSize( content ); }
    }
}
