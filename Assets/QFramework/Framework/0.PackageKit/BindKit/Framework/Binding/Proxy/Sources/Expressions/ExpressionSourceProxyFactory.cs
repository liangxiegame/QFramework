/*
 * MIT License
 *
 * Copyright (c) 2018 Clark Yang
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
 * SOFTWARE.
 */

using System;
using System.Collections.Generic;
using BindKit.Binding.Paths;
using BindKit.Binding.Proxy.Sources.Object;
using BindKit.Binding.Expressions;

namespace BindKit.Binding.Proxy.Sources.Expressions
{
    public class ExpressionSourceProxyFactory : TypedSourceProxyFactory<ExpressionSourceDescription>
    {
        private ISourceProxyFactory factory;
        private IExpressionPathFinder pathFinder;
        public ExpressionSourceProxyFactory(ISourceProxyFactory factory, IExpressionPathFinder pathFinder)
        {
            this.factory = factory;
            this.pathFinder = pathFinder;
        }

        protected override bool TryCreateProxy(object source, ExpressionSourceDescription description, out ISourceProxy proxy)
        {
            proxy = null;
            var expression = description.Expression;
            List<ISourceProxy> list = new List<ISourceProxy>();
            List<Path> paths = this.pathFinder.FindPaths(expression);
            foreach (Path path in paths)
            {
                ISourceProxy innerProxy = this.factory.CreateProxy(source, new ObjectSourceDescription() { Path = path });
                if (innerProxy != null)
                    list.Add(innerProxy);
            }

#if UNITY_IOS
            Func<object[], object> del = expression.DynamicCompile();
            proxy = new ExpressionSourceProxy(description.IsStatic ? null : source, del, description.ReturnType, list);
#else
            try
            {
                var del = expression.Compile();
                Type returnType = del.ReturnType();
                Type parameterType = del.ParameterType();
                if (parameterType != null)
                {
                    proxy = (ISourceProxy)Activator.CreateInstance(typeof(ExpressionSourceProxy<,>).MakeGenericType(parameterType, returnType), source, del, list);
                }
                else
                {
                    proxy = (ISourceProxy)Activator.CreateInstance(typeof(ExpressionSourceProxy<>).MakeGenericType(returnType), del, list);
                }
            }
            catch (Exception)
            {
                //JIT Exception
                Func<object[], object> del = expression.DynamicCompile();
                proxy = new ExpressionSourceProxy(description.IsStatic ? null : source, del, description.ReturnType, list);
            }
#endif
            if (proxy != null)
                return true;

            return false;
        }
    }
}
