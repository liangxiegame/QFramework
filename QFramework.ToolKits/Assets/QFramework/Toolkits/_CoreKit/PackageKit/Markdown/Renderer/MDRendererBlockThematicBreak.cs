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
    internal class MDRendererBlockThematicBreak : MarkdownObjectRenderer<MDRendererMarkdown, ThematicBreakBlock>
    {
        protected override void Write(MDRendererMarkdown renderer, ThematicBreakBlock block)
        {
            renderer.Layout.HorizontalLine();
            renderer.FinishBlock();
        }
    }
}
#endif