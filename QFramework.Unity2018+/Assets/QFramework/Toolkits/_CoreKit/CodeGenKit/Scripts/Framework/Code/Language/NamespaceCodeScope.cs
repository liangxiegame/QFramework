/****************************************************************************
 * Copyright (c) 2015 ~ 2022 liangxiegame UNDER MIT LICENSE
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;

namespace QFramework
{
    public class NamespaceCodeScope : CodeScope
    {
        private string mNamespace { get; set; }
        
        public NamespaceCodeScope(string ns)
        {
            mNamespace = ns;
        }

        protected override void GenFirstLine(ICodeWriter codeWriter)
        {
            //if (!string.IsNullOrWhiteSpace(mNamespace))
            //{
            //    codeWriter.WriteLine(string.Format("namespace {0}", mNamespace));
            //}

            codeWriter.WriteLine(string.Format("namespace {0}", mNamespace));
        }
    }

    public static partial class ICodeScopeExtensions
    {
        public static ICodeScope Namespace(this ICodeScope self, string ns,
            Action<NamespaceCodeScope> namespaceCodeScopeSetting)
        {
            var namespaceCodeScope = new NamespaceCodeScope(ns);
            namespaceCodeScopeSetting(namespaceCodeScope);
            self.Codes.Add(namespaceCodeScope);
            return self;
        }

        public static ICodeScope NewNamespace(this ICodeScope self, string ns,
            ICodeScope codeScope)
        {
            var namespaceCodeScope = new NamespaceCodeScope(ns);
            namespaceCodeScope.Codes = codeScope.Codes;
            self.Codes.Add(namespaceCodeScope);
            return self;
        }
    }
}