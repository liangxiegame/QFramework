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
using System.Reflection;
using System.Linq.Expressions;

namespace BindKit.Binding.Proxy.Sources.Expressions
{
    public class ExpressionSourceDescription : SourceDescription
    {
        private LambdaExpression expression;

        private Type returnType;

        public ExpressionSourceDescription()
        {
        }

        public LambdaExpression Expression
        {
            get { return this.expression; }
            set
            {
                this.expression = value;

                Type[] types = expression.GetType().GetGenericArguments();
                var delType = types[0];

                if (!typeof(Delegate).IsAssignableFrom(delType))
                    throw new NotSupportedException();

                MethodInfo info = delType.GetMethod("Invoke");

                this.returnType = info.ReturnType;

                ParameterInfo[] parameters = info.GetParameters();
                this.IsStatic = (parameters == null || parameters.Length <= 0) ? true : false;
            }
        }

        public Type ReturnType { get { return this.returnType; } }

        public override string ToString()
        {
            return this.expression == null ? "Expression:null" : "Expression:" + this.expression.ToString();
        }
    }
}
