/****************************************************************************
 * Copyright (c) 2019 Gwaredd Mountain UNDER MIT License
 * Copyright (c) 2022 liangxiegame UNDER MIT License
 *
 * https://github.com/gwaredd/UnityMarkdownViewer
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

namespace QFramework
{
    internal interface IMDBuilder
    {
        void Text(string text, MDStyle style, string link, string tooltip);
        void Image(string url, string alt, string tooltip);

        void NewLine();
        void Space();
        void HorizontalLine();

        void Indent();
        void Outdent();
        void Prefix(string text, MDStyle style);

        void StartBlock(bool quoted);
        void EndBlock();

        void StartTable();
        void EndTable();

        void StartTableRow(bool isHeader);
        void EndTableRow();
    }
}