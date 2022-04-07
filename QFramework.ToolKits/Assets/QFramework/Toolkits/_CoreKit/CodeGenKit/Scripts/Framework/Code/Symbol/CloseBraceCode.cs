/****************************************************************************
 * Copyright (c) 2015 ~ 2022 liangxiegame UNDER MIT LICENSE
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

namespace QFramework
{
    /// <summary>
    /// 后花括号
    /// </summary>
    public class CloseBraceCode : ICode
    {
        private readonly bool mSemicolon;

        public CloseBraceCode(bool semicolon)
        {
            mSemicolon = semicolon;
        }

        public void Gen(ICodeWriter writer)
        {
            var semicolonKey = mSemicolon ? ";" : string.Empty;
            writer.WriteLine("}" + semicolonKey);
        }
    }
}