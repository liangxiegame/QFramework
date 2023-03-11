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
using Markdig.Syntax.Inlines;

namespace QFramework
{
    internal class MDRendererMarkdown : RendererBase
    {
        internal IMDLayoutBuilder Layout;
        internal MDStyle Style = new MDStyle();
        internal string ToolTip = null;

        internal string Link
        {
            get { return mLink; }

            set
            {
                mLink = value;
                Style.Link = !string.IsNullOrEmpty(mLink);
            }
        }

        public bool ConsumeSpace = false;
        public bool ConsumeNewLine = false;

        private string mLink = null;

        internal void Text(string text)
        {
            Layout.Text(text, Style, Link, ToolTip);
        }


        //------------------------------------------------------------------------------

        public override object Render(MarkdownObject document)
        {
            Write(document);
            return this;
        }

        public MDRendererMarkdown(IMDLayoutBuilder doc)
        {
            Layout = doc;

            ObjectRenderers.Add(new MDRendererBlockCode());
            ObjectRenderers.Add(new MDRendererBlockList());
            ObjectRenderers.Add(new MDRendererBlockHeading());
            ObjectRenderers.Add(new MDRendererBlockHtml());
            ObjectRenderers.Add(new MDRendererBlockParagraph());
            ObjectRenderers.Add(new MDRendererBlockQuote());
            ObjectRenderers.Add(new MDRendererBlockThematicBreak());
            ObjectRenderers.Add(new MDRendererTable());

            ObjectRenderers.Add(new MDRendererInlineLink());
            ObjectRenderers.Add(new MDRendererInlineAutoLink());
            ObjectRenderers.Add(new MDRendererInlineCode());
            ObjectRenderers.Add(new MDRendererInlineDelimiter());
            ObjectRenderers.Add(new MDRendererInlineEmphasis());
            ObjectRenderers.Add(new MDRendererInlineLineBreak());
            ObjectRenderers.Add(new MDRendererInlineHtml());
            ObjectRenderers.Add(new MDRendererInlineHtmlEntity());
            ObjectRenderers.Add(new MDRendererInlineLiteral());
        }


        ////////////////////////////////////////////////////////////////////////////////

        /// <see cref="Markdig.Renderers.TextRendererBase.WriteLeafInline"/>
        internal void WriteLeafBlockInline(LeafBlock block)
        {
            var inline = block.Inline as Inline;

            while (inline != null)
            {
                Write(inline);
                inline = inline.NextSibling;
            }
        }

        /// <summary>
        /// Output child nodes as raw text
        /// </summary>
        /// <see cref="Markdig.Renderers.HtmlRenderer.WriteLeafRawLines"/>
        internal void WriteLeafRawLines(LeafBlock block)
        {
            if (block.Lines.Lines == null)
            {
                return;
            }

            var lines = block.Lines;
            var slices = lines.Lines;

            for (int i = 0; i < lines.Count; i++)
            {
                Text(slices[i].ToString());
                Layout.NewLine();
            }
        }

        internal void WriteCode(LeafBlock block)
        {
            if (block.Lines.Lines == null)
            {
                return;
            }

   


            // var parser = new LanguageParser(new LanguageCompiler(Languages.CompiledLanguages, Languages.CompileLock), Languages.LanguageRepository);
            //
            // List<string> resultCode = new List<string>();
            //
            // parser.Parse(code,Languages.CSharp,(sourceCode, list) =>
            // {
            //     // Debug.Log(s);
            //     foreach (var scope in list)
            //     {
            //         scope.Name.LogInfo();
            //
            //         if (scope.Name == "Keyword")
            //         {
            //             Text($"<color=#4ec9b0>{sourceCode}</color>");
            //             Layout.NewLine();
            //         } else if (scope.Name == "Comment")
            //         {
            //             Text($"<color=#57a64a>{sourceCode}</color>");
            //             Layout.NewLine();
            //         }
            //     }
            //     // Debug.Log(list.Count);
            //     
            // });
            
            var lines = block.Lines;
            var slices = lines.Lines;
            
            var code  = string.Join("@\n@", slices);

            
            foreach (var line in code
                         .Replace(" public ", " <color=#4ec9b0>public</color> ")
                         .Replace(" class ", " <color=#4ec9b0>class</color> ")
                         .Replace(" void ", " <color=#4ec9b0>void</color> ")
                         .Replace("using ", "<color=#4ec9b0>using</color> ")
                         .Replace("namespace ", "<color=#4ec9b0>namespace</color> ")
                         .Replace("private ", "<color=#4ec9b0>private</color> ")
                         .Replace("internal ", "<color=#4ec9b0>internal</color> ")
                         .Replace("protected ", "<color=#4ec9b0>protected</color> ")
                         .Replace("// ", "<color=#57a64a>//</color> ")
                         .Replace("@\n@// ", "@\n@<color=#57a64a>//</color> ")
                         .Replace("/// ", "<color=#57a64a>///</color>  ")
                         .Split("@\n@".ToCharArray()))
            {
                Text(line);
                Layout.NewLine();
            }
            
            // for (int i = 0; i < lines.Count; i++)
            // {
                // Text(slices[i].ToString());
                // Layout.NewLine();
            // }
            
            
        }

        internal string GetContents(ContainerInline node)
        {
            if (node == null)
            {
                return string.Empty;
            }
            
            var inline = node.FirstChild;
            var content = string.Empty;

            while (inline != null)
            {
                var lit = inline as LiteralInline;

                if (lit != null)
                {
                    content += lit.Content.ToString();
                }

                inline = inline.NextSibling;
            }

            return content;
        }

        //------------------------------------------------------------------------------

        internal void FinishBlock(bool space = false)
        {
            if (space && !ConsumeSpace)
            {
                Layout.Space();
            }
            else if (!ConsumeNewLine)
            {
                Layout.NewLine();
            }
        }
    }
}
#endif