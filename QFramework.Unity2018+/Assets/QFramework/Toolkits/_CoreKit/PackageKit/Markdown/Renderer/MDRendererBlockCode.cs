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
    internal class MDRendererBlockCode : MarkdownObjectRenderer<MDRendererMarkdown, CodeBlock>
    {
        protected override void Write(MDRendererMarkdown renderer, CodeBlock block)
        {
            var fencedCodeBlock = block as FencedCodeBlock;

            if (fencedCodeBlock != null && !string.IsNullOrEmpty(fencedCodeBlock.Info))
            {
                // TODO: support for syntax hightlighting ...
            }

            var prevStyle = renderer.Style;

            renderer.Style.Fixed = true;
            renderer.Style.Block = true;

            renderer.Layout.StartBlock(false);
            renderer.WriteCode(block);
            renderer.Layout.EndBlock();

            renderer.Style = prevStyle;

            renderer.FinishBlock(true);
        }
    }
}
#endif