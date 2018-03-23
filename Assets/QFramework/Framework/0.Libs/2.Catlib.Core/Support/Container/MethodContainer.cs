/*
 * This file is part of the CatLib package.
 *
 * (c) Yu Bin <support@catlib.io>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 *
 * Document: http://catlib.io/
 */

using System;
using System.Collections.Generic;
using System.Reflection;

namespace CatLib
{
    /// <summary>
    /// 方法容器
    /// </summary>
    internal sealed class MethodContainer
    {
        /// <summary>
        /// 调用方法目标 映射到 方法名字
        /// </summary>
        private readonly Dictionary<object, List<string>> targetToMethodsMappings;

        /// <summary>
        /// 绑定数据
        /// </summary>
        private readonly Dictionary<string, MethodBind> methodMappings;

        /// <summary>
        /// 依赖注入容器
        /// </summary>
        private readonly Container container;

        /// <summary>
        /// 依赖解决器
        /// </summary>
        private readonly Func<Bindable, ParameterInfo[], object[], object[]> dependenciesResolved;

        /// <summary>
        /// 同步锁
        /// </summary>
        private readonly object syncRoot;

        /// <summary>
        /// 构建一个新的方法容器
        /// </summary>
        /// <param name="container"></param>
        /// <param name="dependenciesResolved">依赖解决器</param>
        internal MethodContainer(Container container, Func<Bindable, ParameterInfo[], object[], object[]> dependenciesResolved)
        {
            this.container = container;
            targetToMethodsMappings = new Dictionary<object, List<string>>();
            methodMappings = new Dictionary<string, MethodBind>();
            this.dependenciesResolved = dependenciesResolved;
            syncRoot = new object();
        }

        /// <summary>
        /// 绑定一个方法
        /// </summary>
        /// <param name="method">通过这个名字可以调用方法</param>
        /// <param name="target">方法调用目标</param>
        /// <param name="methodInfo">在方法调用目标中被调用的方法</param>
        /// <returns></returns>
        public IMethodBind Bind(string method, object target, MethodInfo methodInfo)
        {
            Guard.NotEmptyOrNull(method, "method");
            Guard.Requires<ArgumentNullException>(methodInfo != null);

            if (!methodInfo.IsStatic)
            {
                Guard.Requires<ArgumentNullException>(target != null);
            }

            lock (syncRoot)
            {
                if (methodMappings.ContainsKey(method))
                {
                    throw new RuntimeException("Method [" + method + "] is already bind");
                }

                var methodBind = new MethodBind(this, container, method, target, methodInfo);
                methodMappings[method] = methodBind;

                if (target == null)
                {
                    return methodBind;
                }

                List<string> targetMappings;
                if (!targetToMethodsMappings.TryGetValue(target, out targetMappings))
                {
                    targetToMethodsMappings[target] = targetMappings = new List<string>();
                }

                targetMappings.Add(method);
                return methodBind;
            }
        }

        /// <summary>
        /// 调用方法
        /// </summary>
        /// <param name="method">方法名</param>
        /// <param name="userParams">用户传入的参数</param>
        /// <returns>方法调用结果</returns>
        public object Invoke(string method, params object[] userParams)
        {
            Guard.NotEmptyOrNull(method, "method");

            lock (syncRoot)
            {
                MethodBind methodBind;
                if (!methodMappings.TryGetValue(method, out methodBind))
                {
                    throw MakeMethodNotFoundException(method);
                }

                var injectParams = dependenciesResolved(methodBind, methodBind.ParameterInfos, userParams) ??
                                   new object[] { };
                return methodBind.MethodInfo.Invoke(methodBind.Target, injectParams);
            }
        }

        /// <summary>
        /// 解除绑定
        /// </summary>
        /// <param name="target">
        /// 解除目标
        /// <para>如果为字符串则作为调用方法名</para>
        /// <para>如果为<code>IMethodBind</code>则作为指定方法</para>
        /// <para>如果为其他对象则作为调用目标做全体解除</para>
        /// </param>
        public void Unbind(object target)
        {
            Guard.Requires<ArgumentNullException>(target != null);

            lock (syncRoot)
            {
                var methodBind = target as MethodBind;

                if (methodBind != null)
                {
                    methodBind.Unbind();
                    return;
                }

                if (target is string)
                {
                    if (!methodMappings.TryGetValue(target.ToString(), out methodBind))
                    {
                        return;
                    }
                    methodBind.Unbind();
                    return;
                }

                UnbindWithObject(target);
            }
        }

        /// <summary>
        /// 解除绑定
        /// </summary>
        /// <param name="methodBind">方法绑定</param>
        internal void Unbind(MethodBind methodBind)
        {
            lock (syncRoot)
            {
                methodMappings.Remove(methodBind.Service);

                if (methodBind.Target == null)
                {
                    return;
                }

                List<string> methods;
                if (!targetToMethodsMappings.TryGetValue(methodBind.Target, out methods))
                {
                    return;
                }

                methods.Remove(methodBind.Service);

                if (methods.Count <= 0)
                {
                    targetToMethodsMappings.Remove(methodBind.Target);
                }
            }
        }

        /// <summary>
        /// 根据对象绑定移除为该对象绑定的所有方法
        /// </summary>
        /// <param name="target">对象信息</param>
        private void UnbindWithObject(object target)
        {
            List<string> methods;
            if (!targetToMethodsMappings.TryGetValue(target, out methods))
            {
                return;
            }

            foreach (var method in methods.ToArray())
            {
                Unbind(method);
            }
        }

        /// <summary>
        /// 清空容器的所有实例，绑定，别名，标签，解决器
        /// </summary>
        public void Flush()
        {
            lock (syncRoot)
            {
                targetToMethodsMappings.Clear();
                methodMappings.Clear();
            }
        }

        /// <summary>
        /// 生成一个方法没有找到异常
        /// </summary>
        /// <param name="method"></param>
        private RuntimeException MakeMethodNotFoundException(string method)
        {
            return new RuntimeException("Method [" + method + "] is not found.");
        }
    }
}
