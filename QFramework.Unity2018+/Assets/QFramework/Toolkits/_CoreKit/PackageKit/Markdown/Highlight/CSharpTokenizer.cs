/****************************************************************************
 * Copyright (c) 2026 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System.Collections.Generic;
using System.Text;

namespace QFramework
{
    /// <summary>
    /// 轻量 C# 词法高亮器。
    /// 逐行喂入,内部状态机跨行保持(块注释 / verbatim 字符串),
    /// 产出 Unity IMGUI richText(&lt;color&gt;)字符串。
    /// 在 String / Comment 状态下不识别关键字,从根上杜绝
    /// "注释/字符串内的关键字被误伤"与 &lt;color&gt; 标签嵌套破坏。
    /// </summary>
    internal class CSharpTokenizer
    {
        private enum State
        {
            Normal,
            BlockComment,
            StringVerbatim,
        }

        private State mState = State.Normal;

        private static readonly HashSet<string> Keywords = new HashSet<string>
        {
            "abstract","as","base","bool","break","byte","case","catch","char","checked",
            "class","const","continue","decimal","default","delegate","do","double","else",
            "enum","event","explicit","extern","false","finally","fixed","float","for",
            "foreach","get","goto","if","implicit","in","int","interface","internal","is",
            "lock","long","namespace","new","null","object","operator","out","override",
            "params","private","protected","public","readonly","ref","return","sbyte",
            "sealed","set","short","sizeof","stackalloc","static","string","struct",
            "switch","this","throw","true","try","typeof","uint","ulong","unchecked",
            "unsafe","ushort","using","var","virtual","void","volatile","while",
            "async","await","yield","nameof","when","where","partial","global","value",
            "add","remove","alias","ascending","descending","group","into","join","let",
            "orderby","select","by","on","equals","from",
        };

        /// <summary>
        /// 高亮一行代码,返回带 &lt;color&gt; 标签的 richText(不含换行)。
        /// 多行块注释 / verbatim 字符串的状态在多次调用间保持。
        /// </summary>
        public string HighlightLine(string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return string.Empty;
            }

            var sb = new StringBuilder(line.Length + 32);
            int i = 0;
            int n = line.Length;

            // 每行开始时重置类型名期望状态
            mExpectTypeName = false;

            while (i < n)
            {
                switch (mState)
                {
                    case State.BlockComment:
                        i = ReadBlockCommentRest(sb, line, i, n);
                        break;
                    case State.StringVerbatim:
                        i = ReadVerbatimStringRest(sb, line, i, n);
                        break;
                    default:
                        i = ReadNormal(sb, line, i, n);
                        break;
                }
            }

            return sb.ToString();
        }

        private int ReadNormal(StringBuilder sb, string line, int i, int n)
        {
            char c = line[i];

            // 空白(透传,不着色)
            if (c == ' ' || c == '\t')
            {
                int start = i;
                while (i < n && (line[i] == ' ' || line[i] == '\t')) i++;
                sb.Append(line, start, i - start);
                return i;
            }

            // 字符串字面量: " / @" / $" / $@" / @$"
            if (TryMatchStringStart(line, i, n, out int prefixLen, out bool verbatim))
            {
                return ReadString(sb, line, i, n, prefixLen, verbatim);
            }

            // 行注释 //
            if (c == '/' && i + 1 < n && line[i + 1] == '/')
            {
                AppendColored(sb, CodeHighlightColors.Comment, line.Substring(i));
                return n;
            }

            // 块注释 /*
            if (c == '/' && i + 1 < n && line[i + 1] == '*')
            {
                int end = line.IndexOf("*/", i + 2, System.StringComparison.Ordinal);
                if (end < 0)
                {
                    AppendColored(sb, CodeHighlightColors.Comment, line.Substring(i));
                    mState = State.BlockComment;
                    return n;
                }
                int close = end + 2;
                AppendColored(sb, CodeHighlightColors.Comment, line.Substring(i, close - i));
                return close;
            }

            // 数字
            if (c >= '0' && c <= '9')
            {
                int start = i;
                i++;
                while (i < n && IsNumberChar(line[i])) i++;
                while (i < n && IsSuffixChar(line[i])) i++;
                AppendColored(sb, CodeHighlightColors.Number, line.Substring(start, i - start));
                return i;
            }

            // 标识符 / 关键字 / 类型(含 @ 前缀 verbatim 标识符)
            if (char.IsLetter(c) || c == '_' || c == '@')
            {
                int start = i;
                if (c == '@') i++;
                while (i < n && (char.IsLetterOrDigit(line[i]) || line[i] == '_')) i++;
                ClassifyIdentifier(sb, line, start, i);
                return i;
            }

            // 其他:运算符 / 标点(原样,默认色)
            sb.Append(c);
            return i + 1;
        }

        private bool TryMatchStringStart(string line, int i, int n, out int prefixLen, out bool verbatim)
        {
            char c = line[i];
            prefixLen = 0;
            verbatim = false;

            if (c == '"') { prefixLen = 1; verbatim = false; return true; }
            if (c == '@' && i + 1 < n && line[i + 1] == '"') { prefixLen = 2; verbatim = true; return true; }
            if (c == '$' && i + 1 < n && line[i + 1] == '"') { prefixLen = 2; verbatim = false; return true; }
            if (c == '$' && i + 2 < n && line[i + 1] == '@' && line[i + 2] == '"') { prefixLen = 3; verbatim = true; return true; }
            if (c == '@' && i + 2 < n && line[i + 1] == '$' && line[i + 2] == '"') { prefixLen = 3; verbatim = true; return true; }
            return false;
        }

        private int ReadString(StringBuilder sb, string line, int i, int n, int prefixLen, bool verbatim)
        {
            int j = i + prefixLen; // 跳过前缀(含开引号)

            if (verbatim)
            {
                while (j < n)
                {
                    if (line[j] == '"')
                    {
                        if (j + 1 < n && line[j + 1] == '"') { j += 2; continue; } // "" 转义
                        j++; // 闭合引号
                        AppendColored(sb, CodeHighlightColors.String, line.Substring(i, j - i));
                        return j;
                    }
                    j++;
                }
                // 跨行未闭合
                AppendColored(sb, CodeHighlightColors.String, line.Substring(i));
                mState = State.StringVerbatim;
                return n;
            }

            // 普通字符串:处理 \ 转义,本行闭合(普通字面量不跨物理行)
            while (j < n)
            {
                if (line[j] == '\\') { j += 2; continue; }
                if (line[j] == '"') break;
                j++;
            }
            int close = (j < n) ? j + 1 : n;
            AppendColored(sb, CodeHighlightColors.String, line.Substring(i, close - i));
            return close;
        }

        private int ReadBlockCommentRest(StringBuilder sb, string line, int i, int n)
        {
            int start = i;
            int end = line.IndexOf("*/", i, System.StringComparison.Ordinal);
            if (end < 0)
            {
                AppendColored(sb, CodeHighlightColors.Comment, line.Substring(start));
                return n; // 仍处块注释,状态保持
            }
            int close = end + 2;
            AppendColored(sb, CodeHighlightColors.Comment, line.Substring(start, close - start));
            mState = State.Normal;
            return close;
        }

        private int ReadVerbatimStringRest(StringBuilder sb, string line, int i, int n)
        {
            int start = i;
            while (i < n)
            {
                if (line[i] == '"')
                {
                    if (i + 1 < n && line[i + 1] == '"') { i += 2; continue; } // "" 转义
                    i++; // 闭合
                    AppendColored(sb, CodeHighlightColors.String, line.Substring(start, i - start));
                    mState = State.Normal;
                    return i;
                }
                i++;
            }
            AppendColored(sb, CodeHighlightColors.String, line.Substring(start));
            return n; // 仍处 verbatim 字符串,状态保持
        }

        private bool mExpectTypeName = false; // 是否期望下一个标识符是类型名

        private void ClassifyIdentifier(StringBuilder sb, string line, int start, int end)
        {
            // 去掉 @ 前缀后再判定关键字
            int s = start;
            if (s < end && line[s] == '@') s++;

            bool isKeyword = false;

            if (end > s)
            {
                string key = line.Substring(s, end - s);
                isKeyword = Keywords.Contains(key);

                // 如果是 class/struct/interface/enum 关键字，下一个标识符是类型名
                if (isKeyword && (key == "class" || key == "struct" || key == "interface" || key == "enum"))
                {
                    mExpectTypeName = true;
                }
            }

            if (isKeyword)
            {
                AppendColored(sb, CodeHighlightColors.Keyword, line, start, end);
            }
            else if (mExpectTypeName)
            {
                // 类型名声明:青色
                AppendColored(sb, CodeHighlightColors.Type, line, start, end);
                mExpectTypeName = false;
            }
            else
            {
                // 普通标识符(成员方法、成员变量等):默认色,原样追加
                sb.Append(line, start, end - start);
            }
        }

        private static bool IsNumberChar(char c)
        {
            return (c >= '0' && c <= '9')
                || (c >= 'a' && c <= 'f')
                || (c >= 'A' && c <= 'F')
                || c == '.' || c == 'x' || c == 'X' || c == '_';
        }

        private static bool IsSuffixChar(char c)
        {
            return c == 'f' || c == 'F' || c == 'u' || c == 'U'
                || c == 'l' || c == 'L' || c == 'd' || c == 'D' || c == 'm' || c == 'M';
        }

        private static void AppendColored(StringBuilder sb, string color, string text)
        {
            sb.Append("<color=").Append(color).Append(">");
            sb.Append(text);
            sb.Append("</color>");
        }

        private static void AppendColored(StringBuilder sb, string color, string line, int start, int end)
        {
            sb.Append("<color=").Append(color).Append(">");
            sb.Append(line, start, end - start);
            sb.Append("</color>");
        }
    }
}
#endif
