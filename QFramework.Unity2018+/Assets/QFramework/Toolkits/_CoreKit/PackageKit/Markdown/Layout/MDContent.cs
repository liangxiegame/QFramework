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
using System.Text.RegularExpressions;
using UnityEngine;

namespace QFramework
{
    internal abstract class MDContent
    {
        public Rect Location;
        public MDStyle Style;
        public GUIContent Payload;
        public string Link;

        public float Width => Location.width;

        public float Height => Location.height;

        public bool CanUpdate => false;

        public MDContent(GUIContent payload, MDStyle style, string link)
        {
            Payload = payload;
            Style = style;
            Link = link;
        }

        public void CalcSize(MDContext context)
        {
            Location.size = context.CalcSize(Payload);
        }

        public void Draw(MDContext context)
        {
            if (string.IsNullOrEmpty(Link))
            {
                GUI.Label(Location, Payload, context.Apply(Style));
            }
            else if (GUI.Button(Location, Payload, context.Apply(Style)))
            {
                if (Regex.IsMatch(Link, @"^\w+:", RegexOptions.Singleline))
                {
                    Application.OpenURL(Link);
                }
                else
                {
                    context.SelectPage(Link);
                }
            }
        }

        public virtual void Update(MDContext context, float leftWidth)
        {
        }
    }
}
#endif