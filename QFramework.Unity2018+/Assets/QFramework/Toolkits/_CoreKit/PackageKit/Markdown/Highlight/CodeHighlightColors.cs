/****************************************************************************
 * Copyright (c) 2026 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
namespace QFramework
{
    /// <summary>
    /// C# 语法高亮配色表(VS Dark 基调)。
    /// 集中于此:换主题 / 改色只动这一处,无需触及 tokenizer。
    /// </summary>
    internal static class CodeHighlightColors
    {
        public const string Keyword = "#569CD6";
        public const string Type    = "#4EC9B0";
        public const string String  = "#D69D85";
        public const string Comment = "#57A64A";
        public const string Number  = "#B5CEA8";
    }
}
#endif
