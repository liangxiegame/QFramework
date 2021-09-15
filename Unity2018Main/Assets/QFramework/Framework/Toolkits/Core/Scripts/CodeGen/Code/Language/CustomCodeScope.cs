/****************************************************************************
 * Copyright (c) 2017 ~ 2021.1 liangxie
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

namespace QFramework
{
    using System;

    public class CustomCodeScope : CodeScope
    {
        private string mFirstLine { get; set; }

        public CustomCodeScope(string firstLine)
        {
            mFirstLine = firstLine;
        }
        
        protected override void GenFirstLine(ICodeWriter codeWriter)
        {
            codeWriter.WriteLine(mFirstLine);
        }
    }
    
    public static partial class ICodeScopeExtensions
    {
        public static ICodeScope CustomScope(this ICodeScope self,string firstLine,bool semicolon, Action<CustomCodeScope> customCodeScopeSetting)
        {
            var custom = new CustomCodeScope(firstLine);
            custom.Semicolon = semicolon;
            customCodeScopeSetting(custom);
            self.Codes.Add(custom);
            return self;
        }
    }
}