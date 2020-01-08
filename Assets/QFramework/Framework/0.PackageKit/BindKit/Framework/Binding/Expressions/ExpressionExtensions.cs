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
using BindKit.Binding.Reflection;

namespace BindKit.Binding.Expressions
{
    public static class ExpressionExtensions
    {
        public static Func<object[], object> DynamicCompile(this LambdaExpression expr)
        {
            return (Func<object[], object>)((ConstantExpression)new EvaluatingVisitor().Visit(expr)).Value;
        }

        public static Func<object[], object> DynamicCompile<T>(this Expression<T> expr)
        {
            return DynamicCompile((LambdaExpression)expr);
        }

        internal static object Get(this MemberInfo info, object root)
        {
            var fieldInfo = info as FieldInfo;
            if (fieldInfo != null)
            {
                var proxyFieldInfo = fieldInfo.AsProxy();
                if (proxyFieldInfo != null)
                    return proxyFieldInfo.GetValue(root);

                return fieldInfo.GetValue(root);
            }

            var propertyInfo = info as PropertyInfo;
            if (propertyInfo != null)
            {
                var proxyPropertyInfo = propertyInfo.AsProxy();
                if (proxyPropertyInfo != null)
                    return proxyPropertyInfo.GetValue(root);

                var method = propertyInfo.GetGetMethod();
                if (method != null)
                    return method.Invoke(root, null);
            }

            throw new NotSupportedException("Bad MemberInfo type.");
        }

        internal static void Set(this MemberInfo info, object root, object value)
        {
            var fieldInfo = info as FieldInfo;
            if (fieldInfo != null)
            {
                var proxyFieldInfo = fieldInfo.AsProxy();
                if (proxyFieldInfo != null)
                {
                    proxyFieldInfo.SetValue(root, value);
                }
                else
                {
                    fieldInfo.SetValue(root, value);
                }
                return;
            }
            var propertyInfo = info as PropertyInfo;
            if (propertyInfo != null)
            {
                var proxyPropertyInfo = propertyInfo.AsProxy();
                if (proxyPropertyInfo != null)
                {
                    proxyPropertyInfo.SetValue(root, value);
                }
                else
                {
                    var method = propertyInfo.GetSetMethod();
                    if (method != null)
                        method.Invoke(root, new object[] { value });
                }
                return;
            }
            throw new NotSupportedException("Bad MemberInfo type.");
        }

        internal static MethodInfo GetMethod(this Type type, string name, int genericParamLength)
        {
            foreach (MethodInfo info in type.GetMethods())
            {
                if (!info.Name.Equals(name))
                    continue;

                Type[] argumentTypes = info.GetGenericArguments();
                if (argumentTypes.Length != genericParamLength)
                    continue;
                return info;
            }
            return null;
        }
    }
}
