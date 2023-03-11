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

using Markdig.Renderers;
using Markdig.Syntax;

namespace QFramework
{
    internal class MDRendererBlockHeading : MarkdownObjectRenderer<MDRendererMarkdown, HeadingBlock>
    {
        protected override void Write(MDRendererMarkdown renderer, HeadingBlock block)
        {
            var prevStyle = renderer.Style.Size;
            renderer.Style.Size = block.Level;
            renderer.WriteLeafBlockInline(block);
            renderer.Style.Size = prevStyle;

            if (block.Level == 1)
            {
                renderer.Layout.HorizontalLine();
                renderer.FinishBlock(true);
            }
            else
            {
                renderer.FinishBlock();
            }
        }
    }
}
#endif