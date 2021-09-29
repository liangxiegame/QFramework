using UnityEngine;

namespace MG.MDV
{
    public class ContentImage : Content
    {
        public string URL;
        public string Alt;

        public ContentImage(GUIContent payload, Style style, string link)
            : base(payload, style, link)
        {
        }

        public override void Update(Context context, float leftWidth)
        {
            Payload.image = context.FetchImage(URL);
            Payload.text = null;

            if (Payload.image == null)
            {
                context.Apply(Style);
                var text = !string.IsNullOrEmpty(Alt) ? Alt : URL;
                Payload.text = string.Format("[{0}]", text);
            }

            var size = context.CalcSize(Payload);

            var offset = 0;
            if ((leftWidth -  offset) < size.x)
            {
                var aspect =  size.y / size.x;
                Location.size = new Vector2(leftWidth -  offset, (leftWidth -  offset) * aspect);
            }
            else
            {
                Location.size = size;
            }
        }
    }
}