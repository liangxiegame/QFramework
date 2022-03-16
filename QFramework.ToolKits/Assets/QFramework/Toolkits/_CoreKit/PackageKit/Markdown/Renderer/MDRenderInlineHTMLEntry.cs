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
    internal class MDRendererInlineHtmlEntity : MarkdownObjectRenderer<MDRendererMarkdown, HtmlEntityInline>
    {
        protected override void Write(MDRendererMarkdown renderer, HtmlEntityInline node)
        {
            renderer.Text(node.Transcoded.ToString());
        }
    }
}
#endif