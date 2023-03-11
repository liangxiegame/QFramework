/****************************************************************************
 * Copyright (c) 2015 ~ 2022 liangxiegame UNDER MIT LICENSE
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

namespace QFramework
{
    public class UsingCode : ICode
    {
        private string mNamespace { get; set; }

        public UsingCode(string ns)
        {
            mNamespace = ns;
        }
        
        public void Gen(ICodeWriter writer)
        {
            writer.WriteFormatLine("using {0};", mNamespace);
        }
    }
    
    public static partial class ICodeScopeExtensions
    {
        public static ICodeScope Using(this ICodeScope self,string ns)
        {
            self.Codes.Add(new UsingCode(ns));
            return self;
        }
    }
}