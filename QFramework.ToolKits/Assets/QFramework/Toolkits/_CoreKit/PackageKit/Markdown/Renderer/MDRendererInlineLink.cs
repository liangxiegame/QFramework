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
    internal class MDRendererInlineLink : MarkdownObjectRenderer<MDRendererMarkdown, LinkInline>
    {
        protected override void Write(MDRendererMarkdown renderer, LinkInline node)
        {
            var url = node.GetDynamicUrl != null ? node.GetDynamicUrl() : node.Url;

            if (node.IsImage)
            {
                renderer.Layout.Image(url, renderer.GetContents(node), node.Title);
            }
            else
            {
                renderer.Link = url;

                if (string.IsNullOrEmpty(node.Title) == false)
                {
                    renderer.ToolTip = node.Title;
                }

                renderer.WriteChildren(node);

                renderer.ToolTip = null;
                renderer.Link = null;
            }
        }
    }
}
#endif