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
using System.Linq;
using Markdig.Extensions.Tables;
using Markdig.Renderers;
using Markdig.Syntax.Inlines;
using UnityEngine;

namespace QFramework
{
    internal class MDRendererTable : MarkdownObjectRenderer<MDRendererMarkdown, Table>
    {
        protected override void Write(MDRendererMarkdown renderer, Table table)
        {
            var layout = renderer.Layout;

            if (table.Count == 0)
            {
                return;
            }

            layout.StartTable();

            // limit the columns to the number of headers
            var numCols = (table[0] as TableRow).Count(c => (c as TableCell).Count > 0);

            // column alignment
            var alignment = table.ColumnDefinitions
                .Select(cd => cd.Alignment.HasValue ? cd.Alignment.Value : TableColumnAlign.Left).ToArray();

            foreach (TableRow row in table)
            {
                if (row == null)
                {
                    continue;
                }

                layout.StartTableRow(row.IsHeader);
                var consumeSpace = renderer.ConsumeSpace;
                renderer.ConsumeSpace = true;

                var numCells = Mathf.Min(numCols, row.Count);

                for (var cellIndex = 0; cellIndex < numCells; cellIndex++)
                {
                    var cell = row[cellIndex] as TableCell;

                    if (cell == null || cell.Count == 0)
                    {
                        continue;
                    }

                    if (cell[0].Span.IsEmpty)
                    {
                        renderer.Write(new LiteralInline(" "));

                        if (cellIndex != row.Count - 1)
                        {
                            layout.NewLine();
                        }
                    }
                    else
                    {
                        var consumeNewLine = renderer.ConsumeNewLine;

                        if (cellIndex == numCols - 1)
                        {
                            renderer.ConsumeNewLine = true;
                        }

                        renderer.Write(new LiteralInline(" "));
                        renderer.WriteChildren(cell);
                        renderer.ConsumeNewLine = consumeNewLine;
                    }
                }

                renderer.ConsumeSpace = consumeSpace;
                layout.EndTableRow();
            }

            layout.EndTable();
        }
    }
}
#endif