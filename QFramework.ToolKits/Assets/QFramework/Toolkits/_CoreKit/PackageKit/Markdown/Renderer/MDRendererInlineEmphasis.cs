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
    internal class MDRendererInlineEmphasis : MarkdownObjectRenderer<MDRendererMarkdown, EmphasisInline>
    {
        protected override void Write(MDRendererMarkdown renderer, EmphasisInline node)
        {
            bool prev = false;

            if (node.IsDouble)
            {
                prev = renderer.Style.Bold;
                renderer.Style.Bold = true;
            }
            else
            {
                prev = renderer.Style.Italic;
                renderer.Style.Italic = true;
            }

            renderer.WriteChildren(node);

            if (node.IsDouble)
            {
                renderer.Style.Bold = prev;
            }
            else
            {
                renderer.Style.Italic = prev;
            }
        }
    }
}
#endif