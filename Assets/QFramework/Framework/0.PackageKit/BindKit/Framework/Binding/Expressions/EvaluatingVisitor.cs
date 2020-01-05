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
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace BindKit.Binding.Expressions
{
    class Scope
    {
        private readonly Scope parent;
        private readonly Dictionary<ParameterExpression, object> values = new Dictionary<ParameterExpression, object>();

        public Scope()
        {
            parent = null;
        }
        public Scope(Scope parent)
        {
            this.parent = parent;
        }
        public bool ContainsKey(ParameterExpression key)
        {
            return values.ContainsKey(key) || (parent != null && parent.ContainsKey(key));
        }

        public object this[ParameterExpression key]
        {
            get
            {
                object result;
                if (values.TryGetValue(key, out result))
                    return result;

                if (parent != null)
                    return parent[key];

                throw new InvalidOperationException("Parameter not defined.");
            }
            set
            {
                if (values.ContainsKey(key))
                {
                    values[key] = value;
                    return;
                }

                if (parent != null)
                {
                    parent[key] = value;
                    return;
                }

                throw new KeyNotFoundException();
            }
        }

        public void Register(ParameterExpression expr, object value)
        {
            values[expr] = value;
        }
    }

    class ParameterReplacer : ExpressionVisitor
    {
        private readonly Scope scope;

        public ParameterReplacer(Scope scope)
        {
            this.scope = scope;
        }

        protected override Expression VisitParameter(ParameterExpression expr)
        {
            if (scope.ContainsKey(expr))
            {
                var boxType = typeof(StrongBox<>).MakeGenericType(expr.Type);
                return Expression.Field(Expression.Constant(Activator.CreateInstance(boxType, scope[expr]), boxType), "Value");
            }
            return base.VisitParameter(expr);
        }
    }

    class EvaluatingVisitor : ExpressionVisitor
    {
        private Scope values = new Scope();

        private object BinaryOperate(ExpressionType exprType, TypeCode typeCode, object left, object right)
        {
            switch (exprType)
            {
                case ExpressionType.Add:
                    {
                        switch (typeCode)
                        {
                            case TypeCode.SByte: return (sbyte)left + (sbyte)right;
                            case TypeCode.Byte: return (byte)left + (byte)right;
                            case TypeCode.Int16: return (short)left + (short)right;
                            case TypeCode.UInt16: return (ushort)left + (ushort)right;
                            case TypeCode.Int32: return (int)left + (int)right;
                            case TypeCode.UInt32: return (uint)left + (uint)right;
                            case TypeCode.Int64: return (long)left + (long)right;
                            case TypeCode.UInt64: return (ulong)left + (ulong)right;
                            case TypeCode.Char: return (char)left + (char)right;
                            case TypeCode.Single: return (float)left + (float)right;
                            case TypeCode.Double: return (double)left + (double)right;
                            case TypeCode.Decimal: return (decimal)left + (decimal)right;
                        }
                        break;
                    }
                case ExpressionType.AddChecked:
                    {
                        switch (typeCode)
                        {
                            case TypeCode.SByte: return checked((sbyte)left + (sbyte)right);
                            case TypeCode.Byte: return checked((byte)left + (byte)right);
                            case TypeCode.Int16: return checked((short)left + (short)right);
                            case TypeCode.UInt16: return checked((ushort)left + (ushort)right);
                            case TypeCode.Int32: return checked((int)left + (int)right);
                            case TypeCode.UInt32: return checked((uint)left + (uint)right);
                            case TypeCode.Int64: return checked((long)left + (long)right);
                            case TypeCode.UInt64: return checked((ulong)left + (ulong)right);
                            case TypeCode.Char: return checked((char)left + (char)right);
                            case TypeCode.Single: return checked((float)left + (float)right);
                            case TypeCode.Double: return checked((double)left + (double)right);
                            case TypeCode.Decimal: return checked((decimal)left + (decimal)right);
                        }
                        break;
                    }
                case ExpressionType.Subtract:
                    {
                        switch (typeCode)
                        {
                            case TypeCode.SByte: return (sbyte)left - (sbyte)right;
                            case TypeCode.Byte: return (byte)left - (byte)right;
                            case TypeCode.Int16: return (short)left - (short)right;
                            case TypeCode.UInt16: return (ushort)left - (ushort)right;
                            case TypeCode.Int32: return (int)left - (int)right;
                            case TypeCode.UInt32: return (uint)left - (uint)right;
                            case TypeCode.Int64: return (long)left - (long)right;
                            case TypeCode.UInt64: return (ulong)left - (ulong)right;
                            case TypeCode.Char: return (char)left - (char)right;
                            case TypeCode.Single: return (float)left - (float)right;
                            case TypeCode.Double: return (double)left - (double)right;
                            case TypeCode.Decimal: return (decimal)left - (decimal)right;
                        }
                        break;
                    }
                case ExpressionType.SubtractChecked:
                    {
                        switch (typeCode)
                        {
                            case TypeCode.SByte: return checked((sbyte)left - (sbyte)right);
                            case TypeCode.Byte: return checked((byte)left - (byte)right);
                            case TypeCode.Int16: return checked((short)left - (short)right);
                            case TypeCode.UInt16: return checked((ushort)left - (ushort)right);
                            case TypeCode.Int32: return checked((int)left - (int)right);
                            case TypeCode.UInt32: return checked((uint)left - (uint)right);
                            case TypeCode.Int64: return checked((long)left - (long)right);
                            case TypeCode.UInt64: return checked((ulong)left - (ulong)right);
                            case TypeCode.Char: return checked((char)left - (char)right);
                            case TypeCode.Single: return checked((float)left - (float)right);
                            case TypeCode.Double: return checked((double)left - (double)right);
                            case TypeCode.Decimal: return checked((decimal)left - (decimal)right);
                        }
                        break;
                    }
                case ExpressionType.Multiply:
                    {
                        switch (typeCode)
                        {
                            case TypeCode.SByte: return (sbyte)left * (sbyte)right;
                            case TypeCode.Byte: return (byte)left * (byte)right;
                            case TypeCode.Int16: return (short)left * (short)right;
                            case TypeCode.UInt16: return (ushort)left * (ushort)right;
                            case TypeCode.Int32: return (int)left * (int)right;
                            case TypeCode.UInt32: return (uint)left * (uint)right;
                            case TypeCode.Int64: return (long)left * (long)right;
                            case TypeCode.UInt64: return (ulong)left * (ulong)right;
                            case TypeCode.Char: return (char)left * (char)right;
                            case TypeCode.Single: return (float)left * (float)right;
                            case TypeCode.Double: return (double)left * (double)right;
                            case TypeCode.Decimal: return (decimal)left * (decimal)right;
                        }
                        break;
                    }
                case ExpressionType.MultiplyChecked:
                    {
                        switch (typeCode)
                        {
                            case TypeCode.SByte: return checked((sbyte)left * (sbyte)right);
                            case TypeCode.Byte: return checked((byte)left * (byte)right);
                            case TypeCode.Int16: return checked((short)left * (short)right);
                            case TypeCode.UInt16: return checked((ushort)left * (ushort)right);
                            case TypeCode.Int32: return checked((int)left * (int)right);
                            case TypeCode.UInt32: return checked((uint)left * (uint)right);
                            case TypeCode.Int64: return checked((long)left * (long)right);
                            case TypeCode.UInt64: return checked((ulong)left * (ulong)right);
                            case TypeCode.Char: return checked((char)left * (char)right);
                            case TypeCode.Single: return checked((float)left * (float)right);
                            case TypeCode.Double: return checked((double)left * (double)right);
                            case TypeCode.Decimal: return checked((decimal)left * (decimal)right);
                        }
                        break;
                    }
                case ExpressionType.Divide:
                    {
                        switch (typeCode)
                        {
                            case TypeCode.SByte: return (sbyte)left / (sbyte)right;
                            case TypeCode.Byte: return (byte)left / (byte)right;
                            case TypeCode.Int16: return (short)left / (short)right;
                            case TypeCode.UInt16: return (ushort)left / (ushort)right;
                            case TypeCode.Int32: return (int)left / (int)right;
                            case TypeCode.UInt32: return (uint)left / (uint)right;
                            case TypeCode.Int64: return (long)left / (long)right;
                            case TypeCode.UInt64: return (ulong)left / (ulong)right;
                            case TypeCode.Char: return (char)left / (char)right;
                            case TypeCode.Single: return (float)left / (float)right;
                            case TypeCode.Double: return (double)left / (double)right;
                            case TypeCode.Decimal: return (decimal)left / (decimal)right;
                        }
                        break;
                    }
                case ExpressionType.Modulo:
                    {
                        switch (typeCode)
                        {
                            case TypeCode.SByte: return (sbyte)left % (sbyte)right;
                            case TypeCode.Byte: return (byte)left % (byte)right;
                            case TypeCode.Int16: return (short)left % (short)right;
                            case TypeCode.UInt16: return (ushort)left % (ushort)right;
                            case TypeCode.Int32: return (int)left % (int)right;
                            case TypeCode.UInt32: return (uint)left % (uint)right;
                            case TypeCode.Int64: return (long)left % (long)right;
                            case TypeCode.UInt64: return (ulong)left % (ulong)right;
                            case TypeCode.Char: return (char)left % (char)right;
                            case TypeCode.Single: return (float)left % (float)right;
                            case TypeCode.Double: return (double)left % (double)right;
                            case TypeCode.Decimal: return (decimal)left % (decimal)right;
                        }
                        break;
                    }
                case ExpressionType.And:
                    {
                        switch (typeCode)
                        {
                            case TypeCode.SByte: return (sbyte)left & (sbyte)right;
                            case TypeCode.Byte: return (byte)left & (byte)right;
                            case TypeCode.Int16: return (short)left & (short)right;
                            case TypeCode.UInt16: return (ushort)left & (ushort)right;
                            case TypeCode.Int32: return (int)left & (int)right;
                            case TypeCode.UInt32: return (uint)left & (uint)right;
                            case TypeCode.Int64: return (long)left & (long)right;
                            case TypeCode.UInt64: return (ulong)left & (ulong)right;
                            case TypeCode.Char: return (char)left & (char)right;
                            case TypeCode.Boolean: return (bool)left & (bool)right;
                        }
                        break;
                    }
                case ExpressionType.Or:
                    {
                        switch (typeCode)
                        {
                            case TypeCode.SByte: return Convert.ToByte(left) | Convert.ToByte(right);
                            case TypeCode.Byte: return (byte)left | (byte)right;
                            case TypeCode.Int16: return (short)left | (short)right;
                            case TypeCode.UInt16: return (ushort)left | (ushort)right;
                            case TypeCode.Int32: return (int)left | (int)right;
                            case TypeCode.UInt32: return (uint)left | (uint)right;
                            case TypeCode.Int64: return (long)left | (long)right;
                            case TypeCode.UInt64: return (ulong)left | (ulong)right;
                            case TypeCode.Char: return (char)left | (char)right;
                            case TypeCode.Boolean: return (bool)left | (bool)right;
                        }
                        break;
                    }
                case ExpressionType.ExclusiveOr:
                    {
                        switch (typeCode)
                        {
                            case TypeCode.SByte: return (sbyte)left ^ (sbyte)right;
                            case TypeCode.Byte: return (byte)left ^ (byte)right;
                            case TypeCode.Int16: return (short)left ^ (short)right;
                            case TypeCode.UInt16: return (ushort)left ^ (ushort)right;
                            case TypeCode.Int32: return (int)left ^ (int)right;
                            case TypeCode.UInt32: return (uint)left ^ (uint)right;
                            case TypeCode.Int64: return (long)left ^ (long)right;
                            case TypeCode.UInt64: return (ulong)left ^ (ulong)right;
                            case TypeCode.Char: return (char)left ^ (char)right;
                            case TypeCode.Boolean: return (bool)left ^ (bool)right;
                        }
                        break;
                    }
                case ExpressionType.AndAlso:
                    {
                        switch (typeCode)
                        {
                            case TypeCode.Boolean: return (bool)left && (bool)right;
                        }
                        break;
                    }
                case ExpressionType.OrElse:
                    {
                        switch (typeCode)
                        {
                            case TypeCode.Boolean: return (bool)left || (bool)right;
                        }
                        break;
                    }
                case ExpressionType.Equal:
                    {
                        switch (typeCode)
                        {
                            case TypeCode.SByte: return (sbyte)left == (sbyte)right;
                            case TypeCode.Byte: return (byte)left == (byte)right;
                            case TypeCode.Int16: return (short)left == (short)right;
                            case TypeCode.UInt16: return (ushort)left == (ushort)right;
                            case TypeCode.Int32: return (int)left == (int)right;
                            case TypeCode.UInt32: return (uint)left == (uint)right;
                            case TypeCode.Int64: return (long)left == (long)right;
                            case TypeCode.UInt64: return (ulong)left == (ulong)right;
                            case TypeCode.Char: return (char)left == (char)right;
                            case TypeCode.Single: return (float)left == (float)right;
                            case TypeCode.Double: return (double)left == (double)right;
                            case TypeCode.Decimal: return (decimal)left == (decimal)right;
                            case TypeCode.Boolean: return (bool)left == (bool)right;
                        }
                        break;
                    }
                case ExpressionType.NotEqual:
                    {
                        switch (typeCode)
                        {
                            case TypeCode.SByte: return (sbyte)left != (sbyte)right;
                            case TypeCode.Byte: return (byte)left != (byte)right;
                            case TypeCode.Int16: return (short)left != (short)right;
                            case TypeCode.UInt16: return (ushort)left != (ushort)right;
                            case TypeCode.Int32: return (int)left != (int)right;
                            case TypeCode.UInt32: return (uint)left != (uint)right;
                            case TypeCode.Int64: return (long)left != (long)right;
                            case TypeCode.UInt64: return (ulong)left != (ulong)right;
                            case TypeCode.Char: return (char)left != (char)right;
                            case TypeCode.Single: return (float)left != (float)right;
                            case TypeCode.Double: return (double)left != (double)right;
                            case TypeCode.Decimal: return (decimal)left != (decimal)right;
                            case TypeCode.Boolean: return (bool)left != (bool)right;
                        }
                        break;
                    }
                case ExpressionType.LessThan:
                    {
                        switch (typeCode)
                        {
                            case TypeCode.SByte: return (sbyte)left < (sbyte)right;
                            case TypeCode.Byte: return (byte)left < (byte)right;
                            case TypeCode.Int16: return (short)left < (short)right;
                            case TypeCode.UInt16: return (ushort)left < (ushort)right;
                            case TypeCode.Int32: return (int)left < (int)right;
                            case TypeCode.UInt32: return (uint)left < (uint)right;
                            case TypeCode.Int64: return (long)left < (long)right;
                            case TypeCode.UInt64: return (ulong)left < (ulong)right;
                            case TypeCode.Char: return (char)left < (char)right;
                            case TypeCode.Single: return (float)left < (float)right;
                            case TypeCode.Double: return (double)left < (double)right;
                            case TypeCode.Decimal: return (decimal)left < (decimal)right;
                        }
                        break;
                    }
                case ExpressionType.LessThanOrEqual:
                    {
                        switch (typeCode)
                        {
                            case TypeCode.SByte: return (sbyte)left <= (sbyte)right;
                            case TypeCode.Byte: return (byte)left <= (byte)right;
                            case TypeCode.Int16: return (short)left <= (short)right;
                            case TypeCode.UInt16: return (ushort)left <= (ushort)right;
                            case TypeCode.Int32: return (int)left <= (int)right;
                            case TypeCode.UInt32: return (uint)left <= (uint)right;
                            case TypeCode.Int64: return (long)left <= (long)right;
                            case TypeCode.UInt64: return (ulong)left <= (ulong)right;
                            case TypeCode.Char: return (char)left <= (char)right;
                            case TypeCode.Single: return (float)left <= (float)right;
                            case TypeCode.Double: return (double)left <= (double)right;
                            case TypeCode.Decimal: return (decimal)left <= (decimal)right;
                        }
                        break;
                    }
                case ExpressionType.GreaterThan:
                    {
                        switch (typeCode)
                        {
                            case TypeCode.SByte: return (sbyte)left > (sbyte)right;
                            case TypeCode.Byte: return (byte)left > (byte)right;
                            case TypeCode.Int16: return (short)left > (short)right;
                            case TypeCode.UInt16: return (ushort)left > (ushort)right;
                            case TypeCode.Int32: return (int)left > (int)right;
                            case TypeCode.UInt32: return (uint)left > (uint)right;
                            case TypeCode.Int64: return (long)left > (long)right;
                            case TypeCode.UInt64: return (ulong)left > (ulong)right;
                            case TypeCode.Char: return (char)left > (char)right;
                            case TypeCode.Single: return (float)left > (float)right;
                            case TypeCode.Double: return (double)left > (double)right;
                            case TypeCode.Decimal: return (decimal)left > (decimal)right;
                        }
                        break;
                    }
                case ExpressionType.GreaterThanOrEqual:
                    {
                        switch (typeCode)
                        {
                            case TypeCode.SByte: return (sbyte)left >= (sbyte)right;
                            case TypeCode.Byte: return (byte)left >= (byte)right;
                            case TypeCode.Int16: return (short)left >= (short)right;
                            case TypeCode.UInt16: return (ushort)left >= (ushort)right;
                            case TypeCode.Int32: return (int)left >= (int)right;
                            case TypeCode.UInt32: return (uint)left >= (uint)right;
                            case TypeCode.Int64: return (long)left >= (long)right;
                            case TypeCode.UInt64: return (ulong)left >= (ulong)right;
                            case TypeCode.Char: return (char)left >= (char)right;
                            case TypeCode.Single: return (float)left >= (float)right;
                            case TypeCode.Double: return (double)left >= (double)right;
                            case TypeCode.Decimal: return (decimal)left >= (decimal)right;
                        }
                        break;
                    }
                case ExpressionType.RightShift:
                    {
                        switch (typeCode)
                        {
                            case TypeCode.SByte: return (sbyte)left >> (int)right;
                            case TypeCode.Byte: return (byte)left >> (int)right;
                            case TypeCode.Int16: return (short)left >> (int)right;
                            case TypeCode.UInt16: return (ushort)left >> (int)right;
                            case TypeCode.Int32: return (int)left >> (int)right;
                            case TypeCode.UInt32: return (uint)left >> (int)right;
                            case TypeCode.Int64: return (long)left >> (int)right;
                            case TypeCode.UInt64: return (ulong)left >> (int)right;
                            case TypeCode.Char: return (char)left >> (int)right;
                        }
                        break;
                    }
                case ExpressionType.LeftShift:
                    {
                        switch (typeCode)
                        {
                            case TypeCode.SByte: return (sbyte)left << (int)right;
                            case TypeCode.Byte: return (byte)left << (int)right;
                            case TypeCode.Int16: return (short)left << (int)right;
                            case TypeCode.UInt16: return (ushort)left << (int)right;
                            case TypeCode.Int32: return (int)left << (int)right;
                            case TypeCode.UInt32: return (uint)left << (int)right;
                            case TypeCode.Int64: return (long)left << (int)right;
                            case TypeCode.UInt64: return (ulong)left << (int)right;
                            case TypeCode.Char: return (char)left << (int)right;
                        }
                        break;
                    }
            }

            throw new NotSupportedException("Expressions of type " + exprType + " failed.");
        }

        protected override Expression VisitBinary(BinaryExpression expr)
        {
            var left = ((ConstantExpression)Visit(expr.Left)).Value;
            var right = ((ConstantExpression)Visit(expr.Right)).Value;

            var unliftedLeft = Unlift(expr.Left.Type);
            var unliftedRight = Unlift(expr.Right.Type);
#if NETFX_CORE
            if (unliftedLeft.GetTypeInfo().IsEnum && unliftedLeft.Equals(unliftedRight))
#else
            if (unliftedLeft.IsEnum && unliftedLeft.Equals(unliftedRight))
#endif
            {
                var underlyingType = Enum.GetUnderlyingType(unliftedLeft);
                var nullableUnderlying = typeof(Nullable<>).MakeGenericType(underlyingType);
                return Visit(Expression.Convert(Expression.MakeBinary(expr.NodeType, Expression.Convert(expr.Left, nullableUnderlying), Expression.Convert(expr.Right, nullableUnderlying), expr.IsLiftedToNull, expr.Method, expr.Conversion), expr.Type));
            }
            object result = null;
#if NETFX_CORE
            if (!unliftedLeft.GetTypeInfo().IsPrimitive)
#else
            if (!unliftedLeft.IsPrimitive)
#endif
            {
                if (expr.NodeType == ExpressionType.AndAlso || expr.NodeType == ExpressionType.OrElse)
                {
                    var truthMethod = unliftedLeft.GetMethod(expr.NodeType == ExpressionType.AndAlso ? "op_False" : "op_True", new[] { unliftedLeft });
                    if (truthMethod != null && (bool)truthMethod.Invoke(null, new object[] { left }))
                    {
                        return Expression.Constant(left, expr.Type);
                    }
                    if (expr.IsLiftedToNull && right == null)
                    {
                        return Expression.Constant(null, expr.Type);
                    }
                    if (expr.Method != null)
                    {
                        return Expression.Constant(expr.Method.Invoke(null, new object[] { left, right }), expr.Type);
                    }
                }
            }
            if (expr.IsLiftedToNull && (expr.Left.Type.Equals(typeof(bool?))
                || expr.Left.Type.Equals(typeof(bool)))
                && (expr.Right.Type.Equals(typeof(bool?))
                || expr.Right.Type.Equals(typeof(bool))) && expr.Type.Equals(typeof(bool?))
                && (expr.NodeType == ExpressionType.And || expr.NodeType == ExpressionType.Or))
            {
                Func<bool?, bool?, bool?> evaluator = null;
                switch (expr.NodeType)
                {
                    case ExpressionType.And:
                        evaluator = (l, r) => l & r;
                        break;
                    case ExpressionType.Or:
                        evaluator = (l, r) => l | r;
                        break;
                }
                return Expression.Constant(evaluator.DynamicInvoke(left, right), expr.Type);
            }
            if (expr.IsLiftedToNull)
            {
                if ((expr.Left.Type.Equals(typeof(bool?)) || expr.Left.Type.Equals(typeof(bool))) &&
                  (expr.Right.Type.Equals(typeof(bool?)) || expr.Right.Type.Equals(typeof(bool))))
                {
                    if (expr.NodeType == ExpressionType.AndAlso && false.Equals(left))
                    {
                        return Expression.Constant(false, expr.Type);
                    }
                    if (expr.NodeType == ExpressionType.OrElse && true.Equals(left))
                    {
                        return Expression.Constant(true, expr.Type);
                    }
                }
                if (left == null || right == null)
                {
                    return Expression.Constant(null, expr.Type);
                }
            }
            if (expr.IsLifted)
            {
                switch (expr.NodeType)
                {
                    case ExpressionType.LessThan:
                    case ExpressionType.LessThanOrEqual:
                    case ExpressionType.GreaterThan:
                    case ExpressionType.GreaterThanOrEqual:
                        if (left == null || right == null)
                        {
                            return Expression.Constant(false, expr.Type);
                        }
                        break;
                    case ExpressionType.Equal:
                        if (left == null || right == null)
                        {
                            return Expression.Constant(left == right, expr.Type);
                        }
                        break;
                    case ExpressionType.NotEqual:
                        if (left == null || right == null)
                        {
                            return Expression.Constant(left != right, expr.Type);
                        }
                        break;
                }
            }
            if (expr.Method != null)
            {
                result = expr.Method.Invoke(null, new[] { left, right });
            }
            else if (expr.NodeType == ExpressionType.Coalesce || expr.NodeType == ExpressionType.ArrayIndex)
            {
                switch (expr.NodeType)
                {
                    case ExpressionType.Coalesce:
                        if (left != null)
                        {
                            result = left;
                            if (expr.Conversion != null)
                                result = Evaluate(expr.Conversion, values, result);
                        }
                        else
                        {
                            result = right;
                        }
                        break;
                    case ExpressionType.ArrayIndex:
#if NETFX_CORE
                        result = ((Array)left).GetValue((int)right);
#else
                        if (expr.Right.Type.Equals(typeof(int)))
                        {
                            result = ((Array)left).GetValue((int)right);
                        }
                        else
                        {
                            result = ((Array)left).GetValue((long)right);
                        }
#endif
                        break;
                }
            }
            else
            {
#if NETFX_CORE
                TypeCode typeCode = WinRTLegacy.TypeExtensions.GetTypeCode(unliftedLeft);
#else
                TypeCode typeCode = Type.GetTypeCode(unliftedLeft);
#endif
                result = Convert.ChangeType(this.BinaryOperate(expr.NodeType, typeCode, left, right), Unlift(expr.Type));
            }

            return Expression.Constant(result, expr.Type);
        }

        protected override Expression VisitMember(MemberExpression expr)
        {
            object root = null;
            if (expr.Expression != null)
            {
                root = ((ConstantExpression)Visit(expr.Expression)).Value;
                if (IsNullable(expr.Expression.Type))
                    return Expression.Constant(PerformOnNullable(root, expr.Member, new Expression[0]), expr.Type);
            }
            return Expression.Constant(expr.Member.Get(root));
        }

        private bool IsNullable(Type t)
        {
#if NETFX_CORE
            return t.GetTypeInfo().IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
#else
            return t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
#endif
        }

        private Type Unlift(Type t)
        {
            if (IsNullable(t))
                return t.GetGenericArguments()[0];
            return t;
        }

        protected override Expression VisitUnary(UnaryExpression expr)
        {
            if (expr.NodeType == ExpressionType.Quote)
                return Expression.Constant(new ParameterReplacer(values).Visit(expr.Operand), expr.Type);

            var val = ((ConstantExpression)Visit(expr.Operand)).Value;
            if (expr.IsLiftedToNull && val == null)
                return Expression.Constant(null, expr.Type);

            if (expr.Method != null)
            {
                var parameterType = expr.Method.GetParameters()[0].ParameterType;
#if NETFX_CORE
                if (val == null && parameterType.GetTypeInfo().IsValueType && !IsNullable(parameterType))
#else
                if (val == null && parameterType.IsValueType && !IsNullable(parameterType))
#endif
                    throw new InvalidOperationException("Cannot pass null into a conversion expecting a value type.");
                return Expression.Constant(expr.Method.Invoke(null, new[] { val }), expr.Type);
            }

            Type realSourceType;
            Type realTargetType;
            if (expr.IsLifted)
            {
                realSourceType = Unlift(expr.Operand.Type);
                realTargetType = Unlift(expr.Type);
            }
            else
            {
                realSourceType = expr.Operand.Type;
                realTargetType = expr.Type;
            }

#if NETFX_CORE
            TypeCode typeCode = WinRTLegacy.TypeExtensions.GetTypeCode(realSourceType);
#else
            TypeCode typeCode = Type.GetTypeCode(realSourceType);
#endif

            object result = null;
            switch (expr.NodeType)
            {
                case ExpressionType.TypeAs:
                    return Expression.Constant(expr.Type.IsInstanceOfType(val) ? val : null, expr.Type);
                case ExpressionType.Convert:
                    return Expression.Constant(Convert.ChangeType(val, realTargetType), expr.Type);
                case ExpressionType.ConvertChecked:
                    var convertMethod = typeof(Convert).GetMethod("To" + realTargetType.Name, new[] { val.GetType() });
                    return Expression.Constant(convertMethod.Invoke(null, new object[] { val }), expr.Type);
                case ExpressionType.ArrayLength:
                    return Expression.Constant(((Array)val).Length, expr.Type);
                case ExpressionType.Negate:
                    {
                        switch (typeCode)
                        {
                            case TypeCode.SByte:
                                result = -(sbyte)val;
                                break;
                            case TypeCode.Byte:
                                result = -(byte)val;
                                break;
                            case TypeCode.Int16:
                                result = -(short)val;
                                break;
                            case TypeCode.UInt16:
                                result = -(ushort)val;
                                break;
                            case TypeCode.Int32:
                                result = -(int)val;
                                break;
                            case TypeCode.UInt32:
                                result = -(uint)val;
                                break;
                            case TypeCode.Int64:
                                result = -(long)val;
                                break;
                            case TypeCode.Char:
                                result = -(char)val;
                                break;
                            case TypeCode.Single:
                                result = -(float)val;
                                break;
                            case TypeCode.Double:
                                result = -(double)val;
                                break;
                            case TypeCode.Decimal:
                                result = -(decimal)val;
                                break;
                            default:
                                break;
                        }
                        break;
                    }
                case ExpressionType.NegateChecked:
                    {
                        switch (typeCode)
                        {
                            case TypeCode.SByte:
                                result = checked(-(sbyte)val);
                                break;
                            case TypeCode.Byte:
                                result = checked(-(byte)val);
                                break;
                            case TypeCode.Int16:
                                result = checked(-(short)val);
                                break;
                            case TypeCode.UInt16:
                                result = checked(-(ushort)val);
                                break;
                            case TypeCode.Int32:
                                result = checked(-(int)val);
                                break;
                            case TypeCode.UInt32:
                                result = checked(-(uint)val);
                                break;
                            case TypeCode.Int64:
                                result = checked(-(long)val);
                                break;
                            case TypeCode.Char:
                                result = checked(-(char)val);
                                break;
                            case TypeCode.Single:
                                result = checked(-(float)val);
                                break;
                            case TypeCode.Double:
                                result = checked(-(double)val);
                                break;
                            case TypeCode.Decimal:
                                result = checked(-(decimal)val);
                                break;
                            default:
                                break;
                        }
                        break;
                    }
                case ExpressionType.UnaryPlus:
                    {
                        switch (typeCode)
                        {
                            case TypeCode.SByte:
                                result = +(sbyte)val;
                                break;
                            case TypeCode.Byte:
                                result = +(byte)val;
                                break;
                            case TypeCode.Int16:
                                result = +(short)val;
                                break;
                            case TypeCode.UInt16:
                                result = +(ushort)val;
                                break;
                            case TypeCode.Int32:
                                result = +(int)val;
                                break;
                            case TypeCode.UInt32:
                                result = +(uint)val;
                                break;
                            case TypeCode.Int64:
                                result = +(long)val;
                                break;
                            case TypeCode.Char:
                                result = +(char)val;
                                break;
                            case TypeCode.Single:
                                result = +(float)val;
                                break;
                            case TypeCode.Double:
                                result = +(double)val;
                                break;
                            case TypeCode.Decimal:
                                result = +(decimal)val;
                                break;
                            default:
                                break;
                        }
                        break;
                    }
                case ExpressionType.Not:
                    {
                        switch (typeCode)
                        {
                            case TypeCode.SByte:
                                result = ~(sbyte)val;
                                break;
                            case TypeCode.Byte:
                                result = ~(byte)val;
                                break;
                            case TypeCode.Int16:
                                result = ~(short)val;
                                break;
                            case TypeCode.UInt16:
                                result = ~(ushort)val;
                                break;
                            case TypeCode.Int32:
                                result = ~(int)val;
                                break;
                            case TypeCode.UInt32:
                                result = ~(uint)val;
                                break;
                            case TypeCode.Int64:
                                result = ~(long)val;
                                break;
                            case TypeCode.Char:
                                result = ~(char)val;
                                break;
                            case TypeCode.Boolean:
                                result = !(bool)val;
                                break;
                            default:
                                break;
                        }
                        break;
                    }
            }

            if (result != null)
                return Expression.Constant(result, expr.Type);

            throw new NotSupportedException("Bad unary operation: " + expr);
        }

        private object InvokeMethod(Func<object[], object> invoke, IEnumerable<Expression> arguments)
        {
            var args = arguments.Select(a => ((ConstantExpression)Visit(a)).Value).ToArray();
            var result = invoke(args);
            return result;
        }

        private object PerformOnNullable(object root, MemberInfo member, IEnumerable<Expression> arguments)
        {
            var args = arguments.Select(a => ((ConstantExpression)Visit(a)).Value).ToArray();
            if (member.Name.Equals("HasValue"))
                return root != null;

            if (member.Name.Equals("Value"))
            {
                if (root == null)
                    throw new InvalidOperationException("Nullable object must have a value.");
                return root;
            }

            if (member.Name.Equals("ToString"))
            {
                if (root == null)
                    return string.Empty;
                return root.ToString();
            }

            if (member.Name.Equals("Equals"))
                return object.Equals(root, args[0]);

            if (member.Name.Equals("GetHashCode"))
            {
                if (root == null)
                    return 0;

                return root.GetHashCode();
            }

            if (member.Name.Equals("GetValueOrDefault"))
            {
                if (root == null)
                    return args.FirstOrDefault();
                return root;
            }

            throw new NotSupportedException("Cannot call on Nullable");
        }

        protected override Expression VisitMethodCall(MethodCallExpression expr)
        {
            object root;
            if (expr.Method.IsStatic)
            {
                root = null;
            }
            else
            {
                root = ((ConstantExpression)Visit(expr.Object)).Value;
                if (IsNullable(expr.Object.Type))
                    return Expression.Constant(PerformOnNullable(root, expr.Method, expr.Arguments), expr.Type);
            }

            return Expression.Constant(InvokeMethod(args => expr.Method.Invoke(root, args), expr.Arguments), expr.Type.Equals(typeof(void)) ? typeof(object) : expr.Type);
        }

        protected override Expression VisitConditional(ConditionalExpression expr)
        {
            return ((bool)((ConstantExpression)Visit(expr.Test)).Value) ? Visit(expr.IfTrue) : Visit(expr.IfFalse);
        }

        protected override Expression VisitTypeBinary(TypeBinaryExpression expr)
        {
            return Expression.Constant(expr.TypeOperand.IsInstanceOfType(((ConstantExpression)Visit(expr.Expression)).Value), expr.Type);
        }

        protected override Expression VisitParameter(ParameterExpression expr)
        {
            return Expression.Constant(values[expr], expr.Type);
        }

        protected override Expression VisitLambda(LambdaExpression expr)
        {
            Func<object[], object> proxy = args => Evaluate(expr, values, args);
            return Expression.Constant(proxy, typeof(Func<object[], object>));
        }

        protected override Expression VisitInvocation(InvocationExpression expr)
        {
            var toInvoke = (Delegate)((ConstantExpression)Visit(expr.Expression)).Value;
            var result = InvokeMethod(args => toInvoke.DynamicInvoke(args), expr.Arguments);
            return Expression.Constant(result, expr.Type.Equals(typeof(void)) ? typeof(object) : expr.Type);
        }

        protected override Expression VisitNewArrayInit(NewArrayExpression expr)
        {
            var expressions = expr.Expressions;
            int count = expressions != null ? expressions.Count : 0;
            Array array = (Array)Activator.CreateInstance(expr.Type, count);
            for (int i = 0; i < count; i++)
            {
                var expression = this.Visit(expressions[i]);
                ConstantExpression constantExpression = expression as ConstantExpression;
                if (constantExpression != null)
                    array.SetValue(constantExpression.Value, i);
            }
            return Expression.Constant(array, expr.Type);
        }

        internal static object Evaluate(LambdaExpression expr, Scope scope, params object[] args)
        {
            var visitor = new EvaluatingVisitor();
            visitor.values = new Scope(scope);

            var first = expr.Parameters.GetEnumerator();
            var sencond = args != null ? args.GetEnumerator() : null;
            while (first.MoveNext())
            {
                if (sencond.MoveNext())
                {
                    visitor.values.Register(first.Current, sencond.Current);
                }
            }
            var result = visitor.Visit(expr.Body);
            return ((ConstantExpression)result).Value;
        }
    }

    public delegate object InvokeProxy(params object[] args);
}
