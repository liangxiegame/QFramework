/****************************************************************************
 * Copyright (c) 2015 ~ 2022 liangxiegame UNDER MIT LICENSE
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

namespace QFramework
{
    public class CustomCode : ICode
    {
        private string mLine;

        public CustomCode(string line)
        {
            mLine = line;
        }
        
        public void Gen(ICodeWriter writer)
        {
            writer.WriteLine(mLine);
        }
    }
    
    public static partial class ICodeScopeExtensions
    {
        public static ICodeScope Custom(this ICodeScope self, string line)
        {
            self.Codes.Add(new CustomCode(line));
            return self;
        }
    }
    
    
}