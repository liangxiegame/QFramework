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
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace BindKit.Binding.Expressions
{
    abstract class ExpressionVisitor
    {
        public virtual Expression Visit(Expression expr)
        {
            if (expr == null)
                return null;

            switch (expr.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    return this.VisitUnary((UnaryExpression)expr);

                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                    return this.VisitBinary((BinaryExpression)expr);

                case ExpressionType.Call:
                    return this.VisitMethodCall((MethodCallExpression)expr);

                case ExpressionType.Invoke:
                    return this.VisitInvocation((InvocationExpression)expr);

                case ExpressionType.MemberAccess:
                    return this.VisitMember((MemberExpression)expr);

                case ExpressionType.TypeIs:
                    return this.VisitTypeBinary((TypeBinaryExpression)expr);

                case ExpressionType.Lambda:
                    return this.VisitLambda((LambdaExpression)expr);

                case ExpressionType.Conditional:
                    return this.VisitConditional((ConditionalExpression)expr);

                case ExpressionType.Constant:
                    return this.VisitConstant((ConstantExpression)expr);

                case ExpressionType.Parameter:
                    return this.VisitParameter((ParameterExpression)expr);
                case ExpressionType.NewArrayInit:
                    return this.VisitNewArrayInit((NewArrayExpression)expr);
                //case ExpressionType.New:
                //return this.VisitNew((NewExpression)expr);
                //case ExpressionType.NewArrayBounds:
                //    return this.VisitNewArray((NewArrayExpression)expr);
                //case ExpressionType.MemberInit:
                //    return this.VisitMemberInit((MemberInitExpression)expr);
                //case ExpressionType.ListInit:
                //    return this.VisitListInit((ListInitExpression)expr);
                default:
                    throw new NotSupportedException("Expressions of type " + expr.Type + " are not supported.");
            }
        }

        protected virtual ReadOnlyCollection<T> VisitExpressionList<T>(ReadOnlyCollection<T> original) where T : Expression
        {
            List<T> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                Expression p = this.Visit(original[i]);
                if (list != null)
                {
                    list.Add((T)p);
                }
                else if (p != original[i])
                {
                    list = new List<T>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }
                    list.Add((T)p);
                }
            }

            if (list != null)
                return list.AsReadOnly();
            return original;
        }

        protected virtual Expression VisitBinary(BinaryExpression expr)
        {
            Expression left = this.Visit(expr.Left);
            Expression right = this.Visit(expr.Right);
            Expression conversion = this.Visit(expr.Conversion);

            if (left != expr.Left || right != expr.Right || conversion != expr.Conversion)
            {
                if (expr.NodeType == ExpressionType.Coalesce && expr.Conversion != null)
                    return Expression.Coalesce(left, right, conversion as LambdaExpression);
                else
                    return Expression.MakeBinary(expr.NodeType, left, right, expr.IsLiftedToNull, expr.Method);
            }
            return expr;
        }

        protected virtual Expression VisitConditional(ConditionalExpression expr)
        {
            Expression test = this.Visit(expr.Test);
            Expression ifTrue = this.Visit(expr.IfTrue);
            Expression ifFalse = this.Visit(expr.IfFalse);
            if (test != expr.Test || ifTrue != expr.IfTrue || ifFalse != expr.IfFalse)
                return Expression.Condition(test, ifTrue, ifFalse);
            return expr;
        }

        protected virtual Expression VisitLambda(LambdaExpression expr)
        {
            Expression body = this.Visit(expr.Body);
            IEnumerable<ParameterExpression> parameters = VisitExpressionList(expr.Parameters);
            if (body != expr.Body || parameters != expr.Parameters)
                return Expression.Lambda(expr.Type, body, parameters);
            return expr;
        }

        protected virtual Expression VisitInvocation(InvocationExpression expr)
        {
            IEnumerable<Expression> args = VisitExpressionList(expr.Arguments);
            Expression expression = this.Visit(expr.Expression);
            if (args != expr.Arguments || expression != expr.Expression)
                return Expression.Invoke(expression, args);
            return expr;
        }

        protected virtual Expression VisitMember(MemberExpression expr)
        {
            Expression expression = this.Visit(expr.Expression);
            if (expression != expr.Expression)
                return Expression.MakeMemberAccess(expression, expr.Member);
            return expr;
        }

        protected virtual Expression VisitMethodCall(MethodCallExpression expr)
        {
            Expression obj = this.Visit(expr.Object);
            IEnumerable<Expression> args = VisitExpressionList(expr.Arguments);
            if (obj != expr.Object || args != expr.Arguments)
                return Expression.Call(obj, expr.Method, args);
            return expr;
        }

        protected virtual Expression VisitUnary(UnaryExpression expr)
        {
            Expression operand = this.Visit(expr.Operand);
            if (operand != expr.Operand)
                return Expression.MakeUnary(expr.NodeType, operand, expr.Type, expr.Method);
            return expr;
        }

        protected virtual Expression VisitTypeBinary(TypeBinaryExpression expr)
        {
            Expression expression = this.Visit(expr.Expression);
            if (expression != expr.Expression)
                return Expression.TypeIs(expression, expr.TypeOperand);
            return expr;
        }

        protected virtual Expression VisitNewArrayInit(NewArrayExpression expr)
        {
            IEnumerable<Expression> args = VisitExpressionList(expr.Expressions);
            if (args != expr.Expressions)
                return Expression.NewArrayInit(expr.Type, args);
            return expr;
        }

        protected virtual Expression VisitConstant(ConstantExpression expr)
        {
            return expr;
        }

        protected virtual Expression VisitParameter(ParameterExpression expr)
        {
            return expr;
        }
    }
}
