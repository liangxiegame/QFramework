/****************************************************************************
 * Copyright (c) 2015 ~ 2022 liangxiegame UNDER MIT LICENSE
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

namespace QFramework
{
    public class EmptyLineCode : ICode
    {
        public void Gen(ICodeWriter writer)
        {
            writer.WriteLine();
        }
    }

    public static partial class ICodeScopeExtensions
    {
        public static ICodeScope EmptyLine(this ICodeScope self)
        {
            self.Codes.Add(new EmptyLineCode());
            return self;
        }
    }
}