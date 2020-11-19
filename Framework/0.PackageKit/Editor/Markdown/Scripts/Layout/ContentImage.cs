using UnityEngine;

namespace MG.MDV
{
    public class ContentImage : Content
    {
        public string URL;
        public string Alt;

        public ContentImage( GUIContent payload, Style style, string link )
            : base( payload, style, link )
        {
        }

        public override void Update( Context context )
        {
            Payload.image = context.FetchImage( URL );
            Payload.text = null;

            if( Payload.image == null )
            {
                context.Apply( Style );
                var text = !string.IsNullOrEmpty( Alt ) ? Alt : URL;
                Payload.text = string.Format( "[{0}]", text );
            }

            Location.size = context.CalcSize( Payload );
        }
    }
}

