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
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    internal class MDBlockContent : MDBlock
    {
        MDContent mPrefix = null;
        List<MDContent> mContent = new List<MDContent>();

        public bool IsEmpty
        {
            get { return mContent.Count == 0; }
        }

        public MDBlockContent(float indent) : base(indent)
        {
        }

        public void Add(MDContent content)
        {
            mContent.Add(content);
        }

        public void Prefix(MDContent content)
        {
            mPrefix = content;
        }

        public override void Arrange(MDContext context, Vector2 pos, float maxWidth)
        {
            var origin = pos;

            pos.x += Indent;
            maxWidth = Mathf.Max(maxWidth - Indent, context.MinWidth);

            Rect.position = pos;

            // prefix

            if (mPrefix != null)
            {
                mPrefix.Location.x = pos.x - context.IndentSize * 0.5f;
                mPrefix.Location.y = pos.y;
            }

            // content

            if (mContent.Count == 0)
            {
                Rect.width = 0.0f;
                Rect.height = 0.0f;
                return;
            }

            mContent.ForEach(c => c.Update(context, maxWidth));

            var rowWidth = mContent[0].Width;
            var rowHeight = mContent[0].Height;
            var startIndex = 0;

            for (var i = 1; i < mContent.Count; i++)
            {
                var content = mContent[i];

                if (rowWidth + content.Width > maxWidth)
                {
                    LayoutRow(pos, startIndex, i, rowHeight);
                    pos.y += rowHeight;

                    startIndex = i;
                    rowWidth = content.Width;
                    rowHeight = content.Height;
                }
                else
                {
                    rowWidth += content.Width;
                    rowHeight = Mathf.Max(rowHeight, content.Height);
                }
            }

            if (startIndex < mContent.Count)
            {
                LayoutRow(pos, startIndex, mContent.Count, rowHeight);
                pos.y += rowHeight;
            }

            Rect.width = maxWidth;
            Rect.height = pos.y - origin.y;
        }

        void LayoutRow(Vector2 pos, int from, int until, float rowHeight)
        {
            for (var i = from; i < until; i++)
            {
                var content = mContent[i];

                content.Location.x = pos.x;
                content.Location.y = pos.y + rowHeight - content.Height;

                pos.x += content.Width;
            }
        }

        public override void Draw(MDContext context)
        {
            mContent.ForEach(c => c.Draw(context));

            if (mPrefix != null)
            {
                mPrefix.Draw(context);
            }
        }
    }
}
#endif