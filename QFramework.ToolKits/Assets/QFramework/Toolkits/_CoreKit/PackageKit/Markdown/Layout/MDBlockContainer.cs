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
using System;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    internal class MDBlockContainer : MDBlock
    {
        public bool Quoted = false;
        public bool Highlight = false;
        public bool Horizontal = false;
        public bool IsTableRow = false;
        public bool IsTableHeader = false;

        List<MDBlock> mBlocks = new List<MDBlock>();

        public MDBlockContainer(float indent) : base(indent)
        {
        }

        public MDBlock Add(MDBlock block)
        {
            block.Parent = this;
            mBlocks.Add(block);
            return block;
        }

        public override MDBlock Find(string id)
        {
            if (id.Equals(ID, StringComparison.Ordinal))
            {
                return this;
            }

            foreach (var block in mBlocks)
            {
                var match = block.Find(id);

                if (match != null)
                {
                    return match;
                }
            }

            return null;
        }

        public override void Arrange(MDContext context, Vector2 pos, float maxWidth)
        {
            Rect.position = new Vector2(pos.x + Indent, pos.y);
            Rect.width = maxWidth - Indent - context.IndentSize;

            var paddingBottom = 0.0f;
            var paddingVertical = 0.0f;

            if (Highlight || IsTableHeader || IsTableRow)
            {
                GUIStyle style;

                if (Highlight)
                {
                    style = GUI.skin.GetStyle(Quoted ? "blockquote" : "blockcode");
                }
                else
                {
                    style = GUI.skin.GetStyle(IsTableHeader ? "th" : "tr");
                }

                pos.x += style.padding.left;
                pos.y += style.padding.top;

                maxWidth -= style.padding.horizontal;
                paddingBottom = style.padding.bottom;
                paddingVertical = style.padding.vertical;
            }

            if (Horizontal)
            {
                Rect.height = 0;
                maxWidth = mBlocks.Count == 0 ? maxWidth : maxWidth / mBlocks.Count;

                foreach (var block in mBlocks)
                {
                    block.Arrange(context, pos, maxWidth);
                    pos.x += block.Rect.width;
                    Rect.height = Mathf.Max(Rect.height, block.Rect.height);
                }

                Rect.height += paddingVertical;
            }
            else
            {
                foreach (var block in mBlocks)
                {
                    block.Arrange(context, pos, maxWidth);
                    pos.y += block.Rect.height;
                }

                Rect.height = pos.y - Rect.position.y + paddingBottom;
            }
        }

        public override void Draw(MDContext context)
        {
            if (Highlight && !Quoted)
            {
                GUI.Box(Rect, string.Empty, GUI.skin.GetStyle("blockcode"));
            }
            else if (IsTableHeader)
            {
                GUI.Box(Rect, string.Empty, GUI.skin.GetStyle("th"));
            }
            else if (IsTableRow)
            {
                var parentBlock = Parent as MDBlockContainer;
                if (parentBlock == null)
                {
                    GUI.Box(Rect, string.Empty, GUI.skin.GetStyle("tr"));
                }
                else
                {
                    var idx = parentBlock.mBlocks.IndexOf(this);
                    GUI.Box(Rect, string.Empty, GUI.skin.GetStyle(idx % 2 == 0 ? "tr" : "trl"));
                }
            }

            mBlocks.ForEach(block => block.Draw(context));

            if (Highlight && Quoted)
            {
                GUI.Box(Rect, string.Empty, GUI.skin.GetStyle("blockquote"));
            }
        }

        public void RemoveTrailingSpace()
        {
            if (mBlocks.Count > 0 && mBlocks[mBlocks.Count - 1] is MDBlockSpace)
            {
                mBlocks.RemoveAt(mBlocks.Count - 1);
            }
        }
    }
}
#endif