/****************************************************************************
 * Copyright (c) 2015 ~ 2024 liangxiegame UNDER MIT LICENSE
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 *
 * provide by misakiMeiii     https://github.com/misakiMeiii
 ****************************************************************************/

namespace QFramework
{
    public class TabCustomCode : ICode
    {
        private readonly string mLine;

        public TabCustomCode(string line)
        {
            mLine = line;
        }

        public void Gen(ICodeWriter writer)
        {
            writer.WriteLine("\t" + mLine);
        }
    }

    public static partial class ICodeScopeExtensions
    {
        public static ICodeScope TabCustom(this ICodeScope self, string line)
        {
            self.Codes.Add(new TabCustomCode(line));
            return self;
        }
    }
}