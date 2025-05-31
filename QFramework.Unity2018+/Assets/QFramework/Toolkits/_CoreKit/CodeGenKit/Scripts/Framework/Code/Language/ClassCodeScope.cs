/****************************************************************************
 * Copyright (c) 2015 ~ 2022 liangxiegame UNDER MIT LICENSE
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
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