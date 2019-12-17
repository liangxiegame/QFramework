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
using System.Reflection;
using BindKit.Binding.Reflection;

namespace BindKit.Binding.Proxy.Targets
{
    public class UniversalTargetProxyFactory : ITargetProxyFactory
    {
        public ITargetProxy CreateProxy(object target, BindingDescription description)
        {
            IProxyType type = target.GetType().AsProxy();
            IProxyMemberInfo memberInfo = type.GetMember(description.TargetName);
            if (memberInfo == null)
                memberInfo = type.GetMember(description.TargetName, BindingFlags.Instance | BindingFlags.NonPublic);

            if (memberInfo == null)
                return null;

            var propertyInfo = memberInfo as IProxyPropertyInfo;
            if (propertyInfo != null)
            {
                var valueType = propertyInfo.ValueType;
                if (typeof(IObservableProperty).IsAssignableFrom(valueType))
                {
                    object observableValue = propertyInfo.GetValue(target);
                    if (observableValue == null)
                        return null;

                    return new ObservableTargetProxy(target, (IObservableProperty)observableValue);
                }

                // if (typeof(IInteractionAction).IsAssignableFrom(valueType))
                // {
                //     object interactionAction = propertyInfo.GetValue(target);
                //     if (interactionAction == null)
                //         return null;
                //
                //     return new InteractionTargetProxy(target, (IInteractionAction)interactionAction);
                // }

                return new PropertyTargetProxy(target, propertyInfo);
            }

            var fieldInfo = memberInfo as IProxyFieldInfo;
            if (fieldInfo != null)
            {
                var valueType = fieldInfo.ValueType;
                if (typeof(IObservableProperty).IsAssignableFrom(valueType))
                {
                    object observableValue = fieldInfo.GetValue(target);
                    if (observableValue == null)
                        return null;

                    return new ObservableTargetProxy(target, (IObservableProperty)observableValue);
                }

                // if (typeof(IInteractionAction).IsAssignableFrom(valueType))
                // {
                //     object interactionAction = fieldInfo.GetValue(target);
                //     if (interactionAction == null)
                //         return null;
                //
                //     return new InteractionTargetProxy(target, (IInteractionAction)interactionAction);
                // }

                return new FieldTargetProxy(target, fieldInfo);
            }

            var eventInfo = memberInfo as IProxyEventInfo;
            if (eventInfo != null)
                return new EventTargetProxy(target, eventInfo);

            var methodInfo = memberInfo as IProxyMethodInfo;
            if (methodInfo != null)
                return new MethodTargetProxy(target, methodInfo);

            return null;
        }
    }
}
