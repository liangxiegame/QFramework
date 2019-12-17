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

// using BindKit.Interactivity;
using BindKit.Observables;
using System;
using System.Collections;
using System.Reflection;
using BindKit.Binding.Paths;
using BindKit.Binding.Reflection;

namespace BindKit.Binding.Proxy.Sources.Object
{
    public class UniversalNodeProxyFactory : INodeProxyFactory
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(UniversalNodeProxyFactory));

        public ISourceProxy Create(object source, PathToken token)
        {
            IPathNode node = token.Current;
            if (source == null && !node.IsStatic)
                return null;

            if (node.IsStatic)
                return this.CreateStaticProxy(node);

            return CreateProxy(source, node);
        }

        protected virtual ISourceProxy CreateProxy(object source, IPathNode node)
        {
            IProxyType proxyType = source.GetType().AsProxy();
            if (node is IndexedNode)
            {
                var itemInfo = proxyType.GetItem();
                if (itemInfo == null)
                    return null;

                var intIndexedNode = node as IntegerIndexedNode;
                if (intIndexedNode != null)
                    return new IntItemNodeProxy((ICollection)source, intIndexedNode.Value, itemInfo);

                var stringIndexedNode = node as StringIndexedNode;
                if (stringIndexedNode != null)
                    return new StringItemNodeProxy((ICollection)source, stringIndexedNode.Value, itemInfo);

                return null;
            }

            var memberNode = node as MemberNode;
            if (memberNode == null)
                return null;

            var memberInfo = memberNode.MemberInfo;
            if (memberInfo == null)
                memberInfo = source.GetType().FindFirstMemberInfo(memberNode.Name);

            if (memberInfo == null || memberInfo.IsStatic())
                return null;

            var propertyInfo = memberInfo as PropertyInfo;
            if (propertyInfo != null)
            {
                IProxyPropertyInfo proxyPropertyInfo = propertyInfo.AsProxy();
                var valueType = proxyPropertyInfo.ValueType;
                if (typeof(IObservableProperty).IsAssignableFrom(valueType))
                {
                    object observableValue = proxyPropertyInfo.GetValue(source);
                    if (observableValue == null)
                        return null;

                    return new ObservableNodeProxy(source, (IObservableProperty)observableValue);
                }
                // else if (typeof(IInteractionRequest).IsAssignableFrom(valueType))
                // {
                    // object request = proxyPropertyInfo.GetValue(source);
                    // if (request == null)
                        // return null;

                    // return new InteractionNodeProxy(source, (IInteractionRequest)request);
                // }
                else
                {
                    return new PropertyNodeProxy(source, proxyPropertyInfo);
                }
            }

            var fieldInfo = memberInfo as FieldInfo;
            if (fieldInfo != null)
            {
                IProxyFieldInfo proxyFieldInfo = fieldInfo.AsProxy();
                var valueType = proxyFieldInfo.ValueType;
                if (typeof(IObservableProperty).IsAssignableFrom(valueType))
                {
                    object observableValue = proxyFieldInfo.GetValue(source);
                    if (observableValue == null)
                        return null;

                    return new ObservableNodeProxy(source, (IObservableProperty)observableValue);
                }
                // else if (typeof(IInteractionRequest).IsAssignableFrom(valueType))
                // {
                    // object request = proxyFieldInfo.GetValue(source);
                    // if (request == null)
                        // return null;

                    // return new InteractionNodeProxy(source, (IInteractionRequest)request);
                // }
                else
                {
                    return new FieldNodeProxy(source, proxyFieldInfo);
                }
            }

            var methodInfo = memberInfo as MethodInfo;
            if (methodInfo != null && methodInfo.ReturnType.Equals(typeof(void)))
                return new MethodNodeProxy(source, methodInfo.AsProxy());

            var eventInfo = memberInfo as EventInfo;
            if (eventInfo != null)
                return new EventNodeProxy(source, eventInfo.AsProxy());

            return null;
        }

        protected virtual ISourceProxy CreateStaticProxy(IPathNode node)
        {
            var memberNode = node as MemberNode;
            if (memberNode == null)
                return null;

            Type type = memberNode.Type;
            var memberInfo = memberNode.MemberInfo;
            if (memberInfo == null)
                memberInfo = type.FindFirstMemberInfo(memberNode.Name, BindingFlags.Public | BindingFlags.Static);

            if (memberInfo == null)
                return null;

            var propertyInfo = memberInfo as PropertyInfo;
            if (propertyInfo != null)
            {
                var proxyPropertyInfo = propertyInfo.AsProxy();
                var valueType = proxyPropertyInfo.ValueType;
                if (typeof(IObservableProperty).IsAssignableFrom(valueType))
                {
                    object observableValue = proxyPropertyInfo.GetValue(null);
                    if (observableValue == null)
                        throw new ArgumentNullException();

                    return new ObservableNodeProxy((IObservableProperty)observableValue);
                }
                // else if (typeof(IInteractionRequest).IsAssignableFrom(valueType))
                // {
                    // object request = proxyPropertyInfo.GetValue(null);
                    // if (request == null)
                        // throw new ArgumentNullException();

                    // return new InteractionNodeProxy((IInteractionRequest)request);
                // }
                else
                {
                    return new PropertyNodeProxy(proxyPropertyInfo);
                }
            }

            var fieldInfo = memberInfo as FieldInfo;
            if (fieldInfo != null)
            {
                var proxyFieldInfo = fieldInfo.AsProxy();
                var valueType = proxyFieldInfo.ValueType;
                if (typeof(IObservableProperty).IsAssignableFrom(valueType))
                {
                    object observableValue = proxyFieldInfo.GetValue(null);
                    if (observableValue == null)
                        throw new ArgumentNullException();

                    return new ObservableNodeProxy((IObservableProperty)observableValue);
                }
                // else if (typeof(IInteractionRequest).IsAssignableFrom(valueType))
                // {
                    // object request = proxyFieldInfo.GetValue(null);
                    // if (request == null)
                        // throw new ArgumentNullException();

                    // return new InteractionNodeProxy((IInteractionRequest)request);
                // }
                else
                {
                    return new FieldNodeProxy(proxyFieldInfo);
                }
            }

            var methodInfo = memberInfo as MethodInfo;
            if (methodInfo != null && methodInfo.ReturnType.Equals(typeof(void)))
                return new MethodNodeProxy(methodInfo.AsProxy());

            var eventInfo = memberInfo as EventInfo;
            if (eventInfo != null)
                return new EventNodeProxy(eventInfo.AsProxy());

            return null;
        }
    }
}
