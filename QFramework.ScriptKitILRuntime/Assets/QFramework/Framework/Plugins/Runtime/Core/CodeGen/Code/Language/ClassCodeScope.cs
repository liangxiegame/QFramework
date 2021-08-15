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

    public class ClassCodeScope : CodeScope
    {
        public ClassCodeScope(string className, string parentClassName, bool isPartial, bool isStatic)
        {
            mClassName = className;
            mParentClassName = parentClassName;
            mIsPartial = isPartial;
            mIsStatic = isStatic;
        }

        private string mClassName;
        private string mParentClassName;
        private bool mIsPartial;
        private bool mIsStatic;

        protected override void GenFirstLine(ICodeWriter codeWriter)
        {
            var staticKey = mIsStatic ? " static" : string.Empty;
            var partialKey = mIsPartial ? " partial" : string.Empty;
            var parentClassKey = !string.IsNullOrEmpty(mParentClassName) ? " : " + mParentClassName : string.Empty;

            codeWriter.WriteLine(string.Format("public{0}{1} class {2}{3}", staticKey, partialKey, mClassName,
                parentClassKey));
        }
    }
    
    public static partial class ICodeScopeExtensions
    {
        public static ICodeScope Class(this ICodeScope self,string className, string parentClassName, bool isPartial, bool isStatic,Action<ClassCodeScope> classCodeScopeSetting)
        {
            var classCodeScope = new ClassCodeScope(className,parentClassName,isPartial,isStatic);
            classCodeScopeSetting(classCodeScope);
            self.Codes.Add(classCodeScope);
            return self;
        }
    }
}