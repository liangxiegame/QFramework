////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

namespace MG.MDV
{
    public class StyleConverter
    {
        private Style       mCurrentStyle = Style.Default;
        private GUIStyle[]  mWorking;
        private GUIStyle[]  mReference;

        Color linkColor         = new Color(0.41f, 0.71f, 1.0f, 1.0f);
        const int Variable      = 0;
        const int FixedInline   = 7;
        const int FixedBlock    = 8;

        static readonly string[] CustomStyles = new string[] {
            "variable",
            "h1",
            "h2",
            "h3",
            "h4",
            "h5",
            "h6",
            "fixed_inline",
            "fixed_block",
        };

        public StyleConverter( GUISkin skin )
        {
            mReference  = new GUIStyle[ CustomStyles.Length ];
            mWorking    = new GUIStyle[ CustomStyles.Length ];

            for( var i = 0; i < CustomStyles.Length; i++ )
            {
                mReference[ i ] = skin.GetStyle( CustomStyles[ i ] );
                mWorking[ i ]   = new GUIStyle( mReference[ i ] );
            }
        }


        //------------------------------------------------------------------------------

        public GUIStyle Apply( Style src )
        {
            if( src.Block )
            {
                return mWorking[ FixedBlock ];
            }

            var style = mWorking[ src.Size ];

            if( mCurrentStyle != src )
            {
                var reference = mReference[ src.Fixed ? FixedInline : Variable ];

                style.font             = reference.font;
                style.fontStyle        = src.GetFontStyle();
                style.normal.textColor = src.Link ? linkColor : reference.normal.textColor;

                mCurrentStyle = src;
            }

            return style;
        }
    }
}
