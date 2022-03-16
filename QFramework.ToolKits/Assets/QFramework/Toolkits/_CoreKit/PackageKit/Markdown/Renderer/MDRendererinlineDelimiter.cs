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
using Markdig.Syntax.Inlines;

namespace QFramework
{
    internal class MDRendererInlineDelimiter : MarkdownObjectRenderer<MDRendererMarkdown, DelimiterInline>
    {
        protected override void Write(MDRendererMarkdown renderer, DelimiterInline node)
        {
            renderer.Text(node.ToLiteral());
            renderer.WriteChildren(node);
        }
    }
}
#endif