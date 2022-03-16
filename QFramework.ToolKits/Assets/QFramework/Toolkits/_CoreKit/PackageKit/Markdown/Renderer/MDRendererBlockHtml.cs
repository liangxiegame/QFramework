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
    internal class MDRendererBlockHtml : MarkdownObjectRenderer<MDRendererMarkdown, HtmlBlock>
    {
        protected override void Write(MDRendererMarkdown renderer, HtmlBlock block)
        {
            if (!MDPreferences.StripHTML)
            {
                renderer.WriteLeafRawLines(block);
                renderer.FinishBlock();
            }
        }
    }
}
#endif