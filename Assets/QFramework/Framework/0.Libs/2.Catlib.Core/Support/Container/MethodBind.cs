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

using System.Reflection;

namespace CatLib
{
    /// <summary>
    /// 方法绑定数据
    /// </summary>
    internal sealed class MethodBind : Bindable<IMethodBind> , IMethodBind
    {
        /// <summary>
        /// 方法信息
        /// </summary>
        public MethodInfo MethodInfo { get; private set; }

        /// <summary>
        /// 调用目标
        /// </summary>
        public object Target { get; private set; }

        /// <summary>
        /// 参数表
        /// </summary>
        public ParameterInfo[] ParameterInfos { get; private set; }

        /// <summary>
        /// 方法容器
        /// </summary>
        private readonly MethodContainer methodContainer;

        /// <summary>
        /// 构建一个绑定数据
        /// </summary>
        /// <param name="methodContainer">方法容器</param>
        /// <param name="container">依赖注入容器</param>
        /// <param name="service">服务名</param>
        /// <param name="target">调用目标</param>
        /// <param name="call">调用方法</param>
        public MethodBind(MethodContainer methodContainer, Container container, string service, object target, MethodInfo call)
            :base(container, service)
        {
            this.methodContainer = methodContainer;
            Target = target;
            MethodInfo = call;
            ParameterInfos = call.GetParameters();
        }

        /// <summary>
        /// 解除绑定
        /// </summary>
        protected override void ReleaseBind()
        {
            methodContainer.Unbind(this);
        }
    }
}
