/****************************************************************************
 * Copyright (c) 2019 Gwaredd Mountain UNDER MIT License
 * Copyright (c) 2022 liangxiegame UNDER MIT License
 *
 * https://github.com/gwaredd/UnityMarkdownViewer
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using UnityEngine;

namespace QFramework
{
    internal class MDContentImage : MDContent
    {
        public string URL;
        public string Alt;

        public MDContentImage(GUIContent payload, MDStyle style, string link)
            : base(payload, style, link)
        {
        }


        public override void Update(MDContext context, float leftWidth)
        {
            Payload.image = context.FetchImage(URL);
            Payload.text = null;

            if (Payload.image == null)
            {
                context.Apply(Style);
                var text = !string.IsNullOrEmpty(Alt) ? Alt : URL;
                Payload.text = $"[{text}]";
            }

            var size = context.CalcSize(Payload);

            var offset = 40;
            if ((leftWidth - offset) < size.x)
            {
                var aspect = size.y / size.x;
                Location.size = new Vector2(leftWidth - offset, (leftWidth - offset) * aspect);
            }
            else
            {
                Location.size = size;
            }
        }
    }
}
#endif