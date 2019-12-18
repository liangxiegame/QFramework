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
using System.Collections.Generic;
#if UNITY_IOS || ENABLE_IL2CPP
using BindKit.Binding.Expressions;
#endif

namespace BindKit.Binding.Paths
{
    public class PathParser : IPathParser
    {
        public virtual Path Parse(string pathText)
        {
            return new TextPathParser(pathText.Replace(" ", "")).Parse();
        }

        public virtual Path Parse(LambdaExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            Path path = new Path();
            var body = expression.Body as MemberExpression;
            if (body != null)
            {
                this.Parse(body, path);
                return path;
            }

            var method = expression.Body as MethodCallExpression;
            if (method != null)
            {
                this.Parse(method, path);
                return path;
            }

            var unary = expression.Body as UnaryExpression;
            if (unary != null && unary.NodeType == ExpressionType.Convert)
            {
                this.Parse(unary.Operand, path);
                return path;
            }

            var binary = expression.Body as BinaryExpression;
            if (binary != null && binary.NodeType == ExpressionType.ArrayIndex)
            {
                this.Parse(binary, path);
                return path;
            }

            throw new ArgumentException(string.Format("Invalid expression:{0}", expression));
        }

        private MethodInfo GetDelegateMethodInfo(MethodCallExpression expression)
        {
            var target = expression.Object;
            var arguments = expression.Arguments;
            if (target == null)
            {
                foreach (var expr in arguments)
                {
                    if (!(expr is ConstantExpression))
                        continue;

                    var value = (expr as ConstantExpression).Value;
                    if (value is MethodInfo)
                        return (MethodInfo)value;
                }
                return null;
            }
            else if (target is ConstantExpression)
            {
                var value = (target as ConstantExpression).Value;
                if (value is MethodInfo)
                    return (MethodInfo)value;
            }
            return null;
        }

        private void Parse(Expression expression, Path path)
        {
            if (expression == null || !(expression is MemberExpression || expression is MethodCallExpression || expression is BinaryExpression))
                return;

            if (expression is MemberExpression)
            {
                MemberExpression me = (MemberExpression)expression;
                var memberInfo = me.Member;
                if (memberInfo.IsStatic())
                {
                    path.Prepend(new MemberNode(memberInfo));
                    return;
                }
                else
                {
                    path.Prepend(new MemberNode(memberInfo));
                    if (me.Expression != null)
                        this.Parse(me.Expression, path);
                    return;
                }
            }

            if (expression is MethodCallExpression)
            {
                MethodCallExpression methodCall = (MethodCallExpression)expression;
                if (methodCall.Method.Name.Equals("get_Item") && methodCall.Arguments.Count == 1)
                {
                    var argument = methodCall.Arguments[0];
                    if (!(argument is ConstantExpression))
                        argument = ConvertMemberAccessToConstant(argument);

                    object value = (argument as ConstantExpression).Value;
                    if (value is string)
                    {
                        path.PrependIndexed((string)value);
                    }
                    else if (value is Int32)
                    {
                        path.PrependIndexed((int)value);
                    }
                    if (methodCall.Object != null)
                        this.Parse(methodCall.Object, path);
                    return;
                }

                //Delegate.CreateDelegate(Type type, object firstArgument, MethodInfo method)
                if (methodCall.Method.Name.Equals("CreateDelegate"))
                {
                    var info = this.GetDelegateMethodInfo(methodCall);
                    if (info == null)
                        throw new ArgumentException(string.Format("Invalid expression:{0}", expression));

                    if (info.IsStatic)
                    {
                        path.Prepend(new MemberNode(info));
                        return;
                    }
                    else
                    {
                        path.Prepend(new MemberNode(info));
                        this.Parse(methodCall.Arguments[1], path);
                        return;
                    }
                }

                if (methodCall.Method.ReturnType.Equals(typeof(void)))
                {
                    var info = methodCall.Method;
                    if (info.IsStatic)
                    {
                        path.Prepend(new MemberNode(info));
                        return;
                    }
                    else
                    {
                        path.Prepend(new MemberNode(info));
                        if (methodCall.Object != null)
                            this.Parse(methodCall.Object, path);
                        return;
                    }
                }

                throw new ArgumentException(string.Format("Invalid expression:{0}", expression));
            }

            if (expression is BinaryExpression)
            {
                var binary = expression as BinaryExpression;
                if (binary.NodeType == ExpressionType.ArrayIndex)
                {
                    var left = binary.Left;
                    var right = binary.Right;
                    if (!(right is ConstantExpression))
                        right = ConvertMemberAccessToConstant(right);

                    object value = (right as ConstantExpression).Value;
                    if (value is string)
                    {
                        path.PrependIndexed((string)value);
                    }
                    else if (value is Int32)
                    {
                        path.PrependIndexed((int)value);
                    }

                    if (left != null)
                        this.Parse(left, path);
                    return;
                }

                throw new ArgumentException(string.Format("Invalid expression:{0}", expression));
            }
        }

        private static Expression ConvertMemberAccessToConstant(Expression argument)
        {
            if (argument is ConstantExpression)
                return argument;

            var boxed = Expression.Convert(argument, typeof(object));
#if UNITY_IOS || ENABLE_IL2CPP
            var fun = (Func<object[], object>)Expression.Lambda<Func<object>>(boxed).DynamicCompile();
            var constant = fun(new object[] { });
#else
            var fun = Expression.Lambda<Func<object>>(boxed).Compile();
            var constant = fun();
#endif

            return Expression.Constant(constant);
        }

        public virtual Path ParseStaticPath(LambdaExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            var current = expression.Body;
            var unary = current as UnaryExpression;
            if (unary != null)
                current = unary.Operand;

            if (current is MemberExpression)
            {
                Path path = new Path();
                this.Parse(current, path);
                return path;
            }

            if (current is MethodCallExpression)
            {
                Path path = new Path();
                this.Parse(current, path);
                return path;
            }

            var binary = current as BinaryExpression;
            if (binary != null && binary.NodeType == ExpressionType.ArrayIndex)
            {
                Path path = new Path();
                this.Parse(current, path);
                return path;
            }

            throw new ArgumentException(string.Format("Invalid expression:{0}", expression));
        }

        public virtual Path ParseStaticPath(string pathText)
        {
            string typeName = this.ParserTypeName(pathText);
            string memberName = this.ParserMemberName(pathText);
            Type type = TypeFinderUtils.FindType(typeName);

            Path path = new Path();
            path.Append(new MemberNode(type, memberName, true));
            return path;
        }

        protected string ParserTypeName(string pathText)
        {
            if (pathText == null)
                throw new ArgumentNullException("pathText");

            pathText = pathText.Replace(" ", "");
            if (string.IsNullOrEmpty(pathText))
                throw new ArgumentException("The pathText is empty");

            int index = pathText.LastIndexOf('.');
            if (index <= 0)
                throw new ArgumentException("pathText");

            return pathText.Substring(0, index);
        }

        protected string ParserMemberName(string pathText)
        {
            if (pathText == null)
                throw new ArgumentNullException("pathText");

            pathText = pathText.Replace(" ", "");
            if (string.IsNullOrEmpty(pathText))
                throw new ArgumentException("The pathText is empty");

            int index = pathText.LastIndexOf('.');
            if (index <= 0)
                throw new ArgumentException("pathText");

            return pathText.Substring(index + 1);
        }

        public virtual string ParseMemberName(LambdaExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            var method = expression.Body as MethodCallExpression;
            if (method != null)
                return method.Method.Name;

            //Delegate.CreateDelegate(Type type, object firstArgument, MethodInfo method)
            var unary = expression.Body as UnaryExpression;
            if (unary != null && unary.NodeType == ExpressionType.Convert)
            {
                MethodCallExpression methodCall = (MethodCallExpression)unary.Operand;
                if (methodCall.Method.Name.Equals("CreateDelegate"))
                {
                    var info = this.GetDelegateMethodInfo(methodCall);
                    if (info != null)
                        return info.Name;
                }

                throw new ArgumentException(string.Format("Invalid expression:{0}", expression));
            }

            var body = expression.Body as MemberExpression;
            if (body == null)
                throw new ArgumentException(string.Format("Invalid expression:{0}", expression));

            if (!(body.Expression is ParameterExpression))
                throw new ArgumentException(string.Format("Invalid expression:{0}", expression));

            return body.Member.Name;
        }
    }
}
