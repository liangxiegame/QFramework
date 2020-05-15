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
using System.Linq;
#if UNITY_IOS || ENABLE_IL2CPP
using BindKit.Binding.Expressions;
#endif

namespace BindKit.Binding.Paths
{
    public class PathExpressionVisitor
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(PathExpressionVisitor));

        private readonly List<Path> list = new List<Path>();

        public List<Path> Paths { get { return list; } }

        public virtual Expression Visit(Expression expression)
        {
            if (expression == null)
                return null;

            var bin = expression as BinaryExpression;
            if (bin != null)
                return VisitBinary(bin);

            var cond = expression as ConditionalExpression;
            if (cond != null)
                return VisitConditional(cond);

            var constant = expression as ConstantExpression;
            if (constant != null)
                return VisitConstant(constant);

            var lambda = expression as LambdaExpression;
            if (lambda != null)
                return VisitLambda(lambda);

            var listInit = expression as ListInitExpression;
            if (listInit != null)
                return VisitListInit(listInit);

            var member = expression as MemberExpression;
            if (member != null)
                return VisitMember(member);

            var memberInit = expression as MemberInitExpression;
            if (memberInit != null)
                return VisitMemberInit(memberInit);

            var methodCall = expression as MethodCallExpression;
            if (methodCall != null)
                return VisitMethodCall(methodCall);

            var newExpr = expression as NewExpression;
            if (newExpr != null)
                return VisitNew(newExpr);

            var newArrayExpr = expression as NewArrayExpression;
            if (newArrayExpr != null)
                return VisitNewArray(newArrayExpr);

            var param = expression as ParameterExpression;
            if (param != null)
                return VisitParameter(param);

            var typeBinary = expression as TypeBinaryExpression;
            if (typeBinary != null)
                return VisitTypeBinary(typeBinary);

            var unary = expression as UnaryExpression;
            if (unary != null)
                return VisitUnary(unary);

            var invocation = expression as InvocationExpression;
            if (invocation != null)
                return VisitInvocation(invocation);

            throw new NotSupportedException("Expressions of type " + expression.Type + " are not supported.");
        }

        protected virtual void Visit(IList<Expression> nodes)
        {
            if (nodes == null || nodes.Count <= 0)
                return;

            foreach (Expression expression in nodes)
                Visit(expression);
        }

        protected virtual Expression VisitLambda(LambdaExpression node)
        {
            if (node.Parameters != null)
                Visit(node.Parameters.Select(p => (Expression)p).ToList());
            return Visit(node.Body);
        }

        protected virtual Expression VisitBinary(BinaryExpression node)
        {
            if (node.NodeType == ExpressionType.ArrayIndex)
            {
                this.Visit(this.ParseMemberPath(node, null, this.list));
            }
            else
            {
                List<Expression> list = new List<Expression>() { node.Left, node.Right, node.Conversion };
                this.Visit(list);
            }
            return null;
        }

        protected virtual Expression VisitConditional(ConditionalExpression node)
        {
            List<Expression> list = new List<Expression>() { node.IfFalse, node.IfTrue, node.Test };
            this.Visit(list);
            return null;
        }

        protected virtual Expression VisitConstant(ConstantExpression node)
        {
            return null;
        }

        protected virtual void VisitElementInit(ElementInit init)
        {
            if (init == null)
                return;

            this.Visit(init.Arguments);
        }

        protected virtual Expression VisitListInit(ListInitExpression node)
        {
            if (node.Initializers != null)
            {
                foreach (ElementInit init in node.Initializers)
                {
                    VisitElementInit(init);
                }
            }
            return Visit(node.NewExpression);
        }

        protected virtual Expression VisitMember(MemberExpression node)
        {
            this.Visit(ParseMemberPath(node, null, this.list));
            return null;
        }

        protected virtual Expression VisitInvocation(InvocationExpression node)
        {
            this.Visit(node.Arguments);
            return this.Visit(node.Expression);
        }

        protected virtual Expression VisitMemberInit(MemberInitExpression expr)
        {
            return Visit(expr.NewExpression);
        }

        protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding)
        {
            return null;
        }

        protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            return null;
        }

        protected virtual Expression VisitMethodCall(MethodCallExpression node)
        {
            this.Visit(ParseMemberPath(node, null, this.list));
            return null;
        }

        protected virtual Expression VisitNew(NewExpression expr)
        {
            Visit(expr.Arguments);
            return null;
        }

        protected virtual Expression VisitNewArray(NewArrayExpression node)
        {
            this.Visit(node.Expressions);
            return null;
        }

        protected virtual Expression VisitParameter(ParameterExpression node)
        {
            return null;
        }

        protected virtual Expression VisitTypeBinary(TypeBinaryExpression node)
        {
            return Visit(node.Expression);
        }

        protected virtual Expression VisitUnary(UnaryExpression node)
        {
            return Visit(node.Operand);
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

        private IList<Expression> ParseMemberPath(Expression expression, Path path, IList<Path> list)
        {
            if (expression.NodeType != ExpressionType.MemberAccess && expression.NodeType != ExpressionType.Call && expression.NodeType != ExpressionType.ArrayIndex)
                throw new Exception();

            List<Expression> result = new List<Expression>();

            Expression current = expression;
            while (current != null && (current is MemberExpression || current is MethodCallExpression || current is BinaryExpression || current is ParameterExpression || current is ConstantExpression))
            {
                if (current is MemberExpression)
                {
                    if (path == null)
                    {
                        path = new Path();
                        list.Add(path);
                    }

                    MemberExpression me = (MemberExpression)current;
                    var field = me.Member as FieldInfo;
                    if (field != null)
                    {
                        //static or instance
                        path.Prepend(new MemberNode(field));
                    }

                    var property = me.Member as PropertyInfo;
                    if (property != null)
                    {
                        //static or instance
                        path.Prepend(new MemberNode(property));
                    }

                    current = me.Expression;
                }
                else if (current is MethodCallExpression)
                {
                    MethodCallExpression mc = (MethodCallExpression)current;
                    if (mc.Method.Name.Equals("get_Item") && mc.Arguments.Count == 1)
                    {
                        if (path == null)
                        {
                            path = new Path();
                            list.Add(path);
                        }

                        var argument = mc.Arguments[0];
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

                        current = mc.Object;
                    }
                    else
                    {
                        current = null;
                        result.AddRange(mc.Arguments);
                        result.Add(mc.Object);
                    }
                }
                else if (current is BinaryExpression)
                {
                    var binary = current as BinaryExpression;
                    if (binary.NodeType == ExpressionType.ArrayIndex)
                    {
                        if (path == null)
                        {
                            path = new Path();
                            list.Add(path);
                        }

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

                        current = left;
                    }
                    else
                    {
                        current = null;
                    }
                }
                else if (current is ParameterExpression)
                {
                    current = null;
                }
                else if (current is ConstantExpression)
                {
                    current = null;
                }
            }

            if (current != null)
                result.Add(current);

            return result;
        }
    }
}
